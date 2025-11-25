# Family Page Left Sidebar & Right-Click Actions - Implementation Plan

## Improvements Requested

### 1. Left Sidebar for Invite Code
- **Location**: Left side of the page (similar to right sidebar)
- **Content**: 
  - Family invite code display
  - Copy code button (visible to all: owner parent, member parent, child)
  - Change invite code button (visible to owner parent only)
- **Layout**: 
  - Fixed position on left wall
  - Full height (top to bottom)
  - Similar styling to right sidebar
- **Center Area**: Leave empty for now

### 2. Right-Click Context Menu for Members
- **Children**: 
  - Right-click shows context menu with "Kick Out" action
  - Available to: Owner parent and regular parents
  - Confirmation modal with design
- **Parents**: 
  - Right-click shows context menu with "Kick Out" action
  - Available to: Owner parent only
  - Confirmation modal with design

## Implementation Plan

### Phase 1: Left Sidebar Structure
1. Create left sidebar HTML structure
2. Move invite code section from center to left sidebar
3. Add copy and change code buttons to left sidebar
4. Style left sidebar to match right sidebar (Discord-style)
5. Make left sidebar fixed position, full height
6. Adjust center content area margin

### Phase 2: Button Visibility Logic
1. Update code-behind to show/hide buttons based on user role
2. Owner parent: Show both Copy and Change buttons
3. Member parent/Child: Show only Copy button
4. Hide Change button for non-owners

### Phase 3: Right-Click Context Menu
1. Add context menu HTML structure
2. Implement right-click event handlers for member items
3. Position context menu near cursor
4. Add "Kick Out" action button
5. Style context menu (Discord-style)

### Phase 4: Confirmation Modals
1. Create styled confirmation modal for kicking children
2. Create styled confirmation modal for kicking parents
3. Add appropriate warning messages
4. Connect to existing kick methods

### Phase 5: Layout Adjustments
1. Update main content area margins for both sidebars
2. Ensure proper spacing and layout
3. Test responsive design

## Implementation Steps

1. Update CSS for left sidebar positioning
2. Move invite code HTML to left sidebar
3. Update button visibility logic
4. Add context menu HTML and CSS
5. Add JavaScript for right-click handling
6. Add confirmation modals for kick actions
7. Connect to existing kick methods
8. Test all functionality

