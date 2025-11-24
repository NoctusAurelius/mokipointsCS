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
            try
            {
                // Check authentication for current password flow
                bool isCurrentPasswordFlow = Session["CurrentPasswordVerified"] != null && Convert.ToBoolean(Session["CurrentPasswordVerified"]);
                bool isOTPFlow = Session["OTPVerified"] != null && Convert.ToBoolean(Session["OTPVerified"]);

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

                if (string.IsNullOrEmpty(email))
                {
                    ShowError("Session expired. Please start over.");
                    return;
                }

                if (!IsPostBack)
                {
                    // Clear previous errors
                    lblError.Visible = false;
                    lblSuccess.Visible = false;
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ChangePassword Page_Load error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                ShowError("An error occurred. Please try again.");
            }
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear previous errors
                lblError.Visible = false;

                // Validate form
                if (string.IsNullOrEmpty(txtNewPassword.Text))
                {
                    ShowError("New password is required.");
                    return;
                }

                if (txtNewPassword.Text.Length < 6)
                {
                    ShowError("Password must be at least 6 characters long.");
                    return;
                }

                if (txtNewPassword.Text != txtConfirmPassword.Text)
                {
                    ShowError("Passwords do not match.");
                    return;
                }

                // Get email from session (check both flows)
                string email = null;
                bool isCurrentPasswordFlow = Session["CurrentPasswordVerified"] != null && Convert.ToBoolean(Session["CurrentPasswordVerified"]);
                
                if (isCurrentPasswordFlow && Session["ChangePasswordEmail"] != null)
                {
                    email = Session["ChangePasswordEmail"].ToString();
                }
                else if (Session["OTPEmail"] != null)
                {
                    email = Session["OTPEmail"].ToString();
                }
                else
                {
                    ShowError("Session expired. Please start over.");
                    return;
                }

                string newPassword = txtNewPassword.Text;

                // Update password in database
                if (UpdatePassword(email, newPassword))
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
                        // Try to authenticate and login (OTP flow)
                        int userId = AuthenticationHelper.AuthenticateUser(email, newPassword);
                        if (userId > 0)
                        {
                            // Get user info
                            var userInfo = AuthenticationHelper.GetUserById(userId);
                            if (userInfo != null)
                            {
                                Session["UserId"] = userId;
                                Session["UserEmail"] = email;
                                Session["UserName"] = userInfo["FirstName"].ToString() + " " + userInfo["LastName"].ToString();
                                Session["UserRole"] = userInfo["Role"].ToString();
                            }

                            // Redirect to dashboard
                            Response.Redirect("Dashboard.aspx", false);
                        }
                        else
                        {
                            // Redirect to login
                            Response.Redirect("Login.aspx?message=PasswordChanged", false);
                        }
                    }
                }
                else
                {
                    ShowError("Failed to update password. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Change password error: " + ex.Message);
                ShowError("An error occurred while changing your password. Please try again.");
            }
        }

        private bool UpdatePassword(string email, string newPassword)
        {
            try
            {
                // Hash the new password
                string passwordHash = PasswordHelper.HashPassword(newPassword);

                // Update password in database
                string query = @"
                    UPDATE [dbo].[Users]
                    SET Password = @Password
                    WHERE Email = @Email AND IsActive = 1";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@Password", passwordHash),
                    new SqlParameter("@Email", email));

                if (rowsAffected > 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Password updated successfully for email: {0}", email));
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("No user found or password update failed for email: {0}", email));
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Update password error: " + ex.Message);
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

