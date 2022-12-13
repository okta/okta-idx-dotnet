using Okta.Sdk;
using System.Linq;
using System.Threading.Tasks;

namespace embedded_auth_with_sdk
{
    public static class OktaSdkHelper
    {

        public static async Task<string> ForgotPasswordGenerateToken(string email)
        {
            try
            {
                var client = new OktaClient();
                var user = await client.Users.GetUserAsync(email);

                var resetPassResponse = await user.ForgotPasswordGenerateOneTimeTokenAsync(false).ConfigureAwait(false);

                return resetPassResponse?.ResetPasswordUrl?.Split('/').Last();
            }
            catch 
            {
                return null;
            }
        }
    }
}
