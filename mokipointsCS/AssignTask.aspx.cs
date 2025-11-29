using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class AssignTask : System.Web.UI.Page
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

                // Get task ID from query string
                if (string.IsNullOrEmpty(Request.QueryString["taskId"]))
                {
                    ShowError("Task ID is required.");
                    return;
                }

                int taskId = Convert.ToInt32(Request.QueryString["taskId"]);

                int userId = Convert.ToInt32(Session["UserId"]);

                // Set user name - load from session or database
                if (Session["FirstName"] != null)
                {
                    litUserName.Text = Session["FirstName"].ToString();
                    if (Session["LastName"] != null)
                    {
                        litUserName.Text += " " + Session["LastName"].ToString();
                    }
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
                    
                    LoadTaskInfo(taskId);
                    LoadChildren(taskId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AssignTask Page_Load error: " + ex.Message);
                ShowError("An error occurred. Please try again.");
            }
        }

        private void LoadTaskInfo(int taskId)
        {
            try
            {
                DataRow task = TaskHelper.GetTaskDetails(taskId);
                if (task == null)
                {
                    ShowError("Task not found.");
                    return;
                }

                litTaskTitle.Text = task["Title"].ToString();
                litTaskDescription.Text = task["Description"] != DBNull.Value && !string.IsNullOrEmpty(task["Description"].ToString()) 
                    ? task["Description"].ToString() : "No description";
                litTaskCategory.Text = task["Category"].ToString();
                litTaskPoints.Text = task["PointsReward"].ToString();
                litTaskCreatedDate.Text = Convert.ToDateTime(task["CreatedDate"]).ToString("MMM dd, yyyy");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadTaskInfo error: " + ex.Message);
                ShowError("Failed to load task information.");
            }
        }

        private void LoadChildren(int taskId)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int? familyId = FamilyHelper.GetUserFamilyId(userId);

                if (!familyId.HasValue)
                {
                    ShowError("You must be in a family to assign tasks.");
                    return;
                }

                DataTable children = TaskHelper.GetFamilyChildren(familyId.Value);
                cblChildren.Items.Clear();

                foreach (DataRow child in children.Rows)
                {
                    int childId = Convert.ToInt32(child["Id"]);
                    string childName = child["FirstName"].ToString() + " " + child["LastName"].ToString();
                    cblChildren.Items.Add(new ListItem(childName, childId.ToString()));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadChildren error: " + ex.Message);
                ShowError("Failed to load children list.");
            }
        }

        protected void ValidateChildrenSelection(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            
            foreach (ListItem item in cblChildren.Items)
            {
                if (item.Selected)
                {
                    args.IsValid = true;
                    break;
                }
            }
        }

        protected void btnAssignTask_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid)
                {
                    return;
                }

                int taskId = Convert.ToInt32(Request.QueryString["taskId"]);
                
                // Get all selected children
                List<int> selectedChildIds = new List<int>();
                foreach (ListItem item in cblChildren.Items)
                {
                    if (item.Selected)
                    {
                        selectedChildIds.Add(Convert.ToInt32(item.Value));
                    }
                }
                
                if (selectedChildIds.Count == 0)
                {
                    ShowError("Please select at least one child to assign the task to.");
                    return;
                }

                // Parse deadline with validation
                DateTime? deadline = null;
                if (!string.IsNullOrEmpty(txtDeadlineDate.Text))
                {
                    DateTime deadlineDate = Convert.ToDateTime(txtDeadlineDate.Text);
                    if (!string.IsNullOrEmpty(txtDeadlineTime.Text))
                    {
                        TimeSpan time = TimeSpan.Parse(txtDeadlineTime.Text);
                        deadline = deadlineDate.Date.Add(time);
                    }
                    else
                    {
                        // If no time specified, set to end of day (23:59:59)
                        deadline = deadlineDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
                    }
                    
                    // Validate deadline: cannot be on current date, must be at least 10 minutes in the future, maximum 30 days
                    DateTime now = DateTime.Now;
                    DateTime today = now.Date; // Today at midnight
                    DateTime minDeadline = now.AddMinutes(10);
                    DateTime maxDeadline = now.AddDays(30);
                    
                    if (deadline.Value <= now)
                    {
                        ShowError("Deadline must be in the future. Please select a date/time that has not passed.");
                        return;
                    }
                    
                    // Check if deadline is on current date (not allowed - Issue #8)
                    if (deadline.Value.Date == today)
                    {
                        ShowError("Deadline cannot be set on today's date. Please select a future date (tomorrow or later).");
                        return;
                    }
                    
                    if (deadline.Value < minDeadline)
                    {
                        ShowError(string.Format("Deadline must be at least 10 minutes in the future. The earliest deadline is {0:MMM dd, yyyy hh:mm tt}.", minDeadline));
                        return;
                    }
                    
                    if (deadline.Value > maxDeadline)
                    {
                        ShowError(string.Format("Deadline cannot be more than 30 days in the future. The latest deadline is {0:MMM dd, yyyy}.", maxDeadline.Date));
                        return;
                    }
                }

                // Assign task to all selected children
                int successCount = 0;
                int failCount = 0;
                List<string> failedChildren = new List<string>();
                
                foreach (int childId in selectedChildIds)
                {
                    bool success = TaskHelper.AssignTask(taskId, childId, deadline);
                    if (success)
                    {
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        // Get child name for error message
                        string childName = cblChildren.Items.FindByValue(childId.ToString()).Text;
                        failedChildren.Add(childName);
                    }
                }
                
                if (successCount > 0)
                {
                    if (failCount == 0)
                    {
                        // All assignments successful
                        Response.Redirect("Tasks.aspx?assigned=true&count=" + successCount);
                    }
                    else
                    {
                        // Some succeeded, some failed
                        string errorMsg = string.Format("Task assigned to {0} child(ren) successfully. Failed to assign to: {1}", 
                            successCount, string.Join(", ", failedChildren));
                        ShowError(errorMsg);
                        // Still redirect to show success
                        Response.Redirect("Tasks.aspx?assigned=true&count=" + successCount);
                    }
                }
                else
                {
                    // All assignments failed
                    ShowError("Failed to assign task to any selected children. They may be banned, or the task may already be assigned to them.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnAssignTask_Click error: " + ex.Message);
                ShowError("An error occurred while assigning the task.");
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

