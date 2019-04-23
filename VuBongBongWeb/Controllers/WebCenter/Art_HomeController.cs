using VuBongBongWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebCenter.Controllers
{
    [Authentication]
    public class Art_HomeController : Controller
    {
        // GET: Art_Home
        public ActionResult Index()
        {
            return View();
        }
    }
}