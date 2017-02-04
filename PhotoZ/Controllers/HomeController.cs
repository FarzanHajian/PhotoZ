/*
Copyright 2017 Farzan Hajian

This file is part of PhotoZ.

PhotoZ is free software: you can redistribute it and/or modify it under the terms of the GNU
General Public License as published by the Free Software Foundation, either version 3 of the
License, or (at your option) any later version.

PhotoZ is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without
even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License along with PhotoZ.
If not, see http://www.gnu.org/licenses/.
*/

using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using PhotoZ.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
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
                string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority;
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