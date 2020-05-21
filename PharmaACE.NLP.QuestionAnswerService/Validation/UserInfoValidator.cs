using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using FluentValidation;
using FluentValidation.Results;
using FluentValidation.WebApi;
using PharmaACE.ChartAudit.Models;
using PharmaACE.ChartAudit.Reporting.EntityProvider;
using PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit;

namespace PharmaACE.NLP.QuestionAnswerService.Validation
{
    public class UserInfoValidator: BaseValidator<UserInfoModel>
    {
        public UserInfoValidator(IUnitOfWork uow)
        {
                      
                RuleFor(user => user.Email).
                    NotEmpty().
                    WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                    WithMessage("Email not valid");                   
            

                RuleFor(user => user.FirstName).
                    NotEmpty().
                    WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                    WithMessage("FirstName not valid");

                RuleFor(user => user.LastName).
                    NotEmpty().
                    WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                    WithMessage("LastName not valid");


                RuleFor(user => user.Company).
                  NotEmpty().
                  WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                  WithMessage("Company Name not valid");

                RuleFor(user => user.DOB).
                    NotEmpty().
                    WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                    WithMessage("DOB not valid");

                RuleFor(user => user.Permission).
                    GreaterThan(0).
                    WithErrorCode(((int)BaseErrorCode.InvalidParameterException).ToString()).
                    WithMessage("PermissionId not valid");
        }
    }
}