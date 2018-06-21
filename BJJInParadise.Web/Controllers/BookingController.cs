using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BjjInParadise.Business;
using BJJInParadise.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace BJJInParadise.Web.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private AccountService _service;
        private CampService _campService;
        private ApplicationUserManager _userManager;

        public BookingController(AccountService service, CampService campService, ApplicationUserManager userManager)
        {
            _service = service;
            _campService = campService;
            UserManager = userManager;
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

            var vm = new NewBookingViewModel()
            {
                UserId = user.UserId,
                CampId = nextCamp.CampId,
                AvailableCamps = availCamps.ToList(),
                User = user,
                SelectedCamp = nextCamp,
                Email = userOwin.Email
            };
            return View(vm);
        }
    }
}