using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Sdk.Abstractions
{
    public static class UrlHelper
    {
        /// <summary>
        /// Gets the Okta domain given an issuer.
        /// </summary>
        /// <param name="issuer">The issuer, for example, "https://test-org.oktapreview.com/oauth2/default".</param>
        /// <returns>The Okta domain, for example, "https://test-org.oktapreview.com".</returns>
        public static string GetOktaDomain(string issuer)
        {
            if (string.IsNullOrEmpty(issuer))
            {
                throw new ArgumentNullException(nameof(issuer));
            }

            Uri uri = new Uri(issuer);

            return $"{uri.Scheme}://{uri.Host}";
        }

        /// <summary>
        /// Ensures that this URI ends with a trailing slash <c>/</c>.
        /// </summary>
        /// <param name="uri">The URI string.</param>
        /// <returns>The URI string, appended with <c>/</c> if necessary.</returns>
        public static string EnsureTrailingSlash(string uri)
        {
            if (string.IsNullOrEmpty(uri))
            {
                throw new ArgumentNullException(nameof(uri));
            }

            return uri.EndsWith("/")
                ? uri
                : $"{uri}/";
        }
    }
}
