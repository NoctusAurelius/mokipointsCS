using System;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class OTP : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("OTP.aspx Page_Load called at " + DateTime.Now.ToString());
            System.Diagnostics.Debug.WriteLine("IsPostBack: " + IsPostBack.ToString());
            System.Diagnostics.Debug.WriteLine("Request.Form count: " + Request.Form.Count);
            
            try
            {
                if (!IsPostBack)
                {
                    // Check if email and purpose are in session
                    if (Session["OTPEmail"] == null || Session["OTPPurpose"] == null)
                    {
                        System.Diagnostics.Debug.WriteLine("OTP session data missing - redirecting to Register");
                        Response.Redirect("Register.aspx");
                        return;
                    }

                    string email = Session["OTPEmail"].ToString();
                    litEmail.Text = email;
                    System.Diagnostics.Debug.WriteLine("OTP page loaded for email: " + email);
                    
                    // Clear previous messages
                    lblError.Visible = false;
                    lblSuccess.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("OTP Page_Load error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                ShowError("An error occurred. Please try again.");
            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnVerify_Click called at " + DateTime.Now.ToString());
            try
            {
                // Clear previous messages
                lblError.Visible = false;
                lblSuccess.Visible = false;

                // Get OTP code from inputs
                string otpCode = txtOTP1.Text + txtOTP2.Text + txtOTP3.Text + txtOTP4.Text + txtOTP5.Text + txtOTP6.Text;
                System.Diagnostics.Debug.WriteLine("OTP code entered: " + otpCode);

                // Validate OTP code
                if (string.IsNullOrEmpty(otpCode) || otpCode.Length != 6)
                {
                    System.Diagnostics.Debug.WriteLine("OTP validation failed: Code length invalid - " + (otpCode != null ? otpCode.Length : 0));
                    ShowError("Please enter the complete 6-digit code.");
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(otpCode, @"^\d{6}$"))
                {
                    System.Diagnostics.Debug.WriteLine("OTP validation failed: Code contains non-numeric characters");
                    ShowError("OTP code must contain only numbers.");
                    return;
                }

                // Get email and purpose from session
                if (Session["OTPEmail"] == null || Session["OTPPurpose"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("OTP validation failed: Session expired");
                    ShowError("Session expired. Please start over.");
                    return;
                }

                string email = Session["OTPEmail"].ToString();
                string purpose = Session["OTPPurpose"].ToString();
                System.Diagnostics.Debug.WriteLine("Validating OTP for email: " + email + ", purpose: " + purpose);

                // Validate OTP
                bool isValid = OTPHelper.ValidateOTP(email, otpCode, purpose);
                System.Diagnostics.Debug.WriteLine("OTP validation result: " + isValid.ToString());
                
                if (isValid)
                {
                    // OTP is valid
                    System.Diagnostics.Debug.WriteLine("OTP is valid - proceeding with " + purpose);
                    if (purpose == "Registration")
                    {
                        // Complete registration
                        CompleteRegistration();
                    }
                    else if (purpose == "ForgotPassword")
                    {
                        // Redirect to change password page
                        Session["OTPVerified"] = true;
                        System.Diagnostics.Debug.WriteLine("Redirecting to ChangePassword.aspx");
                        Response.Redirect("ChangePassword.aspx");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("OTP validation failed: Invalid or expired code");
                    ShowError("Invalid or expired verification code. Please try again or request a new code.");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // This is normal for Response.Redirect - don't treat as error
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("OTP verification error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowError("An error occurred during verification. Please try again.");
            }
        }

        protected void btnResend_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear previous messages
                lblError.Visible = false;
                lblSuccess.Visible = false;

                if (Session["OTPEmail"] == null || Session["OTPPurpose"] == null)
                {
                    ShowError("Session expired. Please start over.");
                    return;
                }

                string email = Session["OTPEmail"].ToString();
                string purpose = Session["OTPPurpose"].ToString();

                // Generate and send new OTP
                string otpCode = OTPHelper.CreateOTP(email, purpose);
                if (!string.IsNullOrEmpty(otpCode))
                {
                    string emailBody = EmailHelper.GetOTPEmailTemplate(otpCode, purpose);
                    string subject = purpose == "Registration" ? "MOKI POINTS - Registration Verification Code" : "MOKI POINTS - Password Reset Verification Code";
                    
                    if (EmailHelper.SendEmail(email, subject, emailBody))
                    {
                        ShowSuccess("A new verification code has been sent to your email.");
                    }
                    else
                    {
                        ShowError("Failed to send email. Please try again later.");
                    }
                }
                else
                {
                    ShowError("Failed to generate verification code. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Resend OTP error: " + ex.Message);
                ShowError("An error occurred. Please try again.");
            }
        }

        private void CompleteRegistration()
        {
            try
            {
                // Get registration data from session
                if (Session["RegFirstName"] == null || Session["RegLastName"] == null ||
                    Session["RegEmail"] == null || Session["RegPassword"] == null ||
                    Session["RegRole"] == null)
                {
                    ShowError("Registration data not found. Please register again.");
                    return;
                }

                string firstName = Session["RegFirstName"].ToString();
                string lastName = Session["RegLastName"].ToString();
                string middleName = Session["RegMiddleName"] != null ? Session["RegMiddleName"].ToString() : "";
                string email = Session["RegEmail"].ToString();
                string password = Session["RegPassword"].ToString();
                string role = Session["RegRole"].ToString();
                DateTime? birthday = Session["RegBirthday"] as DateTime?;

                System.Diagnostics.Debug.WriteLine("Creating user account for: " + email + ", Role: " + role);

                // Create user account
                int userId = AuthenticationHelper.CreateUser(firstName, lastName, middleName, email, password, role, birthday);

                System.Diagnostics.Debug.WriteLine("User account created with ID: " + userId);

                if (userId > 0)
                {
                    // Clear registration session data FIRST (before setting auth session)
                    // This ensures we don't accidentally clear auth session
                    ClearRegistrationSession();
                    
                    // Set user session AFTER clearing registration data
                    Session["UserId"] = userId;
                    Session["UserEmail"] = email;
                    Session["FirstName"] = firstName;
                    Session["LastName"] = lastName;
                    Session["UserName"] = firstName + " " + lastName;
                    Session["UserRole"] = role;

                    System.Diagnostics.Debug.WriteLine("Session variables set - UserId: " + userId + ", Role: " + role);
                    System.Diagnostics.Debug.WriteLine("Verifying session after setting - UserId: " + (Session["UserId"] != null ? Session["UserId"].ToString() : "NULL") + ", Role: " + (Session["UserRole"] != null ? Session["UserRole"].ToString() : "NULL"));
                    
                    // Force session save by accessing it
                    string testUserId = Session["UserId"] != null ? Session["UserId"].ToString() : null;
                    string testRole = Session["UserRole"] != null ? Session["UserRole"].ToString() : null;
                    System.Diagnostics.Debug.WriteLine("Session verification test - UserId: " + (testUserId != null ? testUserId : "NULL") + ", Role: " + (testRole != null ? testRole : "NULL"));

                    // Ensure session is saved before redirect
                    Session["UserId"] = userId; // Re-set to ensure it's saved
                    Session["UserRole"] = role; // Re-set to ensure it's saved
                    
                    // Redirect based on role
                    System.Diagnostics.Debug.WriteLine("Redirecting based on role: " + role);
                    if (role == "CHILD")
                    {
                        // Children must join a family first
                        System.Diagnostics.Debug.WriteLine("Redirecting CHILD to JoinFamily.aspx");
                        Response.Redirect("JoinFamily.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Redirecting PARENT to Dashboard.aspx");
                        Response.Redirect("Dashboard.aspx", false);
                        Context.ApplicationInstance.CompleteRequest();
                    }
                    return; // Exit method after redirect
                }
                else if (userId == -2)
                {
                    // Duplicate name and birthday combination
                    System.Diagnostics.Debug.WriteLine("User account creation failed - duplicate name and birthday");
                    ShowError("An account with this name and birthday already exists. Please use your existing account or contact support if you believe this is an error.");
                }
                else
                {
                    // userId == -1 (duplicate email) or other error
                    System.Diagnostics.Debug.WriteLine("User account creation failed - userId is 0 or negative");
                    ShowError("Registration failed. This email may already be registered. Please try again.");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // This is normal for Response.Redirect - don't treat as error
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Complete registration error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowError("An error occurred during registration. Please try again.");
            }
        }

        private void ClearRegistrationSession()
        {
            Session.Remove("RegFirstName");
            Session.Remove("RegLastName");
            Session.Remove("RegMiddleName");
            Session.Remove("RegEmail");
            Session.Remove("RegPassword");
            Session.Remove("RegRole");
            Session.Remove("RegBirthday");
            Session.Remove("OTPEmail");
            Session.Remove("OTPPurpose");
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
                    lblSuccess.Visible = false;
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

        private void ShowSuccess(string message)
        {
            System.Diagnostics.Debug.WriteLine("ShowSuccess called with message: " + message);
            try
            {
                if (lblSuccess != null)
                {
                    lblSuccess.Text = message;
                    lblSuccess.Visible = true;
                    lblError.Visible = false;
                    System.Diagnostics.Debug.WriteLine("Success label updated successfully");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: lblSuccess is null!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ShowSuccess exception: " + ex.Message);
            }
        }
    }
}

