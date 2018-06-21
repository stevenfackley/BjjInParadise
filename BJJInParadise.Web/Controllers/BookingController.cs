using System.Web.Mvc;

namespace BJJInParadise.Web.Controllers
{
    public class BookingController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("ComingSoon", "Home");
        }
    }
}