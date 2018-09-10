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
using PayPal.Api;

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
        public async Task<ActionResult> Index()
        {
            var user = await _service.Get(User.Identity.GetUserId());
            //var nextCamp = await _campService.GetNextCampAsync();
            var userOwin = await UserManager.FindByIdAsync(user.AspNetUserId);
            

       
            var list2 = new List<SelectListItem> ().AddEmpty();

            var gateway = config.GetGateway();
            var clientToken = gateway.ClientToken.generate();

            var vm = new NewBookingViewModel
            {
                UserId = user.UserId,
                CampId = 0,
                Email = userOwin.Email,
                AllAvailableCamps = await GetAvailableCampsDropDown(),
                RoomOptions = list2,
                Countries = CreateCountryDropDown(),
                Country = user.Country,
                Street = user.Street,
                State = user.State,
                ZipCode = user.ZipCode,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                HomeGym = user.HomeGym,
                City = user.City,
                ClientToken = clientToken
#if DEBUG

                //Test
                ,
                CreditCard = "4153725853121993"
                ,
                Expiration =new DateTime( 2023, 3, 1)
                ,CVC = "330"
#endif
            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index(FormCollection fc)
        {
            var gateway = config.GetGateway();
            Decimal amount;

            try
            {
                amount = Convert.ToDecimal(Request["amount"]);
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
                return RedirectToAction("Success", new { id = transaction.Id });
            }
            else if (result.Transaction != null)
            {
                return RedirectToAction("Index", new { id = result.Transaction.Id });
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

        //[HttpPost]
        //public async Task<ActionResult> Index(NewBookingViewModel vm)
        //{
           

        //    if (ModelState.Values.Count(x => x.Errors.Any()) <= 1)
        //    {
        //        var result = Mapper.Map<NewBookingViewModel, Booking>(vm);
        //        var t = _bookingService.ProcessPayment(result);
        //        if (t.failed_transactions == null || !t.failed_transactions.Any())
        //        {
        //            await _bookingService.AddAsync(result);

        //        }

        //        return View("Confirmation", t);
        //    }
        //    else
        //    {
        //        foreach (var item in ModelState)
        //        {
        //            if (item.Value.Errors.Any())
        //            {

        //            }
        //        }
        //        vm.AllAvailableCamps = await GetAvailableCampsDropDown();
        //        vm.Countries = CreateCountryDropDown();
        //        vm.RoomOptions = (await GetCampRoomOptions(vm.CampId)).Select(x => new SelectListItem
        //        {
        //            Text = x.RoomType + x.CostPerPerson.ToString("C2"),
        //            Value = x.CampRoomOptionId.ToString()
        //        }).ToList();
        //        return View(vm);
        //    }

         
        //}

        public ActionResult Success()
        {
            return View();
        }

    }
}