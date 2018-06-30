using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BjjInParadise.Business;
using BjjInParadise.Core.Models;
using BJJInParadise.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PayPal.Api;

namespace BJJInParadise.Web.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private AccountService _service;
        private CampService _campService;
        private ApplicationUserManager _userManager;
        private CampRoomOptionService _roomOptionService;

        public BookingController(AccountService service, CampService campService, ApplicationUserManager userManager, CampRoomOptionService roomOptionService)
        {
            _service = service;
            _campService = campService;
            UserManager = userManager;
            _roomOptionService = roomOptionService;
        }
        public ApplicationUserManager UserManager
        {
            get => _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            private set => _userManager = value;
        }

        public async Task<ActionResult> Index()
        {
            var user = await _service.Get(User.Identity.GetUserId());
            var nextCamp = await _campService.GetNextCampAsync();
            var availCamps = await _campService.GetAllActiveAsync();
            var userOwin = await UserManager.FindByIdAsync(user.AspNetUserId);
            

            var list = new List<SelectListItem> {new SelectListItem {Text = "-- Select --", Value = "0"}};
            list.AddRange(availCamps.Select(x => new SelectListItem
            {
                Text = $@"{x.StartDate.ToShortDateString()} - {x.CampName} From ",
                Value = x.CampId.ToString()
            }));

            var list2 = new List<SelectListItem> { new SelectListItem { Text = "-- Select --", Value = "0" } };
     

            var vm = new NewBookingViewModel
            {
                UserId = user.UserId,
                CampId = 0,
                User = user,
                Email = userOwin.Email,
                AllAvailableCamps = list,
                RoomOptions = list2
            };

            return View(vm);
        }
        [HttpPost]
        public ActionResult GetCampOptions(int campId)
        {
            var result = _roomOptionService.GetByCampId(campId).ToList();
            foreach (var campRoomOption in result)
            {
                campRoomOption.Camp = null;
            }
            return Json(new { success = true, data=result },
                JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public async Task<ActionResult> Index(NewBookingViewModel vm)
        {
            // Authenticate with PayPal
            var config = ConfigManager.Instance.GetProperties();
            var accessToken = new OAuthTokenCredential(config).GetAccessToken();
            var apiContext = new APIContext(accessToken);

            // Make an API call
            var payment = Payment.Create(apiContext, new Payment
            {
                intent = "sale",
                payer = new Payer
                {
                    payment_method = "paypal"
                },
                transactions = new List<Transaction>
                {
                    new Transaction
                    {
                        description = "Transaction description.",
                        invoice_number = "001",
                        amount = new Amount
                        {
                            currency = "USD",
                            total = "100.00",
                            details = new Details
                            {
                                tax = "15",
                                shipping = "10",
                                subtotal = "75"
                            }
                        },
                        item_list = new ItemList
                        {
                            items = new List<Item>
                            {
                                new Item
                                {
                                    name = "Item Name",
                                    currency = "USD",
                                    price = "15",
                                    quantity = "5",
                                    sku = "sku"
                                }
                            }
                        }
                    }
                },
                redirect_urls = new RedirectUrls
                {
                    return_url = "http://mysite.com/return",
                    cancel_url = "http://mysite.com/cancel"
                }
            });
            return View(vm);
        }
    }
}