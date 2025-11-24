using System;
using System.Data;
using System.Data.SqlClient;
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

                if (!IsPostBack)
                {
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
    }
}

