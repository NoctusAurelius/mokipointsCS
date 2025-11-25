# Family Page Fixes - Implementation Plan

## Issues Identified

### Issue 1: SQL Error - Invalid Column Name 'CreatedDate'
**Error**: `GetFamilyMembers error: Invalid column name 'CreatedDate'.`
**Root Cause**: The `FamilyMembers` table uses `JoinedDate` column, not `CreatedDate`.
**Location**: `FamilyHelper.cs` - `GetFamilyMembers()` method, line 669 and 681

### Issue 2: JavaScript Error - Family Code Element Not Found
**Error**: `Family code element not found`
**Root Cause**: The JavaScript `copyFamilyCode()` function is looking for the wrong element ID or the element doesn't exist when the function runs.
**Location**: `Family.aspx` - JavaScript function `copyFamilyCode()`

## Fix Plan

### Fix 1: Update SQL Query to Use Correct Column Name
- Change `fm.CreatedDate` to `fm.JoinedDate` in `GetFamilyMembers()` query
- Update ORDER BY clause to use `fm.JoinedDate` instead of `fm.CreatedDate`
- Verify the query works with existing database

### Fix 2: Fix JavaScript Copy Function
- Check if the element exists before trying to access it
- Use correct element selector (may need to check actual rendered ID)
- Add null checks and better error handling
- Ensure the function only runs when the family info panel is visible

### Fix 3: Database Migration (if needed)
- Check if existing databases have `CreatedDate` column
- Add migration to rename `CreatedDate` to `JoinedDate` if it exists
- Or add `JoinedDate` column if it doesn't exist

## Implementation Steps

1. Fix `GetFamilyMembers()` SQL query
2. Fix JavaScript copy function
3. Test with existing database
4. Add database migration if needed

