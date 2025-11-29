using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace mokipointsCS
{
    /// <summary>
    /// Dashboard Helper class for dashboard statistics and metrics
    /// Provides methods for retrieving dashboard metrics for both parent and child dashboards
    /// </summary>
    public class DashboardHelper
    {
        #region Parent Dashboard Methods

        /// <summary>
        /// Gets current treasury balance for a family
        /// </summary>
        public static int GetTreasuryBalance(int familyId)
        {
            try
            {
                return TreasuryHelper.GetTreasuryBalance(familyId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetTreasuryBalance error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets count of tasks pending review for a family
        /// </summary>
        public static int GetPendingReviewsCount(int familyId)
        {
            try
            {
                DataTable tasks = TaskHelper.GetTasksPendingReview(familyId);
                return tasks != null ? tasks.Rows.Count : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetPendingReviewsCount error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets count of pending orders for a family
        /// </summary>
        public static int GetPendingOrdersCount(int familyId)
        {
            try
            {
                DataTable orders = RewardHelper.GetFamilyOrders(familyId);
                if (orders == null) return 0;

                int count = 0;
                foreach (DataRow row in orders.Rows)
                {
                    string status = row["Status"].ToString();
                    if (status == "Pending")
                    {
                        count++;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetPendingOrdersCount error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets count of active children in a family
        /// </summary>
        public static int GetActiveChildrenCount(int familyId)
        {
            try
            {
                return FamilyHelper.GetChildrenCount(familyId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetActiveChildrenCount error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets count of tasks completed today for a family
        /// </summary>
        public static int GetTasksCompletedToday(int familyId)
        {
            try
            {
                string query = @"
                    SELECT COUNT(DISTINCT ta.Id)
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                    WHERE t.FamilyId = @FamilyId
                      AND ta.Status = 'Pending Review'
                      AND CAST(ta.CompletedDate AS DATE) = CAST(GETDATE() AS DATE)
                      AND ta.IsDeleted = 0";

                object result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@FamilyId", familyId));
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetTasksCompletedToday error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets total points awarded this week for a family
        /// </summary>
        public static int GetPointsAwardedThisWeek(int familyId)
        {
            try
            {
                DateTime weekStart = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                string query = @"
                    SELECT ISNULL(SUM(tr.PointsAwarded), 0)
                    FROM [dbo].[TaskReviews] tr
                    INNER JOIN [dbo].[TaskAssignments] ta ON tr.TaskAssignmentId = ta.Id
                    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                    WHERE t.FamilyId = @FamilyId
                      AND tr.IsFailed = 0
                      AND CAST(tr.ReviewDate AS DATE) >= @WeekStart";

                object result = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@WeekStart", weekStart.Date));
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetPointsAwardedThisWeek error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets recent activity for a family
        /// </summary>
        public static DataTable GetRecentActivityForFamily(int familyId, int limit = 10)
        {
            try
            {
                // Validate limit to prevent issues
                if (limit < 1) limit = 10;
                if (limit > 100) limit = 100; // Cap at 100 for performance
                
                // SQL Server doesn't support TOP with parameter in UNION, so we use a subquery
                string query = string.Format(@"
                    SELECT TOP ({0})
                        ActivityType, ChildName, TaskTitle, ActivityDate, Points, OrderNumber
                    FROM (
                        SELECT 
                            'TaskCompleted' AS ActivityType,
                            u.FirstName + ' ' + u.LastName AS ChildName,
                            t.Title AS TaskTitle,
                            ta.CompletedDate AS ActivityDate,
                            t.PointsReward AS Points,
                            NULL AS OrderNumber
                        FROM [dbo].[TaskAssignments] ta
                        INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                        INNER JOIN [dbo].[Users] u ON ta.UserId = u.Id
                        WHERE t.FamilyId = @FamilyId
                          AND ta.Status = 'Pending Review'
                          AND ta.IsDeleted = 0
                        
                        UNION ALL
                        
                        SELECT 
                            'OrderConfirmed' AS ActivityType,
                            u.FirstName + ' ' + u.LastName AS ChildName,
                            NULL AS TaskTitle,
                            ro.ConfirmedDate AS ActivityDate,
                            ro.TotalPoints AS Points,
                            ro.OrderNumber AS OrderNumber
                        FROM [dbo].[RewardOrders] ro
                        INNER JOIN [dbo].[Users] u ON ro.ChildId = u.Id
                        WHERE ro.FamilyId = @FamilyId
                          AND ro.Status IN ('WaitingToFulfill', 'Fulfilled', 'TransactionComplete')
                    ) AS CombinedActivity
                    ORDER BY ActivityDate DESC", limit);

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@FamilyId", familyId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetRecentActivityForFamily error: {0}", ex.Message));
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets individual child task metrics (completed and failed tasks) for a specific time period
        /// </summary>
        /// <param name="familyId">Family ID</param>
        /// <param name="childId">Child user ID</param>
        /// <param name="period">Time period: "day", "week", or "month"</param>
        /// <returns>Dictionary with labels (dates) and datasets (completed/failed counts)</returns>
        public static Dictionary<string, object> GetChildTaskMetrics(int familyId, int childId, string period)
        {
            try
            {
                DateTime startDate;
                string dateFormat;
                
                switch (period.ToLower())
                {
                    case "day":
                        // Last 7 days
                        startDate = DateTime.Now.AddDays(-7).Date;
                        dateFormat = "MMM dd";
                        break;
                    case "week":
                        // Last 4 weeks
                        startDate = DateTime.Now.AddDays(-28).Date;
                        dateFormat = "MMM dd";
                        break;
                    case "month":
                        // Last 6 months
                        startDate = DateTime.Now.AddMonths(-6).Date;
                        dateFormat = "MMM yyyy";
                        break;
                    default:
                        startDate = DateTime.Now.AddDays(-7).Date;
                        dateFormat = "MMM dd";
                        break;
                }
                
                string query = @"
                    SELECT 
                        CAST(COALESCE(ta.CompletedDate, tr.ReviewDate, CASE WHEN ta.Status = 'Failed' THEN GETDATE() ELSE NULL END) AS DATE) AS ActivityDate,
                        SUM(CASE WHEN ta.Status = 'Pending Review' AND (tr.IsFailed = 0 OR tr.IsFailed IS NULL) THEN 1 
                                 WHEN ta.Status = 'Completed' THEN 1 ELSE 0 END) AS CompletedCount,
                        SUM(CASE WHEN ta.Status = 'Failed' THEN 1 
                                 WHEN ta.Status = 'Pending Review' AND tr.IsFailed = 1 THEN 1 ELSE 0 END) AS FailedCount
                    FROM [dbo].[TaskAssignments] ta
                    INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                    LEFT JOIN [dbo].[TaskReviews] tr ON ta.Id = tr.TaskAssignmentId
                    WHERE t.FamilyId = @FamilyId
                      AND ta.UserId = @ChildId
                      AND ta.IsDeleted = 0
                      AND (
                          (ta.CompletedDate IS NOT NULL AND ta.CompletedDate >= @StartDate)
                          OR (ta.Status = 'Failed' AND ta.UpdatedDate >= @StartDate)
                          OR (tr.ReviewDate IS NOT NULL AND tr.ReviewDate >= @StartDate)
                      )
                    GROUP BY CAST(COALESCE(ta.CompletedDate, tr.ReviewDate, CASE WHEN ta.Status = 'Failed' THEN GETDATE() ELSE NULL END) AS DATE)
                    ORDER BY ActivityDate";
                
                Dictionary<string, object> result = new Dictionary<string, object>();
                List<string> labels = new List<string>();
                List<int> completedData = new List<int>();
                List<int> failedData = new List<int>();
                
                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@ChildId", childId),
                    new SqlParameter("@StartDate", startDate)))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        DateTime activityDate = Convert.ToDateTime(row["ActivityDate"]);
                        labels.Add(activityDate.ToString(dateFormat));
                        completedData.Add(Convert.ToInt32(row["CompletedCount"]));
                        failedData.Add(Convert.ToInt32(row["FailedCount"]));
                    }
                }
                
                result["labels"] = labels;
                result["completed"] = completedData;
                result["failed"] = failedData;
                
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetChildTaskMetrics error: {0}", ex.Message));
                return new Dictionary<string, object> 
                { 
                    { "labels", new List<string>() }, 
                    { "completed", new List<int>() }, 
                    { "failed", new List<int>() } 
                };
            }
        }

        /// <summary>
        /// Gets child activity overview for a family
        /// </summary>
        public static DataTable GetChildActivityOverview(int familyId)
        {
            try
            {
                string query = @"
                    SELECT 
                        u.Id AS ChildId,
                        u.FirstName + ' ' + u.LastName AS ChildName,
                        u.Points AS CurrentBalance,
                        (SELECT COUNT(*) 
                         FROM [dbo].[TaskAssignments] ta
                         INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                         WHERE ta.UserId = u.Id 
                           AND ta.Status = 'Pending Review' 
                           AND ta.IsDeleted = 0
                           AND t.FamilyId = @FamilyId) AS PendingReviewsCount,
                        (SELECT MAX(ta.CompletedDate)
                         FROM [dbo].[TaskAssignments] ta
                         INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                         WHERE ta.UserId = u.Id 
                           AND ta.IsDeleted = 0
                           AND t.FamilyId = @FamilyId) AS LastActivityDate
                    FROM [dbo].[FamilyMembers] fm
                    INNER JOIN [dbo].[Users] u ON fm.UserId = u.Id
                    WHERE fm.FamilyId = @FamilyId
                      AND fm.Role = 'CHILD'
                      AND fm.IsActive = 1
                      AND u.IsActive = 1
                    ORDER BY u.FirstName, u.LastName";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@FamilyId", familyId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetChildActivityOverview error: {0}", ex.Message));
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets weekly statistics for a family
        /// </summary>
        public static Dictionary<string, object> GetWeeklyStatistics(int familyId)
        {
            try
            {
                DateTime weekStart = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                
                string query = @"
                    SELECT 
                        (SELECT COUNT(DISTINCT ta.Id)
                         FROM [dbo].[TaskAssignments] ta
                         INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                         WHERE t.FamilyId = @FamilyId
                           AND CAST(ta.CompletedDate AS DATE) >= @WeekStart
                           AND ta.IsDeleted = 0) AS TasksCompleted,
                        (SELECT ISNULL(SUM(tr.PointsAwarded), 0)
                         FROM [dbo].[TaskReviews] tr
                         INNER JOIN [dbo].[TaskAssignments] ta ON tr.TaskAssignmentId = ta.Id
                         INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                         WHERE t.FamilyId = @FamilyId
                           AND tr.IsFailed = 0
                           AND CAST(tr.ReviewDate AS DATE) >= @WeekStart) AS PointsAwarded,
                        (SELECT COUNT(DISTINCT ro.Id)
                         FROM [dbo].[RewardOrders] ro
                         WHERE ro.FamilyId = @FamilyId
                           AND CAST(ro.ConfirmedDate AS DATE) >= @WeekStart) AS OrdersConfirmed";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@WeekStart", weekStart.Date)))
                {
                    Dictionary<string, object> stats = new Dictionary<string, object>();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        stats["TasksCompleted"] = Convert.ToInt32(row["TasksCompleted"]);
                        stats["PointsAwarded"] = Convert.ToInt32(row["PointsAwarded"]);
                        stats["OrdersConfirmed"] = Convert.ToInt32(row["OrdersConfirmed"]);
                    }
                    else
                    {
                        stats["TasksCompleted"] = 0;
                        stats["PointsAwarded"] = 0;
                        stats["OrdersConfirmed"] = 0;
                    }
                    return stats;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetWeeklyStatistics error: {0}", ex.Message));
                return new Dictionary<string, object> { { "TasksCompleted", 0 }, { "PointsAwarded", 0 }, { "OrdersConfirmed", 0 } };
            }
        }

        /// <summary>
        /// Gets monthly statistics for a family
        /// </summary>
        public static Dictionary<string, object> GetMonthlyStatistics(int familyId)
        {
            try
            {
                DateTime monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                
                string query = @"
                    SELECT 
                        (SELECT COUNT(DISTINCT ta.Id)
                         FROM [dbo].[TaskAssignments] ta
                         INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                         WHERE t.FamilyId = @FamilyId
                           AND CAST(ta.CompletedDate AS DATE) >= @MonthStart
                           AND ta.IsDeleted = 0) AS TasksCompleted,
                        (SELECT ISNULL(SUM(tr.PointsAwarded), 0)
                         FROM [dbo].[TaskReviews] tr
                         INNER JOIN [dbo].[TaskAssignments] ta ON tr.TaskAssignmentId = ta.Id
                         INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                         WHERE t.FamilyId = @FamilyId
                           AND tr.IsFailed = 0
                           AND CAST(tr.ReviewDate AS DATE) >= @MonthStart) AS PointsAwarded,
                        (SELECT COUNT(DISTINCT ro.Id)
                         FROM [dbo].[RewardOrders] ro
                         WHERE ro.FamilyId = @FamilyId
                           AND CAST(ro.ConfirmedDate AS DATE) >= @MonthStart) AS OrdersConfirmed";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@MonthStart", monthStart.Date)))
                {
                    Dictionary<string, object> stats = new Dictionary<string, object>();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        stats["TasksCompleted"] = Convert.ToInt32(row["TasksCompleted"]);
                        stats["PointsAwarded"] = Convert.ToInt32(row["PointsAwarded"]);
                        stats["OrdersConfirmed"] = Convert.ToInt32(row["OrdersConfirmed"]);
                    }
                    else
                    {
                        stats["TasksCompleted"] = 0;
                        stats["PointsAwarded"] = 0;
                        stats["OrdersConfirmed"] = 0;
                    }
                    return stats;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetMonthlyStatistics error: {0}", ex.Message));
                return new Dictionary<string, object> { { "TasksCompleted", 0 }, { "PointsAwarded", 0 }, { "OrdersConfirmed", 0 } };
            }
        }

        #endregion

        #region Child Dashboard Methods

        /// <summary>
        /// Gets current point balance for a child
        /// </summary>
        public static int GetChildPoints(int userId)
        {
            try
            {
                return PointHelper.GetChildBalance(userId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetChildPoints error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets count of active tasks for a child
        /// </summary>
        public static int GetActiveTasksCount(int userId)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM [dbo].[TaskAssignments] ta
                    WHERE ta.UserId = @UserId
                      AND ta.Status IN ('Assigned', 'Ongoing')
                      AND ta.IsDeleted = 0";

                object result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@UserId", userId));
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetActiveTasksCount error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets count of tasks completed this week for a child
        /// </summary>
        public static int GetCompletedThisWeek(int userId)
        {
            try
            {
                DateTime weekStart = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                string query = @"
                    SELECT COUNT(*)
                    FROM [dbo].[TaskAssignments] ta
                    WHERE ta.UserId = @UserId
                      AND ta.Status = 'Pending Review'
                      AND CAST(ta.CompletedDate AS DATE) >= @WeekStart
                      AND ta.IsDeleted = 0";

                object result = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@WeekStart", weekStart.Date));
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetCompletedThisWeek error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets count of tasks pending review for a child
        /// </summary>
        public static int GetPendingReviewsCountForChild(int userId)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*)
                    FROM [dbo].[TaskAssignments] ta
                    WHERE ta.UserId = @UserId
                      AND ta.Status = 'Pending Review'
                      AND ta.IsDeleted = 0";

                object result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@UserId", userId));
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetPendingReviewsCountForChild error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets count of rewards child can afford
        /// </summary>
        public static int GetAvailableRewardsCount(int userId, int familyId)
        {
            try
            {
                int currentPoints = GetChildPoints(userId);
                if (currentPoints <= 0) return 0;

                string query = @"
                    SELECT COUNT(*)
                    FROM [dbo].[Rewards]
                    WHERE FamilyId = @FamilyId
                      AND IsActive = 1
                      AND IsDeleted = 0
                      AND PointCost <= @CurrentPoints";

                object result = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@CurrentPoints", currentPoints));
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetAvailableRewardsCount error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets count of active orders for a child
        /// </summary>
        public static int GetActiveOrdersCount(int userId)
        {
            try
            {
                DataTable orders = RewardHelper.GetChildOrders(userId);
                return orders != null ? orders.Rows.Count : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetActiveOrdersCount error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets recent activity for a child
        /// </summary>
        public static DataTable GetRecentActivityForChild(int userId, int limit = 10)
        {
            try
            {
                // Validate limit to prevent issues
                if (limit < 1) limit = 10;
                if (limit > 100) limit = 100; // Cap at 100 for performance
                
                // SQL Server doesn't support TOP with parameter in UNION, so we use a subquery
                string query = string.Format(@"
                    SELECT TOP ({0})
                        ActivityType, TaskTitle, Points, ActivityDate, RewardName
                    FROM (
                        SELECT 
                            'PointsEarned' AS ActivityType,
                            t.Title AS TaskTitle,
                            tr.PointsAwarded AS Points,
                            tr.ReviewDate AS ActivityDate,
                            NULL AS RewardName
                        FROM [dbo].[TaskReviews] tr
                        INNER JOIN [dbo].[TaskAssignments] ta ON tr.TaskAssignmentId = ta.Id
                        INNER JOIN [dbo].[Tasks] t ON ta.TaskId = t.Id
                        WHERE ta.UserId = @UserId
                          AND tr.IsFailed = 0
                        
                        UNION ALL
                        
                        SELECT 
                            'PointsSpent' AS ActivityType,
                            NULL AS TaskTitle,
                            -ro.TotalPoints AS Points,
                            ro.ConfirmedDate AS ActivityDate,
                            (SELECT TOP 1 r.Name FROM [dbo].[RewardOrderItems] roi 
                             INNER JOIN [dbo].[Rewards] r ON roi.RewardId = r.Id 
                             WHERE roi.OrderId = ro.Id) AS RewardName
                        FROM [dbo].[RewardOrders] ro
                        WHERE ro.ChildId = @UserId
                          AND ro.Status IN ('WaitingToFulfill', 'Fulfilled', 'TransactionComplete')
                    ) AS CombinedActivity
                    ORDER BY ActivityDate DESC", limit);

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserId", userId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetRecentActivityForChild error: {0}", ex.Message));
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets weekly statistics for a child
        /// </summary>
        public static Dictionary<string, object> GetWeeklyStatisticsForChild(int userId)
        {
            try
            {
                DateTime weekStart = DateTime.Now.AddDays(-(int)DateTime.Now.DayOfWeek);
                
                string query = @"
                    SELECT 
                        (SELECT COUNT(DISTINCT ta.Id)
                         FROM [dbo].[TaskAssignments] ta
                         WHERE ta.UserId = @UserId
                           AND CAST(ta.CompletedDate AS DATE) >= @WeekStart
                           AND ta.IsDeleted = 0) AS TasksCompleted,
                        (SELECT ISNULL(SUM(tr.PointsAwarded), 0)
                         FROM [dbo].[TaskReviews] tr
                         INNER JOIN [dbo].[TaskAssignments] ta ON tr.TaskAssignmentId = ta.Id
                         WHERE ta.UserId = @UserId
                           AND tr.IsFailed = 0
                           AND CAST(tr.ReviewDate AS DATE) >= @WeekStart) AS PointsEarned,
                        (SELECT ISNULL(SUM(ro.TotalPoints), 0)
                         FROM [dbo].[RewardOrders] ro
                         WHERE ro.ChildId = @UserId
                           AND ro.Status IN ('WaitingToFulfill', 'Fulfilled', 'TransactionComplete')
                           AND CAST(ro.ConfirmedDate AS DATE) >= @WeekStart) AS PointsSpent";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@WeekStart", weekStart.Date)))
                {
                    Dictionary<string, object> stats = new Dictionary<string, object>();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        stats["TasksCompleted"] = Convert.ToInt32(row["TasksCompleted"]);
                        stats["PointsEarned"] = Convert.ToInt32(row["PointsEarned"]);
                        stats["PointsSpent"] = Convert.ToInt32(row["PointsSpent"]);
                    }
                    else
                    {
                        stats["TasksCompleted"] = 0;
                        stats["PointsEarned"] = 0;
                        stats["PointsSpent"] = 0;
                    }
                    return stats;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetWeeklyStatisticsForChild error: {0}", ex.Message));
                return new Dictionary<string, object> { { "TasksCompleted", 0 }, { "PointsEarned", 0 }, { "PointsSpent", 0 } };
            }
        }

        /// <summary>
        /// Gets monthly statistics for a child
        /// </summary>
        public static Dictionary<string, object> GetMonthlyStatisticsForChild(int userId)
        {
            try
            {
                DateTime monthStart = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                
                string query = @"
                    SELECT 
                        (SELECT COUNT(DISTINCT ta.Id)
                         FROM [dbo].[TaskAssignments] ta
                         WHERE ta.UserId = @UserId
                           AND CAST(ta.CompletedDate AS DATE) >= @MonthStart
                           AND ta.IsDeleted = 0) AS TasksCompleted,
                        (SELECT ISNULL(SUM(tr.PointsAwarded), 0)
                         FROM [dbo].[TaskReviews] tr
                         INNER JOIN [dbo].[TaskAssignments] ta ON tr.TaskAssignmentId = ta.Id
                         WHERE ta.UserId = @UserId
                           AND tr.IsFailed = 0
                           AND CAST(tr.ReviewDate AS DATE) >= @MonthStart) AS PointsEarned,
                        (SELECT ISNULL(SUM(ro.TotalPoints), 0)
                         FROM [dbo].[RewardOrders] ro
                         WHERE ro.ChildId = @UserId
                           AND ro.Status IN ('WaitingToFulfill', 'Fulfilled', 'TransactionComplete')
                           AND CAST(ro.ConfirmedDate AS DATE) >= @MonthStart) AS PointsSpent";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@MonthStart", monthStart.Date)))
                {
                    Dictionary<string, object> stats = new Dictionary<string, object>();
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        stats["TasksCompleted"] = Convert.ToInt32(row["TasksCompleted"]);
                        stats["PointsEarned"] = Convert.ToInt32(row["PointsEarned"]);
                        stats["PointsSpent"] = Convert.ToInt32(row["PointsSpent"]);
                    }
                    else
                    {
                        stats["TasksCompleted"] = 0;
                        stats["PointsEarned"] = 0;
                        stats["PointsSpent"] = 0;
                    }
                    return stats;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetMonthlyStatisticsForChild error: {0}", ex.Message));
                return new Dictionary<string, object> { { "TasksCompleted", 0 }, { "PointsEarned", 0 }, { "PointsSpent", 0 } };
            }
        }

        /// <summary>
        /// Gets streak count (consecutive days with completed tasks) for a child
        /// </summary>
        public static int GetStreakCount(int userId)
        {
            try
            {
                // Get distinct dates when child completed tasks (that were reviewed successfully)
                string query = @"
                    SELECT DISTINCT CAST(tr.ReviewDate AS DATE) AS ReviewDate
                    FROM [dbo].[TaskReviews] tr
                    INNER JOIN [dbo].[TaskAssignments] ta ON tr.TaskAssignmentId = ta.Id
                    WHERE ta.UserId = @UserId
                      AND tr.IsFailed = 0
                    ORDER BY CAST(tr.ReviewDate AS DATE) DESC";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserId", userId)))
                {
                    if (dt.Rows.Count == 0) return 0;

                    int streak = 0;
                    DateTime currentDate = DateTime.Now.Date;
                    
                    foreach (DataRow row in dt.Rows)
                    {
                        DateTime reviewDate = Convert.ToDateTime(row["ReviewDate"]).Date;
                        
                        if (reviewDate == currentDate.AddDays(-streak))
                        {
                            streak++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    
                    return streak;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetStreakCount error: {0}", ex.Message));
                return 0;
            }
        }

        #endregion
    }
}

