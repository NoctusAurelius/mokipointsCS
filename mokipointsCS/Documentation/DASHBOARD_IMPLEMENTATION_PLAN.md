# Dashboard Implementation Plan

## Overview
This document outlines the plan for implementing comprehensive dashboard metrics for both Parent and Child dashboards.

## Important Notes
- **ALWAYS use HTML entities for icons/symbols** (e.g., `&#9733;` for stars, `&#10003;` for checkmarks)
- **NEVER use Unicode characters directly** (they will show as gibberish)
- All statistics should be real-time and pulled from the database

---

## Phase 1: Create DashboardHelper.cs

### Purpose
Centralized helper class for all dashboard statistics and metrics.

### Methods to Create:

#### Parent Dashboard Methods:
1. `GetTreasuryBalance(int familyId)` - Returns current treasury balance
2. `GetPendingReviewsCount(int familyId)` - Count of tasks awaiting review
3. `GetPendingOrdersCount(int familyId)` - Count of orders awaiting confirmation
4. `GetActiveChildrenCount(int familyId)` - Count of active children
5. `GetTasksCompletedToday(int familyId)` - Tasks completed today
6. `GetPointsAwardedThisWeek(int familyId)` - Total points awarded this week
7. `GetRecentActivity(int familyId, int limit = 10)` - Recent activity feed
8. `GetChildActivityOverview(int familyId)` - List of children with their stats
9. `GetWeeklyStatistics(int familyId)` - Weekly stats (tasks, points, etc.)
10. `GetMonthlyStatistics(int familyId)` - Monthly stats

#### Child Dashboard Methods:
1. `GetChildPoints(int userId)` - Current point balance
2. `GetActiveTasksCount(int userId)` - Tasks assigned and in progress
3. `GetCompletedThisWeek(int userId)` - Tasks completed this week
4. `GetPendingReviewsCount(int userId)` - Tasks submitted awaiting review
5. `GetAvailableRewardsCount(int userId, int familyId)` - Rewards child can afford
6. `GetActiveOrdersCount(int userId)` - Orders in progress
7. `GetRecentActivity(int userId, int limit = 10)` - Recent activity feed
8. `GetWeeklyStatistics(int userId)` - Weekly stats
9. `GetMonthlyStatistics(int userId)` - Monthly stats
10. `GetStreakCount(int userId)` - Consecutive days with completed tasks

---

## Phase 2: High Priority Implementation

### Parent Dashboard (ParentDashboard.aspx)

#### Metrics Cards to Update:
1. **Treasury Balance Card**
   - Use `DashboardHelper.GetTreasuryBalance(familyId)`
   - Format: `{balance:N0} points`
   - Icon: `&#128176;` (money bag)

2. **Pending Reviews Card**
   - Use `DashboardHelper.GetPendingReviewsCount(familyId)`
   - Make clickable → links to `TaskReview.aspx`
   - Icon: `&#10003;` (checkmark) or `&#9733;` (star)
   - Badge: Show count in red if > 0

3. **Pending Orders Card**
   - Use `DashboardHelper.GetPendingOrdersCount(familyId)`
   - Make clickable → links to `RewardOrders.aspx`
   - Icon: `&#128230;` (package)
   - Badge: Show count in orange if > 0

4. **Active Children Card**
   - Use `DashboardHelper.GetActiveChildrenCount(familyId)`
   - Icon: `&#128106;` (people)

5. **Tasks Completed Today Card**
   - Use `DashboardHelper.GetTasksCompletedToday(familyId)`
   - Icon: `&#10004;` (checkmark)

6. **Points Awarded This Week Card**
   - Use `DashboardHelper.GetPointsAwardedThisWeek(familyId)`
   - Icon: `&#128176;` (money bag)

### Child Dashboard (ChildDashboard.aspx)

#### Hero Section:
1. **Points Display**
   - Use `DashboardHelper.GetChildPoints(userId)`
   - Update the large points value
   - Add progress bar showing progress toward 10,000 cap
   - Progress calculation: `(currentPoints / 10000) * 100`

#### Metrics Cards to Update:
1. **My Active Tasks Card**
   - Use `DashboardHelper.GetActiveTasksCount(userId)`
   - Icon: `&#128203;` (clipboard)

2. **Completed This Week Card**
   - Use `DashboardHelper.GetCompletedThisWeek(userId)`
   - Icon: `&#10004;` (checkmark)

3. **Pending Reviews Card**
   - Use `DashboardHelper.GetPendingReviewsCount(userId)`
   - Icon: `&#9203;` (hourglass)

4. **Available Rewards Card**
   - Use `DashboardHelper.GetAvailableRewardsCount(userId, familyId)`
   - Icon: `&#127873;` (gift)

5. **Active Orders Card**
   - Use `DashboardHelper.GetActiveOrdersCount(userId)`
   - Icon: `&#128230;` (package)

---

## Phase 3: Medium Priority Implementation

### Recent Activity Feeds

#### Parent Dashboard:
- Section: "Recent Activity"
- Use `DashboardHelper.GetRecentActivity(familyId, 10)`
- Display:
  - Task completions (child name, task, points, time)
  - Order confirmations/fulfillments
  - Reward purchases
  - Point transactions
- Format: Timeline-style list
- Icons: Use appropriate HTML entities for each activity type

#### Child Dashboard:
- Section: "Recent Activity"
- Use `DashboardHelper.GetRecentActivity(userId, 10)`
- Display:
  - Points earned (task completions)
  - Points spent (reward purchases)
  - Task completions
  - Order updates
- Format: Timeline-style list

### Quick Action Buttons with Badges

#### Parent Dashboard:
1. **Review Tasks** button
   - Link to `TaskReview.aspx`
   - Badge: Show `GetPendingReviewsCount` if > 0
   - Icon: `&#9733;` (star)

2. **Manage Orders** button
   - Link to `RewardOrders.aspx`
   - Badge: Show `GetPendingOrdersCount` if > 0
   - Icon: `&#128230;` (package)

3. **Create Task** button
   - Link to `Tasks.aspx`
   - Icon: `&#10133;` (plus)

4. **Create Reward** button
   - Link to `Rewards.aspx`
   - Icon: `&#127873;` (gift)

#### Child Dashboard:
1. **View My Tasks** button
   - Link to `ChildTasks.aspx`
   - Badge: Show `GetActiveTasksCount` if > 0
   - Icon: `&#128203;` (clipboard)

2. **Shop Rewards** button
   - Link to `RewardShop.aspx`
   - Badge: Show `GetAvailableRewardsCount` if > 0
   - Icon: `&#127873;` (gift)

3. **My Orders** button
   - Link to `MyOrders.aspx`
   - Badge: Show `GetActiveOrdersCount` if > 0
   - Icon: `&#128230;` (package)

### Weekly/Monthly Statistics

#### Parent Dashboard:
- New section: "This Week's Summary"
- Use `DashboardHelper.GetWeeklyStatistics(familyId)`
- Display:
  - Total tasks completed
  - Total points awarded
  - Average completion rate
  - Top performing child

#### Child Dashboard:
- New section: "This Week's Summary"
- Use `DashboardHelper.GetWeeklyStatistics(userId)`
- Display:
  - Tasks completed
  - Points earned
  - Points spent
  - Completion rate

### Child Activity Overview (Parent Dashboard)

- New section: "Child Activity"
- Use `DashboardHelper.GetChildActivityOverview(familyId)`
- Display for each child:
  - Name
  - Current point balance
  - Tasks pending review count
  - Recent activity indicator (green/yellow/red dot)
  - Link to child's profile/history

---

## Phase 4: Low Priority Implementation

### Progress Bars and Visualizations

#### Child Dashboard:
1. **Points Progress Bar**
   - Show progress toward 10,000 cap
   - Visual bar with percentage
   - Color: Blue to orange gradient

2. **Weekly Completion Chart**
   - Simple bar chart showing daily completions
   - Use CSS/HTML5 canvas or simple div bars

#### Parent Dashboard:
1. **Family Points Distribution**
   - Pie chart or bar chart showing points distribution
   - Each child's current balance

### Streak Counters

#### Child Dashboard:
- New card: "Current Streak"
- Use `DashboardHelper.GetStreakCount(userId)`
- Display:
  - Number of consecutive days
  - Fire icon: `&#128293;` (fire)
  - Motivational message

---

## Implementation Order

1. ✅ Create `DashboardHelper.cs` with all methods
2. ✅ Update Parent Dashboard with High Priority metrics
3. ✅ Update Child Dashboard with High Priority metrics
4. ✅ Add Recent Activity feeds
5. ✅ Add Quick Action buttons with badges
6. ✅ Add Weekly/Monthly statistics
7. ✅ Add Child Activity Overview
8. ✅ Add Progress bars
9. ✅ Add Streak counters

---

## HTML Entity Reference

Common icons to use (always use HTML entities):
- Star: `&#9733;` (filled), `&#9734;` (empty)
- Checkmark: `&#10003;` or `&#10004;`
- Money: `&#128176;`
- Package: `&#128230;`
- Gift: `&#127873;`
- Clipboard: `&#128203;`
- People: `&#128106;`
- Fire: `&#128293;`
- Plus: `&#10133;`
- Hourglass: `&#9203;`

---

## Testing Checklist

- [ ] All metrics display correct values
- [ ] Icons display correctly (no gibberish)
- [ ] Badges show correct counts
- [ ] Links navigate to correct pages
- [ ] Recent activity shows latest items
- [ ] Progress bars calculate correctly
- [ ] Streak counters work correctly
- [ ] Performance is acceptable (no slow queries)
- [ ] Error handling for missing data
- [ ] Responsive design works on mobile

---

## Notes

- All database queries should be optimized
- Use caching where appropriate (but keep it simple for now)
- Handle edge cases (no family, no tasks, etc.)
- Display friendly messages when data is empty
- Use consistent styling with existing pages

