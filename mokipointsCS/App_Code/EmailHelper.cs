using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace mokipointsCS
{
    /// <summary>
    /// Helper class for sending emails via SMTP
    /// </summary>
    public class EmailHelper
    {
        private static string SmtpHost
        {
            get 
            { 
                string host = ConfigurationManager.AppSettings["SMTP_HOST"];
                return host != null ? host : "smtp.gmail.com";
            }
        }

        private static int SmtpPort
        {
            get
            {
                int port = 587;
                int.TryParse(ConfigurationManager.AppSettings["SMTP_PORT"], out port);
                return port;
            }
        }

        private static string SmtpUser
        {
            get 
            { 
                string user = ConfigurationManager.AppSettings["SMTP_USER"];
                return user != null ? user : "";
            }
        }

        private static string SmtpPassword
        {
            get 
            { 
                string password = ConfigurationManager.AppSettings["SMTP_PASSWORD"];
                return password != null ? password : "";
            }
        }

        private static string SmtpFrom
        {
            get 
            { 
                string from = ConfigurationManager.AppSettings["SMTP_FROM"];
                return from != null ? from : SmtpUser;
            }
        }

        private static bool SmtpSecure
        {
            get
            {
                bool secure = true;
                bool.TryParse(ConfigurationManager.AppSettings["SMTP_SECURE"], out secure);
                return secure;
            }
        }

        /// <summary>
        /// Sends an email using SMTP
        /// </summary>
        public static bool SendEmail(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                using (SmtpClient client = new SmtpClient(SmtpHost, SmtpPort))
                {
                    client.EnableSsl = SmtpSecure;
                    client.Credentials = new NetworkCredential(SmtpUser, SmtpPassword);

                    using (MailMessage message = new MailMessage())
                    {
                        message.From = new MailAddress(SmtpFrom, "MOKI POINTS");
                        message.To.Add(new MailAddress(to));
                        message.Subject = subject;
                        message.Body = body;
                        message.IsBodyHtml = isHtml;
                        message.BodyEncoding = Encoding.UTF8;
                        message.SubjectEncoding = Encoding.UTF8;

                        client.Send(message);
                    }
                }

                LogEmailSent(to, subject, true, null);
                return true;
            }
            catch (Exception ex)
            {
                LogEmailSent(to, subject, false, ex.Message);
                System.Diagnostics.Debug.WriteLine("Email sending error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Generates HTML email template for OTP code
        /// </summary>
        public static string GetOTPEmailTemplate(string otpCode, string purpose)
        {
            string purposeText = purpose == "Registration" ? "complete your registration" : "reset your password";
            
            return string.Format(@"
<!DOCTYPE html>
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <meta charset=""utf-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>MOKI POINTS - OTP Code</title>
</head>
<body style=""margin: 0; padding: 0; font-family: Arial, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" style=""width: 100%; border-collapse: collapse; background-color: #f5f5f5; padding: 20px;"">
        <tr>
            <td align=""center"" style=""padding: 20px 0;"">
                <table role=""presentation"" style=""max-width: 600px; width: 100%; border-collapse: collapse; background-color: white; border-radius: 10px; box-shadow: 0 4px 20px rgba(0,0,0,0.1);"">
                    <!-- Header -->
                    <tr>
                        <td style=""background: linear-gradient(135deg, #0066CC 0%, #FF6600 100%); padding: 40px 30px; text-align: center; border-radius: 10px 10px 0 0;"">
                            <h1 style=""margin: 0; color: white; font-size: 36px; letter-spacing: 4px;"">
                                <span style=""color: white;"">MOKI</span> <span style=""color: #FFF9E6;"">POINTS</span>
                            </h1>
                        </td>
                    </tr>
                    
                    <!-- Content -->
                    <tr>
                        <td style=""padding: 40px 30px;"">
                            <h2 style=""color: #333; margin: 0 0 20px 0; font-size: 24px;"">Verification Code</h2>
                            <p style=""color: #666; font-size: 16px; line-height: 1.6; margin: 0 0 30px 0;"">
                                Hello,
                            </p>
                            <p style=""color: #666; font-size: 16px; line-height: 1.6; margin: 0 0 30px 0;"">
                                Please use the following verification code to {0}:
                            </p>
                            
                            <!-- OTP Code Box -->
                            <table role=""presentation"" style=""width: 100%; margin: 30px 0;"">
                                <tr>
                                    <td align=""center"">
                                        <div style=""background: linear-gradient(135deg, #0066CC 0%, #FF6600 50%, #FFB6C1 100%); padding: 3px; border-radius: 10px; display: inline-block;"">
                                            <div style=""background-color: white; padding: 20px 40px; border-radius: 8px;"">
                                                <span style=""font-size: 48px; font-weight: bold; letter-spacing: 8px; color: #0066CC; font-family: 'Courier New', monospace;"">{1}</span>
                                            </div>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            
                            <p style=""color: #666; font-size: 14px; line-height: 1.6; margin: 30px 0 0 0;"">
                                This code will expire in 10 minutes. If you didn't request this code, please ignore this email.
                            </p>
                        </td>
                    </tr>
                    
                    <!-- Footer -->
                    <tr>
                        <td style=""background-color: #FFF9E6; padding: 30px; text-align: center; border-radius: 0 0 10px 10px; border-top: 1px solid #e0e0e0;"">
                            <p style=""color: #666; font-size: 14px; margin: 0 0 10px 0;"">
                                <strong>MOKI POINTS</strong> - Your Family Chore & Point System
                            </p>
                            <p style=""color: #999; font-size: 12px; margin: 0;"">
                                © 2024 MOKI POINTS. All rights reserved.
                            </p>
                            <p style=""color: #999; font-size: 12px; margin: 10px 0 0 0;"">
                                Contact: <a href=""mailto:info.mokipoin@gmail.com"" style=""color: #0066CC; text-decoration: none;"">info.mokipoin@gmail.com</a>
                            </p>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
</body>
</html>", purposeText, otpCode);
        }

        /// <summary>
        /// Logs email sending attempts
        /// </summary>
        private static void LogEmailSent(string to, string subject, bool success, string errorMessage)
        {
            try
            {
                string logMessage = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] Email to: {1}, Subject: {2}, Success: {3}", 
                    DateTime.Now, to, subject, success);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    logMessage += ", Error: " + errorMessage;
                }
                System.Diagnostics.Debug.WriteLine(logMessage);
            }
            catch
            {
                // Ignore logging errors
            }
        }
    }
}

