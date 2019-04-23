using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.API_Security
{
    public class SecurityContext : DbContext
    {
        public SecurityContext()
            : base("SecurityDatabase")
        {

        }


        public virtual DbSet<UserAPI_License> UserAPI_Licenses { get; set; }

    }
}
