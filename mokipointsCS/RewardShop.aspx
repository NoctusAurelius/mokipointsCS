<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RewardShop.aspx.cs" Inherits="mokipointsCS.RewardShop" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Reward Shop - MOKI POINTS</title>
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
        
        .btn-cart {
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
        
        .btn-cart:hover {
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
        
        /* Search and Filter */
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
        
        /* Rewards Grid */
        .rewards-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
            gap: 20px;
        }
        
        .reward-card {
            background-color: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            transition: box-shadow 0.3s, transform 0.3s;
            position: relative;
        }
        
        .reward-card:hover {
            box-shadow: 0 4px 8px rgba(0,0,0,0.15);
            transform: translateY(-2px);
        }
        
        .reward-card.unaffordable {
            opacity: 0.6;
        }
        
        .reward-image {
            width: 100%;
            height: 200px;
            object-fit: cover;
            border-radius: 5px;
            margin-bottom: 15px;
            background-color: #f0f0f0;
        }
        
        .reward-title {
            font-size: 20px;
            font-weight: bold;
            color: #333;
            margin-bottom: 10px;
        }
        
        .reward-meta {
            display: flex;
            gap: 10px;
            margin-bottom: 10px;
            flex-wrap: wrap;
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
        
        .reward-points {
            font-size: 24px;
            font-weight: bold;
            color: #FF6600;
            margin: 15px 0;
        }
        
        .reward-description {
            color: #666;
            font-size: 14px;
            margin-bottom: 15px;
            line-height: 1.5;
            min-height: 40px;
        }
        
        .reward-actions {
            display: flex;
            gap: 10px;
            margin-top: 15px;
            padding-top: 15px;
            border-top: 1px solid #e0e0e0;
        }
        
        .btn-action {
            padding: 10px 20px;
            border: none;
            border-radius: 5px;
            cursor: pointer;
            font-size: 14px;
            font-weight: 500;
            text-decoration: none;
            display: inline-block;
            transition: all 0.3s;
            flex: 1;
            text-align: center;
        }
        
        .btn-add-to-cart {
            background-color: #0066CC;
            color: white;
        }
        
        .btn-add-to-cart:hover {
            background-color: #0052a3;
        }
        
        .btn-add-to-cart:disabled {
            background-color: #ccc;
            cursor: not-allowed;
            opacity: 0.6;
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
        
        // Filter rewards
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
                        <a href="RewardShop.aspx" class="active">Shop</a>
                        <a href="Cart.aspx">Cart</a>
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
                <h1 class="page-title">Reward Shop</h1>
                <a href="Cart.aspx" class="btn-cart">View Cart</a>
            </div>

            <!-- Search and Filter -->
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

            <!-- Rewards List -->
            <asp:Panel ID="pnlRewards" runat="server">
                <div class="rewards-grid">
                    <asp:Repeater ID="rptRewards" runat="server" OnItemCommand="rptRewards_ItemCommand" OnItemDataBound="rptRewards_ItemDataBound">
                        <ItemTemplate>
                            <div id="rewardCard" runat="server" class="reward-card" data-category='<%# Eval("Category") ?? "" %>'>
                                <%# Eval("ImageUrl") != DBNull.Value && !string.IsNullOrEmpty(Eval("ImageUrl").ToString()) ? "<img src='" + Eval("ImageUrl") + "' alt='" + Server.HtmlEncode(Eval("Name").ToString()) + "' class='reward-image' />" : "<div class='reward-image'></div>" %>
                                
                                <div class="reward-title"><%# Eval("Name") %></div>
                                
                                <div class="reward-meta">
                                    <%# Eval("Category") != DBNull.Value && !string.IsNullOrEmpty(Eval("Category").ToString()) ? "<span class='badge badge-category'>" + Eval("Category") + "</span>" : "" %>
                                </div>
                                
                                <div class="reward-points"><%# Eval("PointCost") %> points</div>
                                
                                <%# Eval("Description") != DBNull.Value && !string.IsNullOrEmpty(Eval("Description").ToString()) ? "<div class='reward-description'>" + Server.HtmlEncode(Eval("Description").ToString().Length > 80 ? Eval("Description").ToString().Substring(0, 80) + "..." : Eval("Description").ToString()) + "</div>" : "<div class='reward-description'></div>" %>
                                
                                <div class="reward-actions">
                                    <asp:Button ID="btnAddToCart" runat="server" Text="Add to Cart" CssClass="btn-action btn-add-to-cart" 
                                        CommandName="AddToCart" CommandArgument='<%# Eval("Id") %>' />
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>
            
            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="empty-state">
                    <h3>No rewards available</h3>
                    <p>Check back later for new rewards!</p>
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>

