# Rewards System - Complete Schematic Documentation

**Version**: 1.0  
**Date Created**: November 23, 2025  
**Last Updated**: November 23, 2025  
**Status**: ✅ **READY FOR IMPLEMENTATION**  
**Related Systems**: Task System, Points System, Family Treasury

---

## 📋 Table of Contents

1. [System Overview](#system-overview)
2. [Core Concepts](#core-concepts)
3. [Database Schema](#database-schema)
4. [Business Rules](#business-rules)
5. [Edge Cases & Logic Flaws](#edge-cases--logic-flaws)
6. [Message & Notification Design Requirements](#message--notification-design-requirements)
7. [Process Flows](#process-flows)
8. [API Methods](#api-methods)
9. [Points System Adjustments](#points-system-adjustments)
10. [Status Management](#status-management)
11. [Transaction Refund System](#transaction-refund-system)
12. [UI/UX Requirements](#uiux-requirements)
13. [Error Handling & Logging](#error-handling--logging)
14. [Security Considerations](#security-considerations)
15. [Implementation Checklist](#implementation-checklist)
16. [Integration Points](#integration-points)
17. [Logic Review Summary](#logic-review-summary)

---

## 🎯 System Overview

### Purpose
The Rewards System allows children to redeem points earned from completing tasks for rewards created and managed by parents. The system includes a shopping cart, order management, fulfillment tracking, and a treasury-based points system.

### Key Features
1. **Parent Reward Management**: Create, edit, view, and delete rewards
2. **Child Reward Shop**: Browse and purchase rewards with points
3. **Shopping Cart**: Add multiple rewards, calculate total points
4. **Order Management**: Track orders through fulfillment lifecycle
5. **Points Treasury**: Centralized points management with caps and refunds
6. **Transaction Refund Codes**: Secure refund mechanism for unfulfilled orders

### System Actors
- **Parent**: Creates/manages rewards, confirms orders, fulfills orders
- **Child**: Views rewards, adds to cart, purchases, confirms fulfillment
- **System**: Manages treasury, enforces points caps, generates refund codes

---

## 💡 Core Concepts

### 1. Family Treasury
- **Purpose**: Central repository for all family points
- **Source**: Points deducted from children go to treasury
- **Destination**: Points awarded to children come from treasury
- **Balance**: Tracks total available points in family

### 2. Child Points Cap
- **Maximum**: 10,000 points per child
- **Minimum**: 0 points (cannot go negative)
- **Excess Handling**: Points exceeding 10,000 automatically go to treasury
- **Enforcement**: Server-side validation on all point transactions

### 3. Reward
- **Definition**: Item or privilege that can be purchased with points
- **Properties**: Name, description, point cost, image, category, availability
- **Lifecycle**: Created → Active → (Optional: Archived/Deleted)

### 4. Order
- **Definition**: Child's purchase request for one or more rewards
- **Status Flow**: Pending → Waiting to Fulfill → Fulfilled / Declined / Not Fulfilled
- **Properties**: Order date, total points, status, refund code

### 5. Transaction Refund Code
- **Purpose**: Secure mechanism to refund points for unfulfilled orders
- **Generation**: Created when parent confirms order (status = "Waiting to Fulfill")
- **Usage**: Required for child to claim refund if order not fulfilled
- **Security**: Unique, non-guessable code per order

---

## 🗄️ Database Schema

### New Tables

#### 1. Rewards Table
```sql
CREATE TABLE [dbo].[Rewards] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [FamilyId] INT NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [PointCost] INT NOT NULL CHECK ([PointCost] > 0),
    [Category] NVARCHAR(50) NULL,
    [ImageUrl] NVARCHAR(500) NULL,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    [CreatedBy] INT NOT NULL, -- Parent UserId
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedDate] DATETIME NULL,
    [UpdatedBy] INT NULL,
    FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id]),
    FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([Id]),
    FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[Users]([Id])
);
```

**Indexes**:
- `IX_Rewards_FamilyId_IsActive` ON ([FamilyId], [IsActive]) WHERE [IsDeleted] = 0
- `IX_Rewards_Category` ON ([Category]) WHERE [IsDeleted] = 0

#### 2. RewardOrders Table
```sql
CREATE TABLE [dbo].[RewardOrders] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [OrderNumber] NVARCHAR(50) NOT NULL UNIQUE, -- Format: ORD-YYYYMMDD-XXXXX
    [ChildId] INT NOT NULL, -- UserId of child
    [FamilyId] INT NOT NULL,
    [TotalPoints] INT NOT NULL CHECK ([TotalPoints] > 0),
    [Status] NVARCHAR(50) NOT NULL, -- Pending, WaitingToFulfill, Fulfilled, Declined, NotFulfilled, TransactionComplete
    [RefundCode] NVARCHAR(50) NULL UNIQUE, -- Generated when status = WaitingToFulfill
    [OrderDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [ConfirmedDate] DATETIME NULL, -- When parent confirms
    [ConfirmedBy] INT NULL, -- Parent UserId who confirmed
    [DeclinedDate] DATETIME NULL,
    [DeclinedBy] INT NULL,
    [DeclinedReason] NVARCHAR(500) NULL,
    [FulfilledDate] DATETIME NULL, -- When parent marks as fulfilled
    [FulfilledBy] INT NULL,
    [ChildConfirmedDate] DATETIME NULL, -- When child confirms fulfillment
    [RefundedDate] DATETIME NULL,
    [RefundedBy] INT NULL,
    [Notes] NVARCHAR(MAX) NULL,
    FOREIGN KEY ([ChildId]) REFERENCES [dbo].[Users]([Id]),
    FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id]),
    FOREIGN KEY ([ConfirmedBy]) REFERENCES [dbo].[Users]([Id]),
    FOREIGN KEY ([DeclinedBy]) REFERENCES [dbo].[Users]([Id]),
    FOREIGN KEY ([FulfilledBy]) REFERENCES [dbo].[Users]([Id]),
    FOREIGN KEY ([RefundedBy]) REFERENCES [dbo].[Users]([Id])
);
```

**Indexes**:
- `IX_RewardOrders_ChildId` ON ([ChildId])
- `IX_RewardOrders_FamilyId_Status` ON ([FamilyId], [Status])
- `IX_RewardOrders_OrderNumber` ON ([OrderNumber])
- `IX_RewardOrders_RefundCode` ON ([RefundCode]) WHERE [RefundCode] IS NOT NULL

#### 3. RewardOrderItems Table
```sql
CREATE TABLE [dbo].[RewardOrderItems] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [OrderId] INT NOT NULL,
    [RewardId] INT NOT NULL,
    [Quantity] INT NOT NULL DEFAULT 1 CHECK ([Quantity] > 0),
    [PointCost] INT NOT NULL CHECK ([PointCost] > 0), -- Snapshot of cost at time of purchase
    [Subtotal] INT NOT NULL CHECK ([Subtotal] > 0), -- Quantity * PointCost
    FOREIGN KEY ([OrderId]) REFERENCES [dbo].[RewardOrders]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([RewardId]) REFERENCES [dbo].[Rewards]([Id])
);
```

**Indexes**:
- `IX_RewardOrderItems_OrderId` ON ([OrderId])
- `IX_RewardOrderItems_RewardId` ON ([RewardId])

#### 4. FamilyTreasury Table
```sql
CREATE TABLE [dbo].[FamilyTreasury] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [FamilyId] INT NOT NULL UNIQUE,
    [Balance] INT NOT NULL DEFAULT 0 CHECK ([Balance] >= 0),
    [LastUpdated] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id])
);
```

**Indexes**:
- `IX_FamilyTreasury_FamilyId` ON ([FamilyId])

#### 5. TreasuryTransactions Table
```sql
CREATE TABLE [dbo].[TreasuryTransactions] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [FamilyId] INT NOT NULL,
    [TransactionType] NVARCHAR(50) NOT NULL, -- Deposit, Withdrawal, Refund, ExcessCap
    [Amount] INT NOT NULL, -- Positive for deposits, negative for withdrawals
    [BalanceAfter] INT NOT NULL CHECK ([BalanceAfter] >= 0),
    [Description] NVARCHAR(500) NULL,
    [RelatedOrderId] INT NULL,
    [RelatedTaskAssignmentId] INT NULL,
    [RelatedPointTransactionId] INT NULL,
    [CreatedBy] INT NULL, -- UserId who triggered transaction
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id]),
    FOREIGN KEY ([RelatedOrderId]) REFERENCES [dbo].[RewardOrders]([Id]),
    FOREIGN KEY ([RelatedTaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id]),
    FOREIGN KEY ([RelatedPointTransactionId]) REFERENCES [dbo].[PointTransactions]([Id]),
    FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([Id])
);
```

**Indexes**:
- `IX_TreasuryTransactions_FamilyId` ON ([FamilyId])
- `IX_TreasuryTransactions_CreatedDate` ON ([CreatedDate])
- `IX_TreasuryTransactions_RelatedOrderId` ON ([RelatedOrderId]) WHERE [RelatedOrderId] IS NOT NULL

#### 6. RewardPurchaseHistory Table
```sql
CREATE TABLE [dbo].[RewardPurchaseHistory] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [OrderId] INT NOT NULL,
    [ChildId] INT NOT NULL,
    [RewardId] INT NOT NULL,
    [RewardName] NVARCHAR(200) NOT NULL, -- Snapshot
    [PointCost] INT NOT NULL, -- Snapshot
    [Quantity] INT NOT NULL,
    [PurchaseDate] DATETIME NOT NULL,
    [FulfillmentStatus] NVARCHAR(50) NOT NULL, -- Fulfilled, NotFulfilled, Refunded
    [FulfilledDate] DATETIME NULL,
    FOREIGN KEY ([OrderId]) REFERENCES [dbo].[RewardOrders]([Id]),
    FOREIGN KEY ([ChildId]) REFERENCES [dbo].[Users]([Id]),
    FOREIGN KEY ([RewardId]) REFERENCES [dbo].[Rewards]([Id])
);
```

**Indexes**:
- `IX_RewardPurchaseHistory_ChildId` ON ([ChildId])
- `IX_RewardPurchaseHistory_OrderId` ON ([OrderId])
- `IX_RewardPurchaseHistory_FulfillmentStatus` ON ([FulfillmentStatus])

### Modified Tables

#### 1. PointTransactions Table (Enhanced)
```sql
-- Add new columns
ALTER TABLE [dbo].[PointTransactions] ADD
    [TreasuryTransactionId] INT NULL,
    [IsFromTreasury] BIT NOT NULL DEFAULT 0,
    [IsToTreasury] BIT NOT NULL DEFAULT 0,
    [ExcessAmount] INT NULL, -- Amount that exceeded cap (goes to treasury)
    FOREIGN KEY ([TreasuryTransactionId]) REFERENCES [dbo].[TreasuryTransactions]([Id]);
```

**New Transaction Types**:
- `RewardPurchase` - Points deducted for reward purchase
- `RewardRefund` - Points refunded for unfulfilled order
- `ExcessCap` - Points exceeding 10,000 cap returned to treasury

#### 2. Users Table (Enhanced)
```sql
-- Add points cap constraint (enforced in application logic)
-- Points column already exists, but we'll add a check constraint
ALTER TABLE [dbo].[Users] ADD CONSTRAINT [CK_Users_Points_Range] 
    CHECK ([Points] >= 0 AND [Points] <= 10000);
```

---

## 📜 Business Rules

### Reward Management Rules

#### Rule 1: Reward Edit/Delete Restriction
**Rule**: A reward cannot be edited or deleted if it exists in any order that has been checked out.

**Definition of "Checked Out"**: An order is considered "checked out" when it has been created by a child (Status = 'Pending' or later). This includes:
- **Pending**: Order created, waiting for parent confirmation
- **Waiting to Fulfill**: Parent confirmed, points deducted
- **Fulfilled**: Parent marked as fulfilled
- **NotFulfilled**: Child claimed not fulfilled (refunded)

**Excluded Status**: Orders with Status = 'Declined' are NOT considered checked out, as they were never confirmed and no points were deducted.

**Rationale**:
- Preserves order history integrity
- Prevents changing reward details after purchase
- Ensures order items match what child purchased
- Maintains data consistency for purchase history

**Implementation**:
- Before editing: Check `HasCheckedOutOrders(rewardId)`
- Before deleting: Check `HasCheckedOutOrders(rewardId)`
- If true: Show error message and prevent action
- If false: Allow edit/delete

**Error Messages**:
- Edit: "Cannot edit reward. This reward is in an order that has been checked out."
- Delete: "Cannot delete reward. This reward is in an order that has been checked out."

**SQL Check**:
```sql
SELECT COUNT(*) 
FROM RewardOrderItems roi
INNER JOIN RewardOrders ro ON roi.OrderId = ro.Id
WHERE roi.RewardId = @RewardId
  AND ro.Status IN ('Pending', 'WaitingToFulfill', 'Fulfilled', 'NotFulfilled')
```

If COUNT > 0, reward cannot be edited or deleted.

---

## ⚠️ Edge Cases & Logic Flaws

### Critical Edge Cases to Handle

#### 1. Cart-Related Edge Cases

**Case 1.1: Reward Deleted While in Cart**
- **Scenario**: Child adds reward to cart, parent deletes reward, child tries to checkout
- **Current Logic**: Flow 6 validates "All rewards still active and exist"
- **Fix**: ✅ Handled - Server validation will reject checkout
- **User Message**: "One or more rewards in your cart are no longer available. Please remove them and try again."
- **Action**: Remove invalid items from cart automatically, show notification

**Case 1.2: Reward Point Cost Changed While in Cart**
- **Scenario**: Child adds reward (100 points) to cart, parent changes cost to 200 points, child checks out
- **Current Logic**: Flow 5 stores `PointCost (snapshot)` in cart
- **Issue**: Cart has old price, but reward has new price
- **Fix**: On checkout, validate current reward price matches cart snapshot OR use current price
- **Decision**: Use cart snapshot price (what child saw when adding to cart)
- **Validation**: If reward price changed, show warning: "Price has changed. Update cart?"
- **User Message**: "The price of '{RewardName}' has changed from {OldPrice} to {NewPrice} points. Please update your cart."

**Case 1.3: Reward Becomes Inactive While in Cart**
- **Scenario**: Child adds reward to cart, parent deactivates reward, child checks out
- **Current Logic**: Flow 6 validates "All rewards still active"
- **Fix**: ✅ Handled - Server validation will reject checkout
- **User Message**: "One or more rewards in your cart are no longer available. Please remove them and try again."

**Case 1.4: Child's Points Change While Cart is Open**
- **Scenario**: Child opens cart with 500 points, parent awards 200 points, cart still shows old balance
- **Current Logic**: Cart shows cached balance
- **Fix**: Refresh child balance on cart page load, validate on checkout
- **User Message**: "Your point balance has been updated. Please review your cart."
- **Action**: Recalculate cart totals, disable checkout if insufficient

**Case 1.5: Cart Expiration/Timeout**
- **Scenario**: Child adds items to cart, closes browser, returns days later
- **Current Logic**: Cart stored in session/localStorage
- **Fix**: Validate cart items on page load, remove expired/invalid items
- **Cart Expiration**: 7 days (configurable)
- **User Message**: "Your cart has expired. Items have been removed."

**Case 1.6: Multiple Devices/Tabs Cart Conflict**
- **Scenario**: Child adds item on phone, adds different item on tablet, both in cart
- **Current Logic**: Cart stored per session/device
- **Fix**: Use server-side cart storage OR merge on checkout
- **Decision**: Use server-side cart (stored in database temporarily)
- **Alternative**: Show warning: "Cart updated on another device. Refresh?"

#### 2. Order-Related Edge Cases

**Case 2.1: Child's Points Insufficient Between Checkout and Confirmation**
- **Scenario**: Child checks out with 500 points, order pending, child's points drop to 300, parent confirms
- **Current Logic**: Flow 7 checks "child balance >= order total" before deducting
- **Fix**: ✅ Handled - Validation will fail
- **Issue**: Order exists but cannot be confirmed
- **Solution**: Show error to parent: "Child has insufficient points. Order cannot be confirmed."
- **Action**: Parent can decline order OR wait for child to earn more points
- **User Message (Parent)**: "Cannot confirm order. {ChildName} has {CurrentBalance} points but needs {RequiredPoints} points."

**Case 2.2: Reward Deleted After Order Created**
- **Scenario**: Child creates order, parent deletes reward, parent tries to confirm order
- **Current Logic**: Reward cannot be deleted if in checked-out order
- **Fix**: ✅ Handled - Business Rule prevents deletion
- **Edge Case**: What if reward was soft-deleted before order?
- **Solution**: On order confirmation, check if reward exists and is active
- **User Message**: "One or more rewards in this order are no longer available. Cannot confirm order."

**Case 2.3: Concurrent Order Confirmations**
- **Scenario**: Two parents confirm different orders simultaneously, treasury balance insufficient for both
- **Current Logic**: No transaction locking
- **Fix**: Use database transactions with proper locking
- **Solution**: 
  - Begin transaction
  - Lock treasury row (SELECT ... WITH (UPDLOCK, ROWLOCK))
  - Check balance
  - Update if sufficient
  - Commit or rollback
- **Error Handling**: Second confirmation fails with "Treasury balance insufficient"

**Case 2.4: Order Confirmed But Points Deduction Fails**
- **Scenario**: Parent confirms order, points deduction fails mid-process
- **Current Logic**: No rollback mechanism documented
- **Fix**: Use database transaction, rollback on any failure
- **Solution**:
  ```
  BEGIN TRANSACTION
  TRY:
    - Update order status
    - Deduct points from child
    - Add points to treasury
    - Generate refund code
    COMMIT
  CATCH:
    ROLLBACK
    Show error to parent
  ```

**Case 2.5: Order Status Race Condition**
- **Scenario**: Parent confirms order (Status = "Waiting to Fulfill"), child simultaneously tries to cancel
- **Current Logic**: No status locking
- **Fix**: Check status before any status change operation
- **Validation**: "Cannot perform action. Order status has changed."
- **User Message**: "This order's status has been updated. Please refresh the page."

**Case 2.6: Duplicate Order Creation**
- **Scenario**: Child clicks checkout button multiple times rapidly
- **Current Logic**: No duplicate prevention
- **Fix**: 
  - Disable checkout button after first click
  - Use unique constraint on (ChildId, OrderDate, TotalPoints) OR
  - Check for duplicate orders within 5 seconds
- **User Message**: "Order already being processed. Please wait."

#### 3. Points System Edge Cases

**Case 3.1: Treasury Balance Zero When Awarding Points**
- **Scenario**: Parent tries to award 100 points, treasury has 0 points
- **Current Logic**: Flow 10a checks "treasury balance >= points to award"
- **Fix**: ✅ Handled - Validation prevents award
- **User Message**: "Family treasury has insufficient balance ({CurrentBalance} points). Cannot award {RequiredPoints} points."

**Case 3.2: Child at 10,000 Cap Earns More Points**
- **Scenario**: Child has 10,000 points, parent awards 500 points
- **Current Logic**: Flow 10a calculates excess = 500, adds to treasury
- **Fix**: ✅ Handled - Excess goes to treasury
- **User Message**: "You've reached the maximum points (10,000). {Excess} excess points have been added to family treasury."

**Case 3.3: Child Has Zero Points, Tries to Purchase**
- **Scenario**: Child has 0 points, tries to checkout order for 100 points
- **Current Logic**: Flow 6 validates "Child has enough points"
- **Fix**: ✅ Handled - Validation prevents checkout
- **User Message**: "You don't have enough points. You need 100 more points."

**Case 3.4: Concurrent Point Transactions**
- **Scenario**: Parent confirms order (deducts 100 points) while simultaneously awarding 200 points from task
- **Current Logic**: No transaction locking
- **Fix**: Use database transactions with row-level locking
- **Solution**: Lock user's points row during transaction
- **SQL**: `SELECT Points FROM Users WHERE Id = @UserId WITH (UPDLOCK, ROWLOCK)`

**Case 3.5: Points Calculation Overflow**
- **Scenario**: Child has 9,999 points, earns 10,000 points (total would be 19,999)
- **Current Logic**: Cap enforcement limits to 10,000
- **Fix**: ✅ Handled - Cap enforcement prevents overflow
- **User Message**: "You've reached the maximum points (10,000). 9,999 excess points have been added to family treasury."

**Case 3.6: Negative Balance Prevention**
- **Scenario**: Child has 50 points, order requires 100 points, parent confirms
- **Current Logic**: Flow 7 checks "child balance >= order total"
- **Fix**: ✅ Handled - Validation prevents negative
- **Edge Case**: What if validation passes but balance changes?
- **Solution**: Re-check balance in transaction before deducting

#### 4. Refund System Edge Cases

**Case 4.1: Refund Code Used Multiple Times**
- **Scenario**: Child uses refund code, gets refund, tries to use same code again
- **Current Logic**: Flow 9 validates "RefundCode not already used"
- **Fix**: ✅ Handled - Code invalidated after use
- **User Message**: "This refund code has already been used."

**Case 4.2: Refund Code Expiration**
- **Scenario**: Order fulfilled 60 days ago, child tries to claim refund
- **Current Logic**: No expiration documented
- **Fix**: Add expiration (30 days recommended)
- **Validation**: Check `FulfilledDate + 30 days > GETDATE()`
- **User Message**: "Refund code has expired. Please contact parent for assistance."

**Case 4.3: Invalid Refund Code Format**
- **Scenario**: Child enters "ABC123" instead of "REF-12345-A7B9C2D4"
- **Current Logic**: Flow 9 validates "RefundCode matches order"
- **Fix**: Validate format before database lookup
- **User Message**: "Invalid refund code format. Please check and try again."

**Case 4.4: Refund Code for Wrong Order**
- **Scenario**: Child has two orders, uses refund code from Order A on Order B
- **Current Logic**: Flow 9 validates "RefundCode matches order"
- **Fix**: ✅ Handled - Validation prevents wrong order refund
- **User Message**: "This refund code does not match this order."

**Case 4.5: Refund Process Fails Mid-Transaction**
- **Scenario**: Child claims not fulfilled, refund code validated, but points refund fails
- **Current Logic**: No rollback documented
- **Fix**: Use database transaction, rollback on failure
- **Solution**: 
  ```
  BEGIN TRANSACTION
  TRY:
    - Validate refund code
    - Update order status
    - Refund points to child
    - Deduct from treasury
    - Invalidate refund code
    COMMIT
  CATCH:
    ROLLBACK
    Show error
  ```

#### 5. Status Transition Edge Cases

**Case 5.1: Fulfill Order That's Already Fulfilled**
- **Scenario**: Parent fulfills order, tries to fulfill again
- **Current Logic**: Flow 8 validates "Order status = 'Waiting to Fulfill'"
- **Fix**: ✅ Handled - Validation prevents duplicate fulfillment
- **User Message**: "This order has already been fulfilled."

**Case 5.2: Confirm Fulfillment Twice**
- **Scenario**: Child confirms fulfillment, tries to confirm again
- **Current Logic**: Flow 9 checks if already confirmed
- **Fix**: Check `ChildConfirmedDate IS NULL` before confirming
- **User Message**: "You have already confirmed this order as fulfilled."

**Case 5.3: Decline Order That's Already Confirmed**
- **Scenario**: Parent confirms order, later tries to decline
- **Current Logic**: Decline only works on "Pending" status
- **Fix**: ✅ Handled - Status validation prevents
- **User Message**: "Cannot decline order. Order has already been confirmed."

**Case 5.4: Confirm Order That's Already Declined**
- **Scenario**: Parent declines order, later tries to confirm
- **Current Logic**: Confirm only works on "Pending" status
- **Fix**: ✅ Handled - Status validation prevents
- **User Message**: "Cannot confirm order. Order has already been declined."

#### 6. Data Integrity Edge Cases

**Case 6.1: Reward Deleted But Order Items Reference It**
- **Scenario**: Reward soft-deleted, but order items still reference it
- **Current Logic**: Business Rule prevents deletion if in orders
- **Fix**: ✅ Handled - Cannot delete if in checked-out orders
- **Edge Case**: What if reward was deleted before order?
- **Solution**: On order display, show "Reward no longer available" if deleted

**Case 6.2: Family Treasury Not Initialized**
- **Scenario**: New family, no treasury record exists
- **Current Logic**: TreasuryHelper.InitializeTreasury() should be called
- **Fix**: Auto-initialize on first use
- **Solution**: Check if treasury exists, create if not (balance = 0)

**Case 6.3: Orphaned Orders**
- **Scenario**: Child account deleted, orders still exist
- **Current Logic**: Foreign key constraint prevents deletion
- **Fix**: Soft-delete child account, keep orders for history
- **Solution**: Show "Child account deleted" in order display

**Case 6.4: Reward Point Cost Zero or Negative**
- **Scenario**: Parent creates reward with 0 or negative point cost
- **Current Logic**: Database CHECK constraint `[PointCost] > 0`
- **Fix**: ✅ Handled - Database constraint prevents
- **Additional**: Client-side validation for better UX

---

## 🎨 Message & Notification Design Requirements

### Themed Message Display System

All system messages (success, error, warning, info) must follow the consistent theme design used in TaskReview.aspx.

#### Message Types

1. **Success Messages** (Green theme)
   - Background: `#d4edda` (light green)
   - Border: `#c3e6cb` (green)
   - Text: `#155724` (dark green)
   - Icon: ✓ (checkmark) - Use HTML entity `&#10003;` or `&#10004;`
   - Auto-hide: 5 seconds
   - Position: Top-right corner, fixed

2. **Error Messages** (Red theme)
   - Background: `#f8d7da` (light red)
   - Border: `#f5c6cb` (red)
   - Text: `#721c24` (dark red)
   - Icon: ✗ (cross) - Use HTML entity `&#10007;` or `&#10008;`
   - Auto-hide: 7 seconds
   - Position: Top-right corner, fixed

3. **Warning Messages** (Yellow theme)
   - Background: `#fff3cd` (light yellow)
   - Border: `#ffeaa7` (yellow)
   - Text: `#856404` (dark yellow)
   - Icon: ⚠ (warning) - Use HTML entity `&#9888;`
   - Auto-hide: 6 seconds
   - Position: Top-right corner, fixed

4. **Info Messages** (Blue theme)
   - Background: `#d1ecf1` (light blue)
   - Border: `#bee5eb` (blue)
   - Text: `#0c5460` (dark blue)
   - Icon: ℹ (info) - Use HTML entity `&#8505;` or `&#9432;`
   - Auto-hide: 5 seconds
   - Position: Top-right corner, fixed

#### Message Display Structure

```html
<div class="message-container" id="messageContainer">
    <div class="message message-success" id="successMessage" style="display: none;">
        <span class="message-icon">&#10003;</span>
        <span class="message-text"></span>
        <button class="message-close" onclick="closeMessage('successMessage')">×</button>
    </div>
    <div class="message message-error" id="errorMessage" style="display: none;">
        <span class="message-icon">&#10007;</span>
        <span class="message-text"></span>
        <button class="message-close" onclick="closeMessage('errorMessage')">×</button>
    </div>
    <!-- Similar for warning and info -->
</div>
```

#### CSS Styling (Match TaskReview.aspx Theme)

```css
.message-container {
    position: fixed;
    top: 20px;
    right: 20px;
    z-index: 10000;
    max-width: 400px;
}

.message {
    padding: 15px 20px;
    margin-bottom: 10px;
    border-radius: 5px;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    display: flex;
    align-items: center;
    animation: slideIn 0.3s ease-out;
}

.message-success {
    background-color: #d4edda;
    border-left: 4px solid #28a745;
    color: #155724;
}

.message-error {
    background-color: #f8d7da;
    border-left: 4px solid #dc3545;
    color: #721c24;
}

.message-warning {
    background-color: #fff3cd;
    border-left: 4px solid #ffc107;
    color: #856404;
}

.message-info {
    background-color: #d1ecf1;
    border-left: 4px solid #17a2b8;
    color: #0c5460;
}

.message-icon {
    font-size: 20px;
    margin-right: 10px;
    font-weight: bold;
}

.message-text {
    flex: 1;
    font-size: 14px;
}

.message-close {
    background: none;
    border: none;
    font-size: 20px;
    cursor: pointer;
    margin-left: 10px;
    opacity: 0.7;
}

.message-close:hover {
    opacity: 1;
}

@keyframes slideIn {
    from {
        transform: translateX(100%);
        opacity: 0;
    }
    to {
        transform: translateX(0);
        opacity: 1;
    }
}

@keyframes slideOut {
    from {
        transform: translateX(0);
        opacity: 1;
    }
    to {
        transform: translateX(100%);
        opacity: 0;
    }
}
```

#### JavaScript Functions

```javascript
function showMessage(type, message) {
    const messageId = type + 'Message';
    const messageElement = document.getElementById(messageId);
    const textElement = messageElement.querySelector('.message-text');
    
    textElement.textContent = message;
    messageElement.style.display = 'flex';
    
    // Auto-hide based on type
    const hideDelay = type === 'error' ? 7000 : type === 'warning' ? 6000 : 5000;
    setTimeout(() => {
        closeMessage(messageId);
    }, hideDelay);
}

function closeMessage(messageId) {
    const messageElement = document.getElementById(messageId);
    messageElement.style.animation = 'slideOut 0.3s ease-out';
    setTimeout(() => {
        messageElement.style.display = 'none';
        messageElement.style.animation = 'slideIn 0.3s ease-out';
    }, 300);
}
```

### Symbol Display Requirements

**CRITICAL**: Always use HTML entities for symbols, NEVER use Unicode characters directly in C# code or HTML.

#### Common Symbols to Use

| Symbol | HTML Entity | Usage |
|--------|------------|-------|
| ✓ (Checkmark) | `&#10003;` or `&#10004;` | Success messages |
| ✗ (Cross) | `&#10007;` or `&#10008;` | Error messages |
| ⚠ (Warning) | `&#9888;` | Warning messages |
| ℹ (Info) | `&#8505;` or `&#9432;` | Info messages |
| ★ (Star filled) | `&#9733;` | Star ratings, favorites |
| ☆ (Star empty) | `&#9734;` | Star ratings, favorites |
| × (Close) | `&#215;` or `&times;` | Close buttons |
| → (Arrow) | `&#8594;` or `&rarr;` | Navigation |
| ← (Arrow) | `&#8592;` or `&larr;` | Navigation |
| ↑ (Arrow) | `&#8593;` or `&uarr;` | Up arrow |
| ↓ (Arrow) | `&#8595;` or `&darr;` | Down arrow |
| 💰 (Money) | `&#128176;` | Points, treasury |
| 🛒 (Cart) | `&#128722;` | Shopping cart |
| 📦 (Package) | `&#128230;` | Orders, rewards |

#### Implementation Rules

1. **In ASPX Files (HTML)**:
   ```html
   <!-- CORRECT -->
   <span>&#9733;</span>
   <span>&#10003;</span>
   
   <!-- WRONG - Will show gibberish -->
   <span>★</span>
   <span>✓</span>
   ```

2. **In C# Code-Behind**:
   ```csharp
   // CORRECT
   litStars.Text = new string((char)9733, rating); // For stars
   litIcon.Text = "&#10003;"; // For HTML entities
   
   // WRONG - Will show gibberish
   litStars.Text = "★";
   ```

3. **In JavaScript**:
   ```javascript
   // CORRECT
   icon.innerHTML = '&#10003;';
   icon.innerHTML = String.fromCharCode(10003);
   
   // WRONG
   icon.innerHTML = '✓';
   ```

#### Testing Checklist

- [ ] All success messages show checkmark (not gibberish)
- [ ] All error messages show cross (not gibberish)
- [ ] All warning messages show warning symbol (not gibberish)
- [ ] Star ratings display correctly (not gibberish)
- [ ] Close buttons show × (not gibberish)
- [ ] Icons render correctly in all browsers (Chrome, Firefox, Edge, Safari)

---

## 🔄 Process Flows

### Flow 1: Parent Creates Reward

```
1. Parent navigates to Rewards Management page
2. Clicks "Create Reward" button
3. Fills form:
   - Name (required)
   - Description (optional)
   - Point Cost (required, > 0)
   - Category (optional)
   - Image (optional)
4. Clicks "Save" button
5. Server validates:
   - User is parent
   - User is in family
   - Point cost > 0
   - Name not empty
6. Insert into Rewards table
7. Log action to audit log
8. Show success message
9. Redirect to rewards list
```

**Status**: Reward created with `IsActive = 1`, `IsDeleted = 0`

### Flow 2: Parent Edits Reward

```
1. Parent views reward list
2. Clicks "Edit" on a reward
3. Form pre-populated with existing data
4. Parent modifies fields
5. Clicks "Save Changes"
6. Server validates:
   - User is parent
   - User owns reward (CreatedBy = current user) OR is family owner
   - Point cost > 0
   - Reward not deleted
   - Reward has NO orders that have been checked out
     * Check RewardOrderItems for this reward
     * Check if any RewardOrders with this reward have Status IN ('Pending', 'WaitingToFulfill', 'Fulfilled', 'NotFulfilled')
     * If any found: Show error "Cannot edit reward. This reward is in an order that has been checked out."
7. If validation passes:
   - Update Rewards table
   - Set UpdatedDate = GETDATE(), UpdatedBy = current user
   - Log action to audit log
   - Show success message
8. If validation fails:
   - Show error message
   - Keep form open with data
9. Refresh reward list
```

**Business Rule**: Cannot edit reward if it exists in any order that has been checked out (Status = 'Pending' or later). This prevents changing reward details after a child has already purchased it.

### Flow 3: Parent Deletes Reward

```
1. Parent views reward list
2. Clicks "Delete" on a reward
3. Show confirmation modal: "Are you sure? This will hide the reward from the shop."
4. Parent confirms
5. Server validates:
   - User is parent
   - User owns reward OR is family owner
   - Reward has NO orders that have been checked out
     * Check RewardOrderItems for this reward
     * Check if any RewardOrders with this reward have Status IN ('Pending', 'WaitingToFulfill', 'Fulfilled', 'NotFulfilled')
     * If any found: Show error "Cannot delete reward. This reward is in an order that has been checked out."
6. If validation passes:
   - Soft delete: Set IsDeleted = 1
   - Log action to audit log
   - Show success message
7. If validation fails:
   - Show error message
   - Keep reward visible
8. Refresh reward list
```

**Business Rule**: Cannot delete reward if it exists in any order that has been checked out (Status = 'Pending' or later). This preserves order history and prevents data integrity issues. Only rewards with no checked-out orders can be deleted.

### Flow 4: Child Views Reward Shop

```
1. Child navigates to Reward Shop page
2. Server loads:
   - All active rewards for child's family
   - Child's current point balance
   - Shopping cart items (if any)
3. Display rewards in grid/list view
4. Show:
   - Reward name, description, image
   - Point cost
   - "Add to Cart" button (disabled if child doesn't have enough points)
   - "View Details" button
5. Filter by category (optional)
6. Search by name (optional)
```

**Status**: Read-only view, no database changes

### Flow 5: Child Adds Reward to Cart

```
1. Child clicks "Add to Cart" on a reward
2. Client-side validation:
   - Check if reward already in cart
   - Check if child has enough points (including cart total)
   - Check if quantity + cart total <= available points
3. Add to session/cookie-based cart:
   - RewardId
   - Quantity (default 1)
   - PointCost (snapshot)
4. Update cart total display
5. Show success message: "Added to cart"
6. Update cart icon badge with item count
```

**Storage**: Cart stored in session or localStorage (client-side only, not in database until checkout)

### Flow 6: Child Views Cart & Checks Out

```
1. Child clicks "Cart" icon/button
2. Display cart modal/page with:
   - List of items (reward name, quantity, point cost, subtotal)
   - Total points required
   - Child's current balance
   - Remaining balance after purchase
3. Child can:
   - Remove items
   - Update quantities
   - Clear cart
4. Child clicks "Checkout" button
5. Server validates:
   - Cart not empty
   - All rewards still active and exist
   - Child has enough points (including cap check)
   - Child is not banned
6. Create order:
   - Generate unique OrderNumber
   - Calculate total points
   - Insert into RewardOrders (Status = "Pending")
   - Insert items into RewardOrderItems
7. Reserve points (don't deduct yet - wait for parent confirmation)
8. Log transaction
9. Clear cart
10. Show success message
11. Redirect to "My Orders" page
```

**Status**: Order created with `Status = "Pending"`, points NOT deducted yet

**Edge Cases Handled**:
- Reward deleted while in cart → Validation rejects checkout
- Reward price changed → Use cart snapshot price, show warning
- Reward inactive → Validation rejects checkout
- Child's points insufficient → Validation prevents checkout
- Duplicate checkout clicks → Disable button, check for duplicates
- Cart expiration → Validate items on page load

### Flow 7: Parent Confirms Order

```
1. Parent navigates to "Orders" page
2. View pending orders (Status = "Pending")
3. Click on order to view details:
   - Child name
   - Order items
   - Total points
   - Order date
4. Parent can:
   - Click "Confirm Order" → Status = "Waiting to Fulfill"
   - Click "Decline Order" → Status = "Declined"
5. If Confirm:
   a. Generate unique RefundCode
   b. Update RewardOrders:
      - Status = "Waiting to Fulfill"
      - ConfirmedDate = GETDATE()
      - ConfirmedBy = parent UserId
      - RefundCode = generated code
   c. Deduct points from child:
      - Check child balance >= order total
      - Deduct points (enforce cap: if result > 10000, excess to treasury)
      - Create PointTransaction (Type = "RewardPurchase")
   d. Add points to treasury:
      - Update FamilyTreasury.Balance += order total
      - Create TreasuryTransaction (Type = "Deposit")
   e. Create notification for child
   f. Log all actions
6. If Decline:
   a. Update RewardOrders:
      - Status = "Declined"
      - DeclinedDate = GETDATE()
      - DeclinedBy = parent UserId
      - DeclinedReason = (optional reason)
   b. NO points deducted (order was pending)
   c. Create notification for child
   d. Log action
7. Show success message
8. Refresh orders list
```

**Critical**: Points only deducted when parent confirms, not when child checks out

**Edge Cases Handled**:
- Child's points insufficient at confirmation → Show error, cannot confirm
- Concurrent confirmations → Transaction locking prevents conflicts
- Points deduction fails → Transaction rollback, order status unchanged
- Order status changed → Re-validate status before confirming
- Treasury balance insufficient → Validation prevents confirmation

### Flow 8: Parent Fulfills Order

```
1. Parent navigates to "Orders" page
2. View orders with Status = "Waiting to Fulfill"
3. Click on order
4. Parent gives physical/item to child
5. Parent clicks "Mark as Fulfilled" button
6. Server validates:
   - Order status = "Waiting to Fulfill"
   - Parent is family owner or created reward
7. Update RewardOrders:
   - Status = "Fulfilled"
   - FulfilledDate = GETDATE()
   - FulfilledBy = parent UserId
8. Create notification for child: "Your order has been fulfilled!"
9. Log action
10. Show success message
11. Refresh orders list
```

**Status**: Order marked as fulfilled, waiting for child confirmation

### Flow 9: Child Confirms Fulfillment

```
1. Child navigates to "My Orders" page
2. View orders with Status = "Fulfilled"
3. Click on order
4. Child reviews:
   - Did I receive the item?
   - Is it as described?
5. Child clicks:
   - "Yes, I received it" → Confirm Fulfillment
   - "No, I didn't receive it" → Claim Not Fulfilled
6. If Confirm:
   a. Update RewardOrders:
      - ChildConfirmedDate = GETDATE()
   b. Insert into RewardPurchaseHistory:
      - All order items
      - FulfillmentStatus = "Fulfilled"
      - FulfilledDate = GETDATE()
   c. Create notification for parent
   d. Log action
   e. Show success message
7. If Not Fulfilled:
   a. Show refund code input field
   b. Child enters RefundCode
   c. Server validates:
      - RefundCode matches order
      - Order status = "Fulfilled"
      - RefundCode not already used
   d. Process refund:
      - Update RewardOrders:
        - Status = "NotFulfilled"
        - RefundedDate = GETDATE()
        - RefundedBy = child UserId
      - Refund points to child:
        - Add points back (enforce cap: if result > 10000, excess to treasury)
        - Create PointTransaction (Type = "RewardRefund")
      - Deduct from treasury:
        - Update FamilyTreasury.Balance -= order total
        - Create TreasuryTransaction (Type = "Refund")
      - Insert into RewardPurchaseHistory:
        - FulfillmentStatus = "NotFulfilled"
      - Invalidate RefundCode (set to NULL or mark as used)
      - Create notification for parent
      - Log all actions
   e. Show success message: "Points refunded"
8. Refresh orders list
```

**Security**: RefundCode required to prevent fraudulent refund claims

### Flow 10: Points System Adjustments

#### 10a. Points Awarded (Task Completion)

```
1. Parent reviews task and awards points
2. Server calculates points to award
3. Check treasury balance:
   - If treasury balance < points to award:
     - Show error: "Insufficient treasury balance"
     - Cannot award points
4. If treasury has enough:
   a. Award points to child:
      - Calculate new balance = current + points
      - If new balance > 10000:
        - Child balance = 10000
        - Excess = new balance - 10000
        - Add excess to treasury
        - Create PointTransaction (Type = "ExcessCap", Amount = excess)
      - Else:
        - Child balance = new balance
      - Create PointTransaction (Type = "Earned")
   b. Deduct from treasury:
      - Treasury balance -= points awarded (not excess)
      - Create TreasuryTransaction (Type = "Withdrawal")
   c. Log all actions
```

#### 10b. Points Deducted (Task Failure)

```
1. Parent fails task
2. Server calculates points to deduct (50% of task points)
3. Deduct points from child:
   - Calculate new balance = current - points
   - If new balance < 0:
     - Set balance = 0
     - Actual deducted = current balance (not full amount)
   - Create PointTransaction (Type = "Spent", Amount = actual deducted)
4. Add to treasury:
   - Treasury balance += actual deducted
   - Create TreasuryTransaction (Type = "Deposit")
5. Log all actions
```

#### 10c. Points Cap Enforcement

```
On ANY point transaction:
1. Calculate new balance
2. If new balance > 10000:
   - Excess = new balance - 10000
   - Set balance = 10000
   - Add excess to treasury
   - Create PointTransaction (Type = "ExcessCap")
   - Create TreasuryTransaction (Type = "ExcessCap")
3. If new balance < 0:
   - Set balance = 0
   - Log warning: "Attempted negative balance prevented"
```

---

## 🔌 API Methods

### RewardHelper.cs (New Class)

#### Reward Management

```csharp
// Create reward
public static bool CreateReward(int familyId, int createdBy, string name, 
    string description, int pointCost, string category, string imageUrl)

// Get family rewards (active only)
public static DataTable GetFamilyRewards(int familyId, bool activeOnly = true)

// Get reward details
public static DataRow GetRewardDetails(int rewardId)

// Update reward
// Returns: true if successful, false if failed
// Throws exception if reward has checked-out orders
public static bool UpdateReward(int rewardId, int updatedBy, string name, 
    string description, int pointCost, string category, string imageUrl)
// Validation: Checks HasCheckedOutOrders(rewardId) before updating
// Error: "Cannot edit reward. This reward is in an order that has been checked out."

// Delete reward (soft delete)
// Returns: true if successful, false if failed
// Throws exception if reward has checked-out orders
public static bool DeleteReward(int rewardId, int deletedBy)
// Validation: Checks HasCheckedOutOrders(rewardId) before deleting
// Error: "Cannot delete reward. This reward is in an order that has been checked out."

// Check if reward has orders that have been checked out
// Returns true if reward exists in any order with Status IN ('Pending', 'WaitingToFulfill', 'Fulfilled', 'NotFulfilled')
// Returns false if reward has no orders OR only has orders with Status = 'Declined'
public static bool HasCheckedOutOrders(int rewardId)
// SQL: SELECT COUNT(*) FROM RewardOrderItems roi
//      INNER JOIN RewardOrders ro ON roi.OrderId = ro.Id
//      WHERE roi.RewardId = @RewardId
//        AND ro.Status IN ('Pending', 'WaitingToFulfill', 'Fulfilled', 'NotFulfilled')
```

#### Order Management

```csharp
// Create order from cart
public static int CreateOrder(int childId, int familyId, 
    List<CartItem> cartItems, out string orderNumber)

// Get child orders
// Excludes TransactionComplete and NotFulfilled orders (use GetChildOrderHistory for those)
public static DataTable GetChildOrders(int childId, string status = null)

// Get family orders (for parent)
// Excludes TransactionComplete and NotFulfilled orders (use GetFamilyOrderHistory for those)
public static DataTable GetFamilyOrders(int familyId, string status = null)

// Get order details
public static DataRow GetOrderDetails(int orderId)

// Confirm order (parent)
public static bool ConfirmOrder(int orderId, int confirmedBy, 
    out string refundCode)

// Decline order (parent)
public static bool DeclineOrder(int orderId, int declinedBy, 
    string declinedReason = null)

// Mark order as fulfilled (parent)
public static bool FulfillOrder(int orderId, int fulfilledBy)

// Confirm fulfillment (child)
// Sets Status to 'TransactionComplete' and archives to Order History
public static bool ConfirmFulfillment(int orderId, int childId)

// Claim not fulfilled with refund code (child)
// Sets Status to 'NotFulfilled' and archives to Order History
public static bool ClaimNotFulfilled(int orderId, int childId, 
    string refundCode)

// Get family order history (completed and refunded orders)
public static DataTable GetFamilyOrderHistory(int familyId, 
    DateTime? startDate = null, DateTime? endDate = null)

// Get child order history (completed and refunded orders)
public static DataTable GetChildOrderHistory(int childId, 
    DateTime? startDate = null, DateTime? endDate = null)
```

#### Refund Code Management

```csharp
// Generate unique refund code
private static string GenerateRefundCode(int orderId)

// Validate refund code
public static bool ValidateRefundCode(int orderId, string refundCode)

// Invalidate refund code (after use)
private static void InvalidateRefundCode(int orderId)
```

### TreasuryHelper.cs (New Class)

#### Treasury Management

```csharp
// Get treasury balance
public static int GetTreasuryBalance(int familyId)

// Initialize treasury (if not exists)
public static void InitializeTreasury(int familyId)

// Deposit to treasury
public static bool DepositToTreasury(int familyId, int amount, 
    string description, int? relatedOrderId, int? createdBy)

// Withdraw from treasury
public static bool WithdrawFromTreasury(int familyId, int amount, 
    string description, int? relatedTaskAssignmentId, int? createdBy)

// Get treasury transactions
public static DataTable GetTreasuryTransactions(int familyId, 
    DateTime? startDate = null, DateTime? endDate = null)
```

### PointHelper.cs (Enhanced)

#### Points Cap Enforcement

```csharp
// Award points with cap enforcement
public static bool AwardPointsWithCap(int userId, int points, 
    int familyId, string description, int? taskAssignmentId)

// Deduct points (cannot go negative)
public static bool DeductPoints(int userId, int points, int familyId, 
    string description, int? orderId)

// Get child point balance
public static int GetChildBalance(int userId)

// Check if child can afford purchase
public static bool CanAffordPurchase(int userId, int totalPoints)
```

---

## 💰 Points System Adjustments

### Rules

1. **Child Points Cap**: Maximum 10,000 points per child
2. **No Negative Balance**: Points cannot go below 0
3. **Excess to Treasury**: Points exceeding 10,000 automatically go to treasury
4. **Treasury Source**: All points awarded come from treasury
5. **Treasury Destination**: All points deducted go to treasury

### Implementation

#### When Awarding Points:
```
1. Check treasury balance >= points to award
2. If insufficient: Error, cannot award
3. Award to child: balance = min(current + points, 10000)
4. Calculate excess = max(0, (current + points) - 10000)
5. If excess > 0: Add excess to treasury
6. Deduct awarded amount from treasury
7. Create transactions for all movements
```

#### When Deducting Points:
```
1. Calculate new balance = max(0, current - points)
2. Actual deducted = min(points, current balance)
3. Deduct from child: balance = new balance
4. Add actual deducted to treasury
5. Create transactions for all movements
```

#### Transaction Types:
- `Earned` - Points earned from task completion (from treasury)
- `Spent` - Points deducted for task failure (to treasury)
- `RewardPurchase` - Points deducted for reward purchase (to treasury)
- `RewardRefund` - Points refunded for unfulfilled order (from treasury)
- `ExcessCap` - Points exceeding 10,000 cap (to treasury)

---

## 📊 Status Management

### Order Status Flow

```
Pending
  ↓ (Parent confirms)
Waiting to Fulfill
  ↓ (Parent marks fulfilled)
Fulfilled
  ↓ (Child confirms)
TransactionComplete (archived to Order History)

OR

Pending
  ↓ (Parent declines)
Declined (end transaction, no points deducted)

OR

Waiting to Fulfill
  ↓ (Parent marks fulfilled)
Fulfilled
  ↓ (Child claims not fulfilled with refund code)
Refunded (points refunded, archived to Order History)
```

### Status Definitions

- **Pending**: Child has placed order, waiting for parent confirmation
- **Waiting to Fulfill**: Parent confirmed, points deducted, waiting for fulfillment
- **Fulfilled**: Parent marked as fulfilled, waiting for child confirmation
- **Declined**: Parent declined order, no points deducted, transaction ended
- **Refunded**: Child claimed not fulfilled with refund code, points refunded (archived to Order History)
- **TransactionComplete**: Child confirmed fulfillment, transaction complete (archived to Order History)

### Status Transitions (Allowed)

| From | To | Trigger | Points Action |
|------|-----|---------|---------------|
| Pending | Waiting to Fulfill | Parent confirms | Deduct from child, add to treasury |
| Pending | Declined | Parent declines | No action |
| Waiting to Fulfill | Fulfilled | Parent marks fulfilled | No action |
| Fulfilled | TransactionComplete | Child confirms fulfillment | Record in Order History |
| Fulfilled | Refunded | Child claims with refund code | Refund to child, deduct from treasury, archive to Order History |

### Order History

**Purpose**: Archive completed and refunded orders for historical reference

**Statuses Included**:
- `TransactionComplete` - Order successfully fulfilled and confirmed by child
- `Refunded` - Order refunded due to child claiming not fulfilled (with valid refund code)
- `NotFulfilled` - Legacy status (deprecated, replaced by `Refunded`)

**Access**:
- **Parents**: View all family order history via `OrderHistory.aspx`
- **Children**: View their own order history via `OrderHistory.aspx`

**Features**:
- Date range filtering (optional)
- Complete order timeline (Order Date → Confirmed Date → Fulfilled Date → Child Confirmed Date)
- Order items and totals
- Refund code display (for parents only)
- Read-only view (no action buttons)

**Active Orders vs History**:
- Active order pages (`RewardOrders.aspx`, `MyOrders.aspx`) exclude `TransactionComplete`, `Refunded`, and `Declined` orders
- Only orders requiring action are shown in active order lists
- Completed orders are automatically archived to Order History

### Button Visibility and Error Handling

**Confirm Fulfillment Button Logic** (`MyOrders.aspx`):
- **Visibility Conditions**: Button only shows when ALL of the following are true:
  - Order `Status` = `"Fulfilled"` (parent has marked as fulfilled)
  - `ChildConfirmedDate` is `NULL` (order not already confirmed)
  - Order `Status` ≠ `"TransactionComplete"` (defensive check)
- **Purpose**: Prevents duplicate confirmation attempts and UI confusion

**Claim Not Fulfilled Button Logic** (`MyOrders.aspx`):
- **Visibility Conditions**: Same as Confirm Fulfillment button
- **Purpose**: Ensures refund can only be claimed for unconfirmed fulfilled orders

**Error Handling Improvements**:
- **Pre-validation**: Checks order status and confirmation state before attempting action
- **Specific Error Messages**:
  - "Cannot confirm order. Order status is '{status}'. Only orders marked as 'Fulfilled' by parent can be confirmed."
  - "This order has already been confirmed. It may have been moved to Order History."
  - "Order was already confirmed. Please refresh the page."
- **Post-action Validation**: Re-checks order state after failed attempts to provide accurate feedback
- **Automatic Refresh**: Reloads order list after detecting already-confirmed orders to update UI

**Defensive Checks in Helper Methods**:
- `ConfirmFulfillment`: Checks for `ChildConfirmedDate` and `TransactionComplete` status
- `ClaimNotFulfilled`: Checks for `ChildConfirmedDate` to prevent refunds on confirmed orders
- Both methods log detailed debug information for troubleshooting

---

## 🔐 Transaction Refund System

### Refund Code Generation

**Format**: `REF-{OrderId}-{RandomString}`
**Example**: `REF-12345-A7B9C2D4`

**Generation Algorithm**:
```csharp
private static string GenerateRefundCode(int orderId)
{
    // Generate 8-character random alphanumeric string
    string random = GenerateRandomString(8);
    return $"REF-{orderId}-{random}";
}
```

**Security**:
- Unique per order
- Non-sequential (random component)
- Stored in database when order confirmed
- Invalidated after use

### Refund Process

1. **Child Claims Not Fulfilled**:
   - Child enters refund code
   - System validates code matches order
   - System checks order status = "Fulfilled"
   - System checks code not already used

2. **Process Refund**:
   - Validate refund code matches order
   - Update order status to "Refunded"
   - Refund points to child (with cap enforcement)
   - Deduct from treasury (using existing transaction)
   - Record in purchase history
   - Invalidate refund code (set to NULL)
   - Notify parent

3. **Transaction Records**:
   - PointTransaction (Type = "RewardRefund")
   - TreasuryTransaction (Type = "Refund")
   - RewardPurchaseHistory (FulfillmentStatus = "Refunded")
   - All operations within a single database transaction for atomicity

---

## 🎨 UI/UX Requirements

### Parent Pages

#### 1. Rewards Management Page (`Rewards.aspx`)
- **Header**: Consistent with Tasks page (blue/orange logo, navigation)
- **Actions**: Create, Edit, View, Delete buttons
- **Display**: Grid/list of rewards with:
  - Name, description, point cost
  - Category badge
  - Image thumbnail
  - Active/Inactive status
  - Edit/Delete buttons
  - **Status indicator**: Show if reward has checked-out orders (e.g., "In Use" badge)
- **Edit/Delete Restrictions**:
  - If reward has checked-out orders:
    - Disable Edit button OR show tooltip: "Cannot edit: Reward is in a checked-out order"
    - Disable Delete button OR show tooltip: "Cannot delete: Reward is in a checked-out order"
    - Show visual indicator (e.g., grayed out, lock icon)
  - If reward has no checked-out orders:
    - Enable Edit/Delete buttons normally
- **Create/Edit Modal**: Form with all reward fields
- **Error Display**: Show error message if user attempts to edit/delete reward with checked-out orders
- **Design**: Match Tasks.aspx theme

#### 2. Orders Management Page (`RewardOrders.aspx`)
- **Header**: Consistent theme
- **Filters**: Status filter (Pending, Waiting to Fulfill, Fulfilled, Declined, Refunded)
- **Display**: List of orders with:
  - Order number
  - Child name
  - Order date
  - Total points
  - Status badge (color-coded)
  - Action buttons based on status
- **Order Details Modal**: Full order information
- **Actions**:
  - Confirm (Pending → Waiting to Fulfill)
  - Decline (Pending → Declined)
  - Mark as Fulfilled (Waiting to Fulfill → Fulfilled)

### Child Pages

#### 1. Reward Shop Page (`RewardShop.aspx`)
- **Header**: Consistent theme
- **Display**: Grid of rewards (similar to Tasks page)
- **Filters**: Category filter, search bar
- **Cart Icon**: Badge showing item count
- **Reward Card**: 
  - Image
  - Name, description
  - Point cost
  - "Add to Cart" button (disabled if insufficient points)
  - "View Details" button
- **Design**: Match ChildTasks.aspx theme

#### 2. Shopping Cart Page (`Cart.aspx`)
- **Display**: List of cart items
- **Actions**: Remove item, update quantity, clear cart
- **Summary**: 
  - Total points required
  - Current balance
  - Remaining balance after purchase
- **Checkout Button**: Creates order

#### 3. My Orders Page (`MyOrders.aspx`)
- **Display**: List of child's orders
- **Filters**: Status filter
- **Order Card**:
  - Order number
  - Order date
  - Items list
  - Total points
  - Status badge
  - Action buttons based on status
- **Actions**:
  - Confirm Fulfillment (Fulfilled → History)
  - Claim Not Fulfilled (Fulfilled → NotFulfilled, requires refund code)

#### 4. Purchase History Page (`PurchaseHistory.aspx`)
- **Display**: List of completed/fulfilled orders
- **Information**: 
  - Reward name
  - Point cost
  - Purchase date
  - Fulfillment status
  - Fulfilled date (if fulfilled)

### Design Consistency

- **Colors**: 
  - Primary Blue: `#0066CC`
  - Primary Orange: `#FF6600`
  - Background: `#f5f5f5`
  - Cards: White with shadow
- **Status Badges**: Color-coded (Pending=Yellow, Waiting=Orange, Fulfilled=Green, Declined=Red, NotFulfilled=Red)
- **Buttons**: Consistent styling with Tasks page
- **Modals**: Same confirmation modal design as TaskReview page

---

## 🛡️ Error Handling & Logging

### Error Handling Strategy

#### Client-Side Validation
- Cart total validation (cannot exceed balance)
- Quantity validation (must be > 0)
- Form field validation (required fields, point cost > 0)
- Refund code format validation

#### Server-Side Validation
- User role validation (parent vs child)
- Family membership validation
- Points balance validation (including cap)
- Treasury balance validation
- Order status validation (prevent invalid transitions)
- Refund code validation
- **Reward edit/delete validation**: Check if reward has checked-out orders before allowing edit/delete

#### Error Messages
- **Insufficient Points**: "You don't have enough points. You need {X} more points."
- **Treasury Insufficient**: "Family treasury has insufficient balance to award points."
- **Invalid Refund Code**: "Invalid refund code. Please check and try again."
- **Order Already Processed**: "This order has already been processed."
- **Reward Not Found**: "Reward not found or has been deleted."
- **Reward In Use**: "Cannot edit/delete reward. This reward is in an order that has been checked out."
- **Cap Exceeded**: "Your points have reached the maximum (10,000). Excess points have been added to family treasury."

### Logging Requirements

#### System.Diagnostics.Debug Logging

**Reward Operations**:
```csharp
System.Diagnostics.Debug.WriteLine($"CreateReward: FamilyId={familyId}, CreatedBy={createdBy}, Name={name}, PointCost={pointCost}");
System.Diagnostics.Debug.WriteLine($"UpdateReward: RewardId={rewardId}, UpdatedBy={updatedBy}");
System.Diagnostics.Debug.WriteLine($"UpdateReward Validation: HasCheckedOutOrders={HasCheckedOutOrders(rewardId)}");
System.Diagnostics.Debug.WriteLine($"DeleteReward: RewardId={rewardId}, DeletedBy={deletedBy}");
System.Diagnostics.Debug.WriteLine($"DeleteReward Validation: HasCheckedOutOrders={HasCheckedOutOrders(rewardId)}");
System.Diagnostics.Debug.WriteLine($"HasCheckedOutOrders: RewardId={rewardId}, Result={result}, OrderCount={orderCount}");
```

**Order Operations**:
```csharp
System.Diagnostics.Debug.WriteLine($"CreateOrder: ChildId={childId}, FamilyId={familyId}, TotalPoints={totalPoints}, OrderNumber={orderNumber}");
System.Diagnostics.Debug.WriteLine($"ConfirmOrder: OrderId={orderId}, ConfirmedBy={confirmedBy}, RefundCode={refundCode}");
System.Diagnostics.Debug.WriteLine($"FulfillOrder: OrderId={orderId}, FulfilledBy={fulfilledBy}");
System.Diagnostics.Debug.WriteLine($"ClaimNotFulfilled: OrderId={orderId}, ChildId={childId}, RefundCode={refundCode}");
```

**Points Operations**:
```csharp
System.Diagnostics.Debug.WriteLine($"AwardPointsWithCap: UserId={userId}, Points={points}, CurrentBalance={current}, NewBalance={newBalance}, Excess={excess}");
System.Diagnostics.Debug.WriteLine($"DeductPoints: UserId={userId}, Points={points}, CurrentBalance={current}, NewBalance={newBalance}");
System.Diagnostics.Debug.WriteLine($"PointsCapEnforcement: UserId={userId}, AttemptedBalance={attempted}, CappedBalance={capped}, Excess={excess}");
```

**Treasury Operations**:
```csharp
System.Diagnostics.Debug.WriteLine($"DepositToTreasury: FamilyId={familyId}, Amount={amount}, BalanceBefore={before}, BalanceAfter={after}");
System.Diagnostics.Debug.WriteLine($"WithdrawFromTreasury: FamilyId={familyId}, Amount={amount}, BalanceBefore={before}, BalanceAfter={after}");
System.Diagnostics.Debug.WriteLine($"TreasuryInsufficient: FamilyId={familyId}, Required={required}, Available={available}");
```

**Error Logging**:
```csharp
System.Diagnostics.Debug.WriteLine($"ERROR: {methodName} - {errorMessage}");
System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
```

#### Exception Handling

All methods should have try-catch blocks:
```csharp
try
{
    // Operation
    System.Diagnostics.Debug.WriteLine($"SUCCESS: {methodName}");
    return true;
}
catch (Exception ex)
{
    System.Diagnostics.Debug.WriteLine($"ERROR: {methodName} - {ex.Message}");
    System.Diagnostics.Debug.WriteLine($"Stack Trace: {ex.StackTrace}");
    return false;
}
```

---

## 🔒 Security Considerations

### Authorization
- **Parent Actions**: Only parents can create/edit/delete rewards, confirm/decline orders, fulfill orders
- **Child Actions**: Only children can view shop, add to cart, purchase, confirm fulfillment, claim not fulfilled
- **Family Isolation**: Users can only access rewards/orders from their own family

### Data Validation
- **Point Costs**: Must be > 0
- **Quantities**: Must be > 0
- **Balances**: Cannot go negative, cannot exceed 10,000
- **Refund Codes**: Must match order, must be valid, one-time use

### Transaction Integrity
- **Atomic Operations**: Order creation, point deduction, treasury updates must be atomic
- **Rollback**: If any step fails, rollback all changes
- **Concurrency**: Handle simultaneous order confirmations, point awards
- **Transaction Pattern**: All multi-step operations must use database transactions:
  ```csharp
  using (SqlConnection conn = new SqlConnection(connectionString))
  {
      conn.Open();
      SqlTransaction transaction = conn.BeginTransaction();
      try
      {
          // All operations here
          transaction.Commit();
      }
      catch
      {
          transaction.Rollback();
          throw;
      }
  }
  ```
- **Row-Level Locking**: Use `WITH (UPDLOCK, ROWLOCK)` for critical updates:
  ```sql
  SELECT Balance FROM FamilyTreasury 
  WHERE FamilyId = @FamilyId 
  WITH (UPDLOCK, ROWLOCK)
  ```
- **Deadlock Prevention**: Always lock resources in consistent order (e.g., UserId then FamilyId)

### Refund Code Security
- **Unique**: Each order gets unique code
- **Non-guessable**: Random component prevents guessing
- **One-time use**: Code invalidated after refund
- **Time-bound**: Optional expiration (e.g., 30 days)

---

## 📝 Implementation Checklist

### Phase 1: Database & Backend
- [ ] Create database tables (Rewards, RewardOrders, RewardOrderItems, FamilyTreasury, TreasuryTransactions, RewardPurchaseHistory)
- [ ] Create RewardHelper.cs class
- [ ] Create TreasuryHelper.cs class
- [ ] Enhance PointHelper.cs with cap enforcement
- [ ] Update PointTransactions table schema
- [ ] Implement all API methods
- [ ] Add comprehensive logging

### Phase 2: Points System Adjustments
- [ ] Update task review to use treasury
- [ ] Update task failure to deposit to treasury
- [ ] Implement points cap enforcement
- [ ] Update all point transactions to use treasury
- [ ] Test points cap scenarios
- [ ] Test treasury balance scenarios

### Phase 3: Parent Pages
- [ ] Create Rewards.aspx (CRUD operations)
- [ ] Create RewardOrders.aspx (order management)
- [ ] Create OrderHistory.aspx (completed orders archive)
- [ ] Implement reward creation/edit/delete
- [ ] Implement order confirmation/decline/fulfillment
- [ ] Add error handling and logging

### Phase 4: Child Pages
- [ ] Create RewardShop.aspx (browse rewards)
- [ ] Create Cart.aspx (shopping cart)
- [ ] Create MyOrders.aspx (order tracking)
- [ ] Create OrderHistory.aspx (completed orders archive)
- [ ] Implement cart functionality
- [ ] Implement checkout process
- [ ] Implement fulfillment confirmation (sets Status to TransactionComplete)
- [ ] Implement refund claim with code (sets Status to NotFulfilled)

### Phase 5: Testing & Refinement
- [ ] Test all flows end-to-end
- [ ] Test edge cases (cap, negative, treasury insufficient)
- [ ] Test refund code system
- [ ] Test error scenarios
- [ ] Verify logging
- [ ] Performance testing

---

## 🔗 Integration Points

### With Task System
- Points awarded from task completion come from treasury
- Points deducted from task failure go to treasury
- PointTransactions table links to TaskAssignments

### With Family System
- Treasury is per-family
- Rewards are per-family
- Orders are per-family

### With Notification System
- Notify child when order confirmed
- Notify child when order declined
- Notify child when order fulfilled
- Notify parent when order placed
- Notify parent when refund claimed

---

## 📊 Logic Review Summary

### Issues Identified & Fixed

#### ✅ Fixed Issues
1. **Cart Validation**: Added validation for deleted/inactive rewards on checkout
2. **Price Changes**: Documented handling of reward price changes while in cart
3. **Points Validation**: Added re-validation of child balance before order confirmation
4. **Transaction Integrity**: Added database transaction requirements for all multi-step operations
5. **Concurrency**: Added row-level locking for treasury and points updates
6. **Status Validation**: Added status checks before all status transitions
7. **Refund Code Security**: Added expiration and one-time use validation
8. **Message Design**: Added comprehensive themed message system
9. **Symbol Display**: Added HTML entity requirements to prevent gibberish characters
10. **Order History**: Implemented TransactionComplete status and Order History page for archiving completed orders
11. **Confirm Fulfillment Button Visibility**: Fixed bug where "Confirm Fulfillment" button appeared for already-confirmed orders. Now checks `ChildConfirmedDate` and `TransactionComplete` status before showing buttons
12. **Error Handling for Order Confirmation**: Improved error messages to provide specific feedback when order confirmation fails (already confirmed, wrong status, etc.)
13. **Defensive Checks**: Added additional validation in `ConfirmFulfillment` and `ClaimNotFulfilled` methods to prevent duplicate confirmations and handle edge cases
14. **Refund Process Bug Fixes** (Latest):
    - **Refund Code Validation**: Added `ValidateRefundCode` method to verify refund codes before processing refunds
    - **Transaction Integrity**: Fixed `ClaimNotFulfilled` to use existing `SqlConnection` and `SqlTransaction` objects when calling `TreasuryHelper` methods, ensuring all refund operations occur within a single atomic transaction
    - **Status Update**: Changed refund status from `NotFulfilled` to `Refunded` for better clarity and consistency
    - **History Queries**: Updated `GetFamilyOrderHistory` and `GetChildOrderHistory` to include `Refunded` status
    - **UI Updates**: Added `Refunded` status badge styling to `OrderHistory.aspx` page
    - **Backward Compatibility**: Code supports both `Refunded` (new) and `NotFulfilled` (legacy) statuses for existing records

#### ⚠️ Potential Issues Requiring Implementation Decisions
1. **Cart Storage**: Choose between session/localStorage vs server-side cart
2. **Cart Expiration**: Set expiration time (recommended: 7 days)
3. **Refund Code Expiration**: Set expiration time (recommended: 30 days)
4. **Duplicate Order Prevention**: Choose prevention method (button disable vs database check)
5. **Orphaned Data**: Decide handling of deleted child accounts with orders

#### 🔍 Areas Requiring Additional Testing
1. **Concurrent Transactions**: Test simultaneous order confirmations
2. **Points Cap Edge Cases**: Test exactly at 10,000 points scenarios
3. **Treasury Zero Balance**: Test all operations when treasury is empty
4. **Status Race Conditions**: Test rapid status changes
5. **Cart Expiration**: Test cart behavior after expiration
6. **Refund Code Edge Cases**: Test expired codes, wrong codes, duplicate use

### Implementation Priority

**High Priority** (Critical for System Integrity):
- Database transactions for all multi-step operations
- Row-level locking for treasury and points
- Status validation before transitions
- Cart validation on checkout
- Points balance re-validation before deduction

**Medium Priority** (Important for UX):
- Themed message display system
- HTML entity usage for symbols
- Cart expiration handling
- Duplicate order prevention
- Refund code expiration

**Low Priority** (Nice to Have):
- Server-side cart storage
- Advanced cart merging for multiple devices
- Detailed orphaned data handling

---

**Last Updated**: November 23, 2025  
**Status**: Planning Complete - Logic Review Complete - Implementation In Progress - Bug Fixes Applied

