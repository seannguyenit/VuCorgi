using MainLibrary.Entity.WebCenter;
using SeanAPI.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VuBongBongWeb.Management.WebCenterManagement
{
    public class WebBaseManager : DataExtension, IDisposable
    {
        private WebCenterContext _context = new WebCenterContext();

        public WebBaseManager()
        {
            cn = (SqlConnection)(_context.Database.Connection);
        }

        public void Dispose()
        {
            cn.Close();
        }

        public bool SynMenuWeb(IEnumerable<MenuItem> menus)
        {
            //error = string.Empty;
            try
            {
                var currentMenu = _context.MenuItems.ToList();
                //menus = this.StretchMenus(menus, site.Id);
                foreach (var menu in menus)
                {
                    InsertOrUpdateMenu(menu, currentMenu);
                }
                var disableMenu = currentMenu
                    .Where(p => p.IsActive &&
                    !menus.SortMenuItemToOne().Any(nm =>
                    (nm.MenuAction ?? string.Empty).ToLower() == (p.MenuAction ?? string.Empty).ToLower() &&
                    (nm.MenuController ?? string.Empty).ToLower() == (p.MenuController ?? string.Empty).ToLower()));
                foreach (var menu in disableMenu.ToArray())
                {
                    menu.IsActive = false;
                    //menu.UpdatedDate = DateTime.Now;
                    _context.Entry(menu).State = System.Data.Entity.EntityState.Modified;
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                //error = ex.Message;
                //throw ex;
            }
            return false;
        }

        private bool InsertOrUpdateMenu(MenuItem menuItem, List<MenuItem> currentMenu)
        {
            try
            {
                // Find menu by action and controller
                var currentMenuItem = currentMenu.Where(m => (m.MenuController ?? string.Empty).ToLower()
                    .Equals((menuItem.MenuController ?? string.Empty).ToLower())
                    && (m.MenuAction ?? string.Empty).ToLower()
                    .Equals((menuItem.MenuAction ?? string.Empty).ToLower())).FirstOrDefault();

                var parentItem = currentMenu.Where(p => menuItem.ParentMenu != null &&
                            (p.MenuAction ?? string.Empty).ToLower() ==
                            (menuItem.ParentMenu.MenuName ?? string.Empty).ToLower()).FirstOrDefault();

                if (parentItem == null && menuItem.MenuLevel == 1)
                {
                    parentItem = currentMenu.Where(p => p.MenuController == menuItem.MenuController
                                                    && p.MenuAction == null).FirstOrDefault();
                }

                // If existed, update menu
                if (currentMenuItem != null)
                {
                    currentMenuItem.IsActive = true;
                    currentMenuItem.IsBindingWithParent = menuItem.IsBindingWithParent;
                    currentMenuItem.IsPageBlank = menuItem.IsPageBlank;
                    currentMenuItem.MenuCssClass = menuItem.MenuCssClass;
                    currentMenuItem.MenuLevel = menuItem.MenuLevel;
                    currentMenuItem.MenuName = menuItem.MenuName;
                    currentMenuItem.MenuOrder = menuItem.MenuOrder;
                    currentMenuItem.MenuOrderInList = menuItem.MenuOrderInList;
                    currentMenuItem.MenuTarget = menuItem.MenuTarget;
                    currentMenuItem.IsShownOnMenuBar = menuItem.IsShownOnMenuBar;
                    currentMenuItem.IsMainPage = menuItem.IsMainPage;
                    currentMenuItem.IsAllowGuest = menuItem.IsAllowGuest;

                    if (parentItem != null)
                    {
                        currentMenuItem.ParentId = parentItem.Id;
                    }
                    currentMenuItem.ParentMenu = parentItem;
                    //currentMenuItem.UpdatedUser = Guid.Empty;
                    //currentMenuItem.UpdatedDate = DateTime.Now;
                    _context.Entry(currentMenuItem).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                    if (menuItem.ChildMenuItems != null)
                    {
                        foreach (var child in menuItem.ChildMenuItems)
                        {

                            //currentMenu.Add(child);
                            child.ParentId = menuItem.Id;
                            InsertOrUpdateMenu(child, currentMenu);
                            //_context.Entry(child).State = System.Data.Entity.EntityState.Added;
                        }
                    }
                    //_context.SaveChanges();
                }
                else
                {
                    // If have parent, insert follow parent information
                    if (parentItem != null)
                    {
                        var newMenuItem = new MenuItem
                        {
                            ParentId = parentItem.Id,
                            ParentMenu = parentItem,
                            //SiteId = parentItem.SiteId,
                            MenuLevel = parentItem.MenuLevel + 1,
                            MenuOrder = (parentItem.ChildMenuItems == null || parentItem.ChildMenuItems.Length == 0) ? 1 : (parentItem.ChildMenuItems.Max(p => p.MenuOrder) + 1),
                            MenuController = menuItem.MenuController,
                            MenuAction = menuItem.MenuAction,
                            MenuName = menuItem.MenuName,
                            IsBindingWithParent = true,
                            IsActive = true,
                            IsMainPage = menuItem.IsMainPage,
                            IsAllowGuest = menuItem.IsAllowGuest
                        };
                        _context.Entry(newMenuItem).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                        return true;
                    }
                    else
                    {
                        _context.Entry(menuItem).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                        if (menuItem.ChildMenuItems != null)
                        {
                            foreach (var child in menuItem.ChildMenuItems)
                            {

                                //currentMenu.Add(child);
                                child.ParentId = menuItem.Id;
                                InsertOrUpdateMenu(child, currentMenu);
                                //_context.Entry(child).State = System.Data.Entity.EntityState.Added;
                            }
                        }
                    }
                    //_context.SaveChanges();
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //private IEnumerable<MenuItem> StretchMenus(IEnumerable<MenuItem> menus)
        //{
        //    List<MenuItem> menuList = new List<MenuItem>();
        //    if (menus != null)
        //    {
        //        foreach (var item in menus)
        //        {
        //            menuList.Add(item);
        //            if (item.ChildMenuItems != null && item.ChildMenuItems.Length > 0)
        //            {
        //                menuList.AddRange(StretchMenus(item.ChildMenuItems));
        //            }
        //        }
        //    }
        //    return menuList;
        //}


        /// <summary>
        /// roleid = 0 => admin
        /// roleid = null => guest
        /// </summary>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IEnumerable<MenuItem> GetMenuItemListByRole(int? roleId, bool isAdmin = false)
        {
            try
            {
                Dictionary<int, bool> menuManagement = null;
                if (roleId.HasValue)
                    if (roleId.Value == -1 || isAdmin)
                    {

                        menuManagement = _context.MenuManagements.ToDictionary(m => m.MenuItemId, m => m.IsActive);
                    }
                    else
                    {
                        menuManagement = _context.MenuManagements.Where(m => m.RoleId.Equals(roleId.Value)).ToDictionary(m => m.MenuItemId, m => m.IsActive);
                    }
                var allMenu = _context.MenuItems.Where(m => m.IsActive).ToList();
                var menues = allMenu.Select(m =>
                {
                    var menu = m;
                    menu.ChildMenuItems = allMenu.Where(c => c.ParentId == menu.Id).ToArray();
                    menu.IsActive = m.IsAllowGuest || ((menuManagement != null && menuManagement.ContainsKey(m.Id)) ? menuManagement[m.Id] : menu.IsActive);
                    return menu;
                }).ToArray();
                return menues;
            }
            catch (Exception ex)
            {
                //Log.Error("GetMenuItemListByRole", ex);
            }
            return null;
        }
    }
}