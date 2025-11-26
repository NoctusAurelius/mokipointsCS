# Patch 5.0.2 - Validation and Requirements

**Date Created**: November 26, 2024  
**Status**: In Progress  
**Priority**: High

---

## 📋 Overview

This patch document tracks validation and requirement issues identified during development and testing. Each issue includes a description, root cause analysis, and recommended fix.

---

## 🐛 Bug #1: Reward Creation - Missing Input Requirements Validation

### Description

When creating a Reward, users can input only the name and description and attempt to create the reward immediately, which results in an error. The system lacks proper input requirement validation for creating rewards.

**Example Scenario**:
- User fills in only:
  - ✅ Reward Name: "Test Reward"
  - ✅ Description: "This is a test"
  - ❌ Point Cost: (empty)
  - ❌ Category: (not selected)
- User clicks "Create Reward"
- ❌ **Result**: Error occurs because required fields are missing

**Expected Behavior**:
- System should validate all required fields before submission
- Clear error messages should indicate which fields are required
- Form should prevent submission until all required fields are filled

### Root Cause Analysis

**Location**: 
- `Rewards.aspx` - Reward creation form (missing RequiredFieldValidator controls)
- `Rewards.aspx.cs` - `btnCreateRewardSubmit_Click` method (lines 236-290)
  - Line 251: `int pointCost = Convert.ToInt32(txtCreatePointCost.Text);` - Will throw exception if field is empty
  - Line 255-259: Only validates if name is empty, but doesn't prevent empty point cost
  - Line 261-265: Only validates if pointCost <= 0, but exception occurs before this check if field is empty

**Current Implementation**:

**Rewards.aspx.cs** (lines 236-290):
```csharp
protected void btnCreateRewardSubmit_Click(object sender, EventArgs e)
{
    try
    {
        // ... existing code ...
        
        string name = txtCreateName.Text.Trim();
        string description = txtCreateDescription.Text.Trim();
        int pointCost = Convert.ToInt32(txtCreatePointCost.Text); // ❌ Throws exception if empty
        string category = ddlCreateCategory.SelectedValue;
        string imageUrl = txtCreateImageUrl.Text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            ShowError("Reward name is required.");
            return;
        }

        if (pointCost <= 0) // ❌ Never reached if pointCost field is empty (exception thrown above)
        {
            ShowError("Point cost must be greater than 0.");
            return;
        }
        
        // ... rest of code ...
    }
    catch (Exception ex)
    {
        // Generic error message - doesn't indicate which field is missing
        ShowError("An error occurred while creating the reward.");
    }
}
```

**Issues Identified**:
1. ❌ No `RequiredFieldValidator` for `txtCreatePointCost` in `Rewards.aspx`
2. ❌ No validation to check if `txtCreatePointCost.Text` is empty before `Convert.ToInt32()`
3. ❌ No clear indication of which fields are required in the UI
4. ❌ Generic error message doesn't help user identify the problem
5. ❌ No client-side validation to prevent form submission

### Recommended Fix

**1. Add RequiredFieldValidator Controls to Rewards.aspx**:
- Add `RequiredFieldValidator` for Reward Name
- Add `RequiredFieldValidator` for Point Cost
- Optionally add `RequiredFieldValidator` for Category if it's required
- Set `ValidationGroup` to group validators together
- Set `Display="Dynamic"` for better UX
- Add `CssClass="error-message"` for consistent styling

**2. Update btnCreateRewardSubmit_Click in Rewards.aspx.cs**:
- Add `Page.IsValid` check before processing
- Add try-catch around `Convert.ToInt32()` with specific error message
- Validate point cost field is not empty before conversion
- Provide specific error messages for each validation failure

**3. Add Visual Indicators**:
- Add asterisk (*) or "Required" label to required fields
- Use consistent styling for required field indicators

**Example Fix**:

**Rewards.aspx** (add RequiredFieldValidator):
```html
<div class="form-group">
    <label>Reward Name <span class="required">*</span></label>
    <asp:TextBox ID="txtCreateName" runat="server" CssClass="form-control" />
    <asp:RequiredFieldValidator ID="rfvCreateName" runat="server" 
        ControlToValidate="txtCreateName" 
        ErrorMessage="Reward name is required." 
        ValidationGroup="CreateReward" 
        Display="Dynamic" 
        CssClass="error-message" />
</div>

<div class="form-group">
    <label>Point Cost <span class="required">*</span></label>
    <asp:TextBox ID="txtCreatePointCost" runat="server" CssClass="form-control" TextMode="Number" />
    <asp:RequiredFieldValidator ID="rfvCreatePointCost" runat="server" 
        ControlToValidate="txtCreatePointCost" 
        ErrorMessage="Point cost is required." 
        ValidationGroup="CreateReward" 
        Display="Dynamic" 
        CssClass="error-message" />
    <asp:RangeValidator ID="rvCreatePointCost" runat="server" 
        ControlToValidate="txtCreatePointCost" 
        Type="Integer" 
        MinimumValue="1" 
        MaximumValue="999999" 
        ErrorMessage="Point cost must be between 1 and 999,999." 
        ValidationGroup="CreateReward" 
        Display="Dynamic" 
        CssClass="error-message" />
</div>
```

**Rewards.aspx.cs** (update validation):
```csharp
protected void btnCreateRewardSubmit_Click(object sender, EventArgs e)
{
    try
    {
        // Validate page before processing
        if (!Page.IsValid)
        {
            return;
        }

        int userId = Convert.ToInt32(Session["UserId"]);
        int? familyId = FamilyHelper.GetUserFamilyId(userId);

        if (!familyId.HasValue)
        {
            ShowError("You must be in a family to create rewards.");
            return;
        }

        string name = txtCreateName.Text.Trim();
        string description = txtCreateDescription.Text.Trim();
        
        // Validate point cost with proper error handling
        int pointCost = 0;
        if (string.IsNullOrEmpty(txtCreatePointCost.Text.Trim()))
        {
            ShowError("Point cost is required.");
            return;
        }
        
        if (!int.TryParse(txtCreatePointCost.Text.Trim(), out pointCost))
        {
            ShowError("Point cost must be a valid number.");
            return;
        }
        
        if (pointCost <= 0)
        {
            ShowError("Point cost must be greater than 0.");
            return;
        }
        
        string category = ddlCreateCategory.SelectedValue;
        string imageUrl = txtCreateImageUrl.Text.Trim();

        if (string.IsNullOrEmpty(name))
        {
            ShowError("Reward name is required.");
            return;
        }

        bool success = RewardHelper.CreateReward(familyId.Value, userId, name, 
            string.IsNullOrEmpty(description) ? null : description,
            pointCost,
            string.IsNullOrEmpty(category) ? null : category,
            string.IsNullOrEmpty(imageUrl) ? null : imageUrl);

        if (success)
        {
            ShowSuccess("Reward created successfully!");
            LoadRewards();
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseCreateModal", "closeCreateModal();", true);
        }
        else
        {
            ShowError("Failed to create reward. Please try again.");
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine("btnCreateRewardSubmit_Click error: " + ex.Message);
        System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
        ShowError("An error occurred while creating the reward: " + ex.Message);
    }
}
```

### Testing Checklist

- [ ] Test creating reward with only name and description (should fail with clear error)
- [ ] Test creating reward with empty point cost field (should show validation error)
- [ ] Test creating reward with invalid point cost (non-numeric) (should show validation error)
- [ ] Test creating reward with point cost = 0 (should show validation error)
- [ ] Test creating reward with point cost < 0 (should show validation error)
- [ ] Test creating reward with all required fields filled (should succeed)
- [ ] Verify RequiredFieldValidator displays error messages correctly
- [ ] Verify RangeValidator displays error messages correctly
- [ ] Verify form cannot be submitted until all validations pass
- [ ] Verify error messages are clear and helpful

### Expected Results

After implementing the fix:
- ✅ Users cannot submit the form without filling required fields
- ✅ Clear error messages indicate which fields are missing or invalid
- ✅ Point cost field is properly validated before conversion
- ✅ No exceptions are thrown due to empty or invalid input
- ✅ Visual indicators (asterisks) show which fields are required
- ✅ Better user experience with immediate feedback

---

## 🐛 Bug #2: Task Creation - Missing Validation and Error Handling

### Description

When creating a Task, users can input only the title, category, points, and description (without objectives and instructions) and attempt to create the task. The task doesn't get created, but the modal closes without any error message, leaving the user confused about what went wrong.

**Example Scenario**:
- User fills in only:
  - ✅ Task Title: "Test Task"
  - ✅ Category: "Chores"
  - ✅ Points: "100"
  - ✅ Description: "This is a test task"
  - ❌ Objectives: (empty/none added)
  - ❌ Instructions: (empty)
- User clicks "Create Task"
- ❌ **Result**: Modal closes silently, no task is created, no error message shown

**Expected Behavior**:
- System should validate that at least one objective is required
- Clear error message should indicate that objectives are required
- Modal should remain open when validation fails
- Error message should be visible to the user

### Root Cause Analysis

**Location**: 
- `Tasks.aspx` - Task creation form (may be missing RequiredFieldValidator or validation indicators)
- `Tasks.aspx.cs` - `btnCreateTaskSubmit_Click` method (lines 212-288)
  - Line 216-219: `Page.IsValid` check returns early without showing error message
  - Line 244-248: Validates objectives count, but error may not be visible if modal closes
  - Line 254: `int points = Convert.ToInt32(txtCreatePoints.Text);` - Will throw exception if field is empty
  - Line 276: Modal closes via JavaScript even if there's an error (should only close on success)

**Current Implementation**:

**Tasks.aspx.cs** (lines 212-288):
```csharp
protected void btnCreateTaskSubmit_Click(object sender, EventArgs e)
{
    try
    {
        if (!Page.IsValid) // ❌ Returns early without showing error
        {
            return;
        }

        // ... existing code ...

        // Collect objectives from form
        List<string> objectives = new List<string>();
        foreach (string key in Request.Form.AllKeys)
        {
            if (key.StartsWith("objective_"))
            {
                string value = Request.Form[key];
                if (!string.IsNullOrEmpty(value.Trim()))
                {
                    objectives.Add(value.Trim());
                }
            }
        }

        if (objectives.Count == 0)
        {
            ShowError("At least one objective is required."); // ✅ Error shown, but...
            return; // ❌ Modal might still close via JavaScript
        }

        // Get form values
        string title = txtCreateTitle.Text.Trim();
        string description = txtCreateDescription.Text.Trim();
        string category = ddlCreateCategory.SelectedValue;
        int points = Convert.ToInt32(txtCreatePoints.Text); // ❌ Throws exception if empty
        // ... rest of code ...

        if (taskId > 0)
        {
            ShowSuccess("Task created successfully!");
            LoadTasks();
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseCreateModal", "closeCreateModal();", true);
        }
        else
        {
            ShowError("Failed to create task. Please try again.");
            // ❌ Modal might still close - no explicit "don't close" instruction
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine("btnCreateTaskSubmit_Click error: " + ex.Message);
        ShowError("An error occurred while creating the task."); // ❌ Generic error, modal might close
    }
}
```

**Issues Identified**:
1. ❌ `Page.IsValid` check returns early without showing specific error message
2. ❌ No validation to check if `txtCreatePoints.Text` is empty before `Convert.ToInt32()`
3. ❌ Modal closes via JavaScript even when there's an error (line 276 executes regardless)
4. ❌ No explicit instruction to keep modal open when validation fails
5. ❌ Generic error messages don't help user identify the specific problem
6. ❌ No visual indication that objectives are required fields

### Recommended Fix

**1. Update btnCreateTaskSubmit_Click in Tasks.aspx.cs**:
- Add specific error message when `Page.IsValid` fails
- Add validation for points field before `Convert.ToInt32()`
- Only close modal on successful task creation
- Keep modal open when validation fails or errors occur
- Provide specific error messages for each validation failure

**2. Add Visual Indicators in Tasks.aspx**:
- Add asterisk (*) or "Required" label to required fields (title, category, points, objectives)
- Make it clear that at least one objective is required

**3. Improve Error Handling**:
- Ensure error messages are visible before modal closes
- Add client-side validation to prevent form submission if objectives are empty
- Add server-side validation with clear error messages

**Example Fix**:

**Tasks.aspx.cs** (update validation):
```csharp
protected void btnCreateTaskSubmit_Click(object sender, EventArgs e)
{
    try
    {
        // Validate page before processing
        if (!Page.IsValid)
        {
            ShowError("Please fill in all required fields correctly.");
            // Don't close modal - keep it open to show errors
            return;
        }

        int userId = Convert.ToInt32(Session["UserId"]);
        int? familyId = FamilyHelper.GetUserFamilyId(userId);

        if (!familyId.HasValue)
        {
            ShowError("You must be in a family to create tasks.");
            return; // Keep modal open
        }

        // Collect objectives from form
        List<string> objectives = new List<string>();
        foreach (string key in Request.Form.AllKeys)
        {
            if (key.StartsWith("objective_"))
            {
                string value = Request.Form[key];
                if (!string.IsNullOrEmpty(value.Trim()))
                {
                    objectives.Add(value.Trim());
                }
            }
        }

        if (objectives.Count == 0)
        {
            ShowError("At least one objective is required. Please add at least one objective before creating the task.");
            return; // Keep modal open
        }

        // Get form values
        string title = txtCreateTitle.Text.Trim();
        string description = txtCreateDescription.Text.Trim();
        string category = ddlCreateCategory.SelectedValue;
        
        // Validate points with proper error handling
        int points = 0;
        if (string.IsNullOrEmpty(txtCreatePoints.Text.Trim()))
        {
            ShowError("Points is required.");
            return; // Keep modal open
        }
        
        if (!int.TryParse(txtCreatePoints.Text.Trim(), out points))
        {
            ShowError("Points must be a valid number.");
            return; // Keep modal open
        }
        
        if (points <= 0)
        {
            ShowError("Points must be greater than 0.");
            return; // Keep modal open
        }
        
        string priority = ddlCreatePriority.SelectedValue;
        string difficulty = ddlCreateDifficulty.SelectedValue;
        int? estimatedMinutes = null;
        if (!string.IsNullOrEmpty(txtCreateEstimatedMinutes.Text))
        {
            if (!int.TryParse(txtCreateEstimatedMinutes.Text, out int estMins))
            {
                ShowError("Estimated minutes must be a valid number.");
                return; // Keep modal open
            }
            estimatedMinutes = estMins;
        }
        string instructions = txtCreateInstructions.Text.Trim();
        string recurrencePattern = ddlCreateRecurrence.SelectedValue;

        // Validate title
        if (string.IsNullOrEmpty(title))
        {
            ShowError("Task title is required.");
            return; // Keep modal open
        }

        // Validate category
        if (string.IsNullOrEmpty(category))
        {
            ShowError("Category is required.");
            return; // Keep modal open
        }

        // Create task
        int taskId = TaskHelper.CreateTask(title, description, category, points, userId, familyId.Value, objectives,
            priority, string.IsNullOrEmpty(difficulty) ? null : difficulty, estimatedMinutes, 
            string.IsNullOrEmpty(instructions) ? null : instructions, 
            string.IsNullOrEmpty(recurrencePattern) ? null : recurrencePattern);

        if (taskId > 0)
        {
            ShowSuccess("Task created successfully!");
            LoadTasks();
            // Only close modal on success
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseCreateModal", "closeCreateModal();", true);
        }
        else
        {
            ShowError("Failed to create task. Please try again.");
            // Don't close modal - keep it open to show error
        }
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine("btnCreateTaskSubmit_Click error: " + ex.Message);
        System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
        ShowError("An error occurred while creating the task: " + ex.Message);
        // Don't close modal - keep it open to show error
    }
}
```

**Tasks.aspx** (add visual indicators):
```html
<div class="form-group">
    <label>Task Title <span class="required">*</span></label>
    <asp:TextBox ID="txtCreateTitle" runat="server" CssClass="form-control" />
    <asp:RequiredFieldValidator ID="rfvCreateTitle" runat="server" 
        ControlToValidate="txtCreateTitle" 
        ErrorMessage="Task title is required." 
        ValidationGroup="CreateTask" 
        Display="Dynamic" 
        CssClass="error-message" />
</div>

<div class="form-group">
    <label>Points <span class="required">*</span></label>
    <asp:TextBox ID="txtCreatePoints" runat="server" CssClass="form-control" TextMode="Number" />
    <asp:RequiredFieldValidator ID="rfvCreatePoints" runat="server" 
        ControlToValidate="txtCreatePoints" 
        ErrorMessage="Points is required." 
        ValidationGroup="CreateTask" 
        Display="Dynamic" 
        CssClass="error-message" />
    <asp:RangeValidator ID="rvCreatePoints" runat="server" 
        ControlToValidate="txtCreatePoints" 
        Type="Integer" 
        MinimumValue="1" 
        MaximumValue="999999" 
        ErrorMessage="Points must be between 1 and 999,999." 
        ValidationGroup="CreateTask" 
        Display="Dynamic" 
        CssClass="error-message" />
</div>

<div class="form-group">
    <label>Objectives <span class="required">*</span></label>
    <small class="form-text text-muted">At least one objective is required</small>
    <!-- Objectives input fields -->
</div>
```

### Testing Checklist

- [ ] Test creating task with only title, category, points, description (no objectives) - should show error and keep modal open
- [ ] Test creating task with empty points field - should show validation error
- [ ] Test creating task with invalid points (non-numeric) - should show validation error
- [ ] Test creating task with points = 0 - should show validation error
- [ ] Test creating task with points < 0 - should show validation error
- [ ] Test creating task with empty title - should show validation error
- [ ] Test creating task with empty category - should show validation error
- [ ] Test creating task with all required fields filled including at least one objective - should succeed and close modal
- [ ] Verify error messages are visible before modal closes
- [ ] Verify modal only closes on successful task creation
- [ ] Verify modal stays open when validation fails
- [ ] Verify RequiredFieldValidator displays error messages correctly
- [ ] Verify RangeValidator displays error messages correctly

### Expected Results

After implementing the fix:
- ✅ Users cannot submit the form without filling required fields
- ✅ Clear error messages indicate which fields are missing or invalid
- ✅ Modal stays open when validation fails or errors occur
- ✅ Modal only closes on successful task creation
- ✅ Points field is properly validated before conversion
- ✅ No exceptions are thrown due to empty or invalid input
- ✅ Visual indicators (asterisks) show which fields are required
- ✅ Better user experience with immediate feedback
- ✅ Users understand that at least one objective is required

---

## 🐛 Bug #3: Family Page - Review Button Visible to Children

### Description

In the child account, when navigating to the Family page, the "Review" button appears in the top navigation bar. This button should only be visible to parents, not children, as children cannot review tasks.

**Example Scenario**:
- User logs in as a child account
- User navigates to Family page
- ❌ **Issue**: "Review" button is visible in the top navigation bar
- User clicks "Review" button
- ❌ **Result**: Child can access TaskReview.aspx page (which should be parent-only)

**Expected Behavior**:
- "Review" button should only be visible to parents (PARENT role)
- "Review" button should be hidden for children (CHILD role)
- Navigation should be role-based

### Root Cause Analysis

**Location**: 
- `Family.aspx` - Navigation links in header section (line 3263)
  - Line 3263: Hardcoded `<a href="TaskReview.aspx">Review</a>` link visible to all users
  - No role-based conditional rendering for the Review link

**Current Implementation**:

**Family.aspx** (lines 3259-3266):
```html
<div class="nav-links" style="display: flex; gap: 20px; align-items: center; margin-right: 20px;">
    <a href="ParentDashboard.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Dashboard</a>
    <a href="Family.aspx" class="active" style="color: #0066CC; text-decoration: none; font-weight: 500; font-size: 16px;">Family</a>
    <a href="Tasks.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Tasks</a>
    <a href="TaskReview.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Review</a> <!-- ❌ Always visible -->
    <a href="Rewards.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Rewards</a>
    <a href="RewardOrders.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Orders</a>
</div>
```

**Issues Identified**:
1. ❌ Review link is hardcoded and always visible
2. ❌ No role-based conditional rendering
3. ❌ Children can see and access parent-only functionality
4. ❌ Inconsistent with other pages that may have role-based navigation

### Recommended Fix

**1. Update Family.aspx to Conditionally Show Review Link**:
- Wrap the Review link in an `<asp:Panel>` or use server-side conditional rendering
- Check user role in code-behind (`Family.aspx.cs`)
- Only show Review link when `Session["UserRole"] == "PARENT"`

**2. Alternative: Use Literal Control for Dynamic Navigation**:
- Create navigation links dynamically in code-behind based on user role
- More flexible approach for future role-based navigation changes

**Example Fix**:

**Family.aspx** (update navigation):
```html
<div class="nav-links" style="display: flex; gap: 20px; align-items: center; margin-right: 20px;">
    <a href="ParentDashboard.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Dashboard</a>
    <a href="Family.aspx" class="active" style="color: #0066CC; text-decoration: none; font-weight: 500; font-size: 16px;">Family</a>
    <a href="Tasks.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Tasks</a>
    <asp:Panel ID="pnlReviewLink" runat="server" Visible="false">
        <a href="TaskReview.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Review</a>
    </asp:Panel>
    <a href="Rewards.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Rewards</a>
    <a href="RewardOrders.aspx" style="color: #333; text-decoration: none; font-weight: 500; font-size: 16px;">Orders</a>
</div>
```

**Family.aspx.cs** (add role-based visibility):
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    // ... existing code ...
    
    if (!IsPostBack)
    {
        // ... existing code ...
        
        // Show Review link only for parents
        string userRole = Session["UserRole"]?.ToString() ?? "";
        if (pnlReviewLink != null)
        {
            pnlReviewLink.Visible = (userRole == "PARENT");
        }
        
        // ... rest of code ...
    }
}
```

### Testing Checklist

- [ ] Test as parent account - Review button should be visible
- [ ] Test as child account - Review button should be hidden
- [ ] Verify navigation layout doesn't break when Review link is hidden
- [ ] Verify child cannot access TaskReview.aspx directly via URL (should be handled by TaskReview.aspx.cs)
- [ ] Verify other navigation links still work correctly
- [ ] Test navigation on different screen sizes (responsive design)

### Expected Results

After implementing the fix:
- ✅ Review button only visible to parents
- ✅ Review button hidden for children
- ✅ Navigation layout remains consistent
- ✅ Better role-based access control
- ✅ Improved user experience (children don't see irrelevant options)

---

## 🔄 Implementation Status

| Bug # | Description | Status | Assigned | Date Fixed |
|-------|-------------|--------|----------|------------|
| #1 | Reward creation - missing input requirements validation | ✅ Fixed | - | 2024-11-26 |
| #2 | Task creation - missing validation and error handling | ✅ Fixed | - | 2024-11-26 |
| #3 | Family page - Review button visible to children | ✅ Fixed | - | 2024-11-26 |

---

## 📝 Implementation Summary

All bugs in Patch 5.0.2 have been successfully implemented:

1. ✅ **Bug #1**: Reward creation validation - Added RequiredFieldValidator and RangeValidator controls, improved server-side validation with proper error handling, and ensured modal only closes on success
2. ✅ **Bug #2**: Task creation validation - Added comprehensive validation for all required fields, improved error handling for points field, added objectives requirement message, and ensured modal only closes on success
3. ✅ **Bug #3**: Family page Review button - Wrapped Review link in Panel control and added role-based visibility (only visible to parents)

**Total Issues Fixed**: 3  
**Implementation Date**: November 26, 2024  
**Status**: All fixes completed and ready for testing

---

**Last Updated**: November 26, 2024  
**Next Review**: After testing and user feedback

