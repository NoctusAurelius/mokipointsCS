using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class Family : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Ensure UTF-8 encoding for emojis
            Response.Charset = "utf-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            
            System.Diagnostics.Debug.WriteLine("Family.aspx Page_Load called at " + DateTime.Now.ToString());
            try
            {
                // Check authentication
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("Family: User not authenticated - redirecting to Login");
                    Response.Redirect("Login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                // Check role - parents and children can access this page
                string userRole = Session["UserRole"]?.ToString() ?? "";
                if (userRole != "PARENT" && userRole != "CHILD")
                {
                    System.Diagnostics.Debug.WriteLine("Family: User is not PARENT or CHILD - redirecting to Dashboard");
                    Response.Redirect("Dashboard.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                int userId = Convert.ToInt32(Session["UserId"]);

                // Set user name - load from session or database
                if (litUserName != null)
                {
                    if (Session["FirstName"] != null)
                    {
                        litUserName.Text = Session["FirstName"].ToString();
                        if (Session["LastName"] != null)
                        {
                            litUserName.Text += " " + Session["LastName"].ToString();
                        }
                    }
                    else
                    {
                        // Fallback: Load from database if session is missing
                        var userInfo = AuthenticationHelper.GetUserById(userId);
                        if (userInfo != null)
                        {
                            string firstName = userInfo["FirstName"].ToString();
                            string lastName = userInfo["LastName"].ToString();
                            Session["FirstName"] = firstName;
                            Session["LastName"] = lastName;
                            litUserName.Text = firstName + " " + lastName;
                        }
                    }
                }

                if (!IsPostBack)
                {
                    // Load profile picture
                    LoadProfilePicture(userId);
                    
                    // Show Review link only for parents (Fix #3)
                    if (pnlReviewLink != null)
                    {
                        pnlReviewLink.Visible = (userRole == "PARENT");
                    }
                    
                    // Check for query string messages
                    if (Request.QueryString["left"] == "true")
                    {
                        ShowFamilySuccess("You have successfully left the family.");
                        // Show toast message
                        ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                            "showToastMessage('You have successfully left the family.', 'success');", true);
                    }
                    
                    // Check for change code success message
                    if (Request.QueryString["codeChanged"] == "true")
                    {
                        // Only show toast message, not inline message (toast is less intrusive)
                        ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                            "showToastMessage('Family code changed successfully! Share the new code with children.', 'success');", true);
                    }
                    
                    System.Diagnostics.Debug.WriteLine("Family: Loading family info (not postback)");
                    LoadFamilyInfo();
                    
                    // Set data attributes AFTER LoadFamilyInfo (so panel is visible)
                    SetLayoutDataAttributes(userId);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Family: Postback detected");
                    // Reload family info on postback to refresh sidebar (profile pictures, etc.)
                    LoadFamilyInfo();
                    // Set data attributes on postback too (in case they're needed)
                    SetLayoutDataAttributes(userId);
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
                System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Starting");
                int userId = Convert.ToInt32(Session["UserId"]);
                System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: UserId = " + userId);
                
                System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Checking panel controls");
                if (pnlFamilyInfo == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: ERROR - pnlFamilyInfo is null");
                }
                if (pnlFamilyActions == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: ERROR - pnlFamilyActions is null");
                }
                if (litFamilyCode == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: WARNING - litFamilyCode is null");
                }
                
                int? familyId = FamilyHelper.GetUserFamilyId(userId);
                System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: FamilyId = " + (familyId.HasValue ? familyId.Value.ToString() : "NULL"));

                if (familyId.HasValue)
                {
                    // User is in a family - show family info
                    System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Loading family info for familyId = " + familyId.Value);
                    var familyInfo = FamilyHelper.GetFamilyInfo(familyId.Value);
                    if (familyInfo != null)
                    {
                        System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: FamilyInfo retrieved successfully");
                        
                        // Set family code (moved to left sidebar)
                        if (litFamilyCode != null)
                        {
                        litFamilyCode.Text = familyInfo["FamilyCode"].ToString();
                            System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Set family code = " + litFamilyCode.Text);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: WARNING - litFamilyCode is null, cannot set code");
                        }
                        
                        // Set family name in header
                        if (litFamilyNameHeader != null)
                        {
                            litFamilyNameHeader.Text = familyInfo["Name"].ToString();
                            System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Set family name = " + litFamilyNameHeader.Text);
                        }
                        
                        // Hide subtitle when in family
                        if (pSubtitle != null)
                        {
                            pSubtitle.Visible = false;
                        }
                        
                        // Set hidden fields for chat
                        if (hidFamilyId != null)
                        {
                            hidFamilyId.Value = familyId.Value.ToString();
                        }
                        if (hidUserId != null)
                        {
                            hidUserId.Value = userId.ToString();
                        }
                        if (hidGiphyApiKey != null)
                        {
                            string giphyKey = System.Configuration.ConfigurationManager.AppSettings["GIPHY_API_KEY"] ?? "";
                            hidGiphyApiKey.Value = giphyKey;
                        }
                        
                        // Note: litTreasuryPoints was removed when moving to sidebar layout

                        if (pnlFamilyInfo != null)
                        {
                        pnlFamilyInfo.Visible = true;
                            System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Set pnlFamilyInfo.Visible = true");
                            
                            // Remove container width constraint when showing family layout
                            var mainContainer = this.FindControl("mainContainer") as System.Web.UI.HtmlControls.HtmlGenericControl;
                            if (mainContainer != null)
                            {
                                mainContainer.Style["max-width"] = "100%";
                                mainContainer.Style["padding"] = "0";
                                mainContainer.Style["margin"] = "0";
                                mainContainer.Style["width"] = "100%";
                                System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Removed container width constraints");
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: ERROR - pnlFamilyInfo is null, cannot set visible");
                        }
                        
                        if (pnlFamilyActions != null)
                        {
                        pnlFamilyActions.Visible = false;
                            System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Set pnlFamilyActions.Visible = false");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: WARNING - pnlFamilyActions is null, cannot set visible");
                        }
                        
                        System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Family info displayed - Code: " + (litFamilyCode != null ? litFamilyCode.Text : "N/A"));
                        
                        // Load family members (sidebar)
                        System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Calling LoadFamilyMembers");
                        LoadFamilyMembers(familyId.Value, userId);
                        System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: LoadFamilyMembers completed");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: FamilyInfo is null");
                        if (pnlFamilyInfo != null)
                        {
                        pnlFamilyInfo.Visible = false;
                        }
                        if (pnlFamilyActions != null)
                        {
                        pnlFamilyActions.Visible = true;
                        }
                    }
                }
                else
                {
                    // User is not in a family - show create/join forms
                    System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: User not in family - showing create/join forms");
                    if (pnlFamilyInfo != null)
                    {
                    pnlFamilyInfo.Visible = false;
                    }
                    if (pnlFamilyActions != null)
                    {
                    pnlFamilyActions.Visible = true;
                    }
                }

                // Clear messages
                System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Clearing messages");
                ClearMessages();
                System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Completed successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: EXCEPTION - " + ex.Message);
                System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Stack trace - " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Inner exception - " + ex.InnerException.Message);
                    System.Diagnostics.Debug.WriteLine("LoadFamilyInfo: Inner stack trace - " + ex.InnerException.StackTrace);
                }
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

        private void LoadFamilyMembers(int familyId, int currentUserId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Starting for familyId = " + familyId + ", currentUserId = " + currentUserId);
                
                // Check if required controls exist
                if (rptParents == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: ERROR - rptParents is null");
                    return;
                }
                if (rptChildren == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: ERROR - rptChildren is null");
                    return;
                }
                if (parentCount == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: ERROR - parentCount is null");
                    return;
                }
                if (childCount == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: ERROR - childCount is null");
                    return;
                }

                // Get all family members
                System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Calling FamilyHelper.GetFamilyMembers");
                var members = FamilyHelper.GetFamilyMembers(familyId);
                if (members == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: ERROR - GetFamilyMembers returned null");
                    return;
                }
                if (members.Rows.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: No members found");
                    return;
                }
                System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Found " + members.Rows.Count + " total members");

                // Separate parents and children
                DataTable parents = new DataTable();
                if (members.Columns.Count > 0)
                {
                    parents = members.Clone(); // Copy structure
                    foreach (DataRow row in members.Rows)
                    {
                        if (row["Role"].ToString() == "PARENT")
                        {
                            parents.ImportRow(row);
                        }
                    }
                }
                System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Separated " + parents.Rows.Count + " parents");
                
                System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Calling FamilyHelper.GetChildrenWithStats");
                var children = FamilyHelper.GetChildrenWithStats(familyId);
                if (children == null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: ERROR - GetChildrenWithStats returned null");
                    children = new DataTable();
                }
                System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Found " + children.Rows.Count + " children");

                // Bind parents
                System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Binding parents repeater");
                rptParents.DataSource = parents;
                rptParents.DataBind();
                if (parentCount != null)
                {
                    parentCount.InnerText = parents.Rows.Count.ToString();
                }

                // Bind children
                System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Binding children repeater");
                    rptChildren.DataSource = children;
                    rptChildren.DataBind();
                if (childCount != null)
                {
                    childCount.InnerText = children.Rows.Count.ToString();
                }

                // Check if current user is owner
                System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Checking if user is owner");
                int? ownerId = FamilyHelper.GetFamilyOwnerId(familyId);
                bool isOwner = ownerId.HasValue && ownerId.Value == currentUserId;
                System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: ownerId = " + (ownerId.HasValue ? ownerId.Value.ToString() : "NULL") + ", isOwner = " + isOwner);

                // Show owner actions if user is owner (only if control exists)
                if (pnlOwnerActions != null)
                {
                    pnlOwnerActions.Visible = isOwner;
                    System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Set pnlOwnerActions.Visible = " + isOwner);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: WARNING - pnlOwnerActions is null, skipping visibility set");
                }
                
                // Show change code button if owner (using CSS, not Visible property, so JavaScript can access it)
                if (btnChangeCode != null)
                {
                    if (isOwner)
                    {
                        btnChangeCode.Style["display"] = "inline-block";
                        System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Set btnChangeCode display = inline-block");
                    }
                    else
                    {
                        btnChangeCode.Style["display"] = "none";
                        System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Set btnChangeCode display = none");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: WARNING - btnChangeCode is null, skipping display set");
                }

                System.Diagnostics.Debug.WriteLine(string.Format("LoadFamilyMembers: Completed - {0} parents, {1} children, isOwner={2}", 
                    parents.Rows.Count, children.Rows.Count, isOwner));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: EXCEPTION - " + ex.Message);
                System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Stack trace - " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("LoadFamilyMembers: Inner exception - " + ex.InnerException.Message);
                }
                LogError("LoadFamilyMembers", ex);
            }
        }

        protected void rptParents_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item || 
                    e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
                {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    int? familyId = FamilyHelper.GetUserFamilyId(userId);
                    if (!familyId.HasValue) return;

                    bool isOwner = Convert.ToBoolean(DataBinder.Eval(e.Item.DataItem, "IsOwner"));
                    int memberId = Convert.ToInt32(DataBinder.Eval(e.Item.DataItem, "Id"));
                    bool isCurrentUser = memberId == userId;

                    // Set owner badge
                    System.Web.UI.WebControls.Label lblOwnerBadge = e.Item.FindControl("lblOwnerBadge") as System.Web.UI.WebControls.Label;
                    if (lblOwnerBadge != null)
                    {
                        lblOwnerBadge.Visible = isOwner;
                    }

                    // Set profile picture
                    System.Web.UI.WebControls.PlaceHolder phParentAvatar = e.Item.FindControl("phParentAvatar") as System.Web.UI.WebControls.PlaceHolder;
                    if (phParentAvatar != null)
                    {
                        LoadMemberAvatar(phParentAvatar, e.Item.DataItem);
                    }

                    // Set join date in tooltip
                    System.Web.UI.WebControls.Literal litJoinDate = e.Item.FindControl("litJoinDate") as System.Web.UI.WebControls.Literal;
                    if (litJoinDate != null)
                    {
                        object joinDateObj = DataBinder.Eval(e.Item.DataItem, "JoinDate");
                        if (joinDateObj != null && joinDateObj != DBNull.Value)
                        {
                            DateTime joinDate = Convert.ToDateTime(joinDateObj);
                            litJoinDate.Text = joinDate.ToString("MMMM dd, yyyy");
                        }
                        else
                        {
                            litJoinDate.Text = "Unknown";
                        }
                    }

                    // Show owner actions if current user is owner and this is not the current user
                    System.Web.UI.HtmlControls.HtmlGenericControl parentActions = e.Item.FindControl("parentActions") as System.Web.UI.HtmlControls.HtmlGenericControl;
                    System.Web.UI.WebControls.Button btnTransfer = e.Item.FindControl("btnTransferOwnership") as System.Web.UI.WebControls.Button;
                    System.Web.UI.WebControls.Button btnKick = e.Item.FindControl("btnKickParent") as System.Web.UI.WebControls.Button;

                    if (isOwner && !isCurrentUser)
                    {
                        // Current user is owner, show transfer and kick buttons for other parents
                        if (btnTransfer != null)
                        {
                            btnTransfer.Visible = true;
                            string firstName = DataBinder.Eval(e.Item.DataItem, "FirstName")?.ToString() ?? "";
                            string lastName = DataBinder.Eval(e.Item.DataItem, "LastName")?.ToString() ?? "";
                            btnTransfer.OnClientClick = string.Format("return confirm('Are you sure you want to transfer ownership to {0} {1}?');", firstName, lastName);
                        }
                        if (btnKick != null)
                        {
                            btnKick.Visible = true;
                            string firstName = DataBinder.Eval(e.Item.DataItem, "FirstName")?.ToString() ?? "";
                            string lastName = DataBinder.Eval(e.Item.DataItem, "LastName")?.ToString() ?? "";
                            btnKick.OnClientClick = string.Format("return confirm('Are you sure you want to remove {0} {1} from the family?');", firstName, lastName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("rptParents_ItemDataBound error: " + ex.Message);
            }
        }

        protected void rptChildren_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
        {
            try
            {
                if (e.Item.ItemType == System.Web.UI.WebControls.ListItemType.Item || 
                    e.Item.ItemType == System.Web.UI.WebControls.ListItemType.AlternatingItem)
                {
                    // Set profile picture
                    System.Web.UI.WebControls.PlaceHolder phChildAvatar = e.Item.FindControl("phChildAvatar") as System.Web.UI.WebControls.PlaceHolder;
                    if (phChildAvatar != null)
                    {
                        LoadMemberAvatar(phChildAvatar, e.Item.DataItem);
                    }
                }
                    }
                    catch (Exception ex)
                    {
                System.Diagnostics.Debug.WriteLine("rptChildren_ItemDataBound error: " + ex.Message);
            }
        }

        protected string GetJoinDateString(object joinDateObj)
        {
            try
            {
                if (joinDateObj != null && joinDateObj != DBNull.Value)
                {
                    DateTime joinDate = Convert.ToDateTime(joinDateObj);
                    return joinDate.ToString("yyyy-MM-dd");
                }
                return "";
            }
            catch
            {
                return "";
            }
        }

        private void LoadMemberAvatar(System.Web.UI.WebControls.PlaceHolder ph, object dataItem)
        {
            try
            {
                // Get profile picture - handle DBNull properly
                object profilePictureObj = DataBinder.Eval(dataItem, "ProfilePicture");
                string profilePicture = (profilePictureObj != null && profilePictureObj != DBNull.Value) ? profilePictureObj.ToString() : "";
                
                string firstName = DataBinder.Eval(dataItem, "FirstName")?.ToString() ?? "";
                string lastName = DataBinder.Eval(dataItem, "LastName")?.ToString() ?? "";
                string initials = (firstName.Length > 0 ? firstName[0].ToString() : "") + (lastName.Length > 0 ? lastName[0].ToString() : "");

                if (!string.IsNullOrEmpty(profilePicture))
                {
                    // Check if file exists before trying to display it
                    string picturePath = Server.MapPath("~/Images/ProfilePictures/" + profilePicture);
                    if (File.Exists(picturePath))
                    {
                        System.Web.UI.HtmlControls.HtmlImage img = new System.Web.UI.HtmlControls.HtmlImage();
                        img.Src = ResolveUrl("~/Images/ProfilePictures/" + profilePicture);
                        img.Alt = "Profile";
                        img.Style["width"] = "100%";
                        img.Style["height"] = "100%";
                        img.Style["object-fit"] = "cover";
                        img.Style["border-radius"] = "50%";
                        ph.Controls.Add(img);
                    }
                    else
                    {
                        // File doesn't exist, show placeholder
                        System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                        div.InnerText = string.IsNullOrEmpty(initials) ? "U" : initials.ToUpper();
                        div.Style["font-size"] = "16px";
                        div.Style["color"] = "#999";
                        div.Style["font-weight"] = "bold";
                        ph.Controls.Add(div);
                    }
                }
                else
                {
                    // No profile picture, show placeholder
                    System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    div.InnerText = string.IsNullOrEmpty(initials) ? "U" : initials.ToUpper();
                    div.Style["font-size"] = "16px";
                    div.Style["color"] = "#999";
                    div.Style["font-weight"] = "bold";
                    ph.Controls.Add(div);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadMemberAvatar error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("LoadMemberAvatar stack trace: " + ex.StackTrace);
                // Show placeholder on error
                try
                {
                    System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
                    div.InnerText = "U";
                    div.Style["font-size"] = "16px";
                    div.Style["color"] = "#999";
                    div.Style["font-weight"] = "bold";
                    ph.Controls.Add(div);
                }
                catch { }
            }
        }

        protected void btnCopyCode_Click(object sender, EventArgs e)
        {
            try
            {
                // This is called via JavaScript after successful copy
                // The JavaScript already shows a toast, but we can also show inline message
                ShowFamilySuccess("Family code copied to clipboard!");
                // Also show toast for consistency
                ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                    "showToastMessage('Family code copied to clipboard!', 'success');", true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnCopyCode_Click error: " + ex.Message);
                ShowFamilyError("Failed to copy code. Please copy manually.");
                ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                    "showToastMessage('Failed to copy code. Please copy manually.', 'error');", true);
            }
        }

        protected void btnChangeCode_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnChangeCode_Click called at " + DateTime.Now.ToString());
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                System.Diagnostics.Debug.WriteLine("btnChangeCode_Click: userId = " + userId);
                
                int? familyId = FamilyHelper.GetUserFamilyId(userId);
                System.Diagnostics.Debug.WriteLine("btnChangeCode_Click: familyId = " + (familyId.HasValue ? familyId.Value.ToString() : "NULL"));
                
                if (!familyId.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine("btnChangeCode_Click: No family ID found");
                    ShowFamilyError("Unable to determine family. Please try refreshing the page.");
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                        "showToastMessage('Unable to determine family. Please try refreshing the page.', 'error');", true);
                    return;
                }

                // Check if user is owner
                int? ownerId = FamilyHelper.GetFamilyOwnerId(familyId.Value);
                System.Diagnostics.Debug.WriteLine("btnChangeCode_Click: ownerId = " + (ownerId.HasValue ? ownerId.Value.ToString() : "NULL"));
                
                if (!ownerId.HasValue || ownerId.Value != userId)
                {
                    System.Diagnostics.Debug.WriteLine("btnChangeCode_Click: User is not owner");
                    ShowFamilyError("Only the family owner can change the family code.");
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                        "showToastMessage('Only the family owner can change the family code.', 'error');", true);
                    return;
                }

                System.Diagnostics.Debug.WriteLine("btnChangeCode_Click: Calling ChangeFamilyCode...");
                bool result = FamilyHelper.ChangeFamilyCode(familyId.Value, userId);
                System.Diagnostics.Debug.WriteLine("btnChangeCode_Click: ChangeFamilyCode returned " + result);
                
                if (result)
                {
                    System.Diagnostics.Debug.WriteLine("btnChangeCode_Click: Code changed successfully, redirecting...");
                    // Redirect to show success message (don't show message here, it will be shown after redirect)
                    Response.Redirect("Family.aspx?codeChanged=true", false);
                    return;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("btnChangeCode_Click: ChangeFamilyCode returned false");
                    ShowFamilyError("Failed to change family code. Please try again or contact support if the problem persists.");
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                        "showToastMessage('Failed to change family code. Please try again or contact support if the problem persists.', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnChangeCode_Click error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                ShowFamilyError("An unexpected error occurred while changing the family code. Please try again.");
                ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                    "showToastMessage('An unexpected error occurred while changing the family code. Please try again.', 'error');", true);
            }
        }

        protected void btnTransferOwnership_Click(object sender, EventArgs e)
        {
            try
            {
                System.Web.UI.WebControls.Button btn = sender as System.Web.UI.WebControls.Button;
                if (btn == null) return;

                int newOwnerId = Convert.ToInt32(btn.CommandArgument);
                int userId = Convert.ToInt32(Session["UserId"]);
                int? familyId = FamilyHelper.GetUserFamilyId(userId);

                if (!familyId.HasValue)
                {
                    ShowFamilyError("Unable to determine family.");
                    return;
                }

                if (FamilyHelper.TransferOwnership(familyId.Value, newOwnerId, userId))
                {
                    ShowFamilySuccess("Ownership transferred successfully!");
                    // Reload family members
                    LoadFamilyMembers(familyId.Value, userId);
                }
                else
                {
                    ShowFamilyError("Failed to transfer ownership. You must be the owner to transfer ownership.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnTransferOwnership_Click error: " + ex.Message);
                ShowFamilyError("An error occurred. Please try again.");
            }
        }

        protected void btnKickParent_Click(object sender, EventArgs e)
        {
            try
            {
                System.Web.UI.WebControls.Button btn = sender as System.Web.UI.WebControls.Button;
                if (btn == null) return;

                int parentId = Convert.ToInt32(btn.CommandArgument);
                        int userId = Convert.ToInt32(Session["UserId"]);
                        int? familyId = FamilyHelper.GetUserFamilyId(userId);

                if (!familyId.HasValue)
                {
                    // Only show toast message (not inline message)
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                        "showToastMessage('Unable to determine family.', 'error');", true);
                    return;
                }

                if (FamilyHelper.KickParent(familyId.Value, parentId, userId))
                {
                    // Only show toast message (not inline message)
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                        "showToastMessage('Parent removed from family successfully.', 'success');", true);
                    // Reload family members
                    LoadFamilyMembers(familyId.Value, userId);
                    }
                    else
                    {
                    // Only show toast message (not inline message)
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                        "showToastMessage('Failed to remove parent. You must be the owner to remove other parents.', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnKickParent_Click error: " + ex.Message);
                // Only show toast message (not inline message)
                ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                    "showToastMessage('An error occurred. Please try again.', 'error');", true);
            }
        }

        protected void btnKickChild_Click(object sender, EventArgs e)
        {
            try
            {
                        int userId = Convert.ToInt32(Session["UserId"]);
                        int? familyId = FamilyHelper.GetUserFamilyId(userId);

                if (!familyId.HasValue)
                {
                    // Only show toast message (not inline message)
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                        "showToastMessage('Unable to determine family.', 'error');", true);
                    return;
                }

                // Get user ID and type from hidden fields
                string targetUserIdStr = hidKickUserId.Value;
                string userType = hidKickUserType.Value;

                if (string.IsNullOrEmpty(targetUserIdStr))
                {
                    // Only show toast message (not inline message)
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                        "showToastMessage('No user selected.', 'error');", true);
                    return;
                }

                int targetUserId = Convert.ToInt32(targetUserIdStr);

                // Handle based on user type
                if (userType == "child")
                {
                    // Verify user is a parent (any parent can kick children)
                    if (Session["UserRole"] == null || Session["UserRole"].ToString() != "PARENT")
                    {
                        // Only show toast message (not inline message)
                        ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                            "showToastMessage('Only parents can remove children from the family.', 'error');", true);
                        return;
                    }

                    if (FamilyHelper.RemoveChildFromFamily(familyId.Value, targetUserId))
                    {
                        // Only show toast message (not inline message)
                        ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                            "showToastMessage('Child removed from family successfully.', 'success');", true);
                        // Reload family members
                        LoadFamilyMembers(familyId.Value, userId);
                    }
                    else
                    {
                        // Only show toast message (not inline message)
                        ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                            "showToastMessage('Failed to remove child. Please try again.', 'error');", true);
                    }
                }
                else if (userType == "parent")
                {
                    // Only owner can kick parents
                if (FamilyHelper.KickParent(familyId.Value, targetUserId, userId))
                {
                    // Only show toast message (not inline message)
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                        "showToastMessage('Parent removed from family successfully.', 'success');", true);
                    // Reload family members
                    LoadFamilyMembers(familyId.Value, userId);
                }
                else
                {
                    // Only show toast message (not inline message)
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                        "showToastMessage('Failed to remove parent. You must be the owner to remove other parents.', 'error');", true);
                }
                }
                else
                {
                    // Only show toast message (not inline message)
                    ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                        "showToastMessage('Invalid user type.', 'error');", true);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnKickChild_Click error: " + ex.Message);
                ShowFamilyError("An error occurred. Please try again.");
                ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                    "showToastMessage('An error occurred. Please try again.', 'error');", true);
            }
        }

        protected void btnLeaveFamily_Click(object sender, EventArgs e)
        {
            try
            {
                    int userId = Convert.ToInt32(Session["UserId"]);
                    int? familyId = FamilyHelper.GetUserFamilyId(userId);
                    
                    if (!familyId.HasValue)
                    {
                    ShowFamilyError("Unable to determine family.");
                        return;
                    }

                // Check if user is owner
                bool isOwner = FamilyHelper.IsFamilyOwner(familyId.Value, userId);
                
                if (isOwner)
                {
                    // Check if owner can leave
                    if (!FamilyHelper.CanOwnerLeave(familyId.Value))
                    {
                        ShowFamilyError("You cannot leave the family while there are children. Please remove all children first or transfer ownership to another parent.");
                        return;
                    }

                    // Owner leaves with auto-transfer
                    if (FamilyHelper.OwnerLeaveFamily(familyId.Value, userId))
                    {
                        // Redirect to family actions page with success message
                        Response.Redirect("Family.aspx?left=true&msg=success", false);
                        return;
                    }
                    else
                    {
                        ShowFamilyError("Failed to leave family. Please try again.");
                        ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                            "showToastMessage('Failed to leave family. Please try again.', 'error');", true);
                    }
                }
                else
                {
                    // Regular parent leaves
                    if (FamilyHelper.LeaveFamily(userId))
                    {
                        // Redirect to family actions page with success message
                        Response.Redirect("Family.aspx?left=true&msg=success", false);
                        return;
                    }
                    else
                    {
                        ShowFamilyError("Failed to leave family. Please try again.");
                        ClientScript.RegisterStartupScript(this.GetType(), "ShowToast", 
                            "showToastMessage('Failed to leave family. Please try again.', 'error');", true);
                    }
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("btnLeaveFamily_Click error: " + ex.Message);
                ShowFamilyError("An error occurred. Please try again.");
            }
        }

        private void ShowFamilyError(string message)
        {
            lblFamilyError.Text = message;
            lblFamilyError.Visible = true;
            lblFamilySuccess.Visible = false;
        }

        private void ShowFamilySuccess(string message)
        {
            lblFamilySuccess.Text = message;
            lblFamilySuccess.Visible = true;
            lblFamilyError.Visible = false;
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
                        string.IsNullOrEmpty(initials) ? "P" : initials.ToUpper());
                    litProfilePlaceholder.Visible = true;
                    imgProfilePicture.Visible = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadProfilePicture error: " + ex.Message);
                // Show default placeholder on error
                litProfilePlaceholder.Text = "<div class=\"profile-avatar-placeholder\">P</div>";
                litProfilePlaceholder.Visible = true;
                imgProfilePicture.Visible = false;
            }
        }

        private void SetLayoutDataAttributes(int userId)
        {
            try
            {
                // Check authentication first
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("SetLayoutDataAttributes: User not authenticated, skipping");
                    return;
                }

                // Only set attributes if panel exists and is visible
                if (pnlFamilyInfo == null)
                {
                    System.Diagnostics.Debug.WriteLine("SetLayoutDataAttributes: Panel is null, skipping");
                    return;
                }

                if (!pnlFamilyInfo.Visible)
                {
                    System.Diagnostics.Debug.WriteLine("SetLayoutDataAttributes: Panel not visible, skipping");
                    return;
                }

                // Find the family layout div
                System.Web.UI.HtmlControls.HtmlGenericControl layoutDiv = pnlFamilyInfo.FindControl("familyLayout") as System.Web.UI.HtmlControls.HtmlGenericControl;
                if (layoutDiv != null)
                {
                    // Set user role
                    string userRole = Session["UserRole"] != null ? Session["UserRole"].ToString() : "";
                    layoutDiv.Attributes["data-current-user-role"] = userRole;

                    // Set owner status
                    bool isOwner = false;
                    int? familyId = FamilyHelper.GetUserFamilyId(userId);
                    if (familyId.HasValue)
                    {
                        isOwner = FamilyHelper.IsFamilyOwner(familyId.Value, userId);
                    }
                    layoutDiv.Attributes["data-current-user-is-owner"] = isOwner.ToString();
                    
                    System.Diagnostics.Debug.WriteLine("SetLayoutDataAttributes: Set role=" + userRole + ", isOwner=" + isOwner);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("SetLayoutDataAttributes: familyLayout control not found");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SetLayoutDataAttributes error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                // Don't throw - this is not critical, just for JavaScript permission checks
            }
        }

        #region Chat Web Methods

        [System.Web.Services.WebMethod]
        public static object GetChatMessages(int familyId, int userId)
        {
            try
            {
                // Verify user is in family
                if (!FamilyHelper.IsFamilyMember(familyId, userId))
                {
                    return new { success = false, error = "User is not a member of this family" };
                }

                DataTable messages = ChatHelper.GetMessages(familyId, userId, 50);
                var messageList = new List<object>();

                foreach (DataRow row in messages.Rows)
                {
                    messageList.Add(new
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        UserId = row["UserId"] != DBNull.Value ? (int?)Convert.ToInt32(row["UserId"]) : null,
                        MessageType = row["MessageType"].ToString(),
                        MessageText = row["MessageText"] != DBNull.Value ? row["MessageText"].ToString() : null,
                        ImagePath = row["ImagePath"] != DBNull.Value ? (row["ImagePath"].ToString().StartsWith("/") ? row["ImagePath"].ToString() : "/" + row["ImagePath"].ToString()) : null,
                        GIFUrl = row["GIFUrl"] != DBNull.Value ? row["GIFUrl"].ToString() : null,
                        ReplyToMessageId = row["ReplyToMessageId"] != DBNull.Value ? (int?)Convert.ToInt32(row["ReplyToMessageId"]) : null,
                        ReplyToMessageText = row["ReplyToMessageText"] != DBNull.Value ? row["ReplyToMessageText"].ToString() : null,
                        ReplyToFirstName = row["ReplyToFirstName"] != DBNull.Value ? row["ReplyToFirstName"].ToString() : null,
                        ReplyToLastName = row["ReplyToLastName"] != DBNull.Value ? row["ReplyToLastName"].ToString() : null,
                        FirstName = row["FirstName"] != DBNull.Value ? row["FirstName"].ToString() : "",
                        LastName = row["LastName"] != DBNull.Value ? row["LastName"].ToString() : "",
                        ProfilePicture = row["ProfilePicture"] != DBNull.Value ? "/Images/ProfilePictures/" + row["ProfilePicture"].ToString() : null,
                        SystemEventType = row["SystemEventType"] != DBNull.Value ? row["SystemEventType"].ToString() : null,
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]).ToString("yyyy-MM-ddTHH:mm:ss")
                    });
                }

                return new { success = true, messages = messageList };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetChatMessages error: " + ex.Message);
                return new { success = false, error = ex.Message };
            }
        }

        [System.Web.Services.WebMethod]
        public static object GetNewChatMessages(int familyId, int userId, int lastMessageId)
        {
            try
            {
                // Verify user is in family
                if (!FamilyHelper.IsFamilyMember(familyId, userId))
                {
                    return new { success = false, error = "User is not a member of this family" };
                }

                DataTable messages = ChatHelper.GetNewMessages(familyId, userId, lastMessageId);
                var messageList = new List<object>();

                foreach (DataRow row in messages.Rows)
                {
                    messageList.Add(new
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        UserId = row["UserId"] != DBNull.Value ? (int?)Convert.ToInt32(row["UserId"]) : null,
                        MessageType = row["MessageType"].ToString(),
                        MessageText = row["MessageText"] != DBNull.Value ? row["MessageText"].ToString() : null,
                        ImagePath = row["ImagePath"] != DBNull.Value ? (row["ImagePath"].ToString().StartsWith("/") ? row["ImagePath"].ToString() : "/" + row["ImagePath"].ToString()) : null,
                        GIFUrl = row["GIFUrl"] != DBNull.Value ? row["GIFUrl"].ToString() : null,
                        ReplyToMessageId = row["ReplyToMessageId"] != DBNull.Value ? (int?)Convert.ToInt32(row["ReplyToMessageId"]) : null,
                        ReplyToMessageText = row["ReplyToMessageText"] != DBNull.Value ? row["ReplyToMessageText"].ToString() : null,
                        ReplyToFirstName = row["ReplyToFirstName"] != DBNull.Value ? row["ReplyToFirstName"].ToString() : null,
                        ReplyToLastName = row["ReplyToLastName"] != DBNull.Value ? row["ReplyToLastName"].ToString() : null,
                        FirstName = row["FirstName"] != DBNull.Value ? row["FirstName"].ToString() : "",
                        LastName = row["LastName"] != DBNull.Value ? row["LastName"].ToString() : "",
                        ProfilePicture = row["ProfilePicture"] != DBNull.Value ? "/Images/ProfilePictures/" + row["ProfilePicture"].ToString() : null,
                        SystemEventType = row["SystemEventType"] != DBNull.Value ? row["SystemEventType"].ToString() : null,
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"]).ToString("yyyy-MM-ddTHH:mm:ss")
                    });
                }

                return new { success = true, messages = messageList };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetNewChatMessages error: " + ex.Message);
                return new { success = false, error = ex.Message };
            }
        }

        [System.Web.Services.WebMethod]
        public static object SendChatMessage(int familyId, int userId, string messageText, int? replyToMessageId)
        {
            try
            {
                // Verify user is in family
                if (!FamilyHelper.IsFamilyMember(familyId, userId))
                {
                    return new { success = false, error = "User is not a member of this family" };
                }

                if (string.IsNullOrWhiteSpace(messageText))
                {
                    return new { success = false, error = "Message cannot be empty" };
                }

                int messageId = ChatHelper.SendMessage(familyId, userId, messageText, replyToMessageId);
                if (messageId > 0)
                {
                    return new { success = true, messageId = messageId };
                }
                else
                {
                    return new { success = false, error = "Failed to send message" };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SendChatMessage error: " + ex.Message);
                return new { success = false, error = ex.Message };
            }
        }

        [System.Web.Services.WebMethod]
        public static object SendChatImage(int familyId, int userId, string imageUrl)
        {
            try
            {
                // Verify user is in family
                if (!FamilyHelper.IsFamilyMember(familyId, userId))
                {
                    return new { success = false, error = "User is not a member of this family" };
                }

                if (string.IsNullOrEmpty(imageUrl))
                {
                    return new { success = false, error = "Image URL is required" };
                }

                // Image URL is already relative (Images/FamilyChat/...)
                // Store as relative path in database
                int messageId = ChatHelper.SendImageMessage(familyId, userId, imageUrl);
                if (messageId > 0)
                {
                    return new { success = true, messageId = messageId };
                }
                else
                {
                    return new { success = false, error = "Failed to send image" };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SendChatImage error: " + ex.Message);
                return new { success = false, error = ex.Message };
            }
        }

        [System.Web.Services.WebMethod]
        public static object SendChatGIF(int familyId, int userId, string gifUrl)
        {
            try
            {
                // Verify user is in family
                if (!FamilyHelper.IsFamilyMember(familyId, userId))
                {
                    return new { success = false, error = "User is not a member of this family" };
                }

                if (string.IsNullOrEmpty(gifUrl))
                {
                    return new { success = false, error = "GIF URL is required" };
                }

                // Send GIF message
                int messageId = ChatHelper.SendGIFMessage(familyId, userId, gifUrl);
                if (messageId > 0)
                {
                    return new { success = true, messageId = messageId };
                }
                else
                {
                    return new { success = false, error = "Failed to send GIF" };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SendChatGIF error: " + ex.Message);
                return new { success = false, error = ex.Message };
            }
        }

        [System.Web.Services.WebMethod]
        public static object GetMessageReactions(int messageId, int userId)
        {
            try
            {
                DataTable reactions = ChatHelper.GetReactions(messageId);
                var reactionList = new List<object>();

                foreach (DataRow row in reactions.Rows)
                {
                    reactionList.Add(new
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        MessageId = Convert.ToInt32(row["MessageId"]),
                        UserId = Convert.ToInt32(row["UserId"]),
                        ReactionType = row["ReactionType"].ToString(),
                        FirstName = row["FirstName"] != DBNull.Value ? row["FirstName"].ToString() : "",
                        LastName = row["LastName"] != DBNull.Value ? row["LastName"].ToString() : ""
                    });
                }

                return new { success = true, reactions = reactionList };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetMessageReactions error: " + ex.Message);
                return new { success = false, error = ex.Message };
            }
        }

        [System.Web.Services.WebMethod]
        public static object ToggleReaction(int messageId, int userId, string reactionType)
        {
            try
            {
                bool success = ChatHelper.AddReaction(messageId, userId, reactionType);
                return new { success = success };
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ToggleReaction error: " + ex.Message);
                return new { success = false, error = ex.Message };
            }
        }

        #endregion
    }
}

