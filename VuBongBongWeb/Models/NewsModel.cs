using MainLibrary.Entity.WebCenter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VuBongBongWeb.Models
{
    public class NewsModel : News
    {
        public NewsModel()
        {
            IsActive = true;
            IsDelete = false;
        }

        [AllowHtml]
        public string HTMLstr { get { return Content; } set { this.Content = value; } }
        //public HttpPostedFileBase myFile { get; set; }
        public News GetOriginal()
        {
            string obj = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<News>(obj);
        }
    }
}