using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BjjInParadise.Business;

namespace BJJInParadise.Web.Controllers
{
    public class CampRoomOptionController : Controller
    {
        private readonly CampRoomOptionService _campRoomOptionService;

        public CampRoomOptionController(CampRoomOptionService campService)
        {

            _campRoomOptionService = campService;
        }
        // GET: CampRoomOption
        public ActionResult Index(int id)
        {
            var result = _campRoomOptionService.GetByCampId(id);
            return PartialView("_Index",result);
        }
    }
}