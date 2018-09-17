using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AutoMapper;
using BjjInParadise.Business;
using BjjInParadise.Core.Models;
using BJJInParadise.Web.Helpers;
using BJJInParadise.Web.ViewModels;
using Braintree;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace BJJInParadise.Web.Controllers
{
    public class BraintreeConfiguration : IBraintreeConfiguration
    {
        public string Environment { get; set; }
        public string MerchantId { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        private IBraintreeGateway BraintreeGateway { get; set; }

        public IBraintreeGateway CreateGateway()
        {
            Environment = System.Environment.GetEnvironmentVariable("BraintreeEnvironment");
            MerchantId = System.Environment.GetEnvironmentVariable("BraintreeMerchantId");
            PublicKey = System.Environment.GetEnvironmentVariable("BraintreePublicKey");
            PrivateKey = System.Environment.GetEnvironmentVariable("BraintreePrivateKey");

            if (MerchantId == null || PublicKey == null || PrivateKey == null)
            {
                Environment = GetConfigurationSetting("BraintreeEnvironment");
                MerchantId = GetConfigurationSetting("BraintreeMerchantId");
                PublicKey = GetConfigurationSetting("BraintreePublicKey");
                PrivateKey = GetConfigurationSetting("BraintreePrivateKey");
            }

            return new BraintreeGateway(Environment, MerchantId, PublicKey, PrivateKey);
        }

        public string GetConfigurationSetting(string setting)
        {
            return ConfigurationManager.AppSettings[setting];
        }

        public IBraintreeGateway GetGateway()
        {
            if (BraintreeGateway == null)
            {
                BraintreeGateway = CreateGateway();
            }

            return BraintreeGateway;
        }
    }
    public interface IBraintreeConfiguration
    {
        IBraintreeGateway CreateGateway();
        string GetConfigurationSetting(string setting);
        IBraintreeGateway GetGateway();
    }

    [Authorize]
    public class BookingController : BaseController
    {
        private AccountService _service;
        private CampService _campService;
        private ApplicationUserManager _userManager;
        private CampRoomOptionService _roomOptionService;
        private BookingService _bookingService;
        private IBraintreeConfiguration config = new BraintreeConfiguration();
        public static readonly TransactionStatus[] transactionSuccessStatuses = {
            TransactionStatus.AUTHORIZED,
            TransactionStatus.AUTHORIZING,
            TransactionStatus.SETTLED,
            TransactionStatus.SETTLING,
            TransactionStatus.SETTLEMENT_CONFIRMED,
            TransactionStatus.SETTLEMENT_PENDING,
            TransactionStatus.SUBMITTED_FOR_SETTLEMENT
        };

        public BookingController(AccountService service, CampService campService, ApplicationUserManager userManager, CampRoomOptionService roomOptionService, BookingService bookingService)
        {
            _service = service;
            _campService = campService;
            UserManager = userManager;
            _roomOptionService = roomOptionService;
            _bookingService = bookingService;
        }
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        private async Task<List<SelectListItem>> GetAvailableCampsDropDown()
        {
            var availCamps = await _campService.GetAllActiveAsync();

            var list = availCamps.Select(x => new SelectListItem
            {
                Text = $@"{x.StartDate.ToShortDateString()} - {x.EndDate.ToShortDateString()}: {x.CampName}  ",
                Value = x.CampId.ToString()
            }).AddEmpty();
            return list;

        }
        public async Task<ActionResult> Index(int? id)
        {
            var user = await _service.Get(User.Identity.GetUserId());
            //var nextCamp = await _campService.GetNextCampAsync();
            var userOwin = await UserManager.FindByIdAsync(user.AspNetUserId);
            

       

            var cro =await GetCampRoomOptions(id.Value);
            var list2 = cro.Select(x => new SelectListItem
                {Value = x.CampId.ToString(), Text = x.RoomType + " " + x.CostPerPerson.ToString("C0")}).ToList();
            var gateway = config.GetGateway();
            var clientToken = gateway.ClientToken.generate();

            var vm = new NewBookingViewModel
            {
                UserId = user.UserId,
                CampId = id.Value,
                Email = userOwin.Email,
               RoomOptions = list2,

                ClientToken = clientToken
            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {


            var gateway = config.GetGateway();
            decimal amount;

            try
            {
                amount =GetRoomOptionAmount(int.Parse( fc["CampRoomOptionId"])) ;
            }
            catch (FormatException e)
            {
                TempData["Flash"] = "Error: 81503: Amount is an invalid format.";
                return RedirectToAction("Index");
            }

            var nonce = Request["payment_method_nonce"];
            var request = new TransactionRequest
            {
                Amount = amount,
                PaymentMethodNonce = nonce,
                
                Options = new TransactionOptionsRequest
                {
                    SubmitForSettlement = true
                }
            };

            var result = gateway.Transaction.Sale(request);
            if (result.IsSuccess())
            {
                var transaction = result.Target;
                _bookingService.AddNew(new Booking
                {
                    CampId = int.Parse(fc["CampId"]), AmountPaid = request.Amount, BookingDate = DateTime.UtcNow,
                    CampRoomOptionId = int.Parse(fc["CampRoomOptionId"])
                });
                return RedirectToAction("Show", new { id = transaction.Id });
            }
            else if (result.Transaction != null)
            {
                return RedirectToAction("Show", new { id = result.Transaction.Id });
            }
            else
            {
                string errorMessages = "";
                foreach (ValidationError error in result.Errors.DeepAll())
                {
                    errorMessages += "Error: " + (int)error.Code + " - " + error.Message + "\n";
                }
                TempData["Flash"] = errorMessages;
                return RedirectToAction("Index");
            }


        }

        private decimal GetRoomOptionAmount(int parse)
        {
            var cro = _roomOptionService.Get(parse);
            return cro.CostPerPerson;
        }


        [HttpPost]
        public ActionResult GetCampOptions(int campId)
        {
            var result = GetCampRoomOptions(campId);
            return Json(new { success = true, data=result },
                JsonRequestBehavior.AllowGet);
        }

 
        private async Task<List<CampRoomOption>> GetCampRoomOptions(int campId)
        {
            var result = (await _roomOptionService.GetActiveOptionsByCampIdAsync(campId)).ToList();
            foreach (var campRoomOption in result)
            {
                campRoomOption.Camp = null;
            }

            return result;
        }

        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Show(string id)
        {
            var gateway = config.GetGateway();
            Transaction transaction = gateway.Transaction.Find(id);

            if (transactionSuccessStatuses.Contains(transaction.Status))
            {
                TempData["header"] = "Sweet Success!";
                TempData["icon"] = "success";
                TempData["message"] = "Your test transaction has been successfully processed.";

                return View("Success", transaction);
            }
            else
            {
                TempData["header"] = "Transaction Failed";
                TempData["icon"] = "fail";
                TempData["message"] = "Your test transaction has a status of " + transaction.Status + ".";
            };

            ViewBag.Transaction = transaction;
            return View();
        }
    }
}