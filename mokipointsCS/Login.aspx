<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="mokipointsCS.Login" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Login - MOKI POINTS</title>
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
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            padding: 20px;
        }
        
        .login-container {
            width: 100%;
            max-width: 450px;
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
        
        .welcome-text {
            font-size: 24px;
            color: #333;
            text-align: center;
            margin-bottom: 40px;
        }
        
        .form-group {
            margin-bottom: 25px;
        }
        
        .form-group label {
            display: block;
            margin-bottom: 8px;
            color: #333;
            font-weight: 500;
            font-size: 14px;
        }
        
        .input-wrapper {
            position: relative;
        }
        
        .form-control {
            width: 100%;
            padding: 12px 45px 12px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 5px;
            font-size: 16px;
            transition: border-color 0.3s;
        }
        
        .form-control:focus {
            outline: none;
            border-color: #0066CC;
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
            transition: color 0.3s;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .password-toggle:hover {
            color: #0066CC;
        }
        
        .eye-icon {
            width: 20px;
            height: 20px;
            fill: currentColor;
        }
        
        .btn-login {
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
            margin-top: 10px;
        }
        
        .btn-login:hover {
            background-color: #0052a3;
            transform: translateY(-2px);
        }
        
        .btn-login:active {
            transform: translateY(0);
        }
        
        .forgot-password {
            text-align: center;
            margin-top: 20px;
        }
        
        .forgot-password a {
            color: #0066CC;
            text-decoration: none;
            font-size: 14px;
        }
        
        .forgot-password a:hover {
            text-decoration: underline;
        }
        
        .register-section {
            text-align: center;
            margin-top: 40px;
            padding-top: 30px;
            border-top: 1px solid #e0e0e0;
        }
        
        .register-section p {
            color: #666;
            font-size: 14px;
            margin-bottom: 15px;
        }
        
        .btn-register {
            display: inline-block;
            padding: 10px 30px;
            background-color: #FF6600; /* Orange */
            color: white;
            border: none;
            border-radius: 5px;
            font-size: 16px;
            font-weight: bold;
            text-decoration: none;
            transition: background-color 0.3s, transform 0.2s;
        }
        
        .btn-register:hover {
            background-color: #e55a00;
            transform: translateY(-2px);
            text-decoration: none;
            color: white;
        }
        
        .error-message {
            color: #d32f2f;
            font-size: 14px;
            margin-top: 10px;
            text-align: center;
            padding: 10px;
            background-color: #ffebee;
            border-radius: 5px;
            border-left: 3px solid #d32f2f;
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
        
        .btn-login:disabled {
            background-color: #999;
            cursor: not-allowed;
            transform: none;
        }
        
        .btn-login:disabled:hover {
            background-color: #999;
            transform: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="login-container">
            <div class="branding">
                <div class="branding-text">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
            </div>
            
            <h2 class="welcome-text">Welcome back!</h2>
            
            <div class="form-group">
                <label for="txtEmail">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="Enter your email"></asp:TextBox>
                <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" 
                    ErrorMessage="Email is required" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revEmail" runat="server" ControlToValidate="txtEmail" 
                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" 
                    ErrorMessage="Please enter a valid email address" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RegularExpressionValidator>
            </div>
            
            <div class="form-group">
                <label for="txtPassword">Password</label>
                <div class="input-wrapper">
                    <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter your password"></asp:TextBox>
                    <button type="button" class="password-toggle" id="btnTogglePassword" onclick="togglePassword()">
                        <svg class="eye-icon" id="eyeIcon" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                            <path d="M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z"/>
                        </svg>
                    </button>
                </div>
                <asp:RequiredFieldValidator ID="rfvPassword" runat="server" ControlToValidate="txtPassword" 
                    ErrorMessage="Password is required" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
            </div>
            
            <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="btn-login" OnClick="btnLogin_Click" OnClientClick="console.log('Login button clicked'); var isValid = true; if(typeof Page_ClientValidate === 'function') { console.log('Running validation...'); isValid = Page_ClientValidate(); console.log('Validation result:', isValid); } if(isValid) { console.log('Validation passed - allowing submit'); showLoading(); return true; } else { console.log('Validation failed - preventing submit'); return false; }" UseSubmitBehavior="true" />
            
            <asp:Label ID="lblError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
            
            <div class="forgot-password">
                <a href="ForgotPassword.aspx">Forgot Password?</a>
            </div>
            
            <div class="register-section">
                <p>Don't have an account?</p>
                <a href="Register.aspx" class="btn-register">Register</a>
            </div>
        </div>
    </form>
    
    <script>
        function togglePassword() {
            var passwordField = document.getElementById('<%= txtPassword.ClientID %>');
            var eyeIcon = document.getElementById('eyeIcon');
            
            if (passwordField.type === 'password') {
                passwordField.type = 'text';
                // Eye closed icon
                eyeIcon.innerHTML = '<path d="M12 7c2.76 0 5 2.24 5 5 0 .65-.13 1.26-.36 1.83l2.92 2.92c1.51-1.26 2.7-2.89 3.43-4.75-1.73-4.39-6-7.5-11-7.5-1.4 0-2.74.25-3.98.7l2.16 2.16C10.74 7.13 11.35 7 12 7zM2 4.27l2.28 2.28.46.46C3.08 8.3 1.78 10.02 1 12c1.73 4.39 6 7.5 11 7.5 1.55 0 3.03-.3 4.38-.84l.42.42L19.73 22 21 20.73 3.27 3 2 4.27zM7.53 9.8l1.55 1.55c-.05.21-.08.43-.08.65 0 1.66 1.34 3 3 3 .22 0 .44-.03.65-.08l1.55 1.55c-.67.33-1.41.53-2.2.53-2.76 0-5-2.24-5-5 0-.79.2-1.53.53-2.2zm4.31-.78l3.15 3.15.02-.16c0-1.66-1.34-3-3-3l-.17.01z"/>';
            } else {
                passwordField.type = 'password';
                // Eye open icon
                eyeIcon.innerHTML = '<path d="M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5zM12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5zm0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3z"/>';
            }
        }
        
        function showLoading() {
            try {
                console.log('Login showLoading() called');
                var btn = document.getElementById('<%= btnLogin.ClientID %>');
                console.log('Login button found:', btn);
                if (btn) {
                    console.log('Button current state - disabled:', btn.disabled, 'value:', btn.value, 'innerHTML:', btn.innerHTML);
                    // Use setTimeout to delay disabling until after form submission starts
                    setTimeout(function() {
                        if (btn && !btn.disabled) {
                            btn.disabled = true;
                            // For input buttons, we need to change the value attribute
                            if (btn.tagName === 'INPUT') {
                                btn.value = 'Logging in...';
                            } else {
                                btn.innerHTML = '<span class="spinner"></span>Logging in...';
                            }
                            console.log('Button disabled and text updated (delayed)');
                        }
                    }, 10);
                } else {
                    console.error('Login button not found!');
                }
            } catch (e) {
                console.error('Error in showLoading:', e);
            }
        }
        
        // Add page load logging
        window.addEventListener('load', function() {
            console.log('Login Page loaded');
            var btn = document.getElementById('<%= btnLogin.ClientID %>');
            console.log('Login button on page load:', btn ? 'Found' : 'NOT FOUND', btn ? btn.id : 'N/A');
        });
    </script>
</body>
</html>

