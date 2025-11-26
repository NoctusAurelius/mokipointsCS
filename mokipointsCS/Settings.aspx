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

        /* Logout Confirmation Modal */
        .logout-modal {
            display: none;
            position: fixed;
            z-index: 10000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgba(0, 0, 0, 0.6);
            animation: fadeIn 0.3s ease;
        }

        .logout-modal.active {
            display: flex;
            align-items: center;
            justify-content: center;
            padding: 20px;
        }

        .logout-modal-content {
            background-color: white;
            margin: auto;
            padding: 0;
            border-radius: 15px;
            max-width: 450px;
            width: 100%;
            box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
            animation: slideDown 0.4s ease;
            overflow: hidden;
        }

        @keyframes fadeIn {
            from {
                opacity: 0;
            }
            to {
                opacity: 1;
            }
        }

        @keyframes slideDown {
            from {
                transform: translateY(-50px);
                opacity: 0;
            }
            to {
                transform: translateY(0);
                opacity: 1;
            }
        }

        @keyframes shake {
            0%, 100% {
                transform: translateX(0);
            }
            10%, 30%, 50%, 70%, 90% {
                transform: translateX(-5px);
            }
            20%, 40%, 60%, 80% {
                transform: translateX(5px);
            }
        }

        .logout-modal-header {
            background: linear-gradient(135deg, #ff6b6b 0%, #ee5a6f 100%);
            padding: 30px;
            text-align: center;
            position: relative;
        }

        .logout-modal-icon {
            width: 80px;
            height: 80px;
            margin: 0 auto 20px;
            background-color: white;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 40px;
            color: #ff6b6b;
            animation: pulse 2s ease-in-out infinite;
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.2);
        }

        @keyframes pulse {
            0%, 100% {
                transform: scale(1);
            }
            50% {
                transform: scale(1.1);
            }
        }

        .logout-modal-title {
            color: white;
            font-size: 24px;
            font-weight: bold;
            margin-bottom: 10px;
        }

        .logout-modal-body {
            padding: 30px;
            text-align: center;
        }

        .logout-modal-message {
            color: #333;
            font-size: 16px;
            line-height: 1.6;
            margin-bottom: 30px;
        }

        .logout-modal-buttons {
            display: flex;
            gap: 15px;
            justify-content: center;
        }

        .logout-btn {
            padding: 12px 30px;
            font-size: 16px;
            font-weight: bold;
            border: none;
            border-radius: 8px;
            cursor: pointer;
            transition: all 0.3s ease;
            min-width: 120px;
        }

        .logout-btn-cancel {
            background-color: #e0e0e0;
            color: #333;
        }

        .logout-btn-cancel:hover {
            background-color: #d0d0d0;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
        }

        .logout-btn-confirm {
            background: linear-gradient(135deg, #ff6b6b 0%, #ee5a6f 100%);
            color: white;
        }

        .logout-btn-confirm:hover {
            background: linear-gradient(135deg, #ee5a6f 0%, #ff6b6b 100%);
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(255, 107, 107, 0.4);
        }

        .logout-btn-confirm:active {
            transform: translateY(0);
            animation: shake 0.5s ease;
        }
    </style>
    <script>
        function showLogoutConfirmation() {
            var modal = document.getElementById('logoutModal');
            if (modal) {
                modal.classList.add('active');
                document.body.style.overflow = 'hidden';
            }
        }

        function closeLogoutModal() {
            var modal = document.getElementById('logoutModal');
            if (modal) {
                modal.classList.remove('active');
                document.body.style.overflow = 'auto';
            }
        }

        function confirmLogout() {
            var btnLogout = document.getElementById('<%= btnLogout.ClientID %>');
            if (btnLogout) {
                closeLogoutModal();
                btnLogout.click();
            }
        }

        // Close modal on backdrop click
        function closeLogoutModalOnBackdrop(event) {
            if (event.target.classList.contains('logout-modal')) {
                closeLogoutModal();
            }
        }

        // Close modal with Escape key
        document.addEventListener('keydown', function(event) {
            if (event.key === 'Escape') {
                closeLogoutModal();
            }
        });

        // Set warning icon using Unicode to avoid encoding issues
        document.addEventListener('DOMContentLoaded', function() {
            var warningIcon = document.querySelector('.logout-modal-icon');
            if (warningIcon) {
                // Warning sign: U+26A0 (⚠)
                warningIcon.textContent = String.fromCharCode(0x26A0);
            }
        });
    </script>
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

                <!-- Logout -->
                <div class="settings-item" onclick="showLogoutConfirmation();">
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

        <!-- Logout Confirmation Modal -->
        <div id="logoutModal" class="logout-modal" onclick="closeLogoutModalOnBackdrop(event);">
            <div class="logout-modal-content" onclick="event.stopPropagation();">
                <div class="logout-modal-header">
                    <div class="logout-modal-icon"></div>
                    <div class="logout-modal-title">Confirm Logout</div>
                </div>
                <div class="logout-modal-body">
                    <div class="logout-modal-message">
                        Are you sure you want to logout?<br />
                        You will need to login again to access your account.
                    </div>
                    <div class="logout-modal-buttons">
                        <button type="button" class="logout-btn logout-btn-cancel" onclick="closeLogoutModal();">Cancel</button>
                        <button type="button" class="logout-btn logout-btn-confirm" onclick="confirmLogout();">Logout</button>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>

