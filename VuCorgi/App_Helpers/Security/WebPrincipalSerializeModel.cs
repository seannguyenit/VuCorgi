using MainLibrary.Entity.WebCenter;
using System;
using System.Collections.Generic;

namespace VuBongBongWeb.Security
{
    public class WebPrincipalSerializeModel
    {
        public User UserDetail { get; set; }
        public IEnumerable<MenuItem> MenuItems { get; set; }

        //public int RoleId { get; set; }
        //public string RoleName { get; set; }
        //public string Token { get; set; }
        public string AvatarUrl { get; set; }
        public string ProfileImgUrl { get; set; }
        public DateTime LoginTime { get; set; }
        //public Guid EmployeeId { get; set; }
    }
}