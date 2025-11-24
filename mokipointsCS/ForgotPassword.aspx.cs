using System;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class ForgotPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ForgotPassword.aspx Page_Load called at " + DateTime.Now.ToString());
            System.Diagnostics.Debug.WriteLine("IsPostBack: " + IsPostBack.ToString());
            System.Diagnostics.Debug.WriteLine("Request.Form count: " + Request.Form.Count);
            
            try
            {
                if (!IsPostBack)
                {
                    // Clear previous messages
                    lblMessage.Visible = false;
                    
                    // Auto-fill email if user came from VerifyCurrentPassword page (logged in user)
                    if (Session["ForgotPasswordEmail"] != null)
                    {
                        string email = Session["ForgotPasswordEmail"].ToString();
                        txtEmail.Text = email;
                        System.Diagnostics.Debug.WriteLine("ForgotPassword: Auto-filled email from session: " + email);
                        // Clear the session variable after using it
                        Session.Remove("ForgotPasswordEmail");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ForgotPassword Page_Load error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
            }
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnSubmit_Click called at " + DateTime.Now.ToString());
            try
            {
                // Clear previous messages
                lblMessage.Visible = false;

                string email = txtEmail.Text.Trim().ToLower();
                System.Diagnostics.Debug.WriteLine("ForgotPassword attempt for email: " + email);

                // Validate email input
                if (string.IsNullOrEmpty(email))
                {
                    System.Diagnostics.Debug.WriteLine("ForgotPassword validation failed: Email is empty");
                    ShowError("Please enter your email address.");
                    return;
                }

                // Validate email format
                if (!IsValidEmail(email))
                {
                    System.Diagnostics.Debug.WriteLine("ForgotPassword validation failed: Invalid email format");
                    ShowError("Please enter a valid email address.");
                    return;
                }

                // Validate email exists in database and is active
                System.Diagnostics.Debug.WriteLine("Checking if email exists in database...");
                bool emailExists = ValidateEmailInDatabase(email);
                System.Diagnostics.Debug.WriteLine("Email exists in database: " + emailExists.ToString());
                
                if (!emailExists)
                {
                    // Don't reveal if email exists for security
                    System.Diagnostics.Debug.WriteLine("Email not found - showing generic success message");
                    ShowSuccess("If this email is registered, a verification code has been sent to your email.");
                    return;
                }

                // Generate and send OTP
                System.Diagnostics.Debug.WriteLine("Generating OTP...");
                string otpCode = OTPHelper.CreateOTP(email, "ForgotPassword");
                if (!string.IsNullOrEmpty(otpCode))
                {
                    System.Diagnostics.Debug.WriteLine("OTP generated successfully");
                    string emailBody = EmailHelper.GetOTPEmailTemplate(otpCode, "ForgotPassword");
                    string subject = "MOKI POINTS - Password Reset Verification Code";
                    
                    System.Diagnostics.Debug.WriteLine("Sending email...");
                    if (EmailHelper.SendEmail(email, subject, emailBody))
                    {
                        System.Diagnostics.Debug.WriteLine("Email sent successfully - redirecting to OTP page");
                        // Set OTP session variables
                        Session["OTPEmail"] = email;
                        Session["OTPPurpose"] = "ForgotPassword";

                        // Redirect to OTP page
                        Response.Redirect("OTP.aspx");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to send email");
                        ShowError("Failed to send verification email. Please try again later.");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to generate OTP");
                    ShowError("Failed to generate verification code. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Forgot password error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowError("An error occurred. Please try again later.");
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                return regex.IsMatch(email);
            }
            catch
            {
                return false;
            }
        }

        private bool ValidateEmailInDatabase(string email)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM [dbo].[Users] 
                    WHERE Email = @Email AND IsActive = 1";

                object count = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@Email", email));
                
                return Convert.ToInt32(count) > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database validation error: " + ex.Message);
                return false;
            }
        }

        private void ShowError(string message)
        {
            System.Diagnostics.Debug.WriteLine("ShowError called with message: " + message);
            try
            {
                if (lblMessage != null)
                {
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message message-error";
                    lblMessage.Visible = true;
                    System.Diagnostics.Debug.WriteLine("Error message label updated successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: lblMessage is null!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ShowError exception: " + ex.Message);
            }
        }

        private void ShowSuccess(string message)
        {
            System.Diagnostics.Debug.WriteLine("ShowSuccess called with message: " + message);
            try
            {
                if (lblMessage != null)
                {
                    lblMessage.Text = message;
                    lblMessage.CssClass = "message message-success";
                    lblMessage.Visible = true;
                    System.Diagnostics.Debug.WriteLine("Success message label updated successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: lblMessage is null!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ShowSuccess exception: " + ex.Message);
            }
        }
    }
}

