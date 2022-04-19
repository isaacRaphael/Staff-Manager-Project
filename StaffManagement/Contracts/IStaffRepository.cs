using StaffManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace StaffManagement.Contracts
{
    public interface IStaffRepository
    {
        bool AddStaff(Staff staff);
        bool RemoveStaff(int id);
        Staff GetStaff(Expression<Func<Staff, bool>> predicate);
        IEnumerable<Staff> GetAllStaff();
        Staff GetTheStaff(string Id);
    }
}
