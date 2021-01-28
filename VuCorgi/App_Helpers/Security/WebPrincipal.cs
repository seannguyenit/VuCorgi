using MainLibrary.Entity.WebCenter;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace VuBongBongWeb.Security
{
    public class WebPrincipal : IWebPrincipal
    {
        public WebPrincipal(string name)
        {
            this.Identity = new GenericIdentity(name);
        }
        public IIdentity Identity { get; private set; }
        public bool IsInRole(string role)
        {
            return true;
        }

        public User UserDetail { get; set; }
        //public int RoleId { get; set; }
        //public string RoleName { get; set; }
        //public string Token { get; set; }
        public string AvatarUrl { get; set; }
        public string ProfileImgUrl { get; set; }
        public DateTime LoginTime { get; set; }
        public IEnumerable<MenuItem> MenuItems { get; set; }

    }
}