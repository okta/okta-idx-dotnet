using Microsoft.AspNet.Identity;
using Okta.Idx.Sdk;
using System.Security.Claims;

namespace embedded_sign_in_widget
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.SessionState;
    using IdentityModel.Client;

    using Okta.Idx.Sdk.Configuration;
    using Okta.Idx.Sdk.Helpers;

    public static class AuthenticationHelper
    {
        public static async Task<ClaimsIdentity> GetIdentityFromTokenResponseAsync(IdxConfiguration configuration, ITokenResponse tokenResponse)
        {
            var claims = await GetClaimsFromUserInfoAsync(configuration, tokenResponse.AccessToken);
            claims = claims.Append(new Claim("access_token", tokenResponse.AccessToken));
            claims = claims.Append(new Claim("id_token", tokenResponse.IdToken));
            if(!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                claims = claims.Append(new Claim("refresh_token", tokenResponse.RefreshToken));
            }            
            ClaimsIdentity identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

            return identity;
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

        public static Task<SignInWidgetConfiguration> StartWidgetSignInAsync(this HttpContextBase httpContext, IIdxClient idxClient, string state = null, CancellationToken cancellationToken = default)
        {
            return StartWidgetSignInAsync(httpContext.Session, idxClient, state);
        }

        public static async Task<SignInWidgetConfiguration> StartWidgetSignInAsync(this HttpSessionStateBase session, IIdxClient idxClient, string state = null, CancellationToken cancellationToken = default)
        {
            IIdxContext idxContext = (IIdxContext)session[state];
            SignInWidgetConfiguration signInWidgetConfiguration = new SignInWidgetConfiguration(idxClient.Configuration, idxContext);
            if (idxContext == null)
            {
                WidgetSignInResponse widgetSignInResponse = await idxClient.StartWidgetSignInAsync(cancellationToken);
                idxContext = widgetSignInResponse.IdxContext;
                signInWidgetConfiguration = widgetSignInResponse.SignInWidgetConfiguration;
            }
            session[idxContext.State] = idxContext;
            return signInWidgetConfiguration;
        }
    }
}