using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.ModelBinding;
using PharmaACE.NLP.QuestionAnswerService.Validation;

namespace PharmaACE.NLP.QuestionAnswerService.Filters
{
    public class BadRequestFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {

            if (IsValidRequest(actionContext))
            {
                actionContext.ModelState.AddModelError("NullRequest", "3||^||Request cannot be null");
            }
            if (!actionContext.ModelState.IsValid)
            {
                actionContext.Response = actionContext.Request.CreateResponse(HttpStatusCode.BadRequest, new ValidationErrorWrapper(actionContext.ModelState));
            }
            base.OnActionExecuting(actionContext);
        }

        public bool IsValidRequest(HttpActionContext actionContext)
        {
            return (HttpContext.Current.Request.Form.AllKeys.Count() == 0 &&
                actionContext.Request.Method.ToString() != "GET" &&
                actionContext.Request.Method.ToString() != "DELETE" &&
                !actionContext.ActionArguments.Values.Any(v => v != null)); 
        }
    }
}