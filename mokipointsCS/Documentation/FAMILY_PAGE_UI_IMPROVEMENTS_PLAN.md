# Family Page UI Improvements - Implementation Plan

## Improvements Requested

1. **Move Sidebar to Right Side** - Like Discord, sidebar should be on the right
2. **Full Height Sidebar** - Sidebar should span from top to bottom
3. **Match Button Design** - Copy and Change buttons should match the sidebar container design
4. **Styled Message Confirmations** - Add design to confirmation dialogs for changing code and leaving family
5. **Message Responses** - Add styled message responses after changing code and/or leaving family

## Implementation Plan

### 1. Layout Changes
- **Current**: Sidebar on left, main content on right
- **New**: Main content on left, sidebar on right (Discord-style)
- **Full Height**: Sidebar should use `position: fixed` or `height: calc(100vh - header_height)` to span full viewport height

### 2. Sidebar Styling
- Update `.family-sidebar` to:
  - Position on right side
  - Full height (top to bottom)
  - Match Discord aesthetic (dark/light theme based on current design)
  - Fixed position or sticky positioning

### 3. Button Design Updates
- **Copy Button**: Match sidebar container style
- **Change Button**: Match sidebar container style
- Both buttons should have consistent styling with the sidebar

### 4. Confirmation Dialogs
- Replace browser `confirm()` with custom styled modal
- Design should match the application theme
- Include icons and proper styling
- For "Change Code" confirmation
- For "Leave Family" confirmation

### 5. Message Display System
- Add styled message panels for:
  - Success message after changing code
  - Success message after leaving family
  - Error messages if operations fail
- Messages should appear at top of page or in a toast-style notification
- Auto-dismiss after a few seconds or manual close

## Implementation Steps

1. Update CSS for sidebar positioning (right side, full height)
2. Update button styles to match sidebar design
3. Create custom confirmation modal component
4. Update JavaScript to use custom modals instead of browser confirm()
5. Add message display system with styled panels
6. Update code-behind to show messages after operations
7. Test all functionality

