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

        Task<bool> ChangeStaffImage(Staff staff, string photopath);

        Task<bool> UpdateStaff(Staff staff);

        Staff GetStaff(Expression<Func<Staff, bool>> predicate);
        IEnumerable<Staff> GetAllStaff();
    }
}
