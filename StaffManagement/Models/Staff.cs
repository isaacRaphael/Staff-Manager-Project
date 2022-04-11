using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StaffManagement.Models
{
    public class Staff : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public override string UserName { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }

        public string PhotoPath { get; set; }

        //public int ImageNumber { get => new Random().Next(); }


    }
}



