<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error500.aspx.cs" Inherits="mokipointsCS.Error500" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>500 - Server Error | MOKI POINTS</title>
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
        
        .error-container {
            text-align: center;
            max-width: 600px;
        }
        
        .error-code {
            font-size: 120px;
            font-weight: bold;
            line-height: 1;
            margin-bottom: 20px;
        }
        
        .error-code .moki {
            color: #0066CC; /* Blue */
        }
        
        .error-code .points {
            color: #FF6600; /* Orange */
        }
        
        .branding {
            font-size: 48px;
            font-weight: bold;
            letter-spacing: 4px;
            margin-bottom: 30px;
        }
        
        .branding .moki {
            color: #0066CC;
        }
        
        .branding .points {
            color: #FF6600;
        }
        
        .error-title {
            font-size: 32px;
            color: #333;
            margin-bottom: 15px;
        }
        
        .error-message {
            font-size: 18px;
            color: #666;
            margin-bottom: 40px;
            line-height: 1.6;
        }
        
        .btn-home {
            display: inline-block;
            padding: 15px 40px;
            background-color: #0066CC;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            font-size: 18px;
            font-weight: bold;
            transition: background-color 0.3s, transform 0.2s;
        }
        
        .btn-home:hover {
            background-color: #0052a3;
            transform: translateY(-2px);
            text-decoration: none;
            color: white;
        }
        
        .error-details {
            background-color: #fff3cd;
            border: 1px solid #ffc107;
            border-radius: 5px;
            padding: 20px;
            margin: 30px 0;
            text-align: left;
            max-width: 100%;
            overflow-x: auto;
        }
        
        .error-details h3 {
            color: #856404;
            margin-bottom: 15px;
            font-size: 20px;
        }
        
        .error-detail-item {
            margin-bottom: 15px;
            color: #333;
        }
        
        .error-detail-item strong {
            color: #856404;
            display: block;
            margin-bottom: 5px;
        }
        
        .stack-trace {
            background-color: #f8f9fa;
            border: 1px solid #dee2e6;
            border-radius: 3px;
            padding: 10px;
            font-family: 'Courier New', monospace;
            font-size: 12px;
            white-space: pre-wrap;
            word-wrap: break-word;
            max-height: 300px;
            overflow-y: auto;
            color: #333;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="error-container">
            <div class="error-code">
                <span class="moki">500</span>
            </div>
            <div class="branding">
                <span class="moki">MOKI</span><span class="points"> POINTS</span>
            </div>
            <h1 class="error-title">Server Error</h1>
            <p class="error-message">We're experiencing some technical difficulties. Our team has been notified and is working to fix the issue.</p>
            
            <!-- Error Details (for debugging) -->
            <asp:Panel ID="pnlErrorDetails" runat="server" Visible="false" CssClass="error-details">
                <h3>Error Details:</h3>
                <div class="error-detail-item">
                    <strong>Error Type:</strong> <asp:Literal ID="litErrorType" runat="server"></asp:Literal>
                </div>
                <div class="error-detail-item">
                    <strong>Error Message:</strong> <asp:Literal ID="litErrorMessage" runat="server"></asp:Literal>
                </div>
                <asp:Panel ID="pnlInnerError" runat="server" Visible="false">
                    <div class="error-detail-item">
                        <strong>Inner Exception:</strong> <asp:Literal ID="litInnerError" runat="server"></asp:Literal>
                    </div>
                </asp:Panel>
                <div class="error-detail-item">
                    <strong>Source:</strong> <asp:Literal ID="litErrorSource" runat="server"></asp:Literal>
                </div>
                <div class="error-detail-item">
                    <strong>Stack Trace:</strong>
                    <pre class="stack-trace"><asp:Literal ID="litStackTrace" runat="server"></asp:Literal></pre>
                </div>
            </asp:Panel>
            
            <a href="Default.aspx" class="btn-home">Go to Home</a>
        </div>
    </form>
</body>
</html>

