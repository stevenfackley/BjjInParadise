using System;
using System.Collections.Generic;
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
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PayPal.Api;

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
        public async Task<ActionResult> Index()
        {
            var user = await _service.Get(User.Identity.GetUserId());
            //var nextCamp = await _campService.GetNextCampAsync();
            var userOwin = await UserManager.FindByIdAsync(user.AspNetUserId);
            

       
            var list2 = new List<SelectListItem> ().AddEmpty();
     
           
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
                City = user.City
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

        [HttpPost]
        public async Task<ActionResult> Index(NewBookingViewModel vm)
        {
           

            if (ModelState.Values.Count(x => x.Errors.Any()) <= 1)
            {
                var result = Mapper.Map<NewBookingViewModel, Booking>(vm);
                var t = _bookingService.ProcessPayment(result);
                if (t.failed_transactions == null || !t.failed_transactions.Any())
                {
                    await _bookingService.AddAsync(result);

                }

                return View("Confirmation", t);
            }
            else
            {
                foreach (var item in ModelState)
                {
                    if (item.Value.Errors.Any())
                    {

                    }
                }
                vm.AllAvailableCamps = await GetAvailableCampsDropDown();
                vm.Countries = CreateCountryDropDown();
                vm.RoomOptions = (await GetCampRoomOptions(vm.CampId)).Select(x => new SelectListItem
                {
                    Text = x.RoomType + x.CostPerPerson.ToString("C2"),
                    Value = x.CampRoomOptionId.ToString()
                }).ToList();
                return View(vm);
            }

         
        }

    }
}