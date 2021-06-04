using Microsoft.AspNet.Identity;
using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace embedded_auth_with_sdk.Models
{
    public class AuthenticationService
    {
        public static ClaimsIdentity BuildClaimsIdentity(ITokenResponse tokenInfo, string username = "")
        {
            return new ClaimsIdentity(
                        new[] { new Claim(ClaimTypes.Name, username),
                                new Claim("access_token", tokenInfo.AccessToken),
                                new Claim("id_token", tokenInfo.IdToken),
                                new Claim("refresh_token", tokenInfo.RefreshToken ?? string.Empty),
                                },
                        DefaultAuthenticationTypes.ApplicationCookie);
        }
    }
}