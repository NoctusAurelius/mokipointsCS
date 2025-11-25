using System;
using System.Data;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class RewardOrders : System.Web.UI.Page
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
                    
                    LoadOrders();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("RewardOrders Page_Load error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred. Please try again.");
            }
        }

        private void LoadOrders()
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

                DataTable orders = RewardHelper.GetFamilyOrders(familyId.Value);

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
                    case "Confirm":
                        // Handled by confirmation modal
                        break;

                    case "Decline":
                        // Handled by confirmation modal
                        break;

                    case "Fulfill":
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

                // Show refund code to parents when order is confirmed (WaitingToFulfill, Fulfilled, or Refunded)
                Panel pnlRefundCode = (Panel)e.Item.FindControl("pnlRefundCode");
                Literal litRefundCode = (Literal)e.Item.FindControl("litRefundCode");
                
                if (status == "WaitingToFulfill" || status == "Fulfilled" || status == "Refunded" || status == "NotFulfilled") // Support legacy status
                {
                    if (pnlRefundCode != null && litRefundCode != null)
                    {
                        string refundCode = row["RefundCode"] != DBNull.Value ? row["RefundCode"].ToString() : "";
                        if (!string.IsNullOrEmpty(refundCode))
                        {
                            litRefundCode.Text = refundCode;
                            pnlRefundCode.Visible = true;
                        }
                        else
                        {
                            pnlRefundCode.Visible = false;
                        }
                    }
                }
                else
                {
                    if (pnlRefundCode != null)
                    {
                        pnlRefundCode.Visible = false;
                    }
                }

                // Show/hide action buttons based on status
                Button btnConfirm = (Button)e.Item.FindControl("btnConfirm");
                Button btnDecline = (Button)e.Item.FindControl("btnDecline");
                Button btnFulfill = (Button)e.Item.FindControl("btnFulfill");

                if (status == "Pending")
                {
                    if (btnConfirm != null) btnConfirm.Visible = true;
                    if (btnDecline != null) btnDecline.Visible = true;
                    if (btnFulfill != null) btnFulfill.Visible = false;
                }
                else if (status == "WaitingToFulfill")
                {
                    if (btnConfirm != null) btnConfirm.Visible = false;
                    if (btnDecline != null) btnDecline.Visible = false;
                    if (btnFulfill != null) btnFulfill.Visible = true;
                }
                else
                {
                    if (btnConfirm != null) btnConfirm.Visible = false;
                    if (btnDecline != null) btnDecline.Visible = false;
                    if (btnFulfill != null) btnFulfill.Visible = false;
                }
            }
        }

        protected void btnConfirmOrder_Click(object sender, EventArgs e)
        {
            try
            {
                int orderId = Convert.ToInt32(confirmOrderId.Value);
                int userId = Convert.ToInt32(Session["UserId"]);

                string refundCode;
                bool success = RewardHelper.ConfirmOrder(orderId, userId, out refundCode);

                if (success)
                {
                    ShowSuccess("Order confirmed successfully! Points have been deducted and refund code generated.");
                    LoadOrders();
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseConfirmModal", "closeConfirmOrderModal();", true);
                }
                else
                {
                    ShowError("Failed to confirm order. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnConfirmOrder_Click error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred while confirming the order.");
            }
        }

        protected void btnDeclineOrder_Click(object sender, EventArgs e)
        {
            try
            {
                int orderId = Convert.ToInt32(declineOrderId.Value);
                int userId = Convert.ToInt32(Session["UserId"]);

                bool success = RewardHelper.DeclineOrder(orderId, userId);

                if (success)
                {
                    ShowSuccess("Order declined successfully.");
                    LoadOrders();
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseDeclineModal", "closeDeclineOrderModal();", true);
                }
                else
                {
                    ShowError("Failed to decline order. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnDeclineOrder_Click error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred while declining the order.");
            }
        }

        protected void btnFulfillOrder_Click(object sender, EventArgs e)
        {
            try
            {
                int orderId = Convert.ToInt32(fulfillOrderId.Value);
                int userId = Convert.ToInt32(Session["UserId"]);

                bool success = RewardHelper.FulfillOrder(orderId, userId);

                if (success)
                {
                    ShowSuccess("Order marked as fulfilled successfully.");
                    LoadOrders();
                    ScriptManager.RegisterStartupScript(this, GetType(), "CloseFulfillModal", "closeFulfillOrderModal();", true);
                }
                else
                {
                    ShowError("Failed to mark order as fulfilled. Please try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnFulfillOrder_Click error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred while marking the order as fulfilled.");
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
    }
}

