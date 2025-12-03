<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="mokipointsCS.Register" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Register - MOKI POINTS</title>
    <link rel="icon" type="image/x-icon" href="/favicon/favicon.ico" />
    <link rel="icon" type="image/png" sizes="16x16" href="/favicon/favicon-16x16.png" />
    <link rel="icon" type="image/png" sizes="32x32" href="/favicon/favicon-32x32.png" />
    <link rel="apple-touch-icon" sizes="180x180" href="/favicon/apple-touch-icon.png" />
    <link rel="manifest" href="/favicon/site.webmanifest" />
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: Arial, sans-serif;
            background-color: white;
            padding: 20px;
        }
        
        .register-container {
            max-width: 600px;
            margin: 0 auto;
            background-color: white;
            padding: 50px 40px;
            border-radius: 10px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }
        
        .branding {
            text-align: center;
            margin-bottom: 30px;
        }
        
        .branding-text {
            font-size: 42px;
            font-weight: bold;
            letter-spacing: 4px;
            margin-bottom: 10px;
        }
        
        .branding .moki {
            color: #0066CC; /* Blue */
        }
        
        .branding .points {
            color: #FF6600; /* Orange */
        }
        
        .page-title {
            font-size: 28px;
            color: #333;
            text-align: center;
            margin-bottom: 40px;
        }
        
        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 15px;
            margin-bottom: 20px;
        }
        
        .form-group {
            margin-bottom: 20px;
        }
        
        .form-group.full-width {
            grid-column: 1 / -1;
        }
        
        .form-group label {
            display: block;
            margin-bottom: 8px;
            color: #333;
            font-weight: 500;
            font-size: 14px;
        }
        
        .form-group label .required {
            color: #d32f2f;
        }
        
        .form-control {
            width: 100%;
            padding: 12px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 5px;
            font-size: 16px;
            transition: border-color 0.3s;
        }
        
        .form-control:focus {
            outline: none;
            border-color: #0066CC;
        }
        
        .form-control.date-picker {
            cursor: pointer;
        }
        
        .input-wrapper {
            position: relative;
        }
        
        .password-toggle {
            position: absolute;
            right: 15px;
            top: 50%;
            transform: translateY(-50%);
            background: none;
            border: none;
            cursor: pointer;
            color: #666;
            font-size: 18px;
            padding: 5px;
            z-index: 10;
        }
        
        .password-toggle:hover {
            color: #0066CC;
        }
        
        .password-toggle:focus {
            outline: none;
        }
        
        .password-hint {
            font-size: 12px;
            color: #999;
            margin-top: 5px;
            line-height: 1.4;
        }
        
        .btn-register {
            width: 100%;
            padding: 14px;
            background-color: #0066CC; /* Blue */
            color: white;
            border: none;
            border-radius: 5px;
            font-size: 18px;
            font-weight: bold;
            cursor: pointer;
            transition: background-color 0.3s, transform 0.2s;
            margin-top: 20px;
        }
        
        .btn-register:hover {
            background-color: #0052a3;
            transform: translateY(-2px);
        }
        
        .btn-register:disabled {
            background-color: #999;
            cursor: not-allowed;
            transform: none;
        }
        
        .checkbox-group {
            margin: 20px 0;
            padding: 15px;
            background-color: #f9f9f9;
            border-radius: 5px;
            border-left: 4px solid #FFB6C1; /* Pink accent */
        }
        
        .checkbox-item {
            margin-bottom: 15px;
        }
        
        .checkbox-item:last-child {
            margin-bottom: 0;
        }
        
        .checkbox-item label {
            display: flex;
            align-items: center;
            cursor: pointer;
            font-weight: normal;
        }
        
        .checkbox-item input[type="checkbox"] {
            margin-right: 10px;
            width: 20px;
            height: 20px;
            cursor: pointer;
        }
        
        .checkbox-item a {
            color: #0066CC;
            text-decoration: none;
            margin-left: 5px;
        }
        
        .checkbox-item a:hover {
            text-decoration: underline;
        }
        
        .error-message {
            color: #d32f2f;
            font-size: 14px;
            padding: 15px 20px;
            background-color: #ffebee;
            border-radius: 5px;
            border-left: 3px solid #d32f2f;
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 10000;
            max-width: 400px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            opacity: 1;
            transition: opacity 0.5s ease-out;
        }
        
        .error-message.fade-out {
            opacity: 0;
            pointer-events: none;
        }
        
        .validation-error {
            color: #d32f2f;
            font-size: 12px;
            margin-top: 5px;
            display: block;
        }
        
        .spinner {
            border: 3px solid rgba(255, 255, 255, 0.3);
            border-top: 3px solid white;
            border-radius: 50%;
            width: 20px;
            height: 20px;
            animation: spin 1s linear infinite;
            display: inline-block;
            margin-right: 10px;
            vertical-align: middle;
        }
        
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
        }
        
        .login-link {
            text-align: center;
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid #e0e0e0;
        }
        
        .login-link a {
            color: #0066CC;
            text-decoration: none;
        }
        
        .login-link a:hover {
            text-decoration: underline;
        }
        
        /* Role Selection Cards */
        .role-cards {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            margin-top: 10px;
        }
        
        .role-card {
            border: 2px solid #e0e0e0;
            border-radius: 10px;
            padding: 25px;
            text-align: center;
            cursor: pointer;
            transition: all 0.3s ease;
            background-color: white;
        }
        
        .role-card:hover {
            border-color: #0066CC;
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0, 102, 204, 0.2);
        }
        
        .role-card.selected {
            border-color: #0066CC;
            background-color: #e7f3ff;
            box-shadow: 0 4px 12px rgba(0, 102, 204, 0.3);
        }
        
        .role-icon {
            font-size: 48px;
            margin-bottom: 15px;
        }
        
        .role-title {
            font-size: 20px;
            font-weight: bold;
            color: #333;
            margin-bottom: 10px;
        }
        
        .role-description {
            font-size: 14px;
            color: #666;
            line-height: 1.5;
        }
        
        @media (max-width: 768px) {
            .form-row {
                grid-template-columns: 1fr;
            }
            
            .role-cards {
                grid-template-columns: 1fr;
            }
        }
    </style>
    <link rel="stylesheet" href="https://code.jquery.com/ui/1.12.1/themes/ui-lightness/jquery-ui.css" />
    <script src="https://code.jquery.com/jquery-1.12.4.js"></script>
    <script src="https://code.jquery.com/ui/1.12.1/jquery-ui.js"></script>
    <script>
        $(function () {
            $("#<%= txtBirthday.ClientID %>").datepicker({
                changeMonth: true,
                changeYear: true,
                yearRange: "1900:2025",
                dateFormat: "mm/dd/yy",
                maxDate: new Date()
            });
        });
        
        function showLoading() {
            try {
                console.log('showLoading() called');
                var btn = document.getElementById('<%= btnRegister.ClientID %>');
                console.log('Button found:', btn);
                if (btn) {
                    console.log('Button current state - disabled:', btn.disabled, 'value:', btn.value, 'innerHTML:', btn.innerHTML);
                    // Use setTimeout to delay disabling until after form submission starts
                    setTimeout(function() {
                        if (btn && !btn.disabled) {
                            btn.disabled = true;
                            // For input buttons, we need to change the value attribute
                            if (btn.tagName === 'INPUT') {
                                btn.value = 'Creating Account...';
                            } else {
                                btn.innerHTML = '<span class="spinner"></span>Creating Account...';
                            }
                            console.log('Button disabled and text updated (delayed)');
                        }
                    }, 10);
                } else {
                    console.error('Button not found!');
                }
            } catch (e) {
                console.error('Error in showLoading:', e);
            }
        }
        
        // Add page load logging
        window.addEventListener('load', function() {
            console.log('Page loaded');
            var btn = document.getElementById('<%= btnRegister.ClientID %>');
            console.log('Register button on page load:', btn ? 'Found' : 'NOT FOUND', btn ? btn.id : 'N/A');
        });
        
        // Log validation attempts
        if (typeof Page_ClientValidate === 'function') {
            console.log('Page_ClientValidate function exists');
        } else {
            console.warn('Page_ClientValidate function does NOT exist');
        }
        
        // Password toggle function
        function togglePassword(inputId, toggleId) {
            var input = document.getElementById(inputId);
            var toggle = document.getElementById(toggleId);
            if (input && toggle) {
                if (input.type === 'password') {
                    input.type = 'text';
                    toggle.innerHTML = '&#128584;'; // See-no-evil monkey (hidden)
                } else {
                    input.type = 'password';
                    toggle.innerHTML = '&#128065;'; // Eye (visible)
                }
            }
        }
        
        // Role selection function
        function selectRole(role) {
            // Update hidden field
            document.getElementById('<%= hidRole.ClientID %>').value = role;
            
            // Update card visuals
            var cards = document.querySelectorAll('.role-card');
            cards.forEach(function(card) {
                if (card.getAttribute('data-role') === role) {
                    card.classList.add('selected');
                } else {
                    card.classList.remove('selected');
                }
            });
            
            // Clear validation error
            var errorLabel = document.getElementById('<%= lblRoleError.ClientID %>');
            if (errorLabel) {
                errorLabel.style.display = 'none';
            }
        }
        
        // Validate role before form submission
        function validateRole() {
            var role = document.getElementById('<%= hidRole.ClientID %>').value;
            var errorLabel = document.getElementById('<%= lblRoleError.ClientID %>');
            
            if (!role || role === '') {
                if (errorLabel) {
                    errorLabel.style.display = 'block';
                }
                return false;
            } else {
                if (errorLabel) {
                    errorLabel.style.display = 'none';
                }
                return true;
            }
        }
        
        // Initialize role selection on page load if value exists
        window.addEventListener('load', function() {
            var selectedRole = document.getElementById('<%= hidRole.ClientID %>').value;
            if (selectedRole) {
                selectRole(selectedRole);
            }
            
            // Auto-fade error messages after 5 seconds if visible
            var errorMessage = document.getElementById('<%= lblError.ClientID %>');
            if (errorMessage) {
                // Check if error message is visible (either inline style or server-side Visible property made it render)
                var isVisible = errorMessage.offsetParent !== null || 
                               window.getComputedStyle(errorMessage).display !== 'none';
                
                if (isVisible && errorMessage.textContent.trim() !== '') {
                    // Reset fade state
                    errorMessage.classList.remove('fade-out');
                    errorMessage.style.opacity = '1';
                    
                    // Clear any existing timeout
                    if (window.errorMessageTimeout) {
                        clearTimeout(window.errorMessageTimeout);
                    }
                    
                    // Set timeout to fade out after 5 seconds
                    window.errorMessageTimeout = setTimeout(function() {
                        errorMessage.classList.add('fade-out');
                        setTimeout(function() {
                            errorMessage.style.display = 'none';
                        }, 500); // Wait for fade transition to complete
                    }, 5000);
                }
            }
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="register-container">
            <div class="branding">
                <div class="branding-text">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
            </div>
            
            <h2 class="page-title">Create Your Account</h2>
            
            <div class="form-row">
                <div class="form-group">
                    <label for="txtFirstName">First Name <span class="required">*</span></label>
                    <asp:TextBox ID="txtFirstName" runat="server" CssClass="form-control" placeholder="Enter first name"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvFirstName" runat="server" ControlToValidate="txtFirstName" 
                        ErrorMessage="First name is required" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
                </div>
                
                <div class="form-group">
                    <label for="txtLastName">Last Name <span class="required">*</span></label>
                    <asp:TextBox ID="txtLastName" runat="server" CssClass="form-control" placeholder="Enter last name"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvLastName" runat="server" ControlToValidate="txtLastName" 
                        ErrorMessage="Last name is required" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
                </div>
            </div>
            
            <div class="form-group">
                <label for="txtMiddleName">Middle Name</label>
                <asp:TextBox ID="txtMiddleName" runat="server" CssClass="form-control" placeholder="Enter middle name (optional)"></asp:TextBox>
            </div>
            
            <div class="form-group">
                <label for="txtBirthday">Birthday <span class="required">*</span></label>
                <asp:TextBox ID="txtBirthday" runat="server" CssClass="form-control date-picker" placeholder="MM/DD/YYYY"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvBirthday" runat="server" ControlToValidate="txtBirthday" 
                    ErrorMessage="Birthday is required" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
            </div>
            
            <div class="form-group">
                <label for="txtEmail">Email <span class="required">*</span></label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="Enter your email"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" 
                    ErrorMessage="Email is required" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" 
                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                    ErrorMessage="Please enter a valid email address" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RegularExpressionValidator>
            </div>
            
            <div class="form-group">
                <label>Role <span class="required">*</span></label>
                <div class="role-cards">
                    <div class="role-card" data-role="PARENT" onclick="selectRole('PARENT')">
                        <div class="role-icon">&#128106;</div>
                        <div class="role-title">Parent</div>
                        <div class="role-description">Create and manage tasks, review submissions, and manage rewards</div>
                    </div>
                    <div class="role-card" data-role="CHILD" onclick="selectRole('CHILD')">
                        <div class="role-icon">&#128118;</div>
                        <div class="role-title">Child</div>
                        <div class="role-description">Complete tasks, earn points, and redeem rewards</div>
                    </div>
                </div>
                <asp:HiddenField ID="hidRole" runat="server" Value="" />
                <asp:Label ID="lblRoleError" runat="server" CssClass="validation-error" style="display: none; color: #d32f2f; font-size: 12px; margin-top: 5px;">Please select a role</asp:Label>
            </div>
            
            <div class="form-row">
                <div class="form-group">
                    <label for="txtPassword">Password <span class="required">*</span></label>
                    <div class="input-wrapper">
                        <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter password"></asp:TextBox>
                        <button type="button" class="password-toggle" onclick="togglePassword('<%= txtPassword.ClientID %>', 'togglePassword1')" id="togglePassword1" aria-label="Toggle password visibility">&#128065;</button>
                    </div>
                    <div class="password-hint">Password must be 8-16 characters with at least one letter and one number</div>
                    <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" 
                        ErrorMessage="Password is required" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revPassword" runat="server" ControlToValidate="txtPassword" 
                        ValidationExpression="^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,16}$" 
                        ErrorMessage="Password must be 8-16 characters with at least one letter and one number" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RegularExpressionValidator>
                </div>
                
                <div class="form-group">
                    <label for="txtConfirmPassword">Confirm Password <span class="required">*</span></label>
                    <div class="input-wrapper">
                        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Confirm password"></asp:TextBox>
                        <button type="button" class="password-toggle" onclick="togglePassword('<%= txtConfirmPassword.ClientID %>', 'togglePassword2')" id="togglePassword2" aria-label="Toggle password visibility">&#128065;</button>
                    </div>
                    <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" 
                        ErrorMessage="Please confirm your password" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
                    <asp:CompareValidator ID="cvPassword" runat="server" ControlToValidate="txtConfirmPassword" 
                        ControlToCompare="txtPassword" 
                        ErrorMessage="Passwords do not match" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:CompareValidator>
                </div>
            </div>
            
            <div class="checkbox-group">
                <div class="checkbox-item">
                    <label>
                        <asp:CheckBox ID="chkTerms" runat="server" />
                        I agree to the <a href="TermsAndConditions.aspx" target="_blank">Terms and Conditions</a> <span class="required">*</span>
                    </label>
                    <asp:CustomValidator ID="cvTerms" runat="server" 
                        ErrorMessage="You must agree to the Terms and Conditions" 
                        CssClass="validation-error" Display="Dynamic" 
                        OnServerValidate="cvTerms_ServerValidate" 
                        EnableClientScript="false"></asp:CustomValidator>
                </div>
                
                <div class="checkbox-item">
                    <label>
                        <asp:CheckBox ID="chkPrivacy" runat="server" />
                        I agree to the <a href="PrivacyPolicy.aspx" target="_blank">Privacy Policy</a> <span class="required">*</span>
                    </label>
                    <asp:CustomValidator ID="cvPrivacy" runat="server" 
                        ErrorMessage="You must agree to the Privacy Policy" 
                        CssClass="validation-error" Display="Dynamic" 
                        OnServerValidate="cvPrivacy_ServerValidate" 
                        EnableClientScript="false"></asp:CustomValidator>
                </div>
            </div>
            
            <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="btn-register" OnClick="btnRegister_Click" OnClientClick="console.log('Button clicked'); if (!validateRole()) { console.log('Role validation failed'); return false; } var isValid = true; if(typeof Page_ClientValidate === 'function') { console.log('Running validation...'); isValid = Page_ClientValidate(); console.log('Validation result:', isValid); } if(isValid) { console.log('Validation passed - allowing submit'); showLoading(); return true; } else { console.log('Validation failed - preventing submit'); return false; }" UseSubmitBehavior="true" />
            
            <asp:Label ID="lblError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
            
            <div class="login-link">
                <p>Already have an account? <a href="Login.aspx">Login here</a></p>
            </div>
        </div>
    </form>
</body>
</html>

