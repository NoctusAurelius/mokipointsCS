# Rewards System - Testing Guide

**Last Updated:** November 23, 2025  
**Status:** Ready for Testing

## Overview

This document provides comprehensive testing procedures for the Rewards System. Follow these test cases to verify all functionality works correctly.

---

## Table of Contents

1. [Pre-Testing Setup](#pre-testing-setup)
2. [Test Environment](#test-environment)
3. [Test Cases - Parent Features](#test-cases---parent-features)
4. [Test Cases - Child Features](#test-cases---child-features)
5. [Test Cases - Points System](#test-cases---points-system)
6. [Test Cases - Order Workflow](#test-cases---order-workflow)
7. [Test Cases - Edge Cases](#test-cases---edge-cases)
8. [Test Cases - Error Handling](#test-cases---error-handling)
9. [Regression Testing](#regression-testing)
10. [Performance Testing](#performance-testing)

---

## Pre-Testing Setup

### 1. Database Preparation

**Option A: Fresh Database**
- Delete existing `App_Data/Mokipoints.mdf` and `App_Data/Mokipoints_log.ldf`
- Restart application - tables will be created automatically

**Option B: Existing Database**
- Verify all tables exist:
  - `Rewards`
  - `RewardOrders`
  - `RewardOrderItems`
  - `FamilyTreasury`
  - `TreasuryTransactions`
  - `RewardPurchaseHistory`
- Check `PointTransactions` has new columns:
  - `TreasuryTransactionId`
  - `IsFromTreasury`
  - `IsToTreasury`
  - `ExcessAmount`
- Check `Users` has constraint:
  - `CK_Users_Points_Range` (Points >= 0 AND Points <= 10000)

### 2. Test Accounts

Create or verify you have:
- **1 Parent Account** (Role: PARENT)
- **2+ Child Accounts** (Role: CHILD)
- All accounts in the same family

### 3. Initial Treasury Balance

**Important:** The treasury starts at 0. You need to add points to the treasury before children can earn points.

**Option A: Via Task System**
- Create a task and assign to a child
- Child completes task
- Parent reviews and awards points
- Points will come from treasury (if treasury has balance)

**Option B: Direct SQL (for testing)**
```sql
-- Add 50,000 points to treasury for testing
UPDATE FamilyTreasury SET Balance = 50000 WHERE FamilyId = 1;
```

---

## Test Environment

### Browser Requirements
- Chrome (latest)
- Firefox (latest)
- Edge (latest)
- Safari (if on Mac)

### Prerequisites
- Application running without errors
- Database connection working
- All pages accessible
- Session management working

---

## Test Cases - Parent Features

### TC-P-001: Create Reward

**Steps:**
1. Login as Parent
2. Navigate to Rewards page
3. Click "+ Create Reward"
4. Fill in form:
   - Name: "Test Reward 1"
   - Description: "This is a test reward"
   - Point Cost: 100
   - Category: "Toys"
   - Image URL: (optional)
5. Click "Create Reward"

**Expected Results:**
- ✅ Success message appears
- ✅ Modal closes
- ✅ Reward appears in rewards grid
- ✅ Reward shows correct information

**Verify:**
- Check database: `SELECT * FROM Rewards WHERE Name = 'Test Reward 1'`
- Verify `IsActive = 1`, `IsDeleted = 0`

---

### TC-P-002: Edit Reward

**Steps:**
1. Login as Parent
2. Navigate to Rewards page
3. Find a reward (not in any orders)
4. Click "Edit"
5. Change point cost to 150
6. Click "Save Changes"

**Expected Results:**
- ✅ Success message appears
- ✅ Modal closes
- ✅ Reward shows updated point cost

**Verify:**
- Check database: `SELECT PointCost FROM Rewards WHERE Id = [rewardId]`
- Should be 150

---

### TC-P-003: Edit Reward (In Use)

**Steps:**
1. Create an order with a reward (see TC-C-005)
2. Parent confirms the order (see TC-P-006)
3. Try to edit the reward
4. Click "Edit"

**Expected Results:**
- ✅ Error message: "Cannot edit reward. This reward is in an order that has been checked out."
- ✅ Edit button should be disabled (grayed out)

**Verify:**
- Check UI: Edit button has `disabled` attribute
- Check UI: "In Use" badge appears on reward card

---

### TC-P-004: Delete Reward

**Steps:**
1. Login as Parent
2. Navigate to Rewards page
3. Find a reward (not in any orders)
4. Click "Delete"
5. Confirm deletion in modal

**Expected Results:**
- ✅ Success message appears
- ✅ Reward disappears from list (soft-deleted)
- ✅ Modal closes

**Verify:**
- Check database: `SELECT IsDeleted FROM Rewards WHERE Id = [rewardId]`
- Should be 1

---

### TC-P-005: Delete Reward (In Use)

**Steps:**
1. Create an order with a reward
2. Parent confirms the order
3. Try to delete the reward
4. Click "Delete"
5. Confirm deletion

**Expected Results:**
- ✅ Error message: "Cannot delete reward. This reward is in an order that has been checked out."
- ✅ Delete button should be disabled

**Verify:**
- Check UI: Delete button has `disabled` attribute

---

### TC-P-006: View Orders

**Steps:**
1. Login as Parent
2. Navigate to Orders page
3. View orders list

**Expected Results:**
- ✅ All family orders displayed
- ✅ Orders show correct status badges
- ✅ Orders show child name, date, total points
- ✅ Order items displayed correctly

**Verify:**
- Check database: `SELECT * FROM RewardOrders WHERE FamilyId = [familyId]`
- Compare with displayed orders

---

### TC-P-007: Confirm Order

**Steps:**
1. Child creates an order (see TC-C-005)
2. Login as Parent
3. Navigate to Orders page
4. Find order with status "Pending"
5. Click "Confirm Order"
6. Confirm in modal

**Expected Results:**
- ✅ Success message appears
- ✅ Order status changes to "WaitingToFulfill"
- ✅ Child's points deducted
- ✅ Points added to treasury
- ✅ Refund code generated
- ✅ Modal closes

**Verify:**
- Check database:
  ```sql
  SELECT Status, RefundCode FROM RewardOrders WHERE Id = [orderId]
  -- Status should be 'WaitingToFulfill'
  -- RefundCode should not be NULL
  ```
- Check child points: `SELECT Points FROM Users WHERE Id = [childId]`
- Check treasury: `SELECT Balance FROM FamilyTreasury WHERE FamilyId = [familyId]`
- Check PointTransaction: `SELECT * FROM PointTransactions WHERE RelatedOrderId = [orderId]`

---

### TC-P-008: Decline Order

**Steps:**
1. Child creates an order
2. Login as Parent
3. Navigate to Orders page
4. Find order with status "Pending"
5. Click "Decline Order"
6. Confirm in modal

**Expected Results:**
- ✅ Success message appears
- ✅ Order status changes to "Declined"
- ✅ Child's points NOT deducted
- ✅ Modal closes

**Verify:**
- Check database: `SELECT Status FROM RewardOrders WHERE Id = [orderId]`
- Should be "Declined"
- Check child points: Should remain unchanged

---

### TC-P-009: Fulfill Order

**Steps:**
1. Confirm an order (see TC-P-007)
2. Navigate to Orders page
3. Find order with status "WaitingToFulfill"
4. Click "Mark as Fulfilled"
5. Confirm in modal

**Expected Results:**
- ✅ Success message appears
- ✅ Order status changes to "Fulfilled"
- ✅ Modal closes

**Verify:**
- Check database: `SELECT Status, FulfilledDate FROM RewardOrders WHERE Id = [orderId]`
- Status should be "Fulfilled"
- FulfilledDate should be set

---

### TC-P-010: Search and Filter Rewards

**Steps:**
1. Login as Parent
2. Navigate to Rewards page
3. Enter search term in search box
4. Select category from filter dropdown

**Expected Results:**
- ✅ Rewards filtered by search term
- ✅ Rewards filtered by category
- ✅ Filtering works in real-time

---

## Test Cases - Child Features

### TC-C-001: Browse Rewards

**Steps:**
1. Login as Child
2. Navigate to Shop page
3. View rewards grid

**Expected Results:**
- ✅ All active rewards displayed
- ✅ Rewards show name, description, point cost, image
- ✅ Points balance displayed in header
- ✅ "Add to Cart" buttons visible

**Verify:**
- Check database: `SELECT * FROM Rewards WHERE FamilyId = [familyId] AND IsActive = 1 AND IsDeleted = 0`
- Compare with displayed rewards

---

### TC-C-002: Add to Cart (Sufficient Points)

**Prerequisites:**
- Child has enough points for reward

**Steps:**
1. Login as Child
2. Navigate to Shop page
3. Find a reward child can afford
4. Click "Add to Cart"

**Expected Results:**
- ✅ Success message appears
- ✅ Item added to cart
- ✅ Cart count updates (if displayed)

**Verify:**
- Check session: `Session["Cart"]` should contain reward ID

---

### TC-C-003: Add to Cart (Insufficient Points)

**Prerequisites:**
- Child does NOT have enough points for reward

**Steps:**
1. Login as Child
2. Navigate to Shop page
3. Find a reward child cannot afford
4. Try to click "Add to Cart"

**Expected Results:**
- ✅ "Add to Cart" button disabled
- ✅ Button text shows "Not Enough Points"
- ✅ Reward card has reduced opacity (unaffordable class)

**Verify:**
- Check UI: Button has `disabled` attribute
- Check UI: Card has `unaffordable` class

---

### TC-C-004: View Cart

**Prerequisites:**
- Cart has items

**Steps:**
1. Login as Child
2. Add items to cart (see TC-C-002)
3. Navigate to Cart page

**Expected Results:**
- ✅ All cart items displayed
- ✅ Quantities shown correctly
- ✅ Subtotals calculated correctly
- ✅ Total calculated correctly
- ✅ Points balance displayed

**Verify:**
- Check session: `Session["Cart"]` contents
- Verify calculations: Quantity × PointCost = Subtotal

---

### TC-C-005: Update Cart Quantity

**Steps:**
1. Login as Child
2. Navigate to Cart page
3. Use +/- buttons to change quantity
4. Or type quantity directly

**Expected Results:**
- ✅ Quantity updates immediately
- ✅ Subtotal recalculates
- ✅ Total recalculates
- ✅ Page refreshes with updated values

**Verify:**
- Check session: `Session["Cart"]` should reflect new quantities

---

### TC-C-006: Remove from Cart

**Steps:**
1. Login as Child
2. Navigate to Cart page
3. Click "Remove" on an item

**Expected Results:**
- ✅ Success message appears
- ✅ Item removed from cart
- ✅ Cart updates (or shows empty state)

**Verify:**
- Check session: Item should be removed from `Session["Cart"]`

---

### TC-C-007: Checkout (Sufficient Points)

**Prerequisites:**
- Cart has items
- Child has enough points

**Steps:**
1. Login as Child
2. Navigate to Cart page
3. Verify total points
4. Click "Checkout"

**Expected Results:**
- ✅ Success message appears
- ✅ Order created
- ✅ Cart cleared
- ✅ Redirected to My Orders page
- ✅ Order appears with status "Pending"

**Verify:**
- Check database:
  ```sql
  SELECT * FROM RewardOrders WHERE ChildId = [childId] ORDER BY OrderDate DESC
  -- Should have new order with Status = 'Pending'
  ```
- Check order items: `SELECT * FROM RewardOrderItems WHERE OrderId = [orderId]`
- Check session: `Session["Cart"]` should be null

---

### TC-C-008: Checkout (Insufficient Points)

**Prerequisites:**
- Cart has items
- Child does NOT have enough points

**Steps:**
1. Login as Child
2. Navigate to Cart page
3. Verify insufficient points warning appears
4. Try to click "Checkout"

**Expected Results:**
- ✅ Warning message: "You don't have enough points..."
- ✅ Checkout button disabled
- ✅ Points needed displayed

**Verify:**
- Check UI: Button has `disabled` attribute
- Check calculation: Total - Balance = Points Needed

---

### TC-C-009: View My Orders

**Steps:**
1. Login as Child
2. Navigate to My Orders page
3. View orders list

**Expected Results:**
- ✅ All child orders displayed
- ✅ Orders show correct status badges
- ✅ Orders show date, total points
- ✅ Order items displayed correctly

**Verify:**
- Check database: `SELECT * FROM RewardOrders WHERE ChildId = [childId]`
- Compare with displayed orders

---

### TC-C-010: View Refund Code

**Prerequisites:**
- Order confirmed by parent (status: WaitingToFulfill)

**Steps:**
1. Login as Child
2. Navigate to My Orders page
3. Find order with status "WaitingToFulfill"
4. View order details

**Expected Results:**
- ✅ Refund code displayed in blue box
- ✅ Code is 8 characters, alphanumeric
- ✅ Code is clearly visible

**Verify:**
- Check database: `SELECT RefundCode FROM RewardOrders WHERE Id = [orderId]`
- Should match displayed code

---

### TC-C-011: Confirm Fulfillment

**Prerequisites:**
- Order status: WaitingToFulfill

**Steps:**
1. Login as Child
2. Navigate to My Orders page
3. Find order with status "WaitingToFulfill"
4. Click "Confirm Received"
5. Confirm in modal

**Expected Results:**
- ✅ Success message appears
- ✅ Order status changes to "Fulfilled"
- ✅ Entry created in RewardPurchaseHistory
- ✅ Modal closes

**Verify:**
- Check database:
  ```sql
  SELECT Status FROM RewardOrders WHERE Id = [orderId]
  -- Should be 'Fulfilled'
  ```
- Check purchase history: `SELECT * FROM RewardPurchaseHistory WHERE OrderId = [orderId]`

---

### TC-C-012: Claim Not Fulfilled (Valid Code)

**Prerequisites:**
- Order status: WaitingToFulfill
- Have refund code

**Steps:**
1. Login as Child
2. Navigate to My Orders page
3. Find order with status "WaitingToFulfill"
4. Click "Not Received"
5. Enter refund code
6. Click "Claim Not Fulfilled"

**Expected Results:**
- ✅ Success message appears
- ✅ Order status changes to "NotFulfilled"
- ✅ Points refunded to child
- ✅ Points deducted from treasury
- ✅ Refund code marked as used
- ✅ Modal closes

**Verify:**
- Check database:
  ```sql
  SELECT Status, RefundCodeUsed FROM RewardOrders WHERE Id = [orderId]
  -- Status should be 'NotFulfilled'
  -- RefundCodeUsed should be 1
  ```
- Check child points: Should be increased by order total
- Check treasury: Should be decreased by order total
- Check PointTransaction: Should have refund transaction

---

### TC-C-013: Claim Not Fulfilled (Invalid Code)

**Steps:**
1. Login as Child
2. Navigate to My Orders page
3. Find order with status "WaitingToFulfill"
4. Click "Not Received"
5. Enter WRONG refund code
6. Click "Claim Not Fulfilled"

**Expected Results:**
- ✅ Error message: "Invalid refund code or order cannot be refunded."
- ✅ Order status unchanged
- ✅ Points NOT refunded

**Verify:**
- Check database: Order status should still be "WaitingToFulfill"
- Check child points: Should be unchanged

---

### TC-C-014: Claim Not Fulfilled (Used Code)

**Steps:**
1. Use a refund code (see TC-C-012)
2. Try to use the same code again

**Expected Results:**
- ✅ Error message: "Invalid refund code or order cannot be refunded."
- ✅ Refund fails

**Verify:**
- Check database: `SELECT RefundCodeUsed FROM RewardOrders WHERE Id = [orderId]`
- Should be 1

---

## Test Cases - Points System

### TC-PT-001: Points Cap Enforcement

**Steps:**
1. Set child points to 9,500
2. Award 1,000 points via task review
3. Check child balance

**Expected Results:**
- ✅ Child balance = 10,000 (capped)
- ✅ Excess 500 points go to treasury
- ✅ PointTransaction has ExcessAmount = 500

**Verify:**
- Check database:
  ```sql
  SELECT Points FROM Users WHERE Id = [childId]
  -- Should be 10000
  ```
- Check treasury: Should have +500
- Check PointTransaction: `SELECT ExcessAmount FROM PointTransactions WHERE ...`

---

### TC-PT-002: Negative Points Prevention

**Steps:**
1. Set child points to 100
2. Try to deduct 200 points (via order or direct)

**Expected Results:**
- ✅ Deduction fails or stops at 0
- ✅ Child balance = 0 (not negative)

**Verify:**
- Check database: `SELECT Points FROM Users WHERE Id = [childId]`
- Should be >= 0
- Check constraint: `CK_Users_Points_Range` should prevent negative

---

### TC-PT-003: Treasury Integration - Points Earning

**Steps:**
1. Set treasury balance to 1,000
2. Award 500 points to child via task review
3. Check balances

**Expected Results:**
- ✅ Treasury balance = 500 (decreased)
- ✅ Child balance = 500 (increased)
- ✅ TreasuryTransaction recorded

**Verify:**
- Check treasury: `SELECT Balance FROM FamilyTreasury WHERE FamilyId = [familyId]`
- Check child: `SELECT Points FROM Users WHERE Id = [childId]`
- Check treasury transactions: `SELECT * FROM TreasuryTransactions WHERE FamilyId = [familyId]`

---

### TC-PT-004: Treasury Integration - Points Spending

**Steps:**
1. Set child points to 500
2. Set treasury balance to 1,000
3. Deduct 300 points (via order confirmation)
4. Check balances

**Expected Results:**
- ✅ Child balance = 200 (decreased)
- ✅ Treasury balance = 1,300 (increased)
- ✅ TreasuryTransaction recorded

**Verify:**
- Check treasury: Should be +300
- Check child: Should be -300

---

### TC-PT-005: Treasury Insufficient Balance

**Steps:**
1. Set treasury balance to 100
2. Try to award 500 points to child

**Expected Results:**
- ✅ Award fails
- ✅ Error logged
- ✅ Balances unchanged

**Verify:**
- Check treasury: Should still be 100
- Check child: Should be unchanged

---

## Test Cases - Order Workflow

### TC-OW-001: Complete Order Flow

**Full End-to-End Test:**

1. **Parent creates reward**
   - Create reward: "Test Toy", 200 points

2. **Child shops**
   - Child browses shop
   - Adds reward to cart
   - Views cart
   - Checks out

3. **Parent manages order**
   - Parent views orders
   - Confirms order
   - Marks as fulfilled

4. **Child confirms**
   - Child views orders
   - Confirms receipt

**Expected Results:**
- ✅ All steps complete successfully
- ✅ Points flow correctly
- ✅ Status transitions correctly
- ✅ All database records created

**Verify:**
- Check all tables for correct data
- Verify point transactions
- Verify treasury transactions

---

### TC-OW-002: Order Decline Flow

**Steps:**
1. Child creates order
2. Parent declines order
3. Check balances

**Expected Results:**
- ✅ Order status = "Declined"
- ✅ Child points NOT deducted
- ✅ Treasury NOT affected

**Verify:**
- Check database: Order status
- Check child points: Unchanged
- Check treasury: Unchanged

---

### TC-OW-003: Refund Flow

**Steps:**
1. Complete order flow (see TC-OW-001) up to "WaitingToFulfill"
2. Child claims not fulfilled with refund code
3. Check balances

**Expected Results:**
- ✅ Order status = "NotFulfilled"
- ✅ Child points refunded
- ✅ Treasury decreased
- ✅ Refund code marked as used

**Verify:**
- Check all balances
- Check refund code status

---

## Test Cases - Edge Cases

### TC-EC-001: Empty Cart Checkout

**Steps:**
1. Login as Child
2. Navigate to Cart page (empty)
3. Try to checkout

**Expected Results:**
- ✅ Empty cart message displayed
- ✅ Checkout button not visible or disabled

---

### TC-EC-002: Multiple Items in Cart

**Steps:**
1. Add 3 different rewards to cart
2. Add same reward twice (quantity = 2)
3. Checkout

**Expected Results:**
- ✅ All items in order
- ✅ Quantities correct
- ✅ Total calculated correctly

**Verify:**
- Check RewardOrderItems: Should have 4 rows (3 unique + 1 duplicate)

---

### TC-EC-003: Reward Deletion with Pending Order

**Steps:**
1. Child creates order (not confirmed)
2. Parent tries to delete reward in order

**Expected Results:**
- ✅ Reward can be deleted (order not checked out yet)
- ✅ Order still exists but reward is soft-deleted

**Note:** This is expected behavior - only checked-out orders prevent deletion.

---

### TC-EC-004: Concurrent Order Creation

**Steps:**
1. Two children add same reward to cart
2. Both checkout simultaneously

**Expected Results:**
- ✅ Both orders created
- ✅ Both orders have correct items
- ✅ No data corruption

---

## Test Cases - Error Handling

### TC-EH-001: Invalid Refund Code Format

**Steps:**
1. Try to claim not fulfilled with invalid code format
2. Enter code with wrong length or characters

**Expected Results:**
- ✅ Validation error
- ✅ Refund fails

---

### TC-EH-002: Database Connection Error

**Steps:**
1. Stop database
2. Try to create reward

**Expected Results:**
- ✅ Error message displayed
- ✅ Application doesn't crash
- ✅ Error logged

---

### TC-EH-003: Session Expiry

**Steps:**
1. Add items to cart
2. Wait for session to expire (or clear session)
3. Try to checkout

**Expected Results:**
- ✅ Redirected to login
- ✅ Cart preserved (if session still valid) or cleared

---

## Regression Testing

### Verify Task System Still Works

**Test:**
1. Create task
2. Assign to child
3. Child completes task
4. Parent reviews and awards points

**Expected Results:**
- ✅ All task system features work
- ✅ Points awarded correctly
- ✅ Points come from treasury
- ✅ Cap enforced

---

## Performance Testing

### TC-PF-001: Large Reward List

**Steps:**
1. Create 100 rewards
2. Load Rewards page
3. Load Shop page

**Expected Results:**
- ✅ Pages load in < 3 seconds
- ✅ No performance degradation

---

### TC-PF-002: Large Order List

**Steps:**
1. Create 50 orders
2. Load Orders page
3. Load My Orders page

**Expected Results:**
- ✅ Pages load in < 3 seconds
- ✅ Filtering works smoothly

---

## Test Checklist

Use this checklist to track testing progress:

### Parent Features
- [ ] Create Reward
- [ ] Edit Reward
- [ ] Edit Reward (In Use) - Should Fail
- [ ] Delete Reward
- [ ] Delete Reward (In Use) - Should Fail
- [ ] View Orders
- [ ] Confirm Order
- [ ] Decline Order
- [ ] Fulfill Order
- [ ] Search/Filter Rewards

### Child Features
- [ ] Browse Rewards
- [ ] Add to Cart (Sufficient Points)
- [ ] Add to Cart (Insufficient Points)
- [ ] View Cart
- [ ] Update Cart Quantity
- [ ] Remove from Cart
- [ ] Checkout (Sufficient Points)
- [ ] Checkout (Insufficient Points)
- [ ] View My Orders
- [ ] View Refund Code
- [ ] Confirm Fulfillment
- [ ] Claim Not Fulfilled (Valid Code)
- [ ] Claim Not Fulfilled (Invalid Code)
- [ ] Claim Not Fulfilled (Used Code)

### Points System
- [ ] Points Cap Enforcement
- [ ] Negative Points Prevention
- [ ] Treasury Integration - Earning
- [ ] Treasury Integration - Spending
- [ ] Treasury Insufficient Balance

### Order Workflow
- [ ] Complete Order Flow
- [ ] Order Decline Flow
- [ ] Refund Flow

### Edge Cases
- [ ] Empty Cart Checkout
- [ ] Multiple Items in Cart
- [ ] Reward Deletion with Pending Order
- [ ] Concurrent Order Creation

### Error Handling
- [ ] Invalid Refund Code Format
- [ ] Database Connection Error
- [ ] Session Expiry

### Regression
- [ ] Task System Still Works

### Performance
- [ ] Large Reward List
- [ ] Large Order List

---

## Reporting Issues

When reporting issues, include:
1. **Test Case ID** (e.g., TC-C-001)
2. **Steps to Reproduce**
3. **Expected Result**
4. **Actual Result**
5. **Screenshots** (if applicable)
6. **Error Messages** (from browser console and output logs)
7. **Database State** (relevant queries)

---

## Notes

- All test cases assume a fresh database or clean test data
- Treasury balance must be initialized before testing point earning
- Use different test accounts for parent and child testing
- Clear browser cache if experiencing session issues

---

**End of Testing Guide**

