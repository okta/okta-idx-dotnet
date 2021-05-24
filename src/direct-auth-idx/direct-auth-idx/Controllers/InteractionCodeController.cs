using direct_auth_idx.Models;
using IdentityModel.Client;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace direct_auth_idx.Controllers
{
    public class InteractionCodeController : Controller
    {
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IIdxClient _idxClient;

        public InteractionCodeController(IAuthenticationManager authenticationManager, IIdxClient idxClient)
        {
            this._authenticationManager = authenticationManager;
            this._idxClient = idxClient;
        }

        /// <summary>
        /// This method is called by a 302 browser redirect back to the application from Okta after successful authentication to an external
        /// identity provider.
        /// </summary>
        /// <param name="state">The state handle.</param>
        /// <param name="interaction_code">The interaction code.  This is the value that is exchanged for tokens.</param>
        /// <param name="error">The error if an error occurred.</param>
        /// <param name="error_description">The error description if an error occurred.</param>
        /// <returns></returns>
        public async Task<ActionResult> Callback(string state = null, string interaction_code = null, string error = null, string error_description = null)
        {
            IIdxContext idxContext = Session[state] as IIdxContext;

            if("interaction_required".Equals(error))
            {
                return View("Error", new InteractionCodeErrorViewModel { Error = error, ErrorDescription = "Multifactor Authentication and Social Identity Providers is not currently supported, Authentication failed." });
            }

            if (!string.IsNullOrEmpty(error))
            {
                return View("Error", new InteractionCodeErrorViewModel { Error = error, ErrorDescription = error_description });
            }

            if (string.IsNullOrEmpty(interaction_code))
            {
                return View("Error", new InteractionCodeErrorViewModel { Error = "null_interaction_code", ErrorDescription = "interaction_code was not specified" });
            }

            await RedeemInteractionCodeAndSignInAsync(idxContext, interaction_code);
            return RedirectToAction("Index", "Home");
        }

        private async Task RedeemInteractionCodeAndSignInAsync(IIdxContext idxContext, string interactionCode)
        {
            try
            {
                Okta.Idx.Sdk.TokenResponse tokens = await _idxClient.RedeemInteractionCodeAsync(idxContext, interactionCode);

                if (tokens == null)
                {
                    HttpContext.Response.Redirect("/");
                }
                else
                {
                    ClaimsIdentity claimsIdentity = await GetClaimsIdentityAsync(tokens);
                    _authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, claimsIdentity);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
                HttpContext.Response.Redirect("/");
            }
        }

        private async Task<ClaimsIdentity> GetClaimsIdentityAsync(Okta.Idx.Sdk.TokenResponse tokens)
        {
            List<Claim> userInfoClaims = (await AuthenticationHelper.GetClaimsFromUserInfoAsync(_idxClient.Configuration, tokens.AccessToken)).ToList();

            Claim[] tokenClaims = AuthenticationHelper.GetClaimsFromTokenResponse(tokens);
            userInfoClaims.AddRange(tokenClaims);

            ClaimsIdentity identity = new ClaimsIdentity(userInfoClaims.ToArray(), DefaultAuthenticationTypes.ApplicationCookie);
            return identity;
        }

        private void HandleException(Exception ex)
        {
            // provide exception handling appropriate to your application
            HttpContext.Response.Redirect("/");
        }
    }
}