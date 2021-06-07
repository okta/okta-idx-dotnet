using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace embedded_auth_with_sdk.Models
{
    public class VerifyAuthenticatorViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Passcode")]
        public string Code { get; set; }

    }
}