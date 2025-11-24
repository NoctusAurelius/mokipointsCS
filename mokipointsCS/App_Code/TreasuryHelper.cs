using System;
using System.Data;
using System.Data.SqlClient;

namespace mokipointsCS
{
    /// <summary>
    /// Treasury Helper class for managing family treasury operations
    /// Handles all treasury balance and transaction management
    /// </summary>
    public class TreasuryHelper
    {
        /// <summary>
        /// Gets the current treasury balance for a family
        /// If treasury doesn't exist or has 0 balance, initializes it with starting balance
        /// </summary>
        public static int GetTreasuryBalance(int familyId)
        {
            try
            {
                string query = @"
                    SELECT Balance FROM [dbo].[FamilyTreasury]
                    WHERE FamilyId = @FamilyId";

                object result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@FamilyId", familyId));
                
                if (result != null)
                {
                    int balance = Convert.ToInt32(result);
                    
                    // If treasury exists but has 0 balance, check if it's a new treasury (no transactions)
                    // This handles existing families that were created before treasury initialization
                    if (balance == 0)
                    {
                        string checkTransactionsQuery = @"
                            SELECT COUNT(*) FROM [dbo].[TreasuryTransactions]
                            WHERE FamilyId = @FamilyId";
                        
                        object transactionCount = DatabaseHelper.ExecuteScalar(checkTransactionsQuery, new SqlParameter("@FamilyId", familyId));
                        int count = transactionCount != null ? Convert.ToInt32(transactionCount) : 0;
                        
                        // If no transactions exist, this is a new treasury - set starting balance
                        if (count == 0)
                        {
                            System.Diagnostics.Debug.WriteLine(string.Format("GetTreasuryBalance: Treasury exists but balance is 0 with no transactions for FamilyId={0}, setting to starting balance", familyId));
                            SetTreasuryBalance(familyId, 1000000, "Initial treasury balance");
                            return 1000000;
                        }
                    }
                    
                    return balance;
                }
                
                // Treasury doesn't exist, initialize it with starting balance
                System.Diagnostics.Debug.WriteLine(string.Format("GetTreasuryBalance: Treasury doesn't exist for FamilyId={0}, initializing with starting balance", familyId));
                InitializeTreasury(familyId, 1000000);
                return 1000000;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetTreasuryBalance error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return 0;
            }
        }

        /// <summary>
        /// Sets the treasury balance directly (used for initialization/migration)
        /// </summary>
        public static bool SetTreasuryBalance(int familyId, int balance, string description)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Check if treasury exists
                            string checkQuery = @"
                                SELECT Id, Balance FROM [dbo].[FamilyTreasury]
                                WHERE FamilyId = @FamilyId
                                WITH (UPDLOCK, ROWLOCK)";

                            int treasuryId = 0;
                            int currentBalance = 0;
                            using (SqlCommand cmd = new SqlCommand(checkQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@FamilyId", familyId);
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        treasuryId = reader.GetInt32(0);
                                        currentBalance = reader.GetInt32(1);
                                    }
                                }
                            }

                            if (treasuryId == 0)
                            {
                                // Create treasury
                                string insertQuery = @"
                                    INSERT INTO [dbo].[FamilyTreasury] (FamilyId, Balance, LastUpdated)
                                    VALUES (@FamilyId, @Balance, GETDATE());
                                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                                using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@FamilyId", familyId);
                                    cmd.Parameters.AddWithValue("@Balance", balance);
                                    treasuryId = Convert.ToInt32(cmd.ExecuteScalar());
                                }
                            }
                            else
                            {
                                // Update balance
                                string updateQuery = @"
                                    UPDATE [dbo].[FamilyTreasury]
                                    SET Balance = @Balance, LastUpdated = GETDATE()
                                    WHERE Id = @TreasuryId";

                                using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@TreasuryId", treasuryId);
                                    cmd.Parameters.AddWithValue("@Balance", balance);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            // Record transaction
                            int difference = balance - currentBalance;
                            if (difference != 0)
                            {
                                string transactionType = difference > 0 ? "Deposit" : "Withdrawal";
                                string insertTransactionQuery = @"
                                    INSERT INTO [dbo].[TreasuryTransactions] 
                                    (FamilyId, TransactionType, Amount, BalanceAfter, Description, CreatedDate)
                                    VALUES (@FamilyId, @TransactionType, @Amount, @BalanceAfter, @Description, GETDATE())";

                                using (SqlCommand cmd = new SqlCommand(insertTransactionQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@FamilyId", familyId);
                                    cmd.Parameters.AddWithValue("@TransactionType", transactionType);
                                    cmd.Parameters.AddWithValue("@Amount", Math.Abs(difference));
                                    cmd.Parameters.AddWithValue("@BalanceAfter", balance);
                                    cmd.Parameters.AddWithValue("@Description", description ?? "Treasury balance set");
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            System.Diagnostics.Debug.WriteLine(string.Format("SetTreasuryBalance: Set balance to {0} for FamilyId={1}", balance, familyId));
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
                System.Diagnostics.Debug.WriteLine(string.Format("SetTreasuryBalance error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Initializes treasury for a family if it doesn't exist
        /// </summary>
        public static void InitializeTreasury(int familyId, int startingBalance = 1000000)
        {
            try
            {
                string checkQuery = @"
                    SELECT Id FROM [dbo].[FamilyTreasury]
                    WHERE FamilyId = @FamilyId";

                object exists = DatabaseHelper.ExecuteScalar(checkQuery, new SqlParameter("@FamilyId", familyId));

                if (exists == null)
                {
                    string insertQuery = @"
                        INSERT INTO [dbo].[FamilyTreasury] (FamilyId, Balance, LastUpdated)
                        VALUES (@FamilyId, @StartingBalance, GETDATE())";

                    DatabaseHelper.ExecuteNonQuery(insertQuery, 
                        new SqlParameter("@FamilyId", familyId),
                        new SqlParameter("@StartingBalance", startingBalance));
                    System.Diagnostics.Debug.WriteLine(string.Format("InitializeTreasury: Created treasury for FamilyId={0} with starting balance {1}", familyId, startingBalance));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("InitializeTreasury error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
            }
        }

        /// <summary>
        /// Deposits points to treasury
        /// Uses transaction with row-level locking for concurrency safety
        /// </summary>
        public static bool DepositToTreasury(int familyId, int amount, string description, int? relatedOrderId, int? createdBy)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            bool result = DepositToTreasury(familyId, amount, description, relatedOrderId, createdBy, conn, transaction);
                            if (result)
                            {
                                transaction.Commit();
                            }
                            else
                            {
                                transaction.Rollback();
                            }
                            return result;
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
                System.Diagnostics.Debug.WriteLine(string.Format("DepositToTreasury error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Deposits points to treasury (transaction-aware overload)
        /// </summary>
        public static bool DepositToTreasury(int familyId, int amount, string description, int? relatedOrderId, int? createdBy, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(string.Format("DepositToTreasury: Starting - FamilyId={0}, Amount={1}", familyId, amount));

                // Lock treasury row for update
                string lockQuery = @"
                    SELECT Balance 
                    FROM [dbo].[FamilyTreasury] WITH (UPDLOCK, ROWLOCK)
                    WHERE FamilyId = @FamilyId";

                int currentBalance = 0;
                using (SqlCommand cmd = new SqlCommand(lockQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@FamilyId", familyId);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        currentBalance = Convert.ToInt32(result);
                        System.Diagnostics.Debug.WriteLine(string.Format("DepositToTreasury: Current balance={0} for FamilyId={1}", currentBalance, familyId));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("DepositToTreasury ERROR: Treasury not found for FamilyId={0}", familyId));
                        return false;
                    }
                }

                int newBalance = currentBalance + amount;
                System.Diagnostics.Debug.WriteLine(string.Format("DepositToTreasury: Calculating - BalanceBefore={0}, Amount={1}, BalanceAfter={2}", currentBalance, amount, newBalance));

                // Update treasury balance
                string updateQuery = @"
                    UPDATE [dbo].[FamilyTreasury]
                    SET Balance = @NewBalance, LastUpdated = GETDATE()
                    WHERE FamilyId = @FamilyId";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                    cmd.Parameters.AddWithValue("@FamilyId", familyId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine(string.Format("DepositToTreasury: UPDATE executed - RowsAffected={0}", rowsAffected));
                }

                // Create treasury transaction record
                string insertQuery = @"
                    INSERT INTO [dbo].[TreasuryTransactions] 
                    (FamilyId, TransactionType, Amount, BalanceAfter, Description, RelatedOrderId, CreatedBy, CreatedDate)
                    VALUES (@FamilyId, 'Deposit', @Amount, @BalanceAfter, @Description, @RelatedOrderId, @CreatedBy, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@FamilyId", familyId);
                    cmd.Parameters.AddWithValue("@Amount", amount);
                    cmd.Parameters.AddWithValue("@BalanceAfter", newBalance);
                    cmd.Parameters.AddWithValue("@Description", description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RelatedOrderId", relatedOrderId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? (object)DBNull.Value);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine(string.Format("DepositToTreasury: Transaction record inserted - RowsAffected={0}", rowsAffected));
                }

                System.Diagnostics.Debug.WriteLine(string.Format("DepositToTreasury SUCCESS: FamilyId={0}, Amount={1}, BalanceBefore={2}, BalanceAfter={3}", familyId, amount, currentBalance, newBalance));
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("DepositToTreasury error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                throw;
            }
        }

        /// <summary>
        /// Withdraws points from treasury
        /// Uses transaction with row-level locking for concurrency safety
        /// </summary>
        public static bool WithdrawFromTreasury(int familyId, int amount, string description, int? relatedTaskAssignmentId, int? createdBy)
        {
            try
            {
                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            bool result = WithdrawFromTreasury(familyId, amount, description, relatedTaskAssignmentId, createdBy, conn, transaction);
                            if (result)
                            {
                                transaction.Commit();
                            }
                            else
                            {
                                transaction.Rollback();
                            }
                            return result;
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
                System.Diagnostics.Debug.WriteLine(string.Format("WithdrawFromTreasury error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Withdraws points from treasury (transaction-aware overload)
        /// </summary>
        public static bool WithdrawFromTreasury(int familyId, int amount, string description, int? relatedTaskAssignmentId, int? createdBy, SqlConnection conn, SqlTransaction transaction)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine(string.Format("WithdrawFromTreasury: Starting - FamilyId={0}, Amount={1}", familyId, amount));

                // Lock treasury row for update
                string lockQuery = @"
                    SELECT Balance 
                    FROM [dbo].[FamilyTreasury] WITH (UPDLOCK, ROWLOCK)
                    WHERE FamilyId = @FamilyId";

                int currentBalance = 0;
                using (SqlCommand cmd = new SqlCommand(lockQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@FamilyId", familyId);
                    object result = cmd.ExecuteScalar();
                    if (result != null)
                    {
                        currentBalance = Convert.ToInt32(result);
                        System.Diagnostics.Debug.WriteLine(string.Format("WithdrawFromTreasury: Current balance={0} for FamilyId={1}", currentBalance, familyId));
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("WithdrawFromTreasury ERROR: Treasury not found for FamilyId={0}", familyId));
                        return false;
                    }
                }

                // Check if sufficient balance
                if (currentBalance < amount)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("WithdrawFromTreasury ERROR: Insufficient balance - FamilyId={0}, Required={1}, Available={2}", familyId, amount, currentBalance));
                    return false;
                }

                int newBalance = currentBalance - amount;
                System.Diagnostics.Debug.WriteLine(string.Format("WithdrawFromTreasury: Calculating - BalanceBefore={0}, Amount={1}, BalanceAfter={2}", currentBalance, amount, newBalance));

                // Update treasury balance
                string updateQuery = @"
                    UPDATE [dbo].[FamilyTreasury]
                    SET Balance = @NewBalance, LastUpdated = GETDATE()
                    WHERE FamilyId = @FamilyId";

                using (SqlCommand cmd = new SqlCommand(updateQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@NewBalance", newBalance);
                    cmd.Parameters.AddWithValue("@FamilyId", familyId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine(string.Format("WithdrawFromTreasury: UPDATE executed - RowsAffected={0}", rowsAffected));
                }

                // Create treasury transaction record
                string insertQuery = @"
                    INSERT INTO [dbo].[TreasuryTransactions] 
                    (FamilyId, TransactionType, Amount, BalanceAfter, Description, RelatedTaskAssignmentId, CreatedBy, CreatedDate)
                    VALUES (@FamilyId, 'Withdrawal', @Amount, @BalanceAfter, @Description, @RelatedTaskAssignmentId, @CreatedBy, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@FamilyId", familyId);
                    cmd.Parameters.AddWithValue("@Amount", -amount); // Negative for withdrawal
                    cmd.Parameters.AddWithValue("@BalanceAfter", newBalance);
                    cmd.Parameters.AddWithValue("@Description", description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@RelatedTaskAssignmentId", relatedTaskAssignmentId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? (object)DBNull.Value);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    System.Diagnostics.Debug.WriteLine(string.Format("WithdrawFromTreasury: Transaction record inserted - RowsAffected={0}", rowsAffected));
                }

                System.Diagnostics.Debug.WriteLine(string.Format("WithdrawFromTreasury SUCCESS: FamilyId={0}, Amount={1}, BalanceBefore={2}, BalanceAfter={3}", familyId, amount, currentBalance, newBalance));
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("WithdrawFromTreasury error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                throw;
            }
        }

        /// <summary>
        /// Gets treasury transaction history
        /// </summary>
        public static DataTable GetTreasuryTransactions(int familyId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                string query = @"
                    SELECT Id, TransactionType, Amount, BalanceAfter, Description, 
                           RelatedOrderId, RelatedTaskAssignmentId, CreatedBy, CreatedDate
                    FROM [dbo].[TreasuryTransactions]
                    WHERE FamilyId = @FamilyId";

                if (startDate.HasValue)
                {
                    query += " AND CreatedDate >= @StartDate";
                }
                if (endDate.HasValue)
                {
                    query += " AND CreatedDate <= @EndDate";
                }

                query += " ORDER BY CreatedDate DESC";

                System.Data.SqlClient.SqlParameter[] parameters = new System.Data.SqlClient.SqlParameter[]
                {
                    new SqlParameter("@FamilyId", familyId)
                };

                if (startDate.HasValue)
                {
                    Array.Resize(ref parameters, parameters.Length + 1);
                    parameters[parameters.Length - 1] = new SqlParameter("@StartDate", startDate.Value);
                }
                if (endDate.HasValue)
                {
                    Array.Resize(ref parameters, parameters.Length + 1);
                    parameters[parameters.Length - 1] = new SqlParameter("@EndDate", endDate.Value);
                }

                return DatabaseHelper.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetTreasuryTransactions error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return new DataTable();
            }
        }
    }
}

