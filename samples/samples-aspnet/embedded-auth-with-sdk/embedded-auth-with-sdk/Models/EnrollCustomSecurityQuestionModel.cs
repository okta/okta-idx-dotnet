using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace embedded_auth_with_sdk.Models
{
    public class EnrollCustomSecurityQuestionModel : EnrollSecurityQuestionModel
    {
        public EnrollCustomSecurityQuestionModel()
        {
            this.QuestionKey = "custom";
        }

        [Required]
        [Display(Name = "Question")]
        public string Question { get; set; }

    }
}