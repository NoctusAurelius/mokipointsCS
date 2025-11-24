<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ChildTasks.aspx.cs" Inherits="mokipointsCS.ChildTasks" EnableEventValidation="false" %>

<%@ Register Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31BF3856AD364E35" Namespace="System.Web.UI" TagPrefix="asp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>My Tasks - MOKI POINTS</title>
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
        
        /* Container */
        .container {
            max-width: 1000px;
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
        
        .btn-history {
            padding: 10px 20px;
            background-color: #666;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            text-decoration: none;
            display: inline-block;
        }
        
        .btn-history:hover {
            background-color: #555;
        }
        
        /* Task Cards */
        .tasks-list {
            display: flex;
            flex-direction: column;
            gap: 20px;
        }
        
        .task-card {
            background-color: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            transition: box-shadow 0.3s;
        }
        
        .task-card:hover {
            box-shadow: 0 4px 8px rgba(0,0,0,0.15);
        }
        
        .task-card.assigned {
            border-left: 4px solid #1976D2;
        }
        
        .task-card.ongoing {
            border-left: 4px solid #F57C00;
        }
        
        .task-card.pending-review {
            border-left: 4px solid #7B1FA2;
        }
        
        .task-header {
            display: flex;
            justify-content: space-between;
            align-items: start;
            margin-bottom: 15px;
        }
        
        .task-title {
            font-size: 22px;
            font-weight: bold;
            color: #333;
        }
        
        .task-status {
            padding: 6px 12px;
            border-radius: 15px;
            font-size: 12px;
            font-weight: 500;
        }
        
        .status-assigned {
            background-color: #E3F2FD;
            color: #1976D2;
        }
        
        .status-ongoing {
            background-color: #FFF3E0;
            color: #F57C00;
        }
        
        .status-pending-review {
            background-color: #F3E5F5;
            color: #7B1FA2;
        }
        
        .task-meta {
            display: flex;
            gap: 20px;
            margin-bottom: 15px;
            font-size: 14px;
            color: #666;
            flex-wrap: wrap;
        }
        
        .task-meta span {
            display: flex;
            align-items: center;
            gap: 5px;
        }
        
        .task-points {
            font-size: 28px;
            font-weight: bold;
            color: #FF6600;
            margin: 15px 0;
        }
        
        .task-description {
            color: #666;
            font-size: 14px;
            margin-bottom: 20px;
            line-height: 1.6;
        }
        
        /* Deadline Warning */
        .deadline-warning {
            padding: 10px;
            border-radius: 5px;
            margin-bottom: 15px;
            font-size: 14px;
            font-weight: 500;
        }
        
        .deadline-warning.green {
            background-color: #E8F5E9;
            color: #2e7d32;
            border-left: 3px solid #2e7d32;
        }
        
        .deadline-warning.yellow {
            background-color: #FFF9C4;
            color: #F57C00;
            border-left: 3px solid #F57C00;
        }
        
        .deadline-warning.orange {
            background-color: #FFE0B2;
            color: #E65100;
            border-left: 3px solid #E65100;
        }
        
        .deadline-warning.red {
            background-color: #FFCDD2;
            color: #C62828;
            border-left: 3px solid #C62828;
        }
        
        /* Objectives Section */
        .objectives-section {
            margin: 20px 0;
            padding: 20px;
            background-color: #f9f9f9;
            border-radius: 8px;
        }
        
        .objectives-title {
            font-size: 16px;
            font-weight: 500;
            margin-bottom: 15px;
            color: #333;
        }
        
        .objective-item {
            display: flex;
            align-items: center;
            gap: 12px;
            padding: 12px;
            background-color: white;
            border-radius: 5px;
            margin-bottom: 10px;
        }
        
        .objective-checkbox {
            width: 20px;
            height: 20px;
            cursor: pointer;
        }
        
        .objective-text {
            flex: 1;
            font-size: 14px;
            color: #333;
        }
        
        .objective-text.completed {
            text-decoration: line-through;
            color: #999;
        }
        
        .progress-bar {
            width: 100%;
            height: 8px;
            background-color: #e0e0e0;
            border-radius: 4px;
            overflow: hidden;
            margin-top: 15px;
        }
        
        .progress-fill {
            height: 100%;
            background-color: #0066CC;
            transition: width 0.3s;
        }
        
        .progress-text {
            text-align: center;
            margin-top: 8px;
            font-size: 12px;
            color: #666;
        }
        
        /* Actions */
        .task-actions {
            display: flex;
            gap: 10px;
            margin-top: 20px;
            padding-top: 20px;
            border-top: 1px solid #e0e0e0;
        }
        
        .btn-action {
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            transition: all 0.3s;
        }
        
        .btn-accept {
            background-color: #2e7d32;
            color: white;
        }
        
        .btn-accept:hover {
            background-color: #1b5e20;
        }
        
        .btn-decline {
            background-color: #d32f2f;
            color: white;
        }
        
        .btn-decline:hover {
            background-color: #b71c1c;
        }
        
        .btn-submit {
            background-color: #0066CC;
            color: white;
        }
        
        .btn-submit:hover {
            background-color: #0052a3;
        }
        
        .btn-submit:disabled {
            background-color: #ccc;
            cursor: not-allowed;
            opacity: 0.6;
        }
        
        .message {
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
        }
        
        .message.success {
            background-color: #e8f5e9;
            color: #2e7d32;
            border-left: 4px solid #2e7d32;
        }
        
        .message.error {
            background-color: #ffebee;
            color: #d32f2f;
            border-left: 4px solid #d32f2f;
        }
        
        .empty-state {
            text-align: center;
            padding: 60px 20px;
            color: #999;
        }
        
        .empty-state h3 {
            font-size: 24px;
            margin-bottom: 10px;
            color: #666;
        }
    </style>
    <script>
        // Objective checkbox change handler (Fix #1: Server-side tracking)
        function objectiveChanged(assignmentId, objectiveId, checkbox) {
            var isChecked = checkbox.checked;
            
            // Update UI immediately
            var textElement = checkbox.nextElementSibling;
            if (textElement) {
                if (isChecked) {
                    textElement.classList.add('completed');
                } else {
                    textElement.classList.remove('completed');
                }
            }
            
            // Update server-side (Fix #1) - Use PageMethods or direct postback
            PageMethods.UpdateObjective(assignmentId, objectiveId, isChecked, 
                function(result) {
                    if (result && result.Success) {
                        updateProgress(assignmentId);
                        checkCanSubmit(assignmentId);
                    } else {
                        // Revert checkbox if server update failed
                        checkbox.checked = !isChecked;
                        if (textElement) {
                            textElement.classList.toggle('completed');
                        }
                        alert('Failed to update objective. Please try again.');
                    }
                },
                function(error) {
                    // Revert checkbox on error
                    checkbox.checked = !isChecked;
                    if (textElement) {
                        textElement.classList.toggle('completed');
                    }
                    alert('Error updating objective. Please try again.');
                }
            );
        }
        
        // Update progress bar
        function updateProgress(assignmentId) {
            var container = document.querySelector('[data-assignment-id="' + assignmentId + '"]');
            if (!container) return;
            
            var checkboxes = container.querySelectorAll('.objective-checkbox');
            var checked = container.querySelectorAll('.objective-checkbox:checked').length;
            var total = checkboxes.length;
            var percentage = total > 0 ? (checked / total) * 100 : 0;
            
            var progressFill = container.querySelector('.progress-fill');
            var progressText = container.querySelector('.progress-text');
            
            if (progressFill) {
                progressFill.style.width = percentage + '%';
            }
            if (progressText) {
                progressText.textContent = checked + ' of ' + total + ' objectives completed';
            }
        }
        
        // Check if can submit (all objectives completed)
        function checkCanSubmit(assignmentId) {
            var container = document.querySelector('[data-assignment-id="' + assignmentId + '"]');
            if (!container) return;
            
            var checkboxes = container.querySelectorAll('.objective-checkbox');
            var checked = container.querySelectorAll('.objective-checkbox:checked').length;
            var total = checkboxes.length;
            var allCompleted = total > 0 && checked === total;
            
            var submitBtn = container.querySelector('.btn-submit');
            if (submitBtn) {
                submitBtn.disabled = !allCompleted;
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <!-- Header -->
        <div class="header">
            <div class="header-content">
                <div class="brand">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
                <div class="user-info">
                    <div class="nav-links">
                        <a href="ChildDashboard.aspx">Dashboard</a>
                        <a href="ChildTasks.aspx" class="active">My Tasks</a>
                        <a href="TaskHistory.aspx">History</a>
                        <a href="PointsHistory.aspx">Points</a>
                        <a href="RewardShop.aspx">Shop</a>
                        <a href="Cart.aspx">Cart</a>
                        <a href="MyOrders.aspx">My Orders</a>
                    </div>
                    <span class="user-name">Welcome, <asp:Literal ID="litUserName" runat="server"></asp:Literal></span>
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
            <div class="page-header">
                <h1 class="page-title">My Tasks</h1>
                <a href="TaskHistory.aspx" class="btn-history">View History</a>
            </div>

            <!-- Messages -->
            <asp:Panel ID="pnlSuccess" runat="server" CssClass="message success" Visible="false">
                <asp:Label ID="lblSuccess" runat="server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlError" runat="server" CssClass="message error" Visible="false">
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </asp:Panel>

            <!-- Tasks List -->
            <asp:Panel ID="pnlTasks" runat="server">
                <div class="tasks-list">
                    <asp:Repeater ID="rptTasks" runat="server" OnItemCommand="rptTasks_ItemCommand" OnItemDataBound="rptTasks_ItemDataBound">
                        <ItemTemplate>
                            <div class="task-card <%# Eval("Status").ToString().ToLower().Replace(" ", "-") %>" data-assignment-id='<%# Eval("AssignmentId") %>'>
                                <div class="task-header">
                                    <div class="task-title"><%# Eval("Title") %></div>
                                    <span class="task-status status-<%# Eval("Status").ToString().ToLower().Replace(" ", "-") %>"><%# Eval("Status") %></span>
                                </div>
                                
                                <div class="task-meta">
                                    <span>Category: <%# Eval("Category") %></span>
                                    <span>Assigned: <%# Convert.ToDateTime(Eval("AssignedDate")).ToString("MMM dd, yyyy") %></span>
                                    <%# Eval("Deadline") != DBNull.Value ? "<span>Deadline: " + Convert.ToDateTime(Eval("Deadline")).ToString("MMM dd, yyyy HH:mm") + "</span>" : "" %>
                                </div>
                                
                                <!-- Deadline Warning (Fix #15) -->
                                <asp:Panel ID="pnlDeadlineWarning" runat="server" Visible="false" CssClass="deadline-warning">
                                    <asp:Literal ID="litDeadlineWarning" runat="server"></asp:Literal>
                                </asp:Panel>
                                
                                <div class="task-points"><%# Eval("PointsReward") %> points</div>
                                
                                <%# Eval("Description") != DBNull.Value && !string.IsNullOrEmpty(Eval("Description").ToString()) ? "<div class='task-description'>" + Server.HtmlEncode(Eval("Description").ToString()) + "</div>" : "" %>
                                
                                <!-- Objectives Section (Fix #1: Server-side tracking) -->
                                <asp:Panel ID="pnlObjectives" runat="server" Visible="false" CssClass="objectives-section">
                                    <div class="objectives-title">Objectives:</div>
                                    <asp:Repeater ID="rptObjectives" runat="server">
                                        <ItemTemplate>
                                            <div class="objective-item">
                                                <input type="checkbox" 
                                                    class="objective-checkbox" 
                                                    data-assignment-id='<%# Eval("AssignmentId") %>'
                                                    data-objective-id='<%# Eval("TaskObjectiveId") %>'
                                                    <%# Convert.ToBoolean(Eval("IsCompleted")) ? "checked" : "" %>
                                                    onchange='objectiveChanged(<%# Eval("AssignmentId") %>, <%# Eval("TaskObjectiveId") %>, this)' />
                                                <span class="objective-text <%# Convert.ToBoolean(Eval("IsCompleted")) ? "completed" : "" %>"><%# Eval("ObjectiveText") %></span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                    <div class="progress-bar">
                                        <div class="progress-fill" style="width: 0%;"></div>
                                    </div>
                                    <div class="progress-text">0 of 0 objectives completed</div>
                                </asp:Panel>
                                
                                <div class="task-actions">
                                    <asp:Button ID="btnAccept" runat="server" Text="Accept" CssClass="btn-action btn-accept" 
                                        CommandName="Accept" CommandArgument='<%# Eval("AssignmentId") %>' 
                                        Visible='<%# Eval("Status").ToString() == "Assigned" %>' />
                                    <asp:Button ID="btnDecline" runat="server" Text="Decline" CssClass="btn-action btn-decline" 
                                        CommandName="Decline" CommandArgument='<%# Eval("AssignmentId") %>' 
                                        Visible='<%# Eval("Status").ToString() == "Assigned" %>' />
                                    <asp:Button ID="btnSubmit" runat="server" Text="Submit for Review" CssClass="btn-action btn-submit" 
                                        CommandName="Submit" CommandArgument='<%# Eval("AssignmentId") %>' 
                                        Visible='<%# Eval("Status").ToString() == "Ongoing" %>' 
                                        Enabled="false" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>
            
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="empty-state">
                    <h3>No tasks assigned</h3>
                    <p>You don't have any tasks yet. Check back later!</p>
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>

