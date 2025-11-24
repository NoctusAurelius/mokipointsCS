using System;
using System.Data;
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

                // Set user name
                if (Session["FirstName"] != null && Session["LastName"] != null)
                {
                    litUserName.Text = Session["FirstName"].ToString() + " " + Session["LastName"].ToString();
                }

                if (!IsPostBack)
                {
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
                ddlChild.Items.Clear();
                ddlChild.Items.Add(new ListItem("-- Select a child --", ""));

                foreach (DataRow child in children.Rows)
                {
                    int childId = Convert.ToInt32(child["Id"]);
                    string childName = child["FirstName"].ToString() + " " + child["LastName"].ToString();
                    ddlChild.Items.Add(new ListItem(childName, childId.ToString()));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadChildren error: " + ex.Message);
                ShowError("Failed to load children list.");
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
                int childId = Convert.ToInt32(ddlChild.SelectedValue);

                // Parse deadline
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
                        deadline = deadlineDate.Date;
                    }
                }

                // Assign task (Fix #5: Server-side deadline validation is in TaskHelper.AssignTask)
                bool success = TaskHelper.AssignTask(taskId, childId, deadline);

                if (success)
                {
                    Response.Redirect("Tasks.aspx?assigned=true");
                }
                else
                {
                    ShowError("Failed to assign task. The child may be banned, or the task may already be assigned to this child.");
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
    }
}

