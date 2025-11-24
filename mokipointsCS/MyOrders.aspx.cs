using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class MyOrders : System.Web.UI.Page
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
                    // Check for success message from cart checkout
                    if (Request.QueryString["orderCreated"] == "true")
                    {
                        ShowSuccess("Order placed successfully! Waiting for parent approval.");
                    }

                    LoadOrders(userId);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MyOrders Page_Load error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred. Please try again.");
            }
        }

        private void LoadOrders(int userId)
        {
            try
            {
                DataTable orders = RewardHelper.GetChildOrders(userId);

                if (orders.Rows.Count > 0)
                {
                    rptOrders.DataSource = orders;
                    rptOrders.DataBind();
                    pnlOrders.Visible = true;
                    pnlEmpty.Visible = false;
                }
                else
                {
                    pnlOrders.Visible = false;
                    pnlEmpty.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadOrders error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("Failed to load orders.");
            }
        }

        protected void rptOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            try
            {
                int orderId = Convert.ToInt32(e.CommandArgument);
                int userId = Convert.ToInt32(Session["UserId"]);

                switch (e.CommandName)
                {
                    case "ConfirmFulfillment":
                        // Handled by confirmation modal
                        break;

                    case "ClaimNotFulfilled":
                        // Handled by confirmation modal
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("rptOrders_ItemCommand error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred.");
            }
        }

        protected void rptOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                int orderId = Convert.ToInt32(row["Id"]);
                string status = row["Status"].ToString();

                // Load order items
                Repeater rptOrderItems = (Repeater)e.Item.FindControl("rptOrderItems");
                if (rptOrderItems != null)
                {
                    DataTable items = RewardHelper.GetOrderItems(orderId);
                    if (items.Rows.Count > 0)
                    {
                        rptOrderItems.DataSource = items;
                        rptOrderItems.DataBind();
                    }
                }

                // Hide refund code from children - only parents should see it
                Panel pnlRefundCode = (Panel)e.Item.FindControl("pnlRefundCode");
                if (pnlRefundCode != null)
                {
                    pnlRefundCode.Visible = false;
                }

                // Show/hide action buttons based on status
                // Children can only confirm/claim not fulfilled when parent has marked order as "Fulfilled"
                // AND the order hasn't been confirmed yet
                Button btnConfirmFulfillment = (Button)e.Item.FindControl("btnConfirmFulfillment");
                Button btnClaimNotFulfilled = (Button)e.Item.FindControl("btnClaimNotFulfilled");

                // Check if order is already confirmed
                bool isAlreadyConfirmed = row["ChildConfirmedDate"] != DBNull.Value;
                bool isTransactionComplete = status == "TransactionComplete";

                // Only show buttons when:
                // 1. Status is "Fulfilled" (parent has marked it as fulfilled)
                // 2. Order is NOT already confirmed (ChildConfirmedDate is null)
                // 3. Status is NOT "TransactionComplete" (defensive check)
                if (status == "Fulfilled" && !isAlreadyConfirmed && !isTransactionComplete)
                {
                    if (btnConfirmFulfillment != null) btnConfirmFulfillment.Visible = true;
                    if (btnClaimNotFulfilled != null) btnClaimNotFulfilled.Visible = true;
                }
                else
                {
                    if (btnConfirmFulfillment != null) btnConfirmFulfillment.Visible = false;
                    if (btnClaimNotFulfilled != null) btnClaimNotFulfilled.Visible = false;
                }
            }
        }

        protected void btnConfirmFulfillment_Click(object sender, EventArgs e)
        {
            try
            {
                int orderId = Convert.ToInt32(confirmFulfillmentOrderId.Value);
                int userId = Convert.ToInt32(Session["UserId"]);

                // Check order status before attempting confirmation
                DataRow orderDetails = RewardHelper.GetOrderDetails(orderId);
                if (orderDetails == null)
                {
                    ShowError("Order not found. Please refresh the page.");
                    return;
                }

                string status = orderDetails["Status"].ToString();
                bool isAlreadyConfirmed = orderDetails["ChildConfirmedDate"] != DBNull.Value;

                // Provide specific error messages
                if (status != "Fulfilled")
                {
                    ShowError(string.Format("Cannot confirm order. Order status is '{0}'. Only orders marked as 'Fulfilled' by parent can be confirmed.", status));
                    return;
                }

                if (isAlreadyConfirmed)
                {
                    ShowError("This order has already been confirmed. It may have been moved to Order History.");
                    // Reload orders to refresh the list
                    LoadOrders(userId);
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseConfirmModal", "closeConfirmFulfillmentModal();", true);
                    return;
                }

                bool success = RewardHelper.ConfirmFulfillment(orderId, userId);

                if (success)
                {
                    ShowSuccess("Order confirmed successfully! The order has been moved to Order History.");
                    LoadOrders(userId);
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseConfirmModal", "closeConfirmFulfillmentModal();", true);
                }
                else
                {
                    // Check again after the call to see what went wrong
                    DataRow orderAfter = RewardHelper.GetOrderDetails(orderId);
                    if (orderAfter != null)
                    {
                        string statusAfter = orderAfter["Status"].ToString();
                        bool confirmedAfter = orderAfter["ChildConfirmedDate"] != DBNull.Value;
                        
                        if (confirmedAfter || statusAfter == "TransactionComplete")
                        {
                            ShowError("Order was already confirmed. Please refresh the page.");
                            LoadOrders(userId);
                        }
                        else
                        {
                            ShowError("Failed to confirm fulfillment. Please try again or contact support if the problem persists.");
                        }
                    }
                    else
                    {
                        ShowError("Failed to confirm fulfillment. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnConfirmFulfillment_Click error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred while confirming fulfillment. Please try again.");
            }
        }

        protected void btnClaimNotFulfilled_Click(object sender, EventArgs e)
        {
            try
            {
                int orderId = Convert.ToInt32(claimNotFulfilledOrderId.Value);
                int userId = Convert.ToInt32(Session["UserId"]);
                string refundCode = txtRefundCode.Text.Trim();

                if (string.IsNullOrEmpty(refundCode))
                {
                    ShowError("Please enter your refund code.");
                    return;
                }

                // Check order status before attempting refund
                DataRow orderDetails = RewardHelper.GetOrderDetails(orderId);
                if (orderDetails == null)
                {
                    ShowError("Order not found. Please refresh the page.");
                    return;
                }

                string status = orderDetails["Status"].ToString();
                bool isAlreadyConfirmed = orderDetails["ChildConfirmedDate"] != DBNull.Value;

                // Provide specific error messages
                if (status != "Fulfilled")
                {
                    ShowError(string.Format("Cannot claim refund. Order status is '{0}'. Only orders marked as 'Fulfilled' by parent can be refunded.", status));
                    return;
                }

                if (isAlreadyConfirmed)
                {
                    ShowError("This order has already been confirmed. Refunds cannot be processed for confirmed orders.");
                    LoadOrders(userId);
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseClaimModal", "closeClaimNotFulfilledModal();", true);
                    return;
                }

                bool success = RewardHelper.ClaimNotFulfilled(orderId, userId, refundCode);

                if (success)
                {
                    ShowSuccess("Points refunded successfully! The order has been moved to Order History.");
                    LoadOrders(userId);
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseClaimModal", "closeClaimNotFulfilledModal();", true);
                }
                else
                {
                    // Check again after the call to see what went wrong
                    DataRow orderAfter = RewardHelper.GetOrderDetails(orderId);
                    if (orderAfter != null)
                    {
                        string statusAfter = orderAfter["Status"].ToString();
                        if (statusAfter == "Refunded" || statusAfter == "NotFulfilled") // Support both new and legacy status
                        {
                            ShowError("Order was already refunded. Please refresh the page.");
                            LoadOrders(userId);
                        }
                        else
                        {
                            ShowError("Invalid refund code or order cannot be refunded. Please verify the refund code with your parent.");
                        }
                    }
                    else
                    {
                        ShowError("Invalid refund code or order cannot be refunded. Please verify the refund code with your parent.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnClaimNotFulfilled_Click error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred while processing your refund request. Please try again.");
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

