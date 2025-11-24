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
                // Check authentication
                if (Session["UserId"] == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                // Check role - only children should access this page
                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "CHILD")
                {
                    Response.Redirect("Dashboard.aspx");
                    return;
                }

                // Check if child is already in a family
                int userId = Convert.ToInt32(Session["UserId"]);
                int? familyId = FamilyHelper.GetUserFamilyId(userId);

                if (familyId.HasValue)
                {
                    // Already in a family - redirect to dashboard
                    Response.Redirect("ChildDashboard.aspx");
                    return;
                }

                if (!IsPostBack)
                {
                    // Clear messages
                    lblError.Visible = false;
                    lblSuccess.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JoinFamily Page_Load error: " + ex.Message);
                Response.Redirect("Login.aspx");
            }
        }

        protected void btnJoin_Click(object sender, EventArgs e)
        {
            try
            {
                // Clear previous messages
                lblError.Visible = false;
                lblSuccess.Visible = false;

                // Validate
                string familyCode = txtFamilyCode.Text.Trim().ToUpper();

                if (string.IsNullOrEmpty(familyCode))
                {
                    ShowError("Please enter the family code.");
                    return;
                }

                if (familyCode.Length != 6)
                {
                    ShowError("Family code must be 6 characters (2 letters + 4 numbers).");
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(familyCode, @"^[A-Z]{2}\d{4}$"))
                {
                    ShowError("Invalid format. Code must be 2 letters followed by 4 numbers (e.g., LP2222).");
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);

                // Check if user is already in a family
                if (FamilyHelper.GetUserFamilyId(userId).HasValue)
                {
                    ShowError("You are already in a family.");
                    return;
                }

                // Join family
                if (FamilyHelper.JoinFamilyByCode(familyCode, userId))
                {
                    // Success - redirect to dashboard
                    Response.Redirect("ChildDashboard.aspx");
                }
                else
                {
                    ShowError("Family code not found. Please check the code and try again.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JoinFamily error: " + ex.Message);
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

