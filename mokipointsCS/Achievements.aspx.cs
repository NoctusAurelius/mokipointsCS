using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class Achievements : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Check authentication
                if (Session["UserId"] == null)
                {
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                if (!IsPostBack)
                {
                    LoadAchievements();
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Achievements Page_Load error: " + ex.Message);
                Response.Redirect("Login.aspx", false);
            }
        }

        private void LoadAchievements()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                string role = Session["UserRole"]?.ToString() ?? "";

                if (string.IsNullOrEmpty(role))
                {
                    pnlNoAchievements.Visible = true;
                    pnlAchievements.Visible = false;
                    return;
                }

                // Get all achievements for user's role
                List<AchievementWithStatus> achievements = AchievementHelper.GetAllAchievementsForUser(userId, role);

                if (achievements == null || achievements.Count == 0)
                {
                    pnlNoAchievements.Visible = true;
                    pnlAchievements.Visible = false;
                    return;
                }

                pnlNoAchievements.Visible = false;
                pnlAchievements.Visible = true;

                // Calculate stats
                int total = achievements.Count;
                int earned = achievements.Count(a => a.IsEarned);
                int progress = total > 0 ? (int)Math.Round((double)earned / total * 100) : 0;

                statTotal.InnerText = total.ToString();
                statEarned.InnerText = earned.ToString();
                statProgress.InnerText = progress + "%";

                // Bind to repeater
                rptAchievements.DataSource = achievements;
                rptAchievements.DataBind();

                // Store achievement data in hidden field for JavaScript
                StringBuilder jsonBuilder = new StringBuilder();
                jsonBuilder.Append("[");
                for (int i = 0; i < achievements.Count; i++)
                {
                    var a = achievements[i];
                    if (i > 0) jsonBuilder.Append(",");
                    jsonBuilder.Append("{");
                    jsonBuilder.AppendFormat("\"Id\":{0},", a.Id);
                    jsonBuilder.AppendFormat("\"Name\":\"{0}\",", EscapeJson(a.Name));
                    jsonBuilder.AppendFormat("\"Description\":\"{0}\",", EscapeJson(a.Description));
                    jsonBuilder.AppendFormat("\"Rarity\":\"{0}\",", EscapeJson(a.Rarity));
                    jsonBuilder.AppendFormat("\"BadgeImagePath\":\"{0}\",", EscapeJson(a.BadgeImagePath));
                    jsonBuilder.AppendFormat("\"IsEarned\":{0},", a.IsEarned ? "true" : "false");
                    jsonBuilder.AppendFormat("\"HowToAchieve\":\"{0}\",", EscapeJson(a.HowToAchieve ?? ""));
                    jsonBuilder.AppendFormat("\"DeveloperMessage\":\"{0}\",", EscapeJson(a.DeveloperMessage ?? ""));
                    
                    if (a.IsEarned && a.EarnedDate.HasValue)
                    {
                        jsonBuilder.AppendFormat("\"EarnedDate\":\"{0}\",", a.EarnedDate.Value.ToString("yyyy-MM-dd"));
                    }
                    else
                    {
                        jsonBuilder.Append("\"EarnedDate\":null,");
                    }
                    
                    // Get achievement details for progress
                    var detail = AchievementHelper.GetAchievementDetails(a.Id, userId);
                    if (detail != null && !detail.IsEarned)
                    {
                        jsonBuilder.AppendFormat("\"CurrentProgress\":{0},", detail.CurrentProgress);
                        jsonBuilder.AppendFormat("\"TargetProgress\":{0}", detail.TargetProgress);
                    }
                    else
                    {
                        jsonBuilder.Append("\"CurrentProgress\":0,");
                        jsonBuilder.Append("\"TargetProgress\":0");
                    }
                    
                    jsonBuilder.Append("}");
                }
                jsonBuilder.Append("]");
                hdnAchievementsData.Value = jsonBuilder.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadAchievements error: " + ex.Message);
                pnlNoAchievements.Visible = true;
                pnlAchievements.Visible = false;
            }
        }

        protected void rptAchievements_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                AchievementWithStatus achievement = (AchievementWithStatus)e.Item.DataItem;
                
                Panel pnlProgress = (Panel)e.Item.FindControl("pnlProgress");
                Panel pnlEarned = (Panel)e.Item.FindControl("pnlEarned");
                Literal litProgressText = (Literal)e.Item.FindControl("litProgressText");
                Literal litEarnedDate = (Literal)e.Item.FindControl("litEarnedDate");
                System.Web.UI.HtmlControls.HtmlGenericControl progressBar = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("progressBar");

                if (achievement.IsEarned)
                {
                    pnlProgress.Visible = false;
                    pnlEarned.Visible = true;
                    if (achievement.EarnedDate.HasValue)
                    {
                        litEarnedDate.Text = "\u2713 Earned on " + achievement.EarnedDate.Value.ToString("MMM dd, yyyy");
                    }
                    else
                    {
                        litEarnedDate.Text = "\u2713 Earned";
                    }
                }
                else
                {
                    pnlEarned.Visible = false;
                    
                    // Get achievement details for progress
                    int userId = Convert.ToInt32(Session["UserId"]);
                    AchievementDetail detail = AchievementHelper.GetAchievementDetails(achievement.Id, userId);
                    
                    if (detail != null && detail.TargetProgress > 0)
                    {
                        pnlProgress.Visible = true;
                        int percentage = (int)Math.Round((double)detail.CurrentProgress / detail.TargetProgress * 100);
                        percentage = Math.Min(100, percentage);
                        litProgressText.Text = detail.CurrentProgress + " / " + detail.TargetProgress + " (" + percentage + "%)";
                        
                        // Set progress bar width
                        if (progressBar != null)
                        {
                            progressBar.Style["width"] = percentage + "%";
                        }
                    }
                    else
                    {
                        pnlProgress.Visible = false;
                    }
                }
            }
        }

        protected string GetRarityClass(string rarity)
        {
            return "rarity-" + rarity.ToLower();
        }

        protected string GetProgressPercentage(object dataItem)
        {
            try
            {
                AchievementWithStatus achievement = (AchievementWithStatus)dataItem;
                if (achievement.IsEarned)
                {
                    return "100";
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                AchievementDetail detail = AchievementHelper.GetAchievementDetails(achievement.Id, userId);
                
                if (detail != null && detail.TargetProgress > 0)
                {
                    int percentage = (int)Math.Round((double)detail.CurrentProgress / detail.TargetProgress * 100);
                    return Math.Min(100, percentage).ToString();
                }
            }
            catch
            {
                // Ignore errors
            }
            return "0";
        }

        private string EscapeJson(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";
            
            return value.Replace("\\", "\\\\")
                       .Replace("\"", "\\\"")
                       .Replace("\n", "\\n")
                       .Replace("\r", "\\r")
                       .Replace("\t", "\\t");
        }
    }
}

