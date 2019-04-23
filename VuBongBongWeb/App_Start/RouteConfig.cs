#region Using

using VuBongBongWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;

#endregion

namespace VuBongBongWeb
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.LowercaseUrls = true;
            routes.MapRoute(
                "Admin",
                "Admin/{action}/{id}",
                new { controller = "Art_Home", action = "Index", id = UrlParameter.Optional }
                );

            //create new route  
            //routes.MapRoute(
            //  name: "DefaultRewriteURL",
            //  url: "Workflow/WorkingDetails/{id}"
            //  .Replace("parentId=", string.Empty)
            //  .Replace("hashtag=",string.Empty)
            //  .Replace("&","/").Replace(' ','-'),
            //  defaults: new { controller = "Workflow", action = "WorkingDetails", id = UrlParameter.Optional }
            //);


            #region Use this if you want to rewrite all controller

            //RegisAllFEUrl(routes); 

            #endregion

            routes.MapMvcAttributeRoutes();
            routes.MapRoute("Default", "{controller}/{action}/{id}", new
            {
                controller = "Art_FEHome",
                action = "Index",
                id = UrlParameter.Optional
            }).RouteHandler = new DashRouteHandler();



            //routes.MapRoute(
            //name: "Management",
            //url: "{controller}/{action}",
            //defaults: new { controller = "Management", action = "Index" }
            //);
            //routes.MapRoute(
            //"Management",
            //"Management",
            //new { controller = "Management", action = "Index" },
            //);
        }

        /// <summary>
        /// rewrite url
        /// </summary>
        /// <param name="routes"></param>
        public static void RegisAllFEUrl(RouteCollection routes)
        {
            var assembly = Assembly.GetCallingAssembly();
            IEnumerable<Type> typeOfcontrollers = assembly.GetTypes()
               .Where(type => typeof(Controller).IsAssignableFrom(type));
            foreach (var item in typeOfcontrollers)
            {
                var methods = item.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                                         .Where(m => m.ReturnType == typeof(ActionResult))
                                         .Where(m => m.GetCustomAttribute(typeof(DisplayAttribute)) != null);
                foreach (var method in methods)
                {
                    var rootInfo = method.GetCustomAttribute<DisplayAttribute>();
                    if (rootInfo != null && !(string.IsNullOrEmpty(rootInfo.Action)))
                    {
                        var md = new DisplayModel()
                        {
                            OriginalController = item.Name,
                            OriginalAction = method.Name,
                            Controller = rootInfo.Controller,
                            Action = rootInfo.Action
                        };
                        routes.MapRoute(
                                          name: "DefaultRewriteURL" + md.Controller + md.Action,
                                          url: /*"workflow/details/{id}"*/ string.Concat(md.Controller, "/", md.Action, "/"),
                                          defaults: new { controller = md.OriginalController.Replace("Controller", string.Empty), action = md.OriginalAction, id = UrlParameter.Optional }
                                        );
                    }
                }
            }
        }
    }
}