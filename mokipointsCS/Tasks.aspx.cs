using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class Tasks : System.Web.UI.Page
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

                // Check role - only parents can access this page
                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "PARENT")
                {
                    Response.Redirect("Dashboard.aspx");
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);

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

                if (!IsPostBack)
                {
                    // Load profile picture
                    LoadProfilePicture(userId);
                    
                    // Check for success message from assignment
                    if (Request.QueryString["assigned"] == "true")
                    {
                        ShowSuccess("Task assigned successfully!");
                    }
                    
                    LoadTasks();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Tasks Page_Load error: " + ex.Message);
                ShowError("An error occurred. Please try again.");
            }
        }

        private void LoadTasks()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int? familyId = FamilyHelper.GetUserFamilyId(userId);

                if (!familyId.HasValue)
                {
                    Response.Redirect("Family.aspx");
                    return;
                }

                DataTable tasks = TaskHelper.GetFamilyTasks(familyId.Value);

                if (tasks.Rows.Count > 0)
                {
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
                ShowError("Failed to load tasks.");
            }
        }

        protected void rptTasks_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                int taskId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName)
                {
                    case "Assign":
                        Response.Redirect("AssignTask.aspx?taskId=" + taskId);
                        break;

                    case "Edit":
                        LoadTaskForEdit(taskId);
                        break;

                    case "View":
                        LoadTaskForView(taskId);
                        break;

                    case "Delete":
                        DeleteTask(taskId);
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("rptTasks_ItemCommand error: " + ex.Message);
                ShowError("An error occurred.");
            }
        }

        protected void rptTasks_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                int taskId = Convert.ToInt32(row["Id"]);

                // Load assignments for this task
                Panel pnlAssignments = (Panel)e.Item.FindControl("pnlAssignments");
                Repeater rptAssignments = (Repeater)e.Item.FindControl("rptAssignments");
                Button btnEdit = (Button)e.Item.FindControl("btnEdit");
                Button btnDelete = (Button)e.Item.FindControl("btnDelete");

                if (pnlAssignments != null && rptAssignments != null)
                {
                    DataTable assignments = TaskHelper.GetTaskAssignments(taskId);
                    if (assignments.Rows.Count > 0)
                    {
                        rptAssignments.DataSource = assignments;
                        rptAssignments.DataBind();
                        pnlAssignments.Visible = true;

                        // Disable edit/delete if task has assignments
                        if (btnEdit != null) btnEdit.Enabled = false;
                        if (btnDelete != null) btnDelete.Enabled = false;
                    }
                    else
                    {
                        pnlAssignments.Visible = false;
                    }
                }

                // Load completion history (Fix #6)
                Panel pnlCompletionHistory = (Panel)e.Item.FindControl("pnlCompletionHistory");
                Literal litCompletionCount = (Literal)e.Item.FindControl("litCompletionCount");

                if (pnlCompletionHistory != null && litCompletionCount != null)
                {
                    DataTable history = TaskHelper.GetTaskCompletionHistory(taskId);
                    if (history.Rows.Count > 0)
                    {
                        litCompletionCount.Text = history.Rows.Count.ToString();
                        pnlCompletionHistory.Visible = true;
                    }
                    else
                    {
                        pnlCompletionHistory.Visible = false;
                    }
                }
            }
        }

        protected void btnCreateTaskSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                int? familyId = FamilyHelper.GetUserFamilyId(userId);

                if (!familyId.HasValue)
                {
                    ShowError("You must be in a family to create tasks.");
                    return;
                }

                // Collect objectives from form
                List<string> objectives = new List<string>();
                foreach (string key in Request.Form.AllKeys)
                {
                    if (key.StartsWith("objective_"))
                    {
                        string value = Request.Form[key];
                        if (!string.IsNullOrEmpty(value.Trim()))
                        {
                            objectives.Add(value.Trim());
                        }
                    }
                }

                if (objectives.Count == 0)
                {
                    ShowError("At least one objective is required.");
                    return;
                }

                // Get form values
                string title = txtCreateTitle.Text.Trim();
                string description = txtCreateDescription.Text.Trim();
                string category = ddlCreateCategory.SelectedValue;
                int points = Convert.ToInt32(txtCreatePoints.Text);
                string priority = ddlCreatePriority.SelectedValue;
                string difficulty = ddlCreateDifficulty.SelectedValue;
                int? estimatedMinutes = null;
                if (!string.IsNullOrEmpty(txtCreateEstimatedMinutes.Text))
                {
                    estimatedMinutes = Convert.ToInt32(txtCreateEstimatedMinutes.Text);
                }
                string instructions = txtCreateInstructions.Text.Trim();
                string recurrencePattern = ddlCreateRecurrence.SelectedValue;

                // Create task
                int taskId = TaskHelper.CreateTask(title, description, category, points, userId, familyId.Value, objectives,
                    priority, string.IsNullOrEmpty(difficulty) ? null : difficulty, estimatedMinutes, 
                    string.IsNullOrEmpty(instructions) ? null : instructions, 
                    string.IsNullOrEmpty(recurrencePattern) ? null : recurrencePattern);

                if (taskId > 0)
                {
                    ShowSuccess("Task created successfully!");
                    LoadTasks();
                    // Close modal via JavaScript
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseCreateModal", "closeCreateModal();", true);
                }
                else
                {
                    ShowError("Failed to create task. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnCreateTaskSubmit_Click error: " + ex.Message);
                ShowError("An error occurred while creating the task.");
            }
        }

        private void LoadTaskForEdit(int taskId)
        {
            try
            {
                // Check if task is assigned
                if (TaskHelper.IsTaskAssigned(taskId))
                {
                    ShowError("Cannot edit task that is assigned to children.");
                    return;
                }

                DataRow task = TaskHelper.GetTaskDetails(taskId);
                if (task == null)
                {
                    ShowError("Task not found.");
                    return;
                }

                hidEditTaskId.Value = taskId.ToString();
                txtEditTitle.Text = task["Title"].ToString();
                txtEditDescription.Text = task["Description"] != DBNull.Value ? task["Description"].ToString() : "";
                ddlEditCategory.SelectedValue = task["Category"].ToString();
                txtEditPoints.Text = task["PointsReward"].ToString();
                ddlEditPriority.SelectedValue = task["Priority"] != DBNull.Value ? task["Priority"].ToString() : "Medium";
                ddlEditDifficulty.SelectedValue = task["Difficulty"] != DBNull.Value ? task["Difficulty"].ToString() : "";
                txtEditEstimatedMinutes.Text = task["EstimatedMinutes"] != DBNull.Value ? task["EstimatedMinutes"].ToString() : "";
                txtEditInstructions.Text = task["Instructions"] != DBNull.Value ? task["Instructions"].ToString() : "";

                // Load objectives
                DataTable objectives = TaskHelper.GetTaskObjectives(taskId);
                string objectivesScript = "var container = document.getElementById('editObjectivesContainer'); container.innerHTML = '';";
                int index = 0;
                foreach (DataRow obj in objectives.Rows)
                {
                    string objText = obj["ObjectiveText"].ToString().Replace("'", "\\'").Replace("\"", "\\\"");
                    objectivesScript += string.Format("addObjective('editObjectivesContainer'); var inputs = container.querySelectorAll('input'); if (inputs[{0}]) inputs[{0}].value = '{1}';", index, objText);
                    index++;
                }
                if (objectives.Rows.Count == 0)
                {
                    objectivesScript += "addObjective('editObjectivesContainer');";
                }

                ScriptManager.RegisterStartupScript(this, GetType(), "LoadEditModal", "openEditModal(); " + objectivesScript, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadTaskForEdit error: " + ex.Message);
                ShowError("Failed to load task for editing.");
            }
        }

        protected void btnEditTaskSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    return;
                }

                int taskId = Convert.ToInt32(hidEditTaskId.Value);
                int userId = Convert.ToInt32(Session["UserId"]);

                // Check if task is assigned
                if (TaskHelper.IsTaskAssigned(taskId))
                {
                    ShowError("Cannot edit task that is assigned to children.");
                    return;
                }

                // Collect objectives from form
                List<string> objectives = new List<string>();
                foreach (string key in Request.Form.AllKeys)
                {
                    if (key.StartsWith("objective_"))
                    {
                        string value = Request.Form[key];
                        if (!string.IsNullOrEmpty(value.Trim()))
                        {
                            objectives.Add(value.Trim());
                        }
                    }
                }

                // Get form values
                string title = txtEditTitle.Text.Trim();
                string description = txtEditDescription.Text.Trim();
                string category = ddlEditCategory.SelectedValue;
                int points = Convert.ToInt32(txtEditPoints.Text);
                string priority = ddlEditPriority.SelectedValue;
                string difficulty = ddlEditDifficulty.SelectedValue;
                int? estimatedMinutes = null;
                if (!string.IsNullOrEmpty(txtEditEstimatedMinutes.Text))
                {
                    estimatedMinutes = Convert.ToInt32(txtEditEstimatedMinutes.Text);
                }
                string instructions = txtEditInstructions.Text.Trim();

                // Update task
                bool success = TaskHelper.UpdateTask(taskId, title, description, category, points, objectives,
                    priority, string.IsNullOrEmpty(difficulty) ? null : difficulty, estimatedMinutes,
                    string.IsNullOrEmpty(instructions) ? null : instructions, userId);

                if (success)
                {
                    ShowSuccess("Task updated successfully!");
                    LoadTasks();
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseEditModal", "closeEditModal();", true);
                }
                else
                {
                    ShowError("Failed to update task. It may be assigned to children.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnEditTaskSubmit_Click error: " + ex.Message);
                ShowError("An error occurred while updating the task.");
            }
        }

        private void LoadTaskForView(int taskId)
        {
            try
            {
                DataRow task = TaskHelper.GetTaskDetails(taskId);
                if (task == null)
                {
                    ShowError("Task not found.");
                    return;
                }

                // Get objectives
                DataTable objectives = TaskHelper.GetTaskObjectives(taskId);
                
                // Get assignments
                DataTable assignments = TaskHelper.GetTaskAssignments(taskId);
                
                // Get completion history (Fix #6)
                DataTable history = TaskHelper.GetTaskCompletionHistory(taskId);

                // Build view content
                string content = "<div style='line-height: 1.8;'>";
                content += "<p><strong>Title:</strong> " + Server.HtmlEncode(task["Title"].ToString()) + "</p>";
                content += "<p><strong>Description:</strong> " + (task["Description"] != DBNull.Value ? Server.HtmlEncode(task["Description"].ToString()) : "None") + "</p>";
                content += "<p><strong>Category:</strong> " + Server.HtmlEncode(task["Category"].ToString()) + "</p>";
                content += "<p><strong>Points:</strong> " + task["PointsReward"].ToString() + "</p>";
                if (task["Priority"] != DBNull.Value)
                    content += "<p><strong>Priority:</strong> " + Server.HtmlEncode(task["Priority"].ToString()) + "</p>";
                if (task["Difficulty"] != DBNull.Value)
                    content += "<p><strong>Difficulty:</strong> " + Server.HtmlEncode(task["Difficulty"].ToString()) + "</p>";
                if (task["EstimatedMinutes"] != DBNull.Value)
                    content += "<p><strong>Estimated Time:</strong> " + task["EstimatedMinutes"].ToString() + " minutes</p>";
                if (task["Instructions"] != DBNull.Value && !string.IsNullOrEmpty(task["Instructions"].ToString()))
                    content += "<p><strong>Instructions:</strong><br/>" + Server.HtmlEncode(task["Instructions"].ToString()).Replace("\n", "<br/>") + "</p>";

                content += "<h3 style='margin-top: 20px; margin-bottom: 10px;'>Objectives:</h3><ul>";
                foreach (DataRow obj in objectives.Rows)
                {
                    content += "<li>" + Server.HtmlEncode(obj["ObjectiveText"].ToString()) + "</li>";
                }
                content += "</ul>";

                if (assignments.Rows.Count > 0)
                {
                    content += "<h3 style='margin-top: 20px; margin-bottom: 10px;'>Current Assignments:</h3><ul>";
                    foreach (DataRow assign in assignments.Rows)
                    {
                        content += "<li>" + Server.HtmlEncode(assign["ChildName"].ToString()) + " - " + Server.HtmlEncode(assign["Status"].ToString()) + "</li>";
                    }
                    content += "</ul>";
                }

                if (history.Rows.Count > 0)
                {
                    content += "<h3 style='margin-top: 20px; margin-bottom: 10px;'>Completion History:</h3>";
                    content += "<p>This task has been completed <strong>" + history.Rows.Count + "</strong> time(s).</p>";
                    content += "<ul>";
                    foreach (DataRow h in history.Rows)
                    {
                        string rating = h["Rating"] != DBNull.Value ? h["Rating"].ToString() + " stars" : "Failed";
                        content += "<li>" + Server.HtmlEncode(h["ChildName"].ToString()) + " - " + rating + " (" + Convert.ToDateTime(h["ReviewDate"]).ToString("MMM dd, yyyy") + ")</li>";
                    }
                    content += "</ul>";
                }

                content += "</div>";

                // Inject into modal
                string script = string.Format("document.getElementById('viewTaskTitle').textContent = '{0}'; document.getElementById('viewTaskContent').innerHTML = {1}; openViewModal();",
                    Server.HtmlEncode(task["Title"].ToString()).Replace("'", "\\'"),
                    "'" + content.Replace("'", "\\'").Replace("\n", "\\n") + "'");

                ScriptManager.RegisterStartupScript(this, GetType(), "LoadViewModal", script, true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadTaskForView error: " + ex.Message);
                ShowError("Failed to load task details.");
            }
        }

        private void DeleteTask(int taskId)
        {
            try
            {
                // Check if task is assigned
                if (TaskHelper.IsTaskAssigned(taskId))
                {
                    ShowError("Cannot delete task that is assigned to children.");
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                bool success = TaskHelper.DeleteTask(taskId, userId);

                if (success)
                {
                    ShowSuccess("Task deleted successfully!");
                    LoadTasks();
                }
                else
                {
                    ShowError("Failed to delete task. It may be assigned to children.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DeleteTask error: " + ex.Message);
                ShowError("An error occurred while deleting the task.");
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

