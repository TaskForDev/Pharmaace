using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.IO;
using PharmaACE.ChartAudit.IRepository;
using PharmaACE.ChartAudit.Models;
using PharmaACE.Utility;
using System.Web.Http.Cors;
using System.Web.Hosting;
using NLog;
using PharmaACE.NLP.QuestionAnswerService.Controllers;
using PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit;
using PharmaACE.NLP.QuestionAnswerService.Filters;
using FluentValidation.WebApi;
using PharmaACE.NLP.QuestionAnswerService.Validation;

namespace PharmaACE.ChartAudit.Reports.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [TokenExtractFilter]
    public class UserController : BaseController
    {

        IUserService userService;
       
        
        public UserController(IUserService userService)
        {
            this.userService = userService;
           
        }
        static Logger logger = LogManager.GetCurrentClassLogger();



        [HttpPost]
        [Route("api/User/AddNewUser")]
        public IHttpActionResult AddNewUser(List<UserInfoModel> userInfoModel)
        {
            logger.Info("Inside User/AddNewUser");
            ActionStatus status = new ActionStatus();
            int result = 0;
            try
            {

                result = userService.AddNewUser(userInfoModel,permission);
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
                logger.Error("Exception in Report/AddNewUser: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new {result = result, Status = status });
            else
                return InternalServerError();
        }



        [Route("api/User/GetAllUsers")]
        public IHttpActionResult GetAllUsers()
        {
            logger.Info("Inside User/GetAllUsers");
            ActionStatus status = new ActionStatus();
            List<LoginDetail> userInfo = new List<LoginDetail>();
            try
            {
                userInfo = userService.GetAllUsers(userId,permission);
                if (!userInfo.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
                logger.Info("No UserList Found");
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in User/GetAllUsers: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)       //remove msg use errorcode
                return Ok(new {  UserInfo = userInfo,Status = status});
            else
                return InternalServerError();
        }



        [Route("api/User/GetUserInfo")]
        public IHttpActionResult GetUserInfo()
        {
            logger.Info("Inside User/GetUserInfo");
            List<UserInfoModel> userInfoModel = null;
            ActionStatus status = new ActionStatus();
            try
            {
                userInfoModel = userService.GetUserInfo(permission);
                if (!userInfoModel.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }
            }
            catch (BaseException ex)
            {
                status.Number =(int) ex.ErrorCode;
                logger.Info("No UserInfo Found");
            }

            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in User/GetUserInfo: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }

            if (status.Number != -1)
                return Ok(new {  userInfoModel = userInfoModel, Status = status });
            else
                return InternalServerError();

        }

        [Route("api/User/DeleteUser")]
        [HttpDelete]
        public IHttpActionResult DeleteUser(int userId)
        {
            logger.Info("Inside User/DeleteUser");
            ActionStatus status = new ActionStatus();
            int result=0;
            try
            {

                result = userService.DeleteUser(userId,permission);
               
            }
            catch(BaseException ex)
            {
                status.Number =(int) ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in User/DeleteUser: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }

            if (status.Number != -1)
                return Ok(new {  result = result, Status = status });
            else
                return InternalServerError();
        }



        [Route("api/User/GetSpecificUser")]
        [HttpGet]
        public IHttpActionResult GetSpecificUser(int userId)
        {
            logger.Info("Inside User/GetSpecificUser");
            UserInfoModel userInfoModel = null;
            ActionStatus status = new ActionStatus();
            string msg = String.Empty;
            try
            {
                userInfoModel = userService.GetSpecificUser(userId,permission);

                if(userInfoModel==null)
                {
                    throw new NoDataFoundException();
                }
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }

            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in User/GetSpecificUser: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number == 0)
                return Ok(new { success = true, userInfoModel = userInfoModel, Status = status });
            else
                return InternalServerError();
        }


        [HttpPut]
        [Route("api/User/UpdateUser")]
        public IHttpActionResult UpdateUser(UserInfoModel userInfo)
        {
            logger.Info("Inside User/UpdateUser");
            int result = 0;
            string msg = String.Empty;
            ActionStatus status = new ActionStatus();
            try
            {
                result = userService.UpdateUser(userInfo, permission);
              
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in User/UpdateUser: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }

            if (status.Number != -1)
                return Ok(new { result = result, Status = status });
            else
                return InternalServerError();

        }

        [HttpPost]
        [Route("api/User/CreateNewGroup")]
        public IHttpActionResult CreateNewGroup(GroupDetailsModel groupDetailsModel)
        {
            logger.Info("Inside User/CreateNewGroup");
            int result = 0;
            ActionStatus status = new ActionStatus();
            string msg = String.Empty;
            try
            {
                result = userService.CreateNewGroup(groupDetailsModel,permission);       
            }
            catch(UserServiceException ex)
            {
                status.Number = (int)ex.ErrorCodeService;
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in User/CreateNewGroup: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new {  result = result,Status=status });
            else
                return InternalServerError();

        }


        [Route("api/User/GetGroupList")]
        public IHttpActionResult GetGroupList()
        {
            logger.Info("Inside User/GetGroupList");
            string msg = String.Empty;
            ActionStatus status = new ActionStatus();
            List<GroupListModel> groupListModel = null;
            try
            {
                groupListModel = userService.GetGroupList(permission);
                if(groupListModel==null)
                {
                    throw new NoDataFoundException();
                }
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in User/GetGroupList: {0} \r\n {1}", ex.ToString(), ex.StackTrace);

            }
            if (status.Number != -1)
                return Ok(new { groupListModel = groupListModel, Status = status });
            else
                return InternalServerError();
        }


        [Route("api/User/GetSpecificGroup")]
        public IHttpActionResult GetSpecificGroup(int groupId)
        {

            GroupDetailsModel groupDetailsModel = null;
            ActionStatus status = new ActionStatus();
            try
            {
                groupDetailsModel = userService.GetSpecificGroup(groupId,permission);
                if (groupDetailsModel == null)
                {
                    throw new NoDataFoundException();
                }
            }

            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in User/GetSpecificGroup: {0} \r\n {1}", ex.ToString(), ex.StackTrace);

            }
            if (status.Number  != -1)
                return Ok(new {  groupDetailsModel = groupDetailsModel, Status=status });
            else
                return InternalServerError();
        }

        [HttpPut]
        [Route("api/User/UpdateGroupInformation")]
        public IHttpActionResult UpdateGroupInformation(GroupDetailsModel groupDetailsModel)
        {
            int result = 0;
            ActionStatus status = new ActionStatus();
            try
            {
                result = userService.UpdateGroupInformation(groupDetailsModel,permission);
            }

            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in User/UpdateGroupInformation: {0} \r\n {1}", ex.ToString(), ex.StackTrace);

            }
            if (status.Number != -1)
                return Ok (new {  result = result, Status= status });
            else
                return InternalServerError();
        }


        [Route("api/User/DeleteSpecificGroup")]
        [HttpDelete]
        public IHttpActionResult DeleteSpecificGroup(int groupId)
        {
            int result = 0;
            ActionStatus status = new ActionStatus();
            try
            {
                result = userService.DeleteSpecificGroup(groupId,permission);
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in User/DeleteSpecificGroup: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok(new { Status = status });
            else
                return InternalServerError();
        }

        [Route("api/User/GetRoleInfo")]
        [HttpGet]
        public IHttpActionResult GetRoleInfo()
        {
            List<RoleInfo> roleInfo = null;
            ActionStatus status = new ActionStatus();
            try
            {
                roleInfo = userService.GetRoleInfo(permission);

                if(!roleInfo.AnyOrNotNull())
                {
                    throw new NoDataFoundException();
                }
            }
            catch (BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in User/GetRoleInfo: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok (new {roleInfo=roleInfo, Status = status });
            else
                return InternalServerError();
        }

        [Route("api/User/PushNotification")]
        [HttpGet]
        public IHttpActionResult PushNotification(String message)
        {

            int result = 0;
            string cpath = HostingEnvironment.MapPath("~/Content/" + Util.AppleProdCert + "");
            ActionStatus status = new ActionStatus();
            try
            {
                result = userService.PushNotification(message, cpath,permission);
            
            }
            catch(UserServiceException ex)
            {
                status.Number = (int)ex.ErrorCodeService;
            }
            catch(BaseException ex)
            {
                status.Number = (int)ex.ErrorCode;
            }
            catch (Exception ex)
            {

                status.Number = -1;
                logger.Error("Exception in User/PushNotification: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }

            if (status.Number != -1)
                return Ok (new {  result = result,Status = status });
            else
                return InternalServerError();

        }
        

        [Route("api/User/ResetPassword")]
        [HttpPut]
        public IHttpActionResult ResetPassword(ResetDetails resetDetails)
        {
            
            LoginDetail loginDetail = null;
            ActionStatus status = new ActionStatus();
            try
            {
                loginDetail = userService.ResetPassword(resetDetails,userId);              
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
                logger.Error("Exception in User/ResetPassword: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok (new { LoginDetail = loginDetail, Status = status });
            else
                return InternalServerError();
        }

        
        [Route("api/User/UpdateDeviceKey")]
        [HttpPost]
        public IHttpActionResult UpdateDeviceKey(DeviceToken deviceToken)
        {
            logger.Info("Inside User/DeviceTokenKey");
            int result = 0;
            ActionStatus status = new ActionStatus();
            try
            {
                result = userService.UpdateDeviceKey(deviceToken,userId,permission);
            }
            catch (Exception ex)
            {
                status.Number = -1;
                logger.Error("Exception in User/UpdateDeviceKey: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
            }
            if (status.Number != -1)
                return Ok (new {  result = result, Status = status });
            else
                return InternalServerError();
        }
    }
}