using System;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class TermsAndConditions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Check if accessed from Settings
                string fromParam = Request.QueryString["from"];
                if (fromParam == "settings")
                {
                    lnkBack.NavigateUrl = "Settings.aspx";
                    lnkBack.Text = "← Back to Settings";
                }
                else
                {
                    lnkBack.NavigateUrl = "Register.aspx";
                    lnkBack.Text = "← Back to Registration";
                }
            }
        }
    }
}

