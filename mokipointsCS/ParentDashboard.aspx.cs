using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class ParentDashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("ParentDashboard.aspx Page_Load called at " + DateTime.Now.ToString());
            System.Diagnostics.Debug.WriteLine("Session UserId: " + (Session["UserId"] != null ? Session["UserId"].ToString() : "NULL"));
            System.Diagnostics.Debug.WriteLine("Session UserRole: " + (Session["UserRole"] != null ? Session["UserRole"].ToString() : "NULL"));
            
            try
            {
                // Check authentication
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("ParentDashboard: User not authenticated - redirecting to Login");
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                // Check role
                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "PARENT")
                {
                    System.Diagnostics.Debug.WriteLine("ParentDashboard: User is not PARENT - redirecting to Dashboard");
                    Response.Redirect("Dashboard.aspx", false);
                    return;
                }

                if (!IsPostBack)
                {
                    // Check and auto-fail overdue tasks
                    int userId = Convert.ToInt32(Session["UserId"]);
                    int? familyId = FamilyHelper.GetUserFamilyId(userId);
                    if (familyId.HasValue)
                    {
                        TaskHelper.AutoFailOverdueTasks(familyId.Value);
                        LoadDashboardMetrics(familyId.Value);
                    }
                    
                    // Display user name - load from session or database
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
                        else
                        {
                            litUserName.Text = "Parent";
                        }
                    }
                    
                    // Load profile picture
                    LoadProfilePicture(userId);
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // This is normal for Response.Redirect - don't treat as error
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ParentDashboard Page_Load error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void LoadDashboardMetrics(int familyId)
        {
            try
            {
                // High Priority Metrics
                litTreasuryBalance.Text = DashboardHelper.GetTreasuryBalance(familyId).ToString("N0");
                litPendingReviews.Text = DashboardHelper.GetPendingReviewsCount(familyId).ToString();
                litPendingOrders.Text = DashboardHelper.GetPendingOrdersCount(familyId).ToString();
                litActiveChildren.Text = DashboardHelper.GetActiveChildrenCount(familyId).ToString();
                
                // Medium Priority: Quick Action Badges
                int pendingReviews = DashboardHelper.GetPendingReviewsCount(familyId);
                if (pendingReviews > 0)
                {
                    pnlReviewBadge.Visible = true;
                    litReviewBadge.Text = pendingReviews > 9 ? "9+" : pendingReviews.ToString();
                }
                else
                {
                    pnlReviewBadge.Visible = false;
                }

                int pendingOrders = DashboardHelper.GetPendingOrdersCount(familyId);
                if (pendingOrders > 0)
                {
                    pnlOrderBadge.Visible = true;
                    litOrderBadge.Text = pendingOrders > 9 ? "9+" : pendingOrders.ToString();
                }
                else
                {
                    pnlOrderBadge.Visible = false;
                }
                
                // Medium Priority: Weekly Statistics (for charts)
                var weekStats = DashboardHelper.GetWeeklyStatistics(familyId);
                int weekTasks = Convert.ToInt32(weekStats["TasksCompleted"]);
                int weekPoints = Convert.ToInt32(weekStats["PointsAwarded"]);
                int weekOrders = Convert.ToInt32(weekStats["OrdersConfirmed"]);
                
                // Monthly Statistics (for charts)
                var monthStats = DashboardHelper.GetMonthlyStatistics(familyId);
                int monthTasks = Convert.ToInt32(monthStats["TasksCompleted"]);
                int monthPoints = Convert.ToInt32(monthStats["PointsAwarded"]);
                int monthOrders = Convert.ToInt32(monthStats["OrdersConfirmed"]);
                
                // Set chart data in hidden fields (JSON format)
                if (hdnWeekData != null)
                {
                    hdnWeekData.Value = string.Format("{{\"tasks\":{0},\"points\":{1},\"orders\":{2}}}", weekTasks, weekPoints, weekOrders);
                }
                if (hdnMonthData != null)
                {
                    hdnMonthData.Value = string.Format("{{\"tasks\":{0},\"points\":{1},\"orders\":{2}}}", monthTasks, monthPoints, monthOrders);
                }
                
                // Medium Priority: Child Activity Overview
                DataTable childActivity = DashboardHelper.GetChildActivityOverview(familyId);
                if (childActivity != null && childActivity.Rows.Count > 0)
                {
                    rptChildActivity.DataSource = childActivity;
                    rptChildActivity.DataBind();
                    rptChildActivity.Visible = true;
                    pnlNoChildren.Visible = false;
                }
                else
                {
                    rptChildActivity.Visible = false;
                    pnlNoChildren.Visible = true;
                }
                
                // Recent Activity section removed to reduce clutter
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadDashboardMetrics error: " + ex.Message);
            }
        }

        protected string GetActivityText(object dataItem)
        {
            try
            {
                System.Data.DataRowView row = (System.Data.DataRowView)dataItem;
                string activityType = row["ActivityType"].ToString();
                string childName = row["ChildName"] != DBNull.Value ? row["ChildName"].ToString() : "";
                string taskTitle = row["TaskTitle"] != DBNull.Value ? row["TaskTitle"].ToString() : "";
                int points = row["Points"] != DBNull.Value ? Convert.ToInt32(row["Points"]) : 0;
                string orderNumber = row["OrderNumber"] != DBNull.Value ? row["OrderNumber"].ToString() : "";

                switch (activityType)
                {
                    case "TaskCompleted":
                        return string.Format("{0} completed '{1}' (+{2} points)", childName, taskTitle, points);
                    case "OrderConfirmed":
                        return string.Format("{0}'s order {1} confirmed ({2} points)", childName, orderNumber, points);
                    default:
                        return "Activity";
                }
            }
            catch
            {
                return "Activity";
            }
        }

        protected string GetActivityDate(object dataItem)
        {
            try
            {
                System.Data.DataRowView row = (System.Data.DataRowView)dataItem;
                if (row["ActivityDate"] != DBNull.Value)
                {
                    DateTime date = Convert.ToDateTime(row["ActivityDate"]);
                    if (date.Date == DateTime.Now.Date)
                    {
                        return "Today " + date.ToString("HH:mm");
                    }
                    else if (date.Date == DateTime.Now.AddDays(-1).Date)
                    {
                        return "Yesterday " + date.ToString("HH:mm");
                    }
                    else
                    {
                        return date.ToString("MMM dd, HH:mm");
                    }
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        protected void rptChildActivity_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item || 
                e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
            {
                System.Data.DataRowView row = (System.Data.DataRowView)e.Item.DataItem;
                Literal litActivityDot = (Literal)e.Item.FindControl("litActivityDot");
                
                if (litActivityDot != null)
                {
                    string color = "#999"; // Default gray
                    
                    try
                    {
                        if (row["LastActivityDate"] != DBNull.Value && row["LastActivityDate"] != null)
                        {
                            DateTime lastActivity = Convert.ToDateTime(row["LastActivityDate"]);
                            TimeSpan timeSince = DateTime.Now - lastActivity;

                            if (timeSince.TotalDays <= 1)
                            {
                                color = "#28a745"; // Green - active today
                            }
                            else if (timeSince.TotalDays <= 7)
                            {
                                color = "#ffc107"; // Yellow - active this week
                            }
                            else
                            {
                                color = "#dc3545"; // Red - inactive
                            }
                        }
                    }
                    catch
                    {
                        // Use default color
                    }
                    
                    litActivityDot.Text = string.Format(
                        "<div style=\"width: 12px; height: 12px; border-radius: 50%; background: {0};\"></div>", 
                        color);
                }
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

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Clear();
                Response.Redirect("Login.aspx", false);
            }
            catch (System.Threading.ThreadAbortException)
            {
                // This is normal for Response.Redirect - don't treat as error
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Logout error: " + ex.Message);
                Response.Redirect("Login.aspx", false);
            }
        }
    }
}

