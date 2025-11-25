<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RewardOrders.aspx.cs" Inherits="mokipointsCS.RewardOrders" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reward Orders - MOKI POINTS</title>
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
        
        /* Filter Bar */
        .filter-bar {
            display: flex;
            gap: 15px;
            margin-bottom: 20px;
            flex-wrap: wrap;
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
        
        /* Orders List */
        .orders-list {
            display: flex;
            flex-direction: column;
            gap: 20px;
        }
        
        .order-card {
            background: white;
            border-radius: 12px;
            padding: 25px;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
            transition: transform 0.2s, box-shadow 0.2s;
        }
        
        .order-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 6px 16px rgba(0, 0, 0, 0.2);
        }
        
        .order-header {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            margin-bottom: 15px;
            padding-bottom: 15px;
            border-bottom: 2px solid #f0f0f0;
        }
        
        .order-info {
            flex: 1;
        }
        
        .order-number {
            font-size: 20px;
            font-weight: 600;
            color: #333;
            margin-bottom: 5px;
        }
        
        .order-meta {
            display: flex;
            gap: 20px;
            margin-top: 10px;
            flex-wrap: wrap;
            color: #666;
            font-size: 14px;
        }
        
        .status-badge {
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 500;
            text-transform: uppercase;
        }
        
        .status-pending {
            background-color: #fff3cd;
            color: #856404;
        }
        
        .status-waitingtofulfill {
            background-color: #ffeaa7;
            color: #d63031;
        }
        
        .status-fulfilled {
            background-color: #d4edda;
            color: #155724;
        }
        
        .status-declined {
            background-color: #f8d7da;
            color: #721c24;
        }
        
        .status-notfulfilled {
            background-color: #f8d7da;
            color: #721c24;
        }
        
        .order-items {
            margin-bottom: 20px;
        }
        
        .order-items h4 {
            color: #0066CC;
            margin-bottom: 10px;
            font-size: 16px;
        }
        
        .order-item {
            padding: 10px;
            margin-bottom: 8px;
            background: #f8f9fa;
            border-radius: 6px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        
        .order-item-name {
            font-weight: 500;
            color: #333;
        }
        
        .order-item-details {
            color: #666;
            font-size: 14px;
        }
        
        .order-total {
            background: #fff3cd;
            padding: 12px;
            border-radius: 6px;
            margin-bottom: 15px;
            font-weight: 600;
            color: #856404;
            font-size: 18px;
        }
        
        .order-actions {
            display: flex;
            gap: 10px;
            margin-top: 20px;
            flex-wrap: wrap;
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
        
        .confirmation-icon {
            font-size: 48px;
            text-align: center;
            margin-bottom: 15px;
        }
        
        .confirmation-icon.success {
            color: #28a745;
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
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 16px;
            font-weight: 500;
        }
        
        .btn-confirm:hover {
            background-color: #0052a3;
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
        
        // Confirmation modals
        function confirmOrder(orderId, orderNumber) {
            document.getElementById('confirmOrderId').value = orderId;
            document.getElementById('confirmOrderNumber').textContent = orderNumber;
            document.getElementById('confirmOrderModal').style.display = 'block';
            return false;
        }
        
        function closeConfirmOrderModal() {
            document.getElementById('confirmOrderModal').style.display = 'none';
        }
        
        function declineOrder(orderId, orderNumber) {
            document.getElementById('declineOrderId').value = orderId;
            document.getElementById('declineOrderNumber').textContent = orderNumber;
            document.getElementById('declineOrderModal').style.display = 'block';
            return false;
        }
        
        function closeDeclineOrderModal() {
            document.getElementById('declineOrderModal').style.display = 'none';
        }
        
        function fulfillOrder(orderId, orderNumber) {
            document.getElementById('fulfillOrderId').value = orderId;
            document.getElementById('fulfillOrderNumber').textContent = orderNumber;
            document.getElementById('fulfillOrderModal').style.display = 'block';
            return false;
        }
        
        function closeFulfillOrderModal() {
            document.getElementById('fulfillOrderModal').style.display = 'none';
        }
        
        // Filter orders
        function filterOrders() {
            var statusFilter = document.getElementById('ddlStatusFilter').value.toLowerCase();
            var cards = document.querySelectorAll('.order-card');
            
            cards.forEach(function(card) {
                var status = card.getAttribute('data-status') || '';
                if (!statusFilter || status.toLowerCase() === statusFilter) {
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
                        <a href="Rewards.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Rewards</a>
                        <a href="RewardOrders.aspx" class="active" style="color: #0066CC; text-decoration: none; font-weight: 500; font-size: 16px;">Orders</a>
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
                <h1 class="page-title">Reward Orders</h1>
            </div>

            <!-- Filter Bar -->
            <div class="filter-bar">
                <div class="filter-group">
                    <select id="ddlStatusFilter" onchange="filterOrders()">
                        <option value="">All Statuses</option>
                        <option value="Pending">Pending</option>
                        <option value="WaitingToFulfill">Waiting to Fulfill</option>
                        <option value="Fulfilled">Fulfilled</option>
                        <option value="Declined">Declined</option>
                        <option value="NotFulfilled">Not Fulfilled</option>
                    </select>
                </div>
            </div>

            <!-- Orders List -->
            <asp:Panel ID="pnlOrders" runat="server">
                <div class="orders-list">
                    <asp:Repeater ID="rptOrders" runat="server" OnItemCommand="rptOrders_ItemCommand" OnItemDataBound="rptOrders_ItemDataBound">
                        <ItemTemplate>
                            <div class="order-card" data-status='<%# Eval("Status") %>'>
                                <div class="order-header">
                                    <div class="order-info">
                                        <div class="order-number">Order: <%# Eval("OrderNumber") %></div>
                                        <div class="order-meta">
                                            <span><strong>Child:</strong> <%# Eval("ChildName") %></span>
                                            <span><strong>Date:</strong> <%# Convert.ToDateTime(Eval("OrderDate")).ToString("MMM dd, yyyy HH:mm") %></span>
                                        </div>
                                    </div>
                                    <span class="status-badge status-<%# Eval("Status").ToString().ToLower().Replace(" ", "") %>"><%# Eval("Status") %></span>
                                </div>
                                
                                <div class="order-items">
                                    <h4>Order Items</h4>
                                    <asp:Repeater ID="rptOrderItems" runat="server">
                                        <ItemTemplate>
                                            <div class="order-item">
                                                <div>
                                                    <div class="order-item-name"><%# Eval("RewardName") %></div>
                                                    <div class="order-item-details">Quantity: <%# Eval("Quantity") %> × <%# Eval("PointCost") %> points = <%# Eval("Subtotal") %> points</div>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                                
                                <div class="order-total">
                                    Total: <%# Eval("TotalPoints") %> points
                                </div>
                                
                                <!-- Refund Code (for parents only) -->
                                <asp:Panel ID="pnlRefundCode" runat="server" Visible="false" CssClass="refund-code-section" style="background: #e7f3ff; padding: 12px; border-radius: 6px; margin-bottom: 15px;">
                                    <div style="font-weight: 600; color: #0066cc; margin-bottom: 5px;">Refund Code:</div>
                                    <span style="font-family: monospace; font-size: 16px; color: #333;"><asp:Literal ID="litRefundCode" runat="server"></asp:Literal></span>
                                    <div style="font-size: 12px; color: #666; margin-top: 5px;">Keep this code safe. Child will need it if they claim the order was not fulfilled.</div>
                                </asp:Panel>
                                
                                <div class="order-actions">
                                    <asp:Button ID="btnConfirm" runat="server" Text="Confirm Order" CssClass="btn btn-primary" 
                                        CommandName="Confirm" CommandArgument='<%# Eval("Id") %>' 
                                        data-order-id='<%# Eval("Id") %>' data-order-number='<%# Eval("OrderNumber") %>'
                                        OnClientClick='<%# "return confirmOrder(" + Eval("Id") + ", \"" + Eval("OrderNumber") + "\");" %>' 
                                        Visible="false" />
                                    <asp:Button ID="btnDecline" runat="server" Text="Decline Order" CssClass="btn btn-danger" 
                                        CommandName="Decline" CommandArgument='<%# Eval("Id") %>' 
                                        data-order-id='<%# Eval("Id") %>' data-order-number='<%# Eval("OrderNumber") %>'
                                        OnClientClick='<%# "return declineOrder(" + Eval("Id") + ", \"" + Eval("OrderNumber") + "\");" %>' 
                                        Visible="false" />
                                    <asp:Button ID="btnFulfill" runat="server" Text="Mark as Fulfilled" CssClass="btn btn-success" 
                                        CommandName="Fulfill" CommandArgument='<%# Eval("Id") %>' 
                                        data-order-id='<%# Eval("Id") %>' data-order-number='<%# Eval("OrderNumber") %>'
                                        OnClientClick='<%# "return fulfillOrder(" + Eval("Id") + ", \"" + Eval("OrderNumber") + "\");" %>' 
                                        Visible="false" />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>
            
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="empty-state">
                    <h3>No orders yet</h3>
                    <p>Orders from children will appear here.</p>
                </div>
            </asp:Panel>

            <!-- Confirm Order Modal -->
            <div id="confirmOrderModal" class="confirmation-modal" onclick="if(event.target==this) closeConfirmOrderModal();">
                <div class="confirmation-content" onclick="event.stopPropagation();">
                    <div class="confirmation-icon success">&#10003;</div>
                    <div class="confirmation-title">Confirm Order</div>
                    <div class="confirmation-message">
                        Are you sure you want to confirm order "<span id="confirmOrderNumber"></span>"?<br />
                        This will deduct points from the child and generate a refund code.
                    </div>
                    <div class="confirmation-actions">
                        <asp:Button ID="btnConfirmOrder" runat="server" Text="Confirm" CssClass="btn-confirm" OnClick="btnConfirmOrder_Click" />
                        <button type="button" class="btn-cancel-confirm" onclick="closeConfirmOrderModal()">Cancel</button>
                    </div>
                    <asp:HiddenField ID="confirmOrderId" runat="server" />
                </div>
            </div>

            <!-- Decline Order Modal -->
            <div id="declineOrderModal" class="confirmation-modal" onclick="if(event.target==this) closeDeclineOrderModal();">
                <div class="confirmation-content" onclick="event.stopPropagation();">
                    <div class="confirmation-icon danger">&#9888;</div>
                    <div class="confirmation-title">Decline Order</div>
                    <div class="confirmation-message">
                        Are you sure you want to decline order "<span id="declineOrderNumber"></span>"?<br />
                        No points will be deducted.
                    </div>
                    <div class="confirmation-actions">
                        <asp:Button ID="btnDeclineOrder" runat="server" Text="Decline" CssClass="btn-confirm" style="background-color: #dc3545;" OnClick="btnDeclineOrder_Click" />
                        <button type="button" class="btn-cancel-confirm" onclick="closeDeclineOrderModal()">Cancel</button>
                    </div>
                    <asp:HiddenField ID="declineOrderId" runat="server" />
                </div>
            </div>

            <!-- Fulfill Order Modal -->
            <div id="fulfillOrderModal" class="confirmation-modal" onclick="if(event.target==this) closeFulfillOrderModal();">
                <div class="confirmation-content" onclick="event.stopPropagation();">
                    <div class="confirmation-icon success">&#10003;</div>
                    <div class="confirmation-title">Mark as Fulfilled</div>
                    <div class="confirmation-message">
                        Have you given the items from order "<span id="fulfillOrderNumber"></span>" to the child?<br />
                        This will mark the order as fulfilled.
                    </div>
                    <div class="confirmation-actions">
                        <asp:Button ID="btnFulfillOrder" runat="server" Text="Yes, Mark as Fulfilled" CssClass="btn-confirm" OnClick="btnFulfillOrder_Click" />
                        <button type="button" class="btn-cancel-confirm" onclick="closeFulfillOrderModal()">Cancel</button>
                    </div>
                    <asp:HiddenField ID="fulfillOrderId" runat="server" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>

