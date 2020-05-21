using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit;

namespace PharmaACE.NLP.QuestionAnswerService.Validation
{
    public class IntRequestValidator : BaseValidator<int>
    {
        public IntRequestValidator()
        {
           
        }
    }
}