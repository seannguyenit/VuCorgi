using MainLibrary.Entity.Base;
using MainLibrary.Resource.WebCenter;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainLibrary.Entity.WebCenter
{
    [Table("Task")]
    public class TaskWeb : BasicEntity
    {
        public TaskWeb()
        {
            IsActive = true;
            IsUrgent = false;
            Order = 0;
            ListDetails = new List<Task_Details>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsUrgent { get; set; }
        public int TypeId { get; set; }
        public int Order { get; set; }
        public decimal? Cost { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime? Estimate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? FinishTime { get; set; }
        public Guid? TagetUserId { get; set; }
        public Guid? Executor { get; set; }
       
        public int? Pin { get; set; }
        [NotMapped]
        public string TypeName { get; set; }
        [NotMapped]
        public string TagetUserName { get; set; }
        [NotMapped]
        public string Status
        {
            get
            {
                if (StatusId.HasValue)
                    return Enum.GetName(typeof(TaskStatus), StatusId.Value);
                else
                    return TaskStatus.New.ToString();
            }
        }
        [NotMapped]
        public int? StatusId { get; set; }
        [NotMapped]
        public int Duration { get; set; }
        [NotMapped]
        public string TrCss
        {
            get
            {
                if (Status == TaskStatus.Working.ToString())
                    return "info";
                else if (Status == TaskStatus.Pending.ToString())
                    return "warning";
                else if (Status == TaskStatus.Finished.ToString())
                    return "success";
                else if (Status == TaskStatus.Cancel.ToString())
                    return "danger";
                else
                    return null;
            }
        }

        [NotMapped]
        public List<Task_Details> ListDetails { get; set; }
        [NotMapped]
        public string ExecutorName { get; set; }
    }
}
