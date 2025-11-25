using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class TaskReview : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check authentication
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            string userRole = Session["UserRole"]?.ToString() ?? "";

            // Only parents can review tasks
            if (userRole != "PARENT")
            {
                ShowError("Only parents can review tasks.");
                pnlTasks.Visible = false;
                return;
            }

            // Set user name - load from session or database
            if (Session["FirstName"] != null && Session["LastName"] != null)
            {
                litUserName.Text = Session["FirstName"].ToString() + " " + Session["LastName"].ToString();
            }
            else if (Session["FirstName"] != null)
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
                    string lastName = userInfo["LastName"].ToString();
                    Session["FirstName"] = firstName;
                    Session["LastName"] = lastName;
                    litUserName.Text = firstName + " " + lastName;
                }
            }

            // Get family ID
            int? familyId = FamilyHelper.GetUserFamilyId(userId);
            if (!familyId.HasValue)
            {
                ShowError("You are not part of a family.");
                pnlTasks.Visible = false;
                return;
            }

            if (!IsPostBack)
            {
                // Load profile picture
                LoadProfilePicture(userId);
                
                LoadTasksPendingReview(familyId.Value);
            }

            // Handle success/error messages from redirects
            if (Request.QueryString["success"] != null)
            {
                ShowSuccess(Request.QueryString["success"]);
            }
            if (Request.QueryString["error"] != null)
            {
                ShowError(Request.QueryString["error"]);
            }
        }

        protected void LoadTasksPendingReview(int familyId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(string.Format("LoadTasksPendingReview: Loading tasks for family {0}", familyId));
                DataTable tasks = TaskHelper.GetTasksPendingReview(familyId);

                if (tasks != null && tasks.Rows.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("LoadTasksPendingReview: Found {0} tasks pending review", tasks.Rows.Count));
                    rptTasks.DataSource = tasks;
                    rptTasks.DataBind();
                    pnlTasks.Visible = true;
                    pnlEmpty.Visible = false;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("LoadTasksPendingReview: No tasks pending review");
                    pnlTasks.Visible = false;
                    pnlEmpty.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("LoadTasksPendingReview ERROR: {0}\nStack Trace: {1}", ex.Message, ex.StackTrace));
                ShowError(string.Format("Error loading tasks pending review: {0}. Please try refreshing the page.", ex.Message));
                pnlTasks.Visible = false;
                pnlEmpty.Visible = true;
            }
        }

        protected void rptTasks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                int taskAssignmentId = Convert.ToInt32(e.CommandArgument);
                int reviewedBy = Convert.ToInt32(Session["UserId"]);

                System.Diagnostics.Debug.WriteLine(string.Format("TaskReview rptTasks_ItemCommand: CommandName={0}, AssignmentId={1}, ReviewedBy={2}", 
                    e.CommandName, taskAssignmentId, reviewedBy));

                if (e.CommandName == "Fail")
                {
                    System.Diagnostics.Debug.WriteLine("========================================");
                    System.Diagnostics.Debug.WriteLine("TaskReview: FAIL COMMAND RECEIVED");
                    System.Diagnostics.Debug.WriteLine(string.Format("AssignmentId: {0}, ReviewedBy: {1}", taskAssignmentId, reviewedBy));
                    
                    // Check assignment status before attempting to fail
                    string statusCheck = TaskHelper.GetTaskAssignmentStatus(taskAssignmentId);
                    System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Fail: Assignment {0} current status: {1}", taskAssignmentId, statusCheck ?? "NULL"));

                    if (string.IsNullOrEmpty(statusCheck))
                    {
                        string errorMsg = "Error: Task assignment not found. It may have already been reviewed or deleted.";
                        System.Diagnostics.Debug.WriteLine("TaskReview Fail: Assignment not found - " + errorMsg);
                        ShowError(errorMsg);
                        return;
                    }

                    if (statusCheck != "Pending Review")
                    {
                        string errorMsg = string.Format("Error: Task is not in 'Pending Review' status. Current status: {0}. The task may have already been reviewed.", statusCheck);
                        System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Fail: Invalid status - expected 'Pending Review', got '{0}' - {1}", statusCheck, errorMsg));
                        ShowError(errorMsg);
                        return;
                    }

                    // Fail the task immediately (rating = 0, isFailed = true)
                    System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Fail: Calling ReviewTask for assignment {0} with parameters: rating=0, isFailed=true, isAutoFailed=false", taskAssignmentId));
                    
                    try
                    {
                        bool success = TaskHelper.ReviewTask(taskAssignmentId, 0, reviewedBy, true, false);
                        System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Fail: ReviewTask returned: {0}", success));

                        if (success)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Fail: Successfully failed task assignment {0}", taskAssignmentId));
                            // Reload tasks
                            int? familyId = FamilyHelper.GetUserFamilyId(reviewedBy);
                            if (familyId.HasValue)
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Fail: Reloading tasks for family {0}", familyId.Value));
                                LoadTasksPendingReview(familyId.Value);
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("TaskReview Fail: WARNING - Could not get family ID for user " + reviewedBy);
                            }
                            ShowSuccess("Task marked as failed. Points deducted from child.");
                            System.Diagnostics.Debug.WriteLine("TaskReview Fail: SUCCESS - Task failed and page reloaded");
                        }
                        else
                        {
                            string errorMsg = "Failed to mark task as failed. The task may have already been reviewed or is not in 'Pending Review' status. Please refresh the page.";
                            System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Fail: ReviewTask returned false for assignment {0} - {1}", taskAssignmentId, errorMsg));
                            ShowError(errorMsg);
                        }
                    }
                    catch (Exception ex)
                    {
                        string errorMsg = string.Format("An error occurred while failing the task: {0}", ex.Message);
                        System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Fail: EXCEPTION - {0}\nStack Trace: {1}", errorMsg, ex.StackTrace));
                        ShowError(errorMsg);
                    }
                    
                    System.Diagnostics.Debug.WriteLine("========================================");
                }
                else if (e.CommandName == "Review")
                {
                    // Get rating from hidden field
                    HiddenField hidRating = (HiddenField)e.Item.FindControl("hidRating");
                    int rating = 0;
                    if (hidRating != null && !string.IsNullOrEmpty(hidRating.Value))
                    {
                        int.TryParse(hidRating.Value, out rating);
                    }

                    System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Review: Assignment {0}, Rating={1}", taskAssignmentId, rating));

                    // Validate - must have a rating (1-5 stars)
                    if (rating < 1 || rating > 5)
                    {
                        ShowError("Please select a rating (1-5 stars) before submitting.");
                        return;
                    }

                    // Check assignment status before attempting to review
                    string statusCheck = TaskHelper.GetTaskAssignmentStatus(taskAssignmentId);
                    System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Review: Assignment {0} current status: {1}", taskAssignmentId, statusCheck ?? "NULL"));

                    if (string.IsNullOrEmpty(statusCheck))
                    {
                        ShowError("Error: Task assignment not found. It may have already been reviewed or deleted.");
                        System.Diagnostics.Debug.WriteLine("TaskReview Review: Assignment not found");
                        return;
                    }

                    if (statusCheck != "Pending Review")
                    {
                        ShowError(string.Format("Error: Task is not in 'Pending Review' status. Current status: {0}. The task may have already been reviewed.", statusCheck));
                        System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Review: Invalid status - expected 'Pending Review', got '{0}'", statusCheck));
                        return;
                    }

                    // Review task with rating (isFailed = false)
                    System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Review: Calling ReviewTask for assignment {0} with rating {1}", taskAssignmentId, rating));
                    bool success = TaskHelper.ReviewTask(taskAssignmentId, rating, reviewedBy, false, false);

                    if (success)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Review: Successfully reviewed task assignment {0}", taskAssignmentId));
                        // Reload tasks
                        int? familyId = FamilyHelper.GetUserFamilyId(reviewedBy);
                        if (familyId.HasValue)
                        {
                            LoadTasksPendingReview(familyId.Value);
                        }
                        ShowSuccess("Task reviewed successfully! Points awarded to child.");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("TaskReview Review: ReviewTask returned false for assignment {0}", taskAssignmentId));
                        ShowError("Failed to review task. The task may have already been reviewed or is not in 'Pending Review' status. Please refresh the page.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("TaskReview rptTasks_ItemCommand ERROR: {0}\nStack Trace: {1}", ex.Message, ex.StackTrace));
                ShowError(string.Format("An error occurred: {0}. Please try again or refresh the page.", ex.Message));
            }
        }

        protected void rptTasks_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                int taskId = Convert.ToInt32(row["TaskId"]);
                int assignmentId = Convert.ToInt32(row["AssignmentId"]);

                // Load objectives for this task
                Repeater rptObjectives = (Repeater)e.Item.FindControl("rptObjectives");
                if (rptObjectives != null)
                {
                    DataTable objectives = TaskHelper.GetTaskObjectives(taskId);
                    
                    // Get completion status for each objective
                    if (objectives != null && objectives.Rows.Count > 0)
                    {
                        DataTable objectivesWithCompletion = objectives.Clone();
                        objectivesWithCompletion.Columns.Add("IsCompleted", typeof(bool));

                        foreach (DataRow objRow in objectives.Rows)
                        {
                            int objectiveId = Convert.ToInt32(objRow["Id"]);
                            bool isCompleted = TaskHelper.GetObjectiveCompletionStatus(assignmentId, objectiveId);

                            DataRow newRow = objectivesWithCompletion.NewRow();
                            newRow.ItemArray = objRow.ItemArray;
                            newRow["IsCompleted"] = isCompleted;
                            objectivesWithCompletion.Rows.Add(newRow);
                        }

                        rptObjectives.DataSource = objectivesWithCompletion;
                        rptObjectives.DataBind();
                    }
                }
            }
        }

        protected void rptObjectives_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Objectives are already bound with completion status
            // No additional processing needed
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }

        private void ShowSuccess(string message)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(string.Format("TaskReview ShowSuccess: {0}", message));
                lblSuccess.Text = message;
                pnlSuccess.CssClass = "message success-message";
                pnlSuccess.Visible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("TaskReview ShowSuccess ERROR: {0}", ex.Message));
            }
        }

        private void ShowError(string message)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(string.Format("TaskReview ShowError: {0}", message));
                lblError.Text = message;
                pnlError.CssClass = "message error-message";
                pnlError.Visible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("TaskReview ShowError ERROR: {0}", ex.Message));
            }
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
                        string.IsNullOrEmpty(initials) ? "P" : initials.ToUpper());
                    litProfilePlaceholder.Visible = true;
                    imgProfilePicture.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadProfilePicture error: " + ex.Message);
                // Show default placeholder on error
                litProfilePlaceholder.Text = "<div class=\"profile-avatar-placeholder\">P</div>";
                litProfilePlaceholder.Visible = true;
                imgProfilePicture.Visible = false;
            }
        }
    }
}

