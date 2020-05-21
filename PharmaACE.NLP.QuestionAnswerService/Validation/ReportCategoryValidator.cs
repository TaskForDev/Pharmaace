using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.WebApi;
using PharmaACE.ChartAudit.Models;
using PharmaACE.ChartAudit.Reporting.EntityProvider;
using PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit;
using PharmaACE.NLP.QuestionAnswerService.Filters;


namespace PharmaACE.NLP.QuestionAnswerService.Validation
{
    public class ReportCategoryValidator:BaseValidator<ReportCategoryModel>
    {
        public ReportCategoryValidator(IUnitOfWork uow)
        {
            RuleFor(repotCategoryModel => repotCategoryModel.CategoryName).
                NotEmpty().NotNull().
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid Category Name");
        }
    }
}