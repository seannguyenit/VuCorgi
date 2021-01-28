using MainLibrary.Entity.WebCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VuBongBongWeb.Models
{
    public class WebInfoModel : WebInfomation
    {
        [AllowHtml]
        public string HTMLstr { get; set; }
    }
}