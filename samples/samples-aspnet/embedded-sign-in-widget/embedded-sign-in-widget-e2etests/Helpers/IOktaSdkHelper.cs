using Okta.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace embedded_sign_in_widget_e2etests.Helpers
{
    public interface IOktaSdkHelper
    {
        Task<IUser> CreateActiveUser(string userName, string firstName, string password);
        Task AddUserToGroup(IUser user, string groupName);
        Task DeleteUserAsync(string email);
        Task<IUserFactor> EnrollPhoneFactor(string emailAddress, string phoneNumber);
        Task ActivateFactor(IUserFactor factor, string emailAddress, string passCode);
    }
}
