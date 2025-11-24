<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error400.aspx.cs" Inherits="mokipointsCS.Error400" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>400 - Bad Request | MOKI POINTS</title>
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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="error-container">
            <div class="error-code">
                <span class="moki">400</span>
            </div>
            <div class="branding">
                <span class="moki">MOKI</span><span class="points"> POINTS</span>
            </div>
            <h1 class="error-title">Bad Request</h1>
            <p class="error-message">The request you sent was invalid or malformed. Please check your input and try again.</p>
            <a href="Default.aspx" class="btn-home">Go to Home</a>
        </div>
    </form>
</body>
</html>

