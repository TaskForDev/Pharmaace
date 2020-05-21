using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PharmaACE.ChartAudit.Models;
using static PharmaACE.ChartAudit.Models.UserInfoModel;
using PharmaACE.ChartAudit.Repository;
using PharmaACE.ChartAudit.IRepository;
using System.Web.Http.Cors;
using NLog;
using PharmaACE.Utility;
using PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit;
using static PharmaACE.Utility.Util;

namespace PharmaACE.NLP.QuestionAnswerService.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class LoginController : BaseController
    {
        IUserService userService;

        public LoginController(IUserService userService)
        {
            this.userService = userService;
        }
        public static Logger logger = LogManager.GetCurrentClassLogger();
        [HttpPost]
        [Route("api/Login/SignIn")]
        public IHttpActionResult SignIn(SignInDetail signin)
        {
            logger.Info("Inside Login/SignIn");
            ActionStatus status = new ActionStatus();
            LoginDetail loginDetail = new LoginDetail();
            try
            {
                loginDetail = userService.SignIn(signin);
                if (loginDetail.Result == 2)
                {
       
                    logger.Info("Login Unsuccessful.Invalid Password");
                    throw new PasswordNotValidException(); 
                }
                else if (loginDetail.Result == 3)
                {
                    logger.Info("Login Unsuccessful.Invalid User");
                    throw new UserNotValidException();
                }
            }
            catch (UserServiceException ex)
            {
                status.Number = (int)ex.ErrorCodeService;
                if (status.Number == 7)
                {
                    status.Message = "Login unsuccessful.Invalid User.";
                }
                else if (status.Number == 8)
                {
                    status.Message = "Login Unsuccessful.Invalid Password";
                }
            }
            
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Login/SignIn: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }

            if (status.Number != -1)
                return Ok(new {  LoginDetail = loginDetail,Status= status });
            else
                return InternalServerError();
        }


        [Route("api/Login/ForgotPassword")]
        [HttpGet]
        public IHttpActionResult ForgotPassword(string userEmail)
        {
            logger.Info("Inside Login/ForgotPassword");
            ActionStatus status = new ActionStatus();
            int result = 0;
            try
            {
                result = userService.ForgotPassword(userEmail);
              
            }
            catch (UserServiceException ex)
            {
                status.Number = (int)ex.ErrorCodeService;
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in Login/ForgotPassword: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new {  result = result,Status = status });
            else
                return InternalServerError();
        }
    }
}

       
