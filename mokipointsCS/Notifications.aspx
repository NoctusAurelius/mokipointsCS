<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Notifications.aspx.cs" Inherits="mokipointsCS.Notifications" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Notifications - MOKI POINTS</title>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            min-height: 100vh;
            padding: 20px;
        }

        .header {
            background: rgba(255, 255, 255, 0.95);
            padding: 15px 30px;
            border-radius: 10px;
            margin-bottom: 20px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .header h1 {
            color: #667eea;
            font-size: 24px;
        }

        .header-nav {
            display: flex;
            gap: 15px;
        }

        .header-nav a {
            color: #667eea;
            text-decoration: none;
            padding: 8px 15px;
            border-radius: 5px;
            transition: background 0.3s;
        }

        .header-nav a:hover {
            background: #f0f0f0;
        }

        .container {
            max-width: 1200px;
            margin: 0 auto;
        }

        .page-title {
            color: white;
            font-size: 32px;
            margin-bottom: 10px;
            text-align: center;
        }

        .page-subtitle {
            color: rgba(255, 255, 255, 0.9);
            text-align: center;
            margin-bottom: 30px;
        }

        .notifications-actions {
            background: white;
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

        .notifications-count {
            font-weight: 500;
            color: #667eea;
        }

        .btn {
            padding: 10px 20px;
            border: none;
            border-radius: 6px;
            font-size: 14px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s;
            text-decoration: none;
            display: inline-block;
        }

        .btn-primary {
            background: #667eea;
            color: white;
        }

        .btn-primary:hover {
            background: #5568d3;
        }

        .btn-secondary {
            background: #6c757d;
            color: white;
        }

        .btn-secondary:hover {
            background: #5a6268;
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

        .notifications-list {
            display: grid;
            gap: 15px;
        }

        .notification-item {
            background: white;
            border-radius: 8px;
            padding: 20px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            transition: transform 0.2s, box-shadow 0.2s;
            border-left: 4px solid #667eea;
            position: relative;
        }

        .notification-item.unread {
            border-left-color: #ffc107;
            background: #fffbf0;
        }

        .notification-item:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
        }

        .notification-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 10px;
        }

        .notification-title {
            font-size: 18px;
            font-weight: 600;
            color: #333;
            flex: 1;
        }

        .notification-badge {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 500;
            margin-left: 10px;
        }

        .badge-unread {
            background: #ffc107;
            color: #856404;
        }

        .badge-read {
            background: #e0e0e0;
            color: #666;
        }

        .notification-type {
            font-size: 12px;
            color: #667eea;
            text-transform: uppercase;
            font-weight: 500;
        }

        .notification-message {
            color: #555;
            line-height: 1.6;
            margin-bottom: 10px;
        }

        .notification-meta {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 15px;
            padding-top: 15px;
            border-top: 1px solid #f0f0f0;
        }

        .notification-date {
            font-size: 12px;
            color: #999;
        }

        .notification-actions {
            display: flex;
            gap: 10px;
        }

        .btn-small {
            padding: 6px 12px;
            font-size: 12px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn-mark-read {
            background: #28a745;
            color: white;
        }

        .btn-mark-read:hover {
            background: #218838;
        }

        .btn-link {
            background: transparent;
            color: #667eea;
            text-decoration: underline;
            border: none;
            cursor: pointer;
            font-size: 12px;
        }

        .btn-link:hover {
            color: #5568d3;
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

        @media (max-width: 768px) {
            .notifications-actions {
                flex-direction: column;
                gap: 10px;
            }

            .notification-header {
                flex-direction: column;
            }

            .notification-actions {
                flex-direction: column;
            }
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="header">
            <h1>MOKI POINTS</h1>
            <div class="header-nav">
                <a href="Dashboard.aspx">Dashboard</a>
                <a href="Tasks.aspx">Tasks</a>
                <a href="TaskReview.aspx">Review Tasks</a>
                <a href="Notifications.aspx">Notifications</a>
                <a href="Profile.aspx">Profile</a>
                <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLogout_Click" CssClass="header-nav-link">Logout</asp:LinkButton>
            </div>
        </div>

        <div class="container">
            <h1 class="page-title">Notifications</h1>
            <p class="page-subtitle">Stay updated with your task activities.</p>

            <!-- Success/Error Messages -->
            <asp:Panel ID="pnlSuccess" runat="server" CssClass="success-message hidden">
                <asp:Literal ID="lblSuccess" runat="server"></asp:Literal>
            </asp:Panel>
            <asp:Panel ID="pnlError" runat="server" CssClass="error-message hidden">
                <asp:Literal ID="lblError" runat="server"></asp:Literal>
            </asp:Panel>

            <!-- Notifications Actions -->
            <div class="notifications-actions">
                <div class="notifications-count">
                    <asp:Literal ID="litNotificationCount" runat="server"></asp:Literal>
                </div>
                <asp:Button ID="btnMarkAllRead" runat="server" Text="Mark All as Read" CssClass="btn btn-primary" OnClick="btnMarkAllRead_Click" />
            </div>

            <!-- Notifications List -->
            <asp:Panel ID="pnlNotifications" runat="server">
                <div class="notifications-list">
                    <asp:Repeater ID="rptNotifications" runat="server" OnItemCommand="rptNotifications_ItemCommand" OnItemDataBound="rptNotifications_ItemDataBound">
                        <ItemTemplate>
                            <div class="notification-item <%# Convert.ToBoolean(Eval("IsRead")) ? "" : "unread" %>">
                                <div class="notification-header">
                                    <div>
                                        <span class="notification-title"><%# Eval("Title") %></span>
                                        <span class="notification-badge <%# Convert.ToBoolean(Eval("IsRead")) ? "badge-read" : "badge-unread" %>">
                                            <%# Convert.ToBoolean(Eval("IsRead")) ? "Read" : "New" %>
                                        </span>
                                    </div>
                                    <span class="notification-type"><%# Eval("Type") %></span>
                                </div>
                                <div class="notification-message">
                                    <%# Eval("Message") %>
                                </div>
                                <div class="notification-meta">
                                    <span class="notification-date">
                                        <%# Convert.ToDateTime(Eval("CreatedDate")).ToString("MMM dd, yyyy HH:mm") %>
                                    </span>
                                    <div class="notification-actions">
                                        <%# !Convert.ToBoolean(Eval("IsRead")) ? "<asp:Button ID=\"btnMarkRead\" runat=\"server\" Text=\"Mark as Read\" CommandName=\"MarkRead\" CommandArgument=\"" + Eval("Id") + "\" CssClass=\"btn-small btn-mark-read\" />" : "" %>
                                        <%# Eval("RelatedTaskId") != DBNull.Value ? "<a href=\"Tasks.aspx?taskId=" + Eval("RelatedTaskId") + "\" class=\"btn-link\">View Task</a>" : "" %>
                                        <%# Eval("RelatedAssignmentId") != DBNull.Value && Eval("Type").ToString() == "TaskSubmitted" ? "<a href=\"TaskReview.aspx\" class=\"btn-link\">Review Task</a>" : "" %>
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty-state">
                <p>No notifications yet. You're all caught up! 🎉</p>
            </asp:Panel>
        </div>
    </form>
</body>
</html>

