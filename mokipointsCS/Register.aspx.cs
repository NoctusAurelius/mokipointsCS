using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;
using System.Globalization;

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
                    // Only show generic error if no specific error message was already set
                    if (lblError == null || string.IsNullOrEmpty(lblError.Text) || !lblError.Visible)
                    {
                        ShowError("Please fill in all required fields correctly.");
                    }
                    return;
                }
                System.Diagnostics.Debug.WriteLine("Form validation PASSED");

                // Get form values
                string firstName = txtFirstName.Text.Trim();
                string lastName = txtLastName.Text.Trim();
                string middleName = txtMiddleName.Text.Trim();
                string email = txtEmail.Text.Trim().ToLower();
                string password = txtPassword.Text;
                string role = hidRole.Value; // Get role from hidden field instead of dropdown
                DateTime? birthday = null;

                // Parse birthday using explicit format to avoid culture-dependent parsing issues
                // Datepicker outputs MM/dd/yyyy format, so we parse with that exact format
                if (!string.IsNullOrEmpty(txtBirthday.Text))
                {
                    if (DateTime.TryParseExact(txtBirthday.Text.Trim(), "MM/dd/yyyy", 
                        CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime bday))
                    {
                        birthday = bday;
                    }
                    else
                    {
                        // Log parsing failure for debugging
                        System.Diagnostics.Debug.WriteLine("Birthday parsing failed in btnRegister_Click: " + txtBirthday.Text);
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

                // Check if FirstName + LastName + Birthday combination already exists
                // This prevents creating multiple accounts with the same identity using different emails
                if (birthday.HasValue)
                {
                    string duplicateCheckQuery = @"
                        SELECT COUNT(*) FROM [dbo].[Users] 
                        WHERE LOWER(LTRIM(RTRIM(FirstName))) = LOWER(LTRIM(RTRIM(@FirstName)))
                        AND LOWER(LTRIM(RTRIM(LastName))) = LOWER(LTRIM(RTRIM(@LastName)))
                        AND CAST(Birthday AS DATE) = CAST(@Birthday AS DATE)";
                    
                    object duplicateCount = DatabaseHelper.ExecuteScalar(duplicateCheckQuery,
                        new System.Data.SqlClient.SqlParameter("@FirstName", firstName),
                        new System.Data.SqlClient.SqlParameter("@LastName", lastName),
                        new System.Data.SqlClient.SqlParameter("@Birthday", birthday.Value.Date));
                    
                    if (Convert.ToInt32(duplicateCount) > 0)
                    {
                        ShowError("An account with this name and birthday already exists. Please use your existing account or contact support if you believe this is an error.");
                        return;
                    }
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

                // Validate birthday format using explicit format to avoid culture-dependent parsing issues
                // Datepicker outputs MM/dd/yyyy format, so we validate with that exact format
                if (!DateTime.TryParseExact(txtBirthday.Text.Trim(), "MM/dd/yyyy", 
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthday))
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Invalid birthday format - " + txtBirthday.Text);
                    ShowError("Please enter a valid birthday in MM/DD/YYYY format (e.g., 12/16/2015).");
                    return false;
                }

                // Check if birthday is in the future
                if (birthday > DateTime.Now)
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Birthday is in the future");
                    ShowError("Birthday cannot be in the future.");
                    return false;
                }

                // Validate age for child accounts (must be 8-19 years old)
                if (hidRole != null && !string.IsNullOrEmpty(hidRole.Value) && hidRole.Value == "CHILD")
                {
                    int age = CalculateAge(birthday);
                    if (age < 8 || age > 19)
                    {
                        System.Diagnostics.Debug.WriteLine("Validation failed: Child age is " + age + " (must be 8-19)");
                        ShowError("Child accounts must be between 8 and 19 years old. Your age is " + age + " years.");
                        return false;
                    }
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

                if (hidRole == null || string.IsNullOrEmpty(hidRole.Value))
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Role not selected");
                    if (lblRoleError != null)
                    {
                        lblRoleError.Visible = true;
                        lblRoleError.Style["display"] = "block";
                    }
                    ShowError("Please select a role.");
                    return false;
                }
                else
                {
                    if (lblRoleError != null)
                    {
                        lblRoleError.Visible = false;
                        lblRoleError.Style["display"] = "none";
                    }
                }

                if (txtPassword == null || string.IsNullOrEmpty(txtPassword.Text))
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Password is empty");
                    ShowError("Password is required.");
                    return false;
                }

                // Validate password format (8-16 characters, at least one letter and one number)
                if (txtPassword.Text.Length < 8 || txtPassword.Text.Length > 16)
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Password length invalid");
                    ShowError("Password must be 8-16 characters long.");
                    return false;
                }
                
                // Check if password contains at least one letter and one number
                bool hasLetter = false;
                bool hasNumber = false;
                foreach (char c in txtPassword.Text)
                {
                    if (char.IsLetter(c)) hasLetter = true;
                    if (char.IsDigit(c)) hasNumber = true;
                }
                
                if (!hasLetter || !hasNumber)
                {
                    System.Diagnostics.Debug.WriteLine("Validation failed: Password must contain letters and numbers");
                    ShowError("Password must contain at least one letter and one number.");
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

        private int CalculateAge(DateTime birthday)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthday.Year;
            if (birthday.Date > today.AddYears(-age)) age--;
            return age;
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
                    
                    // Register client script to trigger auto-fade after 5 seconds
                    string script = @"
                        setTimeout(function() {
                            var errorMsg = document.getElementById('" + lblError.ClientID + @"');
                            if (errorMsg) {
                                errorMsg.classList.add('fade-out');
                                setTimeout(function() {
                                    errorMsg.style.display = 'none';
                                }, 500);
                            }
                        }, 5000);";
                    ClientScript.RegisterStartupScript(this.GetType(), "AutoFadeError", script, true);
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

