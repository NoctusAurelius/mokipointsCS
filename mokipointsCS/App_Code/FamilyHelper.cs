using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace mokipointsCS
{
    /// <summary>
    /// Helper class for family management operations
    /// </summary>
    public class FamilyHelper
    {
        /// <summary>
        /// Generates a unique family code (2 letters + 4 digits, e.g., LP2222)
        /// </summary>
        public static string GenerateFamilyCode()
        {
            Random random = new Random();
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string code = "";
            
            // Generate until unique
            int attempts = 0;
            do
            {
                // 2 random letters
                code = letters[random.Next(letters.Length)].ToString() + 
                       letters[random.Next(letters.Length)].ToString();
                
                // 4 random digits
                code += random.Next(1000, 9999).ToString();
                
                attempts++;
                if (attempts > 100)
                {
                    // Fallback: add timestamp to ensure uniqueness
                    code = code.Substring(0, 2) + (random.Next(1000, 9999) + attempts).ToString().PadLeft(4, '0');
                    break;
                }
            } while (FamilyCodeExists(code));
            
            return code;
        }

        /// <summary>
        /// Checks if a family code already exists
        /// </summary>
        private static bool FamilyCodeExists(string familyCode)
        {
            try
            {
                string query = "SELECT COUNT(*) FROM [dbo].[Families] WHERE FamilyCode = @FamilyCode";
                object count = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@FamilyCode", familyCode));
                return Convert.ToInt32(count) > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a new family
        /// </summary>
        public static int CreateFamily(string name, string pinCode, int ownerId)
        {
            System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily called with Name=" + name + ", PinCode=" + pinCode + ", OwnerId=" + ownerId);
            try
            {
                // Generate unique family code
                System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily: Generating family code...");
                string familyCode = GenerateFamilyCode();
                System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily: Generated family code: " + familyCode);

                // Insert family
                string query = @"
                    INSERT INTO [dbo].[Families] (Name, PinCode, FamilyCode, OwnerId, TreasuryPoints)
                    VALUES (@Name, @PinCode, @FamilyCode, @OwnerId, 1000000);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily: Executing INSERT query...");
                object familyId = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@Name", name),
                    new SqlParameter("@PinCode", pinCode),
                    new SqlParameter("@FamilyCode", familyCode),
                    new SqlParameter("@OwnerId", ownerId));

                System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily: INSERT returned familyId object: " + (familyId != null && familyId != DBNull.Value ? familyId.ToString() : "NULL"));

                if (familyId == null || familyId == DBNull.Value)
                {
                    System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily: ERROR - familyId is NULL or DBNull");
                    return -1;
                }

                int familyIdInt = Convert.ToInt32(familyId);
                System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily: Converted familyId to int: " + familyIdInt);

                // Initialize treasury with starting balance (1,000,000 points)
                System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily: Initializing treasury with starting balance...");
                TreasuryHelper.InitializeTreasury(familyIdInt, 1000000);
                System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily: Treasury initialized");

                // Add owner as family member
                System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily: Adding owner as family member...");
                bool memberAdded = AddFamilyMember(familyIdInt, ownerId, "PARENT");
                System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily: AddFamilyMember returned: " + memberAdded);

                if (!memberAdded)
                {
                    System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily: WARNING - Failed to add owner as family member");
                }

                System.Diagnostics.Debug.WriteLine(string.Format("FamilyHelper.CreateFamily: SUCCESS - Family created: {0}, Code: {1}, Owner: {2}, FamilyId: {3}", name, familyCode, ownerId, familyIdInt));
                return familyIdInt;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("FamilyHelper.CreateFamily ERROR: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                return -1;
            }
        }

        /// <summary>
        /// Joins an existing family (for parents)
        /// </summary>
        public static bool JoinFamily(string familyName, string pinCode, int userId)
        {
            try
            {
                // Verify family name and PIN
                string query = @"
                    SELECT Id FROM [dbo].[Families] 
                    WHERE Name = @Name AND PinCode = @PinCode";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@Name", familyName),
                    new SqlParameter("@PinCode", pinCode)))
                {
                    if (dt.Rows.Count == 0)
                    {
                        return false; // Family not found or wrong PIN
                    }

                    int familyId = Convert.ToInt32(dt.Rows[0]["Id"]);

                    // Check if user is already a member
                    if (IsFamilyMember(familyId, userId))
                    {
                        return false; // Already a member
                    }

                    // Add user as family member
                    return AddFamilyMember(familyId, userId, "PARENT");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JoinFamily error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Joins a family using family code (for children)
        /// </summary>
        public static bool JoinFamilyByCode(string familyCode, int userId)
        {
            try
            {
                // Find family by code
                string query = @"
                    SELECT Id FROM [dbo].[Families] 
                    WHERE FamilyCode = @FamilyCode";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@FamilyCode", familyCode.ToUpper())))
                {
                    if (dt.Rows.Count == 0)
                    {
                        return false; // Family not found
                    }

                    int familyId = Convert.ToInt32(dt.Rows[0]["Id"]);

                    // Check if user is already a member
                    if (IsFamilyMember(familyId, userId))
                    {
                        return false; // Already a member
                    }

                    // Get user role
                    var userInfo = AuthenticationHelper.GetUserById(userId);
                    if (userInfo == null)
                    {
                        return false;
                    }

                    string role = userInfo["Role"].ToString();

                    // Add user as family member
                    return AddFamilyMember(familyId, userId, role);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("JoinFamilyByCode error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Adds a user to a family (or reactivates if previously removed)
        /// </summary>
        private static bool AddFamilyMember(int familyId, int userId, string role)
        {
            try
            {
                // Check if record already exists (even if inactive)
                string checkQuery = @"
                    SELECT COUNT(*) FROM [dbo].[FamilyMembers]
                    WHERE FamilyId = @FamilyId AND UserId = @UserId";
                
                object existingCount = DatabaseHelper.ExecuteScalar(checkQuery,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@UserId", userId));
                
                bool recordExists = Convert.ToInt32(existingCount) > 0;
                
                int rowsAffected = 0;
                
                if (recordExists)
                {
                    // Record exists (likely inactive) - UPDATE to reactivate
                    string updateQuery = @"
                        UPDATE [dbo].[FamilyMembers]
                        SET Role = @Role, IsActive = 1
                        WHERE FamilyId = @FamilyId AND UserId = @UserId";
                    
                    rowsAffected = DatabaseHelper.ExecuteNonQuery(updateQuery,
                        new SqlParameter("@FamilyId", familyId),
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@Role", role));
                    
                    System.Diagnostics.Debug.WriteLine(string.Format("AddFamilyMember: Reactivated existing membership for user {0} in family {1} as {2}", userId, familyId, role));
                }
                else
                {
                    // Record doesn't exist - INSERT new
                    string insertQuery = @"
                        INSERT INTO [dbo].[FamilyMembers] (FamilyId, UserId, Role, IsActive)
                        VALUES (@FamilyId, @UserId, @Role, 1)";
                    
                    rowsAffected = DatabaseHelper.ExecuteNonQuery(insertQuery,
                        new SqlParameter("@FamilyId", familyId),
                        new SqlParameter("@UserId", userId),
                        new SqlParameter("@Role", role));
                    
                    System.Diagnostics.Debug.WriteLine(string.Format("AddFamilyMember: Created new membership for user {0} in family {1} as {2}", userId, familyId, role));
                }

                if (rowsAffected > 0)
                {
                    // If child joins, reset their points to 0
                    if (role == "CHILD")
                    {
                        ResetUserPoints(userId);
                    }

                    // Post welcome message to family chat (only for new joins, not reactivations)
                    if (!recordExists)
                    {
                        try
                        {
                            var userInfo = AuthenticationHelper.GetUserById(userId);
                            if (userInfo != null)
                            {
                                string firstName = (userInfo["FirstName"] != null && userInfo["FirstName"] != DBNull.Value) 
                                    ? userInfo["FirstName"].ToString() : "User";
                                string lastName = (userInfo["LastName"] != null && userInfo["LastName"] != DBNull.Value) 
                                    ? userInfo["LastName"].ToString() : "";
                                string fullName = string.IsNullOrEmpty(lastName) ? firstName : firstName + " " + lastName;
                                
                                string welcomeMessage = string.Format("Welcome {0} to the family! {1} has joined as a {2}.", 
                                    firstName, fullName, role == "CHILD" ? "child" : "parent");
                                
                                ChatHelper.PostSystemMessage(familyId, "MemberJoined", welcomeMessage, 
                                    string.Format("{{\"UserId\":{0},\"Role\":\"{1}\"}}", userId, role));
                                
                                System.Diagnostics.Debug.WriteLine(string.Format("AddFamilyMember: Posted welcome message for user {0} joining family {1}", userId, familyId));
                            }
                        }
                        catch (Exception ex)
                        {
                            // Don't fail the join if welcome message fails
                            System.Diagnostics.Debug.WriteLine(string.Format("AddFamilyMember: Failed to post welcome message: {0}", ex.Message));
                        }
                    }

                    System.Diagnostics.Debug.WriteLine(string.Format("User {0} added to family {1} as {2}", userId, familyId, role));
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("AddFamilyMember error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("AddFamilyMember inner error: " + ex.InnerException.Message);
                }
                return false;
            }
        }

        /// <summary>
        /// Checks if a user is a member of a family
        /// </summary>
        public static bool IsFamilyMember(int familyId, int userId)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*) FROM [dbo].[FamilyMembers]
                    WHERE FamilyId = @FamilyId AND UserId = @UserId AND IsActive = 1";

                object count = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@UserId", userId));

                return Convert.ToInt32(count) > 0;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the family ID for a user
        /// </summary>
        public static int? GetUserFamilyId(int userId)
        {
            try
            {
                string query = @"
                    SELECT FamilyId FROM [dbo].[FamilyMembers]
                    WHERE UserId = @UserId AND IsActive = 1";

                object result = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@UserId", userId));

                if (result != null && result != DBNull.Value)
                {
                    return Convert.ToInt32(result);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets family information
        /// </summary>
        public static DataRow GetFamilyInfo(int familyId)
        {
            try
            {
                string query = @"
                    SELECT Id, Name, FamilyCode, OwnerId, TreasuryPoints, CreatedDate
                    FROM [dbo].[Families]
                    WHERE Id = @FamilyId";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@FamilyId", familyId)))
                {
                    if (dt.Rows.Count > 0)
                    {
                        return dt.Rows[0];
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetFamilyInfo error: " + ex.Message);
            }
            return null;
        }

        /// <summary>
        /// Checks if user is the family owner
        /// </summary>
        public static bool IsFamilyOwner(int familyId, int userId)
        {
            try
            {
                string query = @"
                    SELECT OwnerId FROM [dbo].[Families]
                    WHERE Id = @FamilyId";

                object ownerId = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@FamilyId", familyId));

                if (ownerId != null)
                {
                    return Convert.ToInt32(ownerId) == userId;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets count of children in a family
        /// </summary>
        public static int GetChildrenCount(int familyId)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*) FROM [dbo].[FamilyMembers]
                    WHERE FamilyId = @FamilyId AND Role = 'CHILD' AND IsActive = 1";

                object count = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@FamilyId", familyId));

                return Convert.ToInt32(count);
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// Removes a user from a family
        /// </summary>
        public static bool LeaveFamily(int userId)
        {
            try
            {
                int? familyId = GetUserFamilyId(userId);
                if (!familyId.HasValue)
                {
                    return false;
                }

                // Check if user is owner
                if (IsFamilyOwner(familyId.Value, userId))
                {
                    // Check if there are children
                    int childrenCount = GetChildrenCount(familyId.Value);
                    if (childrenCount > 0)
                    {
                        return false; // Owner cannot leave if children exist
                    }
                }

                // Deactivate family membership
                string query = @"
                    UPDATE [dbo].[FamilyMembers]
                    SET IsActive = 0
                    WHERE UserId = @UserId AND FamilyId = @FamilyId";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@FamilyId", familyId.Value));

                if (rowsAffected > 0)
                {
                    // If child leaves, reset points to 0
                    var userInfo = AuthenticationHelper.GetUserById(userId);
                    if (userInfo != null && userInfo["Role"].ToString() == "CHILD")
                    {
                        ResetUserPoints(userId);
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LeaveFamily error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Resets user points to 0
        /// </summary>
        private static void ResetUserPoints(int userId)
        {
            try
            {
                // Delete all point transactions for the user
                string deleteQuery = "DELETE FROM [dbo].[PointTransactions] WHERE UserId = @UserId";
                DatabaseHelper.ExecuteNonQuery(deleteQuery, new SqlParameter("@UserId", userId));
                
                System.Diagnostics.Debug.WriteLine(string.Format("Points reset for user {0}", userId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ResetUserPoints error: " + ex.Message);
            }
        }

        /// <summary>
        /// Transfers family ownership to another parent
        /// </summary>
        public static bool TransferOwnership(int familyId, int newOwnerId, int currentOwnerId)
        {
            try
            {
                // Verify current user is owner
                if (!IsFamilyOwner(familyId, currentOwnerId))
                {
                    return false;
                }

                // Verify new owner is a parent in the family
                if (!IsFamilyMember(familyId, newOwnerId))
                {
                    return false;
                }

                // Get new owner's role
                string query = @"
                    SELECT Role FROM [dbo].[FamilyMembers] fm
                    INNER JOIN [dbo].[Users] u ON fm.UserId = u.Id
                    WHERE fm.FamilyId = @FamilyId AND fm.UserId = @UserId";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@UserId", newOwnerId)))
                {
                    if (dt.Rows.Count == 0)
                    {
                        return false;
                    }

                    string role = dt.Rows[0]["Role"].ToString();
                    if (role != "PARENT")
                    {
                        return false; // Cannot transfer to child
                    }
                }

                // Update owner
                string updateQuery = @"
                    UPDATE [dbo].[Families]
                    SET OwnerId = @NewOwnerId
                    WHERE Id = @FamilyId AND OwnerId = @CurrentOwnerId";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(updateQuery,
                    new SqlParameter("@NewOwnerId", newOwnerId),
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@CurrentOwnerId", currentOwnerId));

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("TransferOwnership error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets children in a family with their statistics (points, completed tasks, failed tasks, profile picture)
        /// </summary>
        public static DataTable GetFamilyChildrenWithStats(int familyId)
        {
            try
            {
                string query = @"
                    SELECT 
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.ProfilePicture,
                        u.IsBanned,
                        ISNULL(u.Points, 0) AS TotalPoints,
                        (SELECT COUNT(*) FROM [dbo].[TaskReviews] tr 
                         INNER JOIN [dbo].[TaskAssignments] ta ON tr.TaskAssignmentId = ta.Id 
                         WHERE ta.UserId = u.Id AND tr.IsFailed = 0) AS CompletedTasks,
                        (SELECT COUNT(*) FROM [dbo].[TaskReviews] tr 
                         INNER JOIN [dbo].[TaskAssignments] ta ON tr.TaskAssignmentId = ta.Id 
                         WHERE ta.UserId = u.Id AND tr.IsFailed = 1) AS FailedTasks
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
                System.Diagnostics.Debug.WriteLine("GetFamilyChildrenWithStats error: " + ex.Message);
                return new DataTable();
            }
        }

        /// <summary>
        /// Bans or unbans a child (prevents them from receiving tasks)
        /// </summary>
        public static bool BanUnbanChild(int childId, bool isBanned)
        {
            try
            {
                string query = @"
                    UPDATE [dbo].[Users]
                    SET IsBanned = @IsBanned
                    WHERE Id = @ChildId AND Role = 'CHILD'";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@IsBanned", isBanned),
                    new SqlParameter("@ChildId", childId));

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BanUnbanChild error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Removes a child from the family (deactivates membership and resets points)
        /// </summary>
        public static bool RemoveChildFromFamily(int familyId, int childId)
        {
            try
            {
                // Verify child is in the family
                if (!IsFamilyMember(familyId, childId))
                {
                    return false;
                }

                // Verify user is a child
                var userInfo = AuthenticationHelper.GetUserById(childId);
                if (userInfo == null || userInfo["Role"].ToString() != "CHILD")
                {
                    return false;
                }

                // Deactivate family membership
                string query = @"
                    UPDATE [dbo].[FamilyMembers]
                    SET IsActive = 0
                    WHERE UserId = @ChildId AND FamilyId = @FamilyId";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@ChildId", childId),
                    new SqlParameter("@FamilyId", familyId));

                if (rowsAffected > 0)
                {
                    // Reset child's points
                    ResetUserPoints(childId);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("RemoveChildFromFamily error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets the family owner ID
        /// </summary>
        public static int? GetFamilyOwnerId(int familyId)
        {
            try
            {
                string query = @"
                    SELECT OwnerId FROM [dbo].[Families]
                    WHERE Id = @FamilyId";

                object ownerId = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@FamilyId", familyId));

                if (ownerId != null && ownerId != DBNull.Value)
                {
                    return Convert.ToInt32(ownerId);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetFamilyOwnerId error: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets all family members with their details (for sidebar display)
        /// </summary>
        public static DataTable GetFamilyMembers(int familyId)
        {
            try
            {
                string query = @"
                    SELECT 
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.ProfilePicture,
                        u.Role,
                        fm.JoinedDate AS JoinDate,
                        f.OwnerId,
                        CASE WHEN f.OwnerId = u.Id THEN 1 ELSE 0 END AS IsOwner
                    FROM [dbo].[FamilyMembers] fm
                    INNER JOIN [dbo].[Users] u ON fm.UserId = u.Id
                    INNER JOIN [dbo].[Families] f ON fm.FamilyId = f.Id
                    WHERE fm.FamilyId = @FamilyId 
                      AND fm.IsActive = 1 
                      AND u.IsActive = 1
                    ORDER BY 
                        CASE WHEN f.OwnerId = u.Id THEN 0 ELSE 1 END,
                        u.Role DESC,
                        fm.JoinedDate ASC";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@FamilyId", familyId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetFamilyMembers error: " + ex.Message);
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets children with stats for hover tooltip
        /// </summary>
        public static DataTable GetChildrenWithStats(int familyId)
        {
            try
            {
                int? ownerId = GetFamilyOwnerId(familyId);
                if (!ownerId.HasValue)
                {
                    return new DataTable();
                }

                string query = @"
                    SELECT
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.ProfilePicture,
                        ISNULL(u.Points, 0) AS TotalPoints,
                        (SELECT COUNT(*) FROM [dbo].[TaskReviews] tr
                         WHERE tr.ReviewedBy = @FamilyOwnerId AND tr.TaskAssignmentId IN (SELECT Id FROM [dbo].[TaskAssignments] WHERE UserId = u.Id) AND tr.IsFailed = 0) AS CompletedTasks,
                        (SELECT COUNT(*) FROM [dbo].[TaskReviews] tr
                         WHERE tr.ReviewedBy = @FamilyOwnerId AND tr.TaskAssignmentId IN (SELECT Id FROM [dbo].[TaskAssignments] WHERE UserId = u.Id) AND tr.IsFailed = 1) AS FailedTasks
                    FROM [dbo].[FamilyMembers] fm
                    INNER JOIN [dbo].[Users] u ON fm.UserId = u.Id
                    WHERE fm.FamilyId = @FamilyId
                      AND fm.Role = 'CHILD'
                      AND fm.IsActive = 1
                      AND u.IsActive = 1
                    ORDER BY u.FirstName, u.LastName";

                return DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@FamilyOwnerId", ownerId.Value));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetChildrenWithStats error: " + ex.Message);
                return new DataTable();
            }
        }

        /// <summary>
        /// Kicks a parent from the family (only owner can do this)
        /// </summary>
        public static bool KickParent(int familyId, int parentId, int ownerId)
        {
            try
            {
                // Verify current user is owner
                if (!IsFamilyOwner(familyId, ownerId))
                {
                    System.Diagnostics.Debug.WriteLine("KickParent: User is not owner");
                    return false;
                }

                // Verify parent is not the owner
                if (parentId == ownerId)
                {
                    System.Diagnostics.Debug.WriteLine("KickParent: Cannot kick owner");
                    return false;
                }

                // Verify parent is in the family
                if (!IsFamilyMember(familyId, parentId))
                {
                    System.Diagnostics.Debug.WriteLine("KickParent: Parent not in family");
                    return false;
                }

                // Verify user is a parent
                var userInfo = AuthenticationHelper.GetUserById(parentId);
                if (userInfo == null || userInfo["Role"].ToString() != "PARENT")
                {
                    System.Diagnostics.Debug.WriteLine("KickParent: User is not a parent");
                    return false;
                }

                // Deactivate family membership
                string query = @"
                    UPDATE [dbo].[FamilyMembers]
                    SET IsActive = 0
                    WHERE UserId = @ParentId AND FamilyId = @FamilyId";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@ParentId", parentId),
                    new SqlParameter("@FamilyId", familyId));

                System.Diagnostics.Debug.WriteLine(string.Format("KickParent: Removed parent {0} from family {1}, rows affected: {2}", parentId, familyId, rowsAffected));
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("KickParent error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Changes the family code (only owner can do this)
        /// </summary>
        public static bool ChangeFamilyCode(int familyId, int ownerId)
        {
            try
            {
                // Verify current user is owner
                if (!IsFamilyOwner(familyId, ownerId))
                {
                    System.Diagnostics.Debug.WriteLine("ChangeFamilyCode: User is not owner");
                    return false;
                }

                // Generate new unique family code
                string newCode = GenerateFamilyCode();
                System.Diagnostics.Debug.WriteLine(string.Format("ChangeFamilyCode: Generated new code {0} for family {1}", newCode, familyId));

                // Update family code
                string query = @"
                    UPDATE [dbo].[Families]
                    SET FamilyCode = @FamilyCode
                    WHERE Id = @FamilyId AND OwnerId = @OwnerId";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@FamilyCode", newCode),
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@OwnerId", ownerId));

                System.Diagnostics.Debug.WriteLine(string.Format("ChangeFamilyCode: Updated family code, rows affected: {0}", rowsAffected));
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ChangeFamilyCode error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Checks if owner can leave the family (no children)
        /// </summary>
        public static bool CanOwnerLeave(int familyId)
        {
            try
            {
                int childrenCount = GetChildrenCount(familyId);
                System.Diagnostics.Debug.WriteLine(string.Format("CanOwnerLeave: Family {0} has {1} children", familyId, childrenCount));
                return childrenCount == 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CanOwnerLeave error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets the first parent by join date (for auto-transfer ownership)
        /// </summary>
        public static int? GetFirstParentByJoinDate(int familyId)
        {
            try
            {
                string query = @"
                    SELECT TOP 1 u.Id
                    FROM [dbo].[FamilyMembers] fm
                    INNER JOIN [dbo].[Users] u ON fm.UserId = u.Id
                    WHERE fm.FamilyId = @FamilyId 
                      AND u.Role = 'PARENT'
                      AND fm.IsActive = 1
                      AND u.IsActive = 1
                    ORDER BY fm.CreatedDate ASC";

                object parentId = DatabaseHelper.ExecuteScalar(query,
                    new SqlParameter("@FamilyId", familyId));

                if (parentId != null && parentId != DBNull.Value)
                {
                    return Convert.ToInt32(parentId);
                }

                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetFirstParentByJoinDate error: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Owner leaves family (with auto-transfer if needed)
        /// </summary>
        public static bool OwnerLeaveFamily(int familyId, int ownerId)
        {
            try
            {
                // Verify user is owner
                if (!IsFamilyOwner(familyId, ownerId))
                {
                    System.Diagnostics.Debug.WriteLine("OwnerLeaveFamily: User is not owner");
                    return false;
                }

                // Check if there are children
                int childrenCount = GetChildrenCount(familyId);
                if (childrenCount > 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("OwnerLeaveFamily: Cannot leave - family has {0} children", childrenCount));
                    return false;
                }

                // Get first parent by join date for auto-transfer
                int? newOwnerId = GetFirstParentByJoinDate(familyId);
                
                if (newOwnerId.HasValue && newOwnerId.Value != ownerId)
                {
                    // Transfer ownership to first parent
                    System.Diagnostics.Debug.WriteLine(string.Format("OwnerLeaveFamily: Auto-transferring ownership to parent {0}", newOwnerId.Value));
                    if (!TransferOwnership(familyId, newOwnerId.Value, ownerId))
                    {
                        System.Diagnostics.Debug.WriteLine("OwnerLeaveFamily: Failed to transfer ownership");
                        return false;
                    }
                }
                else if (!newOwnerId.HasValue)
                {
                    // No other parents - family will be orphaned (this shouldn't happen, but handle it)
                    System.Diagnostics.Debug.WriteLine("OwnerLeaveFamily: WARNING - No other parents found, family will be orphaned");
                }

                // Deactivate owner's family membership
                string query = @"
                    UPDATE [dbo].[FamilyMembers]
                    SET IsActive = 0
                    WHERE UserId = @OwnerId AND FamilyId = @FamilyId";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@OwnerId", ownerId),
                    new SqlParameter("@FamilyId", familyId));

                System.Diagnostics.Debug.WriteLine(string.Format("OwnerLeaveFamily: Owner {0} left family {1}, rows affected: {2}", ownerId, familyId, rowsAffected));
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("OwnerLeaveFamily error: " + ex.Message);
                return false;
            }
        }
    }
}

