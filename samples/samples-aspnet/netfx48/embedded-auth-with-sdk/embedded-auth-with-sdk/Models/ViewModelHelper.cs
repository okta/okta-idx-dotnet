using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace embedded_auth_with_sdk.Models
{
    public static class ViewModelHelper
    {
        public static IList<AuthenticatorViewModel> ConvertToAuthenticatorViewModelList(IList<IAuthenticator> authenticators)
            => authenticators?
                .Select(x =>
                            new AuthenticatorViewModel
                            {
                                AuthenticatorId = x.Id,
                                Name = x.Name,
                                EnrollmentId = x.EnrollmentId,
                            })
                .ToList() ?? new List<AuthenticatorViewModel>();
    }
}