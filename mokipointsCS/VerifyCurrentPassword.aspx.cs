using System;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class VerifyCurrentPassword : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("VerifyCurrentPassword.aspx Page_Load called at " + DateTime.Now.ToString());
            try
            {
                // Check authentication
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("VerifyCurrentPassword: User not authenticated - redirecting to Login");
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                if (!IsPostBack)
                {
                    System.Diagnostics.Debug.WriteLine("VerifyCurrentPassword: Loading page");
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
                System.Diagnostics.Debug.WriteLine("VerifyCurrentPassword Page_Load error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                ShowError("An error occurred. Please try again.");
            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("VerifyCurrentPassword: Verify button clicked");
            try
            {
                lblError.Visible = false;
                lblSuccess.Visible = false;

                // Validate input
                if (string.IsNullOrEmpty(txtCurrentPassword.Text))
                {
                    ShowError("Please enter your current password.");
                    return;
                }

                // Get user info
                if (Session["UserId"] == null)
                {
                    ShowError("Session expired. Please login again.");
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                var userInfo = AuthenticationHelper.GetUserById(userId);
                
                if (userInfo == null)
                {
                    ShowError("User not found. Please login again.");
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                string email = userInfo["Email"].ToString();
                string currentPassword = txtCurrentPassword.Text;

                // Verify current password
                int authUserId = AuthenticationHelper.AuthenticateUser(email, currentPassword);
                
                if (authUserId > 0 && authUserId == userId)
                {
                    // Password is correct - set session flag and redirect to change password
                    System.Diagnostics.Debug.WriteLine("VerifyCurrentPassword: Password verified - redirecting to ChangePassword");
                    Session["CurrentPasswordVerified"] = true;
                    Session["ChangePasswordEmail"] = email;
                    Response.Redirect("ChangePassword.aspx", false);
                    return;
                }
                else
                {
                    ShowError("Current password is incorrect. Please try again.");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Verify password error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                ShowError("An error occurred while verifying your password. Please try again.");
            }
        }

        protected void btnForgotPassword_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("VerifyCurrentPassword: Forgot Password button clicked");
            try
            {
                // Get user's email from session since they're logged in
                if (Session["UserId"] != null)
                {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    var userInfo = AuthenticationHelper.GetUserById(userId);
                    
                    if (userInfo != null)
                    {
                        string email = userInfo["Email"].ToString();
                        // Set email in session for ForgotPassword page to auto-fill
                        Session["ForgotPasswordEmail"] = email;
                        System.Diagnostics.Debug.WriteLine("VerifyCurrentPassword: Setting email in session: " + email);
                    }
                }
                
                // Redirect to ForgotPassword page (will auto-fill email if available)
                Response.Redirect("ForgotPassword.aspx", false);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ForgotPassword redirect error: " + ex.Message);
                // Still redirect even if there's an error
                Response.Redirect("ForgotPassword.aspx", false);
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }
    }
}

