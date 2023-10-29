using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace gatech.Models
{
    [Table("Shift")]
    public class Shift
    {
        public int ShiftId { get; set; }
        public string ShiftName { get; set; }
        public DateTime ShiftDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int ClientId { get; set; }
        public int CaregiverId { get; set; }
    }
}