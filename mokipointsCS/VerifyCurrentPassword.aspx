<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerifyCurrentPassword.aspx.cs" Inherits="mokipointsCS.VerifyCurrentPassword" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Verify Password - MOKI POINTS</title>
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
            background-color: #f5f5f5;
            color: #333;
        }
        
        /* Header */
        .header {
            background-color: white;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            padding: 15px 0;
            margin-bottom: 30px;
        }
        
        .header-content {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 30px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        
        .brand {
            font-size: 24px;
            font-weight: bold;
            letter-spacing: 3px;
        }
        
        .brand .moki {
            color: #0066CC;
        }
        
        .brand .points {
            color: #FF6600;
        }
        
        .nav-links {
            display: flex;
            gap: 20px;
            align-items: center;
        }
        
        .nav-links a {
            color: #333;
            text-decoration: none;
            font-weight: 500;
        }
        
        .nav-links a:hover {
            color: #0066CC;
        }
        
        /* Main Container */
        .container {
            max-width: 500px;
            margin: 0 auto;
            padding: 0 30px;
        }
        
        .page-title {
            font-size: 32px;
            color: #333;
            margin-bottom: 10px;
            text-align: center;
        }
        
        .page-subtitle {
            color: #666;
            font-size: 16px;
            margin-bottom: 30px;
            text-align: center;
        }
        
        /* Form Card */
        .form-card {
            background-color: white;
            border-radius: 10px;
            padding: 40px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
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
        
        .password-toggle {
            position: relative;
        }
        
        .password-toggle-btn {
            position: absolute;
            right: 15px;
            top: 50%;
            transform: translateY(-50%);
            background: none;
            border: none;
            cursor: pointer;
            font-size: 18px;
            color: #666;
        }
        
        .btn-submit {
            width: 100%;
            padding: 14px;
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            font-size: 18px;
            font-weight: bold;
            cursor: pointer;
            transition: background-color 0.3s;
            margin-top: 10px;
        }
        
        .btn-submit:hover {
            background-color: #0052a3;
        }
        
        .btn-forgot-password {
            width: 100%;
            padding: 12px;
            background-color: transparent;
            color: #0066CC;
            border: 2px solid #0066CC;
            border-radius: 5px;
            font-size: 16px;
            font-weight: 500;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            text-align: center;
            transition: all 0.3s;
            margin-top: 15px;
        }
        
        .btn-forgot-password:hover {
            background-color: #0066CC;
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Header -->
        <div class="header">
            <div class="header-content">
                <div class="brand">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
                <div class="nav-links">
                    <a href="Profile.aspx">Back to Profile</a>
                </div>
            </div>
        </div>

        <!-- Main Content -->
        <div class="container">
            <h1 class="page-title">Change Password</h1>
            <p class="page-subtitle">Please enter your current password to continue</p>

            <div class="form-card">
                <asp:Label ID="lblError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                <asp:Label ID="lblSuccess" runat="server" CssClass="success-message" Visible="false"></asp:Label>

                <div class="form-group">
                    <label for="txtCurrentPassword">Current Password</label>
                    <div class="password-toggle">
                        <asp:TextBox ID="txtCurrentPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Enter your current password"></asp:TextBox>
                        <button type="button" class="password-toggle-btn" onclick="togglePassword('txtCurrentPassword', this)">Show</button>
                    </div>
                    <asp:RequiredFieldValidator ID="rfvCurrentPassword" runat="server" ControlToValidate="txtCurrentPassword" 
                        ErrorMessage="Current password is required" CssClass="error-message" Display="Dynamic" ValidationGroup="VerifyPassword"></asp:RequiredFieldValidator>
                </div>

                <asp:Button ID="btnVerify" runat="server" Text="Continue" CssClass="btn-submit" OnClick="btnVerify_Click" ValidationGroup="VerifyPassword" />
                
                <asp:Button ID="btnForgotPassword" runat="server" Text="Forgot Password?" CssClass="btn-forgot-password" OnClick="btnForgotPassword_Click" CausesValidation="false" />
            </div>
        </div>
    </form>

    <script>
        function togglePassword(textBoxId, button) {
            var textBox = document.getElementById(textBoxId);
            if (textBox.type === "password") {
                textBox.type = "text";
                button.textContent = "Hide";
            } else {
                textBox.type = "password";
                button.textContent = "Show";
            }
        }
    </script>
</body>
</html>

