using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using PharmaACE.ChartAudit.Models;
using System.Web;
using PharmaACE.NLP.ChartAudit;
using PharmaACE.Utility;
using NLog;
using PharmaACE.NLP.ChartAudit.NLIDB.ChartAudit;

namespace PharmaACE.ChartAudit.Utility
{
    public static class SendMail
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        public static bool SendMailForRegistration(string Email,string PasswordToMail)
        {
            logger.Info("Inside SendMail/SendMailForSignUpAdmin");
            string htmlBody = GenUtil.GetHtmlBody(Email,PasswordToMail);
            logger.Info("Successfully got htmlbody,sending mail...");
            return SendEmail("Registration Successful", htmlBody, Email); 
        }
        
            public static bool SendMailForForgotPassword(string toEmail, string password)
            {
            logger.Info("Inside SendMail/SendMailForForgotPassword");
            string htmlBody = GenUtil.GetHtmlBody(toEmail, password);
            logger.Info("Successfully got htmlbody,sending mail...");
            return SendEmail("PassWord Updated Successful", htmlBody, toEmail);

        }

            public static bool SendEmail(string subject, string body, string to, List<AlternateView> alternateViews = null, List<string> fileNames = null, List<byte[]> fileBytes = null)
            {
            logger.Info("Inside SendMail/SendMail");
            try
            {
                bool status = false;
                MailMessage mailMessage = new MailMessage(Util.mailForm, to);
                mailMessage.IsBodyHtml = true;
                if (alternateViews.AnyOrNotNull())
                {
                    foreach (var alternateView in alternateViews)
                    {
                        mailMessage.AlternateViews.Add(alternateView);
                    }
                }
                else
                    mailMessage.Body = body;
                mailMessage.Subject = subject;
                string SMTPHost = Util.SMTPMailingHost;
                int Port = Util.SMTPMailingPort.SafeToNum();
                SmtpClient smtpClient = new SmtpClient(SMTPHost, Port);
                smtpClient.Credentials = new System.Net.NetworkCredential()
                {
                    UserName = Util.mailForm,
                    Password = Util.MailSenderPassword
                };

                smtpClient.EnableSsl = true;
                try
                {
                    if (fileBytes != null)
                    {
                        Attachment attachment;
                        for (int i = 0; i < fileBytes.Count; i++)
                        {
                            Stream stream = new MemoryStream(fileBytes[i]);
                            attachment = new Attachment(stream, fileNames[i]);
                            mailMessage.Attachments.Add(attachment);
                        }
                    }

                    smtpClient.Send(mailMessage);
                    status = true;
                    logger.Info("Mail sent successfully to :{0} from :{1}",to,Util.mailForm);
                    return status;

                }
                catch (BaseException ex)
                {
                    logger.Error("Exception in SendMail {0} \r\n {1}", ex.ToString(), ex.StackTrace);
                    throw new MailNotSentException();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Exception in SendMail {0} \r\n {1}", ex.ToString(), ex.StackTrace);

                throw new MailNotSentException();
            }
            
        }

    }

    public static class Validator
    {
        public static void ValidateReportDetail(List<ReportDetail> reportDetails)
        {
            try
            {
                foreach (var report in reportDetails)
                {
                    if (report.SubcategoryId <= 0 || report.AuthorId <= 0 || report.Height <= 0 || report.Width <= 0)
                    {
                        throw new InvalidParameterException();
                    }
                }
                   
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
    }
}
