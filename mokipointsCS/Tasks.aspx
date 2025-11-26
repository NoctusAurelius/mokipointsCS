<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Tasks.aspx.cs" Inherits="mokipointsCS.Tasks" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Tasks - MOKI POINTS</title>
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
        
        .btn-create {
            padding: 12px 24px;
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 500;
            text-decoration: none;
            display: inline-block;
            transition: background-color 0.3s;
        }
        
        .btn-create:hover {
            background-color: #0052a3;
        }
        
        /* Search and Filter */
        .search-filter-bar {
            background-color: white;
            padding: 20px;
            border-radius: 10px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-bottom: 20px;
            display: flex;
            gap: 15px;
            flex-wrap: wrap;
            align-items: center;
        }
        
        .search-box {
            flex: 1;
            min-width: 200px;
        }
        
        .search-box input {
            width: 100%;
            padding: 10px;
            border: 1px solid #e0e0e0;
            border-radius: 5px;
            font-size: 14px;
        }
        
        .filter-group {
            display: flex;
            gap: 10px;
            align-items: center;
        }
        
        .filter-group select {
            padding: 10px;
            border: 1px solid #e0e0e0;
            border-radius: 5px;
            font-size: 14px;
        }
        
        /* Task Cards */
        .tasks-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(350px, 1fr));
            gap: 20px;
        }
        
        .task-card {
            background-color: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            transition: box-shadow 0.3s;
            position: relative;
        }
        
        .task-card:hover {
            box-shadow: 0 4px 8px rgba(0,0,0,0.15);
        }
        
        .task-header {
            display: flex;
            justify-content: space-between;
            align-items: start;
            margin-bottom: 15px;
        }
        
        .task-title {
            font-size: 20px;
            font-weight: bold;
            color: #333;
            margin-bottom: 5px;
        }
        
        .task-meta {
            display: flex;
            gap: 15px;
            flex-wrap: wrap;
            margin-bottom: 15px;
            font-size: 14px;
            color: #666;
        }
        
        .task-meta span {
            display: flex;
            align-items: center;
            gap: 5px;
        }
        
        .badge {
            padding: 4px 12px;
            border-radius: 15px;
            font-size: 12px;
            font-weight: 500;
        }
        
        .badge-priority-low { background-color: #E3F2FD; color: #1976D2; }
        .badge-priority-medium { background-color: #FFF3E0; color: #F57C00; }
        .badge-priority-high { background-color: #FFEBEE; color: #C62828; }
        .badge-priority-urgent { background-color: #F3E5F5; color: #7B1FA2; }
        
        .badge-difficulty-easy { background-color: #E8F5E9; color: #388E3C; }
        .badge-difficulty-medium { background-color: #FFF3E0; color: #F57C00; }
        .badge-difficulty-hard { background-color: #FFEBEE; color: #C62828; }
        
        .badge-category {
            background-color: #F5F5F5;
            color: #666;
        }
        
        .task-points {
            font-size: 24px;
            font-weight: bold;
            color: #FF6600;
            margin: 10px 0;
        }
        
        .task-description {
            color: #666;
            font-size: 14px;
            margin-bottom: 15px;
            line-height: 1.5;
        }
        
        .task-actions {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            margin-top: 15px;
            padding-top: 15px;
            border-top: 1px solid #e0e0e0;
        }
        
        .btn-action {
            padding: 8px 16px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            text-decoration: none;
            display: inline-block;
            transition: all 0.3s;
        }
        
        .btn-assign {
            background-color: #0066CC;
            color: white;
        }
        
        .btn-assign:hover {
            background-color: #0052a3;
        }
        
        .btn-edit {
            background-color: #FF6600;
            color: white;
        }
        
        .btn-edit:hover {
            background-color: #e55a00;
        }
        
        .btn-delete {
            background-color: #d32f2f;
            color: white;
        }
        
        .btn-delete:hover {
            background-color: #b71c1c;
        }
        
        .btn-view {
            background-color: #666;
            color: white;
        }
        
        .btn-view:hover {
            background-color: #555;
        }
        
        .btn-action:disabled {
            background-color: #ccc;
            cursor: not-allowed;
            opacity: 0.6;
        }
        
        /* Assignment Status */
        .assignment-status {
            margin-top: 15px;
            padding-top: 15px;
            border-top: 1px solid #e0e0e0;
        }
        
        .assignment-status-title {
            font-size: 12px;
            color: #666;
            margin-bottom: 8px;
            font-weight: 500;
        }
        
        .assignment-list {
            display: flex;
            flex-direction: column;
            gap: 8px;
        }
        
        .assignment-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 8px;
            background-color: #f9f9f9;
            border-radius: 5px;
            font-size: 13px;
        }
        
        .assignment-child-name {
            font-weight: 500;
            color: #333;
        }
        
        .status-badge {
            padding: 4px 10px;
            border-radius: 12px;
            font-size: 11px;
            font-weight: 500;
        }
        
        .status-assigned { background-color: #E3F2FD; color: #1976D2; }
        .status-ongoing { background-color: #FFF3E0; color: #F57C00; }
        .status-pending-review { background-color: #F3E5F5; color: #7B1FA2; }
        .status-completed { background-color: #E8F5E9; color: #388E3C; }
        
        /* Completion History */
        .completion-history {
            margin-top: 15px;
            padding: 10px;
            background-color: #f9f9f9;
            border-radius: 5px;
            font-size: 12px;
            color: #666;
        }
        
        .completion-history strong {
            color: #333;
        }
        
        /* Modals */
        .modal {
            display: none;
            position: fixed;
            z-index: 1000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            overflow: auto;
            background-color: rgba(0,0,0,0.5);
        }
        
        .modal-content {
            background-color: white;
            margin: 5% auto;
            padding: 30px;
            border-radius: 10px;
            width: 90%;
            max-width: 600px;
            max-height: 80vh;
            overflow-y: auto;
            box-shadow: 0 4px 20px rgba(0,0,0,0.3);
        }
        
        .modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 2px solid #e0e0e0;
        }
        
        .modal-title {
            font-size: 24px;
            font-weight: bold;
            color: #333;
        }
        
        .close {
            color: #aaa;
            font-size: 28px;
            font-weight: bold;
            cursor: pointer;
            line-height: 20px;
        }
        
        .close:hover {
            color: #000;
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
        
        .form-group {
            margin-bottom: 20px;
        }
        
        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: 500;
            color: #333;
        }
        
        .form-group label .required {
            color: #d32f2f;
        }
        
        .form-control {
            width: 100%;
            padding: 10px;
            border: 1px solid #e0e0e0;
            border-radius: 5px;
            font-size: 14px;
        }
        
        .form-control:focus {
            outline: none;
            border-color: #0066CC;
        }
        
        textarea.form-control {
            min-height: 100px;
            resize: vertical;
        }
        
        .objectives-container {
            margin-top: 10px;
        }
        
        .objective-item {
            display: flex;
            gap: 10px;
            margin-bottom: 10px;
            align-items: center;
        }
        
        .objective-item input {
            flex: 1;
        }
        
        .btn-remove-objective {
            padding: 8px 12px;
            background-color: #d32f2f;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 12px;
        }
        
        .btn-add-objective {
            padding: 8px 16px;
            background-color: #666;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            margin-top: 10px;
        }
        
        .btn-submit {
            padding: 12px 24px;
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 500;
            width: 100%;
            margin-top: 20px;
        }
        
        .btn-submit:hover {
            background-color: #0052a3;
        }
        
        .btn-cancel {
            padding: 12px 24px;
            background-color: #666;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 500;
            width: 100%;
            margin-top: 10px;
        }
        
        .btn-cancel:hover {
            background-color: #555;
        }
        
        /* Messages */
        .message {
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
            display: none;
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
        
        .hidden {
            display: none;
        }

        /* No Family Message */
        .no-family-message {
            background: linear-gradient(135deg, #fff3e0 0%, #ffe0b2 100%);
            border-radius: 15px;
            padding: 40px;
            text-align: center;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
            margin: 30px 0;
            animation: slideDown 0.5s ease-out;
        }

        @keyframes slideDown {
            from {
                opacity: 0;
                transform: translateY(-20px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .no-family-icon {
            width: 80px;
            height: 80px;
            margin: 0 auto 20px;
            background-color: #ff9800;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 40px;
            color: white;
            box-shadow: 0 4px 15px rgba(255, 152, 0, 0.3);
        }

        .no-family-title {
            font-size: 28px;
            font-weight: bold;
            color: #e65100;
            margin-bottom: 15px;
        }

        .no-family-text {
            font-size: 16px;
            color: #666;
            line-height: 1.6;
            margin-bottom: 25px;
            max-width: 600px;
            margin-left: auto;
            margin-right: auto;
        }

        .no-family-button {
            display: inline-block;
            padding: 14px 30px;
            background: linear-gradient(135deg, #ff9800 0%, #f57c00 100%);
            color: white;
            text-decoration: none;
            border-radius: 8px;
            font-size: 16px;
            font-weight: bold;
            transition: all 0.3s ease;
            box-shadow: 0 4px 8px rgba(255, 152, 0, 0.3);
        }

        .no-family-button:hover {
            background: linear-gradient(135deg, #f57c00 0%, #ff9800 100%);
            transform: translateY(-2px);
            box-shadow: 0 6px 12px rgba(255, 152, 0, 0.4);
        }
    </style>
    <script>
        // Modal functions
        function openCreateModal() {
            try {
                var modal = document.getElementById('createTaskModal');
                if (modal) {
                    modal.style.display = 'block';
                    // Reset form after a small delay to ensure modal is visible
                    setTimeout(function() {
                        try {
                            resetCreateForm();
                        } catch (e) {
                            console.error('Error resetting form:', e);
                        }
                    }, 100);
                }
            } catch (e) {
                console.error('Error opening create modal:', e);
            }
        }
        
        function closeCreateModal() {
            document.getElementById('createTaskModal').style.display = 'none';
            resetCreateForm();
        }
        
        function openEditModal() {
            document.getElementById('editTaskModal').style.display = 'block';
        }
        
        function closeEditModal() {
            document.getElementById('editTaskModal').style.display = 'none';
        }
        
        function openViewModal() {
            document.getElementById('viewTaskModal').style.display = 'block';
        }
        
        function closeViewModal() {
            document.getElementById('viewTaskModal').style.display = 'none';
        }
        
        // Modal closing is handled inline in the modal div onclick attribute
        // This prevents conflicts with button clicks and other events
        
        // Objective management
        function addObjective(containerId) {
            var container = document.getElementById(containerId);
            var objectiveCount = container.children.length;
            var newObjective = document.createElement('div');
            newObjective.className = 'objective-item';
            newObjective.innerHTML = '<input type="text" name="objective_' + objectiveCount + '" class="form-control" placeholder="Enter objective" />' +
                '<button type="button" class="btn-remove-objective" onclick="removeObjective(this)">Remove</button>';
            container.appendChild(newObjective);
        }
        
        function removeObjective(btn) {
            btn.parentElement.remove();
        }
        
        function resetCreateForm() {
            // Reset all form fields manually (since createTaskForm is now a div, not a form)
            var form = document.getElementById('createTaskForm');
            if (form) {
                var inputs = form.querySelectorAll('input[type="text"], input[type="number"], textarea, select');
                inputs.forEach(function(input) {
                    if (input.type === 'text' || input.type === 'number') {
                        input.value = '';
                    } else if (input.tagName === 'TEXTAREA') {
                        input.value = '';
                    } else if (input.tagName === 'SELECT') {
                        input.selectedIndex = 0;
                    }
                });
            }
            var container = document.getElementById('createObjectivesContainer');
            if (container) {
                container.innerHTML = '';
                addObjective('createObjectivesContainer');
            }
        }
        
        // Confirm delete
        function confirmDelete(taskId, taskTitle) {
            return confirm('Are you sure you want to delete the task "' + taskTitle + '"?\n\nThis action cannot be undone.');
        }
        
        // Show message
        function showMessage(message, isSuccess) {
            var messageDiv = document.getElementById('pageMessage');
            if (messageDiv) {
                messageDiv.className = 'message ' + (isSuccess ? 'success' : 'error');
                messageDiv.textContent = message;
                messageDiv.style.display = 'block';
                setTimeout(function() {
                    messageDiv.style.display = 'none';
                }, 5000);
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
                        <a href="Family.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Family</a>
                        <a href="Tasks.aspx" class="active" style="color: #0066CC; text-decoration: none; font-weight: 500; font-size: 16px;">Tasks</a>
                        <a href="TaskReview.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Review</a>
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

        <!-- Main Content -->
        <div class="container">
            <div class="page-header">
                <h1 class="page-title">Tasks</h1>
                <asp:Button ID="btnCreateTask" runat="server" Text="+ Create Task" CssClass="btn-create" OnClientClick="openCreateModal(); return false;" UseSubmitBehavior="false" />
            </div>

            <!-- Messages -->
            <asp:Panel ID="pnlSuccess" runat="server" CssClass="message success" Visible="false">
                <asp:Label ID="lblSuccess" runat="server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlError" runat="server" CssClass="message error" Visible="false">
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </asp:Panel>
            <div id="pageMessage" class="message" style="display: none;"></div>

            <!-- No Family Message -->
            <asp:Panel ID="pnlNoFamily" runat="server" Visible="false">
                <div class="no-family-message">
                    <div class="no-family-icon" id="noFamilyIcon"></div>
                    <div class="no-family-title">Family Required</div>
                    <div class="no-family-text">
                        You need to be part of a family to access Tasks. Create a new family or join an existing one to start managing tasks for your children.
                    </div>
                    <a href="Family.aspx" class="no-family-button">Go to Family Page</a>
                </div>
            </asp:Panel>

            <!-- Search and Filter -->
            <asp:Panel ID="pnlSearchFilter" runat="server">
            <div class="search-filter-bar">
                <div class="search-box">
                    <input type="text" id="txtSearch" placeholder="Search tasks..." onkeyup="filterTasks(this.value)" />
                </div>
                <div class="filter-group">
                    <select id="ddlCategoryFilter" onchange="filterTasks()">
                        <option value="">All Categories</option>
                        <option value="Chores">Chores</option>
                        <option value="Homework">Homework</option>
                        <option value="Exercise">Exercise</option>
                        <option value="Other">Other</option>
                    </select>
                    <select id="ddlPriorityFilter" onchange="filterTasks()">
                        <option value="">All Priorities</option>
                        <option value="Low">Low</option>
                        <option value="Medium">Medium</option>
                        <option value="High">High</option>
                        <option value="Urgent">Urgent</option>
                    </select>
                    <select id="ddlSortBy" onchange="sortTasks()">
                        <option value="date">Sort by Date</option>
                        <option value="title">Sort by Title</option>
                        <option value="points">Sort by Points</option>
                        <option value="priority">Sort by Priority</option>
                    </select>
                </div>
            </div>
            </asp:Panel>

            <!-- Tasks List -->
            <asp:Panel ID="pnlTasks" runat="server">
                <div class="tasks-grid">
                    <asp:Repeater ID="rptTasks" runat="server" OnItemCommand="rptTasks_ItemCommand" OnItemDataBound="rptTasks_ItemDataBound">
                        <ItemTemplate>
                            <div class="task-card" data-task-id='<%# Eval("Id") %>' data-category='<%# Eval("Category") %>' data-priority='<%# Eval("Priority") %>'>
                                <div class="task-header">
                                    <div>
                                        <div class="task-title"><%# Eval("Title") %></div>
                                        <div class="task-meta">
                                            <span class="badge badge-category"><%# Eval("Category") %></span>
                                            <%# Eval("Priority") != DBNull.Value && !string.IsNullOrEmpty(Eval("Priority").ToString()) ? "<span class='badge badge-priority-" + Eval("Priority").ToString().ToLower() + "'>" + Eval("Priority") + "</span>" : "" %>
                                            <%# Eval("Difficulty") != DBNull.Value && !string.IsNullOrEmpty(Eval("Difficulty").ToString()) ? "<span class='badge badge-difficulty-" + Eval("Difficulty").ToString().ToLower() + "'>" + Eval("Difficulty") + "</span>" : "" %>
                                            <%# Eval("EstimatedMinutes") != DBNull.Value ? "<span><i class='time-icon'>&#9201;</i> " + Eval("EstimatedMinutes") + " min</span>" : "" %>
                                        </div>
                                    </div>
                                </div>
                                
                                <div class="task-points"><%# Eval("PointsReward") %> points</div>
                                
                                <%# Eval("Description") != DBNull.Value && !string.IsNullOrEmpty(Eval("Description").ToString()) ? "<div class='task-description'>" + Server.HtmlEncode(Eval("Description").ToString().Length > 100 ? Eval("Description").ToString().Substring(0, 100) + "..." : Eval("Description").ToString()) + "</div>" : "" %>
                                
                                <!-- Assignment Status -->
                                <asp:Panel ID="pnlAssignments" runat="server" Visible="false" CssClass="assignment-status">
                                    <div class="assignment-status-title">Assignments:</div>
                                    <asp:Repeater ID="rptAssignments" runat="server">
                                        <ItemTemplate>
                                            <div class="assignment-item">
                                                <span class="assignment-child-name"><%# Eval("ChildName") %></span>
                                                <span class="status-badge status-<%# Eval("Status").ToString().ToLower().Replace(" ", "-") %>"><%# Eval("Status") %></span>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </asp:Panel>
                                
                                <!-- Completion History (Fix #6) -->
                                <asp:Panel ID="pnlCompletionHistory" runat="server" Visible="false" CssClass="completion-history">
                                    <strong>Previously completed:</strong> <asp:Literal ID="litCompletionCount" runat="server"></asp:Literal> time(s)
                                </asp:Panel>
                                
                                <div class="task-actions">
                                    <asp:Button ID="btnAssign" runat="server" Text="Assign" CssClass="btn-action btn-assign" 
                                        CommandName="Assign" CommandArgument='<%# Eval("Id") %>' />
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn-action btn-edit" 
                                        CommandName="Edit" CommandArgument='<%# Eval("Id") %>' />
                                    <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn-action btn-view" 
                                        CommandName="View" CommandArgument='<%# Eval("Id") %>' />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn-action btn-delete" 
                                        CommandName="Delete" CommandArgument='<%# Eval("Id") %>' 
                                        data-task-id='<%# Eval("Id") %>' data-task-title='<%# Server.HtmlEncode(Eval("Title").ToString()) %>'
                                        OnClientClick='<%# "return confirmDeleteTaskFromButton(this);" %>' />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>
            
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="empty-state">
                    <h3>No tasks yet</h3>
                    <p>Create your first task to get started!</p>
                </div>
            </asp:Panel>

            <!-- Create Task Modal -->
            <div id="createTaskModal" class="modal" onclick="if(event.target==this) closeCreateModal();">
                <div class="modal-content" onclick="event.stopPropagation();">
                    <div class="modal-header">
                        <h2 class="modal-title">Create New Task</h2>
                        <span class="close" onclick="closeCreateModal()">&times;</span>
                    </div>
                    <div id="createTaskForm">
                        <div class="form-group">
                            <label>Task Title <span class="required">*</span></label>
                            <asp:TextBox ID="txtCreateTitle" runat="server" CssClass="form-control" placeholder="Enter task title" />
                            <asp:RequiredFieldValidator ID="rfvCreateTitle" runat="server" ControlToValidate="txtCreateTitle" 
                                ErrorMessage="Title is required" CssClass="error-message" Display="Dynamic" ValidationGroup="CreateTask" />
                        </div>
                        
                        <div class="form-group">
                            <label>Description</label>
                            <asp:TextBox ID="txtCreateDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" placeholder="Enter task description (optional)" />
                        </div>
                        
                        <div class="form-group">
                            <label>Category <span class="required">*</span></label>
                            <asp:DropDownList ID="ddlCreateCategory" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Select Category</asp:ListItem>
                                <asp:ListItem Value="Chores">Chores</asp:ListItem>
                                <asp:ListItem Value="Homework">Homework</asp:ListItem>
                                <asp:ListItem Value="Exercise">Exercise</asp:ListItem>
                                <asp:ListItem Value="Other">Other</asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvCreateCategory" runat="server" ControlToValidate="ddlCreateCategory" 
                                ErrorMessage="Category is required" CssClass="error-message" Display="Dynamic" ValidationGroup="CreateTask" />
                        </div>
                        
                        <div class="form-group">
                            <label>Points Reward <span class="required">*</span></label>
                            <asp:TextBox ID="txtCreatePoints" runat="server" CssClass="form-control" TextMode="Number" min="1" placeholder="Enter points" />
                            <asp:RequiredFieldValidator ID="rfvCreatePoints" runat="server" ControlToValidate="txtCreatePoints" 
                                ErrorMessage="Points are required" CssClass="error-message" Display="Dynamic" ValidationGroup="CreateTask" />
                            <asp:RangeValidator ID="rvCreatePoints" runat="server" ControlToValidate="txtCreatePoints" 
                                Type="Integer" MinimumValue="1" MaximumValue="10000" 
                                ErrorMessage="Points must be between 1 and 10000" CssClass="error-message" Display="Dynamic" ValidationGroup="CreateTask" />
                        </div>
                        
                        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px;">
                            <div class="form-group">
                                <label>Priority</label>
                                <asp:DropDownList ID="ddlCreatePriority" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="Low">Low</asp:ListItem>
                                    <asp:ListItem Value="Medium" Selected="True">Medium</asp:ListItem>
                                    <asp:ListItem Value="High">High</asp:ListItem>
                                    <asp:ListItem Value="Urgent">Urgent</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            
                            <div class="form-group">
                                <label>Difficulty</label>
                                <asp:DropDownList ID="ddlCreateDifficulty" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Select Difficulty</asp:ListItem>
                                    <asp:ListItem Value="Easy">Easy</asp:ListItem>
                                    <asp:ListItem Value="Medium">Medium</asp:ListItem>
                                    <asp:ListItem Value="Hard">Hard</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        
                        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px;">
                            <div class="form-group">
                                <label>Estimated Time (minutes)</label>
                                <asp:TextBox ID="txtCreateEstimatedMinutes" runat="server" CssClass="form-control" TextMode="Number" min="1" placeholder="Optional" />
                            </div>
                            
                            <div class="form-group">
                                <label>Recurrence</label>
                                <asp:DropDownList ID="ddlCreateRecurrence" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">None</asp:ListItem>
                                    <asp:ListItem Value="Daily">Daily</asp:ListItem>
                                    <asp:ListItem Value="Weekly">Weekly</asp:ListItem>
                                    <asp:ListItem Value="Monthly">Monthly</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        
                        <div class="form-group">
                            <label>Instructions</label>
                            <asp:TextBox ID="txtCreateInstructions" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Detailed instructions for completing the task (optional)" />
                        </div>
                        
                        <div class="form-group">
                            <label>Objectives <span class="required">*</span></label>
                            <small class="form-text" style="display: block; color: #666; margin-bottom: 10px; font-size: 12px;">At least one objective is required</small>
                            <div id="createObjectivesContainer" class="objectives-container">
                                <div class="objective-item">
                                    <input type="text" name="objective_0" class="form-control" placeholder="Enter objective" />
                                    <button type="button" class="btn-remove-objective" onclick="removeObjective(this)">Remove</button>
                                </div>
                            </div>
                            <button type="button" class="btn-add-objective" onclick="addObjective('createObjectivesContainer')">+ Add Objective</button>
                        </div>
                        
                        <asp:Button ID="btnCreateTaskSubmit" runat="server" Text="Create Task" CssClass="btn-submit" 
                            ValidationGroup="CreateTask" OnClick="btnCreateTaskSubmit_Click" />
                        <button type="button" class="btn-cancel" onclick="closeCreateModal()">Cancel</button>
                    </div>
                </div>
            </div>

            <!-- Edit Task Modal -->
            <div id="editTaskModal" class="modal" onclick="if(event.target==this) closeEditModal();">
                <div class="modal-content" onclick="event.stopPropagation();">
                    <div class="modal-header">
                        <h2 class="modal-title">Edit Task</h2>
                        <span class="close" onclick="closeEditModal()">&times;</span>
                    </div>
                    <div id="editTaskForm">
                        <asp:HiddenField ID="hidEditTaskId" runat="server" />
                        
                        <div class="form-group">
                            <label>Task Title <span class="required">*</span></label>
                            <asp:TextBox ID="txtEditTitle" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvEditTitle" runat="server" ControlToValidate="txtEditTitle" 
                                ErrorMessage="Title is required" CssClass="error-message" Display="Dynamic" ValidationGroup="EditTask" />
                        </div>
                        
                        <div class="form-group">
                            <label>Description</label>
                            <asp:TextBox ID="txtEditDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
                        </div>
                        
                        <div class="form-group">
                            <label>Category <span class="required">*</span></label>
                            <asp:DropDownList ID="ddlEditCategory" runat="server" CssClass="form-control">
                                <asp:ListItem Value="Chores">Chores</asp:ListItem>
                                <asp:ListItem Value="Homework">Homework</asp:ListItem>
                                <asp:ListItem Value="Exercise">Exercise</asp:ListItem>
                                <asp:ListItem Value="Other">Other</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        
                        <div class="form-group">
                            <label>Points Reward <span class="required">*</span></label>
                            <asp:TextBox ID="txtEditPoints" runat="server" CssClass="form-control" TextMode="Number" min="1" />
                            <asp:RequiredFieldValidator ID="rfvEditPoints" runat="server" ControlToValidate="txtEditPoints" 
                                ErrorMessage="Points are required" CssClass="error-message" Display="Dynamic" ValidationGroup="EditTask" />
                        </div>
                        
                        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 15px;">
                            <div class="form-group">
                                <label>Priority</label>
                                <asp:DropDownList ID="ddlEditPriority" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="Low">Low</asp:ListItem>
                                    <asp:ListItem Value="Medium">Medium</asp:ListItem>
                                    <asp:ListItem Value="High">High</asp:ListItem>
                                    <asp:ListItem Value="Urgent">Urgent</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            
                            <div class="form-group">
                                <label>Difficulty</label>
                                <asp:DropDownList ID="ddlEditDifficulty" runat="server" CssClass="form-control">
                                    <asp:ListItem Value="">Select Difficulty</asp:ListItem>
                                    <asp:ListItem Value="Easy">Easy</asp:ListItem>
                                    <asp:ListItem Value="Medium">Medium</asp:ListItem>
                                    <asp:ListItem Value="Hard">Hard</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        
                        <div class="form-group">
                            <label>Estimated Time (minutes)</label>
                            <asp:TextBox ID="txtEditEstimatedMinutes" runat="server" CssClass="form-control" TextMode="Number" min="1" />
                        </div>
                        
                        <div class="form-group">
                            <label>Instructions</label>
                            <asp:TextBox ID="txtEditInstructions" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                        </div>
                        
                        <div class="form-group">
                            <label>Objectives</label>
                            <div id="editObjectivesContainer" class="objectives-container">
                                <!-- Populated dynamically -->
                            </div>
                            <button type="button" class="btn-add-objective" onclick="addObjective('editObjectivesContainer')">+ Add Objective</button>
                        </div>
                        
                        <asp:Button ID="btnEditTaskSubmit" runat="server" Text="Save Changes" CssClass="btn-submit" 
                            ValidationGroup="EditTask" OnClick="btnEditTaskSubmit_Click" />
                        <button type="button" class="btn-cancel" onclick="closeEditModal()">Cancel</button>
                    </div>
                </div>
            </div>

            <!-- View Task Modal -->
            <div id="viewTaskModal" class="modal" onclick="if(event.target==this) closeViewModal();">
                <div class="modal-content" onclick="event.stopPropagation();">
                    <div class="modal-header">
                        <h2 class="modal-title" id="viewTaskTitle">Task Details</h2>
                        <span class="close" onclick="closeViewModal()">&times;</span>
                    </div>
                    <div id="viewTaskContent">
                        <!-- Populated dynamically -->
                    </div>
                </div>
            </div>
        </div>
    </form>
    
    <script>
        // Filter and sort functions
        function filterTasks() {
            var searchText = document.getElementById('txtSearch') ? document.getElementById('txtSearch').value.toLowerCase() : '';
            var categoryFilter = document.getElementById('ddlCategoryFilter') ? document.getElementById('ddlCategoryFilter').value : '';
            var priorityFilter = document.getElementById('ddlPriorityFilter') ? document.getElementById('ddlPriorityFilter').value : '';
            
            var cards = document.querySelectorAll('.task-card');
            cards.forEach(function(card) {
                var title = card.querySelector('.task-title').textContent.toLowerCase();
                var category = card.getAttribute('data-category') || '';
                var priority = card.getAttribute('data-priority') || '';
                
                var matchesSearch = !searchText || title.indexOf(searchText) !== -1;
                var matchesCategory = !categoryFilter || category === categoryFilter;
                var matchesPriority = !priorityFilter || priority === priorityFilter;
                
                if (matchesSearch && matchesCategory && matchesPriority) {
                    card.style.display = 'block';
                } else {
                    card.style.display = 'none';
                }
            });
        }
        
        function sortTasks() {
            var sortBy = document.getElementById('ddlSortBy') ? document.getElementById('ddlSortBy').value : 'date';
            var container = document.querySelector('.tasks-grid');
            var cards = Array.from(container.querySelectorAll('.task-card'));
            
            cards.sort(function(a, b) {
                switch(sortBy) {
                    case 'title':
                        return a.querySelector('.task-title').textContent.localeCompare(b.querySelector('.task-title').textContent);
                    case 'points':
                        var aPoints = parseInt(a.querySelector('.task-points').textContent) || 0;
                        var bPoints = parseInt(b.querySelector('.task-points').textContent) || 0;
                        return bPoints - aPoints;
                    case 'priority':
                        var priorityOrder = { 'Urgent': 4, 'High': 3, 'Medium': 2, 'Low': 1 };
                        var aPriority = priorityOrder[a.getAttribute('data-priority')] || 0;
                        var bPriority = priorityOrder[b.getAttribute('data-priority')] || 0;
                        return bPriority - aPriority;
                    default: // date
                        return 0; // Keep original order
                }
            });
            
            cards.forEach(function(card) {
                container.appendChild(card);
            });
        }

        // Set no family icon using Unicode to avoid encoding issues
        document.addEventListener('DOMContentLoaded', function() {
            var noFamilyIcon = document.getElementById('noFamilyIcon');
            if (noFamilyIcon) {
                // Family icon: U+1F46A (👪) - using Unicode escape
                noFamilyIcon.textContent = String.fromCharCode(0xD83D, 0xDC6A);
            }
        });

        var pendingDeleteTaskId = null;
        var pendingDeleteButton = null;

        function confirmDeleteTaskFromButton(buttonElement) {
            // Get task ID and title from data attributes
            var taskId = buttonElement.getAttribute('data-task-id');
            var taskTitle = buttonElement.getAttribute('data-task-title');
            
            if (!taskId || !taskTitle) {
                alert('Error: Could not find task information. Please refresh the page.');
                return false;
            }
            
            pendingDeleteTaskId = taskId;
            pendingDeleteButton = buttonElement;

            // Update modal content
            document.getElementById('deleteConfirmTaskTitle').textContent = taskTitle;
            document.getElementById('deleteTaskModal').classList.add('show');
            
            return false; // Prevent default submission
        }

        function closeDeleteTaskModal() {
            document.getElementById('deleteTaskModal').classList.remove('show');
            pendingDeleteTaskId = null;
            pendingDeleteButton = null;
        }

        // Handle confirm delete button click
        document.addEventListener('DOMContentLoaded', function() {
            var btnConfirmDelete = document.getElementById('btnConfirmDeleteTask');
            if (btnConfirmDelete) {
                btnConfirmDelete.addEventListener('click', function() {
                    if (pendingDeleteButton) {
                        closeDeleteTaskModal();
                        // Trigger the actual form submission
                        setTimeout(function() {
                            if (pendingDeleteButton) {
                                pendingDeleteButton.click();
                            }
                        }, 100);
                    }
                });
            }

            // Close modal when clicking outside
            var deleteModal = document.getElementById('deleteTaskModal');
            if (deleteModal) {
                deleteModal.addEventListener('click', function(e) {
                    if (e.target === this) {
                        closeDeleteTaskModal();
                    }
                });
            }
        });
    </script>

    <!-- Confirmation Modal for Task Deletion -->
    <div id="deleteTaskModal" class="confirmation-modal">
        <div class="confirmation-content">
            <div class="confirmation-icon danger">⚠</div>
            <div class="confirmation-title">Delete Task</div>
            <div class="confirmation-message">
                Are you sure you want to delete the task "<span id="deleteConfirmTaskTitle"></span>"? This action cannot be undone.
            </div>
            <div class="confirmation-actions">
                <button type="button" class="btn-confirm btn-confirm-secondary" onclick="closeDeleteTaskModal()">Cancel</button>
                <button type="button" class="btn-confirm btn-confirm-danger" id="btnConfirmDeleteTask">Yes, Delete Task</button>
            </div>
        </div>
    </div>
</body>
</html>

