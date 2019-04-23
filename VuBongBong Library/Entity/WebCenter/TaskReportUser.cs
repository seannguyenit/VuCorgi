using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.WebCenter
{
    public class TaskReportUser
    {
        public string Date { get; set; }
        public int TotalTask { get; set; }
        public int TotalNotFinished { get; set; }
        public int TotalFinished { get; set; }
        public decimal MaximumBonus { get; set; }
        public decimal RealBonus { get; set; }
    }
}
