using MainLibrary.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.API_Security
{
    [Table("UserAPI_License")]
    public class UserAPI_License : BasicEntity
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public Guid UserId { get; set; }
        public int ProjectId { get; set; }

        [NotMapped]
        public string Username { get; set; }
        [NotMapped]
        public string Company { get; set; }
        [NotMapped]
        public string FullName { get; set; }
    }
}
