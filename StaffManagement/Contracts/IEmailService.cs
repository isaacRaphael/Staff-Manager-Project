using StaffManagement.Models;
using System.Threading.Tasks;

namespace StaffManagement.Contracts
{
    public interface IEmailService
    {
        Task<bool> SendEmail(EmailModel content, string body);
        Task<bool> SendResetToKen(EmailModel content, string link);
    }
}
