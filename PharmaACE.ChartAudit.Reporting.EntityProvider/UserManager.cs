using System.Configuration;

namespace PharmaACE.ChartAudit.Reporting.EntityProvider
{
    public class GetUsers
    {
        public static string ConnectionStringBuilder(string dbServer, string dataBase, string dbUser, string dbPassword)
        {
            return "Data Source=" + dbServer + ";Initial Catalog=" + dataBase + ";Persist Security Info=True;User ID=" + dbUser + ";Password=" + dbPassword + ";";

        }
        public static string MasterModelConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["PaceMasterConnectionString"].ConnectionString;
            }
        }

        //public List<UserDetails> GetAllUsers()
        //{
           
        //    try
        //    {
        //        MasterModel MasterContext = new MasterModel(ConnectionStringBuilder);
        //       List<UserDetails> users = MasterContext.user_details.Select(u=>new UserDetails
        //       {
        //           user_details_id = u.user_details_id,
        //           user_name = u.user_name
        //       }).ToList();

        //        return users;

        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //    return null;
        //}


    }
}
