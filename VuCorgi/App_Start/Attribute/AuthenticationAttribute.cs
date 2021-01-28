using Library.Entity.Base;
using Library.Resource;
using log4net;
using VuBongBongWeb;
using VuBongBongWeb.Security;
using MainLibrary.Entity.WebCenter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace VuBongBongWeb
{
    public class AuthenticationAttribute : AuthorizeAttribute
    {
        //private readonly ILog Log = LogManager.GetLogger(typeof(AuthenticationAttribute));
        private string ip;

        public bool IsSeparationView { get; set; }

        public AuthenticationAttribute()
        {
            //this.ip = HttpContext.Current.Request.UserHostAddress;
            ip = "Unknown";
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            ResponseObject jsonResult = new ResponseObject();
            jsonResult.Data = new Hashtable();
            jsonResult.Data.Add(Resource.K_RETURN_STATUS, false);
            jsonResult.Data.Add(Resource.K_RETURN_MESSAGE, string.Empty);
            var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            string loginAction = "login";
            string errorAction = "error404";
            string loginController = "Art_Account";
            string requestType = HttpContext.Current.Request.Headers["X-Requested-With"];
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;
            if (controller.ToLower() == "notification" && action.ToLower() == "getnotification")
                return;

            if (controller.ToLower() == "Art_Account" && action.ToLower() == "login")
                return;

            if (HttpContext.Current.User == null
            || (HttpContext.Current.User != null && !HttpContext.Current.User.Identity.IsAuthenticated))
            {
                if (filterContext.HttpContext.Request.IsAjaxRequest() || requestType == "XMLHttpRequest")
                {
                    jsonResult.Data[Resource.K_RETURN_STATUS] = false;
                    jsonResult.Data[Resource.K_RETURN_MESSAGE] = "System session time out.";
                    jsonResult.Data[Resource.K_CURRENT_ACTION] = loginAction;
                    jsonResult.Data[Resource.K_CURRENT_CONTROLLER] = loginController;
                    jsonResult.Data["returnUrl"] = string.Format("/{0}/{1}", controller, action);
                    filterContext.Result = new JsonResult { Data = jsonResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                }
                else
                {
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "action", loginAction },
                        { "controller", loginController },
                        { "returnUrl", string.Format("/{0}/{1}", controller, action) }
                    });
                }
            }
            else
            {
                #region Check permisison and redirect

                if (!UserManager.Instance.GetAndCheckPermissionOnMenu(controller, action))
                {
                    // Get Current Login Employee
                    var userModel = HttpContext.Current.User as WebPrincipal;
                    HttpContext.Current.Session[Resource.K_USER_INFORMATION] = userModel;
                    if (controller.ToLower() == "art_home" && action.ToLower() == "index")
                    {
                        // Try to get menu items in session
                        var menuItems = HttpContext.Current.Session[Resource.K_USER_MENU_LIST.ToString()] as IEnumerable<MenuItem>;
                        if (menuItems != null)
                        {
                            int? currentOrder = null;
                            var firstMenu = menuItems.SortMenuItem(menuItems, ref currentOrder).Where(m => m.IsActive && !m.IsPageBlank && !m.IsBindingWithParent).FirstOrDefault(m => m.MenuAction != null && m.MenuController != null && m.MenuAction != string.Empty && m.MenuController != string.Empty);
                            if (firstMenu != null)
                            {
                                filterContext.HttpContext.Session["DefaultUrl"] = new KeyValuePair<string, string>(firstMenu.MenuAction, firstMenu.MenuController);
                            }
                        }
                    }

                    // Get Current Login Site
                    Uri url = HttpContext.Current.Request.Url;
                    string siteUrl = url.AbsoluteUri.EndsWith("/" + controller) ||
                        url.AbsoluteUri.EndsWith("/" + action)
                            ? url.AbsoluteUri.Replace(url.PathAndQuery, string.Empty) : url.AbsoluteUri;
                    siteUrl = siteUrl.EndsWith("/") ? siteUrl : siteUrl + "/";
                    string siteName = Settings.Project;
                    string message = string.Format("Access Of User[{0}] to Site[{1} - {2}] & Function[{3} - {4}] has been denied. Access From [{5}]",
                        userModel.UserDetail.UserName, siteUrl, siteName, controller, action, ip);
                    //Log.Warn(message);
                    if (requestType == "XMLHttpRequest")
                    {
                        jsonResult.Data[Resource.K_RETURN_STATUS] = false;
                        jsonResult.Data[Resource.K_RETURN_MESSAGE] = "You don't have permission to use this function";
                        jsonResult.Data[Resource.K_CURRENT_ACTION] = errorAction;
                        jsonResult.Data[Resource.K_CURRENT_CONTROLLER] = loginController;
                        filterContext.Result = new JsonResult { Data = jsonResult, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    }
                    else
                    {
                        if (filterContext.HttpContext.Session["DefaultUrl"] != null)
                        {
                            KeyValuePair<string, string> defaultAction = (KeyValuePair<string, string>)filterContext.HttpContext.Session["DefaultUrl"];
                            filterContext.HttpContext.Session["DefaultUrl"] = null;
                            filterContext.Result = new RedirectToRouteResult(
                                 new RouteValueDictionary
                                 {
                                       { "action", defaultAction.Key },
                                       { "controller", defaultAction.Value }
                                 });
                        }
                        else
                        {
                            filterContext.Result = new RedirectToRouteResult(
                                 new RouteValueDictionary
                                 {
                                       { "action", errorAction },
                                       { "controller", loginController }
                                 });
                        }
                    }
                }

                #endregion
            }
        }
    }
}