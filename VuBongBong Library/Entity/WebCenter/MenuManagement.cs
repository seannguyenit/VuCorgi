using MainLibrary.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.WebCenter
{
    [Table("MenuManagement")]
    public class MenuManagement : BasicEntity
    {
        public int Id { get; set; }

        public int RoleId { get; set; }

        public int MenuItemId { get; set; }

        public bool IsActive { get; set; }

        [NotMapped]
        public MenuItem MenuItem { get; set; }

    }
}
