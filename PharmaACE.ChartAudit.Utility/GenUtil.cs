using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NLog;
using PharmaACE.ChartAudit.Models;

namespace PharmaACE.ChartAudit.Utility
{

    public static class GenUtil
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        //public static bool SendEmail(string subject, string body, string to, List<string> fileNames = null, List<byte[]> fileBytes = null)
        //{
        //    bool status = false;

        //    MailMessage mailMessage = new MailMessage("tech.support@pharmaace.com", to);
        //    mailMessage.IsBodyHtml = true;
        //    mailMessage.Body = body;
        //    mailMessage.Subject = subject;
        //    string SMTPHost = "smtp.office365.com";
        //    int Port = 587;
        //    System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(SMTPHost, Port);
        //    smtpClient.Credentials = new System.Net.NetworkCredential()
        //    {
        //        UserName = "tech.support@pharmaace.com",
        //        Password = "Ph@rm@#2015"
        //    };

        //    smtpClient.EnableSsl = true;
        //    try
        //    {
        //        if (fileBytes != null)
        //        {
        //            System.Net.Mail.Attachment attachment;
        //            for (int i = 0; i < fileBytes.Count; i++)
        //            {
        //                Stream stream = new MemoryStream(fileBytes[i]);
        //                attachment = new System.Net.Mail.Attachment(stream, fileNames[i]);
        //                mailMessage.Attachments.Add(attachment);
        //            }
        //        }

        //        smtpClient.Send(mailMessage);
        //        status = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;

        //    }
        //    return status;

        //}
        public static string GetHtmlBody(string Email,string PasswordToMail)
        {
            logger.Info("Inside GenUtil/GetHtmlBody");
            try
            {
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~/Content/Images");
                //var vb = File.ReadAllBytes(path + "\\logo.png");
               // var imageString = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(vb));

                StringBuilder sbEmailBody = new StringBuilder();
                sbEmailBody.Append("<table style =\"color:#535353;font-family:arial,helvetica,sans-serif;font-size:12px;width:100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                sbEmailBody.Append("<tbody>");
                sbEmailBody.Append("<tr>");
                sbEmailBody.Append("<td style=\"padding:30px 0 100px 0;text-align:center\" align=\"center\" bgcolor=\"#f0f0f0\">");
                sbEmailBody.Append("<table  style=\"margin:0 auto;width:600px\" align=\"center\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                sbEmailBody.Append("<tbody>");
                sbEmailBody.Append("<tr>");
                sbEmailBody.Append("<td style=\"padding: 20px 0 20px 0; border - bottom:1px dotted #b8b8b8\" bgcolor=\"#ffffff\">");
                sbEmailBody.Append("<table style=\"width:100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                sbEmailBody.Append("<tbody>");
                sbEmailBody.Append("<tr>");
                sbEmailBody.Append("<td style=\"width:420px;text-align:left;padding-left:20px\" align=\"left\" width=\"420\" bgcolor=\"#ffffff\"><a href=\"\" target =\"\" ><img  src =\"\"border = \"0\" ></a></td>");
                sbEmailBody.Append("<td style=\"width:180px;text-align:right;color:#003049;font-size:12px;font-family:Cambria,\"Hoefler Text\",\"Liberation Serif\",Times,\"Times New Roman\",serif;padding-right:20px\" align=\"right\" width=\"180\" valign=\"top\"></td>");
                sbEmailBody.Append("</tr>");
                sbEmailBody.Append("</tbody>");
                sbEmailBody.Append("</table>");
                sbEmailBody.Append("</td>");
                sbEmailBody.Append("</tr>");
                sbEmailBody.Append("</tbody>");
                sbEmailBody.Append("</td></tr></tbody>");
                sbEmailBody.Append("</table>");
                sbEmailBody.Append("<table style=\"margin:0 auto;width:600px\" align=\"center\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                sbEmailBody.Append("<tbody>");
                sbEmailBody.Append("<tr>");
                sbEmailBody.Append("<td style=\"padding:30px\" valign=\"top\" bgcolor=\"#ffffff\">");
                sbEmailBody.Append("<table style=\"width:100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                sbEmailBody.Append("<tbody>");
                sbEmailBody.Append("<tr>");
                sbEmailBody.Append("<td style=\"padding:0;text-align:left;color:#3c3c3b;font-size:12px;font-family:arial,helvetica,sans-serif;line-height:16px\">");
                sbEmailBody.Append("<p>");
                sbEmailBody.Append("<b style =\"color:#2a7e80;font-size:16px\"> Thank you for registering with PharmaACE</b></p> <p><b>Your Credentials for login </b></p>");
                sbEmailBody.Append("<table><tr><th></th><th></th></tr><tr>");
                sbEmailBody.Append("<td><p> Email :" + Email + " </p></td>");
                sbEmailBody.Append("</tr>");
                sbEmailBody.Append("<tr>");
                sbEmailBody.Append("<td><p> Password : " + PasswordToMail + " </p></td>");
                sbEmailBody.Append("</tr></table>");
                sbEmailBody.Append("<p>");
                sbEmailBody.Append("To reset your password please click on the link below </p>");
                sbEmailBody.Append("<a href=\"http://caivaportal.azurewebsites.net/login\" target=\"_blank\">");
                sbEmailBody.Append("caivaportal.azurewebsites.net");
                sbEmailBody.Append("<p>If any of the above is incorrect, please contact us on");
                sbEmailBody.Append("<a href=\"mail: tech.support @pharmaace.com\" value=\"tech.support @pharmaace.com\" target=\"_blank\">");
                sbEmailBody.Append("tech.support@pharmaace.com");
                sbEmailBody.Append("</a>");
                sbEmailBody.Append("</p>");
                sbEmailBody.Append("<table style=\"border-collapse:collapse;min-width:100%;width:100%\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                sbEmailBody.Append("<tbody>");
                sbEmailBody.Append("<tr>");
                sbEmailBody.Append("<td style=\"padding:0\" align=\"center\" valign=\"top\"></td>");
                sbEmailBody.Append("</tr>");
                sbEmailBody.Append("</tbody>");
                sbEmailBody.Append("</table>");
                sbEmailBody.Append("</td>");
                sbEmailBody.Append("</tr>");
                sbEmailBody.Append("</tbody>");
                sbEmailBody.Append("</table>");
                sbEmailBody.Append("</td>");
                sbEmailBody.Append("</tr>");
                sbEmailBody.Append("</tbody>");
                sbEmailBody.Append("</table>");
                sbEmailBody.Append("<table  style=\"margin:0 auto;width:600px\" align=\"center\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                sbEmailBody.Append("<tbody>");
                sbEmailBody.Append("<tr>");
                sbEmailBody.Append("<td width=\"617\" bgcolor=\"#003049\">");
                sbEmailBody.Append("<table style=\"width:600px\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                sbEmailBody.Append("<tbody>");
                sbEmailBody.Append("<tr>");
                sbEmailBody.Append("<td style=\"text-align:left;width:420px;margin:0 auto;font-family:arial,helvetica,sans-serif;font-size:12px;color:#ffffff;padding:20px\" align=\"center\" width=\"420\" valign=\"top\" bgcolor=\"#003049\">Visit our website");
                sbEmailBody.Append("<a href=\"http://support.pacehomepage.com\" style=\"color:#ffffff\" target=\"_blank\">");
                sbEmailBody.Append("</a>");
                sbEmailBody.Append("<br>");
                sbEmailBody.Append("</td>");
                sbEmailBody.Append("<td style=\"text-align:center;width:180px;margin:0 auto;font-family:arial,helvetica,sans-serif;font-size:11px;color:#ffffff\" align=\"center\" width=\"180\" bgcolor=\"#003049\">");
                sbEmailBody.Append("<table style=\"width:30%\" align=\"center\" border=\"0\">");
                sbEmailBody.Append("<tbody><tr></tr>");
                sbEmailBody.Append("</tbody>");
                sbEmailBody.Append("</table>");
                sbEmailBody.Append("</td>");
                sbEmailBody.Append("</tr>");
                sbEmailBody.Append("</tbody>");
                sbEmailBody.Append("</table>");
                sbEmailBody.Append("</td>");
                sbEmailBody.Append("</tr>");
                sbEmailBody.Append("</tbody>");
                sbEmailBody.Append("</table>");
                sbEmailBody.Append("</td>");
                sbEmailBody.Append("</tr>");
                sbEmailBody.Append("</tbody>");
                sbEmailBody.Append("</table>");

                logger.Info("Returning Html Body for mail");
                return sbEmailBody.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            
        }
    }
}
