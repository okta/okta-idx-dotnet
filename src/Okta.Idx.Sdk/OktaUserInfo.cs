// <copyright file="OktaUserInfo.cs" company="Okta, Inc">
// Copyright (c) 2020 - present Okta, Inc. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.
// </copyright>

using Newtonsoft.Json;

namespace Okta.Idx.Sdk
{
    public class OktaUserInfo
    {
        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        [JsonProperty("sub")]
        public string Sub { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        [JsonProperty("locale")]
        public string Locale { get; set; }

        /// <summary>
        /// Gets or sets the preferred username.
        /// </summary>
        [JsonProperty("preferred_username")]
        public string PreferredUserName { get; set; }

        /// <summary>
        /// Gets or sets the given name.
        /// </summary>
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        /// <summary>
        /// Gets or sets the family name.
        /// </summary>
        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        /// <summary>
        /// Gets or sets the zone info.
        /// </summary>
        [JsonProperty("zoneinfo")]
        public string ZoneInfo { get; set; }

        /// <summary>
        /// Gets or sets the update at value.
        /// </summary>
        [JsonProperty("updated_at")]
        public int UpdatedAt { get; set; }
    }
}
