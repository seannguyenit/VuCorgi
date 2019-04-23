using VuBongBongWeb.Management.WebCenterManagement;
using MainLibrary.Entity.WebCenter;
using System.Linq;
using System.Web.Mvc;
using VuBongBongWeb.Security;
using System;
using System.Collections.Generic;

namespace WebCenter.Controllers.FE
{
    public class Art_FEHomeController : Controller
    {
        // GET: Art_FEHome
        public ActionResult Index()
        {
            ViewBag.IsHome = true;
            return View();
        }

        public ActionResult TopBanner()
        {
            var data = new Banner[0];
            using (BannerManager manager = new BannerManager())
            {
                data = manager.GetAllBanner(string.Empty, out string error, true).ToArray();
            }
            return PartialView("TopBanner", data);
        }

        public ActionResult BangCacDichVu()
        {
            var data = new Category[0];
            using (var _manager = new CategoryManager())
            {
                data = _manager.GetAllCategory(string.Empty, out string error, isMainTable: true);
            }
            return PartialView(data);
        }

        public ActionResult AboutUs()
        {
            var data = new WebInfomation();
            using (WebInfoManager manager = new WebInfoManager())
            {
                data = manager.GetInfoByType(MainLibrary.Resource.WebCenter.EnumTypeInfo.About.ToString());
            }
            return View(data);
        }

        public ActionResult Contact()
        {
            var data = new WebInfomation();
            using (WebInfoManager manager = new WebInfoManager())
            {
                data = manager.GetInfoByType(MainLibrary.Resource.WebCenter.EnumTypeInfo.Contact.ToString());
            }
            return View(data);
        }

        public ActionResult Service()
        {
            //ViewBag.BusinessKind = null;
            //using (BusinessKindManager manager = new BusinessKindManager())
            //{
            //    var data = manager.GetAllBusinessKind(null, out string error, true).Take(6).ToList();
            //    ViewBag.BusinessKind = data;
            //}
            return View();
        }

        public ActionResult QuickQuestion()
        {
            var data = new WebInfomation();
            using (WebInfoManager manager = new WebInfoManager())
            {
                data = manager.GetInfoByType(MainLibrary.Resource.WebCenter.EnumTypeInfo.QuickQuestion.ToString());
            }
            return View(data);
        }

        public ActionResult Library()
        {
            //ViewBag.BusinessKind = null;
            //using (BusinessKindManager manager = new BusinessKindManager())
            //{
            //    var data = manager.GetAllBusinessKind(null, out string error, true).Take(6).ToList();
            //    ViewBag.BusinessKind = data;
            //}
            return View();
        }

        public ActionResult HumanResource()
        {
            var data = new WebInfomation();
            using (WebInfoManager manager = new WebInfoManager())
            {
                data = manager.GetInfoByType(MainLibrary.Resource.WebCenter.EnumTypeInfo.HumanResource.ToString());
            }
            return View(data);
        }

        public ActionResult NewEventPartial()
        {
            var data = new Category[0];
            using (var _manager = new CategoryManager())
            {
                data = _manager.GetAllCategory(string.Empty, out string error,isShowHomePage: true);
            }
            if (data != null && data.Any())
            {
                using (var repo = new NewsAndAlbumManager())
                {
                    foreach(var i in data)
                    {
                        i.Child = repo.GetAllNews(string.Empty,null,null,null, out string error, true, cateId:i.Id,type: MainLibrary.Resource.WebCenter.NewsType.News.ToString()).ToList();
                    }
                }
            }
            return PartialView(data);
        }

        public ActionResult LibraryPartial()
        {
            Dictionary<Category, AlbumDetail[]> data = new Dictionary<Category, AlbumDetail[]>();
            var lstImg = new AlbumDetail[0];
            using (var _manager = new NewsAndAlbumManager())
            {
                lstImg = _manager.GetAllAlbumDetail(null, out string error, isPin: true);
            }
            foreach(var item in lstImg.Select(c=>c.CateId).Distinct())
            {
                Category cate = new Category()
                {
                    Id = item,
                    Name = lstImg.FirstOrDefault(a => a.CateId == item)?.CateName
                };
                AlbumDetail[] arr = lstImg.Where(w => w.CateId == item).ToArray();
                data.Add(cate, arr);
            }
            return PartialView(data);
        }

        public ActionResult SpecialImg()
        {
            var data = new AlbumDetail[0];
            using (var _manager = new NewsAndAlbumManager())
            {
                data = _manager.GetAllAlbumDetail(null, out string error, isPin: true);
            }
            return PartialView(data);
        }

        public ActionResult ContactImg()
        {
            var data = new WebInfomation();
            using (var repo = new WebInfoManager())
            {
                data = repo.GetInfoByType(MainLibrary.Resource.WebCenter.EnumTypeInfo.ContactImg.ToString());
            }
            return PartialView(data);
        }

        public ActionResult FacebookPage()
        {
            return PartialView();
        }

        public ActionResult LeftCate()
        {
            var data = new Category[0];
            using (var _manager = new CategoryManager())
            {
                data = _manager.GetAllCategory(string.Empty, out string error, isShowLeft: true).OrderBy(c=>c.Order).ToArray();
            }
            return PartialView(data);
        }

        public ActionResult Workflow()
        {
            return PartialView();
        }

    }
}