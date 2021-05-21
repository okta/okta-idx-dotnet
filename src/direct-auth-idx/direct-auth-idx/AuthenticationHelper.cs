using Microsoft.AspNet.Identity;
using Okta.Idx.Sdk;
using System.Security.Claims;

namespace direct_auth_idx
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;

    using IdentityModel.Client;

    using Okta.Idx.Sdk.Configuration;
    using Okta.Idx.Sdk.Helpers;

    public static class AuthenticationHelper
    {
        public static async Task<ClaimsIdentity> GetIdentityFromAuthResponseAsync(IdxConfiguration configuration, AuthenticationResponse authnResponse)
        {
            var claims = await GetClaimsFromUserInfoAsync(configuration, authnResponse.TokenInfo.AccessToken);
            claims = claims.Append(new Claim("access_token", authnResponse.TokenInfo.AccessToken));
            claims = claims.Append(new Claim("id_token", authnResponse.TokenInfo.IdToken));
            ClaimsIdentity identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            return identity;
        }

        public static Claim[] GetClaimsFromTokenResponse(ITokenResponse tokenResponse)
        {
            return new[]
            {
                new Claim("access_token", tokenResponse.AccessToken),
                new Claim("id_token", tokenResponse.IdToken),
                new Claim("refresh_token", tokenResponse.RefreshToken ?? string.Empty),
            };
        }

        public static async Task<IEnumerable<Claim>> GetClaimsFromUserInfoAsync(IdxConfiguration configuration, string accessToken)
        { 
            Uri userInfoUri = new Uri(IdxUrlHelper.GetNormalizedUriString(configuration.Issuer, "v1/userinfo"));
            HttpClient httpClient = new HttpClient();
            var userInfoResponse = await httpClient.GetUserInfoAsync(new UserInfoRequest
                                                                         {
                                                                             Address = userInfoUri.ToString(),
                                                                             Token = accessToken,
                                                                         }).ConfigureAwait(false);
            var nameClaim = new Claim(
                ClaimTypes.Name,
                userInfoResponse.Claims.FirstOrDefault(x => x.Type == "name")?.Value);
            return userInfoResponse.Claims.Append(nameClaim);
        }
    }
}