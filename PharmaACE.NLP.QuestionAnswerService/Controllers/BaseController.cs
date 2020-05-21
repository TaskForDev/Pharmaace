using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Cors;
using System.Web.Http.Filters;
using Microsoft.AspNet.Identity;
using Ninject;
using NLog;
using PharmaACE.ChartAudit.Reporting.EntityProvider;
using PharmaACE.Utility;

namespace PharmaACE.NLP.QuestionAnswerService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BaseController : ApiController
    {
        [Inject]
        public IUnitOfWork uow { get; set; }

        protected int userId;
        // public string PasswordResetOn { get; set; }
        public string userName;

        protected int permission { get; set; }

        public BaseController()
        {
            //UnitOfWork = new UnitOfWork();
        }

        public void OnActionBegin()  
        {
            userId = RequestContext.Principal.Identity.GetUserId<int>();
            //PasswordResetOn = (System.Security.Claims.ClaimsPrincipal)RequestContext.Principal.Claims.Where().Single().Value;
            permission= (((ClaimsPrincipal)RequestContext.Principal).Claims.Where(c => c.Type == "Permission").Single().Value).SafeToNum();
            uow.BeginTransaction();
            GlobalDiagnosticsContext.Set("Username", userName);

        }

        public void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
        {

            bool success = true;
            try
            {
                if (actionExecutedContext.Response.IsSuccessStatusCode)
                {
                    success = true;
                }
                else
                {
                    success = false;
                }

            }
            catch (Exception ex)
            {
                //eat it
            }
            if (success)
                uow.Commit();
            else
                uow.RollBack();
        }

    }
    
}



    
