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
    public class ReportDetailsValidator: BaseValidator<ReportDetail>
    {
        public ReportDetailsValidator(IUnitOfWork uow)
        {
            RuleFor(report => report.AuthorId).
                 GreaterThanOrEqualTo(1).
                 WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                 WithMessage("Invalid Author Id");


            RuleFor(report => report.SubcategoryId).
                GreaterThanOrEqualTo(1).
                 WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                 WithMessage("Invalid SubcategoryId Id");

            RuleFor(report => report.IsPublic).
                NotEmpty().NotNull().
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("isPublic flag not valid");
       }
    }
}