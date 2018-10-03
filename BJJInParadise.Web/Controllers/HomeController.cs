using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BjjInParadise.Business;
using BjjInParadise.Core.Constants;

namespace BJJInParadise.Web.Controllers
{
 
    public class HomeController : BaseController
    {
        private CampService _service;

        public HomeController(CampService service)
        {
            _service = service;
        }
        public async Task<ActionResult >Index()
        {
            var nextCamp = await _service.GetNextCampAsync();
            return View(nextCamp);
        }


        public ActionResult ComingSoon()
        {
            return new FilePathResult("~/Views/Home/ComingSoon/index.html", "text/html");
        }

        public ActionResult SendMessage(EmailMessage message)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MailMessage message1 = new MailMessage {From = new MailAddress(EMAIL_FROM_ADDRESS)};

                    message1.To.Add(new MailAddress(EMAIL_TO_ADDRESS));
                    message1.Bcc.Add(new MailAddress("stevenfackley@gmail.com"));

                    message1.Subject = "BJJ In Paradise Website Info Request";
                    message1.Body = "From: " + message.Name + " " + message.Email + " " + message.Phone + "\n" + message.Message;

                    var client = new SmtpClient();
                    client.Send(message1);

                    return Json(new {success = true, data = "Mail sent"},
                        JsonRequestBehavior.AllowGet);
                }
                catch (Exception e)
                {

                    return Json(new {success = false, data = new {message = "Failure", exception = e.Message}},
                        JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new { success = false, data = new { message = "Validation Failure", exception = string.Join(" | ", ModelState.Values
                            .SelectMany(v => v.Errors)
                            .Select(e => e.ErrorMessage))} },
                    JsonRequestBehavior.AllowGet);
            }

        }

        private const string EMAIL_FROM_ADDRESS = "bradwolfson@bjjinparadise.com";
        private const string EMAIL_TO_ADDRESS = "Soulcraftjiujitsu@gmail.com";
    }

    public class EmailMessage
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        [Required]
        public string Message { get; set; }

    }
}