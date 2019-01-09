using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BJJInParadise.Web.Helpers;
using Microsoft.AspNet.Identity;

namespace BJJInParadise.Web.Controllers
{
    [RequireHttps][HandleError]
    public abstract class BaseController : Controller
    {
        protected void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            filterContext.ExceptionHandled = true;
          
            // Redirect on error:
            filterContext.Result = RedirectToAction("Index", "Error");

        }
        protected List<SelectListItem> CreateCountryDropDown()
        {
            Dictionary<string, string> objDic = new Dictionary<string, string>();

            foreach (var ObjCultureInfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                var objRegionInfo = new RegionInfo(ObjCultureInfo.Name);
                if (!objDic.ContainsKey(objRegionInfo.EnglishName))
                {
                    objDic.Add(objRegionInfo.EnglishName, objRegionInfo.TwoLetterISORegionName.ToLower());
                }
            }

            var obj = objDic.OrderBy(p => p.Key);
            var list = new List<SelectListItem>();
            foreach (var val in obj)
            {
                list.Add(new SelectListItem { Value = val.Value.ToUpper(), Text = val.Key });
            }

            return list.OrderBy(x => x.Text).ToList().AddEmpty();
        }

    }
}