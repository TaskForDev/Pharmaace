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
    public class GroupDetailsValidator:BaseValidator<GroupDetailsModel>
    {
        public GroupDetailsValidator(IUnitOfWork uow)
        {
                RuleFor(group => group.groupName).
                    NotEmpty().
                    WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                    WithMessage("Group name not valid");

                RuleFor(group => group.UserIds).
                    NotEmpty().
                    WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                    WithMessage("User Ids not valid");
                   
        }

      
    }
}