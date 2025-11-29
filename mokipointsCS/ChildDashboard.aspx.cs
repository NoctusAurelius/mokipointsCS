using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class ChildDashboard : System.Web.UI.Page
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

                int userId = Convert.ToInt32(Session["UserId"]);

                // Check if child is in a family
                int? familyId = FamilyHelper.GetUserFamilyId(userId);
                if (!familyId.HasValue)
                {
                    // Not in a family - redirect to join family page
                    Response.Redirect("JoinFamily.aspx");
                    return;
                }

                if (!IsPostBack)
                {
                    // Check for newly earned achievements
                    CheckAndShowAchievementNotification();
                    
                    // Check and auto-fail overdue tasks
                    if (familyId.HasValue)
                    {
                        TaskHelper.AutoFailOverdueTasks(familyId.Value);
                        LoadDashboardMetrics(userId, familyId.Value);
                    }
                    
                    // Display user name - load from session or database
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
                        else
                        {
                            litUserName.Text = "Child";
                        }
                    }
                    
                    // Load profile picture
                    LoadProfilePicture(userId);
                    
                    // Load top achievements
                    LoadTopAchievements(userId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ChildDashboard Page_Load error: " + ex.Message);
                Response.Redirect("Login.aspx");
            }
        }

        private void CheckAndShowAchievementNotification()
        {
            try
            {
                // Check if there's a newly earned achievement in session
                if (Session["NewAchievement"] != null)
                {
                    AchievementData achievement = Session["NewAchievement"] as AchievementData;
                    if (achievement != null)
                    {
                        // Serialize achievement data to JSON for JavaScript
                        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                        string json = serializer.Serialize(new
                        {
                            Id = achievement.Id,
                            Name = achievement.Name,
                            Description = achievement.Description,
                            Rarity = achievement.Rarity,
                            BadgeImagePath = ResolveUrl("~/" + achievement.BadgeImagePath)
                        });
                        hdnAchievementNotification.Value = json;
                        
                        // Clear session to prevent showing again
                        Session.Remove("NewAchievement");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CheckAndShowAchievementNotification error: " + ex.Message);
            }
        }

        protected void LoadDashboardMetrics(int userId, int familyId)
        {
            try
            {
                // High Priority Metrics
                litCurrentPoints.Text = DashboardHelper.GetChildPoints(userId).ToString("N0");
                litActiveTasks.Text = DashboardHelper.GetActiveTasksCount(userId).ToString();
                litCompletedWeek.Text = DashboardHelper.GetCompletedThisWeek(userId).ToString();
                litPendingReviews.Text = DashboardHelper.GetPendingReviewsCountForChild(userId).ToString();
                litAvailableRewards.Text = DashboardHelper.GetAvailableRewardsCount(userId, familyId).ToString();
                litActiveOrders.Text = DashboardHelper.GetActiveOrdersCount(userId).ToString();
                
                // Calculate progress toward 10,000 cap
                int currentPoints = DashboardHelper.GetChildPoints(userId);
                double progressPercent = Math.Min(100, (currentPoints / 10000.0) * 100);
                litProgressPercent.Text = progressPercent.ToString("F1");
                
                // Set progress bar width
                System.Web.UI.HtmlControls.HtmlGenericControl progressBar = 
                    (System.Web.UI.HtmlControls.HtmlGenericControl)FindControl("progressBar");
                if (progressBar != null)
                {
                    progressBar.Style["width"] = progressPercent.ToString("F1") + "%";
                }
                
                // Medium Priority: Quick Action Badges
                int activeTasks = DashboardHelper.GetActiveTasksCount(userId);
                if (activeTasks > 0)
                {
                    pnlTaskBadge.Visible = true;
                    litTaskBadge.Text = activeTasks > 9 ? "9+" : activeTasks.ToString();
                }
                else
                {
                    pnlTaskBadge.Visible = false;
                }

                int availableRewards = DashboardHelper.GetAvailableRewardsCount(userId, familyId);
                if (availableRewards > 0)
                {
                    pnlRewardBadge.Visible = true;
                    litRewardBadge.Text = availableRewards > 9 ? "9+" : availableRewards.ToString();
                }
                else
                {
                    pnlRewardBadge.Visible = false;
                }

                int activeOrders = DashboardHelper.GetActiveOrdersCount(userId);
                if (activeOrders > 0)
                {
                    pnlOrderBadge.Visible = true;
                    litOrderBadge.Text = activeOrders > 9 ? "9+" : activeOrders.ToString();
                }
                else
                {
                    pnlOrderBadge.Visible = false;
                }
                
                // Medium Priority: Weekly Statistics
                var weekStats = DashboardHelper.GetWeeklyStatisticsForChild(userId);
                litWeekTasks.Text = weekStats["TasksCompleted"].ToString();
                litWeekEarned.Text = Convert.ToInt32(weekStats["PointsEarned"]).ToString("N0");
                litWeekSpent.Text = Convert.ToInt32(weekStats["PointsSpent"]).ToString("N0");
                
                // Low Priority: Streak Counter
                int streak = DashboardHelper.GetStreakCount(userId);
                litStreak.Text = streak.ToString();
                if (streak == 0)
                {
                    litStreakMessage.Text = "Start completing tasks to build your streak!";
                }
                else if (streak == 1)
                {
                    litStreakMessage.Text = "Great start! Complete another task tomorrow to continue!";
                }
                else if (streak < 7)
                {
                    litStreakMessage.Text = string.Format("Awesome! {0} days in a row!", streak);
                }
                else if (streak < 30)
                {
                    litStreakMessage.Text = string.Format("Incredible! {0} day streak! Keep going!", streak);
                }
                else
                {
                    litStreakMessage.Text = string.Format("Legendary! {0} day streak! You're amazing!", streak);
                }
                
                // Medium Priority: Recent Activity
                DataTable recentActivity = DashboardHelper.GetRecentActivityForChild(userId, 10);
                if (recentActivity != null && recentActivity.Rows.Count > 0)
                {
                    rptRecentActivity.DataSource = recentActivity;
                    rptRecentActivity.DataBind();
                    rptRecentActivity.Visible = true;
                    pnlNoActivity.Visible = false;
                }
                else
                {
                    rptRecentActivity.Visible = false;
                    pnlNoActivity.Visible = true;
                }
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
                string taskTitle = row["TaskTitle"] != DBNull.Value ? row["TaskTitle"].ToString() : "";
                string rewardName = row["RewardName"] != DBNull.Value ? row["RewardName"].ToString() : "";

                switch (activityType)
                {
                    case "PointsEarned":
                        return string.Format("Completed '{0}'", taskTitle);
                    case "PointsSpent":
                        return string.Format("Purchased '{0}'", rewardName);
                    default:
                        return "Activity";
                }
            }
            catch
            {
                return "Activity";
            }
        }

        protected string GetActivityPoints(object dataItem)
        {
            try
            {
                System.Data.DataRowView row = (System.Data.DataRowView)dataItem;
                int points = row["Points"] != DBNull.Value ? Convert.ToInt32(row["Points"]) : 0;
                if (points > 0)
                {
                    return "+" + points.ToString("N0");
                }
                else
                {
                    return points.ToString("N0");
                }
            }
            catch
            {
                return "0";
            }
        }

        protected string GetActivityClass(object dataItem)
        {
            try
            {
                System.Data.DataRowView row = (System.Data.DataRowView)dataItem;
                int points = row["Points"] != DBNull.Value ? Convert.ToInt32(row["Points"]) : 0;
                return points > 0 ? "points-earned" : "";
            }
            catch
            {
                return "";
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

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                Session.Clear();
                Response.Redirect("Login.aspx");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Logout error: " + ex.Message);
                Response.Redirect("Login.aspx");
            }
        }

        private void LoadTopAchievements(int userId)
        {
            try
            {
                List<AchievementData> topAchievements = AchievementHelper.GetTop3Achievements(userId);
                
                if (topAchievements != null && topAchievements.Count > 0)
                {
                    pnlTopAchievements.Visible = true;
                    pnlNoAchievements.Visible = false;
                    rptTopAchievements.DataSource = topAchievements;
                    rptTopAchievements.DataBind();
                }
                else
                {
                    pnlTopAchievements.Visible = false;
                    pnlNoAchievements.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadTopAchievements error: " + ex.Message);
                pnlTopAchievements.Visible = false;
                pnlNoAchievements.Visible = true;
            }
        }
    }
}

