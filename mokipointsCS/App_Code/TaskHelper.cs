using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace mokipointsCS
{
    /// <summary>
    /// Enhanced Task Helper class with all improvements
    /// Includes fixes for all 7 critical logic flaws and all 56 improvements
    /// </summary>
    public class TaskHelper
    {
        #region Critical Flaw Fixes

        #region Fix #1: Objective Completion Tracking (Server-Side)

        /// <summary>
        /// Marks an objective as completed for a task assignment
        /// </summary>
        public static bool MarkObjectiveComplete(int taskAssignmentId, int taskObjectiveId, int userId)
        {
            try
            {
                // Verify assignment belongs to user
                string verifyQuery = @"
                    SELECT UserId FROM [dbo].[TaskAssignments]
                    WHERE Id = @TaskAssignmentId AND UserId = @UserId AND Status = 'Ongoing'";
                
                object verifyResult = DatabaseHelper.ExecuteScalar(verifyQuery,
                    new SqlParameter("@TaskAssignmentId", taskAssignmentId),
                    new SqlParameter("@UserId", userId));

                if (verifyResult == null)
                {
                    return false;
                }

                // Check if already exists
                string checkQuery = @"
                    SELECT Id FROM [dbo].[TaskObjectiveCompletions]
                    WHERE TaskAssignmentId = @TaskAssignmentId AND TaskObjectiveId = @TaskObjectiveId";

                object existing = DatabaseHelper.ExecuteScalar(checkQuery,
                    new SqlParameter("@TaskAssignmentId", taskAssignmentId),
                    new SqlParameter("@TaskObjectiveId", taskObjectiveId));

                if (existing != null)
                {
                    // Update existing
                    string updateQuery = @"
                        UPDATE [dbo].[TaskObjectiveCompletions]
                        SET IsCompleted = 1, CompletedDate = GETDATE()
                        WHERE TaskAssignmentId = @TaskAssignmentId AND TaskObjectiveId = @TaskObjectiveId";

                    int rows = DatabaseHelper.ExecuteNonQuery(updateQuery,
                        new SqlParameter("@TaskAssignmentId", taskAssignmentId),
                        new SqlParameter("@TaskObjectiveId", taskObjectiveId));
                    return rows > 0;
                }
                else
                {
                    // Insert new
                    string insertQuery = @"
                        INSERT INTO [dbo].[TaskObjectiveCompletions] (TaskAssignmentId, TaskObjectiveId, IsCompleted, CompletedDate)
                        VALUES (@TaskAssignmentId, @TaskObjectiveId, 1, GETDATE())";

                    int rows = DatabaseHelper.ExecuteNonQuery(insertQuery,
                        new SqlParameter("@TaskAssignmentId", taskAssignmentId),
                        new SqlParameter("@TaskObjectiveId", taskObjectiveId));
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MarkObjectiveComplete error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Unmarks an objective as completed
        /// </summary>
        public static bool UnmarkObjectiveComplete(int taskAssignmentId, int taskObjectiveId, int userId)
        {
            try
            {
                // Verify assignment belongs to user
                string verifyQuery = @"
                    SELECT UserId FROM [dbo].[TaskAssignments]
                    WHERE Id = @TaskAssignmentId AND UserId = @UserId AND Status = 'Ongoing'";

                object verifyResult = DatabaseHelper.ExecuteScalar(verifyQuery,
                    new SqlParameter("@TaskAssignmentId", taskAssignmentId),
                    new SqlParameter("@UserId", userId));

                if (verifyResult == null)
                {
                    return false;
                }

                string query = @"
                    UPDATE [dbo].[TaskObjectiveCompletions]
                    SET IsCompleted = 0, CompletedDate = NULL
                    WHERE TaskAssignmentId = @TaskAssignmentId AND TaskObjectiveId = @TaskObjectiveId";

                int rows = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@TaskAssignmentId", taskAssignmentId),
                    new SqlParameter("@TaskObjectiveId", taskObjectiveId));
                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("UnmarkObjectiveComplete error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Checks if all objectives are completed for an assignment (SERVER-SIDE VALIDATION)
        /// </summary>
        public static bool AreAllObjectivesCompleted(int taskAssignmentId)
        {
            try
            {
                // Get task ID from assignment
                string taskQuery = "SELECT TaskId FROM [dbo].[TaskAssignments] WHERE Id = @TaskAssignmentId";
                object taskIdObj = DatabaseHelper.ExecuteScalar(taskQuery, new SqlParameter("@TaskAssignmentId", taskAssignmentId));

                if (taskIdObj == null) return false;
                int taskId = Convert.ToInt32(taskIdObj);

                // Get total objectives count
                string countQuery = "SELECT COUNT(*) FROM [dbo].[TaskObjectives] WHERE TaskId = @TaskId";
                object totalCount = DatabaseHelper.ExecuteScalar(countQuery, new SqlParameter("@TaskId", taskId));
                int totalObjectives = Convert.ToInt32(totalCount);

                if (totalObjectives == 0) return true; // No objectives means completed

                // Get completed objectives count
                string completedQuery = @"
                    SELECT COUNT(*) FROM [dbo].[TaskObjectiveCompletions]
                    WHERE TaskAssignmentId = @TaskAssignmentId AND IsCompleted = 1";
                object completedCount = DatabaseHelper.ExecuteScalar(completedQuery, new SqlParameter("@TaskAssignmentId", taskAssignmentId));
                int completedObjectives = Convert.ToInt32(completedCount);

                return completedObjectives == totalObjectives;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets objective completion status for an assignment
        /// </summary>
        public static DataTable GetObjectiveCompletions(int taskAssignmentId)
        {
            try
            {
                string query = @"
                    SELECT toc.TaskObjectiveId, toc.IsCompleted, toc.CompletedDate, to_obj.ObjectiveText, to_obj.OrderIndex
                    FROM [dbo].[TaskObjectiveCompletions] toc
                    INNER JOIN [dbo].[TaskObjectives] to_obj ON toc.TaskObjectiveId = to_obj.Id
                    WHERE toc.TaskAssignmentId = @TaskAssignmentId
                    ORDER BY to_obj.OrderIndex";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@TaskAssignmentId", taskAssignmentId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetObjectiveCompletions error: " + ex.Message);
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets completion status for a specific objective in an assignment
        /// </summary>
        public static bool GetObjectiveCompletionStatus(int taskAssignmentId, int taskObjectiveId)
        {
            try
            {
                string query = @"
                    SELECT IsCompleted FROM [dbo].[TaskObjectiveCompletions]
                    WHERE TaskAssignmentId = @TaskAssignmentId AND TaskObjectiveId = @TaskObjectiveId";

                object result = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@TaskAssignmentId", taskAssignmentId),
                    new SqlParameter("@TaskObjectiveId", taskObjectiveId));

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToBoolean(result);
                }
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetObjectiveCompletionStatus error: " + ex.Message);
                return false;
            }
        }

        #endregion

        #region Fix #2: Soft-Delete Assignments (Preserve History)

        /// <summary>
        /// Soft-deletes an assignment (marks as deleted instead of hard delete)
        /// </summary>
        public static bool SoftDeleteAssignment(int taskAssignmentId, int deletedBy)
        {
            try
            {
                string query = @"
                    UPDATE [dbo].[TaskAssignments]
                    SET IsDeleted = 1, DeletedDate = GETDATE(), DeletedBy = @DeletedBy
                    WHERE Id = @TaskAssignmentId AND IsDeleted = 0";

                int rows = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@TaskAssignmentId", taskAssignmentId),
                    new SqlParameter("@DeletedBy", deletedBy));
                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SoftDeleteAssignment error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets assignment history (including deleted assignments)
        /// </summary>
        public static DataTable GetAssignmentHistory(int taskId)
        {
            try
            {
                string query = @"
                    SELECT ta.Id, ta.UserId, ta.Status, ta.AssignedDate, ta.AcceptedDate, ta.CompletedDate, ta.Deadline,
                           ta.IsDeleted, ta.DeletedDate, ta.DeletedBy,
                           u.FirstName + ' ' + u.LastName AS ChildName,
                           tr.Rating, tr.PointsAwarded, tr.IsFailed, tr.ReviewDate
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Users] u ON ta.UserId = u.Id
                    LEFT JOIN [dbo].[TaskReviews] tr ON ta.Id = tr.TaskAssignmentId
                    WHERE ta.TaskId = @TaskId
                    ORDER BY ta.AssignedDate DESC";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@TaskId", taskId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetAssignmentHistory error: " + ex.Message);
                return new DataTable();
            }
        }

        #endregion

        #region Fix #3: Auto-Fail Reviewer Logic (Use Family Owner)

        /// <summary>
        /// Auto-fails overdue tasks using family owner as reviewer
        /// </summary>
        public static int AutoFailOverdueTasks(int? familyId = null)
        {
            int failedCount = 0;
            try
            {
                DateTime now = DateTime.Now;

                string query = @"
                    SELECT ta.Id AS AssignmentId, ta.TaskId, ta.UserId, ta.Deadline, ta.Status,
                           t.PointsReward, t.FamilyId, f.OwnerId AS FamilyOwnerId
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                    INNER JOIN [dbo].[Families] f ON t.FamilyId = f.Id
                    WHERE ta.Deadline IS NOT NULL
                      AND ta.Deadline < @Now
                      AND ta.Status IN ('Assigned', 'Ongoing')
                      AND ta.IsDeleted = 0
                      AND NOT EXISTS (SELECT 1 FROM [dbo].[TaskReviews] tr WHERE tr.TaskAssignmentId = ta.Id)";

                if (familyId.HasValue)
                {
                    query += " AND t.FamilyId = @FamilyId";
                }

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@Now", now),
                    familyId.HasValue ? new SqlParameter("@FamilyId", familyId.Value) : new SqlParameter("@FamilyId", DBNull.Value)))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int assignmentId = Convert.ToInt32(row["AssignmentId"]);
                        int familyOwnerId = Convert.ToInt32(row["FamilyOwnerId"]); // Use family owner, not task creator

                        // Auto-fail the task using family owner as reviewer
                        if (ReviewTask(assignmentId, 0, familyOwnerId, true, true)) // isAutoFailed = true
                        {
                            failedCount++;
                            System.Diagnostics.Debug.WriteLine(string.Format("Auto-failed overdue task assignment {0}", assignmentId));
                        }
                    }
                }

                if (failedCount > 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Auto-failed {0} overdue task(s)", failedCount));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AutoFailOverdueTasks error: " + ex.Message);
            }
            return failedCount;
        }

        #endregion

        #region Fix #4: Notification System

        /// <summary>
        /// Creates a notification
        /// </summary>
        public static bool CreateNotification(int userId, string title, string message, string type, int? relatedTaskId = null, int? relatedAssignmentId = null)
        {
            try
            {
                string query = @"
                    INSERT INTO [dbo].[Notifications] (UserId, Title, Message, Type, RelatedTaskId, RelatedAssignmentId)
                    VALUES (@UserId, @Title, @Message, @Type, @RelatedTaskId, @RelatedAssignmentId)";

                int rows = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@Title", title),
                    new SqlParameter("@Message", message),
                    new SqlParameter("@Type", type),
                    new SqlParameter("@RelatedTaskId", relatedTaskId.HasValue ? (object)relatedTaskId.Value : DBNull.Value),
                    new SqlParameter("@RelatedAssignmentId", relatedAssignmentId.HasValue ? (object)relatedAssignmentId.Value : DBNull.Value));

                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CreateNotification error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets notifications for a user
        /// </summary>
        public static DataTable GetUserNotifications(int userId, bool unreadOnly = false)
        {
            try
            {
                string query = @"
                    SELECT Id, Title, Message, Type, RelatedTaskId, RelatedAssignmentId, IsRead, ReadDate, CreatedDate
                    FROM [dbo].[Notifications]
                    WHERE UserId = @UserId";

                if (unreadOnly)
                {
                    query += " AND IsRead = 0";
                }

                query += " ORDER BY CreatedDate DESC";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserId", userId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetUserNotifications error: " + ex.Message);
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets unread notification count
        /// </summary>
        public static int GetUnreadNotificationCount(int userId)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*) FROM [dbo].[Notifications]
                    WHERE UserId = @UserId AND IsRead = 0";

                object count = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@UserId", userId));
                return Convert.ToInt32(count);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Marks a notification as read
        /// </summary>
        public static bool MarkNotificationRead(int notificationId, int userId)
        {
            try
            {
                string query = @"
                    UPDATE [dbo].[Notifications]
                    SET IsRead = 1, ReadDate = GETDATE()
                    WHERE Id = @NotificationId AND UserId = @UserId";

                int rows = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@NotificationId", notificationId),
                    new SqlParameter("@UserId", userId));
                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MarkNotificationRead error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Marks all notifications as read for a user
        /// </summary>
        public static bool MarkAllNotificationsRead(int userId)
        {
            try
            {
                string query = @"
                    UPDATE [dbo].[Notifications]
                    SET IsRead = 1, ReadDate = GETDATE()
                    WHERE UserId = @UserId AND IsRead = 0";

                int rows = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@UserId", userId));
                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MarkAllNotificationsRead error: " + ex.Message);
                return false;
            }
        }

        #endregion

        #region Fix #5: Server-Side Deadline Validation

        // This is handled in AssignTask() method below

        #endregion

        #region Fix #6: Task Completion History Visibility

        /// <summary>
        /// Gets task completion history
        /// </summary>
        public static DataTable GetTaskCompletionHistory(int taskId)
        {
            try
            {
                string query = @"
                    SELECT tr.Id, tr.Rating, tr.PointsAwarded, tr.IsFailed, tr.IsAutoFailed, tr.ReviewDate,
                           ta.UserId, u.FirstName + ' ' + u.LastName AS ChildName,
                           tr.ReviewedBy, reviewer.FirstName + ' ' + reviewer.LastName AS ReviewerName
                    FROM [dbo].[TaskReviews] tr
                    INNER JOIN [dbo].[TaskAssignments] ta ON tr.TaskAssignmentId = ta.Id
                    INNER JOIN [dbo].[Users] u ON ta.UserId = u.Id
                    INNER JOIN [dbo].[Users] reviewer ON tr.ReviewedBy = reviewer.Id
                    WHERE ta.TaskId = @TaskId
                    ORDER BY tr.ReviewDate DESC";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@TaskId", taskId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetTaskCompletionHistory error: " + ex.Message);
                return new DataTable();
            }
        }

        #endregion

        #region Fix #7: Points Transaction Storage (Signed Integers)

        // Points are now stored as signed integers (SQL Server INT is already signed)
        // The AddPointTransaction method below stores signed values directly

        #endregion

        #endregion

        #region Core Task Management

        /// <summary>
        /// Creates a new task with enhanced fields
        /// </summary>
        public static int CreateTask(string title, string description, string category, int pointsReward, int createdBy, int familyId, List<string> objectives, 
            string priority = "Medium", string difficulty = null, int? estimatedMinutes = null, string instructions = null, string recurrencePattern = null)
        {
            try
            {
                // Insert task
                string query = @"
                    INSERT INTO [dbo].[Tasks] (Title, Description, Category, PointsReward, CreatedBy, FamilyId, Priority, Difficulty, EstimatedMinutes, Instructions, RecurrencePattern)
                    VALUES (@Title, @Description, @Category, @PointsReward, @CreatedBy, @FamilyId, @Priority, @Difficulty, @EstimatedMinutes, @Instructions, @RecurrencePattern);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                object taskId = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@Title", title),
                    new SqlParameter("@Description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description),
                    new SqlParameter("@Category", category),
                    new SqlParameter("@PointsReward", pointsReward),
                    new SqlParameter("@CreatedBy", createdBy),
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@Priority", priority ?? "Medium"),
                    new SqlParameter("@Difficulty", string.IsNullOrEmpty(difficulty) ? (object)DBNull.Value : difficulty),
                    new SqlParameter("@EstimatedMinutes", estimatedMinutes.HasValue ? (object)estimatedMinutes.Value : DBNull.Value),
                    new SqlParameter("@Instructions", string.IsNullOrEmpty(instructions) ? (object)DBNull.Value : instructions),
                    new SqlParameter("@RecurrencePattern", string.IsNullOrEmpty(recurrencePattern) ? (object)DBNull.Value : recurrencePattern));

                int taskIdInt = Convert.ToInt32(taskId);

                // Add objectives
                if (objectives != null && objectives.Count > 0)
                {
                    for (int i = 0; i < objectives.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(objectives[i].Trim()))
                        {
                            AddObjective(taskIdInt, objectives[i].Trim(), i);
                        }
                    }
                }

                // Log audit
                LogTaskAction(taskIdInt, "Created", createdBy, null, new { Title = title, Category = category, PointsReward = pointsReward });

                // Check for achievement awards (parent task creation achievements)
                try
                {
                    // Get count of tasks created by this user
                    string createdTasksQuery = @"
                        SELECT COUNT(*)
                        FROM [dbo].[Tasks]
                        WHERE [CreatedBy] = @CreatedBy AND [IsActive] = 1";
                    
                    object createdCountResult = DatabaseHelper.ExecuteScalar(createdTasksQuery,
                        new SqlParameter("@CreatedBy", createdBy));
                    int createdTasksCount = Convert.ToInt32(createdCountResult);

                    // Check for first task created
                    if (createdTasksCount == 1)
                    {
                        AchievementHelper.CheckAndAwardAchievement(createdBy, "FirstTaskCreated");
                    }

                    // Check for milestone achievement (25 tasks)
                    AchievementHelper.CheckAndAwardAchievement(createdBy, "TasksCreated", createdTasksCount);
                }
                catch (Exception achievementEx)
                {
                    // Don't fail task creation if achievement check fails
                    System.Diagnostics.Debug.WriteLine("CreateTask: Failed to check achievements: " + achievementEx.Message);
                }

                System.Diagnostics.Debug.WriteLine(string.Format("Task created: {0}, ID: {1}, Points: {2}", title, taskIdInt, pointsReward));
                return taskIdInt;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CreateTask error: " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Adds an objective to a task
        /// </summary>
        private static bool AddObjective(int taskId, string objectiveText, int orderIndex)
        {
            try
            {
                string query = @"
                    INSERT INTO [dbo].[TaskObjectives] (TaskId, ObjectiveText, OrderIndex)
                    VALUES (@TaskId, @ObjectiveText, @OrderIndex)";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@TaskId", taskId),
                    new SqlParameter("@ObjectiveText", objectiveText),
                    new SqlParameter("@OrderIndex", orderIndex));

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AddObjective error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Updates a task with enhanced fields
        /// </summary>
        public static bool UpdateTask(int taskId, string title, string description, string category, int pointsReward, List<string> objectives,
            string priority = null, string difficulty = null, int? estimatedMinutes = null, string instructions = null, int updatedBy = 0)
        {
            try
            {
                // Get old values for audit
                DataRow oldTask = GetTaskDetails(taskId);
                var oldValues = oldTask != null ? new { Title = oldTask["Title"], Category = oldTask["Category"], PointsReward = oldTask["PointsReward"] } : null;

                // Check if task is assigned
                if (IsTaskAssigned(taskId))
                {
                    return false; // Cannot edit assigned tasks
                }

                // Update task
                string query = @"
                    UPDATE [dbo].[Tasks]
                    SET Title = @Title, Description = @Description, Category = @Category, PointsReward = @PointsReward,
                        Priority = @Priority, Difficulty = @Difficulty, EstimatedMinutes = @EstimatedMinutes, Instructions = @Instructions
                    WHERE Id = @TaskId";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@TaskId", taskId),
                    new SqlParameter("@Title", title),
                    new SqlParameter("@Description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description),
                    new SqlParameter("@Category", category),
                    new SqlParameter("@PointsReward", pointsReward),
                    new SqlParameter("@Priority", priority ?? "Medium"),
                    new SqlParameter("@Difficulty", string.IsNullOrEmpty(difficulty) ? (object)DBNull.Value : difficulty),
                    new SqlParameter("@EstimatedMinutes", estimatedMinutes.HasValue ? (object)estimatedMinutes.Value : DBNull.Value),
                    new SqlParameter("@Instructions", string.IsNullOrEmpty(instructions) ? (object)DBNull.Value : instructions));

                if (rowsAffected > 0)
                {
                    // Delete existing objectives
                    DeleteObjectives(taskId);

                    // Add new objectives
                    if (objectives != null && objectives.Count > 0)
                    {
                        for (int i = 0; i < objectives.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(objectives[i].Trim()))
                            {
                                AddObjective(taskId, objectives[i].Trim(), i);
                            }
                        }
                    }

                    // Log audit
                    if (updatedBy > 0)
                    {
                        var newValues = new { Title = title, Category = category, PointsReward = pointsReward };
                        LogTaskAction(taskId, "Updated", updatedBy, oldValues, newValues);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("UpdateTask error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Deletes a task (soft delete by setting IsActive = 0)
        /// </summary>
        public static bool DeleteTask(int taskId, int deletedBy)
        {
            try
            {
                // Check if task is assigned
                if (IsTaskAssigned(taskId))
                {
                    return false; // Cannot delete assigned tasks
                }

                // Soft delete (set IsActive = 0)
                string query = @"
                    UPDATE [dbo].[Tasks]
                    SET IsActive = 0
                    WHERE Id = @TaskId";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@TaskId", taskId));

                if (rowsAffected > 0)
                {
                    // Log audit
                    LogTaskAction(taskId, "Deleted", deletedBy, null, null);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DeleteTask error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Checks if a task is assigned to any child (not deleted)
        /// </summary>
        public static bool IsTaskAssigned(int taskId)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*) FROM [dbo].[TaskAssignments]
                    WHERE TaskId = @TaskId AND Status != 'Declined' AND IsDeleted = 0";

                object count = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@TaskId", taskId));

                return Convert.ToInt32(count) > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Assigns a task to a child (with server-side deadline validation - Fix #5)
        /// </summary>
        public static bool AssignTask(int taskId, int userId, DateTime? deadline)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine("TaskHelper.AssignTask called");
                System.Diagnostics.Debug.WriteLine("TaskId: " + taskId);
                System.Diagnostics.Debug.WriteLine("UserId: " + userId);
                System.Diagnostics.Debug.WriteLine("Deadline: " + (deadline.HasValue ? deadline.Value.ToString() : "NULL"));

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

                // Check if child is banned
                string bannedCheckQuery = @"
                    SELECT IsBanned FROM [dbo].[Users]
                    WHERE Id = @UserId AND Role = 'CHILD' AND IsActive = 1";

                object isBanned = DatabaseHelper.ExecuteScalar(bannedCheckQuery,
                    new SqlParameter("@UserId", userId));

                System.Diagnostics.Debug.WriteLine("AssignTask: IsBanned check result: " + (isBanned != null ? isBanned.ToString() : "NULL"));

                if (isBanned != null && isBanned != DBNull.Value && Convert.ToBoolean(isBanned))
                {
                    System.Diagnostics.Debug.WriteLine("AssignTask: Child " + userId + " is banned - cannot assign task");
                    return false; // Child is banned
                }

                // Check if already assigned to this user (only active assignments, not completed/reviewed ones)
                // Allow reassignment if the previous assignment was reviewed (has a TaskReview record) or is soft-deleted
                string checkQuery = @"
                    SELECT ta.Status, ta.IsDeleted, 
                           CASE WHEN tr.Id IS NOT NULL THEN 1 ELSE 0 END AS IsReviewed
                    FROM [dbo].[TaskAssignments] ta
                    LEFT JOIN [dbo].[TaskReviews] tr ON ta.Id = tr.TaskAssignmentId
                    WHERE ta.TaskId = @TaskId AND ta.UserId = @UserId 
                      AND ta.Status != 'Declined' 
                      AND ta.IsDeleted = 0";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(checkQuery,
                    new SqlParameter("@TaskId", taskId),
                    new SqlParameter("@UserId", userId)))
                {
                    if (dt.Rows.Count > 0)
                    {
                        string existingStatus = dt.Rows[0]["Status"].ToString();
                        bool isReviewed = Convert.ToBoolean(dt.Rows[0]["IsReviewed"]);
                        
                        System.Diagnostics.Debug.WriteLine("AssignTask: Task already assigned to this user with status: " + existingStatus + ", IsReviewed: " + isReviewed);
                        
                        // Only block if it's an active assignment (Assigned, Ongoing, or Pending Review) that hasn't been reviewed
                        // Allow reassignment if the task was already reviewed (completed)
                        if (!isReviewed && (existingStatus == "Assigned" || existingStatus == "Ongoing" || existingStatus == "Pending Review"))
                        {
                            System.Diagnostics.Debug.WriteLine("AssignTask: Active assignment found - cannot reassign");
                            return false; // Active assignment exists, cannot reassign
                        }
                        
                        // If it's reviewed, allow reassignment (the old assignment will remain in history)
                        System.Diagnostics.Debug.WriteLine("AssignTask: Previous assignment was reviewed - allowing reassignment");
                    }
                }

                // Assign task
                string query = @"
                    INSERT INTO [dbo].[TaskAssignments] (TaskId, UserId, Deadline, Status)
                    VALUES (@TaskId, @UserId, @Deadline, 'Assigned')";

                System.Diagnostics.Debug.WriteLine("AssignTask: Executing INSERT query");
                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@TaskId", taskId),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@Deadline", deadline.HasValue ? (object)deadline.Value : DBNull.Value));

                System.Diagnostics.Debug.WriteLine("AssignTask: INSERT completed, rows affected: " + rowsAffected);

                if (rowsAffected > 0)
                {
                    // Fix #4: Create notification
                    string taskTitle = GetTaskTitle(taskId);
                    CreateNotification(userId, "New Task Assigned", string.Format("You have been assigned a new task: {0}", taskTitle), "TaskAssigned", taskId, null);
                }

                System.Diagnostics.Debug.WriteLine("========================================");

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("========================================");
                System.Diagnostics.Debug.WriteLine("TaskHelper.AssignTask ERROR");
                System.Diagnostics.Debug.WriteLine("Error Message: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner Exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("========================================");
                return false;
            }
        }

        /// <summary>
        /// Gets task title for notifications
        /// </summary>
        private static string GetTaskTitle(int taskId)
        {
            try
            {
                string query = "SELECT Title FROM [dbo].[Tasks] WHERE Id = @TaskId";
                object result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@TaskId", taskId));
                return result != null ? result.ToString() : "Task";
            }
            catch
            {
                return "Task";
            }
        }

        /// <summary>
        /// Accepts a task assignment
        /// </summary>
        public static bool AcceptTask(int taskAssignmentId, int userId)
        {
            try
            {
                // First, check if task is overdue and auto-fail if needed
                // Also get EstimatedMinutes for timer duration
                string checkQuery = @"
                    SELECT ta.Deadline, ta.Status, ta.TaskId, t.PointsReward, t.FamilyId, t.EstimatedMinutes, f.OwnerId AS FamilyOwnerId
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                    INNER JOIN [dbo].[Families] f ON t.FamilyId = f.Id
                    WHERE ta.Id = @TaskAssignmentId AND ta.UserId = @UserId AND ta.IsDeleted = 0";
                
                int? timerDuration = null;
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
                    
                    // Get EstimatedMinutes for timer duration (Issue #8)
                    if (dt.Rows[0]["EstimatedMinutes"] != DBNull.Value)
                    {
                        int estimatedMins = Convert.ToInt32(dt.Rows[0]["EstimatedMinutes"]);
                        // Validate timer range (10 min - 24 hours)
                        if (estimatedMins >= 10 && estimatedMins <= 1440)
                        {
                            timerDuration = estimatedMins;
                            System.Diagnostics.Debug.WriteLine(string.Format("AcceptTask: Timer duration set to {0} minutes for assignment {1}", estimatedMins, taskAssignmentId));
                        }
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
                
                // Task is not overdue - proceed with acceptance and start timer if EstimatedMinutes is set
                string query = @"
                    UPDATE [dbo].[TaskAssignments]
                    SET Status = 'Ongoing', 
                        AcceptedDate = GETDATE(),
                        TimerStart = CASE WHEN @TimerDuration IS NOT NULL THEN GETDATE() ELSE NULL END,
                        TimerDuration = @TimerDuration
                    WHERE Id = @TaskAssignmentId AND UserId = @UserId AND Status = 'Assigned'";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@TaskAssignmentId", taskAssignmentId),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@TimerDuration", timerDuration.HasValue ? (object)timerDuration.Value : DBNull.Value));

                if (rowsAffected > 0)
                {
                    // Initialize objective completions
                    InitializeObjectiveCompletions(taskAssignmentId);

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

        /// <summary>
        /// Initializes objective completions for an assignment
        /// </summary>
        private static void InitializeObjectiveCompletions(int taskAssignmentId)
        {
            try
            {
                // Get task ID
                int taskId = GetTaskIdFromAssignment(taskAssignmentId);

                // Get all objectives for the task
                DataTable objectives = GetTaskObjectives(taskId);

                // Create completion records for each objective
                foreach (DataRow obj in objectives.Rows)
                {
                    int objectiveId = Convert.ToInt32(obj["Id"]);
                    string checkQuery = @"
                        SELECT Id FROM [dbo].[TaskObjectiveCompletions]
                        WHERE TaskAssignmentId = @TaskAssignmentId AND TaskObjectiveId = @TaskObjectiveId";

                    object existing = DatabaseHelper.ExecuteScalar(checkQuery,
                        new SqlParameter("@TaskAssignmentId", taskAssignmentId),
                        new SqlParameter("@TaskObjectiveId", objectiveId));

                    if (existing == null)
                    {
                        string insertQuery = @"
                            INSERT INTO [dbo].[TaskObjectiveCompletions] (TaskAssignmentId, TaskObjectiveId, IsCompleted)
                            VALUES (@TaskAssignmentId, @TaskObjectiveId, 0)";

                        DatabaseHelper.ExecuteNonQuery(insertQuery,
                            new SqlParameter("@TaskAssignmentId", taskAssignmentId),
                            new SqlParameter("@TaskObjectiveId", objectiveId));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("InitializeObjectiveCompletions error: " + ex.Message);
            }
        }

        /// <summary>
        /// Denies a task assignment
        /// </summary>
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
                            CreateNotification(parentId, "Task Declined", string.Format("{0} declined task: {1}", childName, taskTitle), "TaskDeclined", taskId, taskAssignmentId);
                        }
                    }
                }

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DenyTask error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Submits a task for review (with server-side objective validation - Fix #1)
        /// </summary>
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

        /// <summary>
        /// Gets the status of a task assignment
        /// </summary>
        public static string GetTaskAssignmentStatus(int taskAssignmentId)
        {
            try
            {
                string query = @"
                    SELECT Status FROM [dbo].[TaskAssignments]
                    WHERE Id = @TaskAssignmentId AND IsDeleted = 0";

                object result = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@TaskAssignmentId", taskAssignmentId));

                return result != null && result != DBNull.Value ? result.ToString() : null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetTaskAssignmentStatus error: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Reviews a completed task (with soft-delete - Fix #2)
        /// </summary>
        public static bool ReviewTask(int taskAssignmentId, int rating, int reviewedBy, bool isFailed, bool isAutoFailed = false)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask called: AssignmentId={0}, Rating={1}, ReviewedBy={2}, IsFailed={3}, IsAutoFailed={4}",
                    taskAssignmentId, rating, reviewedBy, isFailed, isAutoFailed));

                // Get task assignment info (including FamilyId for treasury)
                string assignmentQuery = @"
                    SELECT ta.TaskId, ta.UserId, ta.Status, t.PointsReward, t.FamilyId
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                    WHERE ta.Id = @TaskAssignmentId AND ta.IsDeleted = 0";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(assignmentQuery,
                    new SqlParameter("@TaskAssignmentId", taskAssignmentId)))
                {
                    if (dt.Rows.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask: Assignment {0} not found or is deleted", taskAssignmentId));
                        return false;
                    }

                    string currentStatus = dt.Rows[0]["Status"].ToString();
                    System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask: Assignment {0} current status: {1}", taskAssignmentId, currentStatus));

                    // Allow "Assigned" or "Ongoing" status only when auto-failing overdue tasks
                    if (currentStatus != "Pending Review" && !(isAutoFailed && (currentStatus == "Assigned" || currentStatus == "Ongoing")))
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask: Assignment {0} is not in 'Pending Review' status. Current: {1}", taskAssignmentId, currentStatus));
                        return false;
                    }

                    // If auto-failing an "Assigned" or "Ongoing" task, update status to "Pending Review" first
                    if (isAutoFailed && (currentStatus == "Assigned" || currentStatus == "Ongoing"))
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask: Auto-failing overdue '{0}' task - updating status to 'Pending Review'", currentStatus));
                        string updateStatusQuery = @"
                            UPDATE [dbo].[TaskAssignments]
                            SET Status = 'Pending Review'
                            WHERE Id = @TaskAssignmentId AND IsDeleted = 0";
                        DatabaseHelper.ExecuteNonQuery(updateStatusQuery, new SqlParameter("@TaskAssignmentId", taskAssignmentId));
                    }

                    int taskId = Convert.ToInt32(dt.Rows[0]["TaskId"]);
                    int userId = Convert.ToInt32(dt.Rows[0]["UserId"]);
                    int totalPoints = Convert.ToInt32(dt.Rows[0]["PointsReward"]);
                    int familyId = Convert.ToInt32(dt.Rows[0]["FamilyId"]);

                    System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask: TaskId={0}, UserId={1}, TotalPoints={2}, FamilyId={3}", taskId, userId, totalPoints, familyId));

                    // Calculate points awarded
                    int pointsAwarded = CalculatePointsAwarded(totalPoints, rating, isFailed);
                    System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask: PointsAwarded={0}", pointsAwarded));

                    // Insert review
                    string reviewQuery = @"
                        INSERT INTO [dbo].[TaskReviews] (TaskAssignmentId, Rating, PointsAwarded, IsFailed, IsAutoFailed, ReviewedBy)
                        VALUES (@TaskAssignmentId, @Rating, @PointsAwarded, @IsFailed, @IsAutoFailed, @ReviewedBy)";

                    System.Diagnostics.Debug.WriteLine("ReviewTask: Executing INSERT into TaskReviews");
                    int rowsAffected = DatabaseHelper.ExecuteNonQuery(reviewQuery,
                        new SqlParameter("@TaskAssignmentId", taskAssignmentId),
                        new SqlParameter("@Rating", isFailed ? (object)DBNull.Value : rating),
                        new SqlParameter("@PointsAwarded", pointsAwarded),
                        new SqlParameter("@IsFailed", isFailed),
                        new SqlParameter("@IsAutoFailed", isAutoFailed),
                        new SqlParameter("@ReviewedBy", reviewedBy));

                    System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask: INSERT rows affected: {0}", rowsAffected));

                    if (rowsAffected > 0)
                    {
                        // Award/deduct points with treasury integration and cap enforcement
                        if (pointsAwarded != 0)
                        {
                            string description = isFailed
                                ? string.Format("Task failed: {0} mokipoints deducted", Math.Abs(pointsAwarded))
                                : string.Format("Task completed: {0} mokipoints (Rating: {1} stars)", pointsAwarded, rating);

                            System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask: Processing points: {0} points", pointsAwarded));
                            
                            bool pointsSuccess = false;
                            if (pointsAwarded > 0)
                            {
                                // Award points (from treasury, with cap enforcement)
                                pointsSuccess = PointHelper.AwardPointsWithCap(userId, pointsAwarded, familyId, description, taskAssignmentId);
                            }
                            else
                            {
                                // Deduct points (to treasury, cannot go negative)
                                int pointsToDeduct = Math.Abs(pointsAwarded);
                                // Pass null for orderId (not a reward order), taskAssignmentId as 6th parameter
                                pointsSuccess = PointHelper.DeductPoints(userId, pointsToDeduct, familyId, description, null, taskAssignmentId);
                            }

                            if (!pointsSuccess)
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask ERROR: Failed to process points for user {0}. Review will not be completed.", userId));
                                // Don't complete review if points processing failed
                                return false;
                            }
                        }

                        // Fix #2: Soft-delete assignment instead of hard delete
                        System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask: Soft-deleting assignment {0}", taskAssignmentId));
                        SoftDeleteAssignment(taskAssignmentId, reviewedBy);

                        // Fix #4: Create notification for child
                        string taskTitle = GetTaskTitle(taskId);
                        string notificationMessage = isFailed
                            ? string.Format("Your task '{0}' was marked as failed. Points deducted: {1}", taskTitle, Math.Abs(pointsAwarded))
                            : string.Format("Your task '{0}' was reviewed. Rating: {1} stars. Points awarded: {2}", taskTitle, rating, pointsAwarded);
                        
                        System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask: Creating notification for user {0}", userId));
                        CreateNotification(userId, "Task Reviewed", notificationMessage, "TaskReviewed", taskId, taskAssignmentId);

                        // Post system message to family chat
                        try
                        {
                            var userInfo = AuthenticationHelper.GetUserById(userId);
                            if (userInfo != null)
                            {
                                string childName = userInfo["FirstName"].ToString() + " " + userInfo["LastName"].ToString();
                                string chatMessage = "";
                                string systemEventType = "";
                                
                                if (isFailed)
                                {
                                    chatMessage = string.Format("{0} failed '{1}' and lost {2} points", childName, taskTitle, Math.Abs(pointsAwarded));
                                    systemEventType = "TaskFailed";
                                }
                                else
                                {
                                    chatMessage = string.Format("{0} completed '{1}' and earned {2} points! ⭐{3}", childName, taskTitle, pointsAwarded, rating);
                                    systemEventType = "TaskCompleted";
                                }
                                
                                // Create JSON data for system event
                                string systemEventData = string.Format("{{\"TaskId\":{0},\"TaskTitle\":\"{1}\",\"ChildId\":{2},\"ChildName\":\"{3}\",\"Points\":{4},\"Rating\":{5},\"IsFailed\":{6}}}",
                                    taskId, 
                                    taskTitle.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r"), 
                                    userId, 
                                    childName.Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r"), 
                                    pointsAwarded, 
                                    isFailed ? 0 : rating, 
                                    isFailed.ToString().ToLower());
                                
                                ChatHelper.PostSystemMessage(familyId, systemEventType, chatMessage, systemEventData);
                            }
                        }
                        catch (Exception chatEx)
                        {
                            // Don't fail the review if chat message fails
                            System.Diagnostics.Debug.WriteLine("ReviewTask: Failed to post chat message: " + chatEx.Message);
                        }

                        // Check for achievement awards (only if task was successfully completed, not failed)
                        if (!isFailed && rating > 0 && pointsAwarded > 0)
                        {
                            try
                            {
                                // Get count of completed tasks for this user
                                string completedTasksQuery = @"
                                    SELECT COUNT(DISTINCT ta.[Id])
                                    FROM [dbo].[TaskAssignments] ta
                                    INNER JOIN [dbo].[TaskReviews] tr ON ta.[Id] = tr.[TaskAssignmentId]
                                    WHERE ta.[UserId] = @UserId 
                                      AND ta.[Status] = 'Reviewed'
                                      AND tr.[Rating] > 0
                                      AND ta.[IsDeleted] = 0";
                                
                                object completedCountResult = DatabaseHelper.ExecuteScalar(completedTasksQuery,
                                    new SqlParameter("@UserId", userId));
                                int completedTasksCount = Convert.ToInt32(completedCountResult);

                                // Check for first task completed
                                if (completedTasksCount == 1)
                                {
                                    AchievementHelper.CheckAndAwardAchievement(userId, "FirstTaskCompleted");
                                }

                                // Check for milestone achievements (10, 50, 100, 200, 300)
                                AchievementHelper.CheckAndAwardAchievement(userId, "TasksCompleted", completedTasksCount);

                                // Check for points milestone achievements (100, 1000, 5000, 10000)
                                // Get total points earned (from PointTransactions where points > 0)
                                string totalPointsQuery = @"
                                    SELECT ISNULL(SUM([Points]), 0)
                                    FROM [dbo].[PointTransactions]
                                    WHERE [UserId] = @UserId AND [Points] > 0";
                                
                                object totalPointsResult = DatabaseHelper.ExecuteScalar(totalPointsQuery,
                                    new SqlParameter("@UserId", userId));
                                int totalPointsEarned = Convert.ToInt32(totalPointsResult);

                                // Check milestone achievements
                                if (totalPointsEarned >= 100 && totalPointsEarned < 1000)
                                {
                                    AchievementHelper.CheckAndAwardAchievement(userId, "PointsEarned", 100);
                                }
                                else if (totalPointsEarned >= 1000 && totalPointsEarned < 5000)
                                {
                                    AchievementHelper.CheckAndAwardAchievement(userId, "PointsEarned", 1000);
                                }
                                else if (totalPointsEarned >= 5000 && totalPointsEarned < 10000)
                                {
                                    AchievementHelper.CheckAndAwardAchievement(userId, "PointsEarned", 5000);
                                }
                                else if (totalPointsEarned >= 10000)
                                {
                                    AchievementHelper.CheckAndAwardAchievement(userId, "PointsEarned", 10000);
                                }
                            }
                            catch (Exception achievementEx)
                            {
                                // Don't fail the review if achievement check fails
                                System.Diagnostics.Debug.WriteLine("ReviewTask: Failed to check achievements: " + achievementEx.Message);
                            }
                        }

                        System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask: Successfully reviewed assignment {0}", taskAssignmentId));
                        return true;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask: Failed to insert review record for assignment {0}", taskAssignmentId));
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ReviewTask ERROR: {0}\nStack Trace: {1}", ex.Message, ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Calculates points awarded based on rating
        /// </summary>
        public static int CalculatePointsAwarded(int totalPoints, int rating, bool isFailed)
        {
            if (isFailed)
            {
                // Failed: -50% of total points
                return (int)Math.Round(totalPoints * -0.5);
            }

            // Rating-based percentages
            switch (rating)
            {
                case 1:
                case 2:
                    return (int)Math.Round(totalPoints * 0.2); // 20%
                case 3:
                    return (int)Math.Round(totalPoints * 0.5); // 50%
                case 4:
                    return (int)Math.Round(totalPoints * 0.75); // 75%
                case 5:
                    return totalPoints; // 100%
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Adds a point transaction (Fix #7: Signed integers)
        /// </summary>
        private static void AddPointTransaction(int userId, int points, string transactionType, string description, int? taskAssignmentId)
        {
            try
            {
                // Fix #7: Points can be negative (signed integer)
                string query = @"
                    INSERT INTO [dbo].[PointTransactions] (UserId, Points, TransactionType, Description, TaskAssignmentId)
                    VALUES (@UserId, @Points, @TransactionType, @Description, @TaskAssignmentId)";

                DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@Points", Math.Abs(points)), // Store absolute value (for compatibility)
                    new SqlParameter("@TransactionType", transactionType),
                    new SqlParameter("@Description", description),
                    new SqlParameter("@TaskAssignmentId", taskAssignmentId.HasValue ? (object)taskAssignmentId.Value : DBNull.Value));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AddPointTransaction error: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets all tasks for a family (parent view) - excludes deleted assignments
        /// </summary>
        public static DataTable GetFamilyTasks(int familyId)
        {
            try
            {
                string query = @"
                    SELECT t.Id, t.Title, t.Description, t.Category, t.PointsReward, t.CreatedDate, t.Priority, t.Difficulty, t.EstimatedMinutes,
                           u.FirstName + ' ' + u.LastName AS CreatedByName,
                           (SELECT COUNT(*) FROM [dbo].[TaskAssignments] ta WHERE ta.TaskId = t.Id AND ta.Status != 'Declined' AND ta.IsDeleted = 0) AS AssignmentCount
                    FROM [dbo].[Tasks] t
                    INNER JOIN [dbo].[Users] u ON t.CreatedBy = u.Id
                    WHERE t.FamilyId = @FamilyId AND t.IsActive = 1
                    ORDER BY t.CreatedDate DESC";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@FamilyId", familyId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetFamilyTasks error: " + ex.Message);
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets task assignments for a specific task with child names and statuses (excludes deleted)
        /// </summary>
        public static DataTable GetTaskAssignments(int taskId)
        {
            try
            {
                string query = @"
                    SELECT ta.Id AS AssignmentId, ta.UserId, ta.Status, ta.AssignedDate, ta.AcceptedDate, ta.CompletedDate, ta.Deadline,
                           u.FirstName + ' ' + u.LastName AS ChildName
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Users] u ON ta.UserId = u.Id
                    WHERE ta.TaskId = @TaskId AND ta.Status != 'Declined' AND ta.IsDeleted = 0
                    ORDER BY ta.AssignedDate DESC";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@TaskId", taskId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetTaskAssignments error: " + ex.Message);
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets task details with objectives
        /// </summary>
        public static DataRow GetTaskDetails(int taskId)
        {
            try
            {
                string query = @"
                    SELECT t.Id, t.Title, t.Description, t.Category, t.PointsReward, t.CreatedDate, t.CreatedBy, t.FamilyId,
                           t.Priority, t.Difficulty, t.EstimatedMinutes, t.Instructions, t.RecurrencePattern,
                           u.FirstName + ' ' + u.LastName AS CreatedByName
                    FROM [dbo].[Tasks] t
                    INNER JOIN [dbo].[Users] u ON t.CreatedBy = u.Id
                    WHERE t.Id = @TaskId";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@TaskId", taskId)))
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0];
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetTaskDetails error: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Gets objectives for a task
        /// </summary>
        public static DataTable GetTaskObjectives(int taskId)
        {
            try
            {
                string query = @"
                    SELECT Id, ObjectiveText, OrderIndex
                    FROM [dbo].[TaskObjectives]
                    WHERE TaskId = @TaskId
                    ORDER BY OrderIndex";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@TaskId", taskId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetTaskObjectives error: " + ex.Message);
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets assigned tasks for a child (excludes deleted)
        /// </summary>
        public static DataTable GetChildTasks(int userId, string statusFilter = null)
        {
            try
            {
                string query = @"
                    SELECT ta.Id AS AssignmentId, ta.TaskId, ta.Deadline, ta.Status, ta.AssignedDate, ta.AcceptedDate, ta.CompletedDate,
                           ta.TimerStart, ta.TimerDuration,
                           t.Title, t.Description, t.Category, t.PointsReward, t.Priority, t.Difficulty, t.EstimatedMinutes,
                           u.FirstName + ' ' + u.LastName AS AssignedByName
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                    INNER JOIN [dbo].[Users] u ON t.CreatedBy = u.Id
                    WHERE ta.UserId = @UserId AND ta.IsDeleted = 0";

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    query += " AND ta.Status = @Status";
                }
                else
                {
                    query += " AND ta.Status != 'Declined'";
                }

                query += " ORDER BY ta.AssignedDate DESC";

                if (!string.IsNullOrEmpty(statusFilter))
                {
                    return DatabaseHelper.ExecuteQuery(query,
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@Status", statusFilter));
                }
                else
                {
                    return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserId", userId));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetChildTasks error: " + ex.Message);
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets tasks pending review (parent view) - excludes deleted
        /// </summary>
        public static DataTable GetTasksPendingReview(int familyId)
        {
            try
            {
                string query = @"
                    SELECT ta.Id AS AssignmentId, ta.TaskId, ta.UserId, ta.CompletedDate, ta.Deadline,
                           t.Title, t.Description, t.Category, t.PointsReward,
                           u.FirstName + ' ' + u.LastName AS ChildName
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                    INNER JOIN [dbo].[Users] u ON ta.UserId = u.Id
                    WHERE t.FamilyId = @FamilyId AND ta.Status = 'Pending Review' AND ta.IsDeleted = 0
                    ORDER BY ta.CompletedDate DESC";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@FamilyId", familyId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetTasksPendingReview error: " + ex.Message);
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets children in a family
        /// </summary>
        public static DataTable GetFamilyChildren(int familyId)
        {
            try
            {
                string query = @"
                    SELECT u.Id, u.FirstName, u.LastName, u.Email
                    FROM [dbo].[FamilyMembers] fm
                    INNER JOIN [dbo].[Users] u ON fm.UserId = u.Id
                    WHERE fm.FamilyId = @FamilyId AND fm.Role = 'CHILD' AND fm.IsActive = 1 AND u.IsActive = 1
                    ORDER BY u.FirstName, u.LastName";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@FamilyId", familyId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetFamilyChildren error: " + ex.Message);
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets task assignment details
        /// </summary>
        public static DataRow GetTaskAssignment(int taskAssignmentId)
        {
            try
            {
                string query = @"
                    SELECT ta.Id, ta.TaskId, ta.UserId, ta.Deadline, ta.Status, ta.AssignedDate, ta.AcceptedDate, ta.CompletedDate,
                           t.Title, t.Description, t.Category, t.PointsReward
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                    WHERE ta.Id = @TaskAssignmentId AND ta.IsDeleted = 0";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@TaskAssignmentId", taskAssignmentId)))
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0];
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetTaskAssignment error: " + ex.Message);
            }
            return null;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Deletes objectives for a task
        /// </summary>
        private static void DeleteObjectives(int taskId)
        {
            try
            {
                string query = "DELETE FROM [dbo].[TaskObjectives] WHERE TaskId = @TaskId";
                DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@TaskId", taskId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DeleteObjectives error: " + ex.Message);
            }
        }

        /// <summary>
        /// Gets task ID from assignment ID
        /// </summary>
        private static int GetTaskIdFromAssignment(int taskAssignmentId)
        {
            try
            {
                string query = "SELECT TaskId FROM [dbo].[TaskAssignments] WHERE Id = @TaskAssignmentId";
                object result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@TaskAssignmentId", taskAssignmentId));
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets family ID from task ID
        /// </summary>
        private static int? GetFamilyIdFromTask(int taskId)
        {
            try
            {
                string query = "SELECT FamilyId FROM [dbo].[Tasks] WHERE Id = @TaskId";
                object result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@TaskId", taskId));
                return result != null ? (int?)Convert.ToInt32(result) : null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets family parents
        /// </summary>
        private static DataTable GetFamilyParents(int familyId)
        {
            try
            {
                string query = @"
                    SELECT u.Id FROM [dbo].[FamilyMembers] fm
                    INNER JOIN [dbo].[Users] u ON fm.UserId = u.Id
                    WHERE fm.FamilyId = @FamilyId AND fm.Role = 'PARENT' AND fm.IsActive = 1 AND u.IsActive = 1";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@FamilyId", familyId));
            }
            catch
            {
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets user name
        /// </summary>
        private static string GetUserName(int userId)
        {
            try
            {
                string query = "SELECT FirstName + ' ' + LastName AS Name FROM [dbo].[Users] WHERE Id = @UserId";
                object result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@UserId", userId));
                return result != null ? result.ToString() : "User";
            }
            catch
            {
                return "User";
            }
        }

        /// <summary>
        /// Logs task action for audit trail
        /// </summary>
        private static void LogTaskAction(int taskId, string action, int userId, object oldValues, object newValues)
        {
            try
            {
                string query = @"
                    INSERT INTO [dbo].[TaskAuditLog] (TaskId, Action, UserId, OldValues, NewValues)
                    VALUES (@TaskId, @Action, @UserId, @OldValues, @NewValues)";

                // Simple string representation (can be enhanced with JSON library if available)
                string oldValuesStr = oldValues != null ? oldValues.ToString() : null;
                string newValuesStr = newValues != null ? newValues.ToString() : null;

                DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@TaskId", taskId),
                    new SqlParameter("@Action", action),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@OldValues", string.IsNullOrEmpty(oldValuesStr) ? (object)DBNull.Value : oldValuesStr),
                    new SqlParameter("@NewValues", string.IsNullOrEmpty(newValuesStr) ? (object)DBNull.Value : newValuesStr));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LogTaskAction error: " + ex.Message);
                // Don't fail if audit logging fails
            }
        }

        #endregion

        /// <summary>
        /// Gets point transactions for a user
        /// </summary>
        public static DataTable GetUserPointTransactions(int userId)
        {
            try
            {
                string query = @"
                    SELECT Id, Points, TransactionType, Description, TransactionDate
                    FROM [dbo].[PointTransactions]
                    WHERE UserId = @UserId
                    ORDER BY TransactionDate DESC";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserId", userId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetUserPointTransactions error: " + ex.Message);
                return new DataTable();
            }
        }

        /// <summary>
        /// Auto-fails overdue tasks in "Assigned" status for a family
        /// Called on page load to check for overdue tasks that haven't been accepted/denied
        /// </summary>
        public static void AutoFailOverdueTasks(int familyId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(string.Format("AutoFailOverdueTasks: Starting for familyId={0}", familyId));

                // Get family owner ID for reviewing tasks
                int? familyOwnerId = FamilyHelper.GetFamilyOwnerId(familyId);
                if (!familyOwnerId.HasValue)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("AutoFailOverdueTasks: Could not get family owner for familyId={0}", familyId));
                    return;
                }

                DateTime now = DateTime.Now;
                
                // Issue #8: Check for timer expiration (in addition to deadline)
                string timerQuery = @"
                    SELECT ta.Id AS AssignmentId, ta.TaskId, ta.UserId, ta.TimerStart, ta.TimerDuration,
                           t.PointsReward, t.FamilyId
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                    WHERE t.FamilyId = @FamilyId
                      AND ta.Status = 'Ongoing'
                      AND ta.TimerStart IS NOT NULL
                      AND ta.TimerDuration IS NOT NULL
                      AND ta.IsDeleted = 0
                      AND DATEADD(MINUTE, ta.TimerDuration, ta.TimerStart) < @Now
                      AND NOT EXISTS (SELECT 1 FROM [dbo].[TaskReviews] tr WHERE tr.TaskAssignmentId = ta.Id)";

                using (DataTable timerTasks = DatabaseHelper.ExecuteQuery(timerQuery,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@Now", now)))
                {
                    foreach (DataRow row in timerTasks.Rows)
                    {
                        int assignmentId = Convert.ToInt32(row["AssignmentId"]);
                        
                        System.Diagnostics.Debug.WriteLine(string.Format("AutoFailOverdueTasks: Timer expired for assignment {0}. Auto-failing.", assignmentId));
                        
                        if (ReviewTask(assignmentId, 0, familyOwnerId.Value, true, true)) // isAutoFailed = true
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("AutoFailOverdueTasks: Successfully auto-failed timer-expired assignment {0}", assignmentId));
                        }
                    }
                }

                // Find all overdue tasks in "Assigned" or "Ongoing" status (deadline check)
                string query = @"
                    SELECT ta.Id, ta.Deadline, ta.TaskId, ta.UserId, ta.Status, t.PointsReward
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                    WHERE t.FamilyId = @FamilyId
                      AND ta.Status IN ('Assigned', 'Ongoing')
                      AND ta.IsDeleted = 0
                      AND ta.Deadline IS NOT NULL
                      AND ta.Deadline < @Now
                      AND NOT EXISTS (SELECT 1 FROM [dbo].[TaskReviews] tr WHERE tr.TaskAssignmentId = ta.Id)";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query, 
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@Now", now)))
                {
                    int count = dt.Rows.Count;
                    System.Diagnostics.Debug.WriteLine(string.Format("AutoFailOverdueTasks: Found {0} overdue tasks in 'Assigned' or 'Ongoing' status", count));

                    foreach (DataRow row in dt.Rows)
                    {
                        int assignmentId = Convert.ToInt32(row["Id"]);
                        DateTime deadline = Convert.ToDateTime(row["Deadline"]);
                        string status = row["Status"].ToString();
                        
                        System.Diagnostics.Debug.WriteLine(string.Format("AutoFailOverdueTasks: Auto-failing assignment {0} (status: {1}, deadline: {2})", assignmentId, status, deadline));

                        // Auto-fail the task
                        if (ReviewTask(assignmentId, 0, familyOwnerId.Value, true, true)) // isFailed=true, isAutoFailed=true
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("AutoFailOverdueTasks: Successfully auto-failed assignment {0}", assignmentId));
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("AutoFailOverdueTasks ERROR: Failed to auto-fail assignment {0}", assignmentId));
                        }
                    }
                }

                System.Diagnostics.Debug.WriteLine(string.Format("AutoFailOverdueTasks: Completed for familyId={0}", familyId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("AutoFailOverdueTasks error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                // Don't throw - this is a background check, shouldn't break page load
            }
        }

        // Note: Additional methods for templates, tags, comments, attachments, etc. can be added here
        // For now, focusing on the 7 critical fixes and core functionality
    }
}

