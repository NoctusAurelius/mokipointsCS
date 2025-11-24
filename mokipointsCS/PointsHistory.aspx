<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PointsHistory.aspx.cs" Inherits="mokipointsCS.PointsHistory" EnableEventValidation="false" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Points History - MOKI POINTS</title>
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
        
        .user-info {
            display: flex;
            align-items: center;
            gap: 20px;
        }
        
        .nav-links {
            display: flex;
            gap: 20px;
            align-items: center;
            margin-right: 20px;
        }
        
        .nav-links a {
            color: #333;
            text-decoration: none;
            font-weight: 500;
            font-size: 16px;
        }
        
        .nav-links a:hover {
            color: #0066CC;
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
        
        /* Points Summary */
        .points-summary {
            background: linear-gradient(135deg, #0066CC 0%, #FF6600 100%);
            border-radius: 15px;
            padding: 30px;
            color: white;
            margin-bottom: 30px;
            box-shadow: 0 4px 20px rgba(0,0,0,0.2);
        }
        
        .summary-label {
            font-size: 18px;
            opacity: 0.9;
            margin-bottom: 10px;
        }
        
        .summary-value {
            font-size: 48px;
            font-weight: bold;
            margin-bottom: 10px;
        }
        
        /* Transactions List */
        .transactions-list {
            display: grid;
            gap: 15px;
        }
        
        .transaction-card {
            background-color: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            display: flex;
            justify-content: space-between;
            align-items: center;
            border-left: 4px solid #0066CC;
        }
        
        .transaction-card.earned {
            border-left-color: #2e7d32;
        }
        
        .transaction-card.spent {
            border-left-color: #d32f2f;
        }
        
        .transaction-info {
            flex: 1;
        }
        
        .transaction-description {
            font-size: 16px;
            font-weight: 500;
            color: #333;
            margin-bottom: 5px;
        }
        
        .transaction-date {
            font-size: 14px;
            color: #999;
        }
        
        .transaction-amount {
            font-size: 24px;
            font-weight: bold;
            margin-left: 20px;
        }
        
        .transaction-amount.earned {
            color: #2e7d32;
        }
        
        .transaction-amount.spent {
            color: #d32f2f;
        }
        
        .empty-state {
            text-align: center;
            padding: 60px 20px;
            color: #999;
            background-color: white;
            border-radius: 10px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        
        .empty-state-icon {
            font-size: 64px;
            margin-bottom: 20px;
        }
    </style>
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
                    <div class="nav-links">
                        <a href="ChildDashboard.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Dashboard</a>
                        <a href="ChildTasks.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Tasks</a>
                        <a href="PointsHistory.aspx" style="color: #0066CC; text-decoration: none; font-weight: 500; font-size: 16px;">Points</a>
                        <a href="RewardShop.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Shop</a>
                        <a href="Cart.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Cart</a>
                        <a href="MyOrders.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">My Orders</a>
                    </div>
                    <span class="user-name">Points History</span>
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
                <h1 class="page-title">Points Transaction History</h1>
            </div>

            <!-- Points Summary -->
            <div class="points-summary">
                <div class="summary-label">Your Total Points</div>
                <div class="summary-value">
                    <asp:Literal ID="litTotalPoints" runat="server">0</asp:Literal>
                </div>
            </div>

            <!-- Transactions List -->
            <asp:Panel ID="pnlTransactions" runat="server">
                <div class="transactions-list">
                    <asp:Repeater ID="rptTransactions" runat="server">
                        <ItemTemplate>
                            <div class="transaction-card <%# Eval("TransactionType").ToString().ToLower() %>">
                                <div class="transaction-info">
                                    <div class="transaction-description"><%# Eval("Description") %></div>
                                    <div class="transaction-date"><%# Convert.ToDateTime(Eval("TransactionDate")).ToString("MMM dd, yyyy hh:mm tt") %></div>
                                </div>
                                <div class="transaction-amount <%# Eval("TransactionType").ToString().ToLower() %>">
                                    <%# Eval("TransactionType").ToString() == "Earned" ? "+" : "-" %><%# Convert.ToInt32(Eval("Points")).ToString("N0") %>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlEmpty" runat="server" Visible="false">
                <div class="empty-state">
                    <div class="empty-state-icon">💰</div>
                    <h3>No transactions yet</h3>
                    <p>Complete tasks to start earning points!</p>
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>

