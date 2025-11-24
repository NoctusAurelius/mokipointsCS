using System;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class Error500 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Set response status code
            Response.StatusCode = 500;
            Response.StatusDescription = "Internal Server Error";
            
            // Load error details from Session if available
            if (Session["LastError"] != null)
            {
                pnlErrorDetails.Visible = true;
                litErrorType.Text = Session["LastErrorType"]?.ToString() ?? "Unknown";
                litErrorMessage.Text = Session["LastError"]?.ToString() ?? "No error message available";
                litErrorSource.Text = Session["LastSource"]?.ToString() ?? "Unknown";
                litStackTrace.Text = Session["LastStackTrace"]?.ToString() ?? "No stack trace available";
                
                if (Session["LastInnerError"] != null)
                {
                    pnlInnerError.Visible = true;
                    litInnerError.Text = Session["LastInnerError"].ToString();
                }
                
                // Clear error details from session after displaying
                Session.Remove("LastError");
                Session.Remove("LastErrorType");
                Session.Remove("LastInnerError");
                Session.Remove("LastStackTrace");
                Session.Remove("LastSource");
                Session.Remove("LastTargetSite");
            }
        }
    }
}

