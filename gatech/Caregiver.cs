using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace gatech.Models
{
    [Table("Caregiver")]
    public class Caregiver
    {
        public int CaregiverId { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string  Gender { get; set; }
        public string ContactNumber { get; set; }
        public string Address { get; set; }
    }
}