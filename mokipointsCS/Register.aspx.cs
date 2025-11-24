using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;

namespace mokipointsCS
{
    public partial class Register : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Register.aspx Page_Load called at " + DateTime.Now.ToString());
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
                System.Diagnostics.Debug.WriteLine("Page_Load error: " + ex.Message);
            }
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnRegister_Click called at " + DateTime.Now.ToString());
            try
            {
                // Clear previous errors
                lblError.Visible = false;
                lblError.Text = string.Empty;
                
                System.Diagnostics.Debug.WriteLine("Form validation starting...");

                // Validate all fields
                System.Diagnostics.Debug.WriteLine("Starting form validation...");
                if (!ValidateForm())
                {
                    System.Diagnostics.Debug.WriteLine("Form validation FAILED");
                    ShowError("Please fill in all required fields correctly.");
                    return;
                }
                System.Diagnostics.Debug.WriteLine("Form validation PASSED");

                // Get form values
                string firstName = txtFirstName.Text.Trim();
                string lastName = txtLastName.Text.Trim();
                string middleName = txtMiddleName.Text.Trim();
                string email = txtEmail.Text.Trim().ToLower();
                string password = txtPassword.Text;
                string role = ddlRole.SelectedValue;
                DateTime? birthday = null;

                // Parse birthday
                if (!string.IsNullOrEmpty(txtBirthday.Text))
                {
                    if (DateTime.TryParse(txtBirthday.Text, out DateTime bday))
                    {
                        birthday = bday;
                    }
                }

                // Check if email already exists
                string checkQuery = "SELECT COUNT(*) FROM [dbo].[Users] WHERE Email = @Email";
                object count = DatabaseHelper.ExecuteScalar(checkQuery, new System.Data.SqlClient.SqlParameter("@Email", email));
                
                if (Convert.ToInt32(count) > 0)
                {
                    ShowError("This email is already registered. Please login or use a different email.");
                    return;
                }

                // Store registration data in session
                Session["RegFirstName"] = firstName;
                Session["RegLastName"] = lastName;
                Session["RegMiddleName"] = middleName;
                Session["RegEmail"] = email;
                Session["RegPassword"] = password;
                Session["RegRole"] = role;
                Session["RegBirthday"] = birthday;

                // Generate and send OTP
                string otpCode = OTPHelper.CreateOTP(email, "Registration");
                if (!string.IsNullOrEmpty(otpCode))
                {
                    string emailBody = EmailHelper.GetOTPEmailTemplate(otpCode, "Registration");
                    string subject = "MOKI POINTS - Registration Verification Code";
                    
                    if (EmailHelper.SendEmail(email, subject, emailBody))
                    {
                        // Set OTP session variables
                        Session["OTPEmail"] = email;
                        Session["OTPPurpose"] = "Registration";

                        // Redirect to OTP page
                        Response.Redirect("OTP.aspx");
                    }
                    else
                    {
                        // Clear session data
                        ClearRegistrationSession();
                        ShowError("Failed to send verification email. Please try again later.");
                    }
                }
                else
                {
                    // Clear session data
                    ClearRegistrationSession();
                    ShowError("Failed to generate verification code. Please try again.");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // This is normal for Response.Redirect - don't treat as error
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Registration error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowError("An error occurred during registration: " + ex.Message + ". Please try again later.");
            }
        }

        private bool ValidateForm()
        {
            System.Diagnostics.Debug.WriteLine("ValidateForm() called");
            try
            {
                // Check required fields
                if (txtFirstName == null || string.IsNullOrEmpty(txtFirstName.Text.Trim()))
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: First name is empty");
                    ShowError("First name is required.");
                    return false;
                }

                if (txtLastName == null || string.IsNullOrEmpty(txtLastName.Text.Trim()))
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Last name is empty");
                    ShowError("Last name is required.");
                    return false;
                }

                if (txtBirthday == null || string.IsNullOrEmpty(txtBirthday.Text.Trim()))
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Birthday is empty");
                    ShowError("Birthday is required.");
                    return false;
                }

                // Validate birthday format
                if (!DateTime.TryParse(txtBirthday.Text, out DateTime birthday))
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Invalid birthday format - " + txtBirthday.Text);
                    ShowError("Please enter a valid birthday.");
                    return false;
                }

                // Check if birthday is in the future
                if (birthday > DateTime.Now)
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Birthday is in the future");
                    ShowError("Birthday cannot be in the future.");
                    return false;
                }

                if (txtEmail == null || string.IsNullOrEmpty(txtEmail.Text.Trim()))
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Email is empty");
                    ShowError("Email is required.");
                    return false;
                }

                // Validate email format
                if (!IsValidEmail(txtEmail.Text.Trim()))
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Invalid email format - " + txtEmail.Text);
                    ShowError("Please enter a valid email address.");
                    return false;
                }

                if (ddlRole == null || string.IsNullOrEmpty(ddlRole.SelectedValue))
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Role not selected");
                    ShowError("Please select a role.");
                    return false;
                }

                if (txtPassword == null || string.IsNullOrEmpty(txtPassword.Text))
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Password is empty");
                    ShowError("Password is required.");
                    return false;
                }

                // Validate password length
                if (txtPassword.Text.Length < 6)
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Password too short");
                    ShowError("Password must be at least 6 characters long.");
                    return false;
                }

                if (txtConfirmPassword == null || txtPassword.Text != txtConfirmPassword.Text)
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Passwords do not match");
                    ShowError("Passwords do not match.");
                    return false;
                }

                // Check checkboxes
                if (chkTerms == null || !chkTerms.Checked)
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Terms not checked");
                    ShowError("You must agree to the Terms and Conditions.");
                    return false;
                }

                if (chkPrivacy == null || !chkPrivacy.Checked)
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Privacy not checked");
                    ShowError("You must agree to the Privacy Policy.");
                    return false;
                }

                System.Diagnostics.Debug.WriteLine("Validation PASSED - all fields valid");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ValidateForm exception: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowError("An error occurred during validation. Please try again.");
                return false;
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

        protected void cvTerms_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = chkTerms.Checked;
        }

        protected void cvPrivacy_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = chkPrivacy.Checked;
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

