# Family Sidebar Fixes - Implementation Plan

## Issues to Fix

1. **Sidebar Positioning**: Sidebar should stick to the right wall (fixed position)
2. **Full Height**: Sidebar should reach from top nav bar to bottom of viewport
3. **Tooltip Positioning**: Hover modals should display on the LEFT side of sidebar items
4. **Tooltip Overflow**: Tooltips should be allowed to display outside the container (not get cut off)
5. **Main Content Adjustment**: Main content should account for fixed sidebar width

## Implementation Plan

### 1. Sidebar Positioning
- Change from `position: sticky` to `position: fixed`
- Position on right side: `right: 0`
- Calculate height: `height: calc(100vh - header_height)`
- Set `top` to match header height

### 2. Tooltip Positioning
- Change tooltip `left: 100%` to `right: 100%` (show on left side)
- Adjust margin from `margin-left: 10px` to `margin-right: 10px`
- Ensure tooltips can overflow parent containers

### 3. Container Overflow
- Add `overflow: visible` to sidebar and parent containers
- Remove any `overflow: hidden` that might clip tooltips
- Ensure tooltips have high z-index to appear above other elements

### 4. Main Content Layout
- Add right margin/padding to main content equal to sidebar width
- Ensure content doesn't overlap with fixed sidebar

### 5. Responsive Considerations
- On mobile, sidebar should overlay or stack below content
- Tooltips should still work on mobile (touch-friendly)

## Implementation Steps

1. Update `.family-sidebar` CSS for fixed positioning
2. Update `.family-layout` to account for fixed sidebar
3. Update tooltip CSS to show on left side
4. Fix overflow issues for tooltips
5. Adjust main content spacing
6. Test tooltip positioning and overflow

