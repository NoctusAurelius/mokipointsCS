<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OTP.aspx.cs" Inherits="mokipointsCS.OTP" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Verify OTP - MOKI POINTS</title>
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
        
        .otp-container {
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
        
        .email-display {
            text-align: center;
            padding: 15px;
            background-color: #FFF9E6;
            border-left: 4px solid #FFB6C1;
            border-radius: 5px;
            margin-bottom: 30px;
        }
        
        .email-display strong {
            color: #0066CC;
        }
        
        .otp-input-group {
            margin-bottom: 30px;
        }
        
        .otp-input-group label {
            display: block;
            margin-bottom: 12px;
            color: #333;
            font-weight: 500;
            font-size: 14px;
            text-align: center;
        }
        
        .otp-input-wrapper {
            display: flex;
            justify-content: center;
            gap: 10px;
            margin-bottom: 20px;
        }
        
        .otp-digit {
            width: 60px;
            height: 70px;
            text-align: center;
            font-size: 32px;
            font-weight: bold;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            transition: all 0.3s;
            background-color: white;
            color: #0066CC;
        }
        
        .otp-digit:focus {
            outline: none;
            border-color: #0066CC;
            box-shadow: 0 0 0 3px rgba(0, 102, 204, 0.1);
        }
        
        .otp-help-text {
            text-align: center;
            color: #666;
            font-size: 14px;
            margin-bottom: 30px;
        }
        
        .btn-verify {
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
            margin-bottom: 20px;
        }
        
        .btn-verify:hover {
            background-color: #0052a3;
            transform: translateY(-2px);
        }
        
        .btn-verify:disabled {
            background-color: #999;
            cursor: not-allowed;
            transform: none;
        }
        
        .resend-section {
            text-align: center;
            padding-top: 20px;
            border-top: 1px solid #e0e0e0;
        }
        
        .resend-text {
            color: #666;
            font-size: 14px;
            margin-bottom: 10px;
        }
        
        .btn-resend {
            background: none;
            border: none;
            color: #0066CC;
            font-size: 14px;
            font-weight: bold;
            cursor: pointer;
            text-decoration: underline;
            padding: 5px;
        }
        
        .btn-resend:hover {
            color: #0052a3;
        }
        
        .btn-resend:disabled {
            color: #999;
            cursor: not-allowed;
            text-decoration: none;
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
        function setupOTPInputs() {
            var inputs = document.querySelectorAll('.otp-digit');
            var otpValue = document.getElementById('<%= hdnOTPValue.ClientID %>');
            
            inputs.forEach(function(input, index) {
                input.addEventListener('input', function(e) {
                    if (this.value.length > 1) {
                        this.value = this.value.slice(0, 1);
                    }
                    if (this.value && index < inputs.length - 1) {
                        inputs[index + 1].focus();
                    }
                    updateHiddenField();
                });
                
                input.addEventListener('keydown', function(e) {
                    if (e.key === 'Backspace' && !this.value && index > 0) {
                        inputs[index - 1].focus();
                    }
                });
                
                input.addEventListener('paste', function(e) {
                    e.preventDefault();
                    var paste = (e.clipboardData || window.clipboardData).getData('text');
                    if (paste.length === 6 && /^\d+$/.test(paste)) {
                        for (var i = 0; i < 6; i++) {
                            if (inputs[i]) {
                                inputs[i].value = paste[i];
                            }
                        }
                        updateHiddenField();
                        inputs[5].focus();
                    }
                });
            });
            
            function updateHiddenField() {
                var code = '';
                inputs.forEach(function(input) {
                    code += input.value || '';
                });
                if (otpValue) {
                    otpValue.value = code;
                }
            }
            
            // Focus first input on load
            if (inputs.length > 0) {
                inputs[0].focus();
            }
        }
        
        function showLoading() {
            try {
                console.log('OTP showLoading() called');
                var btn = document.getElementById('<%= btnVerify.ClientID %>');
                console.log('Verify button found:', btn);
                if (btn) {
                    console.log('Button current state - disabled:', btn.disabled, 'value:', btn.value, 'innerHTML:', btn.innerHTML);
                    // Use setTimeout to delay disabling until after form submission starts
                    setTimeout(function() {
                        if (btn && !btn.disabled) {
                            btn.disabled = true;
                            // For input buttons, we need to change the value attribute
                            if (btn.tagName === 'INPUT') {
                                btn.value = 'Verifying...';
                            } else {
                                btn.innerHTML = '<span class="spinner"></span>Verifying...';
                            }
                            console.log('Button disabled and text updated (delayed)');
                        }
                    }, 10);
                } else {
                    console.error('Verify button not found!');
                }
            } catch (e) {
                console.error('Error in showLoading:', e);
            }
        }
        
        // Add page load logging
        window.addEventListener('load', function() {
            console.log('OTP Page loaded');
            var btn = document.getElementById('<%= btnVerify.ClientID %>');
            console.log('Verify button on page load:', btn ? 'Found' : 'NOT FOUND', btn ? btn.id : 'N/A');
        });
        
        document.addEventListener('DOMContentLoaded', function() {
            setupOTPInputs();
        });
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="otp-container">
            <div class="branding">
                <div class="branding-text">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
            </div>
            
            <h2 class="page-title">Verify Your Email</h2>
            <p class="page-subtitle">Enter the 6-digit code sent to your email</p>
            
            <div class="email-display">
                <strong>Email:</strong> <asp:Literal ID="litEmail" runat="server"></asp:Literal>
            </div>
            
            <div class="otp-input-group">
                <label>Verification Code</label>
                <div class="otp-input-wrapper">
                    <asp:TextBox ID="txtOTP1" runat="server" CssClass="otp-digit" MaxLength="1" TextMode="SingleLine" autocomplete="off"></asp:TextBox>
                    <asp:TextBox ID="txtOTP2" runat="server" CssClass="otp-digit" MaxLength="1" TextMode="SingleLine" autocomplete="off"></asp:TextBox>
                    <asp:TextBox ID="txtOTP3" runat="server" CssClass="otp-digit" MaxLength="1" TextMode="SingleLine" autocomplete="off"></asp:TextBox>
                    <asp:TextBox ID="txtOTP4" runat="server" CssClass="otp-digit" MaxLength="1" TextMode="SingleLine" autocomplete="off"></asp:TextBox>
                    <asp:TextBox ID="txtOTP5" runat="server" CssClass="otp-digit" MaxLength="1" TextMode="SingleLine" autocomplete="off"></asp:TextBox>
                    <asp:TextBox ID="txtOTP6" runat="server" CssClass="otp-digit" MaxLength="1" TextMode="SingleLine" autocomplete="off"></asp:TextBox>
                </div>
                <asp:HiddenField ID="hdnOTPValue" runat="server" />
                <p class="otp-help-text">This code will expire in 10 minutes</p>
            </div>
            
            <asp:Button ID="btnVerify" runat="server" Text="Verify Code" CssClass="btn-verify" OnClick="btnVerify_Click" OnClientClick="console.log('Verify button clicked'); showLoading(); return true;" UseSubmitBehavior="true" />
            
            <asp:Label ID="lblError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
            <asp:Label ID="lblSuccess" runat="server" CssClass="success-message" Visible="false"></asp:Label>
            
            <div class="resend-section">
                <p class="resend-text">Didn't receive the code?</p>
                <asp:Button ID="btnResend" runat="server" Text="Resend Code" CssClass="btn-resend" OnClick="btnResend_Click" />
            </div>
        </div>
    </form>
</body>
</html>

