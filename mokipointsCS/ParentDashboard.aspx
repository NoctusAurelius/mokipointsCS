<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ParentDashboard.aspx.cs" Inherits="mokipointsCS.ParentDashboard" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Parent Dashboard - MOKI POINTS</title>
    <link rel="icon" type="image/x-icon" href="/favicon/favicon.ico" />
    <link rel="icon" type="image/png" sizes="16x16" href="/favicon/favicon-16x16.png" />
    <link rel="icon" type="image/png" sizes="32x32" href="/favicon/favicon-32x32.png" />
    <link rel="apple-touch-icon" sizes="180x180" href="/favicon/apple-touch-icon.png" />
    <link rel="manifest" href="/favicon/site.webmanifest" />
    <script src="https://cdn.jsdelivr.net/npm/chart.js@3.9.1/dist/chart.min.js"></script>
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
        
        /* Header/Navigation */
        .header {
            background-color: white;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            padding: 15px 0;
            position: sticky;
            top: 0;
            z-index: 1000;
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
        
        .user-name {
            color: #333;
            font-weight: 500;
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
            height: 2px;
            background-color: currentColor;
            border-radius: 2px;
            transition: all 0.3s;
        }
        
        /* Main Container */
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 30px;
        }
        
        .page-title {
            font-size: 32px;
            color: #333;
            margin-bottom: 10px;
        }
        
        .page-subtitle {
            color: #666;
            font-size: 16px;
            margin-bottom: 30px;
        }
        
        /* Dashboard Cards */
        .dashboard-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        
        .dashboard-card {
            background-color: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            border-left: 4px solid #0066CC;
        }
        
        .dashboard-card.orange {
            border-left-color: #FF6600;
        }
        
        .dashboard-card.pink {
            border-left-color: #FFB6C1;
        }
        
        .dashboard-card.yellow {
            border-left-color: #FFF9E6;
            background-color: #FFF9E6;
        }
        
        .card-title {
            font-size: 18px;
            font-weight: bold;
            color: #333;
            margin-bottom: 15px;
        }
        
        .card-value {
            font-size: 36px;
            font-weight: bold;
            color: #0066CC;
            margin-bottom: 10px;
        }
        
        .card-description {
            color: #666;
            font-size: 14px;
        }
        
        /* Quick Actions */
        .quick-actions {
            background-color: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin-bottom: 30px;
        }
        
        .section-title {
            font-size: 24px;
            color: #333;
            margin-bottom: 20px;
            padding-bottom: 10px;
            border-bottom: 2px solid #e0e0e0;
        }
        
        .actions-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 15px;
        }
        
        .action-btn {
            padding: 15px 20px;
            background-color: #0066CC;
            color: white;
            border: none;
            border-radius: 5px;
            font-size: 16px;
            font-weight: bold;
            cursor: pointer;
            text-decoration: none;
            display: inline-block;
            text-align: center;
            transition: background-color 0.3s, transform 0.2s;
        }
        
        .action-btn:hover {
            background-color: #0052a3;
            transform: translateY(-2px);
        }
        
        .action-btn.secondary {
            background-color: #FF6600;
        }
        
        .action-btn.secondary:hover {
            background-color: #e55a00;
        }
        
        /* Recent Activity */
        .recent-activity {
            background-color: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
        }
        
        .activity-item {
            padding: 15px;
            border-bottom: 1px solid #e0e0e0;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }
        
        .activity-item:last-child {
            border-bottom: none;
        }
        
        .activity-text {
            color: #333;
            font-size: 14px;
        }
        
        .activity-date {
            color: #999;
            font-size: 12px;
        }
        
        .placeholder-text {
            color: #999;
            font-style: italic;
            text-align: center;
            padding: 40px;
        }
        
        /* Achievements Display */
        .achievements-display {
            display: flex;
            gap: 30px;
            justify-content: center;
            flex-wrap: wrap;
            margin-bottom: 20px;
        }
        
        .achievement-badge-small {
            position: relative;
            width: 300px;
            height: 300px;
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: space-between;
            background-color: #f9f9f9;
            border-radius: 15px;
            padding: 15px;
            box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            transition: transform 0.3s, box-shadow 0.3s;
        }
        
        .achievement-badge-small:hover {
            transform: translateY(-5px);
            box-shadow: 0 6px 20px rgba(0,0,0,0.15);
        }
        
        .achievement-badge-small img {
            width: 220px;
            height: 220px;
            object-fit: contain;
            flex-shrink: 0;
        }
        
        .achievement-name-display {
            font-size: 16px;
            font-weight: bold;
            color: #333;
            text-align: center;
            margin-bottom: 10px;
            min-height: 40px;
            display: flex;
            align-items: center;
            justify-content: center;
        }
        
        .achievement-rarity-badge {
            display: inline-block;
            padding: 6px 16px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: bold;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            box-shadow: 0 2px 6px rgba(0,0,0,0.2);
        }
        
        .achievement-rarity-badge.rarity-common {
            background: linear-gradient(135deg, #9E9E9E 0%, #757575 100%);
            color: white;
        }
        
        .achievement-rarity-badge.rarity-uncommon {
            background: linear-gradient(135deg, #4CAF50 0%, #388E3C 100%);
            color: white;
        }
        
        .achievement-rarity-badge.rarity-rare {
            background: linear-gradient(135deg, #2196F3 0%, #1976D2 100%);
            color: white;
        }
        
        .achievement-rarity-badge.rarity-epic {
            background: linear-gradient(135deg, #9C27B0 0%, #7B1FA2 100%);
            color: white;
        }
        
        .achievement-rarity-badge.rarity-legendary {
            background: linear-gradient(135deg, #FF9800 0%, #F57C00 100%);
            color: white;
        }
        
        .achievement-rarity-badge.rarity-mythical {
            background: linear-gradient(135deg, #F44336 0%, #D32F2F 100%);
            color: white;
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
                    <div class="nav-links" style="display: flex; gap: 20px; align-items: center; margin-right: 20px;">
                        <a href="Family.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Family</a>
                        <a href="Tasks.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Tasks</a>
                        <a href="TaskReview.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Review</a>
                        <a href="Rewards.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Rewards</a>
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
            <h1 class="page-title">Parent Dashboard</h1>
            <p class="page-subtitle">Manage your family's chores and points</p>

            <!-- Stats Cards -->
            <div class="dashboard-grid">
                <div class="dashboard-card">
                    <div class="card-title">&#128176; Treasury Balance</div>
                    <div class="card-value"><asp:Literal ID="litTreasuryBalance" runat="server">0</asp:Literal></div>
                    <div class="card-description">Available points in family treasury</div>
                </div>
                <a href="TaskReview.aspx" style="text-decoration: none; color: inherit;">
                    <div class="dashboard-card orange" style="cursor: pointer; transition: transform 0.2s;" onmouseover="this.style.transform='scale(1.02)'" onmouseout="this.style.transform='scale(1)'">
                        <div class="card-title">&#9733; Pending Reviews</div>
                        <div class="card-value"><asp:Literal ID="litPendingReviews" runat="server">0</asp:Literal></div>
                        <div class="card-description">Tasks awaiting your review</div>
                    </div>
                </a>
                <a href="RewardOrders.aspx" style="text-decoration: none; color: inherit;">
                    <div class="dashboard-card pink" style="cursor: pointer; transition: transform 0.2s;" onmouseover="this.style.transform='scale(1.02)'" onmouseout="this.style.transform='scale(1)'">
                        <div class="card-title">&#128230; Pending Orders</div>
                        <div class="card-value"><asp:Literal ID="litPendingOrders" runat="server">0</asp:Literal></div>
                        <div class="card-description">Orders awaiting confirmation</div>
                    </div>
                </a>
                <div class="dashboard-card yellow">
                    <div class="card-title">&#128106; Active Children</div>
                    <div class="card-value"><asp:Literal ID="litActiveChildren" runat="server">0</asp:Literal></div>
                    <div class="card-description">Children in your family</div>
                </div>
            </div>

            <!-- Quick Actions -->
            <div class="quick-actions">
                <h2 class="section-title">Quick Actions</h2>
                <div class="actions-grid">
                    <a href="TaskReview.aspx" class="action-btn" style="position: relative;">
                        &#9733; Review Tasks
                        <asp:Panel ID="pnlReviewBadge" runat="server" Visible="false" 
                            style="position: absolute; top: -8px; right: -8px; background: #dc3545; color: white; 
                            border-radius: 50%; width: 24px; height: 24px; display: flex; align-items: center; 
                            justify-content: center; font-size: 12px; font-weight: bold;">
                            <asp:Literal ID="litReviewBadge" runat="server"></asp:Literal>
                        </asp:Panel>
                    </a>
                    <a href="RewardOrders.aspx" class="action-btn secondary" style="position: relative;">
                        &#128230; Manage Orders
                        <asp:Panel ID="pnlOrderBadge" runat="server" Visible="false" 
                            style="position: absolute; top: -8px; right: -8px; background: #dc3545; color: white; 
                            border-radius: 50%; width: 24px; height: 24px; display: flex; align-items: center; 
                            justify-content: center; font-size: 12px; font-weight: bold;">
                            <asp:Literal ID="litOrderBadge" runat="server"></asp:Literal>
                        </asp:Panel>
                    </a>
                    <a href="Tasks.aspx" class="action-btn">&#10133; Create Task</a>
                    <a href="Rewards.aspx" class="action-btn secondary">&#127873; Create Reward</a>
                    <a href="Family.aspx" class="action-btn">&#128106; View Family</a>
                </div>
            </div>

            <!-- Top Achievements -->
            <div class="quick-actions" style="margin-bottom: 30px;">
                <h2 class="section-title">Top Achievements</h2>
                <asp:Panel ID="pnlTopAchievements" runat="server">
                    <div class="achievements-display">
                        <asp:Repeater ID="rptTopAchievements" runat="server">
                            <ItemTemplate>
                                <div class="achievement-badge-small" title='<%# Eval("Name") %> - <%# Eval("Rarity") %>'>
                                    <img src='<%# Eval("BadgeImagePath") %>' alt='<%# Eval("Name") %>' />
                                    <div class="achievement-name-display"><%# Eval("Name") %></div>
                                    <div class="achievement-rarity-badge rarity-<%# Eval("Rarity").ToString().ToLower() %>"><%# Eval("Rarity") %></div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    <div style="text-align: center; margin-top: 15px;">
                        <a href="Achievements.aspx" style="color: #0066CC; text-decoration: none; font-weight: 500;">View All Achievements →</a>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnlNoAchievements" runat="server" Visible="false">
                    <div style="text-align: center; color: #999; padding: 20px;">
                        No achievements earned yet. Complete tasks and earn points to unlock achievements!
                        <br />
                        <a href="Achievements.aspx" style="color: #0066CC; text-decoration: none; font-weight: 500; margin-top: 10px; display: inline-block;">View All Achievements →</a>
                    </div>
                </asp:Panel>
            </div>

            <!-- Individual Child Task Metrics -->
            <div class="quick-actions" style="margin-bottom: 30px;">
                <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;">
                    <h2 class="section-title" style="margin: 0;">Individual Child Task Metrics</h2>
                    <div style="display: flex; gap: 10px; align-items: center;">
                        <label style="font-size: 14px; color: #666;">Time Period:</label>
                        <select id="ddlTimePeriod" style="padding: 8px 12px; border: 1px solid #ddd; border-radius: 5px; font-size: 14px; cursor: pointer;">
                            <option value="day">Last 7 Days</option>
                            <option value="week" selected="selected">Last 4 Weeks</option>
                            <option value="month">Last 6 Months</option>
                        </select>
                    </div>
                </div>
                
                <!-- Information Panel (Collapsible) -->
                <div style="position: relative; margin-bottom: 20px;">
                    <button type="button" class="info-toggle-btn" onmouseenter="showDashboardInfo()" onmouseleave="hideDashboardInfo()" style="background-color: #2196f3; color: white; border: none; border-radius: 50%; width: 35px; height: 35px; cursor: pointer; font-size: 18px; display: flex; align-items: center; justify-content: center; box-shadow: 0 2px 5px rgba(0,0,0,0.2); transition: all 0.3s ease;" title="Show instructions">&#8505;</button>
                    <div id="dashboardInfoPanel" onmouseenter="showDashboardInfo()" onmouseleave="hideDashboardInfo()" style="display: none; position: absolute; top: 45px; left: 0; z-index: 1000; background-color: #e3f2fd; border-left: 4px solid #2196f3; padding: 15px; border-radius: 5px; min-width: 400px; max-width: 500px; box-shadow: 0 4px 12px rgba(0,0,0,0.15);">
                        <div style="display: flex; align-items: flex-start; gap: 10px;">
                            <div style="font-size: 20px; color: #2196f3; flex-shrink: 0;">&#8505;</div>
                            <div style="flex: 1;">
                                <div style="font-weight: 600; color: #1976d2; margin-bottom: 8px; font-size: 15px;">About Individual Child Task Metrics</div>
                                <div style="color: #333; font-size: 14px; line-height: 1.6;">
                                    <p style="margin: 0 0 8px 0;">These charts show each child's task completion and failure rates over time. Use the time period selector to view daily, weekly, or monthly trends.</p>
                                    <p style="margin: 0;">&bull; <strong>Completed Tasks</strong> (green line): Tasks successfully completed and approved</p>
                                    <p style="margin: 0;">&bull; <strong>Failed Tasks</strong> (red line): Tasks that were not completed or failed review</p>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <script>
                    function showDashboardInfo() {
                        document.getElementById('dashboardInfoPanel').style.display = 'block';
                    }
                    function hideDashboardInfo() {
                        document.getElementById('dashboardInfoPanel').style.display = 'none';
                    }
                </script>
                
                <asp:Panel ID="pnlChildMetrics" runat="server">
                    <div id="childMetricsContainer" style="display: flex; gap: 20px; overflow-x: auto; padding-bottom: 10px; scroll-behavior: smooth;">
                        <!-- Child metric charts will be dynamically generated here -->
                    </div>
                </asp:Panel>
                
                <asp:Panel ID="pnlNoChildrenMetrics" runat="server" CssClass="placeholder-text" Visible="false">
                    No children in your family
                </asp:Panel>
                
                <asp:HiddenField ID="hdnChildMetricsData" runat="server" />
                <asp:HiddenField ID="hdnAchievementNotification" runat="server" />
            </div>
        </div>
    </form>

    <script>
        var childCharts = [];
        var currentPeriod = 'week';
        
        // Load child metrics data
        function loadChildMetrics() {
            try {
                var metricsField = document.getElementById('<%= hdnChildMetricsData.ClientID %>');
                if (!metricsField || !metricsField.value) {
                    console.log('No child metrics data available');
                    return;
                }
                
                var childrenData = JSON.parse(metricsField.value);
                var container = document.getElementById('childMetricsContainer');
                if (!container) return;
                
                container.innerHTML = '';
                childCharts = [];
                
                // Limit to 4 children at a time, show horizontal scroll for more
                childrenData.forEach(function(childData, index) {
                    if (index >= 4) return; // Show max 4 at a time
                    
                    var chartCard = document.createElement('div');
                    chartCard.style.cssText = 'min-width: 300px; max-width: 300px; background: white; border-radius: 10px; padding: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1);';
                    
                    var chartTitle = document.createElement('div');
                    chartTitle.style.cssText = 'font-weight: bold; font-size: 16px; color: #333; margin-bottom: 15px; text-align: center;';
                    chartTitle.textContent = childData.childName;
                    chartCard.appendChild(chartTitle);
                    
                    var canvas = document.createElement('canvas');
                    canvas.id = 'childChart_' + childData.childId;
                    canvas.style.cssText = 'max-height: 250px;';
                    chartCard.appendChild(canvas);
                    
                    container.appendChild(chartCard);
                    
                    // Create chart
                    var ctx = canvas.getContext('2d');
                    var chart = new Chart(ctx, {
                        type: 'line',
                        data: {
                            labels: childData.labels,
                            datasets: [
                                {
                                    label: 'Completed',
                                    data: childData.completed,
                                    borderColor: 'rgba(76, 175, 80, 1)',
                                    backgroundColor: 'rgba(76, 175, 80, 0.1)',
                                    borderWidth: 2,
                                    fill: true,
                                    tension: 0.4
                                },
                                {
                                    label: 'Failed',
                                    data: childData.failed,
                                    borderColor: 'rgba(244, 67, 54, 1)',
                                    backgroundColor: 'rgba(244, 67, 54, 0.1)',
                                    borderWidth: 2,
                                    fill: true,
                                    tension: 0.4
                                }
                            ]
                        },
                        options: {
                            responsive: true,
                            maintainAspectRatio: true,
                            plugins: {
                                legend: {
                                    display: true,
                                    position: 'bottom'
                                }
                            },
                            scales: {
                                y: {
                                    beginAtZero: true,
                                    ticks: {
                                        stepSize: 1
                                    }
                                }
                            }
                        }
                    });
                    
                    childCharts.push(chart);
                });
                
                // Show scroll indicator if more than 4 children
                if (childrenData.length > 4) {
                    var scrollIndicator = document.createElement('div');
                    scrollIndicator.style.cssText = 'text-align: center; color: #666; font-size: 12px; margin-top: 10px;';
                    scrollIndicator.textContent = '← Scroll horizontally to see more children →';
                    container.parentElement.appendChild(scrollIndicator);
                }
            } catch(e) {
                console.error('Error loading child metrics:', e);
            }
        }
        
        // Handle time period change
        document.addEventListener('DOMContentLoaded', function() {
            loadChildMetrics();
            
            var periodSelect = document.getElementById('ddlTimePeriod');
            if (periodSelect) {
                periodSelect.addEventListener('change', function() {
                    currentPeriod = this.value;
                    // Reload page with new period (or use AJAX to reload data)
                    // For now, we'll reload the page
                    window.location.href = 'ParentDashboard.aspx?period=' + currentPeriod;
                });
            }
        });
    </script>

    <!-- Achievement Notification Popup -->
    <div id="achievementNotification" class="achievement-notification" style="display: none;">
        <div class="achievement-notification-content">
            <div class="achievement-notification-close" onclick="closeAchievementNotification()">&times;</div>
            <div class="achievement-notification-header">&#127941; Achievement Unlocked!</div>
            <div class="achievement-notification-badge">
                <img id="notificationBadgeImg" src="#" alt="Achievement Badge" />
            </div>
            <div class="achievement-notification-name" id="notificationName"></div>
            <div class="achievement-notification-rarity" id="notificationRarity"></div>
            <div class="achievement-notification-description" id="notificationDescription"></div>
        </div>
    </div>

    <script>
        // Achievement Notification System
        function showAchievementNotification(achievementData) {
            if (!achievementData) return;
            
            const notification = document.getElementById('achievementNotification');
            const badgeImg = document.getElementById('notificationBadgeImg');
            const nameEl = document.getElementById('notificationName');
            const rarityEl = document.getElementById('notificationRarity');
            const descEl = document.getElementById('notificationDescription');
            
            // Set content
            badgeImg.src = achievementData.BadgeImagePath || '';
            nameEl.textContent = achievementData.Name || '';
            descEl.textContent = achievementData.Description || '';
            
            // Set rarity badge with color
            const rarity = achievementData.Rarity || 'Common';
            const rarityColors = {
                'Common': '#9E9E9E',
                'Uncommon': '#4CAF50',
                'Rare': '#2196F3',
                'Epic': '#9C27B0',
                'Legendary': '#FF9800',
                'Mythical': '#F44336'
            };
            const color = rarityColors[rarity] || '#9E9E9E';
            rarityEl.innerHTML = '<span style="background-color: ' + color + '; color: white; padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: bold; text-transform: uppercase;">' + rarity + '</span>';
            
            // Show notification with fade-in animation
            notification.style.display = 'flex';
            setTimeout(() => {
                notification.style.opacity = '1';
                notification.style.transform = 'translateY(0)';
            }, 10);
            
            // Play sound effect (ONLY with dashboard notification)
            playAchievementSound(rarity);
            
            // Auto-fade out after 5 seconds
            setTimeout(() => {
                closeAchievementNotification();
            }, 5000);
        }
        
        function closeAchievementNotification() {
            const notification = document.getElementById('achievementNotification');
            notification.style.opacity = '0';
            notification.style.transform = 'translateY(-20px)';
            setTimeout(() => {
                notification.style.display = 'none';
            }, 300);
        }
        
        function playAchievementSound(rarity) {
            try {
                const rarityLower = (rarity || 'common').toLowerCase();
                const soundPath = '/Sounds/Achievements/unlock_' + rarityLower + '.mp3';
                
                const audio = new Audio(soundPath);
                audio.volume = 0.6; // 60% volume
                audio.play().catch(function(error) {
                    // Fallback to common sound if rarity-specific not found
                    console.log('Failed to play rarity-specific sound, trying fallback');
                    const fallbackAudio = new Audio('/Sounds/Achievements/unlock_common.mp3');
                    fallbackAudio.volume = 0.6;
                    fallbackAudio.play().catch(function(fallbackError) {
                        console.log('Failed to play achievement sound:', fallbackError);
                    });
                });
            } catch (e) {
                console.log('Error playing achievement sound:', e);
            }
        }
        
        // Check for achievement notification on page load
        document.addEventListener('DOMContentLoaded', function() {
            const achievementData = document.getElementById('<%= hdnAchievementNotification.ClientID %>').value;
            if (achievementData && achievementData.trim() !== '') {
                try {
                    const data = JSON.parse(achievementData);
                    if (data && data.Name) {
                        // Small delay to ensure page is fully loaded
                        setTimeout(() => {
                            showAchievementNotification(data);
                        }, 500);
                    }
                } catch (e) {
                    console.error('Error parsing achievement notification data:', e);
                }
            }
        });
    </script>

    <style>
        /* Achievement Notification Styles */
        .achievement-notification {
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 10000;
            opacity: 0;
            transform: translateY(-20px);
            transition: opacity 0.3s ease-in, transform 0.3s ease-in;
        }
        
        .achievement-notification-content {
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            border-radius: 15px;
            padding: 20px;
            box-shadow: 0 8px 24px rgba(0,0,0,0.3);
            min-width: 300px;
            max-width: 400px;
            position: relative;
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
        
        .achievement-notification-close {
            position: absolute;
            top: 10px;
            right: 10px;
            font-size: 24px;
            cursor: pointer;
            color: white;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            transition: background-color 0.3s;
        }
        
        .achievement-notification-close:hover {
            background-color: rgba(255,255,255,0.2);
        }
        
        .achievement-notification-header {
            color: white;
            font-size: 18px;
            font-weight: bold;
            text-align: center;
            margin-bottom: 15px;
            animation: fadeInDown 0.5s ease-out;
        }
        
        @keyframes fadeInDown {
            from {
                opacity: 0;
                transform: translateY(-10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }
        
        .achievement-notification-badge {
            width: 100px;
            height: 100px;
            margin: 0 auto 15px;
            border-radius: 10px;
            overflow: hidden;
            background-color: rgba(255,255,255,0.2);
            animation: scaleIn 0.5s ease-out;
        }
        
        @keyframes scaleIn {
            from {
                transform: scale(0.8);
                opacity: 0;
            }
            to {
                transform: scale(1);
                opacity: 1;
            }
        }
        
        .achievement-notification-badge img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }
        
        .achievement-notification-name {
            color: white;
            font-size: 20px;
            font-weight: bold;
            text-align: center;
            margin-bottom: 8px;
            animation: fadeIn 0.6s ease-out;
        }
        
        .achievement-notification-rarity {
            text-align: center;
            margin-bottom: 10px;
            animation: fadeIn 0.7s ease-out;
        }
        
        .achievement-notification-description {
            color: rgba(255,255,255,0.9);
            font-size: 14px;
            text-align: center;
            line-height: 1.4;
            animation: fadeIn 0.8s ease-out;
        }
        
        @keyframes fadeIn {
            from {
                opacity: 0;
            }
            to {
                opacity: 1;
            }
        }
    </style>
</body>
</html>

