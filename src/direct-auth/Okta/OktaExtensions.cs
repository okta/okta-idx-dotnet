using Newtonsoft.Json;
using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace direct_auth.Okta
{
    public static class OktaExtensions
    {
        public const string IdxStateKey = "IdxStateHandle";

/*        public static IdxContext GetIdxContext(this HttpContext httpContext)
        {
            return GetIdxContext(httpContext.Session);
        }

        public static IdxContext GetIdxContext(this ISession session)
        {
            string stateHandle = session.GetString(IdxStateKey);
            return GetIdxContext(session, stateHandle);
        }

        public static IdxContext GetIdxContext(this ISession session, string stateHandle)
        {
            string idxContextJson = session.GetString(stateHandle);
            IdxContext idxContext = JsonConvert.DeserializeObject<IdxContext>(idxContextJson);
            return idxContext;
        }*/
/*
        public static SocialLoginResponse StartSocialLoginAsync(this IIdxClient idxClient, HttpContext httpContext, string state = null, CancellationToken cancellationToken = default)
        {
            return StartSocialLoginAsync(idxClient, httpContext.Session, state, cancellationToken);
        }

        /// <inheritdoc />
        public static SocialLoginResponse StartSocialLoginAsync(this IIdxClient idxClient, ISession session, string state = null, CancellationToken cancellationToken = default)
        {
            var socialLoginSettings = await idxClient.StartSocialLoginAsync(state, cancellationToken);

            session.SetString(IdxStateKey, socialLoginSettings.Context.State);
            string contextJson = JsonConvert.SerializeObject(socialLoginSettings.Context);
            session.SetString(socialLoginSettings.Context.State, contextJson);

            return socialLoginSettings;
        }*/
    }
}