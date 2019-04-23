using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.WebCenter
{
    [Table("User")]
    [Serializable]
    public class User
    {
        public User()
        {
            IsActive = true;
        }

        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsAdmin { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Slogan { get; set; }
        public string Company { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public DateTime? DOB { get; set; }
        public string CreateToken { get; set; }
        public DateTime? CreatedDate { get; set; }

        [NotMapped]
        public int? RoleId { get; set; }
        [NotMapped]
        public string RoleName { get; set; }
        [NotMapped]
        public string Token { get; set; }
        [NotMapped]
        public DateTime Session_Start { get; set; }
        [NotMapped]
        public DateTime Session_End { get; set; }
    }
}
