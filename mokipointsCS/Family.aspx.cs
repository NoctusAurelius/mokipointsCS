using System;
using System.Data;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class Family : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Family.aspx Page_Load called at " + DateTime.Now.ToString());
            try
            {
                // Check authentication
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("Family: User not authenticated - redirecting to Login");
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                // Check role - only parents can access this page
                if (Session["UserRole"] == null || Session["UserRole"].ToString() != "PARENT")
                {
                    System.Diagnostics.Debug.WriteLine("Family: User is not PARENT - redirecting to Dashboard");
                    Response.Redirect("Dashboard.aspx", false);
                    return;
                }

                if (!IsPostBack)
                {
                    System.Diagnostics.Debug.WriteLine("Family: Loading family info (not postback)");
                    LoadFamilyInfo();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Family: Postback detected");
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
            System.Diagnostics.Debug.WriteLine("Family.aspx - Error in " + method);
            System.Diagnostics.Debug.WriteLine("Error Message: " + ex.Message);
            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine("Inner Exception: " + ex.InnerException.Message);
                System.Diagnostics.Debug.WriteLine("Inner Stack Trace: " + ex.InnerException.StackTrace);
            }
            System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
            System.Diagnostics.Debug.WriteLine("========================================");
        }

        private void LoadFamilyInfo()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: UserId = " + userId);
                
                int? familyId = FamilyHelper.GetUserFamilyId(userId);
                System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: FamilyId = " + (familyId.HasValue ? familyId.Value.ToString() : "NULL"));

                if (familyId.HasValue)
                {
                    // User is in a family - show family info
                    System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Loading family info for familyId = " + familyId.Value);
                    var familyInfo = FamilyHelper.GetFamilyInfo(familyId.Value);
                    if (familyInfo != null)
                    {
                        litFamilyName.Text = familyInfo["Name"].ToString();
                        litFamilyCode.Text = familyInfo["FamilyCode"].ToString();
                        
                        // Get actual treasury balance from FamilyTreasury table
                        int treasuryBalance = TreasuryHelper.GetTreasuryBalance(familyId.Value);
                        litTreasuryPoints.Text = treasuryBalance.ToString("N0");

                        pnlFamilyInfo.Visible = true;
                        pnlFamilyActions.Visible = false;
                        System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Family info displayed - Name: " + litFamilyName.Text + ", Code: " + litFamilyCode.Text);
                        
                        // Load children monitoring
                        LoadChildrenMonitoring(familyId.Value);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: FamilyInfo is null");
                        pnlFamilyInfo.Visible = false;
                        pnlFamilyActions.Visible = true;
                        pnlChildrenMonitoring.Visible = false;
                    }
                }
                else
                {
                    // User is not in a family - show create/join forms
                    System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: User not in family - showing create/join forms");
                    pnlFamilyInfo.Visible = false;
                    pnlFamilyActions.Visible = true;
                    pnlChildrenMonitoring.Visible = false;
                }

                // Clear messages
                ClearMessages();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadFamilyInfo error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
            }
        }

        protected void btnCreateFamily_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnCreateFamily_Click called at " + DateTime.Now.ToString());
            try
            {
                ClearMessages();

                // Validate
                if (string.IsNullOrEmpty(txtCreateFamilyName.Text.Trim()))
                {
                    System.Diagnostics.Debug.WriteLine("CreateFamily: Family name is empty");
                    ShowCreateError("Family name is required.");
                    return;
                }

                if (string.IsNullOrEmpty(txtCreatePinCode.Text.Trim()) || txtCreatePinCode.Text.Trim().Length != 6)
                {
                    System.Diagnostics.Debug.WriteLine("CreateFamily: PIN code validation failed - length: " + (txtCreatePinCode.Text.Trim().Length));
                    ShowCreateError("PIN code must be 6 digits.");
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(txtCreatePinCode.Text.Trim(), @"^\d{6}$"))
                {
                    System.Diagnostics.Debug.WriteLine("CreateFamily: PIN code validation failed - not all digits");
                    ShowCreateError("PIN code must contain only numbers.");
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                System.Diagnostics.Debug.WriteLine("CreateFamily: UserId = " + userId);

                // Check if user is already in a family
                int? existingFamilyId = FamilyHelper.GetUserFamilyId(userId);
                if (existingFamilyId.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine("CreateFamily: User already in family " + existingFamilyId.Value);
                    ShowCreateError("You are already in a family.");
                    return;
                }

                // Create family
                System.Diagnostics.Debug.WriteLine("CreateFamily: Calling FamilyHelper.CreateFamily with Name=" + txtCreateFamilyName.Text.Trim() + ", PinCode=" + txtCreatePinCode.Text.Trim() + ", OwnerId=" + userId);
                int familyId = FamilyHelper.CreateFamily(
                    txtCreateFamilyName.Text.Trim(),
                    txtCreatePinCode.Text.Trim(),
                    userId);

                System.Diagnostics.Debug.WriteLine("CreateFamily: FamilyHelper.CreateFamily returned familyId = " + familyId);

                if (familyId > 0)
                {
                    System.Diagnostics.Debug.WriteLine("CreateFamily: Success! Redirecting to Family.aspx");
                    // Reload page to show family info
                    Response.Redirect("Family.aspx", false);
                    return;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("CreateFamily: Failed - familyId <= 0");
                    ShowCreateError("Failed to create family. Please try again.");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CreateFamily error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowCreateError("An error occurred. Please try again.");
            }
        }

        protected void btnJoinFamily_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnJoinFamily_Click called at " + DateTime.Now.ToString());
            try
            {
                ClearMessages();

                // Validate
                if (string.IsNullOrEmpty(txtJoinFamilyName.Text.Trim()))
                {
                    System.Diagnostics.Debug.WriteLine("JoinFamily: Family name is empty");
                    ShowJoinError("Family name is required.");
                    return;
                }

                if (string.IsNullOrEmpty(txtJoinPinCode.Text.Trim()) || txtJoinPinCode.Text.Trim().Length != 6)
                {
                    System.Diagnostics.Debug.WriteLine("JoinFamily: PIN code validation failed - length: " + (txtJoinPinCode.Text.Trim().Length));
                    ShowJoinError("PIN code must be 6 digits.");
                    return;
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(txtJoinPinCode.Text.Trim(), @"^\d{6}$"))
                {
                    System.Diagnostics.Debug.WriteLine("JoinFamily: PIN code validation failed - not all digits");
                    ShowJoinError("PIN code must contain only numbers.");
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);
                System.Diagnostics.Debug.WriteLine("JoinFamily: UserId = " + userId);

                // Check if user is already in a family
                int? existingFamilyId = FamilyHelper.GetUserFamilyId(userId);
                if (existingFamilyId.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine("JoinFamily: User already in family " + existingFamilyId.Value);
                    ShowJoinError("You are already in a family.");
                    return;
                }

                // Join family
                System.Diagnostics.Debug.WriteLine("JoinFamily: Calling FamilyHelper.JoinFamily with Name=" + txtJoinFamilyName.Text.Trim() + ", PinCode=" + txtJoinPinCode.Text.Trim() + ", UserId=" + userId);
                bool joinResult = FamilyHelper.JoinFamily(
                    txtJoinFamilyName.Text.Trim(),
                    txtJoinPinCode.Text.Trim(),
                    userId);

                System.Diagnostics.Debug.WriteLine("JoinFamily: FamilyHelper.JoinFamily returned " + joinResult);

                if (joinResult)
                {
                    System.Diagnostics.Debug.WriteLine("JoinFamily: Success! Redirecting to Family.aspx");
                    // Reload page to show family info
                    Response.Redirect("Family.aspx", false);
                    return;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("JoinFamily: Failed - family not found or wrong PIN");
                    ShowJoinError("Family not found or PIN code is incorrect. Please try again.");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JoinFamily error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowJoinError("An error occurred. Please try again.");
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
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

        private void ClearMessages()
        {
            lblCreateError.Visible = false;
            lblCreateSuccess.Visible = false;
            lblJoinError.Visible = false;
            lblJoinSuccess.Visible = false;
        }

        private void ShowCreateError(string message)
        {
            lblCreateError.Text = message;
            lblCreateError.Visible = true;
        }

        private void ShowJoinError(string message)
        {
            lblJoinError.Text = message;
            lblJoinError.Visible = true;
        }

        private void LoadChildrenMonitoring(int familyId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadChildrenMonitoring: Starting for familyId = " + familyId);
                var children = FamilyHelper.GetFamilyChildrenWithStats(familyId);
                
                if (children == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadChildrenMonitoring: children is null");
                    pnlChildrenMonitoring.Visible = false;
                    return;
                }

                System.Diagnostics.Debug.WriteLine("LoadChildrenMonitoring: Found " + children.Rows.Count + " children");
                
                if (children.Rows.Count > 0)
                {
                    rptChildren.DataSource = children;
                    rptChildren.DataBind();
                    pnlChildrenMonitoring.Visible = true;
                    pnlNoChildren.Visible = false;
                    System.Diagnostics.Debug.WriteLine("LoadChildrenMonitoring: Children displayed");
                }
                else
                {
                    rptChildren.DataSource = null;
                    rptChildren.DataBind();
                    pnlChildrenMonitoring.Visible = true; // Still show the section even if empty
                    pnlNoChildren.Visible = true;
                    System.Diagnostics.Debug.WriteLine("LoadChildrenMonitoring: No children - showing empty state");
                }
            }
            catch (Exception ex)
            {
                LogError("LoadChildrenMonitoring", ex);
                pnlChildrenMonitoring.Visible = false;
            }
        }

        protected void rptChildren_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item || 
                    e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
                {
                    // Get the IsBanned value from data
                    bool isBanned = false;
                    try
                    {
                        isBanned = Convert.ToBoolean(DataBinder.Eval(e.Item.DataItem, "IsBanned"));
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error getting IsBanned value: " + ex.Message);
                    }

                    // Set border color on child card
                    System.Web.UI.HtmlControls.HtmlGenericControl childCard = e.Item.FindControl("childCard") as System.Web.UI.HtmlControls.HtmlGenericControl;
                    if (childCard != null)
                    {
                        string borderColor = isBanned ? "#d32f2f" : "#0066CC";
                        childCard.Style["border-left"] = "4px solid " + borderColor;
                    }

                    // Set profile picture
                    System.Web.UI.WebControls.PlaceHolder phProfilePicture = e.Item.FindControl("phProfilePicture") as System.Web.UI.WebControls.PlaceHolder;
                    if (phProfilePicture != null)
                    {
                        try
                        {
                            string profilePicture = DataBinder.Eval(e.Item.DataItem, "ProfilePicture")?.ToString() ?? "";
                            if (!string.IsNullOrEmpty(profilePicture))
                            {
                                System.Web.UI.HtmlControls.HtmlImage img = new System.Web.UI.HtmlControls.HtmlImage();
                                // Use Images/ProfilePictures/ location (new location)
                                img.Src = "~/Images/ProfilePictures/" + profilePicture;
                                img.Alt = "Profile";
                                img.Style["width"] = "100%";
                                img.Style["height"] = "100%";
                                img.Style["object-fit"] = "cover";
                                phProfilePicture.Controls.Add(img);
                            }
                            else
                            {
                                System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                                div.InnerText = "U";
                                div.Style["font-size"] = "36px";
                                div.Style["color"] = "#999";
                                phProfilePicture.Controls.Add(div);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine("Error setting profile picture: " + ex.Message);
                        }
                    }

                    // Set banned badge visibility
                    System.Web.UI.WebControls.Label lblBannedBadge = e.Item.FindControl("lblBannedBadge") as System.Web.UI.WebControls.Label;
                    if (lblBannedBadge != null)
                    {
                        lblBannedBadge.Visible = isBanned;
                    }

                    // Set Ban button properties
                    System.Web.UI.WebControls.Button btnBan = e.Item.FindControl("btnBan") as System.Web.UI.WebControls.Button;
                    if (btnBan != null)
                    {
                        btnBan.Text = isBanned ? "Unban" : "Ban";
                        btnBan.CommandName = isBanned ? "Unban" : "Ban";
                        btnBan.Style["background-color"] = isBanned ? "#2e7d32" : "#d32f2f";
                        string action = isBanned ? "unban" : "ban";
                        btnBan.OnClientClick = string.Format("return confirm('Are you sure you want to {0} this child?');", action);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("rptChildren_ItemDataBound error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
            }
        }

        protected void rptChildren_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
        {
            try
            {
                ClearChildrenMessages();
                int childId = Convert.ToInt32(e.CommandArgument);
                
                System.Diagnostics.Debug.WriteLine("rptChildren_ItemCommand: CommandName = " + e.CommandName + ", ChildId = " + childId);

                if (e.CommandName == "Ban")
                {
                    // Ban the child
                    if (FamilyHelper.BanUnbanChild(childId, true))
                    {
                        ShowChildrenSuccess("Child has been banned. They will not receive new tasks.");
                        // Reload children
                        int userId = Convert.ToInt32(Session["UserId"]);
                        int? familyId = FamilyHelper.GetUserFamilyId(userId);
                        if (familyId.HasValue)
                        {
                            LoadChildrenMonitoring(familyId.Value);
                        }
                    }
                    else
                    {
                        ShowChildrenError("Failed to ban child. Please try again.");
                    }
                }
                else if (e.CommandName == "Unban")
                {
                    // Unban the child
                    if (FamilyHelper.BanUnbanChild(childId, false))
                    {
                        ShowChildrenSuccess("Child has been unbanned. They can now receive tasks.");
                        // Reload children
                        int userId = Convert.ToInt32(Session["UserId"]);
                        int? familyId = FamilyHelper.GetUserFamilyId(userId);
                        if (familyId.HasValue)
                        {
                            LoadChildrenMonitoring(familyId.Value);
                        }
                    }
                    else
                    {
                        ShowChildrenError("Failed to unban child. Please try again.");
                    }
                }
                else if (e.CommandName == "Remove")
                {
                    // Remove the child from family
                    int userId = Convert.ToInt32(Session["UserId"]);
                    int? familyId = FamilyHelper.GetUserFamilyId(userId);
                    
                    if (!familyId.HasValue)
                    {
                        ShowChildrenError("Unable to determine family. Please try again.");
                        return;
                    }

                    if (FamilyHelper.RemoveChildFromFamily(familyId.Value, childId))
                    {
                        ShowChildrenSuccess("Child has been removed from the family. Their points have been reset to 0.");
                        // Reload children
                        LoadChildrenMonitoring(familyId.Value);
                    }
                    else
                    {
                        ShowChildrenError("Failed to remove child. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("rptChildren_ItemCommand error: " + ex.Message);
                ShowChildrenError("An error occurred. Please try again.");
            }
        }

        private void ClearChildrenMessages()
        {
            lblChildrenError.Visible = false;
            lblChildrenError.Text = "";
            lblChildrenSuccess.Visible = false;
            lblChildrenSuccess.Text = "";
        }

        private void ShowChildrenError(string message)
        {
            lblChildrenError.Text = message;
            lblChildrenError.Visible = true;
        }

        private void ShowChildrenSuccess(string message)
        {
            lblChildrenSuccess.Text = message;
            lblChildrenSuccess.Visible = true;
        }
    }
}

