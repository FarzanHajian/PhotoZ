﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace PhotoZ.Controllers
{
    public class PhotosController : ApiController
    {
        public IHttpActionResult Get()
        {
            IGridFSBucket bucket = GetGridFSBucket();
            return Json(bucket.Find(Builders<GridFSFileInfo>.Filter.Empty).ToList());
        }

        public HttpResponseMessage Get(string id)
        {
            IGridFSBucket bucket = GetGridFSBucket();
            ObjectId objectId = new ObjectId(id);
            //GridFSFileInfo fileInfo = bucket.Find(Builders<GridFSFileInfo>.Filter.Eq(p => p.Id, objectId)).First();

            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(bucket.DownloadAsBytes(objectId));
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpg");
            return result;
        }

        public async Task<IHttpActionResult> Post()
        {
            // Check if the request contains multipart/form-data.
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            // Read the form data.
            string root = HttpContext.Current.Server.MapPath("~/App_Data");
            var provider = new MultipartFormDataStreamProvider(root);
            await Request.Content.ReadAsMultipartAsync(provider);

            // This illustrates how to get the file names.
            foreach (MultipartFileData file in provider.FileData)
            {
                IGridFSBucket bucket = GetGridFSBucket();
                using (FileStream fileStream = new FileStream(file.LocalFileName, FileMode.Open))
                {
                    string fileName = provider.FormData["name"];
                    bucket.UploadFromStream(
                        String.IsNullOrEmpty(fileName) ? file.Headers.ContentDisposition.FileName : fileName,
                        fileStream,
                        new GridFSUploadOptions { ContentType = file.Headers.ContentType.ToString() }
                    );
                }
                File.Delete(file.LocalFileName);
            }

            return RedirectToRoute("Default", new { Controller = "Home", Action = "Index" });
        }

        public IHttpActionResult Delete(string id)
        {
            IGridFSBucket bucket = GetGridFSBucket();
            bucket.Delete(new ObjectId(id));
            return Ok();
        }

        private IGridFSBucket GetGridFSBucket()
        {
            MongoClient mongo = new MongoClient("mongodb://" + ConfigurationManager.AppSettings["DatabaseServer"]);
            IMongoDatabase db = mongo.GetDatabase(ConfigurationManager.AppSettings["DatabaseName"]);
            return new GridFSBucket(db);
        }
    }
}