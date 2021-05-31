using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace direct_auth_idx.Models
{
    public static class ViewModelHelper
    {
        public static IList<AuthenticatorViewModel> ConvertToAuthenticatorViewModelList(IList<IAuthenticator> authenticators)
        { 
            var auth= authenticators?
                .Select(x =>
                            new AuthenticatorViewModel
                            {
                                AuthenticatorId = x.Id,
                                Name = x.Name,
                                EnrollmentId = x.EnrollmentId,
                            })
                .ToList() ?? new List<AuthenticatorViewModel>();
            var fa = auth.First();

            auth.Add(new AuthenticatorViewModel
            {
                AuthenticatorId = "zzzzxxxx",
                Name = "SMS",
                EnrollmentId = fa.EnrollmentId
            }
            );
            return auth;
        }
    }
}