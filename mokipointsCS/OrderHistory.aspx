<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OrderHistory.aspx.cs" Inherits="mokipointsCS.OrderHistory" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Order History - MOKI POINTS</title>
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
            background-color: #f0f0f0;
            border-color: #0066CC;
            color: #0066CC;
        }
        
        /* Container */
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 30px 50px;
        }
        
        .page-title {
            font-size: 32px;
            font-weight: 600;
            color: #333;
            margin-bottom: 10px;
        }
        
        .page-subtitle {
            color: #666;
            font-size: 16px;
            margin-bottom: 30px;
        }
        
        /* Date Filter */
        .filter-bar {
            background: white;
            padding: 20px;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
            margin-bottom: 30px;
            display: flex;
            gap: 15px;
            align-items: flex-end;
            flex-wrap: wrap;
        }
        
        .filter-group {
            display: flex;
            flex-direction: column;
            gap: 5px;
        }
        
        .filter-group label {
            font-size: 14px;
            color: #666;
            font-weight: 500;
        }
        
        .filter-group input[type="date"] {
            padding: 10px;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 14px;
        }
        
        .btn-filter {
            padding: 10px 20px;
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            transition: background-color 0.3s;
        }
        
        .btn-filter:hover {
            background-color: #0052a3;
        }
        
        .btn-clear {
            padding: 10px 20px;
            background-color: #6c757d;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            transition: background-color 0.3s;
        }
        
        .btn-clear:hover {
            background-color: #5a6268;
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
        
        .status-transactioncomplete {
            background-color: #d4edda;
            color: #155724;
        }
        
        .status-notfulfilled {
            background-color: #f8d7da;
            color: #721c24;
        }
        
        .status-refunded {
            background-color: #fff3cd;
            color: #856404;
        }
        
        .status-declined {
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
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 15px;
            background: #e7f3ff;
            border-radius: 6px;
            margin-bottom: 15px;
        }
        
        .order-total-label {
            font-size: 18px;
            font-weight: 600;
            color: #0066CC;
        }
        
        .order-total-amount {
            font-size: 20px;
            font-weight: 700;
            color: #0066CC;
        }
        
        .order-timeline {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 6px;
            margin-bottom: 15px;
        }
        
        .order-timeline h4 {
            color: #0066CC;
            margin-bottom: 10px;
            font-size: 14px;
            font-weight: 600;
        }
        
        .timeline-item {
            display: flex;
            gap: 10px;
            margin-bottom: 8px;
            font-size: 14px;
            color: #666;
        }
        
        .timeline-item:last-child {
            margin-bottom: 0;
        }
        
        .timeline-label {
            font-weight: 500;
            min-width: 120px;
        }
        
        .timeline-value {
            color: #333;
        }
        
        .child-info {
            background: #e7f3ff;
            padding: 12px;
            border-radius: 6px;
            margin-bottom: 15px;
            font-weight: 500;
            color: #0066cc;
        }
        
        .refund-code-section {
            background: #e7f3ff;
            padding: 12px;
            border-radius: 6px;
            margin-bottom: 15px;
        }
        
        .refund-code-section div:first-child {
            font-weight: 600;
            color: #0066cc;
            margin-bottom: 5px;
        }
        
        .refund-code-section span {
            font-family: monospace;
            font-size: 16px;
            color: #333;
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
    </style>
    <script>
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
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" />
        
        <!-- Header -->
        <div class="header">
            <div class="header-content">
                <a href="Dashboard.aspx" class="brand">
                    <span class="moki">MOKI</span> <span class="points">POINTS</span>
                </a>
                <div class="nav-links">
                    <asp:Literal ID="litNavigation" runat="server"></asp:Literal>
                </div>
                <div class="user-info">
                    <span class="user-name"><asp:Literal ID="litUserName" runat="server"></asp:Literal></span>
                    <a href="Settings.aspx" class="btn-settings">⚙</a>
                </div>
            </div>
        </div>
        
        <!-- Container -->
        <div class="container">
            <h1 class="page-title">Order History</h1>
            <p class="page-subtitle">View all completed and refunded orders</p>
            
            <!-- Date Filter -->
            <div class="filter-bar">
                <div class="filter-group">
                    <label>Start Date</label>
                    <asp:TextBox ID="txtStartDate" runat="server" TextMode="Date"></asp:TextBox>
                </div>
                <div class="filter-group">
                    <label>End Date</label>
                    <asp:TextBox ID="txtEndDate" runat="server" TextMode="Date"></asp:TextBox>
                </div>
                <asp:Button ID="btnFilter" runat="server" Text="Filter" CssClass="btn-filter" OnClick="btnFilter_Click" />
                <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn-clear" OnClick="btnClear_Click" />
            </div>
            
            <!-- Orders List -->
            <asp:Panel ID="pnlOrders" runat="server" CssClass="orders-list">
                <asp:Repeater ID="rptOrders" runat="server" OnItemDataBound="rptOrders_ItemDataBound">
                    <ItemTemplate>
                        <div class="order-card">
                            <div class="order-header">
                                <div class="order-info">
                                    <div class="order-number">Order #<%# Eval("OrderNumber") %></div>
                                    <asp:Panel ID="pnlChildInfo" runat="server" CssClass="child-info" Visible="false">
                                        Child: <asp:Literal ID="litChildName" runat="server"></asp:Literal>
                                    </asp:Panel>
                                    <div class="order-meta">
                                        <span>Order Date: <%# Eval("OrderDate", "{0:MMM dd, yyyy HH:mm}") %></span>
                                    </div>
                                </div>
                                <span class="status-badge status-<%# Eval("Status").ToString().ToLower() %>">
                                    <%# Eval("Status") %>
                                </span>
                            </div>
                            
                            <!-- Order Items -->
                            <div class="order-items">
                                <h4>Order Items</h4>
                                <asp:Repeater ID="rptOrderItems" runat="server">
                                    <ItemTemplate>
                                        <div class="order-item">
                                            <div>
                                                <div class="order-item-name"><%# Eval("RewardName") %></div>
                                                <div class="order-item-details">Quantity: <%# Eval("Quantity") %> × <%# Eval("PointCost", "{0:N0}") %> points</div>
                                            </div>
                                            <div class="order-item-details">
                                                <%# Convert.ToInt32(Eval("Quantity")) * Convert.ToInt32(Eval("PointCost")) %> points
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                            
                            <!-- Order Total -->
                            <div class="order-total">
                                <span class="order-total-label">Total Points:</span>
                                <span class="order-total-amount"><%# Eval("TotalPoints", "{0:N0}") %> points</span>
                            </div>
                            
                            <!-- Order Timeline -->
                            <div class="order-timeline">
                                <h4>Order Timeline</h4>
                                <div class="timeline-item">
                                    <span class="timeline-label">Order Placed:</span>
                                    <span class="timeline-value"><%# Eval("OrderDate", "{0:MMM dd, yyyy HH:mm}") %></span>
                                </div>
                                <asp:Panel ID="pnlConfirmed" runat="server" Visible="false">
                                    <div class="timeline-item">
                                        <span class="timeline-label">Confirmed:</span>
                                        <span class="timeline-value"><asp:Literal ID="litConfirmedDate" runat="server"></asp:Literal></span>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlFulfilled" runat="server" Visible="false">
                                    <div class="timeline-item">
                                        <span class="timeline-label">Fulfilled:</span>
                                        <span class="timeline-value"><asp:Literal ID="litFulfilledDate" runat="server"></asp:Literal></span>
                                    </div>
                                </asp:Panel>
                                <asp:Panel ID="pnlChildConfirmed" runat="server" Visible="false">
                                    <div class="timeline-item">
                                        <span class="timeline-label">Child Confirmed:</span>
                                        <span class="timeline-value"><asp:Literal ID="litChildConfirmedDate" runat="server"></asp:Literal></span>
                                    </div>
                                </asp:Panel>
                            </div>
                            
                            <!-- Refund Code (for parents only) -->
                            <asp:Panel ID="pnlRefundCode" runat="server" CssClass="refund-code-section" Visible="false">
                                <div>Refund Code:</div>
                                <span><asp:Literal ID="litRefundCode" runat="server"></asp:Literal></span>
                            </asp:Panel>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>
            
            <!-- Empty State -->
            <asp:Panel ID="pnlEmpty" runat="server" CssClass="empty-state" Visible="false">
                <p>No completed orders found.</p>
                <p style="font-size: 14px; margin-top: 10px; color: #999;">Completed orders will appear here once transactions are finished.</p>
            </asp:Panel>
        </div>
        
        <!-- Message Container -->
        <div class="message-container">
            <div class="message message-success" id="successMessage" style="display: none;">
                <span class="message-icon">✓</span>
                <span class="message-text"></span>
                <button class="message-close" onclick="closeMessage('successMessage')">×</button>
            </div>
            <div class="message message-error" id="errorMessage" style="display: none;">
                <span class="message-icon">✗</span>
                <span class="message-text"></span>
                <button class="message-close" onclick="closeMessage('errorMessage')">×</button>
            </div>
        </div>
    </form>
</body>
</html>

