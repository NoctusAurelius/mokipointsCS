<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrivacyPolicy.aspx.cs" Inherits="mokipointsCS.PrivacyPolicy" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Privacy Policy - MOKI POINTS</title>
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
            
            <h1>Privacy Policy</h1>
            
            <p><strong>Last Updated: March 2024</strong></p>
            
            <h2>1. Information We Collect</h2>
            <p>We collect information that you provide directly to us, including:</p>
            <ul>
                <li>Name, email address, and contact information</li>
                <li>Birthday and role (Parent or Child)</li>
                <li>Account credentials (password is securely hashed)</li>
                <li>Chore completion data and point transactions</li>
            </ul>
            
            <h2>2. How We Use Your Information</h2>
            <p>We use the information we collect to:</p>
            <ul>
                <li>Provide and maintain our service</li>
                <li>Track chores and manage point systems</li>
                <li>Communicate with you about your account</li>
                <li>Improve our services</li>
            </ul>
            
            <h2>3. Data Storage and Security</h2>
            <p>Your data is stored securely in our database. We implement appropriate technical and organizational measures to protect your personal information against unauthorized access, alteration, disclosure, or destruction.</p>
            
            <h2>4. Data Sharing</h2>
            <p>We do not sell, trade, or rent your personal information to third parties. Your data is only accessible within your family account.</p>
            
            <h2>5. Children's Privacy</h2>
            <p>MOKI POINTS is designed for family use. Parents are responsible for managing their children's accounts and data. We do not knowingly collect personal information from children without parental consent.</p>
            
            <h2>6. Your Rights</h2>
            <p>You have the right to:</p>
            <ul>
                <li>Access your personal information</li>
                <li>Correct inaccurate data</li>
                <li>Request deletion of your account</li>
                <li>Export your data</li>
            </ul>
            
            <h2>7. Cookies and Tracking</h2>
            <p>We use session cookies to maintain your login state. These cookies are essential for the functionality of the service.</p>
            
            <h2>8. Changes to Privacy Policy</h2>
            <p>We may update this Privacy Policy from time to time. We will notify you of any changes by posting the new Privacy Policy on this page.</p>
            
            <h2>9. Contact Us</h2>
            <p>If you have any questions about this Privacy Policy, please contact us at:</p>
            <p>Email: info.mokipoin@gmail.com<br />
            Phone: +63 62 794 1298</p>
            
            <div class="back-link">
                <asp:HyperLink ID="lnkBack" runat="server" NavigateUrl="Register.aspx">← Back to Registration</asp:HyperLink>
            </div>
        </div>
    </form>
</body>
</html>

