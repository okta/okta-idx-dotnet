using FluentAssertions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Okta.Idx.Sdk.UnitTests
{
    public class SignInWidgetConfigurationShould
    {
        [Fact]
        public void Deserialize()
        {
            var signInWidgetJson = @"{
    ""interactionHandle"": ""eyJ6aXAiOiJERUYiLCJhbGlhcyI6ImVuY3J5cHRpb25rZXkiLCJ2ZXIiOiIxIiwib2lkIjoiMDBvM3hxZzBzMkNLN004VXgxZDciLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiZGlyIn0..c7baq8SFzqPZMN8q.wtOwSwpvaLc2xbiIvywbauHCFb8hj4dxmb6Y5NArj82M9Sq5P7n7meCkhhcPgSBN-YJh6hPoGnbHWQxVWfjyamG5enyhjzwzyjx5mxi0q4N_uHVjHykL68iQj3kbHAPJjgJcbV-UgeeXvc18A39HfyVeK3b1o4-4zauknENNHUwMqlX10BPa1F2ydtDhFH6ViAhGUF4wKW1g5AR3QxqjvvvEndx33U506x0IN0bpgwI65rWKedQsRPcqAYV8JJgp6k-vrN1M0pNkXpnUan0mZx3X0wbYqGu9ZcTL-fU-cRBkL2wM1Z4bfyNF7gg1l6LMjZMz1n_JoUGLCwnHsW52JWNdRlj2ttnPSu7OS5M3iKmdMYrv5u7k4jK487lI7p373wZm52yoCl5aZCHM71E_nOV4tHg-rIKGce7FHjI2ci2zxMFsLKmyyYDBTbuqRLKI-HFz4fJu5Wdbihxju9OPAv80ydQ14rrPyEL5MTlcy1Qrl7REYpZNtxCpmyXyJC_CYFzwTOIp4rjjpKqJOBdT05Ga4d9CDRs9NIsLb9QNZxIwUiWk2pFyDxZ0JON_zWTuorJQDHl_69ZORuwAOHOJh88Kd79PEqpcUQ462gkd1HsoF7nhlHQH8oQkJugO73MaiyICcfio8UBENq63DUbemNzQYLKo3VODq8UJFSoQBDA_zN5posT_XbkAEYXXMv5h8ZqpMS22APuNEbs_ILwouDMDqP0lAMH2c77EczjwCmzJnifi2xrjK6_GnKkBEHmRteqb8vPGYQ1qcsmsksyfaa3q5Mt1.xVSXBwFLSxNcL-PYvDrWTw"",
    ""version"": ""5.8.1"",
    ""baseUrl"": ""https://oktaljc.oktapreview.com"",
    ""clientId"": ""0oa738lmiljOyCtEn1d7"",
    ""redirectUri"": ""https://localhost:51936/interactioncode/callback"",
    ""authParams"": {
        ""issuer"": ""https://oktaljc.oktapreview.com"",
        ""scopes"": [
            ""openid"",
            ""profile"",
            ""email"",
            ""offline_access""
        ]
    },
    ""useInteractionCodeFlow"": true,
    ""state"": ""dyR1gauT_RxOP299Bn9q1A"",
    ""codeChallenge"": ""bqKBsjNt4TsV-p9NUcZvKpFN-g5VT_uMHKVy72d6fGI"",
    ""codeChallengeMethod"": ""S256""
}";
            var signInWidgetConfiguratin = JsonConvert.DeserializeObject<SignInWidgetConfiguration>(signInWidgetJson);
            signInWidgetConfiguratin.Should().NotBeNull();
        }
    }
}
