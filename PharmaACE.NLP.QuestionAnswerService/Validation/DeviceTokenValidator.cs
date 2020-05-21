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
    public class DeviceTokenValidator:BaseValidator<DeviceToken>
    {
        public DeviceTokenValidator(IUnitOfWork uow)
        {
            RuleFor(deviceToken => deviceToken.DeviceKey).
                NotEmpty().NotNull().
                WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid Device key");

            RuleFor(deviceToken => deviceToken.OldDeviceKey).
                NotNull().
                WithMessage(((int)BaseErrorCode.InvalidParameterException).ToString()).
                WithMessage("Invalid OldDevice Key");
        }
    }
}