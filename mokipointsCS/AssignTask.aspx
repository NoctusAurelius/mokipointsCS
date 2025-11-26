<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssignTask.aspx.cs" Inherits="mokipointsCS.AssignTask" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assign Task - MOKI POINTS</title>
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
            max-width: 800px;
            margin: 0 auto;
            padding: 0 30px 30px;
        }
        
        .page-title {
            font-size: 32px;
            color: #333;
            margin-bottom: 30px;
        }
        
        /* Task Info Card */
        .task-info-card {
            background-color: white;
            border-radius: 10px;
            padding: 25px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
            margin-bottom: 30px;
        }
        
        .task-info-title {
            font-size: 24px;
            font-weight: bold;
            color: #333;
            margin-bottom: 15px;
        }
        
        .task-info-item {
            margin-bottom: 12px;
            display: flex;
            align-items: start;
        }
        
        .task-info-label {
            font-weight: 500;
            color: #666;
            min-width: 120px;
        }
        
        .task-info-value {
            color: #333;
            flex: 1;
        }
        
        /* Form */
        .form-container {
            background-color: white;
            border-radius: 10px;
            padding: 30px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
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
        
        .form-row {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 15px;
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
            text-decoration: none;
            display: inline-block;
            text-align: center;
        }
        
        .btn-cancel:hover {
            background-color: #555;
        }
        
        .message {
            padding: 15px;
            border-radius: 5px;
            margin-bottom: 20px;
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
        
        .error-message {
            color: #d32f2f;
            font-size: 12px;
            margin-top: 5px;
            display: block;
        }
        
        .form-control.error {
            border-color: #d32f2f;
            border-width: 2px;
        }
        
        .validation-error-message {
            background-color: #ffebee;
            color: #d32f2f;
            padding: 12px 16px;
            border-radius: 5px;
            border-left: 4px solid #d32f2f;
            margin-top: 10px;
            margin-bottom: 10px;
            font-size: 14px;
            animation: slideDown 0.3s ease-out;
            display: flex;
            align-items: center;
            gap: 8px;
        }
        
        .validation-error-message::before {
            content: "⚠️";
            font-size: 18px;
        }
        
        @keyframes slideDown {
            from {
                opacity: 0;
                transform: translateY(-10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
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
                        <a href="ParentDashboard.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Dashboard</a>
                        <a href="Family.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Family</a>
                        <a href="Tasks.aspx" class="active" style="color: #0066CC; text-decoration: none; font-weight: 500; font-size: 16px;">Tasks</a>
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
            <h1 class="page-title">Assign Task</h1>

            <!-- Messages -->
            <asp:Panel ID="pnlSuccess" runat="server" CssClass="message success" Visible="false">
                <asp:Label ID="lblSuccess" runat="server"></asp:Label>
            </asp:Panel>
            <asp:Panel ID="pnlError" runat="server" CssClass="message error" Visible="false">
                <asp:Label ID="lblError" runat="server"></asp:Label>
            </asp:Panel>

            <!-- Task Information -->
            <div class="task-info-card">
                <div class="task-info-title">
                    <asp:Literal ID="litTaskTitle" runat="server"></asp:Literal>
                </div>
                <div class="task-info-item">
                    <span class="task-info-label">Description:</span>
                    <span class="task-info-value"><asp:Literal ID="litTaskDescription" runat="server"></asp:Literal></span>
                </div>
                <div class="task-info-item">
                    <span class="task-info-label">Category:</span>
                    <span class="task-info-value"><asp:Literal ID="litTaskCategory" runat="server"></asp:Literal></span>
                </div>
                <div class="task-info-item">
                    <span class="task-info-label">Points Reward:</span>
                    <span class="task-info-value" style="color: #FF6600; font-weight: bold; font-size: 18px;"><asp:Literal ID="litTaskPoints" runat="server"></asp:Literal> points</span>
                </div>
                <div class="task-info-item">
                    <span class="task-info-label">Created:</span>
                    <span class="task-info-value"><asp:Literal ID="litTaskCreatedDate" runat="server"></asp:Literal></span>
                </div>
            </div>

            <!-- Assignment Form -->
            <div class="form-container">
                <div class="form-group">
                    <label>Select Child <span class="required">*</span></label>
                    <asp:DropDownList ID="ddlChild" runat="server" CssClass="form-control">
                        <asp:ListItem Value="">-- Select a child --</asp:ListItem>
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="rfvChild" runat="server" ControlToValidate="ddlChild" 
                        ErrorMessage="Please select a child" CssClass="error-message" Display="Dynamic" ValidationGroup="AssignTask" />
                </div>

                <div class="form-row">
                    <div class="form-group">
                        <label>Deadline Date (Optional)</label>
                        <asp:TextBox ID="txtDeadlineDate" runat="server" CssClass="form-control" TextMode="Date" />
                        <div id="deadlineDateError" class="error-message" style="display: none;"></div>
                    </div>
                    <div class="form-group">
                        <label>Deadline Time (Optional)</label>
                        <asp:TextBox ID="txtDeadlineTime" runat="server" CssClass="form-control" TextMode="Time" />
                        <div id="deadlineTimeError" class="error-message" style="display: none;"></div>
                    </div>
                </div>
                <div id="deadlineValidationError" class="validation-error-message" style="display: none;"></div>

                <asp:Button ID="btnAssignTask" runat="server" Text="Assign Task" CssClass="btn-submit" 
                    ValidationGroup="AssignTask" OnClick="btnAssignTask_Click" OnClientClick="return validateDeadline();" />
                <a href="Tasks.aspx" class="btn-cancel">Cancel</a>
            </div>
        </div>
    </form>
    
    <script type="text/javascript">
        function validateDeadline() {
            var dateInput = document.getElementById('<%= txtDeadlineDate.ClientID %>');
            var timeInput = document.getElementById('<%= txtDeadlineTime.ClientID %>');
            var dateError = document.getElementById('deadlineDateError');
            var timeError = document.getElementById('deadlineTimeError');
            var validationError = document.getElementById('deadlineValidationError');
            
            // Clear previous errors
            if (dateError) dateError.style.display = 'none';
            if (timeError) timeError.style.display = 'none';
            if (validationError) validationError.style.display = 'none';
            
            // Remove error styling
            if (dateInput) dateInput.classList.remove('error');
            if (timeInput) timeInput.classList.remove('error');
            
            if (!dateInput || !dateInput.value) {
                return true; // Deadline is optional
            }
            
            var deadlineDate = new Date(dateInput.value);
            var deadlineTime = timeInput && timeInput.value ? timeInput.value.split(':') : null;
            
            if (deadlineTime) {
                deadlineDate.setHours(parseInt(deadlineTime[0]), parseInt(deadlineTime[1]), 0, 0);
            } else {
                // If no time specified, set to end of day (23:59)
                deadlineDate.setHours(23, 59, 0, 0);
            }
            
            var now = new Date();
            var minDeadline = new Date(now.getTime() + (10 * 60 * 1000)); // 10 minutes from now
            
            if (deadlineDate <= now) {
                showDeadlineError('Deadline must be in the future. Please select a date/time that has not passed.', dateInput, timeInput, dateError, timeError, validationError);
                return false;
            }
            
            if (deadlineDate < minDeadline) {
                var minTimeStr = minDeadline.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
                showDeadlineError('Deadline must be at least 10 minutes in the future. The earliest deadline is ' + minTimeStr + '.', dateInput, timeInput, dateError, timeError, validationError);
                return false;
            }
            
            return true;
        }
        
        function showDeadlineError(message, dateInput, timeInput, dateError, timeError, validationError) {
            // Add error styling to inputs
            if (dateInput) dateInput.classList.add('error');
            if (timeInput) timeInput.classList.add('error');
            
            // Show validation error message
            if (validationError) {
                validationError.textContent = message;
                validationError.style.display = 'flex';
                // Auto-hide after 5 seconds
                setTimeout(function() {
                    validationError.style.display = 'none';
                }, 5000);
            }
        }
        
        // Set minimum date on page load
        window.onload = function() {
            var dateInput = document.getElementById('<%= txtDeadlineDate.ClientID %>');
            if (dateInput) {
                var today = new Date();
                var minDate = today.toISOString().split('T')[0];
                dateInput.setAttribute('min', minDate);
                
                // If today is selected, set minimum time to 10 minutes from now
                dateInput.addEventListener('change', function() {
                    var selectedDate = new Date(this.value);
                    var today = new Date();
                    if (selectedDate.toDateString() === today.toDateString()) {
                        var timeInput = document.getElementById('<%= txtDeadlineTime.ClientID %>');
                        if (timeInput) {
                            var minTime = new Date(today.getTime() + (10 * 60 * 1000));
                            var minTimeStr = minTime.toTimeString().substring(0, 5); // HH:mm format
                            timeInput.setAttribute('min', minTimeStr);
                        }
                    } else {
                        var timeInput = document.getElementById('<%= txtDeadlineTime.ClientID %>');
                        if (timeInput) {
                            timeInput.removeAttribute('min');
                        }
                    }
                });
            }
        };
    </script>
</body>
</html>

