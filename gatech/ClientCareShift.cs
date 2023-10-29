using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gatech.Models
{
    public class ClientCareShift
    {
        public int CCSId { get; set; }
        public int ClientId { get; set; }
        public int CaregiverId { get; set; }
        public int ShiftId { get; set; }

       
    }
}