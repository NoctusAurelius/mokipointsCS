using System;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class Error404 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Set response status code
            Response.StatusCode = 404;
            Response.StatusDescription = "Not Found";
        }
    }
}

