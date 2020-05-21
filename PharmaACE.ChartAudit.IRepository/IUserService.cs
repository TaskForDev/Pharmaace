using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using PharmaACE.ChartAudit.Models;


namespace PharmaACE.ChartAudit.IRepository
{
    public interface IUserService
    {
        List<UserInfoModel> GetUserInfo(int permission);

        UserInfoModel GetSpecificUser(int userId, int permission);

        int UpdateUser(UserInfoModel userInfo, int permission);

        int AddNewUser(List<UserInfoModel> userInfoModel,int permission);

        int DeleteUser(int userId, int permission);

        List<GroupListModel> GetGroupList(int permission);

        GroupDetailsModel GetSpecificGroup(int groupId,int permission);

        int UpdateGroupInformation(GroupDetailsModel groupDetailsModel,int permission);

        int CreateNewGroup(GroupDetailsModel groupDetailsModel,int permission);

        int DeleteSpecificGroup(int groupId, int permission);

        int PushNotification(string message, string cpath,int permission);
        
        List<LoginDetail> GetAllUsers(int userId, int permission);

        LoginDetail SignIn(SignInDetail signin);

        //LoginDetail SignInAdmin(SignInDetail signin);

        int ForgotPassword(string userEmail);

        LoginDetail ResetPassword(ResetDetails resetDetails,int userId);

        int UpdateDeviceKey(DeviceToken deviceToken, int UserId, int permission);

        List<RoleInfo> GetRoleInfo(int permission);
    }
}
