using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace mokipointsCS
{
    public partial class TaskTemplates : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Check authentication
            if (Session["UserId"] == null)
            {
                Response.Redirect("Login.aspx");
                return;
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            string userRole = Session["UserRole"]?.ToString() ?? "";

            // Only parents can manage templates
            if (userRole != "PARENT")
            {
                ShowError("Only parents can manage task templates.");
                pnlTemplates.Visible = false;
                return;
            }

            // Get family ID
            int? familyId = FamilyHelper.GetFamilyId(userId);
            if (!familyId.HasValue)
            {
                ShowError("You are not part of a family.");
                pnlTemplates.Visible = false;
                return;
            }

            if (!IsPostBack)
            {
                LoadTemplates(familyId.Value);
            }

            // Handle success/error messages from redirects
            if (Request.QueryString["success"] != null)
            {
                ShowSuccess(Request.QueryString["success"]);
            }
            if (Request.QueryString["error"] != null)
            {
                ShowError(Request.QueryString["error"]);
            }
        }

        protected void LoadTemplates(int familyId)
        {
            try
            {
                // Get all tasks where IsTemplate = 1
                DataTable tasks = TaskHelper.GetFamilyTasks(familyId);
                
                // Filter to only templates
                DataTable templates = tasks.Clone();
                foreach (DataRow row in tasks.Rows)
                {
                    if (row["IsTemplate"] != DBNull.Value && Convert.ToBoolean(row["IsTemplate"]))
                    {
                        templates.ImportRow(row);
                    }
                }

                if (templates != null && templates.Rows.Count > 0)
                {
                    rptTemplates.DataSource = templates;
                    rptTemplates.DataBind();
                    pnlTemplates.Visible = true;
                    pnlEmpty.Visible = false;
                }
                else
                {
                    pnlTemplates.Visible = false;
                    pnlEmpty.Visible = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadTemplates error: " + ex.Message);
                ShowError("Error loading templates.");
                pnlTemplates.Visible = false;
                pnlEmpty.Visible = true;
            }
        }

        protected void rptTemplates_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int templateId = Convert.ToInt32(e.CommandArgument);
            int userId = Convert.ToInt32(Session["UserId"]);

            if (e.CommandName == "UseTemplate")
            {
                // Redirect to Tasks page with template ID to create task from template
                Response.Redirect(string.Format("Tasks.aspx?useTemplate={0}", templateId));
            }
            else if (e.CommandName == "Edit")
            {
                // Redirect to Tasks page to edit the template
                Response.Redirect(string.Format("Tasks.aspx?editTemplate={0}", templateId));
            }
            else if (e.CommandName == "Delete")
            {
                // Delete template (soft delete by setting IsActive = 0)
                bool success = TaskHelper.DeleteTask(templateId, userId);

                if (success)
                {
                    int? familyId = FamilyHelper.GetFamilyId(userId);
                    if (familyId.HasValue)
                    {
                        LoadTemplates(familyId.Value);
                    }
                    ShowSuccess("Template deleted successfully.");
                }
                else
                {
                    ShowError("Failed to delete template. It may be in use.");
                }
            }
        }

        protected void rptTemplates_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                int taskId = Convert.ToInt32(row["Id"]);

                // Load objectives for this template
                Repeater rptObjectives = (Repeater)e.Item.FindControl("rptObjectives");
                if (rptObjectives != null)
                {
                    DataTable objectives = TaskHelper.GetTaskObjectives(taskId);
                    if (objectives != null && objectives.Rows.Count > 0)
                    {
                        rptObjectives.DataSource = objectives;
                        rptObjectives.DataBind();
                    }
                }
            }
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("Login.aspx");
        }

        private void ShowSuccess(string message)
        {
            lblSuccess.Text = message;
            pnlSuccess.CssClass = "success-message";
            pnlSuccess.Visible = true;
        }

        private void ShowError(string message)
        {
            lblError.Text = message;
            pnlError.CssClass = "error-message";
            pnlError.Visible = true;
        }
    }
}

