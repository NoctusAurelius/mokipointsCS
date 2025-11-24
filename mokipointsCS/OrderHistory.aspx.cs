using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class OrderHistory : System.Web.UI.Page
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

                // Check role - both parents and children can access
                if (Session["UserRole"] == null)
                {
                    Response.Redirect("Dashboard.aspx");
                    return;
                }

                string userRole = Session["UserRole"].ToString();
                if (userRole != "PARENT" && userRole != "CHILD")
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

                // Set navigation based on role
                SetNavigation(userRole);

                if (!IsPostBack)
                {
                    LoadOrderHistory();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("OrderHistory Page_Load error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("An error occurred. Please try again.");
            }
        }

        private void SetNavigation(string userRole)
        {
            if (userRole == "PARENT")
            {
                litNavigation.Text = @"
                    <a href=""ParentDashboard.aspx"">Dashboard</a>
                    <a href=""Tasks.aspx"">Tasks</a>
                    <a href=""Rewards.aspx"">Rewards</a>
                    <a href=""RewardOrders.aspx"">Orders</a>
                    <a href=""OrderHistory.aspx"" class=""active"">Order History</a>
                    <a href=""Family.aspx"">Family</a>
                ";
            }
            else // CHILD
            {
                litNavigation.Text = @"
                    <a href=""ChildDashboard.aspx"">Dashboard</a>
                    <a href=""ChildTasks.aspx"">Tasks</a>
                    <a href=""RewardShop.aspx"">Reward Shop</a>
                    <a href=""Cart.aspx"">Cart</a>
                    <a href=""MyOrders.aspx"">My Orders</a>
                    <a href=""OrderHistory.aspx"" class=""active"">Order History</a>
                    <a href=""PointsHistory.aspx"">Points History</a>
                ";
            }
        }

        private void LoadOrderHistory()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                string userRole = Session["UserRole"].ToString();
                DataTable orders;

                DateTime? startDate = null;
                DateTime? endDate = null;

                // Get date filters if set
                if (!string.IsNullOrEmpty(txtStartDate.Text))
                {
                    if (DateTime.TryParse(txtStartDate.Text, out DateTime start))
                    {
                        startDate = start;
                    }
                }

                if (!string.IsNullOrEmpty(txtEndDate.Text))
                {
                    if (DateTime.TryParse(txtEndDate.Text, out DateTime end))
                    {
                        endDate = end;
                    }
                }

                if (userRole == "PARENT")
                {
                    int? familyId = FamilyHelper.GetUserFamilyId(userId);
                    if (!familyId.HasValue)
                    {
                        Response.Redirect("Family.aspx");
                        return;
                    }

                    orders = RewardHelper.GetFamilyOrderHistory(familyId.Value, startDate, endDate);
                }
                else // CHILD
                {
                    orders = RewardHelper.GetChildOrderHistory(userId, startDate, endDate);
                }

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
                System.Diagnostics.Debug.WriteLine("LoadOrderHistory error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                ShowError("Failed to load order history.");
            }
        }

        protected void rptOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                int orderId = Convert.ToInt32(row["Id"]);
                string userRole = Session["UserRole"].ToString();

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

                // Show child info for parents
                if (userRole == "PARENT")
                {
                    Panel pnlChildInfo = (Panel)e.Item.FindControl("pnlChildInfo");
                    Literal litChildName = (Literal)e.Item.FindControl("litChildName");
                    if (pnlChildInfo != null && litChildName != null)
                    {
                        litChildName.Text = row["ChildName"].ToString();
                        pnlChildInfo.Visible = true;
                    }
                }

                // Show refund code for parents (if exists)
                if (userRole == "PARENT")
                {
                    Panel pnlRefundCode = (Panel)e.Item.FindControl("pnlRefundCode");
                    Literal litRefundCode = (Literal)e.Item.FindControl("litRefundCode");
                    if (pnlRefundCode != null && litRefundCode != null)
                    {
                        if (row["RefundCode"] != DBNull.Value && !string.IsNullOrEmpty(row["RefundCode"].ToString()))
                        {
                            litRefundCode.Text = row["RefundCode"].ToString();
                            pnlRefundCode.Visible = true;
                        }
                    }
                }

                // Show timeline dates
                Panel pnlConfirmed = (Panel)e.Item.FindControl("pnlConfirmed");
                Literal litConfirmedDate = (Literal)e.Item.FindControl("litConfirmedDate");
                if (pnlConfirmed != null && litConfirmedDate != null)
                {
                    if (row["ConfirmedDate"] != DBNull.Value)
                    {
                        DateTime confirmedDate = Convert.ToDateTime(row["ConfirmedDate"]);
                        litConfirmedDate.Text = confirmedDate.ToString("MMM dd, yyyy HH:mm");
                        pnlConfirmed.Visible = true;
                    }
                }

                Panel pnlFulfilled = (Panel)e.Item.FindControl("pnlFulfilled");
                Literal litFulfilledDate = (Literal)e.Item.FindControl("litFulfilledDate");
                if (pnlFulfilled != null && litFulfilledDate != null)
                {
                    if (row["FulfilledDate"] != DBNull.Value)
                    {
                        DateTime fulfilledDate = Convert.ToDateTime(row["FulfilledDate"]);
                        litFulfilledDate.Text = fulfilledDate.ToString("MMM dd, yyyy HH:mm");
                        pnlFulfilled.Visible = true;
                    }
                }

                Panel pnlChildConfirmed = (Panel)e.Item.FindControl("pnlChildConfirmed");
                Literal litChildConfirmedDate = (Literal)e.Item.FindControl("litChildConfirmedDate");
                if (pnlChildConfirmed != null && litChildConfirmedDate != null)
                {
                    if (row["ChildConfirmedDate"] != DBNull.Value)
                    {
                        DateTime childConfirmedDate = Convert.ToDateTime(row["ChildConfirmedDate"]);
                        litChildConfirmedDate.Text = childConfirmedDate.ToString("MMM dd, yyyy HH:mm");
                        pnlChildConfirmed.Visible = true;
                    }
                }
            }
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            LoadOrderHistory();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            LoadOrderHistory();
        }

        private void ShowError(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowError", string.Format("showMessage('error', '{0}');", message.Replace("'", "\\'")), true);
        }

        private void ShowSuccess(string message)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "ShowSuccess", string.Format("showMessage('success', '{0}');", message.Replace("'", "\\'")), true);
        }
    }
}

