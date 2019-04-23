using MainLibrary.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.WebCenter
{
    [Table("MenuItem")]
    public class MenuItem
    {

        public MenuItem()
        {
            this.ChildMenuItems = new MenuItem[0];
            this.MenuTarget = "_parent";
            this.IsBindingWithParent = false;
            this.IsShownOnMenuBar = false;
            this.IsActive = true;
        }

        public int Id { get; set; }

        public int? ParentId { get; set; }

        public int MenuLevel { get; set; }

        public int MenuOrder { get; set; }

        public string MenuCssClass { get; set; }

        public string MenuController { get; set; }

        public string MenuAction { get; set; }

        public string MenuName { get; set; }

        public string MenuTarget { get; set; }

        public bool IsBindingWithParent { get; set; }

        //public int SiteId { get; set; }

        public bool IsPageBlank { get; set; }

        public bool IsShownOnMenuBar { get; set; }

        public bool IsActive { get; set; }
        public bool IsMainPage { get; set; }
        public bool IsAllowGuest { get; set; }

        [NotMapped]
        //[DataMember]
        public string DisplayName
        {
            get
            {
                //int level = GetParentLevel(ParentDepartment);
                return GetNodeDisplay(MenuLevel) + MenuName;
                //return MenuName;
            }
        }

        private string GetNodeDisplay(int level)
        {
            string node = string.Empty;
            for (int i = 0; i < level; i++)
            {
                node += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            }
            return level == 0 ? "&#9899; " : node + "&#9899; ";
        }

        [NotMapped]
        public int MenuOrderInList { get; set; }

        [NotMapped]
        public MenuItem[] ChildMenuItems { get; set; }

        [NotMapped]
        public MenuItem ParentMenu { get; set; }
    }

    public class MenuOrder
    {
        public int id { get; set; }
        public int level { get; set; }
        public int order { get; set; }
        public MenuOrder[] children { get; set; }

    }

    public static class MenuHelper
    {
        public static IEnumerable<MenuItem> SortMenuItem(this IEnumerable<MenuItem> menus, IEnumerable<MenuItem> fullMenus, ref int? currentOrder)
        {
            List<MenuItem> result = new List<MenuItem>();
            if (!currentOrder.HasValue)
                currentOrder = 0;
            var lowestLevel = menus.Min(m => m.MenuLevel);
            var lowestList = menus.Where(m => m.MenuLevel.Equals(lowestLevel)).OrderBy(m => m.MenuOrder).ToArray();
            foreach (var menu in lowestList)
            {
                currentOrder++;
                menu.MenuOrderInList = currentOrder.Value;
                result.Add(menu);
                var childMenu = fullMenus.Where(m => m.ParentId == menu.Id);
                if (childMenu?.Count() > 0)
                    result.AddRange(childMenu.SortMenuItem(fullMenus, ref currentOrder));
            }
            return result.OrderBy(m => m.MenuOrderInList);
        }
        public static IEnumerable<RoleMenu> SortMenuItem(this IEnumerable<RoleMenu> menus)
        {
            int? menuOrder = null;
            var menuList = menus.Select(m => m.Menu);
            var dictMenus = menus.ToDictionary(m => m.Menu, m => m.IsActive);
            return menuList.SortMenuItem(menuList, ref menuOrder).ToDictionary(m => m, m => dictMenus[m]).Select(m => new RoleMenu { Menu = m.Key, IsActive = m.Value });
        }

        public static IEnumerable<MenuItem> SortMenuItemToOne(this IEnumerable<MenuItem> menus)
        {
            List<MenuItem> result = new List<MenuItem>();
            foreach (var menu in menus)
            {
                result.Add(menu);
                if (menu.ChildMenuItems.Count() > 0)
                    result.AddRange(menu.ChildMenuItems.SortMenuItemToOne());
            }
            return result.OrderBy(m => m.MenuLevel);
        }
    }
}
