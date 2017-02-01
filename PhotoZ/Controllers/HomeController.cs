using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using PhotoZ.Models;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace PhotoZ.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            using (var client = new HttpClient())
            {
                string baseUrl = Request.Url.Scheme+ "://" +Request.Url.Authority;
                var url = baseUrl + Url.RouteUrl("DefaultApi", new { httproute = "", controller = "Photos", Request.Url.Scheme });
                ViewBag.Photos = client.GetAsync(url).Result.Content.ReadAsAsync<List<FileViewInfo>>().Result;
            }

            return View();
        }

        public ActionResult AddPhoto()
        {
            return View();
        }

        [HttpDelete]
        public ActionResult DeletePhoto(string id)
        {
            return View("Index");
        }

        private IGridFSBucket GetGridFSBucket()
        {
            MongoClient mongo = new MongoClient("mongodb://" + ConfigurationManager.AppSettings["DatabaseServer"]);
            IMongoDatabase db = mongo.GetDatabase(ConfigurationManager.AppSettings["DatabaseName"]);
            return new GridFSBucket(db);
        }
    }
}