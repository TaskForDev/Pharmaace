using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using PharmaACE.NLP.QuestionAnswerService.Controllers;

namespace PharmaACE.NLP.QuestionAnswerService.Filters
{
    public class TokenExtractFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            (actionContext.ControllerContext.Controller as BaseController).OnActionBegin(); //Retrieve the current controller from the context.
            //base.OnActionExecuting(actionContext);
        }

        public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {
            (actionExecutedContext.ActionContext.ControllerContext.Controller as BaseController).OnActionExecuted(actionExecutedContext);
            //new BaseController().OnActionExecuted(actionExecutedContext);
            //base.OnActionExecuted(actionExecutedContext);
        }
    }
}