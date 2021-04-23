using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace direct_auth_idx.Models
{
    public class ForgotPasswordViewModel
    {

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }
    }
}