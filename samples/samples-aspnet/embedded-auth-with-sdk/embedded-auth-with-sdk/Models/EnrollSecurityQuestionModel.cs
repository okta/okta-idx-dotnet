using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace embedded_auth_with_sdk.Models
{
    public class EnrollSecurityQuestionModel
    {
        [Required]
        [Display(Name = "Answer")]
        [StringLength(100, MinimumLength = 4)]
        public string Answer { get; set; }

        [Required]
        [Display(Name = "Question")]
        public string QuestionKey { get; set; }

        public List<string> QuestionKeys { get; set; }

        public List<SecurityQuestion> Questions { get; set; }

        public SelectList SelectListItems
        {
            get => new SelectList(Questions, "QuestionKey", "Question");
        }
    }
}