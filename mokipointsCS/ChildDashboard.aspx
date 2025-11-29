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
        
        /* Achievements Display */
        .achievements-display {
            display: flex;
            gap: 30px;
            justify-content: center;
            flex-wrap: wrap;
            margin-bottom: 20px;
        }
        
        .achievement-badge-small {
            position: relative;
            width: 300px;
            height: 300px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: space-between;
            background-color: #f9f9f9;
            border-radius: 15px;
            padding: 15px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            transition: transform 0.3s, box-shadow 0.3s;
        }
        
        .achievement-badge-small:hover {
            transform: translateY(-5px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.15);
        }
        
        .achievement-badge-small img {
            width: 220px;
            height: 220px;
            object-fit: contain;
            flex-shrink: 0;
        }
        
        .achievement-name-display {
            font-size: 16px;
            font-weight: bold;
            color: #333;
            text-align: center;
            margin-bottom: 10px;
            min-height: 40px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .achievement-rarity-badge {
            display: inline-block;
            padding: 6px 16px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: bold;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            box-shadow: 0 2px 6px rgba(0,0,0,0.2);
        }
        
        .achievement-rarity-badge.rarity-common {
            background: linear-gradient(135deg, #9E9E9E 0%, #757575 100%);
            color: white;
        }
        
        .achievement-rarity-badge.rarity-uncommon {
            background: linear-gradient(135deg, #4CAF50 0%, #388E3C 100%);
            color: white;
        }
        
        .achievement-rarity-badge.rarity-rare {
            background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
            color: white;
        }
        
        .achievement-rarity-badge.rarity-epic {
            background: linear-gradient(135deg, #9C27B0 0%, #7B1FA2 100%);
            color: white;
        }
        
        .achievement-rarity-badge.rarity-legendary {
            background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);
            color: white;
        }
        
        .achievement-rarity-badge.rarity-mythical {
            background: linear-gradient(135deg, #F44336 0%, #D32F2F 100%);
            color: white;
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

            <!-- Top Achievements -->
            <div class="quick-actions" style="margin-bottom: 30px;">
                <h2 class="section-title">Top Achievements</h2>
                <asp:Panel ID="pnlTopAchievements" runat="server">
                    <div class="achievements-display">
                        <asp:Repeater ID="rptTopAchievements" runat="server">
                            <ItemTemplate>
                                <div class="achievement-badge-small" title='<%# Eval("Name") %> - <%# Eval("Rarity") %>'>
                                    <img src='<%# Eval("BadgeImagePath") %>' alt='<%# Eval("Name") %>' />
                                    <div class="achievement-name-display"><%# Eval("Name") %></div>
                                    <div class="achievement-rarity-badge rarity-<%# Eval("Rarity").ToString().ToLower() %>"><%# Eval("Rarity") %></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div style="text-align: center; margin-top: 15px;">
                        <a href="Achievements.aspx" style="color: #0066CC; text-decoration: none; font-weight: 500;">View All Achievements →</a>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlNoAchievements" runat="server" Visible="false">
                    <div style="text-align: center; color: #999; padding: 20px;">
                        No achievements earned yet. Complete tasks and earn points to unlock achievements!
                        <br />
                        <a href="Achievements.aspx" style="color: #0066CC; text-decoration: none; font-weight: 500; margin-top: 10px; display: inline-block;">View All Achievements →</a>
                    </div>
                </asp:Panel>
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
                <asp:HiddenField ID="hdnAchievementNotification" runat="server" />
            </div>
        </div>
    </form>

    <!-- Achievement Notification Popup -->
    <div id="achievementNotification" class="achievement-notification" style="display: none;">
        <div class="achievement-notification-content">
            <div class="achievement-notification-close" onclick="closeAchievementNotification()">&times;</div>
            <div class="achievement-notification-header">&#127941; Achievement Unlocked!</div>
            <div class="achievement-notification-badge">
                <img id="notificationBadgeImg" src="" alt="Achievement Badge" />
            </div>
            <div class="achievement-notification-name" id="notificationName"></div>
            <div class="achievement-notification-rarity" id="notificationRarity"></div>
            <div class="achievement-notification-description" id="notificationDescription"></div>
        </div>
    </div>

    <script>
        // Achievement Notification System
        function showAchievementNotification(achievementData) {
            if (!achievementData) return;
            
            const notification = document.getElementById('achievementNotification');
            const badgeImg = document.getElementById('notificationBadgeImg');
            const nameEl = document.getElementById('notificationName');
            const rarityEl = document.getElementById('notificationRarity');
            const descEl = document.getElementById('notificationDescription');
            
            // Set content
            badgeImg.src = achievementData.BadgeImagePath || '';
            nameEl.textContent = achievementData.Name || '';
            descEl.textContent = achievementData.Description || '';
            
            // Set rarity badge with color
            const rarity = achievementData.Rarity || 'Common';
            const rarityColors = {
                'Common': '#9E9E9E',
                'Uncommon': '#4CAF50',
                'Rare': '#2196F3',
                'Epic': '#9C27B0',
                'Legendary': '#FF9800',
                'Mythical': '#F44336'
            };
            const color = rarityColors[rarity] || '#9E9E9E';
            rarityEl.innerHTML = '<span style="background-color: ' + color + '; color: white; padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: bold; text-transform: uppercase;">' + rarity + '</span>';
            
            // Show notification with fade-in animation
            notification.style.display = 'flex';
            setTimeout(() => {
                notification.style.opacity = '1';
                notification.style.transform = 'translateY(0)';
            }, 10);
            
            // Play sound effect (ONLY with dashboard notification)
            playAchievementSound(rarity);
            
            // Auto-fade out after 5 seconds
            setTimeout(() => {
                closeAchievementNotification();
            }, 5000);
        }
        
        function closeAchievementNotification() {
            const notification = document.getElementById('achievementNotification');
            notification.style.opacity = '0';
            notification.style.transform = 'translateY(-20px)';
            setTimeout(() => {
                notification.style.display = 'none';
            }, 300);
        }
        
        function playAchievementSound(rarity) {
            try {
                const rarityLower = (rarity || 'common').toLowerCase();
                const soundPath = '/Sounds/Achievements/unlock_' + rarityLower + '.mp3';
                
                const audio = new Audio(soundPath);
                audio.volume = 0.6; // 60% volume
                audio.play().catch(function(error) {
                    // Fallback to common sound if rarity-specific not found
                    console.log('Failed to play rarity-specific sound, trying fallback');
                    const fallbackAudio = new Audio('/Sounds/Achievements/unlock_common.mp3');
                    fallbackAudio.volume = 0.6;
                    fallbackAudio.play().catch(function(fallbackError) {
                        console.log('Failed to play achievement sound:', fallbackError);
                    });
                });
            } catch (e) {
                console.log('Error playing achievement sound:', e);
            }
        }
        
        // Check for achievement notification on page load
        document.addEventListener('DOMContentLoaded', function() {
            const achievementData = document.getElementById('<%= hdnAchievementNotification.ClientID %>').value;
            if (achievementData && achievementData.trim() !== '') {
                try {
                    const data = JSON.parse(achievementData);
                    if (data && data.Name) {
                        // Small delay to ensure page is fully loaded
                        setTimeout(() => {
                            showAchievementNotification(data);
                        }, 500);
                    }
                } catch (e) {
                    console.error('Error parsing achievement notification data:', e);
                }
            }
        });
    </script>

    <style>
        /* Achievement Notification Styles */
        .achievement-notification {
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 10000;
            opacity: 0;
            transform: translateY(-20px);
            transition: opacity 0.3s ease-in, transform 0.3s ease-in;
        }
        
        .achievement-notification-content {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border-radius: 15px;
            padding: 20px;
            box-shadow: 0 8px 24px rgba(0,0,0,0.3);
            min-width: 300px;
            max-width: 400px;
            position: relative;
            animation: slideInRight 0.3s ease-out;
        }
        
        @keyframes slideInRight {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
        
        .achievement-notification-close {
            position: absolute;
            top: 10px;
            right: 10px;
            font-size: 24px;
            cursor: pointer;
            color: white;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            transition: background-color 0.3s;
        }
        
        .achievement-notification-close:hover {
            background-color: rgba(255,255,255,0.2);
        }
        
        .achievement-notification-header {
            color: white;
            font-size: 18px;
            font-weight: bold;
            text-align: center;
            margin-bottom: 15px;
            animation: fadeInDown 0.5s ease-out;
        }
        
        @keyframes fadeInDown {
            from {
                opacity: 0;
                transform: translateY(-10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
        
        .achievement-notification-badge {
            width: 100px;
            height: 100px;
            margin: 0 auto 15px;
            border-radius: 10px;
            overflow: hidden;
            background-color: rgba(255,255,255,0.2);
            animation: scaleIn 0.5s ease-out;
        }
        
        @keyframes scaleIn {
            from {
                transform: scale(0.8);
                opacity: 0;
            }
            to {
                transform: scale(1);
                opacity: 1;
            }
        }
        
        .achievement-notification-badge img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }
        
        .achievement-notification-name {
            color: white;
            font-size: 20px;
            font-weight: bold;
            text-align: center;
            margin-bottom: 8px;
            animation: fadeIn 0.6s ease-out;
        }
        
        .achievement-notification-rarity {
            text-align: center;
            margin-bottom: 10px;
            animation: fadeIn 0.7s ease-out;
        }
        
        .achievement-notification-description {
            color: rgba(255,255,255,0.9);
            font-size: 14px;
            text-align: center;
            line-height: 1.4;
            animation: fadeIn 0.8s ease-out;
        }
        
        @keyframes fadeIn {
            from {
                opacity: 0;
            }
            to {
                opacity: 1;
            }
        }
    </style>
</body>
</html>

