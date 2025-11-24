# Rewards System - Implementation Documentation

**Last Updated:** November 23, 2025  
**Status:** ✅ Complete

## Overview

This document provides a comprehensive overview of the Rewards System implementation, including all components, files, database schema, and integration points.

---

## Table of Contents

1. [Implementation Summary](#implementation-summary)
2. [Database Schema](#database-schema)
3. [Backend Components](#backend-components)
4. [Frontend Pages](#frontend-pages)
5. [Integration Points](#integration-points)
6. [Key Features Implemented](#key-features-implemented)
7. [File Structure](#file-structure)

---

## Implementation Summary

The Rewards System has been fully implemented with the following components:

- ✅ **6 New Database Tables**: Rewards, RewardOrders, RewardOrderItems, FamilyTreasury, TreasuryTransactions, RewardPurchaseHistory
- ✅ **3 New Helper Classes**: RewardHelper, PointHelper, TreasuryHelper
- ✅ **5 New ASP.NET Pages**: Rewards.aspx, RewardOrders.aspx, RewardShop.aspx, Cart.aspx, MyOrders.aspx
- ✅ **Points System Integration**: 10,000 point cap, treasury system, excess points handling
- ✅ **Order Workflow**: Complete order lifecycle from creation to fulfillment
- ✅ **Refund System**: Secure refund code generation and validation

---

## Database Schema

### New Tables

#### 1. Rewards
Stores reward definitions created by parents.

**Columns:**
- `Id` (int, PK, Identity)
- `FamilyId` (int, FK → Families)
- `Name` (nvarchar(200))
- `Description` (nvarchar(max))
- `PointCost` (int)
- `Category` (nvarchar(50))
- `ImageUrl` (nvarchar(500))
- `IsActive` (bit)
- `IsDeleted` (bit)
- `CreatedBy` (int, FK → Users)
- `CreatedDate` (datetime)
- `ModifiedBy` (int, FK → Users, nullable)
- `ModifiedDate` (datetime, nullable)

**Indexes:**
- `IX_Rewards_FamilyId_IsActive_IsDeleted` on (FamilyId, IsActive, IsDeleted)

#### 2. RewardOrders
Stores orders created by children.

**Columns:**
- `Id` (int, PK, Identity)
- `OrderNumber` (nvarchar(50), unique)
- `ChildId` (int, FK → Users)
- `FamilyId` (int, FK → Families)
- `TotalPoints` (int)
- `Status` (nvarchar(50)) - Pending, WaitingToFulfill, Fulfilled, Declined, NotFulfilled
- `RefundCode` (nvarchar(50), nullable)
- `RefundCodeUsed` (bit, default 0)
- `OrderDate` (datetime)
- `ConfirmedDate` (datetime, nullable)
- `FulfilledDate` (datetime, nullable)
- `ConfirmedBy` (int, FK → Users, nullable)
- `FulfilledBy` (int, FK → Users, nullable)

**Indexes:**
- `IX_RewardOrders_ChildId` on (ChildId)
- `IX_RewardOrders_FamilyId` on (FamilyId)
- `IX_RewardOrders_Status` on (Status)

#### 3. RewardOrderItems
Stores individual items within an order.

**Columns:**
- `Id` (int, PK, Identity)
- `OrderId` (int, FK → RewardOrders)
- `RewardId` (int, FK → Rewards)
- `Quantity` (int)
- `PointCost` (int) - Snapshot of cost at time of order
- `Subtotal` (int) - Quantity × PointCost

**Indexes:**
- `IX_RewardOrderItems_OrderId` on (OrderId)
- `IX_RewardOrderItems_RewardId` on (RewardId)

#### 4. FamilyTreasury
Stores the central points treasury for each family.

**Columns:**
- `Id` (int, PK, Identity)
- `FamilyId` (int, FK → Families, unique)
- `Balance` (int, default 0)
- `CreatedDate` (datetime)
- `LastUpdated` (datetime)

**Constraints:**
- `CK_FamilyTreasury_Balance_NonNegative` - Balance >= 0

#### 5. TreasuryTransactions
Audit trail for all treasury transactions.

**Columns:**
- `Id` (int, PK, Identity)
- `FamilyId` (int, FK → Families)
- `TransactionType` (nvarchar(50)) - Deposit, Withdraw
- `Amount` (int)
- `Description` (nvarchar(500))
- `RelatedOrderId` (int, FK → RewardOrders, nullable)
- `RelatedTaskAssignmentId` (int, FK → TaskAssignments, nullable)
- `RelatedPointTransactionId` (int, FK → PointTransactions, nullable)
- `CreatedBy` (int, FK → Users, nullable)
- `CreatedDate` (datetime)

**Indexes:**
- `IX_TreasuryTransactions_FamilyId` on (FamilyId)
- `IX_TreasuryTransactions_CreatedDate` on (CreatedDate)

#### 6. RewardPurchaseHistory
Tracks completed purchases for children.

**Columns:**
- `Id` (int, PK, Identity)
- `OrderId` (int, FK → RewardOrders)
- `ChildId` (int, FK → Users)
- `RewardId` (int, FK → Rewards)
- `Quantity` (int)
- `PointCost` (int)
- `Subtotal` (int)
- `ConfirmedDate` (datetime)

**Indexes:**
- `IX_RewardPurchaseHistory_ChildId` on (ChildId)
- `IX_RewardPurchaseHistory_OrderId` on (OrderId)

### Modified Tables

#### PointTransactions
Added new columns for treasury integration:
- `TreasuryTransactionId` (int, FK → TreasuryTransactions, nullable)
- `IsFromTreasury` (bit, default 0)
- `IsToTreasury` (bit, default 0)
- `ExcessAmount` (int, nullable) - Amount that exceeded cap

#### Users
Added check constraint:
- `CK_Users_Points_Range` - Points >= 0 AND Points <= 10000

---

## Backend Components

### 1. RewardHelper.cs

**Location:** `App_Code/RewardHelper.cs`

**Purpose:** Manages all reward and order-related operations.

**Key Methods:**

#### Reward Management
- `CreateReward(int familyId, int createdBy, string name, string description, int pointCost, string category, string imageUrl)` - Creates a new reward
- `GetFamilyRewards(int familyId, bool activeOnly)` - Retrieves rewards for a family
- `GetRewardDetails(int rewardId)` - Gets detailed information about a reward
- `UpdateReward(int rewardId, int userId, string name, string description, int pointCost, string category, string imageUrl)` - Updates reward (with checked-out order validation)
- `DeleteReward(int rewardId, int userId)` - Soft-deletes a reward (with checked-out order validation)
- `HasCheckedOutOrders(int rewardId)` - Checks if reward is in any checked-out orders

#### Order Management
- `CreateOrder(int childId, int familyId, Dictionary<int, int> cart)` - Creates an order from cart
- `GetChildOrders(int childId)` - Gets all orders for a child
- `GetFamilyOrders(int familyId)` - Gets all orders for a family (parent view)
- `GetOrderDetails(int orderId)` - Gets order details with items
- `ConfirmOrder(int orderId, int userId)` - Parent confirms order (deducts points, generates refund code)
- `DeclineOrder(int orderId, int userId)` - Parent declines order
- `FulfillOrder(int orderId, int userId)` - Parent marks order as fulfilled
- `ConfirmFulfillment(int orderId, int userId)` - Child confirms receipt
- `ClaimNotFulfilled(int orderId, int userId, string refundCode)` - Child claims non-fulfillment with refund

#### Refund Code Management
- `GenerateRefundCode(int orderId)` - Generates unique alphanumeric refund code
- `ValidateRefundCode(int orderId, string refundCode)` - Validates refund code
- `InvalidateRefundCode(int orderId)` - Marks refund code as used

**Error Handling:** All methods include try-catch blocks with detailed logging via `System.Diagnostics.Debug.WriteLine`.

### 2. PointHelper.cs

**Location:** `App_Code/PointHelper.cs`

**Purpose:** Centralizes all point operations with cap enforcement and treasury integration.

**Key Methods:**

- `AwardPointsWithCap(int userId, int points, int familyId, string description, int? taskAssignmentId, int? relatedOrderId)` - Awards points with 10,000 cap
  - Checks treasury balance
  - Withdraws from treasury
  - Adds to child balance
  - Deposits excess back to treasury
  - Records PointTransaction
  
- `DeductPoints(int userId, int points, int familyId, string description, int? relatedOrderId, int? taskAssignmentId)` - Deducts points
  - Deducts from child balance (cannot go negative)
  - Deposits to treasury
  - Records PointTransaction
  
- `GetChildBalance(int userId)` - Gets current point balance
- `CanAffordPurchase(int userId, int totalPoints)` - Checks if child can afford purchase

**Features:**
- Row-level locking for concurrency control
- Database transactions for atomicity
- Automatic treasury initialization
- Excess points handling (beyond 10,000 cap)

### 3. TreasuryHelper.cs

**Location:** `App_Code/TreasuryHelper.cs`

**Purpose:** Manages family treasury operations.

**Key Methods:**

- `GetTreasuryBalance(int familyId)` - Gets current balance
- `InitializeTreasury(int familyId)` - Creates treasury if it doesn't exist
- `DepositToTreasury(int familyId, int amount, string description, ...)` - Deposits points
- `WithdrawFromTreasury(int familyId, int amount, string description, ...)` - Withdraws points (with balance check)
- `GetTreasuryTransactions(int familyId, DateTime? startDate, DateTime? endDate)` - Gets transaction history

**Features:**
- Row-level locking (UPDLOCK, ROWLOCK)
- Automatic initialization
- Transaction audit trail
- Balance validation

---

## Frontend Pages

### 1. Rewards.aspx (Parent)

**Location:** `Rewards.aspx`, `Rewards.aspx.cs`, `Rewards.aspx.designer.cs`

**Purpose:** Parent reward management (CRUD operations).

**Features:**
- View all family rewards in grid layout
- Create new rewards (modal)
- Edit existing rewards (modal, with validation)
- View reward details (modal)
- Delete rewards (confirmation modal, with validation)
- Search and filter by category
- "In Use" badge for rewards in checked-out orders
- Disabled edit/delete buttons for rewards in checked-out orders
- Themed message system (success/error)

**Navigation:** Parent Dashboard → Rewards

**Access Control:** PARENT role only

### 2. RewardOrders.aspx (Parent)

**Location:** `RewardOrders.aspx`, `RewardOrders.aspx.cs`, `RewardOrders.aspx.designer.cs`

**Purpose:** Parent order management.

**Features:**
- View all family orders
- Filter by status
- Confirm orders (deducts points, generates refund code)
- Decline orders
- Mark orders as fulfilled
- View order details (items, totals, child info)
- Status badges
- Confirmation modals for all actions
- Themed message system

**Navigation:** Parent Dashboard → Orders

**Access Control:** PARENT role only

### 3. RewardShop.aspx (Child)

**Location:** `RewardShop.aspx`, `RewardShop.aspx.cs`, `RewardShop.aspx.designer.cs`

**Purpose:** Child reward browsing and shopping.

**Features:**
- Browse available rewards in grid layout
- Points balance display in header
- Add to cart functionality
- Affordability checks (disables "Add to Cart" if insufficient points)
- Search and filter by category
- "Not Enough Points" button text for unaffordable rewards
- Themed message system

**Navigation:** Child Dashboard → Shop

**Access Control:** CHILD role only

### 4. Cart.aspx (Child)

**Location:** `Cart.aspx`, `Cart.aspx.cs`, `Cart.aspx.designer.cs`

**Purpose:** Shopping cart management and checkout.

**Features:**
- View cart items with images
- Update quantities (+/- buttons and direct input)
- Remove items
- Calculate subtotals and totals
- Points balance display
- Insufficient points warning
- Disabled checkout button if insufficient points
- Checkout creates order
- Empty cart state
- Themed message system

**Navigation:** RewardShop → Cart

**Access Control:** CHILD role only

**Session Management:** Cart stored in `Session["Cart"]` as `Dictionary<int, int>` (RewardId → Quantity)

### 5. MyOrders.aspx (Child)

**Location:** `MyOrders.aspx`, `MyOrders.aspx.cs`, `MyOrders.aspx.designer.cs`

**Purpose:** Child order tracking and fulfillment confirmation.

**Features:**
- View all child orders
- Filter by status
- Display order details (items, totals, dates)
- Display refund codes for WaitingToFulfill/Fulfilled orders
- Confirm fulfillment (marks order as fulfilled)
- Claim not fulfilled (with refund code validation)
- Status badges
- Confirmation modals
- Themed message system

**Navigation:** Child Dashboard → My Orders

**Access Control:** CHILD role only

---

## Integration Points

### Task System Integration

**File:** `App_Code/TaskHelper.cs`

**Method:** `ReviewTask`

**Changes:**
- Updated to use `PointHelper.AwardPointsWithCap()` instead of direct point manipulation
- Points now come from treasury
- Excess points go to treasury
- Cap enforcement (10,000 points)

**Example:**
```csharp
// Old approach (removed):
// AddPointTransaction(userId, pointsAwarded, ...);

// New approach:
PointHelper.AwardPointsWithCap(userId, pointsAwarded, familyId.Value, 
    description, taskAssignmentId, null);
```

### Database Initialization

**File:** `App_Code/DatabaseInitializer.cs`

**Changes:**
- Added `CREATE TABLE` statements for all 6 new tables
- Added `ALTER TABLE` statements for `PointTransactions` and `Users`
- All tables created automatically on application start

---

## Key Features Implemented

### ✅ Points System Adjustments

1. **Child Points Cap**: Maximum 10,000 points per child
2. **No Negative Points**: Enforced via check constraint and validation
3. **Excess Points Handling**: Points beyond cap go to family treasury
4. **Treasury Integration**: All point movements tracked through treasury
5. **Points Deduction**: Deducted points go to treasury
6. **Points Earning**: Earned points come from treasury

### ✅ Order Workflow

1. **Order Creation**: Child creates order from cart
2. **Parent Confirmation**: Parent confirms (deducts points, generates refund code)
3. **Parent Decline**: Parent declines (no points deducted)
4. **Fulfillment**: Parent marks as fulfilled
5. **Child Confirmation**: Child confirms receipt
6. **Refund Process**: Child can claim not fulfilled with refund code

### ✅ Business Rules

1. **Reward Edit/Delete Restriction**: Cannot edit/delete rewards in checked-out orders
2. **Refund Code Security**: Unique, one-time use codes
3. **Status Management**: Proper status transitions
4. **Concurrency Control**: Row-level locking for treasury operations

### ✅ UI/UX Features

1. **Themed Message System**: Consistent success/error messages
2. **Symbol Display**: HTML entities for special characters (stars, checkmarks)
3. **Confirmation Modals**: Custom styled modals for all critical actions
4. **Responsive Design**: Grid layouts adapt to screen size
5. **Empty States**: Helpful messages when no data
6. **Status Badges**: Color-coded status indicators

---

## File Structure

```
mokipointsCS/
├── App_Code/
│   ├── RewardHelper.cs          (Reward and order management)
│   ├── PointHelper.cs            (Point operations with cap)
│   ├── TreasuryHelper.cs         (Treasury management)
│   ├── TaskHelper.cs             (Updated for treasury integration)
│   └── DatabaseInitializer.cs   (Updated with new tables)
│
├── Rewards.aspx                  (Parent reward management)
├── Rewards.aspx.cs
├── Rewards.aspx.designer.cs
│
├── RewardOrders.aspx             (Parent order management)
├── RewardOrders.aspx.cs
├── RewardOrders.aspx.designer.cs
│
├── RewardShop.aspx               (Child reward shop)
├── RewardShop.aspx.cs
├── RewardShop.aspx.designer.cs
│
├── Cart.aspx                     (Shopping cart)
├── Cart.aspx.cs
├── Cart.aspx.designer.cs
│
├── MyOrders.aspx                 (Child order tracking)
├── MyOrders.aspx.cs
├── MyOrders.aspx.designer.cs
│
└── Documentation/
    └── RewardsSystem/
        ├── REWARDS_SYSTEM_SCHEMATIC.md  (System design)
        ├── IMPLEMENTATION.md             (This file)
        └── TESTING.md                    (Testing guide)
```

---

## Dependencies

### Required Helper Classes
- `DatabaseHelper` - Database operations
- `FamilyHelper` - Family operations
- `TaskHelper` - Task system integration

### Session Variables Used
- `Session["UserId"]` - Current user ID
- `Session["UserRole"]` - User role (PARENT/CHILD)
- `Session["FirstName"]`, `Session["LastName"]` - User name
- `Session["UserName"]` - Child username
- `Session["Cart"]` - Shopping cart (Dictionary<int, int>)

---

## Error Handling

All methods include:
- Try-catch blocks
- Detailed logging via `System.Diagnostics.Debug.WriteLine`
- User-friendly error messages
- Themed error message display

---

## Security Considerations

1. **Role-Based Access Control**: Pages check user role before allowing access
2. **Authentication Checks**: All pages verify user is logged in
3. **Family Validation**: Operations verify user is in a family
4. **Refund Code Validation**: Secure validation before refunds
5. **Row-Level Locking**: Prevents race conditions in treasury operations
6. **Database Transactions**: Ensures atomicity of multi-step operations

---

## Performance Considerations

1. **Indexed Queries**: All foreign keys and frequently queried columns are indexed
2. **Efficient Filtering**: Client-side filtering for reward/order lists
3. **Lazy Loading**: Order items loaded only when needed
4. **Session Management**: Cart stored in session for quick access

---

## Future Enhancements (Not Implemented)

1. Image upload for rewards (currently URL-based)
2. Reward templates
3. Bulk reward operations
4. Order history export
5. Treasury balance reports
6. Email notifications for order status changes

---

## Notes

- All dates stored in UTC
- Point costs are integers (no decimals)
- Refund codes are alphanumeric, 8 characters
- Order numbers are auto-generated (format: ORD-YYYYMMDD-XXXX)
- Treasury balance cannot go negative (enforced by constraint)

---

**End of Implementation Documentation**

