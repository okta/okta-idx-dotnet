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
            => authenticators?
                .Select(x =>
                            new AuthenticatorViewModel
                            {
                                AuthenticatorId = x.Id,
                                Name = x.DisplayName
                            })
                .ToList() ?? new List<AuthenticatorViewModel>();

        public static IList<AuthenticatorViewModel> ConvertToAuthenticatorViewModelList(IList<IAuthenticatorEnrollment> authenticatorEnrollments, IList<IAuthenticator> authenticators)

          =>  authenticatorEnrollments.Select(x =>
                                                new AuthenticatorViewModel 
                                                {
                                                    EnrollmentId = x.Id,
                                                    Name = x.DisplayName,
                                                    AuthenticatorId = authenticators?.FirstOrDefault(y => y.Key == x.Key).Id,
                                                }).ToList();
    }
}