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

using MongoDB.Bson;
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