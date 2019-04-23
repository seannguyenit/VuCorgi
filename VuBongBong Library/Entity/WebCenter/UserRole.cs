using MainLibrary.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.WebCenter
{
    [Table("UserRole")]
    public class UserRole : BasicEntity
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }

        public int RoleId { get; set; }

        public bool IsActive { get; set; }
    }
}
