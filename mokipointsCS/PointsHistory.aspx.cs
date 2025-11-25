using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class PointsHistory : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("PointsHistory.aspx Page_Load called at " + DateTime.Now.ToString());
            try
            {
                // Check authentication
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("PointsHistory: User not authenticated - redirecting to Login");
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                // Check role - only children can access this page
                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "CHILD")
                {
                    System.Diagnostics.Debug.WriteLine("PointsHistory: User is not CHILD - redirecting to Dashboard");
                    Response.Redirect("Dashboard.aspx", false);
                    return;
                }

                // Check family membership
                int userId = Convert.ToInt32(Session["UserId"]);
                System.Diagnostics.Debug.WriteLine("PointsHistory: UserId = " + userId);
                
                int? familyId = FamilyHelper.GetUserFamilyId(userId);
                System.Diagnostics.Debug.WriteLine("PointsHistory: FamilyId = " + (familyId.HasValue ? familyId.Value.ToString() : "NULL"));
                
                if (!familyId.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine("PointsHistory: User not in family - redirecting to JoinFamily");
                    Response.Redirect("JoinFamily.aspx", false);
                    return;
                }

                // Set user name - load from session or database
                if (Session["FirstName"] != null)
                {
                    litUserName.Text = Session["FirstName"].ToString();
                }
                else
                {
                    // Fallback: Load from database if session is missing
                    var userInfo = AuthenticationHelper.GetUserById(userId);
                    if (userInfo != null)
                    {
                        string firstName = userInfo["FirstName"].ToString();
                        Session["FirstName"] = firstName;
                        Session["LastName"] = userInfo["LastName"].ToString();
                        litUserName.Text = firstName;
                    }
                }

                if (!IsPostBack)
                {
                    // Load profile picture
                    LoadProfilePicture(userId);
                    
                    System.Diagnostics.Debug.WriteLine("PointsHistory: Loading point transactions (not postback)");
                    LoadPointTransactions(userId);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("PointsHistory: Postback detected");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                LogError("Page_Load", ex);
                Response.Redirect("Error500.aspx", false);
            }
        }

        protected void Page_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex != null)
            {
                LogError("Page_Error", ex);
                Server.ClearError();
                Response.Redirect("Error500.aspx", false);
            }
        }

        private void LogError(string method, Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("========================================");
            System.Diagnostics.Debug.WriteLine("PointsHistory.aspx - Error in " + method);
            System.Diagnostics.Debug.WriteLine("Error Message: " + ex.Message);
            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine("Inner Exception: " + ex.InnerException.Message);
                System.Diagnostics.Debug.WriteLine("Inner Stack Trace: " + ex.InnerException.StackTrace);
            }
            System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
            System.Diagnostics.Debug.WriteLine("========================================");
        }

        private void LoadPointTransactions(int userId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadPointTransactions: Starting for userId = " + userId);
                
                // Get actual balance from database (source of truth)
                string balanceQuery = "SELECT Points FROM [dbo].[Users] WHERE Id = @UserId";
                object balanceResult = DatabaseHelper.ExecuteScalar(balanceQuery, new SqlParameter("@UserId", userId));
                int totalPoints = balanceResult != null ? Convert.ToInt32(balanceResult) : 0;
                
                System.Diagnostics.Debug.WriteLine("LoadPointTransactions: Current balance from database = " + totalPoints);
                
                litTotalPoints.Text = totalPoints.ToString("N0");
                
                DataTable transactions = TaskHelper.GetUserPointTransactions(userId);
                
                if (transactions == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadPointTransactions: transactions is null");
                    pnlTransactions.Visible = false;
                    pnlEmpty.Visible = true;
                    return;
                }

                System.Diagnostics.Debug.WriteLine("LoadPointTransactions: Found " + transactions.Rows.Count + " transactions");

                if (transactions.Rows.Count > 0)
                {
                    rptTransactions.DataSource = transactions;
                    rptTransactions.DataBind();
                    pnlTransactions.Visible = true;
                    pnlEmpty.Visible = false;
                    System.Diagnostics.Debug.WriteLine("LoadPointTransactions: Transactions displayed");
                }
                else
                {
                    pnlTransactions.Visible = false;
                    pnlEmpty.Visible = true;
                    System.Diagnostics.Debug.WriteLine("LoadPointTransactions: No transactions - showing empty state");
                }
            }
            catch (Exception ex)
            {
                LogError("LoadPointTransactions", ex);
                pnlTransactions.Visible = false;
                pnlEmpty.Visible = true;
                litTotalPoints.Text = "0";
            }
        }

        protected void LoadProfilePicture(int userId)
        {
            try
            {
                var userInfo = AuthenticationHelper.GetUserById(userId);
                if (userInfo != null)
                {
                    string firstName = userInfo["FirstName"].ToString();
                    string lastName = userInfo["LastName"].ToString();
                    string initials = (firstName.Length > 0 ? firstName[0].ToString() : "") + (lastName.Length > 0 ? lastName[0].ToString() : "");
                    
                    // Check if ProfilePicture column exists
                    string profilePicture = null;
                    if (userInfo.Table.Columns.Contains("ProfilePicture"))
                    {
                        profilePicture = (userInfo["ProfilePicture"] != null && userInfo["ProfilePicture"] != DBNull.Value) ? userInfo["ProfilePicture"].ToString() : null;
                    }
                    
                    // Load profile picture if exists
                    if (!string.IsNullOrEmpty(profilePicture))
                    {
                        string picturePath = Server.MapPath("~/Images/ProfilePictures/" + profilePicture);
                        if (File.Exists(picturePath))
                        {
                            imgProfilePicture.ImageUrl = "~/Images/ProfilePictures/" + profilePicture;
                            imgProfilePicture.Visible = true;
                            litProfilePlaceholder.Visible = false;
                            return;
                        }
                    }
                    
                    // Show placeholder with initials
                    litProfilePlaceholder.Text = string.Format(
                        "<div class=\"profile-avatar-placeholder\">{0}</div>",
                        string.IsNullOrEmpty(initials) ? "C" : initials.ToUpper());
                    litProfilePlaceholder.Visible = true;
                    imgProfilePicture.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadProfilePicture error: " + ex.Message);
                // Show default placeholder on error
                litProfilePlaceholder.Text = "<div class=\"profile-avatar-placeholder\">C</div>";
                litProfilePlaceholder.Visible = true;
                imgProfilePicture.Visible = false;
            }
        }
    }
}

