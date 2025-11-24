using System;
using System.Web;
using System.Web.UI;

namespace mokipointsCS
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Disable unobtrusive validation mode to avoid jQuery dependency
            ValidationSettings.UnobtrusiveValidationMode = UnobtrusiveValidationMode.None;
            
            // Initialize database on application startup
            try
            {
                DatabaseInitializer.InitializeDatabase();
            }
            catch (Exception ex)
            {
                // Log error but don't crash the application
                System.Diagnostics.Debug.WriteLine("Database initialization error: " + ex.Message);
            }
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown
        }

        void Application_Error(object sender, EventArgs e)
        {
            // Get the exception
            Exception ex = Server.GetLastError();
            
            // Log the error (in production, log to file/database)
            System.Diagnostics.Debug.WriteLine("Application Error: " + ex.Message);
            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine("Inner Exception: " + ex.InnerException.Message);
                System.Diagnostics.Debug.WriteLine("Inner Stack Trace: " + ex.InnerException.StackTrace);
            }
            System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
            
            // Store error details in Session for error page
            Session["LastError"] = ex.Message;
            Session["LastErrorType"] = ex.GetType().Name;
            if (ex.InnerException != null)
            {
                Session["LastInnerError"] = ex.InnerException.Message;
            }
            Session["LastStackTrace"] = ex.StackTrace;
            Session["LastSource"] = ex.Source;
            Session["LastTargetSite"] = ex.TargetSite != null ? ex.TargetSite.ToString() : "";
            
            // Clear the error to prevent default error page
            Server.ClearError();
            
            // Redirect to appropriate error page based on status code
            HttpException httpEx = ex as HttpException;
            if (httpEx != null)
            {
                int statusCode = httpEx.GetHttpCode();
                switch (statusCode)
                {
                    case 400:
                        Response.Redirect("Error400.aspx");
                        break;
                    case 404:
                        Response.Redirect("Error404.aspx");
                        break;
                    case 500:
                    default:
                        Response.Redirect("Error500.aspx");
                        break;
                }
            }
            else
            {
                // Generic server error
                Response.Redirect("Error500.aspx");
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started
        }

        void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends.
        }
    }
}

