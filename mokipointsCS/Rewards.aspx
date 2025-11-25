<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Rewards.aspx.cs" Inherits="mokipointsCS.Rewards" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Rewards - MOKI POINTS</title>
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
            transition: all 0.3s;
        }
        
        .btn-create:hover {
            background-color: #0052a3;
        }
        
        /* Message Container */
        .message-container {
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 10000;
            max-width: 400px;
        }
        
        .message {
            padding: 15px 20px;
            margin-bottom: 10px;
            border-radius: 5px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            display: flex;
            align-items: center;
            animation: slideIn 0.3s ease-out;
        }
        
        .message-success {
            background-color: #d4edda;
            border-left: 4px solid #28a745;
            color: #155724;
        }
        
        .message-error {
            background-color: #f8d7da;
            border-left: 4px solid #dc3545;
            color: #721c24;
        }
        
        .message-icon {
            font-size: 20px;
            margin-right: 10px;
            font-weight: bold;
        }
        
        .message-text {
            flex: 1;
            font-size: 14px;
        }
        
        .message-close {
            background: none;
            border: none;
            font-size: 20px;
            cursor: pointer;
            margin-left: 10px;
            opacity: 0.7;
        }
        
        .message-close:hover {
            opacity: 1;
        }
        
        @keyframes slideIn {
            from {
                transform: translateX(100%);
                opacity: 0;
            }
            to {
                transform: translateX(0);
                opacity: 1;
            }
        }
        
        @keyframes slideOut {
            from {
                transform: translateX(0);
                opacity: 1;
            }
            to {
                transform: translateX(100%);
                opacity: 0;
            }
        }
        
        /* Rewards Grid */
        .rewards-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
            gap: 20px;
        }
        
        .reward-card {
            background-color: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            transition: box-shadow 0.3s;
            position: relative;
        }
        
        .reward-card:hover {
            box-shadow: 0 4px 8px rgba(0,0,0,0.15);
        }
        
        .reward-header {
            display: flex;
            justify-content: space-between;
            align-items: start;
            margin-bottom: 15px;
        }
        
        .reward-title {
            font-size: 20px;
            font-weight: bold;
            color: #333;
            margin-bottom: 5px;
        }
        
        .reward-meta {
            display: flex;
            gap: 10px;
            flex-wrap: wrap;
            margin-bottom: 15px;
            font-size: 14px;
            color: #666;
        }
        
        .badge {
            padding: 4px 12px;
            border-radius: 15px;
            font-size: 12px;
            font-weight: 500;
        }
        
        .badge-category {
            background-color: #F5F5F5;
            color: #666;
        }
        
        .badge-in-use {
            background-color: #fff3cd;
            color: #856404;
        }
        
        .reward-points {
            font-size: 24px;
            font-weight: bold;
            color: #FF6600;
            margin: 10px 0;
        }
        
        .reward-description {
            color: #666;
            font-size: 14px;
            margin-bottom: 15px;
            line-height: 1.5;
        }
        
        .reward-image {
            width: 100%;
            height: 200px;
            object-fit: cover;
            border-radius: 5px;
            margin-bottom: 15px;
        }
        
        .reward-actions {
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
        
        .btn-edit {
            background-color: #0066CC;
            color: white;
        }
        
        .btn-edit:hover {
            background-color: #0052a3;
        }
        
        .btn-edit:disabled {
            background-color: #ccc;
            cursor: not-allowed;
            opacity: 0.6;
        }
        
        .btn-delete {
            background-color: #dc3545;
            color: white;
        }
        
        .btn-delete:hover {
            background-color: #c82333;
        }
        
        .btn-delete:disabled {
            background-color: #ccc;
            cursor: not-allowed;
            opacity: 0.6;
        }
        
        .btn-view {
            background-color: #6c757d;
            color: white;
        }
        
        .btn-view:hover {
            background-color: #5a6268;
        }
        
        /* Modal */
        .modal {
            display: none;
            position: fixed;
            z-index: 1000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5);
            animation: fadeIn 0.3s;
        }
        
        @keyframes fadeIn {
            from { opacity: 0; }
            to { opacity: 1; }
        }
        
        .modal-content {
            background-color: white;
            margin: auto;
            padding: 30px;
            border-radius: 12px;
            width: 90%;
            max-width: 500px;
            box-shadow: 0 4px 20px rgba(0, 0, 0, 0.3);
            animation: slideDown 0.3s ease-out;
            position: relative;
            top: 50%;
            transform: translateY(-50%);
        }
        
        @keyframes slideDown {
            from {
                transform: translateY(-60%);
                opacity: 0;
            }
            to {
                transform: translateY(-50%);
                opacity: 1;
            }
        }
        
        .modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 20px;
        }
        
        .modal-title {
            font-size: 24px;
            font-weight: 600;
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
        
        .form-group {
            margin-bottom: 20px;
        }
        
        .form-label {
            display: block;
            margin-bottom: 8px;
            font-weight: 500;
            color: #333;
        }
        
        .form-control {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
        }
        
        .form-control:focus {
            outline: none;
            border-color: #0066CC;
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
        
        /* Confirmation Modal */
        .confirmation-modal {
            display: none;
            position: fixed;
            z-index: 2000;
            left: 0;
            top: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.5);
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
            position: relative;
            top: 50%;
            transform: translateY(-50%);
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
        }
        
        .btn-confirm {
            flex: 1;
            padding: 12px 24px;
            background-color: #dc3545;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 500;
        }
        
        .btn-confirm:hover {
            background-color: #c82333;
        }
        
        .btn-cancel-confirm {
            flex: 1;
            padding: 12px 24px;
            background-color: #6c757d;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 500;
        }
        
        .btn-cancel-confirm:hover {
            background-color: #5a6268;
        }
        
        .search-filter-bar {
            display: flex;
            gap: 15px;
            margin-bottom: 20px;
            flex-wrap: wrap;
        }
        
        .search-box {
            flex: 1;
            min-width: 200px;
        }
        
        .search-box input {
            width: 100%;
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
        }
        
        .filter-group {
            display: flex;
            gap: 10px;
        }
        
        .filter-group select {
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
        }
        
        .badge-availability {
            display: inline-block;
            padding: 4px 10px;
            border-radius: 12px;
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
            margin-left: 8px;
        }
        
        .badge-available {
            background-color: #d4edda;
            color: #155724;
        }
        
        .badge-outofstock {
            background-color: #fff3cd;
            color: #856404;
        }
        
        .badge-hidden {
            background-color: #e2e3e5;
            color: #383d41;
        }
        
        .availability-error {
            color: #dc3545;
            font-size: 12px;
            margin-top: 5px;
            display: block;
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
        // Message functions
        function showMessage(type, message) {
            const messageId = type + 'Message';
            const messageElement = document.getElementById(messageId);
            if (messageElement) {
                const textElement = messageElement.querySelector('.message-text');
                if (textElement) {
                    textElement.textContent = message;
                    messageElement.style.display = 'flex';
                    
                    const hideDelay = type === 'error' ? 7000 : 5000;
                    setTimeout(() => {
                        closeMessage(messageId);
                    }, hideDelay);
                }
            }
        }
        
        function closeMessage(messageId) {
            const messageElement = document.getElementById(messageId);
            if (messageElement) {
                messageElement.style.animation = 'slideOut 0.3s ease-out';
                setTimeout(() => {
                    messageElement.style.display = 'none';
                    messageElement.style.animation = 'slideIn 0.3s ease-out';
                }, 300);
            }
        }
        
        // Modal functions
        function openCreateModal() {
            var modal = document.getElementById('createRewardModal');
            if (modal) {
                modal.style.display = 'block';
                setTimeout(function() {
                    resetCreateForm();
                }, 100);
            }
        }
        
        function closeCreateModal() {
            document.getElementById('createRewardModal').style.display = 'none';
            resetCreateForm();
        }
        
        function openEditModal() {
            document.getElementById('editRewardModal').style.display = 'block';
        }
        
        function closeEditModal() {
            document.getElementById('editRewardModal').style.display = 'none';
        }
        
        function openViewModal() {
            document.getElementById('viewRewardModal').style.display = 'block';
        }
        
        function closeViewModal() {
            document.getElementById('viewRewardModal').style.display = 'none';
        }
        
        function resetCreateForm() {
            var form = document.getElementById('createRewardForm');
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
        }
        
        // Set no family icon using Unicode to avoid encoding issues
        document.addEventListener('DOMContentLoaded', function() {
            var noFamilyIcon = document.getElementById('noFamilyIcon');
            if (noFamilyIcon) {
                // Family icon: U+1F46A (👪) - using Unicode escape
                noFamilyIcon.textContent = String.fromCharCode(0xD83D, 0xDC6A);
            }
        });

        // Delete confirmation
        function confirmDeleteRewardFromButton(buttonElement) {
            var rewardId = buttonElement.getAttribute('data-reward-id');
            var rewardName = buttonElement.getAttribute('data-reward-name');
            
            document.getElementById('deleteRewardId').value = rewardId;
            document.getElementById('deleteRewardName').textContent = rewardName;
            document.getElementById('deleteRewardModal').style.display = 'block';
            
            return false;
        }
        
        function closeDeleteRewardModal() {
            document.getElementById('deleteRewardModal').style.display = 'none';
        }
        
        // Filter and search
        function filterRewards() {
            var searchText = document.getElementById('txtSearch').value.toLowerCase();
            var categoryFilter = document.getElementById('ddlCategoryFilter').value.toLowerCase();
            var cards = document.querySelectorAll('.reward-card');
            
            cards.forEach(function(card) {
                var title = card.querySelector('.reward-title').textContent.toLowerCase();
                var category = card.getAttribute('data-category') || '';
                var matchesSearch = title.includes(searchText);
                var matchesCategory = !categoryFilter || category.toLowerCase() === categoryFilter;
                
                if (matchesSearch && matchesCategory) {
                    card.style.display = 'block';
                } else {
                    card.style.display = 'none';
                }
            });
        }
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        <!-- Message Container -->
        <div class="message-container">
            <div class="message message-success" id="successMessage" style="display: none;">
                <span class="message-icon">&#10003;</span>
                <span class="message-text"></span>
                <button class="message-close" onclick="closeMessage('successMessage')">&#215;</button>
            </div>
            <div class="message message-error" id="errorMessage" style="display: none;">
                <span class="message-icon">&#10007;</span>
                <span class="message-text"></span>
                <button class="message-close" onclick="closeMessage('errorMessage')">&#215;</button>
            </div>
        </div>
        
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
                        <a href="Tasks.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Tasks</a>
                        <a href="TaskReview.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Review</a>
                        <a href="Rewards.aspx" class="active" style="color: #0066CC; text-decoration: none; font-weight: 500; font-size: 16px;">Rewards</a>
                        <a href="RewardOrders.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Orders</a>
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
                <h1 class="page-title">Rewards</h1>
                <asp:Button ID="btnCreateReward" runat="server" Text="+ Create Reward" CssClass="btn-create" OnClientClick="openCreateModal(); return false;" UseSubmitBehavior="false" />
            </div>

            <!-- No Family Message -->
            <asp:Panel ID="pnlNoFamily" runat="server" Visible="false">
                <div class="no-family-message">
                    <div class="no-family-icon" id="noFamilyIcon"></div>
                    <div class="no-family-title">Family Required</div>
                    <div class="no-family-text">
                        You need to be part of a family to access Rewards. Create a new family or join an existing one to start creating rewards for your children.
                    </div>
                    <a href="Family.aspx" class="no-family-button">Go to Family Page</a>
                </div>
            </asp:Panel>

            <!-- Search and Filter -->
            <asp:Panel ID="pnlSearchFilter" runat="server">
            <div class="search-filter-bar">
                <div class="search-box">
                    <input type="text" id="txtSearch" placeholder="Search rewards..." onkeyup="filterRewards()" />
                </div>
                <div class="filter-group">
                    <select id="ddlCategoryFilter" onchange="filterRewards()">
                        <option value="">All Categories</option>
                        <option value="Toys">Toys</option>
                        <option value="Privileges">Privileges</option>
                        <option value="Activities">Activities</option>
                        <option value="Other">Other</option>
                    </select>
                </div>
            </div>
            </asp:Panel>

            <!-- Rewards List -->
            <asp:Panel ID="pnlRewards" runat="server">
                <div class="rewards-grid">
                    <asp:Repeater ID="rptRewards" runat="server" OnItemCommand="rptRewards_ItemCommand" OnItemDataBound="rptRewards_ItemDataBound">
                        <ItemTemplate>
                            <div class="reward-card" data-category='<%# Eval("Category") ?? "" %>'>
                                <div class="reward-header">
                                    <div>
                                        <div class="reward-title"><%# Eval("Name") %></div>
                                        <div class="reward-meta">
                                            <%# Eval("Category") != DBNull.Value && !string.IsNullOrEmpty(Eval("Category").ToString()) ? "<span class='badge badge-category'>" + Eval("Category") + "</span>" : "" %>
                                            <asp:Literal ID="litInUseBadge" runat="server"></asp:Literal>
                                            <asp:Literal ID="litAvailabilityBadge" runat="server"></asp:Literal>
                                        </div>
                                    </div>
                                </div>
                                
                                <%# Eval("ImageUrl") != DBNull.Value && !string.IsNullOrEmpty(Eval("ImageUrl").ToString()) ? "<img src='" + Eval("ImageUrl") + "' alt='" + Server.HtmlEncode(Eval("Name").ToString()) + "' class='reward-image' />" : "" %>
                                
                                <div class="reward-points"><%# Eval("PointCost") %> points</div>
                                
                                <%# Eval("Description") != DBNull.Value && !string.IsNullOrEmpty(Eval("Description").ToString()) ? "<div class='reward-description'>" + Server.HtmlEncode(Eval("Description").ToString().Length > 100 ? Eval("Description").ToString().Substring(0, 100) + "..." : Eval("Description").ToString()) + "</div>" : "" %>
                                
                                <!-- Availability Status -->
                                <div class="reward-availability" style="margin: 15px 0; padding: 10px; background: #f8f9fa; border-radius: 5px;">
                                    <label style="display: block; margin-bottom: 5px; font-size: 12px; color: #666; font-weight: 500;">Availability Status:</label>
                                    <div style="display: flex; gap: 10px; align-items: center;">
                                        <asp:DropDownList ID="ddlAvailabilityStatus" runat="server" CssClass="form-control" 
                                            style="flex: 1; padding: 8px; border: 1px solid #ddd; border-radius: 5px; font-size: 14px;"
                                            data-reward-id='<%# Eval("Id") %>'>
                                            <asp:ListItem Value="Available" Text="Available"></asp:ListItem>
                                            <asp:ListItem Value="OutOfStock" Text="Out of Stock"></asp:ListItem>
                                            <asp:ListItem Value="Hidden" Text="Hidden"></asp:ListItem>
                                        </asp:DropDownList>
                                        <asp:Button ID="btnUpdateAvailability" runat="server" Text="Update" CssClass="btn-action" 
                                            style="padding: 8px 16px; background: #0066CC; color: white; border: none; border-radius: 5px; cursor: pointer; font-size: 14px;"
                                            CommandName="UpdateAvailability" CommandArgument='<%# Eval("Id") %>' />
                                    </div>
                                    <asp:Literal ID="litAvailabilityError" runat="server"></asp:Literal>
                                </div>
                                
                                <div class="reward-actions">
                                    <asp:Button ID="btnEdit" runat="server" Text="Edit" CssClass="btn-action btn-edit" 
                                        CommandName="Edit" CommandArgument='<%# Eval("Id") %>' />
                                    <asp:Button ID="btnView" runat="server" Text="View" CssClass="btn-action btn-view" 
                                        CommandName="View" CommandArgument='<%# Eval("Id") %>' />
                                    <asp:Button ID="btnDelete" runat="server" Text="Delete" CssClass="btn-action btn-delete" 
                                        CommandName="Delete" CommandArgument='<%# Eval("Id") %>' 
                                        data-reward-id='<%# Eval("Id") %>' data-reward-name='<%# Server.HtmlEncode(Eval("Name").ToString()) %>'
                                        OnClientClick='<%# "return confirmDeleteRewardFromButton(this);" %>' />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>
            
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="empty-state">
                    <h3>No rewards yet</h3>
                    <p>Create your first reward to get started!</p>
                </div>
            </asp:Panel>

            <!-- Create Reward Modal -->
            <div id="createRewardModal" class="modal" onclick="if(event.target==this) closeCreateModal();">
                <div class="modal-content" onclick="event.stopPropagation();">
                    <div class="modal-header">
                        <h2 class="modal-title">Create New Reward</h2>
                        <span class="close" onclick="closeCreateModal()">&#215;</span>
                    </div>
                    <div id="createRewardForm">
                        <div class="form-group">
                            <label class="form-label">Reward Name *</label>
                            <asp:TextBox ID="txtCreateName" runat="server" CssClass="form-control" placeholder="Enter reward name"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Description</label>
                            <asp:TextBox ID="txtCreateDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" placeholder="Enter reward description"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Point Cost *</label>
                            <asp:TextBox ID="txtCreatePointCost" runat="server" CssClass="form-control" TextMode="Number" min="1" placeholder="Enter point cost"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Category</label>
                            <asp:DropDownList ID="ddlCreateCategory" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Select Category</asp:ListItem>
                                <asp:ListItem Value="Toys">Toys</asp:ListItem>
                                <asp:ListItem Value="Privileges">Privileges</asp:ListItem>
                                <asp:ListItem Value="Activities">Activities</asp:ListItem>
                                <asp:ListItem Value="Other">Other</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Image URL</label>
                            <asp:TextBox ID="txtCreateImageUrl" runat="server" CssClass="form-control" placeholder="Enter image URL (optional)"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnCreateRewardSubmit" runat="server" Text="Create Reward" CssClass="btn-submit" OnClick="btnCreateRewardSubmit_Click" />
                        <button type="button" class="btn-cancel" onclick="closeCreateModal()">Cancel</button>
                    </div>
                </div>
            </div>

            <!-- Edit Reward Modal -->
            <div id="editRewardModal" class="modal" onclick="if(event.target==this) closeEditModal();">
                <div class="modal-content" onclick="event.stopPropagation();">
                    <div class="modal-header">
                        <h2 class="modal-title">Edit Reward</h2>
                        <span class="close" onclick="closeEditModal()">&#215;</span>
                    </div>
                    <div>
                        <asp:HiddenField ID="hidEditRewardId" runat="server" />
                        <div class="form-group">
                            <label class="form-label">Reward Name *</label>
                            <asp:TextBox ID="txtEditName" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Description</label>
                            <asp:TextBox ID="txtEditDescription" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Point Cost *</label>
                            <asp:TextBox ID="txtEditPointCost" runat="server" CssClass="form-control" TextMode="Number" min="1"></asp:TextBox>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Category</label>
                            <asp:DropDownList ID="ddlEditCategory" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">Select Category</asp:ListItem>
                                <asp:ListItem Value="Toys">Toys</asp:ListItem>
                                <asp:ListItem Value="Privileges">Privileges</asp:ListItem>
                                <asp:ListItem Value="Activities">Activities</asp:ListItem>
                                <asp:ListItem Value="Other">Other</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="form-group">
                            <label class="form-label">Image URL</label>
                            <asp:TextBox ID="txtEditImageUrl" runat="server" CssClass="form-control"></asp:TextBox>
                        </div>
                        <asp:Button ID="btnEditRewardSubmit" runat="server" Text="Save Changes" CssClass="btn-submit" OnClick="btnEditRewardSubmit_Click" />
                        <button type="button" class="btn-cancel" onclick="closeEditModal()">Cancel</button>
                    </div>
                </div>
            </div>

            <!-- View Reward Modal -->
            <div id="viewRewardModal" class="modal" onclick="if(event.target==this) closeViewModal();">
                <div class="modal-content" onclick="event.stopPropagation();">
                    <div class="modal-header">
                        <h2 class="modal-title">Reward Details</h2>
                        <span class="close" onclick="closeViewModal()">&#215;</span>
                    </div>
                    <div>
                        <asp:Literal ID="litViewReward" runat="server"></asp:Literal>
                    </div>
                    <button type="button" class="btn-cancel" onclick="closeViewModal()">Close</button>
                </div>
            </div>

            <!-- Delete Confirmation Modal -->
            <div id="deleteRewardModal" class="confirmation-modal" onclick="if(event.target==this) closeDeleteRewardModal();">
                <div class="confirmation-content" onclick="event.stopPropagation();">
                    <div class="confirmation-icon danger">&#9888;</div>
                    <div class="confirmation-title">Delete Reward</div>
                    <div class="confirmation-message">
                        Are you sure you want to delete the reward "<span id="deleteRewardName"></span>"?<br />
                        This will hide the reward from the shop.
                    </div>
                    <div class="confirmation-actions">
                        <asp:Button ID="btnConfirmDeleteReward" runat="server" Text="Delete" CssClass="btn-confirm" OnClick="btnConfirmDeleteReward_Click" />
                        <button type="button" class="btn-cancel-confirm" onclick="closeDeleteRewardModal()">Cancel</button>
                    </div>
                    <asp:HiddenField ID="deleteRewardId" runat="server" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>

