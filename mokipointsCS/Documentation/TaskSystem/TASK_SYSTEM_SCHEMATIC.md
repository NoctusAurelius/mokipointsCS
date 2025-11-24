# Task System - Detailed Schematic Documentation

**Document Purpose**: Comprehensive technical documentation of the current task system implementation for planning system rework.

**Last Updated**: November 22, 2025

**Status**: Current Production Implementation

---

## Table of Contents

1. [System Overview](#system-overview)
2. [Database Schema](#database-schema)
3. [Complete Process Flow](#complete-process-flow)
4. [Status State Machine](#status-state-machine)
5. [User Roles & Actions](#user-roles--actions)
6. [Business Rules](#business-rules)
7. [Points Calculation System](#points-calculation-system)
8. [Validation Rules](#validation-rules)
9. [Edge Cases & Special Scenarios](#edge-cases--special-scenarios)
10. [Current Implementation Details](#current-implementation-details)
11. [API/Method Reference](#apimethod-reference)
12. [Known Issues & Limitations](#known-issues--limitations)

---

## System Overview

The Mokipoints Task System is a parent-child task management system where:
- **Parents** create tasks with objectives and assign them to children
- **Children** accept, work on, and submit tasks for review
- **Parents** review completed tasks and award points based on performance
- Tasks can be assigned to multiple children simultaneously
- Each child's assignment has independent status tracking

### Key Features
- ✅ Task creation with multiple objectives
- ✅ Task editing (only when not assigned)
- ✅ Task assignment to children (with optional deadlines)
- ✅ Child acceptance/decline workflow
- ✅ Objective-based task completion
- ✅ Parent review system with star ratings (1-5)
- ✅ Points calculation based on rating
- ✅ Auto-fail overdue tasks
- ✅ Per-child assignment status tracking
- ✅ Task reassignment after review

---

## Database Schema

### Core Tables

#### 1. Tasks Table
```sql
CREATE TABLE [dbo].[Tasks] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Category] NVARCHAR(100) NOT NULL,
    [PointsReward] INT NOT NULL DEFAULT 0,
    [CreatedBy] INT NOT NULL,                    -- FK to Users.Id (Parent)
    [FamilyId] INT NOT NULL,                      -- FK to Families.Id
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1
)
```

**Key Relationships**:
- `CreatedBy` → `Users.Id` (Parent who created the task)
- `FamilyId` → `Families.Id` (Family the task belongs to)

**Indexes**:
- `IX_Tasks_CreatedBy` on `[CreatedBy]`
- `IX_Tasks_FamilyId` on `[FamilyId]`

---

#### 2. TaskObjectives Table
```sql
CREATE TABLE [dbo].[TaskObjectives] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TaskId] INT NOT NULL,                       -- FK to Tasks.Id
    [ObjectiveText] NVARCHAR(500) NOT NULL,
    [OrderIndex] INT NOT NULL DEFAULT 0
)
```

**Key Relationships**:
- `TaskId` → `Tasks.Id` ON DELETE CASCADE (objectives deleted when task deleted)

**Indexes**:
- `IX_TaskObjectives_TaskId` on `[TaskId]`

**Notes**:
- Objectives are ordered by `OrderIndex`
- All objectives must be completed before task can be submitted for review

---

#### 3. TaskAssignments Table
```sql
CREATE TABLE [dbo].[TaskAssignments] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TaskId] INT NOT NULL,                       -- FK to Tasks.Id
    [UserId] INT NOT NULL,                       -- FK to Users.Id (Child)
    [AssignedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [Deadline] DATETIME NULL,                    -- Optional deadline
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Assigned',
    [AcceptedDate] DATETIME NULL,                -- When child accepted
    [CompletedDate] DATETIME NULL                 -- When child submitted for review
)
```

**Key Relationships**:
- `TaskId` → `Tasks.Id`
- `UserId` → `Users.Id` (Child user)

**Indexes**:
- `IX_TaskAssignments_TaskId` on `[TaskId]`
- `IX_TaskAssignments_UserId` on `[UserId]`
- `IX_TaskAssignments_Status` on `[Status]`

**Status Values**:
- `'Assigned'` - Task assigned, waiting for child to accept/decline
- `'Ongoing'` - Child accepted, working on task
- `'Pending Review'` - Child submitted, waiting for parent review
- `'Declined'` - Child declined the task (can be reassigned)
- `'Reviewed'` - **NOT USED** - Assignment is deleted instead

**Important**: After parent review, the assignment record is **DELETED** (not marked as 'Reviewed'), making the task available for reassignment.

---

#### 4. TaskReviews Table
```sql
CREATE TABLE [dbo].[TaskReviews] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TaskAssignmentId] INT NOT NULL,            -- FK to TaskAssignments.Id
    [Rating] INT NULL,                           -- 1-5 stars (NULL if failed)
    [PointsAwarded] INT NOT NULL,                -- Calculated points (can be negative)
    [IsFailed] BIT NOT NULL DEFAULT 0,           -- True if task marked as failed
    [ReviewDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ReviewedBy] INT NOT NULL                    -- FK to Users.Id (Parent)
)
```

**Key Relationships**:
- `TaskAssignmentId` → `TaskAssignments.Id`
- `ReviewedBy` → `Users.Id` (Parent who reviewed)

**Indexes**:
- `IX_TaskReviews_TaskAssignmentId` on `[TaskAssignmentId]`

**Notes**:
- `Rating` is NULL when `IsFailed = 1`
- `PointsAwarded` can be negative (for failed tasks)
- Review is created BEFORE assignment is deleted

---

#### 5. PointTransactions Table
```sql
CREATE TABLE [dbo].[PointTransactions] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,                       -- FK to Users.Id (Child)
    [Points] INT NOT NULL,                        -- Always positive (absolute value)
    [TransactionType] NVARCHAR(50) NOT NULL,      -- 'Earned' or 'Spent'
    [Description] NVARCHAR(500) NULL,
    [TransactionDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [TaskAssignmentId] INT NULL                   -- FK to TaskAssignments.Id
)
```

**Key Relationships**:
- `UserId` → `Users.Id` (Child receiving/losing points)
- `TaskAssignmentId` → `TaskAssignments.Id` (Links to task assignment)

**Notes**:
- `Points` is always stored as positive value
- `TransactionType` determines if points are added ('Earned') or subtracted ('Spent')
- Description includes rating and points awarded/deducted

---

### Entity Relationship Diagram

```
┌─────────────┐
│   Families  │
└──────┬──────┘
       │
       │ 1:N
       │
┌──────▼──────┐      ┌──────────────┐
│    Tasks    │      │    Users     │
│             │      │  (Parents)   │
└──────┬──────┘      └──────┬───────┘
       │                    │
       │ 1:N                │ 1:N
       │                    │
┌──────▼──────────────┐    │
│  TaskObjectives     │    │
└─────────────────────┘    │
                          │
┌─────────────────────────▼──────┐
│      TaskAssignments           │
│  (Links Tasks to Child Users)  │
└──────┬─────────────────────────┘
       │
       │ 1:1
       │
┌──────▼──────────┐    ┌──────────────┐
│  TaskReviews     │    │    Users     │
│                  │    │  (Children)  │
└──────────────────┘    └──────┬───────┘
                               │
                               │ 1:N
                               │
                    ┌──────────▼──────────┐
                    │ PointTransactions   │
                    │  (Points History)    │
                    └─────────────────────┘
```

---

## Complete Process Flow

### High-Level Flow Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                    TASK SYSTEM LIFECYCLE                         │
└─────────────────────────────────────────────────────────────────┘

1. TASK CREATION (Parent)
   ┌─────────────────┐
   │ Parent creates   │
   │ task with        │
   │ objectives       │
   └────────┬────────┘
            │
            ▼
   ┌─────────────────┐
   │ Task stored in  │
   │ Tasks table     │
   │ Status: None    │
   └────────┬────────┘
            │
            ▼
2. TASK ASSIGNMENT (Parent)
   ┌─────────────────┐
   │ Parent assigns   │
   │ to child via     │
   │ AssignTask.aspx  │
   └────────┬────────┘
            │
            ▼
   ┌─────────────────┐
   │ TaskAssignment  │
   │ created         │
   │ Status:         │
   │ "Assigned"      │
   └────────┬────────┘
            │
            ▼
3. CHILD ACCEPTANCE (Child)
   ┌─────────────────┐
   │ Child views on   │
   │ ChildTasks.aspx  │
   └────────┬────────┘
            │
            ├─── Accept ───► Status: "Ongoing"
            │
            └─── Decline ───► Status: "Declined"
                              (Task can be reassigned)
            │
            ▼
4. TASK COMPLETION (Child)
   ┌─────────────────┐
   │ Child marks all  │
   │ objectives      │
   │ complete        │
   └────────┬────────┘
            │
            ▼
   ┌─────────────────┐
   │ Child submits    │
   │ for review       │
   └────────┬────────┘
            │
            ▼
   ┌─────────────────┐
   │ Status:         │
   │ "Pending Review"│
   └────────┬────────┘
            │
            ▼
5. PARENT REVIEW (Parent)
   ┌─────────────────┐
   │ Parent reviews   │
   │ on TaskReview.   │
   │ aspx            │
   └────────┬────────┘
            │
            ├─── Rate 1-5 stars ───► Points calculated
            │
            └─── Mark as Failed ───► -50% points deducted
            │
            ▼
   ┌─────────────────┐
   │ TaskReview       │
   │ record created   │
   │ Points awarded/  │
   │ deducted         │
   └────────┬────────┘
            │
            ▼
   ┌─────────────────┐
   │ Assignment      │
   │ DELETED         │
   │ (Not marked as  │
   │  "Reviewed")    │
   └────────┬────────┘
            │
            ▼
   ┌─────────────────┐
   │ Task available  │
   │ for reassignment│
   └─────────────────┘
```

---

### Detailed Step-by-Step Flow

#### Phase 1: Task Creation

**Actor**: Parent

**Steps**:
1. Parent navigates to `Tasks.aspx`
2. Parent clicks "Create Task" button
3. Parent fills in:
   - Title (required)
   - Description (optional)
   - Category (required, dropdown)
   - Points Reward (required, integer)
   - Objectives (one or more, required)
4. Parent clicks "Create Task"
5. System validates input
6. `TaskHelper.CreateTask()` is called
7. Task record inserted into `Tasks` table
8. Objectives inserted into `TaskObjectives` table with `OrderIndex`
9. Success message displayed
10. Task appears in task list

**Database Operations**:
```sql
-- Insert task
INSERT INTO Tasks (Title, Description, Category, PointsReward, CreatedBy, FamilyId)
VALUES (@Title, @Description, @Category, @PointsReward, @CreatedBy, @FamilyId)

-- Insert objectives (for each)
INSERT INTO TaskObjectives (TaskId, ObjectiveText, OrderIndex)
VALUES (@TaskId, @ObjectiveText, @OrderIndex)
```

**Validation**:
- Title: Required, max 200 characters
- Category: Required, must be from predefined list
- Points: Required, must be positive integer
- Objectives: At least one required, max 500 characters each

---

#### Phase 2: Task Assignment

**Actor**: Parent

**Steps**:
1. Parent views task list on `Tasks.aspx`
2. Parent clicks "Assign" button on a task card
3. System redirects to `AssignTask.aspx?taskId={id}`
4. System loads task details (read-only display)
5. System populates child dropdown (excludes banned children)
6. Parent selects child (required)
7. Parent optionally sets deadline (date + time)
8. Parent clicks "Assign Task"
9. System validates:
   - Child selected
   - Child not banned
   - No duplicate assignment (unless previous was "Declined")
   - Child is in same family
10. `TaskHelper.AssignTask()` is called
11. TaskAssignment record inserted with Status = 'Assigned'
12. Success message displayed
13. Redirect to `Tasks.aspx?assigned=true`
14. Success message shown on Tasks page

**Database Operations**:
```sql
-- Check if child is banned
SELECT IsBanned FROM Users WHERE Id = @UserId AND Role = 'CHILD'

-- Check for existing assignment
SELECT Status FROM TaskAssignments
WHERE TaskId = @TaskId AND UserId = @UserId AND Status != 'Declined'

-- Insert assignment
INSERT INTO TaskAssignments (TaskId, UserId, Deadline, Status)
VALUES (@TaskId, @UserId, @Deadline, 'Assigned')
```

**Validation Rules**:
- Child must be selected
- Child must not be banned (`IsBanned = 0`)
- Child must be in same family as task
- Cannot assign if task already assigned to same child (unless previous status was "Declined")
- Deadline (if provided) must be in the future

**Error Messages**:
- "Please select a child to assign the task to."
- "This child is currently banned from receiving tasks."
- "This task is already assigned to this child. [Status: Assigned/Ongoing/Completed/Reviewed]"
- "Invalid date format. Please use MM/DD/YYYY format."

---

#### Phase 3: Child Acceptance/Decline

**Actor**: Child

**Steps**:
1. Child navigates to `ChildTasks.aspx`
2. System calls `TaskHelper.AutoFailOverdueTasks()` to auto-fail overdue tasks
3. System loads tasks via `TaskHelper.GetChildTasks(userId)`
4. Child sees tasks with Status = "Assigned"
5. Child can:
   - **Accept**: Click "Accept" button
   - **Decline**: Click "Decline" button
6. If Accept:
   - `TaskHelper.AcceptTask()` is called
   - Status updated to "Ongoing"
   - `AcceptedDate` set to current date/time
7. If Decline:
   - `TaskHelper.DenyTask()` is called
   - Status updated to "Declined"
8. Task list refreshed
9. Accepted tasks show objectives and "Submit" button
10. Declined tasks disappear from list (can be reassigned by parent)

**Database Operations**:

**Accept**:
```sql
UPDATE TaskAssignments
SET Status = 'Ongoing', AcceptedDate = GETDATE()
WHERE Id = @TaskAssignmentId AND UserId = @UserId AND Status = 'Assigned'
```

**Decline**:
```sql
UPDATE TaskAssignments
SET Status = 'Declined'
WHERE Id = @TaskAssignmentId AND UserId = @UserId AND Status = 'Assigned'
```

**Validation**:
- Assignment must exist
- Assignment must belong to current user
- Status must be "Assigned" (cannot accept/decline if already ongoing)

---

#### Phase 4: Task Completion

**Actor**: Child

**Steps**:
1. Child views task with Status = "Ongoing" on `ChildTasks.aspx`
2. Task displays all objectives with checkboxes
3. Child marks each objective as complete (client-side validation)
4. When all objectives are checked, "Submit for Review" button becomes enabled
5. Child clicks "Submit for Review"
6. System validates all objectives are completed (client-side + server-side)
7. `TaskHelper.SubmitTaskForReview()` is called
8. Status updated to "Pending Review"
9. `CompletedDate` set to current date/time
10. Task disappears from child's active task list
11. Task appears in parent's review queue

**Database Operations**:
```sql
-- Verify all objectives exist (server-side check)
SELECT COUNT(*) FROM TaskObjectives WHERE TaskId = @TaskId

-- Update assignment
UPDATE TaskAssignments
SET Status = 'Pending Review', CompletedDate = GETDATE()
WHERE Id = @TaskAssignmentId AND UserId = @UserId AND Status = 'Ongoing'
```

**Validation**:
- All objectives must be marked as complete (client-side checkbox validation)
- Assignment must exist and belong to current user
- Status must be "Ongoing" (cannot submit if not accepted)

**Note**: Current implementation assumes client-side validation of objectives. Server-side check exists but always returns `true` (see `AreAllObjectivesCompleted()` method).

---

#### Phase 5: Parent Review

**Actor**: Parent

**Steps**:
1. Parent navigates to `TaskReview.aspx`
2. System calls `TaskHelper.AutoFailOverdueTasks()` to auto-fail overdue tasks
3. System loads tasks via `TaskHelper.GetTasksPendingReview(familyId)`
4. Parent sees list of tasks with Status = "Pending Review"
5. For each task, parent sees:
   - Task title, description, category
   - Child name who completed it
   - Completion date
   - Deadline (if set)
   - All objectives (for reference)
6. Parent can:
   - **Rate**: Select 1-5 stars (radio buttons)
   - **Fail**: Click "Mark as Failed" button
7. If rating selected:
   - Points preview shown (calculated based on rating)
   - Parent clicks "Submit Review"
8. If failed:
   - Parent clicks "Mark as Failed"
9. `TaskHelper.ReviewTask()` is called with:
   - `taskAssignmentId`
   - `rating` (1-5 or 0 if failed)
   - `reviewedBy` (parent user ID)
   - `isFailed` (boolean)
10. System calculates points using `TaskHelper.CalculatePointsAwarded()`
11. `TaskReview` record inserted
12. Points transaction created via `AddPointTransaction()`
13. **Assignment record DELETED** (not updated to "Reviewed")
14. Task becomes available for reassignment
15. Review list refreshed

**Database Operations**:

**Review Task**:
```sql
-- Get assignment info
SELECT ta.TaskId, ta.UserId, t.PointsReward
FROM TaskAssignments ta
INNER JOIN Tasks t ON ta.TaskId = t.Id
WHERE ta.Id = @TaskAssignmentId AND ta.Status = 'Pending Review'

-- Insert review
INSERT INTO TaskReviews (TaskAssignmentId, Rating, PointsAwarded, IsFailed, ReviewedBy)
VALUES (@TaskAssignmentId, @Rating, @PointsAwarded, @IsFailed, @ReviewedBy)

-- Add point transaction
INSERT INTO PointTransactions (UserId, Points, TransactionType, Description, TaskAssignmentId)
VALUES (@UserId, @Points, @TransactionType, @Description, @TaskAssignmentId)

-- DELETE assignment (not update!)
DELETE FROM TaskAssignments WHERE Id = @TaskAssignmentId
```

**Points Calculation**:
- See [Points Calculation System](#points-calculation-system) section below

**Validation**:
- Assignment must exist
- Status must be "Pending Review"
- Rating must be 1-5 (if not failed)
- Parent must be in same family as task

---

### Auto-Fail Overdue Tasks

**Trigger**: 
- On `ChildTasks.aspx` page load (for children)
- On `TaskReview.aspx` page load (for parents)

**Process**:
1. System calls `TaskHelper.AutoFailOverdueTasks(familyId)`
2. System queries for overdue tasks:
   - `Deadline IS NOT NULL`
   - `Deadline < GETDATE()`
   - `Status IN ('Assigned', 'Ongoing')`
   - No review exists yet
3. For each overdue task:
   - System calls `TaskHelper.ReviewTask()` with:
     - `rating = 0`
     - `isFailed = true`
     - `reviewedBy = task.CreatedBy` (task creator)
4. Points deducted (-50% of total points)
5. Review record created
6. Assignment deleted
7. Task becomes available for reassignment

**Database Query**:
```sql
SELECT ta.Id AS AssignmentId, ta.TaskId, ta.UserId, ta.Deadline, ta.Status,
       t.PointsReward, t.FamilyId, t.CreatedBy
FROM TaskAssignments ta
INNER JOIN Tasks t ON ta.TaskId = t.Id
WHERE ta.Deadline IS NOT NULL
  AND ta.Deadline < @Now
  AND ta.Status IN ('Assigned', 'Ongoing')
  AND NOT EXISTS (SELECT 1 FROM TaskReviews tr WHERE tr.TaskAssignmentId = ta.Id)
  AND t.FamilyId = @FamilyId
```

---

## Status State Machine

### Status Transition Diagram

```
                    ┌─────────────┐
                    │   CREATED   │  (Task exists, no assignment)
                    └──────┬──────┘
                           │
                           │ [Parent assigns]
                           ▼
                    ┌─────────────┐
                    │  ASSIGNED    │  (Waiting for child response)
                    └──────┬──────┘
                           │
        ┌──────────────────┼──────────────────┐
        │                  │                    │
        │ [Child accepts]  │ [Child declines]   │ [Auto-fail if overdue]
        ▼                  ▼                    ▼
┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   ONGOING   │    │  DECLINED   │    │   FAILED    │
│             │    │             │    │  (Auto)     │
└──────┬──────┘    └─────────────┘    └──────┬──────┘
       │                                    │
       │ [Child submits]                    │
       │                                    │
       ▼                                    │
┌─────────────┐                            │
│PENDING REVIEW│                            │
└──────┬──────┘                            │
       │                                    │
       │ [Parent reviews]                  │
       │                                    │
       ├────────────────────────────────────┘
       │
       ▼
┌─────────────┐
│   DELETED   │  (Assignment deleted, task available for reassignment)
└─────────────┘
```

### Status Definitions

| Status | Description | Who Can See | Next Actions |
|--------|-------------|-------------|--------------|
| **Created** | Task exists but not assigned | Parent | Assign to child |
| **Assigned** | Task assigned, waiting for child | Parent, Child | Child: Accept/Decline |
| **Ongoing** | Child accepted, working on task | Parent, Child | Child: Submit for review |
| **Pending Review** | Child submitted, waiting parent | Parent, Child | Parent: Review (rate/fail) |
| **Declined** | Child declined the task | Parent | Parent: Reassign |
| **Failed** | Auto-failed (overdue) or parent marked failed | Parent, Child | Task available for reassignment |
| **Deleted** | Assignment deleted after review | N/A | Task available for reassignment |

### Status Transition Rules

**Valid Transitions**:
- `Created` → `Assigned` (via assignment)
- `Assigned` → `Ongoing` (via child accept)
- `Assigned` → `Declined` (via child decline)
- `Assigned` → `Failed` (via auto-fail if overdue)
- `Ongoing` → `Pending Review` (via child submit)
- `Ongoing` → `Failed` (via auto-fail if overdue)
- `Pending Review` → `Deleted` (via parent review)

**Invalid Transitions** (prevented by validation):
- Cannot accept if status is not "Assigned"
- Cannot decline if status is not "Assigned"
- Cannot submit if status is not "Ongoing"
- Cannot review if status is not "Pending Review"

---

## User Roles & Actions

### Parent Role

**Pages**:
- `Tasks.aspx` - Create, view, edit, delete, assign tasks
- `AssignTask.aspx` - Assign task to child
- `TaskReview.aspx` - Review completed tasks

**Actions**:

| Action | Method | Validation |
|--------|--------|------------|
| Create Task | `TaskHelper.CreateTask()` | Must be parent, in family |
| Edit Task | `TaskHelper.UpdateTask()` | Task must not be assigned |
| Delete Task | `TaskHelper.DeleteTask()` | Task must not be assigned |
| Assign Task | `TaskHelper.AssignTask()` | Child not banned, no duplicate |
| Review Task | `TaskHelper.ReviewTask()` | Status must be "Pending Review" |
| View Tasks | `TaskHelper.GetFamilyTasks()` | Must be in family |
| View Assignments | `TaskHelper.GetTaskAssignments()` | Must own task |

---

### Child Role

**Pages**:
- `ChildTasks.aspx` - View assigned tasks, accept/decline, submit

**Actions**:

| Action | Method | Validation |
|--------|--------|------------|
| Accept Task | `TaskHelper.AcceptTask()` | Status must be "Assigned" |
| Decline Task | `TaskHelper.DenyTask()` | Status must be "Assigned" |
| Submit Task | `TaskHelper.SubmitTaskForReview()` | Status must be "Ongoing", all objectives complete |
| View Tasks | `TaskHelper.GetChildTasks()` | Must be assigned to user |

---

## Business Rules

### Task Creation Rules

1. **Only parents can create tasks**
   - Role check: `Session["UserRole"] == "PARENT"`
   - Must be member of a family

2. **Task must belong to a family**
   - `FamilyId` is required
   - Parent must be in the family

3. **Task must have at least one objective**
   - Objectives are required for task completion tracking

4. **Points must be positive**
   - `PointsReward > 0`

---

### Task Assignment Rules

1. **Cannot assign to banned children**
   - Check: `Users.IsBanned = 0`
   - Banned children cannot receive new tasks

2. **Cannot duplicate assign**
   - Cannot assign same task to same child if:
     - Previous assignment status is "Assigned", "Ongoing", "Completed", or "Reviewed"
   - **Exception**: Can reassign if previous status was "Declined"

3. **Child must be in same family**
   - `Task.FamilyId = Child.FamilyId`

4. **Task must exist and be active**
   - `Tasks.IsActive = 1`

5. **Deadline must be in future** (if provided)
   - `Deadline > GETDATE()`

---

### Task Editing Rules

1. **Cannot edit assigned tasks**
   - Check: `IsTaskAssigned(taskId)` returns `false`
   - If any assignment exists with `Status != 'Declined'`, editing is blocked

2. **Only task creator can edit**
   - `Task.CreatedBy = CurrentUser.Id`

3. **Objectives can be modified**
   - Old objectives deleted
   - New objectives inserted with new `OrderIndex`

---

### Task Deletion Rules

1. **Cannot delete assigned tasks**
   - Same check as editing: `IsTaskAssigned(taskId)`

2. **Only task creator can delete**
   - `Task.CreatedBy = CurrentUser.Id`

3. **Cascade deletion**
   - Objectives deleted via `ON DELETE CASCADE`
   - Assignments must be deleted first (or prevented if exist)

---

### Task Acceptance Rules

1. **Only assigned child can accept**
   - `TaskAssignment.UserId = CurrentUser.Id`

2. **Status must be "Assigned"**
   - Cannot accept if already "Ongoing" or other status

3. **Child must not be banned**
   - Check: `Users.IsBanned = 0`

---

### Task Submission Rules

1. **All objectives must be completed**
   - Client-side: All checkboxes checked
   - Server-side: `AreAllObjectivesCompleted()` check (currently always returns true)

2. **Status must be "Ongoing"**
   - Cannot submit if not accepted

3. **Only assigned child can submit**
   - `TaskAssignment.UserId = CurrentUser.Id`

---

### Task Review Rules

1. **Only parents can review**
   - Role check: `Session["UserRole"] == "PARENT"`

2. **Status must be "Pending Review"**
   - Cannot review if not submitted by child

3. **Parent must be in same family**
   - `Task.FamilyId = Parent.FamilyId`

4. **Rating must be 1-5** (if not failed)
   - Validation: `rating >= 1 && rating <= 5`

5. **Assignment is deleted after review**
   - Not marked as "Reviewed"
   - Task becomes available for reassignment

---

### Auto-Fail Rules

1. **Only applies to tasks with deadlines**
   - `TaskAssignment.Deadline IS NOT NULL`

2. **Only applies to active assignments**
   - `Status IN ('Assigned', 'Ongoing')`

3. **Deadline must be past**
   - `Deadline < GETDATE()`

4. **No review must exist**
   - `NOT EXISTS (SELECT 1 FROM TaskReviews WHERE TaskAssignmentId = ta.Id)`

5. **Points deducted: -50%**
   - Same as manual "Failed" marking

---

## Points Calculation System

### Points Calculation Formula

**Method**: `TaskHelper.CalculatePointsAwarded(int totalPoints, int rating, bool isFailed)`

**Formula**:

```csharp
if (isFailed)
{
    return (int)Math.Round(totalPoints * -0.5);  // -50% deduction
}

switch (rating)
{
    case 1:
    case 2:
        return (int)Math.Round(totalPoints * 0.2);  // 20%
    case 3:
        return (int)Math.Round(totalPoints * 0.5);  // 50%
    case 4:
        return (int)Math.Round(totalPoints * 0.75); // 75%
    case 5:
        return totalPoints;  // 100%
    default:
        return 0;
}
```

### Points Award Table

| Rating | Points Awarded | Example (100 point task) |
|--------|----------------|--------------------------|
| 1 star | 20% | 20 points |
| 2 stars | 20% | 20 points |
| 3 stars | 50% | 50 points |
| 4 stars | 75% | 75 points |
| 5 stars | 100% | 100 points |
| Failed | -50% | -50 points (deduction) |

### Points Transaction Details

**Transaction Creation**:
- **Type**: `"Earned"` if points > 0, `"Spent"` if points < 0
- **Points**: Stored as absolute value (always positive in database)
- **Description**: 
  - Success: `"Task completed: +{points} mokipoints (Rating: {rating} stars)"`
  - Failed: `"Task failed: -{points} mokipoints"`
- **TaskAssignmentId**: Links to the assignment (before deletion)

**Example Transactions**:

```sql
-- 5 star rating (100 point task)
INSERT INTO PointTransactions (UserId, Points, TransactionType, Description, TaskAssignmentId)
VALUES (123, 100, 'Earned', 'Task completed: +100 mokipoints (Rating: 5 stars)', 456)

-- Failed task (100 point task)
INSERT INTO PointTransactions (UserId, Points, TransactionType, Description, TaskAssignmentId)
VALUES (123, 50, 'Spent', 'Task failed: -50 mokipoints', 456)
```

---

## Validation Rules

### Client-Side Validation

**Task Creation Form** (`Tasks.aspx`):
- Title: Required, max 200 characters
- Category: Required, must select from dropdown
- Points: Required, must be positive integer
- Objectives: At least one required, max 500 characters each

**Task Assignment Form** (`AssignTask.aspx`):
- Child Selection: Required (RequiredFieldValidator)
- Deadline Date: Optional, must be valid date format
- Deadline Time: Optional, must be valid time format

**Task Review Form** (`TaskReview.aspx`):
- Rating: Required if not failed, must be 1-5
- Cannot submit without rating or "Fail" button

---

### Server-Side Validation

**Task Creation**:
- Title: Required, not empty, max 200 chars
- Category: Required, must be valid category
- Points: Required, must be > 0
- Objectives: At least one, each max 500 chars
- User must be parent
- User must be in family

**Task Assignment**:
- TaskId: Must exist and be valid
- UserId (child): Must exist, must be CHILD role, must not be banned
- Family check: Child must be in same family as task
- Duplicate check: No existing assignment with Status != 'Declined'
- Deadline: If provided, must be valid datetime, should be in future

**Task Acceptance/Decline**:
- AssignmentId: Must exist
- UserId: Must match current user
- Status: Must be "Assigned"

**Task Submission**:
- AssignmentId: Must exist
- UserId: Must match current user
- Status: Must be "Ongoing"
- Objectives: All must be completed (client-side validation)

**Task Review**:
- AssignmentId: Must exist
- Status: Must be "Pending Review"
- Rating: Must be 1-5 (if not failed)
- ReviewedBy: Must be parent, must be in same family

---

## Edge Cases & Special Scenarios

### 1. Multiple Children, Same Task

**Scenario**: Parent assigns same task to multiple children simultaneously.

**Behavior**:
- Each child gets independent `TaskAssignment` record
- Each assignment has independent status
- Parent can see all assignments on `Tasks.aspx` with per-child status badges
- Each child can accept/decline independently
- Each child can submit independently
- Parent reviews each assignment separately

**Database**:
```sql
-- Task: "Clean Room" (TaskId = 1)
-- Assignment 1: Child A (Status = "Ongoing")
-- Assignment 2: Child B (Status = "Assigned")
-- Assignment 3: Child C (Status = "Pending Review")
```

**Display**: Parent sees task with 3 assignments, each with different status.

---

### 2. Task Reassignment After Review

**Scenario**: After parent reviews task, assignment is deleted. Task can be reassigned.

**Behavior**:
- Assignment record is **DELETED** (not marked as "Reviewed")
- Task remains in `Tasks` table
- `TaskReview` record remains (historical record)
- Parent can assign same task again to:
  - Same child (new assignment)
  - Different child
  - Multiple children

**Database State After Review**:
```sql
-- Before Review:
TaskAssignments: [Id=1, TaskId=1, UserId=2, Status='Pending Review']

-- After Review:
TaskAssignments: [DELETED]
TaskReviews: [Id=1, TaskAssignmentId=1, Rating=5, PointsAwarded=100]
PointTransactions: [Id=1, UserId=2, Points=100, Type='Earned', TaskAssignmentId=1]

-- Task still exists:
Tasks: [Id=1, Title='Clean Room', ...]
```

**Reassignment**:
- Parent can assign TaskId=1 again
- New `TaskAssignment` record created (new Id)
- Previous review/points remain in history

---

### 3. Overdue Task Auto-Fail

**Scenario**: Task has deadline, child doesn't complete in time.

**Behavior**:
- On page load (`ChildTasks.aspx` or `TaskReview.aspx`), system checks for overdue tasks
- Tasks with `Deadline < GETDATE()` and `Status IN ('Assigned', 'Ongoing')` are auto-failed
- Points deducted: -50% of total points
- Review record created with `IsFailed = true`
- Assignment deleted
- Task available for reassignment

**Timeline Example**:
```
Day 1: Task assigned with deadline = Day 3, 5:00 PM
Day 3, 5:00 PM: Deadline passes
Day 4: Child logs in → Auto-fail triggered
       - Status: "Assigned" → "Failed" (via review)
       - Points: -50 deducted
       - Assignment deleted
```

---

### 4. Child Declines Task

**Scenario**: Child declines assigned task.

**Behavior**:
- Status updated to "Declined"
- Task disappears from child's task list
- Task remains in parent's task list
- Parent can see task has "Declined" status
- Parent can reassign to:
  - Same child (new assignment)
  - Different child
- Previous "Declined" assignment remains in database (for history)

**Database**:
```sql
-- Assignment remains:
TaskAssignments: [Id=1, TaskId=1, UserId=2, Status='Declined']

-- Parent can create new assignment:
TaskAssignments: [Id=2, TaskId=1, UserId=2, Status='Assigned']  -- New assignment
```

---

### 5. Editing Task with Assignments

**Scenario**: Parent tries to edit task that has active assignments.

**Behavior**:
- Edit button is **disabled** in UI (if assignments exist)
- Server-side check: `IsTaskAssigned(taskId)` returns `true`
- `TaskHelper.UpdateTask()` returns `false`
- Error message: "Cannot edit task that is assigned to children"
- Task can only be edited if all assignments are "Declined" or deleted

**Validation**:
```csharp
if (IsTaskAssigned(taskId))
{
    return false; // Cannot edit
}
```

---

### 6. Banned Child Assignment

**Scenario**: Parent tries to assign task to banned child.

**Behavior**:
- Banned children are **excluded** from child dropdown
- Server-side check: `Users.IsBanned = 1`
- `TaskHelper.AssignTask()` returns `false`
- Error message: "This child is currently banned from receiving tasks"
- Assignment prevented

**Database Check**:
```sql
SELECT IsBanned FROM Users
WHERE Id = @UserId AND Role = 'CHILD' AND IsActive = 1
```

---

### 7. Task with No Objectives

**Scenario**: Task created without objectives (edge case).

**Behavior**:
- Current implementation requires at least one objective
- Client-side validation prevents submission without objectives
- Server-side: `AreAllObjectivesCompleted()` returns `true` if no objectives exist
- Child can submit task even if no objectives (considered "completed")

**Code**:
```csharp
if (totalObjectives == 0) return true; // No objectives means completed
```

---

## Current Implementation Details

### File Structure

**Backend (C#)**:
- `App_Code/TaskHelper.cs` - All task-related business logic
- `App_Code/DatabaseHelper.cs` - Database operations
- `App_Code/FamilyHelper.cs` - Family operations

**Frontend (ASP.NET Web Forms)**:
- `Tasks.aspx` / `Tasks.aspx.cs` - Parent task management
- `AssignTask.aspx` / `AssignTask.aspx.cs` - Task assignment page
- `ChildTasks.aspx` / `ChildTasks.aspx.cs` - Child task view
- `TaskReview.aspx` / `TaskReview.aspx.cs` - Parent review page

**Database**:
- `App_Code/DatabaseInitializer.cs` - Table creation and schema

---

### Key Methods Reference

#### TaskHelper Class

**Task Management**:
- `CreateTask()` - Create new task with objectives
- `UpdateTask()` - Update task (only if not assigned)
- `DeleteTask()` - Delete task (only if not assigned)
- `GetTaskDetails()` - Get task information
- `GetTaskObjectives()` - Get task objectives
- `IsTaskAssigned()` - Check if task has active assignments

**Assignment Management**:
- `AssignTask()` - Assign task to child
- `UnassignTask()` - Remove assignment (only if not accepted)
- `GetTaskAssignments()` - Get all assignments for a task
- `GetTaskAssignment()` - Get specific assignment details

**Child Actions**:
- `AcceptTask()` - Child accepts assignment
- `DenyTask()` - Child declines assignment
- `SubmitTaskForReview()` - Child submits completed task
- `GetChildTasks()` - Get tasks assigned to child

**Parent Actions**:
- `GetFamilyTasks()` - Get all tasks for family
- `GetTasksPendingReview()` - Get tasks waiting for review
- `ReviewTask()` - Parent reviews and awards points
- `AutoFailOverdueTasks()` - Auto-fail overdue tasks

**Points**:
- `CalculatePointsAwarded()` - Calculate points based on rating
- `AddPointTransaction()` - Record point transaction (private)

---

### Data Flow Examples

#### Example 1: Complete Task Lifecycle

```
1. Parent creates task:
   Tasks: [Id=1, Title='Clean Room', PointsReward=100, FamilyId=1]
   TaskObjectives: [Id=1, TaskId=1, Text='Vacuum floor', OrderIndex=0]
                  [Id=2, TaskId=1, Text='Make bed', OrderIndex=1]

2. Parent assigns to Child A:
   TaskAssignments: [Id=1, TaskId=1, UserId=2, Status='Assigned', Deadline='2025-11-25 17:00']

3. Child A accepts:
   TaskAssignments: [Id=1, Status='Ongoing', AcceptedDate='2025-11-22 10:00']

4. Child A submits:
   TaskAssignments: [Id=1, Status='Pending Review', CompletedDate='2025-11-23 15:00']

5. Parent reviews (5 stars):
   TaskReviews: [Id=1, TaskAssignmentId=1, Rating=5, PointsAwarded=100, IsFailed=0]
   PointTransactions: [Id=1, UserId=2, Points=100, Type='Earned', Description='Task completed: +100 mokipoints (Rating: 5 stars)']
   TaskAssignments: [Id=1, DELETED]

6. Task available for reassignment:
   Tasks: [Id=1, ...] (still exists)
   TaskAssignments: (empty for this task)
```

#### Example 2: Failed Task

```
1. Task assigned with deadline:
   TaskAssignments: [Id=1, TaskId=1, UserId=2, Status='Assigned', Deadline='2025-11-20 17:00']

2. Deadline passes (2025-11-21):
   (No action yet)

3. Child logs in (2025-11-22):
   Auto-fail triggered:
   TaskReviews: [Id=1, TaskAssignmentId=1, Rating=NULL, PointsAwarded=-50, IsFailed=1]
   PointTransactions: [Id=1, UserId=2, Points=50, Type='Spent', Description='Task failed: -50 mokipoints']
   TaskAssignments: [Id=1, DELETED]
```

---

## API/Method Reference

### TaskHelper Methods

#### `CreateTask()`
```csharp
public static int CreateTask(
    string title, 
    string description, 
    string category, 
    int pointsReward, 
    int createdBy, 
    int familyId, 
    List<string> objectives
)
```
**Returns**: Task ID (or -1 on error)

---

#### `UpdateTask()`
```csharp
public static bool UpdateTask(
    int taskId, 
    string title, 
    string description, 
    string category, 
    int pointsReward, 
    List<string> objectives
)
```
**Returns**: `true` if successful, `false` if task is assigned

---

#### `AssignTask()`
```csharp
public static bool AssignTask(
    int taskId, 
    int userId, 
    DateTime? deadline
)
```
**Returns**: `true` if successful, `false` if:
- Child is banned
- Duplicate assignment exists
- Other validation fails

---

#### `AcceptTask()`
```csharp
public static bool AcceptTask(
    int taskAssignmentId, 
    int userId
)
```
**Returns**: `true` if successful, `false` if status is not "Assigned"

---

#### `SubmitTaskForReview()`
```csharp
public static bool SubmitTaskForReview(
    int taskAssignmentId, 
    int userId
)
```
**Returns**: `true` if successful, `false` if:
- Status is not "Ongoing"
- Objectives not completed (server-side check)

---

#### `ReviewTask()`
```csharp
public static bool ReviewTask(
    int taskAssignmentId, 
    int rating, 
    int reviewedBy, 
    bool isFailed
)
```
**Returns**: `true` if successful, `false` if:
- Status is not "Pending Review"
- Assignment doesn't exist

**Side Effects**:
- Creates `TaskReview` record
- Creates `PointTransaction` record
- **Deletes** `TaskAssignment` record

---

#### `CalculatePointsAwarded()`
```csharp
public static int CalculatePointsAwarded(
    int totalPoints, 
    int rating, 
    bool isFailed
)
```
**Returns**: Points to award (can be negative for failed tasks)

---

#### `AutoFailOverdueTasks()`
```csharp
public static int AutoFailOverdueTasks(
    int? familyId = null
)
```
**Returns**: Number of tasks auto-failed

**Process**:
- Finds overdue tasks
- Calls `ReviewTask()` for each with `isFailed = true`
- Returns count of failed tasks

---

## Known Issues & Limitations

### 1. Objective Completion Validation

**Issue**: Server-side validation for objective completion always returns `true`.

**Location**: `TaskHelper.AreAllObjectivesCompleted()`

**Current Code**:
```csharp
// Get completed objectives count (stored in a separate table or we need to track this)
// For now, we'll assume if task is submitted, objectives are checked client-side
// This will be handled by the UI checking all checkboxes
return true;
```

**Impact**: 
- Relies entirely on client-side validation
- Malicious users could submit tasks without completing objectives
- No server-side verification

**Recommendation for Rework**:
- Add `TaskObjectiveCompletions` table to track which objectives are completed
- Store completion status per assignment
- Validate server-side before allowing submission

---

### 2. Assignment Deletion After Review

**Issue**: Assignment is deleted after review, losing historical assignment data.

**Current Behavior**:
- Assignment record deleted
- Only `TaskReview` and `PointTransaction` remain
- Cannot track which child completed which task historically

**Impact**:
- No historical record of assignments
- Cannot query "tasks completed by child X"
- Cannot show assignment history

**Recommendation for Rework**:
- Consider soft-delete (mark as "Reviewed" instead of delete)
- Or add `AssignmentHistory` table
- Or add `IsDeleted` flag to `TaskAssignments`

---

### 3. Auto-Fail Uses Task Creator as Reviewer

**Issue**: When auto-failing, `reviewedBy` is set to `task.CreatedBy` (task creator), not necessarily the parent reviewing.

**Current Code**:
```csharp
if (ReviewTask(assignmentId, 0, createdBy, true))  // createdBy = task creator
```

**Impact**:
- Review record shows task creator as reviewer
- May not be the actual parent who should review
- Could be different parent in same family

**Recommendation for Rework**:
- Use family owner or specific parent for auto-fail reviews
- Or add separate "Auto-Failed" status

---

### 4. No Notification System

**Issue**: No notifications when:
- Task is assigned to child
- Child accepts/declines task
- Child submits task for review
- Task is auto-failed

**Impact**:
- Users must manually check pages
- No real-time updates
- Poor user experience

**Recommendation for Rework**:
- Add notification system
- Email/push notifications
- In-app notification center

---

### 5. Deadline Validation

**Issue**: Deadline validation allows past dates during assignment (only client-side check).

**Current Behavior**:
- Client-side: Date picker prevents past dates
- Server-side: No validation that deadline is in future

**Impact**:
- Could assign task with past deadline (if client-side bypassed)
- Auto-fail would trigger immediately

**Recommendation for Rework**:
- Add server-side deadline validation
- Ensure `Deadline > GETDATE()` if provided

---

### 6. Task Reassignment Logic

**Issue**: After review, task can be reassigned, but there's no clear indication of previous completions.

**Current Behavior**:
- Task can be reassigned after review
- No indication it was previously completed
- Could confuse parents

**Impact**:
- Parents might not realize task was already completed
- No history of previous completions visible

**Recommendation for Rework**:
- Show completion history on task
- Add "Previously completed by..." indicator
- Or prevent reassignment of completed tasks

---

### 7. Points Transaction Storage

**Issue**: `Points` is stored as absolute value, `TransactionType` determines direction.

**Current Behavior**:
- `Points` always positive (e.g., 50 for -50 deduction)
- `TransactionType = 'Spent'` indicates deduction
- Requires calculation to determine actual points change

**Impact**:
- More complex queries to calculate balance
- Must check both `Points` and `TransactionType`

**Recommendation for Rework**:
- Consider storing signed integers (positive/negative)
- Or keep current approach but document clearly

---

## Summary

This document provides a comprehensive overview of the current task system implementation. Key points:

1. **Status Flow**: Created → Assigned → Ongoing → Pending Review → Deleted (after review)
2. **Multi-Child Support**: Same task can be assigned to multiple children with independent status
3. **Review System**: Parent rates 1-5 stars or marks as failed, points calculated accordingly
4. **Auto-Fail**: Overdue tasks automatically fail with -50% point deduction
5. **Reassignment**: After review, assignment is deleted, task can be reassigned

**Areas for Improvement** (identified for rework):
- Objective completion tracking (server-side)
- Assignment history preservation
- Notification system
- Deadline validation
- Task completion history visibility

---

**Document Version**: 1.0  
**Last Updated**: November 22, 2025  
**Maintained By**: Development Team

