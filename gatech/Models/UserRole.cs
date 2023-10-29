using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace gatech.Models
{
    [Table("UserRole")]
    public class UserRole
    {        
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        //[Key, Column(Order = 1)]
        public int RoleId { get; set; }
    }
}