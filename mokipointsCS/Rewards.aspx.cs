using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class Rewards : System.Web.UI.Page
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

                // Set user name
                if (Session["FirstName"] != null)
                {
                    litUserName.Text = Session["FirstName"].ToString();
                    if (Session["LastName"] != null)
                    {
                        litUserName.Text += " " + Session["LastName"].ToString();
                    }
                }

                if (!IsPostBack)
                {
                    LoadRewards();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Rewards Page_Load error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred. Please try again.");
            }
        }

        private void LoadRewards()
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

                DataTable rewards = RewardHelper.GetFamilyRewards(familyId.Value, true);

                if (rewards.Rows.Count > 0)
                {
                    rptRewards.DataSource = rewards;
                    rptRewards.DataBind();
                    pnlRewards.Visible = true;
                    pnlEmpty.Visible = false;
                }
                else
                {
                    pnlRewards.Visible = false;
                    pnlEmpty.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadRewards error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("Failed to load rewards.");
            }
        }

        protected void rptRewards_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                int rewardId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName)
                {
                    case "Edit":
                        LoadRewardForEdit(rewardId);
                        break;

                    case "View":
                        LoadRewardForView(rewardId);
                        break;

                    case "Delete":
                        // Handled by confirmation modal
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("rptRewards_ItemCommand error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred.");
            }
        }

        protected void rptRewards_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                int rewardId = Convert.ToInt32(row["Id"]);

                Button btnEdit = (Button)e.Item.FindControl("btnEdit");
                Button btnDelete = (Button)e.Item.FindControl("btnDelete");
                Literal litInUseBadge = (Literal)e.Item.FindControl("litInUseBadge");

                // Check if reward has checked-out orders
                bool hasCheckedOutOrders = RewardHelper.HasCheckedOutOrders(rewardId);

                if (hasCheckedOutOrders)
                {
                    // Show "In Use" badge
                    if (litInUseBadge != null)
                    {
                        litInUseBadge.Text = "<span class='badge badge-in-use'>In Use</span>";
                    }

                    // Disable edit/delete buttons
                    if (btnEdit != null)
                    {
                        btnEdit.Enabled = false;
                        btnEdit.ToolTip = "Cannot edit: Reward is in a checked-out order";
                    }
                    if (btnDelete != null)
                    {
                        btnDelete.Enabled = false;
                        btnDelete.ToolTip = "Cannot delete: Reward is in a checked-out order";
                    }
                }
            }
        }

        protected void btnCreateRewardSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int? familyId = FamilyHelper.GetUserFamilyId(userId);

                if (!familyId.HasValue)
                {
                    ShowError("You must be in a family to create rewards.");
                    return;
                }

                string name = txtCreateName.Text.Trim();
                string description = txtCreateDescription.Text.Trim();
                int pointCost = Convert.ToInt32(txtCreatePointCost.Text);
                string category = ddlCreateCategory.SelectedValue;
                string imageUrl = txtCreateImageUrl.Text.Trim();

                if (string.IsNullOrEmpty(name))
                {
                    ShowError("Reward name is required.");
                    return;
                }

                if (pointCost <= 0)
                {
                    ShowError("Point cost must be greater than 0.");
                    return;
                }

                bool success = RewardHelper.CreateReward(familyId.Value, userId, name, 
                    string.IsNullOrEmpty(description) ? null : description,
                    pointCost,
                    string.IsNullOrEmpty(category) ? null : category,
                    string.IsNullOrEmpty(imageUrl) ? null : imageUrl);

                if (success)
                {
                    ShowSuccess("Reward created successfully!");
                    LoadRewards();
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseCreateModal", "closeCreateModal();", true);
                }
                else
                {
                    ShowError("Failed to create reward. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnCreateRewardSubmit_Click error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred while creating the reward.");
            }
        }

        protected void btnEditRewardSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                int rewardId = Convert.ToInt32(hidEditRewardId.Value);
                int userId = Convert.ToInt32(Session["UserId"]);

                string name = txtEditName.Text.Trim();
                string description = txtEditDescription.Text.Trim();
                int pointCost = Convert.ToInt32(txtEditPointCost.Text);
                string category = ddlEditCategory.SelectedValue;
                string imageUrl = txtEditImageUrl.Text.Trim();

                if (string.IsNullOrEmpty(name))
                {
                    ShowError("Reward name is required.");
                    return;
                }

                if (pointCost <= 0)
                {
                    ShowError("Point cost must be greater than 0.");
                    return;
                }

                bool success = RewardHelper.UpdateReward(rewardId, userId, name,
                    string.IsNullOrEmpty(description) ? null : description,
                    pointCost,
                    string.IsNullOrEmpty(category) ? null : category,
                    string.IsNullOrEmpty(imageUrl) ? null : imageUrl);

                if (success)
                {
                    ShowSuccess("Reward updated successfully!");
                    LoadRewards();
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseEditModal", "closeEditModal();", true);
                }
                else
                {
                    ShowError("Cannot edit reward. This reward is in an order that has been checked out.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnEditRewardSubmit_Click error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred while updating the reward.");
            }
        }

        protected void btnConfirmDeleteReward_Click(object sender, EventArgs e)
        {
            try
            {
                int rewardId = Convert.ToInt32(deleteRewardId.Value);
                int userId = Convert.ToInt32(Session["UserId"]);

                bool success = RewardHelper.DeleteReward(rewardId, userId);

                if (success)
                {
                    ShowSuccess("Reward deleted successfully!");
                    LoadRewards();
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseDeleteModal", "closeDeleteRewardModal();", true);
                }
                else
                {
                    ShowError("Cannot delete reward. This reward is in an order that has been checked out.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnConfirmDeleteReward_Click error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred while deleting the reward.");
            }
        }

        private void LoadRewardForEdit(int rewardId)
        {
            try
            {
                DataRow reward = RewardHelper.GetRewardDetails(rewardId);
                if (reward == null)
                {
                    ShowError("Reward not found.");
                    return;
                }

                // Check if reward has checked-out orders
                if (RewardHelper.HasCheckedOutOrders(rewardId))
                {
                    ShowError("Cannot edit reward. This reward is in an order that has been checked out.");
                    return;
                }

                hidEditRewardId.Value = rewardId.ToString();
                txtEditName.Text = reward["Name"].ToString();
                txtEditDescription.Text = reward["Description"] != DBNull.Value ? reward["Description"].ToString() : "";
                txtEditPointCost.Text = reward["PointCost"].ToString();
                ddlEditCategory.SelectedValue = reward["Category"] != DBNull.Value ? reward["Category"].ToString() : "";
                txtEditImageUrl.Text = reward["ImageUrl"] != DBNull.Value ? reward["ImageUrl"].ToString() : "";

                ScriptManager.RegisterStartupScript(this, GetType(), "OpenEditModal", "openEditModal();", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadRewardForEdit error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("Failed to load reward for editing.");
            }
        }

        private void LoadRewardForView(int rewardId)
        {
            try
            {
                DataRow reward = RewardHelper.GetRewardDetails(rewardId);
                if (reward == null)
                {
                    ShowError("Reward not found.");
                    return;
                }

                string html = "<div style='margin-bottom: 20px;'>";
                html += "<h3 style='margin-bottom: 10px;'>" + Server.HtmlEncode(reward["Name"].ToString()) + "</h3>";
                
                if (reward["ImageUrl"] != DBNull.Value && !string.IsNullOrEmpty(reward["ImageUrl"].ToString()))
                {
                    html += "<img src='" + Server.HtmlEncode(reward["ImageUrl"].ToString()) + "' alt='Reward Image' style='width: 100%; max-height: 300px; object-fit: cover; border-radius: 5px; margin-bottom: 15px;' />";
                }
                
                html += "<div style='font-size: 24px; font-weight: bold; color: #FF6600; margin-bottom: 15px;'>" + reward["PointCost"] + " points</div>";
                
                if (reward["Description"] != DBNull.Value && !string.IsNullOrEmpty(reward["Description"].ToString()))
                {
                    html += "<div style='color: #666; margin-bottom: 15px; line-height: 1.5;'>" + Server.HtmlEncode(reward["Description"].ToString()) + "</div>";
                }
                
                if (reward["Category"] != DBNull.Value && !string.IsNullOrEmpty(reward["Category"].ToString()))
                {
                    html += "<div style='margin-bottom: 10px;'><strong>Category:</strong> " + Server.HtmlEncode(reward["Category"].ToString()) + "</div>";
                }
                
                html += "<div style='margin-bottom: 10px;'><strong>Created:</strong> " + Convert.ToDateTime(reward["CreatedDate"]).ToString("MMM dd, yyyy") + "</div>";
                html += "</div>";

                litViewReward.Text = html;
                ScriptManager.RegisterStartupScript(this, GetType(), "OpenViewModal", "openViewModal();", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadRewardForView error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("Failed to load reward details.");
            }
        }

        private void ShowSuccess(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccess", 
                string.Format("showMessage('success', '{0}');", message.Replace("'", "\\'")), true);
        }

        private void ShowError(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError", 
                string.Format("showMessage('error', '{0}');", message.Replace("'", "\\'")), true);
        }
    }
}

