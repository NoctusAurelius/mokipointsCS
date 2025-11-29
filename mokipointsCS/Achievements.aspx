<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Achievements.aspx.cs" Inherits="mokipointsCS.Achievements" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Achievements - MOKI POINTS</title>
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
        
        .nav-links a:hover {
            color: #0066CC;
        }
        
        /* Main Container */
        .container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 0 30px;
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
        
        /* Filters */
        .filters-section {
            background-color: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin-bottom: 30px;
        }
        
        .filters-title {
            font-size: 18px;
            font-weight: bold;
            margin-bottom: 15px;
            color: #333;
        }
        
        .filter-buttons {
            display: flex;
            flex-wrap: wrap;
            gap: 10px;
        }
        
        .filter-btn {
            padding: 8px 16px;
            border: 2px solid #ddd;
            background-color: white;
            border-radius: 20px;
            cursor: pointer;
            font-size: 14px;
            transition: all 0.3s;
        }
        
        .filter-btn:hover {
            border-color: #0066CC;
            color: #0066CC;
        }
        
        .filter-btn.active {
            background-color: #0066CC;
            color: white;
            border-color: #0066CC;
        }
        
        /* Achievement Grid */
        .achievements-grid {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(280px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }
        
        .achievement-card {
            background-color: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            cursor: pointer;
            transition: all 0.3s;
            position: relative;
            overflow: hidden;
        }
        
        .achievement-card:hover {
            transform: translateY(-5px);
            box-shadow: 0 4px 12px rgba(0,0,0,0.15);
        }
        
        .achievement-card.earned {
            border: 2px solid transparent;
        }
        
        .achievement-card.unearned {
            opacity: 0.6;
            filter: grayscale(100%);
        }
        
        .achievement-card.unearned::before {
            content: '\1F512';
            position: absolute;
            top: 10px;
            right: 10px;
            font-size: 24px;
        }
        
        /* Rarity Colors */
        .achievement-card.rarity-common {
            border-left: 4px solid #9E9E9E;
        }
        
        .achievement-card.rarity-uncommon {
            border-left: 4px solid #4CAF50;
        }
        
        .achievement-card.rarity-rare {
            border-left: 4px solid #2196F3;
        }
        
        .achievement-card.rarity-epic {
            border-left: 4px solid #9C27B0;
        }
        
        .achievement-card.rarity-legendary {
            border-left: 4px solid #FF9800;
        }
        
        .achievement-card.rarity-mythical {
            border-left: 4px solid #F44336;
        }
        
        .achievement-badge {
            width: 80px;
            height: 80px;
            margin: 0 auto 15px;
            border-radius: 10px;
            overflow: hidden;
            background-color: #f0f0f0;
        }
        
        .achievement-badge img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }
        
        .achievement-name {
            font-size: 18px;
            font-weight: bold;
            margin-bottom: 8px;
            text-align: center;
            color: #333;
        }
        
        .achievement-rarity {
            display: block;
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: bold;
            margin: 0 auto 10px auto;
            text-transform: uppercase;
            text-align: center;
            width: fit-content;
            min-width: 80px;
        }
        
        .rarity-common .achievement-rarity {
            background-color: #9E9E9E;
            color: white;
        }
        
        .rarity-uncommon .achievement-rarity {
            background-color: #4CAF50;
            color: white;
        }
        
        .rarity-rare .achievement-rarity {
            background-color: #2196F3;
            color: white;
        }
        
        .rarity-epic .achievement-rarity {
            background-color: #9C27B0;
            color: white;
        }
        
        .rarity-legendary .achievement-rarity {
            background-color: #FF9800;
            color: white;
        }
        
        .rarity-mythical .achievement-rarity {
            background-color: #F44336;
            color: white;
        }
        
        .achievement-description {
            font-size: 14px;
            color: #666;
            margin-bottom: 15px;
            text-align: center;
            min-height: 40px;
        }
        
        .achievement-progress {
            margin-top: 15px;
        }
        
        .progress-bar-container {
            background-color: #e0e0e0;
            border-radius: 10px;
            height: 8px;
            overflow: hidden;
            margin-bottom: 5px;
        }
        
        .progress-bar {
            height: 100%;
            background: linear-gradient(90deg, #4CAF50, #8BC34A);
            border-radius: 10px;
            transition: width 1s ease-in-out;
            animation: progressAnimation 1s ease-in-out;
        }
        
        @keyframes progressAnimation {
            from {
                width: 0%;
            }
        }
        
        .progress-text {
            font-size: 12px;
            color: #666;
            text-align: center;
        }
        
        .achievement-earned-date {
            font-size: 12px;
            color: #4CAF50;
            text-align: center;
            margin-top: 10px;
            font-weight: bold;
        }
        
        /* Achievement Detail Modal */
        .modal-overlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100%;
            height: 100%;
            background-color: rgba(0,0,0,0.7);
            z-index: 1000;
            animation: fadeIn 0.3s ease-in;
        }
        
        .modal-overlay.show {
            display: flex;
            justify-content: center;
            align-items: center;
        }
        
        @keyframes fadeIn {
            from {
                opacity: 0;
            }
            to {
                opacity: 1;
            }
        }
        
        .modal-content {
            background-color: white;
            border-radius: 15px;
            padding: 30px;
            max-width: 500px;
            width: 90%;
            max-height: 90vh;
            overflow-y: auto;
            position: relative;
            animation: slideIn 0.3s ease-out;
        }
        
        @keyframes slideIn {
            from {
                transform: translateY(-50px);
                opacity: 0;
            }
            to {
                transform: translateY(0);
                opacity: 1;
            }
        }
        
        .modal-close {
            position: absolute;
            top: 15px;
            right: 15px;
            font-size: 24px;
            cursor: pointer;
            color: #999;
            width: 30px;
            height: 30px;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            transition: all 0.3s;
        }
        
        .modal-close:hover {
            background-color: #f0f0f0;
            color: #333;
        }
        
        .modal-badge {
            width: 128px;
            height: 128px;
            margin: 0 auto 20px;
            border-radius: 15px;
            overflow: hidden;
            background-color: #f0f0f0;
        }
        
        .modal-badge img {
            width: 100%;
            height: 100%;
            object-fit: cover;
        }
        
        .modal-name {
            font-size: 24px;
            font-weight: bold;
            text-align: center;
            margin-bottom: 10px;
            color: #333;
        }
        
        .modal-rarity {
            text-align: center;
            margin-bottom: 20px;
        }
        
        .modal-description {
            font-size: 16px;
            color: #666;
            margin-bottom: 20px;
            text-align: center;
        }
        
        .modal-section {
            margin-bottom: 20px;
            padding-top: 20px;
            border-top: 1px solid #e0e0e0;
        }
        
        .modal-section-title {
            font-size: 14px;
            font-weight: bold;
            color: #333;
            margin-bottom: 10px;
        }
        
        .modal-section-content {
            font-size: 14px;
            color: #666;
            line-height: 1.6;
        }
        
        .modal-status {
            text-align: center;
            padding: 10px;
            border-radius: 8px;
            margin-bottom: 15px;
        }
        
        .modal-status.earned {
            background-color: #E8F5E9;
            color: #2E7D32;
        }
        
        .modal-status.unearned {
            background-color: #FFF3E0;
            color: #E65100;
        }
        
        .modal-developer-message {
            font-style: italic;
            background-color: #F5F5F5;
            padding: 15px;
            border-radius: 8px;
            border-left: 4px solid #0066CC;
        }
        
        .no-achievements {
            text-align: center;
            padding: 60px 20px;
            color: #999;
        }
        
        .no-achievements-icon {
            font-size: 64px;
            margin-bottom: 20px;
        }
        
        .stats-section {
            background-color: white;
            border-radius: 10px;
            padding: 20px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin-bottom: 30px;
            display: flex;
            justify-content: space-around;
            flex-wrap: wrap;
            gap: 20px;
        }
        
        .stat-item {
            text-align: center;
        }
        
        .stat-value {
            font-size: 32px;
            font-weight: bold;
            color: #0066CC;
        }
        
        .stat-label {
            font-size: 14px;
            color: #666;
            margin-top: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <!-- Header -->
        <div class="header">
            <div class="header-content">
                <div class="brand">
                    <span class="moki">MOKI</span> <span class="points">POINTS</span>
                </div>
                <div class="nav-links">
                    <a href="Dashboard.aspx">Dashboard</a>
                    <a href="Settings.aspx">Settings</a>
                </div>
            </div>
        </div>

        <!-- Main Content -->
        <div class="container">
            <h1 class="page-title">Achievements</h1>
            <p class="page-subtitle">Track your progress and unlock achievements</p>

            <!-- Stats Section -->
            <div class="stats-section">
                <div class="stat-item">
                    <div class="stat-value" id="statTotal" runat="server">0</div>
                    <div class="stat-label">Total Achievements</div>
                </div>
                <div class="stat-item">
                    <div class="stat-value" id="statEarned" runat="server">0</div>
                    <div class="stat-label">Earned</div>
                </div>
                <div class="stat-item">
                    <div class="stat-value" id="statProgress" runat="server">0%</div>
                    <div class="stat-label">Progress</div>
                </div>
            </div>

            <!-- Filters -->
            <div class="filters-section">
                <div class="filters-title">Filter Achievements</div>
                <div class="filter-buttons">
                    <button type="button" class="filter-btn active" data-filter="all">All</button>
                    <button type="button" class="filter-btn" data-filter="earned">Earned</button>
                    <button type="button" class="filter-btn" data-filter="unearned">Unearned</button>
                    <button type="button" class="filter-btn" data-filter="common">Common</button>
                    <button type="button" class="filter-btn" data-filter="uncommon">Uncommon</button>
                    <button type="button" class="filter-btn" data-filter="rare">Rare</button>
                    <button type="button" class="filter-btn" data-filter="epic">Epic</button>
                    <button type="button" class="filter-btn" data-filter="legendary">Legendary</button>
                    <button type="button" class="filter-btn" data-filter="mythical">Mythical</button>
                </div>
            </div>

            <!-- Achievements Grid -->
            <asp:Panel ID="pnlAchievements" runat="server">
                <div class="achievements-grid">
                    <asp:Repeater ID="rptAchievements" runat="server" OnItemDataBound="rptAchievements_ItemDataBound">
                        <ItemTemplate>
                            <div class="achievement-card <%# GetRarityClass(Eval("Rarity").ToString()) %> <%# Convert.ToBoolean(Eval("IsEarned")) ? "earned" : "unearned" %>"
                                 data-achievement-id='<%# Eval("Id") %>'
                                 data-earned='<%# Convert.ToBoolean(Eval("IsEarned")) ? "true" : "false" %>'
                                 data-rarity='<%# Eval("Rarity").ToString().ToLower() %>'
                                 onclick='showAchievementDetails(<%# Eval("Id") %>)'>
                                <div class="achievement-badge">
                                    <img src='<%# Eval("BadgeImagePath") %>' alt='<%# Eval("Name") %>' />
                                </div>
                                <div class="achievement-name"><%# Eval("Name") %></div>
                                <div class="achievement-rarity"><%# Eval("Rarity") %></div>
                                <div class="achievement-description"><%# Eval("Description") %></div>
                                
                                <asp:Panel ID="pnlProgress" runat="server" CssClass="achievement-progress" Visible="false">
                                    <div class="progress-bar-container">
                                        <div class="progress-bar" id="progressBar" runat="server"></div>
                                    </div>
                                    <div class="progress-text">
                                        <asp:Literal ID="litProgressText" runat="server"></asp:Literal>
                                    </div>
                                </asp:Panel>
                                
                                <asp:Panel ID="pnlEarned" runat="server" CssClass="achievement-earned-date" Visible="false">
                                    <asp:Literal ID="litEarnedDate" runat="server"></asp:Literal>
                                </asp:Panel>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnlNoAchievements" runat="server" Visible="false">
                <div class="no-achievements">
                    <div class="no-achievements-icon">&#127941;</div>
                    <div>No achievements found</div>
                </div>
            </asp:Panel>
        </div>

        <!-- Achievement Detail Modal -->
        <div id="achievementModal" class="modal-overlay" onclick="closeModalOnBackdrop(event)">
            <div class="modal-content" onclick="event.stopPropagation();">
                <div class="modal-close" onclick="closeAchievementModal()">&times;</div>
                <div id="modalBadge" class="modal-badge"></div>
                <div id="modalName" class="modal-name"></div>
                <div id="modalRarity" class="modal-rarity"></div>
                <div id="modalDescription" class="modal-description"></div>
                
                <div class="modal-section">
                    <div class="modal-section-title">How to Achieve</div>
                    <div id="modalHowToAchieve" class="modal-section-content"></div>
                </div>
                
                <div class="modal-section">
                    <div class="modal-section-title">Status</div>
                    <div id="modalStatus" class="modal-status"></div>
                </div>
                
                <div class="modal-section">
                    <div class="modal-section-title">&#128172; Message from Developers</div>
                    <div id="modalDeveloperMessage" class="modal-section-content modal-developer-message"></div>
                </div>
            </div>
        </div>

        <asp:HiddenField ID="hdnAchievementsData" runat="server" />
    </form>

    <script>
        // Filter functionality
        document.querySelectorAll('.filter-btn').forEach(btn => {
            btn.addEventListener('click', function() {
                // Update active state
                document.querySelectorAll('.filter-btn').forEach(b => b.classList.remove('active'));
                this.classList.add('active');
                
                const filter = this.getAttribute('data-filter');
                filterAchievements(filter);
            });
        });

        function filterAchievements(filter) {
            const cards = document.querySelectorAll('.achievement-card');
            cards.forEach(card => {
                let show = false;
                
                if (filter === 'all') {
                    show = true;
                } else if (filter === 'earned') {
                    show = card.getAttribute('data-earned') === 'true';
                } else if (filter === 'unearned') {
                    show = card.getAttribute('data-earned') === 'false';
                } else {
                    show = card.getAttribute('data-rarity') === filter.toLowerCase();
                }
                
                card.style.display = show ? 'block' : 'none';
            });
        }

        // Achievement detail modal
        function showAchievementDetails(achievementId) {
            const dataStr = document.getElementById('<%= hdnAchievementsData.ClientID %>').value;
            if (!dataStr) return;
            
            try {
                const achievements = JSON.parse(dataStr);
                const achievement = achievements.find(a => a.Id === achievementId);
                if (!achievement) return;
                
                // Populate modal
                document.getElementById('modalBadge').innerHTML = '<img src="' + achievement.BadgeImagePath + '" alt="' + achievement.Name + '" />';
                document.getElementById('modalName').textContent = achievement.Name;
                document.getElementById('modalRarity').innerHTML = '<span class="achievement-rarity" style="background-color: ' + getRarityColor(achievement.Rarity) + '; color: white; padding: 4px 12px; border-radius: 12px; font-size: 12px; font-weight: bold; text-transform: uppercase;">' + achievement.Rarity + '</span>';
                document.getElementById('modalDescription').textContent = achievement.Description;
                document.getElementById('modalHowToAchieve').textContent = achievement.HowToAchieve || 'Complete the requirements to unlock this achievement.';
                
                // Status
                const statusDiv = document.getElementById('modalStatus');
                if (achievement.IsEarned) {
                    statusDiv.className = 'modal-status earned';
                    statusDiv.innerHTML = '\u2713 Earned on ' + (achievement.EarnedDate ? new Date(achievement.EarnedDate).toLocaleDateString() : 'Unknown');
                } else {
                    statusDiv.className = 'modal-status unearned';
                    if (achievement.CurrentProgress !== undefined && achievement.TargetProgress !== undefined) {
                        const percentage = achievement.TargetProgress > 0 ? Math.round((achievement.CurrentProgress / achievement.TargetProgress) * 100) : 0;
                        statusDiv.innerHTML = '\u23F3 Not yet earned<br>Progress: ' + achievement.CurrentProgress + ' / ' + achievement.TargetProgress + ' (' + percentage + '%)';
                    } else {
                        statusDiv.innerHTML = '\u23F3 Not yet earned';
                    }
                }
                
                // Developer message
                document.getElementById('modalDeveloperMessage').textContent = achievement.DeveloperMessage || '[Developer message placeholder - to be customized per achievement]';
                
                // Show modal
                document.getElementById('achievementModal').classList.add('show');
            } catch (e) {
                console.error('Error showing achievement details:', e);
            }
        }

        function closeAchievementModal() {
            document.getElementById('achievementModal').classList.remove('show');
        }

        function closeModalOnBackdrop(event) {
            if (event.target.id === 'achievementModal') {
                closeAchievementModal();
            }
        }

        function getRarityColor(rarity) {
            const colors = {
                'Common': '#9E9E9E',
                'Uncommon': '#4CAF50',
                'Rare': '#2196F3',
                'Epic': '#9C27B0',
                'Legendary': '#FF9800',
                'Mythical': '#F44336'
            };
            return colors[rarity] || '#9E9E9E';
        }

        // Close modal on ESC key
        document.addEventListener('keydown', function(e) {
            if (e.key === 'Escape') {
                closeAchievementModal();
            }
        });
    </script>
</body>
</html>

