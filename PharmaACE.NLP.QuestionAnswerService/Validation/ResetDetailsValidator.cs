using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using PharmaACE.ChartAudit.Models;
using PharmaACE.ChartAudit.Reporting.EntityProvider;
using PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit;
using PharmaACE.NLP.QuestionAnswerService.Filters;

namespace PharmaACE.NLP.QuestionAnswerService.Validation
{
    public class ResetDetailsValidator:BaseValidator<ResetDetails>
    {
        public ResetDetailsValidator(IUnitOfWork uow)
        {
            RuleFor(rd => rd.NewPassword).
                NotNull().NotEmpty().
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid password");

            RuleFor(rd => rd.OldPassword).
                NotNull().NotEmpty().
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid Old Password");
        }
    }
}