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
        #region Basic Login
        
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
        public async Task ThrowWhenWithWrongUsername()
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
                                       ""stateHandle"":""02qojkK8c"",
                                       ""expiresAt"":""2021-05-21T21:59:12.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""messages"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""message"":""You do not have permission to perform the requested action."",
                                                ""i18n"":{
                                                   ""key"":""security.access_denied""
                                                },
                                                ""class"":""ERROR""
                                             }
                                          ]
                                       }
                                    }";
            #endregion

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 403, Response = identifyResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            Func<Task<AuthenticationResponse>> function = async () => await testClient.AuthenticateAsync(
                                      new AuthenticationOptions
                                      {
                                          Username = "wrongUsername@mail.com",
                                          Password = "P4zzw0rd"
                                      });

            await function.Should()
                .ThrowAsync<OktaException>()
                .WithMessage("*You do not have permission to perform the requested action.*");

        }

        [Fact]
        public async Task ThrowWhenWithUnassignedUser()
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
                                       ""stateHandle"":""02Clu3ID3n-BKnpbg3i6_dGHDJm54NKo4ML2IRAFMr"",
                                       ""expiresAt"":""2021-05-25T17:36:20.000Z"",
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
                                                      ""value"":""02Clu3ID3n-BKnpbg3i6_dGHDJm54NKo4ML2IRAFMr"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             }
                                          ]
                                       },
                                       ""messages"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""message"":""User is not assigned to this application"",
                                                ""class"":""ERROR""
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
                                                      ""value"":""02Clu3ID3n-BKnpbg3i6_dGHDJm54NKo4ML2IRAFMr"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             },
                                             ""type"":""password"",
                                             ""key"":""okta_password"",
                                             ""id"":""autsz6n08UcSpixTa5d6"",
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
                                                ""value"":""02Clu3ID3n-BKnpbg3i6_dGHDJm54NKo4ML2IRAFMr"",
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
                                             ""label"":""Dotnet IDX Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";
            #endregion

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 400, Response = identifyResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            Func<Task<AuthenticationResponse>> function = async () => await testClient.AuthenticateAsync(
                                      new AuthenticationOptions
                                      {
                                          Username = "unassigneduser@mail.com",
                                          Password = "P4zzw0rd"
                                      });

            await function.Should()
                                                                 .ThrowAsync<OktaException>()
                                                                 .WithMessage("*User is not assigned to this application*");

        }

        [Fact]
        public async Task ThrowWhenWithSuspendedUser()
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
                                       ""stateHandle"":""02LoMISNAG_qPyfPc6MYbq6aM_nk0mY-IUnRY0XrF2"",
                                       ""expiresAt"":""2021-05-25T18:31:48.000Z"",
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
                                                      ""value"":""02LoMISNAG_qPyfPc6MYbq6aM_nk0mY-IUnRY0XrF2"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             }
                                          ]
                                       },
                                       ""messages"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""message"":""Authentication failed"",
                                                ""i18n"":{
                                                   ""key"":""errors.E0000004""
                                                },
                                                ""class"":""ERROR""
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
                                                      ""value"":""02LoMISNAG_qPyfPc6MYbq6aM_nk0mY-IUnRY0XrF2"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             },
                                             ""type"":""password"",
                                             ""key"":""okta_password"",
                                             ""id"":""autsz6n08UcSpixTa5d6"",
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
                                                ""value"":""02LoMISNAG_qPyfPc6MYbq6aM_nk0mY-IUnRY0XrF2"",
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
                                             ""label"":""Dotnet IDX Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";
            #endregion

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 400, Response = identifyResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            Func<Task<AuthenticationResponse>> function = async () => await testClient.AuthenticateAsync(
                                      new AuthenticationOptions
                                      {
                                          Username = "suspendeduser@mail.com",
                                          Password = "P4zzw0rd"
                                      });

            await function.Should()
                                                                 .ThrowAsync<OktaException>()
                                                                 .WithMessage("*Authentication Failed*");

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
        public async Task ThrowWhenWithLockedUser()
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
                                       ""stateHandle"":""02HR9wnRePFrXEW5si4KgbSMec5ehRbmhGhyv3PfZ0"",
                                       ""expiresAt"":""2021-05-25T18:51:08.000Z"",
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
                                                      ""value"":""02HR9wnRePFrXEW5si4KgbSMec5ehRbmhGhyv3PfZ0"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             },
                                             {
                                                ""rel"":[
                                                   ""create-form""
                                                ],
                                                ""name"":""select-enroll-profile"",
                                                ""href"":""https://foo.okta.com/idp/idx/enroll"",
                                                ""method"":""POST"",
                                                ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                ""value"":[
                                                   {
                                                      ""name"":""stateHandle"",
                                                      ""required"":true,
                                                      ""value"":""02HR9wnRePFrXEW5si4KgbSMec5ehRbmhGhyv3PfZ0"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             }
                                          ]
                                       },
                                       ""messages"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""message"":""Authentication failed"",
                                                ""i18n"":{
                                                   ""key"":""errors.E0000004""
                                                },
                                                ""class"":""ERROR""
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
                                                      ""value"":""02HR9wnRePFrXEW5si4KgbSMec5ehRbmhGhyv3PfZ0"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             },
                                             ""type"":""password"",
                                             ""key"":""okta_password"",
                                             ""id"":""autsz6n08UcSpixTa5d6"",
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
                                                ""value"":""02HR9wnRePFrXEW5si4KgbSMec5ehRbmhGhyv3PfZ0"",
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
                                             ""label"":""Dotnet IDX Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";
            #endregion

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 400, Response = identifyResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            Func<Task<AuthenticationResponse>> function = async () => await testClient.AuthenticateAsync(
                                      new AuthenticationOptions
                                      {
                                          Username = "lockeduser@mail.com",
                                          Password = "P4zzw0rd"
                                      });

            await function.Should()
                                                                 .ThrowAsync<OktaException>()
                                                                 .WithMessage("*Authentication Failed*");

        }

        [Fact]
        public async Task ThrowWhenWithDeactivatedUser()
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
                                       ""stateHandle"":""02150KxIHJpe7suQ3gsL-V8W_yhSl8zNKlOdtsZj6d"",
                                       ""expiresAt"":""2021-05-25T19:09:21.000Z"",
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
                                                      ""value"":""02150KxIHJpe7suQ3gsL-V8W_yhSl8zNKlOdtsZj6d"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             },
                                             {
                                                ""rel"":[
                                                   ""create-form""
                                                ],
                                                ""name"":""select-enroll-profile"",
                                                ""href"":""https://foo.okta.com/idp/idx/enroll"",
                                                ""method"":""POST"",
                                                ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                ""value"":[
                                                   {
                                                      ""name"":""stateHandle"",
                                                      ""required"":true,
                                                      ""value"":""02150KxIHJpe7suQ3gsL-V8W_yhSl8zNKlOdtsZj6d"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             }
                                          ]
                                       },
                                       ""messages"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""message"":""Authentication failed"",
                                                ""i18n"":{
                                                   ""key"":""errors.E0000004""
                                                },
                                                ""class"":""ERROR""
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
                                                      ""value"":""02150KxIHJpe7suQ3gsL-V8W_yhSl8zNKlOdtsZj6d"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             },
                                             ""type"":""password"",
                                             ""key"":""okta_password"",
                                             ""id"":""autsz6n08UcSpixTa5d6"",
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
                                                ""value"":""02150KxIHJpe7suQ3gsL-V8W_yhSl8zNKlOdtsZj6d"",
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
                                             ""label"":""Dotnet IDX Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";
            #endregion

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 400, Response = identifyResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            Func<Task<AuthenticationResponse>> function = async () => await testClient.AuthenticateAsync(
                                      new AuthenticationOptions
                                      {
                                          Username = "deactivateduser@mail.com",
                                          Password = "P4zzw0rd"
                                      });

            await function.Should()
                                                                 .ThrowAsync<OktaException>()
                                                                 .WithMessage("*Authentication Failed*");

        }

        #endregion

        #region Self-Hosted SIW

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

        [Fact]
        public async Task CallInteractAndIntrospectOnGetIdentityProviders()
        {
            #region mockResponses
            string interactResponse = @"{ ""interaction_handle"":""AcSDZw_kwcQtFUwLAgyUyBzl_OifS5Nn6IZQ_X1WXWI""}";
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02WGhu-hBqJ0GVV8IjFiAU18m0fFKWv9sFNQ-E_-bP"",
    ""expiresAt"": ""2021-05-10T18:13:24.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""name"": ""redirect-idp"",
                ""type"": ""FACEBOOK"",
                ""idp"": {
                    ""id"": ""test-facebook-id"",
                    ""name"": ""Facebook IdP""
                },
                ""href"": ""test facebook href"",
                ""method"": ""GET""
            },
            {
                ""name"": ""redirect-idp"",
                ""type"": ""GOOGLE"",
                ""idp"": {
                    ""id"": ""test-google-id"",
                    ""name"": ""Google IdP""
                },
                ""href"": ""test google href"",
                ""method"": ""GET""
            }
        ]
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://dev-xxxx.okta.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02WGhu-hBqJ0GVV8IjFiAU18m0fFKWv9sFNQ-E_-bP"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""Test App"",
            ""id"": ""0oanjyc15Srn3JNIU5d6""
        }
    }
}";
            #endregion

            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/oauth2/v1/interact", interactResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);

            IdentityProvidersResponse identityProvidersResponse = await idxClient.GetIdentityProvidersAsync();
            Assert.NotNull(identityProvidersResponse);
            Assert.NotNull(identityProvidersResponse.IdpOptions);
            Assert.Equal(2, identityProvidersResponse.IdpOptions.Count);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/oauth2/v1/interact"]);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/idp/idx/introspect"]);
        }

        #endregion

        #region Self-Service Recovery

        [Fact]
        public async Task CallsMethodsInExpectedSequenceForTwoStepLoginOnRecoverPassword()
        {
            #region mockResponses
            string interactResponse = @"{ ""interaction_handle"":""AcSDZw_kwcQtFUwLAgyUyBzl_OifS5Nn6IZQ_X1WXWI""}";
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""027EjMMkmHhLrqupUYdusSmgSgSWVhWePaGUjT8c7q"",
    ""expiresAt"": ""2021-05-27T17:03:50.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""identify"",
                ""href"": ""https://fake.example.com/idp/idx/identify"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
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
                        ""value"": ""027EjMMkmHhLrqupUYdusSmgSgSWVhWePaGUjT8c7q"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }
        ]
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""027EjMMkmHhLrqupUYdusSmgSgSWVhWePaGUjT8c7q"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""- Unit Test Web App"",
            ""id"": ""0oatiq0j3Mw5an9Br5d6""
        }
    }
}";
            string identifyResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""027EjMMkmHhLrqupUYdusSmgSgSWVhWePaGUjT8c7q"",
    ""expiresAt"": ""2021-05-27T15:09:36.000Z"",
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
                ""href"": ""https://fake.example.com/idp/idx/challenge/answer"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
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
                        ""value"": ""027EjMMkmHhLrqupUYdusSmgSgSWVhWePaGUjT8c7q"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
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
                ""href"": ""https://fake.example.com/idp/idx/recover"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""027EjMMkmHhLrqupUYdusSmgSgSWVhWePaGUjT8c7q"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""lae1v20asJQsy3OmH5d6"",
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
                ""type"": ""password"",
                ""key"": ""okta_password"",
                ""id"": ""autkuwj37bcjlumFq5d6"",
                ""displayName"": ""Password"",
                ""methods"": [
                    {
                        ""type"": ""password""
                    }
                ]
            }
        ]
    },
    ""authenticatorEnrollments"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""password"",
                ""key"": ""okta_password"",
                ""id"": ""lae1v20asJQsy3OmH5d6"",
                ""displayName"": ""Password"",
                ""methods"": [
                    {
                        ""type"": ""password""
                    }
                ]
            }
        ]
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00utitetzQR52pdyP5d6""
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""027EjMMkmHhLrqupUYdusSmgSgSWVhWePaGUjT8c7q"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""- Unit Test Web App"",
            ""id"": ""0oatiq0j3Mw5an9Br5d6""
        }
    }
}";
            string recoverResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""027EjMMkmHhLrqupUYdusSmgSgSWVhWePaGUjT8c7q"",
    ""expiresAt"": ""2021-05-27T15:09:39.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-authenticate"",
                ""href"": ""https://fake.example.com/idp/idx/challenge"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
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
                                                ""value"": ""autkuwj38Z23hZvgN5d6"",
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
                        ""value"": ""027EjMMkmHhLrqupUYdusSmgSgSWVhWePaGUjT8c7q"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }
        ]
    },
    ""authenticators"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""email"",
                ""key"": ""okta_email"",
                ""id"": ""autkuwj38Z23hZvgN5d6"",
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
                    ""email"": ""u***r@threeheadz.com""
                },
                ""type"": ""email"",
                ""key"": ""okta_email"",
                ""id"": ""eaetiteu0pZhRgkAo5d6"",
                ""displayName"": ""Email"",
                ""methods"": [
                    {
                        ""type"": ""email""
                    }
                ]
            }
        ]
    },
    ""recoveryAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""autkuwj37bcjlumFq5d6"",
            ""displayName"": ""Password"",
            ""methods"": [
                {
                    ""type"": ""password""
                }
            ]
        }
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00utitetzQR52pdyP5d6""
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""027EjMMkmHhLrqupUYdusSmgSgSWVhWePaGUjT8c7q"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""- Unit Test Web App"",
            ""id"": ""0oatiq0j3Mw5an9Br5d6""
        }
    }
}";
            #endregion

            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/oauth2/v1/interact", interactResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/identify", identifyResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/recover", recoverResponse);

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);

            RecoverPasswordOptions recoverPasswordOptions = new RecoverPasswordOptions { Username = "testuser@fake.com" };
            AuthenticationResponse authenticationResponse = await idxClient.RecoverPasswordAsync(recoverPasswordOptions);
            Assert.NotNull(authenticationResponse);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/oauth2/v1/interact"]);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/idp/idx/introspect"]);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/idp/idx/identify"]);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/idp/idx/recover"]);
        }

        [Fact]
        public async Task CallsMethodsInExpectedSequenceForOneStepLoginOnRecoverPassword()
        {
            #region mockResponses
            string interactResponse = @"{ ""interaction_handle"":""AcSDZw_kwcQtFUwLAgyUyBzl_OifS5Nn6IZQ_X1WXWI""}";
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02AA0jYbOK6x0oP4dtjEl24hLrKWph7A-tyOF0fFNr"",
    ""expiresAt"": ""2021-05-27T17: 29: 15.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""identify"",
                ""href"": ""https://fake.example.com/idp/idx/identify"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""identifier"",
                        ""label"": ""Username""
                    },
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
                        ""name"": ""rememberMe"",
                        ""type"": ""boolean"",
                        ""label"": ""Remember this device""
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02AA0jYbOK6x0oP4dtjEl24hLrKWph7A-tyOF0fFNr"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }
        ]
    },
    ""currentAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""recover"": {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""recover"",
                ""href"": ""https://fake.example.com/idp/idx/recover"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02AA0jYbOK6x0oP4dtjEl24hLrKWph7A-tyOF0fFNr"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""autkuwj37bcjlumFq5d6"",
            ""displayName"": ""Password"",
            ""methods"": [
                {
                    ""type"": ""password""
                }
            ]
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https: //fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02AA0jYbOK6x0oP4dtjEl24hLrKWph7A-tyOF0fFNr"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""- Unit Test Web App"",
            ""id"": ""0oatiq0j3Mw5an9Br5d6""
        }
    }
}";
            string identifyResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02AA0jYbOK6x0oP4dtjEl24hLrKWph7A-tyOF0fFNr"",
    ""expiresAt"": ""2021-05-27T15:34:29.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-authenticate"",
                ""href"": ""https://fake.example.com/idp/idx/challenge"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
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
                                                ""value"": ""autkuwj38Z23hZvgN5d6"",
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
                        ""value"": ""02AA0jYbOK6x0oP4dtjEl24hLrKWph7A-tyOF0fFNr"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }
        ]
    },
    ""authenticators"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""email"",
                ""key"": ""okta_email"",
                ""id"": ""autkuwj38Z23hZvgN5d6"",
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
                    ""email"": ""u***r@threeheadz.com""
                },
                ""type"": ""email"",
                ""key"": ""okta_email"",
                ""id"": ""eaetiteu0pZhRgkAo5d6"",
                ""displayName"": ""Email"",
                ""methods"": [
                    {
                        ""type"": ""email""
                    }
                ]
            }
        ]
    },
    ""recoveryAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""autkuwj37bcjlumFq5d6"",
            ""displayName"": ""Password"",
            ""methods"": [
                {
                    ""type"": ""password""
                }
            ]
        }
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00utitetzQR52pdyP5d6""
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02AA0jYbOK6x0oP4dtjEl24hLrKWph7A-tyOF0fFNr"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""- Unit Test Web App"",
            ""id"": ""0oatiq0j3Mw5an9Br5d6""
        }
    }
}";
            string recoverResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02AA0jYbOK6x0oP4dtjEl24hLrKWph7A-tyOF0fFNr"",
    ""expiresAt"": ""2021-05-27T17:29:21.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""identify-recovery"",
                ""href"": ""https://fake.example.com/idp/idx/identify"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""identifier"",
                        ""label"": ""Username""
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02AA0jYbOK6x0oP4dtjEl24hLrKWph7A-tyOF0fFNr"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }
        ]
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02AA0jYbOK6x0oP4dtjEl24hLrKWph7A-tyOF0fFNr"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""- Unit Test Web App"",
            ""id"": ""0oatiq0j3Mw5an9Br5d6""
        }
    }
}";
            #endregion

            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/oauth2/v1/interact", interactResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/identify", identifyResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/recover", recoverResponse);

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);

            RecoverPasswordOptions recoverPasswordOptions = new RecoverPasswordOptions { Username = "testuser@fake.com" };
            AuthenticationResponse authenticationResponse = await idxClient.RecoverPasswordAsync(recoverPasswordOptions);
            Assert.NotNull(authenticationResponse);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/oauth2/v1/interact"]);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/idp/idx/introspect"]);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/idp/idx/identify"]);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/idp/idx/recover"]);
        }

        [Fact]
        public async Task SelectsEmailToRecoverPassword()
        {
            #region mockResponses
            string interactResponse = @"{ ""interaction_handle"":""AcSDZw_kwcQtFUwLAgyUyBzl_OifS5Nn6IZQ_X1WXWI""}";
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02YfOiY40Pf8NGucAnaPc10ATSt1YhASX8oESR2WqC"",
    ""expiresAt"": ""2021-06-01T15:43:54.000Z"",
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
                ""href"": ""https://fake.example.com/idp/idx/challenge/answer"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
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
                        ""value"": ""02YfOiY40Pf8NGucAnaPc10ATSt1YhASX8oESR2WqC"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
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
                ""href"": ""https://fake.example.com/idp/idx/recover"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02YfOiY40Pf8NGucAnaPc10ATSt1YhASX8oESR2WqC"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""lae1xknugB4xxrZDS5d6"",
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
                ""type"": ""password"",
                ""key"": ""okta_password"",
                ""id"": ""auttzfsi2fKQlZVl15d6"",
                ""displayName"": ""Password"",
                ""methods"": [
                    {
                        ""type"": ""password""
                    }
                ]
            }
        ]
    },
    ""authenticatorEnrollments"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""password"",
                ""key"": ""okta_password"",
                ""id"": ""lae1xknugB4xxrZDS5d6"",
                ""displayName"": ""Password"",
                ""methods"": [
                    {
                        ""type"": ""password""
                    }
                ]
            }
        ]
    },
    ""recoveryAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""auttzfsi2fKQlZVl15d6"",
            ""displayName"": ""Password"",
            ""methods"": [
                {
                    ""type"": ""password""
                }
            ]
        }
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uuh90p3LLkky72y5d6""
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02YfOiY40Pf8NGucAnaPc10ATSt1YhASX8oESR2WqC"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""Dotnet IDX Web App"",
            ""id"": ""0oatzfskmLm4faAaQ5d6""
        }
    }
}";
            string recoverResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02YfOiY40Pf8NGucAnaPc10ATSt1YhASX8oESR2WqC"",
    ""expiresAt"": ""2021-06-01T15:44:44.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-authenticate"",
                ""href"": ""https://fake.example.com/idp/idx/challenge"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
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
                                                ""value"": ""auttzfsi3IuMuCpwD5d6"",
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
                        ""value"": ""02YfOiY40Pf8NGucAnaPc10ATSt1YhASX8oESR2WqC"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }
        ]
    },
    ""authenticators"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""email"",
                ""key"": ""okta_email"",
                ""id"": ""auttzfsi3IuMuCpwD5d6"",
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
                    ""email"": ""r***r@threeheadz.com""
                },
                ""type"": ""email"",
                ""key"": ""okta_email"",
                ""id"": ""eaeuh90p4RZvNWebB5d6"",
                ""displayName"": ""Email"",
                ""methods"": [
                    {
                        ""type"": ""email""
                    }
                ]
            }
        ]
    },
    ""recoveryAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""auttzfsi2fKQlZVl15d6"",
            ""displayName"": ""Password"",
            ""methods"": [
                {
                    ""type"": ""password""
                }
            ]
        }
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uuh90p3LLkky72y5d6""
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02YfOiY40Pf8NGucAnaPc10ATSt1YhASX8oESR2WqC"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""Dotnet IDX Web App"",
            ""id"": ""0oatzfskmLm4faAaQ5d6""
        }
    }
}";
            string challengeResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02YfOiY40Pf8NGucAnaPc10ATSt1YhASX8oESR2WqC"",
    ""expiresAt"": ""2021-06-01T15:44:44.000Z"",
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
                ""href"": ""https://fake.example.com/idp/idx/challenge/answer"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
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
                        ""value"": ""02YfOiY40Pf8NGucAnaPc10ATSt1YhASX8oESR2WqC"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }
        ]
    },
    ""currentAuthenticatorEnrollment"": {
        ""type"": ""object"",
        ""value"": {
            ""profile"": {
                ""email"": ""r***r@threeheadz.com""
            },
            ""resend"": {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""resend"",
                ""href"": ""https://fake.example.com/idp/idx/challenge/resend"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02YfOiY40Pf8NGucAnaPc10ATSt1YhASX8oESR2WqC"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""poll"": {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""poll"",
                ""href"": ""https://fake.example.com/idp/idx/challenge/poll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""refresh"": 4000,
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02YfOiY40Pf8NGucAnaPc10ATSt1YhASX8oESR2WqC"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""type"": ""email"",
            ""key"": ""okta_email"",
            ""id"": ""eaeuh90p4RZvNWebB5d6"",
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
                ""key"": ""okta_email"",
                ""id"": ""auttzfsi3IuMuCpwD5d6"",
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
                    ""email"": ""r***r@threeheadz.com""
                },
                ""type"": ""email"",
                ""key"": ""okta_email"",
                ""id"": ""eaeuh90p4RZvNWebB5d6"",
                ""displayName"": ""Email"",
                ""methods"": [
                    {
                        ""type"": ""email""
                    }
                ]
            }
        ]
    },
    ""recoveryAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""auttzfsi2fKQlZVl15d6"",
            ""displayName"": ""Password"",
            ""methods"": [
                {
                    ""type"": ""password""
                }
            ]
        }
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uuh90p3LLkky72y5d6""
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02YfOiY40Pf8NGucAnaPc10ATSt1YhASX8oESR2WqC"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""Dotnet IDX Web App"",
            ""id"": ""0oatzfskmLm4faAaQ5d6""
        }
    }
}";

            string testAuthenticatorId = "auttzfsi3IuMuCpwD5d6";
            #endregion

            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/oauth2/v1/interact", interactResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/recover", recoverResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/challenge", challengeResponse);
            
            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);
            AuthenticationResponse applyAuthenticatorResponse = await idxClient.SelectRecoveryAuthenticatorAsync(
                                                    new SelectAuthenticatorOptions { AuthenticatorId = testAuthenticatorId },
                                                    new IdxContext());

            Assert.Equal(AuthenticationStatus.AwaitingAuthenticatorVerification, applyAuthenticatorResponse.AuthenticationStatus);
        }

        [Fact]
        public async Task VerifyEmailToRecoverPassword()
        {
            #region mockResponses
            string interactResponse = @"{ ""interaction_handle"":""AcSDZw_kwcQtFUwLAgyUyBzl_OifS5Nn6IZQ_X1WXWI""}";
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02-kPJJDNxHMj4-nFhyzJZH_ZnSgEWX00X_jYUFP8G"",
    ""expiresAt"": ""2021-06-01T16:17:23.000Z"",
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
                ""href"": ""https://fake.example.com/idp/idx/challenge/answer"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
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
                        ""value"": ""02-kPJJDNxHMj4-nFhyzJZH_ZnSgEWX00X_jYUFP8G"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }
        ]
    },
    ""currentAuthenticatorEnrollment"": {
        ""type"": ""object"",
        ""value"": {
            ""profile"": {
                ""email"": ""r***2@threeheadz.com""
            },
            ""resend"": {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""resend"",
                ""href"": ""https://fake.example.com/idp/idx/challenge/resend"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02-kPJJDNxHMj4-nFhyzJZH_ZnSgEWX00X_jYUFP8G"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""poll"": {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""poll"",
                ""href"": ""https://fake.example.com/idp/idx/challenge/poll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""refresh"": 4000,
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02-kPJJDNxHMj4-nFhyzJZH_ZnSgEWX00X_jYUFP8G"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""type"": ""email"",
            ""key"": ""okta_email"",
            ""id"": ""eaeuhtvnarGiKdWkr5d6"",
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
                ""key"": ""okta_email"",
                ""id"": ""autkuwj38Z23hZvgN5d6"",
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
                    ""email"": ""r***2@threeheadz.com""
                },
                ""type"": ""email"",
                ""key"": ""okta_email"",
                ""id"": ""eaeuhtvnarGiKdWkr5d6"",
                ""displayName"": ""Email"",
                ""methods"": [
                    {
                        ""type"": ""email""
                    }
                ]
            }
        ]
    },
    ""recoveryAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""autkuwj37bcjlumFq5d6"",
            ""displayName"": ""Password"",
            ""methods"": [
                {
                    ""type"": ""password""
                }
            ]
        }
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uuhtvn9oIBAHaje5d6""
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02-kPJJDNxHMj4-nFhyzJZH_ZnSgEWX00X_jYUFP8G"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""Bryan Apellanes Unit Test Web App"",
            ""id"": ""0oatiq0j3Mw5an9Br5d6""
        }
    }
}";
            string challengeAnswerResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02-kPJJDNxHMj4-nFhyzJZH_ZnSgEWX00X_jYUFP8G"",
    ""expiresAt"": ""2021-06-01T16:18:16.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""reset-authenticator"",
                ""relatesTo"": [
                    ""$.currentAuthenticator""
                ],
                ""href"": ""https://fake.example.com/idp/idx/challenge/answer"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""credentials"",
                        ""type"": ""object"",
                        ""form"": {
                            ""value"": [
                                {
                                    ""name"": ""passcode"",
                                    ""label"": ""New password"",
                                    ""secret"": true
                                }
                            ]
                        },
                        ""required"": true
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02-kPJJDNxHMj4-nFhyzJZH_ZnSgEWX00X_jYUFP8G"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }
        ]
    },
    ""currentAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""autkuwj37bcjlumFq5d6"",
            ""displayName"": ""Password"",
            ""methods"": [
                {
                    ""type"": ""password""
                }
            ],
            ""settings"": {
                ""complexity"": {
                    ""minLength"": 8,
                    ""minLowerCase"": 1,
                    ""minUpperCase"": 1,
                    ""minNumber"": 1,
                    ""minSymbol"": 0,
                    ""excludeUsername"": true,
                    ""excludeAttributes"": []
                },
                ""age"": {
                    ""minAgeMinutes"": 0,
                    ""historyCount"": 4
                }
            }
        }
    },
    ""authenticators"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""password"",
                ""key"": ""okta_password"",
                ""id"": ""autkuwj37bcjlumFq5d6"",
                ""displayName"": ""Password"",
                ""methods"": [
                    {
                        ""type"": ""password""
                    }
                ]
            }
        ]
    },
    ""authenticatorEnrollments"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""email"",
                ""key"": ""okta_email"",
                ""id"": ""eaeuhtvnarGiKdWkr5d6"",
                ""displayName"": ""Email"",
                ""methods"": [
                    {
                        ""type"": ""email""
                    }
                ]
            },
            {
                ""type"": ""password"",
                ""key"": ""okta_password"",
                ""id"": ""lae1xly261ebvEdcL5d6"",
                ""displayName"": ""Password"",
                ""methods"": [
                    {
                        ""type"": ""password""
                    }
                ]
            },
            {
                ""type"": ""security_question"",
                ""key"": ""security_question"",
                ""id"": ""qaeuhtvngzUf3jXpz5d6"",
                ""displayName"": ""Security Question"",
                ""methods"": [
                    {
                        ""type"": ""security_question""
                    }
                ]
            }
        ]
    },
    ""recoveryAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""autkuwj37bcjlumFq5d6"",
            ""displayName"": ""Password"",
            ""methods"": [
                {
                    ""type"": ""password""
                }
            ],
            ""settings"": {
                ""complexity"": {
                    ""minLength"": 8,
                    ""minLowerCase"": 1,
                    ""minUpperCase"": 1,
                    ""minNumber"": 1,
                    ""minSymbol"": 0,
                    ""excludeUsername"": true,
                    ""excludeAttributes"": []
                },
                ""age"": {
                    ""minAgeMinutes"": 0,
                    ""historyCount"": 4
                }
            }
        }
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uuhtvn9oIBAHaje5d6""
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02-kPJJDNxHMj4-nFhyzJZH_ZnSgEWX00X_jYUFP8G"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""Bryan Apellanes Unit Test Web App"",
            ""id"": ""0oatiq0j3Mw5an9Br5d6""
        }
    }
}";
            #endregion

            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/oauth2/v1/interact", interactResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/challenge/answer", challengeAnswerResponse);
            
            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            var verifyAuthenticatorOptions = new VerifyAuthenticatorOptions
            {
                Code = "12345",
            };
            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);
            AuthenticationResponse authnResponse = await idxClient.VerifyAuthenticatorAsync(verifyAuthenticatorOptions, new IdxContext());

            Assert.Equal(AuthenticationStatus.AwaitingPasswordReset, authnResponse.AuthenticationStatus);
        }

        [Fact]
        public async Task ResetPassword()
        {
            #region mockResponses
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02n7XX7W94pY8QZblFexcnHGR5XZNN_Qp3gVYDTFEv"",
    ""expiresAt"": ""2021-06-01T19:01:36.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""reset-authenticator"",
                ""relatesTo"": [
                    ""$.currentAuthenticator""
                ],
                ""href"": ""https://fake.example.com/idp/idx/challenge/answer"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""credentials"",
                        ""type"": ""object"",
                        ""form"": {
                            ""value"": [
                                {
                                    ""name"": ""passcode"",
                                    ""label"": ""New password"",
                                    ""secret"": true
                                }
                            ]
                        },
                        ""required"": true
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02n7XX7W94pY8QZblFexcnHGR5XZNN_Qp3gVYDTFEv"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }
        ]
    },
    ""currentAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""auttzfsi2fKQlZVl15d6"",
            ""displayName"": ""Password"",
            ""methods"": [
                {
                    ""type"": ""password""
                }
            ],
            ""settings"": {
                ""complexity"": {
                    ""minLength"": 8,
                    ""minLowerCase"": 1,
                    ""minUpperCase"": 1,
                    ""minNumber"": 1,
                    ""minSymbol"": 0,
                    ""excludeUsername"": true,
                    ""excludeAttributes"": []
                },
                ""age"": {
                    ""minAgeMinutes"": 0,
                    ""historyCount"": 4
                }
            }
        }
    },
    ""authenticators"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""password"",
                ""key"": ""okta_password"",
                ""id"": ""auttzfsi2fKQlZVl15d6"",
                ""displayName"": ""Password"",
                ""methods"": [
                    {
                        ""type"": ""password""
                    }
                ]
            }
        ]
    },
    ""authenticatorEnrollments"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""email"",
                ""key"": ""okta_email"",
                ""id"": ""eaeuh90p4RZvNWebB5d6"",
                ""displayName"": ""Email"",
                ""methods"": [
                    {
                        ""type"": ""email""
                    }
                ]
            },
            {
                ""type"": ""password"",
                ""key"": ""okta_password"",
                ""id"": ""lae1xknugB4xxrZDS5d6"",
                ""displayName"": ""Password"",
                ""methods"": [
                    {
                        ""type"": ""password""
                    }
                ]
            }
        ]
    },
    ""recoveryAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""auttzfsi2fKQlZVl15d6"",
            ""displayName"": ""Password"",
            ""methods"": [
                {
                    ""type"": ""password""
                }
            ],
            ""settings"": {
                ""complexity"": {
                    ""minLength"": 8,
                    ""minLowerCase"": 1,
                    ""minUpperCase"": 1,
                    ""minNumber"": 1,
                    ""minSymbol"": 0,
                    ""excludeUsername"": true,
                    ""excludeAttributes"": []
                },
                ""age"": {
                    ""minAgeMinutes"": 0,
                    ""historyCount"": 4
                }
            }
        }
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uuh90p3LLkky72y5d6""
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02n7XX7W94pY8QZblFexcnHGR5XZNN_Qp3gVYDTFEv"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""Dotnet IDX Web App"",
            ""id"": ""0oatzfskmLm4faAaQ5d6""
        }
    }
}";
            string challengeAnswerResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02n7XX7W94pY8QZblFexcnHGR5XZNN_Qp3gVYDTFEv"",
    ""expiresAt"": ""2021-06-01T18:58:15.000Z"",
    ""intent"": ""LOGIN"",
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uuh90p3LLkky72y5d6""
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://fake.example.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02n7XX7W94pY8QZblFexcnHGR5XZNN_Qp3gVYDTFEv"",
                ""visible"": false,
                ""mutable"": false
            }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
    },
    ""app"": {
        ""type"": ""object"",
        ""value"": {
            ""name"": ""oidc_client"",
            ""label"": ""Dotnet IDX Web App"",
            ""id"": ""0oatzfskmLm4faAaQ5d6""
        }
    },
    ""successWithInteractionCode"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""issue"",
        ""href"": ""https://fake.example.com/oauth2/austyqkbjaFoOxkl45d6/v1/token"",
        ""method"": ""POST"",
        ""value"": [
            {
                ""name"": ""grant_type"",
                ""required"": true,
                ""value"": ""interaction_code""
            },
            {
                ""name"": ""interaction_code"",
                ""required"": true,
                ""value"": ""T6J_vuM3rcvXxU8Awrx2uxE56bBXMDvcBZ9Z03gz-zk""
            },
            {
                ""name"": ""client_id"",
                ""required"": true,
                ""value"": ""0oatzfskmLm4faAaQ5d6""
            },
            {
                ""name"": ""client_secret"",
                ""required"": true
            },
            {
                ""name"": ""code_verifier"",
                ""required"": true
            }
        ],
        ""accepts"": ""application/x-www-form-urlencoded""
    }
}";
            string tokenResponse = @"{
    ""token_type"": ""Bearer"",
    ""expires_in"": 3600,
    ""access_token"": ""test access token"",
    ""scope"": ""profile openid"",
    ""id_token"": ""test id token""
}";
            #endregion

            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/challenge/answer", challengeAnswerResponse);
            mockHttpMessageHandler.AddTestResponse("/oauth2/austyqkbjaFoOxkl45d6/v1/token", tokenResponse);
            
            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);
            AuthenticationResponse authnResponse = await idxClient.ChangePasswordAsync(new ChangePasswordOptions()
            {
                NewPassword = "Abcd12345",
            }, new IdxContext());

            Assert.Equal(AuthenticationStatus.Success, authnResponse.AuthenticationStatus);
            Assert.NotNull(authnResponse.TokenInfo);
            Assert.Equal("test access token", authnResponse.TokenInfo.AccessToken);
            Assert.Equal("test id token", authnResponse.TokenInfo.IdToken);
        }

        #endregion

        #region 2FA Tests

        /****** BEGIN SCENARIOS 6.1.X ******/
        [Fact]
        public async Task ReturnAwaitingForAuthenticatorSelectionWhenLoginWith2FA()
        {
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
                                       ""stateHandle"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                       ""expiresAt"":""2021-05-26T16:28:02.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""remediation"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""rel"":[
                                                   ""create-form""
                                                ],
                                                ""name"":""select-authenticator-authenticate"",
                                                ""href"":""https://foo.com/idp/idx/challenge"",
                                                ""method"":""POST"",
                                                ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                ""value"":[
                                                   {
                                                      ""name"":""authenticator"",
                                                      ""type"":""object"",
                                                      ""options"":[
                                                         {
                                                            ""label"":""Email"",
                                                            ""value"":{
                                                               ""form"":{
                                                                  ""value"":[
                                                                     {
                                                                        ""name"":""id"",
                                                                        ""required"":true,
                                                                        ""value"":""autsz6n09SkHhW8uT5d6"",
                                                                        ""mutable"":false
                                                                     },
                                                                     {
                                                                        ""name"":""methodType"",
                                                                        ""required"":false,
                                                                        ""value"":""email"",
                                                                        ""mutable"":false
                                                                     }
                                                                  ]
                                                               }
                                                            },
                                                            ""relatesTo"":""$.authenticatorEnrollments.value[0]""
                                                         }
                                                      ]
                                                   },
                                                   {
                                                      ""name"":""stateHandle"",
                                                      ""required"":true,
                                                      ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             }
                                          ]
                                       },
                                       ""authenticators"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""type"":""email"",
                                                ""key"":""okta_email"",
                                                ""id"":""autsz6n09SkHhW8uT5d6"",
                                                ""displayName"":""Email"",
                                                ""methods"":[
                                                   {
                                                      ""type"":""email""
                                                   }
                                                ]
                                             }
                                          ]
                                       },
                                       ""authenticatorEnrollments"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""profile"":{
                                                   ""email"":""l***a@mailinator.com""
                                                },
                                                ""type"":""email"",
                                                ""key"":""okta_email"",
                                                ""id"":""eaetifxy6IiGvOxPq5d6"",
                                                ""displayName"":""Email"",
                                                ""methods"":[
                                                   {
                                                      ""type"":""email""
                                                   }
                                                ]
                                             }
                                          ]
                                       },
                                       ""user"":{
                                          ""type"":""object"",
                                          ""value"":{
                                             ""id"":""00utifxy5ivSnQVgn5d6""
                                          }
                                       },
                                       ""cancel"":{
                                          ""rel"":[
                                             ""create-form""
                                          ],
                                          ""name"":""cancel"",
                                          ""href"":""https://foo.com/idp/idx/cancel"",
                                          ""method"":""POST"",
                                          ""produces"":""application/ion+json; okta-version=1.0.0"",
                                          ""value"":[
                                             {
                                                ""name"":""stateHandle"",
                                                ""required"":true,
                                                ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
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
                                             ""label"":""Dotnet IDX Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.AuthenticateAsync(
                                   new AuthenticationOptions()
                                       {
                                           Username = "user@test.com",
                                           Password = "p4zzw0rd"
                                       });

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingChallengeAuthenticatorSelection);
            authResponse.TokenInfo.Should().BeNull();
        }

        [Fact]
        public async Task SelectEmailAsSecondFactor()
        {
            var identifyResponse = @"{
                                       ""version"":""1.0.0"",
                                       ""stateHandle"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                       ""expiresAt"":""2021-05-26T16:28:02.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""remediation"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""rel"":[
                                                   ""create-form""
                                                ],
                                                ""name"":""select-authenticator-authenticate"",
                                                ""href"":""https://foo.com/idp/idx/challenge"",
                                                ""method"":""POST"",
                                                ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                ""value"":[
                                                   {
                                                      ""name"":""authenticator"",
                                                      ""type"":""object"",
                                                      ""options"":[
                                                         {
                                                            ""label"":""Email"",
                                                            ""value"":{
                                                               ""form"":{
                                                                  ""value"":[
                                                                     {
                                                                        ""name"":""id"",
                                                                        ""required"":true,
                                                                        ""value"":""autsz6n09SkHhW8uT5d6"",
                                                                        ""mutable"":false
                                                                     },
                                                                     {
                                                                        ""name"":""methodType"",
                                                                        ""required"":false,
                                                                        ""value"":""email"",
                                                                        ""mutable"":false
                                                                     }
                                                                  ]
                                                               }
                                                            },
                                                            ""relatesTo"":""$.authenticatorEnrollments.value[0]""
                                                         }
                                                      ]
                                                   },
                                                   {
                                                      ""name"":""stateHandle"",
                                                      ""required"":true,
                                                      ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             }
                                          ]
                                       },
                                       ""authenticators"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""type"":""email"",
                                                ""key"":""okta_email"",
                                                ""id"":""autsz6n09SkHhW8uT5d6"",
                                                ""displayName"":""Email"",
                                                ""methods"":[
                                                   {
                                                      ""type"":""email""
                                                   }
                                                ]
                                             }
                                          ]
                                       },
                                       ""authenticatorEnrollments"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""profile"":{
                                                   ""email"":""l***a@mailinator.com""
                                                },
                                                ""type"":""email"",
                                                ""key"":""okta_email"",
                                                ""id"":""eaetifxy6IiGvOxPq5d6"",
                                                ""displayName"":""Email"",
                                                ""methods"":[
                                                   {
                                                      ""type"":""email""
                                                   }
                                                ]
                                             }
                                          ]
                                       },
                                       ""user"":{
                                          ""type"":""object"",
                                          ""value"":{
                                             ""id"":""00utifxy5ivSnQVgn5d6""
                                          }
                                       },
                                       ""cancel"":{
                                          ""rel"":[
                                             ""create-form""
                                          ],
                                          ""name"":""cancel"",
                                          ""href"":""https://foo.com/idp/idx/cancel"",
                                          ""method"":""POST"",
                                          ""produces"":""application/ion+json; okta-version=1.0.0"",
                                          ""value"":[
                                             {
                                                ""name"":""stateHandle"",
                                                ""required"":true,
                                                ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
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
                                             ""label"":""Dotnet IDX Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";
            var selectEmailResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                           ""expiresAt"":""2021-05-26T16:28:10.000Z"",
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
                                                    ""href"":""https://foo.com/idp/idx/challenge/answer"",
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
                                                                   ""label"":""Enter code""
                                                                }
                                                             ]
                                                          },
                                                          ""required"":true
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
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
                                                 ""profile"":{
                                                    ""email"":""l***a@mailinator.com""
                                                 },
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo.com/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""poll"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""poll"",
                                                    ""href"":""https://foo.com/idp/idx/challenge/poll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""refresh"":4000,
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""email"",
                                                 ""key"":""okta_email"",
                                                 ""id"":""eaetifxy6IiGvOxPq5d6"",
                                                 ""displayName"":""Email"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""email""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""email"",
                                                    ""key"":""okta_email"",
                                                    ""id"":""autsz6n09SkHhW8uT5d6"",
                                                    ""displayName"":""Email"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""email""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""authenticatorEnrollments"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""profile"":{
                                                       ""email"":""l***a@mailinator.com""
                                                    },
                                                    ""type"":""email"",
                                                    ""key"":""okta_email"",
                                                    ""id"":""eaetifxy6IiGvOxPq5d6"",
                                                    ""displayName"":""Email"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""email""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""user"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""id"":""00utifxy5ivSnQVgn5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo.com/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = selectEmailResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.SelectChallengeAuthenticatorAsync(
                                   new SelectAuthenticatorOptions
                                   {
                                       AuthenticatorId = "autsz6n09SkHhW8uT5d6"
                                   }, Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingAuthenticatorVerification);
            authResponse.CurrentAuthenticatorEnrollment.Id.Should().Be("autsz6n09SkHhW8uT5d6");
            authResponse.TokenInfo.Should().BeNull();

        }

        [Fact]
        public async Task VerifyEmailAsSecondFactor()
        {
            var selectEmailResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                           ""expiresAt"":""2021-05-26T16:28:10.000Z"",
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
                                                    ""href"":""https://foo.com/idp/idx/challenge/answer"",
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
                                                                   ""label"":""Enter code""
                                                                }
                                                             ]
                                                          },
                                                          ""required"":true
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
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
                                                 ""profile"":{
                                                    ""email"":""l***a@mailinator.com""
                                                 },
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo.com/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""poll"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""poll"",
                                                    ""href"":""https://foo.com/idp/idx/challenge/poll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""refresh"":4000,
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""email"",
                                                 ""key"":""okta_email"",
                                                 ""id"":""eaetifxy6IiGvOxPq5d6"",
                                                 ""displayName"":""Email"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""email""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""email"",
                                                    ""key"":""okta_email"",
                                                    ""id"":""autsz6n09SkHhW8uT5d6"",
                                                    ""displayName"":""Email"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""email""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""authenticatorEnrollments"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""profile"":{
                                                       ""email"":""l***a@mailinator.com""
                                                    },
                                                    ""type"":""email"",
                                                    ""key"":""okta_email"",
                                                    ""id"":""eaetifxy6IiGvOxPq5d6"",
                                                    ""displayName"":""Email"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""email""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""user"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""id"":""00utifxy5ivSnQVgn5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo.com/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";
            var verifyEmailResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                           ""expiresAt"":""2021-05-26T16:24:32.000Z"",
                                           ""intent"":""LOGIN"",
                                           ""user"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""id"":""00utifxy5ivSnQVgn5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo.com/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           },
                                           ""successWithInteractionCode"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""issue"",
                                              ""href"":""https://foo.com/oauth2/foo/v1/token"",
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
                                                    ""value"":""9UQ9MbLqaPblD2EZXky_9cO3T1F46udxP77YI1PZxsY""
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
            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = selectEmailResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = verifyEmailResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = tokenResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.VerifyAuthenticatorAsync(
                                   new VerifyAuthenticatorOptions
                                       {
                                           Code = "foo"
                                       }, Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.Success);
            authResponse.TokenInfo.AccessToken.Should().NotBeNullOrEmpty();

        }

        [Fact]
        public async Task ThrowWithInvalidCodeWhenEmailIsSecondFactor()
        {
            var selectEmailResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                           ""expiresAt"":""2021-05-26T16:28:10.000Z"",
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
                                                    ""href"":""https://foo.com/idp/idx/challenge/answer"",
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
                                                                   ""label"":""Enter code""
                                                                }
                                                             ]
                                                          },
                                                          ""required"":true
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
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
                                                 ""profile"":{
                                                    ""email"":""l***a@mailinator.com""
                                                 },
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo.com/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""poll"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""poll"",
                                                    ""href"":""https://foo.com/idp/idx/challenge/poll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""refresh"":4000,
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""email"",
                                                 ""key"":""okta_email"",
                                                 ""id"":""eaetifxy6IiGvOxPq5d6"",
                                                 ""displayName"":""Email"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""email""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""email"",
                                                    ""key"":""okta_email"",
                                                    ""id"":""autsz6n09SkHhW8uT5d6"",
                                                    ""displayName"":""Email"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""email""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""authenticatorEnrollments"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""profile"":{
                                                       ""email"":""l***a@mailinator.com""
                                                    },
                                                    ""type"":""email"",
                                                    ""key"":""okta_email"",
                                                    ""id"":""eaetifxy6IiGvOxPq5d6"",
                                                    ""displayName"":""Email"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""email""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""user"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""id"":""00utifxy5ivSnQVgn5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo.com/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02UBdQctJcvYdnefRC6j0xFDBwacGgPqa7ahQ5mFho"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";
            var verifyEmailResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02_0w-np-s-tCwAkz80BXmrYAqQFY0sV3gmeAWhDge"",
                                           ""expiresAt"":""2021-05-26T17:09:00.000Z"",
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
                                                    ""href"":""https://foo.com/idp/idx/challenge/answer"",
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
                                                                   ""label"":""Enter code"",
                                                                   ""messages"":{
                                                                      ""type"":""array"",
                                                                      ""value"":[
                                                                         {
                                                                            ""message"":""Invalid code. Try again."",
                                                                            ""i18n"":{
                                                                               ""key"":""api.authn.error.PASSCODE_INVALID"",
                                                                               ""params"":[
                                                                                  
                                                                               ]
                                                                            },
                                                                            ""class"":""ERROR""
                                                                         }
                                                                      ]
                                                                   }
                                                                }
                                                             ]
                                                          },
                                                          ""required"":true
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02_0w-np-s-tCwAkz80BXmrYAqQFY0sV3gmeAWhDge"",
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
                                                 ""profile"":{
                                                    ""email"":""l***a@mailinator.com""
                                                 },
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo.com/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02_0w-np-s-tCwAkz80BXmrYAqQFY0sV3gmeAWhDge"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""poll"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""poll"",
                                                    ""href"":""https://foo.com/idp/idx/challenge/poll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""refresh"":4000,
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02_0w-np-s-tCwAkz80BXmrYAqQFY0sV3gmeAWhDge"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""email"",
                                                 ""key"":""okta_email"",
                                                 ""id"":""eaetifxy6IiGvOxPq5d6"",
                                                 ""displayName"":""Email"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""email""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""email"",
                                                    ""key"":""okta_email"",
                                                    ""id"":""autsz6n09SkHhW8uT5d6"",
                                                    ""displayName"":""Email"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""email""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""authenticatorEnrollments"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""profile"":{
                                                       ""email"":""l***a@mailinator.com""
                                                    },
                                                    ""type"":""email"",
                                                    ""key"":""okta_email"",
                                                    ""id"":""eaetifxy6IiGvOxPq5d6"",
                                                    ""displayName"":""Email"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""email""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""user"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""id"":""00utifxy5ivSnQVgn5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo.com/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02_0w-np-s-tCwAkz80BXmrYAqQFY0sV3gmeAWhDge"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = selectEmailResponse });
            queue.Enqueue(new MockResponse { StatusCode = 401, Response = verifyEmailResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            Func<Task<AuthenticationResponse>> function = async () => await testClient.AuthenticateAsync(
                                                                          new AuthenticationOptions
                                                                          {
                                                                              Username = "deactivateduser@mail.com",
                                                                              Password = "P4zzw0rd"
                                                                          });

            await function.Should()
                .ThrowAsync<OktaException>()
                .WithMessage("*Invalid code. Try again.*");

        }

        /****** END SCENARIOS 6.1.X ******/

        /****** BEGIN SCENARIOS 6.2.X ******/

        [Fact]
        public async Task SelectPhoneForEnrollment()
        {
            var identifyResponse = @"{
                                       ""version"":""1.0.0"",
                                       ""stateHandle"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                       ""expiresAt"":""2021-05-28T20:35:22.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""remediation"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""rel"":[
                                                   ""create-form""
                                                ],
                                                ""name"":""select-authenticator-enroll"",
                                                ""href"":""https://foo/idp/idx/credential/enroll"",
                                                ""method"":""POST"",
                                                ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                ""value"":[
                                                   {
                                                      ""name"":""authenticator"",
                                                      ""type"":""object"",
                                                      ""options"":[
                                                         {
                                                            ""label"":""Phone"",
                                                            ""value"":{
                                                               ""form"":{
                                                                  ""value"":[
                                                                     {
                                                                        ""name"":""id"",
                                                                        ""required"":true,
                                                                        ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                        ""mutable"":false
                                                                     },
                                                                     {
                                                                        ""name"":""methodType"",
                                                                        ""type"":""string"",
                                                                        ""required"":false,
                                                                        ""options"":[
                                                                           {
                                                                              ""label"":""SMS"",
                                                                              ""value"":""sms""
                                                                           }
                                                                        ]
                                                                     },
                                                                     {
                                                                        ""name"":""phoneNumber"",
                                                                        ""label"":""Phone number"",
                                                                        ""required"":false
                                                                     }
                                                                  ]
                                                               }
                                                            },
                                                            ""relatesTo"":""$.authenticators.value[0]""
                                                         }
                                                      ]
                                                   },
                                                   {
                                                      ""name"":""stateHandle"",
                                                      ""required"":true,
                                                      ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             }
                                          ]
                                       },
                                       ""authenticators"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""type"":""phone"",
                                                ""key"":""phone_number"",
                                                ""id"":""auttzfsi4eiZIdLK85d6"",
                                                ""displayName"":""Phone"",
                                                ""methods"":[
                                                   {
                                                      ""type"":""sms""
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
                                                ""id"":""eaetzmvljkwPEf96s5d6"",
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
                                                ""id"":""lae1vzgr4N1Sstrku5d6"",
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
                                             ""id"":""00utzmvli0oz5ReJB5d6""
                                          }
                                       },
                                       ""cancel"":{
                                          ""rel"":[
                                             ""create-form""
                                          ],
                                          ""name"":""cancel"",
                                          ""href"":""https://foo/idp/idx/cancel"",
                                          ""method"":""POST"",
                                          ""produces"":""application/ion+json; okta-version=1.0.0"",
                                          ""value"":[
                                             {
                                                ""name"":""stateHandle"",
                                                ""required"":true,
                                                ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                             ""label"":""Dotnet IDX Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";
            var enrollPhoneResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                           ""expiresAt"":""2021-05-28T20:35:36.000Z"",
                                           ""intent"":""LOGIN"",
                                           ""remediation"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""authenticator-enrollment-data"",
                                                    ""relatesTo"":[
                                                       ""$.currentAuthenticator""
                                                    ],
                                                    ""href"":""https://foo.com/idp/idx/credential/enroll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""label"":""Phone"",
                                                          ""form"":{
                                                             ""value"":[
                                                                {
                                                                   ""name"":""id"",
                                                                   ""required"":true,
                                                                   ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                   ""mutable"":false
                                                                },
                                                                {
                                                                   ""name"":""methodType"",
                                                                   ""type"":""string"",
                                                                   ""required"":true,
                                                                   ""options"":[
                                                                      {
                                                                         ""label"":""SMS"",
                                                                         ""value"":""sms""
                                                                      }
                                                                   ]
                                                                },
                                                                {
                                                                   ""name"":""phoneNumber"",
                                                                   ""required"":true
                                                                }
                                                             ]
                                                          }
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""select-authenticator-enroll"",
                                                    ""href"":""https://foo.com/idp/idx/credential/enroll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""type"":""object"",
                                                          ""options"":[
                                                             {
                                                                ""label"":""Phone"",
                                                                ""value"":{
                                                                   ""form"":{
                                                                      ""value"":[
                                                                         {
                                                                            ""name"":""id"",
                                                                            ""required"":true,
                                                                            ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                            ""mutable"":false
                                                                         },
                                                                         {
                                                                            ""name"":""methodType"",
                                                                            ""type"":""string"",
                                                                            ""required"":false,
                                                                            ""options"":[
                                                                               {
                                                                                  ""label"":""SMS"",
                                                                                  ""value"":""sms""
                                                                               }
                                                                            ]
                                                                         },
                                                                         {
                                                                            ""name"":""phoneNumber"",
                                                                            ""label"":""Phone number"",
                                                                            ""required"":false
                                                                         }
                                                                      ]
                                                                   }
                                                                },
                                                                ""relatesTo"":""$.authenticators.value[0]""
                                                             }
                                                          ]
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo.com/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""phone"",
                                                 ""key"":""phone_number"",
                                                 ""id"":""auttzfsi4eiZIdLK85d6"",
                                                 ""displayName"":""Phone"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""sms""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""auttzfsi4eiZIdLK85d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
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
                                                    ""id"":""eaetzmvljkwPEf96s5d6"",
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
                                                    ""id"":""lae1vzgr4N1Sstrku5d6"",
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
                                                 ""id"":""00utzmvli0oz5ReJB5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo.com/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = enrollPhoneResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.SelectEnrollAuthenticatorAsync(
                                   new SelectEnrollAuthenticatorOptions()
                                   {
                                       AuthenticatorId = "auttzfsi4eiZIdLK85d6",

                                   }, Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingAuthenticatorEnrollmentData);
            authResponse.CurrentAuthenticator.Id.Should().Be("auttzfsi4eiZIdLK85d6");
            authResponse.TokenInfo.Should().BeNull();
        }

        [Fact]
        public async Task EnrollPhone()
        {
            var SelectPhoneResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                           ""expiresAt"":""2021-05-28T20:35:36.000Z"",
                                           ""intent"":""LOGIN"",
                                           ""remediation"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""authenticator-enrollment-data"",
                                                    ""relatesTo"":[
                                                       ""$.currentAuthenticator""
                                                    ],
                                                    ""href"":""https://foo.com/idp/idx/credential/enroll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""label"":""Phone"",
                                                          ""form"":{
                                                             ""value"":[
                                                                {
                                                                   ""name"":""id"",
                                                                   ""required"":true,
                                                                   ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                   ""mutable"":false
                                                                },
                                                                {
                                                                   ""name"":""methodType"",
                                                                   ""type"":""string"",
                                                                   ""required"":true,
                                                                   ""options"":[
                                                                      {
                                                                         ""label"":""SMS"",
                                                                         ""value"":""sms""
                                                                      }
                                                                   ]
                                                                },
                                                                {
                                                                   ""name"":""phoneNumber"",
                                                                   ""required"":true
                                                                }
                                                             ]
                                                          }
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""select-authenticator-enroll"",
                                                    ""href"":""https://foo.com/idp/idx/credential/enroll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""type"":""object"",
                                                          ""options"":[
                                                             {
                                                                ""label"":""Phone"",
                                                                ""value"":{
                                                                   ""form"":{
                                                                      ""value"":[
                                                                         {
                                                                            ""name"":""id"",
                                                                            ""required"":true,
                                                                            ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                            ""mutable"":false
                                                                         },
                                                                         {
                                                                            ""name"":""methodType"",
                                                                            ""type"":""string"",
                                                                            ""required"":false,
                                                                            ""options"":[
                                                                               {
                                                                                  ""label"":""SMS"",
                                                                                  ""value"":""sms""
                                                                               }
                                                                            ]
                                                                         },
                                                                         {
                                                                            ""name"":""phoneNumber"",
                                                                            ""label"":""Phone number"",
                                                                            ""required"":false
                                                                         }
                                                                      ]
                                                                   }
                                                                },
                                                                ""relatesTo"":""$.authenticators.value[0]""
                                                             }
                                                          ]
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo.com/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""phone"",
                                                 ""key"":""phone_number"",
                                                 ""id"":""auttzfsi4eiZIdLK85d6"",
                                                 ""displayName"":""Phone"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""sms""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""auttzfsi4eiZIdLK85d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
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
                                                    ""id"":""eaetzmvljkwPEf96s5d6"",
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
                                                    ""id"":""lae1vzgr4N1Sstrku5d6"",
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
                                                 ""id"":""00utzmvli0oz5ReJB5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo.com/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";
            var enrollPhoneResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                           ""expiresAt"":""2021-05-28T20:36:20.000Z"",
                                           ""intent"":""LOGIN"",
                                           ""remediation"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""enroll-authenticator"",
                                                    ""relatesTo"":[
                                                       ""$.currentAuthenticator""
                                                    ],
                                                    ""href"":""https://foo/idp/idx/challenge/answer"",
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
                                                                   ""label"":""Enter code""
                                                                }
                                                             ]
                                                          },
                                                          ""required"":true
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""select-authenticator-enroll"",
                                                    ""href"":""https://foo/idp/idx/credential/enroll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""type"":""object"",
                                                          ""options"":[
                                                             {
                                                                ""label"":""Phone"",
                                                                ""value"":{
                                                                   ""form"":{
                                                                      ""value"":[
                                                                         {
                                                                            ""name"":""id"",
                                                                            ""required"":true,
                                                                            ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                            ""mutable"":false
                                                                         },
                                                                         {
                                                                            ""name"":""methodType"",
                                                                            ""type"":""string"",
                                                                            ""required"":false,
                                                                            ""options"":[
                                                                               {
                                                                                  ""label"":""SMS"",
                                                                                  ""value"":""sms""
                                                                               }
                                                                            ]
                                                                         },
                                                                         {
                                                                            ""name"":""phoneNumber"",
                                                                            ""label"":""Phone number"",
                                                                            ""required"":false
                                                                         }
                                                                      ]
                                                                   }
                                                                },
                                                                ""relatesTo"":""$.authenticators.value[0]""
                                                             }
                                                          ]
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""phone"",
                                                 ""key"":""phone_number"",
                                                 ""id"":""auttzfsi4eiZIdLK85d6"",
                                                 ""displayName"":""Phone"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""sms""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""auttzfsi4eiZIdLK85d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
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
                                                    ""id"":""eaetzmvljkwPEf96s5d6"",
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
                                                    ""id"":""lae1vzgr4N1Sstrku5d6"",
                                                    ""displayName"":""Password"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""password""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""enrollmentAuthenticator"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""type"":""phone"",
                                                 ""key"":""phone_number"",
                                                 ""id"":""auttzfsi4eiZIdLK85d6"",
                                                 ""displayName"":""Phone"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""sms""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""user"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""id"":""00utzmvli0oz5ReJB5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = SelectPhoneResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = enrollPhoneResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.EnrollAuthenticatorAsync(
                                   new EnrollPhoneAuthenticatorOptions
                                   {
                                       AuthenticatorId = "auttzfsi4eiZIdLK85d6",
                                       MethodType = AuthenticatorMethodType.Sms,
                                       PhoneNumber = "+1111111111",

                                   }, Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingAuthenticatorVerification);
            authResponse.TokenInfo.Should().BeNull();
        }

        [Fact]
        public async Task VerifyPhone()
        {
            var enrollPhoneResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                           ""expiresAt"":""2021-05-28T20:36:20.000Z"",
                                           ""intent"":""LOGIN"",
                                           ""remediation"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""enroll-authenticator"",
                                                    ""relatesTo"":[
                                                       ""$.currentAuthenticator""
                                                    ],
                                                    ""href"":""https://foo/idp/idx/challenge/answer"",
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
                                                                   ""label"":""Enter code""
                                                                }
                                                             ]
                                                          },
                                                          ""required"":true
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""select-authenticator-enroll"",
                                                    ""href"":""https://foo/idp/idx/credential/enroll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""type"":""object"",
                                                          ""options"":[
                                                             {
                                                                ""label"":""Phone"",
                                                                ""value"":{
                                                                   ""form"":{
                                                                      ""value"":[
                                                                         {
                                                                            ""name"":""id"",
                                                                            ""required"":true,
                                                                            ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                            ""mutable"":false
                                                                         },
                                                                         {
                                                                            ""name"":""methodType"",
                                                                            ""type"":""string"",
                                                                            ""required"":false,
                                                                            ""options"":[
                                                                               {
                                                                                  ""label"":""SMS"",
                                                                                  ""value"":""sms""
                                                                               }
                                                                            ]
                                                                         },
                                                                         {
                                                                            ""name"":""phoneNumber"",
                                                                            ""label"":""Phone number"",
                                                                            ""required"":false
                                                                         }
                                                                      ]
                                                                   }
                                                                },
                                                                ""relatesTo"":""$.authenticators.value[0]""
                                                             }
                                                          ]
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""phone"",
                                                 ""key"":""phone_number"",
                                                 ""id"":""auttzfsi4eiZIdLK85d6"",
                                                 ""displayName"":""Phone"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""sms""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""auttzfsi4eiZIdLK85d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
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
                                                    ""id"":""eaetzmvljkwPEf96s5d6"",
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
                                                    ""id"":""lae1vzgr4N1Sstrku5d6"",
                                                    ""displayName"":""Password"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""password""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""enrollmentAuthenticator"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""type"":""phone"",
                                                 ""key"":""phone_number"",
                                                 ""id"":""auttzfsi4eiZIdLK85d6"",
                                                 ""displayName"":""Phone"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""sms""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""user"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""id"":""00utzmvli0oz5ReJB5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";
            var verifyCodeResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                           ""expiresAt"":""2021-05-28T20:32:37.000Z"",
                                           ""intent"":""LOGIN"",
                                           ""user"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""id"":""00utzmvli0oz5ReJB5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo.com/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           },
                                           ""successWithInteractionCode"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""issue"",
                                              ""href"":""https://foo.com/oauth2/foo/v1/token"",
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
                                                    ""value"":""Mj36FIck-Fr1845qXozlmCVD64Mx3sk3DrvPNVFB2E4""
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

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = enrollPhoneResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = verifyCodeResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = tokenResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.VerifyAuthenticatorAsync(
                                   new VerifyAuthenticatorOptions
                                       {
                                           Code = "foo",
                                       }
                                   , Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.Success);
            authResponse.TokenInfo.Should().NotBeNull();
            authResponse.TokenInfo.AccessToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task FailWithInvalidPhoneNumber()
        {
            var SelectPhoneResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                           ""expiresAt"":""2021-05-28T20:35:36.000Z"",
                                           ""intent"":""LOGIN"",
                                           ""remediation"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""authenticator-enrollment-data"",
                                                    ""relatesTo"":[
                                                       ""$.currentAuthenticator""
                                                    ],
                                                    ""href"":""https://foo.com/idp/idx/credential/enroll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""label"":""Phone"",
                                                          ""form"":{
                                                             ""value"":[
                                                                {
                                                                   ""name"":""id"",
                                                                   ""required"":true,
                                                                   ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                   ""mutable"":false
                                                                },
                                                                {
                                                                   ""name"":""methodType"",
                                                                   ""type"":""string"",
                                                                   ""required"":true,
                                                                   ""options"":[
                                                                      {
                                                                         ""label"":""SMS"",
                                                                         ""value"":""sms""
                                                                      }
                                                                   ]
                                                                },
                                                                {
                                                                   ""name"":""phoneNumber"",
                                                                   ""required"":true
                                                                }
                                                             ]
                                                          }
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""select-authenticator-enroll"",
                                                    ""href"":""https://foo.com/idp/idx/credential/enroll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""type"":""object"",
                                                          ""options"":[
                                                             {
                                                                ""label"":""Phone"",
                                                                ""value"":{
                                                                   ""form"":{
                                                                      ""value"":[
                                                                         {
                                                                            ""name"":""id"",
                                                                            ""required"":true,
                                                                            ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                            ""mutable"":false
                                                                         },
                                                                         {
                                                                            ""name"":""methodType"",
                                                                            ""type"":""string"",
                                                                            ""required"":false,
                                                                            ""options"":[
                                                                               {
                                                                                  ""label"":""SMS"",
                                                                                  ""value"":""sms""
                                                                               }
                                                                            ]
                                                                         },
                                                                         {
                                                                            ""name"":""phoneNumber"",
                                                                            ""label"":""Phone number"",
                                                                            ""required"":false
                                                                         }
                                                                      ]
                                                                   }
                                                                },
                                                                ""relatesTo"":""$.authenticators.value[0]""
                                                             }
                                                          ]
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo.com/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""phone"",
                                                 ""key"":""phone_number"",
                                                 ""id"":""auttzfsi4eiZIdLK85d6"",
                                                 ""displayName"":""Phone"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""sms""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""auttzfsi4eiZIdLK85d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
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
                                                    ""id"":""eaetzmvljkwPEf96s5d6"",
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
                                                    ""id"":""lae1vzgr4N1Sstrku5d6"",
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
                                                 ""id"":""00utzmvli0oz5ReJB5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo.com/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";
            var enrollPhoneResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                           ""expiresAt"":""2021-05-28T20:35:36.000Z"",
                                           ""intent"":""LOGIN"",
                                           ""remediation"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""select-authenticator-enroll"",
                                                    ""href"":""https://foo.com/idp/idx/credential/enroll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""type"":""object"",
                                                          ""options"":[
                                                             {
                                                                ""label"":""Phone"",
                                                                ""value"":{
                                                                   ""form"":{
                                                                      ""value"":[
                                                                         {
                                                                            ""name"":""id"",
                                                                            ""required"":true,
                                                                            ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                            ""mutable"":false
                                                                         },
                                                                         {
                                                                            ""name"":""methodType"",
                                                                            ""type"":""string"",
                                                                            ""required"":false,
                                                                            ""options"":[
                                                                               {
                                                                                  ""label"":""SMS"",
                                                                                  ""value"":""sms""
                                                                               }
                                                                            ]
                                                                         },
                                                                         {
                                                                            ""name"":""phoneNumber"",
                                                                            ""label"":""Phone number"",
                                                                            ""required"":false
                                                                         }
                                                                      ]
                                                                   }
                                                                },
                                                                ""relatesTo"":""$.authenticators.value[0]""
                                                             }
                                                          ]
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 }
                                              ]
                                           },
                                           ""messages"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""message"":""Unable to initiate factor enrollment: Invalid Phone Number."",
                                                    ""class"":""ERROR""
                                                 }
                                              ]
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""auttzfsi4eiZIdLK85d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
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
                                                    ""id"":""eaetzmvljkwPEf96s5d6"",
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
                                                    ""id"":""lae1vzgr4N1Sstrku5d6"",
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
                                                 ""id"":""00utzmvli0oz5ReJB5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo.com/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = SelectPhoneResponse });
            queue.Enqueue(new MockResponse { StatusCode = 400, Response = enrollPhoneResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);
            
            Func<Task<AuthenticationResponse>> function = async () => await testClient.EnrollAuthenticatorAsync(
                                                                          new EnrollPhoneAuthenticatorOptions
                                                                              {
                                                                                  AuthenticatorId = "auttzfsi4eiZIdLK85d6",
                                                                                  MethodType = AuthenticatorMethodType.Sms,
                                                                                  PhoneNumber = "1",

                                                                              }, Substitute.For<IIdxContext>());

            await function.Should()
                .ThrowAsync<OktaException>()
                .WithMessage("*Unable to initiate factor enrollment: Invalid Phone Number*");
        }

        [Fact]
        public async Task FailWithInvalidVerificationCode()
        {
            var enrollPhoneResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                           ""expiresAt"":""2021-05-28T20:36:20.000Z"",
                                           ""intent"":""LOGIN"",
                                           ""remediation"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""enroll-authenticator"",
                                                    ""relatesTo"":[
                                                       ""$.currentAuthenticator""
                                                    ],
                                                    ""href"":""https://foo/idp/idx/challenge/answer"",
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
                                                                   ""label"":""Enter code""
                                                                }
                                                             ]
                                                          },
                                                          ""required"":true
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""select-authenticator-enroll"",
                                                    ""href"":""https://foo/idp/idx/credential/enroll"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""type"":""object"",
                                                          ""options"":[
                                                             {
                                                                ""label"":""Phone"",
                                                                ""value"":{
                                                                   ""form"":{
                                                                      ""value"":[
                                                                         {
                                                                            ""name"":""id"",
                                                                            ""required"":true,
                                                                            ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                            ""mutable"":false
                                                                         },
                                                                         {
                                                                            ""name"":""methodType"",
                                                                            ""type"":""string"",
                                                                            ""required"":false,
                                                                            ""options"":[
                                                                               {
                                                                                  ""label"":""SMS"",
                                                                                  ""value"":""sms""
                                                                               }
                                                                            ]
                                                                         },
                                                                         {
                                                                            ""name"":""phoneNumber"",
                                                                            ""label"":""Phone number"",
                                                                            ""required"":false
                                                                         }
                                                                      ]
                                                                   }
                                                                },
                                                                ""relatesTo"":""$.authenticators.value[0]""
                                                             }
                                                          ]
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""phone"",
                                                 ""key"":""phone_number"",
                                                 ""id"":""auttzfsi4eiZIdLK85d6"",
                                                 ""displayName"":""Phone"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""sms""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""auttzfsi4eiZIdLK85d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
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
                                                    ""id"":""eaetzmvljkwPEf96s5d6"",
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
                                                    ""id"":""lae1vzgr4N1Sstrku5d6"",
                                                    ""displayName"":""Password"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""password""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""enrollmentAuthenticator"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""type"":""phone"",
                                                 ""key"":""phone_number"",
                                                 ""id"":""auttzfsi4eiZIdLK85d6"",
                                                 ""displayName"":""Phone"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""sms""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""user"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""id"":""00utzmvli0oz5ReJB5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02gfb3KEpSVIvn_-wa0214BOhkpCzhS1s4TmNvvyIx"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";
            var verifyCodeResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                           ""expiresAt"":""2021-05-28T20:48:51.000Z"",
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
                                                    ""href"":""https://foo/idp/idx/challenge/answer"",
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
                                                                   ""label"":""Enter code"",
                                                                   ""messages"":{
                                                                      ""type"":""array"",
                                                                      ""value"":[
                                                                         {
                                                                            ""message"":""Invalid code. Try again."",
                                                                            ""i18n"":{
                                                                               ""key"":""api.authn.error.PASSCODE_INVALID"",
                                                                               ""params"":[
                                                                                  
                                                                               ]
                                                                            },
                                                                            ""class"":""ERROR""
                                                                         }
                                                                      ]
                                                                   }
                                                                }
                                                             ]
                                                          },
                                                          ""required"":true
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
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
                                                 ""profile"":{
                                                    ""phoneNumber"":""+1 XXX-XXX-4709""
                                                 },
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""phone"",
                                                 ""key"":""phone_number"",
                                                 ""id"":""smsu01v750pnUXuCH5d6"",
                                                 ""displayName"":""Phone"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""sms""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""auttzfsi4eiZIdLK85d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""authenticatorEnrollments"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""profile"":{
                                                       ""phoneNumber"":""+1 XXX-XXX-4709""
                                                    },
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""smsu01v750pnUXuCH5d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""user"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""id"":""00utzmvli0oz5ReJB5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = enrollPhoneResponse });
            queue.Enqueue(new MockResponse { StatusCode = 401, Response = verifyCodeResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            Func<Task<AuthenticationResponse>> function = async () => await testClient.EnrollAuthenticatorAsync(
                                                                          new EnrollPhoneAuthenticatorOptions
                                                                          {
                                                                              AuthenticatorId = "auttzfsi4eiZIdLK85d6",
                                                                              MethodType = AuthenticatorMethodType.Sms,
                                                                              PhoneNumber = "1",

                                                                          }, Substitute.For<IIdxContext>());

            await function.Should()
                .ThrowAsync<OktaException>()
                .WithMessage("*Invalid code. Try again.*");
        }

        [Fact]
        public async Task SelectPhoneForChallengeSecondFactor()
        {
            var identifyResponse = @"{
                                       ""version"":""1.0.0"",
                                       ""stateHandle"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                       ""expiresAt"":""2021-05-28T20:48:39.000Z"",
                                       ""intent"":""LOGIN"",
                                       ""remediation"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""rel"":[
                                                   ""create-form""
                                                ],
                                                ""name"":""authenticator-verification-data"",
                                                ""relatesTo"":[
                                                   ""$.currentAuthenticatorEnrollment""
                                                ],
                                                ""href"":""https://foo/idp/idx/challenge"",
                                                ""method"":""POST"",
                                                ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                ""value"":[
                                                   {
                                                      ""name"":""authenticator"",
                                                      ""label"":""Phone"",
                                                      ""form"":{
                                                         ""value"":[
                                                            {
                                                               ""name"":""id"",
                                                               ""required"":true,
                                                               ""value"":""auttzfsi4eiZIdLK85d6"",
                                                               ""mutable"":false
                                                            },
                                                            {
                                                               ""name"":""methodType"",
                                                               ""type"":""string"",
                                                               ""required"":true,
                                                               ""options"":[
                                                                  {
                                                                     ""label"":""SMS"",
                                                                     ""value"":""sms""
                                                                  }
                                                               ]
                                                            },
                                                            {
                                                               ""name"":""enrollmentId"",
                                                               ""required"":true,
                                                               ""value"":""smsu01v750pnUXuCH5d6"",
                                                               ""mutable"":false
                                                            }
                                                         ]
                                                      }
                                                   },
                                                   {
                                                      ""name"":""stateHandle"",
                                                      ""required"":true,
                                                      ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             },
                                             {
                                                ""rel"":[
                                                   ""create-form""
                                                ],
                                                ""name"":""select-authenticator-authenticate"",
                                                ""href"":""https://foo/idp/idx/challenge"",
                                                ""method"":""POST"",
                                                ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                ""value"":[
                                                   {
                                                      ""name"":""authenticator"",
                                                      ""type"":""object"",
                                                      ""options"":[
                                                         {
                                                            ""label"":""Phone"",
                                                            ""value"":{
                                                               ""form"":{
                                                                  ""value"":[
                                                                     {
                                                                        ""name"":""id"",
                                                                        ""required"":true,
                                                                        ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                        ""mutable"":false
                                                                     },
                                                                     {
                                                                        ""name"":""methodType"",
                                                                        ""type"":""string"",
                                                                        ""required"":false,
                                                                        ""options"":[
                                                                           {
                                                                              ""label"":""SMS"",
                                                                              ""value"":""sms""
                                                                           }
                                                                        ]
                                                                     },
                                                                     {
                                                                        ""name"":""enrollmentId"",
                                                                        ""required"":true,
                                                                        ""value"":""smsu01v750pnUXuCH5d6"",
                                                                        ""mutable"":false
                                                                     }
                                                                  ]
                                                               }
                                                            },
                                                            ""relatesTo"":""$.authenticatorEnrollments.value[0]""
                                                         }
                                                      ]
                                                   },
                                                   {
                                                      ""name"":""stateHandle"",
                                                      ""required"":true,
                                                      ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
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
                                             ""profile"":{
                                                ""phoneNumber"":""+1 XXX-XXX-4709""
                                             },
                                             ""resend"":{
                                                ""rel"":[
                                                   ""create-form""
                                                ],
                                                ""name"":""resend"",
                                                ""href"":""https://foo/idp/idx/challenge/resend"",
                                                ""method"":""POST"",
                                                ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                ""value"":[
                                                   {
                                                      ""name"":""stateHandle"",
                                                      ""required"":true,
                                                      ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                                      ""visible"":false,
                                                      ""mutable"":false
                                                   }
                                                ],
                                                ""accepts"":""application/json; okta-version=1.0.0""
                                             },
                                             ""type"":""phone"",
                                             ""key"":""phone_number"",
                                             ""id"":""smsu01v750pnUXuCH5d6"",
                                             ""displayName"":""Phone"",
                                             ""methods"":[
                                                {
                                                   ""type"":""sms""
                                                }
                                             ]
                                          }
                                       },
                                       ""authenticators"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""type"":""phone"",
                                                ""key"":""phone_number"",
                                                ""id"":""auttzfsi4eiZIdLK85d6"",
                                                ""displayName"":""Phone"",
                                                ""methods"":[
                                                   {
                                                      ""type"":""sms""
                                                   }
                                                ]
                                             }
                                          ]
                                       },
                                       ""authenticatorEnrollments"":{
                                          ""type"":""array"",
                                          ""value"":[
                                             {
                                                ""profile"":{
                                                   ""phoneNumber"":""+1 XXX-XXX-4709""
                                                },
                                                ""type"":""phone"",
                                                ""key"":""phone_number"",
                                                ""id"":""smsu01v750pnUXuCH5d6"",
                                                ""displayName"":""Phone"",
                                                ""methods"":[
                                                   {
                                                      ""type"":""sms""
                                                   }
                                                ]
                                             }
                                          ]
                                       },
                                       ""user"":{
                                          ""type"":""object"",
                                          ""value"":{
                                             ""id"":""00utzmvli0oz5ReJB5d6""
                                          }
                                       },
                                       ""cancel"":{
                                          ""rel"":[
                                             ""create-form""
                                          ],
                                          ""name"":""cancel"",
                                          ""href"":""https://foo/idp/idx/cancel"",
                                          ""method"":""POST"",
                                          ""produces"":""application/ion+json; okta-version=1.0.0"",
                                          ""value"":[
                                             {
                                                ""name"":""stateHandle"",
                                                ""required"":true,
                                                ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
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
                                             ""label"":""Dotnet IDX Web App"",
                                             ""id"":""foo""
                                          }
                                       }
                                    }";
            var selectPhoneResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                           ""expiresAt"":""2021-05-28T20:48:44.000Z"",
                                           ""intent"":""LOGIN"",
                                           ""remediation"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""authenticator-verification-data"",
                                                    ""relatesTo"":[
                                                       ""$.currentAuthenticatorEnrollment""
                                                    ],
                                                    ""href"":""https://foo/idp/idx/challenge"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""label"":""Phone"",
                                                          ""form"":{
                                                             ""value"":[
                                                                {
                                                                   ""name"":""id"",
                                                                   ""required"":true,
                                                                   ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                   ""mutable"":false
                                                                },
                                                                {
                                                                   ""name"":""methodType"",
                                                                   ""type"":""string"",
                                                                   ""required"":true,
                                                                   ""options"":[
                                                                      {
                                                                         ""label"":""SMS"",
                                                                         ""value"":""sms""
                                                                      }
                                                                   ]
                                                                },
                                                                {
                                                                   ""name"":""enrollmentId"",
                                                                   ""required"":true,
                                                                   ""value"":""smsu01v750pnUXuCH5d6"",
                                                                   ""mutable"":false
                                                                }
                                                             ]
                                                          }
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""select-authenticator-authenticate"",
                                                    ""href"":""https://foo/idp/idx/challenge"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""type"":""object"",
                                                          ""options"":[
                                                             {
                                                                ""label"":""Phone"",
                                                                ""value"":{
                                                                   ""form"":{
                                                                      ""value"":[
                                                                         {
                                                                            ""name"":""id"",
                                                                            ""required"":true,
                                                                            ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                            ""mutable"":false
                                                                         },
                                                                         {
                                                                            ""name"":""methodType"",
                                                                            ""type"":""string"",
                                                                            ""required"":false,
                                                                            ""options"":[
                                                                               {
                                                                                  ""label"":""SMS"",
                                                                                  ""value"":""sms""
                                                                               }
                                                                            ]
                                                                         },
                                                                         {
                                                                            ""name"":""enrollmentId"",
                                                                            ""required"":true,
                                                                            ""value"":""smsu01v750pnUXuCH5d6"",
                                                                            ""mutable"":false
                                                                         }
                                                                      ]
                                                                   }
                                                                },
                                                                ""relatesTo"":""$.authenticatorEnrollments.value[0]""
                                                             }
                                                          ]
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
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
                                                 ""profile"":{
                                                    ""phoneNumber"":""+1 XXX-XXX-4709""
                                                 },
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""phone"",
                                                 ""key"":""phone_number"",
                                                 ""id"":""smsu01v750pnUXuCH5d6"",
                                                 ""displayName"":""Phone"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""sms""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""auttzfsi4eiZIdLK85d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""authenticatorEnrollments"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""profile"":{
                                                       ""phoneNumber"":""+1 XXX-XXX-4709""
                                                    },
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""smsu01v750pnUXuCH5d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""user"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""id"":""00utzmvli0oz5ReJB5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = selectPhoneResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.SelectChallengeAuthenticatorAsync(
                                   new SelectPhoneAuthenticatorOptions
                                   {
                                       AuthenticatorId = "auttzfsi4eiZIdLK85d6",
                                       EnrollmentId = "smsu01v750pnUXuCH5d6",

                                   }, Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingChallengeAuthenticatorData);
            authResponse.CurrentAuthenticatorEnrollment.Id.Should().Be("auttzfsi4eiZIdLK85d6");
            authResponse.CurrentAuthenticatorEnrollment.EnrollmentId.Should().Be("smsu01v750pnUXuCH5d6");
            authResponse.TokenInfo.Should().BeNull();
        }

        [Fact]
        public async Task SelectPhoneMethodTypeDuringChallenge()
        {
            var selectPhoneResponse = @"{
                                           ""version"":""1.0.0"",
                                           ""stateHandle"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                           ""expiresAt"":""2021-05-28T20:48:44.000Z"",
                                           ""intent"":""LOGIN"",
                                           ""remediation"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""authenticator-verification-data"",
                                                    ""relatesTo"":[
                                                       ""$.currentAuthenticatorEnrollment""
                                                    ],
                                                    ""href"":""https://foo/idp/idx/challenge"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""label"":""Phone"",
                                                          ""form"":{
                                                             ""value"":[
                                                                {
                                                                   ""name"":""id"",
                                                                   ""required"":true,
                                                                   ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                   ""mutable"":false
                                                                },
                                                                {
                                                                   ""name"":""methodType"",
                                                                   ""type"":""string"",
                                                                   ""required"":true,
                                                                   ""options"":[
                                                                      {
                                                                         ""label"":""SMS"",
                                                                         ""value"":""sms""
                                                                      }
                                                                   ]
                                                                },
                                                                {
                                                                   ""name"":""enrollmentId"",
                                                                   ""required"":true,
                                                                   ""value"":""smsu01v750pnUXuCH5d6"",
                                                                   ""mutable"":false
                                                                }
                                                             ]
                                                          }
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 {
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""select-authenticator-authenticate"",
                                                    ""href"":""https://foo/idp/idx/challenge"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""authenticator"",
                                                          ""type"":""object"",
                                                          ""options"":[
                                                             {
                                                                ""label"":""Phone"",
                                                                ""value"":{
                                                                   ""form"":{
                                                                      ""value"":[
                                                                         {
                                                                            ""name"":""id"",
                                                                            ""required"":true,
                                                                            ""value"":""auttzfsi4eiZIdLK85d6"",
                                                                            ""mutable"":false
                                                                         },
                                                                         {
                                                                            ""name"":""methodType"",
                                                                            ""type"":""string"",
                                                                            ""required"":false,
                                                                            ""options"":[
                                                                               {
                                                                                  ""label"":""SMS"",
                                                                                  ""value"":""sms""
                                                                               }
                                                                            ]
                                                                         },
                                                                         {
                                                                            ""name"":""enrollmentId"",
                                                                            ""required"":true,
                                                                            ""value"":""smsu01v750pnUXuCH5d6"",
                                                                            ""mutable"":false
                                                                         }
                                                                      ]
                                                                   }
                                                                },
                                                                ""relatesTo"":""$.authenticatorEnrollments.value[0]""
                                                             }
                                                          ]
                                                       },
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
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
                                                 ""profile"":{
                                                    ""phoneNumber"":""+1 XXX-XXX-4709""
                                                 },
                                                 ""resend"":{
                                                    ""rel"":[
                                                       ""create-form""
                                                    ],
                                                    ""name"":""resend"",
                                                    ""href"":""https://foo/idp/idx/challenge/resend"",
                                                    ""method"":""POST"",
                                                    ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                    ""value"":[
                                                       {
                                                          ""name"":""stateHandle"",
                                                          ""required"":true,
                                                          ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                                          ""visible"":false,
                                                          ""mutable"":false
                                                       }
                                                    ],
                                                    ""accepts"":""application/json; okta-version=1.0.0""
                                                 },
                                                 ""type"":""phone"",
                                                 ""key"":""phone_number"",
                                                 ""id"":""smsu01v750pnUXuCH5d6"",
                                                 ""displayName"":""Phone"",
                                                 ""methods"":[
                                                    {
                                                       ""type"":""sms""
                                                    }
                                                 ]
                                              }
                                           },
                                           ""authenticators"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""auttzfsi4eiZIdLK85d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""authenticatorEnrollments"":{
                                              ""type"":""array"",
                                              ""value"":[
                                                 {
                                                    ""profile"":{
                                                       ""phoneNumber"":""+1 XXX-XXX-4709""
                                                    },
                                                    ""type"":""phone"",
                                                    ""key"":""phone_number"",
                                                    ""id"":""smsu01v750pnUXuCH5d6"",
                                                    ""displayName"":""Phone"",
                                                    ""methods"":[
                                                       {
                                                          ""type"":""sms""
                                                       }
                                                    ]
                                                 }
                                              ]
                                           },
                                           ""user"":{
                                              ""type"":""object"",
                                              ""value"":{
                                                 ""id"":""00utzmvli0oz5ReJB5d6""
                                              }
                                           },
                                           ""cancel"":{
                                              ""rel"":[
                                                 ""create-form""
                                              ],
                                              ""name"":""cancel"",
                                              ""href"":""https://foo/idp/idx/cancel"",
                                              ""method"":""POST"",
                                              ""produces"":""application/ion+json; okta-version=1.0.0"",
                                              ""value"":[
                                                 {
                                                    ""name"":""stateHandle"",
                                                    ""required"":true,
                                                    ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
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
                                                 ""label"":""Dotnet IDX Web App"",
                                                 ""id"":""foo""
                                              }
                                           }
                                        }";
            var challengeWithMethodResponse = @"{
                                                   ""version"":""1.0.0"",
                                                   ""stateHandle"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                                   ""expiresAt"":""2021-05-28T20:48:51.000Z"",
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
                                                            ""href"":""https://foo/idp/idx/challenge/answer"",
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
                                                                           ""label"":""Enter code""
                                                                        }
                                                                     ]
                                                                  },
                                                                  ""required"":true
                                                               },
                                                               {
                                                                  ""name"":""stateHandle"",
                                                                  ""required"":true,
                                                                  ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
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
                                                         ""profile"":{
                                                            ""phoneNumber"":""+1 XXX-XXX-4709""
                                                         },
                                                         ""resend"":{
                                                            ""rel"":[
                                                               ""create-form""
                                                            ],
                                                            ""name"":""resend"",
                                                            ""href"":""https://foo/idp/idx/challenge/resend"",
                                                            ""method"":""POST"",
                                                            ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                            ""value"":[
                                                               {
                                                                  ""name"":""stateHandle"",
                                                                  ""required"":true,
                                                                  ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
                                                                  ""visible"":false,
                                                                  ""mutable"":false
                                                               }
                                                            ],
                                                            ""accepts"":""application/json; okta-version=1.0.0""
                                                         },
                                                         ""type"":""phone"",
                                                         ""key"":""phone_number"",
                                                         ""id"":""smsu01v750pnUXuCH5d6"",
                                                         ""displayName"":""Phone"",
                                                         ""methods"":[
                                                            {
                                                               ""type"":""sms""
                                                            }
                                                         ]
                                                      }
                                                   },
                                                   ""authenticators"":{
                                                      ""type"":""array"",
                                                      ""value"":[
                                                         {
                                                            ""type"":""phone"",
                                                            ""key"":""phone_number"",
                                                            ""id"":""auttzfsi4eiZIdLK85d6"",
                                                            ""displayName"":""Phone"",
                                                            ""methods"":[
                                                               {
                                                                  ""type"":""sms""
                                                               }
                                                            ]
                                                         }
                                                      ]
                                                   },
                                                   ""authenticatorEnrollments"":{
                                                      ""type"":""array"",
                                                      ""value"":[
                                                         {
                                                            ""profile"":{
                                                               ""phoneNumber"":""+1 XXX-XXX-4709""
                                                            },
                                                            ""type"":""phone"",
                                                            ""key"":""phone_number"",
                                                            ""id"":""smsu01v750pnUXuCH5d6"",
                                                            ""displayName"":""Phone"",
                                                            ""methods"":[
                                                               {
                                                                  ""type"":""sms""
                                                               }
                                                            ]
                                                         }
                                                      ]
                                                   },
                                                   ""user"":{
                                                      ""type"":""object"",
                                                      ""value"":{
                                                         ""id"":""00utzmvli0oz5ReJB5d6""
                                                      }
                                                   },
                                                   ""cancel"":{
                                                      ""rel"":[
                                                         ""create-form""
                                                      ],
                                                      ""name"":""cancel"",
                                                      ""href"":""https://foo/idp/idx/cancel"",
                                                      ""method"":""POST"",
                                                      ""produces"":""application/ion+json; okta-version=1.0.0"",
                                                      ""value"":[
                                                         {
                                                            ""name"":""stateHandle"",
                                                            ""required"":true,
                                                            ""value"":""02Ne-0FN9pKGQELUTxhXSPXVmUtdDuZqO2381oihou"",
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
                                                         ""label"":""Dotnet IDX Web App"",
                                                         ""id"":""foo""
                                                      }
                                                   }
                                                }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = selectPhoneResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = challengeWithMethodResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.ChallengeAuthenticatorAsync(
                                   new ChallengePhoneAuthenticatorOptions()
                                   {
                                       AuthenticatorId = "auttzfsi4eiZIdLK85d6",
                                       EnrollmentId = "smsu01v750pnUXuCH5d6",
                                       MethodType = AuthenticatorMethodType.Sms,

                                   }, Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingAuthenticatorVerification);
            authResponse.TokenInfo.Should().BeNull();
        }

        /****** END SCENARIOS 6.2.X ******/

        #endregion
    }
}
