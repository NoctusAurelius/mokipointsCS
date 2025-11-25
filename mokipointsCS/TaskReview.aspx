<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaskReview.aspx.cs" Inherits="mokipointsCS.TaskReview" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Task Review - MOKI POINTS</title>
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
            border-color: #0066CC;
            transform: scale(1.05);
            box-shadow: 0 2px 8px rgba(0, 102, 204, 0.3);
        }
        
        .profile-avatar-placeholder {
            width: 45px;
            height: 45px;
            border-radius: 50%;
            border: 2px solid #e0e0e0;
            background: linear-gradient(135deg, #0066CC 0%, #0052a3 100%);
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
            border-color: #0066CC;
            transform: scale(1.05);
            box-shadow: 0 2px 8px rgba(0, 102, 204, 0.3);
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

        /* Message Styles */
        .message-container {
            position: fixed;
            top: 80px;
            right: 30px;
            z-index: 1000;
            max-width: 400px;
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

        @keyframes slideOutRight {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(100%);
                opacity: 0;
            }
        }

        .message {
            padding: 16px 20px;
            border-radius: 8px;
            margin-bottom: 15px;
            font-weight: 500;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            display: flex;
            align-items: center;
            gap: 12px;
            position: relative;
            animation: slideInRight 0.3s ease-out;
        }

        .message.hiding {
            animation: slideOutRight 0.3s ease-out;
        }

        .message-icon {
            font-size: 24px;
            flex-shrink: 0;
        }

        .message-content {
            flex: 1;
            font-size: 15px;
            line-height: 1.4;
        }

        .message-close {
            background: none;
            border: none;
            font-size: 20px;
            cursor: pointer;
            color: inherit;
            opacity: 0.7;
            padding: 0;
            width: 24px;
            height: 24px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            transition: all 0.2s;
            flex-shrink: 0;
        }

        .message-close:hover {
            opacity: 1;
            background: rgba(0, 0, 0, 0.1);
        }

        .success-message {
            background: linear-gradient(135deg, #d4edda 0%, #c3e6cb 100%);
            color: #155724;
            border-left: 4px solid #28a745;
        }

        .success-message .message-icon {
            color: #28a745;
        }

        .error-message {
            background: linear-gradient(135deg, #f8d7da 0%, #f5c6cb 100%);
            color: #721c24;
            border-left: 4px solid #dc3545;
        }

        .error-message .message-icon {
            color: #dc3545;
        }

        .hidden {
            display: none;
        }

        /* Confirmation Modal */
        .confirmation-modal {
            display: none;
            position: fixed;
            z-index: 2000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgba(0, 0, 0, 0.5);
            animation: fadeIn 0.2s ease-out;
        }

        @keyframes fadeIn {
            from { opacity: 0; }
            to { opacity: 1; }
        }

        .confirmation-modal.show {
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .confirmation-content {
            background-color: white;
            margin: auto;
            padding: 30px;
            border-radius: 12px;
            width: 90%;
            max-width: 450px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
            animation: slideDown 0.3s ease-out;
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

        .confirmation-icon {
            font-size: 48px;
            text-align: center;
            margin-bottom: 15px;
        }

        .confirmation-icon.warning {
            color: #ffc107;
        }

        .confirmation-icon.danger {
            color: #dc3545;
        }

        .confirmation-title {
            font-size: 20px;
            font-weight: 600;
            color: #333;
            text-align: center;
            margin-bottom: 15px;
        }

        .confirmation-message {
            font-size: 15px;
            color: #666;
            text-align: center;
            margin-bottom: 25px;
            line-height: 1.5;
        }

        .confirmation-actions {
            display: flex;
            gap: 10px;
            justify-content: center;
        }

        .btn-confirm {
            padding: 10px 24px;
            border: none;
            border-radius: 6px;
            font-size: 15px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s;
        }

        .btn-confirm-primary {
            background: #0066CC;
            color: white;
        }

        .btn-confirm-primary:hover {
            background: #0052a3;
            transform: translateY(-1px);
            box-shadow: 0 2px 8px rgba(0, 102, 204, 0.3);
        }

        .btn-confirm-danger {
            background: #dc3545;
            color: white;
        }

        .btn-confirm-danger:hover {
            background: #c82333;
            transform: translateY(-1px);
            box-shadow: 0 2px 8px rgba(220, 53, 69, 0.3);
        }

        .btn-confirm-secondary {
            background: #6c757d;
            color: white;
        }

        .btn-confirm-secondary:hover {
            background: #5a6268;
        }

        .fail-button {
            padding: 10px 20px;
            background-color: #dc3545;
            color: white;
            border: none;
            border-radius: 6px;
            font-size: 14px;
            font-weight: 500;
            cursor: pointer;
            transition: all 0.3s;
            display: inline-flex;
            align-items: center;
            gap: 8px;
        }

        .fail-button:hover {
            background-color: #c82333;
            transform: translateY(-1px);
            box-shadow: 0 2px 8px rgba(220, 53, 69, 0.3);
        }

        .fail-button.active {
            background-color: #28a745;
        }

        .fail-button.active:hover {
            background-color: #218838;
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

        .task-description {
            margin-bottom: 20px;
            color: #555;
            line-height: 1.6;
        }

        .task-objectives {
            margin-bottom: 20px;
        }

        .task-objectives h4 {
            color: #0066CC;
            margin-bottom: 10px;
            font-size: 16px;
        }

        .objective-item {
            padding: 10px;
            margin-bottom: 8px;
            background: #f8f9fa;
            border-radius: 6px;
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .objective-item.completed {
            background: #d4edda;
        }

        .objective-checkbox {
            width: 18px;
            height: 18px;
            cursor: pointer;
        }

        .review-section {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 8px;
            margin-top: 15px;
        }

        .review-section h4 {
            color: #0066CC;
            margin-bottom: 15px;
            font-size: 18px;
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

        .rating-section {
            margin-bottom: 20px;
        }

        .rating-label {
            display: block;
            margin-bottom: 10px;
            font-weight: 500;
            color: #333;
        }

        .star-rating {
            display: flex;
            gap: 3px;
            margin-bottom: 15px;
        }

        .star {
            font-size: 20px;
            color: #ddd;
            cursor: pointer;
            transition: color 0.2s, transform 0.2s;
        }

        .star:hover {
            transform: scale(1.1);
        }

        .star.active {
            color: #ffc107;
        }

        .star.selected {
            color: #ffc107;
        }

        .star.empty {
            color: #ddd;
        }

        .fail-option {
            margin-bottom: 20px;
        }

        .fail-checkbox {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .fail-checkbox input[type="checkbox"] {
            width: 20px;
            height: 20px;
            cursor: pointer;
        }

        .fail-checkbox label {
            font-weight: 500;
            color: #dc3545;
            cursor: pointer;
        }

        .review-actions {
            display: flex;
            gap: 10px;
            margin-top: 20px;
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
            background: #0066CC;
            color: white;
        }

        .btn-primary:hover {
            background: #0052a3;
            transform: translateY(-2px);
            box-shadow: 0 4px 8px rgba(0, 102, 204, 0.3);
        }

        .btn-danger {
            background: #dc3545;
            color: white;
        }

        .btn-danger:hover {
            background: #c82333;
        }

        .btn-secondary {
            background: #6c757d;
            color: white;
        }

        .btn-secondary:hover {
            background: #5a6268;
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

        .child-info {
            background: #e7f3ff;
            padding: 12px;
            border-radius: 6px;
            margin-bottom: 15px;
            font-weight: 500;
            color: #0066cc;
        }

        .points-display {
            background: #fff3cd;
            padding: 12px;
            border-radius: 6px;
            margin-bottom: 15px;
            font-weight: 500;
            color: #856404;
        }

        @media (max-width: 768px) {
            .task-header {
                flex-direction: column;
            }

            .review-actions {
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
            <div class="header-content">
                <div class="brand">
                    <span class="moki">MOKI</span><span class="points"> POINTS</span>
                </div>
                <div class="user-info">
                    <div class="nav-links" style="display: flex; gap: 20px; align-items: center; margin-right: 20px;">
                        <a href="ParentDashboard.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Dashboard</a>
                        <a href="Family.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Family</a>
                        <a href="Tasks.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Tasks</a>
                        <a href="TaskReview.aspx" class="active" style="color: #0066CC; text-decoration: none; font-weight: 500; font-size: 16px;">Review</a>
                        <a href="Rewards.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Rewards</a>
                        <a href="RewardOrders.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Orders</a>
                        <a href="OrderHistory.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Order History</a>
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
                <h1 class="page-title">Task Review</h1>
            </div>

            <!-- Success/Error Messages -->
            <div class="message-container">
                <asp:Panel ID="pnlSuccess" runat="server" CssClass="message success-message hidden">
                    <span class="message-icon">&#10003;</span>
                    <span class="message-content">
                        <asp:Literal ID="lblSuccess" runat="server"></asp:Literal>
                    </span>
                    <button type="button" class="message-close" onclick="closeMessage(this)">&#215;</button>
                </asp:Panel>
                <asp:Panel ID="pnlError" runat="server" CssClass="message error-message hidden">
                    <span class="message-icon">&#9888;</span>
                    <span class="message-content">
                        <asp:Literal ID="lblError" runat="server"></asp:Literal>
                    </span>
                    <button type="button" class="message-close" onclick="closeMessage(this)">&#215;</button>
                </asp:Panel>
            </div>

            <!-- Tasks Pending Review -->
            <asp:Panel ID="pnlTasks" runat="server">
                <div class="tasks-list">
                    <asp:Repeater ID="rptTasks" runat="server" OnItemCommand="rptTasks_ItemCommand" OnItemDataBound="rptTasks_ItemDataBound">
                        <ItemTemplate>
                            <div class="task-card">
                                <div class="task-header">
                                    <div class="task-title"><%# Eval("Title") %></div>
                                </div>

                                <div class="child-info">
                                    Completed by: <strong><%# Eval("ChildName") %></strong>
                                </div>

                                <div class="task-meta">
                                    <span>Category: <%# Eval("Category") %></span>
                                    <span>Points: <%# Eval("PointsReward") %></span>
                                    <span>Completed: <%# Convert.ToDateTime(Eval("CompletedDate")).ToString("MMM dd, yyyy HH:mm") %></span>
                                    <%# Eval("Deadline") != DBNull.Value ? "<span>Deadline: " + Convert.ToDateTime(Eval("Deadline")).ToString("MMM dd, yyyy HH:mm") + "</span>" : "" %>
                                </div>

                                <div class="task-description">
                                    <p><%# Eval("Description") %></p>
                                </div>

                                <div class="task-objectives">
                                    <h4>Objectives:</h4>
                                    <asp:Repeater ID="rptObjectives" runat="server" OnItemDataBound="rptObjectives_ItemDataBound">
                                        <ItemTemplate>
                                            <div class="objective-item <%# Convert.ToBoolean(Eval("IsCompleted")) ? "completed" : "" %>">
                                                <asp:CheckBox ID="chkObjective" runat="server" Checked='<%# Convert.ToBoolean(Eval("IsCompleted")) %>' Enabled="false" CssClass="objective-checkbox" />
                                                <span><%# Eval("ObjectiveText") %></span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>

                                <div class="review-section">
                                    <h4>Review Task</h4>
                                    
                                    <div class="rating-section">
                                        <label class="rating-label">Rating (1-5 stars):</label>
                                        <div class="star-rating" data-assignment-id='<%# Eval("AssignmentId") %>'>
                                            <span class="star" data-rating="1" onclick="setRating(<%# Eval("AssignmentId") %>, 1)">&#9733;</span>
                                            <span class="star" data-rating="2" onclick="setRating(<%# Eval("AssignmentId") %>, 2)">&#9733;</span>
                                            <span class="star" data-rating="3" onclick="setRating(<%# Eval("AssignmentId") %>, 3)">&#9733;</span>
                                            <span class="star" data-rating="4" onclick="setRating(<%# Eval("AssignmentId") %>, 4)">&#9733;</span>
                                            <span class="star" data-rating="5" onclick="setRating(<%# Eval("AssignmentId") %>, 5)">&#9733;</span>
                                        </div>
                                        <asp:HiddenField ID="hidRating" runat="server" Value="0" />
                                    </div>

                                    <div class="review-actions">
                                        <asp:Button ID="btnFailTask" runat="server" Text="Fail Task" CommandName="Fail" CommandArgument='<%# Eval("AssignmentId") %>' CssClass="btn btn-danger" OnClientClick='<%# "return confirmFailTask(" + Eval("AssignmentId") + ", this);" %>' UseSubmitBehavior="true" />
                                        <asp:Button ID="btnSubmitReview" runat="server" Text="Submit Review" CommandName="Review" CommandArgument='<%# Eval("AssignmentId") %>' CssClass="btn btn-primary" OnClientClick='<%# "return validateReview(" + Eval("AssignmentId") + ", this);" %>' UseSubmitBehavior="true" />
                                    </div>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlEmpty" runat="server" Visible="false" CssClass="empty-state">
                <p>No tasks pending review. All caught up! <span style="font-size: 1.2em;">&#127881;</span></p>
            </asp:Panel>
        </div>

        <!-- Confirmation Modal for Review Submission -->
        <div id="reviewConfirmModal" class="confirmation-modal">
            <div class="confirmation-content">
                <div class="confirmation-icon warning">&#9888;</div>
                <div class="confirmation-title">Confirm Review Submission</div>
                <div class="confirmation-message" id="reviewConfirmMessage">
                    Are you sure you want to submit this review? This action cannot be undone.
                </div>
                <div class="confirmation-actions">
                    <button type="button" class="btn-confirm btn-confirm-secondary" onclick="closeReviewConfirmModal()">Cancel</button>
                    <button type="button" class="btn-confirm btn-confirm-primary" id="btnConfirmReview">Submit Review</button>
                </div>
            </div>
        </div>

        <!-- Confirmation Modal for Mark as Failed -->
        <div id="failConfirmModal" class="confirmation-modal">
            <div class="confirmation-content">
                <div class="confirmation-icon danger">&#9888;</div>
                <div class="confirmation-title">Mark Task as Failed</div>
                <div class="confirmation-message">
                    Are you sure you want to fail this task? The child will lose 50% of the task points. This action cannot be undone.
                </div>
                <div class="confirmation-actions">
                    <button type="button" class="btn-confirm btn-confirm-secondary" onclick="closeFailConfirmModal()">Cancel</button>
                    <button type="button" class="btn-confirm btn-confirm-danger" id="btnConfirmFail">Yes, Mark as Failed</button>
                </div>
            </div>
        </div>

        <script type="text/javascript">
            // Auto-hide messages after 5 seconds
            document.addEventListener('DOMContentLoaded', function() {
                var successMessage = document.querySelector('.success-message:not(.hidden)');
                var errorMessage = document.querySelector('.error-message:not(.hidden)');
                
                if (successMessage) {
                    setTimeout(function() {
                        var closeBtn = successMessage.querySelector('.message-close');
                        if (closeBtn) closeMessage(closeBtn);
                    }, 5000);
                }
                
                if (errorMessage) {
                    setTimeout(function() {
                        var closeBtn = errorMessage.querySelector('.message-close');
                        if (closeBtn) closeMessage(closeBtn);
                    }, 7000);
                }
            });

            function closeMessage(closeButton) {
                var message = closeButton.closest('.message');
                if (message) {
                    message.classList.add('hiding');
                    setTimeout(function() {
                        message.classList.add('hidden');
                        message.classList.remove('hiding');
                    }, 300);
                }
            }

            var ratings = {}; // Store ratings per assignment

            function setRating(assignmentId, rating) {
                ratings[assignmentId] = rating;
                
                // Update star display
                var stars = document.querySelectorAll('[data-assignment-id="' + assignmentId + '"] .star');
                stars.forEach(function(star, index) {
                    if (index < rating) {
                        star.classList.add('selected');
                        star.classList.add('active');
                    } else {
                        star.classList.remove('selected');
                        star.classList.remove('active');
                    }
                });

                // Update hidden field
                var hiddenField = document.querySelector('[data-assignment-id="' + assignmentId + '"]').closest('.review-section').querySelector('input[type="hidden"][id*="hidRating"]');
                if (hiddenField) {
                    hiddenField.value = rating;
                }
            }

            var currentAssignmentId = null;
            var pendingSubmitButton = null;

            function showError(message) {
                var errorPanel = document.getElementById('<%= pnlError.ClientID %>');
                var errorLabel = document.getElementById('<%= lblError.ClientID %>');
                if (errorPanel && errorLabel) {
                    errorLabel.textContent = message;
                    errorPanel.classList.remove('hidden');
                    errorPanel.classList.add('error-message');
                    // Auto-hide after 7 seconds
                    setTimeout(function() {
                        closeMessage(errorPanel.querySelector('.message-close'));
                    }, 7000);
                }
            }

            function validateReview(assignmentId, buttonElement) {
                console.log('validateReview called for assignment:', assignmentId);
                var rating = ratings[assignmentId] || 0;

                if (rating === 0) {
                    showError('Please select a rating (1-5 stars) before submitting.');
                    return false;
                }

                if (rating < 1 || rating > 5) {
                    showError('Please select a valid rating (1-5 stars).');
                    return false;
                }

                // Store assignment ID and button reference directly
                currentAssignmentId = assignmentId;
                pendingSubmitButton = buttonElement;
                
                if (!pendingSubmitButton) {
                    console.error('validateReview: Button element is null');
                    alert('Error: Could not find submit button. Please refresh the page.');
                    return false;
                }
                
                console.log('validateReview: Stored submit button, showing modal');
                
                // Update message with rating
                var message = 'Are you sure you want to submit this review? Rating: ' + rating + ' stars. Points will be awarded to the child. This action cannot be undone.';
                
                document.getElementById('reviewConfirmMessage').textContent = message;
                showReviewConfirmModal();
                
                return false; // Prevent default submission
            }

            function confirmFailTask(assignmentId, buttonElement) {
                console.log('confirmFailTask called for assignment:', assignmentId);
                
                // Store assignment ID and button reference directly
                currentFailAssignmentId = assignmentId;
                pendingFailButton = buttonElement;
                
                if (!pendingFailButton) {
                    console.error('confirmFailTask: Button element is null');
                    alert('Error: Could not find fail button. Please refresh the page.');
                    return false;
                }
                
                console.log('confirmFailTask: Stored fail button, showing modal');
                showFailConfirmModal();
                return false; // Prevent default submission
            }

            function showReviewConfirmModal() {
                document.getElementById('reviewConfirmModal').classList.add('show');
            }

            function closeReviewConfirmModal() {
                document.getElementById('reviewConfirmModal').classList.remove('show');
                currentAssignmentId = null;
                pendingSubmitButton = null;
            }

            // Handle confirm review button click
            var btnConfirmReview = document.getElementById('btnConfirmReview');
            if (btnConfirmReview) {
                btnConfirmReview.addEventListener('click', function() {
                    console.log('btnConfirmReview clicked. pendingSubmitButton:', pendingSubmitButton);
                    if (pendingSubmitButton) {
                        // Store button reference and info before closing modal (which clears it)
                        var buttonToClick = pendingSubmitButton;
                        var buttonName = buttonToClick.name;
                        var buttonId = buttonToClick.id;
                        
                        console.log('Stored button reference:', buttonToClick);
                        console.log('Button ID:', buttonId);
                        console.log('Button name:', buttonName);
                        
                        closeReviewConfirmModal();
                        
                        // Remove onclick handler temporarily to prevent infinite loop, then trigger postback
                        console.log('Removing onclick handler and triggering postback...');
                        setTimeout(function() {
                            try {
                                // Store original onclick handler
                                var originalOnclick = buttonToClick.getAttribute('onclick');
                                
                                console.log('Original onclick attribute:', originalOnclick);
                                
                                // Remove onclick handler completely
                                buttonToClick.removeAttribute('onclick');
                                buttonToClick.onclick = null;
                                
                                console.log('Onclick handler removed. Clicking button now...');
                                
                                // Trigger the click
                                buttonToClick.click();
                                
                                console.log('Button click triggered. Postback should occur.');
                                
                                // Note: We don't restore the onclick because the page will reload after postback
                            } catch (ex) {
                                console.error('Error triggering postback:', ex);
                                console.error('Stack trace:', ex.stack);
                                alert('Error: Failed to submit review. Error: ' + ex.message + '. Please try again.');
                            }
                        }, 100);
                    } else {
                        console.error('Cannot submit review: pendingSubmitButton is null');
                        alert('Error: Could not process review submission. Please refresh the page and try again.');
                    }
                });
            } else {
                console.error('btnConfirmReview element not found');
            }

            // Close modal when clicking outside
            document.getElementById('reviewConfirmModal').addEventListener('click', function(e) {
                if (e.target === this) {
                    closeReviewConfirmModal();
                }
            });

            var currentFailAssignmentId = null;
            var pendingFailButton = null;

            function showFailConfirmModal() {
                document.getElementById('failConfirmModal').classList.add('show');
            }

            function closeFailConfirmModal() {
                document.getElementById('failConfirmModal').classList.remove('show');
                currentFailAssignmentId = null;
                pendingFailButton = null;
            }

            // Handle confirm fail button click
            var btnConfirmFail = document.getElementById('btnConfirmFail');
            if (btnConfirmFail) {
                btnConfirmFail.addEventListener('click', function() {
                    console.log('btnConfirmFail clicked. pendingFailButton:', pendingFailButton, 'currentFailAssignmentId:', currentFailAssignmentId);
                    if (pendingFailButton && currentFailAssignmentId) {
                        // Store button reference and info before closing modal (which clears it)
                        var buttonToClick = pendingFailButton;
                        var assignmentId = currentFailAssignmentId;
                        var buttonName = buttonToClick.name;
                        var buttonId = buttonToClick.id;
                        
                        console.log('Stored button reference:', buttonToClick);
                        console.log('Button ID:', buttonId);
                        console.log('Button name:', buttonName);
                        console.log('Button type:', buttonToClick.type);
                        console.log('Assignment ID:', assignmentId);
                        
                        closeFailConfirmModal();
                        
                        // Remove onclick handler temporarily to prevent infinite loop, then trigger postback
                        console.log('Removing onclick handler and triggering postback...');
                        setTimeout(function() {
                            try {
                                // Store original onclick handler
                                var originalOnclick = buttonToClick.getAttribute('onclick');
                                var originalOnclickHandler = buttonToClick.onclick;
                                
                                console.log('Original onclick attribute:', originalOnclick);
                                
                                // Remove onclick handler completely
                                buttonToClick.removeAttribute('onclick');
                                buttonToClick.onclick = null;
                                
                                console.log('Onclick handler removed. Clicking button now...');
                                
                                // Trigger the click
                                buttonToClick.click();
                                
                                console.log('Button click triggered. Postback should occur.');
                                
                                // Note: We don't restore the onclick because the page will reload after postback
                            } catch (ex) {
                                console.error('Error triggering postback:', ex);
                                console.error('Stack trace:', ex.stack);
                                alert('Error: Failed to submit fail request. Error: ' + ex.message + '. Please try again.');
                            }
                        }, 100);
                    } else {
                        console.error('Cannot fail task: pendingFailButton:', pendingFailButton, 'currentFailAssignmentId:', currentFailAssignmentId);
                        alert('Error: Could not process fail request. Please refresh the page and try again.');
                    }
                });
            } else {
                console.error('btnConfirmFail element not found');
            }

            // Close modal when clicking outside
            document.getElementById('failConfirmModal').addEventListener('click', function(e) {
                if (e.target === this) {
                    closeFailConfirmModal();
                }
            });
        </script>
    </form>
</body>
</html>

