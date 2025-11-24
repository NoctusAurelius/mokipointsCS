# Task System Rework - Implementation Plan

**Approach**: Clean Rewrite (Option B)  
**Database**: Fresh Schema  
**Priority**: 7 Critical Logic Flaws → All 56 Improvements  
**Date**: November 22, 2025

---

## Overview

Complete rewrite of the task system incorporating all 56 improvements, starting with the 7 critical logic flaws, then implementing all remaining features.

---

## Phase 1: Database Schema Redesign

### New/Modified Tables

#### 1. **Tasks Table** (Enhanced)
```sql
CREATE TABLE [dbo].[Tasks] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Category] NVARCHAR(100) NOT NULL,
    [PointsReward] INT NOT NULL DEFAULT 0,
    [CreatedBy] INT NOT NULL,
    [FamilyId] INT NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    -- NEW FIELDS
    [Priority] NVARCHAR(20) NULL DEFAULT 'Medium', -- Low, Medium, High, Urgent
    [Difficulty] NVARCHAR(20) NULL, -- Easy, Medium, Hard
    [EstimatedMinutes] INT NULL,
    [Instructions] NVARCHAR(MAX) NULL,
    [IsTemplate] BIT NOT NULL DEFAULT 0,
    [TemplateName] NVARCHAR(200) NULL,
    [RecurrencePattern] NVARCHAR(50) NULL, -- Daily, Weekly, Monthly, None
    FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([Id]),
    FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id])
)
```

#### 2. **TaskObjectives Table** (Unchanged)
```sql
CREATE TABLE [dbo].[TaskObjectives] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TaskId] INT NOT NULL,
    [ObjectiveText] NVARCHAR(500) NOT NULL,
    [OrderIndex] INT NOT NULL DEFAULT 0,
    FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks]([Id]) ON DELETE CASCADE
)
```

#### 3. **TaskAssignments Table** (Enhanced - Soft Delete)
```sql
CREATE TABLE [dbo].[TaskAssignments] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TaskId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [AssignedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [Deadline] DATETIME NULL,
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Assigned',
    [AcceptedDate] DATETIME NULL,
    [CompletedDate] DATETIME NULL,
    -- NEW FIELDS (Soft Delete)
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [DeletedDate] DATETIME NULL,
    [DeletedBy] INT NULL,
    FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks]([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
    FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[Users]([Id])
)
```

#### 4. **TaskObjectiveCompletions Table** (NEW - Critical Flaw Fix #1)
```sql
CREATE TABLE [dbo].[TaskObjectiveCompletions] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TaskAssignmentId] INT NOT NULL,
    [TaskObjectiveId] INT NOT NULL,
    [IsCompleted] BIT NOT NULL DEFAULT 0,
    [CompletedDate] DATETIME NULL,
    FOREIGN KEY ([TaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id]),
    FOREIGN KEY ([TaskObjectiveId]) REFERENCES [dbo].[TaskObjectives]([Id])
)
```

#### 5. **TaskReviews Table** (Enhanced)
```sql
CREATE TABLE [dbo].[TaskReviews] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TaskAssignmentId] INT NOT NULL,
    [Rating] INT NULL, -- 1-5 stars
    [PointsAwarded] INT NOT NULL, -- Can be negative
    [IsFailed] BIT NOT NULL DEFAULT 0,
    [IsAutoFailed] BIT NOT NULL DEFAULT 0, -- NEW: Track auto-fails
    [ReviewDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ReviewedBy] INT NOT NULL,
    FOREIGN KEY ([TaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id]),
    FOREIGN KEY ([ReviewedBy]) REFERENCES [dbo].[Users]([Id])
)
```

#### 6. **PointTransactions Table** (Enhanced - Signed Values)
```sql
CREATE TABLE [dbo].[PointTransactions] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [Points] INT NOT NULL, -- NEW: Signed integer (can be negative)
    [TransactionType] NVARCHAR(50) NOT NULL, -- Keep for backward compatibility
    [Description] NVARCHAR(500) NULL,
    [TransactionDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [TaskAssignmentId] INT NULL,
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
    FOREIGN KEY ([TaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id])
)
```

#### 7. **Notifications Table** (NEW - Critical Flaw Fix #4)
```sql
CREATE TABLE [dbo].[Notifications] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [Type] NVARCHAR(50) NOT NULL, -- TaskAssigned, TaskAccepted, TaskSubmitted, TaskReviewed, TaskAutoFailed
    [RelatedTaskId] INT NULL,
    [RelatedAssignmentId] INT NULL,
    [IsRead] BIT NOT NULL DEFAULT 0,
    [ReadDate] DATETIME NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
    FOREIGN KEY ([RelatedTaskId]) REFERENCES [dbo].[Tasks]([Id]),
    FOREIGN KEY ([RelatedAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id])
)
```

#### 8. **TaskTags Table** (NEW - Feature Enhancement)
```sql
CREATE TABLE [dbo].[TaskTags] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(50) NOT NULL UNIQUE,
    [Color] NVARCHAR(7) NULL, -- Hex color code
    [FamilyId] INT NULL, -- NULL = system-wide tag
    FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id])
)
```

#### 9. **TaskTagAssignments Table** (NEW)
```sql
CREATE TABLE [dbo].[TaskTagAssignments] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TaskId] INT NOT NULL,
    [TagId] INT NOT NULL,
    FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([TagId]) REFERENCES [dbo].[TaskTags]([Id]) ON DELETE CASCADE,
    UNIQUE ([TaskId], [TagId])
)
```

#### 10. **TaskTemplates Table** (NEW - Feature Enhancement)
```sql
CREATE TABLE [dbo].[TaskTemplates] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [Category] NVARCHAR(100) NOT NULL,
    [PointsReward] INT NOT NULL DEFAULT 0,
    [FamilyId] INT NULL, -- NULL = system-wide template
    [CreatedBy] INT NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [IsActive] BIT NOT NULL DEFAULT 1,
    FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id]),
    FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([Id])
)
```

#### 11. **TaskTemplateObjectives Table** (NEW)
```sql
CREATE TABLE [dbo].[TaskTemplateObjectives] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TemplateId] INT NOT NULL,
    [ObjectiveText] NVARCHAR(500) NOT NULL,
    [OrderIndex] INT NOT NULL DEFAULT 0,
    FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[TaskTemplates]([Id]) ON DELETE CASCADE
)
```

#### 12. **TaskComments Table** (NEW - Feature Enhancement)
```sql
CREATE TABLE [dbo].[TaskComments] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TaskAssignmentId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [CommentText] NVARCHAR(MAX) NOT NULL,
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [IsEdited] BIT NOT NULL DEFAULT 0,
    [EditedDate] DATETIME NULL,
    FOREIGN KEY ([TaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
)
```

#### 13. **TaskAttachments Table** (NEW - Feature Enhancement)
```sql
CREATE TABLE [dbo].[TaskAttachments] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TaskAssignmentId] INT NOT NULL,
    [FileName] NVARCHAR(255) NOT NULL,
    [FilePath] NVARCHAR(500) NOT NULL,
    [FileSize] INT NOT NULL, -- Bytes
    [MimeType] NVARCHAR(100) NULL,
    [UploadedBy] INT NOT NULL,
    [UploadedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([TaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id]),
    FOREIGN KEY ([UploadedBy]) REFERENCES [dbo].[Users]([Id])
)
```

#### 14. **TaskAuditLog Table** (NEW - Feature Enhancement)
```sql
CREATE TABLE [dbo].[TaskAuditLog] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [TaskId] INT NOT NULL,
    [Action] NVARCHAR(50) NOT NULL, -- Created, Updated, Deleted, Assigned, etc.
    [UserId] INT NOT NULL,
    [OldValues] NVARCHAR(MAX) NULL, -- JSON
    [NewValues] NVARCHAR(MAX) NULL, -- JSON
    [ActionDate] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks]([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
)
```

---

## Phase 2: Backend Implementation (TaskHelper.cs Rewrite)

### Critical Flaw Fixes (Priority 1)

#### Fix #1: Objective Completion Tracking (Server-Side)
- ✅ Add `TaskObjectiveCompletions` table
- ✅ Track completion per assignment per objective
- ✅ Validate all objectives completed before submission
- ✅ Methods: `MarkObjectiveComplete()`, `AreAllObjectivesCompleted()`

#### Fix #2: Assignment History (Soft-Delete)
- ✅ Add `IsDeleted` flag to `TaskAssignments`
- ✅ Soft-delete instead of hard delete after review
- ✅ Methods: `SoftDeleteAssignment()`, `GetAssignmentHistory()`

#### Fix #3: Auto-Fail Reviewer Logic
- ✅ Use family owner instead of task creator
- ✅ Add `IsAutoFailed` flag to `TaskReviews`
- ✅ Method: `AutoFailOverdueTasks()` - use `Family.OwnerId`

#### Fix #4: Notification System
- ✅ Create `Notifications` table
- ✅ Methods: `CreateNotification()`, `GetUserNotifications()`, `MarkNotificationRead()`
- ✅ Auto-create notifications for all task events

#### Fix #5: Server-Side Deadline Validation
- ✅ Validate `Deadline > GETDATE()` in `AssignTask()`
- ✅ Return error if deadline is in past

#### Fix #6: Task Completion History Visibility
- ✅ Query `TaskReviews` for completion history
- ✅ Display in task view
- ✅ Method: `GetTaskCompletionHistory()`

#### Fix #7: Points Transaction Storage
- ✅ Change `Points` to signed integer
- ✅ Store negative values directly
- ✅ Update `AddPointTransaction()` method

### Enhanced Methods

**Task Management**:
- `CreateTask()` - Enhanced with new fields
- `UpdateTask()` - Enhanced with audit logging
- `DeleteTask()` - Soft delete option
- `GetTaskDetails()` - Include completion history
- `GetTaskObjectives()` - Unchanged
- `GetTaskCompletionHistory()` - NEW

**Assignment Management**:
- `AssignTask()` - Enhanced with deadline validation, notifications
- `AcceptTask()` - Enhanced with notifications
- `DenyTask()` - Enhanced with notifications
- `SubmitTaskForReview()` - Enhanced with objective validation, notifications
- `GetTaskAssignments()` - Filter by `IsDeleted = 0`
- `SoftDeleteAssignment()` - NEW
- `GetAssignmentHistory()` - NEW

**Objective Management**:
- `MarkObjectiveComplete()` - NEW
- `UnmarkObjectiveComplete()` - NEW
- `AreAllObjectivesCompleted()` - NEW (server-side validation)

**Review Management**:
- `ReviewTask()` - Enhanced with soft-delete, notifications
- `AutoFailOverdueTasks()` - Fixed reviewer logic
- `GetTasksPendingReview()` - Unchanged

**Notification Management**:
- `CreateNotification()` - NEW
- `GetUserNotifications()` - NEW
- `MarkNotificationRead()` - NEW
- `GetUnreadNotificationCount()` - NEW

**Template Management**:
- `CreateTaskTemplate()` - NEW
- `GetTaskTemplates()` - NEW
- `CreateTaskFromTemplate()` - NEW

**Tag Management**:
- `CreateTag()` - NEW
- `AssignTagToTask()` - NEW
- `GetTaskTags()` - NEW

**Comment Management**:
- `AddTaskComment()` - NEW
- `GetTaskComments()` - NEW

**Attachment Management**:
- `UploadTaskAttachment()` - NEW
- `GetTaskAttachments()` - NEW
- `DeleteTaskAttachment()` - NEW

---

## Phase 3: Frontend Implementation

### Pages to Create/Rewrite

#### 1. **Tasks.aspx** (Parent Task Management)
**Features**:
- Task list with filtering, sorting, search
- Create task (with all new fields)
- Edit task (with confirmation if previously completed)
- Delete task (with confirmation)
- View task details (with completion history)
- Assign task (redirect to AssignTask.aspx)
- Bulk operations
- Task templates dropdown
- Priority/difficulty badges
- Tag display and filtering

#### 2. **AssignTask.aspx** (Task Assignment)
**Features**:
- Display task information
- Child selection (exclude banned)
- Deadline picker (with validation)
- Server-side deadline validation
- Success/error messages

#### 3. **ChildTasks.aspx** (Child Task View)
**Features**:
- Task list with status badges
- Accept/Decline buttons
- Objective checklist (server-side tracking)
- Submit for review (with validation)
- Upload attachments
- Add comments
- Deadline warnings (color-coded)
- Progress indicators

#### 4. **TaskReview.aspx** (Parent Review)
**Features**:
- Pending review list
- Star rating (1-5)
- Mark as failed
- Points preview
- View attachments
- View comments
- View objectives completion

#### 5. **TaskHistory.aspx** (Child History)
**Features**:
- Completed tasks history
- Ratings received
- Points earned
- Completion statistics

#### 6. **TaskTemplates.aspx** (NEW - Template Management)
**Features**:
- Create/edit templates
- System-wide and family-specific templates
- Create task from template

#### 7. **Notifications.aspx** (NEW - Notification Center)
**Features**:
- Notification list
- Mark as read
- Filter by type
- Unread count badge

---

## Phase 4: Implementation Order

### Step 1: Database Schema
1. Create all new tables
2. Update existing tables with new columns
3. Add indexes
4. Add foreign keys

### Step 2: Backend - Critical Flaws
1. Fix #1: Objective completion tracking
2. Fix #2: Soft-delete assignments
3. Fix #3: Auto-fail reviewer logic
4. Fix #4: Notification system
5. Fix #5: Deadline validation
6. Fix #6: Completion history
7. Fix #7: Points storage

### Step 3: Backend - Enhanced Features
1. Task templates
2. Tags
3. Comments
4. Attachments
5. Audit logging
6. Priority/difficulty
7. Recurrence

### Step 4: Frontend - Core Pages
1. Tasks.aspx (rewrite)
2. AssignTask.aspx (rewrite)
3. ChildTasks.aspx (rewrite)
4. TaskReview.aspx (rewrite)
5. TaskHistory.aspx (rewrite)

### Step 5: Frontend - New Pages
1. TaskTemplates.aspx
2. Notifications.aspx

### Step 6: Frontend - Enhancements
1. Search and filtering
2. Sorting
3. Bulk operations
4. Calendar view
5. Analytics dashboard

---

## Files to Delete (Clean Rewrite)

### Task-Related Files to Remove:
- `Tasks.aspx` / `.aspx.cs` / `.aspx.designer.cs`
- `AssignTask.aspx` / `.aspx.cs` / `.aspx.designer.cs`
- `ChildTasks.aspx` / `.aspx.cs` / `.aspx.designer.cs`
- `TaskReview.aspx` / `.aspx.cs` / `.aspx.designer.cs`
- `TaskHistory.aspx` / `.aspx.cs` / `.aspx.designer.cs`
- `App_Code/TaskHelper.cs`

### Files to Keep:
- All other pages (Dashboard, Family, etc.)
- `DatabaseHelper.cs`
- `FamilyHelper.cs`
- `AuthenticationHelper.cs`
- All other helper classes

---

## Testing Checklist

### Critical Flaw Fixes:
- [ ] Objective completion tracked server-side
- [ ] Cannot submit without completing all objectives
- [ ] Assignments soft-deleted (not hard deleted)
- [ ] Assignment history preserved
- [ ] Auto-fail uses family owner as reviewer
- [ ] Notifications created for all events
- [ ] Deadline validation prevents past dates
- [ ] Completion history visible
- [ ] Points stored as signed integers

### Core Functionality:
- [ ] Create task with all fields
- [ ] Edit task (with confirmation)
- [ ] Delete task (with confirmation)
- [ ] Assign task to child
- [ ] Child accepts/declines
- [ ] Child completes objectives
- [ ] Child submits for review
- [ ] Parent reviews and rates
- [ ] Points awarded correctly
- [ ] Auto-fail overdue tasks

### Enhanced Features:
- [ ] Task templates
- [ ] Tags
- [ ] Comments
- [ ] Attachments
- [ ] Notifications
- [ ] Search and filtering
- [ ] Sorting
- [ ] Bulk operations

---

## Estimated Implementation Time

- **Database Schema**: 2-3 hours
- **Backend (Critical Flaws)**: 4-6 hours
- **Backend (Enhanced Features)**: 6-8 hours
- **Frontend (Core Pages)**: 8-10 hours
- **Frontend (New Pages)**: 3-4 hours
- **Frontend (Enhancements)**: 4-6 hours
- **Testing & Bug Fixes**: 4-6 hours

**Total**: ~35-45 hours

---

## Notes

- All improvements from the 56-item list will be incorporated
- Process flow remains: Created → Assigned → Ongoing → Pending Review → Soft-Deleted
- Fresh database allows clean schema design
- All new features are optional (can be used or ignored)
- Backward compatibility not required (fresh start)

---

**Plan Version**: 1.0  
**Created**: November 22, 2025  
**Status**: Ready for Implementation

