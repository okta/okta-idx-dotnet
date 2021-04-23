using Microsoft.AspNet.Identity;
using Okta.Idx.Sdk;
using System.Security.Claims;

namespace direct_auth_idx
{
    public static class AuthenticationHelper
    {
        public static ClaimsIdentity GetIdentityFromAuthResponse(string userName, AuthenticationResponse authnResponse) =>
                      new ClaimsIdentity(
                            new[]
                                {
                                    new Claim(ClaimTypes.Name, userName),
                                    new Claim("access_token", authnResponse.TokenInfo.AccessToken),
                                    new Claim("id_token", authnResponse.TokenInfo.IdToken),
                                    new Claim("refresh_token", authnResponse.TokenInfo.RefreshToken ?? string.Empty),
                                 },
                            DefaultAuthenticationTypes.ApplicationCookie);
    }
}