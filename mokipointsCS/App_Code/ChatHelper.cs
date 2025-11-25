using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace mokipointsCS
{
    /// <summary>
    /// Helper class for family chat operations
    /// </summary>
    public class ChatHelper
    {
        #region Message Management

        /// <summary>
        /// Sends a text message to the family chat
        /// </summary>
        public static int SendMessage(int familyId, int userId, string messageText, int? replyToMessageId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(messageText))
                {
                    System.Diagnostics.Debug.WriteLine("ChatHelper.SendMessage: Message text is empty");
                    return 0;
                }

                // Validate message length (5000 characters max)
                if (messageText.Length > 5000)
                {
                    System.Diagnostics.Debug.WriteLine("ChatHelper.SendMessage: Message exceeds 5000 characters");
                    return 0;
                }

                // Verify user is in family
                if (!FamilyHelper.IsFamilyMember(familyId, userId))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.SendMessage: User {0} is not in family {1}", userId, familyId));
                    return 0;
                }

                string query = @"
                    INSERT INTO [dbo].[FamilyMessages] (FamilyId, UserId, MessageType, MessageText, ReplyToMessageId, CreatedDate)
                    VALUES (@FamilyId, @UserId, 'Text', @MessageText, @ReplyToMessageId, GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                object result = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@MessageText", messageText),
                    new SqlParameter("@ReplyToMessageId", replyToMessageId.HasValue ? (object)replyToMessageId.Value : DBNull.Value));

                int messageId = result != null ? Convert.ToInt32(result) : 0;
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.SendMessage: Message sent successfully. MessageId={0}", messageId));
                return messageId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.SendMessage error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return 0;
            }
        }

        /// <summary>
        /// Sends an image message to the family chat
        /// </summary>
        public static int SendImageMessage(int familyId, int userId, string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath))
                {
                    System.Diagnostics.Debug.WriteLine("ChatHelper.SendImageMessage: Image path is empty");
                    return 0;
                }

                // Verify user is in family
                if (!FamilyHelper.IsFamilyMember(familyId, userId))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.SendImageMessage: User {0} is not in family {1}", userId, familyId));
                    return 0;
                }

                string query = @"
                    INSERT INTO [dbo].[FamilyMessages] (FamilyId, UserId, MessageType, ImagePath, CreatedDate)
                    VALUES (@FamilyId, @UserId, 'Image', @ImagePath, GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                object result = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@ImagePath", imagePath));

                int messageId = result != null ? Convert.ToInt32(result) : 0;
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.SendImageMessage: Image message sent successfully. MessageId={0}", messageId));
                return messageId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.SendImageMessage error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Sends a GIF message to the family chat
        /// </summary>
        public static int SendGIFMessage(int familyId, int userId, string gifUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(gifUrl))
                {
                    System.Diagnostics.Debug.WriteLine("ChatHelper.SendGIFMessage: GIF URL is empty");
                    return 0;
                }

                // Verify user is in family
                if (!FamilyHelper.IsFamilyMember(familyId, userId))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.SendGIFMessage: User {0} is not in family {1}", userId, familyId));
                    return 0;
                }

                string query = @"
                    INSERT INTO [dbo].[FamilyMessages] (FamilyId, UserId, MessageType, GIFUrl, CreatedDate)
                    VALUES (@FamilyId, @UserId, 'GIF', @GIFUrl, GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                object result = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@GIFUrl", gifUrl));

                int messageId = result != null ? Convert.ToInt32(result) : 0;
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.SendGIFMessage: GIF message sent successfully. MessageId={0}", messageId));
                return messageId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.SendGIFMessage error: {0}", ex.Message));
                return 0;
            }
        }

        /// <summary>
        /// Gets messages for a family (with pagination)
        /// </summary>
        public static DataTable GetMessages(int familyId, int userId, int pageSize = 50, int? lastMessageId = null)
        {
            try
            {
                // Verify user is in family
                if (!FamilyHelper.IsFamilyMember(familyId, userId))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.GetMessages: User {0} is not in family {1}", userId, familyId));
                    return new DataTable();
                }

                string query = @"
                    SELECT TOP (@PageSize)
                        fm.Id, fm.FamilyId, fm.UserId, fm.MessageType, fm.MessageText, 
                        fm.ImagePath, fm.GIFUrl, fm.ReplyToMessageId, fm.SystemEventType, 
                        fm.SystemEventData, fm.CreatedDate, fm.IsDeleted,
                        u.FirstName,
                        u.LastName,
                        u.ProfilePicture,
                        replyMsg.MessageText AS ReplyToMessageText,
                        replyUser.FirstName AS ReplyToFirstName,
                        replyUser.LastName AS ReplyToLastName
                    FROM [dbo].[FamilyMessages] fm
                    LEFT JOIN [dbo].[Users] u ON fm.UserId = u.Id
                    LEFT JOIN [dbo].[FamilyMessages] replyMsg ON fm.ReplyToMessageId = replyMsg.Id
                    LEFT JOIN [dbo].[Users] replyUser ON replyMsg.UserId = replyUser.Id
                    WHERE fm.FamilyId = @FamilyId 
                      AND fm.IsDeleted = 0";

                if (lastMessageId.HasValue)
                {
                    query += " AND fm.Id < @LastMessageId";
                }

                query += " ORDER BY fm.CreatedDate DESC";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@PageSize", pageSize)
                };

                if (lastMessageId.HasValue)
                {
                    parameters.Add(new SqlParameter("@LastMessageId", lastMessageId.Value));
                }

                DataTable messages = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());

                // Return messages in DESC order (newest first) - no reversal needed
                return messages;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.GetMessages error: {0}", ex.Message));
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets new messages since a specific message ID
        /// </summary>
        public static DataTable GetNewMessages(int familyId, int userId, int lastMessageId)
        {
            try
            {
                // Verify user is in family
                if (!FamilyHelper.IsFamilyMember(familyId, userId))
                {
                    return new DataTable();
                }

                string query = @"
                    SELECT fm.Id, fm.FamilyId, fm.UserId, fm.MessageType, fm.MessageText, 
                           fm.ImagePath, fm.GIFUrl, fm.ReplyToMessageId, fm.SystemEventType, 
                           fm.SystemEventData, fm.CreatedDate, fm.IsDeleted,
                           u.FirstName,
                           u.LastName,
                           u.ProfilePicture,
                           replyMsg.MessageText AS ReplyToMessageText,
                           replyUser.FirstName AS ReplyToFirstName,
                           replyUser.LastName AS ReplyToLastName
                    FROM [dbo].[FamilyMessages] fm
                    LEFT JOIN [dbo].[Users] u ON fm.UserId = u.Id
                    LEFT JOIN [dbo].[FamilyMessages] replyMsg ON fm.ReplyToMessageId = replyMsg.Id
                    LEFT JOIN [dbo].[Users] replyUser ON replyMsg.UserId = replyUser.Id
                    WHERE fm.FamilyId = @FamilyId 
                      AND fm.IsDeleted = 0
                      AND fm.Id > @LastMessageId
                    ORDER BY fm.CreatedDate DESC";

                return DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@LastMessageId", lastMessageId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.GetNewMessages error: {0}", ex.Message));
                return new DataTable();
            }
        }

        #endregion

        #region Reactions

        /// <summary>
        /// Adds a reaction to a message
        /// </summary>
        public static bool AddReaction(int messageId, int userId, string reactionType)
        {
            try
            {
                // Validate reaction type
                string[] validReactions = { "Like", "Love", "Haha", "Sad", "Angry" };
                if (!validReactions.Contains(reactionType))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.AddReaction: Invalid reaction type: {0}", reactionType));
                    return false;
                }

                // Check if reaction already exists
                string checkQuery = @"
                    SELECT Id FROM [dbo].[FamilyMessageReactions]
                    WHERE MessageId = @MessageId AND UserId = @UserId AND ReactionType = @ReactionType";

                object existing = DatabaseHelper.ExecuteScalar(checkQuery,
                    new SqlParameter("@MessageId", messageId),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@ReactionType", reactionType));

                if (existing != null)
                {
                    // Reaction already exists, remove it (toggle behavior)
                    return RemoveReaction(messageId, userId, reactionType);
                }

                // Add reaction
                string query = @"
                    INSERT INTO [dbo].[FamilyMessageReactions] (MessageId, UserId, ReactionType, CreatedDate)
                    VALUES (@MessageId, @UserId, @ReactionType, GETDATE())";

                int rows = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@MessageId", messageId),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@ReactionType", reactionType));

                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.AddReaction error: {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Removes a reaction from a message
        /// </summary>
        public static bool RemoveReaction(int messageId, int userId, string reactionType)
        {
            try
            {
                string query = @"
                    DELETE FROM [dbo].[FamilyMessageReactions]
                    WHERE MessageId = @MessageId AND UserId = @UserId AND ReactionType = @ReactionType";

                int rows = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@MessageId", messageId),
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@ReactionType", reactionType));

                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.RemoveReaction error: {0}", ex.Message));
                return false;
            }
        }

        /// <summary>
        /// Gets all reactions for a message
        /// </summary>
        public static DataTable GetReactions(int messageId)
        {
            try
            {
                string query = @"
                    SELECT fmr.Id, fmr.MessageId, fmr.UserId, fmr.ReactionType, fmr.CreatedDate,
                           u.FirstName, u.LastName, u.ProfilePicture
                    FROM [dbo].[FamilyMessageReactions] fmr
                    INNER JOIN [dbo].[Users] u ON fmr.UserId = u.Id
                    WHERE fmr.MessageId = @MessageId
                    ORDER BY fmr.CreatedDate ASC";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@MessageId", messageId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.GetReactions error: {0}", ex.Message));
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets reaction counts grouped by type for a message
        /// </summary>
        public static DataTable GetReactionCounts(int messageId)
        {
            try
            {
                string query = @"
                    SELECT ReactionType, COUNT(*) AS Count
                    FROM [dbo].[FamilyMessageReactions]
                    WHERE MessageId = @MessageId
                    GROUP BY ReactionType";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@MessageId", messageId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.GetReactionCounts error: {0}", ex.Message));
                return new DataTable();
            }
        }

        #endregion

        #region System Messages

        /// <summary>
        /// Posts a system message to the family chat
        /// </summary>
        public static int PostSystemMessage(int familyId, string systemEventType, string messageText, string systemEventData = null)
        {
            try
            {
                // System messages use NULL for UserId (since UserId = 0 doesn't exist and violates FK constraint)
                string query = @"
                    INSERT INTO [dbo].[FamilyMessages] (FamilyId, UserId, MessageType, MessageText, SystemEventType, SystemEventData, CreatedDate)
                    VALUES (@FamilyId, NULL, 'System', @MessageText, @SystemEventType, @SystemEventData, GETDATE());
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                object result = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@MessageText", messageText),
                    new SqlParameter("@SystemEventType", systemEventType),
                    new SqlParameter("@SystemEventData", string.IsNullOrEmpty(systemEventData) ? (object)DBNull.Value : systemEventData));

                int messageId = result != null ? Convert.ToInt32(result) : 0;
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.PostSystemMessage: System message posted. MessageId={0}, EventType={1}", messageId, systemEventType));
                return messageId;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ChatHelper.PostSystemMessage error: {0}", ex.Message));
                return 0;
            }
        }

        #endregion
    }
}

