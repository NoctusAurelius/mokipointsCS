using System;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Dashboard.aspx Page_Load called at " + DateTime.Now.ToString());
            System.Diagnostics.Debug.WriteLine("Session UserId: " + (Session["UserId"] != null ? Session["UserId"].ToString() : "NULL"));
            System.Diagnostics.Debug.WriteLine("Session UserRole: " + (Session["UserRole"] != null ? Session["UserRole"].ToString() : "NULL"));
            
            try
            {
                // Check if user is authenticated
                if (Session["UserId"] == null || Session["UserRole"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("Dashboard: User not authenticated - redirecting to Login");
                    System.Diagnostics.Debug.WriteLine("Session state: UserId=" + (Session["UserId"] != null ? Session["UserId"].ToString() : "NULL") + ", UserRole=" + (Session["UserRole"] != null ? Session["UserRole"].ToString() : "NULL"));
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                // Get user role
                string role = Session["UserRole"].ToString();
                System.Diagnostics.Debug.WriteLine("Dashboard: User role is " + role);

                // Redirect based on role
                if (role == "PARENT")
                {
                    System.Diagnostics.Debug.WriteLine("Dashboard: Redirecting PARENT to ParentDashboard");
                    Response.Redirect("ParentDashboard.aspx", false);
                }
                else if (role == "CHILD")
                {
                    // Check if child is in a family
                    int userId = Convert.ToInt32(Session["UserId"]);
                    int? familyId = FamilyHelper.GetUserFamilyId(userId);
                    
                    System.Diagnostics.Debug.WriteLine("Dashboard: CHILD userId=" + userId + ", familyId=" + (familyId.HasValue ? familyId.Value.ToString() : "NULL"));
                    
                    if (!familyId.HasValue)
                    {
                        // Not in a family - redirect to join family page
                        System.Diagnostics.Debug.WriteLine("Dashboard: CHILD not in family - redirecting to JoinFamily");
                        Response.Redirect("JoinFamily.aspx", false);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Dashboard: CHILD in family - redirecting to ChildDashboard");
                        Response.Redirect("ChildDashboard.aspx", false);
                    }
                }
                else
                {
                    // Invalid role, redirect to login
                    System.Diagnostics.Debug.WriteLine("Dashboard: Invalid role - clearing session and redirecting to Login");
                    Session.Clear();
                    Response.Redirect("Login.aspx", false);
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // This is normal for Response.Redirect - don't treat as error
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Dashboard redirect error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                Response.Redirect("Login.aspx", false);
            }
        }
    }
}

