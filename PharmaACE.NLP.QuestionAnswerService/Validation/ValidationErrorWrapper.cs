using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.ModelBinding;
using FluentValidation;
using FluentValidation.WebApi;
using FluentValidation.Results;
using System.Web.Http.Controllers;
using PharmaACE.Utility;
using System.Net.Http;

namespace PharmaACE.NLP.QuestionAnswerService.Validation
{
    //Stolen from http://sergeyakopov.com/restful-validation-with-asp-net-web-api-and-fluentvalidation/
    public class ValidationErrorWrapper
    {
        private const string ErrorMessage = "The request is invalid.";
        private const string MissingPropertyError = "Undefined error.";
        public string Message { get; private set; }
        public IEnumerable<ActionStatus> Errors { get; private set; }

        public ValidationErrorWrapper(ModelStateDictionary modelState)
        {
            Message = ErrorMessage;
            SerializeModelState(modelState);
        }

        public ValidationErrorWrapper(string message, ModelStateDictionary modelState)
        {
            Message = message;
            SerializeModelState(modelState);
        }

        private void SerializeModelState(ModelStateDictionary modelState)
        {
            Errors = new List<ActionStatus>();

            foreach (var keyModelStatePair in modelState)
            {
                var key = keyModelStatePair.Key;

                var errors = keyModelStatePair.Value.Errors;

                if (errors != null && errors.Count > 0)
                {
                    List<ActionStatus> errList = new List<ActionStatus>();
                    foreach (var err in errors)
                    {
                        if (String.IsNullOrWhiteSpace(err.ErrorMessage))
                            errList.Add(new ActionStatus { Number = -1, Message = MissingPropertyError });
                        else
                        {
                            var errSplit = err.ErrorMessage.Split(new string[] { "||^||" }, StringSplitOptions.None);
                                int errNumber = errSplit[0].SafeToNum();
                            if (errNumber != -1)
                                errList.Add(new ActionStatus { Number = errNumber, Message = errSplit[1].SafeTrim() });
                            else
                                errList.Add(new ActionStatus { Message = String.Format("{0}:{1}", errSplit[0], errSplit[1])});
                        }
                    }
                    Errors = errList;
                }
            }
        }
    }

    public abstract class BaseValidator<T> : AbstractValidator<T>, IValidatorInterceptor
    {
        //protected List<ValidationRuleSet> ruleSets=new List<ValidationRuleSet>();
        //protected DeletionItem deletionItem;
        public ValidationContext BeforeMvcValidation(HttpActionContext controllerContext, ValidationContext validationContext)
        {
            //Validate(validationContext);
            return validationContext;
        }

        public ValidationResult AfterMvcValidation(HttpActionContext controllerContext, ValidationContext validationContext, ValidationResult result)
        {
            var projection = result.Errors.Select(failure => new ValidationFailure(failure.PropertyName, String.Format("{0}||^||{1}", failure.ErrorCode, failure.ErrorMessage)));
            return new ValidationResult(projection);
        }

        protected override void EnsureInstanceNotNull(object instanceToValidate)
        {
            if (instanceToValidate == null)
                throw new ValidationException(new List<ValidationFailure> { new ValidationFailure("Request", "Request cannot be null")});
        }

        //public override ValidationResult Validate(ValidationContext<T> context)
        //{
        //    ValidationFailure vf = new ValidationFailure("Request", "Request cannot be null");
        //    ValidationResult vr = new ValidationResult(new[] { vf });
        //    vf.ErrorCode = "3";

        //    return context.InstanceToValidate == null ? vr
        //        : base.Validate(context);
        //}
    }
}