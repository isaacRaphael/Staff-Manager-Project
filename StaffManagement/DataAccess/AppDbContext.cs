using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StaffManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace StaffManagement.DataAccess
{
    public class AppDbContext : IdentityDbContext<Staff>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Staff> Staffs { get; set; }
    }
}
