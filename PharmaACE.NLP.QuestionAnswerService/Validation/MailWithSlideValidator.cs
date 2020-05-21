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
    public class MailWithSlideValidator:BaseValidator<MailWithSlide>
    {
 
        public MailWithSlideValidator(IUnitOfWork uow)
        {
            RuleFor(mailWithSlide => mailWithSlide.UserId).
                GreaterThan(0).
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid User Id");

            RuleFor(mailWithSlide => mailWithSlide.SlideId).
                GreaterThan(0).
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid SlideId");

            RuleFor(mailWithSlide => mailWithSlide.Subject).
                NotEmpty().NotNull().
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid Subject value");

            RuleFor(mailWithSlide => mailWithSlide.Message).
                NotEmpty().NotNull().
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid Messsage value");

            RuleFor(mailWithSlide => mailWithSlide.ReceiverIds).
                NotNull().NotEmpty().
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid Receiver Ids set");
        }
    }
}