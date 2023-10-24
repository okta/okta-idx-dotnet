using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace embedded_auth_with_sdk.Models
{
    public class EnrollGoogleAuthenticatorViewModel : BaseViewModel
    {

        [Display(Name = "QR code")]
        public string QrCodeHref { get; set; }

        [Display(Name = "Shared secret")]
        public string SharedSecret { get; set; }
    }
}
