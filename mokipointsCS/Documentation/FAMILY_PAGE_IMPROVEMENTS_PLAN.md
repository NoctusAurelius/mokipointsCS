# Family Page Improvements - Implementation Plan

## Overview
Redesign the Family page with a Discord-style sidebar showing family members, enhanced owner management, and improved family code management features.

## Requirements

### 1. Remove Child Monitoring Section
- Remove the existing `pnlChildrenMonitoring` panel and all related code
- Remove `LoadChildrenMonitoring` method
- Remove `rptChildren` repeater and related event handlers

### 2. Discord-Style Sidebar
- **Layout**: Fixed sidebar on the left, main content on the right
- **Sections**:
  - **Parents Section**: List all parent members
  - **Children Section**: List all child members
- **Member Display**:
  - Profile picture (or placeholder)
  - Full name
  - Owner badge/indicator for owner
- **Child Hover Tooltip**:
  - Points amount
  - Completed tasks count
  - Failed tasks count

### 3. Owner Management Features
- **Owner Indicator**: Visual badge showing who is the owner
- **Owner Restrictions**:
  - Owner cannot leave family if there are children
  - Owner can transfer ownership to another parent
  - Owner can leave if only parents remain
  - Auto-transfer ownership to first parent (by join date) if owner leaves
- **Owner Actions**:
  - Transfer ownership (dropdown/button for each parent)
  - Kick other parents (button with confirmation)
  - Leave family (with validation)

### 4. Family Code Management
- **Copy Button**: Button next to family code to copy to clipboard
  - Success message when copied
  - Error message if copy fails
- **Change Code Button**: Button to generate new family code
  - Confirmation modal before changing
  - Success/error messages
  - Watch for gibberish bugs (use HTML entities)

### 5. Backend Methods Needed (FamilyHelper.cs)
- `GetFamilyMembers(int familyId)` - Get all members with role, join date, owner status
- `GetFamilyOwnerId(int familyId)` - Get current owner ID
- `TransferOwnership(int familyId, int newOwnerId, int currentOwnerId)` - Already exists
- `KickParent(int familyId, int parentId, int ownerId)` - Remove parent from family
- `ChangeFamilyCode(int familyId, int ownerId)` - Generate new family code
- `CanOwnerLeave(int familyId)` - Check if owner can leave (no children)
- `GetFirstParentByJoinDate(int familyId)` - Get first parent (by join date) for auto-transfer

## Implementation Steps

### Step 1: Update FamilyHelper.cs
1. Add `GetFamilyMembers` method
2. Add `GetFamilyOwnerId` method (if not exists)
3. Add `KickParent` method
4. Add `ChangeFamilyCode` method
5. Add `CanOwnerLeave` method
6. Add `GetFirstParentByJoinDate` method

### Step 2: Redesign Family.aspx
1. Remove child monitoring section
2. Create sidebar layout structure
3. Add parent and child sections
4. Add hover tooltip structure
5. Add copy code button
6. Add change code button
7. Add owner action buttons (transfer, kick, leave)

### Step 3: Update Family.aspx.cs
1. Remove `LoadChildrenMonitoring` method
2. Remove `rptChildren_ItemDataBound` and `rptChildren_ItemCommand` handlers
3. Add `LoadFamilyMembers` method
4. Add event handlers for:
   - Copy code button
   - Change code button
   - Transfer ownership
   - Kick parent
   - Leave family

### Step 4: Add JavaScript
1. Copy to clipboard functionality
2. Hover tooltip for children
3. Confirmation modals
4. Success/error message display

### Step 5: CSS Styling
1. Sidebar layout styles
2. Member list styles
3. Hover tooltip styles
4. Owner badge styles
5. Button styles

### Step 6: Testing
1. Test sidebar display
2. Test hover tooltips
3. Test copy functionality
4. Test owner restrictions
5. Test ownership transfer
6. Test kick parent
7. Test change code
8. Check for gibberish bugs

## Files to Modify
- `Family.aspx` - Complete redesign
- `Family.aspx.cs` - Update code-behind
- `Family.aspx.designer.cs` - Update control declarations
- `App_Code/FamilyHelper.cs` - Add new methods

## Files to Create
- None (all changes to existing files)

## Notes
- Use HTML entities for all special characters to prevent gibberish bugs
- Ensure responsive design for sidebar
- Add comprehensive logging for debugging
- Follow existing code patterns and conventions

