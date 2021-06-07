using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Okta.Idx.Sdk.Helpers
{
    /// <summary>
    /// Static url helper methods relevant to Idx.
    /// </summary>
    public static class IdxUrlHelper
    {
        /// <summary>
        /// Gets a normalized URI string for the specified issuer and resource.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="resourceUri">The relative path of the resource.</param>
        /// <returns>Normalied URI string.</returns>
        public static string GetNormalizedUriString(string issuer, string resourceUri)
        {
            string normalized = issuer;
            if (IsRootOrgIssuer(issuer))
            {
                normalized = Path.Combine(normalized, "oauth2", resourceUri);
            }
            else
            {
                normalized = Path.Combine(normalized, resourceUri);
            }

            normalized = normalized.Replace("\\", "/");

            return normalized;
        }

        /// <summary>
        /// Determines if the specified issuer URI is the root issuer for the organization.
        /// </summary>
        /// <param name="issuerUri">The issuer URI.</param>
        /// <returns>bool</returns>
        public static bool IsRootOrgIssuer(string issuerUri)
        {
            string path = new Uri(issuerUri).AbsolutePath;
            string[] splitUri = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (splitUri.Length >= 2 &&
            "oauth2".Equals(splitUri[0]) &&
            !string.IsNullOrEmpty(splitUri[1]))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Adds a key value parameter to the specified string builder in a query string format.
        /// </summary>
        /// <param name="stringBuilder">The string builder</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <param name="ampersandPrefix">A value indicating whether to append an ampersand before the key value pair.</param>
        public static void AddParameter(StringBuilder stringBuilder, string key, string value, bool ampersandPrefix = false)
        {
            if (ampersandPrefix)
            {
                stringBuilder.Append("&");
            }

            stringBuilder.Append($"{key}={value}");
        }
    }
}
