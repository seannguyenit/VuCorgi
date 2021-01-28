using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VuBongBongWeb.Models
{
    public class DisplayModel
    {
        public string Controller { get; set; }
        public string Action { get; set; }

        public string OriginalController { get; set; }
        public string OriginalAction { get; set; }
    }
}