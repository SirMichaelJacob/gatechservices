using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace gatech.Models
{
    public class JoinUserRole
    {
        public int URId { get; set; }
        [Key]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int  RoleId { get; set; }
        public string RoleName { get; set; }

    }
}