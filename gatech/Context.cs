using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace gatech.Models
{
    public class Context : DbContext
    {
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        public DbSet<Client> Clients { get; set; }

        public DbSet<Shift> Shifts { get; set; }

        public DbSet<Caregiver> Caregivers { get; set; }

        public System.Data.Entity.DbSet<gatech.Models.JoinUserRole> JoinUserRoles { get; set; }

      
    }
}