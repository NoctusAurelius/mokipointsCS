<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Cart.aspx.cs" Inherits="mokipointsCS.Cart" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Shopping Cart - MOKI POINTS</title>
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
        
        .points-display-header {
            background: linear-gradient(135deg, #FF6600 0%, #FF8533 100%);
            color: white;
            padding: 10px 20px;
            border-radius: 20px;
            font-weight: 600;
            font-size: 16px;
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
        
        .btn-continue-shopping {
            padding: 10px 20px;
            background-color: #6c757d;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            text-decoration: none;
            display: inline-block;
            transition: all 0.3s;
        }
        
        .btn-continue-shopping:hover {
            background-color: #5a6268;
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
        
        /* Cart Items */
        .cart-items {
            background: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }
        
        .cart-item {
            display: flex;
            gap: 20px;
            padding: 20px;
            border-bottom: 1px solid #e0e0e0;
            align-items: center;
        }
        
        .cart-item:last-child {
            border-bottom: none;
        }
        
        .cart-item-image {
            width: 100px;
            height: 100px;
            object-fit: cover;
            border-radius: 5px;
            background-color: #f0f0f0;
        }
        
        .cart-item-details {
            flex: 1;
        }
        
        .cart-item-name {
            font-size: 18px;
            font-weight: 600;
            color: #333;
            margin-bottom: 5px;
        }
        
        .cart-item-points {
            font-size: 16px;
            color: #FF6600;
            font-weight: 600;
        }
        
        .cart-item-quantity {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        
        .quantity-controls {
            display: flex;
            align-items: center;
            gap: 10px;
        }
        
        .btn-quantity {
            width: 30px;
            height: 30px;
            border: 1px solid #ddd;
            background: white;
            border-radius: 5px;
            cursor: pointer;
            font-size: 18px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .btn-quantity:hover {
            background: #f5f5f5;
        }
        
        .quantity-input {
            width: 60px;
            padding: 5px;
            text-align: center;
            border: 1px solid #ddd;
            border-radius: 5px;
            font-size: 16px;
        }
        
        .cart-item-subtotal {
            font-size: 18px;
            font-weight: 600;
            color: #333;
            min-width: 100px;
            text-align: right;
        }
        
        .btn-remove {
            padding: 8px 16px;
            background-color: #dc3545;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
        }
        
        .btn-remove:hover {
            background-color: #c82333;
        }
        
        /* Cart Summary */
        .cart-summary {
            background: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }
        
        .summary-row {
            display: flex;
            justify-content: space-between;
            margin-bottom: 15px;
            font-size: 16px;
        }
        
        .summary-row.total {
            border-top: 2px solid #e0e0e0;
            padding-top: 15px;
            margin-top: 15px;
            font-size: 24px;
            font-weight: 600;
            color: #FF6600;
        }
        
        .btn-checkout {
            width: 100%;
            padding: 15px;
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 18px;
            font-weight: 600;
            margin-top: 20px;
            transition: all 0.3s;
        }
        
        .btn-checkout:hover {
            background-color: #0052a3;
        }
        
        .btn-checkout:disabled {
            background-color: #ccc;
            cursor: not-allowed;
        }
        
        .empty-state {
            text-align: center;
            padding: 60px 20px;
            color: #999;
            background: white;
            border-radius: 10px;
        }
        
        .empty-state h3 {
            font-size: 24px;
            margin-bottom: 10px;
            color: #666;
        }
        
        .insufficient-points {
            background: #fff3cd;
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
            color: #856404;
            border-left: 4px solid #ffc107;
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
        
        // Update quantity
        function updateQuantity(rewardId, change) {
            var quantityInput = document.getElementById('quantity_' + rewardId);
            if (quantityInput) {
                var currentQty = parseInt(quantityInput.value) || 1;
                var newQty = currentQty + change;
                if (newQty < 1) newQty = 1;
                quantityInput.value = newQty;
                
                // Trigger postback to update
                __doPostBack('UpdateQuantity', rewardId + '|' + newQty);
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
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
                    <div class="nav-links">
                        <a href="ChildDashboard.aspx">Dashboard</a>
                        <a href="ChildTasks.aspx">Tasks</a>
                        <a href="RewardShop.aspx">Shop</a>
                        <a href="Cart.aspx" class="active">Cart</a>
                        <a href="MyOrders.aspx">My Orders</a>
                        <a href="PointsHistory.aspx">Points</a>
                    </div>
                    <div class="points-display-header">
                        <asp:Literal ID="litPointsBalance" runat="server"></asp:Literal> Points
                    </div>
                    <span class="user-name">Hi, <asp:Literal ID="litUserName" runat="server"></asp:Literal>!</span>
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
                <h1 class="page-title">Shopping Cart</h1>
                <a href="RewardShop.aspx" class="btn-continue-shopping">Continue Shopping</a>
            </div>

            <asp:Panel ID="pnlCart" runat="server">
                <div class="cart-items">
                    <asp:Repeater ID="rptCartItems" runat="server" OnItemCommand="rptCartItems_ItemCommand">
                        <ItemTemplate>
                            <div class="cart-item">
                                <%# Eval("ImageUrl") != DBNull.Value && !string.IsNullOrEmpty(Eval("ImageUrl").ToString()) ? "<img src='" + Eval("ImageUrl") + "' alt='" + Server.HtmlEncode(Eval("Name").ToString()) + "' class='cart-item-image' />" : "<div class='cart-item-image'></div>" %>
                                
                                <div class="cart-item-details">
                                    <div class="cart-item-name"><%# Eval("Name") %></div>
                                    <div class="cart-item-points"><%# Eval("PointCost") %> points each</div>
                                </div>
                                
                                <div class="cart-item-quantity">
                                    <div class="quantity-controls">
                                        <button type="button" class="btn-quantity" onclick='updateQuantity(<%# Eval("RewardId") %>, -1)'>-</button>
                                        <asp:TextBox ID="txtQuantity" runat="server" CssClass="quantity-input" Text='<%# Eval("Quantity") %>' 
                                            data-reward-id='<%# Eval("RewardId") %>' onchange='__doPostBack("UpdateQuantity", "<%# Eval("RewardId") %>|" + this.value)' />
                                        <button type="button" class="btn-quantity" onclick='updateQuantity(<%# Eval("RewardId") %>, 1)'>+</button>
                                    </div>
                                </div>
                                
                                <div class="cart-item-subtotal">
                                    <asp:Literal ID="litSubtotal" runat="server"></asp:Literal>
                                </div>
                                
                                <asp:Button ID="btnRemove" runat="server" Text="Remove" CssClass="btn-remove" 
                                    CommandName="Remove" CommandArgument='<%# Eval("RewardId") %>' />
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="cart-summary">
                    <div class="summary-row">
                        <span>Subtotal:</span>
                        <span><asp:Literal ID="litSubtotal" runat="server"></asp:Literal></span>
                    </div>
                    <div class="summary-row total">
                        <span>Total:</span>
                        <span><asp:Literal ID="litTotal" runat="server"></asp:Literal></span>
                    </div>
                    
                    <asp:Panel ID="pnlInsufficientPoints" runat="server" Visible="false" CssClass="insufficient-points">
                        You don't have enough points for this purchase. You need <asp:Literal ID="litPointsNeeded" runat="server"></asp:Literal> more points.
                    </asp:Panel>
                    
                    <asp:Button ID="btnCheckout" runat="server" Text="Checkout" CssClass="btn-checkout" OnClick="btnCheckout_Click" />
                </div>
            </asp:Panel>
            
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="empty-state">
                    <h3>Your cart is empty</h3>
                    <p>Add some rewards to your cart to get started!</p>
                    <a href="RewardShop.aspx" class="btn-continue-shopping" style="margin-top: 20px;">Continue Shopping</a>
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>

