using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace mokipointsCS
{
    /// <summary>
    /// Data class for achievement information
    /// </summary>
    public class AchievementData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Rarity { get; set; }
        public string BadgeImagePath { get; set; }
        public string Role { get; set; }
        public DateTime? EarnedDate { get; set; }
    }

    /// <summary>
    /// Data class for achievement with earned status
    /// </summary>
    public class AchievementWithStatus : AchievementData
    {
        public bool IsEarned { get; set; }
        public string HowToAchieve { get; set; }
        public string DeveloperMessage { get; set; }
    }

    /// <summary>
    /// Data class for full achievement details
    /// </summary>
    public class AchievementDetail : AchievementWithStatus
    {
        public string TriggerType { get; set; }
        public int? TriggerValue { get; set; }
        public int CurrentProgress { get; set; }
        public int TargetProgress { get; set; }
    }

    /// <summary>
    /// Helper class for achievement system operations
    /// </summary>
    public class AchievementHelper
    {
        /// <summary>
        /// Checks and awards achievements based on trigger type and value
        /// Only awards if achievement not already earned (permanent once earned)
        /// </summary>
        public static AchievementData CheckAndAwardAchievement(int userId, string triggerType, int? triggerValue = null)
        {
            try
            {
                // Find matching achievements for this trigger
                string findQuery = @"
                    SELECT [Id], [Name], [Description], [Rarity], [BadgeImagePath], [Role], [TriggerType], [TriggerValue]
                    FROM [dbo].[Achievements]
                    WHERE [TriggerType] = @TriggerType
                      AND [IsActive] = 1
                      AND ([TriggerValue] = @TriggerValue OR (@TriggerValue IS NULL AND [TriggerValue] IS NULL))";

                List<int> achievementIds = new List<int>();
                using (DataTable dt = DatabaseHelper.ExecuteQuery(findQuery,
                    new SqlParameter("@TriggerType", triggerType),
                    new SqlParameter("@TriggerValue", triggerValue.HasValue ? (object)triggerValue.Value : DBNull.Value)))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        int achievementId = Convert.ToInt32(row["Id"]);
                        achievementIds.Add(achievementId);
                    }
                }

                // Check each achievement and award if not already earned
                AchievementData awardedAchievement = null;
                foreach (int achievementId in achievementIds)
                {
                    // Check if user already has this achievement
                    string checkQuery = @"
                        SELECT COUNT(*) FROM [dbo].[UserAchievements]
                        WHERE [UserId] = @UserId AND [AchievementId] = @AchievementId";

                    object count = DatabaseHelper.ExecuteScalar(checkQuery,
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@AchievementId", achievementId));

                    if (Convert.ToInt32(count) == 0)
                    {
                        // User doesn't have this achievement yet - award it
                        awardedAchievement = AwardAchievement(userId, achievementId);
                        if (awardedAchievement != null)
                        {
                            // Only return the first achievement awarded (in case multiple match)
                            break;
                        }
                    }
                }

                return awardedAchievement;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CheckAndAwardAchievement error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                return null;
            }
        }

        /// <summary>
        /// Awards a specific achievement to a user
        /// Returns achievement data for notification, or null if already earned
        /// </summary>
        public static AchievementData AwardAchievement(int userId, int achievementId)
        {
            try
            {
                // Check if user already has this achievement
                string checkQuery = @"
                    SELECT COUNT(*) FROM [dbo].[UserAchievements]
                    WHERE [UserId] = @UserId AND [AchievementId] = @AchievementId";

                object count = DatabaseHelper.ExecuteScalar(checkQuery,
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@AchievementId", achievementId));

                if (Convert.ToInt32(count) > 0)
                {
                    // Already earned - return null
                    System.Diagnostics.Debug.WriteLine(string.Format("User {0} already has achievement {1}", userId, achievementId));
                    return null;
                }

                // Get achievement details
                string getAchievementQuery = @"
                    SELECT [Id], [Name], [Description], [Rarity], [BadgeImagePath], [Role]
                    FROM [dbo].[Achievements]
                    WHERE [Id] = @AchievementId";

                AchievementData achievementData = null;
                using (DataTable dt = DatabaseHelper.ExecuteQuery(getAchievementQuery,
                    new SqlParameter("@AchievementId", achievementId)))
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        achievementData = new AchievementData
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = row["Name"].ToString(),
                            Description = row["Description"].ToString(),
                            Rarity = row["Rarity"].ToString(),
                            BadgeImagePath = row["BadgeImagePath"].ToString(),
                            Role = row["Role"].ToString()
                        };
                    }
                }

                if (achievementData == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Achievement {0} not found", achievementId));
                    return null;
                }

                // Insert into UserAchievements
                string insertQuery = @"
                    INSERT INTO [dbo].[UserAchievements] ([UserId], [AchievementId], [EarnedDate], [IsDisplayed])
                    VALUES (@UserId, @AchievementId, GETDATE(), 1)";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(insertQuery,
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@AchievementId", achievementId));

                if (rowsAffected > 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("Achievement '{0}' awarded to user {1}", achievementData.Name, userId));
                    
                    // Store achievement in session for dashboard notification (if HttpContext is available)
                    try
                    {
                        if (System.Web.HttpContext.Current != null)
                        {
                            System.Web.HttpContext.Current.Session["NewAchievement"] = achievementData;
                        }
                    }
                    catch (Exception sessionEx)
                    {
                        // Don't fail if session storage fails
                        System.Diagnostics.Debug.WriteLine("Failed to store achievement in session: " + sessionEx.Message);
                    }
                    
                    // Post system message to family chat
                    try
                    {
                        int? familyId = FamilyHelper.GetUserFamilyId(userId);
                        if (familyId.HasValue)
                        {
                            var userInfo = AuthenticationHelper.GetUserById(userId);
                            if (userInfo != null)
                            {
                                string firstName = (userInfo["FirstName"] != null && userInfo["FirstName"] != DBNull.Value)
                                    ? userInfo["FirstName"].ToString() : "User";
                                string lastName = (userInfo["LastName"] != null && userInfo["LastName"] != DBNull.Value)
                                    ? userInfo["LastName"].ToString() : "";
                                string fullName = string.IsNullOrEmpty(lastName) ? firstName : firstName + " " + lastName;

                                string message = string.Format("{0} has earned the {1} achievement! {2}", 
                                    fullName, achievementData.Name, achievementData.Description);

                                ChatHelper.PostSystemMessage(familyId.Value, "AchievementEarned", message,
                                    string.Format("{{\"UserId\":{0},\"AchievementId\":{1},\"AchievementName\":\"{2}\",\"Rarity\":\"{3}\"}}",
                                        userId, achievementId, achievementData.Name, achievementData.Rarity));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Don't fail achievement award if chat message fails
                        System.Diagnostics.Debug.WriteLine("Failed to post achievement message to chat: " + ex.Message);
                    }

                    return achievementData;
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AwardAchievement error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                return null;
            }
        }

        /// <summary>
        /// Gets all earned achievements for a user
        /// </summary>
        public static List<AchievementData> GetUserAchievements(int userId)
        {
            List<AchievementData> achievements = new List<AchievementData>();
            try
            {
                string query = @"
                    SELECT a.[Id], a.[Name], a.[Description], a.[Rarity], a.[BadgeImagePath], a.[Role], ua.[EarnedDate]
                    FROM [dbo].[UserAchievements] ua
                    INNER JOIN [dbo].[Achievements] a ON ua.[AchievementId] = a.[Id]
                    WHERE ua.[UserId] = @UserId AND ua.[IsDisplayed] = 1
                    ORDER BY 
                        CASE a.[Rarity]
                            WHEN 'Mythical' THEN 1
                            WHEN 'Legendary' THEN 2
                            WHEN 'Epic' THEN 3
                            WHEN 'Rare' THEN 4
                            WHEN 'Uncommon' THEN 5
                            WHEN 'Common' THEN 6
                        END,
                        ua.[EarnedDate] DESC";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@UserId", userId)))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        achievements.Add(new AchievementData
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = row["Name"].ToString(),
                            Description = row["Description"].ToString(),
                            Rarity = row["Rarity"].ToString(),
                            BadgeImagePath = row["BadgeImagePath"].ToString(),
                            Role = row["Role"].ToString(),
                            EarnedDate = row["EarnedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["EarnedDate"]) : null
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetUserAchievements error: " + ex.Message);
            }
            return achievements;
        }

        /// <summary>
        /// Gets top 3 achievements by rarity and recent (for profile page)
        /// </summary>
        public static List<AchievementData> GetTop3Achievements(int userId)
        {
            List<AchievementData> allAchievements = GetUserAchievements(userId);
            
            // Already sorted by rarity and date, just take top 3
            List<AchievementData> top3 = new List<AchievementData>();
            for (int i = 0; i < Math.Min(3, allAchievements.Count); i++)
            {
                top3.Add(allAchievements[i]);
            }
            
            return top3;
        }

        /// <summary>
        /// Gets all achievements for a role (CHILD or PARENT)
        /// </summary>
        public static List<AchievementData> GetAchievementsByRole(string role)
        {
            List<AchievementData> achievements = new List<AchievementData>();
            try
            {
                string query = @"
                    SELECT [Id], [Name], [Description], [Rarity], [BadgeImagePath], [Role], [TriggerType], [TriggerValue]
                    FROM [dbo].[Achievements]
                    WHERE [Role] = @Role AND [IsActive] = 1
                    ORDER BY 
                        CASE [Rarity]
                            WHEN 'Mythical' THEN 1
                            WHEN 'Legendary' THEN 2
                            WHEN 'Epic' THEN 3
                            WHEN 'Rare' THEN 4
                            WHEN 'Uncommon' THEN 5
                            WHEN 'Common' THEN 6
                        END,
                        [Name]";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@Role", role)))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        achievements.Add(new AchievementData
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = row["Name"].ToString(),
                            Description = row["Description"].ToString(),
                            Rarity = row["Rarity"].ToString(),
                            BadgeImagePath = row["BadgeImagePath"].ToString(),
                            Role = row["Role"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetAchievementsByRole error: " + ex.Message);
            }
            return achievements;
        }

        /// <summary>
        /// Gets all achievements (earned and unearned) for user's role with earned status
        /// </summary>
        public static List<AchievementWithStatus> GetAllAchievementsForUser(int userId, string role)
        {
            List<AchievementWithStatus> achievements = new List<AchievementWithStatus>();
            try
            {
                string query = @"
                    SELECT 
                        a.[Id], 
                        a.[Name], 
                        a.[Description], 
                        a.[Rarity], 
                        a.[BadgeImagePath], 
                        a.[Role],
                        a.[HowToAchieve],
                        a.[DeveloperMessage],
                        a.[TriggerType],
                        a.[TriggerValue],
                        ua.[EarnedDate],
                        CASE WHEN ua.[Id] IS NOT NULL THEN 1 ELSE 0 END AS [IsEarned]
                    FROM [dbo].[Achievements] a
                    LEFT JOIN [dbo].[UserAchievements] ua ON a.[Id] = ua.[AchievementId] AND ua.[UserId] = @UserId
                    WHERE a.[Role] = @Role AND a.[IsActive] = 1
                    ORDER BY 
                        CASE a.[Rarity]
                            WHEN 'Mythical' THEN 1
                            WHEN 'Legendary' THEN 2
                            WHEN 'Epic' THEN 3
                            WHEN 'Rare' THEN 4
                            WHEN 'Uncommon' THEN 5
                            WHEN 'Common' THEN 6
                        END,
                        a.[Name]";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@Role", role)))
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        achievements.Add(new AchievementWithStatus
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = row["Name"].ToString(),
                            Description = row["Description"].ToString(),
                            Rarity = row["Rarity"].ToString(),
                            BadgeImagePath = row["BadgeImagePath"].ToString(),
                            Role = row["Role"].ToString(),
                            HowToAchieve = row["HowToAchieve"] != DBNull.Value ? row["HowToAchieve"].ToString() : null,
                            DeveloperMessage = row["DeveloperMessage"] != DBNull.Value ? row["DeveloperMessage"].ToString() : null,
                            IsEarned = Convert.ToBoolean(row["IsEarned"]),
                            EarnedDate = row["EarnedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["EarnedDate"]) : null
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetAllAchievementsForUser error: " + ex.Message);
            }
            return achievements;
        }

        /// <summary>
        /// Gets full achievement details including user's earned status and progress
        /// </summary>
        public static AchievementDetail GetAchievementDetails(int achievementId, int userId)
        {
            try
            {
                string query = @"
                    SELECT 
                        a.[Id], 
                        a.[Name], 
                        a.[Description], 
                        a.[Rarity], 
                        a.[BadgeImagePath], 
                        a.[Role],
                        a.[HowToAchieve],
                        a.[DeveloperMessage],
                        a.[TriggerType],
                        a.[TriggerValue],
                        ua.[EarnedDate],
                        CASE WHEN ua.[Id] IS NOT NULL THEN 1 ELSE 0 END AS [IsEarned]
                    FROM [dbo].[Achievements] a
                    LEFT JOIN [dbo].[UserAchievements] ua ON a.[Id] = ua.[AchievementId] AND ua.[UserId] = @UserId
                    WHERE a.[Id] = @AchievementId";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@AchievementId", achievementId),
                    new SqlParameter("@UserId", userId)))
                {
                    if (dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        AchievementDetail detail = new AchievementDetail
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            Name = row["Name"].ToString(),
                            Description = row["Description"].ToString(),
                            Rarity = row["Rarity"].ToString(),
                            BadgeImagePath = row["BadgeImagePath"].ToString(),
                            Role = row["Role"].ToString(),
                            HowToAchieve = row["HowToAchieve"] != DBNull.Value ? row["HowToAchieve"].ToString() : null,
                            DeveloperMessage = row["DeveloperMessage"] != DBNull.Value ? row["DeveloperMessage"].ToString() : null,
                            IsEarned = Convert.ToBoolean(row["IsEarned"]),
                            EarnedDate = row["EarnedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["EarnedDate"]) : null,
                            TriggerType = row["TriggerType"].ToString(),
                            TriggerValue = row["TriggerValue"] != DBNull.Value ? (int?)Convert.ToInt32(row["TriggerValue"]) : null
                        };

                        // Calculate progress if not earned
                        if (!detail.IsEarned && detail.TriggerValue.HasValue)
                        {
                            detail.CurrentProgress = GetAchievementProgress(userId, achievementId, detail.TriggerType, detail.TriggerValue.Value);
                            detail.TargetProgress = detail.TriggerValue.Value;
                        }

                        return detail;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetAchievementDetails error: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Gets progress for a progress-based achievement
        /// </summary>
        private static int GetAchievementProgress(int userId, int achievementId, string triggerType, int targetValue)
        {
            try
            {
                switch (triggerType)
                {
                    case "PointsEarned":
                        // Get total points earned (from PointTransactions where points > 0)
                        string pointsQuery = @"
                            SELECT ISNULL(SUM([Points]), 0)
                            FROM [dbo].[PointTransactions]
                            WHERE [UserId] = @UserId AND [Points] > 0";
                        object pointsResult = DatabaseHelper.ExecuteScalar(pointsQuery,
                            new SqlParameter("@UserId", userId));
                        return Convert.ToInt32(pointsResult);

                    case "TasksCompleted":
                        // Get count of completed tasks (reviewed with rating > 0)
                        string tasksQuery = @"
                            SELECT COUNT(DISTINCT ta.[Id])
                            FROM [dbo].[TaskAssignments] ta
                            INNER JOIN [dbo].[TaskReviews] tr ON ta.[Id] = tr.[TaskAssignmentId]
                            WHERE ta.[UserId] = @UserId 
                              AND ta.[Status] = 'Reviewed'
                              AND tr.[Rating] > 0
                              AND ta.[IsDeleted] = 0";
                        object tasksResult = DatabaseHelper.ExecuteScalar(tasksQuery,
                            new SqlParameter("@UserId", userId));
                        return Convert.ToInt32(tasksResult);

                    case "TasksCreated":
                        // Get count of tasks created by user
                        string createdTasksQuery = @"
                            SELECT COUNT(*)
                            FROM [dbo].[Tasks]
                            WHERE [CreatedBy] = @UserId AND [IsActive] = 1";
                        object createdTasksResult = DatabaseHelper.ExecuteScalar(createdTasksQuery,
                            new SqlParameter("@UserId", userId));
                        return Convert.ToInt32(createdTasksResult);

                    case "RewardsFulfilled":
                        // Get count of rewards fulfilled and confirmed by child
                        string fulfilledQuery = @"
                            SELECT COUNT(*)
                            FROM [dbo].[RewardOrders]
                            WHERE [FamilyId] IN (SELECT [FamilyId] FROM [dbo].[FamilyMembers] WHERE [UserId] = @UserId AND [IsActive] = 1)
                              AND [Status] = 'Fulfilled'";
                        // Note: This counts all fulfilled orders in family, not just by this parent
                        // May need refinement based on actual reward fulfillment tracking
                        object fulfilledResult = DatabaseHelper.ExecuteScalar(fulfilledQuery,
                            new SqlParameter("@UserId", userId));
                        return Convert.ToInt32(fulfilledResult);

                    default:
                        return 0;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetAchievementProgress error: " + ex.Message);
                return 0;
            }
        }
    }
}

