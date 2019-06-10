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

        
        public async Task<ActionResult> Index(int id)
        {

            var user =User.Identity.IsAuthenticated? await _service.GetAsync(User.Identity.GetUserId()) : null;
            var userOwin = user != null ? await UserManager.FindByIdAsync(user.AspNetUserId) : null;

            var camp = _campService.Get(id);
            var cro =await GetCampRoomOptions(id);
          
            bool isLive = !HttpContext.IsDebuggingEnabled;

            var vm = new NewBookingViewModel
            {
                UserId = user?.UserId,
                CampId = id,
                Email = userOwin?.Email,
                IsLive = isLive,
               RoomOptions = cro,
                CampRoomOptionId = cro.First().CampRoomOptionId,
               CampName =  $@" {camp.CampName}: {camp.StartDate.ToShortDateString()} - {camp.EndDate.ToShortDateString()} ",
                ClientToken = "123",//clientToken
                FirstName =  user?.FirstName,
                LastName = user?.LastName
            };
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index(NewBookingViewModel fc)
        {
            
              var user =  User.Identity.IsAuthenticated ? _service.Get(User.Identity.GetUserId()) : null;
              if (user == null && (fc.FirstName == null || fc.LastName == null || fc.Email == null))
              {
                  throw new Exception("First name, last name, or email is null");
              }
              var booking = new Booking
              {
                  CampId = fc.CampId,
                  BookingDate = DateTime.UtcNow,
                  UserId = user?.UserId,
                  AmountPaid = fc.AmountPaid,
                  BrainTreeTransactionId = fc.PayPalTransactionId,
                  CampRoomOptionId = fc.CampRoomOptionId,
                  FirstName = fc.FirstName,
                  LastName = fc.LastName,
                  EmailAddress = fc.Email
              };
                _bookingService.AddNew(booking);

                //Only try to send an email if it is a live env
                if (!HttpContext.IsDebuggingEnabled)
                {
                    if (user == null)
                    {
                        SendConfirmationEmail(booking);
                    }
                    else
                    {
                        SendConfirmationEmail(user, booking);

                    }
                }

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
        private void SendConfirmationEmail(Booking booking)
        {
            if (booking == null) throw new ArgumentNullException(nameof(booking));

            var message = CreateConfirmationMessage(booking);
            SendMessage(booking.FirstName + " " + booking.LastName, booking.EmailAddress, null, message);
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