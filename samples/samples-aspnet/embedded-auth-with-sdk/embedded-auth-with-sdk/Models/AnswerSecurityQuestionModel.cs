using Okta.Idx.Sdk;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace embedded_auth_with_sdk.Models
{
    public class AnswerSecurityQuestionModel : EnrollSecurityQuestionModel
    {
        public AnswerSecurityQuestionModel()
        {
        }

        public string QuestionKey { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }

    }
}