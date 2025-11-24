<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="mokipointsCS.ChangePassword" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Change Password - MOKI POINTS</title>
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
        
        .change-password-container {
            width: 100%;
            max-width: 500px;
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
            margin-bottom: 15px;
        }
        
        .page-subtitle {
            font-size: 16px;
            color: #666;
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
        
        .form-group label .required {
            color: #d32f2f;
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
        }
        
        .password-toggle:hover {
            color: #0066CC;
        }
        
        .btn-change-password {
            width: 100%;
            padding: 14px;
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            font-size: 18px;
            font-weight: bold;
            cursor: pointer;
            transition: background-color 0.3s, transform 0.2s;
            margin-top: 20px;
        }
        
        .btn-change-password:hover {
            background-color: #0052a3;
            transform: translateY(-2px);
        }
        
        .btn-change-password:disabled {
            background-color: #999;
            cursor: not-allowed;
            transform: none;
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
        
        .success-message {
            color: #2e7d32;
            font-size: 14px;
            margin-top: 10px;
            text-align: center;
            padding: 10px;
            background-color: #e8f5e9;
            border-radius: 5px;
            border-left: 3px solid #2e7d32;
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
    </style>
    <script>
        function togglePasswordVisibility(inputId, buttonId) {
            var input = document.getElementById(inputId);
            var button = document.getElementById(buttonId);
            
            if (input.type === 'password') {
                input.type = 'text';
                button.innerHTML = '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"></path><line x1="1" y1="1" x2="23" y2="23"></line></svg>';
            } else {
                input.type = 'password';
                button.innerHTML = '<svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path><circle cx="12" cy="12" r="3"></circle></svg>';
            }
        }
        
        function showLoading() {
            var btn = document.getElementById('<%= btnChangePassword.ClientID %>');
            if (Page_ClientValidate()) {
                btn.disabled = true;
                btn.innerHTML = '<span class="spinner"></span>Changing Password...';
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="change-password-container">
            <div class="branding">
                <div class="branding-text">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
            </div>
            
            <h2 class="page-title">Change Your Password</h2>
            <p class="page-subtitle">Enter your new password below</p>
            
            <div class="form-group">
                <label for="txtNewPassword">New Password <span class="required">*</span></label>
                <div class="input-wrapper">
                    <asp:TextBox ID="txtNewPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Enter new password"></asp:TextBox>
                    <button type="button" class="password-toggle" onclick="togglePasswordVisibility('<%= txtNewPassword.ClientID %>', 'toggleNewPassword')" id="toggleNewPassword">
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                            <circle cx="12" cy="12" r="3"></circle>
                        </svg>
                    </button>
                </div>
                <asp:RequiredFieldValidator ID="rfvNewPassword" runat="server" ControlToValidate="txtNewPassword" 
                    ErrorMessage="New password is required" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revNewPassword" runat="server" ControlToValidate="txtNewPassword" 
                    ValidationExpression=".{6,}" 
                    ErrorMessage="Password must be at least 6 characters" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RegularExpressionValidator>
            </div>
            
            <div class="form-group">
                <label for="txtConfirmPassword">Confirm New Password <span class="required">*</span></label>
                <div class="input-wrapper">
                    <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Confirm new password"></asp:TextBox>
                    <button type="button" class="password-toggle" onclick="togglePasswordVisibility('<%= txtConfirmPassword.ClientID %>', 'toggleConfirmPassword')" id="toggleConfirmPassword">
                        <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                            <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"></path>
                            <circle cx="12" cy="12" r="3"></circle>
                        </svg>
                    </button>
                </div>
                <asp:RequiredFieldValidator ID="rfvConfirmPassword" runat="server" ControlToValidate="txtConfirmPassword" 
                    ErrorMessage="Please confirm your password" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:RequiredFieldValidator>
                <asp:CompareValidator ID="cvPassword" runat="server" ControlToValidate="txtConfirmPassword" 
                    ControlToCompare="txtNewPassword" 
                    ErrorMessage="Passwords do not match" CssClass="validation-error" Display="Dynamic" EnableClientScript="true"></asp:CompareValidator>
            </div>
            
            <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="btn-change-password" OnClick="btnChangePassword_Click" OnClientClick="showLoading()" />
            
            <asp:Label ID="lblError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
            <asp:Label ID="lblSuccess" runat="server" CssClass="success-message" Visible="false"></asp:Label>
        </div>
    </form>
</body>
</html>

