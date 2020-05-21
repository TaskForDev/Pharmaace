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
    public class CommentDetailsValidator:BaseValidator<CommentDetails>
    {
        public CommentDetailsValidator(IUnitOfWork uow)
        {
            RuleFor(comment => comment.UserId).
                GreaterThan(0).
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid UserId ");

            RuleFor(comment => comment.SlideId).
                GreaterThan(0).NotNull().
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid Slide Id");

            RuleFor(comment => comment.IsPublic).
                NotNull().NotNull().
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid IsPublic bit");

            RuleFor(comment => comment.ParentId).
                NotNull().
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid Parent Id");
            
        }
    }
}