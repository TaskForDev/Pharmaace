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
    public class CommentFiltersValidator:BaseValidator<CommentsFilter>
    {
        public CommentFiltersValidator(IUnitOfWork uow)
        {
            RuleFor(comment => comment.UserId).
                GreaterThan(0).
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid User Id");

            RuleFor(comment => comment.ThreadBy).
                InclusiveBetween(0, 2).
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid ThreadBy Id");

            RuleFor(comment => comment.SortBy).
                InclusiveBetween(0, 2).
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid SortBy Id");

            RuleFor(comment => comment.PrivacyBy).
                InclusiveBetween(0, 2).
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid PrivacyBy Id");

        }
    }
}