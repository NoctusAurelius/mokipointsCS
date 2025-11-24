<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermsAndConditions.aspx.cs" Inherits="mokipointsCS.TermsAndConditions" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Terms and Conditions - MOKI POINTS</title>
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
        
        .container {
            max-width: 900px;
            margin: 0 auto;
            background-color: white;
            padding: 40px;
            border-radius: 10px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.1);
        }
        
        .branding {
            text-align: center;
            margin-bottom: 30px;
        }
        
        .branding-text {
            font-size: 36px;
            font-weight: bold;
            letter-spacing: 3px;
            margin-bottom: 10px;
        }
        
        .branding .moki {
            color: #0066CC; /* Blue */
        }
        
        .branding .points {
            color: #FF6600; /* Orange */
        }
        
        h1 {
            color: #333;
            margin-bottom: 30px;
            font-size: 32px;
        }
        
        h2 {
            color: #0066CC;
            margin-top: 30px;
            margin-bottom: 15px;
            font-size: 24px;
        }
        
        p {
            line-height: 1.8;
            margin-bottom: 15px;
            color: #666;
        }
        
        ul {
            margin-left: 30px;
            margin-bottom: 20px;
            line-height: 1.8;
            color: #666;
        }
        
        li {
            margin-bottom: 10px;
        }
        
        .back-link {
            text-align: center;
            margin-top: 40px;
            padding-top: 20px;
            border-top: 1px solid #e0e0e0;
        }
        
        .back-link a {
            color: #0066CC;
            text-decoration: none;
            font-size: 16px;
        }
        
        .back-link a:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="branding">
                <div class="branding-text">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
            </div>
            
            <h1>Terms and Conditions</h1>
            
            <p><strong>Last Updated: March 2024</strong></p>
            
            <h2>1. Acceptance of Terms</h2>
            <p>By accessing and using MOKI POINTS, you accept and agree to be bound by the terms and provision of this agreement.</p>
            
            <h2>2. Use License</h2>
            <p>Permission is granted to temporarily use MOKI POINTS for personal, non-commercial use only. This is the grant of a license, not a transfer of title, and under this license you may not:</p>
            <ul>
                <li>Modify or copy the materials</li>
                <li>Use the materials for any commercial purpose</li>
                <li>Attempt to decompile or reverse engineer any software</li>
                <li>Remove any copyright or other proprietary notations</li>
            </ul>
            
            <h2>3. User Accounts</h2>
            <p>You are responsible for maintaining the confidentiality of your account and password. You agree to accept responsibility for all activities that occur under your account.</p>
            
            <h2>4. User Responsibilities</h2>
            <p>Users agree to:</p>
            <ul>
                <li>Provide accurate and complete information when registering</li>
                <li>Keep account information updated</li>
                <li>Maintain the security of your password</li>
                <li>Notify us immediately of any unauthorized use</li>
            </ul>
            
            <h2>5. Points System</h2>
            <p>Points earned through completing chores are virtual and have no monetary value. Points may be adjusted or reset at the discretion of account administrators (parents).</p>
            
            <h2>6. Limitation of Liability</h2>
            <p>MOKI POINTS shall not be liable for any indirect, incidental, special, consequential, or punitive damages resulting from your use of the service.</p>
            
            <h2>7. Changes to Terms</h2>
            <p>We reserve the right to modify these terms at any time. Your continued use of the service after changes constitutes acceptance of the new terms.</p>
            
            <h2>8. Contact Information</h2>
            <p>If you have any questions about these Terms and Conditions, please contact us at:</p>
            <p>Email: info.mokipoin@gmail.com<br />
            Phone: +63 62 794 1298</p>
            
            <div class="back-link">
                <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="Register.aspx">← Back to Registration</asp:HyperLink>
            </div>
        </div>
    </form>
</body>
</html>

