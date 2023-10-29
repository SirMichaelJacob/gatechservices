using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gatech.Models
{
    public class Task
    {
        public int TaskId { get; set; }
        public int ClientId { get; set; }
        public string Description { get; set; }
        public bool IsCompleted { get; set; }
        public List<Medication> Medications { get; set; }
        public Client Client { get; set; }
    }
}