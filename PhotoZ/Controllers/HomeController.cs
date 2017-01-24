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
                string baseUrl = Request.Url.ToString();
                var url = baseUrl.Remove(baseUrl.Length - 1) + Url.RouteUrl("DefaultApi", new { httproute = "", controller = "Photos", Request.Url.Scheme });
                ViewBag.Photos = client.GetAsync(url).Result.Content.ReadAsAsync<List<FileViewInfo>>().Result;
            }

            //IGridFSBucket bucket = GetGridFSBucket();
            //ViewBag.Photos = bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();
            return View();
        }

        public ActionResult AddPhoto()
        {
            return View();
        }

        //[HttpGet]
        //public ActionResult GetPhoto(string id)
        //{
        //    IGridFSBucket bucket = GetGridFSBucket();
        //    ObjectId objectId = new ObjectId(id);
        //    //GridFSFileInfo fileInfo = bucket.Find(Builders<GridFSFileInfo>.Filter.Eq(p => p.Id, objectId)).First();
        //    return File(bucket.DownloadAsBytes(objectId), "image/jpg");
        //}

        [HttpDelete]
        public ActionResult DeletePhoto(string id)
        {
            return View("Index");
        }

        //public ActionResult Foo()
        //{
        //    IGridFSBucket bucket = GetGridFSBucket();
        //    Task<ObjectId> task1 = bucket.UploadFromStreamAsync("_MG_0515.jpg", new FileStream(@"D:\Stuff\Wallpapers\_MG_0515.jpg", FileMode.Open));
        //    Task<ObjectId> task2 = bucket.UploadFromStreamAsync("Do, Re, Mi, Fa, Sol, La, Si.jpg", new FileStream(@"D:\Stuff\Wallpapers\Do, Re, Mi, Fa, Sol, La, Si.jpg", FileMode.Open));
        //    Task.WaitAll(task1, task2);

        //    return View("AddPhoto");
        //}

        private IGridFSBucket GetGridFSBucket()
        {
            MongoClient mongo = new MongoClient("mongodb://" + ConfigurationManager.AppSettings["DatabaseServer"]);
            IMongoDatabase db = mongo.GetDatabase(ConfigurationManager.AppSettings["DatabaseName"]);
            return new GridFSBucket(db);
        }
    }
}