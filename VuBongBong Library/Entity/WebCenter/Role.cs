using MainLibrary.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.WebCenter
{
    [Table("Role")]
    public class Role : BasicEntity
    {
        public Role()
        {
            MenuList = new List<RoleMenu>();
        }
        public int Id { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        [NotMapped]
        public string TotalRole { get; set; }
        [NotMapped]
        public string TotalUser { get; set; }
        [NotMapped]
        public DateTime LastModifier { get; set; }
        //[NotMapped]
        //public string Editer { get; set; }

        [NotMapped]
        public List<MenuManagement> MenuRoles { get; set; }

        private IEnumerable<RoleMenu> _menuList;

        [NotMapped]
        public IEnumerable<RoleMenu> MenuList
        {
            get
            {

                if (MenuRoles != null && MenuRoles.Count > 0 && _menuList == null)
                {
                    IEnumerable<RoleMenu> menuList = MenuRoles.Where(m => m.MenuItem != null && m.MenuItem.IsActive).ToDictionary(m => m.MenuItem, m => m.IsActive).Select(m => new RoleMenu { Menu = m.Key, IsActive = m.Value });
                    //int? menuOrder = null;
                    _menuList = menuList;//.Select(m => m.Key).SortMenuItem(ref menuOrder).ToDictionary(m => m, m => menuList[m]);

                }
                return _menuList;
            }
            set
            {
                _menuList = value;
            }
        }
        public void SortMenuOrder()
        {
            MenuList = MenuList.SortMenuItem();
        }
    }

    public class RoleMenu
    {
        public MenuItem Menu { get; set; }
        public bool IsActive { get; set; }
    }
}
