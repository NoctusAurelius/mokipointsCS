using System;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class Settings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Settings.aspx Page_Load called at " + DateTime.Now.ToString());
            try
            {
                // Check authentication
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("Settings: User not authenticated - redirecting to Login");
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                if (!IsPostBack)
                {
                    System.Diagnostics.Debug.WriteLine("Settings: Loading settings page");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Settings Page_Load error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                Response.Redirect("Login.aspx", false);
            }
        }

        protected void btnProfile_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Settings: Profile button clicked");
            Response.Redirect("Profile.aspx", false);
        }

        protected void btnTerms_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Settings: Terms button clicked");
            Response.Redirect("TermsAndConditions.aspx?from=settings", false);
        }

        protected void btnPrivacy_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Settings: Privacy button clicked");
            Response.Redirect("PrivacyPolicy.aspx?from=settings", false);
        }

        protected void btnNotifications_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Settings: Notifications button clicked");
            // TODO: Route to notifications preference page
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Settings: Logout button clicked");
            try
            {
                Session.Clear();
                Response.Redirect("Login.aspx", false);
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Logout error: " + ex.Message);
                Response.Redirect("Login.aspx", false);
            }
        }
    }
}

