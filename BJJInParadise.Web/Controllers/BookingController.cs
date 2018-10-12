using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using AutoMapper;
using BjjInParadise.Business;
using BjjInParadise.Core.Models;
using BJJInParadise.Web.Helpers;
using BJJInParadise.Web.ViewModels;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;


namespace BJJInParadise.Web.Controllers
{
 

    [Authorize]
    public class BookingController : BaseController
    {
        private AccountService _service;
        private CampService _campService;
        private ApplicationUserManager _userManager;
        private CampRoomOptionService _roomOptionService;
        private BookingService _bookingService;
   

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
            var user = await _service.GetAsync(User.Identity.GetUserId());
            var userOwin = await UserManager.FindByIdAsync(user.AspNetUserId);

            var camp = _campService.Get(id);
            var cro =await GetCampRoomOptions(id.Value);
          
            bool isLive = !HttpContext.IsDebuggingEnabled;

            var vm = new NewBookingViewModel()
            {
                UserId = user.UserId,
                CampId = id.Value,
                Email = userOwin.Email,
                IsLive = isLive,
               RoomOptions = cro,
                CampRoomOptionId = cro.First().CampRoomOptionId,
               CampName =  $@" {camp.CampName}: {camp.StartDate.ToShortDateString()} - {camp.EndDate.ToShortDateString()} ",
                ClientToken = "123"//clientToken
            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index(NewBookingViewModel fc)
        {



              var user =  _service.Get(User.Identity.GetUserId());

                var booking = new Booking
                {
                    CampId = fc.CampId,
                    BookingDate = DateTime.UtcNow,
                    UserId = user.UserId,
                    AmountPaid = fc.AmountPaid,
                   BrainTreeTransactionId = fc.PayPalTransactionId,
                    CampRoomOptionId = fc.CampRoomOptionId
                };
                _bookingService.AddNew(booking);

            SendConfirmationEmail(user,  booking);

            return Json(new { success = true, data = booking },
                JsonRequestBehavior.AllowGet);


        }

        private void SendConfirmationEmail(User user,  Booking booking)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
     
            if (booking == null) throw new ArgumentNullException(nameof(booking));

            var userOwin = UserManager.FindById(user.AspNetUserId);
            var message = CreateConfirmationMessage( booking);
            SendMessage(user.FirstName + " " + user.LastName, userOwin.Email, userOwin.PhoneNumber, message);
        }

        private string CreateConfirmationMessage(Booking booking)
        {
            var camp = _campService.Get(booking.CampId);
            var template = string.Format(@"<h1>BJJ In Paradise {4}</h1><h2>Thank you for your order! </h2><dl>
        <dt>
        Amount
        </dt>
        <dd>
        {0}
        </dd>
        <dt>
        Transaction Date
        </dt>
        <dd>
        {1}
        </dd>
        <dt>
        Status
        </dt>
        <dd>
        {2}
        </dd>
        <dt>
        Confirmation #
        </dt>
        <dd>
        {3}
        </dd>

        </dl>", booking.AmountPaid.Value.ToString("c2"), booking.CreatedDate, "Submitted To PayPpal", booking.BrainTreeTransactionId, camp.CampName);


            return template;
        }

        private const string EMAIL_FROM_ADDRESS = "bradwolfson@bjjinparadise.com";
        public ActionResult SendMessage(string name, string email, string phone, string message)
        {
            try
            {
                var message1 = new MailMessage { From = new MailAddress(EMAIL_FROM_ADDRESS) };

                message1.To.Add(new MailAddress(email));
                message1.Bcc.Add(new MailAddress(EMAIL_FROM_ADDRESS));
                message1.Subject = "BJJ In Paradise Order Confirmation";
                message1.Body = "From: " + name + " " + email + " " + phone + "\n" + message;
                message1.IsBodyHtml = true;
                var client = new SmtpClient();
                client.Send(message1);

                return Json(new { success = true, data = "Mail sent" },
                    JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {

                return Json(new { success = false, data = new { message = "Failure", exception = e.Message } },
                    JsonRequestBehavior.AllowGet);
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
        [HttpGet]
        public ActionResult Success(int id)
        {
            var b = _bookingService.Get(id);
            return View(b);
        }

    }
}