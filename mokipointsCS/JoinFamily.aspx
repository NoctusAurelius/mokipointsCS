<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JoinFamily.aspx.cs" Inherits="mokipointsCS.JoinFamily" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Join Family - MOKI POINTS</title>
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
        
        .join-container {
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
            color: #0066CC;
        }
        
        .branding .points {
            color: #FF6600;
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
        
        .form-control {
            width: 100%;
            padding: 12px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 5px;
            font-size: 16px;
            transition: border-color 0.3s;
            text-align: center;
            letter-spacing: 4px;
            font-size: 24px;
            font-weight: bold;
            text-transform: uppercase;
        }
        
        .form-control:focus {
            outline: none;
            border-color: #0066CC;
        }
        
        .code-hint {
            text-align: center;
            color: #666;
            font-size: 14px;
            margin-top: 10px;
        }
        
        .btn-join {
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
            margin-top: 20px;
        }
        
        .btn-join:hover {
            background-color: #0052a3;
        }
        
        .btn-join:disabled {
            background-color: #999;
            cursor: not-allowed;
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
        
        .info-box {
            background-color: #FFF9E6;
            border-left: 4px solid #FFB6C1;
            border-radius: 5px;
            padding: 15px;
            margin-bottom: 30px;
        }
        
        .info-box p {
            color: #666;
            font-size: 14px;
            line-height: 1.6;
            margin: 0;
        }
    </style>
    <script>
        function formatFamilyCode(input) {
            // Remove non-alphanumeric characters
            var value = input.value.replace(/[^A-Z0-9]/gi, '').toUpperCase();
            
            // Limit to 6 characters (2 letters + 4 digits)
            if (value.length > 6) {
                value = value.substring(0, 6);
            }
            
            input.value = value;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="join-container">
            <div class="branding">
                <div class="branding-text">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
            </div>
            
            <h2 class="page-title">Join Your Family</h2>
            <p class="page-subtitle">Enter the family code to get started</p>
            
            <div class="info-box">
                <p><strong>Welcome!</strong> To join your family, ask a parent for the 6-character family code (e.g., LP2222).</p>
            </div>
            
            <div class="form-group">
                <label for="txtFamilyCode">Family Code</label>
                <asp:TextBox ID="txtFamilyCode" runat="server" CssClass="form-control" MaxLength="6" 
                    placeholder="LP2222" oninput="formatFamilyCode(this)"></asp:TextBox>
                <p class="code-hint">Enter 2 letters and 4 numbers</p>
                <asp:RequiredFieldValidator ID="rfvFamilyCode" runat="server" ControlToValidate="txtFamilyCode" 
                    ErrorMessage="Family code is required" CssClass="error-message" Display="Dynamic"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="revFamilyCode" runat="server" ControlToValidate="txtFamilyCode" 
                    ValidationExpression="^[A-Z]{2}\d{4}$" ErrorMessage="Code must be 2 letters followed by 4 numbers (e.g., LP2222)" 
                    CssClass="error-message" Display="Dynamic"></asp:RegularExpressionValidator>
            </div>
            
            <asp:Button ID="btnJoin" runat="server" Text="Join Family" CssClass="btn-join" OnClick="btnJoin_Click" />
            
            <asp:Label ID="lblError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
            <asp:Label ID="lblSuccess" runat="server" CssClass="success-message" Visible="false"></asp:Label>
        </div>
    </form>
</body>
</html>

