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
    public class ReportFilterValidator:BaseValidator<ReportFilter>
    {
        public ReportFilterValidator(IUnitOfWork uow)
        {
            RuleFor(reportFilter => reportFilter.UserId).
                GreaterThan(0).
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid User Id");

            RuleFor(reportFilter => reportFilter.SortBy).
                InclusiveBetween(0, 2).
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid SortBy Id");

            RuleFor(reportFilter => reportFilter.ViewBy).
                InclusiveBetween(0, 2).
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid ViewBy Id");
                
        }
    }
}