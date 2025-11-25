# Implementation Plan - November 23, 2025

## Overview
This plan covers improvements and bug fixes based on user testing feedback.

---

## Task 1: Top Navigation Bar Uniformity

### Goal
Standardize navigation bar across all pages for both parent and child accounts, matching dashboard style with profile picture and name.

### Current State
- ParentDashboard.aspx has: Family, Tasks, Review, Rewards, Orders + Profile pic + Name + Settings
- ChildDashboard.aspx has: (need to check) + Profile pic + Name + Settings
- Other pages have inconsistent navigation

### Parent Pages to Update
1. Tasks.aspx
2. TaskReview.aspx
3. Rewards.aspx
4. RewardOrders.aspx
5. Family.aspx
6. AssignTask.aspx
7. TaskTemplates.aspx
8. OrderHistory.aspx
9. Notifications.aspx (remove notification button if not implemented)

### Child Pages to Update
1. ChildTasks.aspx
2. TaskHistory.aspx
3. RewardShop.aspx
4. Cart.aspx
5. MyOrders.aspx
6. PointsHistory.aspx
7. OrderHistory.aspx
8. Notifications.aspx (remove notification button if not implemented)

### Implementation Steps
1. Extract navigation HTML/CSS from ParentDashboard.aspx
2. Extract navigation HTML/CSS from ChildDashboard.aspx
3. Create standard navigation template for parent pages
4. Create standard navigation template for child pages
5. Update each page to use standard navigation
6. Remove notification button from pages where notification system doesn't exist
7. Ensure profile picture and name display correctly on all pages

### Navigation Structure (Parent)
```
Brand | Family | Tasks | Review | Rewards | Orders | [Profile Pic] | Welcome, [Name] | [Settings]
```

### Navigation Structure (Child)
```
Brand | Tasks | Shop | My Orders | Points | [Profile Pic] | Welcome, [Name] | [Settings]
```

---

## Task 2: Reward Availability Status Feature

### Goal
Add availability status management for rewards (Available, Out of Stock, Hidden) with business rule preventing status change if there are existing orders.

### Database Changes
1. Add `AvailabilityStatus` column to `Rewards` table
   - Type: NVARCHAR(50)
   - Values: 'Available', 'OutOfStock', 'Hidden'
   - Default: 'Available'
   - Add to DatabaseInitializer.cs

### Backend Changes
1. Update `RewardHelper.cs`:
   - Add `UpdateRewardAvailability(int rewardId, string availabilityStatus, int userId)` method
   - Add validation: Check if reward has existing orders (checked-out orders)
   - Update `GetFamilyRewards` to filter by availability status for children
   - Update `HasCheckedOutOrders` or create similar method for availability validation

2. Update `Rewards.aspx.cs`:
   - Add availability status dropdown/buttons in reward card
   - Add handler for status change
   - Show error if status change not allowed (existing orders)

### Frontend Changes
1. Update `Rewards.aspx`:
   - Add availability status display (badge)
   - Add availability status controls (dropdown or buttons)
   - Show "Cannot change status" message if reward has orders
   - Update reward card styling for different statuses

2. Update `RewardShop.aspx`:
   - Filter rewards by availability status (only show 'Available' and 'OutOfStock')
   - Show "Out of Stock" badge for OutOfStock items
   - Disable "Add to Cart" for OutOfStock items

### Business Rules
- **Available**: Show to all children, can be purchased
- **OutOfStock**: Show to all children, cannot be purchased (button disabled)
- **Hidden**: Hide from all children (not shown in shop)
- **Status Change Restriction**: Cannot change status if reward has existing orders (checked-out orders only, not pending)

### Implementation Steps
1. Add database column via DatabaseInitializer.cs
2. Update RewardHelper with availability methods
3. Update Rewards.aspx UI
4. Update Rewards.aspx.cs handlers
5. Update RewardShop.aspx to filter by availability
6. Test all scenarios

---

## Task 3: Family Page Bug Fix

### Issue
Child monitoring section not displaying:
- Actual child points
- Actual count of completed tasks
- Actual count of failed tasks

### Root Cause Analysis
Looking at `FamilyHelper.GetFamilyChildrenWithStats`:
- Points calculation: Uses SUM from PointTransactions (may be incorrect - should use Users.Points)
- CompletedTasks: Counts TaskAssignments with TaskReviews where IsFailed = 0 and Status = 'Reviewed'
- FailedTasks: Counts TaskAssignments with TaskReviews where IsFailed = 1 and Status = 'Reviewed'

### Potential Issues
1. Points: Should use `Users.Points` directly instead of calculating from PointTransactions
2. CompletedTasks/FailedTasks: Query may be incorrect - need to verify TaskReviews table structure and relationships

### Fix Strategy
1. Update `GetFamilyChildrenWithStats` query:
   - Change TotalPoints to use `u.Points` directly
   - Verify CompletedTasks query logic
   - Verify FailedTasks query logic
   - Test with actual data

2. Verify TaskReviews table structure:
   - Check if IsFailed column exists
   - Check if Status = 'Reviewed' is correct (may need to check TaskAssignments.Status instead)

### Implementation Steps
1. Check database schema for TaskReviews table
2. Update GetFamilyChildrenWithStats query
3. Test with sample data
4. Verify display in Family.aspx

---

## Execution Order

1. **Task 3** (Bug Fix) - Quick fix, high priority
2. **Task 1** (Navigation Uniformity) - User experience improvement
3. **Task 2** (Reward Availability) - New feature

---

## Testing Checklist

### Task 1: Navigation Uniformity
- [ ] All parent pages have consistent navigation
- [ ] All child pages have consistent navigation
- [ ] Profile picture displays on all pages
- [ ] User name displays on all pages
- [ ] Navigation links work correctly
- [ ] Notification button removed where not implemented
- [ ] Active page highlighted in navigation

### Task 2: Reward Availability
- [ ] Database column added successfully
- [ ] Can change reward status to Available
- [ ] Can change reward status to OutOfStock
- [ ] Can change reward status to Hidden
- [ ] Cannot change status if reward has existing orders
- [ ] OutOfStock rewards show in shop but cannot be purchased
- [ ] Hidden rewards do not show in shop
- [ ] Available rewards show in shop and can be purchased

### Task 3: Family Page Bug Fix
- [ ] Child points display correctly
- [ ] Completed tasks count displays correctly
- [ ] Failed tasks count displays correctly
- [ ] All children show correct statistics

---

## Files to Modify

### Task 1: Navigation Uniformity
- All parent .aspx files (Tasks, TaskReview, Rewards, RewardOrders, Family, AssignTask, TaskTemplates, OrderHistory)
- All child .aspx files (ChildTasks, TaskHistory, RewardShop, Cart, MyOrders, PointsHistory, OrderHistory)
- May need to update .aspx.cs files for profile picture loading

### Task 2: Reward Availability
- `App_Code/DatabaseInitializer.cs` (add column)
- `App_Code/RewardHelper.cs` (add methods)
- `Rewards.aspx` (add UI)
- `Rewards.aspx.cs` (add handlers)
- `RewardShop.aspx` (filter by availability)
- `RewardShop.aspx.cs` (update query)

### Task 3: Family Page Bug Fix
- `App_Code/FamilyHelper.cs` (fix GetFamilyChildrenWithStats query)
- `Family.aspx` (verify display - may not need changes)

---

**Plan Created**: November 23, 2025  
**Status**: Ready for Execution

