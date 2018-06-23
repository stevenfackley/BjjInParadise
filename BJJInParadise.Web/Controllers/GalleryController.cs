using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BJJInParadise.Web.Controllers
{
    public class GalleryController : Controller
    {
        // GET: Gallery
        public ActionResult Index()
        {
            var dir = Server.MapPath("/Images/Images");
            var retVal = new List<string>();
          
            string projectFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).FullName;
            string folderAppData = Path.Combine(projectFolder, "Images");
            if (Directory.Exists(folderAppData))
            {
                foreach (var file in Directory.EnumerateFiles(folderAppData))
                {
                    retVal.Add("~/Images/" + Path.GetFileName(file));
                }
            }

            return View(retVal);
        }
    }
}