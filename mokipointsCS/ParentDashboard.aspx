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

            <!-- Individual Child Task Metrics -->
            <div class="quick-actions" style="margin-bottom: 30px;">
                <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px;">
                    <h2 class="section-title" style="margin: 0;">Individual Child Task Metrics</h2>
                    <div style="display: flex; gap: 10px; align-items: center;">
                        <label style="font-size: 14px; color: #666;">Time Period:</label>
                        <select id="ddlTimePeriod" style="padding: 8px 12px; border: 1px solid #ddd; border-radius: 5px; font-size: 14px; cursor: pointer;">
                            <option value="day">Last 7 Days</option>
                            <option value="week" selected>Last 4 Weeks</option>
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
</body>
</html>

