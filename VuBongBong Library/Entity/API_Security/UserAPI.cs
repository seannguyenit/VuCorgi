using MainLibrary.Entity.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.API_Security
{
    public class UserAPI : BasicEntity
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string FullName { get; set; }
        public string Company { get; set; }
        public bool IsActive { get; set; }
    }
}
