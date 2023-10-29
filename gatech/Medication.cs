using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gatech.Models
{
    public class Medication
    {
        public int MedicationId { get; set; }
        public int TaskId { get; set; }
        public string Name { get; set; }
        public string Dosage { get; set; }
        public Task Task { get; set; }
    }
}