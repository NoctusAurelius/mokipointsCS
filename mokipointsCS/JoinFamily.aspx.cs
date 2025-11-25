using System;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class JoinFamily : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("JoinFamily Page_Load called at " + DateTime.Now.ToString());
                System.Diagnostics.Debug.WriteLine("IsPostBack: " + IsPostBack);
                
                // Check authentication
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("JoinFamily: User not authenticated - redirecting to Login");
                    Response.Redirect("Login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                string userRole = Session["UserRole"]?.ToString();
                System.Diagnostics.Debug.WriteLine(string.Format("JoinFamily: UserId={0}, Role={1}", userId, userRole ?? "NULL"));

                // Check role - only children should access this page
                if (userRole == null || userRole != "CHILD")
                {
                    System.Diagnostics.Debug.WriteLine("JoinFamily: User is not CHILD - redirecting to Dashboard");
                    Response.Redirect("Dashboard.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // Check if child is already in a family
                int? familyId = FamilyHelper.GetUserFamilyId(userId);
                System.Diagnostics.Debug.WriteLine(string.Format("JoinFamily: FamilyId={0}", familyId.HasValue ? familyId.Value.ToString() : "NULL"));

                if (familyId.HasValue)
                {
                    // Already in a family - redirect to dashboard
                    System.Diagnostics.Debug.WriteLine("JoinFamily: User already in family - redirecting to ChildDashboard");
                    Response.Redirect("ChildDashboard.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                if (!IsPostBack)
                {
                    // Clear messages
                    lblError.Visible = false;
                    lblSuccess.Visible = false;
                    System.Diagnostics.Debug.WriteLine("JoinFamily: Page loaded successfully (not postback)");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // This is expected when Response.Redirect is called - re-throw to allow redirect to complete
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JoinFamily Page_Load error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                // Only redirect if we're not already redirecting
                if (!Response.IsRequestBeingRedirected)
                {
                    Response.Redirect("Login.aspx?error=session", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
        }

        protected void btnJoin_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("JoinFamily btnJoin_Click called at " + DateTime.Now.ToString());
                
                // Clear previous messages
                lblError.Visible = false;
                lblSuccess.Visible = false;

                // Validate session
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("JoinFamily btnJoin: Session UserId is NULL");
                    ShowError("Your session has expired. Please log in again.");
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                System.Diagnostics.Debug.WriteLine(string.Format("JoinFamily btnJoin: UserId={0}", userId));

                // Validate
                string familyCode = txtFamilyCode.Text.Trim().ToUpper();
                System.Diagnostics.Debug.WriteLine(string.Format("JoinFamily btnJoin: FamilyCode={0}", familyCode));

                if (string.IsNullOrEmpty(familyCode))
                {
                    System.Diagnostics.Debug.WriteLine("JoinFamily btnJoin: Family code is empty");
                    ShowError("Please enter the family code.");
                    return;
                }

                if (familyCode.Length != 6)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("JoinFamily btnJoin: Family code length invalid: {0}", familyCode.Length));
                    ShowError("Family code must be 6 characters (2 letters + 4 numbers).");
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(familyCode, @"^[A-Z]{2}\d{4}$"))
                {
                    System.Diagnostics.Debug.WriteLine("JoinFamily btnJoin: Family code format invalid");
                    ShowError("Invalid format. Code must be 2 letters followed by 4 numbers (e.g., LP2222).");
                    return;
                }

                // Check if user is already in a family
                int? existingFamilyId = FamilyHelper.GetUserFamilyId(userId);
                if (existingFamilyId.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("JoinFamily btnJoin: User already in family {0}", existingFamilyId.Value));
                    ShowError("You are already in a family.");
                    return;
                }

                // Join family
                System.Diagnostics.Debug.WriteLine(string.Format("JoinFamily btnJoin: Attempting to join family with code {0}", familyCode));
                bool joinResult = FamilyHelper.JoinFamilyByCode(familyCode, userId);
                System.Diagnostics.Debug.WriteLine(string.Format("JoinFamily btnJoin: JoinFamilyByCode returned {0}", joinResult));

                if (joinResult)
                {
                    // Verify session is still valid before redirect
                    if (Session["UserId"] == null)
                    {
                        System.Diagnostics.Debug.WriteLine("JoinFamily btnJoin: ERROR - Session lost after join!");
                        ShowError("An error occurred. Please log in again.");
                        return;
                    }

                    // Verify family membership was created
                    int? newFamilyId = FamilyHelper.GetUserFamilyId(userId);
                    if (!newFamilyId.HasValue)
                    {
                        System.Diagnostics.Debug.WriteLine("JoinFamily btnJoin: ERROR - Family membership not found after join!");
                        ShowError("An error occurred. Please try again.");
                        return;
                    }

                    System.Diagnostics.Debug.WriteLine(string.Format("JoinFamily btnJoin: SUCCESS - User {0} joined family {1}, redirecting to ChildDashboard", userId, newFamilyId.Value));
                    
                    // Success - redirect to dashboard
                    // Use endResponse: false to prevent ThreadAbortException issues
                    Response.Redirect("ChildDashboard.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("JoinFamily btnJoin: Family code not found or join failed");
                    ShowError("Family code not found. Please check the code and try again.");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // This is expected when Response.Redirect is called - re-throw to allow redirect to complete
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JoinFamily btnJoin error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                ShowError("An error occurred. Please try again.");
            }
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            lblError.Visible = true;
        }
    }
}

