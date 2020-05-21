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
    public class SignInDetailsValidator:BaseValidator<SignInDetail>
    {
        public SignInDetailsValidator(IUnitOfWork uow)
        {

            RuleFor(signIn => signIn.Password).
                NotEmpty().NotNull().
                WithErrorCode(((int)UserServiceErrorCode.PasswordNotValidException).ToString()).
                WithMessage("Password not valid");

          
        }
    }
}