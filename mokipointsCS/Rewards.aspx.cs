using System;
using System.Data;
using System.IO;
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

                int userId = Convert.ToInt32(Session["UserId"]);

                // Check if parent is in a family
                int? familyId = FamilyHelper.GetUserFamilyId(userId);
                if (!familyId.HasValue)
                {
                    // Not in a family - show message panel
                    pnlNoFamily.Visible = true;
                    pnlSearchFilter.Visible = false;
                    pnlRewards.Visible = false;
                    pnlEmpty.Visible = false;
                    return;
                }
                
                // Hide no family message if user has family
                pnlNoFamily.Visible = false;
                pnlSearchFilter.Visible = true;

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

                    case "UpdateAvailability":
                        UpdateAvailabilityStatus(e);
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
                Literal litAvailabilityBadge = (Literal)e.Item.FindControl("litAvailabilityBadge");
                DropDownList ddlAvailabilityStatus = (DropDownList)e.Item.FindControl("ddlAvailabilityStatus");
                Literal litAvailabilityError = (Literal)e.Item.FindControl("litAvailabilityError");

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
                    
                    // Disable availability status dropdown
                    if (ddlAvailabilityStatus != null)
                    {
                        ddlAvailabilityStatus.Enabled = false;
                        ddlAvailabilityStatus.ToolTip = "Cannot change status: Reward is in a checked-out order";
                        if (litAvailabilityError != null)
                        {
                            litAvailabilityError.Text = "<span class='availability-error'>Cannot change status: Reward has existing orders</span>";
                        }
                    }
                }

                // Set availability status badge and dropdown
                string availabilityStatus = row["AvailabilityStatus"] != DBNull.Value ? row["AvailabilityStatus"].ToString() : "Available";
                
                if (litAvailabilityBadge != null)
                {
                    string badgeClass = "badge-availability ";
                    string badgeText = "";
                    switch (availabilityStatus)
                    {
                        case "Available":
                            badgeClass += "badge-available";
                            badgeText = "Available";
                            break;
                        case "OutOfStock":
                            badgeClass += "badge-outofstock";
                            badgeText = "Out of Stock";
                            break;
                        case "Hidden":
                            badgeClass += "badge-hidden";
                            badgeText = "Hidden";
                            break;
                    }
                    litAvailabilityBadge.Text = string.Format("<span class='{0}'>{1}</span>", badgeClass, badgeText);
                }

                if (ddlAvailabilityStatus != null)
                {
                    ddlAvailabilityStatus.SelectedValue = availabilityStatus;
                }
            }
        }

        protected void btnCreateRewardSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate page before processing (Fix #1)
                if (!Page.IsValid)
                {
                    ShowError("Please fill in all required fields correctly.");
                    // Don't close modal - keep it open to show errors
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                int? familyId = FamilyHelper.GetUserFamilyId(userId);

                if (!familyId.HasValue)
                {
                    ShowError("You must be in a family to create rewards.");
                    return; // Keep modal open
                }

                string name = txtCreateName.Text.Trim();
                string description = txtCreateDescription.Text.Trim();
                
                // Validate point cost with proper error handling (Fix #1)
                int pointCost = 0;
                if (string.IsNullOrEmpty(txtCreatePointCost.Text.Trim()))
                {
                    ShowError("Point cost is required.");
                    return; // Keep modal open
                }
                
                if (!int.TryParse(txtCreatePointCost.Text.Trim(), out pointCost))
                {
                    ShowError("Point cost must be a valid number.");
                    return; // Keep modal open
                }
                
                if (pointCost <= 0)
                {
                    ShowError("Point cost must be greater than 0.");
                    return; // Keep modal open
                }
                
                if (pointCost > 10000)
                {
                    ShowError("Point cost cannot exceed 10,000. Please enter a value between 1 and 10,000.");
                    return; // Keep modal open
                }
                
                string category = ddlCreateCategory.SelectedValue;
                string imageUrl = txtCreateImageUrl.Text.Trim();

                if (string.IsNullOrEmpty(name))
                {
                    ShowError("Reward name is required.");
                    return; // Keep modal open
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
                    // Only close modal on success
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseCreateModal", "closeCreateModal();", true);
                }
                else
                {
                    ShowError("Failed to create reward. Please try again.");
                    // Don't close modal - keep it open to show error
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnCreateRewardSubmit_Click error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred while creating the reward: " + ex.Message);
                // Don't close modal - keep it open to show error
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

        private void UpdateAvailabilityStatus(RepeaterCommandEventArgs e)
        {
            try
            {
                int rewardId = Convert.ToInt32(e.CommandArgument);
                int userId = Convert.ToInt32(Session["UserId"]);

                // Find the dropdown in the repeater item
                DropDownList ddlAvailabilityStatus = (DropDownList)e.Item.FindControl("ddlAvailabilityStatus");
                if (ddlAvailabilityStatus == null)
                {
                    ShowError("Could not find availability status dropdown.");
                    return;
                }

                string availabilityStatus = ddlAvailabilityStatus.SelectedValue;

                // Validate status
                if (availabilityStatus != "Available" && availabilityStatus != "OutOfStock" && availabilityStatus != "Hidden")
                {
                    ShowError("Invalid availability status.");
                    return;
                }

                // Check if reward has checked-out orders
                if (RewardHelper.HasCheckedOutOrders(rewardId))
                {
                    ShowError("Cannot change availability status. This reward has existing orders.");
                    LoadRewards(); // Reload to refresh UI
                    return;
                }

                // Update availability status
                bool success = RewardHelper.UpdateRewardAvailability(rewardId, availabilityStatus, userId);

                if (success)
                {
                    ShowSuccess("Availability status updated successfully!");
                    LoadRewards(); // Reload to refresh UI
                }
                else
                {
                    ShowError("Failed to update availability status. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("UpdateAvailabilityStatus error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred while updating availability status.");
            }
        }
    }
}

