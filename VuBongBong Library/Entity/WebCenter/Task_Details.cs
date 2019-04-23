using MainLibrary.Entity.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainLibrary.Entity.WebCenter
{
    [Table("Task_Details")]
    public class Task_Details
    {
        public Task_Details()
        {
            IsActive = true;
        }

        public int Id { get; set; }
        public int TaskId { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime? ToTime { get; set; }
        public string Note { get; set; }
        public DateTime LastModified { get; set; }
        public Guid EditerId { get; set; }
        public bool IsActive { get; set; }
        [NotMapped]
        public string EditerName { get; set; }
    }
}
