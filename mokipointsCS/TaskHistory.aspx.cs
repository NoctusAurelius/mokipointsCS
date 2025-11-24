using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class TaskHistory : System.Web.UI.Page
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

            // Only children can view their task history
            if (userRole != "CHILD")
            {
                ShowError("Only children can view task history.");
                pnlTasks.Visible = false;
                return;
            }

            // Set user name
            if (Session["FirstName"] != null && Session["LastName"] != null)
            {
                litUserName.Text = Session["FirstName"].ToString() + " " + Session["LastName"].ToString();
            }

            if (!IsPostBack)
            {
                LoadTaskHistory(userId);
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

        protected void LoadTaskHistory(int userId)
        {
            try
            {
                // Get all reviewed tasks for this child (tasks that have been reviewed)
                string query = @"
                    SELECT ta.Id AS AssignmentId, ta.TaskId, ta.CompletedDate,
                           t.Title, t.Description, t.Category, t.PointsReward,
                           tr.Rating, tr.PointsAwarded, tr.IsFailed, tr.IsAutoFailed, tr.ReviewDate,
                           reviewer.FirstName + ' ' + reviewer.LastName AS ReviewerName
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                    INNER JOIN [dbo].[TaskReviews] tr ON ta.Id = tr.TaskAssignmentId
                    INNER JOIN [dbo].[Users] reviewer ON tr.ReviewedBy = reviewer.Id
                    WHERE ta.UserId = @UserId AND tr.ReviewDate IS NOT NULL";

                string statusFilter = ddlStatusFilter.SelectedValue;
                if (!string.IsNullOrEmpty(statusFilter))
                {
                    if (statusFilter == "Failed")
                    {
                        query += " AND tr.IsFailed = 1";
                    }
                    else if (statusFilter == "Reviewed")
                    {
                        query += " AND tr.IsFailed = 0";
                    }
                }

                query += " ORDER BY tr.ReviewDate DESC";

                DataTable tasks = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserId", userId));

                if (tasks != null && tasks.Rows.Count > 0)
                {
                    rptTasks.DataSource = tasks;
                    rptTasks.DataBind();
                    pnlTasks.Visible = true;
                    pnlEmpty.Visible = false;

                    // Calculate statistics
                    CalculateStatistics(tasks);
                }
                else
                {
                    pnlTasks.Visible = false;
                    pnlEmpty.Visible = true;
                    ResetStatistics();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadTaskHistory error: " + ex.Message);
                ShowError("Error loading task history.");
                pnlTasks.Visible = false;
                pnlEmpty.Visible = true;
            }
        }

        protected void CalculateStatistics(DataTable tasks)
        {
            int totalCompleted = 0;
            int totalPoints = 0;
            int totalRating = 0;
            int ratingCount = 0;
            int tasksFailed = 0;

            foreach (DataRow row in tasks.Rows)
            {
                bool isFailed = Convert.ToBoolean(row["IsFailed"]);
                
                if (isFailed)
                {
                    tasksFailed++;
                }
                else
                {
                    totalCompleted++;
                }

                int pointsAwarded = Convert.ToInt32(row["PointsAwarded"]);
                totalPoints += pointsAwarded;

                if (!isFailed && row["Rating"] != DBNull.Value)
                {
                    int rating = Convert.ToInt32(row["Rating"]);
                    totalRating += rating;
                    ratingCount++;
                }
            }

            litTotalCompleted.Text = totalCompleted.ToString();
            litTotalPoints.Text = totalPoints.ToString();
            litTasksFailed.Text = tasksFailed.ToString();

            if (ratingCount > 0)
            {
                double avgRating = (double)totalRating / ratingCount;
                litAverageRating.Text = avgRating.ToString("F1");
            }
            else
            {
                litAverageRating.Text = "N/A";
            }
        }

        protected void ResetStatistics()
        {
            litTotalCompleted.Text = "0";
            litTotalPoints.Text = "0";
            litAverageRating.Text = "0";
            litTasksFailed.Text = "0";
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            LoadTaskHistory(userId);
        }

        protected void rptTasks_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Additional data binding if needed
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }

        private void ShowSuccess(string message)
        {
            lblSuccess.Text = message;
            pnlSuccess.CssClass = "success-message";
            pnlSuccess.Visible = true;
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            pnlError.CssClass = "error-message";
            pnlError.Visible = true;
        }
    }
}

