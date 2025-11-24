<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Settings.aspx.cs" Inherits="mokipointsCS.Settings" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Settings - MOKI POINTS</title>
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
            max-width: 800px;
            margin: 0 auto;
            padding: 0 30px;
        }
        
        .page-title {
            font-size: 32px;
            color: #333;
            margin-bottom: 10px;
        }
        
        .page-subtitle {
            color: #666;
            font-size: 16px;
            margin-bottom: 30px;
        }
        
        /* Settings Card */
        .settings-card {
            background-color: white;
            border-radius: 10px;
            padding: 0;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin-bottom: 30px;
        }
        
        .settings-item {
            padding: 20px 30px;
            border-bottom: 1px solid #e0e0e0;
            display: flex;
            justify-content: space-between;
            align-items: center;
            transition: background-color 0.3s;
            cursor: pointer;
        }
        
        .settings-item:last-child {
            border-bottom: none;
        }
        
        .settings-item:hover {
            background-color: #f9f9f9;
        }
        
        .settings-item-content {
            display: flex;
            align-items: center;
            gap: 15px;
            flex: 1;
        }
        
        .settings-icon {
            width: 40px;
            height: 40px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 18px;
            font-weight: bold;
            flex-shrink: 0;
        }
        
        .settings-icon.profile {
            background-color: #e3f2fd;
            color: #0066CC;
        }
        
        .settings-icon.terms {
            background-color: #fff3e0;
            color: #FF6600;
        }
        
        .settings-icon.privacy {
            background-color: #f3e5f5;
            color: #9c27b0;
        }
        
        .settings-icon.notifications {
            background-color: #e8f5e9;
            color: #2e7d32;
        }
        
        .settings-icon.logout {
            background-color: #ffebee;
            color: #d32f2f;
        }
        
        .settings-text {
            flex: 1;
        }
        
        .settings-title {
            font-size: 18px;
            font-weight: 500;
            color: #333;
            margin-bottom: 5px;
        }
        
        .settings-description {
            font-size: 14px;
            color: #666;
        }
        
        .settings-arrow {
            color: #999;
            font-size: 20px;
            flex-shrink: 0;
            width: 0;
            height: 0;
            border-left: 8px solid #999;
            border-top: 6px solid transparent;
            border-bottom: 6px solid transparent;
        }
        
        .btn-settings-item {
            background: none;
            border: none;
            width: 100%;
            text-align: left;
            cursor: pointer;
            padding: 0;
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
                    <a href="Dashboard.aspx">Dashboard</a>
                </div>
            </div>
        </div>

        <!-- Main Content -->
        <div class="container">
            <h1 class="page-title">Settings</h1>
            <p class="page-subtitle">Manage your account preferences and settings</p>

            <!-- Settings Options -->
            <div class="settings-card">
                <!-- Profile -->
                <div class="settings-item" onclick="document.getElementById('<%= btnProfile.ClientID %>').click();">
                    <div class="settings-item-content">
                        <div class="settings-icon profile">U</div>
                        <div class="settings-text">
                            <div class="settings-title">Profile</div>
                            <div class="settings-description">View and edit your profile information</div>
                        </div>
                    </div>
                    <div class="settings-arrow"></div>
                    <asp:Button ID="btnProfile" runat="server" OnClick="btnProfile_Click" style="display: none;" />
                </div>

                <!-- Terms and Service -->
                <div class="settings-item" onclick="document.getElementById('<%= btnTerms.ClientID %>').click();">
                    <div class="settings-item-content">
                        <div class="settings-icon terms">T</div>
                        <div class="settings-text">
                            <div class="settings-title">Terms and Service</div>
                            <div class="settings-description">Read our terms and conditions</div>
                        </div>
                    </div>
                    <div class="settings-arrow"></div>
                    <asp:Button ID="btnTerms" runat="server" OnClick="btnTerms_Click" style="display: none;" />
                </div>

                <!-- Privacy Policy -->
                <div class="settings-item" onclick="document.getElementById('<%= btnPrivacy.ClientID %>').click();">
                    <div class="settings-item-content">
                        <div class="settings-icon privacy">P</div>
                        <div class="settings-text">
                            <div class="settings-title">Privacy Policy</div>
                            <div class="settings-description">Learn how we protect your privacy</div>
                        </div>
                    </div>
                    <div class="settings-arrow"></div>
                    <asp:Button ID="btnPrivacy" runat="server" OnClick="btnPrivacy_Click" style="display: none;" />
                </div>

                <!-- Notifications Preference -->
                <div class="settings-item" onclick="document.getElementById('<%= btnNotifications.ClientID %>').click();">
                    <div class="settings-item-content">
                        <div class="settings-icon notifications">N</div>
                        <div class="settings-text">
                            <div class="settings-title">Notifications Preference</div>
                            <div class="settings-description">Manage your notification settings</div>
                        </div>
                    </div>
                    <div class="settings-arrow"></div>
                    <asp:Button ID="btnNotifications" runat="server" OnClick="btnNotifications_Click" style="display: none;" />
                </div>

                <!-- Logout -->
                <div class="settings-item" onclick="document.getElementById('<%= btnLogout.ClientID %>').click();">
                    <div class="settings-item-content">
                        <div class="settings-icon logout">X</div>
                        <div class="settings-text">
                            <div class="settings-title">Logout</div>
                            <div class="settings-description">Sign out of your account</div>
                        </div>
                    </div>
                    <div class="settings-arrow"></div>
                    <asp:Button ID="btnLogout" runat="server" OnClick="btnLogout_Click" style="display: none;" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>

