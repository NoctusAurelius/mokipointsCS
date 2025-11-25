<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChildDashboard.aspx.cs" Inherits="mokipointsCS.ChildDashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>My Dashboard - MOKI POINTS</title>
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
        
        /* Header/Navigation */
        .header {
            background-color: white;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            padding: 15px 0;
            position: sticky;
            top: 0;
            z-index: 1000;
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
        
        .user-info {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        
        .user-name {
            color: #333;
            font-weight: 500;
        }
        
        /* Profile Picture Avatar */
        .profile-avatar {
            width: 45px;
            height: 45px;
            border-radius: 50%;
            border: 2px solid #e0e0e0;
            object-fit: cover;
            cursor: pointer;
            transition: all 0.3s ease;
            margin-right: 12px;
        }
        
        .profile-avatar:hover {
            border-color: #0066CC;
            transform: scale(1.05);
            box-shadow: 0 2px 8px rgba(0, 102, 204, 0.3);
        }
        
        .profile-avatar-placeholder {
            width: 45px;
            height: 45px;
            border-radius: 50%;
            border: 2px solid #e0e0e0;
            background: linear-gradient(135deg, #FF6600 0%, #e55a00 100%);
            color: white;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 18px;
            cursor: pointer;
            transition: all 0.3s ease;
            margin-right: 12px;
            text-decoration: none;
        }
        
        .profile-avatar-placeholder:hover {
            border-color: #FF6600;
            transform: scale(1.05);
            box-shadow: 0 2px 8px rgba(255, 102, 0, 0.3);
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
            height: 2px;
            background-color: currentColor;
            border-radius: 2px;
            transition: all 0.3s;
        }
        
        /* Main Container */
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 30px;
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
        
        /* Points Display */
        .points-display {
            background: linear-gradient(135deg, #0066CC 0%, #FF6600 100%);
            border-radius: 15px;
            padding: 40px;
            text-align: center;
            color: white;
            margin-bottom: 30px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.2);
        }
        
        .points-label {
            font-size: 18px;
            margin-bottom: 10px;
            opacity: 0.9;
        }
        
        .points-value {
            font-size: 72px;
            font-weight: bold;
            margin-bottom: 10px;
        }
        
        .points-subtitle {
            font-size: 16px;
            opacity: 0.8;
        }
        
        /* Dashboard Cards */
        .dashboard-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        
        .dashboard-card {
            background-color: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            border-left: 4px solid #0066CC;
        }
        
        .dashboard-card.orange {
            border-left-color: #FF6600;
        }
        
        .dashboard-card.pink {
            border-left-color: #FFB6C1;
        }
        
        .card-title {
            font-size: 18px;
            font-weight: bold;
            color: #333;
            margin-bottom: 15px;
        }
        
        .card-value {
            font-size: 36px;
            font-weight: bold;
            color: #0066CC;
            margin-bottom: 10px;
        }
        
        .card-description {
            color: #666;
            font-size: 14px;
        }
        
        /* My Chores Section */
        .my-chores {
            background-color: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin-bottom: 30px;
        }
        
        .section-title {
            font-size: 24px;
            color: #333;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid #e0e0e0;
        }
        
        .chore-item {
            padding: 15px;
            border-bottom: 1px solid #e0e0e0;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        
        .chore-item:last-child {
            border-bottom: none;
        }
        
        .chore-info {
            flex: 1;
        }
        
        .chore-name {
            font-size: 16px;
            font-weight: bold;
            color: #333;
            margin-bottom: 5px;
        }
        
        .chore-description {
            font-size: 14px;
            color: #666;
            margin-bottom: 5px;
        }
        
        .chore-points {
            font-size: 18px;
            font-weight: bold;
            color: #FF6600;
        }
        
        .chore-status {
            padding: 5px 15px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: bold;
            margin-left: 15px;
        }
        
        .status-pending {
            background-color: #FFF9E6;
            color: #FF6600;
        }
        
        .status-completed {
            background-color: #e8f5e9;
            color: #2e7d32;
        }
        
        .btn-complete {
            padding: 8px 20px;
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            transition: background-color 0.3s;
        }
        
        .btn-complete:hover {
            background-color: #0052a3;
        }
        
        .placeholder-text {
            color: #999;
            font-style: italic;
            text-align: center;
            padding: 40px;
        }
        
        /* Recent Points */
        .recent-points {
            background-color: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        
        .points-item {
            padding: 15px;
            border-bottom: 1px solid #e0e0e0;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        
        .points-item:last-child {
            border-bottom: none;
        }
        
        .points-text {
            color: #333;
            font-size: 14px;
        }
        
        .points-amount {
            font-size: 18px;
            font-weight: bold;
        }
        
        .points-earned {
            color: #2e7d32;
        }
        
        .points-date {
            color: #999;
            font-size: 12px;
        }
        
        /* Quick Actions - Fun and Colorful for Kids */
        .quick-actions {
            background-color: white;
            border-radius: 15px;
            padding: 25px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            margin-bottom: 30px;
        }
        
        .actions-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));
            gap: 15px;
            margin-top: 20px;
        }
        
        .action-btn {
            padding: 18px 24px;
            background: linear-gradient(135deg, #0066CC 0%, #0052a3 100%);
            color: white;
            border: none;
            border-radius: 12px;
            font-size: 16px;
            font-weight: bold;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            text-align: center;
            transition: all 0.3s ease;
            box-shadow: 0 4px 8px rgba(0, 102, 204, 0.3);
            position: relative;
        }
        
        .action-btn:hover {
            transform: translateY(-3px);
            box-shadow: 0 6px 16px rgba(0, 102, 204, 0.4);
            background: linear-gradient(135deg, #0052a3 0%, #0066CC 100%);
        }
        
        .action-btn.secondary {
            background: linear-gradient(135deg, #FF6600 0%, #e55a00 100%);
            box-shadow: 0 4px 8px rgba(255, 102, 0, 0.3);
        }
        
        .action-btn.secondary:hover {
            box-shadow: 0 6px 16px rgba(255, 102, 0, 0.4);
            background: linear-gradient(135deg, #e55a00 0%, #FF6600 100%);
        }
        
        /* Make cards more fun and colorful */
        .dashboard-card {
            transition: transform 0.2s ease, box-shadow 0.2s ease;
        }
        
        .dashboard-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.15);
        }
        
        /* Points display - more exciting */
        .points-display {
            background: linear-gradient(135deg, #0066CC 0%, #0052a3 100%);
            border-radius: 20px;
            padding: 30px;
            text-align: center;
            color: white;
            box-shadow: 0 6px 20px rgba(0, 102, 204, 0.3);
            margin-bottom: 30px;
        }
        
        .points-value {
            font-size: 56px;
            font-weight: bold;
            margin: 15px 0;
            text-shadow: 2px 2px 4px rgba(0,0,0,0.2);
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
                <div class="user-info">
                    <div class="nav-links" style="display: flex; gap: 20px; align-items: center; margin-right: 20px;">
                        <a href="Family.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Family</a>
                        <a href="ChildTasks.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Tasks</a>
                        <a href="PointsHistory.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Points</a>
                        <a href="RewardShop.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Shop</a>
                        <a href="Cart.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Cart</a>
                        <a href="MyOrders.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">My Orders</a>
                    </div>
                    <a href="Profile.aspx" style="text-decoration: none; display: flex; align-items: center;">
                        <asp:Image ID="imgProfilePicture" runat="server" CssClass="profile-avatar" Visible="false" />
                        <asp:Literal ID="litProfilePlaceholder" runat="server"></asp:Literal>
                    </a>
                    <span class="user-name"><asp:Literal ID="litUserName" runat="server"></asp:Literal></span>
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
            <h1 class="page-title">My Dashboard</h1>
            <p class="page-subtitle">Track your chores and points</p>

            <!-- Points Display -->
            <div class="points-display">
                <div class="points-label" style="font-size: 18px; opacity: 0.95;">&#128176; Your Total Points</div>
                <div class="points-value"><asp:Literal ID="litCurrentPoints" runat="server">0</asp:Literal></div>
                <div class="points-subtitle" style="font-size: 16px; opacity: 0.9; margin-top: 10px;">Keep completing chores to earn more! &#127881;</div>
                <!-- Progress Bar -->
                <div style="margin-top: 20px; width: 100%;">
                    <div style="background: rgba(255,255,255,0.3); height: 8px; border-radius: 4px; overflow: hidden;">
                        <div id="progressBar" runat="server" style="background: white; height: 100%; width: 0%; transition: width 0.3s;"></div>
                    </div>
                    <div style="margin-top: 5px; font-size: 12px; opacity: 0.9;">
                        <asp:Literal ID="litProgressPercent" runat="server">0</asp:Literal>% toward 10,000 point cap
                    </div>
                </div>
            </div>

            <!-- Stats Cards -->
            <div class="dashboard-grid">
                <div class="dashboard-card">
                    <div class="card-title">&#128203; My Active Tasks</div>
                    <div class="card-value"><asp:Literal ID="litActiveTasks" runat="server">0</asp:Literal></div>
                    <div class="card-description">Tasks assigned and in progress</div>
                </div>
                <div class="dashboard-card orange">
                    <div class="card-title">&#10004; Completed This Week</div>
                    <div class="card-value"><asp:Literal ID="litCompletedWeek" runat="server">0</asp:Literal></div>
                    <div class="card-description">Tasks completed this week</div>
                </div>
                <div class="dashboard-card pink">
                    <div class="card-title">&#9203; Pending Reviews</div>
                    <div class="card-value"><asp:Literal ID="litPendingReviews" runat="server">0</asp:Literal></div>
                    <div class="card-description">Tasks awaiting parent review</div>
                </div>
                <div class="dashboard-card">
                    <div class="card-title">&#127873; Available Rewards</div>
                    <div class="card-value"><asp:Literal ID="litAvailableRewards" runat="server">0</asp:Literal></div>
                    <div class="card-description">Rewards you can afford</div>
                </div>
                <div class="dashboard-card orange">
                    <div class="card-title">&#128230; Active Orders</div>
                    <div class="card-value"><asp:Literal ID="litActiveOrders" runat="server">0</asp:Literal></div>
                    <div class="card-description">Orders in progress</div>
                </div>
            </div>

            <!-- Quick Actions -->
            <div class="quick-actions">
                <h2 class="section-title">Quick Actions</h2>
                <div class="actions-grid">
                    <a href="ChildTasks.aspx" class="action-btn" style="position: relative;">
                        &#128203; View My Tasks
                        <asp:Panel ID="pnlTaskBadge" runat="server" Visible="false" 
                            style="position: absolute; top: -8px; right: -8px; background: #dc3545; color: white; 
                            border-radius: 50%; width: 24px; height: 24px; display: flex; align-items: center; 
                            justify-content: center; font-size: 12px; font-weight: bold;">
                            <asp:Literal ID="litTaskBadge" runat="server"></asp:Literal>
                        </asp:Panel>
                    </a>
                    <a href="RewardShop.aspx" class="action-btn secondary" style="position: relative;">
                        &#127873; Shop Rewards
                        <asp:Panel ID="pnlRewardBadge" runat="server" Visible="false" 
                            style="position: absolute; top: -8px; right: -8px; background: #28a745; color: white; 
                            border-radius: 50%; width: 24px; height: 24px; display: flex; align-items: center; 
                            justify-content: center; font-size: 12px; font-weight: bold;">
                            <asp:Literal ID="litRewardBadge" runat="server"></asp:Literal>
                        </asp:Panel>
                    </a>
                    <a href="MyOrders.aspx" class="action-btn" style="position: relative;">
                        &#128230; My Orders
                        <asp:Panel ID="pnlOrderBadge" runat="server" Visible="false" 
                            style="position: absolute; top: -8px; right: -8px; background: #dc3545; color: white; 
                            border-radius: 50%; width: 24px; height: 24px; display: flex; align-items: center; 
                            justify-content: center; font-size: 12px; font-weight: bold;">
                            <asp:Literal ID="litOrderBadge" runat="server"></asp:Literal>
                        </asp:Panel>
                    </a>
                    <a href="PointsHistory.aspx" class="action-btn secondary">&#128176; Points History</a>
                </div>
            </div>

            <!-- Weekly Statistics - Simplified -->
            <div class="quick-actions" style="margin-bottom: 30px;">
                <h2 class="section-title">&#127881; This Week's Progress</h2>
                <div class="dashboard-grid" style="grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));">
                    <div class="dashboard-card" style="background: linear-gradient(135deg, #e8f5e9 0%, #c8e6c9 100%);">
                        <div class="card-title">&#10004; Tasks Done</div>
                        <div class="card-value" style="color: #2e7d32;"><asp:Literal ID="litWeekTasks" runat="server">0</asp:Literal></div>
                    </div>
                    <div class="dashboard-card" style="background: linear-gradient(135deg, #fff3e0 0%, #ffe0b2 100%);">
                        <div class="card-title">&#128176; Points Earned</div>
                        <div class="card-value" style="color: #FF6600;"><asp:Literal ID="litWeekEarned" runat="server">0</asp:Literal></div>
                    </div>
                    <div class="dashboard-card" style="background: linear-gradient(135deg, #fce4ec 0%, #f8bbd0 100%);">
                        <div class="card-title">&#128230; Points Spent</div>
                        <div class="card-value" style="color: #c2185b;"><asp:Literal ID="litWeekSpent" runat="server">0</asp:Literal></div>
                    </div>
                </div>
            </div>

            <!-- Streak Counter - Fun and Colorful -->
            <div class="quick-actions" style="margin-bottom: 30px; background: linear-gradient(135deg, #fff3e0 0%, #ffe0b2 100%);">
                <h2 class="section-title" style="color: #FF6600;">&#128293; Your Streak!</h2>
                <div style="text-align: center; padding: 15px;">
                    <div style="font-size: 42px; font-weight: bold; color: #FF6600; margin-bottom: 8px;">
                        <asp:Literal ID="litStreak" runat="server">0</asp:Literal> <span style="font-size: 28px;">days</span> &#128293;
                    </div>
                    <div style="font-size: 16px; color: #e55a00; font-weight: 500;">
                        <asp:Literal ID="litStreakMessage" runat="server"></asp:Literal>
                    </div>
                </div>
            </div>

            <!-- Recent Activity - Hidden to reduce clutter -->
            <div class="recent-points" style="display: none;">
                <h2 class="section-title">Recent Activity</h2>
                <asp:Repeater ID="rptRecentActivity" runat="server">
                    <ItemTemplate>
                        <div class="points-item">
                            <div class="points-text">
                                <%# GetActivityText(Container.DataItem) %>
                            </div>
                            <div class="points-amount <%# GetActivityClass(Container.DataItem) %>">
                                <%# GetActivityPoints(Container.DataItem) %>
                            </div>
                        </div>
                        <div class="points-date">
                            <%# GetActivityDate(Container.DataItem) %>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlNoActivity" runat="server" CssClass="placeholder-text" Visible="false">
                    No recent activity to display
                </asp:Panel>
            </div>
        </div>
    </form>
</body>
</html>

