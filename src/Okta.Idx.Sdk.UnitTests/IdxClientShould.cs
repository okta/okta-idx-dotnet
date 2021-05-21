using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Okta.Sdk.Abstractions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Okta.Idx.Sdk.UnitTests
{
    using Microsoft.Extensions.Logging;
    using NSubstitute;
    using System.Collections.Generic;
    using System.Net.Http;

    public class IdxClientShould
    {
        [Fact]
        public async Task LoginSuccessfullyWithOneStepLoginConfiguration()
        {

            #region mocks

            var interactResponse = @"{ 'interaction_handle' : 'foo' }";
            var introspectResponse = @"{
                                       ""version"":""1.0.0"",
                                       ""stateHandle"":""026g-m0qr-bk2jbZ5bECFOVCEbnS6I9uXUFjLEn63y"",
                                       ""expiresAt"":""2021-05-14T18:32:10.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""remediation"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""rel"":[
                                                   ""create-form""
                                                ],
                                                ""name"":""identify"",
                                                ""href"":""https://foo.okta.com/idp/idx/identify"",
                                                ""method"":""POST"",
                                                ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                ""value"":[
                                                   {
                                                      ""name"":""identifier"",
                                                      ""label"":""Username""
                                                   },
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
                                                      ""name"":""rememberMe"",
                                                      ""type"":""boolean"",
                                                      ""label"":""Remember this device""
                                                   },
                                                   {
                                                      ""name"":""stateHandle"",
                                                      ""required"":true,
                                                      ""value"":""026g-m0qr-bk2jbZ5bECFOVCEbnS6I9uXUFjLEn63y"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             }
                                          ]
                                       },
                                       ""currentAuthenticator"":{
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
                                                      ""value"":""026g-m0qr-bk2jbZ5bECFOVCEbnS6I9uXUFjLEn63y"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             },
                                             ""type"":""password"",
                                             ""key"":""okta_password"",
                                             ""id"":""autksbgegGSb3LW2j5d6"",
                                             ""displayName"":""Password"",
                                             ""methods"":[
                                                {
                                                   ""type"":""password""
                                                }
                                             ]
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
                                                ""value"":""026g-m0qr-bk2jbZ5bECFOVCEbnS6I9uXUFjLEn63y"",
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
                                             ""label"":""Laura - My Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";
            var identifyResponse = @"{
                                       ""version"":""1.0.0"",
                                       ""stateHandle"":""026g-m0qr-bk2jbZ5bECFOVCEbnS6I9uXUFjLEn63y"",
                                       ""expiresAt"":""2021-05-14T16:33:13.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""user"":{
                                          ""type"":""object"",
                                          ""value"":{
                                             ""id"":""00un2onoa5JhS00qz5d6""
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
                                                ""value"":""026g-m0qr-bk2jbZ5bECFOVCEbnS6I9uXUFjLEn63y"",
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
                                             ""label"":""Laura - My Web App"",
                                             ""id"":""foo""
                                          }
                                       },
                                       ""successWithInteractionCode"":{
                                          ""rel"":[
                                             ""create-form""
                                          ],
                                          ""name"":""issue"",
                                          ""href"":""https://foo.okta.com/oauth2/foo/v1/token"",
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
                                                ""value"":""GYpD6HSoaV2gXTyuyp54bwg_82sQcgALL2aIZjNAZU8""
                                             },
                                             {
                                                ""name"":""client_id"",
                                                ""required"":true,
                                                ""value"":""foo""
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
            var tokenResponse = @"{
                                   ""token_type"":""Bearer"",
                                   ""expires_in"":3600,
                                   ""access_token"":""eyJraWQiOiJuV0xV"",
                                   ""scope"":""openid profile"",
                                   ""id_token"":""eyJraWQiOiJuV0xVc05v""
                                }";

            #endregion

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = tokenResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.AuthenticateAsync(
                new AuthenticationOptions
                    {
                        Username = "user@mail.com", 
                        Password = "P4zzw0rd"
                    });

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.Success);
            authResponse.TokenInfo.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task LoginSuccessfullyWithTwoStepsLoginConfiguration()
        {

            #region mocks

            var interactResponse = @"{ 'interaction_handle' : 'foo' }";
            var introspectResponse = @"{
                                       ""version"":""1.0.0"",
                                       ""stateHandle"":""021ASG5DWgfcbJZ5jRU_KzF-K-6EHgE_Cq9o48TEZ4"",
                                       ""expiresAt"":""2021-05-17T18:11:34.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""remediation"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""rel"":[
                                                   ""create-form""
                                                ],
                                                ""name"":""identify"",
                                                ""href"":""https://foo.okta.com/idp/idx/identify"",
                                                ""method"":""POST"",
                                                ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                ""value"":[
                                                   {
                                                      ""name"":""identifier"",
                                                      ""label"":""Username""
                                                   },
                                                   {
                                                      ""name"":""rememberMe"",
                                                      ""type"":""boolean"",
                                                      ""label"":""Remember this device""
                                                   },
                                                   {
                                                      ""name"":""stateHandle"",
                                                      ""required"":true,
                                                      ""value"":""021ASG5DWgfcbJZ5jRU_KzF-K-6EHgE_Cq9o48TEZ4"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             }
                                          ]
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
                                                ""value"":""021ASG5DWgfcbJZ5jRU_KzF-K-6EHgE_Cq9o48TEZ4"",
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
                                             ""label"":""Laura - My Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";
            var identifyResponse = @"{
                                       ""version"":""1.0.0"",
                                       ""stateHandle"":""021ASG5DWgfcbJZ5jRU_KzF-K-6EHgE_Cq9o48TEZ4"",
                                       ""expiresAt"":""2021-05-17T16:16:35.000Z"",
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
                                                      ""value"":""021ASG5DWgfcbJZ5jRU_KzF-K-6EHgE_Cq9o48TEZ4"",
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
                                                      ""value"":""021ASG5DWgfcbJZ5jRU_KzF-K-6EHgE_Cq9o48TEZ4"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             },
                                             ""type"":""password"",
                                             ""key"":""okta_password"",
                                             ""id"":""lae1g3ciasfOrw7Bl5d6"",
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
                                                ""id"":""autksbgegGSb3LW2j5d6"",
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
                                                ""id"":""lae1g3ciasfOrw7Bl5d6"",
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
                                             ""id"":""foo""
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
                                                ""value"":""021ASG5DWgfcbJZ5jRU_KzF-K-6EHgE_Cq9o48TEZ4"",
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
                                             ""label"":""Laura - My Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";
            var challengeResponse = @"{
                                       ""version"":""1.0.0"",
                                       ""stateHandle"":""021ASG5DWgfcbJZ5jRU_KzF-K-6EHgE_Cq9o48TEZ4"",
                                       ""expiresAt"":""2021-05-17T16:12:37.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""user"":{
                                          ""type"":""object"",
                                          ""value"":{
                                             ""id"":""foo""
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
                                                ""value"":""021ASG5DWgfcbJZ5jRU_KzF-K-6EHgE_Cq9o48TEZ4"",
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
                                             ""label"":""Laura - My Web App"",
                                             ""id"":""foo""
                                          }
                                       },
                                       ""successWithInteractionCode"":{
                                          ""rel"":[
                                             ""create-form""
                                          ],
                                          ""name"":""issue"",
                                          ""href"":""https://foo.okta.com/oauth2/foo/v1/token"",
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
                                                ""value"":""u1bB0tA9bXIeHeESnDse_AvMrfEdSkfaSesemey14e4""
                                             },
                                             {
                                                ""name"":""client_id"",
                                                ""required"":true,
                                                ""value"":""foo""
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
            var tokenResponse = @"{
                                   ""token_type"":""Bearer"",
                                   ""expires_in"":3600,
                                   ""access_token"":""eyJraWQiOiJuV0xV"",
                                   ""scope"":""openid profile"",
                                   ""id_token"":""eyJraWQiOiJuV0xVc05v""
                                }";

            #endregion

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = challengeResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = tokenResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.AuthenticateAsync(
                new AuthenticationOptions
                {
                    Username = "user@mail.com",
                    Password = "P4zzw0rd"
                });

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.Success);
            authResponse.TokenInfo.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task ReturnPasswordExpiredAfterSuccessfulIdentify()
        {

            #region mocks

            var interactResponse = @"{ 'interaction_handle' : 'foo' }";
            var introspectResponse = @"{
                                       ""version"":""1.0.0"",
                                       ""stateHandle"":""026g-m0qr-bk2jbZ5bECFOVCEbnS6I9uXUFjLEn63y"",
                                       ""expiresAt"":""2021-05-14T18:32:10.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""remediation"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""rel"":[
                                                   ""create-form""
                                                ],
                                                ""name"":""identify"",
                                                ""href"":""https://foo.okta.com/idp/idx/identify"",
                                                ""method"":""POST"",
                                                ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                ""value"":[
                                                   {
                                                      ""name"":""identifier"",
                                                      ""label"":""Username""
                                                   },
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
                                                      ""name"":""rememberMe"",
                                                      ""type"":""boolean"",
                                                      ""label"":""Remember this device""
                                                   },
                                                   {
                                                      ""name"":""stateHandle"",
                                                      ""required"":true,
                                                      ""value"":""026g-m0qr-bk2jbZ5bECFOVCEbnS6I9uXUFjLEn63y"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             }
                                          ]
                                       },
                                       ""currentAuthenticator"":{
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
                                                      ""value"":""026g-m0qr-bk2jbZ5bECFOVCEbnS6I9uXUFjLEn63y"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             },
                                             ""type"":""password"",
                                             ""key"":""okta_password"",
                                             ""id"":""autksbgegGSb3LW2j5d6"",
                                             ""displayName"":""Password"",
                                             ""methods"":[
                                                {
                                                   ""type"":""password""
                                                }
                                             ]
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
                                                ""value"":""026g-m0qr-bk2jbZ5bECFOVCEbnS6I9uXUFjLEn63y"",
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
                                             ""label"":""Laura - My Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";
            var identifyResponse = @"{
                                       ""version"":""1.0.0"",
                                       ""stateHandle"":""02QnGduopjlQFbOWvKlmNtolZigSXhX9iWt8480Pum"",
                                       ""expiresAt"":""2021-05-17T21:31:10.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""remediation"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""rel"":[
                                                   ""create-form""
                                                ],
                                                ""name"":""reenroll-authenticator"",
                                                ""relatesTo"":[
                                                   ""$.currentAuthenticator""
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
                                                               ""label"":""New password"",
                                                               ""secret"":true
                                                            }
                                                         ]
                                                      },
                                                      ""required"":true
                                                   },
                                                   {
                                                      ""name"":""stateHandle"",
                                                      ""required"":true,
                                                      ""value"":""02QnGduopjlQFbOWvKlmNtolZigSXhX9iWt8480Pum"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             }
                                          ]
                                       },
                                       ""currentAuthenticator"":{
                                          ""type"":""object"",
                                          ""value"":{
                                             ""type"":""password"",
                                             ""key"":""okta_password"",
                                             ""id"":""autksbgegGSb3LW2j5d6"",
                                             ""displayName"":""Password"",
                                             ""methods"":[
                                                {
                                                   ""type"":""password""
                                                }
                                             ],
                                             ""settings"":{
                                                ""complexity"":{
                                                   ""minLength"":8,
                                                   ""minLowerCase"":0,
                                                   ""minUpperCase"":0,
                                                   ""minNumber"":0,
                                                   ""minSymbol"":0,
                                                   ""excludeUsername"":true,
                                                   ""excludeAttributes"":[
                                                      
                                                   ]
                                                },
                                                ""age"":{
                                                   ""minAgeMinutes"":0,
                                                   ""historyCount"":4
                                                }
                                             }
                                          }
                                       },
                                       ""authenticators"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""type"":""password"",
                                                ""key"":""okta_password"",
                                                ""id"":""autksbgegGSb3LW2j5d6"",
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
                                                ""type"":""email"",
                                                ""key"":""okta_email"",
                                                ""id"":""eaen2ex9xYe9UKlsr5d6"",
                                                ""displayName"":""Email"",
                                                ""methods"":[
                                                   {
                                                      ""type"":""email""
                                                   }
                                                ]
                                             },
                                             {
                                                ""type"":""password"",
                                                ""key"":""okta_password"",
                                                ""id"":""lae1g3ciasfOrw7Bl5d6"",
                                                ""displayName"":""Password"",
                                                ""methods"":[
                                                   {
                                                      ""type"":""password""
                                                   }
                                                ]
                                             },
                                             {
                                                ""type"":""phone"",
                                                ""key"":""phone_number"",
                                                ""id"":""paen2godbghmQ2Ats5d6"",
                                                ""displayName"":""Phone"",
                                                ""methods"":[
                                                   {
                                                      ""type"":""sms""
                                                   },
                                                   {
                                                      ""type"":""voice""
                                                   }
                                                ]
                                             }
                                          ]
                                       },
                                       ""recoveryAuthenticator"":{
                                          ""type"":""object"",
                                          ""value"":{
                                             ""type"":""password"",
                                             ""key"":""okta_password"",
                                             ""id"":""autksbgegGSb3LW2j5d6"",
                                             ""displayName"":""Password"",
                                             ""methods"":[
                                                {
                                                   ""type"":""password""
                                                }
                                             ],
                                             ""settings"":{
                                                ""complexity"":{
                                                   ""minLength"":8,
                                                   ""minLowerCase"":0,
                                                   ""minUpperCase"":0,
                                                   ""minNumber"":0,
                                                   ""minSymbol"":0,
                                                   ""excludeUsername"":true,
                                                   ""excludeAttributes"":[
                                                      
                                                   ]
                                                },
                                                ""age"":{
                                                   ""minAgeMinutes"":0,
                                                   ""historyCount"":4
                                                }
                                             }
                                          }
                                       },
                                       ""user"":{
                                          ""type"":""object"",
                                          ""value"":{
                                             ""id"":""00un2onoa5JhS00qz5d6""
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
                                                ""value"":""02QnGduopjlQFbOWvKlmNtolZigSXhX9iWt8480Pum"",
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
                                             ""label"":""Laura - My Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";
            
            #endregion

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.AuthenticateAsync(
                new AuthenticationOptions
                {
                    Username = "user@mail.com",
                    Password = "P4zzw0rd"
                });

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.PasswordExpired);
            authResponse.TokenInfo.Should().BeNull();
        }

        [Fact]
        public async Task ThrowRedeemInteractionCodeExceptionIfNoInteractionCodeOnRedeem()
        {
            HttpClient fakeHttpClient = Substitute.For<HttpClient>();
            ILogger fakeLogger = Substitute.For<ILogger>();
            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, fakeHttpClient, fakeLogger);

            bool exceptionWasThrown = false;
            try
            {
                IdxContext testIdxContext = new IdxContext();
                await idxClient.RedeemInteractionCodeAsync(testIdxContext, null);
            }
            catch (RedeemInteractionCodeException redeemInteractionCodeException)
            {
                Assert.IsType<ArgumentNullException>(redeemInteractionCodeException.InnerException);
                ArgumentNullException argumentNullException = (ArgumentNullException)redeemInteractionCodeException.InnerException;
                Assert.Equal("interactionCode", argumentNullException.ParamName);
                exceptionWasThrown = true;
            }
            Assert.True(exceptionWasThrown);
        }

        [Fact]
        public async Task ThrowRedeemInteractionCodeExceptionIfNoCodeVerifierOnRedeem()
        {
            HttpClient fakeHttpClient = Substitute.For<HttpClient>();
            ILogger fakeLogger = Substitute.For<ILogger>();
            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, fakeHttpClient, fakeLogger);

            bool exceptionWasThrown = false;
            try
            {
                IdxContext testIdxContext = new IdxContext();
                await idxClient.RedeemInteractionCodeAsync(testIdxContext, "not null interaction code");
            }
            catch (RedeemInteractionCodeException redeemInteractionCodeException)
            {
                Assert.IsType<ArgumentNullException>(redeemInteractionCodeException.InnerException);
                ArgumentNullException argumentNullException = (ArgumentNullException)redeemInteractionCodeException.InnerException;
                Assert.Equal("CodeVerifier", argumentNullException.ParamName);
                exceptionWasThrown = true;
            }
            Assert.True(exceptionWasThrown);
        }

        [Fact]
        public async Task ThrowRedeemInteractionCodeExceptionIfNoSuccessOnResponse()
        {
            string testResponse = @"{
   ""error"":""some error message""
}";
            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler(testResponse, System.Net.HttpStatusCode.BadRequest);
            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);
            ILogger logger = NullLogger.Instance;

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, logger);
            bool exceptionWasThrown = false;
            try
            {
                IdxContext idxContext = new IdxContext("non null code verifier", "code challenge", "code challenge method", "interaction handler", "state");
                await idxClient.RedeemInteractionCodeAsync(idxContext, "non null interaction code value");
            }
            catch (RedeemInteractionCodeException redeemInteractionCodeException)
            {
                Assert.Equal(testResponse, redeemInteractionCodeException.ApiResponse);
                exceptionWasThrown = true;
            }
            Assert.True(exceptionWasThrown);
        }

        [Fact]
        public async Task LogErrorOnRedeemInteractionCodeException()
        {
            string testResponse = @"{
   ""error"":""some error message""
}";
            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler(testResponse, System.Net.HttpStatusCode.BadRequest);
            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);
            ILogger logger = NullLogger.Instance;

            MockIdxClient idxClient = new MockIdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, logger);
            bool exceptionWasThrown = false;
            try
            {
                IdxContext idxContext = new IdxContext("non null code verifier", "code challenge", "code challenge method", "interaction handler", "state");
                await idxClient.RedeemInteractionCodeAsync(idxContext, "non null interaction code value");
            }
            catch (RedeemInteractionCodeException)
            {
                idxClient.LogErrorCallCount.Should().Be(1);
                exceptionWasThrown = true;
            }
            Assert.True(exceptionWasThrown);
        }

        [Fact]
        public async Task CallInteractOnStartSignInWidget()
        {
            string interactResponse = @"{""interaction_handle"":""this is a test interaction handle""}";
            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/oauth2/v1/interact", interactResponse);

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);

            WidgetSignInResponse widgetResponse = await idxClient.StartWidgetSignInAsync();
            Assert.NotNull(widgetResponse);
            Assert.NotNull(widgetResponse.IdxContext);
            Assert.Equal("this is a test interaction handle", widgetResponse.IdxContext.InteractionHandle);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/oauth2/v1/interact"]);
        }

    }
}
