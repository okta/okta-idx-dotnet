using Okta.Sdk.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Okta.Idx.Sdk
{
    public class RedeemInteractionCodeException : OktaException
    {
        public RedeemInteractionCodeException(Exception innerException) : base("Exception occurred redeeming interaction code.", innerException)
        {
        }

        public RedeemInteractionCodeException(string apiResponse, Exception innerException = null) : base("Exception occurred redeeming interaction code.", innerException)
        {
            this.ApiResponse = apiResponse;
        }

        public string ApiResponse { get; set; }
    }
}
