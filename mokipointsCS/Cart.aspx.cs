using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class Cart : System.Web.UI.Page
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
                    LoadCart();
                }
                else
                {
                    // Handle postback for quantity updates
                    string eventTarget = Request["__EVENTTARGET"];
                    string eventArgument = Request["__EVENTARGUMENT"];
                    
                    if (eventTarget == "UpdateQuantity" && !string.IsNullOrEmpty(eventArgument))
                    {
                        string[] parts = eventArgument.Split('|');
                        if (parts.Length == 2)
                        {
                            int rewardId = Convert.ToInt32(parts[0]);
                            int quantity = Convert.ToInt32(parts[1]);
                            UpdateCartQuantity(rewardId, quantity);
                            LoadCart();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Cart Page_Load error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred. Please try again.");
            }
        }

        private void LoadCart()
        {
            try
            {
                Dictionary<int, int> cart = Session["Cart"] as Dictionary<int, int>;
                int userId = Convert.ToInt32(Session["UserId"]);
                int pointsBalance = PointHelper.GetChildBalance(userId);

                if (cart == null || cart.Count == 0)
                {
                    pnlCart.Visible = false;
                    pnlEmpty.Visible = true;
                    return;
                }

                // Build cart items DataTable
                DataTable cartItems = new DataTable();
                cartItems.Columns.Add("RewardId", typeof(int));
                cartItems.Columns.Add("Name", typeof(string));
                cartItems.Columns.Add("PointCost", typeof(int));
                cartItems.Columns.Add("Quantity", typeof(int));
                cartItems.Columns.Add("Subtotal", typeof(int));
                cartItems.Columns.Add("ImageUrl", typeof(string));

                int totalPoints = 0;

                foreach (var item in cart)
                {
                    int rewardId = item.Key;
                    int quantity = item.Value;

                    DataRow reward = RewardHelper.GetRewardDetails(rewardId);
                    if (reward != null)
                    {
                        int pointCost = Convert.ToInt32(reward["PointCost"]);
                        int subtotal = pointCost * quantity;
                        totalPoints += subtotal;

                        DataRow row = cartItems.NewRow();
                        row["RewardId"] = rewardId;
                        row["Name"] = reward["Name"];
                        row["PointCost"] = pointCost;
                        row["Quantity"] = quantity;
                        row["Subtotal"] = subtotal;
                        row["ImageUrl"] = reward["ImageUrl"] != DBNull.Value ? reward["ImageUrl"] : "";
                        cartItems.Rows.Add(row);
                    }
                }

                if (cartItems.Rows.Count > 0)
                {
                    rptCartItems.DataSource = cartItems;
                    rptCartItems.DataBind();
                    pnlCart.Visible = true;
                    pnlEmpty.Visible = false;

                    litSubtotal.Text = totalPoints + " points";
                    litTotal.Text = totalPoints + " points";

                    // Check if child has enough points
                    if (pointsBalance < totalPoints)
                    {
                        pnlInsufficientPoints.Visible = true;
                        litPointsNeeded.Text = (totalPoints - pointsBalance).ToString();
                        btnCheckout.Enabled = false;
                    }
                    else
                    {
                        pnlInsufficientPoints.Visible = false;
                        btnCheckout.Enabled = true;
                    }
                }
                else
                {
                    pnlCart.Visible = false;
                    pnlEmpty.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadCart error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("Failed to load cart.");
            }
        }

        protected void rptCartItems_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                int rewardId = Convert.ToInt32(e.CommandArgument);

                switch (e.CommandName)
                {
                    case "Remove":
                        RemoveFromCart(rewardId);
                        LoadCart();
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("rptCartItems_ItemCommand error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred.");
            }
        }

        protected void rptCartItems_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                int subtotal = Convert.ToInt32(row["Subtotal"]);

                Literal litSubtotal = (Literal)e.Item.FindControl("litSubtotal");
                if (litSubtotal != null)
                {
                    litSubtotal.Text = subtotal + " points";
                }
            }
        }

        private void UpdateCartQuantity(int rewardId, int quantity)
        {
            Dictionary<int, int> cart = Session["Cart"] as Dictionary<int, int>;
            if (cart != null && cart.ContainsKey(rewardId))
            {
                if (quantity <= 0)
                {
                    cart.Remove(rewardId);
                }
                else
                {
                    cart[rewardId] = quantity;
                }
                Session["Cart"] = cart;
            }
        }

        private void RemoveFromCart(int rewardId)
        {
            Dictionary<int, int> cart = Session["Cart"] as Dictionary<int, int>;
            if (cart != null && cart.ContainsKey(rewardId))
            {
                cart.Remove(rewardId);
                Session["Cart"] = cart;
                ShowSuccess("Item removed from cart.");
            }
        }

        protected void btnCheckout_Click(object sender, EventArgs e)
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                int? familyId = FamilyHelper.GetUserFamilyId(userId);

                if (!familyId.HasValue)
                {
                    ShowError("You must be in a family to checkout.");
                    return;
                }

                Dictionary<int, int> cart = Session["Cart"] as Dictionary<int, int>;
                if (cart == null || cart.Count == 0)
                {
                    ShowError("Your cart is empty.");
                    return;
                }

                // Calculate total
                int totalPoints = 0;
                foreach (var item in cart)
                {
                    DataRow reward = RewardHelper.GetRewardDetails(item.Key);
                    if (reward != null)
                    {
                        totalPoints += Convert.ToInt32(reward["PointCost"]) * item.Value;
                    }
                }

                // Check if child can afford
                if (!PointHelper.CanAffordPurchase(userId, totalPoints))
                {
                    ShowError("You don't have enough points for this purchase.");
                    return;
                }

                // Convert cart dictionary to CartItem list
                List<RewardHelper.CartItem> cartItems = new List<RewardHelper.CartItem>();
                foreach (var item in cart)
                {
                    DataRow reward = RewardHelper.GetRewardDetails(item.Key);
                    if (reward != null)
                    {
                        cartItems.Add(new RewardHelper.CartItem
                        {
                            RewardId = item.Key,
                            Quantity = item.Value,
                            PointCost = Convert.ToInt32(reward["PointCost"])
                        });
                    }
                }

                // Create order
                string orderNumber;
                int orderId = RewardHelper.CreateOrder(userId, familyId.Value, cartItems, out orderNumber);

                if (orderId > 0)
                {
                    // Clear cart
                    Session["Cart"] = null;
                    ShowSuccess("Order placed successfully! Waiting for parent approval.");
                    Response.Redirect("MyOrders.aspx?orderCreated=true");
                }
                else
                {
                    ShowError("Failed to create order. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnCheckout_Click error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred while processing your order.");
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

