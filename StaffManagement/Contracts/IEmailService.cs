using StaffManagement.Models;
using System.Threading.Tasks;

namespace StaffManagement.Contracts
{
    public interface IEmailService
    {
        Task<bool> SendLoginCredential(EmailModel content, string username, string password);
    }
}
