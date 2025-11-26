# Patch 5.0.1 - Logic Fix and Adjustments

**Date Created**: November 26, 2024  
**Status**: In Progress  
**Priority**: High

---

## 📋 Overview

This patch document tracks logic fixes and system adjustments identified during development and testing. Each issue includes a description, root cause analysis, and recommended fix.

---

## 🐛 Bug #1: Parent Deadline Logic - No Past Dates and Minimum 10 Minutes Ahead

### Description

Parents can currently set deadline dates that have already passed, or set deadlines too close to the current time. The system should enforce that:

1. **No past dates**: Parents cannot set a deadline date/time that has already passed
2. **Minimum 10 minutes ahead**: Parents can only set deadline dates at least 10 minutes in the future

**Example Scenario**:
- Current date/time: `11/26/2025 9:00 AM`
- ❌ **Invalid**: `11/26/2025 8:50 AM` (past date)
- ❌ **Invalid**: `11/26/2025 9:00 AM` (current time)
- ❌ **Invalid**: `11/26/2025 9:05 AM` (less than 10 minutes ahead)
- ✅ **Valid**: `11/26/2025 9:10 AM` (exactly 10 minutes ahead)
- ✅ **Valid**: `11/26/2025 9:20 AM` (more than 10 minutes ahead)
- ✅ **Valid**: `11/27/2025 9:00 AM` (future date)

### Root Cause Analysis

**Location**: 
- `AssignTask.aspx` - Deadline date/time input fields (lines 394-400)
- `AssignTask.aspx.cs` - Deadline parsing and validation (lines 147-161)
- `App_Code/TaskHelper.cs` - `AssignTask` method deadline validation (lines 722-727)

**Current Implementation**:

**AssignTask.aspx.cs** (lines 147-161):
```csharp
// Parse deadline
DateTime? deadline = null;
if (!string.IsNullOrEmpty(txtDeadlineDate.Text))
{
    DateTime deadlineDate = Convert.ToDateTime(txtDeadlineDate.Text);
    if (!string.IsNullOrEmpty(txtDeadlineTime.Text))
    {
        TimeSpan time = TimeSpan.Parse(txtDeadlineTime.Text);
        deadline = deadlineDate.Date.Add(time);
    }
    else
    {
        deadline = deadlineDate.Date;
    }
}
```

**TaskHelper.cs** (lines 722-727):
```csharp
// Fix #5: Server-side deadline validation
if (deadline.HasValue && deadline.Value <= DateTime.Now)
{
    System.Diagnostics.Debug.WriteLine("AssignTask: Deadline is in the past - validation failed");
    return false; // Deadline must be in the future
}
```

**Problems**:
1. **No client-side validation**: Users can select past dates or times in the date/time pickers without immediate feedback
2. **Insufficient server-side validation**: Current validation only checks if deadline is `<= DateTime.Now`, but doesn't enforce the 10-minute minimum requirement
3. **No user-friendly error messages**: When validation fails, the error message is generic ("Failed to assign task...")
4. **Date picker allows past dates**: HTML5 date picker doesn't have `min` attribute set to prevent selecting past dates
5. **Time picker allows immediate times**: HTML5 time picker doesn't validate minimum time requirement

**Why it happens**:
- The current validation only checks if deadline is in the past (`<= DateTime.Now`)
- No minimum time buffer is enforced (10 minutes)
- HTML5 date/time inputs don't have constraints to prevent invalid selections
- Client-side validation is missing, so users only discover the error after form submission

### Recommended Fix

**Step 1: Add Client-Side Validation in AssignTask.aspx**

Add JavaScript validation and set minimum values for date/time inputs:

```javascript
<script type="text/javascript">
    function validateDeadline() {
        var dateInput = document.getElementById('<%= txtDeadlineDate.ClientID %>');
        var timeInput = document.getElementById('<%= txtDeadlineTime.ClientID %>');
        
        if (!dateInput || !dateInput.value) {
            return true; // Deadline is optional
        }
        
        var deadlineDate = new Date(dateInput.value);
        var deadlineTime = timeInput && timeInput.value ? timeInput.value.split(':') : null;
        
        if (deadlineTime) {
            deadlineDate.setHours(parseInt(deadlineTime[0]), parseInt(deadlineTime[1]), 0, 0);
        } else {
            // If no time specified, set to end of day (23:59)
            deadlineDate.setHours(23, 59, 0, 0);
        }
        
        var now = new Date();
        var minDeadline = new Date(now.getTime() + (10 * 60 * 1000)); // 10 minutes from now
        
        if (deadlineDate <= now) {
            alert('Deadline must be in the future. Please select a date/time that has not passed.');
            return false;
        }
        
        if (deadlineDate < minDeadline) {
            var minTimeStr = minDeadline.toLocaleTimeString('en-US', { hour: '2-digit', minute: '2-digit' });
            alert('Deadline must be at least 10 minutes in the future. The earliest deadline is ' + minTimeStr + '.');
            return false;
        }
        
        return true;
    }
    
    // Set minimum date on page load
    window.onload = function() {
        var dateInput = document.getElementById('<%= txtDeadlineDate.ClientID %>');
        if (dateInput) {
            var today = new Date();
            var minDate = today.toISOString().split('T')[0];
            dateInput.setAttribute('min', minDate);
        }
    };
</script>
```

**Step 2: Update AssignTask.aspx.cs with Enhanced Validation**

Replace the deadline parsing section (lines 147-161) with:

```csharp
// Parse deadline with validation
DateTime? deadline = null;
if (!string.IsNullOrEmpty(txtDeadlineDate.Text))
{
    DateTime deadlineDate = Convert.ToDateTime(txtDeadlineDate.Text);
    if (!string.IsNullOrEmpty(txtDeadlineTime.Text))
    {
        TimeSpan time = TimeSpan.Parse(txtDeadlineTime.Text);
        deadline = deadlineDate.Date.Add(time);
    }
    else
    {
        // If no time specified, set to end of day (23:59:59)
        deadline = deadlineDate.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
    }
    
    // Validate deadline: must be at least 10 minutes in the future
    DateTime now = DateTime.Now;
    DateTime minDeadline = now.AddMinutes(10);
    
    if (deadline.Value <= now)
    {
        ShowError("Deadline must be in the future. Please select a date/time that has not passed.");
        return;
    }
    
    if (deadline.Value < minDeadline)
    {
        ShowError(string.Format("Deadline must be at least 10 minutes in the future. The earliest deadline is {0:MMM dd, yyyy hh:mm tt}.", minDeadline));
        return;
    }
}
```

**Step 3: Update TaskHelper.cs with Enhanced Validation**

Replace the deadline validation in `AssignTask` method (lines 722-727) with:

```csharp
// Enhanced deadline validation: must be at least 10 minutes in the future
if (deadline.HasValue)
{
    DateTime now = DateTime.Now;
    DateTime minDeadline = now.AddMinutes(10);
    
    if (deadline.Value <= now)
    {
        System.Diagnostics.Debug.WriteLine("AssignTask: Deadline is in the past - validation failed. Deadline: " + deadline.Value.ToString() + ", Now: " + now.ToString());
        return false; // Deadline must be in the future
    }
    
    if (deadline.Value < minDeadline)
    {
        System.Diagnostics.Debug.WriteLine("AssignTask: Deadline is less than 10 minutes ahead - validation failed. Deadline: " + deadline.Value.ToString() + ", MinDeadline: " + minDeadline.ToString());
        return false; // Deadline must be at least 10 minutes in the future
    }
}
```

**Step 4: Update AssignTask.aspx HTML**

Add `OnClientClick` validation to the submit button (line 403):

```html
<asp:Button ID="btnAssignTask" runat="server" Text="Assign Task" CssClass="btn-submit" 
    ValidationGroup="AssignTask" OnClick="btnAssignTask_Click" 
    OnClientClick="return validateDeadline();" />
```

**Step 5: Set Minimum Date/Time Attributes (Optional Enhancement)**

Add `min` attribute to date input in `AssignTask.aspx` (line 395):

```html
<asp:TextBox ID="txtDeadlineDate" runat="server" CssClass="form-control" TextMode="Date" />
```

And add JavaScript to set minimum date dynamically:

```javascript
// Set minimum date to today
var dateInput = document.getElementById('<%= txtDeadlineDate.ClientID %>');
if (dateInput) {
    var today = new Date();
    var minDate = today.toISOString().split('T')[0];
    dateInput.setAttribute('min', minDate);
    
    // If today is selected, set minimum time to 10 minutes from now
    dateInput.addEventListener('change', function() {
        var selectedDate = new Date(this.value);
        var today = new Date();
        if (selectedDate.toDateString() === today.toDateString()) {
            var timeInput = document.getElementById('<%= txtDeadlineTime.ClientID %>');
            if (timeInput) {
                var minTime = new Date(today.getTime() + (10 * 60 * 1000));
                var minTimeStr = minTime.toTimeString().substring(0, 5); // HH:mm format
                timeInput.setAttribute('min', minTimeStr);
            }
        } else {
            var timeInput = document.getElementById('<%= txtDeadlineTime.ClientID %>');
            if (timeInput) {
                timeInput.removeAttribute('min');
            }
        }
    });
}
```

### Files to Modify
- `mokipointsCS/AssignTask.aspx` - Add JavaScript validation and update button
- `mokipointsCS/AssignTask.aspx.cs` - Enhance deadline parsing and validation
- `mokipointsCS/App_Code/TaskHelper.cs` - Update `AssignTask` method deadline validation

### Testing Checklist
- [ ] Test with past date: Set deadline to yesterday - should show error
- [ ] Test with current date but past time: Set deadline to today 8:00 AM when current time is 9:00 AM - should show error
- [ ] Test with current date and current time: Set deadline to current date/time - should show error
- [ ] Test with current date and time less than 10 minutes ahead: Set deadline to 9:05 AM when current time is 9:00 AM - should show error
- [ ] Test with current date and time exactly 10 minutes ahead: Set deadline to 9:10 AM when current time is 9:00 AM - should succeed
- [ ] Test with current date and time more than 10 minutes ahead: Set deadline to 9:20 AM when current time is 9:00 AM - should succeed
- [ ] Test with future date: Set deadline to tomorrow - should succeed
- [ ] Test with no deadline (optional): Leave deadline fields empty - should succeed
- [ ] Test client-side validation: JavaScript should prevent form submission with invalid deadline
- [ ] Test server-side validation: Server should reject invalid deadlines even if client-side validation is bypassed
- [ ] Verify error messages are clear and user-friendly
- [ ] Test with different time zones (if applicable)
- [ ] Test edge case: Set deadline exactly at 10 minutes from now

### Expected Result
- Users cannot select past dates in the date picker
- Users cannot select times that are less than 10 minutes in the future
- Clear error messages explain why the deadline is invalid
- Both client-side and server-side validation enforce the rules
- Validation works correctly for all date/time combinations
- Optional deadline (empty fields) still works correctly

### Additional Considerations
- Consider showing a hint/placeholder text: "Deadline must be at least 10 minutes in the future"
- Consider disabling the submit button until a valid deadline is selected
- Consider showing a countdown or "earliest deadline" indicator
- Consider adding visual feedback (red border) on invalid date/time inputs
- May want to log validation failures for debugging purposes

---

## 🐛 Bug #2: Manual Task Failure Doesn't Deduct Points

### Description

When a parent manually fails a task for a child (using the "Fail" button in TaskReview.aspx), the task is marked as failed successfully, but:
1. **Points are not deducted** from the child's balance
2. **No point transaction record** is created in the points history
3. The child's balance remains unchanged even though the task shows as failed

**Example Scenario**:
- Child has 100 points
- Parent fails a task worth 100 points (should deduct 50 points = -50%)
- Expected: Child should have 50 points remaining
- Actual: Child still has 100 points
- Points history shows the task was failed but no deduction transaction appears

### Root Cause Analysis

**Location**: 
- `App_Code/TaskHelper.cs` - `ReviewTask` method (lines 1043-1201)
- `App_Code/PointHelper.cs` - `DeductPoints` method (lines 205-300)

**Current Implementation**:

**TaskHelper.cs** (lines 1119-1124):
```csharp
else
{
    // Deduct points (to treasury, cannot go negative)
    int pointsToDeduct = Math.Abs(pointsAwarded);
    pointsSuccess = PointHelper.DeductPoints(userId, pointsToDeduct, familyId, description, null);
}
```

**PointHelper.cs** (lines 205-300):
The `DeductPoints` method exists and should work, but there's a potential issue with the treasury deposit call.

**Problems**:
1. **Transaction Isolation Issue**: In `DeductPoints` method (line 275), `TreasuryHelper.DepositToTreasury` is called without passing the connection and transaction, which may cause issues if the treasury deposit fails or is outside the transaction scope
2. **Missing TaskAssignmentId**: The `DeductPoints` call passes `null` for the `orderId` parameter (line 1123), but it should pass `taskAssignmentId` to properly link the transaction
3. **Error Handling**: If `DeductPoints` returns `false`, the error is only logged but the review still succeeds (line 1126-1130), so the user doesn't know points weren't deducted

**Why it happens**:
- The `DeductPoints` method may be failing silently
- The treasury deposit call may be failing outside the transaction
- The `taskAssignmentId` is not passed to link the transaction properly
- Error handling doesn't prevent the review from completing even if points deduction fails

### Recommended Fix

**Step 1: Update ReviewTask to Pass taskAssignmentId**

In `TaskHelper.cs`, update the `DeductPoints` call (line 1123) to pass `taskAssignmentId`:

```csharp
else
{
    // Deduct points (to treasury, cannot go negative)
    int pointsToDeduct = Math.Abs(pointsAwarded);
    pointsSuccess = PointHelper.DeductPoints(userId, pointsToDeduct, familyId, description, taskAssignmentId);
}
```

**Step 2: Fix DeductPoints Treasury Deposit Transaction**

In `PointHelper.cs`, update the `DepositToTreasury` call (line 275) to pass connection and transaction:

```csharp
// Add to treasury (only actual deducted amount)
if (actualDeducted > 0)
{
    if (!TreasuryHelper.DepositToTreasury(familyId, actualDeducted, description ?? "Points deducted from child", orderId, null, conn, transaction))
    {
        System.Diagnostics.Debug.WriteLine(string.Format("DeductPoints ERROR: Failed to deposit to treasury"));
        transaction.Rollback();
        return false;
    }
    System.Diagnostics.Debug.WriteLine(string.Format("DeductPoints: Successfully deposited {0} to treasury", actualDeducted));
}
```

**Step 3: Enhance Error Handling in ReviewTask**

Update error handling in `ReviewTask` (lines 1126-1130) to prevent review completion if points deduction fails:

```csharp
if (!pointsSuccess)
{
    System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask ERROR: Failed to process points for user {0}. Review will not be completed.", userId));
    // Rollback the review if points processing failed
    // Note: This requires transaction handling - may need to refactor to use transactions
    return false; // Don't complete review if points failed
}
```

**Alternative Approach (Better): Use Transactions**

Refactor `ReviewTask` to use a database transaction that includes both the review insertion and points deduction:

```csharp
public static bool ReviewTask(int taskAssignmentId, int rating, int reviewedBy, bool isFailed, bool isAutoFailed = false)
{
    using (SqlConnection conn = DatabaseHelper.GetConnection())
    {
        conn.Open();
        using (SqlTransaction transaction = conn.BeginTransaction())
        {
            try
            {
                // ... existing code to get assignment info ...
                
                // Insert review within transaction
                // ... existing review insertion code ...
                
                if (rowsAffected > 0)
                {
                    // Award/deduct points within same transaction
                    if (pointsAwarded != 0)
                    {
                        string description = isFailed
                            ? string.Format("Task failed: {0} mokipoints deducted", Math.Abs(pointsAwarded))
                            : string.Format("Task completed: {0} mokipoints (Rating: {1} stars)", pointsAwarded, rating);
                        
                        bool pointsSuccess = false;
                        if (pointsAwarded > 0)
                        {
                            pointsSuccess = PointHelper.AwardPointsWithCap(userId, pointsAwarded, familyId, description, taskAssignmentId, conn, transaction);
                        }
                        else
                        {
                            int pointsToDeduct = Math.Abs(pointsAwarded);
                            pointsSuccess = PointHelper.DeductPoints(userId, pointsToDeduct, familyId, description, taskAssignmentId, conn, transaction);
                        }
                        
                        if (!pointsSuccess)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask ERROR: Failed to process points. Rolling back review."));
                            transaction.Rollback();
                            return false;
                        }
                    }
                    
                    // Soft-delete assignment
                    // ... existing soft-delete code ...
                    
                    // Create notification
                    // ... existing notification code ...
                    
                    transaction.Commit();
                    return true;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask ERROR: {0}", ex.Message));
                return false;
            }
        }
    }
}
```

**Note**: This requires updating `AwardPointsWithCap` and `DeductPoints` to accept optional `SqlConnection` and `SqlTransaction` parameters for transaction support.

### Files to Modify
- `mokipointsCS/App_Code/TaskHelper.cs` - Update `ReviewTask` method to pass `taskAssignmentId` and improve error handling
- `mokipointsCS/App_Code/PointHelper.cs` - Update `DeductPoints` to pass connection/transaction to treasury deposit
- `mokipointsCS/App_Code/TreasuryHelper.cs` - May need to verify `DepositToTreasury` supports connection/transaction parameters

### Testing Checklist
- [ ] Test manual task failure: Parent fails a task → verify points are deducted
- [ ] Test points deduction amount: 100 point task → should deduct 50 points (-50%)
- [ ] Test points history: Verify transaction appears in points history page
- [ ] Test balance update: Verify child's balance decreases correctly
- [ ] Test treasury deposit: Verify points are deposited to treasury
- [ ] Test with zero balance: Child with 0 points, fail 100 point task → should stay at 0 (can't go negative)
- [ ] Test with insufficient balance: Child with 20 points, fail 100 point task → should deduct only 20 points
- [ ] Test error handling: If treasury deposit fails, review should not complete
- [ ] Test transaction linking: Verify `taskAssignmentId` is linked in point transaction
- [ ] Test auto-fail: Verify auto-fail still works correctly (uses same code path)

### Expected Result
- When parent manually fails a task, points are immediately deducted from child's balance
- Point transaction record is created with negative points value
- Transaction appears in points history page with proper description
- Child's balance decreases by 50% of task points
- Treasury receives the deducted points
- If points deduction fails, the review does not complete (transaction rollback)

### Additional Considerations
- Consider adding a confirmation message showing how many points will be deducted before failing
- Consider showing the deduction in the success message: "Task failed. 50 points deducted from child."
- May want to add logging for failed point deductions to help debug issues
- Consider adding a retry mechanism if treasury deposit fails
- May want to show a warning if child's balance would go to zero after deduction

---

## 🐛 Bug #3: Overdue Tasks Can Still Be Accepted or Denied

### Description

When a task is overdue because the child did not accept or deny it (status is still "Assigned"), the child should not be able to accept or deny the task. Instead, the task should automatically fail and deduct points from the child.

**Current Behavior**:
- Child can still click "Accept" or "Deny" on overdue tasks
- Task may be accepted/denied even though deadline has passed
- Auto-fail only happens on page load, not when child tries to accept/deny

**Expected Behavior**:
- If task deadline has passed and status is "Assigned", child cannot accept or deny
- Task should automatically fail when child attempts to accept/deny an overdue task
- Points should be deducted immediately (-50% of task points)
- Child should see appropriate error message

**Example Scenario**:
- Task assigned on Day 1 with deadline = Day 1, 5:00 PM
- Day 1, 5:01 PM: Deadline has passed, task status is still "Assigned"
- Child tries to accept task → Should auto-fail and deduct points
- Child tries to deny task → Should auto-fail and deduct points

### Root Cause Analysis

**Location**: 
- `App_Code/TaskHelper.cs` - `AcceptTask` method (lines 838-879)
- `App_Code/TaskHelper.cs` - `DenyTask` method (lines 927-950)
- `App_Code/TaskHelper.cs` - `AutoFailOverdueTasks` method (lines 268-320)
- `mokipointsCS/ChildTasks.aspx.cs` - `rptTasks_ItemCommand` handler (lines 112-178)

**Current Implementation**:

**AcceptTask** (lines 838-879):
```csharp
public static bool AcceptTask(int taskAssignmentId, int userId)
{
    try
    {
        string query = @"
            UPDATE [dbo].[TaskAssignments]
            SET Status = 'Ongoing', AcceptedDate = GETDATE()
            WHERE Id = @TaskAssignmentId AND UserId = @UserId AND Status = 'Assigned'";
        // ... no deadline check ...
    }
}
```

**DenyTask** (lines 927-950):
```csharp
public static bool DenyTask(int taskAssignmentId, int userId)
{
    try
    {
        string query = @"
            UPDATE [dbo].[TaskAssignments]
            SET Status = 'Declined'
            WHERE Id = @TaskAssignmentId AND UserId = @UserId AND Status = 'Assigned'";
        // ... no deadline check ...
    }
}
```

**Problems**:
1. **No deadline validation**: Neither `AcceptTask` nor `DenyTask` checks if the task deadline has passed
2. **Auto-fail only on page load**: `AutoFailOverdueTasks` only runs when the page loads, not when child tries to accept/deny
3. **Race condition**: Child can accept/deny an overdue task if they click before page load auto-fail runs
4. **No user feedback**: Child doesn't know the task is overdue until after attempting to accept/deny
5. **Inconsistent behavior**: Task might be accepted/denied even though it should be auto-failed

**Why it happens**:
- The accept/deny methods only check status ("Assigned"), not deadline
- Auto-fail is reactive (runs on page load) rather than proactive (runs on action attempt)
- No validation prevents accepting/denying overdue tasks
- UI may still show Accept/Deny buttons for overdue tasks

### Recommended Fix

**Step 1: Add Deadline Check to AcceptTask**

Update `AcceptTask` method in `TaskHelper.cs` (lines 838-879):

```csharp
public static bool AcceptTask(int taskAssignmentId, int userId)
{
    try
    {
        // First, check if task is overdue and auto-fail if needed
        string checkQuery = @"
            SELECT ta.Deadline, ta.Status, ta.TaskId, t.PointsReward, t.FamilyId, f.OwnerId AS FamilyOwnerId
            FROM [dbo].[TaskAssignments] ta
            INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
            INNER JOIN [dbo].[Families] f ON t.FamilyId = f.Id
            WHERE ta.Id = @TaskAssignmentId AND ta.UserId = @UserId AND ta.IsDeleted = 0";
        
        using (DataTable dt = DatabaseHelper.ExecuteQuery(checkQuery,
            new SqlParameter("@TaskAssignmentId", taskAssignmentId),
            new SqlParameter("@UserId", userId)))
        {
            if (dt.Rows.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("AcceptTask: Assignment {0} not found for user {1}", taskAssignmentId, userId));
                return false;
            }
            
            string status = dt.Rows[0]["Status"].ToString();
            if (status != "Assigned")
            {
                System.Diagnostics.Debug.WriteLine(string.Format("AcceptTask: Assignment {0} is not in 'Assigned' status. Current: {1}", taskAssignmentId, status));
                return false;
            }
            
            // Check if task is overdue
            if (dt.Rows[0]["Deadline"] != DBNull.Value)
            {
                DateTime deadline = Convert.ToDateTime(dt.Rows[0]["Deadline"]);
                if (deadline < DateTime.Now)
                {
                    // Task is overdue - auto-fail it
                    System.Diagnostics.Debug.WriteLine(string.Format("AcceptTask: Assignment {0} is overdue (deadline: {1}). Auto-failing.", taskAssignmentId, deadline));
                    int familyOwnerId = Convert.ToInt32(dt.Rows[0]["FamilyOwnerId"]);
                    if (ReviewTask(taskAssignmentId, 0, familyOwnerId, true, true)) // isAutoFailed = true
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("AcceptTask: Successfully auto-failed overdue assignment {0}", taskAssignmentId));
                        return false; // Return false to indicate task was auto-failed, not accepted
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("AcceptTask ERROR: Failed to auto-fail overdue assignment {0}", taskAssignmentId));
                        return false;
                    }
                }
            }
        }
        
        // Task is not overdue - proceed with acceptance
        string query = @"
            UPDATE [dbo].[TaskAssignments]
            SET Status = 'Ongoing', AcceptedDate = GETDATE()
            WHERE Id = @TaskAssignmentId AND UserId = @UserId AND Status = 'Assigned'";
        
        int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
            new SqlParameter("@TaskAssignmentId", taskAssignmentId),
            new SqlParameter("@UserId", userId));
        
        if (rowsAffected > 0)
        {
            // Initialize objective completions
            InitializeObjectiveCompletions(taskAssignmentId);
            
            // Create notification for parent
            int taskId = GetTaskIdFromAssignment(taskAssignmentId);
            int? familyId = GetFamilyIdFromTask(taskId);
            if (familyId.HasValue)
            {
                DataTable parents = GetFamilyParents(familyId.Value);
                foreach (DataRow parent in parents.Rows)
                {
                    int parentId = Convert.ToInt32(parent["Id"]);
                    string childName = GetUserName(userId);
                    string taskTitle = GetTaskTitle(taskId);
                    CreateNotification(parentId, "Task Accepted", string.Format("{0} accepted task: {1}", childName, taskTitle), "TaskAccepted", taskId, taskAssignmentId);
                }
            }
        }
        
        return rowsAffected > 0;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine("AcceptTask error: " + ex.Message);
        return false;
    }
}
```

**Step 2: Add Deadline Check to DenyTask**

Update `DenyTask` method in `TaskHelper.cs` (lines 927-950):

```csharp
public static bool DenyTask(int taskAssignmentId, int userId)
{
    try
    {
        // First, check if task is overdue and auto-fail if needed
        string checkQuery = @"
            SELECT ta.Deadline, ta.Status, ta.TaskId, t.PointsReward, t.FamilyId, f.OwnerId AS FamilyOwnerId
            FROM [dbo].[TaskAssignments] ta
            INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
            INNER JOIN [dbo].[Families] f ON t.FamilyId = f.Id
            WHERE ta.Id = @TaskAssignmentId AND ta.UserId = @UserId AND ta.IsDeleted = 0";
        
        using (DataTable dt = DatabaseHelper.ExecuteQuery(checkQuery,
            new SqlParameter("@TaskAssignmentId", taskAssignmentId),
            new SqlParameter("@UserId", userId)))
        {
            if (dt.Rows.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("DenyTask: Assignment {0} not found for user {1}", taskAssignmentId, userId));
                return false;
            }
            
            string status = dt.Rows[0]["Status"].ToString();
            if (status != "Assigned")
            {
                System.Diagnostics.Debug.WriteLine(string.Format("DenyTask: Assignment {0} is not in 'Assigned' status. Current: {1}", taskAssignmentId, status));
                return false;
            }
            
            // Check if task is overdue
            if (dt.Rows[0]["Deadline"] != DBNull.Value)
            {
                DateTime deadline = Convert.ToDateTime(dt.Rows[0]["Deadline"]);
                if (deadline < DateTime.Now)
                {
                    // Task is overdue - auto-fail it
                    System.Diagnostics.Debug.WriteLine(string.Format("DenyTask: Assignment {0} is overdue (deadline: {1}). Auto-failing.", taskAssignmentId, deadline));
                    int familyOwnerId = Convert.ToInt32(dt.Rows[0]["FamilyOwnerId"]);
                    if (ReviewTask(taskAssignmentId, 0, familyOwnerId, true, true)) // isAutoFailed = true
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("DenyTask: Successfully auto-failed overdue assignment {0}", taskAssignmentId));
                        return false; // Return false to indicate task was auto-failed, not denied
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("DenyTask ERROR: Failed to auto-fail overdue assignment {0}", taskAssignmentId));
                        return false;
                    }
                }
            }
        }
        
        // Task is not overdue - proceed with denial
        string query = @"
            UPDATE [dbo].[TaskAssignments]
            SET Status = 'Declined'
            WHERE Id = @TaskAssignmentId AND UserId = @UserId AND Status = 'Assigned'";
        
        int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
            new SqlParameter("@TaskAssignmentId", taskAssignmentId),
            new SqlParameter("@UserId", userId));
        
        return rowsAffected > 0;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine("DenyTask error: " + ex.Message);
        return false;
    }
}
```

**Step 3: Update ChildTasks.aspx.cs to Show Appropriate Error Message**

Update `rptTasks_ItemCommand` in `ChildTasks.aspx.cs` (lines 112-178) to handle auto-fail scenario:

```csharp
case "Accept":
    // Check if task was auto-failed (by checking if it still exists and is in "Assigned" status)
    string currentStatus = TaskHelper.GetTaskAssignmentStatus(assignmentId);
    if (string.IsNullOrEmpty(currentStatus))
    {
        // Task was auto-failed (no longer exists or was reviewed)
        ShowError("This task is overdue and has been automatically failed. Points have been deducted.");
        LoadTasks(); // Reload to show updated task list
    }
    else if (TaskHelper.AcceptTask(assignmentId, userId))
    {
        ShowSuccess("Task accepted!");
        LoadTasks();
    }
    else
    {
        ShowError("Failed to accept task. The task may be overdue and has been automatically failed.");
        LoadTasks(); // Reload to check if task was auto-failed
    }
    break;

case "Decline":
    // Check if task was auto-failed
    currentStatus = TaskHelper.GetTaskAssignmentStatus(assignmentId);
    if (string.IsNullOrEmpty(currentStatus))
    {
        // Task was auto-failed
        ShowError("This task is overdue and has been automatically failed. Points have been deducted.");
        LoadTasks();
    }
    else if (TaskHelper.DenyTask(assignmentId, userId))
    {
        ShowSuccess("Task declined.");
        LoadTasks();
    }
    else
    {
        ShowError("Failed to decline task. The task may be overdue and has been automatically failed.");
        LoadTasks();
    }
    break;
```

**Step 4: Hide Accept/Deny Buttons for Overdue Tasks (UI Enhancement)**

Update `rptTasks_ItemDataBound` in `ChildTasks.aspx.cs` (lines 180-310) to hide Accept/Deny buttons for overdue tasks:

```csharp
// Check if task is overdue and hide Accept/Deny buttons
if (row["Deadline"] != DBNull.Value)
{
    DateTime deadline = Convert.ToDateTime(row["Deadline"]);
    DateTime now = DateTime.Now;
    
    if (deadline < now && status == "Assigned")
    {
        // Task is overdue - hide Accept/Deny buttons
        Button btnAccept = (Button)e.Item.FindControl("btnAccept");
        Button btnDecline = (Button)e.Item.FindControl("btnDecline");
        
        if (btnAccept != null)
        {
            btnAccept.Visible = false;
        }
        if (btnDecline != null)
        {
            btnDecline.Visible = false;
        }
        
        // Show message that task will be auto-failed
        Literal litOverdueMessage = (Literal)e.Item.FindControl("litOverdueMessage");
        if (litOverdueMessage != null)
        {
            litOverdueMessage.Text = "<div class='overdue-message' style='color: #d32f2f; font-weight: bold; padding: 10px; background-color: #ffebee; border-radius: 5px; margin: 10px 0;'>⚠️ This task is overdue and will be automatically failed. Please refresh the page.</div>";
            litOverdueMessage.Visible = true;
        }
    }
}
```

### Files to Modify
- `mokipointsCS/App_Code/TaskHelper.cs` - Update `AcceptTask` and `DenyTask` methods to check deadline and auto-fail if overdue
- `mokipointsCS/ChildTasks.aspx.cs` - Update `rptTasks_ItemCommand` to handle auto-fail messages and `rptTasks_ItemDataBound` to hide buttons for overdue tasks
- `mokipointsCS/ChildTasks.aspx` - May need to add `litOverdueMessage` Literal control to the Repeater template

### Testing Checklist
- [ ] Test accept overdue task: Child tries to accept task after deadline → should auto-fail and deduct points
- [ ] Test deny overdue task: Child tries to deny task after deadline → should auto-fail and deduct points
- [ ] Test accept non-overdue task: Child accepts task before deadline → should work normally
- [ ] Test deny non-overdue task: Child denies task before deadline → should work normally
- [ ] Test points deduction: Verify -50% points are deducted when overdue task is auto-failed
- [ ] Test UI: Verify Accept/Deny buttons are hidden for overdue "Assigned" tasks
- [ ] Test error messages: Verify appropriate error message shows when child tries to accept/deny overdue task
- [ ] Test page load auto-fail: Verify auto-fail still works on page load
- [ ] Test race condition: Verify no race condition between page load auto-fail and accept/deny action
- [ ] Test task without deadline: Verify tasks without deadlines can still be accepted/denied normally
- [ ] Test multiple overdue tasks: Verify all overdue tasks are handled correctly

### Expected Result
- Children cannot accept or deny overdue tasks
- Overdue tasks are automatically failed when child attempts to accept/deny
- Points are deducted immediately (-50% of task points)
- Appropriate error messages inform child that task was auto-failed
- Accept/Deny buttons are hidden for overdue tasks in the UI
- Page load auto-fail still works as before
- Non-overdue tasks can still be accepted/denied normally

### Additional Considerations
- Consider adding a countdown timer showing time until deadline
- Consider showing a warning when deadline is approaching (e.g., "Deadline in 1 hour")
- May want to add a "Refresh" button to manually trigger auto-fail check
- Consider adding visual indicator (red border, warning icon) for overdue tasks
- May want to log when tasks are auto-failed via accept/deny attempt for analytics

---

## 🐛 Bug #4: Overdue Ongoing Tasks Can Still Be Submitted

### Description

When a task is accepted by the child (status = "Ongoing") and then becomes overdue, the child can still submit it for review. Instead, when an "Ongoing" task becomes overdue, it should:
1. **Automatically fail** the task
2. **Deduct points** from the child (-50% of task points)
3. **Move to history** as failed (via review record)

**Current Behavior**:
- Child can submit overdue "Ongoing" tasks
- Task may be submitted even though deadline has passed
- Auto-fail only happens on page load, not when child tries to submit

**Expected Behavior**:
- If task deadline has passed and status is "Ongoing", child cannot submit
- Task should automatically fail when child attempts to submit an overdue task
- Points should be deducted immediately (-50% of task points)
- Task should be moved to history as failed
- Child should see appropriate error message

**Example Scenario**:
- Task assigned on Day 1 with deadline = Day 1, 5:00 PM
- Day 1, 4:00 PM: Child accepts task (status = "Ongoing")
- Day 1, 5:01 PM: Deadline has passed, task status is still "Ongoing"
- Child tries to submit task → Should auto-fail, deduct points, and move to history as failed

### Root Cause Analysis

**Location**: 
- `App_Code/TaskHelper.cs` - `SubmitTaskForReview` method (lines 970-1015)
- `App_Code/TaskHelper.cs` - `AutoFailOverdueTasks` method (lines 268-320)
- `mokipointsCS/ChildTasks.aspx.cs` - `rptTasks_ItemCommand` handler (lines 151-169)

**Current Implementation**:

**SubmitTaskForReview** (lines 970-1015):
```csharp
public static bool SubmitTaskForReview(int taskAssignmentId, int userId)
{
    try
    {
        // Fix #1: Server-side validation - check all objectives are completed
        if (!AreAllObjectivesCompleted(taskAssignmentId))
        {
            return false;
        }
        
        string query = @"
            UPDATE [dbo].[TaskAssignments]
            SET Status = 'Pending Review', CompletedDate = GETDATE()
            WHERE Id = @TaskAssignmentId AND UserId = @UserId AND Status = 'Ongoing'";
        // ... no deadline check ...
    }
}
```

**AutoFailOverdueTasks** (lines 275-285):
```csharp
string query = @"
    SELECT ta.Id AS AssignmentId, ta.TaskId, ta.UserId, ta.Deadline, ta.Status,
           t.PointsReward, t.FamilyId, f.OwnerId AS FamilyOwnerId
    FROM [dbo].[TaskAssignments] ta
    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
    INNER JOIN [dbo].[Families] f ON t.FamilyId = f.Id
    WHERE ta.Deadline IS NOT NULL
      AND ta.Deadline < @Now
      AND ta.Status IN ('Assigned', 'Ongoing')  -- Includes 'Ongoing' status
      AND ta.IsDeleted = 0
      AND NOT EXISTS (SELECT 1 FROM [dbo].[TaskReviews] tr WHERE tr.TaskAssignmentId = ta.Id)";
```

**Problems**:
1. **No deadline validation**: `SubmitTaskForReview` doesn't check if the task deadline has passed
2. **Auto-fail only on page load**: `AutoFailOverdueTasks` only runs when the page loads, not when child tries to submit
3. **Race condition**: Child can submit an overdue task if they click before page load auto-fail runs
4. **No user feedback**: Child doesn't know the task is overdue until after attempting to submit
5. **Inconsistent behavior**: Task might be submitted even though it should be auto-failed

**Why it happens**:
- The `SubmitTaskForReview` method only checks objectives completion and status ("Ongoing"), not deadline
- Auto-fail is reactive (runs on page load) rather than proactive (runs on action attempt)
- No validation prevents submitting overdue "Ongoing" tasks
- UI may still show Submit button for overdue tasks

### Recommended Fix

**Step 1: Add Deadline Check to SubmitTaskForReview**

Update `SubmitTaskForReview` method in `TaskHelper.cs` (lines 970-1015):

```csharp
public static bool SubmitTaskForReview(int taskAssignmentId, int userId)
{
    try
    {
        // Fix #1: Server-side validation - check all objectives are completed
        if (!AreAllObjectivesCompleted(taskAssignmentId))
        {
            System.Diagnostics.Debug.WriteLine("SubmitTaskForReview: Not all objectives completed");
            return false;
        }
        
        // Check if task is overdue and auto-fail if needed
        string checkQuery = @"
            SELECT ta.Deadline, ta.Status, ta.TaskId, t.PointsReward, t.FamilyId, f.OwnerId AS FamilyOwnerId
            FROM [dbo].[TaskAssignments] ta
            INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
            INNER JOIN [dbo].[Families] f ON t.FamilyId = f.Id
            WHERE ta.Id = @TaskAssignmentId AND ta.UserId = @UserId AND ta.IsDeleted = 0";
        
        using (DataTable dt = DatabaseHelper.ExecuteQuery(checkQuery,
            new SqlParameter("@TaskAssignmentId", taskAssignmentId),
            new SqlParameter("@UserId", userId)))
        {
            if (dt.Rows.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("SubmitTaskForReview: Assignment {0} not found for user {1}", taskAssignmentId, userId));
                return false;
            }
            
            string status = dt.Rows[0]["Status"].ToString();
            if (status != "Ongoing")
            {
                System.Diagnostics.Debug.WriteLine(string.Format("SubmitTaskForReview: Assignment {0} is not in 'Ongoing' status. Current: {1}", taskAssignmentId, status));
                return false;
            }
            
            // Check if task is overdue
            if (dt.Rows[0]["Deadline"] != DBNull.Value)
            {
                DateTime deadline = Convert.ToDateTime(dt.Rows[0]["Deadline"]);
                if (deadline < DateTime.Now)
                {
                    // Task is overdue - auto-fail it
                    System.Diagnostics.Debug.WriteLine(string.Format("SubmitTaskForReview: Assignment {0} is overdue (deadline: {1}). Auto-failing.", taskAssignmentId, deadline));
                    int familyOwnerId = Convert.ToInt32(dt.Rows[0]["FamilyOwnerId"]);
                    if (ReviewTask(taskAssignmentId, 0, familyOwnerId, true, true)) // isAutoFailed = true
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("SubmitTaskForReview: Successfully auto-failed overdue assignment {0}", taskAssignmentId));
                        return false; // Return false to indicate task was auto-failed, not submitted
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("SubmitTaskForReview ERROR: Failed to auto-fail overdue assignment {0}", taskAssignmentId));
                        return false;
                    }
                }
            }
        }
        
        // Task is not overdue - proceed with submission
        string query = @"
            UPDATE [dbo].[TaskAssignments]
            SET Status = 'Pending Review', CompletedDate = GETDATE()
            WHERE Id = @TaskAssignmentId AND UserId = @UserId AND Status = 'Ongoing'";
        
        int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
            new SqlParameter("@TaskAssignmentId", taskAssignmentId),
            new SqlParameter("@UserId", userId));
        
        if (rowsAffected > 0)
        {
            // Fix #4: Create notification for parent
            int taskId = GetTaskIdFromAssignment(taskAssignmentId);
            int? familyId = GetFamilyIdFromTask(taskId);
            if (familyId.HasValue)
            {
                DataTable parents = GetFamilyParents(familyId.Value);
                foreach (DataRow parent in parents.Rows)
                {
                    int parentId = Convert.ToInt32(parent["Id"]);
                    string childName = GetUserName(userId);
                    string taskTitle = GetTaskTitle(taskId);
                    CreateNotification(parentId, "Task Submitted for Review", string.Format("{0} submitted task for review: {1}", childName, taskTitle), "TaskSubmitted", taskId, taskAssignmentId);
                }
            }
        }
        
        return rowsAffected > 0;
    }
    catch (Exception ex)
    {
        System.Diagnostics.Debug.WriteLine("SubmitTaskForReview error: " + ex.Message);
        return false;
    }
}
```

**Step 2: Update ChildTasks.aspx.cs to Show Appropriate Error Message**

Update `rptTasks_ItemCommand` in `ChildTasks.aspx.cs` (lines 151-169) to handle auto-fail scenario:

```csharp
case "Submit":
    // Fix #1: Server-side validation - check all objectives completed
    if (TaskHelper.AreAllObjectivesCompleted(assignmentId))
    {
        // Check if task was auto-failed (by checking if it still exists and is in "Ongoing" status)
        string currentStatus = TaskHelper.GetTaskAssignmentStatus(assignmentId);
        if (string.IsNullOrEmpty(currentStatus))
        {
            // Task was auto-failed (no longer exists or was reviewed)
            ShowError("This task is overdue and has been automatically failed. Points have been deducted and the task has been moved to history.");
            LoadTasks(); // Reload to show updated task list
        }
        else if (TaskHelper.SubmitTaskForReview(assignmentId, userId))
        {
            ShowSuccess("Task submitted for review!");
            LoadTasks();
        }
        else
        {
            ShowError("Failed to submit task. The task may be overdue and has been automatically failed. Points have been deducted.");
            LoadTasks(); // Reload to check if task was auto-failed
        }
    }
    else
    {
        ShowError("Please complete all objectives before submitting.");
    }
    break;
```

**Step 3: Hide Submit Button for Overdue Tasks (UI Enhancement)**

Update `rptTasks_ItemDataBound` in `ChildTasks.aspx.cs` (lines 180-310) to hide Submit button for overdue "Ongoing" tasks:

```csharp
// Check if task is overdue and hide Submit button
if (row["Deadline"] != DBNull.Value && status == "Ongoing")
{
    DateTime deadline = Convert.ToDateTime(row["Deadline"]);
    DateTime now = DateTime.Now;
    
    if (deadline < now)
    {
        // Task is overdue - hide Submit button
        Button btnSubmit = (Button)e.Item.FindControl("btnSubmit");
        if (btnSubmit != null)
        {
            btnSubmit.Visible = false;
        }
        
        // Show message that task will be auto-failed
        Literal litOverdueMessage = (Literal)e.Item.FindControl("litOverdueMessage");
        if (litOverdueMessage != null)
        {
            litOverdueMessage.Text = "<div class='overdue-message' style='color: #d32f2f; font-weight: bold; padding: 10px; background-color: #ffebee; border-radius: 5px; margin: 10px 0;'>⚠️ This task is overdue and will be automatically failed. Points will be deducted. Please refresh the page.</div>";
            litOverdueMessage.Visible = true;
        }
    }
}
```

### Files to Modify
- `mokipointsCS/App_Code/TaskHelper.cs` - Update `SubmitTaskForReview` method to check deadline and auto-fail if overdue
- `mokipointsCS/ChildTasks.aspx.cs` - Update `rptTasks_ItemCommand` to handle auto-fail messages and `rptTasks_ItemDataBound` to hide Submit button for overdue tasks
- `mokipointsCS/ChildTasks.aspx` - May need to add `litOverdueMessage` Literal control to the Repeater template (if not already added for Bug #3)

### Testing Checklist
- [ ] Test submit overdue task: Child tries to submit "Ongoing" task after deadline → should auto-fail and deduct points
- [ ] Test submit non-overdue task: Child submits task before deadline → should work normally
- [ ] Test points deduction: Verify -50% points are deducted when overdue task is auto-failed
- [ ] Test task history: Verify failed task appears in task history
- [ ] Test UI: Verify Submit button is hidden for overdue "Ongoing" tasks
- [ ] Test error messages: Verify appropriate error message shows when child tries to submit overdue task
- [ ] Test page load auto-fail: Verify auto-fail still works on page load for "Ongoing" tasks
- [ ] Test race condition: Verify no race condition between page load auto-fail and submit action
- [ ] Test task without deadline: Verify tasks without deadlines can still be submitted normally
- [ ] Test objectives completion: Verify objectives completion check still works before deadline check
- [ ] Test multiple overdue tasks: Verify all overdue "Ongoing" tasks are handled correctly

### Expected Result
- Children cannot submit overdue "Ongoing" tasks
- Overdue "Ongoing" tasks are automatically failed when child attempts to submit
- Points are deducted immediately (-50% of task points)
- Task is moved to history as failed (via review record)
- Appropriate error messages inform child that task was auto-failed
- Submit button is hidden for overdue "Ongoing" tasks in the UI
- Page load auto-fail still works as before
- Non-overdue "Ongoing" tasks can still be submitted normally

### Additional Considerations
- Consider adding a countdown timer showing time until deadline for "Ongoing" tasks
- Consider showing a warning when deadline is approaching (e.g., "Deadline in 1 hour")
- May want to add a "Refresh" button to manually trigger auto-fail check
- Consider adding visual indicator (red border, warning icon) for overdue "Ongoing" tasks
- May want to log when tasks are auto-failed via submit attempt for analytics
- Consider showing progress indicator: "Task will be auto-failed in X minutes" when deadline is approaching

---

## 🔄 Implementation Status

| Bug # | Description | Status | Assigned | Date Fixed |
|-------|-------------|--------|----------|------------|
| #1 | Parent deadline logic - no past dates and minimum 10 minutes ahead | ✅ Fixed | - | 2024-11-26 |
| #2 | Manual task failure doesn't deduct points from child | ✅ Fixed | - | 2024-11-26 |
| #3 | Overdue tasks can still be accepted or denied | ✅ Fixed | - | 2024-11-26 |
| #4 | Overdue ongoing tasks can still be submitted | ✅ Fixed | - | 2024-11-26 |

**Legend**:
- ⏳ Pending - Not yet fixed
- 🔄 In Progress - Currently being worked on
- ✅ Fixed - Completed and tested
- ❌ Blocked - Cannot proceed due to dependencies

---

## 📚 Related Documentation

- [System Scan Summary](./SYSTEM_SCAN_SUMMARY.md)
- [Patch 4.0.5 Documentation](./PATCH_4.0.5_SYSTEM_ADJUSTMENTS_AND_BUG_FIXING.md)
- [Task System Documentation](./TaskSystem/TASK_SYSTEM_SCHEMATIC.md)

---

**Last Updated**: November 26, 2024  
**Next Review**: After testing and user feedback

---

## 📝 Implementation Summary

All bugs in Patch 5.0.1 have been successfully implemented:

1. ✅ **Bug #1**: Parent deadline logic - Added client-side and server-side validation to enforce minimum 10 minutes in the future
2. ✅ **Bug #2**: Manual task failure - Fixed points deduction by passing taskAssignmentId and fixing treasury deposit transaction
3. ✅ **Bug #3**: Overdue tasks accept/deny - Added deadline checks to AcceptTask and DenyTask methods to auto-fail overdue tasks
4. ✅ **Bug #4**: Overdue ongoing tasks submit - Added deadline check to SubmitTaskForReview method to auto-fail overdue tasks

**Total Issues Fixed**: 4  
**Implementation Date**: November 26, 2024  
**Status**: All fixes completed and ready for testing

