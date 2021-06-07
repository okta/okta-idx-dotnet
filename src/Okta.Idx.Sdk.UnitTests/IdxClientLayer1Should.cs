using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Okta.Sdk.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
namespace Okta.Idx.Sdk.UnitTests
{
    public class IdxClientLayer1Should
    {
        [Fact]
        public async Task GeneratePkcePropsWhenCallingInteract()
        {
            var rawResponse = @"{ 'interaction_handle' : 'foo' }";
            var mockRequestExecutor = new MockedStringRequestExecutor(rawResponse);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var idxContext = await testClient.InteractAsync();

            idxContext.InteractionHandle.Should().Be("foo");
            idxContext.CodeChallenge.Should().NotBeNullOrEmpty();
            idxContext.CodeChallengeMethod.Should().NotBeNullOrEmpty();
            idxContext.CodeVerifier.Should().NotBeNullOrEmpty();
            idxContext.State.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task UseProvidedStateWhenCallingInteract()
        {
            var rawResponse = @"{ 'interaction_handle' : 'foo' }";
            var mockRequestExecutor = new MockedStringRequestExecutor(rawResponse);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var idxContext = await testClient.InteractAsync("bar");

            idxContext.State.Should().Be("bar");
        }

        [Fact]
        public async Task SendPkceCodeChallengeWhenExchangingTokens()
        {
            #region rawSuccessResponse
            var rawSuccessResponse = @"{
                                       ""stateHandle"":""02gXmcy1YH8qk6GcefMB447eZuL6EOxTiaSj-N6-80"",
                                       ""version"":""1.0.0"",
                                       ""expiresAt"":""2020-12-10T17:13:12.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""successWithInteractionCode"":{
                                          ""rel"":[
                                             ""create-form""
                                          ],
                                          ""name"":""issue"",
                                          ""href"":""https://test.com/oauth2/v1/token"",
                                          ""method"":""POST"",
                                          ""value"":[
                                             {
                                                ""name"":""grant_type"",
                                                ""required"":true,
                                                ""value"":""interaction_code""
                                             },
                                             {
                                                ""name"":""interaction_code"",
                                                ""required"":true,
                                                ""value"":""TmcFc9MCxmVXGeYTW0pSE7NhvDxdMP8MUlazrmhkIGk""
                                             },
                                             {
                                                ""name"":""client_id"",
                                                ""required"":true,
                                                ""value"":""0oazsmpxZpVEg4chS2o4""
                                             },
                                             {
                                                ""name"":""client_secret"",
                                                ""required"":true
                                             },
                                             {
                                                ""name"":""code_verifier"",
                                                ""required"":true
                                             }
                                          ],
                                          ""accepts"":""application/x-www-form-urlencoded""
                                       }
                                    }";
            #endregion

            var rawResponse = @"{""token_type"":""Bearer"",""expires_in"":3600,""access_token"":""foo"",""scope"":""openid profile"",""id_token"":""bar""}";
            var mockRequestExecutor = new MockedStringRequestExecutor(rawResponse);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            // Create a mock for an success response
            var resourceFactory = new ResourceFactory(testClient, NullLogger.Instance, new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource))));
            var data = new DefaultSerializer().Deserialize(rawSuccessResponse);
            var successResponse = resourceFactory.CreateNew<IdxResponse>(data);

            var mockIdxContext = new IdxContext("codeVer1f13r", "codeChanll3ng3", "S256", "foo", "bar");

            var tokens = await successResponse.SuccessWithInteractionCode.ExchangeCodeAsync(mockIdxContext);
            mockRequestExecutor.ReceivedBody.Should().Contain($"\"code_verifier\":\"{mockIdxContext.CodeVerifier}\"");
        }

        [Fact]
        public void ProcessRecoverResponse()
        {
            #region rawResponse
            var rawResponse = @"
                                {
                                   ""stateHandle"":""02sRsXE-KaQW3sigxQl-0WYEI1clG96hHHL1Vg90VB"",
                                   ""version"":""1.0.0"",
                                   ""expiresAt"":""2021-02-11T22:26:19.000Z"",
                                   ""intent"":""LOGIN"",
                                   ""remediation"":{
                                      ""type"":""array"",
                                      ""value"":[
                                         {
                                            ""rel"":[
                                               ""create-form""
                                            ],
                                            ""name"":""challenge-authenticator"",
                                            ""relatesTo"":[
                                               ""$.currentAuthenticatorEnrollment""
                                            ],
                                            ""href"":""https://foo.okta.com/idp/idx/challenge/answer"",
                                            ""method"":""POST"",
                                            ""produces"":""application/ion+json; okta-version=1.0.0"",
                                            ""value"":[
                                               {
                                                  ""name"":""credentials"",
                                                  ""type"":""object"",
                                                  ""form"":{
                                                     ""value"":[
                                                        {
                                                           ""name"":""passcode"",
                                                           ""label"":""Password"",
                                                           ""secret"":true
                                                        }
                                                     ]
                                                  },
                                                  ""required"":true
                                               },
                                               {
                                                  ""name"":""stateHandle"",
                                                  ""required"":true,
                                                  ""value"":""02sRsXE"",
                                                  ""visible"":false,
                                                  ""mutable"":false
                                               }
                                            ],
                                            ""accepts"":""application/json; okta-version=1.0.0""
                                         }
                                      ]
                                   },
                                   ""currentAuthenticatorEnrollment"":{
                                      ""type"":""object"",
                                      ""value"":{
                                         ""recover"":{
                                            ""rel"":[
                                               ""create-form""
                                            ],
                                            ""name"":""recover"",
                                            ""href"":""https://foo.okta.com/idp/idx/recover"",
                                            ""method"":""POST"",
                                            ""produces"":""application/ion+json; okta-version=1.0.0"",
                                            ""value"":[
                                               {
                                                  ""name"":""stateHandle"",
                                                  ""required"":true,
                                                  ""value"":""02sRsXE-"",
                                                  ""visible"":false,
                                                  ""mutable"":false
                                               }
                                            ],
                                            ""accepts"":""application/json; okta-version=1.0.0""
                                         },
                                         ""type"":""password"",
                                         ""key"":""okta_password"",
                                         ""id"":""lae8uz"",
                                         ""displayName"":""Password"",
                                         ""methods"":[
                                            {
                                               ""type"":""password""
                                            }
                                         ]
                                      }
                                   },
                                   ""authenticators"":{
                                      ""type"":""array"",
                                      ""value"":[
                                         {
                                            ""type"":""password"",
                                            ""key"":""okta_password"",
                                            ""id"":""aut3jya5"",
                                            ""displayName"":""Password"",
                                            ""methods"":[
                                               {
                                                  ""type"":""password""
                                               }
                                            ]
                                         }
                                      ]
                                   },
                                   ""authenticatorEnrollments"":{
                                      ""type"":""array"",
                                      ""value"":[
                                         {
                                            ""type"":""password"",
                                            ""key"":""okta_password"",
                                            ""id"":""lae8u"",
                                            ""displayName"":""Password"",
                                            ""methods"":[
                                               {
                                                  ""type"":""password""
                                               }
                                            ]
                                         }
                                      ]
                                   },
                                   ""user"":{
                                      ""type"":""object"",
                                      ""value"":{
                                         ""id"":""00u3jxoh2""
                                      }
                                   },
                                   ""cancel"":{
                                      ""rel"":[
                                         ""create-form""
                                      ],
                                      ""name"":""cancel"",
                                      ""href"":""https://foo.okta.com/idp/idx/cancel"",
                                      ""method"":""POST"",
                                      ""produces"":""application/ion+json; okta-version=1.0.0"",
                                      ""value"":[
                                         {
                                            ""name"":""stateHandle"",
                                            ""required"":true,
                                            ""value"":""02sRsXE-"",
                                            ""visible"":false,
                                            ""mutable"":false
                                         }
                                      ],
                                      ""accepts"":""application/json; okta-version=1.0.0""
                                   },
                                   ""app"":{
                                      ""type"":""object"",
                                      ""value"":{
                                         ""name"":""oidc_client"",
                                         ""label"":""IDX Backend SDK's"",
                                         ""id"":""0oa3j""
                                      }
                                   }
                                }";
            #endregion

            var mockRequestExecutor = new MockedStringRequestExecutor(rawResponse);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            // Create a mock for a response with recover
            var resourceFactory = new ResourceFactory(testClient, NullLogger.Instance, new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource))));
            var data = new DefaultSerializer().Deserialize(rawResponse);
            var recoverResponse = resourceFactory.CreateNew<IdxResponse>(data);

            recoverResponse.CurrentAuthenticatorEnrollment.Type.Should().Be("object");
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Recover.Name.Should().Be("recover");
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Recover.Rel.Should().Contain("create-form");
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Recover.Href.Should().Be("https://foo.okta.com/idp/idx/recover");
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Recover.Method.Should().Be("POST");
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Recover.Accepts.Should().Be("application/json; okta-version=1.0.0");
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Recover.Form.FirstOrDefault().Name.Should().Be("stateHandle");
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Recover.Form.FirstOrDefault().Required.Should().BeTrue();
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Recover.Form.FirstOrDefault().GetProperty<string>("value").Should().NotBeNullOrEmpty();
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Recover.Form.FirstOrDefault().Visible.Should().BeFalse();
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Recover.Form.FirstOrDefault().Mutable.Should().BeFalse();
            recoverResponse.CurrentAuthenticatorEnrollment.Value.DisplayName.Should().Be("Password");
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Key.Should().Be("okta_password");
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Id.Should().NotBeNullOrEmpty();
            recoverResponse.CurrentAuthenticatorEnrollment.Value.Methods.FirstOrDefault().Type.Should().Be(AuthenticatorMethodType.Password);
        }

        [Fact]
        public async Task ProcessExchangingTokensResponse()
        {
            #region rawSuccessResponse
            var rawSuccessResponse = @"{
                                       ""stateHandle"":""02gXmcy1YH8qk6GcefMB447eZuL6EOxTiaSj-N6-80"",
                                       ""version"":""1.0.0"",
                                       ""expiresAt"":""2020-12-10T17:13:12.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""successWithInteractionCode"":{
                                          ""rel"":[
                                             ""create-form""
                                          ],
                                          ""name"":""issue"",
                                          ""href"":""https://test.com/oauth2/v1/token"",
                                          ""method"":""POST"",
                                          ""value"":[
                                             {
                                                ""name"":""grant_type"",
                                                ""required"":true,
                                                ""value"":""interaction_code""
                                             },
                                             {
                                                ""name"":""interaction_code"",
                                                ""required"":true,
                                                ""value"":""TmcFc9MCxmVXGeYTW0pSE7NhvDxdMP8MUlazrmhkIGk""
                                             },
                                             {
                                                ""name"":""client_id"",
                                                ""required"":true,
                                                ""value"":""0oazsmpxZpVEg4chS2o4""
                                             },
                                             {
                                                ""name"":""client_secret"",
                                                ""required"":true
                                             },
                                             {
                                                ""name"":""code_verifier"",
                                                ""required"":true
                                             }
                                          ],
                                          ""accepts"":""application/x-www-form-urlencoded""
                                       }
                                    }";
            #endregion

            var rawResponse = @"{""token_type"":""Bearer"",""expires_in"":3600,""access_token"":""foo"",""scope"":""openid profile"",""id_token"":""bar""}";
            var mockRequestExecutor = new MockedStringRequestExecutor(rawResponse);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            // Create a mock for an success response
            var resourceFactory = new ResourceFactory(testClient, NullLogger.Instance, new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource))));
            var data = new DefaultSerializer().Deserialize(rawSuccessResponse);
            var successResponse = resourceFactory.CreateNew<IdxResponse>(data);

            var mockIdxContext = new IdxContext("codeVer1f13r", "codeChanll3ng3", "S256", "foo", "bar");

            var tokens = await successResponse.SuccessWithInteractionCode.ExchangeCodeAsync(mockIdxContext);

            tokens.TokenType.Should().Be("Bearer");
            tokens.IdToken.Should().Be("bar");
            tokens.AccessToken.Should().Be("foo");
            tokens.ExpiresIn.Should().Be(3600);
        }

        [Fact]
        public async Task ProcessInfoMessageResponse()
        {
            #region Raw response with a message
            var rawResponse = @"{
                  ""version"": ""1.0.0"",
                  ""stateHandle"": ""02BgAmhAISKk6KZ4fsP-jNTKYqpSMipWcdT6Nw5JZH"",
                  ""expiresAt"": ""2021 -05-18T18:59:56.000Z"",
                  ""intent"": ""LOGIN"",
                  ""messages"": {
                                ""type"": ""array"",
                    ""value"": [
                      {
                                    ""message"": ""To finish signing in, check your email."",
                        ""i18n"": {
                                        ""key"": ""idx.email.verification.required""
                        },
                        ""class"": ""INFO""
                      }
                    ]
                  }
                }";
            #endregion Raw response with a message

            // Response with a message to user
            var mockRequestExecutor = new MockedStringRequestExecutor(rawResponse);
            var testClient = new TesteableIdxClient(mockRequestExecutor);
            var resourceFactory = new ResourceFactory(testClient, NullLogger.Instance, new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource))));
            var data = new DefaultSerializer().Deserialize(rawResponse);
            var responseWithMessage = resourceFactory.CreateNew<IdxResponse>(data);

            responseWithMessage.IdxMessages.Messages.Should().HaveCount(1);
            responseWithMessage.IdxMessages.Messages.First().Class.Should().Be("INFO");
            responseWithMessage.IdxMessages.Messages.First().Text.Should().Be("To finish signing in, check your email.");

            // Response with no messages
            var emptyData = new DefaultSerializer().Deserialize("{}");
            var responseWithNoMessages = resourceFactory.CreateNew<IdxResponse>(emptyData);

            responseWithNoMessages.IdxMessages.Should().BeNull();
        }

        [Fact]
        // STEP 1
        public async Task ProcessIntrospectResponse()
        {
            #region raw response

            var rawResponse = @"
                                {
                                    ""stateHandle"": ""021iDhVt8b_iJouYV-MBpEshNgBNbVui06Uhn_8v63"",
                                    ""version"": ""1.0.0"",
                                    ""expiresAt"": ""2020-10-16T16:56:45.000Z"",
                                    ""intent"": ""LOGIN"",
                                    ""remediation"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""identify"",
                                                ""href"": ""https://foo.okta.com/idp/idx/identify"",
                                                ""method"": ""POST"",
                                                ""value"": [
                                                    {
                                                        ""name"": ""identifier"",
                                                        ""label"": ""Username""
                                                    },
                                                    {
                                                        ""name"": ""rememberMe"",
                                                        ""type"": ""boolean"",
                                                        ""label"": ""Remember this device""
                                                    },
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""021iDhVt8b_iJouYV-MBpEshNgBNbVui06Uhn_8v63"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            }
                                        ]
                                    },
                                    ""cancel"": {
                                        ""rel"": [
                                            ""create-form""
                                        ],
                                        ""name"": ""cancel"",
                                        ""href"": ""https://foo.okta.com/idp/idx/cancel"",
                                        ""method"": ""POST"",
                                        ""value"": [
                                            {
                                                ""name"": ""stateHandle"",
                                                ""required"": true,
                                                ""value"": ""021iDhVt8b_iJouYV-MBpEshNgBNbVui06Uhn_8v63"",
                                                ""visible"": false,
                                                ""mutable"": false
                                            }
                                        ],
                                        ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                    },
                                    ""app"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""name"": ""okta_enduser"",
                                            ""label"": ""okta_enduser"",
                                            ""id"": ""DEFAULT_APP""
                                        }
                                    }
                                }
            ";

            #endregion

            var mockRequestExecutor = new MockedStringRequestExecutor(rawResponse);
            var testClient = new TesteableIdxClient(mockRequestExecutor);
            var mockIdxContext = new IdxContext("foo", "bar", "baz", "qux", "quux");


            var response = await testClient.IntrospectAsync(mockIdxContext);
            response.StateHandle.Should().NotBeNullOrEmpty();
            response.Version.Should().NotBeNullOrEmpty();
            response.ExpiresAt.Value.Should().Be(DateTimeOffset.Parse("2020-10-16T16:56:45.000Z"));
            response.Intent.Should().Be("LOGIN");

            response.Remediation.Should().NotBeNull();
            response.Remediation.GetRaw().Should().NotBeNullOrEmpty();
            response.Remediation.Type.Should().Be("array");

            response.Remediation.RemediationOptions.Should().NotBeNullOrEmpty();
            response.Remediation.RemediationOptions.FirstOrDefault().Rel.Should().Contain("create-form");
            response.Remediation.RemediationOptions.FirstOrDefault().Name.Should().Be("identify");
            response.Remediation.RemediationOptions.FirstOrDefault().Href.Should().Be("https://foo.okta.com/idp/idx/identify");
            response.Remediation.RemediationOptions.FirstOrDefault().Method.Should().Be("POST");
            response.Remediation.RemediationOptions.FirstOrDefault().Accepts.Should().Be("application/ion+json; okta-version=1.0.0");

            response.Remediation.RemediationOptions.FirstOrDefault().Form.Should().NotBeNullOrEmpty();
            var stateHandleFormItem = response.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "stateHandle");
            stateHandleFormItem.Should().NotBeNull();
            stateHandleFormItem.Required.Should().BeTrue();
            stateHandleFormItem.GetProperty<string>("value").Should().Be("021iDhVt8b_iJouYV-MBpEshNgBNbVui06Uhn_8v63");
            stateHandleFormItem.Visible.Should().BeFalse();
            stateHandleFormItem.Mutable.Should().BeFalse();

            var identifierFormItem = response.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "identifier");
            identifierFormItem.Should().NotBeNull();
            identifierFormItem.Label.Should().Be("Username");

            var rememberMeFormItem = response.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "rememberMe");
            rememberMeFormItem.Should().NotBeNull();
            rememberMeFormItem.Label.Should().Be("Remember this device");
            rememberMeFormItem.Type.Should().Be("boolean");
        }

        [Fact]
        // STEP 2
        public async Task ProcessIdentifyResponse()
        {
            #region raw response

            var rawIntrospectResponse = @"
                                {
                                    ""stateHandle"": ""021iDhVt8b_iJouYV-MBpEshNgBNbVui06Uhn_8v63"",
                                    ""version"": ""1.0.0"",
                                    ""expiresAt"": ""2020-10-16T16:56:45.000Z"",
                                    ""intent"": ""LOGIN"",
                                    ""remediation"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""identify"",
                                                ""href"": ""https://foo.okta.com/idp/idx/identify"",
                                                ""method"": ""POST"",
                                                ""value"": [
                                                    {
                                                        ""name"": ""identifier"",
                                                        ""label"": ""Username""
                                                    },
                                                    {
                                                        ""name"": ""rememberMe"",
                                                        ""type"": ""boolean"",
                                                        ""label"": ""Remember this device""
                                                    },
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""021iDhVt8b_iJouYV-MBpEshNgBNbVui06Uhn_8v63"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            }
                                        ]
                                    },
                                    ""cancel"": {
                                        ""rel"": [
                                            ""create-form""
                                        ],
                                        ""name"": ""cancel"",
                                        ""href"": ""https://foo.okta.com/idp/idx/cancel"",
                                        ""method"": ""POST"",
                                        ""value"": [
                                            {
                                                ""name"": ""stateHandle"",
                                                ""required"": true,
                                                ""value"": ""021iDhVt8b_iJouYV-MBpEshNgBNbVui06Uhn_8v63"",
                                                ""visible"": false,
                                                ""mutable"": false
                                            }
                                        ],
                                        ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                    },
                                    ""app"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""name"": ""okta_enduser"",
                                            ""label"": ""okta_enduser"",
                                            ""id"": ""DEFAULT_APP""
                                        }
                                    }
                                }
            ";

            var rawIdentifyResponse = @"
            {
                ""stateHandle"": ""02mOEmmIhklxzTn5W3erSQ0y9RwhjjPDHbvMTfgF5b"",
                ""version"": ""1.0.0"",
                ""expiresAt"": ""2020-10-19T19:32:24.000Z"",
                ""intent"": ""LOGIN"",
                ""remediation"": {
                    ""type"": ""array"",
                    ""value"": [
                        {
                            ""rel"": [
                                ""create-form""
                            ],
                            ""name"": ""select-authenticator-authenticate"",
                            ""href"": ""https://foo.okta.com/idp/idx/challenge"",
                            ""method"": ""POST"",
                            ""value"": [
                                {
                                    ""name"": ""authenticator"",
                                    ""type"": ""object"",
                                    ""options"": [
                                        {
                                            ""label"": ""Email"",
                                            ""value"": {
                                                ""form"": {
                                                    ""value"": [
                                                        {
                                                            ""name"": ""id"",
                                                            ""required"": true,
                                                            ""value"": ""aut2ihzk1gHl7ynhd1d6"",
                                                            ""mutable"": false
                                                        },
                                                        {
                                                            ""name"": ""methodType"",
                                                            ""required"": false,
                                                            ""value"": ""email"",
                                                            ""mutable"": false
                                                        }
                                                    ]
                                                }
                                            },
                                            ""relatesTo"": ""$.authenticatorEnrollments.value[0]""
                                        },
                                        {
                                            ""label"": ""Password"",
                                            ""value"": {
                                                ""form"": {
                                                    ""value"": [
                                                        {
                                                            ""name"": ""id"",
                                                            ""required"": true,
                                                            ""value"": ""aut2ihzk2n15tsQnQ1d6"",
                                                            ""mutable"": false
                                                        },
                                                        {
                                                            ""name"": ""methodType"",
                                                            ""required"": false,
                                                            ""value"": ""password"",
                                                            ""mutable"": false
                                                        }
                                                    ]
                                                }
                                            },
                                            ""relatesTo"": ""$.authenticatorEnrollments.value[1]""
                                        },
                                        {
                                            ""label"": ""Security Question"",
                                            ""value"": {
                                                ""form"": {
                                                    ""value"": [
                                                        {
                                                            ""name"": ""id"",
                                                            ""required"": true,
                                                            ""value"": ""aut2ihzk4hgf9sIQa1d6"",
                                                            ""mutable"": false
                                                        },
                                                        {
                                                            ""name"": ""methodType"",
                                                            ""required"": false,
                                                            ""value"": ""security_question"",
                                                            ""mutable"": false
                                                        }
                                                    ]
                                                }
                                            },
                                            ""relatesTo"": ""$.authenticatorEnrollments.value[2]""
                                        }
                                    ]
                                },
                                {
                                    ""name"": ""stateHandle"",
                                    ""required"": true,
                                    ""value"": ""02mOEmmIhklxzTn5W3erSQ0y9RwhjjPDHbvMTfgF5b"",
                                    ""visible"": false,
                                    ""mutable"": false
                                }
                            ],
                            ""accepts"": ""application/ion+json; okta-version=1.0.0""
                        }
                    ]
                },
                ""authenticators"": {
                    ""type"": ""array"",
                    ""value"": [
                        {
                            ""type"": ""email"",
                            ""id"": ""aut2ihzk1gHl7ynhd1d6"",
                            ""displayName"": ""Email"",
                            ""methods"": [
                                {
                                    ""type"": ""email""
                                }
                            ]
                        },
                        {
                            ""type"": ""password"",
                            ""id"": ""aut2ihzk2n15tsQnQ1d6"",
                            ""displayName"": ""Password"",
                            ""methods"": [
                                {
                                    ""type"": ""password""
                                }
                            ]
                        },
                        {
                            ""type"": ""security_question"",
                            ""id"": ""aut2ihzk4hgf9sIQa1d6"",
                            ""displayName"": ""Security Question"",
                            ""methods"": [
                                {
                                    ""type"": ""security_question""
                                }
                            ]
                        }
                    ]
                },
                ""authenticatorEnrollments"": {
                    ""type"": ""array"",
                    ""value"": [
                        {
                            ""profile"": {
                                ""email"": ""*********""
                            },
                            ""type"": ""email"",
                            ""id"": ""eae36qtuvrCqeOcVn1d6"",
                            ""displayName"": ""Email"",
                            ""methods"": [
                                {
                                    ""type"": ""email""
                                }
                            ]
                        },
                        {
                            ""type"": ""password"",
                            ""id"": ""laefij8ozxWgbU7Oa1d5"",
                            ""displayName"": ""Password"",
                            ""methods"": [
                                {
                                    ""type"": ""password""
                                }
                            ]
                        },
                        {
                            ""profile"": {
                                ""questionKey"": ""name_of_first_plush_toy"",
                                ""question"": ""What is the name of your first stuffed animal?""
                            },
                            ""type"": ""security_question"",
                            ""id"": ""qae36qzf5rQgTpo1e1d6"",
                            ""displayName"": ""Security Question"",
                            ""methods"": [
                                {
                                    ""type"": ""security_question""
                                }
                            ]
                        }
                    ]
                },
                ""user"": {
                    ""type"": ""object"",
                    ""value"": {
                        ""id"": ""00u36qtuuODvRg5Tx1d6""
                    }
                },
                ""cancel"": {
                    ""rel"": [
                        ""create-form""
                    ],
                    ""name"": ""cancel"",
                    ""href"": ""https://foo.okta.com/idp/idx/cancel"",
                    ""method"": ""POST"",
                    ""value"": [
                        {
                            ""name"": ""stateHandle"",
                            ""required"": true,
                            ""value"": ""02mOEmmIhklxzTn5W3erSQ0y9RwhjjPDHbvMTfgF5b"",
                            ""visible"": false,
                            ""mutable"": false
                        }
                    ],
                    ""accepts"": ""application/ion+json; okta-version=1.0.0""
                },
                ""app"": {
                    ""type"": ""object"",
                    ""value"": {
                        ""name"": ""okta_enduser"",
                        ""label"": ""okta_enduser"",
                        ""id"": ""DEFAULT_APP""
                    }
                }
            }
            ";

            #endregion

            var mockRequestExecutor = new MockedStringRequestExecutor(rawIdentifyResponse);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            // Create a mock for an introspect response with "Identify" as a remediation option
            var resourceFactory = new ResourceFactory(testClient, NullLogger.Instance, new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource))));
            var data = new DefaultSerializer().Deserialize(rawIntrospectResponse);
            var introspectResponse = resourceFactory.CreateNew<IdxResponse>(data);


            introspectResponse.Remediation.RemediationOptions.FirstOrDefault().Href.Should().Be("https://foo.okta.com/idp/idx/identify");
            var identifyRemediationOption = introspectResponse.Remediation.RemediationOptions.FirstOrDefault();

            // Create a mock for a possible body when making a call to /identify
            var remediationProceedRequest = new IdxRequestPayload()
            {
                StateHandle = "02mOEmmIhklxzTn5W3erSQ0y9RwhjjPDHbvMTfgF5b",
            };

            remediationProceedRequest.SetProperty("identifier", "test-user@okta.com");
            remediationProceedRequest.SetProperty("rememberMe", false);

            // Proceed with "identifier"
            var identifyProcessedResponse = await identifyRemediationOption.ProceedAsync(remediationProceedRequest);

            identifyProcessedResponse.Should().NotBeNull();
            identifyProcessedResponse.StateHandle.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Version.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.ExpiresAt.Value.Should().Be(DateTimeOffset.Parse("2020-10-19T19:32:24.000Z"));
            identifyProcessedResponse.Intent.Should().Be("LOGIN");

            identifyProcessedResponse.Remediation.GetRaw().Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Remediation.Type.Should().Be("array");

            identifyProcessedResponse.Remediation.RemediationOptions.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Rel.Should().Contain("create-form");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Name.Should().Be("select-authenticator-authenticate");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Href.Should().Be("https://foo.okta.com/idp/idx/challenge");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Method.Should().Be("POST");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Accepts.Should().Be("application/ion+json; okta-version=1.0.0");

            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Form.Should().NotBeNullOrEmpty();
            var stateHandleFormItem = identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "stateHandle");
            stateHandleFormItem.Should().NotBeNull();
            stateHandleFormItem.Required.Should().BeTrue();
            stateHandleFormItem.GetProperty<string>("value").Should().Be("02mOEmmIhklxzTn5W3erSQ0y9RwhjjPDHbvMTfgF5b");
            stateHandleFormItem.Visible.Should().BeFalse();
            stateHandleFormItem.Mutable.Should().BeFalse();

            var authenticatorFormItem = identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "authenticator");
            authenticatorFormItem.Should().NotBeNull();
            authenticatorFormItem.Type.Should().Be("object");

            authenticatorFormItem.Options.Should().NotBeNullOrEmpty();

            authenticatorFormItem.Options.FirstOrDefault(x => x.Label == "Email").Should().NotBeNull();
            var emailOption = authenticatorFormItem.Options.FirstOrDefault(x => x.Label == "Email");
            emailOption.RelatesTo.Should().Be("$.authenticatorEnrollments.value[0]");

            // TODO: Discuss mixed types for value with the team
            var emailOptionFormValue = emailOption.GetProperty<FormValue>("value");
            emailOptionFormValue.Should().NotBeNull();

            var emailOptionFormValueValues = emailOptionFormValue.Form.GetArrayProperty<FormValue>("value");
            emailOptionFormValueValues.Should().NotBeNullOrEmpty();

            var idFormItem = emailOptionFormValueValues.FirstOrDefault(x => x.Name == "id");
            idFormItem.Should().NotBeNull();
            idFormItem.Required.Should().BeTrue();
            idFormItem.GetProperty<string>("value").Should().Be("aut2ihzk1gHl7ynhd1d6");
            idFormItem.Mutable.Should().BeFalse();

            var methodTypeFormItem = emailOptionFormValueValues.FirstOrDefault(x => x.Name == "methodType");
            methodTypeFormItem.Should().NotBeNull();
            methodTypeFormItem.Required.Should().BeFalse();
            methodTypeFormItem.GetProperty<string>("value").Should().Be("email");
            methodTypeFormItem.Mutable.Should().BeFalse();

            // Password
            authenticatorFormItem.Options.FirstOrDefault(x => x.Label == "Password").Should().NotBeNull();
            var passwordOption = authenticatorFormItem.Options.FirstOrDefault(x => x.Label == "Password");
            passwordOption.RelatesTo.Should().Be("$.authenticatorEnrollments.value[1]");


            var passwordOptionFormValue = passwordOption.GetProperty<FormValue>("value");
            passwordOptionFormValue.Should().NotBeNull();

            var passwordOptionFormValueValues = passwordOptionFormValue.Form.GetArrayProperty<FormValue>("value");
            passwordOptionFormValueValues.Should().NotBeNullOrEmpty();

            idFormItem = passwordOptionFormValueValues.FirstOrDefault(x => x.Name == "id");
            idFormItem.Should().NotBeNull();
            idFormItem.Required.Should().BeTrue();
            idFormItem.GetProperty<string>("value").Should().Be("aut2ihzk2n15tsQnQ1d6");
            idFormItem.Mutable.Should().BeFalse();

            methodTypeFormItem = passwordOptionFormValueValues.FirstOrDefault(x => x.Name == "methodType");
            methodTypeFormItem.Should().NotBeNull();
            methodTypeFormItem.Required.Should().BeFalse();
            methodTypeFormItem.GetProperty<string>("value").Should().Be("password");
            methodTypeFormItem.Mutable.Should().BeFalse();

            // Security Question
            authenticatorFormItem.Options.FirstOrDefault(x => x.Label == "Security Question").Should().NotBeNull();
            var securityQuestionOption = authenticatorFormItem.Options.FirstOrDefault(x => x.Label == "Security Question");
            securityQuestionOption.RelatesTo.Should().Be("$.authenticatorEnrollments.value[2]");


            var securityQuestionOptionFormValue = securityQuestionOption.GetProperty<FormValue>("value");
            securityQuestionOptionFormValue.Should().NotBeNull();

            var securityQuestionOptionFormValueValues = securityQuestionOptionFormValue.Form.GetArrayProperty<FormValue>("value");
            securityQuestionOptionFormValueValues.Should().NotBeNullOrEmpty();

            idFormItem = securityQuestionOptionFormValueValues.FirstOrDefault(x => x.Name == "id");
            idFormItem.Should().NotBeNull();
            idFormItem.Required.Should().BeTrue();
            idFormItem.GetProperty<string>("value").Should().Be("aut2ihzk4hgf9sIQa1d6");
            idFormItem.Mutable.Should().BeFalse();

            methodTypeFormItem = securityQuestionOptionFormValueValues.FirstOrDefault(x => x.Name == "methodType");
            methodTypeFormItem.Should().NotBeNull();
            methodTypeFormItem.Required.Should().BeFalse();
            methodTypeFormItem.GetProperty<string>("value").Should().Be("security_question");
            methodTypeFormItem.Mutable.Should().BeFalse();
        }


        [Fact]
        // STEP 3
        public async Task ProcessChallengeResponse()
        {
            #region raw response

            var rawChallengeResponse = @"
                                {
                                    ""stateHandle"": ""02R5z3MS5d0kDVJnNNi3EeY1FUQ1CWUcrPW9Bxlj_C"",
                                    ""version"": ""1.0.0"",
                                    ""expiresAt"": ""2020-10-20T19:11:05.000Z"",
                                    ""intent"": ""LOGIN"",
                                    ""remediation"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""challenge-authenticator"",
                                                ""relatesTo"": [
                                                    ""$.currentAuthenticatorEnrollment""
                                                ],
                                                ""href"": ""https://foo.okta.com/idp/idx/challenge/answer"",
                                                ""method"": ""POST"",
                                                ""value"": [
                                                    {
                                                        ""name"": ""credentials"",
                                                        ""type"": ""object"",
                                                        ""form"": {
                                                            ""value"": [
                                                                {
                                                                    ""name"": ""passcode"",
                                                                    ""label"": ""Password"",
                                                                    ""secret"": true
                                                                }
                                                            ]
                                                        },
                                                        ""required"": true
                                                    },
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""02R5z3MS5d0kDVJnNNi3EeY1FUQ1CWUcrPW9Bxlj_C"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            },
                                            {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""select-authenticator-authenticate"",
                                                ""href"": ""https://foo.okta.com/idp/idx/challenge"",
                                                ""method"": ""POST"",
                                                ""value"": [
                                                    {
                                                        ""name"": ""authenticator"",
                                                        ""type"": ""object"",
                                                        ""options"": [
                                                            {
                                                                ""label"": ""Email"",
                                                                ""value"": {
                                                                    ""form"": {
                                                                        ""value"": [
                                                                            {
                                                                                ""name"": ""id"",
                                                                                ""required"": true,
                                                                                ""value"": ""aut2ihzk1gHl7ynhd1d6"",
                                                                                ""mutable"": false
                                                                            },
                                                                            {
                                                                                ""name"": ""methodType"",
                                                                                ""required"": false,
                                                                                ""value"": ""email"",
                                                                                ""mutable"": false
                                                                            }
                                                                        ]
                                                                    }
                                                                },
                                                                ""relatesTo"": ""$.authenticatorEnrollments.value[0]""
                                                            },
                                                            {
                                                                ""label"": ""Password"",
                                                                ""value"": {
                                                                    ""form"": {
                                                                        ""value"": [
                                                                            {
                                                                                ""name"": ""id"",
                                                                                ""required"": true,
                                                                                ""value"": ""aut2ihzk2n15tsQnQ1d6"",
                                                                                ""mutable"": false
                                                                            },
                                                                            {
                                                                                ""name"": ""methodType"",
                                                                                ""required"": false,
                                                                                ""value"": ""password"",
                                                                                ""mutable"": false
                                                                            }
                                                                        ]
                                                                    }
                                                                },
                                                                ""relatesTo"": ""$.authenticatorEnrollments.value[1]""
                                                            },
                                                            {
                                                                ""label"": ""Security Question"",
                                                                ""value"": {
                                                                    ""form"": {
                                                                        ""value"": [
                                                                            {
                                                                                ""name"": ""id"",
                                                                                ""required"": true,
                                                                                ""value"": ""aut2ihzk4hgf9sIQa1d6"",
                                                                                ""mutable"": false
                                                                            },
                                                                            {
                                                                                ""name"": ""methodType"",
                                                                                ""required"": false,
                                                                                ""value"": ""security_question"",
                                                                                ""mutable"": false
                                                                            }
                                                                        ]
                                                                    }
                                                                },
                                                                ""relatesTo"": ""$.authenticatorEnrollments.value[2]""
                                                            }
                                                        ]
                                                    },
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""02R5z3MS5d0kDVJnNNi3EeY1FUQ1CWUcrPW9Bxlj_C"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            }
                                        ]
                                    },
                                    ""currentAuthenticatorEnrollment"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""recover"": {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""recover"",
                                                ""href"": ""https://foo.okta.com/idp/idx/recover"",
                                                ""method"": ""POST"",
                                                ""value"": [
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""02R5z3MS5d0kDVJnNNi3EeY1FUQ1CWUcrPW9Bxlj_C"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            },
                                            ""type"": ""password"",
                                            ""id"": ""laefij8ozxWgbU7Oa1d5"",
                                            ""displayName"": ""Password"",
                                            ""methods"": [
                                                {
                                                    ""type"": ""password""
                                                }
                                            ]
                                        }
                                    },
                                    ""authenticators"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""type"": ""email"",
                                                ""id"": ""aut2ihzk1gHl7ynhd1d6"",
                                                ""displayName"": ""Email"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""email""
                                                    }
                                                ]
                                            },
                                            {
                                                ""type"": ""password"",
                                                ""id"": ""aut2ihzk2n15tsQnQ1d6"",
                                                ""displayName"": ""Password"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""password""
                                                    }
                                                ]
                                            },
                                            {
                                                ""type"": ""security_question"",
                                                ""id"": ""aut2ihzk4hgf9sIQa1d6"",
                                                ""displayName"": ""Security Question"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""security_question""
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    ""authenticatorEnrollments"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""profile"": {
                                                    ""email"": ""*********""
                                                },
                                                ""type"": ""email"",
                                                ""id"": ""eae36qtuvrCqeOcVn1d6"",
                                                ""displayName"": ""Email"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""email""
                                                    }
                                                ]
                                            },
                                            {
                                                ""type"": ""password"",
                                                ""id"": ""laefij8ozxWgbU7Oa1d5"",
                                                ""displayName"": ""Password"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""password""
                                                    }
                                                ]
                                            },
                                            {
                                                ""profile"": {
                                                    ""questionKey"": ""name_of_first_plush_toy"",
                                                    ""question"": ""What is the name of your first stuffed animal?""
                                                },
                                                ""type"": ""security_question"",
                                                ""id"": ""qae36qzf5rQgTpo1e1d6"",
                                                ""displayName"": ""Security Question"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""security_question""
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    ""user"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""id"": ""00u36qtuuODvRg5Tx1d6""
                                        }
                                    },
                                    ""cancel"": {
                                        ""rel"": [
                                            ""create-form""
                                        ],
                                        ""name"": ""cancel"",
                                        ""href"": ""https://foo.okta.com/idp/idx/cancel"",
                                        ""method"": ""POST"",
                                        ""value"": [
                                            {
                                                ""name"": ""stateHandle"",
                                                ""required"": true,
                                                ""value"": ""02R5z3MS5d0kDVJnNNi3EeY1FUQ1CWUcrPW9Bxlj_C"",
                                                ""visible"": false,
                                                ""mutable"": false
                                            }
                                        ],
                                        ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                    },
                                    ""app"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""name"": ""okta_enduser"",
                                            ""label"": ""okta_enduser"",
                                            ""id"": ""DEFAULT_APP""
                                        }
                                    }
                                }
            ";

            var rawIdentifyResponse = @"
            {
                ""stateHandle"": ""02mOEmmIhklxzTn5W3erSQ0y9RwhjjPDHbvMTfgF5b"",
                ""version"": ""1.0.0"",
                ""expiresAt"": ""2020-10-19T19:32:24.000Z"",
                ""intent"": ""LOGIN"",
                ""remediation"": {
                    ""type"": ""array"",
                    ""value"": [
                        {
                            ""rel"": [
                                ""create-form""
                            ],
                            ""name"": ""select-authenticator-authenticate"",
                            ""href"": ""https://foo.okta.com/idp/idx/challenge"",
                            ""method"": ""POST"",
                            ""value"": [
                                {
                                    ""name"": ""authenticator"",
                                    ""type"": ""object"",
                                    ""options"": [
                                        {
                                            ""label"": ""Email"",
                                            ""value"": {
                                                ""form"": {
                                                    ""value"": [
                                                        {
                                                            ""name"": ""id"",
                                                            ""required"": true,
                                                            ""value"": ""aut2ihzk1gHl7ynhd1d6"",
                                                            ""mutable"": false
                                                        },
                                                        {
                                                            ""name"": ""methodType"",
                                                            ""required"": false,
                                                            ""value"": ""email"",
                                                            ""mutable"": false
                                                        }
                                                    ]
                                                }
                                            },
                                            ""relatesTo"": ""$.authenticatorEnrollments.value[0]""
                                        },
                                        {
                                            ""label"": ""Password"",
                                            ""value"": {
                                                ""form"": {
                                                    ""value"": [
                                                        {
                                                            ""name"": ""id"",
                                                            ""required"": true,
                                                            ""value"": ""aut2ihzk2n15tsQnQ1d6"",
                                                            ""mutable"": false
                                                        },
                                                        {
                                                            ""name"": ""methodType"",
                                                            ""required"": false,
                                                            ""value"": ""password"",
                                                            ""mutable"": false
                                                        }
                                                    ]
                                                }
                                            },
                                            ""relatesTo"": ""$.authenticatorEnrollments.value[1]""
                                        },
                                        {
                                            ""label"": ""Security Question"",
                                            ""value"": {
                                                ""form"": {
                                                    ""value"": [
                                                        {
                                                            ""name"": ""id"",
                                                            ""required"": true,
                                                            ""value"": ""aut2ihzk4hgf9sIQa1d6"",
                                                            ""mutable"": false
                                                        },
                                                        {
                                                            ""name"": ""methodType"",
                                                            ""required"": false,
                                                            ""value"": ""security_question"",
                                                            ""mutable"": false
                                                        }
                                                    ]
                                                }
                                            },
                                            ""relatesTo"": ""$.authenticatorEnrollments.value[2]""
                                        }
                                    ]
                                },
                                {
                                    ""name"": ""stateHandle"",
                                    ""required"": true,
                                    ""value"": ""02mOEmmIhklxzTn5W3erSQ0y9RwhjjPDHbvMTfgF5b"",
                                    ""visible"": false,
                                    ""mutable"": false
                                }
                            ],
                            ""accepts"": ""application/ion+json; okta-version=1.0.0""
                        }
                    ]
                },
                ""authenticators"": {
                    ""type"": ""array"",
                    ""value"": [
                        {
                            ""type"": ""email"",
                            ""id"": ""aut2ihzk1gHl7ynhd1d6"",
                            ""displayName"": ""Email"",
                            ""methods"": [
                                {
                                    ""type"": ""email""
                                }
                            ]
                        },
                        {
                            ""type"": ""password"",
                            ""id"": ""aut2ihzk2n15tsQnQ1d6"",
                            ""displayName"": ""Password"",
                            ""methods"": [
                                {
                                    ""type"": ""password""
                                }
                            ]
                        },
                        {
                            ""type"": ""security_question"",
                            ""id"": ""aut2ihzk4hgf9sIQa1d6"",
                            ""displayName"": ""Security Question"",
                            ""methods"": [
                                {
                                    ""type"": ""security_question""
                                }
                            ]
                        }
                    ]
                },
                ""authenticatorEnrollments"": {
                    ""type"": ""array"",
                    ""value"": [
                        {
                            ""profile"": {
                                ""email"": ""*********""
                            },
                            ""type"": ""email"",
                            ""id"": ""eae36qtuvrCqeOcVn1d6"",
                            ""displayName"": ""Email"",
                            ""methods"": [
                                {
                                    ""type"": ""email""
                                }
                            ]
                        },
                        {
                            ""type"": ""password"",
                            ""id"": ""laefij8ozxWgbU7Oa1d5"",
                            ""displayName"": ""Password"",
                            ""methods"": [
                                {
                                    ""type"": ""password""
                                }
                            ]
                        },
                        {
                            ""profile"": {
                                ""questionKey"": ""name_of_first_plush_toy"",
                                ""question"": ""What is the name of your first stuffed animal?""
                            },
                            ""type"": ""security_question"",
                            ""id"": ""qae36qzf5rQgTpo1e1d6"",
                            ""displayName"": ""Security Question"",
                            ""methods"": [
                                {
                                    ""type"": ""security_question""
                                }
                            ]
                        }
                    ]
                },
                ""user"": {
                    ""type"": ""object"",
                    ""value"": {
                        ""id"": ""00u36qtuuODvRg5Tx1d6""
                    }
                },
                ""cancel"": {
                    ""rel"": [
                        ""create-form""
                    ],
                    ""name"": ""cancel"",
                    ""href"": ""https://foo.okta.com/idp/idx/cancel"",
                    ""method"": ""POST"",
                    ""value"": [
                        {
                            ""name"": ""stateHandle"",
                            ""required"": true,
                            ""value"": ""02mOEmmIhklxzTn5W3erSQ0y9RwhjjPDHbvMTfgF5b"",
                            ""visible"": false,
                            ""mutable"": false
                        }
                    ],
                    ""accepts"": ""application/ion+json; okta-version=1.0.0""
                },
                ""app"": {
                    ""type"": ""object"",
                    ""value"": {
                        ""name"": ""okta_enduser"",
                        ""label"": ""okta_enduser"",
                        ""id"": ""DEFAULT_APP""
                    }
                }
            }
            ";

            #endregion

            var mockRequestExecutor = new MockedStringRequestExecutor(rawChallengeResponse);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            // Create a mock for an introspect response with "Identify" as a remediation option
            var resourceFactory = new ResourceFactory(testClient, NullLogger.Instance, new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource))));
            var data = new DefaultSerializer().Deserialize(rawIdentifyResponse);
            var identifyResponse = resourceFactory.CreateNew<IdxResponse>(data);


            identifyResponse.Remediation.RemediationOptions.FirstOrDefault().Href.Should().Be("https://foo.okta.com/idp/idx/challenge");
            var identifyRemediationOption = identifyResponse.Remediation.RemediationOptions.FirstOrDefault();

            // Create a mock for a possible body when making a call to /identify
            var remediationProceedRequest = new IdxRequestPayload()
            {
                StateHandle = "02mOEmmIhklxzTn5W3erSQ0y9RwhjjPDHbvMTfgF5b",
            };

            remediationProceedRequest.SetProperty("authenticator", new
            {
                id = "aut2ihzk2n15tsQnQ1d6",
                methodType = "password",
            });


            // Proceed with "identifier"
            var identifyProcessedResponse = await identifyRemediationOption.ProceedAsync(remediationProceedRequest);

            identifyProcessedResponse.Should().NotBeNull();
            identifyProcessedResponse.StateHandle.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Version.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.ExpiresAt.Value.Should().Be(DateTimeOffset.Parse("2020-10-20T19:11:05.000Z"));
            identifyProcessedResponse.Intent.Should().Be("LOGIN");
            identifyProcessedResponse.IsLoginSuccess.Should().BeFalse();

            identifyProcessedResponse.Remediation.GetRaw().Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Remediation.Type.Should().Be("array");

            identifyProcessedResponse.Remediation.RemediationOptions.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Rel.Should().Contain("create-form");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Name.Should().Be("challenge-authenticator");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Href.Should().Be("https://foo.okta.com/idp/idx/challenge/answer");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Method.Should().Be("POST");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Accepts.Should().Be("application/ion+json; okta-version=1.0.0");

            // TODO: relatesTo is an array here (unexpected)
            //identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().RelatesTo.Should().Be("$.currentAuthenticatorEnrollment");

            // Password
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Form.Should().NotBeNullOrEmpty();
            var stateHandleFormItem = identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "stateHandle");
            stateHandleFormItem.Should().NotBeNull();
            stateHandleFormItem.Required.Should().BeTrue();
            stateHandleFormItem.GetProperty<string>("value").Should().Be("02R5z3MS5d0kDVJnNNi3EeY1FUQ1CWUcrPW9Bxlj_C");
            stateHandleFormItem.Visible.Should().BeFalse();
            stateHandleFormItem.Mutable.Should().BeFalse();

            var credentialsFormItem = identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "credentials");
            credentialsFormItem.Should().NotBeNull();
            credentialsFormItem.Type.Should().Be("object");
            credentialsFormItem.Required.Should().BeTrue();
            credentialsFormItem.Form.Should().NotBeNull();

            var credentialsOptionFormValue = credentialsFormItem.Form.GetArrayProperty<FormValue>("value").FirstOrDefault();
            credentialsOptionFormValue.Name.Should().Be("passcode");
            credentialsOptionFormValue.Label.Should().Be("Password");
            credentialsOptionFormValue.Secret.Should().BeTrue();

            // other authenticators
            var authenticatorOption = identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault(x => x.Name == "select-authenticator-authenticate");
            authenticatorOption.Rel.Should().Contain("create-form");
            authenticatorOption.Href.Should().Be("https://foo.okta.com/idp/idx/challenge");
            authenticatorOption.Method.Should().Be("POST");

            authenticatorOption.Form.Should().NotBeNullOrEmpty();

            var authenticatorOptions = authenticatorOption.Form.FirstOrDefault(x => x.Name == "authenticator");
            authenticatorOptions.Type.Should().Be("object");
            authenticatorOptions.Options.Should().NotBeNullOrEmpty();

            // Email
            var authenticatorOptionsEmail = authenticatorOptions.Options.FirstOrDefault(x => x.Label == "Email");
            authenticatorOptionsEmail.RelatesTo.Should().Be("$.authenticatorEnrollments.value[0]");

            // TODO: Review GetValue - Review this nested object, it's too complex form.options[0].value.form.value 
            //"options": [
            //               {
            //                "label": "Email",
            //                    "value": {
            //                        "form": {
            //                            "value": [
            //                                        {
            //                                            "name": "id",
            //                                            "required": true,
            //                                            "value": "aut2ihzk1gHl7ynhd1d6",
            //                                            "mutable": false
            //                                        },

            // I had to see the raw json several times to know how to get the email options
            // FUll path: identifyProcessedResponse.Remediation.RemediationOptions[0].Form[0].Options[0].value.Form.value[x]
            var authenticatorOptionsEmailFormValues = authenticatorOptionsEmail.GetProperty<FormValue>("value").Form.GetArrayProperty<FormValue>("value");
            authenticatorOptionsEmailFormValues.Should().NotBeNullOrEmpty();

            var idFormItem = authenticatorOptionsEmailFormValues.FirstOrDefault(x => x.Name == "id");
            idFormItem.Should().NotBeNull();
            idFormItem.Required.Should().BeTrue();
            idFormItem.GetProperty<string>("value").Should().Be("aut2ihzk1gHl7ynhd1d6");
            idFormItem.Mutable.Should().BeFalse();

            var methodTypeFormItem = authenticatorOptionsEmailFormValues.FirstOrDefault(x => x.Name == "methodType");
            methodTypeFormItem.Should().NotBeNull();
            methodTypeFormItem.Required.Should().BeFalse();
            methodTypeFormItem.GetProperty<string>("value").Should().Be("email");
            methodTypeFormItem.Mutable.Should().BeFalse();

            // Password
            var authenticatorOptionsPassword = authenticatorOptions.Options.FirstOrDefault(x => x.Label == "Password");
            authenticatorOptionsPassword.RelatesTo.Should().Be("$.authenticatorEnrollments.value[1]");

            var authenticatorOptionsPasswordFormValues = authenticatorOptionsPassword.GetProperty<FormValue>("value").Form.GetArrayProperty<FormValue>("value");
            authenticatorOptionsPasswordFormValues.Should().NotBeNullOrEmpty();

            idFormItem = authenticatorOptionsPasswordFormValues.FirstOrDefault(x => x.Name == "id");
            idFormItem.Should().NotBeNull();
            idFormItem.Required.Should().BeTrue();
            idFormItem.GetProperty<string>("value").Should().Be("aut2ihzk2n15tsQnQ1d6");
            idFormItem.Mutable.Should().BeFalse();

            methodTypeFormItem = authenticatorOptionsPasswordFormValues.FirstOrDefault(x => x.Name == "methodType");
            methodTypeFormItem.Should().NotBeNull();
            methodTypeFormItem.Required.Should().BeFalse();
            methodTypeFormItem.GetProperty<string>("value").Should().Be("password");
            methodTypeFormItem.Mutable.Should().BeFalse();

            // Security Question
            var authenticatorOptionsSecurityQuestion = authenticatorOptions.Options.FirstOrDefault(x => x.Label == "Security Question");
            authenticatorOptionsSecurityQuestion.RelatesTo.Should().Be("$.authenticatorEnrollments.value[2]");

            var authenticatorOptionsSecurityQuestionFormValues = authenticatorOptionsSecurityQuestion.GetProperty<FormValue>("value").Form.GetArrayProperty<FormValue>("value");
            authenticatorOptionsSecurityQuestionFormValues.Should().NotBeNullOrEmpty();

            idFormItem = authenticatorOptionsSecurityQuestionFormValues.FirstOrDefault(x => x.Name == "id");
            idFormItem.Should().NotBeNull();
            idFormItem.Required.Should().BeTrue();
            idFormItem.GetProperty<string>("value").Should().Be("aut2ihzk4hgf9sIQa1d6");
            idFormItem.Mutable.Should().BeFalse();

            methodTypeFormItem = authenticatorOptionsSecurityQuestionFormValues.FirstOrDefault(x => x.Name == "methodType");
            methodTypeFormItem.Should().NotBeNull();
            methodTypeFormItem.Required.Should().BeFalse();
            methodTypeFormItem.GetProperty<string>("value").Should().Be("security_question");
            methodTypeFormItem.Mutable.Should().BeFalse();
        }

        [Fact]
        // STEP 4
        public async Task ProcessSendCredentialsResponse()
        {
            #region raw response

            var rawChallengeResponse = @"
                                {
                                    ""stateHandle"": ""02R5z3MS5d0kDVJnNNi3EeY1FUQ1CWUcrPW9Bxlj_C"",
                                    ""version"": ""1.0.0"",
                                    ""expiresAt"": ""2020-10-20T19:11:05.000Z"",
                                    ""intent"": ""LOGIN"",
                                    ""remediation"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""challenge-authenticator"",
                                                ""relatesTo"": [
                                                    ""$.currentAuthenticatorEnrollment""
                                                ],
                                                ""href"": ""https://foo.okta.com/idp/idx/challenge/answer"",
                                                ""method"": ""POST"",
                                                ""value"": [
                                                    {
                                                        ""name"": ""credentials"",
                                                        ""type"": ""object"",
                                                        ""form"": {
                                                            ""value"": [
                                                                {
                                                                    ""name"": ""passcode"",
                                                                    ""label"": ""Password"",
                                                                    ""secret"": true
                                                                }
                                                            ]
                                                        },
                                                        ""required"": true
                                                    },
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""02R5z3MS5d0kDVJnNNi3EeY1FUQ1CWUcrPW9Bxlj_C"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            },
                                            {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""select-authenticator-authenticate"",
                                                ""href"": ""https://foo.okta.com/idp/idx/challenge"",
                                                ""method"": ""POST"",
                                                ""value"": [
                                                    {
                                                        ""name"": ""authenticator"",
                                                        ""type"": ""object"",
                                                        ""options"": [
                                                            {
                                                                ""label"": ""Email"",
                                                                ""value"": {
                                                                    ""form"": {
                                                                        ""value"": [
                                                                            {
                                                                                ""name"": ""id"",
                                                                                ""required"": true,
                                                                                ""value"": ""aut2ihzk1gHl7ynhd1d6"",
                                                                                ""mutable"": false
                                                                            },
                                                                            {
                                                                                ""name"": ""methodType"",
                                                                                ""required"": false,
                                                                                ""value"": ""email"",
                                                                                ""mutable"": false
                                                                            }
                                                                        ]
                                                                    }
                                                                },
                                                                ""relatesTo"": ""$.authenticatorEnrollments.value[0]""
                                                            },
                                                            {
                                                                ""label"": ""Password"",
                                                                ""value"": {
                                                                    ""form"": {
                                                                        ""value"": [
                                                                            {
                                                                                ""name"": ""id"",
                                                                                ""required"": true,
                                                                                ""value"": ""aut2ihzk2n15tsQnQ1d6"",
                                                                                ""mutable"": false
                                                                            },
                                                                            {
                                                                                ""name"": ""methodType"",
                                                                                ""required"": false,
                                                                                ""value"": ""password"",
                                                                                ""mutable"": false
                                                                            }
                                                                        ]
                                                                    }
                                                                },
                                                                ""relatesTo"": ""$.authenticatorEnrollments.value[1]""
                                                            },
                                                            {
                                                                ""label"": ""Security Question"",
                                                                ""value"": {
                                                                    ""form"": {
                                                                        ""value"": [
                                                                            {
                                                                                ""name"": ""id"",
                                                                                ""required"": true,
                                                                                ""value"": ""aut2ihzk4hgf9sIQa1d6"",
                                                                                ""mutable"": false
                                                                            },
                                                                            {
                                                                                ""name"": ""methodType"",
                                                                                ""required"": false,
                                                                                ""value"": ""security_question"",
                                                                                ""mutable"": false
                                                                            }
                                                                        ]
                                                                    }
                                                                },
                                                                ""relatesTo"": ""$.authenticatorEnrollments.value[2]""
                                                            }
                                                        ]
                                                    },
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""02R5z3MS5d0kDVJnNNi3EeY1FUQ1CWUcrPW9Bxlj_C"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            }
                                        ]
                                    },
                                    ""currentAuthenticatorEnrollment"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""recover"": {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""recover"",
                                                ""href"": ""https://foo.okta.com/idp/idx/recover"",
                                                ""method"": ""POST"",
                                                ""value"": [
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""02R5z3MS5d0kDVJnNNi3EeY1FUQ1CWUcrPW9Bxlj_C"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            },
                                            ""type"": ""password"",
                                            ""id"": ""laefij8ozxWgbU7Oa1d5"",
                                            ""displayName"": ""Password"",
                                            ""methods"": [
                                                {
                                                    ""type"": ""password""
                                                }
                                            ]
                                        }
                                    },
                                    ""authenticators"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""type"": ""email"",
                                                ""id"": ""aut2ihzk1gHl7ynhd1d6"",
                                                ""displayName"": ""Email"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""email""
                                                    }
                                                ]
                                            },
                                            {
                                                ""type"": ""password"",
                                                ""id"": ""aut2ihzk2n15tsQnQ1d6"",
                                                ""displayName"": ""Password"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""password""
                                                    }
                                                ]
                                            },
                                            {
                                                ""type"": ""security_question"",
                                                ""id"": ""aut2ihzk4hgf9sIQa1d6"",
                                                ""displayName"": ""Security Question"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""security_question""
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    ""authenticatorEnrollments"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""profile"": {
                                                    ""email"": ""*********""
                                                },
                                                ""type"": ""email"",
                                                ""id"": ""eae36qtuvrCqeOcVn1d6"",
                                                ""displayName"": ""Email"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""email""
                                                    }
                                                ]
                                            },
                                            {
                                                ""type"": ""password"",
                                                ""id"": ""laefij8ozxWgbU7Oa1d5"",
                                                ""displayName"": ""Password"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""password""
                                                    }
                                                ]
                                            },
                                            {
                                                ""profile"": {
                                                    ""questionKey"": ""name_of_first_plush_toy"",
                                                    ""question"": ""What is the name of your first stuffed animal?""
                                                },
                                                ""type"": ""security_question"",
                                                ""id"": ""qae36qzf5rQgTpo1e1d6"",
                                                ""displayName"": ""Security Question"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""security_question""
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    ""user"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""id"": ""00u36qtuuODvRg5Tx1d6""
                                        }
                                    },
                                    ""cancel"": {
                                        ""rel"": [
                                            ""create-form""
                                        ],
                                        ""name"": ""cancel"",
                                        ""href"": ""https://foo.okta.com/idp/idx/cancel"",
                                        ""method"": ""POST"",
                                        ""value"": [
                                            {
                                                ""name"": ""stateHandle"",
                                                ""required"": true,
                                                ""value"": ""02R5z3MS5d0kDVJnNNi3EeY1FUQ1CWUcrPW9Bxlj_C"",
                                                ""visible"": false,
                                                ""mutable"": false
                                            }
                                        ],
                                        ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                    },
                                    ""app"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""name"": ""okta_enduser"",
                                            ""label"": ""okta_enduser"",
                                            ""id"": ""DEFAULT_APP""
                                        }
                                    }
                                }
            ";

            var rawAnswerResponse = @"
               {
                ""stateHandle"": ""02SgqMQY8JRJoPgvvME2CYBWjCASB_cuQbZXAWLMo3"",
                ""version"": ""1.0.0"",
                ""expiresAt"": ""2020-10-21T20:25:36.000Z"",
                ""intent"": ""LOGIN"",
                ""remediation"": {
                    ""type"": ""array"",
                    ""value"": [
                        {
                            ""rel"": [
                                ""create-form""
                            ],
                            ""name"": ""select-authenticator-authenticate"",
                            ""href"": ""https://foo.okta.com/idp/idx/challenge"",
                            ""method"": ""POST"",
                            ""value"": [
                                {
                                    ""name"": ""authenticator"",
                                    ""type"": ""object"",
                                    ""options"": [
                                        {
                                            ""label"": ""Email"",
                                            ""value"": {
                                                ""form"": {
                                                    ""value"": [
                                                        {
                                                            ""name"": ""id"",
                                                            ""required"": true,
                                                            ""value"": ""aut2ihzk1gHl7ynhd1d6"",
                                                            ""mutable"": false
                                                        },
                                                        {
                                                            ""name"": ""methodType"",
                                                            ""required"": false,
                                                            ""value"": ""email"",
                                                            ""mutable"": false
                                                        }
                                                    ]
                                                }
                                            },
                                            ""relatesTo"": ""$.authenticatorEnrollments.value[0]""
                                        }
                                    ]
                                },
                                {
                                    ""name"": ""stateHandle"",
                                    ""required"": true,
                                    ""value"": ""02SgqMQY8JRJoPgvvME2CYBWjCASB_cuQbZXAWLMo3"",
                                    ""visible"": false,
                                    ""mutable"": false
                                }
                            ],
                            ""accepts"": ""application/ion+json; okta-version=1.0.0""
                        }
                    ]
                },
                ""authenticators"": {
                    ""type"": ""array"",
                    ""value"": [
                        {
                            ""type"": ""email"",
                            ""id"": ""aut2ihzk1gHl7ynhd1d6"",
                            ""displayName"": ""Email"",
                            ""methods"": [
                                {
                                    ""type"": ""email""
                                }
                            ]
                        }
                    ]
                },
                ""authenticatorEnrollments"": {
                    ""type"": ""array"",
                    ""value"": [
                        {
                            ""profile"": {
                                ""email"": ""*********""
                            },
                            ""type"": ""email"",
                            ""id"": ""eae36qtuvrCqeOcVn1d6"",
                            ""displayName"": ""Email"",
                            ""methods"": [
                                {
                                    ""type"": ""email""
                                }
                            ]
                        }
                    ]
                },
                ""user"": {
                    ""type"": ""object"",
                    ""value"": {
                        ""id"": ""00u36qtuuODvRg5Tx1d6""
                    }
                },
                ""cancel"": {
                    ""rel"": [
                        ""create-form""
                    ],
                    ""name"": ""cancel"",
                    ""href"": ""https://foo.okta.com/idp/idx/cancel"",
                    ""method"": ""POST"",
                    ""value"": [
                        {
                            ""name"": ""stateHandle"",
                            ""required"": true,
                            ""value"": ""02SgqMQY8JRJoPgvvME2CYBWjCASB_cuQbZXAWLMo3"",
                            ""visible"": false,
                            ""mutable"": false
                        }
                    ],
                    ""accepts"": ""application/ion+json; okta-version=1.0.0""
                },
                ""app"": {
                    ""type"": ""object"",
                    ""value"": {
                        ""name"": ""okta_enduser"",
                        ""label"": ""okta_enduser"",
                        ""id"": ""DEFAULT_APP""
                    }
                }
            }";

            #endregion

            var mockRequestExecutor = new MockedStringRequestExecutor(rawAnswerResponse);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            // Create a mock for an introspect response with "Identify" as a remediation option
            var resourceFactory = new ResourceFactory(testClient, NullLogger.Instance, new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource))));
            var data = new DefaultSerializer().Deserialize(rawChallengeResponse);
            var challengeResponse = resourceFactory.CreateNew<IdxResponse>(data);


            challengeResponse.Remediation.RemediationOptions.FirstOrDefault().Href.Should().Be("https://foo.okta.com/idp/idx/challenge/answer");
            var identifyRemediationOption = challengeResponse.Remediation.RemediationOptions.FirstOrDefault();

            // Create a mock for a possible body when making a call to /identify
            var remediationProceedRequest = new IdxRequestPayload()
            {
                StateHandle = "02SgqMQY8JRJoPgvvME2CYBWjCASB_cuQbZXAWLMo3",
            };

            remediationProceedRequest.SetProperty("credentials", new
            {
                passcode = "foo",
            });


            // Proceed with "identifier"
            var identifyProcessedResponse = await identifyRemediationOption.ProceedAsync(remediationProceedRequest);

            identifyProcessedResponse.Should().NotBeNull();
            identifyProcessedResponse.StateHandle.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Version.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.ExpiresAt.Value.Should().Be(DateTimeOffset.Parse("2020-10-21T20:25:36.000Z"));
            identifyProcessedResponse.Intent.Should().Be("LOGIN");
            identifyProcessedResponse.IsLoginSuccess.Should().BeFalse();

            identifyProcessedResponse.Remediation.GetRaw().Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Remediation.Type.Should().Be("array");

            identifyProcessedResponse.Remediation.RemediationOptions.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Rel.Should().Contain("create-form");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Name.Should().Be("select-authenticator-authenticate");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Href.Should().Be("https://foo.okta.com/idp/idx/challenge");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Method.Should().Be("POST");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Accepts.Should().Be("application/ion+json; okta-version=1.0.0");

            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Form.Should().NotBeNullOrEmpty();
            var stateHandleFormItem = identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "stateHandle");
            stateHandleFormItem.Should().NotBeNull();
            stateHandleFormItem.Required.Should().BeTrue();
            stateHandleFormItem.GetProperty<string>("value").Should().Be("02SgqMQY8JRJoPgvvME2CYBWjCASB_cuQbZXAWLMo3");
            stateHandleFormItem.Visible.Should().BeFalse();
            stateHandleFormItem.Mutable.Should().BeFalse();


            var authenticatorOption = identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault(x => x.Name == "select-authenticator-authenticate");
            authenticatorOption.Rel.Should().Contain("create-form");
            authenticatorOption.Href.Should().Be("https://foo.okta.com/idp/idx/challenge");
            authenticatorOption.Method.Should().Be("POST");

            authenticatorOption.Form.Should().NotBeNullOrEmpty();

            var authenticatorOptions = authenticatorOption.Form.FirstOrDefault(x => x.Name == "authenticator");
            authenticatorOptions.Type.Should().Be("object");
            authenticatorOptions.Options.Should().NotBeNullOrEmpty();

            // Email
            var authenticatorOptionsEmail = authenticatorOptions.Options.FirstOrDefault(x => x.Label == "Email");
            authenticatorOptionsEmail.RelatesTo.Should().Be("$.authenticatorEnrollments.value[0]");

            // TODO: Review GetValue - Review this nested object, it's too complex form.options[0].value.form.value 
            //"options": [
            //               {
            //                "label": "Email",
            //                    "value": {
            //                        "form": {
            //                            "value": [
            //                                        {
            //                                            "name": "id",
            //                                            "required": true,
            //                                            "value": "aut2ihzk1gHl7ynhd1d6",
            //                                            "mutable": false
            //                                        },

            // I had to see the raw json several times to know how to get the email options
            // FUll path: identifyProcessedResponse.Remediation.RemediationOptions[0].Form[0].Options[0].value.Form.value[x]
            var authenticatorOptionsEmailFormValues = authenticatorOptionsEmail.GetProperty<FormValue>("value").Form.GetArrayProperty<FormValue>("value");
            authenticatorOptionsEmailFormValues.Should().NotBeNullOrEmpty();

            var idFormItem = authenticatorOptionsEmailFormValues.FirstOrDefault(x => x.Name == "id");
            idFormItem.Should().NotBeNull();
            idFormItem.Required.Should().BeTrue();
            idFormItem.GetProperty<string>("value").Should().Be("aut2ihzk1gHl7ynhd1d6");
            idFormItem.Mutable.Should().BeFalse();

            var methodTypeFormItem = authenticatorOptionsEmailFormValues.FirstOrDefault(x => x.Name == "methodType");
            methodTypeFormItem.Should().NotBeNull();
            methodTypeFormItem.Required.Should().BeFalse();
            methodTypeFormItem.GetProperty<string>("value").Should().Be("email");
            methodTypeFormItem.Mutable.Should().BeFalse();

        }

        [Fact]
        // STEP 5
        public async Task ProcessEmailAsSecondFactorResponse()
        {
            #region raw response

            var rawChallengeEmailResponse = @"
                                {
                                    ""stateHandle"": ""02zLArAwt9TEyKx8rvNzIREZTf6OCvoCbRf9gH4tU1"",
                                    ""version"": ""1.0.0"",
                                    ""expiresAt"": ""2020-10-21T20:58:13.000Z"",
                                    ""intent"": ""LOGIN"",
                                    ""remediation"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""challenge-authenticator"",
                                                ""relatesTo"": [
                                                    ""$.currentAuthenticatorEnrollment""
                                                ],
                                                ""href"": ""https://foo.okta.com/idp/idx/challenge/answer"",
                                                ""method"": ""POST"",
                                                ""value"": [
                                                    {
                                                        ""name"": ""credentials"",
                                                        ""type"": ""object"",
                                                        ""form"": {
                                                            ""value"": [
                                                                {
                                                                    ""name"": ""passcode"",
                                                                    ""label"": ""Enter code""
                                                                }
                                                            ]
                                                        },
                                                        ""required"": true
                                                    },
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""02zLArAwt9TEyKx8rvNzIREZTf6OCvoCbRf9gH4tU1"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            }
                                        ]
                                    },
                                    ""currentAuthenticatorEnrollment"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""profile"": {
                                                ""email"": ""*********""
                                            },
                                            ""resend"": {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""resend"",
                                                ""href"": ""https://foo.okta.com/idp/idx/challenge/resend"",
                                                ""method"": ""POST"",
                                                ""value"": [
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""02zLArAwt9TEyKx8rvNzIREZTf6OCvoCbRf9gH4tU1"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            },
                                            ""poll"": {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""poll"",
                                                ""href"": ""https://foo.okta.com/idp/idx/challenge/poll"",
                                                ""method"": ""POST"",
                                                ""refresh"": 4000,
                                                ""value"": [
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""02zLArAwt9TEyKx8rvNzIREZTf6OCvoCbRf9gH4tU1"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            },
                                            ""type"": ""email"",
                                            ""id"": ""eae36qtuvrCqeOcVn1d6"",
                                            ""displayName"": ""Email"",
                                            ""methods"": [
                                                {
                                                    ""type"": ""email""
                                                }
                                            ]
                                        }
                                    },
                                    ""authenticators"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""type"": ""email"",
                                                ""id"": ""aut2ihzk1gHl7ynhd1d6"",
                                                ""displayName"": ""Email"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""email""
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    ""authenticatorEnrollments"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""profile"": {
                                                    ""email"": ""*********""
                                                },
                                                ""type"": ""email"",
                                                ""id"": ""eae36qtuvrCqeOcVn1d6"",
                                                ""displayName"": ""Email"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""email""
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    ""user"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""id"": ""00u36qtuuODvRg5Tx1d6""
                                        }
                                    },
                                    ""cancel"": {
                                        ""rel"": [
                                            ""create-form""
                                        ],
                                        ""name"": ""cancel"",
                                        ""href"": ""https://foo.okta.com/idp/idx/cancel"",
                                        ""method"": ""POST"",
                                        ""value"": [
                                            {
                                                ""name"": ""stateHandle"",
                                                ""required"": true,
                                                ""value"": ""02zLArAwt9TEyKx8rvNzIREZTf6OCvoCbRf9gH4tU1"",
                                                ""visible"": false,
                                                ""mutable"": false
                                            }
                                        ],
                                        ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                    },
                                    ""app"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""name"": ""okta_enduser"",
                                            ""label"": ""okta_enduser"",
                                            ""id"": ""DEFAULT_APP""
                                        }
                                    }
                                }
            ";

            var rawAnswerResponse = @"
               {
                ""stateHandle"": ""02SgqMQY8JRJoPgvvME2CYBWjCASB_cuQbZXAWLMo3"",
                ""version"": ""1.0.0"",
                ""expiresAt"": ""2020-10-21T20:25:36.000Z"",
                ""intent"": ""LOGIN"",
                ""remediation"": {
                    ""type"": ""array"",
                    ""value"": [
                        {
                            ""rel"": [
                                ""create-form""
                            ],
                            ""name"": ""select-authenticator-authenticate"",
                            ""href"": ""https://foo.okta.com/idp/idx/challenge"",
                            ""method"": ""POST"",
                            ""value"": [
                                {
                                    ""name"": ""authenticator"",
                                    ""type"": ""object"",
                                    ""options"": [
                                        {
                                            ""label"": ""Email"",
                                            ""value"": {
                                                ""form"": {
                                                    ""value"": [
                                                        {
                                                            ""name"": ""id"",
                                                            ""required"": true,
                                                            ""value"": ""aut2ihzk1gHl7ynhd1d6"",
                                                            ""mutable"": false
                                                        },
                                                        {
                                                            ""name"": ""methodType"",
                                                            ""required"": false,
                                                            ""value"": ""email"",
                                                            ""mutable"": false
                                                        }
                                                    ]
                                                }
                                            },
                                            ""relatesTo"": ""$.authenticatorEnrollments.value[0]""
                                        }
                                    ]
                                },
                                {
                                    ""name"": ""stateHandle"",
                                    ""required"": true,
                                    ""value"": ""02SgqMQY8JRJoPgvvME2CYBWjCASB_cuQbZXAWLMo3"",
                                    ""visible"": false,
                                    ""mutable"": false
                                }
                            ],
                            ""accepts"": ""application/ion+json; okta-version=1.0.0""
                        }
                    ]
                },
                ""authenticators"": {
                    ""type"": ""array"",
                    ""value"": [
                        {
                            ""type"": ""email"",
                            ""id"": ""aut2ihzk1gHl7ynhd1d6"",
                            ""displayName"": ""Email"",
                            ""methods"": [
                                {
                                    ""type"": ""email""
                                }
                            ]
                        }
                    ]
                },
                ""authenticatorEnrollments"": {
                    ""type"": ""array"",
                    ""value"": [
                        {
                            ""profile"": {
                                ""email"": ""*********""
                            },
                            ""type"": ""email"",
                            ""id"": ""eae36qtuvrCqeOcVn1d6"",
                            ""displayName"": ""Email"",
                            ""methods"": [
                                {
                                    ""type"": ""email""
                                }
                            ]
                        }
                    ]
                },
                ""user"": {
                    ""type"": ""object"",
                    ""value"": {
                        ""id"": ""00u36qtuuODvRg5Tx1d6""
                    }
                },
                ""cancel"": {
                    ""rel"": [
                        ""create-form""
                    ],
                    ""name"": ""cancel"",
                    ""href"": ""https://foo.okta.com/idp/idx/cancel"",
                    ""method"": ""POST"",
                    ""value"": [
                        {
                            ""name"": ""stateHandle"",
                            ""required"": true,
                            ""value"": ""02SgqMQY8JRJoPgvvME2CYBWjCASB_cuQbZXAWLMo3"",
                            ""visible"": false,
                            ""mutable"": false
                        }
                    ],
                    ""accepts"": ""application/ion+json; okta-version=1.0.0""
                },
                ""app"": {
                    ""type"": ""object"",
                    ""value"": {
                        ""name"": ""okta_enduser"",
                        ""label"": ""okta_enduser"",
                        ""id"": ""DEFAULT_APP""
                    }
                }
            }";

            #endregion

            var mockRequestExecutor = new MockedStringRequestExecutor(rawChallengeEmailResponse);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            // Create a mock for an introspect response with "Identify" as a remediation option
            var resourceFactory = new ResourceFactory(testClient, NullLogger.Instance, new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource))));
            var data = new DefaultSerializer().Deserialize(rawAnswerResponse);
            var challengeResponse = resourceFactory.CreateNew<IdxResponse>(data);


            challengeResponse.Remediation.RemediationOptions.FirstOrDefault().Href.Should().Be("https://foo.okta.com/idp/idx/challenge");
            var identifyRemediationOption = challengeResponse.Remediation.RemediationOptions.FirstOrDefault();

            // Create a mock for a possible body when making a call to /identify
            var remediationProceedRequest = new IdxRequestPayload()
            {
                StateHandle = "02SgqMQY8JRJoPgvvME2CYBWjCASB_cuQbZXAWLMo3",
            };

            remediationProceedRequest.SetProperty("authenticator", new
            {
                id = "aut2ihzk1gHl7ynhd1d6",
                methodType = "email",
            });


            var identifyProcessedResponse = await identifyRemediationOption.ProceedAsync(remediationProceedRequest);

            identifyProcessedResponse.Should().NotBeNull();
            identifyProcessedResponse.StateHandle.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Version.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.ExpiresAt.Value.Should().Be(DateTimeOffset.Parse("2020-10-21T20:58:13.000Z"));
            identifyProcessedResponse.Intent.Should().Be("LOGIN");

            identifyProcessedResponse.Remediation.GetRaw().Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Remediation.Type.Should().Be("array");

            identifyProcessedResponse.Remediation.RemediationOptions.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Rel.Should().Contain("create-form");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Name.Should().Be("challenge-authenticator");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Href.Should().Be("https://foo.okta.com/idp/idx/challenge/answer");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Method.Should().Be("POST");
            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Accepts.Should().Be("application/ion+json; okta-version=1.0.0");
            identifyProcessedResponse.IsLoginSuccess.Should().BeFalse();

            identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Form.Should().NotBeNullOrEmpty();
            var stateHandleFormItem = identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "stateHandle");
            stateHandleFormItem.Should().NotBeNull();
            stateHandleFormItem.Required.Should().BeTrue();
            stateHandleFormItem.GetProperty<string>("value").Should().Be("02zLArAwt9TEyKx8rvNzIREZTf6OCvoCbRf9gH4tU1");
            stateHandleFormItem.Visible.Should().BeFalse();
            stateHandleFormItem.Mutable.Should().BeFalse();

            var credentialsFormItem = identifyProcessedResponse.Remediation.RemediationOptions.FirstOrDefault().Form.FirstOrDefault(x => x.Name == "credentials");
            credentialsFormItem.Should().NotBeNull();
            credentialsFormItem.Type.Should().Be("object");
            credentialsFormItem.Required.Should().BeTrue();
            var credentialsFormItemValues = credentialsFormItem.Form.GetArrayProperty<FormValue>("value");
            credentialsFormItemValues.FirstOrDefault().Name.Should().Be("passcode");
            credentialsFormItemValues.FirstOrDefault().Label.Should().Be("Enter code");

        }

        [Fact]
        // STEP 6
        public async Task ProcessFinalizeWithEmailAsSecondFactorResponse()
        {
            #region raw response

            var rawChallengeEmailResponse = @"
                                {
                                    ""stateHandle"": ""02zLArAwt9TEyKx8rvNzIREZTf6OCvoCbRf9gH4tU1"",
                                    ""version"": ""1.0.0"",
                                    ""expiresAt"": ""2020-10-21T20:58:13.000Z"",
                                    ""intent"": ""LOGIN"",
                                    ""remediation"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""challenge-authenticator"",
                                                ""relatesTo"": [
                                                    ""$.currentAuthenticatorEnrollment""
                                                ],
                                                ""href"": ""https://foo.okta.com/idp/idx/challenge/answer"",
                                                ""method"": ""POST"",
                                                ""value"": [
                                                    {
                                                        ""name"": ""credentials"",
                                                        ""type"": ""object"",
                                                        ""form"": {
                                                            ""value"": [
                                                                {
                                                                    ""name"": ""passcode"",
                                                                    ""label"": ""Enter code""
                                                                }
                                                            ]
                                                        },
                                                        ""required"": true
                                                    },
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""02zLArAwt9TEyKx8rvNzIREZTf6OCvoCbRf9gH4tU1"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            }
                                        ]
                                    },
                                    ""currentAuthenticatorEnrollment"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""profile"": {
                                                ""email"": ""*********""
                                            },
                                            ""resend"": {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""resend"",
                                                ""href"": ""https://foo.okta.com/idp/idx/challenge/resend"",
                                                ""method"": ""POST"",
                                                ""value"": [
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""02zLArAwt9TEyKx8rvNzIREZTf6OCvoCbRf9gH4tU1"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            },
                                            ""poll"": {
                                                ""rel"": [
                                                    ""create-form""
                                                ],
                                                ""name"": ""poll"",
                                                ""href"": ""https://foo.okta.com/idp/idx/challenge/poll"",
                                                ""method"": ""POST"",
                                                ""refresh"": 4000,
                                                ""value"": [
                                                    {
                                                        ""name"": ""stateHandle"",
                                                        ""required"": true,
                                                        ""value"": ""02zLArAwt9TEyKx8rvNzIREZTf6OCvoCbRf9gH4tU1"",
                                                        ""visible"": false,
                                                        ""mutable"": false
                                                    }
                                                ],
                                                ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                            },
                                            ""type"": ""email"",
                                            ""id"": ""eae36qtuvrCqeOcVn1d6"",
                                            ""displayName"": ""Email"",
                                            ""methods"": [
                                                {
                                                    ""type"": ""email""
                                                }
                                            ]
                                        }
                                    },
                                    ""authenticators"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""type"": ""email"",
                                                ""id"": ""aut2ihzk1gHl7ynhd1d6"",
                                                ""displayName"": ""Email"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""email""
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    ""authenticatorEnrollments"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""profile"": {
                                                    ""email"": ""*********""
                                                },
                                                ""type"": ""email"",
                                                ""id"": ""eae36qtuvrCqeOcVn1d6"",
                                                ""displayName"": ""Email"",
                                                ""methods"": [
                                                    {
                                                        ""type"": ""email""
                                                    }
                                                ]
                                            }
                                        ]
                                    },
                                    ""user"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""id"": ""00u36qtuuODvRg5Tx1d6""
                                        }
                                    },
                                    ""cancel"": {
                                        ""rel"": [
                                            ""create-form""
                                        ],
                                        ""name"": ""cancel"",
                                        ""href"": ""https://foo.okta.com/idp/idx/cancel"",
                                        ""method"": ""POST"",
                                        ""value"": [
                                            {
                                                ""name"": ""stateHandle"",
                                                ""required"": true,
                                                ""value"": ""02zLArAwt9TEyKx8rvNzIREZTf6OCvoCbRf9gH4tU1"",
                                                ""visible"": false,
                                                ""mutable"": false
                                            }
                                        ],
                                        ""accepts"": ""application/ion+json; okta-version=1.0.0""
                                    },
                                    ""app"": {
                                        ""type"": ""object"",
                                        ""value"": {
                                            ""name"": ""okta_enduser"",
                                            ""label"": ""okta_enduser"",
                                            ""id"": ""DEFAULT_APP""
                                        }
                                    }
                                }
            ";

            var rawSendEmailCodeResponse = @"
                {
                    ""stateHandle"": ""02eYb3-CKPkh3l9SMxym9zyPZt1bLo5q_yab6ahhFg"",
                    ""version"": ""1.0.0"",
                    ""expiresAt"": ""2020-10-22T18:35:22.000Z"",
                    ""intent"": ""LOGIN"",
                    ""user"": {
                        ""type"": ""object"",
                        ""value"": {
                            ""id"": ""00u36qtuuODvRg5Tx1d6""
                        }
                    },
                    ""successWithInteractionCode"": {
                        ""rel"": [
                            ""create-form""
                        ],
                        ""name"": ""issue"",
                        ""href"": ""https://foo.okta.com/oauth2/v1/token"",
                        ""method"": ""POST"",
                        ""value"": [
                            {
                                ""name"": ""grant_type"",
                                ""label"": ""Grant Type"",
                                ""required"": true,
                                ""value"": ""interaction_code""
                            },
                            {
                                ""name"": ""interaction_code"",
                                ""label"": ""Interaction Code"",
                                ""required"": true,
                                ""value"": ""dUJHngn125_NsSyr86zpFZg8PJWNUgzAKK88GKw781U""
                            },
                            {
                                ""name"": ""client_id"",
                                ""label"": ""Client Id"",
                                ""required"": true,
                                ""value"": ""000000""
                            }
                        ],
                        ""accepts"": ""application/x-www-form-urlencoded""
                    },
                    ""cancel"": {
                        ""rel"": [
                            ""create-form""
                        ],
                        ""name"": ""cancel"",
                        ""href"": ""https://foo.okta.com/idp/idx/cancel"",
                        ""method"": ""POST"",
                        ""value"": [
                            {
                                ""name"": ""stateHandle"",
                                ""required"": true,
                                ""value"": ""02eYb3-CKPkh3l9SMxym9zyPZt1bLo5q_yab6ahhFg"",
                                ""visible"": false,
                                ""mutable"": false
                            }
                        ],
                        ""accepts"": ""application/ion+json; okta-version=1.0.0""
                    },
                    ""app"": {
                        ""type"": ""object"",
                        ""value"": {
                            ""name"": ""okta_enduser"",
                            ""label"": ""okta_enduser"",
                            ""id"": ""DEFAULT_APP""
                        }
                    }
                }
            ";

            #endregion

            var mockRequestExecutor = new MockedStringRequestExecutor(rawSendEmailCodeResponse);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            // Create a mock for an introspect response with "Identify" as a remediation option
            var resourceFactory = new ResourceFactory(
                testClient,
                NullLogger.Instance,
                new AbstractResourceTypeResolverFactory(ResourceTypeHelper.GetAllDefinedTypes(typeof(Resource))));
            var data = new DefaultSerializer().Deserialize(rawChallengeEmailResponse);
            var challengeResponse = resourceFactory.CreateNew<IdxResponse>(data);


            challengeResponse.Remediation.RemediationOptions.FirstOrDefault().Href.Should()
                .Be("https://foo.okta.com/idp/idx/challenge/answer");
            var identifyRemediationOption = challengeResponse.Remediation.RemediationOptions.FirstOrDefault();

            // Create a mock for a possible body when making a call to /identify
            var remediationProceedRequest = new IdxRequestPayload()
                                                {
                                                    StateHandle = "02eYb3-CKPkh3l9SMxym9zyPZt1bLo5q_yab6ahhFg",
                                                };

            remediationProceedRequest.SetProperty("credentials", new { passcode = "31416", });

            var identifyProcessedResponse = await identifyRemediationOption.ProceedAsync(remediationProceedRequest);

            identifyProcessedResponse.Should().NotBeNull();
            identifyProcessedResponse.StateHandle.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.Version.Should().NotBeNullOrEmpty();
            identifyProcessedResponse.ExpiresAt.Value.Should().Be(DateTimeOffset.Parse("2020-10-22T18:35:22.000Z"));
            identifyProcessedResponse.Intent.Should().Be("LOGIN");

            // TODO: Revisit this is not null now
            //identifyProcessedResponse.Remediation.Should().BeNull();

            identifyProcessedResponse.IsLoginSuccess.Should().BeTrue();
            identifyProcessedResponse.SuccessWithInteractionCode.Should().NotBeNull();
            identifyProcessedResponse.SuccessWithInteractionCode.Rel.Should().Contain("create-form");
            identifyProcessedResponse.SuccessWithInteractionCode.Name.Should().Be("issue");
            identifyProcessedResponse.SuccessWithInteractionCode.Href.Should()
                .Be("https://foo.okta.com/oauth2/v1/token");
            identifyProcessedResponse.SuccessWithInteractionCode.Method.Should().Be("POST");
            identifyProcessedResponse.SuccessWithInteractionCode.Accepts.Should()
                .Be("application/x-www-form-urlencoded");

            var successFormValues =
                identifyProcessedResponse.SuccessWithInteractionCode.GetArrayProperty<FormValue>("value");

            var grantTypeFormValue = successFormValues.FirstOrDefault(x => x.Name == "grant_type");
            grantTypeFormValue.Label.Should().Be("Grant Type");
            grantTypeFormValue.Required.Should().BeTrue();
            grantTypeFormValue.GetProperty<string>("value").Should().Be("interaction_code");

            var interactionCodeFormValue = successFormValues.FirstOrDefault(x => x.Name == "interaction_code");
            interactionCodeFormValue.Label.Should().Be("Interaction Code");
            interactionCodeFormValue.Required.Should().BeTrue();
            interactionCodeFormValue.GetProperty<string>("value").Should()
                .Be("dUJHngn125_NsSyr86zpFZg8PJWNUgzAKK88GKw781U");

            var clientIdFormValue = successFormValues.FirstOrDefault(x => x.Name == "client_id");
            clientIdFormValue.Label.Should().Be("Client Id");
            clientIdFormValue.Required.Should().BeTrue();
            clientIdFormValue.GetProperty<string>("value").Should().Be("000000");

        }
    }
}
