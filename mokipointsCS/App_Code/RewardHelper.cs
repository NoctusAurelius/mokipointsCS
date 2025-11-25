using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace mokipointsCS
{
    /// <summary>
    /// Reward Helper class for managing rewards and orders
    /// Handles all reward CRUD operations and order management
    /// </summary>
    public class RewardHelper
    {
        #region Reward Management

        /// <summary>
        /// Creates a new reward
        /// </summary>
        public static bool CreateReward(int familyId, int createdBy, string name, string description, int pointCost, string category, string imageUrl)
        {
            try
            {
                string query = @"
                    INSERT INTO [dbo].[Rewards] 
                    (FamilyId, Name, Description, PointCost, Category, ImageUrl, IsActive, IsDeleted, AvailabilityStatus, CreatedBy, CreatedDate)
                    VALUES (@FamilyId, @Name, @Description, @PointCost, @Category, @ImageUrl, 1, 0, 'Available', @CreatedBy, GETDATE())";

                int rows = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@FamilyId", familyId),
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Description", description ?? (object)DBNull.Value),
                    new SqlParameter("@PointCost", pointCost),
                    new SqlParameter("@Category", category ?? (object)DBNull.Value),
                    new SqlParameter("@ImageUrl", imageUrl ?? (object)DBNull.Value),
                    new SqlParameter("@CreatedBy", createdBy));

                System.Diagnostics.Debug.WriteLine(string.Format("CreateReward: FamilyId={0}, CreatedBy={1}, Name={2}, PointCost={3}", familyId, createdBy, name, pointCost));
                
                // Post system message to family chat
                if (rows > 0)
                {
                    try
                    {
                        string chatMessage = string.Format("New reward available: '{0}' for {1} points!", name, pointCost);
                        
                        // Create JSON data for system event
                        string safeName = (name ?? "").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
                        string safeDescription = (description ?? "").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
                        string safeCategory = (category ?? "").Replace("\"", "\\\"").Replace("\n", "\\n").Replace("\r", "\\r");
                        string systemEventData = string.Format("{{\"RewardId\":{0},\"RewardName\":\"{1}\",\"PointCost\":{2},\"Description\":\"{3}\",\"Category\":\"{4}\"}}",
                            -1, safeName, pointCost, safeDescription, safeCategory);
                        
                        ChatHelper.PostSystemMessage(familyId, "RewardAdded", chatMessage, systemEventData);
                    }
                    catch (Exception chatEx)
                    {
                        // Don't fail reward creation if chat message fails
                        System.Diagnostics.Debug.WriteLine("CreateReward: Failed to post chat message: " + chatEx.Message);
                    }
                }
                
                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("CreateReward error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Gets all active rewards for a family
        /// For children: only shows Available and OutOfStock (not Hidden)
        /// For parents: shows all rewards regardless of availability status
        /// </summary>
        public static DataTable GetFamilyRewards(int familyId, bool activeOnly = true, bool forChild = false)
        {
            try
            {
                string query = @"
                    SELECT Id, Name, Description, PointCost, Category, ImageUrl, IsActive, AvailabilityStatus, CreatedDate, CreatedBy
                    FROM [dbo].[Rewards]
                    WHERE FamilyId = @FamilyId AND IsDeleted = 0";

                if (activeOnly)
                {
                    query += " AND IsActive = 1";
                }

                // For children, filter out Hidden rewards
                if (forChild)
                {
                    query += " AND AvailabilityStatus != 'Hidden'";
                }

                query += " ORDER BY CreatedDate DESC";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@FamilyId", familyId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetFamilyRewards error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets reward details by ID
        /// </summary>
        public static DataRow GetRewardDetails(int rewardId)
        {
            try
            {
                string query = @"
                    SELECT Id, FamilyId, Name, Description, PointCost, Category, ImageUrl, 
                           IsActive, IsDeleted, AvailabilityStatus, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate
                    FROM [dbo].[Rewards]
                    WHERE Id = @RewardId AND IsDeleted = 0";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@RewardId", rewardId));
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetRewardDetails error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return null;
            }
        }

        /// <summary>
        /// Checks if reward has orders that have been checked out
        /// Returns true if reward exists in any order with Status IN ('Pending', 'WaitingToFulfill', 'Fulfilled', 'NotFulfilled')
        /// </summary>
        public static bool HasCheckedOutOrders(int rewardId)
        {
            try
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM RewardOrderItems roi
                    INNER JOIN RewardOrders ro ON roi.OrderId = ro.Id
                    WHERE roi.RewardId = @RewardId
                      AND ro.Status IN ('Pending', 'WaitingToFulfill', 'Fulfilled', 'NotFulfilled')";

                object result = DatabaseHelper.ExecuteScalar(query, new SqlParameter("@RewardId", rewardId));
                int count = result != null ? Convert.ToInt32(result) : 0;
                
                System.Diagnostics.Debug.WriteLine(string.Format("HasCheckedOutOrders: RewardId={0}, Result={1}, OrderCount={2}", rewardId, count > 0, count));
                return count > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("HasCheckedOutOrders error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Updates a reward
        /// Returns false if reward has checked-out orders
        /// </summary>
        public static bool UpdateReward(int rewardId, int updatedBy, string name, string description, int pointCost, string category, string imageUrl)
        {
            try
            {
                // Check if reward has checked-out orders
                if (HasCheckedOutOrders(rewardId))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("UpdateReward Validation: HasCheckedOutOrders=true for RewardId={0}", rewardId));
                    return false;
                }

                System.Diagnostics.Debug.WriteLine(string.Format("UpdateReward Validation: HasCheckedOutOrders=false for RewardId={0}", rewardId));

                string query = @"
                    UPDATE [dbo].[Rewards]
                    SET Name = @Name, Description = @Description, PointCost = @PointCost, 
                        Category = @Category, ImageUrl = @ImageUrl, 
                        UpdatedDate = GETDATE(), UpdatedBy = @UpdatedBy
                    WHERE Id = @RewardId AND IsDeleted = 0";

                int rows = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@RewardId", rewardId),
                    new SqlParameter("@Name", name),
                    new SqlParameter("@Description", description ?? (object)DBNull.Value),
                    new SqlParameter("@PointCost", pointCost),
                    new SqlParameter("@Category", category ?? (object)DBNull.Value),
                    new SqlParameter("@ImageUrl", imageUrl ?? (object)DBNull.Value),
                    new SqlParameter("@UpdatedBy", updatedBy));

                System.Diagnostics.Debug.WriteLine(string.Format("UpdateReward: RewardId={0}, UpdatedBy={1}", rewardId, updatedBy));
                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("UpdateReward error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Deletes a reward (soft delete)
        /// Returns false if reward has checked-out orders
        /// </summary>
        public static bool DeleteReward(int rewardId, int deletedBy)
        {
            try
            {
                // Check if reward has checked-out orders
                if (HasCheckedOutOrders(rewardId))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("DeleteReward Validation: HasCheckedOutOrders=true for RewardId={0}", rewardId));
                    return false;
                }

                System.Diagnostics.Debug.WriteLine(string.Format("DeleteReward Validation: HasCheckedOutOrders=false for RewardId={0}", rewardId));

                string query = @"
                    UPDATE [dbo].[Rewards]
                    SET IsDeleted = 1
                    WHERE Id = @RewardId";

                int rows = DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@RewardId", rewardId));

                System.Diagnostics.Debug.WriteLine(string.Format("DeleteReward: RewardId={0}, DeletedBy={1}", rewardId, deletedBy));
                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("DeleteReward error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Updates reward availability status
        /// Returns false if reward has checked-out orders (cannot change status)
        /// Valid statuses: 'Available', 'OutOfStock', 'Hidden'
        /// </summary>
        public static bool UpdateRewardAvailability(int rewardId, string availabilityStatus, int userId)
        {
            try
            {
                // Validate status
                if (availabilityStatus != "Available" && availabilityStatus != "OutOfStock" && availabilityStatus != "Hidden")
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("UpdateRewardAvailability: Invalid status '{0}' for RewardId={1}", availabilityStatus, rewardId));
                    return false;
                }

                // Check if reward has checked-out orders
                if (HasCheckedOutOrders(rewardId))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("UpdateRewardAvailability Validation: HasCheckedOutOrders=true for RewardId={0}", rewardId));
                    return false;
                }

                System.Diagnostics.Debug.WriteLine(string.Format("UpdateRewardAvailability Validation: HasCheckedOutOrders=false for RewardId={0}", rewardId));

                string query = @"
                    UPDATE [dbo].[Rewards]
                    SET AvailabilityStatus = @AvailabilityStatus, 
                        UpdatedDate = GETDATE(), 
                        UpdatedBy = @UpdatedBy
                    WHERE Id = @RewardId AND IsDeleted = 0";

                int rows = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@RewardId", rewardId),
                    new SqlParameter("@AvailabilityStatus", availabilityStatus),
                    new SqlParameter("@UpdatedBy", userId));

                System.Diagnostics.Debug.WriteLine(string.Format("UpdateRewardAvailability: RewardId={0}, Status={1}, UpdatedBy={2}", rewardId, availabilityStatus, userId));
                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("UpdateRewardAvailability error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        #endregion

        #region Order Management

        /// <summary>
        /// Cart item class for order creation
        /// </summary>
        public class CartItem
        {
            public int RewardId { get; set; }
            public int Quantity { get; set; }
            public int PointCost { get; set; }
        }

        /// <summary>
        /// Generates a unique order number
        /// Format: ORD-YYYYMMDD-XXXXX
        /// </summary>
        private static string GenerateOrderNumber()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string randomPart = new Random().Next(10000, 99999).ToString();
            return string.Format("ORD-{0}-{1}", datePart, randomPart);
        }

        /// <summary>
        /// Creates an order from cart items
        /// </summary>
        public static int CreateOrder(int childId, int familyId, List<CartItem> cartItems, out string orderNumber)
        {
            orderNumber = string.Empty;
            
            try
            {
                if (cartItems == null || cartItems.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("CreateOrder: Cart is empty");
                    return 0;
                }

                // Generate unique order number
                orderNumber = GenerateOrderNumber();
                
                // Calculate total points
                int totalPoints = 0;
                foreach (var item in cartItems)
                {
                    totalPoints += item.PointCost * item.Quantity;
                }

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Validate all rewards still exist and are active
                            foreach (var item in cartItems)
                            {
                                string validateQuery = @"
                                    SELECT Id FROM [dbo].[Rewards]
                                    WHERE Id = @RewardId AND IsActive = 1 AND IsDeleted = 0";

                                using (SqlCommand cmd = new SqlCommand(validateQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@RewardId", item.RewardId);
                                    object result = cmd.ExecuteScalar();
                                    if (result == null)
                                    {
                                        transaction.Rollback();
                                        System.Diagnostics.Debug.WriteLine(string.Format("CreateOrder: Reward {0} not found or inactive", item.RewardId));
                                        return 0;
                                    }
                                }
                            }

                            // Create order
                            string insertOrderQuery = @"
                                INSERT INTO [dbo].[RewardOrders] 
                                (OrderNumber, ChildId, FamilyId, TotalPoints, Status, OrderDate)
                                VALUES (@OrderNumber, @ChildId, @FamilyId, @TotalPoints, 'Pending', GETDATE());
                                SELECT CAST(SCOPE_IDENTITY() AS INT);";

                            int orderId = 0;
                            using (SqlCommand cmd = new SqlCommand(insertOrderQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@OrderNumber", orderNumber);
                                cmd.Parameters.AddWithValue("@ChildId", childId);
                                cmd.Parameters.AddWithValue("@FamilyId", familyId);
                                cmd.Parameters.AddWithValue("@TotalPoints", totalPoints);
                                orderId = Convert.ToInt32(cmd.ExecuteScalar());
                            }

                            // Insert order items
                            foreach (var item in cartItems)
                            {
                                int subtotal = item.PointCost * item.Quantity;
                                string insertItemQuery = @"
                                    INSERT INTO [dbo].[RewardOrderItems] 
                                    (OrderId, RewardId, Quantity, PointCost, Subtotal)
                                    VALUES (@OrderId, @RewardId, @Quantity, @PointCost, @Subtotal)";

                                using (SqlCommand cmd = new SqlCommand(insertItemQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                                    cmd.Parameters.AddWithValue("@RewardId", item.RewardId);
                                    cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                                    cmd.Parameters.AddWithValue("@PointCost", item.PointCost);
                                    cmd.Parameters.AddWithValue("@Subtotal", subtotal);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            System.Diagnostics.Debug.WriteLine(string.Format("CreateOrder: ChildId={0}, FamilyId={1}, TotalPoints={2}, OrderNumber={3}, OrderId={4}", childId, familyId, totalPoints, orderNumber, orderId));
                            return orderId;
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
                System.Diagnostics.Debug.WriteLine(string.Format("CreateOrder error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return 0;
            }
        }

        /// <summary>
        /// Gets child orders
        /// </summary>
        public static DataTable GetChildOrders(int childId, string status = null)
        {
            try
            {
                string query = @"
                    SELECT Id, OrderNumber, TotalPoints, Status, OrderDate, 
                           ConfirmedDate, FulfilledDate, ChildConfirmedDate, RefundCode
                    FROM [dbo].[RewardOrders]
                    WHERE ChildId = @ChildId 
                      AND Status != 'TransactionComplete' 
                      AND Status != 'Declined'";

                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND Status = @Status";
                }

                query += " ORDER BY OrderDate DESC";

                if (!string.IsNullOrEmpty(status))
                {
                    return DatabaseHelper.ExecuteQuery(query,
                        new SqlParameter("@ChildId", childId),
                        new SqlParameter("@Status", status));
                }
                else
                {
                    return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@ChildId", childId));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetChildOrders error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets family orders (for parent)
        /// </summary>
        public static DataTable GetFamilyOrders(int familyId, string status = null)
        {
            try
            {
                string query = @"
                    SELECT ro.Id, ro.OrderNumber, ro.ChildId, u.FirstName + ' ' + u.LastName AS ChildName,
                           ro.TotalPoints, ro.Status, ro.OrderDate, ro.ConfirmedDate, ro.FulfilledDate, ro.RefundCode
                    FROM [dbo].[RewardOrders] ro
                    INNER JOIN [dbo].[Users] u ON ro.ChildId = u.Id
                    WHERE ro.FamilyId = @FamilyId 
                      AND ro.Status != 'TransactionComplete' 
                      AND ro.Status != 'Declined'";

                if (!string.IsNullOrEmpty(status))
                {
                    query += " AND ro.Status = @Status";
                }

                query += " ORDER BY ro.OrderDate DESC";

                if (!string.IsNullOrEmpty(status))
                {
                    return DatabaseHelper.ExecuteQuery(query,
                        new SqlParameter("@FamilyId", familyId),
                        new SqlParameter("@Status", status));
                }
                else
                {
                    return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@FamilyId", familyId));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetFamilyOrders error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets order details including items
        /// </summary>
        public static DataRow GetOrderDetails(int orderId)
        {
            try
            {
                string query = @"
                    SELECT ro.*, u.FirstName + ' ' + u.LastName AS ChildName
                    FROM [dbo].[RewardOrders] ro
                    INNER JOIN [dbo].[Users] u ON ro.ChildId = u.Id
                    WHERE ro.Id = @OrderId";

                DataTable dt = DatabaseHelper.ExecuteQuery(query, new SqlParameter("@OrderId", orderId));
                if (dt.Rows.Count > 0)
                {
                    return dt.Rows[0];
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetOrderDetails error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return null;
            }
        }

        /// <summary>
        /// Gets order items
        /// </summary>
        public static DataTable GetOrderItems(int orderId)
        {
            try
            {
                string query = @"
                    SELECT roi.*, r.Name AS RewardName, r.Description AS RewardDescription
                    FROM [dbo].[RewardOrderItems] roi
                    INNER JOIN [dbo].[Rewards] r ON roi.RewardId = r.Id
                    WHERE roi.OrderId = @OrderId";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@OrderId", orderId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetOrderItems error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return new DataTable();
            }
        }

        /// <summary>
        /// Generates a unique refund code
        /// Format: REF-{OrderId}-{RandomString}
        /// </summary>
        private static string GenerateRefundCode(int orderId)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // Exclude confusing characters
            Random random = new Random();
            StringBuilder randomPart = new StringBuilder(8);
            for (int i = 0; i < 8; i++)
            {
                randomPart.Append(chars[random.Next(chars.Length)]);
            }
            return string.Format("REF-{0}-{1}", orderId, randomPart.ToString());
        }

        /// <summary>
        /// Confirms an order (parent action)
        /// Deducts points from child and adds to treasury
        /// </summary>
        public static bool ConfirmOrder(int orderId, int confirmedBy, out string refundCode)
        {
            refundCode = string.Empty;
            
            try
            {
                // Get order details
                DataRow order = GetOrderDetails(orderId);
                if (order == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ConfirmOrder: Order {0} not found", orderId));
                    return false;
                }

                string status = order["Status"].ToString();
                if (status != "Pending")
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ConfirmOrder: Order {0} status is {1}, cannot confirm", orderId, status));
                    return false;
                }

                int childId = Convert.ToInt32(order["ChildId"]);
                int familyId = Convert.ToInt32(order["FamilyId"]);
                int totalPoints = Convert.ToInt32(order["TotalPoints"]);

                // Check child balance
                string balanceQuery = "SELECT Points FROM [dbo].[Users] WHERE Id = @UserId";
                object balanceResult = DatabaseHelper.ExecuteScalar(balanceQuery, new SqlParameter("@UserId", childId));
                int childBalance = balanceResult != null ? Convert.ToInt32(balanceResult) : 0;

                if (childBalance < totalPoints)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ConfirmOrder: Child {0} has insufficient points. Balance={1}, Required={2}", childId, childBalance, totalPoints));
                    return false;
                }

                // Generate refund code
                refundCode = GenerateRefundCode(orderId);

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Update order status
                            string updateOrderQuery = @"
                                UPDATE [dbo].[RewardOrders]
                                SET Status = 'WaitingToFulfill', ConfirmedDate = GETDATE(), 
                                    ConfirmedBy = @ConfirmedBy, RefundCode = @RefundCode
                                WHERE Id = @OrderId";

                            using (SqlCommand cmd = new SqlCommand(updateOrderQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@OrderId", orderId);
                                cmd.Parameters.AddWithValue("@ConfirmedBy", confirmedBy);
                                cmd.Parameters.AddWithValue("@RefundCode", refundCode);
                                cmd.ExecuteNonQuery();
                            }

                            // Deduct points from child (with cap enforcement)
                            int newChildBalance = Math.Min(10000, Math.Max(0, childBalance - totalPoints));
                            int excess = Math.Max(0, (childBalance - totalPoints) - 10000);
                            
                            string updateChildQuery = @"
                                UPDATE [dbo].[Users]
                                SET Points = @NewBalance
                                WHERE Id = @ChildId";

                            using (SqlCommand cmd = new SqlCommand(updateChildQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@ChildId", childId);
                                cmd.Parameters.AddWithValue("@NewBalance", newChildBalance);
                                cmd.ExecuteNonQuery();
                            }

                            // Create point transaction
                            string insertPointTransactionQuery = @"
                                INSERT INTO [dbo].[PointTransactions] 
                                (UserId, Points, TransactionType, Description, TransactionDate, IsToTreasury)
                                VALUES (@UserId, @Points, 'RewardPurchase', @Description, GETDATE(), 1)";

                            using (SqlCommand cmd = new SqlCommand(insertPointTransactionQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserId", childId);
                                cmd.Parameters.AddWithValue("@Points", -totalPoints);
                                cmd.Parameters.AddWithValue("@Description", string.Format("Reward purchase - Order {0}", orderId));
                                cmd.ExecuteNonQuery();
                            }

                            // Add points to treasury
                            if (!TreasuryHelper.DepositToTreasury(familyId, totalPoints, string.Format("Order {0} confirmed", orderId), orderId, confirmedBy, conn, transaction))
                            {
                                transaction.Rollback();
                                return false;
                            }

                            // If excess, add to treasury
                            if (excess > 0)
                            {
                                TreasuryHelper.DepositToTreasury(familyId, excess, string.Format("Excess from order {0}", orderId), orderId, confirmedBy, conn, transaction);
                            }

                            transaction.Commit();
                            System.Diagnostics.Debug.WriteLine(string.Format("ConfirmOrder: OrderId={0}, ConfirmedBy={1}, RefundCode={2}", orderId, confirmedBy, refundCode));
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
                System.Diagnostics.Debug.WriteLine(string.Format("ConfirmOrder error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Declines an order (parent action)
        /// No points deducted
        /// </summary>
        public static bool DeclineOrder(int orderId, int declinedBy, string declinedReason = null)
        {
            try
            {
                // Get order details
                DataRow order = GetOrderDetails(orderId);
                if (order == null)
                {
                    return false;
                }

                string status = order["Status"].ToString();
                if (status != "Pending")
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("DeclineOrder: Order {0} status is {1}, cannot decline", orderId, status));
                    return false;
                }

                string query = @"
                    UPDATE [dbo].[RewardOrders]
                    SET Status = 'Declined', DeclinedDate = GETDATE(), 
                        DeclinedBy = @DeclinedBy, DeclinedReason = @DeclinedReason
                    WHERE Id = @OrderId";

                int rows = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@OrderId", orderId),
                    new SqlParameter("@DeclinedBy", declinedBy),
                    new SqlParameter("@DeclinedReason", declinedReason ?? (object)DBNull.Value));

                System.Diagnostics.Debug.WriteLine(string.Format("DeclineOrder: OrderId={0}, DeclinedBy={1}", orderId, declinedBy));
                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("DeclineOrder error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Marks order as fulfilled (parent action)
        /// </summary>
        public static bool FulfillOrder(int orderId, int fulfilledBy)
        {
            try
            {
                // Get order details
                DataRow order = GetOrderDetails(orderId);
                if (order == null)
                {
                    return false;
                }

                string status = order["Status"].ToString();
                if (status != "WaitingToFulfill")
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("FulfillOrder: Order {0} status is {1}, cannot fulfill", orderId, status));
                    return false;
                }

                string query = @"
                    UPDATE [dbo].[RewardOrders]
                    SET Status = 'Fulfilled', FulfilledDate = GETDATE(), FulfilledBy = @FulfilledBy
                    WHERE Id = @OrderId";

                int rows = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@OrderId", orderId),
                    new SqlParameter("@FulfilledBy", fulfilledBy));

                System.Diagnostics.Debug.WriteLine(string.Format("FulfillOrder: OrderId={0}, FulfilledBy={1}", orderId, fulfilledBy));
                return rows > 0;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("FulfillOrder error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }


        /// <summary>
        /// Confirms fulfillment (child action)
        /// Records in purchase history
        /// </summary>
        public static bool ConfirmFulfillment(int orderId, int childId)
        {
            try
            {
                // Get order details
                DataRow order = GetOrderDetails(orderId);
                if (order == null)
                {
                    return false;
                }

                // Verify order belongs to child
                if (Convert.ToInt32(order["ChildId"]) != childId)
                {
                    return false;
                }

                string status = order["Status"].ToString();
                if (status != "Fulfilled")
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ConfirmFulfillment: Order {0} status is {1}, cannot confirm", orderId, status));
                    return false;
                }

                // Check if already confirmed
                if (order["ChildConfirmedDate"] != DBNull.Value)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ConfirmFulfillment: Order {0} already confirmed (ChildConfirmedDate is set)", orderId));
                    return false;
                }

                // Check if status is already TransactionComplete (defensive check)
                if (status == "TransactionComplete")
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ConfirmFulfillment: Order {0} status is already TransactionComplete", orderId));
                    return false;
                }

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Update order status to TransactionComplete
                            string updateOrderQuery = @"
                                UPDATE [dbo].[RewardOrders]
                                SET ChildConfirmedDate = GETDATE(), Status = 'TransactionComplete'
                                WHERE Id = @OrderId";

                            using (SqlCommand cmd = new SqlCommand(updateOrderQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@OrderId", orderId);
                                cmd.ExecuteNonQuery();
                            }

                            // Get order items
                            DataTable items = GetOrderItems(orderId);

                            // Insert into purchase history
                            foreach (DataRow item in items.Rows)
                            {
                                string insertHistoryQuery = @"
                                    INSERT INTO [dbo].[RewardPurchaseHistory] 
                                    (OrderId, ChildId, RewardId, RewardName, PointCost, Quantity, PurchaseDate, FulfillmentStatus, FulfilledDate)
                                    VALUES (@OrderId, @ChildId, @RewardId, @RewardName, @PointCost, @Quantity, @PurchaseDate, 'Fulfilled', GETDATE())";

                                using (SqlCommand cmd = new SqlCommand(insertHistoryQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                                    cmd.Parameters.AddWithValue("@ChildId", childId);
                                    cmd.Parameters.AddWithValue("@RewardId", item["RewardId"]);
                                    cmd.Parameters.AddWithValue("@RewardName", item["RewardName"]);
                                    cmd.Parameters.AddWithValue("@PointCost", item["PointCost"]);
                                    cmd.Parameters.AddWithValue("@Quantity", item["Quantity"]);
                                    cmd.Parameters.AddWithValue("@PurchaseDate", order["OrderDate"]);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            System.Diagnostics.Debug.WriteLine(string.Format("ConfirmFulfillment: OrderId={0}, ChildId={1}", orderId, childId));
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
                System.Diagnostics.Debug.WriteLine(string.Format("ConfirmFulfillment error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Claims not fulfilled with refund code (child action)
        /// Refunds points to child and deducts from treasury
        /// </summary>
        public static bool ClaimNotFulfilled(int orderId, int childId, string refundCode)
        {
            try
            {
                // Validate refund code
                if (!ValidateRefundCode(orderId, refundCode))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ClaimNotFulfilled: Invalid refund code for OrderId={0}", orderId));
                    return false;
                }

                // Get order details
                DataRow order = GetOrderDetails(orderId);
                if (order == null)
                {
                    return false;
                }

                // Verify order belongs to child
                if (Convert.ToInt32(order["ChildId"]) != childId)
                {
                    return false;
                }

                string status = order["Status"].ToString();
                if (status != "Fulfilled")
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ClaimNotFulfilled: Order {0} status is {1}, cannot claim refund", orderId, status));
                    return false;
                }

                // Check if already confirmed (defensive check)
                if (order["ChildConfirmedDate"] != DBNull.Value)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ClaimNotFulfilled: Order {0} already confirmed, cannot claim refund", orderId));
                    return false;
                }

                int familyId = Convert.ToInt32(order["FamilyId"]);
                int totalPoints = Convert.ToInt32(order["TotalPoints"]);

                using (SqlConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Update order status to Refunded
                            string updateOrderQuery = @"
                                UPDATE [dbo].[RewardOrders]
                                SET Status = 'Refunded', RefundedDate = GETDATE(), 
                                    RefundedBy = @ChildId, RefundCode = NULL
                                WHERE Id = @OrderId";

                            using (SqlCommand cmd = new SqlCommand(updateOrderQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@OrderId", orderId);
                                cmd.Parameters.AddWithValue("@ChildId", childId);
                                cmd.ExecuteNonQuery();
                            }

                            // Get current child balance
                            string balanceQuery = "SELECT Points FROM [dbo].[Users] WHERE Id = @UserId";
                            using (SqlCommand cmd = new SqlCommand(balanceQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@UserId", childId);
                                object balanceResult = cmd.ExecuteScalar();
                                int currentBalance = balanceResult != null ? Convert.ToInt32(balanceResult) : 0;

                                // Refund points to child (with cap enforcement)
                                int newBalance = Math.Min(10000, currentBalance + totalPoints);
                                int excess = Math.Max(0, (currentBalance + totalPoints) - 10000);

                                string updateChildQuery = @"
                                    UPDATE [dbo].[Users]
                                    SET Points = @NewBalance
                                    WHERE Id = @ChildId";

                                using (SqlCommand updateCmd = new SqlCommand(updateChildQuery, conn, transaction))
                                {
                                    updateCmd.Parameters.AddWithValue("@ChildId", childId);
                                    updateCmd.Parameters.AddWithValue("@NewBalance", newBalance);
                                    updateCmd.ExecuteNonQuery();
                                }

                                // Create point transaction
                                string insertPointTransactionQuery = @"
                                    INSERT INTO [dbo].[PointTransactions] 
                                    (UserId, Points, TransactionType, Description, TransactionDate, IsFromTreasury)
                                    VALUES (@UserId, @Points, 'RewardRefund', @Description, GETDATE(), 1)";

                                using (SqlCommand insertCmd = new SqlCommand(insertPointTransactionQuery, conn, transaction))
                                {
                                    insertCmd.Parameters.AddWithValue("@UserId", childId);
                                    insertCmd.Parameters.AddWithValue("@Points", totalPoints);
                                    insertCmd.Parameters.AddWithValue("@Description", string.Format("Refund for order {0}", orderId));
                                    insertCmd.ExecuteNonQuery();
                                }

                                // If excess, add to treasury (using existing transaction)
                                if (excess > 0)
                                {
                                    if (!TreasuryHelper.DepositToTreasury(familyId, excess, string.Format("Excess from refund order {0}", orderId), orderId, childId, conn, transaction))
                                    {
                                        transaction.Rollback();
                                        System.Diagnostics.Debug.WriteLine(string.Format("ClaimNotFulfilled: Failed to deposit excess to treasury for OrderId={0}", orderId));
                                        return false;
                                    }
                                }
                            }

                            // Deduct from treasury (using existing transaction)
                            if (!TreasuryHelper.WithdrawFromTreasury(familyId, totalPoints, string.Format("Refund for order {0}", orderId), null, childId, conn, transaction))
                            {
                                transaction.Rollback();
                                System.Diagnostics.Debug.WriteLine(string.Format("ClaimNotFulfilled: Failed to withdraw from treasury for OrderId={0}", orderId));
                                return false;
                            }

                            // Get order items and insert into history
                            DataTable items = GetOrderItems(orderId);
                            foreach (DataRow item in items.Rows)
                            {
                                string insertHistoryQuery = @"
                                    INSERT INTO [dbo].[RewardPurchaseHistory] 
                                    (OrderId, ChildId, RewardId, RewardName, PointCost, Quantity, PurchaseDate, FulfillmentStatus)
                                    VALUES (@OrderId, @ChildId, @RewardId, @RewardName, @PointCost, @Quantity, @PurchaseDate, 'Refunded')";

                                using (SqlCommand cmd = new SqlCommand(insertHistoryQuery, conn, transaction))
                                {
                                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                                    cmd.Parameters.AddWithValue("@ChildId", childId);
                                    cmd.Parameters.AddWithValue("@RewardId", item["RewardId"]);
                                    cmd.Parameters.AddWithValue("@RewardName", item["RewardName"]);
                                    cmd.Parameters.AddWithValue("@PointCost", item["PointCost"]);
                                    cmd.Parameters.AddWithValue("@Quantity", item["Quantity"]);
                                    cmd.Parameters.AddWithValue("@PurchaseDate", order["OrderDate"]);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            System.Diagnostics.Debug.WriteLine(string.Format("ClaimNotFulfilled: OrderId={0}, ChildId={1}, RefundCode={2}", orderId, childId, refundCode));
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
                System.Diagnostics.Debug.WriteLine(string.Format("ClaimNotFulfilled error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Validates a refund code for an order
        /// </summary>
        private static bool ValidateRefundCode(int orderId, string refundCode)
        {
            try
            {
                if (string.IsNullOrEmpty(refundCode))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ValidateRefundCode: Empty refund code for OrderId={0}", orderId));
                    return false;
                }

                // Get order details
                DataRow order = GetOrderDetails(orderId);
                if (order == null)
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ValidateRefundCode: Order {0} not found", orderId));
                    return false;
                }

                // Check if refund code matches
                if (order["RefundCode"] == DBNull.Value || string.IsNullOrEmpty(order["RefundCode"].ToString()))
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("ValidateRefundCode: Order {0} has no refund code", orderId));
                    return false;
                }

                string orderRefundCode = order["RefundCode"].ToString().Trim();
                string providedCode = refundCode.Trim();

                bool isValid = string.Equals(orderRefundCode, providedCode, StringComparison.OrdinalIgnoreCase);
                
                System.Diagnostics.Debug.WriteLine(string.Format("ValidateRefundCode: OrderId={0}, ProvidedCode={1}, OrderCode={2}, IsValid={3}", 
                    orderId, providedCode, orderRefundCode, isValid));
                
                return isValid;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("ValidateRefundCode error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return false;
            }
        }

        /// <summary>
        /// Gets purchase history for a child
        /// </summary>
        public static DataTable GetPurchaseHistory(int childId)
        {
            try
            {
                string query = @"
                    SELECT OrderId, RewardName, PointCost, Quantity, PurchaseDate, 
                           FulfillmentStatus, FulfilledDate
                    FROM [dbo].[RewardPurchaseHistory]
                    WHERE ChildId = @ChildId
                    ORDER BY PurchaseDate DESC";

                return DatabaseHelper.ExecuteQuery(query, new SqlParameter("@ChildId", childId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetPurchaseHistory error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets family order history (completed and not fulfilled orders)
        /// Returns orders with Status = 'TransactionComplete' or 'NotFulfilled'
        /// </summary>
        public static DataTable GetFamilyOrderHistory(int familyId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                string query = @"
                    SELECT ro.Id, ro.OrderNumber, ro.ChildId, u.FirstName + ' ' + u.LastName AS ChildName,
                           ro.TotalPoints, ro.Status, ro.OrderDate, ro.ConfirmedDate, ro.FulfilledDate, 
                           ro.ChildConfirmedDate, ro.RefundCode
                    FROM [dbo].[RewardOrders] ro
                    INNER JOIN [dbo].[Users] u ON ro.ChildId = u.Id
                    WHERE ro.FamilyId = @FamilyId 
                      AND (ro.Status = 'TransactionComplete' OR ro.Status = 'NotFulfilled' OR ro.Status = 'Refunded' OR ro.Status = 'Declined')";

                if (startDate.HasValue)
                {
                    query += " AND ro.OrderDate >= @StartDate";
                }

                if (endDate.HasValue)
                {
                    query += " AND ro.OrderDate <= @EndDate";
                }

                query += " ORDER BY ro.OrderDate DESC";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@FamilyId", familyId)
                };

                if (startDate.HasValue)
                {
                    parameters.Add(new SqlParameter("@StartDate", startDate.Value));
                }

                if (endDate.HasValue)
                {
                    parameters.Add(new SqlParameter("@EndDate", endDate.Value));
                }

                return DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetFamilyOrderHistory error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return new DataTable();
            }
        }

        /// <summary>
        /// Gets child order history (completed, refunded, and declined orders)
        /// Returns orders with Status = 'TransactionComplete', 'NotFulfilled', 'Refunded', or 'Declined'
        /// </summary>
        public static DataTable GetChildOrderHistory(int childId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                string query = @"
                    SELECT Id, OrderNumber, TotalPoints, Status, OrderDate, 
                           ConfirmedDate, FulfilledDate, ChildConfirmedDate, RefundCode
                    FROM [dbo].[RewardOrders]
                    WHERE ChildId = @ChildId 
                      AND (Status = 'TransactionComplete' OR Status = 'NotFulfilled' OR Status = 'Refunded' OR Status = 'Declined')";

                if (startDate.HasValue)
                {
                    query += " AND OrderDate >= @StartDate";
                }

                if (endDate.HasValue)
                {
                    query += " AND OrderDate <= @EndDate";
                }

                query += " ORDER BY OrderDate DESC";

                List<SqlParameter> parameters = new List<SqlParameter>
                {
                    new SqlParameter("@ChildId", childId)
                };

                if (startDate.HasValue)
                {
                    parameters.Add(new SqlParameter("@StartDate", startDate.Value));
                }

                if (endDate.HasValue)
                {
                    parameters.Add(new SqlParameter("@EndDate", endDate.Value));
                }

                return DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("GetChildOrderHistory error: {0}", ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("Stack Trace: {0}", ex.StackTrace));
                return new DataTable();
            }
        }

        #endregion
    }
}

