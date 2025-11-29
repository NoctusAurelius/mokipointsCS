<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaskHistory.aspx.cs" Inherits="mokipointsCS.TaskHistory" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Task History - MOKI POINTS</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
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

        .nav-links a:hover, .nav-links a.active {
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
            border-color: #FF6600;
            transform: scale(1.05);
            box-shadow: 0 2px 8px rgba(255, 102, 0, 0.3);
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

        /* Container */
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 30px 30px;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 30px;
        }

        .page-title {
            font-size: 32px;
            color: #333;
        }

        .stats-section {
            background: white;
            padding: 20px;
            border-radius: 12px;
            margin-bottom: 20px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 20px;
        }

        .stat-card {
            text-align: center;
            padding: 15px;
            background: #f8f9fa;
            border-radius: 8px;
        }

        .stat-value {
            font-size: 32px;
            font-weight: bold;
            color: #667eea;
            margin-bottom: 5px;
        }

        .stat-label {
            font-size: 14px;
            color: #666;
        }

        .filter-section {
            background: white;
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            display: flex;
            gap: 15px;
            align-items: center;
            flex-wrap: wrap;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

        .filter-section label {
            font-weight: 500;
            color: #333;
        }

        .filter-section select {
            padding: 8px 12px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
        }

        .tasks-list {
            display: grid;
            gap: 20px;
        }

        .task-card {
            background: white;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            transition: transform 0.2s, box-shadow 0.2s;
        }

        .task-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(0, 0, 0, 0.2);
        }

        .task-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 15px;
            padding-bottom: 15px;
            border-bottom: 2px solid #f0f0f0;
        }

        .task-title {
            font-size: 24px;
            font-weight: 600;
            color: #333;
            flex: 1;
        }

        .task-status {
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 500;
            text-transform: uppercase;
        }

        .status-completed {
            background: #d4edda;
            color: #155724;
        }

        .status-failed {
            background: #f8d7da;
            color: #721c24;
        }

        .task-meta {
            display: flex;
            gap: 20px;
            margin-bottom: 15px;
            flex-wrap: wrap;
            color: #666;
            font-size: 14px;
        }

        .task-meta span {
            display: flex;
            align-items: center;
            gap: 5px;
        }

        .review-section {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 8px;
            margin-top: 15px;
        }

        .review-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 10px;
        }

        .review-title {
            font-weight: 600;
            color: #333;
        }

        .star-rating {
            display: flex;
            gap: 3px;
        }

        .star {
            color: #ffc107;
            font-size: 20px;
        }

        .star.empty {
            color: #ddd;
        }

        .points-earned {
            font-size: 18px;
            font-weight: 600;
            color: #28a745;
        }

        .points-lost {
            font-size: 18px;
            font-weight: 600;
            color: #dc3545;
        }

        .review-date {
            font-size: 12px;
            color: #999;
            margin-top: 5px;
        }

        .empty-state {
            background: white;
            padding: 60px;
            border-radius: 12px;
            text-align: center;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        }

        .empty-state p {
            font-size: 18px;
            color: #666;
        }

        .success-message, .error-message {
            padding: 15px;
            border-radius: 8px;
            margin-bottom: 20px;
            font-weight: 500;
        }

        .success-message {
            background: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }

        .error-message {
            background: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }

        .hidden {
            display: none;
        }

        @media (max-width: 768px) {
            .task-header {
                flex-direction: column;
            }

            .stats-section {
                grid-template-columns: 1fr;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <div class="header-content">
                <div class="brand">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
                <div class="user-info">
                    <div class="nav-links" style="display: flex; gap: 20px; align-items: center; margin-right: 20px;">
                        <a href="ChildDashboard.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Dashboard</a>
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

        <div class="container">
            <div class="page-header">
                <h1 class="page-title">Task History</h1>
            </div>

            <!-- Success/Error Messages -->
            <asp:Panel ID="pnlSuccess" runat="server" CssClass="success-message hidden">
                <asp:Literal ID="lblSuccess" runat="server"></asp:Literal>
            </asp:Panel>
            <asp:Panel ID="pnlError" runat="server" CssClass="error-message hidden">
                <asp:Literal ID="lblError" runat="server"></asp:Literal>
            </asp:Panel>

            <!-- Statistics -->
            <div class="stats-section">
                <div class="stat-card">
                    <div class="stat-value">
                        <asp:Literal ID="litTotalCompleted" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="stat-label">Total Completed</div>
                </div>
                <div class="stat-card">
                    <div class="stat-value">
                        <asp:Literal ID="litTotalPoints" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="stat-label">Total Points Earned</div>
                </div>
                <div class="stat-card">
                    <div class="stat-value">
                        <asp:Literal ID="litAverageRating" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="stat-label">Average Rating</div>
                </div>
                <div class="stat-card">
                    <div class="stat-value">
                        <asp:Literal ID="litTasksFailed" runat="server" Text="0"></asp:Literal>
                    </div>
                    <div class="stat-label">Tasks Failed</div>
                </div>
            </div>

            <!-- Filter Section -->
            <div class="filter-section">
                <label for="ddlStatusFilter">Filter by Status:</label>
                <asp:DropDownList ID="ddlStatusFilter" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged" CssClass="filter-select">
                    <asp:ListItem Value="" Text="All Tasks"></asp:ListItem>
                    <asp:ListItem Value="Reviewed" Text="Completed"></asp:ListItem>
                    <asp:ListItem Value="Failed" Text="Failed"></asp:ListItem>
                </asp:DropDownList>
            </div>

            <!-- Tasks List -->
            <asp:Panel ID="pnlTasks" runat="server">
                <div class="tasks-list">
                    <asp:Repeater ID="rptTasks" runat="server" OnItemDataBound="rptTasks_ItemDataBound">
                        <ItemTemplate>
                            <div class="task-card">
                                <div class="task-header">
                                    <div class="task-title"><%# Eval("Title") %></div>
                                    <span class="task-status <%# Convert.ToBoolean(Eval("IsFailed")) ? "status-failed" : "status-completed" %>">
                                        <%# Convert.ToBoolean(Eval("IsFailed")) ? "Failed" : "Completed" %>
                                    </span>
                                </div>

                                <div class="task-meta">
                                    <span>Category: <%# Eval("Category") %></span>
                                    <span>Completed: <%# Convert.ToDateTime(Eval("ReviewDate")).ToString("MMM dd, yyyy") %></span>
                                    <span>Reviewed by: <%# Eval("ReviewerName") %></span>
                                </div>

                                <div class="review-section">
                                    <div class="review-header">
                                        <div class="review-title">Review Details</div>
                                        <%# !Convert.ToBoolean(Eval("IsFailed")) && Eval("Rating") != DBNull.Value ? 
                                            "<div class=\"star-rating\">" + 
                                            new string((char)9733, Convert.ToInt32(Eval("Rating"))) + 
                                            new string((char)9734, 5 - Convert.ToInt32(Eval("Rating"))) + 
                                            "</div>" : "" %>
                                    </div>
                                    <div class="points-earned <%# Convert.ToBoolean(Eval("IsFailed")) ? "points-lost" : "" %>">
                                        <%# Convert.ToBoolean(Eval("IsFailed")) ? 
                                            "Points Lost: " + Math.Abs(Convert.ToInt32(Eval("PointsAwarded"))) : 
                                            "Points Earned: +" + Eval("PointsAwarded") %>
                                    </div>
                                    <%# Convert.ToBoolean(Eval("IsAutoFailed")) ? 
                                        "<div style=\"color: #856404; font-size: 12px; margin-top: 5px;\">&#9888; Auto-failed (missed deadline)</div>" : "" %>
                                    <div class="review-date">
                                        Reviewed on <%# Convert.ToDateTime(Eval("ReviewDate")).ToString("MMM dd, yyyy HH:mm") %>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty-state">
                <p>No completed tasks yet. Complete tasks to see your history here!</p>
            </asp:Panel>
        </div>
    </form>
</body>
</html>

