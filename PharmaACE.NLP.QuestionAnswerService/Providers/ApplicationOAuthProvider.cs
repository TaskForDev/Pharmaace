using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using PharmaACE.ChartAudit.Models;
using PharmaACE.ChartAudit.Reporting.EntityProvider;
using PharmaACE.ChartAudit.Repository;


namespace PharmaACE.NLP.QuestionAnswerService.Providers
{
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {


        private readonly string _publicClientId;

        public ApplicationOAuthProvider(string publicClientId)
        {
            if (publicClientId == null)
            {
                throw new ArgumentNullException("publicClientId");
            }

            _publicClientId = publicClientId;
        }


        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            var userManager = context.OwinContext.GetUserManager<ApplicationUserManager>();

            var form = await context.Request.ReadFormAsync();
            string passwordResetOn = form["PasswordResetOn"];
            //ApplicationUser user = await userManager.FindAsync(context.UserName, context.Password);

            var loginResult = AuthenticateUser(context, passwordResetOn);
            
                if (loginResult.Result == 7)
                {
                    context.SetError( loginResult.Result.ToString(), "Invalid UserName");
                    return;
                }
                else if(loginResult.Result == 8)
                {
                    context.SetError(loginResult.Result.ToString(), "Invalid Password");
                    return;
                }
                else if(loginResult.Result == 14)
                {
                context.SetError(loginResult.Result.ToString(), "Password Reset");
                return;
            }

                //ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(userManager,
                //   OAuthDefaults.AuthenticationType);
                //ClaimsIdentity cookiesIdentity = await user.GenerateUserIdentityAsync(userManager,
                //    CookieAuthenticationDefaults.AuthenticationType);

                //AuthenticationProperties properties = CreateProperties(user.UserName);
                //AuthenticationTicket ticket = new AuthenticationTicket(oAuthIdentity, properties);
                //context.Validated(ticket);
                //context.Request.Context.Authentication.SignIn(cookiesIdentity);

                ClaimsIdentity oAuthIdentity =
                new ClaimsIdentity(context.Options.AuthenticationType);

                var props = GetProperties(loginResult);
                AuthenticationProperties properties = new AuthenticationProperties(props);
                foreach (var prop in props)
                {
                    oAuthIdentity.AddClaim(new Claim(prop.Key, prop.Value));
                }
                AuthenticationTicket ticket =
                new AuthenticationTicket(oAuthIdentity, properties);
                context.Validated(ticket);
                context.Request.Context.Authentication.SignIn(oAuthIdentity);

            
        }

        private IDictionary<string, string> GetProperties(LoginDetail user)
        {
           
            string pwd = String.Empty;
           
                return new Dictionary<string, string>
            {
                 { ClaimTypes.NameIdentifier, user.UserId.ToString() },
                 { ClaimTypes.Name, user.UserName },
                //{"UserID",user.UserId.ToString()},
                //{"UserName",user.UserName.ToString()},
                {"PasswordResetOn",user.PasswordResetOn.ToString()},
                {"Permission",user.Permission.ToString() },
                {"Result",user.Result.ToString()},
                {"setByAdmin",user.setByAdmin.ToString() }
            };
                    
        }
        
        private LoginDetail AuthenticateUser(OAuthGrantResourceOwnerCredentialsContext context,string passwordResetOn)
        {
            //string conStr = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ToString();
           // var sql = "select * from userDetail where email = @userName and password = @pwd";
            try
            {
                //using (SqlConnection conn = new SqlConnection(conStr))
                //{
                //using (SqlCommand cmd = new SqlCommand(sql, conn))
                //{
                //    cmd.CommandType = CommandType.Text;
                //    cmd.Parameters.AddWithValue("userName", context.UserName);
                //    //var md5Hash = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.Default.GetBytes(context.Password))).Replace("-", "");


                //    cmd.Parameters.AddWithValue("pwd", pwd);

                //    conn.Open();
                //    using (SqlDataReader reader = cmd.ExecuteReader())
                //    {
                //        while (reader.Read())
                //        {
                //        user = new ApplicationUser { UserName = reader["user_name"]?.ToString() };
                //        }
                //    }
                //}
                // }
                IUnitOfWork unitOfWork = new UnitOfWork();
                UserService userService = new UserService(unitOfWork);
                SignInDetail signin= new SignInDetail();
                LoginDetail loginResult = new LoginDetail();
                signin.Email = context.UserName;
                signin.Password = context.Password;
                signin.PasswordResetOn = passwordResetOn;               
                loginResult = userService.SignIn(signin);
                
                return loginResult;
              
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }

        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            // Resource owner password credentials does not provide a client ID.
            if (context.ClientId == null)
            {
                context.Validated();
            }

            return Task.FromResult<object>(null);
        }

        public override Task ValidateClientRedirectUri(OAuthValidateClientRedirectUriContext context)
        {
            if (context.ClientId == _publicClientId)
            {
                Uri expectedRootUri = new Uri(context.Request.Uri, "/");

                if (expectedRootUri.AbsoluteUri == context.RedirectUri)
                {
                    context.Validated();
                }
            }

            return Task.FromResult<object>(null);
        }
        
    }
}