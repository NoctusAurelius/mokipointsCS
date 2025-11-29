using System;
using System.Data;
using System.IO;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class ChildTasks : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Check authentication
                if (Session["UserId"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                // Check role
                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "CHILD")
                {
                    Response.Redirect("Dashboard.aspx");
                    return;
                }

                // Check family membership
                int userId = Convert.ToInt32(Session["UserId"]);
                int? familyId = FamilyHelper.GetUserFamilyId(userId);
                if (!familyId.HasValue)
                {
                    Response.Redirect("JoinFamily.aspx");
                    return;
                }

                // Set user name - load from session or database
                if (Session["FirstName"] != null)
                {
                    litUserName.Text = Session["FirstName"].ToString();
                }
                else
                {
                    // Fallback: Load from database if session is missing
                    var userInfo = AuthenticationHelper.GetUserById(userId);
                    if (userInfo != null)
                    {
                        string firstName = userInfo["FirstName"].ToString();
                        Session["FirstName"] = firstName;
                        Session["LastName"] = userInfo["LastName"].ToString();
                        litUserName.Text = firstName;
                    }
                }

                if (!IsPostBack)
                {
                    // Load profile picture
                    LoadProfilePicture(userId);
                    
                    // Check and auto-fail overdue tasks (Fix #3)
                    if (familyId.HasValue)
                    {
                        TaskHelper.AutoFailOverdueTasks(familyId.Value);
                    }
                    
                    LoadTasks();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ChildTasks Page_Load error: " + ex.Message);
                ShowError("An error occurred. Please try again.");
            }
        }

        private void LoadTasks()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                DataTable tasks = TaskHelper.GetChildTasks(userId);

                if (tasks != null && tasks.Rows.Count > 0)
                {
                    // Verify required columns exist
                    if (!tasks.Columns.Contains("AssignmentId"))
                    {
                        throw new Exception("DataTable missing required column: AssignmentId");
                    }
                    
                    rptTasks.DataSource = tasks;
                    rptTasks.DataBind();
                    pnlTasks.Visible = true;
                    pnlEmpty.Visible = false;
                }
                else
                {
                    pnlTasks.Visible = false;
                    pnlEmpty.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadTasks error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("LoadTasks stack trace: " + ex.StackTrace);
                ShowError("Failed to load tasks: " + ex.Message);
            }
        }

        protected void rptTasks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                
                if (e.CommandArgument == null || string.IsNullOrEmpty(e.CommandArgument.ToString()))
                {
                    throw new Exception("CommandArgument is null or empty");
                }
                
                int assignmentId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName)
                {
                    case "Accept":
                        // Check if task was auto-failed (by checking if it still exists and is in "Assigned" status)
                        string currentStatus = TaskHelper.GetTaskAssignmentStatus(assignmentId);
                        if (string.IsNullOrEmpty(currentStatus))
                        {
                            // Task was auto-failed (no longer exists or was reviewed)
                            ShowError("This task is overdue and has been automatically failed. Points have been deducted.");
                            LoadTasks(); // Reload to show updated task list
                        }
                        else if (TaskHelper.AcceptTask(assignmentId, userId))
                        {
                            ShowSuccess("Task accepted!");
                            LoadTasks();
                        }
                        else
                        {
                            ShowError("Failed to accept task. The task may be overdue and has been automatically failed.");
                            LoadTasks(); // Reload to check if task was auto-failed
                        }
                        break;

                    case "Decline":
                        // Check if task was auto-failed
                        currentStatus = TaskHelper.GetTaskAssignmentStatus(assignmentId);
                        if (string.IsNullOrEmpty(currentStatus))
                        {
                            // Task was auto-failed
                            ShowError("This task is overdue and has been automatically failed. Points have been deducted.");
                            LoadTasks();
                        }
                        else if (TaskHelper.DenyTask(assignmentId, userId))
                        {
                            ShowSuccess("Task declined.");
                            LoadTasks();
                        }
                        else
                        {
                            ShowError("Failed to decline task. The task may be overdue and has been automatically failed.");
                            LoadTasks();
                        }
                        break;

                    case "Submit":
                        // Fix #1: Server-side validation - check all objectives completed
                        if (TaskHelper.AreAllObjectivesCompleted(assignmentId))
                        {
                            // Check if task was auto-failed (by checking if it still exists and is in "Ongoing" status)
                            currentStatus = TaskHelper.GetTaskAssignmentStatus(assignmentId);
                            if (string.IsNullOrEmpty(currentStatus))
                            {
                                // Task was auto-failed (no longer exists or was reviewed)
                                ShowError("This task is overdue and has been automatically failed. Points have been deducted and the task has been moved to history.");
                                LoadTasks(); // Reload to show updated task list
                            }
                            else if (TaskHelper.SubmitTaskForReview(assignmentId, userId))
                            {
                                ShowSuccess("Task submitted for review!");
                                LoadTasks();
                            }
                            else
                            {
                                ShowError("Failed to submit task. The task may be overdue and has been automatically failed. Points have been deducted.");
                                LoadTasks(); // Reload to check if task was auto-failed
                            }
                        }
                        else
                        {
                            ShowError("Please complete all objectives before submitting.");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("rptTasks_ItemCommand error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("rptTasks_ItemCommand stack trace: " + ex.StackTrace);
                ShowError("An error occurred: " + ex.Message);
            }
        }

        protected void rptTasks_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
                {
                    DataRowView row = (DataRowView)e.Item.DataItem;
                    
                    // Verify required columns exist
                    if (!row.Row.Table.Columns.Contains("AssignmentId"))
                    {
                        throw new Exception("DataRowView missing required column: AssignmentId");
                    }
                    if (!row.Row.Table.Columns.Contains("TaskId"))
                    {
                        throw new Exception("DataRowView missing required column: TaskId");
                    }
                    if (!row.Row.Table.Columns.Contains("Status"))
                    {
                        throw new Exception("DataRowView missing required column: Status");
                    }
                    
                    int assignmentId = Convert.ToInt32(row["AssignmentId"]);
                    int taskId = Convert.ToInt32(row["TaskId"]);
                    string status = row["Status"].ToString();

                Panel pnlObjectives = (Panel)e.Item.FindControl("pnlObjectives");
                Repeater rptObjectives = (Repeater)e.Item.FindControl("rptObjectives");
                Button btnSubmit = (Button)e.Item.FindControl("btnSubmit");
                Panel pnlDeadlineWarning = (Panel)e.Item.FindControl("pnlDeadlineWarning");
                Literal litDeadlineWarning = (Literal)e.Item.FindControl("litDeadlineWarning");
                Panel pnlTimer = (Panel)e.Item.FindControl("pnlTimer"); // Issue #8: Timer display

                // Load objectives for "Ongoing" tasks (Fix #1: Server-side tracking)
                if (status == "Ongoing" && pnlObjectives != null && rptObjectives != null)
                {
                    // Get objective completions
                    DataTable completions = TaskHelper.GetObjectiveCompletions(assignmentId);
                    
                    if (completions.Rows.Count > 0)
                    {
                        // Add AssignmentId column to completions DataTable for nested repeater access
                        if (!completions.Columns.Contains("AssignmentId"))
                        {
                            completions.Columns.Add("AssignmentId", typeof(int));
                        }
                        foreach (DataRow completionRow in completions.Rows)
                        {
                            completionRow["AssignmentId"] = assignmentId;
                        }
                        
                        rptObjectives.DataSource = completions;
                        rptObjectives.DataBind();
                        pnlObjectives.Visible = true;

                        // Check if all completed and enable submit button
                        if (btnSubmit != null)
                        {
                            bool allCompleted = TaskHelper.AreAllObjectivesCompleted(assignmentId);
                            btnSubmit.Enabled = allCompleted;
                        }

                        // Calculate and set initial progress
                        int completed = 0;
                        int total = completions.Rows.Count;
                        foreach (DataRow comp in completions.Rows)
                        {
                            if (Convert.ToBoolean(comp["IsCompleted"]))
                                completed++;
                        }
                        double percentage = total > 0 ? (completed / (double)total) * 100 : 0;

                        // Inject progress update script
                        string progressScript = string.Format(
                            "setTimeout(function() {{ var container = document.querySelector('[data-assignment-id=\"{0}\"]'); " +
                            "if (container) {{ var fill = container.querySelector('.progress-fill'); " +
                            "var text = container.querySelector('.progress-text'); " +
                            "if (fill) fill.style.width = '{1}%'; " +
                            "if (text) text.textContent = '{2} of {3} objectives completed'; }} }}, 100);",
                            assignmentId, percentage, completed, total);
                        ScriptManager.RegisterStartupScript(this, GetType(), "Progress_" + assignmentId, progressScript, true);
                    }
                }

                // Deadline warning (Fix #15)
                if (pnlDeadlineWarning != null && litDeadlineWarning != null && row["Deadline"] != DBNull.Value)
                {
                    DateTime deadline = Convert.ToDateTime(row["Deadline"]);
                    DateTime now = DateTime.Now;
                    TimeSpan timeRemaining = deadline - now;

                    if (timeRemaining.TotalDays < 0)
                    {
                        // Overdue
                        pnlDeadlineWarning.CssClass = "deadline-warning red";
                        litDeadlineWarning.Text = "&#9888; This task is overdue!";
                        pnlDeadlineWarning.Visible = true;
                    }
                    else if (timeRemaining.TotalDays < 1)
                    {
                        // Less than 1 day
                        int hours = (int)timeRemaining.TotalHours;
                        int minutes = timeRemaining.Minutes;
                        int totalMinutes = (int)timeRemaining.TotalMinutes;
                        
                        string timeText = "";
                        if (hours > 0)
                        {
                            // More than 1 hour - show hours and minutes
                            pnlDeadlineWarning.CssClass = "deadline-warning orange";
                            timeText = hours == 1 ? "1 hour" : string.Format("{0} hours", hours);
                            if (minutes > 0 && hours < 12) // Show minutes if less than 12 hours
                            {
                                timeText += minutes == 1 ? " 1 minute" : string.Format(" {0} minutes", minutes);
                            }
                        }
                        else
                        {
                            // Less than 1 hour - show minutes countdown (CRITICAL)
                            pnlDeadlineWarning.CssClass = "deadline-warning red"; // Red for urgency
                            timeText = totalMinutes == 1 ? "1 minute" : string.Format("{0} minutes", totalMinutes);
                        }
                        
                        // Include deadline time for reference
                        string deadlineTime = deadline.ToString("h:mm tt");
                        litDeadlineWarning.Text = string.Format(
                            "<span class='deadline-icon'>&#9200;</span> " +
                            "<span class='deadline-text'>Deadline in <strong>{0}</strong> ({1})</span>", 
                            timeText, deadlineTime);
                        
                        // Add data attributes for JavaScript countdown updates
                        pnlDeadlineWarning.Attributes["data-deadline"] = deadline.ToString("yyyy-MM-ddTHH:mm:ss");
                        pnlDeadlineWarning.Attributes["data-total-minutes"] = totalMinutes.ToString();
                        
                        pnlDeadlineWarning.Visible = true;
                    }
                    else if (timeRemaining.TotalDays < 2)
                    {
                        // 1-2 days
                        pnlDeadlineWarning.CssClass = "deadline-warning yellow";
                        int days = (int)timeRemaining.TotalDays;
                        litDeadlineWarning.Text = string.Format("&#9888; Deadline in {0} day(s)!", days);
                        pnlDeadlineWarning.Visible = true;
                    }
                    else
                    {
                        // More than 2 days
                        pnlDeadlineWarning.CssClass = "deadline-warning green";
                        int days = (int)timeRemaining.TotalDays;
                        litDeadlineWarning.Text = string.Format("✓ {0} day(s) remaining", days);
                        pnlDeadlineWarning.Visible = true;
                    }
                }
                
                // Issue #8: Show timer for Ongoing tasks that have a timer
                if (pnlTimer != null && status == "Ongoing")
                {
                    // Check if task has timer (TimerStart and TimerDuration)
                    if (row["TimerStart"] != DBNull.Value && row["TimerDuration"] != DBNull.Value)
                    {
                        pnlTimer.Visible = true;
                        // Timer data attributes are already set in the ASPX template
                    }
                    else
                    {
                        pnlTimer.Visible = false;
                    }
                }
                else if (pnlTimer != null)
                {
                    pnlTimer.Visible = false;
                }
                } // Close if (e.Item.ItemType...)
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("rptTasks_ItemDataBound error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("rptTasks_ItemDataBound stack trace: " + ex.StackTrace);
                // Don't throw - just log the error to prevent page crash
            }
        }

        // Fix #1: Web method for AJAX objective updates (server-side tracking)
        [System.Web.Services.WebMethod]
        public static object UpdateObjective(int assignmentId, int objectiveId, bool isCompleted)
        {
            try
            {
                // Get user ID from session (need to access HttpContext)
                int userId = 0;
                if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session["UserId"] != null)
                {
                    userId = Convert.ToInt32(System.Web.HttpContext.Current.Session["UserId"]);
                }
                else
                {
                    return new { Success = false, Message = "User not authenticated" };
                }

                bool success;
                if (isCompleted)
                {
                    success = TaskHelper.MarkObjectiveComplete(assignmentId, objectiveId, userId);
                }
                else
                {
                    success = TaskHelper.UnmarkObjectiveComplete(assignmentId, objectiveId, userId);
                }

                return new { Success = success, Message = success ? "Updated" : "Failed" };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("UpdateObjective error: " + ex.Message);
                return new { Success = false, Message = ex.Message };
            }
        }

        private void ShowSuccess(string message)
        {
            lblSuccess.Text = message;
            pnlSuccess.Visible = true;
            pnlError.Visible = false;
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            pnlError.Visible = true;
            pnlSuccess.Visible = false;
        }

        protected void LoadProfilePicture(int userId)
        {
            try
            {
                var userInfo = AuthenticationHelper.GetUserById(userId);
                if (userInfo != null)
                {
                    string firstName = userInfo["FirstName"].ToString();
                    string lastName = userInfo["LastName"].ToString();
                    string initials = (firstName.Length > 0 ? firstName[0].ToString() : "") + (lastName.Length > 0 ? lastName[0].ToString() : "");
                    
                    // Check if ProfilePicture column exists
                    string profilePicture = null;
                    if (userInfo.Table.Columns.Contains("ProfilePicture"))
                    {
                        profilePicture = (userInfo["ProfilePicture"] != null && userInfo["ProfilePicture"] != DBNull.Value) ? userInfo["ProfilePicture"].ToString() : null;
                    }
                    
                    // Load profile picture if exists
                    if (!string.IsNullOrEmpty(profilePicture))
                    {
                        string picturePath = Server.MapPath("~/Images/ProfilePictures/" + profilePicture);
                        if (File.Exists(picturePath))
                        {
                            imgProfilePicture.ImageUrl = "~/Images/ProfilePictures/" + profilePicture;
                            imgProfilePicture.Visible = true;
                            litProfilePlaceholder.Visible = false;
                            return;
                        }
                    }
                    
                    // Show placeholder with initials
                    litProfilePlaceholder.Text = string.Format(
                        "<div class=\"profile-avatar-placeholder\">{0}</div>",
                        string.IsNullOrEmpty(initials) ? "C" : initials.ToUpper());
                    litProfilePlaceholder.Visible = true;
                    imgProfilePicture.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadProfilePicture error: " + ex.Message);
                // Show default placeholder on error
                litProfilePlaceholder.Text = "<div class=\"profile-avatar-placeholder\">C</div>";
                litProfilePlaceholder.Visible = true;
                imgProfilePicture.Visible = false;
            }
        }
    }
}

