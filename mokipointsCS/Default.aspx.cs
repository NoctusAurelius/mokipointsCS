using System;

namespace mokipointsCS
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // If user is authenticated, redirect to dashboard
                if (Session["UserId"] != null && Session["UserRole"] != null)
                {
                    Response.Redirect("Dashboard.aspx");
                    return;
                }
                // Otherwise, show the landing page (already in Default.aspx)
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Default Page_Load error: " + ex.Message);
            }
        }
    }
}

