using MainLibrary.Entity.WebCenter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VuBongBongWeb.Models
{
    public class CateModel : Category
    {
        [AllowHtml]
        public string HTMLstr { get { return Description; } set { this.Description = value; } }

        public Category GetOriginal()
        {
            string obj = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<Category>(obj);
        }
    }
}