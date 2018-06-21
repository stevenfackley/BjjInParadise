using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using BjjInParadise.Business;
using BjjInParadise.Core.Constants;

namespace BJJInParadise.Web.Controllers
{
    public class HomeController : Controller
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

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ComingSoon()
        {
            return new FilePathResult("~/Views/Home/ComingSoon/index.html", "text/html");
        }
    }
}