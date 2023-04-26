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
            ""id"": ""xxxxxxxx""
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
            ""id"": ""xxxxxxxx""
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
            ""id"": ""xxxxxxxx""
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
            ""label"": ""- Unit Test Web App"",
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
            ""label"": ""- Unit Test Web App"",
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
            ""id"": ""xxxxxxxx""
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
            ""id"": ""xxxxxxxx""
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
                ""value"": ""xxxxxxxx""
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

        [Fact]
        public async Task ThrowWhenRecoverPasswordWithBadEmail()
        {
            string interactResponse = @"{
    ""interaction_handle"": ""ACv86bsQprBjf4pitCJTMlMbSGVHTwLijEtfIRMvJ6g""
}";
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02_fiCBXrxDU-lpkYgZD_cZ4taUvBeJiibQo0KDM2O"",
    ""expiresAt"": ""2021-06-02T14:30:16.000Z"",
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
                        ""value"": ""02_fiCBXrxDU-lpkYgZD_cZ4taUvBeJiibQo0KDM2O"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-enroll-profile"",
                ""href"": ""https://fake.example.com/idp/idx/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02_fiCBXrxDU-lpkYgZD_cZ4taUvBeJiibQo0KDM2O"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""unlock-account"",
                ""href"": ""https://fake.example.com/idp/idx/unlock-account"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02_fiCBXrxDU-lpkYgZD_cZ4taUvBeJiibQo0KDM2O"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""name"": ""redirect-idp"",
                ""type"": ""FACEBOOK"",
                ""idp"": {
                    ""id"": ""0oau09xo6XAbbPQSN5d6"",
                    ""name"": ""Facebook IdP""
                },
                ""href"": ""https://fake.example.com/oauth2/austyqkbjaFoOxkl45d6/v1/authorize?client_id=xxxxxxxx&request_uri=urn:okta:b0NXT3VtWUpBNmFiUTlGdWlFVlF2UTMtWDhZRllGYTl0UWZYVXJaRFZYczowb2F1MDl4bzZYQWJiUFFTTjVkNg"",
                ""method"": ""GET""
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
                        ""value"": ""02_fiCBXrxDU-lpkYgZD_cZ4taUvBeJiibQo0KDM2O"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
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
                ""value"": ""02_fiCBXrxDU-lpkYgZD_cZ4taUvBeJiibQo0KDM2O"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";
            string recoverResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02_fiCBXrxDU-lpkYgZD_cZ4taUvBeJiibQo0KDM2O"",
    ""expiresAt"": ""2021-06-02T14:30:26.000Z"",
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
                        ""value"": ""02_fiCBXrxDU-lpkYgZD_cZ4taUvBeJiibQo0KDM2O"",
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
                ""value"": ""02_fiCBXrxDU-lpkYgZD_cZ4taUvBeJiibQo0KDM2O"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";
            string identifyResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02_fiCBXrxDU-lpkYgZD_cZ4taUvBeJiibQo0KDM2O"",
    ""expiresAt"": ""2021-06-02T14:30:38.000Z"",
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
                        ""value"": ""02_fiCBXrxDU-lpkYgZD_cZ4taUvBeJiibQo0KDM2O"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }
        ]
    },
    ""messages"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""message"": ""There is no account with the Username non-existentuser@threeheadz.com."",
                ""i18n"": {
                    ""key"": ""idx.unknown.user"",
                    ""params"": []
                },
                ""class"": ""INFO""
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
                ""value"": ""02_fiCBXrxDU-lpkYgZD_cZ4taUvBeJiibQo0KDM2O"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";

            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/oauth2/v1/interact", interactResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/recover", recoverResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/identify", identifyResponse);

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);
            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);
            RecoverPasswordOptions recoverPasswordOptions = new RecoverPasswordOptions { Username = "non-existent-user@email.bad", };
            await Assert.ThrowsAsync<TerminalStateException>(async () =>
            {
                try
                {
                    await idxClient.RecoverPasswordAsync(recoverPasswordOptions);
                }
                catch (TerminalStateException exception)
                {
                    exception.IdxMessages.Messages.Count.Should().Be(1);
                    exception.IdxMessages.Messages.First().I18n.Key.Should().Be("idx.unknown.user");

                    throw exception;
                }
            });
        }

        [Fact]
        public async Task VerifyPhoneDuringRegistration()
        {
            string selectPhoneResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02_Yrkbs19YkNeRELAiXJYJwOu5ggDslnv0E3jXhBZ"",
    ""expiresAt"": ""2021-06-03T13:50:46.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""enroll-authenticator"",
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
                                    ""label"": ""Enter code""
                                }
                            ]
                        },
                        ""required"": true
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02_Yrkbs19YkNeRELAiXJYJwOu5ggDslnv0E3jXhBZ"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""type"": ""object"",
                        ""options"": [
                            {
                                ""label"": ""Phone"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi4eiZIdLK85d6"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""SMS"",
                                                        ""value"": ""sms""
                                                    }
                                                ]
                                            },
                                            {
                                                ""name"": ""phoneNumber"",
                                                ""label"": ""Phone number"",
                                                ""required"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[0]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02_Yrkbs19YkNeRELAiXJYJwOu5ggDslnv0E3jXhBZ"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""skip"",
                ""href"": ""https://fake.example.com/idp/idx/skip"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02_Yrkbs19YkNeRELAiXJYJwOu5ggDslnv0E3jXhBZ"",
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
                        ""value"": ""02_Yrkbs19YkNeRELAiXJYJwOu5ggDslnv0E3jXhBZ"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""type"": ""phone"",
            ""key"": ""phone_number"",
            ""id"": ""auttzfsi4eiZIdLK85d6"",
            ""displayName"": ""Phone"",
            ""methods"": [
                {
                    ""type"": ""sms""
                }
            ]
        }
    },
    ""authenticators"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""phone"",
                ""key"": ""phone_number"",
                ""id"": ""auttzfsi4eiZIdLK85d6"",
                ""displayName"": ""Phone"",
                ""methods"": [
                    {
                        ""type"": ""sms""
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
                ""id"": ""eaev4ay7xzba7ba8p5d6"",
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
                ""id"": ""lae1yn298clwHfSOX5d6"",
                ""displayName"": ""Password"",
                ""methods"": [
                    {
                        ""type"": ""password""
                    }
                ]
            }
        ]
    },
    ""enrollmentAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""phone"",
            ""key"": ""phone_number"",
            ""id"": ""auttzfsi4eiZIdLK85d6"",
            ""displayName"": ""Phone"",
            ""methods"": [
                {
                    ""type"": ""sms""
                }
            ]
        }
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uv4cxgf42axiPU55d6""
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
                ""value"": ""02_Yrkbs19YkNeRELAiXJYJwOu5ggDslnv0E3jXhBZ"",
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
            string verifyCodeResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02_Yrkbs19YkNeRELAiXJYJwOu5ggDslnv0E3jXhBZ"",
    ""expiresAt"": ""2021-06-03T13:47:09.000Z"",
    ""intent"": ""LOGIN"",
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uv4cxgf42axiPU55d6""
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
                ""value"": ""02_Yrkbs19YkNeRELAiXJYJwOu5ggDslnv0E3jXhBZ"",
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
                ""value"": ""igUA7DBWuQrqx7RjaTDaTN9yevtsRDFwFe0eJpOGqp0""
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
            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", selectPhoneResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/challenge/answer", verifyCodeResponse);
            mockHttpMessageHandler.AddTestResponse("/oauth2/austyqkbjaFoOxkl45d6/v1/token", tokenResponse);

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);
            AuthenticationResponse authnResponse = await idxClient.VerifyAuthenticatorAsync(new VerifyAuthenticatorOptions { Code = "test code" }, Substitute.For<IIdxContext>());

            Assert.NotNull(authnResponse);
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
        public async Task SelectGoogleAuthenticatorAsSecondFactor()
        {
            var identifyResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02IlXiHkcgtacn3hIkIEVlW5PIHLu5l384952R6F9e"",
    ""expiresAt"": ""2021-12-07T12:22:06.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [{
                ""rel"": [""create-form""],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://super-idx.okta.com/idp/idx/credential/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [{
                        ""name"": ""authenticator"",
                        ""type"": ""object"",
                        ""options"": [{
                                ""label"": ""Email"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [{
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi3IuMuCpwD5d6"",
                                                ""mutable"": false
                                            }, {
                                                ""name"": ""methodType"",
                                                ""required"": false,
                                                ""value"": ""email"",
                                                ""mutable"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[0]""
                            }, {
                                ""label"": ""Google Authenticator"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [{
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""aut1i38ba5xfcX4cH5d7"",
                                                ""mutable"": false
                                            }, {
                                                ""name"": ""methodType"",
                                                ""required"": false,
                                                ""value"": ""otp"",
                                                ""mutable"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[1]""
                            }, {
                                ""label"": ""Phone"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [{
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi4eiZIdLK85d6"",
                                                ""mutable"": false
                                            }, {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [{
                                                        ""label"": ""SMS"",
                                                        ""value"": ""sms""
                                                    }
                                                ]
                                            }, {
                                                ""name"": ""phoneNumber"",
                                                ""label"": ""Phone number"",
                                                ""required"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[2]""
                            }
                        ]
                    }, {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02IlXiHkcgtacn3hIkIEVlW5PIHLu5l384952R6F9e"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }, {
                ""rel"": [""create-form""],
                ""name"": ""skip"",
                ""href"": ""https://super-idx.okta.com/idp/idx/skip"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [{
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02IlXiHkcgtacn3hIkIEVlW5PIHLu5l384952R6F9e"",
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
        ""value"": [{
                ""type"": ""email"",
                ""key"": ""okta_email"",
                ""id"": ""auttzfsi3IuMuCpwD5d6"",
                ""displayName"": ""Email"",
                ""methods"": [{
                        ""type"": ""email""
                    }
                ]
            }, {
                ""type"": ""app"",
                ""key"": ""google_otp"",
                ""id"": ""aut1i38ba5xfcX4cH5d7"",
                ""displayName"": ""Google Authenticator"",
                ""methods"": [{
                        ""type"": ""otp""
                    }
                ]
            }, {
                ""type"": ""phone"",
                ""key"": ""phone_number"",
                ""id"": ""auttzfsi4eiZIdLK85d6"",
                ""displayName"": ""Phone"",
                ""methods"": [{
                        ""type"": ""sms""
                    }
                ]
            }
        ]
    },
    ""authenticatorEnrollments"": {
        ""type"": ""array"",
        ""value"": [{
                ""type"": ""password"",
                ""key"": ""okta_password"",
                ""id"": ""lae93xvunvfTAeUQZ5d6"",
                ""displayName"": ""Password"",
                ""methods"": [{
                        ""type"": ""password""
                    }
                ]
            }
        ]
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00u2zf16p1pxeHZpY5d7"",
            ""identifier"": ""a@bb.ce"",
            ""profile"": {
                ""firstName"": ""first"",
                ""lastName"": ""last"",
                ""timeZone"": ""America/Los_Angeles"",
                ""locale"": ""en_US""
            }
        }
    },
    ""cancel"": {
        ""rel"": [""create-form""],
        ""name"": ""cancel"",
        ""href"": ""https://dotnet-idx-sdk.okta.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [{
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02IlXiHkcgtacn3hIkIEVlW5PIHLu5l384952R6F9e"",
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
            var selectGAResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02IlXiHkcgtacn3hIkIEVlW5PIHLu5l384952R6F9e"",
    ""expiresAt"": ""2021-12-07T12:22:14.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [{
                ""rel"": [""create-form""],
                ""name"": ""enroll-authenticator"",
                ""relatesTo"": [""$.currentAuthenticator""],
                ""href"": ""https://super-idx.okta.com/idp/idx/challenge/answer"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [{
                        ""name"": ""credentials"",
                        ""type"": ""object"",
                        ""form"": {
                            ""value"": [{
                                    ""name"": ""passcode"",
                                    ""label"": ""Enter code"",
                                    ""required"": true
                                }
                            ]
                        },
                        ""required"": true
                    }, {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02IlXiHkcgtacn3hIkIEVlW5PIHLu5l384952R6F9e"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }, {
                ""rel"": [""create-form""],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://super-idx.okta.com/idp/idx/credential/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [{
                        ""name"": ""authenticator"",
                        ""type"": ""object"",
                        ""options"": [{
                                ""label"": ""Email"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [{
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi3IuMuCpwD5d6"",
                                                ""mutable"": false
                                            }, {
                                                ""name"": ""methodType"",
                                                ""required"": false,
                                                ""value"": ""email"",
                                                ""mutable"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[0]""
                            }, {
                                ""label"": ""Google Authenticator"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [{
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""aut1i38ba5xfcX4cH5d7"",
                                                ""mutable"": false
                                            }, {
                                                ""name"": ""methodType"",
                                                ""required"": false,
                                                ""value"": ""otp"",
                                                ""mutable"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[1]""
                            }, {
                                ""label"": ""Phone"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [{
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi4eiZIdLK85d6"",
                                                ""mutable"": false
                                            }, {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [{
                                                        ""label"": ""SMS"",
                                                        ""value"": ""sms""
                                                    }
                                                ]
                                            }, {
                                                ""name"": ""phoneNumber"",
                                                ""label"": ""Phone number"",
                                                ""required"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[2]""
                            }
                        ]
                    }, {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02IlXiHkcgtacn3hIkIEVlW5PIHLu5l384952R6F9e"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }, {
                ""rel"": [""create-form""],
                ""name"": ""skip"",
                ""href"": ""https://super-idx.okta.com/idp/idx/skip"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [{
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02IlXiHkcgtacn3hIkIEVlW5PIHLu5l384952R6F9e"",
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
            ""contextualData"": {
                ""qrcode"": {
                    ""method"": ""embedded"",
                    ""href"": ""data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAMgAAADICAYAAACtWggg=="",
                    ""type"": ""image/png""
                },
                ""sharedSecret"": ""KAUWFBHWMDFO75AG""
            },
            ""type"": ""app"",
            ""key"": ""google_otp"",
            ""id"": ""aut1i38ba5xfcX4cH5d7"",
            ""displayName"": ""Google Authenticator"",
            ""methods"": [{
                    ""type"": ""otp""
                }
            ]
        }
    },
    ""authenticators"": {
        ""type"": ""array"",
        ""value"": [{
                ""type"": ""email"",
                ""key"": ""okta_email"",
                ""id"": ""auttzfsi3IuMuCpwD5d6"",
                ""displayName"": ""Email"",
                ""methods"": [{
                        ""type"": ""email""
                    }
                ]
            }, {
                ""type"": ""app"",
                ""key"": ""google_otp"",
                ""id"": ""aut1i38ba5xfcX4cH5d7"",
                ""displayName"": ""Google Authenticator"",
                ""methods"": [{
                        ""type"": ""otp""
                    }
                ]
            }, {
                ""type"": ""phone"",
                ""key"": ""phone_number"",
                ""id"": ""auttzfsi4eiZIdLK85d6"",
                ""displayName"": ""Phone"",
                ""methods"": [{
                        ""type"": ""sms""
                    }
                ]
            }
        ]
    },
    ""authenticatorEnrollments"": {
        ""type"": ""array"",
        ""value"": [{
                ""type"": ""password"",
                ""key"": ""okta_password"",
                ""id"": ""lae93xvunvfTAeUQZ5d6"",
                ""displayName"": ""Password"",
                ""methods"": [{
                        ""type"": ""password""
                    }
                ]
            }
        ]
    },
    ""enrollmentAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""app"",
            ""key"": ""google_otp"",
            ""id"": ""aut1i38ba5xfcX4cH5d7"",
            ""displayName"": ""Google Authenticator"",
            ""methods"": [{
                    ""type"": ""otp""
                }
            ]
        }
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00u2zf16p1pxeHZpY5d7"",
            ""identifier"": ""a@bb.ce"",
            ""profile"": {
                ""firstName"": ""first"",
                ""lastName"": ""last"",
                ""timeZone"": ""America/Los_Angeles"",
                ""locale"": ""en_US""
            }
        }
    },
    ""cancel"": {
        ""rel"": [""create-form""],
        ""name"": ""cancel"",
        ""href"": ""https://super-idx.okta.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [{
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02IlXiHkcgtacn3hIkIEVlW5PIHLu5l384952R6F9e"",
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
            
            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = selectGAResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.SelectEnrollAuthenticatorAsync(
                                   new SelectEnrollAuthenticatorOptions
                                   {
                                       AuthenticatorId = "aut1i38ba5xfcX4cH5d7"
                                   }, Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingAuthenticatorVerification);
            authResponse.CurrentAuthenticator.Id.Should().Be("aut1i38ba5xfcX4cH5d7");
            authResponse.CurrentAuthenticator.ContextualData.QrCode.Should().NotBeNull();
            authResponse.CurrentAuthenticator.ContextualData.QrCode.Href.Should().NotBeNull();
            authResponse.CurrentAuthenticator.ContextualData.SharedSecret.Should().Be("KAUWFBHWMDFO75AG");
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
        public async Task ResendCodeDuringEnrollPhone()
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

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = enrollPhoneResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = enrollPhoneResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.ResendCodeAsync(Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingAuthenticatorVerification);
            authResponse.CurrentAuthenticator.Should().NotBeNull();
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

        [Fact] public async Task SelectWebAuthnForEnrollment()
        {
            var identifyResponse = @"{
                                                ""version"": ""1.0.0"",
                                                ""stateHandle"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
                                                ""expiresAt"": ""2021-12-22T18:05:30.000Z"",
                                                ""intent"": ""LOGIN"",
                                                ""remediation"": {
                                                  ""type"": ""array"",
                                                  ""value"": [
                                                    {
                                                      ""rel"": [
                                                        ""create-form""
                                                      ],
                                                      ""name"": ""select-authenticator-enroll"",
                                                      ""href"": ""https://testdomain.com/idp/idx/credential/enroll"",
                                                      ""method"": ""POST"",
                                                      ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                                      ""value"": [
                                                        {
                                                          ""name"": ""authenticator"",
                                                          ""type"": ""object"",
                                                          ""options"": [
                                                            {
                                                              ""label"": ""Google Authenticator"",
                                                              ""value"": {
                                                                ""form"": {
                                                                  ""value"": [
                                                                    {
                                                                      ""name"": ""id"",
                                                                      ""required"": true,
                                                                      ""value"": ""aut2xo7zveTcXMZWW5d7"",
                                                                      ""mutable"": false
                                                                    },
                                                                    {
                                                                      ""name"": ""methodType"",
                                                                      ""required"": false,
                                                                      ""value"": ""otp"",
                                                                      ""mutable"": false
                                                                    }
                                                                  ]
                                                                }
                                                              },
                                                              ""relatesTo"": ""$.authenticators.value[0]""
                                                            },
                                                            {
                                                              ""label"": ""Okta Verify"",
                                                              ""value"": {
                                                                ""form"": {
                                                                  ""value"": [
                                                                    {
                                                                      ""name"": ""id"",
                                                                      ""required"": true,
                                                                      ""value"": ""aut2xo0xj4LK7Fupb5d7"",
                                                                      ""mutable"": false
                                                                    },
                                                                    {
                                                                      ""name"": ""channel"",
                                                                      ""type"": ""string"",
                                                                      ""required"": false,
                                                                      ""options"": [
                                                                        {
                                                                          ""label"": ""QRCODE"",
                                                                          ""value"": ""qrcode""
                                                                        },
                                                                        {
                                                                          ""label"": ""EMAIL"",
                                                                          ""value"": ""email""
                                                                        },
                                                                        {
                                                                          ""label"": ""SMS"",
                                                                          ""value"": ""sms""
                                                                        }
                                                                      ]
                                                                    }
                                                                  ]
                                                                }
                                                              },
                                                              ""relatesTo"": ""$.authenticators.value[1]""
                                                            },
                                                            {
                                                              ""label"": ""Phone"",
                                                              ""value"": {
                                                                ""form"": {
                                                                  ""value"": [
                                                                    {
                                                                      ""name"": ""id"",
                                                                      ""required"": true,
                                                                      ""value"": ""aut2xo0xj28i2PjX15d7"",
                                                                      ""mutable"": false
                                                                    },
                                                                    {
                                                                      ""name"": ""methodType"",
                                                                      ""type"": ""string"",
                                                                      ""required"": false,
                                                                      ""options"": [
                                                                        {
                                                                          ""label"": ""SMS"",
                                                                          ""value"": ""sms""
                                                                        },
                                                                        {
                                                                          ""label"": ""Voice call"",
                                                                          ""value"": ""voice""
                                                                        }
                                                                      ]
                                                                    },
                                                                    {
                                                                      ""name"": ""phoneNumber"",
                                                                      ""label"": ""Phone number"",
                                                                      ""required"": false
                                                                    }
                                                                  ]
                                                                }
                                                              },
                                                              ""relatesTo"": ""$.authenticators.value[2]""
                                                            },
                                                            {
                                                              ""label"": ""Security Key or Biometric"",
                                                              ""value"": {
                                                                ""form"": {
                                                                  ""value"": [
                                                                    {
                                                                      ""name"": ""id"",
                                                                      ""required"": true,
                                                                      ""value"": ""aut2xoroxctf7gf5i5d7"",
                                                                      ""mutable"": false
                                                                    },
                                                                    {
                                                                      ""name"": ""methodType"",
                                                                      ""required"": false,
                                                                      ""value"": ""webauthn"",
                                                                      ""mutable"": false
                                                                    }
                                                                  ]
                                                                }
                                                              },
                                                              ""relatesTo"": ""$.authenticators.value[3]""
                                                            }
                                                          ]
                                                        },
                                                        {
                                                          ""name"": ""stateHandle"",
                                                          ""required"": true,
                                                          ""value"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
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
                                                      ""type"": ""app"",
                                                      ""key"": ""google_otp"",
                                                      ""id"": ""aut2xo7zveTcXMZWW5d7"",
                                                      ""displayName"": ""Google Authenticator"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""otp""
                                                        }
                                                      ]
                                                    },
                                                    {
                                                      ""type"": ""app"",
                                                      ""key"": ""okta_verify"",
                                                      ""id"": ""aut2xo0xj4LK7Fupb5d7"",
                                                      ""displayName"": ""Okta Verify"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""push""
                                                        },
                                                        {
                                                          ""type"": ""signed_nonce""
                                                        },
                                                        {
                                                          ""type"": ""totp""
                                                        }
                                                      ]
                                                    },
                                                    {
                                                      ""type"": ""phone"",
                                                      ""key"": ""phone_number"",
                                                      ""id"": ""aut2xo0xj28i2PjX15d7"",
                                                      ""displayName"": ""Phone"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""sms""
                                                        },
                                                        {
                                                          ""type"": ""voice""
                                                        }
                                                      ]
                                                    },
                                                    {
                                                      ""type"": ""security_key"",
                                                      ""key"": ""webauthn"",
                                                      ""id"": ""aut2xoroxctf7gf5i5d7"",
                                                      ""displayName"": ""Security Key or Biometric"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""webauthn""
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
                                                      ""id"": ""eae3epqy1ekfF5rdf5d7"",
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
                                                      ""id"": ""lae9skpergPf1afbj5d6"",
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
                                                    ""id"": ""00u3epqr99CQeA3mW5d7"",
                                                    ""identifier"": ""testuser@test.com"",
                                                    ""profile"": {
                                                      ""firstName"": ""Laura"",
                                                      ""lastName"": ""T"",
                                                      ""timeZone"": ""America/Los_Angeles"",
                                                      ""locale"": ""en_US""
                                                    }
                                                  }
                                                }
                                              }";
            var selectWebAuthnResponse = @"{
                                                ""version"": ""1.0.0"",
                                                ""stateHandle"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
                                                ""expiresAt"": ""2021-12-22T18:05:39.000Z"",
                                                ""intent"": ""LOGIN"",
                                                ""remediation"": {
                                                  ""type"": ""array"",
                                                  ""value"": [
                                                    {
                                                      ""rel"": [
                                                        ""create-form""
                                                      ],
                                                      ""name"": ""enroll-authenticator"",
                                                      ""relatesTo"": [
                                                        ""$.currentAuthenticator""
                                                      ],
                                                      ""href"": ""https://testorg.com/idp/idx/challenge/answer"",
                                                      ""method"": ""POST"",
                                                      ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                                      ""value"": [
                                                        {
                                                          ""name"": ""credentials"",
                                                          ""type"": ""object"",
                                                          ""form"": {
                                                            ""value"": [
                                                              {
                                                                ""name"": ""attestation"",
                                                                ""label"": ""Attestation"",
                                                                ""required"": true,
                                                                ""visible"": false
                                                              },
                                                              {
                                                                ""name"": ""clientData"",
                                                                ""label"": ""Client Data"",
                                                                ""required"": true,
                                                                ""visible"": false
                                                              }
                                                            ]
                                                          },
                                                          ""required"": true
                                                        },
                                                        {
                                                          ""name"": ""stateHandle"",
                                                          ""required"": true,
                                                          ""value"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
                                                          ""visible"": false,
                                                          ""mutable"": false
                                                        }
                                                      ],
                                                      ""accepts"": ""application/json; okta-version=1.0.0""
                                                    },
                                                    {
                                                      ""rel"": [
                                                        ""create-form""
                                                      ],
                                                      ""name"": ""select-authenticator-enroll"",
                                                      ""href"": ""https://testorg.com/idp/idx/credential/enroll"",
                                                      ""method"": ""POST"",
                                                      ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                                      ""value"": [
                                                        {
                                                          ""name"": ""authenticator"",
                                                          ""type"": ""object"",
                                                          ""options"": [
                                                            {
                                                              ""label"": ""Google Authenticator"",
                                                              ""value"": {
                                                                ""form"": {
                                                                  ""value"": [
                                                                    {
                                                                      ""name"": ""id"",
                                                                      ""required"": true,
                                                                      ""value"": ""aut2xo7zveTcXMZWW5d7"",
                                                                      ""mutable"": false
                                                                    },
                                                                    {
                                                                      ""name"": ""methodType"",
                                                                      ""required"": false,
                                                                      ""value"": ""otp"",
                                                                      ""mutable"": false
                                                                    }
                                                                  ]
                                                                }
                                                              },
                                                              ""relatesTo"": ""$.authenticators.value[0]""
                                                            },
                                                            {
                                                              ""label"": ""Okta Verify"",
                                                              ""value"": {
                                                                ""form"": {
                                                                  ""value"": [
                                                                    {
                                                                      ""name"": ""id"",
                                                                      ""required"": true,
                                                                      ""value"": ""aut2xo0xj4LK7Fupb5d7"",
                                                                      ""mutable"": false
                                                                    },
                                                                    {
                                                                      ""name"": ""channel"",
                                                                      ""type"": ""string"",
                                                                      ""required"": false,
                                                                      ""options"": [
                                                                        {
                                                                          ""label"": ""QRCODE"",
                                                                          ""value"": ""qrcode""
                                                                        },
                                                                        {
                                                                          ""label"": ""EMAIL"",
                                                                          ""value"": ""email""
                                                                        },
                                                                        {
                                                                          ""label"": ""SMS"",
                                                                          ""value"": ""sms""
                                                                        }
                                                                      ]
                                                                    }
                                                                  ]
                                                                }
                                                              },
                                                              ""relatesTo"": ""$.authenticators.value[1]""
                                                            },
                                                            {
                                                              ""label"": ""Phone"",
                                                              ""value"": {
                                                                ""form"": {
                                                                  ""value"": [
                                                                    {
                                                                      ""name"": ""id"",
                                                                      ""required"": true,
                                                                      ""value"": ""aut2xo0xj28i2PjX15d7"",
                                                                      ""mutable"": false
                                                                    },
                                                                    {
                                                                      ""name"": ""methodType"",
                                                                      ""type"": ""string"",
                                                                      ""required"": false,
                                                                      ""options"": [
                                                                        {
                                                                          ""label"": ""SMS"",
                                                                          ""value"": ""sms""
                                                                        },
                                                                        {
                                                                          ""label"": ""Voice call"",
                                                                          ""value"": ""voice""
                                                                        }
                                                                      ]
                                                                    },
                                                                    {
                                                                      ""name"": ""phoneNumber"",
                                                                      ""label"": ""Phone number"",
                                                                      ""required"": false
                                                                    }
                                                                  ]
                                                                }
                                                              },
                                                              ""relatesTo"": ""$.authenticators.value[2]""
                                                            },
                                                            {
                                                              ""label"": ""Security Key or Biometric"",
                                                              ""value"": {
                                                                ""form"": {
                                                                  ""value"": [
                                                                    {
                                                                      ""name"": ""id"",
                                                                      ""required"": true,
                                                                      ""value"": ""aut2xoroxctf7gf5i5d7"",
                                                                      ""mutable"": false
                                                                    },
                                                                    {
                                                                      ""name"": ""methodType"",
                                                                      ""required"": false,
                                                                      ""value"": ""webauthn"",
                                                                      ""mutable"": false
                                                                    }
                                                                  ]
                                                                }
                                                              },
                                                              ""relatesTo"": ""$.authenticators.value[3]""
                                                            }
                                                          ]
                                                        },
                                                        {
                                                          ""name"": ""stateHandle"",
                                                          ""required"": true,
                                                          ""value"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
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
                                                    ""contextualData"": {
                                                      ""activationData"": {
                                                        ""rp"": {
                                                          ""name"": ""Dotnet SDK IDX""
                                                        },
                                                        ""user"": {
                                                          ""displayName"": ""Laura T"",
                                                          ""name"": ""testuser@test.com"",
                                                          ""id"": ""00u3epqr99CQeA3mW5d7""
                                                        },
                                                        ""pubKeyCredParams"": [
                                                          {
                                                            ""type"": ""public-key"",
                                                            ""alg"": -7
                                                          },
                                                          {
                                                            ""type"": ""public-key"",
                                                            ""alg"": -257
                                                          }
                                                        ],
                                                        ""challenge"": ""cqjIFQtw2sV82g0y_xSgGA9Kc0E"",
                                                        ""attestation"": ""direct"",
                                                        ""authenticatorSelection"": {
                                                          ""userVerification"": ""discouraged"",
                                                          ""requireResidentKey"": false
                                                        },
                                                        ""u2fParams"": {
                                                          ""appid"": ""https://testorg.com""
                                                        },
                                                        ""excludeCredentials"": []
                                                      }
                                                    },
                                                    ""type"": ""security_key"",
                                                    ""key"": ""webauthn"",
                                                    ""id"": ""aut2xoroxctf7gf5i5d7"",
                                                    ""displayName"": ""Security Key or Biometric"",
                                                    ""methods"": [
                                                      {
                                                        ""type"": ""webauthn""
                                                      }
                                                    ]
                                                  }
                                                },
                                                ""authenticators"": {
                                                  ""type"": ""array"",
                                                  ""value"": [
                                                    {
                                                      ""type"": ""app"",
                                                      ""key"": ""google_otp"",
                                                      ""id"": ""aut2xo7zveTcXMZWW5d7"",
                                                      ""displayName"": ""Google Authenticator"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""otp""
                                                        }
                                                      ]
                                                    },
                                                    {
                                                      ""type"": ""app"",
                                                      ""key"": ""okta_verify"",
                                                      ""id"": ""aut2xo0xj4LK7Fupb5d7"",
                                                      ""displayName"": ""Okta Verify"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""push""
                                                        },
                                                        {
                                                          ""type"": ""signed_nonce""
                                                        },
                                                        {
                                                          ""type"": ""totp""
                                                        }
                                                      ]
                                                    },
                                                    {
                                                      ""type"": ""phone"",
                                                      ""key"": ""phone_number"",
                                                      ""id"": ""aut2xo0xj28i2PjX15d7"",
                                                      ""displayName"": ""Phone"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""sms""
                                                        },
                                                        {
                                                          ""type"": ""voice""
                                                        }
                                                      ]
                                                    },
                                                    {
                                                      ""type"": ""security_key"",
                                                      ""key"": ""webauthn"",
                                                      ""id"": ""aut2xoroxctf7gf5i5d7"",
                                                      ""displayName"": ""Security Key or Biometric"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""webauthn""
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
                                                      ""id"": ""eae3epqy1ekfF5rdf5d7"",
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
                                                      ""id"": ""lae9skpergPf1afbj5d6"",
                                                      ""displayName"": ""Password"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""password""
                                                        }
                                                      ]
                                                    }
                                                  ]
                                                },
                                                ""enrollmentAuthenticator"": {
                                                  ""type"": ""object"",
                                                  ""value"": {
                                                    ""type"": ""security_key"",
                                                    ""key"": ""webauthn"",
                                                    ""id"": ""aut2xoroxctf7gf5i5d7"",
                                                    ""displayName"": ""Security Key or Biometric"",
                                                    ""methods"": [
                                                      {
                                                        ""type"": ""webauthn""
                                                      }
                                                    ]
                                                  }
                                                },
                                                ""user"": {
                                                  ""type"": ""object"",
                                                  ""value"": {
                                                    ""id"": ""00u3epqr99CQeA3mW5d7"",
                                                    ""identifier"": ""testuser@test.com"",
                                                    ""profile"": {
                                                      ""firstName"": ""Laura"",
                                                      ""lastName"": ""T"",
                                                      ""timeZone"": ""America/Los_Angeles"",
                                                      ""locale"": ""en_US""
                                                    }
                                                  }
                                                },
                                                ""cancel"": {
                                                  ""rel"": [
                                                    ""create-form""
                                                  ],
                                                  ""name"": ""cancel"",
                                                  ""href"": ""https://testorg.com/idp/idx/cancel"",
                                                  ""method"": ""POST"",
                                                  ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                                  ""value"": [
                                                    {
                                                      ""name"": ""stateHandle"",
                                                      ""required"": true,
                                                      ""value"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
                                                      ""visible"": false,
                                                      ""mutable"": false
                                                    }
                                                  ],
                                                  ""accepts"": ""application/json; okta-version=1.0.0""
                                                }
                                              }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = selectWebAuthnResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.SelectEnrollAuthenticatorAsync(
                                   new SelectEnrollAuthenticatorOptions
                                   {
                                       AuthenticatorId = "aut2xoroxctf7gf5i5d7",

                                   }, Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingAuthenticatorVerification);
            authResponse.CurrentAuthenticator.Id.Should().Be("aut2xoroxctf7gf5i5d7");
            authResponse.CurrentAuthenticator.Name.ToLower().Should().Be("security key or biometric");
            authResponse.CurrentAuthenticator.MethodTypes.Should().Contain("webauthn");
            authResponse.CurrentAuthenticator.ContextualData.ActivationData.Attestation.Should().Be("direct");
            authResponse.CurrentAuthenticator.ContextualData.ActivationData.Challenge.Should().Be("cqjIFQtw2sV82g0y_xSgGA9Kc0E");
            authResponse.CurrentAuthenticator.ContextualData.ActivationData.AuthenticatorSelection.RequireResidentKey.Should().Be(false);
            authResponse.CurrentAuthenticator.ContextualData.ActivationData.AuthenticatorSelection.UserVerification.Should().Be("discouraged");
            authResponse.CurrentAuthenticator.ContextualData.ActivationData.PublicKeyCredParams.Should().HaveCount(2);
            // TODO:// check RP and u2fParams
            authResponse.CurrentAuthenticator.ContextualData.ActivationData.U2fParams.ApplicationId.Should()
                .Be("https://testorg.com");
            authResponse.CurrentAuthenticator.ContextualData.ActivationData.User.Name.Should().Be("testuser@test.com");
            authResponse.CurrentAuthenticator.ContextualData.ActivationData.User.DisplayName.Should().Be("Laura T");
            authResponse.CurrentAuthenticator.ContextualData.ActivationData.User.Id.Should().Be("00u3epqr99CQeA3mW5d7");
        }

        [Fact]
        public async Task EnrollWebAuthn()
        {
            var identifyResponse = @"{
                                                ""version"": ""1.0.0"",
                                                ""stateHandle"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
                                                ""expiresAt"": ""2021-12-22T18:05:39.000Z"",
                                                ""intent"": ""LOGIN"",
                                                ""remediation"": {
                                                  ""type"": ""array"",
                                                  ""value"": [
                                                    {
                                                      ""rel"": [
                                                        ""create-form""
                                                      ],
                                                      ""name"": ""enroll-authenticator"",
                                                      ""relatesTo"": [
                                                        ""$.currentAuthenticator""
                                                      ],
                                                      ""href"": ""https://testorg.com/idp/idx/challenge/answer"",
                                                      ""method"": ""POST"",
                                                      ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                                      ""value"": [
                                                        {
                                                          ""name"": ""credentials"",
                                                          ""type"": ""object"",
                                                          ""form"": {
                                                            ""value"": [
                                                              {
                                                                ""name"": ""attestation"",
                                                                ""label"": ""Attestation"",
                                                                ""required"": true,
                                                                ""visible"": false
                                                              },
                                                              {
                                                                ""name"": ""clientData"",
                                                                ""label"": ""Client Data"",
                                                                ""required"": true,
                                                                ""visible"": false
                                                              }
                                                            ]
                                                          },
                                                          ""required"": true
                                                        },
                                                        {
                                                          ""name"": ""stateHandle"",
                                                          ""required"": true,
                                                          ""value"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
                                                          ""visible"": false,
                                                          ""mutable"": false
                                                        }
                                                      ],
                                                      ""accepts"": ""application/json; okta-version=1.0.0""
                                                    },
                                                    {
                                                      ""rel"": [
                                                        ""create-form""
                                                      ],
                                                      ""name"": ""select-authenticator-enroll"",
                                                      ""href"": ""https://testorg.com/idp/idx/credential/enroll"",
                                                      ""method"": ""POST"",
                                                      ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                                      ""value"": [
                                                        {
                                                          ""name"": ""authenticator"",
                                                          ""type"": ""object"",
                                                          ""options"": [
                                                            {
                                                              ""label"": ""Google Authenticator"",
                                                              ""value"": {
                                                                ""form"": {
                                                                  ""value"": [
                                                                    {
                                                                      ""name"": ""id"",
                                                                      ""required"": true,
                                                                      ""value"": ""aut2xo7zveTcXMZWW5d7"",
                                                                      ""mutable"": false
                                                                    },
                                                                    {
                                                                      ""name"": ""methodType"",
                                                                      ""required"": false,
                                                                      ""value"": ""otp"",
                                                                      ""mutable"": false
                                                                    }
                                                                  ]
                                                                }
                                                              },
                                                              ""relatesTo"": ""$.authenticators.value[0]""
                                                            },
                                                            {
                                                              ""label"": ""Okta Verify"",
                                                              ""value"": {
                                                                ""form"": {
                                                                  ""value"": [
                                                                    {
                                                                      ""name"": ""id"",
                                                                      ""required"": true,
                                                                      ""value"": ""aut2xo0xj4LK7Fupb5d7"",
                                                                      ""mutable"": false
                                                                    },
                                                                    {
                                                                      ""name"": ""channel"",
                                                                      ""type"": ""string"",
                                                                      ""required"": false,
                                                                      ""options"": [
                                                                        {
                                                                          ""label"": ""QRCODE"",
                                                                          ""value"": ""qrcode""
                                                                        },
                                                                        {
                                                                          ""label"": ""EMAIL"",
                                                                          ""value"": ""email""
                                                                        },
                                                                        {
                                                                          ""label"": ""SMS"",
                                                                          ""value"": ""sms""
                                                                        }
                                                                      ]
                                                                    }
                                                                  ]
                                                                }
                                                              },
                                                              ""relatesTo"": ""$.authenticators.value[1]""
                                                            },
                                                            {
                                                              ""label"": ""Phone"",
                                                              ""value"": {
                                                                ""form"": {
                                                                  ""value"": [
                                                                    {
                                                                      ""name"": ""id"",
                                                                      ""required"": true,
                                                                      ""value"": ""aut2xo0xj28i2PjX15d7"",
                                                                      ""mutable"": false
                                                                    },
                                                                    {
                                                                      ""name"": ""methodType"",
                                                                      ""type"": ""string"",
                                                                      ""required"": false,
                                                                      ""options"": [
                                                                        {
                                                                          ""label"": ""SMS"",
                                                                          ""value"": ""sms""
                                                                        },
                                                                        {
                                                                          ""label"": ""Voice call"",
                                                                          ""value"": ""voice""
                                                                        }
                                                                      ]
                                                                    },
                                                                    {
                                                                      ""name"": ""phoneNumber"",
                                                                      ""label"": ""Phone number"",
                                                                      ""required"": false
                                                                    }
                                                                  ]
                                                                }
                                                              },
                                                              ""relatesTo"": ""$.authenticators.value[2]""
                                                            },
                                                            {
                                                              ""label"": ""Security Key or Biometric"",
                                                              ""value"": {
                                                                ""form"": {
                                                                  ""value"": [
                                                                    {
                                                                      ""name"": ""id"",
                                                                      ""required"": true,
                                                                      ""value"": ""aut2xoroxctf7gf5i5d7"",
                                                                      ""mutable"": false
                                                                    },
                                                                    {
                                                                      ""name"": ""methodType"",
                                                                      ""required"": false,
                                                                      ""value"": ""webauthn"",
                                                                      ""mutable"": false
                                                                    }
                                                                  ]
                                                                }
                                                              },
                                                              ""relatesTo"": ""$.authenticators.value[3]""
                                                            }
                                                          ]
                                                        },
                                                        {
                                                          ""name"": ""stateHandle"",
                                                          ""required"": true,
                                                          ""value"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
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
                                                    ""contextualData"": {
                                                      ""activationData"": {
                                                        ""rp"": {
                                                          ""name"": ""Dotnet SDK IDX""
                                                        },
                                                        ""user"": {
                                                          ""displayName"": ""Laura T"",
                                                          ""name"": ""testuser@test.com"",
                                                          ""id"": ""00u3epqr99CQeA3mW5d7""
                                                        },
                                                        ""pubKeyCredParams"": [
                                                          {
                                                            ""type"": ""public-key"",
                                                            ""alg"": -7
                                                          },
                                                          {
                                                            ""type"": ""public-key"",
                                                            ""alg"": -257
                                                          }
                                                        ],
                                                        ""challenge"": ""cqjIFQtw2sV82g0y_xSgGA9Kc0E"",
                                                        ""attestation"": ""direct"",
                                                        ""authenticatorSelection"": {
                                                          ""userVerification"": ""discouraged"",
                                                          ""requireResidentKey"": false
                                                        },
                                                        ""u2fParams"": {
                                                          ""appid"": ""https://testorg.com""
                                                        },
                                                        ""excludeCredentials"": []
                                                      }
                                                    },
                                                    ""type"": ""security_key"",
                                                    ""key"": ""webauthn"",
                                                    ""id"": ""aut2xoroxctf7gf5i5d7"",
                                                    ""displayName"": ""Security Key or Biometric"",
                                                    ""methods"": [
                                                      {
                                                        ""type"": ""webauthn""
                                                      }
                                                    ]
                                                  }
                                                },
                                                ""authenticators"": {
                                                  ""type"": ""array"",
                                                  ""value"": [
                                                    {
                                                      ""type"": ""app"",
                                                      ""key"": ""google_otp"",
                                                      ""id"": ""aut2xo7zveTcXMZWW5d7"",
                                                      ""displayName"": ""Google Authenticator"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""otp""
                                                        }
                                                      ]
                                                    },
                                                    {
                                                      ""type"": ""app"",
                                                      ""key"": ""okta_verify"",
                                                      ""id"": ""aut2xo0xj4LK7Fupb5d7"",
                                                      ""displayName"": ""Okta Verify"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""push""
                                                        },
                                                        {
                                                          ""type"": ""signed_nonce""
                                                        },
                                                        {
                                                          ""type"": ""totp""
                                                        }
                                                      ]
                                                    },
                                                    {
                                                      ""type"": ""phone"",
                                                      ""key"": ""phone_number"",
                                                      ""id"": ""aut2xo0xj28i2PjX15d7"",
                                                      ""displayName"": ""Phone"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""sms""
                                                        },
                                                        {
                                                          ""type"": ""voice""
                                                        }
                                                      ]
                                                    },
                                                    {
                                                      ""type"": ""security_key"",
                                                      ""key"": ""webauthn"",
                                                      ""id"": ""aut2xoroxctf7gf5i5d7"",
                                                      ""displayName"": ""Security Key or Biometric"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""webauthn""
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
                                                      ""id"": ""eae3epqy1ekfF5rdf5d7"",
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
                                                      ""id"": ""lae9skpergPf1afbj5d6"",
                                                      ""displayName"": ""Password"",
                                                      ""methods"": [
                                                        {
                                                          ""type"": ""password""
                                                        }
                                                      ]
                                                    }
                                                  ]
                                                },
                                                ""enrollmentAuthenticator"": {
                                                  ""type"": ""object"",
                                                  ""value"": {
                                                    ""type"": ""security_key"",
                                                    ""key"": ""webauthn"",
                                                    ""id"": ""aut2xoroxctf7gf5i5d7"",
                                                    ""displayName"": ""Security Key or Biometric"",
                                                    ""methods"": [
                                                      {
                                                        ""type"": ""webauthn""
                                                      }
                                                    ]
                                                  }
                                                },
                                                ""user"": {
                                                  ""type"": ""object"",
                                                  ""value"": {
                                                    ""id"": ""00u3epqr99CQeA3mW5d7"",
                                                    ""identifier"": ""testuser@test.com"",
                                                    ""profile"": {
                                                      ""firstName"": ""Laura"",
                                                      ""lastName"": ""T"",
                                                      ""timeZone"": ""America/Los_Angeles"",
                                                      ""locale"": ""en_US""
                                                    }
                                                  }
                                                },
                                                ""cancel"": {
                                                  ""rel"": [
                                                    ""create-form""
                                                  ],
                                                  ""name"": ""cancel"",
                                                  ""href"": ""https://testorg.com/idp/idx/cancel"",
                                                  ""method"": ""POST"",
                                                  ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                                  ""value"": [
                                                    {
                                                      ""name"": ""stateHandle"",
                                                      ""required"": true,
                                                      ""value"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
                                                      ""visible"": false,
                                                      ""mutable"": false
                                                    }
                                                  ],
                                                  ""accepts"": ""application/json; okta-version=1.0.0""
                                                }
                                              }";
            var enrollResponse = @"{
                                    ""version"": ""1.0.0"",
                                    ""stateHandle"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
                                    ""expiresAt"": ""2021-12-22T18:05:52.000Z"",
                                    ""intent"": ""LOGIN"",
                                    ""remediation"": {
                                      ""type"": ""array"",
                                      ""value"": [
                                        {
                                          ""rel"": [
                                            ""create-form""
                                          ],
                                          ""name"": ""select-authenticator-enroll"",
                                          ""href"": ""https://testdomain.com/idp/idx/credential/enroll"",
                                          ""method"": ""POST"",
                                          ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                          ""value"": [
                                            {
                                              ""name"": ""authenticator"",
                                              ""type"": ""object"",
                                              ""options"": [
                                                {
                                                  ""label"": ""Google Authenticator"",
                                                  ""value"": {
                                                    ""form"": {
                                                      ""value"": [
                                                        {
                                                          ""name"": ""id"",
                                                          ""required"": true,
                                                          ""value"": ""aut2xo7zveTcXMZWW5d7"",
                                                          ""mutable"": false
                                                        },
                                                        {
                                                          ""name"": ""methodType"",
                                                          ""required"": false,
                                                          ""value"": ""otp"",
                                                          ""mutable"": false
                                                        }
                                                      ]
                                                    }
                                                  },
                                                  ""relatesTo"": ""$.authenticators.value[0]""
                                                },
                                                {
                                                  ""label"": ""Okta Verify"",
                                                  ""value"": {
                                                    ""form"": {
                                                      ""value"": [
                                                        {
                                                          ""name"": ""id"",
                                                          ""required"": true,
                                                          ""value"": ""aut2xo0xj4LK7Fupb5d7"",
                                                          ""mutable"": false
                                                        },
                                                        {
                                                          ""name"": ""channel"",
                                                          ""type"": ""string"",
                                                          ""required"": false,
                                                          ""options"": [
                                                            {
                                                              ""label"": ""QRCODE"",
                                                              ""value"": ""qrcode""
                                                            },
                                                            {
                                                              ""label"": ""EMAIL"",
                                                              ""value"": ""email""
                                                            },
                                                            {
                                                              ""label"": ""SMS"",
                                                              ""value"": ""sms""
                                                            }
                                                          ]
                                                        }
                                                      ]
                                                    }
                                                  },
                                                  ""relatesTo"": ""$.authenticators.value[1]""
                                                },
                                                {
                                                  ""label"": ""Phone"",
                                                  ""value"": {
                                                    ""form"": {
                                                      ""value"": [
                                                        {
                                                          ""name"": ""id"",
                                                          ""required"": true,
                                                          ""value"": ""aut2xo0xj28i2PjX15d7"",
                                                          ""mutable"": false
                                                        },
                                                        {
                                                          ""name"": ""methodType"",
                                                          ""type"": ""string"",
                                                          ""required"": false,
                                                          ""options"": [
                                                            {
                                                              ""label"": ""SMS"",
                                                              ""value"": ""sms""
                                                            },
                                                            {
                                                              ""label"": ""Voice call"",
                                                              ""value"": ""voice""
                                                            }
                                                          ]
                                                        },
                                                        {
                                                          ""name"": ""phoneNumber"",
                                                          ""label"": ""Phone number"",
                                                          ""required"": false
                                                        }
                                                      ]
                                                    }
                                                  },
                                                  ""relatesTo"": ""$.authenticators.value[2]""
                                                },
                                                {
                                                  ""label"": ""Security Question"",
                                                  ""value"": {
                                                    ""form"": {
                                                      ""value"": [
                                                        {
                                                          ""name"": ""id"",
                                                          ""required"": true,
                                                          ""value"": ""aut2xo0xj3mjw79Rd5d7"",
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
                                                  ""relatesTo"": ""$.authenticators.value[3]""
                                                }
                                              ]
                                            },
                                            {
                                              ""name"": ""stateHandle"",
                                              ""required"": true,
                                              ""value"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
                                              ""visible"": false,
                                              ""mutable"": false
                                            }
                                          ],
                                          ""accepts"": ""application/json; okta-version=1.0.0""
                                        },
                                        {
                                          ""rel"": [
                                            ""create-form""
                                          ],
                                          ""name"": ""skip"",
                                          ""href"": ""https://testdomain.com/idp/idx/skip"",
                                          ""method"": ""POST"",
                                          ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                          ""value"": [
                                            {
                                              ""name"": ""stateHandle"",
                                              ""required"": true,
                                              ""value"": ""02WpTMdrSC1bPlQkR9vq65HjRDkhxLD4Gg9GIXPSpa"",
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
                                          ""type"": ""app"",
                                          ""key"": ""google_otp"",
                                          ""id"": ""aut2xo7zveTcXMZWW5d7"",
                                          ""displayName"": ""Google Authenticator"",
                                          ""methods"": [
                                            {
                                              ""type"": ""otp""
                                            }
                                          ]
                                        },
                                        {
                                          ""type"": ""app"",
                                          ""key"": ""okta_verify"",
                                          ""id"": ""aut2xo0xj4LK7Fupb5d7"",
                                          ""displayName"": ""Okta Verify"",
                                          ""methods"": [
                                            {
                                              ""type"": ""push""
                                            },
                                            {
                                              ""type"": ""totp""
                                            }
                                          ]
                                        },
                                        {
                                          ""type"": ""phone"",
                                          ""key"": ""phone_number"",
                                          ""id"": ""aut2xo0xj28i2PjX15d7"",
                                          ""displayName"": ""Phone"",
                                          ""methods"": [
                                            {
                                              ""type"": ""sms""
                                            },
                                            {
                                              ""type"": ""voice""
                                            }
                                          ]
                                        },
                                        {
                                          ""type"": ""security_question"",
                                          ""key"": ""security_question"",
                                          ""id"": ""aut2xo0xj3mjw79Rd5d7"",
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
                                          ""type"": ""security_key"",
                                          ""key"": ""webauthn"",
                                          ""id"": ""fwf3eppyt3VYqlQel5d7"",
                                          ""displayName"": ""Authenticator"",
                                          ""credentialId"": ""foo"",
                                          ""methods"": [
                                            {
                                              ""type"": ""webauthn""
                                            }
                                          ]
                                        },
                                        {
                                          ""type"": ""email"",
                                          ""key"": ""okta_email"",
                                          ""id"": ""eae3epqy1ekfF5rdf5d7"",
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
                                          ""id"": ""lae9skpergPf1afbj5d6"",
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
                                        ""id"": ""00u3epqr99CQeA3mW5d7"",
                                        ""identifier"": ""testuser@test.com"",
                                        ""profile"": {
                                          ""firstName"": ""Laura"",
                                          ""lastName"": ""T"",
                                          ""timeZone"": ""America/Los_Angeles"",
                                          ""locale"": ""en_US""
                                        }
                                      }
                                    }
                                  }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = enrollResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.EnrollAuthenticatorAsync(
                                   new EnrollWebAuthnAuthenticatorOptions
                                   {
                                       Attestation = "foo",
                                       ClientData = "bar",

                                   }, Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingAuthenticatorEnrollment);
            authResponse.Authenticators.FirstOrDefault(x => x.Id == "aut2xoroxctf7gf5i5d7").Should().BeNull();
        }

        [Fact]
        public async Task SelectWebAuthnForChallenge()
        {
            var identifyResponse = @"{
                                        ""version"": ""1.0.0"",
                                        ""stateHandle"": ""02s3_8lt_h2VAymqcwHrxZa7xtfuiUS6fHrMQksX3a"",
                                        ""expiresAt"": ""2021-12-22T18:06:28.000Z"",
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
                                                ""$.currentAuthenticator""
                                              ],
                                              ""href"": ""https://testorg.com/idp/idx/challenge/answer"",
                                              ""method"": ""POST"",
                                              ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                              ""value"": [
                                                {
                                                  ""name"": ""credentials"",
                                                  ""type"": ""object"",
                                                  ""form"": {
                                                    ""value"": [
                                                      {
                                                        ""name"": ""authenticatorData"",
                                                        ""label"": ""Authenticator Data"",
                                                        ""required"": true,
                                                        ""visible"": false
                                                      },
                                                      {
                                                        ""name"": ""clientData"",
                                                        ""label"": ""Client Data"",
                                                        ""required"": true,
                                                        ""visible"": false
                                                      },
                                                      {
                                                        ""name"": ""signatureData"",
                                                        ""label"": ""Signature Data"",
                                                        ""required"": true,
                                                        ""visible"": false
                                                      }
                                                    ]
                                                  },
                                                  ""required"": true
                                                },
                                                {
                                                  ""name"": ""stateHandle"",
                                                  ""required"": true,
                                                  ""value"": ""02s3_8lt_h2VAymqcwHrxZa7xtfuiUS6fHrMQksX3a"",
                                                  ""visible"": false,
                                                  ""mutable"": false
                                                }
                                              ],
                                              ""accepts"": ""application/json; okta-version=1.0.0""
                                            },
                                            {
                                              ""rel"": [
                                                ""create-form""
                                              ],
                                              ""name"": ""select-authenticator-authenticate"",
                                              ""href"": ""https://testorg.com/idp/idx/challenge"",
                                              ""method"": ""POST"",
                                              ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                              ""value"": [
                                                {
                                                  ""name"": ""authenticator"",
                                                  ""type"": ""object"",
                                                  ""options"": [
                                                    {
                                                      ""label"": ""Security Key or Biometric"",
                                                      ""value"": {
                                                        ""form"": {
                                                          ""value"": [
                                                            {
                                                              ""name"": ""id"",
                                                              ""required"": true,
                                                              ""value"": ""aut2xoroxctf7gf5i5d7"",
                                                              ""mutable"": false
                                                            },
                                                            {
                                                              ""name"": ""methodType"",
                                                              ""required"": false,
                                                              ""value"": ""webauthn"",
                                                              ""mutable"": false
                                                            }
                                                          ]
                                                        }
                                                      },
                                                      ""relatesTo"": ""$.authenticators.value[0]""
                                                    }
                                                  ]
                                                },
                                                {
                                                  ""name"": ""stateHandle"",
                                                  ""required"": true,
                                                  ""value"": ""02s3_8lt_h2VAymqcwHrxZa7xtfuiUS6fHrMQksX3a"",
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
                                            ""contextualData"": {
                                              ""challengeData"": {
                                                ""challenge"": ""FTiZR_W3xIsjcGcKy1ujy7lFYtg"",
                                                ""userVerification"": ""preferred"",
                                                ""extensions"": {
                                                  ""appid"": ""https://testorg.com""
                                                }
                                              }
                                            },
                                            ""type"": ""security_key"",
                                            ""key"": ""webauthn"",
                                            ""id"": ""aut2xoroxctf7gf5i5d7"",
                                            ""displayName"": ""Security Key or Biometric"",
                                            ""methods"": [
                                              {
                                                ""type"": ""webauthn""
                                              }
                                            ]
                                          }
                                        },
                                        ""authenticators"": {
                                          ""type"": ""array"",
                                          ""value"": [
                                            {
                                              ""type"": ""security_key"",
                                              ""key"": ""webauthn"",
                                              ""id"": ""aut2xoroxctf7gf5i5d7"",
                                              ""displayName"": ""Security Key or Biometric"",
                                              ""methods"": [
                                                {
                                                  ""type"": ""webauthn""
                                                }
                                              ]
                                            }
                                          ]
                                        },
                                        ""authenticatorEnrollments"": {
                                          ""type"": ""array"",
                                          ""value"": [
                                            {
                                              ""type"": ""security_key"",
                                              ""key"": ""webauthn"",
                                              ""id"": ""fwf3eppyt3VYqlQel5d7"",
                                              ""displayName"": ""Authenticator"",
                                              ""credentialId"": ""EXnmDuGgj22Wb1-oiv5KizXuhzzDhrqp3mrX_3gdu34"",
                                              ""methods"": [
                                                {
                                                  ""type"": ""webauthn""
                                                }
                                              ]
                                            }
                                          ]
                                        },
                                        ""user"": {
                                          ""type"": ""object"",
                                          ""value"": {
                                            ""id"": ""00u3epqr99CQeA3mW5d7"",
                                            ""identifier"": ""foo"",
                                            ""profile"": {
                                              ""firstName"": ""Laura"",
                                              ""lastName"": ""T"",
                                              ""timeZone"": ""America/Los_Angeles"",
                                              ""locale"": ""en_US""
                                            }
                                          }
                                        }
                                      }";
            var selectWebAuthnResponse = @"{
                                          ""version"": ""1.0.0"",
                                          ""stateHandle"": ""02s3_8lt_h2VAymqcwHrxZa7xtfuiUS6fHrMQksX3a"",
                                          ""expiresAt"": ""2021-12-22T18:06:35.000Z"",
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
                                                  ""$.currentAuthenticator""
                                                ],
                                                ""href"": ""https://testorg.com/idp/idx/challenge/answer"",
                                                ""method"": ""POST"",
                                                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                                ""value"": [
                                                  {
                                                    ""name"": ""credentials"",
                                                    ""type"": ""object"",
                                                    ""form"": {
                                                      ""value"": [
                                                        {
                                                          ""name"": ""authenticatorData"",
                                                          ""label"": ""Authenticator Data"",
                                                          ""required"": true,
                                                          ""visible"": false
                                                        },
                                                        {
                                                          ""name"": ""clientData"",
                                                          ""label"": ""Client Data"",
                                                          ""required"": true,
                                                          ""visible"": false
                                                        },
                                                        {
                                                          ""name"": ""signatureData"",
                                                          ""label"": ""Signature Data"",
                                                          ""required"": true,
                                                          ""visible"": false
                                                        }
                                                      ]
                                                    },
                                                    ""required"": true
                                                  },
                                                  {
                                                    ""name"": ""stateHandle"",
                                                    ""required"": true,
                                                    ""value"": ""02s3_8lt_h2VAymqcwHrxZa7xtfuiUS6fHrMQksX3a"",
                                                    ""visible"": false,
                                                    ""mutable"": false
                                                  }
                                                ],
                                                ""accepts"": ""application/json; okta-version=1.0.0""
                                              },
                                              {
                                                ""rel"": [
                                                  ""create-form""
                                                ],
                                                ""name"": ""select-authenticator-authenticate"",
                                                ""href"": ""https://testorg.com/idp/idx/challenge"",
                                                ""method"": ""POST"",
                                                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                                ""value"": [
                                                  {
                                                    ""name"": ""authenticator"",
                                                    ""type"": ""object"",
                                                    ""options"": [
                                                      {
                                                        ""label"": ""Security Key or Biometric"",
                                                        ""value"": {
                                                          ""form"": {
                                                            ""value"": [
                                                              {
                                                                ""name"": ""id"",
                                                                ""required"": true,
                                                                ""value"": ""aut2xoroxctf7gf5i5d7"",
                                                                ""mutable"": false
                                                              },
                                                              {
                                                                ""name"": ""methodType"",
                                                                ""required"": false,
                                                                ""value"": ""webauthn"",
                                                                ""mutable"": false
                                                              }
                                                            ]
                                                          }
                                                        },
                                                        ""relatesTo"": ""$.authenticators.value[0]""
                                                      }
                                                    ]
                                                  },
                                                  {
                                                    ""name"": ""stateHandle"",
                                                    ""required"": true,
                                                    ""value"": ""02s3_8lt_h2VAymqcwHrxZa7xtfuiUS6fHrMQksX3a"",
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
                                              ""contextualData"": {
                                                ""challengeData"": {
                                                  ""challenge"": ""VneRVBrd9y352V66ZyztVmfo6oA"",
                                                  ""userVerification"": ""preferred"",
                                                  ""extensions"": {
                                                    ""appid"": ""https://testorg.com""
                                                  }
                                                }
                                              },
                                              ""type"": ""security_key"",
                                              ""key"": ""webauthn"",
                                              ""id"": ""aut2xoroxctf7gf5i5d7"",
                                              ""displayName"": ""Security Key or Biometric"",
                                              ""methods"": [
                                                {
                                                  ""type"": ""webauthn""
                                                }
                                              ]
                                            }
                                          },
                                          ""authenticators"": {
                                            ""type"": ""array"",
                                            ""value"": [
                                              {
                                                ""type"": ""security_key"",
                                                ""key"": ""webauthn"",
                                                ""id"": ""aut2xoroxctf7gf5i5d7"",
                                                ""displayName"": ""Security Key or Biometric"",
                                                ""methods"": [
                                                  {
                                                    ""type"": ""webauthn""
                                                  }
                                                ]
                                              }
                                            ]
                                          },
                                          ""authenticatorEnrollments"": {
                                            ""type"": ""array"",
                                            ""value"": [
                                              {
                                                ""type"": ""security_key"",
                                                ""key"": ""webauthn"",
                                                ""id"": ""fwf3eppyt3VYqlQel5d7"",
                                                ""displayName"": ""Authenticator"",
                                                ""credentialId"": ""EXnmDuGgj22Wb1-oiv5KizXuhzzDhrqp3mrX_3gdu34"",
                                                ""methods"": [
                                                  {
                                                    ""type"": ""webauthn""
                                                  }
                                                ]
                                              }
                                            ]
                                          }
                                        }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = selectWebAuthnResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.SelectChallengeAuthenticatorAsync(
                                   new SelectAuthenticatorOptions
                                   {
                                       AuthenticatorId = "aut2xoroxctf7gf5i5d7",

                                   }, Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingAuthenticatorVerification);
            authResponse.CurrentAuthenticatorEnrollment.Name.ToLower().Should().Be("security key or biometric");
            authResponse.CurrentAuthenticatorEnrollment.MethodTypes.Should().Contain("webauthn");
            authResponse.CurrentAuthenticatorEnrollment.ContextualData.ChallengeData.Challenge.Should().Be("VneRVBrd9y352V66ZyztVmfo6oA");
            authResponse.CurrentAuthenticatorEnrollment.ContextualData.ChallengeData.UserVerification.Should().Be("preferred");
            authResponse.CurrentAuthenticatorEnrollment.ContextualData.ChallengeData.Extensions.GetProperty<string>("appid")
                .Should().Be("https://testorg.com");
        }

        [Fact]
        public async Task ChallengeWebAuthn()
        {
            var identifyResponse = @"{
                                          ""version"": ""1.0.0"",
                                          ""stateHandle"": ""02s3_8lt_h2VAymqcwHrxZa7xtfuiUS6fHrMQksX3a"",
                                          ""expiresAt"": ""2021-12-22T18:06:35.000Z"",
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
                                                  ""$.currentAuthenticator""
                                                ],
                                                ""href"": ""https://testorg.com/idp/idx/challenge/answer"",
                                                ""method"": ""POST"",
                                                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                                ""value"": [
                                                  {
                                                    ""name"": ""credentials"",
                                                    ""type"": ""object"",
                                                    ""form"": {
                                                      ""value"": [
                                                        {
                                                          ""name"": ""authenticatorData"",
                                                          ""label"": ""Authenticator Data"",
                                                          ""required"": true,
                                                          ""visible"": false
                                                        },
                                                        {
                                                          ""name"": ""clientData"",
                                                          ""label"": ""Client Data"",
                                                          ""required"": true,
                                                          ""visible"": false
                                                        },
                                                        {
                                                          ""name"": ""signatureData"",
                                                          ""label"": ""Signature Data"",
                                                          ""required"": true,
                                                          ""visible"": false
                                                        }
                                                      ]
                                                    },
                                                    ""required"": true
                                                  },
                                                  {
                                                    ""name"": ""stateHandle"",
                                                    ""required"": true,
                                                    ""value"": ""02s3_8lt_h2VAymqcwHrxZa7xtfuiUS6fHrMQksX3a"",
                                                    ""visible"": false,
                                                    ""mutable"": false
                                                  }
                                                ],
                                                ""accepts"": ""application/json; okta-version=1.0.0""
                                              },
                                              {
                                                ""rel"": [
                                                  ""create-form""
                                                ],
                                                ""name"": ""select-authenticator-authenticate"",
                                                ""href"": ""https://testorg.com/idp/idx/challenge"",
                                                ""method"": ""POST"",
                                                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                                                ""value"": [
                                                  {
                                                    ""name"": ""authenticator"",
                                                    ""type"": ""object"",
                                                    ""options"": [
                                                      {
                                                        ""label"": ""Security Key or Biometric"",
                                                        ""value"": {
                                                          ""form"": {
                                                            ""value"": [
                                                              {
                                                                ""name"": ""id"",
                                                                ""required"": true,
                                                                ""value"": ""aut2xoroxctf7gf5i5d7"",
                                                                ""mutable"": false
                                                              },
                                                              {
                                                                ""name"": ""methodType"",
                                                                ""required"": false,
                                                                ""value"": ""webauthn"",
                                                                ""mutable"": false
                                                              }
                                                            ]
                                                          }
                                                        },
                                                        ""relatesTo"": ""$.authenticators.value[0]""
                                                      }
                                                    ]
                                                  },
                                                  {
                                                    ""name"": ""stateHandle"",
                                                    ""required"": true,
                                                    ""value"": ""02s3_8lt_h2VAymqcwHrxZa7xtfuiUS6fHrMQksX3a"",
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
                                              ""contextualData"": {
                                                ""challengeData"": {
                                                  ""challenge"": ""VneRVBrd9y352V66ZyztVmfo6oA"",
                                                  ""userVerification"": ""preferred"",
                                                  ""extensions"": {
                                                    ""appid"": ""https://testorg.com""
                                                  }
                                                }
                                              },
                                              ""type"": ""security_key"",
                                              ""key"": ""webauthn"",
                                              ""id"": ""aut2xoroxctf7gf5i5d7"",
                                              ""displayName"": ""Security Key or Biometric"",
                                              ""methods"": [
                                                {
                                                  ""type"": ""webauthn""
                                                }
                                              ]
                                            }
                                          },
                                          ""authenticators"": {
                                            ""type"": ""array"",
                                            ""value"": [
                                              {
                                                ""type"": ""security_key"",
                                                ""key"": ""webauthn"",
                                                ""id"": ""aut2xoroxctf7gf5i5d7"",
                                                ""displayName"": ""Security Key or Biometric"",
                                                ""methods"": [
                                                  {
                                                    ""type"": ""webauthn""
                                                  }
                                                ]
                                              }
                                            ]
                                          },
                                          ""authenticatorEnrollments"": {
                                            ""type"": ""array"",
                                            ""value"": [
                                              {
                                                ""type"": ""security_key"",
                                                ""key"": ""webauthn"",
                                                ""id"": ""fwf3eppyt3VYqlQel5d7"",
                                                ""displayName"": ""Authenticator"",
                                                ""credentialId"": ""EXnmDuGgj22Wb1-oiv5KizXuhzzDhrqp3mrX_3gdu34"",
                                                ""methods"": [
                                                  {
                                                    ""type"": ""webauthn""
                                                  }
                                                ]
                                              }
                                            ]
                                          }
                                        }";
            var challengeWebAuthnResponse = @"{
                                                  ""version"": ""1.0.0"",
                                                  ""stateHandle"": ""02s3_8lt_h2VAymqcwHrxZa7xtfuiUS6fHrMQksX3a"",
                                                  ""expiresAt"": ""2021-12-22T18:02:38.000Z"",
                                                  ""intent"": ""LOGIN"",
                                                  ""successWithInteractionCode"": {
                                                    ""rel"": [
                                                      ""create-form""
                                                    ],
                                                    ""name"": ""issue"",
                                                    ""href"": ""https://testorg.com/oauth2/aus2xo0xe0oJtPzIS5d7/v1/token"",
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
                                                        ""value"": ""bar""
                                                      },
                                                      {
                                                        ""name"": ""client_id"",
                                                        ""required"": true,
                                                        ""value"": ""foo""
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
                                        ""scope"": ""openid profile"",
                                        ""id_token"": ""test id token""
                                    }";

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = challengeWebAuthnResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = tokenResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.ChallengeAuthenticatorAsync(
                                   new ChallengeWebAuthnAuthenticatorOptions
                                   {
                                       ClientData = "foo",
                                       AuthenticatorData = "bar",
                                       SignatureData = "baz",
                                   }, Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.Success);
            authResponse.TokenInfo.Should().NotBeNull();
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

        [Fact]
        public async Task ResendCodeDuringChallenge()
        {
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
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = challengeWithMethodResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = challengeWithMethodResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.ResendCodeAsync(Substitute.For<IIdxContext>());

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingAuthenticatorVerification);
            authResponse.CurrentAuthenticatorEnrollment.Should().NotBeNull();
            authResponse.TokenInfo.Should().BeNull();
        }

        /****** END SCENARIOS 6.2.X ******/

        /****** BEGIN SCENARIOS 10.2.X WebAuthN ******/
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


        /****** END SCENARIOS 10.2.X WebAuthN ******/

        #endregion

        #region Self Service Registration
        [Fact]
        public async Task RegisterProfile()
        {
            #region mockResponses
            string interactResponse = @"{
    ""interaction_handle"": ""HtXTMBoVlBzhIAzDM-GNGkddbYzTCrt1NmEyCUhdzY4""
}";
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
    ""expiresAt"": ""2021-06-02T16:32:19.000Z"",
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
                        ""value"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-enroll-profile"",
                ""href"": ""https://fake.example.com/idp/idx/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""unlock-account"",
                ""href"": ""https://fake.example.com/idp/idx/unlock-account"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""name"": ""redirect-idp"",
                ""type"": ""FACEBOOK"",
                ""idp"": {
                    ""id"": ""0oau09xo6XAbbPQSN5d6"",
                    ""name"": ""Facebook IdP""
                },
                ""href"": ""https://fake.example.com/oauth2/austyqkbjaFoOxkl45d6/v1/authorize?client_id=xxxxxxxx&request_uri=urn:okta:T01kUzl0SXpTNGhZWEt2ZTUwNmdhWUxTVEl1bTZoMHlkRUtvY2VKZzN6STowb2F1MDl4bzZYQWJiUFFTTjVkNg"",
                ""method"": ""GET""
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
                        ""value"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
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
                ""value"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";
            string enrollResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
    ""expiresAt"": ""2021-06-02T16:32:21.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""enroll-profile"",
                ""href"": ""https://fake.example.com/idp/idx/enroll/new"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""userProfile"",
                        ""form"": {
                            ""value"": [
                                {
                                    ""name"": ""firstName"",
                                    ""label"": ""First name"",
                                    ""required"": true,
                                    ""minLength"": 1,
                                    ""maxLength"": 50
                                },
                                {
                                    ""name"": ""lastName"",
                                    ""label"": ""Last name"",
                                    ""required"": true,
                                    ""minLength"": 1,
                                    ""maxLength"": 50
                                },
                                {
                                    ""name"": ""email"",
                                    ""label"": ""Email"",
                                    ""required"": true
                                }
                            ]
                        }
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-identify"",
                ""href"": ""https://fake.example.com/idp/idx/identify/select"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
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
                ""value"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";
            string enrollNewResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
    ""expiresAt"": ""2021-06-02T14:37:22.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""type"": ""object"",
                        ""options"": [
                            {
                                ""label"": ""Password"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi2fKQlZVl15d6"",
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
                                ""relatesTo"": ""$.authenticators.value[0]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
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
        ""value"": []
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uuxsen4trGtQH7J5d6""
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
                ""value"": ""02To44MY4BUY8oq7zYVpOnsJfQ42bxS0b6wn488n5W"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";
            #endregion

            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/oauth2/v1/interact", interactResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/enroll", enrollResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/enroll/new", enrollNewResponse);

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);
            UserProfile userProfile = new UserProfile();
            userProfile.SetProperty("firstName", "test-firstname");
            userProfile.SetProperty("lastName", "test-lastname");
            userProfile.SetProperty("email", "test-email@fake.com");
            AuthenticationResponse authenticationResponse = await idxClient.RegisterAsync(userProfile);
            Assert.NotNull(authenticationResponse);
            Assert.Equal(AuthenticationStatus.AwaitingAuthenticatorEnrollment, authenticationResponse.AuthenticationStatus);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/oauth2/v1/interact"]);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/idp/idx/introspect"]);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/idp/idx/enroll"]);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/idp/idx/enroll/new"]);
        }

        [Fact]
        public async Task EnrollSmsAuthenticator()
        {
            #region mockResponses
            string introspectResponse = @"{
  ""stateHandle"": ""02StDJyxkiODjhdJ1AzxzR6lL-pMdFivLa9NeAPrjr"",
  ""version"": ""1.0.0"",
  ""expiresAt"": ""2020-12-21T20:47:58.000Z"",
  ""intent"": ""LOGIN"",
  ""remediation"": {
    ""type"": ""array"",
    ""value"": [
      {
        ""rel"": [
          ""create-form""
        ],
        ""name"": ""select-authenticator-enroll"",
        ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
        ""method"": ""POST"",
        ""value"": [
          {
            ""name"": ""authenticator"",
            ""type"": ""object"",
            ""options"": [
              {
                ""label"": ""Security Question"",
                ""value"": {
                  ""form"": {
                    ""value"": [
                      {
                        ""name"": ""id"",
                        ""required"": true,
                        ""value"": ""autzvyil7o5nQqC5j2o4"",
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
                ""relatesTo"": ""$.authenticators.value[0]""
              }
            ]
          },
          {
            ""name"": ""stateHandle"",
            ""required"": true,
            ""value"": ""02StDJyxkiODjhdJ1AzxzR6lL-pMdFivLa9NeAPrjr"",
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
        ""type"": ""security_question"",
        ""id"": ""autzvyil7o5nQqC5j2o4"",
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
        ""type"": ""email"",
        ""id"": ""eae10nuqZasIQ4wg22o5"",
        ""displayName"": ""Email"",
        ""methods"": [
          {
            ""type"": ""email""
          }
        ]
      },
      {
        ""type"": ""password"",
        ""id"": ""lae61ok7g2CBfzUcs2o4"",
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
      ""id"": ""00u10nupD8hbHWY3G2o5""
    }
  },
  ""cancel"": {
    ""rel"": [
      ""create-form""
    ],
    ""name"": ""cancel"",
    ""href"": ""https://fake.example.com/idp/idx/cancel"",
    ""method"": ""POST"",
    ""value"": [
      {
        ""name"": ""stateHandle"",
        ""required"": true,
        ""value"": ""02StDJyxkiODjhdJ1AzxzR6lL-pMdFivLa9NeAPrjr"",
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
      ""label"": ""Okta Dashboard"",
      ""id"": ""DEFAULT_APP""
    }
  }
}";
            string enrollResponse = @"{
  ""version"": ""1.0.0"",
  ""stateHandle"": ""029ZAB"",
  ""expiresAt"": ""2021-05-21T14:46:56.000Z"",
  ""intent"": ""LOGIN"",
  ""remediation"": {
    ""type"": ""array"",
    ""value"": [
      {
        ""rel"": [
          ""create-form""
        ],
        ""name"": ""enroll-authenticator"",
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
                  ""label"": ""Enter code""
                }
              ]
            },
            ""required"": true
          },
          {
            ""name"": ""stateHandle"",
            ""required"": true,
            ""value"": ""029ZAB"",
            ""visible"": false,
            ""mutable"": false
          }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
      },
      {
        ""rel"": [
          ""create-form""
        ],
        ""name"": ""select-authenticator-enroll"",
        ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
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
                        ""value"": ""autkx2thaMq4XkX2I5d6"",
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
                ""relatesTo"": ""$.authenticators.value[0]""
              },
              {
                ""label"": ""Phone"",
                ""value"": {
                  ""form"": {
                    ""value"": [
                      {
                        ""name"": ""id"",
                        ""required"": true,
                        ""value"": ""autkx2thbuHB4hZa75d6"",
                        ""mutable"": false
                      },
                      {
                        ""name"": ""methodType"",
                        ""type"": ""string"",
                        ""required"": false,
                        ""options"": [
                          {
                            ""label"": ""SMS"",
                            ""value"": ""sms""
                          },
                          {
                            ""label"": ""Voice call"",
                            ""value"": ""voice""
                          }
                        ]
                      },
                      {
                        ""name"": ""phoneNumber"",
                        ""label"": ""Phone number"",
                        ""required"": false
                      }
                    ]
                  }
                },
                ""relatesTo"": ""$.authenticators.value[1]""
              }
            ]
          },
          {
            ""name"": ""stateHandle"",
            ""required"": true,
            ""value"": ""029ZAB"",
            ""visible"": false,
            ""mutable"": false
          }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
      },
      {
        ""rel"": [
          ""create-form""
        ],
        ""name"": ""skip"",
        ""href"": ""https://fake.example.com/idp/idx/skip"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
          {
            ""name"": ""stateHandle"",
            ""required"": true,
            ""value"": ""029ZAB"",
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
            ""value"": ""029ZAB"",
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
            ""value"": ""029ZAB"",
            ""visible"": false,
            ""mutable"": false
          }
        ],
        ""accepts"": ""application/json; okta-version=1.0.0""
      },
      ""type"": ""email"",
      ""key"": ""okta_email"",
      ""id"": ""autkx2thaMq4XkX2I5d6"",
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
        ""id"": ""autkx2thaMq4XkX2I5d6"",
        ""displayName"": ""Email"",
        ""methods"": [
          {
            ""type"": ""email""
          }
        ]
      },
      {
        ""type"": ""phone"",
        ""key"": ""phone_number"",
        ""id"": ""autkx2thbuHB4hZa75d6"",
        ""displayName"": ""Phone"",
        ""methods"": [
          {
            ""type"": ""sms""
          },
          {
            ""type"": ""voice""
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
        ""id"": ""lae1t6lhjUlaXaKOG5d6"",
        ""displayName"": ""Password"",
        ""methods"": [
          {
            ""type"": ""password""
          }
        ]
      }
    ]
  },
  ""enrollmentAuthenticator"": {
    ""type"": ""object"",
    ""value"": {
      ""type"": ""email"",
      ""key"": ""okta_email"",
      ""id"": ""autkx2thaMq4XkX2I5d6"",
      ""displayName"": ""Email"",
      ""methods"": [
        {
          ""type"": ""email""
        }
      ]
    }
  },
  ""user"": {
    ""type"": ""object"",
    ""value"": {
      ""id"": ""00usib7f14C7d2nyY5d6""
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
        ""value"": ""029ZAB"",
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
      ""label"": "".NET Unit Test"",
      ""id"": ""xxxxxxxx""
    }
  }
}";
            #endregion;
            IIdxContext idxContext = new IdxContext("test code verifier", "test code challenge", "test code challenge method", "test interaction handle", "test state");
            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/credential/enroll", enrollResponse);

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);
            EnrollPhoneAuthenticatorOptions enrollPhoneAuthenticatorOptions = new EnrollPhoneAuthenticatorOptions
            {
                AuthenticatorId = "testAuthenticatorId",
                PhoneNumber = "2065551212",
                MethodType = AuthenticatorMethodType.Sms
            };
            AuthenticationResponse authenticationResponse = await idxClient.EnrollAuthenticatorAsync(enrollPhoneAuthenticatorOptions, idxContext);
            Assert.NotNull(authenticationResponse);
            Assert.Equal(AuthenticationStatus.AwaitingAuthenticatorVerification, authenticationResponse.AuthenticationStatus);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/idp/idx/introspect"]);
            Assert.Equal(1, mockHttpMessageHandler.CallCounts["/idp/idx/credential/enroll"]);
        }


        [Fact]
        public async Task ThrowForBadPhoneAuthenticator()
        {
            #region mockResponses
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02u302Css93ifIFDrXNJQaCJjRSHLmCVaJ41L2jMOZ"",
    ""expiresAt"": ""2021-06-02T17:02:14.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""authenticator-enrollment-data"",
                ""relatesTo"": [
                    ""$.currentAuthenticator""
                ],
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""label"": ""Phone"",
                        ""form"": {
                            ""value"": [
                                {
                                    ""name"": ""id"",
                                    ""required"": true,
                                    ""value"": ""auttzfsi4eiZIdLK85d6"",
                                    ""mutable"": false
                                },
                                {
                                    ""name"": ""methodType"",
                                    ""type"": ""string"",
                                    ""required"": true,
                                    ""options"": [
                                        {
                                            ""label"": ""SMS"",
                                            ""value"": ""sms""
                                        }
                                    ]
                                },
                                {
                                    ""name"": ""phoneNumber"",
                                    ""required"": true
                                }
                            ]
                        }
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02u302Css93ifIFDrXNJQaCJjRSHLmCVaJ41L2jMOZ"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
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
                                ""relatesTo"": ""$.authenticators.value[0]""
                            },
                            {
                                ""label"": ""Phone"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi4eiZIdLK85d6"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""SMS"",
                                                        ""value"": ""sms""
                                                    }
                                                ]
                                            },
                                            {
                                                ""name"": ""phoneNumber"",
                                                ""label"": ""Phone number"",
                                                ""required"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[1]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02u302Css93ifIFDrXNJQaCJjRSHLmCVaJ41L2jMOZ"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""skip"",
                ""href"": ""https://fake.example.com/idp/idx/skip"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02u302Css93ifIFDrXNJQaCJjRSHLmCVaJ41L2jMOZ"",
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
                        ""value"": ""02u302Css93ifIFDrXNJQaCJjRSHLmCVaJ41L2jMOZ"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""type"": ""phone"",
            ""key"": ""phone_number"",
            ""id"": ""auttzfsi4eiZIdLK85d6"",
            ""displayName"": ""Phone"",
            ""methods"": [
                {
                    ""type"": ""sms""
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
            },
            {
                ""type"": ""phone"",
                ""key"": ""phone_number"",
                ""id"": ""auttzfsi4eiZIdLK85d6"",
                ""displayName"": ""Phone"",
                ""methods"": [
                    {
                        ""type"": ""sms""
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
                ""id"": ""lae1y7cy0SnqvVzjy5d6"",
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
            ""id"": ""00uuyydydG04jXoEA5d6""
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
                ""value"": ""02u302Css93ifIFDrXNJQaCJjRSHLmCVaJ41L2jMOZ"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";
            string enrollResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02u302Css93ifIFDrXNJQaCJjRSHLmCVaJ41L2jMOZ"",
    ""expiresAt"": ""2021-06-02T17:02:14.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
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
                                ""relatesTo"": ""$.authenticators.value[0]""
                            },
                            {
                                ""label"": ""Phone"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi4eiZIdLK85d6"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""SMS"",
                                                        ""value"": ""sms""
                                                    }
                                                ]
                                            },
                                            {
                                                ""name"": ""phoneNumber"",
                                                ""label"": ""Phone number"",
                                                ""required"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[1]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02u302Css93ifIFDrXNJQaCJjRSHLmCVaJ41L2jMOZ"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""skip"",
                ""href"": ""https://fake.example.com/idp/idx/skip"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02u302Css93ifIFDrXNJQaCJjRSHLmCVaJ41L2jMOZ"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            }
        ]
    },
    ""messages"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""message"": ""Unable to initiate factor enrollment: Invalid Phone Number."",
                ""class"": ""ERROR""
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
            },
            {
                ""type"": ""phone"",
                ""key"": ""phone_number"",
                ""id"": ""auttzfsi4eiZIdLK85d6"",
                ""displayName"": ""Phone"",
                ""methods"": [
                    {
                        ""type"": ""sms""
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
                ""id"": ""lae1y7cy0SnqvVzjy5d6"",
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
            ""id"": ""00uuyydydG04jXoEA5d6""
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
                ""value"": ""02u302Css93ifIFDrXNJQaCJjRSHLmCVaJ41L2jMOZ"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";
            #endregion;
            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 400, Response = enrollResponse });
            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);
            EnrollPhoneAuthenticatorOptions enrollPhoneAuthenticatorOptions = new EnrollPhoneAuthenticatorOptions
            {
                AuthenticatorId = "testAuthenticatorId",
                PhoneNumber = "12345678901",
                MethodType = AuthenticatorMethodType.Sms
            };
            Func<Task<AuthenticationResponse>> function = async () => await testClient.EnrollAuthenticatorAsync(
                                                                          enrollPhoneAuthenticatorOptions, Substitute.For<IIdxContext>());
            await function.Should()
                .ThrowAsync<OktaException>()
                .WithMessage("*Unable to initiate factor enrollment: Invalid Phone Number.*");
        }

        [Fact]
        public async Task SelectEmailForVerificationDuringRegistration()
        {
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02KL6zedhb1AUAmB6JAjScM8tFiROMJfBcYGRkLcBu"",
    ""expiresAt"": ""2021-06-03T12:24:24.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
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
                                ""relatesTo"": ""$.authenticators.value[0]""
                            },
                            {
                                ""label"": ""Phone"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi4eiZIdLK85d6"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""SMS"",
                                                        ""value"": ""sms""
                                                    }
                                                ]
                                            },
                                            {
                                                ""name"": ""phoneNumber"",
                                                ""label"": ""Phone number"",
                                                ""required"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[1]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02KL6zedhb1AUAmB6JAjScM8tFiROMJfBcYGRkLcBu"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""skip"",
                ""href"": ""https://fake.example.com/idp/idx/skip"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02KL6zedhb1AUAmB6JAjScM8tFiROMJfBcYGRkLcBu"",
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
            },
            {
                ""type"": ""phone"",
                ""key"": ""phone_number"",
                ""id"": ""auttzfsi4eiZIdLK85d6"",
                ""displayName"": ""Phone"",
                ""methods"": [
                    {
                        ""type"": ""sms""
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
                ""id"": ""lae1ykuhoe8XDrQX45d6"",
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
            ""id"": ""00uv3xmmrqyA1klYo5d6""
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
                ""value"": ""02KL6zedhb1AUAmB6JAjScM8tFiROMJfBcYGRkLcBu"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";
            string credentialEnrollResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02KL6zedhb1AUAmB6JAjScM8tFiROMJfBcYGRkLcBu"",
    ""expiresAt"": ""2021-06-03T12:25:52.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""enroll-authenticator"",
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
                                    ""label"": ""Enter code""
                                }
                            ]
                        },
                        ""required"": true
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02KL6zedhb1AUAmB6JAjScM8tFiROMJfBcYGRkLcBu"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
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
                                ""relatesTo"": ""$.authenticators.value[0]""
                            },
                            {
                                ""label"": ""Phone"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi4eiZIdLK85d6"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""SMS"",
                                                        ""value"": ""sms""
                                                    }
                                                ]
                                            },
                                            {
                                                ""name"": ""phoneNumber"",
                                                ""label"": ""Phone number"",
                                                ""required"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[1]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02KL6zedhb1AUAmB6JAjScM8tFiROMJfBcYGRkLcBu"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""skip"",
                ""href"": ""https://fake.example.com/idp/idx/skip"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02KL6zedhb1AUAmB6JAjScM8tFiROMJfBcYGRkLcBu"",
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
                        ""value"": ""02KL6zedhb1AUAmB6JAjScM8tFiROMJfBcYGRkLcBu"",
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
                        ""value"": ""02KL6zedhb1AUAmB6JAjScM8tFiROMJfBcYGRkLcBu"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
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
            },
            {
                ""type"": ""phone"",
                ""key"": ""phone_number"",
                ""id"": ""auttzfsi4eiZIdLK85d6"",
                ""displayName"": ""Phone"",
                ""methods"": [
                    {
                        ""type"": ""sms""
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
                ""id"": ""lae1ykuhoe8XDrQX45d6"",
                ""displayName"": ""Password"",
                ""methods"": [
                    {
                        ""type"": ""password""
                    }
                ]
            }
        ]
    },
    ""enrollmentAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
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
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uv3xmmrqyA1klYo5d6""
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
                ""value"": ""02KL6zedhb1AUAmB6JAjScM8tFiROMJfBcYGRkLcBu"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";

            IIdxContext idxContext = new IdxContext("test code verifier", "test code challenge", "test code challenge method", "test interaction handle", "test state");
            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/credential/enroll", credentialEnrollResponse);

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);
            SelectEnrollAuthenticatorOptions enrollAuthenticatorOptions = new SelectEnrollAuthenticatorOptions
            {
                AuthenticatorId = "auttzfsi3IuMuCpwD5d6",
            };

            AuthenticationResponse enrollResponse = await idxClient.SelectEnrollAuthenticatorAsync(enrollAuthenticatorOptions, idxContext);
            Assert.Equal(AuthenticationStatus.AwaitingAuthenticatorVerification, enrollResponse.AuthenticationStatus);
        }

        [Fact]
        public async Task VerifyEmailDuringRegistration()
        {
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02vbou8JYYRDMXETuPnUW4fOuW8fBvX4YiafnbnoeT"",
    ""expiresAt"": ""2021-06-03T12:42:33.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""enroll-authenticator"",
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
                                    ""label"": ""Enter code""
                                }
                            ]
                        },
                        ""required"": true
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02vbou8JYYRDMXETuPnUW4fOuW8fBvX4YiafnbnoeT"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
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
                                ""relatesTo"": ""$.authenticators.value[0]""
                            },
                            {
                                ""label"": ""Phone"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi4eiZIdLK85d6"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""SMS"",
                                                        ""value"": ""sms""
                                                    }
                                                ]
                                            },
                                            {
                                                ""name"": ""phoneNumber"",
                                                ""label"": ""Phone number"",
                                                ""required"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[1]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02vbou8JYYRDMXETuPnUW4fOuW8fBvX4YiafnbnoeT"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""skip"",
                ""href"": ""https://fake.example.com/idp/idx/skip"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02vbou8JYYRDMXETuPnUW4fOuW8fBvX4YiafnbnoeT"",
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
                        ""value"": ""02vbou8JYYRDMXETuPnUW4fOuW8fBvX4YiafnbnoeT"",
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
                ""refresh"": 6837,
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02vbou8JYYRDMXETuPnUW4fOuW8fBvX4YiafnbnoeT"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
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
            },
            {
                ""type"": ""phone"",
                ""key"": ""phone_number"",
                ""id"": ""auttzfsi4eiZIdLK85d6"",
                ""displayName"": ""Phone"",
                ""methods"": [
                    {
                        ""type"": ""sms""
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
                ""id"": ""lae1yl6u3frBCiWj15d6"",
                ""displayName"": ""Password"",
                ""methods"": [
                    {
                        ""type"": ""password""
                    }
                ]
            }
        ]
    },
    ""enrollmentAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
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
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uv41kb4oKs6TIge5d6""
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
                ""value"": ""02vbou8JYYRDMXETuPnUW4fOuW8fBvX4YiafnbnoeT"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";
            string challengeAnswerResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02vbou8JYYRDMXETuPnUW4fOuW8fBvX4YiafnbnoeT"",
    ""expiresAt"": ""2021-06-03T12:43:36.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""type"": ""object"",
                        ""options"": [
                            {
                                ""label"": ""Phone"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi4eiZIdLK85d6"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""SMS"",
                                                        ""value"": ""sms""
                                                    }
                                                ]
                                            },
                                            {
                                                ""name"": ""phoneNumber"",
                                                ""label"": ""Phone number"",
                                                ""required"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[0]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02vbou8JYYRDMXETuPnUW4fOuW8fBvX4YiafnbnoeT"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""skip"",
                ""href"": ""https://fake.example.com/idp/idx/skip"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02vbou8JYYRDMXETuPnUW4fOuW8fBvX4YiafnbnoeT"",
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
                ""type"": ""phone"",
                ""key"": ""phone_number"",
                ""id"": ""auttzfsi4eiZIdLK85d6"",
                ""displayName"": ""Phone"",
                ""methods"": [
                    {
                        ""type"": ""sms""
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
                ""id"": ""eaev42164dMcEwycp5d6"",
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
                ""id"": ""lae1yl6u3frBCiWj15d6"",
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
            ""id"": ""00uv41kb4oKs6TIge5d6""
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
                ""value"": ""02vbou8JYYRDMXETuPnUW4fOuW8fBvX4YiafnbnoeT"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";

            IIdxContext idxContext = new IdxContext("test code verifier", "test code challenge", "test code challenge method", "test interaction handle", "test state");
            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/challenge/answer", challengeAnswerResponse);

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);
            VerifyAuthenticatorOptions verifyAuthenticatorOptions = new VerifyAuthenticatorOptions
            {
                Code = "677088",
            };

            AuthenticationResponse verifyResponse = await idxClient.VerifyAuthenticatorAsync(verifyAuthenticatorOptions, idxContext);
            Assert.Equal(AuthenticationStatus.AwaitingAuthenticatorEnrollment, verifyResponse.AuthenticationStatus);
        }

        [Fact]
        public async Task SkipPhoneEnrollmentDuringRegistration()
        {
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02gHYti2kUo5hzKk0l3oxEYi7tQMC53s-TUNTAWO77"",
    ""expiresAt"": ""2021-06-03T14:47:04.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""type"": ""object"",
                        ""options"": [
                            {
                                ""label"": ""Phone"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi4eiZIdLK85d6"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""SMS"",
                                                        ""value"": ""sms""
                                                    }
                                                ]
                                            },
                                            {
                                                ""name"": ""phoneNumber"",
                                                ""label"": ""Phone number"",
                                                ""required"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[0]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02gHYti2kUo5hzKk0l3oxEYi7tQMC53s-TUNTAWO77"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""skip"",
                ""href"": ""https://fake.example.com/idp/idx/skip"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02gHYti2kUo5hzKk0l3oxEYi7tQMC53s-TUNTAWO77"",
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
                ""type"": ""phone"",
                ""key"": ""phone_number"",
                ""id"": ""auttzfsi4eiZIdLK85d6"",
                ""displayName"": ""Phone"",
                ""methods"": [
                    {
                        ""type"": ""sms""
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
                ""id"": ""eaev4o6klA0SU332F5d6"",
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
                ""id"": ""lae1yni5iBvlTG03L5d6"",
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
            ""id"": ""00uv4ntd4kZAY10TF5d6""
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
                ""value"": ""02gHYti2kUo5hzKk0l3oxEYi7tQMC53s-TUNTAWO77"",
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
            string skipResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02gHYti2kUo5hzKk0l3oxEYi7tQMC53s-TUNTAWO77"",
    ""expiresAt"": ""2021-06-03T14:43:58.000Z"",
    ""intent"": ""LOGIN"",
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00uv4ntd4kZAY10TF5d6""
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
                ""value"": ""02gHYti2kUo5hzKk0l3oxEYi7tQMC53s-TUNTAWO77"",
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
                ""value"": ""CI69-b_BZBsEXRs7qfLl5zqwXQMI5Ju-BYziDNkME28""
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
    ""scope"": ""openid profile"",
    ""id_token"": ""test id token""
}";

            IIdxContext idxContext = new IdxContext("test code verifier", "test code challenge", "test code challenge method", "test interaction handle", "test state");
            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/skip", skipResponse);
            mockHttpMessageHandler.AddTestResponse("/oauth2/austyqkbjaFoOxkl45d6/v1/token", tokenResponse);
            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);
            AuthenticationResponse enrollResponse = await idxClient.SkipAuthenticatorSelectionAsync(idxContext);
            Assert.Equal(AuthenticationStatus.Success, enrollResponse.AuthenticationStatus);
            Assert.NotNull(enrollResponse.TokenInfo);
            Assert.Equal("test access token", enrollResponse.TokenInfo.AccessToken);
            Assert.Equal("test id token", enrollResponse.TokenInfo.IdToken);
        }

        [Fact]
        public async Task SelectPhoneEnrollmentAfterEmailVerficationDuringRegistration()
        {
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""025igIyJgsxf9OnM-U47KTA7GuSJqupEiQYGUnt-Yw"",
    ""expiresAt"": ""2021-06-03T15:06:29.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""type"": ""object"",
                        ""options"": [
                            {
                                ""label"": ""Phone"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi4eiZIdLK85d6"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""SMS"",
                                                        ""value"": ""sms""
                                                    }
                                                ]
                                            },
                                            {
                                                ""name"": ""phoneNumber"",
                                                ""label"": ""Phone number"",
                                                ""required"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[0]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""025igIyJgsxf9OnM-U47KTA7GuSJqupEiQYGUnt-Yw"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""skip"",
                ""href"": ""https://fake.example.com/idp/idx/skip"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""025igIyJgsxf9OnM-U47KTA7GuSJqupEiQYGUnt-Yw"",
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
                ""type"": ""phone"",
                ""key"": ""phone_number"",
                ""id"": ""auttzfsi4eiZIdLK85d6"",
                ""displayName"": ""Phone"",
                ""methods"": [
                    {
                        ""type"": ""sms""
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
                ""id"": ""eaev4tju8wZBuXHK05d6"",
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
                ""id"": ""lae1yogd5BjwVVnLu5d6"",
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
            ""id"": ""00uv4robkjiLYqVk15d6""
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
                ""value"": ""025igIyJgsxf9OnM-U47KTA7GuSJqupEiQYGUnt-Yw"",
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
            string credentialEnrollResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""025igIyJgsxf9OnM-U47KTA7GuSJqupEiQYGUnt-Yw"",
    ""expiresAt"": ""2021-06-03T15:06:59.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""authenticator-enrollment-data"",
                ""relatesTo"": [
                    ""$.currentAuthenticator""
                ],
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""label"": ""Phone"",
                        ""form"": {
                            ""value"": [
                                {
                                    ""name"": ""id"",
                                    ""required"": true,
                                    ""value"": ""auttzfsi4eiZIdLK85d6"",
                                    ""mutable"": false
                                },
                                {
                                    ""name"": ""methodType"",
                                    ""type"": ""string"",
                                    ""required"": true,
                                    ""options"": [
                                        {
                                            ""label"": ""SMS"",
                                            ""value"": ""sms""
                                        }
                                    ]
                                },
                                {
                                    ""name"": ""phoneNumber"",
                                    ""required"": true
                                }
                            ]
                        }
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""025igIyJgsxf9OnM-U47KTA7GuSJqupEiQYGUnt-Yw"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://fake.example.com/idp/idx/credential/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""type"": ""object"",
                        ""options"": [
                            {
                                ""label"": ""Phone"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""auttzfsi4eiZIdLK85d6"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""SMS"",
                                                        ""value"": ""sms""
                                                    }
                                                ]
                                            },
                                            {
                                                ""name"": ""phoneNumber"",
                                                ""label"": ""Phone number"",
                                                ""required"": false
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[0]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""025igIyJgsxf9OnM-U47KTA7GuSJqupEiQYGUnt-Yw"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""skip"",
                ""href"": ""https://fake.example.com/idp/idx/skip"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""025igIyJgsxf9OnM-U47KTA7GuSJqupEiQYGUnt-Yw"",
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
                        ""value"": ""025igIyJgsxf9OnM-U47KTA7GuSJqupEiQYGUnt-Yw"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""type"": ""phone"",
            ""key"": ""phone_number"",
            ""id"": ""auttzfsi4eiZIdLK85d6"",
            ""displayName"": ""Phone"",
            ""methods"": [
                {
                    ""type"": ""sms""
                }
            ]
        }
    },
    ""authenticators"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""phone"",
                ""key"": ""phone_number"",
                ""id"": ""auttzfsi4eiZIdLK85d6"",
                ""displayName"": ""Phone"",
                ""methods"": [
                    {
                        ""type"": ""sms""
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
                ""id"": ""eaev4tju8wZBuXHK05d6"",
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
                ""id"": ""lae1yogd5BjwVVnLu5d6"",
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
            ""id"": ""00uv4robkjiLYqVk15d6""
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
                ""value"": ""025igIyJgsxf9OnM-U47KTA7GuSJqupEiQYGUnt-Yw"",
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

            IIdxContext idxContext = new IdxContext("test code verifier", "test code challenge", "test code challenge method", "test interaction handle", "test state");
            MockHttpMessageHandler mockHttpMessageHandler = new MockHttpMessageHandler();
            mockHttpMessageHandler.AddTestResponse("/idp/idx/introspect", introspectResponse);
            mockHttpMessageHandler.AddTestResponse("/idp/idx/credential/enroll", credentialEnrollResponse);

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler);

            IdxClient idxClient = new IdxClient(TesteableIdxClient.DefaultFakeConfiguration, httpClient, NullLogger.Instance);

            SelectEnrollAuthenticatorOptions enrollAuthenticatorOptions = new SelectEnrollAuthenticatorOptions
            {
                AuthenticatorId = "auttzfsi4eiZIdLK85d6",
            };

            AuthenticationResponse enrollResponse = await idxClient.SelectEnrollAuthenticatorAsync(enrollAuthenticatorOptions, idxContext);

            Assert.Equal(AuthenticationStatus.AwaitingAuthenticatorEnrollmentData, enrollResponse.AuthenticationStatus);
        }


        [Fact]
        public async Task ThrowForInvalidEmailRegistration()
        {
            #region mockResponses
            string interactResponse = @"{
    ""interaction_handle"": ""HtXTMBoVlBzhIAzDM-GNGkddbYzTCrt1NmEyCUhdzY4""
}";
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
    ""expiresAt"": ""2021-06-02T17:53:05.000Z"",
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
                        ""value"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-enroll-profile"",
                ""href"": ""https://fake.example.com/idp/idx/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""unlock-account"",
                ""href"": ""https://fake.example.com/idp/idx/unlock-account"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""name"": ""redirect-idp"",
                ""type"": ""FACEBOOK"",
                ""idp"": {
                    ""id"": ""0oau09xo6XAbbPQSN5d6"",
                    ""name"": ""Facebook IdP""
                },
                ""href"": ""https://fake.example.com/oauth2/austyqkbjaFoOxkl45d6/v1/authorize?client_id=xxxxxxxx&request_uri=urn:okta:Ykx2aVlnWk1xc0xYcDlUVXdVbzJ6UjhlRS1uNDd4TVhpQVQ4dU9yNWc3UTowb2F1MDl4bzZYQWJiUFFTTjVkNg"",
                ""method"": ""GET""
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
                        ""value"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
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
                ""value"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";
            string enrollResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
    ""expiresAt"": ""2021-06-02T17:53:06.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""enroll-profile"",
                ""href"": ""https://fake.example.com/idp/idx/enroll/new"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""userProfile"",
                        ""form"": {
                            ""value"": [
                                {
                                    ""name"": ""firstName"",
                                    ""label"": ""First name"",
                                    ""required"": true,
                                    ""minLength"": 1,
                                    ""maxLength"": 50
                                },
                                {
                                    ""name"": ""lastName"",
                                    ""label"": ""Last name"",
                                    ""required"": true,
                                    ""minLength"": 1,
                                    ""maxLength"": 50
                                },
                                {
                                    ""name"": ""email"",
                                    ""label"": ""Email"",
                                    ""required"": true
                                }
                            ]
                        }
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-identify"",
                ""href"": ""https://fake.example.com/idp/idx/identify/select"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
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
                ""value"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";
            string enrollNewResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
    ""expiresAt"": ""2021-06-02T17:53:06.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""enroll-profile"",
                ""href"": ""https://fake.example.com/idp/idx/enroll/new"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""userProfile"",
                        ""form"": {
                            ""value"": [
                                {
                                    ""name"": ""firstName"",
                                    ""label"": ""First name"",
                                    ""required"": true,
                                    ""minLength"": 1,
                                    ""maxLength"": 50
                                },
                                {
                                    ""name"": ""lastName"",
                                    ""label"": ""Last name"",
                                    ""required"": true,
                                    ""minLength"": 1,
                                    ""maxLength"": 50
                                },
                                {
                                    ""name"": ""email"",
                                    ""label"": ""Email"",
                                    ""required"": true,
                                    ""messages"": {
                                        ""type"": ""array"",
                                        ""value"": [
                                            {
                                                ""message"": ""'Email' must be in the form of an email address"",
                                                ""i18n"": {
                                                    ""key"": ""registration.error.invalidLoginEmail"",
                                                    ""params"": [
                                                        ""Email""
                                                    ]
                                                },
                                                ""class"": ""ERROR""
                                            },
                                            {
                                                ""message"": ""Provided value for property 'Email' does not match required pattern"",
                                                ""i18n"": {
                                                    ""key"": ""registration.error.doesNotMatchPattern"",
                                                    ""params"": [
                                                        ""Email""
                                                    ]
                                                },
                                                ""class"": ""ERROR""
                                            }
                                        ]
                                    }
                                }
                            ]
                        }
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-identify"",
                ""href"": ""https://fake.example.com/idp/idx/identify/select"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
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
                ""value"": ""02n8peIw_A9iBksPvgMSFrY7_nzepDvd8OHG_1jecT"",
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
            ""id"": ""xxxxxxxx""
        }
    }
}";
            #endregion
            UserProfile userProfile = new UserProfile();
            userProfile.SetProperty("firstName", "test-firstname");
            userProfile.SetProperty("lastName", "test-lastname");
            userProfile.SetProperty("email", "notagoodemail");
            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = enrollResponse });
            queue.Enqueue(new MockResponse { StatusCode = 400, Response = enrollNewResponse });
            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);
            Func<Task<AuthenticationResponse>> function = async () => await testClient.RegisterAsync(userProfile);
            await function.Should()
                .ThrowAsync<OktaException>()
                .WithMessage("*'Email' must be in the form of an email address*");
        }
        #endregion

        #region Is Password Required
        [Fact]
        public async Task RequirePassword()
        {
            #region mockResponses
            string interactResponse = @"{
    ""interaction_handle"": ""HtXTMBoVlBzhIAzDM-GNGkddbYzTCrt1NmEyCUhdzY4""
}";
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02nICqFbKn4utN-0rrIZaQ9DZU5jeukrr7NCJp0RUv"",
    ""expiresAt"": ""2022-03-10T15:45:49.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""identify"",
                ""href"": ""https://testdomain.com/idp/idx/identify"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""identifier"",
                        ""label"": ""Username"",
                        ""required"": true
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
                        ""value"": ""02nICqFbKn4utN-0rrIZaQ9DZU5jeukrr7NCJp0RUv"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-enroll-profile"",
                ""href"": ""https://testdomain.com/idp/idx/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02nICqFbKn4utN-0rrIZaQ9DZU5jeukrr7NCJp0RUv"",
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
                ""href"": ""https://testdomain.com/idp/idx/recover"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02nICqFbKn4utN-0rrIZaQ9DZU5jeukrr7NCJp0RUv"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""type"": ""password"",
            ""key"": ""okta_password"",
            ""id"": ""aut47hb8fiHtj6pqG0g7"",
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
        ""href"": ""https://testdomain.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02nICqFbKn4utN-0rrIZaQ9DZU5jeukrr7NCJp0RUv"",
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
            ""label"": ""Bryan Magic Link"",
            ""id"": ""0oa47ljmt8gt6YXSx0g7""
        }
    }
}";
            #endregion
            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            (await testClient.CheckIsPasswordRequiredAsync()).IsPasswordRequired.Should().Be(true);
        }

        [Fact]
        public async Task NotRequirePassword()
        {
            #region mockResponses
            string interactResponse = @"{
    ""interaction_handle"": ""HtXTMBoVlBzhIAzDM-GNGkddbYzTCrt1NmEyCUhdzY4""
}";
            string introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02nICqFbKn4utN-0rrIZaQ9DZU5jeukrr7NCJp0RUv"",
    ""expiresAt"": ""2022-03-10T15:45:49.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""identify"",
                ""href"": ""https://testdomain.com/idp/idx/identify"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""identifier"",
                        ""label"": ""Username"",
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
                        ""value"": ""02nICqFbKn4utN-0rrIZaQ9DZU5jeukrr7NCJp0RUv"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-enroll-profile"",
                ""href"": ""https://testdomain.com/idp/idx/enroll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02nICqFbKn4utN-0rrIZaQ9DZU5jeukrr7NCJp0RUv"",
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
        ""href"": ""https://testdomain.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02nICqFbKn4utN-0rrIZaQ9DZU5jeukrr7NCJp0RUv"",
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
            ""label"": ""Bryan Magic Link"",
            ""id"": ""0oa47ljmt8gt6YXSx0g7""
        }
    }
}";
            #endregion
            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            (await testClient.CheckIsPasswordRequiredAsync()).IsPasswordRequired.Should().Be(false);
        }
        #endregion

        # region security question

        [Fact]
        public async Task GetSecurityQuestions()
        {
            var idxContext = Substitute.For<IIdxContext>();
            #region mock responses
            var introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02NEtILgyR1C17MkKIsSAZmccezlHcOuOJeZGkn7rh"",
    ""expiresAt"": ""2022-04-05T14:12:12.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://testdomain.com/idp/idx/credential/enroll"",
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
                                                ""value"": ""aut47hb8fj3PqU6dh0g7"",
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
                                ""relatesTo"": ""$.authenticators.value[0]""
                            },
                            {
                                ""label"": ""Password"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""aut47hb8fiHtj6pqG0g7"",
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
                                ""relatesTo"": ""$.authenticators.value[1]""
                            },
                            {
                                ""label"": ""Security Question"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""aut47hb8fllgs7BO50g7"",
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
                                ""relatesTo"": ""$.authenticators.value[2]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02NEtILgyR1C17MkKIsSAZmccezlHcOuOJeZGkn7rh"",
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
                ""id"": ""aut47hb8fj3PqU6dh0g7"",
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
                ""id"": ""aut47hb8fiHtj6pqG0g7"",
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
                ""id"": ""aut47hb8fllgs7BO50g7"",
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
        ""value"": []
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00u48tqxwdHay91rO0g7"",
            ""identifier"": ""testuser@threeheadz.com"",
            ""profile"": {
                ""firstName"": ""test"",
                ""lastName"": ""user"",
                ""timeZone"": ""America/Los_Angeles"",
                ""locale"": ""en_US""
            }
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://testdomain.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02NEtILgyR1C17MkKIsSAZmccezlHcOuOJeZGkn7rh"",
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
            ""label"": ""SQ Web App"",
            ""id"": ""0oa48qersyX7e6sSt0g7""
        }
    }
}";
            var enrollResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02NEtILgyR1C17MkKIsSAZmccezlHcOuOJeZGkn7rh"",
    ""expiresAt"": ""2022-04-05T14:12:23.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""enroll-authenticator"",
                ""relatesTo"": [
                    ""$.currentAuthenticator""
                ],
                ""href"": ""https://testdomain.com/idp/idx/challenge/answer"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""credentials"",
                        ""type"": ""object"",
                        ""required"": true,
                        ""options"": [
                            {
                                ""label"": ""Choose a security question"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""questionKey"",
                                                ""type"": ""string"",
                                                ""label"": ""Choose a security question"",
                                                ""required"": true,
                                                ""options"": [
                                                    {
                                                        ""label"": ""What is the food you least liked as a child?"",
                                                        ""value"": ""disliked_food""
                                                    },
                                                    {
                                                        ""label"": ""What is the name of your first stuffed animal?"",
                                                        ""value"": ""name_of_first_plush_toy""
                                                    },
                                                    {
                                                        ""label"": ""What did you earn your first medal or award for?"",
                                                        ""value"": ""first_award""
                                                    },
                                                    {
                                                        ""label"": ""What is your favorite security question?"",
                                                        ""value"": ""favorite_security_question""
                                                    },
                                                    {
                                                        ""label"": ""What is the toy/stuffed animal you liked the most as a kid?"",
                                                        ""value"": ""favorite_toy""
                                                    },
                                                    {
                                                        ""label"": ""What was the first computer game you played?"",
                                                        ""value"": ""first_computer_game""
                                                    },
                                                    {
                                                        ""label"": ""What is your favorite movie quote?"",
                                                        ""value"": ""favorite_movie_quote""
                                                    },
                                                    {
                                                        ""label"": ""What was the mascot of the first sports team you played on?"",
                                                        ""value"": ""first_sports_team_mascot""
                                                    },
                                                    {
                                                        ""label"": ""What music album or song did you first purchase?"",
                                                        ""value"": ""first_music_purchase""
                                                    },
                                                    {
                                                        ""label"": ""What is your favorite piece of art?"",
                                                        ""value"": ""favorite_art_piece""
                                                    },
                                                    {
                                                        ""label"": ""What was your grandmother's favorite dessert?"",
                                                        ""value"": ""grandmother_favorite_desert""
                                                    },
                                                    {
                                                        ""label"": ""What was the first thing you learned to cook?"",
                                                        ""value"": ""first_thing_cooked""
                                                    },
                                                    {
                                                        ""label"": ""What was your dream job as a child?"",
                                                        ""value"": ""childhood_dream_job""
                                                    },
                                                    {
                                                        ""label"": ""Where did you meet your spouse/significant other?"",
                                                        ""value"": ""place_where_significant_other_was_met""
                                                    },
                                                    {
                                                        ""label"": ""Where did you go for your favorite vacation?"",
                                                        ""value"": ""favorite_vacation_location""
                                                    },
                                                    {
                                                        ""label"": ""Where were you on New Year's Eve in the year 2000?"",
                                                        ""value"": ""new_years_two_thousand""
                                                    },
                                                    {
                                                        ""label"": ""Who is your favorite speaker/orator?"",
                                                        ""value"": ""favorite_speaker_actor""
                                                    },
                                                    {
                                                        ""label"": ""Who is your favorite book/movie character?"",
                                                        ""value"": ""favorite_book_movie_character""
                                                    },
                                                    {
                                                        ""label"": ""Who is your favorite sports player?"",
                                                        ""value"": ""favorite_sports_player""
                                                    }
                                                ]
                                            },
                                            {
                                                ""name"": ""answer"",
                                                ""label"": ""Answer"",
                                                ""required"": true
                                            }
                                        ]
                                    }
                                }
                            },
                            {
                                ""label"": ""Create my own security question"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""questionKey"",
                                                ""required"": true,
                                                ""value"": ""custom"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""question"",
                                                ""label"": ""Create a security question"",
                                                ""required"": true
                                            },
                                            {
                                                ""name"": ""answer"",
                                                ""label"": ""Answer"",
                                                ""required"": true
                                            }
                                        ]
                                    }
                                }
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02NEtILgyR1C17MkKIsSAZmccezlHcOuOJeZGkn7rh"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-enroll"",
                ""href"": ""https://testdomain.com/idp/idx/credential/enroll"",
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
                                                ""value"": ""aut47hb8fj3PqU6dh0g7"",
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
                                ""relatesTo"": ""$.authenticators.value[0]""
                            },
                            {
                                ""label"": ""Password"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""aut47hb8fiHtj6pqG0g7"",
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
                                ""relatesTo"": ""$.authenticators.value[1]""
                            },
                            {
                                ""label"": ""Security Question"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""aut47hb8fllgs7BO50g7"",
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
                                ""relatesTo"": ""$.authenticators.value[2]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02NEtILgyR1C17MkKIsSAZmccezlHcOuOJeZGkn7rh"",
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
            ""contextualData"": {
                ""questionKeys"": [
                    ""favorite_vacation_location"",
                    ""first_music_purchase"",
                    ""favorite_toy"",
                    ""grandmother_favorite_desert"",
                    ""favorite_sports_player"",
                    ""custom"",
                    ""disliked_food"",
                    ""favorite_movie_quote"",
                    ""favorite_book_movie_character"",
                    ""first_computer_game"",
                    ""first_thing_cooked"",
                    ""first_award"",
                    ""name_of_first_plush_toy"",
                    ""place_where_significant_other_was_met"",
                    ""favorite_art_piece"",
                    ""childhood_dream_job"",
                    ""new_years_two_thousand"",
                    ""favorite_security_question"",
                    ""favorite_speaker_actor"",
                    ""first_sports_team_mascot""
                ],
                ""questions"": [
                    {
                        ""questionKey"": ""disliked_food"",
                        ""question"": ""What is the food you least liked as a child?""
                    },
                    {
                        ""questionKey"": ""name_of_first_plush_toy"",
                        ""question"": ""What is the name of your first stuffed animal?""
                    },
                    {
                        ""questionKey"": ""first_award"",
                        ""question"": ""What did you earn your first medal or award for?""
                    },
                    {
                        ""questionKey"": ""favorite_security_question"",
                        ""question"": ""What is your favorite security question?""
                    },
                    {
                        ""questionKey"": ""favorite_toy"",
                        ""question"": ""What is the toy/stuffed animal you liked the most as a kid?""
                    },
                    {
                        ""questionKey"": ""first_computer_game"",
                        ""question"": ""What was the first computer game you played?""
                    },
                    {
                        ""questionKey"": ""favorite_movie_quote"",
                        ""question"": ""What is your favorite movie quote?""
                    },
                    {
                        ""questionKey"": ""first_sports_team_mascot"",
                        ""question"": ""What was the mascot of the first sports team you played on?""
                    },
                    {
                        ""questionKey"": ""first_music_purchase"",
                        ""question"": ""What music album or song did you first purchase?""
                    },
                    {
                        ""questionKey"": ""favorite_art_piece"",
                        ""question"": ""What is your favorite piece of art?""
                    },
                    {
                        ""questionKey"": ""grandmother_favorite_desert"",
                        ""question"": ""What was your grandmother's favorite dessert?""
                    },
                    {
                        ""questionKey"": ""first_thing_cooked"",
                        ""question"": ""What was the first thing you learned to cook?""
                    },
                    {
                        ""questionKey"": ""childhood_dream_job"",
                        ""question"": ""What was your dream job as a child?""
                    },
                    {
                        ""questionKey"": ""place_where_significant_other_was_met"",
                        ""question"": ""Where did you meet your spouse/significant other?""
                    },
                    {
                        ""questionKey"": ""favorite_vacation_location"",
                        ""question"": ""Where did you go for your favorite vacation?""
                    },
                    {
                        ""questionKey"": ""new_years_two_thousand"",
                        ""question"": ""Where were you on New Year's Eve in the year 2000?""
                    },
                    {
                        ""questionKey"": ""favorite_speaker_actor"",
                        ""question"": ""Who is your favorite speaker/orator?""
                    },
                    {
                        ""questionKey"": ""favorite_book_movie_character"",
                        ""question"": ""Who is your favorite book/movie character?""
                    },
                    {
                        ""questionKey"": ""favorite_sports_player"",
                        ""question"": ""Who is your favorite sports player?""
                    }
                ]
            },
            ""type"": ""security_question"",
            ""key"": ""security_question"",
            ""id"": ""aut47hb8fllgs7BO50g7"",
            ""displayName"": ""Security Question"",
            ""methods"": [
                {
                    ""type"": ""security_question""
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
                ""id"": ""aut47hb8fj3PqU6dh0g7"",
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
                ""id"": ""aut47hb8fiHtj6pqG0g7"",
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
                ""id"": ""aut47hb8fllgs7BO50g7"",
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
        ""value"": []
    },
    ""enrollmentAuthenticator"": {
        ""type"": ""object"",
        ""value"": {
            ""type"": ""security_question"",
            ""key"": ""security_question"",
            ""id"": ""aut47hb8fllgs7BO50g7"",
            ""displayName"": ""Security Question"",
            ""methods"": [
                {
                    ""type"": ""security_question""
                }
            ]
        }
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00u48tqxwdHay91rO0g7"",
            ""identifier"": ""testuser@threeheadz.com"",
            ""profile"": {
                ""firstName"": ""test"",
                ""lastName"": ""user"",
                ""timeZone"": ""America/Los_Angeles"",
                ""locale"": ""en_US""
            }
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://testdomain.com/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02NEtILgyR1C17MkKIsSAZmccezlHcOuOJeZGkn7rh"",
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
            ""label"": ""SQ Web App"",
            ""id"": ""0oa48qersyX7e6sSt0g7""
        }
    }
}";
            #endregion
            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = enrollResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var enrollAuthenticatorOptions = new SelectEnrollAuthenticatorOptions
            {
                AuthenticatorId = "test authenticator id",
            };
            var securityQuestionEnrollResponse = await testClient.SelectEnrollAuthenticatorAsync(enrollAuthenticatorOptions, idxContext);
            securityQuestionEnrollResponse.Should().NotBeNull();
            securityQuestionEnrollResponse.CurrentAuthenticator.Should().NotBeNull();
            securityQuestionEnrollResponse.CurrentAuthenticator.ContextualData.Should().NotBeNull();
            securityQuestionEnrollResponse.CurrentAuthenticator.ContextualData.QuestionKeys.Should().NotBeNull();
            securityQuestionEnrollResponse.CurrentAuthenticator.ContextualData.QuestionKeys.Should().HaveCount(20);
            securityQuestionEnrollResponse.CurrentAuthenticator.ContextualData.Questions.Should().NotBeNull();
            securityQuestionEnrollResponse.CurrentAuthenticator.ContextualData.Questions.Should().HaveCount(19);
        }

        [Fact]
        public async Task ProceedWithOktaVerifyPush()
        {
            var idxContext = Substitute.For<IIdxContext>();
            #region mock responses
            var introspectResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
    ""expiresAt"": ""2022-12-01T14:08:27.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""authenticator-verification-data"",
                ""relatesTo"": [
                    ""$.currentAuthenticator""
                ],
                ""href"": ""https://test.org/idp/idx/challenge"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""label"": ""Okta Verify"",
                        ""form"": {
                            ""value"": [
                                {
                                    ""name"": ""id"",
                                    ""required"": true,
                                    ""value"": ""aut2xo0xj4LK7Fupb5d7"",
                                    ""mutable"": false
                                },
                                {
                                    ""name"": ""methodType"",
                                    ""type"": ""string"",
                                    ""required"": true,
                                    ""options"": [
                                        {
                                            ""label"": ""Enter a code"",
                                            ""value"": ""totp""
                                        },
                                        {
                                            ""label"": ""Get a push notification"",
                                            ""value"": ""push""
                                        }
                                    ]
                                }
                            ]
                        }
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-authenticate"",
                ""href"": ""https://test.org/idp/idx/challenge"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""type"": ""object"",
                        ""options"": [
                            {
                                ""label"": ""Okta Verify"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""aut2xo0xj4LK7Fupb5d7"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""Enter a code"",
                                                        ""value"": ""totp""
                                                    },
                                                    {
                                                        ""label"": ""Get a push notification"",
                                                        ""value"": ""push""
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[0]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
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
            ""type"": ""app"",
            ""key"": ""okta_verify"",
            ""id"": ""aut2xo0xj4LK7Fupb5d7"",
            ""displayName"": ""Okta Verify"",
            ""methods"": [
                {
                    ""type"": ""push""
                },
                {
                    ""type"": ""totp""
                }
            ]
        }
    },
    ""authenticators"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""app"",
                ""key"": ""okta_verify"",
                ""id"": ""aut2xo0xj4LK7Fupb5d7"",
                ""displayName"": ""Okta Verify"",
                ""methods"": [
                    {
                        ""type"": ""push""
                    },
                    {
                        ""type"": ""totp""
                    }
                ],
                ""allowedFor"": ""any""
            }
        ]
    },
    ""authenticatorEnrollments"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""profile"": {
                    ""deviceName"": ""Galaxy Z Fold3 5G""
                },
                ""type"": ""app"",
                ""key"": ""okta_verify"",
                ""id"": ""pfd7gxyytl0yP3uMR5d7"",
                ""displayName"": ""Okta Verify"",
                ""methods"": [
                    {
                        ""type"": ""push""
                    },
                    {
                        ""type"": ""totp""
                    }
                ]
            }
        ]
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00u2xoipvjOIR19225d7"",
            ""identifier"": ""user@email.com"",
            ""profile"": {
                ""firstName"": ""Bryan"",
                ""lastName"": ""Apellanes"",
                ""timeZone"": ""America/Los_Angeles"",
                ""locale"": ""en_US""
            }
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://test.org/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
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
            ""label"": ""Okta Verify Web App"",
            ""id"": ""0oa2zrtjg7kdBZ9aG5d7""
        }
    },
    ""authentication"": {
        ""type"": ""object"",
        ""value"": {
            ""protocol"": ""OAUTH2.0"",
            ""issuer"": {
                ""id"": ""aus2xo0xe0oJtPzIS5d7"",
                ""name"": ""default"",
                ""uri"": ""https://test.org/oauth2/default""
            },
            ""request"": {
                ""max_age"": -1,
                ""scope"": ""openid profile offline_access"",
                ""response_type"": ""code"",
                ""redirect_uri"": ""http://localhost:44314/interactioncode/callback"",
                ""state"": ""XkTQQBirTnnuD_JB8d2IEw"",
                ""code_challenge_method"": ""S256"",
                ""code_challenge"": ""luxCCyaGyq1MmP-NHTxWKCxm3rJdHBHyYiOs-KrkIdM"",
                ""response_mode"": ""query""
            }
        }
    }
}";
            var selectAuthenticatorResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
    ""expiresAt"": ""2022-12-01T14:11:43.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""authenticator-verification-data"",
                ""relatesTo"": [
                    ""$.currentAuthenticator""
                ],
                ""href"": ""https://test.org/idp/idx/challenge"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""label"": ""Okta Verify"",
                        ""form"": {
                            ""value"": [
                                {
                                    ""name"": ""id"",
                                    ""required"": true,
                                    ""value"": ""aut2xo0xj4LK7Fupb5d7"",
                                    ""mutable"": false
                                },
                                {
                                    ""name"": ""methodType"",
                                    ""type"": ""string"",
                                    ""required"": true,
                                    ""options"": [
                                        {
                                            ""label"": ""Get a push notification"",
                                            ""value"": ""push""
                                        }
                                    ]
                                },
                                {
                                    ""name"": ""autoChallenge"",
                                    ""type"": ""boolean"",
                                    ""label"": ""Send push automatically"",
                                    ""required"": false,
                                    ""value"": false,
                                    ""mutable"": true
                                }
                            ]
                        }
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-authenticate"",
                ""href"": ""https://test.org/idp/idx/challenge"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""type"": ""object"",
                        ""options"": [
                            {
                                ""label"": ""Okta Verify"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""aut2xo0xj4LK7Fupb5d7"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""Enter a code"",
                                                        ""value"": ""totp""
                                                    },
                                                    {
                                                        ""label"": ""Get a push notification"",
                                                        ""value"": ""push""
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[0]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
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
            ""resend"": {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""resend"",
                ""href"": ""https://test.org/idp/idx/challenge/resend"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""type"": ""app"",
            ""key"": ""okta_verify"",
            ""id"": ""aut2xo0xj4LK7Fupb5d7"",
            ""displayName"": ""Okta Verify"",
            ""methods"": [
                {
                    ""type"": ""push""
                }
            ]
        }
    },
    ""authenticators"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""app"",
                ""key"": ""okta_verify"",
                ""id"": ""aut2xo0xj4LK7Fupb5d7"",
                ""displayName"": ""Okta Verify"",
                ""methods"": [
                    {
                        ""type"": ""push""
                    },
                    {
                        ""type"": ""totp""
                    }
                ],
                ""allowedFor"": ""any""
            }
        ]
    },
    ""authenticatorEnrollments"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""profile"": {
                    ""deviceName"": ""Galaxy Z Fold3 5G""
                },
                ""type"": ""app"",
                ""key"": ""okta_verify"",
                ""id"": ""pfd7gxyytl0yP3uMR5d7"",
                ""displayName"": ""Okta Verify"",
                ""methods"": [
                    {
                        ""type"": ""push""
                    },
                    {
                        ""type"": ""totp""
                    }
                ]
            }
        ]
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00u2xoipvjOIR19225d7"",
            ""identifier"": ""user@email.com"",
            ""profile"": {
                ""firstName"": ""Bryan"",
                ""lastName"": ""Apellanes"",
                ""timeZone"": ""America/Los_Angeles"",
                ""locale"": ""en_US""
            }
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://test.org/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
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
            ""label"": ""Okta Verify Web App"",
            ""id"": ""0oa2zrtjg7kdBZ9aG5d7""
        }
    },
    ""authentication"": {
        ""type"": ""object"",
        ""value"": {
            ""protocol"": ""OAUTH2.0"",
            ""issuer"": {
                ""id"": ""aus2xo0xe0oJtPzIS5d7"",
                ""name"": ""default"",
                ""uri"": ""https://test.org/oauth2/default""
            },
            ""request"": {
                ""max_age"": -1,
                ""scope"": ""openid profile offline_access"",
                ""response_type"": ""code"",
                ""redirect_uri"": ""http://localhost:44314/interactioncode/callback"",
                ""state"": ""XkTQQBirTnnuD_JB8d2IEw"",
                ""code_challenge_method"": ""S256"",
                ""code_challenge"": ""luxCCyaGyq1MmP-NHTxWKCxm3rJdHBHyYiOs-KrkIdM"",
                ""response_mode"": ""query""
            }
        }
    }
}";
            var pushRequestResponse = @"{
    ""version"": ""1.0.0"",
    ""stateHandle"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
    ""expiresAt"": ""2022-12-01T14:12:28.000Z"",
    ""intent"": ""LOGIN"",
    ""remediation"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""challenge-poll"",
                ""relatesTo"": [
                    ""$.currentAuthenticator""
                ],
                ""href"": ""https://test.org/idp/idx/authenticators/poll"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""refresh"": 4000,
                ""value"": [
                    {
                        ""name"": ""autoChallenge"",
                        ""type"": ""boolean"",
                        ""label"": ""Send push automatically"",
                        ""required"": false,
                        ""value"": false,
                        ""mutable"": true
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""select-authenticator-authenticate"",
                ""href"": ""https://test.org/idp/idx/challenge"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""type"": ""object"",
                        ""options"": [
                            {
                                ""label"": ""Okta Verify"",
                                ""value"": {
                                    ""form"": {
                                        ""value"": [
                                            {
                                                ""name"": ""id"",
                                                ""required"": true,
                                                ""value"": ""aut2xo0xj4LK7Fupb5d7"",
                                                ""mutable"": false
                                            },
                                            {
                                                ""name"": ""methodType"",
                                                ""type"": ""string"",
                                                ""required"": false,
                                                ""options"": [
                                                    {
                                                        ""label"": ""Enter a code"",
                                                        ""value"": ""totp""
                                                    },
                                                    {
                                                        ""label"": ""Get a push notification"",
                                                        ""value"": ""push""
                                                    }
                                                ]
                                            }
                                        ]
                                    }
                                },
                                ""relatesTo"": ""$.authenticators.value[0]""
                            }
                        ]
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
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
            ""resend"": {
                ""rel"": [
                    ""create-form""
                ],
                ""name"": ""resend"",
                ""href"": ""https://test.org/idp/idx/challenge"",
                ""method"": ""POST"",
                ""produces"": ""application/ion+json; okta-version=1.0.0"",
                ""value"": [
                    {
                        ""name"": ""authenticator"",
                        ""required"": true,
                        ""value"": {
                            ""methodType"": ""push"",
                            ""id"": ""aut2xo0xj4LK7Fupb5d7""
                        },
                        ""visible"": false,
                        ""mutable"": false
                    },
                    {
                        ""name"": ""stateHandle"",
                        ""required"": true,
                        ""value"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
                        ""visible"": false,
                        ""mutable"": false
                    }
                ],
                ""accepts"": ""application/json; okta-version=1.0.0""
            },
            ""type"": ""app"",
            ""key"": ""okta_verify"",
            ""id"": ""aut2xo0xj4LK7Fupb5d7"",
            ""displayName"": ""Okta Verify"",
            ""methods"": [
                {
                    ""type"": ""push""
                }
            ]
        }
    },
    ""authenticators"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""type"": ""app"",
                ""key"": ""okta_verify"",
                ""id"": ""aut2xo0xj4LK7Fupb5d7"",
                ""displayName"": ""Okta Verify"",
                ""methods"": [
                    {
                        ""type"": ""push""
                    },
                    {
                        ""type"": ""totp""
                    }
                ],
                ""allowedFor"": ""any""
            }
        ]
    },
    ""authenticatorEnrollments"": {
        ""type"": ""array"",
        ""value"": [
            {
                ""profile"": {
                    ""deviceName"": ""Galaxy Z Fold3 5G""
                },
                ""type"": ""app"",
                ""key"": ""okta_verify"",
                ""id"": ""pfd7gxyytl0yP3uMR5d7"",
                ""displayName"": ""Okta Verify"",
                ""methods"": [
                    {
                        ""type"": ""push""
                    },
                    {
                        ""type"": ""totp""
                    }
                ]
            }
        ]
    },
    ""user"": {
        ""type"": ""object"",
        ""value"": {
            ""id"": ""00u2xoipvjOIR19225d7"",
            ""identifier"": ""user@email.com"",
            ""profile"": {
                ""firstName"": ""Bryan"",
                ""lastName"": ""Apellanes"",
                ""timeZone"": ""America/Los_Angeles"",
                ""locale"": ""en_US""
            }
        }
    },
    ""cancel"": {
        ""rel"": [
            ""create-form""
        ],
        ""name"": ""cancel"",
        ""href"": ""https://test.org/idp/idx/cancel"",
        ""method"": ""POST"",
        ""produces"": ""application/ion+json; okta-version=1.0.0"",
        ""value"": [
            {
                ""name"": ""stateHandle"",
                ""required"": true,
                ""value"": ""02.id.tJMobed3RY5uohKURnea8XiYDOospIHu9_tUatsz"",
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
            ""label"": ""Okta Verify Web App"",
            ""id"": ""0oa2zrtjg7kdBZ9aG5d7""
        }
    },
    ""authentication"": {
        ""type"": ""object"",
        ""value"": {
            ""protocol"": ""OAUTH2.0"",
            ""issuer"": {
                ""id"": ""aus2xo0xe0oJtPzIS5d7"",
                ""name"": ""default"",
                ""uri"": ""https://test.org/oauth2/default""
            },
            ""request"": {
                ""max_age"": -1,
                ""scope"": ""openid profile offline_access"",
                ""response_type"": ""code"",
                ""redirect_uri"": ""http://localhost:44314/interactioncode/callback"",
                ""state"": ""XkTQQBirTnnuD_JB8d2IEw"",
                ""code_challenge_method"": ""S256"",
                ""code_challenge"": ""luxCCyaGyq1MmP-NHTxWKCxm3rJdHBHyYiOs-KrkIdM"",
                ""response_mode"": ""query""
            }
        }
    }
}";
            #endregion
            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = selectAuthenticatorResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = pushRequestResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var oktaOptions = new SelectOktaVerifyAuthenticatorOptions
            {
                AuthenticatorMethodType = AuthenticatorMethodType.Push,
                AuthenticatorId = "aut2xo0xj4LK7Fupb5d7"
            };

            var response = await testClient.SelectChallengeAuthenticatorAsync(oktaOptions, idxContext);
            queue.Count.Should().Be(0);
            response.Should().NotBeNull();
            response.AuthenticationStatus.Should().Be(AuthenticationStatus.AwaitingChallengeAuthenticatorPollResponse);
            response.CurrentAuthenticator.Should().NotBeNull();
            response.CurrentAuthenticator.Name.Should().Be("Okta Verify");
        }

        #endregion

        #region Password Expired

        [Fact]
        public async Task IncludeMessageWhenPasswordExpiredWithWarning()
        {
            #region mocks

            var interactResponse = @"{ 'interaction_handle' : 'foo' }";
            var introspectResponse = @"{
   ""version"":""1.0.0"",
   ""stateHandle"":""foo"",
   ""expiresAt"":""2023-02-01T15:27:40.000Z"",
   ""intent"":""LOGIN"",
   ""remediation"":{
      ""type"":""array"",
      ""value"":[
         {
            ""rel"":[
               ""create-form""
            ],
            ""name"":""identify"",
            ""href"":""https://test.com/idp/idx/identify"",
            ""method"":""POST"",
            ""produces"":""application/ion+json; okta-version=1.0.0"",
            ""value"":[
               {
                  ""name"":""identifier"",
                  ""label"":""Username"",
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
                  ""value"":""foo"",
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
            ""href"":""https://test.com/idp/idx/enroll"",
            ""method"":""POST"",
            ""produces"":""application/ion+json; okta-version=1.0.0"",
            ""value"":[
               {
                  ""name"":""stateHandle"",
                  ""required"":true,
                  ""value"":""foo"",
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
            ""name"":""unlock-account"",
            ""href"":""https://test.com/idp/idx/unlock-account"",
            ""method"":""POST"",
            ""produces"":""application/ion+json; okta-version=1.0.0"",
            ""value"":[
               {
                  ""name"":""stateHandle"",
                  ""required"":true,
                  ""value"":""foo"",
                  ""visible"":false,
                  ""mutable"":false
               }
            ],
            ""accepts"":""application/json; okta-version=1.0.0""
         },
         {
            ""name"":""redirect-idp"",
            ""type"":""OIDC"",
            ""idp"":{
               ""id"":""0oa1qd4m68G3QcAfv1d7"",
               ""name"":""foo.okta.com""
            },
            ""href"":""https://test.com/oauth2/aus1qcwynhEv3AflW1d7/v1/authorize?client_id=0oa2cpl777xczKzL21d7&request_uri=urn:okta:MWxLNTRoRWowSXJkT21hZzVUc0d6RUZGMzNZMFpyclMyWHZkRUVuZkhEMDowb2ExcWQ0bTY4RzNRY0FmdjFkNw"",
            ""method"":""GET""
         },
         {
            ""name"":""redirect-idp"",
            ""type"":""MICROSOFT"",
            ""idp"":{
               ""id"":""0oa2kc4yjqW50K7Zq1d7"",
               ""name"":""Azure foo.okta""
            },
            ""href"":""https://test.com/oauth2/aus1qcwynhEv3AflW1d7/v1/authorize?client_id=0oa2cpl777xczKzL21d7&req
1831
uest_uri=urn:okta:MWxLNTRoRWowSXJkT21hZzVUc0d6RUZGMzNZMFpyclMyWHZkRUVuZkhEMDowb2Eya2M0eWpxVzUwSzdacTFkNw"",
            ""method"":""GET""
         },
         {
            ""name"":""redirect-idp"",
            ""type"":""OIDC"",
            ""idp"":{
               ""id"":""0oa2kc6gweIl3rA011d7"",
               ""name"":""Azure Generic OIDC""
            },
            ""href"":""https://test.com/oauth2/aus1qcwynhEv3AflW1d7/v1/authorize?client_id=0oa2cpl777xczKzL21d7&request_uri=urn:okta:MWxLNTRoRWowSXJkT21hZzVUc0d6RUZGMzNZMFpyclMyWHZkRUVuZkhEMDowb2Eya2M2Z3dlSWwzckEwMTFkNw"",
            ""method"":""GET""
         },
         {
            ""name"":""redirect-idp"",
            ""type"":""APPLE"",
            ""idp"":{
               ""id"":""0oa57eqzjgztgLoWN1d7"",
               ""name"":""SignInWithApple""
            },
            ""href"":""https://test.com/oauth2/aus1qcwynhEv3AflW1d7/v1/authorize?client_id=0oa2cpl777xczKzL21d7&request_uri=urn:okta:MWxLNTRoRWowSXJkT21hZzVUc0d6RUZGMzNZMFpyclMyWHZkRUVuZkhEMDowb2E1N2VxempnenRnTG9XTjFkNw"",
            ""method"":""GET""
         }
      ]
   },
   ""cancel"":{
      ""rel"":[
         ""create-form""
      ],
      ""name"":""cancel"",
      ""href"":""https://test.com/idp/idx/cancel"",
      ""method"":""POST"",
      ""produces"":""application/ion+json; okta-version=1.0.0"",
      ""value"":[
         {
            ""name"":""stateHandle"",
            ""required"":true,
            ""value"":""foo"",
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
         ""label"":""Web App OIE"",
         ""id"":""0oa2cpl777xczKzL21d7""
      }
   }
}";
            var identifyResponse = @"{
   ""version"":""1.0.0"",
   ""stateHandle"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-p8r"",
   ""expiresAt"":""2023-02-01T13:32:42.000Z"",
   ""intent"":""LOGIN"",
   ""remediation"":{
      ""type"":""array"",
      ""value"":[
         {
            ""rel"":[
               ""create-form""
            ],
            ""name"":""select-authenticator-authenticate"",
            ""href"":""https://test.com/idp/idx/challenge"",
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
                                    ""value"":""foo"",
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
                     },
                     {
                        ""label"":""Password"",
                        ""value"":{
                           ""form"":{
                              ""value"":[
                                 {
                                    ""name"":""id"",
                                    ""required"":true,
                                    ""value"":""foo"",
                                    ""mutable"":false
                                 },
                                 {
                                    ""name"":""methodType"",
                                    ""required"":false,
                                    ""value"":""password"",
                                    ""mutable"":false
                                 }
                              ]
                           }
                        },
                        ""relatesTo"":""$.authenticatorEnrollments.value[1]""
                     }
                  ]
               },
               {
                  ""name"":""stateHandle"",
                  ""required"":true,
                  ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-"",
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
            ""id"":""aut1qct8vyPDDlgXq1d7"",
            ""displayName"":""Email"",
            ""methods"":[
               {
                  ""type"":""email""
               }
            ],
            ""allowedFor"":""any""
         },
         {
            ""type"":""password"",
            ""key"":""okta_password"",
            ""id"":""aut1qct8vxGypBXRj1d7"",
            ""displayName"":""Password"",
            ""methods"":[
               {
                  ""type"":""password""
               }
            ],
            ""allowedFor"":""sso""
         }
      ]
   },
   ""authenticatorEnrollments"":{
      ""type"":""array"",
      ""value"":[
         {
            ""type"":""email"",
            ""key"":""okta_email"",
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
         ""identifier"":""user@test.com""
      }
   },
   ""cancel"":{
      ""rel"":[
         ""create-form""
      ],
      ""name"":""cancel"",
      ""href"":""https://test.com/idp/idx/cancel"",
      ""method"":""POST"",
      ""produces"":""application/ion+json; okta-version=1.0.0"",
      ""value"":[
         {
            ""name"":""stateHandle"",
            ""required"":true,
            ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-p"",
            ""visible"":false,
            ""mutable"":false
         }
      ],
      ""accepts"":""application/json; okta-version=1.0.0""
   }
}";
            var challengeResponse = @"{
   ""version"":""1.0.0"",
   ""stateHandle"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-"",
   ""expiresAt"":""2023-02-01T13:32:43.000Z"",
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
            ""href"":""https://test.com/idp/idx/challenge/answer"",
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
                  ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-"",
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
            ""href"":""https://test.com/idp/idx/challenge"",
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
                                    ""value"":""aut1qct8vyPDDlgXq1d7"",
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
                     },
                     {
                        ""label"":""Password"",
                        ""value"":{
                           ""form"":{
                              ""value"":[
                                 {
                                    ""name"":""id"",
                                    ""required"":true,
                                    ""value"":""aut1qct8vxGypBXRj1d7"",
                                    ""mutable"":false
                                 },
                                 {
                                    ""name"":""methodType"",
                                    ""required"":false,
                                    ""value"":""password"",
                                    ""mutable"":false
                                 }
                              ]
                           }
                        },
                        ""relatesTo"":""$.authenticatorEnrollments.value[1]""
                     }
                  ]
               },
               {
                  ""name"":""stateHandle"",
                  ""required"":true,
                  ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj"",
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
            ""href"":""https://test.com/idp/idx/recover"",
            ""method"":""POST"",
            ""produces"":""application/ion+json; okta-version=1.0.0"",
            ""value"":[
               {
                  ""name"":""stateHandle"",
                  ""required"":true,
                  ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-p"",
                  ""visible"":false,
                  ""mutable"":false
               }
            ],
            ""accepts"":""application/json; okta-version=1.0.0""
         },
         ""type"":""password"",
         ""key"":""okta_password"",
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
            ""type"":""email"",
            ""key"":""okta_email"",
            ""id"":""aut1qct8vyPDDlgXq1d7"",
            ""displayName"":""Email"",
            ""methods"":[
               {
                  ""type"":""email""
               }
            ],
            ""allowedFor"":""any""
         },
         {
            ""type"":""password"",
            ""key"":""okta_password"",
            ""id"":""aut1qct8vxGypBXRj1d7"",
            ""displayName"":""Password"",
            ""methods"":[
               {
                  ""type"":""password""
               }
            ],
            ""allowedFor"":""sso""
         }
      ]
   },
   ""authenticatorEnrollments"":{
      ""type"":""array"",
      ""value"":[
         {
            ""type"":""email"",
            ""key"":""okta_email"",
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
         ""identifier"":""user@test.com""
      }
   },
   ""cancel"":{
      ""rel"":[
         ""create-form""
      ],
      ""name"":""cancel"",
      ""href"":""https://test.com/idp/idx/cancel"",
      ""method"":""POST"",
      ""produces"":""application/ion+json; okta-version=1.0.0"",
      ""value"":[
         {
            ""name"":""stateHandle"",
            ""required"":true,
            ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-p8rw9Dd"",
            ""visible"":false,
            ""mutable"":false
         }
      ],
      ""accepts"":""application/json; okta-version=1.0.0""
   }
}";
            var answerResponse = @"{
   ""version"":""1.0.0"",
   ""stateHandle"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj"",
   ""expiresAt"":""2023-02-01T13:32:45.000Z"",
   ""intent"":""LOGIN"",
   ""remediation"":{
      ""type"":""array"",
      ""value"":[
         {
            ""rel"":[
               ""create-form""
            ],
            ""name"":""reenroll-authenticator-warning"",
            ""relatesTo"":[
               ""$.currentAuthenticator""
            ],
            ""href"":""https://test.com/idp/idx/challenge/answer"",
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
                  ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj"",
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
            ""name"":""skip"",
            ""href"":""https://test.com/idp/idx/skip"",
            ""method"":""POST"",
            ""produces"":""application/ion+json; okta-version=1.0.0"",
            ""value"":[
               {
                  ""name"":""stateHandle"",
                  ""required"":true,
                  ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-"",
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
            ""message"":""When your password expires, you will have to change your password before you can login to your emanor-oie account."",
            ""i18n"":{
               ""key"":""idx.password.expiring.message"",
               ""params"":[
                  ""emanor-oie""
               ]
            },
            ""class"":""INFO""
         }
      ]
   },
   ""currentAuthenticator"":{
      ""type"":""object"",
      ""value"":{
         ""type"":""password"",
         ""key"":""okta_password"",
         ""id"":""aut1qct8vxGypBXRj1d7"",
         ""displayName"":""Password"",
         ""methods"":[
            {
               ""type"":""password""
            }
         ],
         ""settings"":{
            ""complexity"":{
               ""minLength"":7,
               ""minLowerCase"":1,
               ""minUpperCase"":1,
               ""minNumber"":1,
               ""minSymbol"":0,
               ""excludeUsername"":true,
               ""excludeAttributes"":[
                  
               ]
            },
            ""age"":{
               ""minAgeMinutes"":0,
               ""historyCount"":0
            },
            ""daysToExpiry"":286
         }
      }
   },
   ""authenticators"":{
      ""type"":""array"",
      ""value"":[
         {
            ""type"":""password"",
            ""key"":""okta_password"",
            ""id"":""aut1qct8vxGypBXRj1d7"",
            ""displayName"":""Password"",
            ""methods"":[
               {
                  ""type"":""password""
               }
            ],
            ""allowedFor"":""sso""
         }
      ]
   },
   ""authenticatorEnrollments"":{
      ""type"":""array"",
      ""value"":[
         {
            ""type"":""email"",
            ""key"":""okta_email"",
            ""id"":""eae65oj47xyx2q6k11d7"",
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
            ""id"":""laenc5qxqA0kjPnwM1d6"",
            ""displayName"":""Password"",
            ""methods"":[
               {
                  ""type"":""password""
               }
            ]
         },
         {
            ""type"":""security_question"",
            ""key"":""security_question"",
            ""id"":""qae65oj47yOZnIZDm1d7"",
            ""displayName"":""Security Question"",
            ""methods"":[
               {
                  ""type"":""security_question""
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
         ""id"":""aut1qct8vxGypBXRj1d7"",
         ""displayName"":""Password"",
         ""methods"":[
            {
               ""type"":""password""
            }
         ],
         ""settings"":{
            ""complexity"":{
               ""minLength"":7,
               ""minLowerCase"":1,
               ""minUpperCase"":1,
               ""minNumber"":1,
               ""minSymbol"":0,
               ""excludeUsername"":true,
               ""excludeAttributes"":[
                  
               ]
            },
            ""age"":{
               ""minAgeMinutes"":0,
               ""historyCount"":0
            },
            ""daysToExpiry"":286
         }
      }
   },
   ""user"":{
      ""type"":""object"",
      ""value"":{
         ""id"":""foo"",
         ""identifier"":""user@test.com"",
         ""profile"":{
            ""firstName"":""Key"",
            ""lastName"":""West"",
            ""timeZone"":""America/Los_Angeles"",
            ""locale"":""en_US""
         }
      }
   },
   ""cancel"":{
      ""rel"":[
         ""create-form""
      ],
      ""name"":""cancel"",
      ""href"":""https://test.com/idp/idx/cancel"",
      ""method"":""POST"",
      ""produces"":""application/ion+json; okta-version=1.0.0"",
      ""value"":[
         {
            ""name"":""stateHandle"",
            ""required"":true,
            ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-"",
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
         ""label"":""Web App OIE"",
         ""id"":""0oa2cpl777xczKzL21d7""
      }
   }
}"; 
     

            #endregion

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = challengeResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = answerResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.AuthenticateAsync(
                new AuthenticationOptions
                {
                    Username = "user@mail.com",
                    Password = "P4zzw0rd"
                });

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.PasswordExpired);
            authResponse.Messages.Should().HaveCount(1);
            authResponse.CanSkip.Should().BeTrue();
        }

        [Fact]
        public async Task IncludeComplexityWhenPasswordExpiredWithWarning()
        {
            #region mocks

            var interactResponse = @"{ 'interaction_handle' : 'foo' }";
            var introspectResponse = @"{
   ""version"":""1.0.0"",
   ""stateHandle"":""foo"",
   ""expiresAt"":""2023-02-01T15:27:40.000Z"",
   ""intent"":""LOGIN"",
   ""remediation"":{
      ""type"":""array"",
      ""value"":[
         {
            ""rel"":[
               ""create-form""
            ],
            ""name"":""identify"",
            ""href"":""https://test.com/idp/idx/identify"",
            ""method"":""POST"",
            ""produces"":""application/ion+json; okta-version=1.0.0"",
            ""value"":[
               {
                  ""name"":""identifier"",
                  ""label"":""Username"",
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
                  ""value"":""foo"",
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
            ""href"":""https://test.com/idp/idx/enroll"",
            ""method"":""POST"",
            ""produces"":""application/ion+json; okta-version=1.0.0"",
            ""value"":[
               {
                  ""name"":""stateHandle"",
                  ""required"":true,
                  ""value"":""foo"",
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
            ""name"":""unlock-account"",
            ""href"":""https://test.com/idp/idx/unlock-account"",
            ""method"":""POST"",
            ""produces"":""application/ion+json; okta-version=1.0.0"",
            ""value"":[
               {
                  ""name"":""stateHandle"",
                  ""required"":true,
                  ""value"":""foo"",
                  ""visible"":false,
                  ""mutable"":false
               }
            ],
            ""accepts"":""application/json; okta-version=1.0.0""
         },
         {
            ""name"":""redirect-idp"",
            ""type"":""OIDC"",
            ""idp"":{
               ""id"":""0oa1qd4m68G3QcAfv1d7"",
               ""name"":""foo.okta.com""
            },
            ""href"":""https://test.com/oauth2/aus1qcwynhEv3AflW1d7/v1/authorize?client_id=0oa2cpl777xczKzL21d7&request_uri=urn:okta:MWxLNTRoRWowSXJkT21hZzVUc0d6RUZGMzNZMFpyclMyWHZkRUVuZkhEMDowb2ExcWQ0bTY4RzNRY0FmdjFkNw"",
            ""method"":""GET""
         },
         {
            ""name"":""redirect-idp"",
            ""type"":""MICROSOFT"",
            ""idp"":{
               ""id"":""0oa2kc4yjqW50K7Zq1d7"",
               ""name"":""Azure foo.okta""
            },
            ""href"":""https://test.com/oauth2/aus1qcwynhEv3AflW1d7/v1/authorize?client_id=0oa2cpl777xczKzL21d7&req
1831
uest_uri=urn:okta:MWxLNTRoRWowSXJkT21hZzVUc0d6RUZGMzNZMFpyclMyWHZkRUVuZkhEMDowb2Eya2M0eWpxVzUwSzdacTFkNw"",
            ""method"":""GET""
         },
         {
            ""name"":""redirect-idp"",
            ""type"":""OIDC"",
            ""idp"":{
               ""id"":""0oa2kc6gweIl3rA011d7"",
               ""name"":""Azure Generic OIDC""
            },
            ""href"":""https://test.com/oauth2/aus1qcwynhEv3AflW1d7/v1/authorize?client_id=0oa2cpl777xczKzL21d7&request_uri=urn:okta:MWxLNTRoRWowSXJkT21hZzVUc0d6RUZGMzNZMFpyclMyWHZkRUVuZkhEMDowb2Eya2M2Z3dlSWwzckEwMTFkNw"",
            ""method"":""GET""
         },
         {
            ""name"":""redirect-idp"",
            ""type"":""APPLE"",
            ""idp"":{
               ""id"":""0oa57eqzjgztgLoWN1d7"",
               ""name"":""SignInWithApple""
            },
            ""href"":""https://test.com/oauth2/aus1qcwynhEv3AflW1d7/v1/authorize?client_id=0oa2cpl777xczKzL21d7&request_uri=urn:okta:MWxLNTRoRWowSXJkT21hZzVUc0d6RUZGMzNZMFpyclMyWHZkRUVuZkhEMDowb2E1N2VxempnenRnTG9XTjFkNw"",
            ""method"":""GET""
         }
      ]
   },
   ""cancel"":{
      ""rel"":[
         ""create-form""
      ],
      ""name"":""cancel"",
      ""href"":""https://test.com/idp/idx/cancel"",
      ""method"":""POST"",
      ""produces"":""application/ion+json; okta-version=1.0.0"",
      ""value"":[
         {
            ""name"":""stateHandle"",
            ""required"":true,
            ""value"":""foo"",
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
         ""label"":""Web App OIE"",
         ""id"":""0oa2cpl777xczKzL21d7""
      }
   }
}";
            var identifyResponse = @"{
   ""version"":""1.0.0"",
   ""stateHandle"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-p8r"",
   ""expiresAt"":""2023-02-01T13:32:42.000Z"",
   ""intent"":""LOGIN"",
   ""remediation"":{
      ""type"":""array"",
      ""value"":[
         {
            ""rel"":[
               ""create-form""
            ],
            ""name"":""select-authenticator-authenticate"",
            ""href"":""https://test.com/idp/idx/challenge"",
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
                                    ""value"":""foo"",
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
                     },
                     {
                        ""label"":""Password"",
                        ""value"":{
                           ""form"":{
                              ""value"":[
                                 {
                                    ""name"":""id"",
                                    ""required"":true,
                                    ""value"":""foo"",
                                    ""mutable"":false
                                 },
                                 {
                                    ""name"":""methodType"",
                                    ""required"":false,
                                    ""value"":""password"",
                                    ""mutable"":false
                                 }
                              ]
                           }
                        },
                        ""relatesTo"":""$.authenticatorEnrollments.value[1]""
                     }
                  ]
               },
               {
                  ""name"":""stateHandle"",
                  ""required"":true,
                  ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-"",
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
            ""id"":""aut1qct8vyPDDlgXq1d7"",
            ""displayName"":""Email"",
            ""methods"":[
               {
                  ""type"":""email""
               }
            ],
            ""allowedFor"":""any""
         },
         {
            ""type"":""password"",
            ""key"":""okta_password"",
            ""id"":""aut1qct8vxGypBXRj1d7"",
            ""displayName"":""Password"",
            ""methods"":[
               {
                  ""type"":""password""
               }
            ],
            ""allowedFor"":""sso""
         }
      ]
   },
   ""authenticatorEnrollments"":{
      ""type"":""array"",
      ""value"":[
         {
            ""type"":""email"",
            ""key"":""okta_email"",
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
         ""identifier"":""user@test.com""
      }
   },
   ""cancel"":{
      ""rel"":[
         ""create-form""
      ],
      ""name"":""cancel"",
      ""href"":""https://test.com/idp/idx/cancel"",
      ""method"":""POST"",
      ""produces"":""application/ion+json; okta-version=1.0.0"",
      ""value"":[
         {
            ""name"":""stateHandle"",
            ""required"":true,
            ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-p"",
            ""visible"":false,
            ""mutable"":false
         }
      ],
      ""accepts"":""application/json; okta-version=1.0.0""
   }
}";
            var challengeResponse = @"{
   ""version"":""1.0.0"",
   ""stateHandle"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-"",
   ""expiresAt"":""2023-02-01T13:32:43.000Z"",
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
            ""href"":""https://test.com/idp/idx/challenge/answer"",
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
                  ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-"",
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
            ""href"":""https://test.com/idp/idx/challenge"",
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
                                    ""value"":""aut1qct8vyPDDlgXq1d7"",
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
                     },
                     {
                        ""label"":""Password"",
                        ""value"":{
                           ""form"":{
                              ""value"":[
                                 {
                                    ""name"":""id"",
                                    ""required"":true,
                                    ""value"":""aut1qct8vxGypBXRj1d7"",
                                    ""mutable"":false
                                 },
                                 {
                                    ""name"":""methodType"",
                                    ""required"":false,
                                    ""value"":""password"",
                                    ""mutable"":false
                                 }
                              ]
                           }
                        },
                        ""relatesTo"":""$.authenticatorEnrollments.value[1]""
                     }
                  ]
               },
               {
                  ""name"":""stateHandle"",
                  ""required"":true,
                  ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj"",
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
            ""href"":""https://test.com/idp/idx/recover"",
            ""method"":""POST"",
            ""produces"":""application/ion+json; okta-version=1.0.0"",
            ""value"":[
               {
                  ""name"":""stateHandle"",
                  ""required"":true,
                  ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-p"",
                  ""visible"":false,
                  ""mutable"":false
               }
            ],
            ""accepts"":""application/json; okta-version=1.0.0""
         },
         ""type"":""password"",
         ""key"":""okta_password"",
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
            ""type"":""email"",
            ""key"":""okta_email"",
            ""id"":""aut1qct8vyPDDlgXq1d7"",
            ""displayName"":""Email"",
            ""methods"":[
               {
                  ""type"":""email""
               }
            ],
            ""allowedFor"":""any""
         },
         {
            ""type"":""password"",
            ""key"":""okta_password"",
            ""id"":""aut1qct8vxGypBXRj1d7"",
            ""displayName"":""Password"",
            ""methods"":[
               {
                  ""type"":""password""
               }
            ],
            ""allowedFor"":""sso""
         }
      ]
   },
   ""authenticatorEnrollments"":{
      ""type"":""array"",
      ""value"":[
         {
            ""type"":""email"",
            ""key"":""okta_email"",
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
         ""identifier"":""user@test.com""
      }
   },
   ""cancel"":{
      ""rel"":[
         ""create-form""
      ],
      ""name"":""cancel"",
      ""href"":""https://test.com/idp/idx/cancel"",
      ""method"":""POST"",
      ""produces"":""application/ion+json; okta-version=1.0.0"",
      ""value"":[
         {
            ""name"":""stateHandle"",
            ""required"":true,
            ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-p8rw9Dd"",
            ""visible"":false,
            ""mutable"":false
         }
      ],
      ""accepts"":""application/json; okta-version=1.0.0""
   }
}";
            var answerResponse = @"{
   ""version"":""1.0.0"",
   ""stateHandle"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj"",
   ""expiresAt"":""2023-02-01T13:32:45.000Z"",
   ""intent"":""LOGIN"",
   ""remediation"":{
      ""type"":""array"",
      ""value"":[
         {
            ""rel"":[
               ""create-form""
            ],
            ""name"":""reenroll-authenticator-warning"",
            ""relatesTo"":[
               ""$.currentAuthenticator""
            ],
            ""href"":""https://test.com/idp/idx/challenge/answer"",
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
                  ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj"",
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
            ""name"":""skip"",
            ""href"":""https://test.com/idp/idx/skip"",
            ""method"":""POST"",
            ""produces"":""application/ion+json; okta-version=1.0.0"",
            ""value"":[
               {
                  ""name"":""stateHandle"",
                  ""required"":true,
                  ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-"",
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
            ""message"":""When your password expires, you will have to change your password before you can login to your emanor-oie account."",
            ""i18n"":{
               ""key"":""idx.password.expiring.message"",
               ""params"":[
                  ""emanor-oie""
               ]
            },
            ""class"":""INFO""
         }
      ]
   },
   ""currentAuthenticator"":{
      ""type"":""object"",
      ""value"":{
         ""type"":""password"",
         ""key"":""okta_password"",
         ""id"":""aut1qct8vxGypBXRj1d7"",
         ""displayName"":""Password"",
         ""methods"":[
            {
               ""type"":""password""
            }
         ],
         ""settings"":{
            ""complexity"":{
               ""minLength"":7,
               ""minLowerCase"":1,
               ""minUpperCase"":1,
               ""minNumber"":1,
               ""minSymbol"":0,
               ""excludeUsername"":true,
               ""excludeAttributes"":[
                  
               ]
            },
            ""age"":{
               ""minAgeMinutes"":5,
               ""historyCount"":2
            },
            ""daysToExpiry"":286
         }
      }
   },
   ""authenticators"":{
      ""type"":""array"",
      ""value"":[
         {
            ""type"":""password"",
            ""key"":""okta_password"",
            ""id"":""aut1qct8vxGypBXRj1d7"",
            ""displayName"":""Password"",
            ""methods"":[
               {
                  ""type"":""password""
               }
            ],
            ""allowedFor"":""sso""
         }
      ]
   },
   ""authenticatorEnrollments"":{
      ""type"":""array"",
      ""value"":[
         {
            ""type"":""email"",
            ""key"":""okta_email"",
            ""id"":""eae65oj47xyx2q6k11d7"",
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
            ""id"":""laenc5qxqA0kjPnwM1d6"",
            ""displayName"":""Password"",
            ""methods"":[
               {
                  ""type"":""password""
               }
            ]
         },
         {
            ""type"":""security_question"",
            ""key"":""security_question"",
            ""id"":""qae65oj47yOZnIZDm1d7"",
            ""displayName"":""Security Question"",
            ""methods"":[
               {
                  ""type"":""security_question""
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
         ""id"":""aut1qct8vxGypBXRj1d7"",
         ""displayName"":""Password"",
         ""methods"":[
            {
               ""type"":""password""
            }
         ],
         ""settings"":{
            ""complexity"":{
               ""minLength"":7,
               ""minLowerCase"":1,
               ""minUpperCase"":1,
               ""minNumber"":1,
               ""minSymbol"":0,
               ""excludeUsername"":true,
               ""excludeAttributes"":[
                  
               ]
            },
            ""age"":{
               ""minAgeMinutes"":0,
               ""historyCount"":0
            },
            ""daysToExpiry"":286
         }
      }
   },
   ""user"":{
      ""type"":""object"",
      ""value"":{
         ""id"":""foo"",
         ""identifier"":""user@test.com"",
         ""profile"":{
            ""firstName"":""Key"",
            ""lastName"":""West"",
            ""timeZone"":""America/Los_Angeles"",
            ""locale"":""en_US""
         }
      }
   },
   ""cancel"":{
      ""rel"":[
         ""create-form""
      ],
      ""name"":""cancel"",
      ""href"":""https://test.com/idp/idx/cancel"",
      ""method"":""POST"",
      ""produces"":""application/ion+json; okta-version=1.0.0"",
      ""value"":[
         {
            ""name"":""stateHandle"",
            ""required"":true,
            ""value"":""02.id.SC9CbSIw1k0SpWAy9YIyfNiKj-"",
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
         ""label"":""Web App OIE"",
         ""id"":""0oa2cpl777xczKzL21d7""
      }
   }
}";

            #endregion

            Queue<MockResponse> queue = new Queue<MockResponse>();
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = interactResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = introspectResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = identifyResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = challengeResponse });
            queue.Enqueue(new MockResponse { StatusCode = 200, Response = answerResponse });

            var mockRequestExecutor = new MockedQueueRequestExecutor(queue);
            var testClient = new TesteableIdxClient(mockRequestExecutor);

            var authResponse = await testClient.AuthenticateAsync(
                new AuthenticationOptions
                {
                    Username = "user@mail.com",
                    Password = "P4zzw0rd"
                });

            authResponse.AuthenticationStatus.Should().Be(AuthenticationStatus.PasswordExpired);
            authResponse.CurrentAuthenticator.Settings.Should().NotBeNull();
            authResponse.CurrentAuthenticator.Settings.Complexity.Should().NotBeNull();
            authResponse.CurrentAuthenticator.Settings.Complexity.MinLength.Should().Be(7);
            authResponse.CurrentAuthenticator.Settings.Complexity.MinLowerCase.Should().Be(1);
            authResponse.CurrentAuthenticator.Settings.Complexity.MinUpperCase.Should().Be(1);
            authResponse.CurrentAuthenticator.Settings.Complexity.MinNumber.Should().Be(1);
            authResponse.CurrentAuthenticator.Settings.Complexity.MinSymbol.Should().Be(0);
            authResponse.CurrentAuthenticator.Settings.Complexity.ExcludeUserName.Should().BeTrue();
            authResponse.CurrentAuthenticator.Settings.Complexity.ExcludeAttributes.Should().NotBeNull();
            authResponse.CurrentAuthenticator.Settings.Complexity.ExcludeAttributes.Count.Should().Be(0);
            authResponse.CurrentAuthenticator.Settings.Age.Should().NotBeNull();
            authResponse.CurrentAuthenticator.Settings.Age.MinAgeMinutes.Should().Be(5);
            authResponse.CurrentAuthenticator.Settings.Age.HistoryCount.Should().Be(2);
            authResponse.CurrentAuthenticator.Settings.DaysToExpiry.Should().Be(286);
        }

        #endregion
    }
}
