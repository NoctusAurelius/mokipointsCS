<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="mokipointsCS.ForgotPassword" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Forgot Password - MOKI POINTS</title>
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
        
        .forgot-container {
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
        
        .title {
            font-size: 24px;
            color: #333;
            text-align: center;
            margin-bottom: 15px;
        }
        
        .subtitle {
            font-size: 14px;
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
        
        .btn-submit {
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
        
        .btn-submit:hover {
            background-color: #0052a3;
            transform: translateY(-2px);
        }
        
        .back-to-login {
            text-align: center;
            margin-top: 30px;
        }
        
        .back-to-login a {
            color: #0066CC;
            text-decoration: none;
            font-size: 14px;
        }
        
        .back-to-login a:hover {
            text-decoration: underline;
        }
        
        .message {
            color: #666;
            font-size: 14px;
            margin-top: 10px;
            text-align: center;
            padding: 10px;
            border-radius: 5px;
        }
        
        .message-success {
            background-color: #d4edda;
            color: #155724;
        }
        
        .message-error {
            background-color: #f8d7da;
            color: #721c24;
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
        
        .btn-submit:disabled {
            background-color: #999;
            cursor: not-allowed;
            transform: none;
        }
    </style>
    <script>
        function showLoading() {
            try {
                console.log('ForgotPassword showLoading() called');
                var btn = document.getElementById('<%= btnSubmit.ClientID %>');
                console.log('Submit button found:', btn);
                if (btn) {
                    console.log('Button current state - disabled:', btn.disabled, 'value:', btn.value, 'innerHTML:', btn.innerHTML);
                    // Use setTimeout to delay disabling until after form submission starts
                    setTimeout(function() {
                        if (btn && !btn.disabled) {
                            btn.disabled = true;
                            // For input buttons, we need to change the value attribute
                            if (btn.tagName === 'INPUT') {
                                btn.value = 'Sending...';
                            } else {
                                btn.innerHTML = '<span class="spinner"></span>Sending...';
                            }
                            console.log('Button disabled and text updated (delayed)');
                        }
                    }, 10);
                } else {
                    console.error('Submit button not found!');
                }
            } catch (e) {
                console.error('Error in showLoading:', e);
            }
        }
        
        // Add page load logging
        window.addEventListener('load', function() {
            console.log('ForgotPassword Page loaded');
            var btn = document.getElementById('<%= btnSubmit.ClientID %>');
            console.log('Submit button on page load:', btn ? 'Found' : 'NOT FOUND', btn ? btn.id : 'N/A');
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="forgot-container">
            <div class="branding">
                <div class="branding-text">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
            </div>
            
            <h2 class="title">Forgot Password?</h2>
            <p class="subtitle">Enter your email address and we'll send you a verification code to reset your password.</p>
            
            <div class="form-group">
                <label for="txtEmail">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="Enter your email" required></asp:TextBox>
            </div>
            
            <asp:Button ID="btnSubmit" runat="server" Text="Send Verification Code" CssClass="btn-submit" OnClick="btnSubmit_Click" OnClientClick="console.log('ForgotPassword button clicked'); showLoading(); return true;" UseSubmitBehavior="true" />
            
            <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false"></asp:Label>
            
            <div class="back-to-login">
                <a href="Login.aspx">← Back to Login</a>
            </div>
        </div>
    </form>
</body>
</html>

