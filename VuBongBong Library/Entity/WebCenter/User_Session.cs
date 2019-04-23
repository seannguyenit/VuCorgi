using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.WebCenter
{
    [Table("User_Session")]
    public class User_Session
    {
        [Key]
        public Guid UserId { get; set; }
        public string User_Token { get; set; }
        public DateTime User_Start { get; set; }
        public DateTime User_End { get; set; }

        public User_Session()
        {
            User_Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
}
