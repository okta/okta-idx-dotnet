using Newtonsoft.Json;
using Okta.Idx.Sdk.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class SignInWidgetConfiguration
    {
        public const string DefaultVersion = "5.5.2";

        public SignInWidgetConfiguration(IdxConfiguration idxConfiguration, IIdxContext idxContext, string version = DefaultVersion)
        {
            this.UseInteractionCodeFlow = true;
            this.Version = version ?? DefaultVersion;

            this.BaseUrl = idxConfiguration?.Issuer?.Split(new string[] { "/oauth2" }, StringSplitOptions.RemoveEmptyEntries)[0];
            this.ClientId = idxConfiguration.ClientId;
            this.RedirectUri = idxConfiguration.RedirectUri;
            this.AuthParams = new SignInWidgetAuthParams(idxConfiguration);

            this.InteractionHandle = idxContext.InteractionHandle;
            this.State = idxContext.State;
            this.CodeChallenge = idxContext.CodeChallenge;
            this.CodeChallengeMethod = idxContext.CodeChallengeMethod;
        }

        [JsonProperty("interactionHandle")]
        public string InteractionHandle { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("baseUrl")]
        public string BaseUrl { get; set; }

        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("redirectUri")]
        public string RedirectUri { get; set; }

        [JsonProperty("authParams")]
        public SignInWidgetAuthParams AuthParams { get; set; }

        [JsonProperty("useInteractionCodeFlow")]
        public bool UseInteractionCodeFlow { get; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("codeChallenge")]
        public string CodeChallenge { get; set; }

        [JsonProperty("codeChallengeMethod")]
        public string CodeChallengeMethod { get; set; }
    }
}
