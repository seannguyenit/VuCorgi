using VuBongBongWeb.Management.WebCenterManagement;
using MainLibrary.Entity.WebCenter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebCenter;

namespace VuBongBongWeb
{
    public static class MenuSyncConfig
    {
        private static IEnumerable<MenuItem> GetChildMenuItem(this IEnumerable<MethodInfo> methods, int level, string controller, string parentAction)
        {
            var childMenus = methods.Where(m => ((MenuSyncAttribute)m.GetCustomAttribute(typeof(MenuSyncAttribute))).Level == level)
                .Where(m => ((MenuSyncAttribute)m.GetCustomAttribute(typeof(MenuSyncAttribute))).ParentAction == parentAction)
                .Select(m =>
                {
                    MenuSyncAttribute menuAttribute = (MenuSyncAttribute)m.GetCustomAttribute(typeof(MenuSyncAttribute));
                    string description = menuAttribute.Description;
                    string menuTagert = menuAttribute.MenuTaget;
                    bool isParentBinding = menuAttribute.IsBindingWithParent;
                    bool isBlankPage = menuAttribute.IsBlankPage;
                    bool isShownOnMenuBar = menuAttribute.IsShownOnMenuBar;
                    string menuCss = menuAttribute.CssClass;
                    int order = menuAttribute.SyncOrder;
                    bool isMainPage = menuAttribute.IsMainPage;
                    bool isAllowGuest = menuAttribute.IsAllowGuest;
                    return new MenuItem
                    {
                        MenuController = controller,
                        MenuAction = m.Name,
                        MenuOrder = order,
                        MenuLevel = level,
                        MenuName = description,
                        IsBindingWithParent = isParentBinding,
                        MenuTarget = menuTagert,
                        MenuCssClass = menuCss,
                        IsPageBlank = isBlankPage,
                        IsShownOnMenuBar = isShownOnMenuBar,
                        IsMainPage = isMainPage,
                        IsAllowGuest = isAllowGuest
                    };
                }).ToArray();
            int maxLevel = 0;
            if (methods.Count() > 0)
            {
                maxLevel = methods.Max(m => ((MenuSyncAttribute)m.GetCustomAttribute(typeof(MenuSyncAttribute))).Level);
            }
            int nextLevel = methods.Select(m => ((MenuSyncAttribute)m.GetCustomAttribute(typeof(MenuSyncAttribute))).Level).OrderBy(l => l).FirstOrDefault(l => l > level && l <= maxLevel);
            if (nextLevel > level)
            {
                int cnt = childMenus.Count();
                for (int i = 0; i < cnt; i++)
                {
                    childMenus[i].ChildMenuItems = methods.GetChildMenuItem(nextLevel, controller, childMenus[i].MenuAction).ToArray();
                    foreach (var child in childMenus[i].ChildMenuItems)
                    {
                        child.ParentMenu = childMenus[i];
                    }
                }
            }

            return childMenus;
        }
        public static bool RegisterMenuSyncConfig()
        {
            //var wcfHelper = DependencyResolver.Current.GetService<IWcfHelper>();
            // Read Current Call Assembly (TestSystem.Web)
            Uri url = HttpContext.Current.Request.Url;
            string siteUrl = url.AbsoluteUri.EndsWith("/") ? url.AbsoluteUri : url.AbsoluteUri.Replace(url.PathAndQuery, string.Empty);
            string siteName = Settings.Project;

            var assembly = Assembly.GetCallingAssembly();

            IEnumerable<Type> typeOfcontrollers = assembly.GetTypes()
                .Where(type => typeof(Controller).IsAssignableFrom(type) && type.GetCustomAttribute<MenuSyncAttribute>() != null);

            var rootMenuItems = typeOfcontrollers.ToDictionary(c =>
            {
                var rootMenuInfo = c.GetCustomAttribute<MenuSyncAttribute>();
                return new MenuItem
                {
                    MenuController = c.Name.Replace("Controller", string.Empty),
                    MenuAction = null,
                    MenuOrder = rootMenuInfo.SyncOrder,
                    MenuName = rootMenuInfo.Description,
                    IsBindingWithParent = rootMenuInfo.IsBindingWithParent,
                    IsShownOnMenuBar = rootMenuInfo.IsShownOnMenuBar,
                    MenuCssClass = rootMenuInfo.CssClass,
                    IsMainPage = rootMenuInfo.IsMainPage,
                    IsAllowGuest = rootMenuInfo.IsAllowGuest,
                    MenuTarget = null,
                    ParentId = null
                };
            }, c =>
            {
                var methods = c.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public)
                                          .Where(m => m.ReturnType == typeof(ActionResult) || m.ReturnType == typeof(JsonResult) || m.ReturnType == typeof(Task<ActionResult>))
                                          .Where(m => m.GetCustomAttribute(typeof(MenuSyncAttribute)) != null);
                return methods.OrderBy(m => ((MenuSyncAttribute)m.GetCustomAttribute(typeof(MenuSyncAttribute))).Level).ToArray();
            });

            List<MenuItem> menuList = new List<MenuItem>();
            foreach (var menu in rootMenuItems)
            {
                var rootMenu = menu.Key;
                int lowestLevel = 0;
                if (menu.Value.Count() > 0)
                {
                    lowestLevel = menu.Value.Min(m => ((MenuSyncAttribute)m.GetCustomAttribute(typeof(MenuSyncAttribute))).Level);
                }
                rootMenu.ChildMenuItems = menu.Value.GetChildMenuItem(lowestLevel, rootMenu.MenuController, rootMenu.MenuAction).ToArray();
                foreach (var child in rootMenu.ChildMenuItems)
                {
                    child.ParentMenu = rootMenu;
                }
                menuList.Add(rootMenu);
            }

            //Hashtable data = new Hashtable();
            //data.Add(CommonResources.Resource.K_MENU_ROUTE, menuList);
            //data.Add(CommonResources.Resource.K_SITE_NAME, siteName);
            //data.Add(CommonResources.Resource.K_SITE_URL, siteUrl);
            try
            {
                var syncStatus = false;
                using (var _manager = new WebBaseManager())
                {
                    syncStatus = _manager.SynMenuWeb(menuList);

                }
                return syncStatus;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}