<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="mokipointsCS.Profile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Profile - MOKI POINTS</title>
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
            max-width: 900px;
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
        
        /* Profile Card */
        .profile-card {
            background-color: white;
            border-radius: 10px;
            padding: 40px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin-bottom: 30px;
        }
        
        .profile-header {
            display: flex;
            align-items: center;
            gap: 30px;
            margin-bottom: 40px;
            padding-bottom: 30px;
            border-bottom: 2px solid #e0e0e0;
        }
        
        .profile-picture-container {
            position: relative;
            flex-shrink: 0;
        }
        
        .profile-picture {
            width: 150px;
            height: 150px;
            border-radius: 50%;
            object-fit: cover;
            border: 4px solid #0066CC;
            background-color: #e3f2fd;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 60px;
            color: #0066CC;
        }
        
        .profile-picture-placeholder {
            width: 150px;
            height: 150px;
            border-radius: 50%;
            background-color: #e3f2fd;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 60px;
            font-weight: bold;
            color: #0066CC;
            border: 4px solid #0066CC;
        }
        
        .upload-overlay {
            position: absolute;
            bottom: 0;
            right: 0;
            width: 45px;
            height: 45px;
            background-color: #0066CC;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            border: 3px solid white;
            transition: background-color 0.3s;
        }
        
        .upload-overlay:hover {
            background-color: #0052a3;
        }
        
        .upload-icon {
            color: white;
            font-size: 24px;
            font-weight: bold;
        }
        
        .profile-info {
            flex: 1;
        }
        
        .profile-name {
            font-size: 28px;
            font-weight: bold;
            color: #333;
            margin-bottom: 10px;
        }
        
        .profile-email {
            font-size: 16px;
            color: #666;
            margin-bottom: 5px;
        }
        
        .profile-role {
            display: inline-block;
            padding: 5px 15px;
            background-color: #e3f2fd;
            color: #0066CC;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 500;
            margin-top: 10px;
        }
        
        /* Info Section */
        .info-section {
            margin-bottom: 30px;
        }
        
        .section-title {
            font-size: 20px;
            font-weight: bold;
            color: #333;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid #e0e0e0;
        }
        
        .info-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
        }
        
        .info-item {
            padding: 15px;
            background-color: #f9f9f9;
            border-radius: 5px;
        }
        
        .info-label {
            font-size: 12px;
            color: #999;
            text-transform: uppercase;
            margin-bottom: 5px;
        }
        
        .info-value {
            font-size: 16px;
            color: #333;
            font-weight: 500;
        }
        
        /* Buttons */
        .btn-change-password {
            padding: 12px 30px;
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            font-size: 16px;
            font-weight: bold;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            transition: background-color 0.3s;
        }
        
        .btn-change-password:hover {
            background-color: #0052a3;
        }
        
        /* Messages */
        .message {
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
            font-size: 14px;
        }
        
        .message-success {
            background-color: #e8f5e9;
            color: #2e7d32;
            border-left: 4px solid #2e7d32;
        }
        
        .message-error {
            background-color: #ffebee;
            color: #d32f2f;
            border-left: 4px solid #d32f2f;
        }
        
        /* Hidden file input */
        #fileUpload {
            display: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server" enctype="multipart/form-data">
        <!-- Header -->
        <div class="header">
            <div class="header-content">
                <div class="brand">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
                <div class="nav-links">
                    <a href="Dashboard.aspx">Dashboard</a>
                    <a href="Settings.aspx">Settings</a>
                </div>
            </div>
        </div>

        <!-- Main Content -->
        <div class="container">
            <h1 class="page-title">Profile</h1>
            <p class="page-subtitle">Manage your profile information</p>

            <!-- Messages -->
            <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false"></asp:Label>

            <!-- Profile Card -->
            <div class="profile-card">
                <!-- Profile Header -->
                <div class="profile-header">
                    <div class="profile-picture-container">
                        <asp:Image ID="imgProfilePicture" runat="server" CssClass="profile-picture" Visible="false" />
                        <div id="profilePlaceholder" runat="server" class="profile-picture-placeholder">U</div>
                        <div class="upload-overlay" onclick="document.getElementById('<%= fileUpload.ClientID %>').click();">
                            <span class="upload-icon">+</span>
                        </div>
                        <asp:FileUpload ID="fileUpload" runat="server" accept="image/*" onchange="uploadProfilePicture();" />
                    </div>
                    <div class="profile-info">
                        <div class="profile-name">
                            <asp:Literal ID="litFullName" runat="server"></asp:Literal>
                        </div>
                        <div class="profile-email">
                            <asp:Literal ID="litEmail" runat="server"></asp:Literal>
                        </div>
                        <div class="profile-role">
                            <asp:Literal ID="litRole" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>

                <!-- General Information -->
                <div class="info-section">
                    <h2 class="section-title">General Information</h2>
                    <div class="info-grid">
                        <div class="info-item">
                            <div class="info-label">First Name</div>
                            <div class="info-value">
                                <asp:Literal ID="litFirstName" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="info-item">
                            <div class="info-label">Last Name</div>
                            <div class="info-value">
                                <asp:Literal ID="litLastName" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="info-item">
                            <div class="info-label">Middle Name</div>
                            <div class="info-value">
                                <asp:Literal ID="litMiddleName" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="info-item">
                            <div class="info-label">Birthday</div>
                            <div class="info-value">
                                <asp:Literal ID="litBirthday" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="info-item">
                            <div class="info-label">Age</div>
                            <div class="info-value">
                                <asp:Literal ID="litAge" runat="server"></asp:Literal>
                            </div>
                        </div>
                        <div class="info-item">
                            <div class="info-label">Member Since</div>
                            <div class="info-value">
                                <asp:Literal ID="litCreatedDate" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Change Password -->
                <div class="info-section">
                    <h2 class="section-title">Security</h2>
                    <asp:Button ID="btnChangePassword" runat="server" Text="Change Password" CssClass="btn-change-password" OnClick="btnChangePassword_Click" />
                </div>
            </div>
        </div>
        
        <asp:Button ID="btnUploadHidden" runat="server" OnClick="btnUploadHidden_Click" style="display: none;" UseSubmitBehavior="true" />
    </form>

    <script>
        function uploadProfilePicture() {
            var fileInput = document.getElementById('<%= fileUpload.ClientID %>');
            if (fileInput && fileInput.files && fileInput.files.length > 0) {
                console.log('File selected: ' + fileInput.files[0].name);
                
                // Show loading indicator
                var overlay = document.querySelector('.upload-overlay');
                if (overlay) {
                    overlay.innerHTML = '<span class="upload-icon">...</span>';
                }
                
                // Submit the form by clicking the hidden button
                // This triggers a full form postback which includes the file data
                var btnUpload = document.getElementById('<%= btnUploadHidden.ClientID %>');
                if (btnUpload) {
                    console.log('Clicking upload button');
                    btnUpload.click();
                } else {
                    console.log('Upload button not found, submitting form directly');
                    // Fallback: submit the form directly
                    var form = document.getElementById('<%= form1.ClientID %>');
                    if (form) {
                        form.submit();
                    }
                }
            } else {
                console.log('No file selected');
            }
        }
    </script>
</body>
</html>

