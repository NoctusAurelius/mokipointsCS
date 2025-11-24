using System;
using System.Data;
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

                // Set user name
                if (Session["UserName"] != null)
                {
                    litUserName.Text = Session["UserName"].ToString();
                }
                else
                {
                    litUserName.Text = "Child";
                }

                // Display points balance
                int pointsBalance = PointHelper.GetChildBalance(userId);
                litPointsBalance.Text = pointsBalance.ToString();

                if (!IsPostBack)
                {
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
                
                DataTable rewards = RewardHelper.GetFamilyRewards(familyId, true);

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

                Button btnAddToCart = (Button)e.Item.FindControl("btnAddToCart");

                // Check if child can afford this reward
                int pointsBalance = PointHelper.GetChildBalance(userId);
                if (pointsBalance < pointCost)
                {
                    if (btnAddToCart != null)
                    {
                        btnAddToCart.Enabled = false;
                        btnAddToCart.Text = "Not Enough Points";
                    }
                    // Add unaffordable class to card
                    System.Web.UI.HtmlControls.HtmlGenericControl cardDiv = (System.Web.UI.HtmlControls.HtmlGenericControl)e.Item.FindControl("rewardCard");
                    if (cardDiv != null)
                    {
                        string currentClass = cardDiv.Attributes["class"] ?? "";
                        cardDiv.Attributes["class"] = currentClass + " unaffordable";
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
    }
}

