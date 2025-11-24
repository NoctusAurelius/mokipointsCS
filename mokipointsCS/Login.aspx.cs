using System;
using System.Web.UI;
using System.Text.RegularExpressions;

namespace mokipointsCS
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Login.aspx Page_Load called at " + DateTime.Now.ToString());
            System.Diagnostics.Debug.WriteLine("IsPostBack: " + IsPostBack.ToString());
            System.Diagnostics.Debug.WriteLine("Request.Form count: " + Request.Form.Count);
            
            try
            {
                // Clear error message on page load
                if (lblError != null)
                {
                    lblError.Visible = false;
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash
                System.Diagnostics.Debug.WriteLine("Page_Load error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnLogin_Click called at " + DateTime.Now.ToString());
            try
            {
                // Clear previous errors
                lblError.Visible = false;
                lblError.Text = string.Empty;

                // Get and validate input
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text;
                System.Diagnostics.Debug.WriteLine("Login attempt for email: " + email);

                // Client-side validation should catch these, but server-side validation as backup
                if (string.IsNullOrEmpty(email))
                {
                    System.Diagnostics.Debug.WriteLine("Login validation failed: Email is empty");
                    ShowError("Email is required.");
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    System.Diagnostics.Debug.WriteLine("Login validation failed: Password is empty");
                    ShowError("Password is required.");
                    return;
                }

                // Validate email format
                if (!IsValidEmail(email))
                {
                    System.Diagnostics.Debug.WriteLine("Login validation failed: Invalid email format");
                    ShowError("Please enter a valid email address.");
                    return;
                }

                // Validate password length (minimum 6 characters)
                if (password.Length < 6)
                {
                    System.Diagnostics.Debug.WriteLine("Login validation failed: Password too short");
                    ShowError("Password must be at least 6 characters long.");
                    return;
                }

                // Authenticate user
                System.Diagnostics.Debug.WriteLine("Attempting authentication...");
                if (AuthenticateUser(email, password))
                {
                    System.Diagnostics.Debug.WriteLine("Authentication successful - redirecting to Dashboard");
                    System.Diagnostics.Debug.WriteLine("Session after authentication - UserId: " + (Session["UserId"] != null ? Session["UserId"].ToString() : "NULL") + ", Role: " + (Session["UserRole"] != null ? Session["UserRole"].ToString() : "NULL"));
                    // Success - redirect to dashboard
                    Response.Redirect("Dashboard.aspx", false);
                    return; // Exit method after redirect
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Authentication failed: Invalid credentials");
                    ShowError("Invalid email or password. Please try again.");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // This is normal for Response.Redirect - don't treat as error
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                // Log the error (in production, log to file/database)
                System.Diagnostics.Debug.WriteLine("Login error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowError("An error occurred during login. Please try again later.");
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

        private bool AuthenticateUser(string email, string password)
        {
            try
            {
                // Authenticate user against database
                int userId = AuthenticationHelper.AuthenticateUser(email, password);

                if (userId > 0)
                {
                    // Authentication successful - store user ID in session
                    Session["UserId"] = userId;
                    Session["UserEmail"] = email;
                    
                    // Get user info and store name
                    var userInfo = AuthenticationHelper.GetUserById(userId);
                    if (userInfo != null)
                    {
                        string firstName = userInfo["FirstName"].ToString();
                        string lastName = userInfo["LastName"].ToString();
                        Session["UserName"] = firstName + " " + lastName;
                        Session["UserRole"] = userInfo["Role"].ToString();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Authentication error: " + ex.Message);
                return false;
            }
        }

        private void ShowError(string message)
        {
            System.Diagnostics.Debug.WriteLine("ShowError called with message: " + message);
            try
            {
                if (lblError != null)
                {
                    lblError.Text = message;
                    lblError.Visible = true;
                    System.Diagnostics.Debug.WriteLine("Error label updated successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: lblError is null!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ShowError exception: " + ex.Message);
            }
        }
    }
}

