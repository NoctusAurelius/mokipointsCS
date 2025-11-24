<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Family.aspx.cs" Inherits="mokipointsCS.Family" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Family - MOKI POINTS</title>
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
        
        .user-info {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        
        .user-name {
            color: #333;
            font-weight: 500;
        }
        
        .btn-settings {
            padding: 8px 12px;
            background-color: transparent;
            color: #333;
            border: 2px solid #e0e0e0;
            border-radius: 5px;
            cursor: pointer;
            text-decoration: none;
            font-size: 18px;
            transition: all 0.3s;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 40px;
            height: 40px;
        }
        
        .btn-settings:hover {
            background-color: #f5f5f5;
            border-color: #0066CC;
            color: #0066CC;
        }
        
        .hamburger-icon {
            display: flex;
            flex-direction: column;
            gap: 4px;
            width: 20px;
        }
        
        .hamburger-line {
            width: 100%;
            height: 3px;
            background-color: currentColor;
            border-radius: 2px;
        }
        
        /* Container */
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
        
        /* Tabs */
        .tabs {
            display: flex;
            gap: 10px;
            margin-bottom: 30px;
            border-bottom: 2px solid #e0e0e0;
        }
        
        .tab {
            padding: 15px 30px;
            background: none;
            border: none;
            font-size: 16px;
            font-weight: bold;
            color: #666;
            cursor: pointer;
            border-bottom: 3px solid transparent;
            transition: all 0.3s;
        }
        
        .tab.active {
            color: #0066CC;
            border-bottom-color: #0066CC;
        }
        
        .tab:hover {
            color: #0066CC;
        }
        
        /* Forms */
        .form-container {
            background-color: white;
            border-radius: 10px;
            padding: 40px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin-bottom: 30px;
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
        
        .form-group label .required {
            color: #d32f2f;
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
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            font-size: 18px;
            font-weight: bold;
            cursor: pointer;
            transition: background-color 0.3s;
            margin-top: 10px;
        }
        
        .btn-submit:hover {
            background-color: #0052a3;
        }
        
        .btn-submit:disabled {
            background-color: #999;
            cursor: not-allowed;
        }
        
        /* Family Info Card */
        .family-info-card {
            background: linear-gradient(135deg, #0066CC 0%, #FF6600 100%);
            border-radius: 15px;
            padding: 40px;
            color: white;
            margin-bottom: 30px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.2);
        }
        
        .family-name {
            font-size: 32px;
            font-weight: bold;
            margin-bottom: 15px;
        }
        
        .family-code-section {
            background-color: rgba(255,255,255,0.2);
            border-radius: 10px;
            padding: 20px;
            margin-top: 20px;
        }
        
        .code-label {
            font-size: 14px;
            opacity: 0.9;
            margin-bottom: 10px;
        }
        
        .family-code {
            font-size: 48px;
            font-weight: bold;
            letter-spacing: 4px;
            font-family: 'Courier New', monospace;
        }
        
        .code-description {
            font-size: 14px;
            opacity: 0.8;
            margin-top: 10px;
        }
        
        .treasury-info {
            margin-top: 20px;
            padding-top: 20px;
            border-top: 1px solid rgba(255,255,255,0.3);
        }
        
        .treasury-label {
            font-size: 14px;
            opacity: 0.9;
            margin-bottom: 5px;
        }
        
        .treasury-amount {
            font-size: 36px;
            font-weight: bold;
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
        
        .hidden {
            display: none;
        }
    </style>
    <script>
        function showTab(tabName) {
            // Hide all forms
            document.getElementById('createForm').classList.add('hidden');
            document.getElementById('joinForm').classList.add('hidden');
            
            // Remove active class from all tabs
            document.querySelectorAll('.tab').forEach(tab => tab.classList.remove('active'));
            
            // Show selected form and activate tab
            if (tabName === 'create') {
                document.getElementById('createForm').classList.remove('hidden');
                document.getElementById('tabCreate').classList.add('active');
            } else {
                document.getElementById('joinForm').classList.remove('hidden');
                document.getElementById('tabJoin').classList.add('active');
            }
        }

        function showLoadingCreate() {
            var btn = document.getElementById('<%= btnCreateFamily.ClientID %>');
            if (btn) {
                setTimeout(function() {
                    if (btn && !btn.disabled) {
                        btn.disabled = true;
                        if (btn.tagName === 'INPUT') {
                            btn.value = 'Creating...';
                        } else {
                            btn.innerHTML = '<span class="spinner"></span>Creating...';
                        }
                    }
                }, 10);
            }
        }

        function showLoadingJoin() {
            var btn = document.getElementById('<%= btnJoinFamily.ClientID %>');
            if (btn) {
                setTimeout(function() {
                    if (btn && !btn.disabled) {
                        btn.disabled = true;
                        if (btn.tagName === 'INPUT') {
                            btn.value = 'Joining...';
                        } else {
                            btn.innerHTML = '<span class="spinner"></span>Joining...';
                        }
                    }
                }, 10);
            }
        }
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
                <div class="user-info">
                    <div class="nav-links" style="display: flex; gap: 20px; align-items: center; margin-right: 20px;">
                        <a href="ParentDashboard.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Dashboard</a>
                        <a href="Family.aspx" style="color: #0066CC; text-decoration: none; font-weight: 500; font-size: 16px;">Family</a>
                        <a href="Tasks.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Tasks</a>
                        <a href="TaskReview.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Review</a>
                    </div>
                    <span class="user-name">Family</span>
                    <a href="Settings.aspx" class="btn-settings" title="Settings">
                        <div class="hamburger-icon">
                            <div class="hamburger-line"></div>
                            <div class="hamburger-line"></div>
                            <div class="hamburger-line"></div>
                        </div>
                    </a>
                </div>
            </div>
        </div>

        <!-- Main Content -->
        <div class="container">
            <h1 class="page-title">Family Management</h1>
            <p class="page-subtitle">Create or join a family to get started</p>

            <!-- Family Info (if already in family) -->
            <asp:Panel ID="pnlFamilyInfo" runat="server" Visible="false">
                <div class="family-info-card">
                    <div class="family-name">
                        <asp:Literal ID="litFamilyName" runat="server"></asp:Literal>
                    </div>
                    <div class="family-code-section">
                        <div class="code-label">Family Code (Share with children)</div>
                        <div class="family-code">
                            <asp:Literal ID="litFamilyCode" runat="server"></asp:Literal>
                        </div>
                        <div class="code-description">Children can use this code to join your family</div>
                    </div>
                    <div class="treasury-info">
                        <div class="treasury-label">Family Treasury</div>
                        <div class="treasury-amount">
                            <asp:Literal ID="litTreasuryPoints" runat="server"></asp:Literal> Mokipoints
                        </div>
                    </div>
                </div>
                
                <!-- Children Monitoring Section -->
                <asp:Panel ID="pnlChildrenMonitoring" runat="server" Visible="false">
                    <div class="form-container" style="margin-top: 30px;">
                        <h2 style="margin-bottom: 20px; color: #333; padding-bottom: 10px; border-bottom: 2px solid #e0e0e0;">Children Monitoring</h2>
                        <asp:Repeater ID="rptChildren" runat="server" OnItemCommand="rptChildren_ItemCommand" OnItemDataBound="rptChildren_ItemDataBound">
                            <ItemTemplate>
                                <div ID="childCard" runat="server" class="child-card" style="display: flex; align-items: center; padding: 20px; margin-bottom: 15px; background-color: #f9f9f9; border-radius: 10px;">
                                    <!-- Profile Picture -->
                                    <div style="width: 80px; height: 80px; border-radius: 50%; overflow: hidden; margin-right: 20px; background-color: #e0e0e0; display: flex; align-items: center; justify-content: center; flex-shrink: 0;">
                                        <asp:PlaceHolder ID="phProfilePicture" runat="server"></asp:PlaceHolder>
                                    </div>
                                    
                                    <!-- Child Info -->
                                    <div style="flex: 1;">
                                        <div style="display: flex; align-items: center; margin-bottom: 10px;">
                                            <h3 style="margin: 0; color: #333; font-size: 20px;">
                                                <%# Eval("FirstName") %> <%# Eval("LastName") %>
                                            </h3>
                                            <asp:Label ID="lblBannedBadge" runat="server" Visible="false" style="margin-left: 10px; padding: 4px 12px; background-color: #d32f2f; color: white; border-radius: 15px; font-size: 12px; font-weight: bold;">BANNED</asp:Label>
                                        </div>
                                        <div style="display: grid; grid-template-columns: repeat(auto-fit, minmax(150px, 1fr)); gap: 15px; margin-top: 10px;">
                                            <div>
                                                <span style="color: #666; font-size: 14px;">Total Points:</span>
                                                <span style="font-weight: bold; color: #0066CC; font-size: 18px; margin-left: 5px;"><%# Convert.ToInt32(Eval("TotalPoints")).ToString("N0") %></span>
                                            </div>
                                            <div>
                                                <span style="color: #666; font-size: 14px;">Completed Tasks:</span>
                                                <span style="font-weight: bold; color: #2e7d32; font-size: 18px; margin-left: 5px;"><%# Eval("CompletedTasks") %></span>
                                            </div>
                                            <div>
                                                <span style="color: #666; font-size: 14px;">Failed Tasks:</span>
                                                <span style="font-weight: bold; color: #d32f2f; font-size: 18px; margin-left: 5px;"><%# Eval("FailedTasks") %></span>
                                            </div>
                                        </div>
                                    </div>
                                    
                                    <!-- Actions -->
                                    <div style="display: flex; gap: 10px; margin-left: 20px;">
                                        <asp:Button ID="btnBan" runat="server" 
                                            Text="Ban" 
                                            CommandName="Ban"
                                            CommandArgument='<%# Eval("Id") %>'
                                            CssClass="btn-action" 
                                            style="padding: 8px 16px; color: white; border: none; border-radius: 5px; cursor: pointer; font-size: 14px; font-weight: 500;" />
                                        <asp:Button ID="btnRemove" runat="server" 
                                            Text="Remove" 
                                            CommandName="Remove"
                                            CommandArgument='<%# Eval("Id") %>'
                                            CssClass="btn-action" 
                                            style="padding: 8px 16px; background-color: #666; color: white; border: none; border-radius: 5px; cursor: pointer; font-size: 14px; font-weight: 500;"
                                            OnClientClick="return confirm('Are you sure you want to remove this child from the family? This will reset their points to 0.');" />
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                        <asp:Panel ID="pnlNoChildren" runat="server" Visible="false">
                            <div style="text-align: center; padding: 40px; color: #999;">
                                <p>No children linked to this family yet.</p>
                                <p style="margin-top: 10px; font-size: 14px;">Share your family code with children so they can join!</p>
                            </div>
                        </asp:Panel>
                        <asp:Label ID="lblChildrenError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                        <asp:Label ID="lblChildrenSuccess" runat="server" CssClass="success-message" Visible="false"></asp:Label>
                    </div>
                </asp:Panel>
            </asp:Panel>

            <!-- Create/Join Tabs (if not in family) -->
            <asp:Panel ID="pnlFamilyActions" runat="server" Visible="false">
                <div class="tabs">
                    <button type="button" class="tab active" id="tabCreate" onclick="showTab('create')">Create Family</button>
                    <button type="button" class="tab" id="tabJoin" onclick="showTab('join')">Join Family</button>
                </div>

                <!-- Create Family Form -->
                <div id="createForm" class="form-container">
                    <h2 style="margin-bottom: 20px; color: #333;">Create a New Family</h2>
                    <div class="form-group">
                        <label for="txtCreateFamilyName">Family Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtCreateFamilyName" runat="server" CssClass="form-control" placeholder="Enter family name"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCreateFamilyName" runat="server" ControlToValidate="txtCreateFamilyName" 
                            ErrorMessage="Family name is required" CssClass="error-message" Display="Dynamic" ValidationGroup="CreateFamily"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <label for="txtCreatePinCode">6-Digit PIN Code <span class="required">*</span></label>
                        <asp:TextBox ID="txtCreatePinCode" runat="server" CssClass="form-control" MaxLength="6" placeholder="000000" TextMode="SingleLine"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvCreatePinCode" runat="server" ControlToValidate="txtCreatePinCode" 
                            ErrorMessage="PIN code is required" CssClass="error-message" Display="Dynamic" ValidationGroup="CreateFamily"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revCreatePinCode" runat="server" ControlToValidate="txtCreatePinCode" 
                            ValidationExpression="^\d{6}$" ErrorMessage="PIN must be 6 digits" CssClass="error-message" Display="Dynamic" ValidationGroup="CreateFamily"></asp:RegularExpressionValidator>
                    </div>
                    <asp:Button ID="btnCreateFamily" runat="server" Text="Create Family" CssClass="btn-submit" ValidationGroup="CreateFamily" OnClick="btnCreateFamily_Click" OnClientClick="console.log('Create Family button clicked'); showLoadingCreate(); return true;" UseSubmitBehavior="true" />
                    <asp:Label ID="lblCreateError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                    <asp:Label ID="lblCreateSuccess" runat="server" CssClass="success-message" Visible="false"></asp:Label>
                </div>

                <!-- Join Family Form -->
                <div id="joinForm" class="form-container hidden">
                    <h2 style="margin-bottom: 20px; color: #333;">Join an Existing Family</h2>
                    <div class="form-group">
                        <label for="txtJoinFamilyName">Family Name <span class="required">*</span></label>
                        <asp:TextBox ID="txtJoinFamilyName" runat="server" CssClass="form-control" placeholder="Enter family name"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvJoinFamilyName" runat="server" ControlToValidate="txtJoinFamilyName" 
                            ErrorMessage="Family name is required" CssClass="error-message" Display="Dynamic" ValidationGroup="JoinFamily"></asp:RequiredFieldValidator>
                    </div>
                    <div class="form-group">
                        <label for="txtJoinPinCode">6-Digit PIN Code <span class="required">*</span></label>
                        <asp:TextBox ID="txtJoinPinCode" runat="server" CssClass="form-control" MaxLength="6" placeholder="000000" TextMode="SingleLine"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvJoinPinCode" runat="server" ControlToValidate="txtJoinPinCode" 
                            ErrorMessage="PIN code is required" CssClass="error-message" Display="Dynamic" ValidationGroup="JoinFamily"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="revJoinPinCode" runat="server" ControlToValidate="txtJoinPinCode" 
                            ValidationExpression="^\d{6}$" ErrorMessage="PIN must be 6 digits" CssClass="error-message" Display="Dynamic" ValidationGroup="JoinFamily"></asp:RegularExpressionValidator>
                    </div>
                    <asp:Button ID="btnJoinFamily" runat="server" Text="Join Family" CssClass="btn-submit" ValidationGroup="JoinFamily" OnClick="btnJoinFamily_Click" OnClientClick="console.log('Join Family button clicked'); showLoadingJoin(); return true;" UseSubmitBehavior="true" />
                    <asp:Label ID="lblJoinError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
                    <asp:Label ID="lblJoinSuccess" runat="server" CssClass="success-message" Visible="false"></asp:Label>
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>

