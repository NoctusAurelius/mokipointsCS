using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class Notifications : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check authentication
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);

            if (!IsPostBack)
            {
                LoadNotifications(userId);
            }

            // Handle success/error messages from redirects
            if (Request.QueryString["success"] != null)
            {
                ShowSuccess(Request.QueryString["success"]);
            }
            if (Request.QueryString["error"] != null)
            {
                ShowError(Request.QueryString["error"]);
            }
        }

        protected void LoadNotifications(int userId)
        {
            try
            {
                DataTable notifications = TaskHelper.GetUserNotifications(userId, false);
                int unreadCount = TaskHelper.GetUnreadNotificationCount(userId);

                // Update notification count
                litNotificationCount.Text = string.Format("You have {0} unread notification{1}", 
                    unreadCount, unreadCount != 1 ? "s" : "");

                if (notifications != null && notifications.Rows.Count > 0)
                {
                    rptNotifications.DataSource = notifications;
                    rptNotifications.DataBind();
                    pnlNotifications.Visible = true;
                    pnlEmpty.Visible = false;
                }
                else
                {
                    pnlNotifications.Visible = false;
                    pnlEmpty.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadNotifications error: " + ex.Message);
                ShowError("Error loading notifications.");
                pnlNotifications.Visible = false;
                pnlEmpty.Visible = true;
            }
        }

        protected void rptNotifications_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "MarkRead")
            {
                int notificationId = Convert.ToInt32(e.CommandArgument);
                int userId = Convert.ToInt32(Session["UserId"]);

                bool success = TaskHelper.MarkNotificationRead(notificationId, userId);

                if (success)
                {
                    LoadNotifications(userId);
                    ShowSuccess("Notification marked as read.");
                }
                else
                {
                    ShowError("Failed to mark notification as read.");
                }
            }
        }

        protected void rptNotifications_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                bool isRead = Convert.ToBoolean(row["IsRead"]);

                // Find and configure Mark Read button
                Button btnMarkRead = (Button)e.Item.FindControl("btnMarkRead");
                if (btnMarkRead != null)
                {
                    btnMarkRead.Visible = !isRead;
                }
            }
        }

        protected void btnMarkAllRead_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["UserId"]);

            bool success = TaskHelper.MarkAllNotificationsRead(userId);

            if (success)
            {
                LoadNotifications(userId);
                ShowSuccess("All notifications marked as read.");
            }
            else
            {
                ShowError("Failed to mark all notifications as read.");
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }

        private void ShowSuccess(string message)
        {
            lblSuccess.Text = message;
            pnlSuccess.CssClass = "success-message";
            pnlSuccess.Visible = true;
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            pnlError.CssClass = "error-message";
            pnlError.Visible = true;
        }
    }
}

