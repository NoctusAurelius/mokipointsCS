<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaskTemplates.aspx.cs" Inherits="mokipointsCS.TaskTemplates" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Task Templates - MOKI POINTS</title>
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

        .templates-actions {
            background: white;
            padding: 15px 20px;
            border-radius: 8px;
            margin-bottom: 20px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
        }

        .btn {
            padding: 12px 24px;
            border: none;
            border-radius: 6px;
            font-size: 16px;
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
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(102, 126, 234, 0.3);
        }

        .btn-success {
            background: #28a745;
            color: white;
        }

        .btn-success:hover {
            background: #218838;
        }

        .btn-danger {
            background: #dc3545;
            color: white;
        }

        .btn-danger:hover {
            background: #c82333;
        }

        .templates-list {
            display: grid;
            gap: 20px;
        }

        .template-card {
            background: white;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            transition: transform 0.2s, box-shadow 0.2s;
        }

        .template-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(0, 0, 0, 0.2);
        }

        .template-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 15px;
            padding-bottom: 15px;
            border-bottom: 2px solid #f0f0f0;
        }

        .template-title {
            font-size: 24px;
            font-weight: 600;
            color: #333;
            flex: 1;
        }

        .template-meta {
            display: flex;
            gap: 20px;
            margin-bottom: 15px;
            flex-wrap: wrap;
            color: #666;
            font-size: 14px;
        }

        .template-meta span {
            display: flex;
            align-items: center;
            gap: 5px;
        }

        .template-description {
            margin-bottom: 20px;
            color: #555;
            line-height: 1.6;
        }

        .template-objectives {
            margin-bottom: 20px;
        }

        .template-objectives h4 {
            color: #667eea;
            margin-bottom: 10px;
            font-size: 16px;
        }

        .objective-item {
            padding: 8px;
            margin-bottom: 5px;
            background: #f8f9fa;
            border-radius: 4px;
            color: #555;
        }

        .template-actions {
            display: flex;
            gap: 10px;
            margin-top: 20px;
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
            .template-header {
                flex-direction: column;
            }

            .template-actions {
                flex-direction: column;
            }

            .btn {
                width: 100%;
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
                <a href="TaskTemplates.aspx">Templates</a>
                <a href="Profile.aspx">Profile</a>
                <asp:LinkButton ID="lnkLogout" runat="server" OnClick="lnkLogout_Click" CssClass="header-nav-link">Logout</asp:LinkButton>
            </div>
        </div>

        <div class="container">
            <h1 class="page-title">Task Templates</h1>
            <p class="page-subtitle">Create reusable task templates for quick task creation.</p>

            <!-- Success/Error Messages -->
            <asp:Panel ID="pnlSuccess" runat="server" CssClass="success-message hidden">
                <asp:Literal ID="lblSuccess" runat="server"></asp:Literal>
            </asp:Panel>
            <asp:Panel ID="pnlError" runat="server" CssClass="error-message hidden">
                <asp:Literal ID="lblError" runat="server"></asp:Literal>
            </asp:Panel>

            <!-- Templates Actions -->
            <div class="templates-actions">
                <div>
                    <asp:Button ID="btnCreateTemplate" runat="server" Text="Create New Template" CssClass="btn btn-primary" OnClientClick="window.location.href='Tasks.aspx?createTemplate=true'; return false;" />
                </div>
            </div>

            <!-- Templates List -->
            <asp:Panel ID="pnlTemplates" runat="server">
                <div class="templates-list">
                    <asp:Repeater ID="rptTemplates" runat="server" OnItemCommand="rptTemplates_ItemCommand" OnItemDataBound="rptTemplates_ItemDataBound">
                        <ItemTemplate>
                            <div class="template-card">
                                <div class="template-header">
                                    <div class="template-title"><%# Eval("Title") %></div>
                                </div>

                                <div class="template-meta">
                                    <span>Category: <%# Eval("Category") %></span>
                                    <span>Points: <%# Eval("PointsReward") %></span>
                                    <%# !string.IsNullOrEmpty(Eval("Priority")?.ToString()) ? "<span>Priority: " + Eval("Priority") + "</span>" : "" %>
                                    <%# !string.IsNullOrEmpty(Eval("Difficulty")?.ToString()) ? "<span>Difficulty: " + Eval("Difficulty") + "</span>" : "" %>
                                    <%# Eval("EstimatedMinutes") != DBNull.Value ? "<span>Estimated: " + Eval("EstimatedMinutes") + " min</span>" : "" %>
                                </div>

                                <div class="template-description">
                                    <p><%# Eval("Description") %></p>
                                </div>

                                <div class="template-objectives">
                                    <h4>Objectives:</h4>
                                    <asp:Repeater ID="rptObjectives" runat="server">
                                        <ItemTemplate>
                                            <div class="objective-item">
                                                <%# Eval("ObjectiveText") %>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>

                                <div class="template-actions">
                                    <asp:Button ID="btnUseTemplate" runat="server" Text="Use Template" CommandName="UseTemplate" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-success" />
                                    <asp:Button ID="btnEditTemplate" runat="server" Text="Edit" CommandName="Edit" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-primary" />
                                    <asp:Button ID="btnDeleteTemplate" runat="server" Text="Delete" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-danger" OnClientClick='<%# "return confirm(\"Are you sure you want to delete template '" + Eval("Title") + "'?\");" %>' />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty-state">
                <p>No templates found. Create your first template to get started!</p>
            </asp:Panel>
        </div>
    </form>
</body>
</html>

