using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace PhotoZ.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            IGridFSBucket bucket = GetGridFSBucket();
            ViewBag.Photos = bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList();
            return View();
        }

        public ActionResult AddPhoto()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Photos(ObjectId id)
        {
            IGridFSBucket bucket = GetGridFSBucket();
            return File(bucket.DownloadAsBytes(id), "image/*");
        }

        public ActionResult Foo()
        {
            IGridFSBucket bucket = GetGridFSBucket();
            Task<ObjectId> task1 = bucket.UploadFromStreamAsync("_MG_0515.jpg", new FileStream(@"D:\Stuff\Wallpapers\_MG_0515.jpg", FileMode.Open));
            Task<ObjectId> task2 = bucket.UploadFromStreamAsync("Do, Re, Mi, Fa, Sol, La, Si.jpg", new FileStream(@"D:\Stuff\Wallpapers\Do, Re, Mi, Fa, Sol, La, Si.jpg", FileMode.Open));
            Task.WaitAll(task1, task2);

            return View("AddPhoto");
        }

        private IGridFSBucket GetGridFSBucket()
        {
            MongoClient mongo = new MongoClient();
            IMongoDatabase db = mongo.GetDatabase("test");
            return new GridFSBucket(db);
        }
    }
}