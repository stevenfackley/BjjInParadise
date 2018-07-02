using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;

namespace BJJInParadise.Web.Controllers
{
    public class GalleryController : BaseController
    {
        // GET: Gallery
        public ActionResult Index()
        {
          
            return View();
        }

        private async Task<IEnumerable<ImageResult>> GetImagesAsync()
        {
            try
            {
                var client = new ImgurClient("bc01b8dbb915f5f", "07927d3b0dfe5c530d663d89b9110100f7a1d506");
                var endpoint = new AlbumEndpoint(client);
                var image = await endpoint.GetAlbumImagesAsync("SZFpO6D");
             
                return image.Select(x => new ImageResult {Link = x.Link,Description = x.Description}).ToList();
            }
            catch (ImgurException imgurEx)
            {
                Debug.Write("An error occurred getting an image from Imgur.");
                Debug.Write(imgurEx.Message);
            }

            return null;
        }

     

     
    }

    public class ImageResult
    {
        public string Link { get; set; }
        public string Description { get; set; }
    }
}