using Okta.Idx.Sdk;
using System.Security.Claims;

namespace embedded_sign_in_widget
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using embedded_sign_in_widget.Models;
    using IdentityModel.Client;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;
    using Okta.Idx.Sdk.Configuration;
    using Okta.Idx.Sdk.Helpers;

    public static class AuthenticationHelper
    {
        public const string IdxStateKey = "IdxStateKey";
        public const string SiwConfigKey = "siwConfig";

        public static async Task<ClaimsIdentity> GetIdentityFromTokenResponseAsync(IdxConfiguration configuration, ITokenResponse tokenResponse)
        {
            var claims = await GetClaimsFromUserInfoAsync(configuration, tokenResponse.AccessToken);
            claims = claims.Append(new Claim("access_token", tokenResponse.AccessToken));
            claims = claims.Append(new Claim("id_token", tokenResponse.IdToken));
            if (!string.IsNullOrEmpty(tokenResponse.RefreshToken))
            {
                claims = claims.Append(new Claim("refresh_token", tokenResponse.RefreshToken));
            }
            ClaimsIdentity identity = new ClaimsIdentity(claims, "ApplicationCookie");

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

        public static async Task<SignInWidgetConfiguration> StartWidgetSignInAsync(this HttpContext httpContext, IIdxClient idxClient, string? state = null, CancellationToken cancellationToken = default)
        {
            IdxContext? idxContext = await httpContext.GetIdxContextAsync(state);
            SignInWidgetConfiguration? signInWidgetConfiguration = new SignInWidgetConfiguration(idxClient.Configuration, idxContext);
            if (idxContext == null)
            {
                WidgetSignInResponse widgetSignInResponse = await idxClient.StartWidgetSignInAsync(cancellationToken);
                idxContext = (IdxContext)widgetSignInResponse.IdxContext;
                signInWidgetConfiguration = widgetSignInResponse.SignInWidgetConfiguration;
                await SetIdxContextAsync(httpContext, idxContext);
            }
            await httpContext.SetSignInWidgetConfiguration(signInWidgetConfiguration);
            return signInWidgetConfiguration;
        }

        public static async Task<IdxContext?> GetIdxContextAsync(this HttpContext httpContext, string? state = null)
        {
            return await GetIdxContextAsync(httpContext.Session, state);
        }

        public static async Task<IdxContext?> GetIdxContextAsync(this ISession session, string? state = null)
        {
            string? idxContextJson;
            IdxContext? idxContext = null;
            if(!string.IsNullOrEmpty(state))
            {
                idxContextJson = session.GetString(state);
                if (!string.IsNullOrEmpty(idxContextJson))
                {
                    idxContext = JsonConvert.DeserializeObject<IdxContextModel>(idxContextJson);
                }
            }
            return idxContext;
        }

        public static async Task SetIdxContextAsync(this HttpContext httpContext, IdxContext idxContext)
        {
            await SetIdxContextAsync(httpContext.Session, idxContext);
        }

        public static async Task SetIdxContextAsync(this ISession session, IdxContext idxContext)
        {
            session.SetString(idxContext.State, JsonConvert.SerializeObject(idxContext));
            session.SetString(IdxStateKey, idxContext.State);
        }

        public static async Task<SignInWidgetConfiguration> GetSignInWidgetConfigurationAsync(this HttpContext httpContext)
        {
            string? signInWidgetJson = httpContext.Session.GetString(SiwConfigKey);
            if (!string.IsNullOrEmpty(signInWidgetJson))
            {
                return JsonConvert.DeserializeObject<SignInWidgetConfiguration>(signInWidgetJson);
            }
            return null;
        }

        public static async Task SetSignInWidgetConfiguration(this HttpContext httpContext, SignInWidgetConfiguration signInWidgetConfiguration)
        {
            var session = httpContext.Session;
            session.SetString(SiwConfigKey, JsonConvert.SerializeObject(signInWidgetConfiguration));
        }
    }
}