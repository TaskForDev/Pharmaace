using System;
using System.Collections.Generic;
using System.Linq;
using PharmaACE.ChartAudit.IRepository;
using PharmaACE.ChartAudit.Models;
using PharmaACE.ChartAudit.Reporting.EntityProvider;
using PharmaACE.ChartAudit.Reporting.EntityProvider.IOS_App;
using PharmaACE.ChartAudit.Utility;
using PushSharp.Apple;
using Newtonsoft.Json.Linq;
using PharmaACE.Utility;
using NLog;
using EntityFramework.Extensions;
using PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit;

namespace PharmaACE.ChartAudit.Repository
{
    public class UserService : IUserService
    {
        public IUnitOfWork unitOfWork { get; set; }

        public UserService()
        {
           //new UserService(unitOfWork);

        }

        public UserService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;

        }
        static Logger logger = LogManager.GetCurrentClassLogger();
        public List<UserInfoModel> GetUserInfo(int permission)
        {
            try
            {
                if(permission >  0 && permission < 3)
                {
                    logger.Info("Inside UserServices/GetUserInfo");
                    List<UserInfoModel> userList = null;

                    userList = unitOfWork.DbContext.UserDetail.Select(ud => new UserInfoModel()
                    {
                        UserId = ud.ID,
                        FullName = ud.Name,
                        Company = ud.CompanyName,
                        Contact = ud.UserTelephone,
                        Email = ud.Email,
                    }).ToList();

                    return userList;
                }               
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int AddNewUser(List<UserInfoModel> userInfoModel, int permission)
        {
            int result = 0;
            List<String> mailIdsList = new List<string>();
            List<String> passwordList = new List<string>();
            try
            {
                if(permission == 1)
                {
                    foreach (UserInfoModel userInfo in userInfoModel)
                    {
                       
                            var userIsPresent = unitOfWork.DbContext.UserDetail
                                        .Any(ud => String.Compare(ud.Email, userInfo.Email, true) == 0
                                        );
                            if (userIsPresent)
                            {
                                throw new UserAlreadyPresentException();
                            }
                            else
                            {
                                userInfo.PasswordToMail = PasswordManager.GeneratePassword1();
                                string salt = String.Empty;
                                userInfo.Password = PasswordManager.GeneratePasswordHash(userInfo.PasswordToMail, out salt);
                                UserDetail userDetail = new UserDetail
                                {
                                    Name = userInfo.FirstName + " " + userInfo.LastName,
                                    Email = userInfo.Email,
                                    Password = userInfo.Password,
                                    //RegisteredDate = DateTime.UtcNow,
                                    UserTelephone = userInfo.Contact,
                                    CompanyName = userInfo.Company,
                                    Dob = userInfo.DOB,
                                    Salt = salt,
                                    SetByAdmin = true
                                };


                                //masterContext.UserDetail.Add(userDetail);
                                PermissionLevelMapping permissionMappingTable = new PermissionLevelMapping()
                                {
                                    UserDetail = userDetail,
                                    PermissionId = userInfo.Permission,
                                    CreationDate = DateTime.UtcNow
                                };

                                PasswordResetMapping passwordResetMapping = new PasswordResetMapping()
                                {
                                    UserDetail = userDetail,
                                    PasswordResetOn = DateTime.UtcNow,
                                };


                                unitOfWork.DbContext.PermissionLevelMappings.Add(permissionMappingTable);
                                unitOfWork.DbContext.PasswordResetMapping.Add(passwordResetMapping);
                                mailIdsList.Add(userDetail.Email);
                                passwordList.Add(userInfo.PasswordToMail);
                            }


                    }

                    unitOfWork.DbContext.SaveChanges();

                    foreach (var mp in mailIdsList.Zip(passwordList, Tuple.Create))
                    {
                        SendMail.SendMailForRegistration(mp.Item1, mp.Item2);
                    }

                    result = 1;
                    return result;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }           
            catch(UserServiceException ex)
            {
                throw ex;
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int DeleteUser(int userId, int permission)
        {
            int result = 0;
            try
            {
                if(permission == 1)
                {
                    var idsToDelete = unitOfWork.DbContext.UserDetail.Where(ud => ud.ID == userId).ToList();

                    if(idsToDelete.AnyOrNotNull())
                    {
                        var userGroupIds = unitOfWork.DbContext.UserGroup.Where(ug => ug.UserID == userId).Future();

                        var reportIds = unitOfWork.DbContext.Report.Where(r => r.Author == userId).Select(r => r.ID).ToList();

                        var slideIds = unitOfWork.DbContext.ReportSlide.Where(rs => reportIds.Contains(rs.ReportID)).ToList();

                        var reportViewIds = unitOfWork.DbContext.ReportView.Where(rv => reportIds.Contains(rv.ReportID) || rv.UserID == userId).Future();

                        var reportGroup = unitOfWork.DbContext.ReportGroup.Where(rg => reportIds.Contains(rg.ReportID)).Future();

                        var reportFav = unitOfWork.DbContext.ReportFavorite.Where(rf => reportIds.Contains(rf.ReportID)).Future();

                        var reportRowList = unitOfWork.DbContext.Report.Where(r => r.Author == userId).Future();

                        var commentId = unitOfWork.DbContext.Comment.Where(c => c.UserID == userId).Select(c => c.ID).ToList();


                        var commentTag = unitOfWork.DbContext.CommentTagUser.Where(ctu => ctu.UserID == userId || commentId.Contains(ctu.CommentID)).Future();

                        var commentView = unitOfWork.DbContext.CommentView.Where(cv => cv.UserId == userId || commentId.Contains(cv.CommentId)).Future();


                        var commentLike = unitOfWork.DbContext.CommentLike.Where(cl => cl.UserID == userId || commentId.Contains(cl.CommentID)).Future();


                        var commentToDelete = unitOfWork.DbContext.Comment.Where(c => c.UserID == userId).Future();

                        var passwordReset = unitOfWork.DbContext.PasswordResetMapping.Where(prm => prm.UserId == userId).Future();

                        var deviceToken = unitOfWork.DbContext.DeviceTokenMapping.Where(dtm => dtm.UserID == userId).Future();
                        var permissionLevel = unitOfWork.DbContext.PermissionLevelMappings.Where(plm => plm.UserId == userId).ToList();





                        unitOfWork.DbContext.UserGroup.RemoveRange(userGroupIds);
                        unitOfWork.DbContext.ReportView.RemoveRange(reportViewIds);
                        unitOfWork.DbContext.ReportSlide.RemoveRange(slideIds);
                        unitOfWork.DbContext.ReportGroup.RemoveRange(reportGroup);
                        unitOfWork.DbContext.ReportFavorite.RemoveRange(reportFav);
                        unitOfWork.DbContext.Report.RemoveRange(reportRowList);
                        unitOfWork.DbContext.CommentTagUser.RemoveRange(commentTag);
                        unitOfWork.DbContext.CommentView.RemoveRange(commentView);
                        unitOfWork.DbContext.CommentLike.RemoveRange(commentLike);
                        unitOfWork.DbContext.Comment.RemoveRange(commentToDelete);
                        unitOfWork.DbContext.DeviceTokenMapping.RemoveRange(deviceToken);
                        unitOfWork.DbContext.PermissionLevelMappings.RemoveRange(permissionLevel);
                        unitOfWork.DbContext.PasswordResetMapping.RemoveRange(passwordReset);

                        unitOfWork.DbContext.UserDetail.RemoveRange(idsToDelete);

                        unitOfWork.DbContext.SaveChanges();
                        result = 1;


                        return result;
                    }
                    else
                    {
                        throw new NoDataFoundException();
                    }
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public UserInfoModel GetSpecificUser(int userId,int permission)
        {

            UserInfoModel userInfo = null;

            try
            {
                if(permission == 1)
                {
                    userInfo = unitOfWork.DbContext.UserDetail.Where(ud => ud.ID == userId)
                        .Select(ud => new UserInfoModel
                        {
                            UserId=ud.ID,
                            FullName = ud.Name,
                            Company = ud.CompanyName,
                            Contact = ud.UserTelephone,
                            Email = ud.Email,
                            DOB = ud.Dob,
                            Permission = ud.PermissionLevelMapping.OrderByDescending(plm => plm.CreationDate).Select(plm => plm.PermissionId).FirstOrDefault()
                        }).FirstOrDefault();

                  
                    return userInfo;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateUser(UserInfoModel userInfo,int permission)
        {
            int result = 0;
            try
            {
                if(permission == 1)
                {
                    
                        var userDetail = unitOfWork.DbContext.UserDetail.Where(ud => ud.ID == userInfo.UserId).FirstOrDefault();
                        if (userDetail != null)
                        {
                            userDetail.Name = userInfo.FirstName;
                            userDetail.CompanyName = userInfo.Company;
                            userDetail.Email = userInfo.Email;
                            userDetail.UserTelephone = userInfo.Contact;
                            userDetail.Dob = userInfo.DOB;

                        PermissionLevelMapping PermissionLevelMapping = new PermissionLevelMapping()
                            {
                                UserDetail = userDetail,
                                PermissionId = userInfo.Permission,
                                CreationDate = DateTime.UtcNow
                            };

                            unitOfWork.DbContext.PermissionLevelMappings.Add(PermissionLevelMapping);

                            unitOfWork.DbContext.SaveChanges();
                            result = 1;
                        }
                        else
                        {
                            throw new NoDataFoundException();
                        }                           
                    

                    return result;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int CreateNewGroup(GroupDetailsModel groupDetailsModel,int permission)
        {
            int result = 0;
            try
            {
                if(permission == 1 )
                {
                    
                        var groupIsPresent = unitOfWork.DbContext.Group
                              .Any(g => String.Compare(g.Name, groupDetailsModel.groupName, true) == 0
                             );
                        if (groupIsPresent)
                        {
                            throw new GroupAlreadyPresentException();
                        }
                        else
                        {
                            Group gr = new Group
                            {
                                Name = groupDetailsModel.groupName,
                                CreationDate = DateTime.UtcNow
                            };
                            unitOfWork.DbContext.Group.Add(gr);
                            foreach (var id in groupDetailsModel.UserIds)
                            {
                                UserGroup group = new UserGroup
                                {
                                    Group = gr,
                                    UserID = id
                                };
                                unitOfWork.DbContext.UserGroup.Add(group);
                            }
                            unitOfWork.DbContext.SaveChanges();
                            result = 1;
                        }

                    
                    return result;
                }  
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(UserServiceException ex)
            {
                throw ex;
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<GroupListModel> GetGroupList(int permission)
        {
            
            List<GroupListModel> groupListModels = null;
            try
            {
                if (permission > 0 && permission < 3)
                {
                    groupListModels = unitOfWork.DbContext.Group
                      .Select(gp => new GroupListModel
                      {
                          ID = gp.ID,
                          GroupName = gp.Name,
                          CreationDate = gp.CreationDate,
                          GroupMemberCount = gp.UserGroup.Count()
                      }
                      ).OrderByDescending(g => g.CreationDate).ToList();

                    return groupListModels;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
                  
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public GroupDetailsModel GetSpecificGroup(int groupId,int permission)
        {
            GroupDetailsModel groupDetailsModel = null;

            try
            {
               if(permission == 1)
               {
                    groupDetailsModel = new GroupDetailsModel();
                    groupDetailsModel.usersDetails = unitOfWork.DbContext.UserDetail.Select(g => new UserDetails
                    {
                        UserName = g.Name,
                        UserId = g.ID,
                        IsMember = g.UserGroup.Any(us => us.GroupID == groupId) == true ? true : false,

                    }).ToList();
                    groupDetailsModel.groupName = unitOfWork.DbContext.Group.Where(gp => gp.ID == groupId).Select(gp => gp.Name).FirstOrDefault();
                    return groupDetailsModel;
                }
               else
               {
                    throw new PermissionDeniedException();
               }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public int UpdateGroupInformation(GroupDetailsModel groupDetailsModel,int permission)
        {
            int result = 0;
            try
            {
                if(permission == 1)
                {
                        var group = unitOfWork.DbContext.Group.Where(g => g.ID == groupDetailsModel.groupId).FirstOrDefault();
                        if (group != null)
                        {
                            group.Name = groupDetailsModel.groupName;
                            List<int> newUserList = groupDetailsModel.usersDetails.Where(u => u.IsMember == true).Select(u => u.UserId).ToList();
                            List<int> oldUserList = unitOfWork.DbContext.UserGroup
                                .Where(ug => ug.GroupID == groupDetailsModel.groupId)
                                .Select(ug => ug.UserID)
                                .ToList();
                            List<int> firstNotSecond = newUserList.Except(oldUserList).ToList();
                            List<int> secondNotFirst = oldUserList.Except(newUserList).ToList();

                            foreach (var UserId in firstNotSecond)
                            {
                                UserGroup userGp = new UserGroup()
                                {
                                    GroupID = groupDetailsModel.groupId,
                                    UserID = UserId
                                };
                                unitOfWork.DbContext.UserGroup.Add(userGp);
                            }

                            var oldUserGroup = unitOfWork.DbContext.UserGroup.Where(u => secondNotFirst.Contains(u.UserID) && u.GroupID == groupDetailsModel.groupId).ToList();
                            unitOfWork.DbContext.UserGroup.RemoveRange(oldUserGroup);
                            unitOfWork.DbContext.SaveChanges();
                            result = 1;
                        }
                        else
                        {
                            throw new NoDataFoundException();
                        }

                    return result;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }          
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public int DeleteSpecificGroup(int groupId,int permission)
        {
            int result = 0;
            try
            {
                if(permission == 1)
                {
                    var groupTableId = unitOfWork.DbContext.Group.Where(g => g.ID == groupId).FirstOrDefault();

                    if (groupTableId != null)
                    {
                        var group = unitOfWork.DbContext.Group
                                  .Where(gr => gr.ID == groupId)
                                  .Select(gr => new
                                  {
                                      groupRecord = gr,
                                      userGroup = gr.UserGroup,
                                      reportGroup = gr.ReportGroup
                                  }).FirstOrDefault();

                        if (group != null)
                        {
                            unitOfWork.DbContext.UserGroup.RemoveRange(group.userGroup);

                            unitOfWork.DbContext.ReportGroup.RemoveRange(group.reportGroup);

                            unitOfWork.DbContext.Group.Remove(group.groupRecord);

                            unitOfWork.DbContext.SaveChanges();

                            result = 1;
                        }

                    }
                    else
                    {
                        throw new NoDataFoundException();
                    }


                    return result;
                }
                else
                {
                    throw new PermissionDeniedException();
                }   
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RoleInfo> GetRoleInfo(int permission)
        {
            logger.Info("Inside UserService/GetRoleInfo");
            List<RoleInfo> roleInfo = null;

            try
            {
                if (permission == 1)
                {
                    roleInfo = unitOfWork.DbContext.PermissionLevel.Select(pl => new RoleInfo
                    { 
                        Id=pl.Id,
                        PermissionId=pl.Permission,
                        RoleName=pl.Role
                    }).ToList();

                    return roleInfo;
                        
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public List<LoginDetail> GetAllUsers(int userId,int permission)
        {
            logger.Info("Inside UserService/GetAllUsers");
            try
            {
                if(permission > 0 && permission < 11)
                {
                    logger.Info("Getting all UserList");
                    List<LoginDetail> userInfo = new List<LoginDetail>();


                    userInfo = unitOfWork.DbContext.UserDetail.Where(ud => ud.ID != userId && ud.Name != "admin")
                               .Select(ud => new LoginDetail()
                               {
                                   UserId = ud.ID,
                                   UserName = ud.Name
                               }).ToList();
                    logger.Info("UserList Got Successfully");


                    return userInfo;
                }              
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public LoginDetail SignIn(SignInDetail signin)
        {
            logger.Info("Inside Userservice/SignIn");
            LoginDetail loginResult = new LoginDetail();
            string pwd = String.Empty;
            DateTime passwordResetDate = new DateTime();
            string salt = String.Empty;
            try
            {
          
                    var userDetail = unitOfWork.DbContext.UserDetail
                                    .Where(ud => string.Compare(ud.Email, signin.Email,true) == 0)
                                    .FirstOrDefault();

                    if (userDetail == null)
                    {
                        loginResult.Result = 7;
                        logger.Info("Invalid UserName");
                    }
                    else
                    {
                        bool validPassword = PasswordManager.IsPasswordMatch(signin.Password, userDetail.Salt, userDetail.Password);
                        if(!validPassword)
                        {
                            loginResult.Result = 8;
                            logger.Info("Invalid password");
                        }
                        else
                        {
                            passwordResetDate = unitOfWork.DbContext.PasswordResetMapping
                                                       .Where(prm => prm.UserId == userDetail.ID)
                                                       .OrderByDescending(prm => prm.PasswordResetOn)
                                                       .Select(prm => prm.PasswordResetOn).
                                                       FirstOrDefault();

                        loginResult.Permission = userDetail.PermissionLevelMapping.OrderByDescending(plm => plm.CreationDate).Select(plm => plm.PermissionLevel.Permission).FirstOrDefault();
 
                           
                            loginResult.PasswordResetOn = passwordResetDate;
                            loginResult.UserId = userDetail.ID;
                            loginResult.UserName = userDetail.Name;
                            loginResult.setByAdmin = userDetail.SetByAdmin;
                            loginResult.StatusMessage = "Valid User.Login Successful";
                            logger.Info("Valid User.Login Successful.");
                        }

                    }
                    return loginResult;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

     
        public int ForgotPassword(string userEmail)
        {
            logger.Info("Inside UserService/ForgotPassword");
            try
            {
                int result = 0;
                string password = String.Empty;
                string salt = String.Empty;
               
                    var userDetail = unitOfWork.DbContext.UserDetail
                                .Where(ud => String.Compare(ud.Email, userEmail, true) == 0).FirstOrDefault();

                    if (userDetail != null)
                    {
                        password = PasswordManager.GeneratePassword1();
                        string pwdHash = PasswordManager.GeneratePasswordHash(password, out salt);
                        userDetail.Salt = salt;
                        userDetail.Password = pwdHash;// save to db password
                        userDetail.SetByAdmin = false;
                        var resetMapping = unitOfWork.DbContext.PasswordResetMapping.Where(prs => prs.UserId == userDetail.ID).FirstOrDefault();

                        if (resetMapping != null)
                        {
                            resetMapping.PasswordResetOn = DateTime.UtcNow;
                        }
                        else
                        {
                            PasswordResetMapping passwordResetMapping = new PasswordResetMapping()
                            {
                                UserId = userDetail.ID,
                                PasswordResetOn = DateTime.UtcNow,                                     
                            };
                            unitOfWork.DbContext.PasswordResetMapping.Add(passwordResetMapping);
                        }

                        unitOfWork.DbContext.SaveChanges();
                        SendMail.SendMailForForgotPassword(userEmail, password);
                     
                        result = 1;
                        logger.Info("Record successfully inserted");
                    }
                    else
                    {
                       throw new NoDataFoundException();
                    }
                
                return result;
            }
            catch (BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        public int PushNotification(string message, string cpath,int permission)
        {
            logger.Info("Inside UserService/PushNotification");
            logger.Info("certificate path:{0}", cpath);
            int result = 0;
           
                try
                {
                    if(permission > 0 && permission < 3)
                    {

                        List<string> deviceTokens = unitOfWork.DbContext.DeviceTokenMapping.Select(dt => dt.UserDeviceToken).Distinct().ToList();

                        var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Sandbox, cpath, Util.GetCertPassword);                                                               //to do==>Password of certificate

                        // Create a new broker
                        var apnsBroker = new ApnsServiceBroker(config);

                        // Wire up events
                        apnsBroker.OnNotificationFailed += (notification, aggregateEx) => {

                            aggregateEx.Handle(ex => {
                                result = 2;
                                // See what kind of exception it was to further diagnose
                                if (ex is ApnsNotificationException notificationException)
                                {

                                    // Deal with the failed notification
                                    var apnsNotification = notificationException.Notification;
                                    var statusCode = notificationException.ErrorStatusCode;

                                    Console.WriteLine($"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}");
                                    logger.Error("Exception in User/PushNotification: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
                                    throw new NotificationNotSentException();
                                }
                                else
                                {                                                                                             //Logger 
                                                                                                                              // Inner exception might hold more useful information like an ApnsConnectionException   
                                    Console.WriteLine($"Apple Notification Failed for some unknown reason : {ex.InnerException}");
                                    logger.Error("Exception in User/PushNotification: {0} \r\n {1}", ex.ToString(), ex.StackTrace);
                                    throw new NotificationNotSentException();
                                }

                                // Mark it as handled
                                return true;
                            });
                        };

                        apnsBroker.OnNotificationSucceeded += (notification) => {
                            Console.WriteLine("Apple Notification Sent!");
                        };

                        // Start the broker
                        apnsBroker.Start();

                        foreach (var device in deviceTokens)
                        {
                            // Queue a notification to send
                            apnsBroker.QueueNotification(new ApnsNotification
                            {
                                DeviceToken = device,
                                Payload = JObject.Parse("{\"aps\":{\"alert\":\"" + message + "\",\"badge\":1,\"sound\":\"default\"}}")
                            });
                        }

                        // Stop the broker, wait for it to finish   
                        // This isn't done after every message, but after you're
                        // done with the broker
                        apnsBroker.Stop();
                        result = 1;
                        logger.Info("Notification sent successfully");
                        return result;
                    }
                    else
                    {
                        throw new PermissionDeniedException();
                    }
                }
               
                catch (UserServiceException ex)
                {

                    throw ex;
                }
                catch(BaseException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
        }

        public LoginDetail ResetPassword(ResetDetails reserDetails,int userId)
        {
            logger.Info("Inside Userservice/ResetPassword");
            LoginDetail loginResult = new LoginDetail();
            string pwd = String.Empty;
            string salt = String.Empty;
            try
            {
                
                    var userDetail = unitOfWork.DbContext.UserDetail
                                    .Where(ud => ud.ID == userId)
                                    .FirstOrDefault();
                    if (userDetail != null)
                    {
                        bool validPassword = PasswordManager.IsPasswordMatch(reserDetails.OldPassword, userDetail.Salt, userDetail.Password);

                        if (validPassword)
                        {
                            logger.Info("Valid credentials.");

                            pwd = PasswordManager.GeneratePasswordHash(reserDetails.NewPassword, out salt);
                            userDetail.Password = pwd;
                            userDetail.Salt = salt;
                            userDetail.SetByAdmin = false;
                            
                                PasswordResetMapping passwordResetMapping = new PasswordResetMapping()
                                {
                                    UserId = userDetail.ID,
                                    PasswordResetOn = DateTime.UtcNow,
                                };
                                unitOfWork.DbContext.PasswordResetMapping.Add(passwordResetMapping);
                            

                            loginResult.UserId = userDetail.ID;
                            loginResult.UserName = userDetail.Name;

                            unitOfWork.DbContext.SaveChanges();

                            loginResult.Result = 1;
                        }
                        else
                        {
                        throw new PasswordNotValidException();
                        }
                    }
                    else
                    {
                        throw new NoDataFoundException();
                    }
                
                return loginResult;
            }
            catch(UserServiceException ex)
            {
                throw ex;
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int UpdateDeviceKey(DeviceToken deviceToken, int userId,int permission)
        {
            int result;
            try
            {
                if(permission > 0 && permission < 11)
                {
                    if (deviceToken.OldDeviceKey == "")
                    {
                        
                            logger.Info("Device token is not present .. Inserting record");

                            DeviceTokenMapping deviceTokenMapping = new DeviceTokenMapping()
                            {
                                UserID = userId,
                                UserDeviceToken = deviceToken.DeviceKey
                            };

                        unitOfWork.DbContext.DeviceTokenMapping.Add(deviceTokenMapping);
                    }
                    else
                    {

                        logger.Info("Device Token present.. Updating record");

                        DeviceTokenMapping deviceTokenMapping = unitOfWork.DbContext.DeviceTokenMapping.Where(dtm => dtm.UserDeviceToken == deviceToken.OldDeviceKey).FirstOrDefault();

                        deviceTokenMapping.UserDeviceToken = deviceToken.DeviceKey;
                    }
                    unitOfWork.DbContext.SaveChanges();

                    result = 1;
                    return result;
                }
                else
                {
                    throw new PermissionDeniedException();
                }
            }
            catch(BaseException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }

}