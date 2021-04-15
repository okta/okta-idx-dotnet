using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace direct_auth_idx.Models
{
    public class VerifyAuthenticatorViewModel : BaseViewModel
    {
        [Required]
        [Display(Name = "Passcode")]
        public string Code { get; set; }

    }
}