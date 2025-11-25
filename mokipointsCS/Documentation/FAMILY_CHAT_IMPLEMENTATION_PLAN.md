# Family Chat Implementation Plan

## Overview
Implement a comprehensive family group chat system in the center area of the Family page, replacing the empty space. The chat will support text messages, photo uploads, replies, emoji reactions, GIFs, and automatic system messages for task completions/failures and new rewards.

## Features to Implement

### 1. Core Chat Functionality
- Real-time message display (polling or SignalR)
- Send text messages with confirmation (success/failed indicators)
- Message history with pagination
- Timestamp display
- User identification (profile picture, name)
- Message ordering (newest at bottom)

### 2. Photo Upload
- Upload photos (max 50MB)
- Auto-compress if file size exceeds limit
- Upload progress bar
- Error logging and handling
- Display uploaded images in chat
- Image preview/lightbox

### 3. Reply Functionality
- Reply to specific messages (like messenger)
- Show quoted message in reply
- Thread-like display
- Reply indicator on original message

### 4. Emoji Reactions
- 5 default emojis: Like (👍), Love (❤️), Haha (😂), Sad (😢), Angry (😠)
- Click to add/remove reaction
- Show reaction count and who reacted
- Multiple reactions per message allowed

### 5. GIF Support
- GIF search functionality
- Integration with GIF API (Giphy/Tenor)
- Send GIFs to chat
- Display GIFs inline

### 6. System Messages
- Auto-post when child completes task
- Auto-post when child fails task
- Auto-post when parent adds new reward
- System messages styled differently (no user info, system icon)

### 7. Child Account Access
- Allow children to access Family page
- Limited permissions:
  - Can view chat
  - Can send messages
  - Can copy family code (not change)
  - Cannot right-click to kick users
  - Cannot access owner actions

## Database Schema

### New Tables Required

#### FamilyMessages Table
```sql
CREATE TABLE [dbo].[FamilyMessages] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [FamilyId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [MessageType] NVARCHAR(20) NOT NULL, -- 'Text', 'Image', 'GIF', 'System'
    [MessageText] NVARCHAR(MAX) NULL,
    [ImagePath] NVARCHAR(500) NULL,
    [GIFUrl] NVARCHAR(500) NULL,
    [ReplyToMessageId] INT NULL, -- For reply functionality
    [SystemEventType] NVARCHAR(50) NULL, -- 'TaskCompleted', 'TaskFailed', 'RewardAdded'
    [SystemEventData] NVARCHAR(MAX) NULL, -- JSON data for system events
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
    FOREIGN KEY ([ReplyToMessageId]) REFERENCES [dbo].[FamilyMessages]([Id])
)
```

#### FamilyMessageReactions Table
```sql
CREATE TABLE [dbo].[FamilyMessageReactions] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [MessageId] INT NOT NULL,
    [UserId] INT NOT NULL,
    [ReactionType] NVARCHAR(20) NOT NULL, -- 'Like', 'Love', 'Haha', 'Sad', 'Angry'
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    FOREIGN KEY ([MessageId]) REFERENCES [dbo].[FamilyMessages]([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
    UNIQUE ([MessageId], [UserId], [ReactionType]) -- One reaction type per user per message
)
```

## Implementation Phases

### Phase 1: Database & Backend Foundation
1. Create database tables (FamilyMessages, FamilyMessageReactions)
2. Create ChatHelper.cs with methods:
   - `SendMessage()`
   - `GetMessages()`
   - `UploadImage()`
   - `AddReaction()`
   - `RemoveReaction()`
   - `GetReactions()`
3. Create image upload handler (ASHX or API endpoint)
4. Add database migration to DatabaseInitializer.cs

### Phase 2: UI Layout & Basic Chat
1. Update Family.aspx layout:
   - Remove "Family Management" header
   - Add family name as header
   - Create chat container in center area
   - Chat message list area
   - Message input area
2. Basic message display (text only)
3. Send message functionality
4. Message confirmation indicators

### Phase 3: Photo Upload
1. File upload control with progress bar
2. Image compression logic (if > 50MB)
3. Image storage in Images/FamilyChat/
4. Display images in chat
5. Error handling and logging

### Phase 4: Reply Functionality
1. Reply button on messages
2. Reply UI (show quoted message)
3. Reply display in chat
4. Backend support for replies

### Phase 5: Emoji Reactions
1. Reaction button on messages
2. Reaction picker UI
3. Display reactions on messages
4. Add/remove reaction functionality
5. Show who reacted

### Phase 6: GIF Integration
1. GIF search UI
2. GIF API integration (Giphy or Tenor)
3. GIF selection and sending
4. Display GIFs in chat

### Phase 7: System Messages
1. Hook into `TaskHelper.ReviewTask()` method:
   - After successful review, call `ChatHelper.PostSystemMessage()` for task completion/failure
   - Include: task name, child name, rating (if completed), points awarded, family ID
2. Hook into `RewardHelper.CreateReward()` method:
   - After successful creation, call `ChatHelper.PostSystemMessage()` for new reward
   - Include: reward name, description, point cost, family ID
3. Auto-post system messages
4. System message styling

### Phase 8: Child Account Access
1. Update Family.aspx.cs to allow CHILD role
2. Hide owner actions for children
3. Disable right-click context menu for children
4. Show only copy button (not change) for children

### Phase 9: Real-time Updates
1. Implement polling or SignalR for real-time updates
2. Auto-refresh message list
3. Show "new messages" indicator

### Phase 10: Polish & Testing
1. Error handling
2. Loading states
3. Empty states
4. Responsive design
5. Performance optimization

## Technical Considerations

### Questions to Clarify:

1. **Real-time Updates**: 
   - Should we use SignalR for real-time updates, or polling every X seconds?
   - Recommendation: Start with polling (simpler), upgrade to SignalR later if needed

2. **GIF API**:
   - Which GIF API to use? (Giphy, Tenor, etc.)
   - Do we need an API key?
   - Recommendation: Giphy (free tier available)

3. **Image Compression**:
   - What compression quality/format? (JPEG quality, max dimensions?)
   - Recommendation: JPEG quality 85%, max 1920x1080 for photos

4. **Message Pagination**:
   - How many messages to load initially?
   - Infinite scroll or "Load More" button?
   - Recommendation: Load last 50 messages, infinite scroll

5. **System Message Format**:
   - **Task Completed**: "{ChildName} completed '{TaskName}' and earned {Points} points! ⭐{Rating}"
   - **Task Failed**: "{ChildName} failed '{TaskName}' and lost {Points} points"
   - **Reward Added**: "New reward available: '{RewardName}' for {PointCost} points! {Description}"

6. **File Storage**:
   - Store in Images/FamilyChat/ or separate folder?
   - Recommendation: Images/FamilyChat/

7. **Message Limits**:
   - Character limit for text messages?
   - Recommendation: 5000 characters

8. **Child Permissions**:
   - Can children upload photos?
   - Can children send GIFs?
   - Recommendation: Yes to both

## File Structure

### New Files to Create:
- `App_Code/ChatHelper.cs` - Chat business logic
- `Handlers/ChatImageUpload.ashx` - Image upload handler
- `Scripts/family-chat.js` - Client-side chat JavaScript (optional, or inline)

### Files to Modify:
- `Family.aspx` - Add chat UI
- `Family.aspx.cs` - Add chat handlers, allow CHILD role
- `App_Code/DatabaseInitializer.cs` - Add table creation
- `App_Code/TaskHelper.cs` - Hook into `ReviewTask()` method (line ~1043) after successful review
- `App_Code/RewardHelper.cs` - Hook into `CreateReward()` method (line ~20) after successful creation

## UI/UX Design

### Chat Layout (Facebook-style):
- **Header**: Family name (replaces "Family Management")
- **Message List**: Scrollable area showing messages
- **Message Input**: Bottom fixed input area with:
  - Text input
  - Photo upload button
  - GIF button
  - Send button
- **Message Bubbles**: 
  - User messages on right (blue)
  - Other messages on left (gray)
  - System messages centered (yellow/neutral)
  - Profile picture + name
  - Timestamp
  - Reactions below message
  - Reply button

## Security Considerations

1. **File Upload Security**:
   - Validate file types (images only)
   - Scan for malicious content
   - Limit file size (50MB)
   - Sanitize filenames

2. **Message Security**:
   - Sanitize user input (XSS prevention)
   - SQL injection prevention (parameterized queries)
   - Only family members can see messages

3. **Permission Checks**:
   - Verify user is in family before sending messages
   - Verify user permissions for actions

## Estimated Implementation Time

- Phase 1: 2-3 hours
- Phase 2: 3-4 hours
- Phase 3: 2-3 hours
- Phase 4: 2 hours
- Phase 5: 2-3 hours
- Phase 6: 3-4 hours
- Phase 7: 2-3 hours
- Phase 8: 1 hour
- Phase 9: 2-3 hours
- Phase 10: 2-3 hours

**Total**: ~23-30 hours

## Next Steps

1. **Answer clarifying questions** (see Technical Considerations section)
2. **Confirm GIF API choice** and obtain API key if needed
3. **Start with Phase 1** (Database & Backend Foundation)
4. **Iterative development** - implement and test each phase

---

## Questions for User (Before Implementation)

Please answer the following questions to proceed:

1. **Real-time Updates**: 
   - Should we use SignalR for real-time updates, or polling every X seconds?
   - **Recommendation**: Start with polling (simpler), upgrade to SignalR later if needed
   - **Your choice**: [ ]

2. **GIF API**:
   - Which GIF API to use? (Giphy, Tenor, etc.)
   - Do you have an API key, or should we use a free tier?
   - **Recommendation**: Giphy (free tier available, no key needed for basic usage)
   - **Your choice**: [ ]

3. **Image Compression**:
   - What compression quality/format? (JPEG quality, max dimensions?)
   - **Recommendation**: JPEG quality 85%, max 1920x1080 for photos
   - **Your choice**: [ ]

4. **Message Pagination**:
   - How many messages to load initially?
   - Infinite scroll or "Load More" button?
   - **Recommendation**: Load last 50 messages, infinite scroll
   - **Your choice**: [ ]

5. **File Storage**:
   - Store in Images/FamilyChat/ or separate folder?
   - **Recommendation**: Images/FamilyChat/
   - **Your choice**: [ ]

6. **Message Limits**:
   - Character limit for text messages?
   - **Recommendation**: 5000 characters
   - **Your choice**: [ ]

7. **Child Permissions**:
   - Can children upload photos?
   - Can children send GIFs?
   - **Recommendation**: Yes to both
   - **Your choice**: [ ]

8. **Polling Interval** (if using polling):
   - How often should we check for new messages? (in seconds)
   - **Recommendation**: 3-5 seconds
   - **Your choice**: [ ]

