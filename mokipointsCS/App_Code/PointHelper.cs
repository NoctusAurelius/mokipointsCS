using System;
using System.Data;
using System.Data.SqlClient;

namespace mokipointsCS
{
    /// <summary>
    /// Point Helper class for managing user points with cap enforcement and treasury integration
    /// Handles all point transactions with 10,000 point cap and treasury integration
    /// </summary>
    public class PointHelper
    {
        /// <summary>
        /// Awards points to a user with cap enforcement and treasury integration
        /// Points come from treasury, excess goes back to treasury
        /// </summary>
        public static bool AwardPointsWithCap(int userId, int points, int familyId, string description, int? taskAssignmentId)
        {
            try
            {
                if (points <= 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap: Invalid points amount: {0}", points));
                    return false;
                }

                System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap: Starting - UserId={0}, Points={1}, FamilyId={2}", userId, points, familyId));

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Check treasury balance within transaction
                            string treasuryQuery = @"
                                SELECT Balance FROM [dbo].[FamilyTreasury] WITH (UPDLOCK, ROWLOCK)
                                WHERE FamilyId = @FamilyId";
                            
                            int treasuryBalance = 0;
                            using (SqlCommand cmd = new SqlCommand(treasuryQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@FamilyId", familyId);
                                object result = cmd.ExecuteScalar();
                                if (result != null)
                                {
                                    treasuryBalance = Convert.ToInt32(result);
                                }
                                else
                                {
                                    // Treasury doesn't exist, create it within transaction
                                    System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap: Treasury not found, creating for FamilyId={0}", familyId));
                                    string createTreasuryQuery = @"
                                        INSERT INTO [dbo].[FamilyTreasury] (FamilyId, Balance, LastUpdated)
                                        VALUES (@FamilyId, @Balance, GETDATE())";
                                    
                                    using (SqlCommand createCmd = new SqlCommand(createTreasuryQuery, conn, transaction))
                                    {
                                        createCmd.Parameters.AddWithValue("@FamilyId", familyId);
                                        createCmd.Parameters.AddWithValue("@Balance", 1000000);
                                        createCmd.ExecuteNonQuery();
                                    }
                                    treasuryBalance = 1000000;
                                }
                            }

                            System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap: Treasury balance={0} for FamilyId={1}", treasuryBalance, familyId));

                            if (treasuryBalance < points)
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap ERROR: TreasuryInsufficient - FamilyId={0}, Required={1}, Available={2}", familyId, points, treasuryBalance));
                                transaction.Rollback();
                                return false;
                            }

                            // Lock user's points row for update
                            string lockQuery = @"
                                SELECT Points 
                                FROM [dbo].[Users] WITH (UPDLOCK, ROWLOCK)
                                WHERE Id = @UserId";

                            int currentBalance = 0;
                            using (SqlCommand cmd = new SqlCommand(lockQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                object result = cmd.ExecuteScalar();
                                if (result != null)
                                {
                                    currentBalance = Convert.ToInt32(result);
                                    System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap: Current user balance={0} for UserId={1}", currentBalance, userId));
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap ERROR: User not found - UserId={0}", userId));
                                    transaction.Rollback();
                                    return false;
                                }
                            }

                            // Calculate new balance with cap enforcement
                            int attemptedBalance = currentBalance + points;
                            int newBalance = Math.Min(10000, attemptedBalance);
                            int excess = Math.Max(0, attemptedBalance - 10000);

                            System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap: UserId={0}, Points={1}, CurrentBalance={2}, NewBalance={3}, Excess={4}", userId, points, currentBalance, newBalance, excess));

                            // Update user's points
                            string updateQuery = @"
                                UPDATE [dbo].[Users]
                                SET Points = @NewBalance
                                WHERE Id = @UserId";

                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                                cmd.ExecuteNonQuery();
                            }

                            // Create point transaction record
                            string insertTransactionQuery = @"
                                INSERT INTO [dbo].[PointTransactions] 
                                (UserId, Points, TransactionType, Description, TransactionDate, TaskAssignmentId, IsFromTreasury)
                                VALUES (@UserId, @Points, 'Earned', @Description, GETDATE(), @TaskAssignmentId, 1)";

                            int pointTransactionId = 0;
                            using (SqlCommand cmd = new SqlCommand(insertTransactionQuery + "; SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                cmd.Parameters.AddWithValue("@Points", points);
                                var descParam = new SqlParameter("@Description", SqlDbType.NVarChar, 500);
                                descParam.Value = description ?? "Points awarded";
                                cmd.Parameters.Add(descParam);
                                cmd.Parameters.AddWithValue("@TaskAssignmentId", taskAssignmentId ?? (object)DBNull.Value);
                                pointTransactionId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // Withdraw from treasury (only the amount actually awarded, not excess)
                            int treasuryWithdrawal = points - excess;
                            if (treasuryWithdrawal > 0)
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap: Withdrawing {0} from treasury for FamilyId={1}", treasuryWithdrawal, familyId));
                                if (!TreasuryHelper.WithdrawFromTreasury(familyId, treasuryWithdrawal, description ?? "Points awarded to child", taskAssignmentId, null, conn, transaction))
                                {
                                    System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap ERROR: Failed to withdraw from treasury"));
                                    transaction.Rollback();
                                    return false;
                                }
                                System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap: Successfully withdrew {0} from treasury", treasuryWithdrawal));
                            }

                            // If excess, add back to treasury
                            if (excess > 0)
                            {
                                System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap: Returning excess {0} to treasury", excess));
                                // Create excess cap transaction
                                string insertExcessQuery = @"
                                    INSERT INTO [dbo].[PointTransactions] 
                                    (UserId, Points, TransactionType, Description, TransactionDate, ExcessAmount)
                                    VALUES (@UserId, @Points, 'ExcessCap', @Description, GETDATE(), @ExcessAmount)";

                                using (SqlCommand cmd = new SqlCommand(insertExcessQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@UserId", userId);
                                    cmd.Parameters.AddWithValue("@Points", 0); // No points to user
                                    cmd.Parameters.AddWithValue("@Description", string.Format("Excess points ({0}) returned to treasury", excess));
                                    cmd.Parameters.AddWithValue("@ExcessAmount", excess);
                                    cmd.ExecuteNonQuery();
                                }

                                if (!TreasuryHelper.DepositToTreasury(familyId, excess, string.Format("Excess cap return for user {0}", userId), null, null, conn, transaction))
                                {
                                    System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap ERROR: Failed to deposit excess to treasury"));
                                    transaction.Rollback();
                                    return false;
                                }
                                System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap: Successfully deposited excess {0} to treasury", excess));
                            }

                            transaction.Commit();
                            System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap SUCCESS: UserId={0}, PointsAwarded={1}, FinalBalance={2}, Excess={3}", userId, points, newBalance, excess));
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("AwardPointsWithCap error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Deducts points from a user (cannot go negative)
        /// Points go to treasury
        /// </summary>
        public static bool DeductPoints(int userId, int points, int familyId, string description, int? orderId, int? taskAssignmentId = null)
        {
            try
            {
                if (points <= 0)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("DeductPoints: Invalid points amount: {0}", points));
                    return false;
                }

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Lock user's points row for update
                            string lockQuery = @"
                                SELECT Points 
                                FROM [dbo].[Users] WITH (UPDLOCK, ROWLOCK)
                                WHERE Id = @UserId";

                            int currentBalance = 0;
                            using (SqlCommand cmd = new SqlCommand(lockQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                object result = cmd.ExecuteScalar();
                                if (result != null)
                                {
                                    currentBalance = Convert.ToInt32(result);
                                }
                            }

                            // Calculate new balance (cannot go negative)
                            int newBalance = Math.Max(0, currentBalance - points);
                            int actualDeducted = Math.Min(points, currentBalance);

                            System.Diagnostics.Debug.WriteLine(string.Format("DeductPoints: UserId={0}, Points={1}, CurrentBalance={2}, NewBalance={3}, ActualDeducted={4}", userId, points, currentBalance, newBalance, actualDeducted));

                            // Update user's points
                            string updateQuery = @"
                                UPDATE [dbo].[Users]
                                SET Points = @NewBalance
                                WHERE Id = @UserId";

                            using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                                cmd.ExecuteNonQuery();
                            }

                            // Create point transaction record
                            string insertTransactionQuery = @"
                                INSERT INTO [dbo].[PointTransactions] 
                                (UserId, Points, TransactionType, Description, TransactionDate, IsToTreasury)
                                VALUES (@UserId, @Points, 'Spent', @Description, GETDATE(), 1)";

                            using (SqlCommand cmd = new SqlCommand(insertTransactionQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserId", userId);
                                cmd.Parameters.AddWithValue("@Points", -actualDeducted);
                                cmd.Parameters.AddWithValue("@Description", description ?? "Points deducted");
                                cmd.ExecuteNonQuery();
                            }

                            // Add to treasury (only actual deducted amount)
                            if (actualDeducted > 0)
                            {
                                // Pass taskAssignmentId as relatedTaskAssignmentId, orderId as relatedOrderId
                                if (!TreasuryHelper.DepositToTreasury(familyId, actualDeducted, description ?? "Points deducted from child", orderId, taskAssignmentId, null, conn, transaction))
                                {
                                    System.Diagnostics.Debug.WriteLine(string.Format("DeductPoints ERROR: Failed to deposit to treasury"));
                                    transaction.Rollback();
                                    return false;
                                }
                                System.Diagnostics.Debug.WriteLine(string.Format("DeductPoints: Successfully deposited {0} to treasury", actualDeducted));
                            }

                            transaction.Commit();
                            System.Diagnostics.Debug.WriteLine(string.Format("DeductPoints SUCCESS: UserId={0}, PointsDeducted={1}, FinalBalance={2}", userId, actualDeducted, newBalance));
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("DeductPoints error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Gets current point balance for a user
        /// </summary>
        public static int GetChildBalance(int userId)
        {
            try
            {
                string query = "SELECT Points FROM [dbo].[Users] WHERE Id = @UserId";
                object result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@UserId", userId));
                return result != null ? Convert.ToInt32(result) : 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetChildBalance error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return 0;
            }
        }

        /// <summary>
        /// Checks if child can afford a purchase
        /// </summary>
        public static bool CanAffordPurchase(int userId, int totalPoints)
        {
            try
            {
                int balance = GetChildBalance(userId);
                return balance >= totalPoints;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("CanAffordPurchase error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Adds a point transaction record (for backward compatibility)
        /// This method is used by existing code that just records transactions
        /// </summary>
        public static void AddPointTransaction(int userId, int points, string transactionType, string description, int? taskAssignmentId)
        {
            try
            {
                string query = @"
                    INSERT INTO [dbo].[PointTransactions] 
                    (UserId, Points, TransactionType, Description, TransactionDate, TaskAssignmentId)
                    VALUES (@UserId, @Points, @TransactionType, @Description, GETDATE(), @TaskAssignmentId)";

                DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@Points", Math.Abs(points)), // Store absolute value for compatibility
                    new SqlParameter("@TransactionType", transactionType),
                    new SqlParameter("@Description", description ?? "Point transaction"),
                    new SqlParameter("@TaskAssignmentId", taskAssignmentId.HasValue ? (object)taskAssignmentId.Value : DBNull.Value));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("AddPointTransaction error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
            }
        }

        /// <summary>
        /// Gets point transaction history for a user
        /// </summary>
        public static DataTable GetUserPointTransactions(int userId, int? limit = null)
        {
            try
            {
                string query = @"
                    SELECT Id, Points, TransactionType, Description, TransactionDate, 
                           TaskAssignmentId, IsFromTreasury, IsToTreasury, ExcessAmount
                    FROM [dbo].[PointTransactions]
                    WHERE UserId = @UserId
                    ORDER BY TransactionDate DESC";

                if (limit.HasValue)
                {
                    query = string.Format("SELECT TOP {0} * FROM ({1}) AS SubQuery", limit.Value, query);
                }

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@UserId", userId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetUserPointTransactions error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return new DataTable();
            }
        }
    }
}

