using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class RewardShop : System.Web.UI.Page
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

                // Check role - only children can access this page
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
                    Response.Redirect("JoinFamily.aspx");
                    return;
                }

                // Set user name - load from session or database
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

                if (!IsPostBack)
                {
                    // Load profile picture
                    LoadProfilePicture(userId);
                    
                    LoadRewards(familyId.Value, userId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("RewardShop Page_Load error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred. Please try again.");
            }
        }

        private void LoadRewards(int familyId, int userId)
        {
            try
            {
                int pointsBalance = PointHelper.GetChildBalance(userId);
                
                DataTable rewards = RewardHelper.GetFamilyRewards(familyId, true, forChild: true);

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
                int userId = Convert.ToInt32(Session["UserId"]);

                switch (e.CommandName)
                {
                    case "AddToCart":
                        AddToCart(userId, rewardId);
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
                int pointCost = Convert.ToInt32(row["PointCost"]);
                int userId = Convert.ToInt32(Session["UserId"]);
                string availabilityStatus = row["AvailabilityStatus"] != DBNull.Value ? row["AvailabilityStatus"].ToString() : "Available";

                Button btnAddToCart = (Button)e.Item.FindControl("btnAddToCart");
                Literal litOutOfStockBadge = (Literal)e.Item.FindControl("litOutOfStockBadge");
                System.Web.UI.HtmlControls.HtmlGenericControl cardDiv = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("rewardCard");

                // Check if reward is out of stock
                if (availabilityStatus == "OutOfStock")
                {
                    if (btnAddToCart != null)
                    {
                        btnAddToCart.Enabled = false;
                        btnAddToCart.Text = "Out of Stock";
                    }
                    if (litOutOfStockBadge != null)
                    {
                        litOutOfStockBadge.Text = "<span class='badge badge-outofstock' style='display: inline-block; padding: 4px 10px; border-radius: 12px; font-size: 11px; font-weight: 600; background-color: #fff3cd; color: #856404; margin-left: 8px;'>Out of Stock</span>";
                    }
                    // Add out-of-stock class to card
                    if (cardDiv != null)
                    {
                        string currentClass = cardDiv.Attributes["class"] ?? "";
                        cardDiv.Attributes["class"] = currentClass + " out-of-stock";
                    }
                }
                else
                {
                    // Check if child can afford this reward (only if not out of stock)
                    int pointsBalance = PointHelper.GetChildBalance(userId);
                    if (pointsBalance < pointCost)
                    {
                        if (btnAddToCart != null)
                        {
                            btnAddToCart.Enabled = false;
                            btnAddToCart.Text = "Not Enough Points";
                        }
                        // Add unaffordable class to card
                        if (cardDiv != null)
                        {
                            string currentClass = cardDiv.Attributes["class"] ?? "";
                            cardDiv.Attributes["class"] = currentClass + " unaffordable";
                        }
                    }
                }
            }
        }

        private void AddToCart(int userId, int rewardId)
        {
            try
            {
                // Get reward details
                DataRow reward = RewardHelper.GetRewardDetails(rewardId);
                if (reward == null)
                {
                    ShowError("Reward not found.");
                    return;
                }

                int pointCost = Convert.ToInt32(reward["PointCost"]);

                // Check if child can afford
                if (!PointHelper.CanAffordPurchase(userId, pointCost))
                {
                    ShowError("You don't have enough points for this reward.");
                    return;
                }

                // Add to session cart (we'll implement Cart.aspx next)
                // For now, store in session
                System.Collections.Generic.Dictionary<int, int> cart = Session["Cart"] as System.Collections.Generic.Dictionary<int, int>;
                if (cart == null)
                {
                    cart = new System.Collections.Generic.Dictionary<int, int>();
                    Session["Cart"] = cart;
                }

                if (cart.ContainsKey(rewardId))
                {
                    cart[rewardId] += 1;
                }
                else
                {
                    cart[rewardId] = 1;
                }

                ShowSuccess("Reward added to cart!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AddToCart error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("Failed to add reward to cart.");
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
    }
}

