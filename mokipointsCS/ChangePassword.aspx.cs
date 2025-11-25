using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ChangePassword.aspx Page_Load called at " + DateTime.Now.ToString());
            System.Diagnostics.Debug.WriteLine("ChangePassword: IsPostBack = " + IsPostBack.ToString());
            System.Diagnostics.Debug.WriteLine("ChangePassword: Request.Form count = " + Request.Form.Count.ToString());
            System.Diagnostics.Debug.WriteLine("ChangePassword: Request.HttpMethod = " + Request.HttpMethod);
            
            try
            {
                // Log all form data on postback
                if (IsPostBack)
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: POSTBACK DETECTED");
                    System.Diagnostics.Debug.WriteLine("ChangePassword: __EVENTTARGET = " + (Request.Form["__EVENTTARGET"] ?? "NULL"));
                    System.Diagnostics.Debug.WriteLine("ChangePassword: __EVENTARGUMENT = " + (Request.Form["__EVENTARGUMENT"] ?? "NULL"));
                    
                    // Check if button was clicked
                    string buttonId = btnChangePassword != null ? btnChangePassword.UniqueID : "NULL";
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Button UniqueID = " + buttonId);
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Button in form = " + (Request.Form[buttonId] != null ? "YES" : "NO"));
                    
                    // Log password fields (length only for security)
                    if (txtNewPassword != null)
                    {
                        System.Diagnostics.Debug.WriteLine("ChangePassword: txtNewPassword length = " + (txtNewPassword.Text ?? "").Length);
                    }
                    if (txtConfirmPassword != null)
                    {
                        System.Diagnostics.Debug.WriteLine("ChangePassword: txtConfirmPassword length = " + (txtConfirmPassword.Text ?? "").Length);
                    }
                }

                // Check authentication for current password flow
                bool isCurrentPasswordFlow = Session["CurrentPasswordVerified"] != null && Convert.ToBoolean(Session["CurrentPasswordVerified"]);
                bool isOTPFlow = Session["OTPVerified"] != null && Convert.ToBoolean(Session["OTPVerified"]);
                
                System.Diagnostics.Debug.WriteLine("ChangePassword: isCurrentPasswordFlow = " + isCurrentPasswordFlow.ToString());
                System.Diagnostics.Debug.WriteLine("ChangePassword: isOTPFlow = " + isOTPFlow.ToString());

                if (!isCurrentPasswordFlow && !isOTPFlow)
                {
                    // Neither flow is active - redirect based on authentication
                    if (Session["UserId"] != null)
                    {
                        // User is logged in - redirect to verify current password
                        System.Diagnostics.Debug.WriteLine("ChangePassword: User authenticated but no verification - redirecting to VerifyCurrentPassword");
                        Response.Redirect("VerifyCurrentPassword.aspx", false);
                    }
                    else
                    {
                        // User not logged in - redirect to forgot password
                        System.Diagnostics.Debug.WriteLine("ChangePassword: User not authenticated - redirecting to ForgotPassword");
                        Response.Redirect("ForgotPassword.aspx", false);
                    }
                    return;
                }

                // Check if email is in session
                string email = null;
                if (isCurrentPasswordFlow && Session["ChangePasswordEmail"] != null)
                {
                    email = Session["ChangePasswordEmail"].ToString();
                }
                else if (isOTPFlow && Session["OTPEmail"] != null)
                {
                    email = Session["OTPEmail"].ToString();
                }
                
                System.Diagnostics.Debug.WriteLine("ChangePassword: Email from session = " + (email ?? "NULL"));

                if (string.IsNullOrEmpty(email))
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: ERROR - Email is null or empty");
                    ShowError("Session expired. Please start over.");
                    return;
                }

                if (!IsPostBack)
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: First load - clearing messages");
                    // Clear previous errors
                    if (lblError != null)
                    {
                        lblError.Visible = false;
                    }
                    if (lblSuccess != null)
                    {
                        lblSuccess.Visible = false;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Postback - Page_Load completed");
                    
                    // Log button state
                    if (btnChangePassword != null)
                    {
                        string buttonUniqueId = btnChangePassword.UniqueID;
                        bool buttonInForm = Request.Form[buttonUniqueId] != null;
                        bool eventTargetMatches = Request.Form["__EVENTTARGET"] == buttonUniqueId;
                        
                        System.Diagnostics.Debug.WriteLine("ChangePassword: Button UniqueID = " + buttonUniqueId);
                        System.Diagnostics.Debug.WriteLine("ChangePassword: Button in Request.Form = " + buttonInForm.ToString());
                        System.Diagnostics.Debug.WriteLine("ChangePassword: __EVENTTARGET matches = " + eventTargetMatches.ToString());
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ChangePassword: ERROR - btnChangePassword is NULL");
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                System.Diagnostics.Debug.WriteLine("ChangePassword: ThreadAbortException in Page_Load (redirect)");
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ChangePassword Page_Load error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("ChangePassword Page_Load stack trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                    System.Diagnostics.Debug.WriteLine("Inner stack trace: " + ex.InnerException.StackTrace);
                }
                ShowError("An error occurred. Please try again.");
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ChangePassword: btnChangePassword_Click called at " + DateTime.Now.ToString());
            System.Diagnostics.Debug.WriteLine("ChangePassword: Button click handler STARTED");
            
            try
            {
                // Clear previous errors
                if (lblError != null)
                {
                    lblError.Visible = false;
                }
                if (lblSuccess != null)
                {
                    lblSuccess.Visible = false;
                }

                System.Diagnostics.Debug.WriteLine("ChangePassword: Validating form inputs");
                
                // Validate form
                if (txtNewPassword == null)
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: ERROR - txtNewPassword is NULL");
                    ShowError("Form error: Password field not found.");
                    return;
                }
                
                if (txtConfirmPassword == null)
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: ERROR - txtConfirmPassword is NULL");
                    ShowError("Form error: Confirm password field not found.");
                    return;
                }

                string newPassword = txtNewPassword.Text ?? "";
                string confirmPassword = txtConfirmPassword.Text ?? "";
                
                System.Diagnostics.Debug.WriteLine("ChangePassword: New password length = " + newPassword.Length);
                System.Diagnostics.Debug.WriteLine("ChangePassword: Confirm password length = " + confirmPassword.Length);

                if (string.IsNullOrEmpty(newPassword))
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Validation failed - New password is empty");
                    ShowError("New password is required.");
                    return;
                }

                if (newPassword.Length < 6)
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Validation failed - Password too short");
                    ShowError("Password must be at least 6 characters long.");
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Validation failed - Passwords do not match");
                    ShowError("Passwords do not match.");
                    return;
                }
                
                System.Diagnostics.Debug.WriteLine("ChangePassword: Form validation passed");

                // Get email from session (check both flows)
                string email = null;
                bool isCurrentPasswordFlow = Session["CurrentPasswordVerified"] != null && Convert.ToBoolean(Session["CurrentPasswordVerified"]);
                
                System.Diagnostics.Debug.WriteLine("ChangePassword: Checking session for email");
                System.Diagnostics.Debug.WriteLine("ChangePassword: isCurrentPasswordFlow = " + isCurrentPasswordFlow.ToString());
                
                if (isCurrentPasswordFlow && Session["ChangePasswordEmail"] != null)
                {
                    email = Session["ChangePasswordEmail"].ToString();
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Email from ChangePasswordEmail session = " + email);
                }
                else if (Session["OTPEmail"] != null)
                {
                    email = Session["OTPEmail"].ToString();
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Email from OTPEmail session = " + email);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: ERROR - No email found in session");
                    // Log session keys
                    var sessionKeysList = new System.Collections.Generic.List<string>();
                    foreach (string key in Session.Keys)
                    {
                        sessionKeysList.Add(key);
                    }
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Session keys: " + string.Join(", ", sessionKeysList.ToArray()));
                    ShowError("Session expired. Please start over.");
                    return;
                }

                if (string.IsNullOrEmpty(email))
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: ERROR - Email is null or empty after retrieval");
                    ShowError("Session expired. Please start over.");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("ChangePassword: Calling UpdatePassword for email: " + email);
                
                // Update password in database
                bool updateResult = UpdatePassword(email, newPassword);
                System.Diagnostics.Debug.WriteLine("ChangePassword: UpdatePassword returned: " + updateResult.ToString());
                
                if (updateResult)
                {
                    // Clear session variables
                    if (isCurrentPasswordFlow)
                    {
                        Session.Remove("CurrentPasswordVerified");
                        Session.Remove("ChangePasswordEmail");
                    }
                    else
                    {
                        Session.Remove("OTPEmail");
                        Session.Remove("OTPPurpose");
                        Session.Remove("OTPVerified");
                    }

                    // Show success message
                    ShowSuccess("Password changed successfully!");
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Password changed successfully for email: " + email);

                    // If user is already logged in (current password flow), keep them logged in
                    if (isCurrentPasswordFlow && Session["UserId"] != null)
                    {
                        // Redirect to profile after 2 seconds
                        System.Diagnostics.Debug.WriteLine("ChangePassword: Password changed - redirecting to Profile");
                        Response.AddHeader("Refresh", "2;url=Profile.aspx?message=PasswordChanged");
                        return;
                    }
                    else
                    {
                        // For OTP flow (forgot password), show success message and redirect to login
                        System.Diagnostics.Debug.WriteLine("ChangePassword: Password changed via OTP flow - redirecting to Login");
                        // Use JavaScript to show message and redirect after a delay
                        string script = @"
                            setTimeout(function() {
                                window.location.href = 'Login.aspx?message=PasswordChanged';
                            }, 2000);
                        ";
                        ClientScript.RegisterStartupScript(this.GetType(), "RedirectToLogin", script, true);
                        return;
                    }
                }
                else
                {
                    ShowError("Failed to update password. Please try again.");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                System.Diagnostics.Debug.WriteLine("ChangePassword: ThreadAbortException in btnChangePassword_Click (redirect)");
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ChangePassword: btnChangePassword_Click ERROR: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("ChangePassword: btnChangePassword_Click stack trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Inner exception: " + ex.InnerException.Message);
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Inner stack trace: " + ex.InnerException.StackTrace);
                }
                ShowError("An error occurred while changing your password. Please try again.");
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("ChangePassword: btnChangePassword_Click completed");
            }
        }

        private bool UpdatePassword(string email, string newPassword)
        {
            System.Diagnostics.Debug.WriteLine("ChangePassword: UpdatePassword called for email: " + email);
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: UpdatePassword ERROR - Email is null or empty");
                    return false;
                }
                
                if (string.IsNullOrEmpty(newPassword))
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: UpdatePassword ERROR - New password is null or empty");
                    return false;
                }
                
                System.Diagnostics.Debug.WriteLine("ChangePassword: Hashing password...");
                // Hash the new password
                string passwordHash = PasswordHelper.HashPassword(newPassword);
                System.Diagnostics.Debug.WriteLine("ChangePassword: Password hashed successfully (length: " + (passwordHash ?? "").Length + ")");

                // Update password in database
                string query = @"
                    UPDATE [dbo].[Users]
                    SET Password = @Password
                    WHERE Email = @Email AND IsActive = 1";

                System.Diagnostics.Debug.WriteLine("ChangePassword: Executing database update query...");
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@Password", passwordHash),
                    new SqlParameter("@Email", email));

                System.Diagnostics.Debug.WriteLine("ChangePassword: Database update completed. Rows affected: " + rowsAffected.ToString());

                if (rowsAffected > 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ChangePassword: Password updated successfully for email: {0}", email));
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ChangePassword: No user found or password update failed for email: {0}", email));
                    System.Diagnostics.Debug.WriteLine("ChangePassword: Checking if user exists in database...");
                    
                    // Check if user exists
                    try
                    {
                        string checkQuery = "SELECT COUNT(*) FROM [dbo].[Users] WHERE Email = @Email";
                        object userCount = DatabaseHelper.ExecuteScalar(checkQuery, new SqlParameter("@Email", email));
                        System.Diagnostics.Debug.WriteLine("ChangePassword: User count for email: " + (userCount ?? "NULL"));
                    }
                    catch (Exception checkEx)
                    {
                        System.Diagnostics.Debug.WriteLine("ChangePassword: Error checking user existence: " + checkEx.Message);
                    }
                    
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ChangePassword: UpdatePassword ERROR: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("ChangePassword: UpdatePassword stack trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("ChangePassword: UpdatePassword inner exception: " + ex.InnerException.Message);
                }
                return false;
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
            lblSuccess.Visible = false;
        }

        private void ShowSuccess(string message)
        {
            lblSuccess.Text = message;
            lblSuccess.Visible = true;
            lblError.Visible = false;
        }
    }
}

