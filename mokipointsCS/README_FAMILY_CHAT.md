# Family Chat System

## Overview

The Family Chat System is a comprehensive real-time messaging feature integrated into the MokiPoints family management platform. It enables family members (parents and children) to communicate through text messages, images, GIFs, and emoji reactions. The system also automatically posts system messages for task completions, task failures, and new reward items.

## Features

### ✅ Core Features

- **Text Messaging**: Send and receive text messages with real-time updates
- **Image Upload**: Upload and share images (max 50MB) with automatic compression
- **GIF Support**: Search and send GIFs using the Giphy API
- **Emoji Reactions**: React to messages with 5 default emojis (👍 Like, ❤️ Love, 😂 Haha, 😢 Sad, 😠 Angry)
- **System Messages**: Automatic notifications for:
  - Task completions (with rating and points earned)
  - Task failures (with points lost)
  - New reward items added by parents
- **Message Ordering**: Latest messages displayed first (newest at top)
- **Real-time Updates**: Polling mechanism checks for new messages every 3 seconds
- **Auto-sizing Chat Bubbles**: Message bubbles automatically adjust to content length
- **Animation Effects**: New messages have entrance animations (slide down, bounce in, pulse)

### 🎨 UI/UX Features

- **Responsive Design**: Chat adapts to different screen sizes
- **Profile Pictures**: User profile pictures displayed with messages
- **Message Timestamps**: Shows when messages were sent
- **Status Indicators**: Visual feedback for message sending status
- **Distinct Styling**: Different colors for user messages, other messages, and system messages
- **Reward Message Highlighting**: Special styling for new reward notifications (light cyan background)

## Architecture

### Technology Stack

- **Backend**: ASP.NET Web Forms (.NET Framework 4.7.2)
- **Database**: SQL Server LocalDB
- **Frontend**: JavaScript (vanilla), CSS3, HTML5
- **External API**: Giphy API for GIF search
- **Image Processing**: System.Drawing for image compression

### File Structure

```
mokipointsCS/
├── Family.aspx                    # Main chat UI and frontend logic
├── Family.aspx.cs                 # Server-side chat handlers (WebMethods)
├── Family.aspx.designer.cs        # Auto-generated designer file
├── App_Code/
│   ├── ChatHelper.cs              # Core chat business logic
│   ├── ChatImageUploadHandler.cs  # Image upload handler (ASHX)
│   ├── RewardHelper.cs            # Reward creation (triggers system messages)
│   └── DatabaseInitializer.cs    # Database schema and migrations
├── Web.config                     # Configuration (Giphy API key)
└── Images/
    └── FamilyChat/                # Uploaded chat images storage
```

## Database Schema

### FamilyMessages Table

```sql
CREATE TABLE [dbo].[FamilyMessages] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [FamilyId] INT NOT NULL,
    [UserId] INT NULL,                    -- NULL for system messages
    [MessageType] NVARCHAR(20) NOT NULL,  -- 'Text', 'Image', 'GIF', 'System'
    [MessageText] NVARCHAR(MAX) NULL,
    [ImagePath] NVARCHAR(500) NULL,
    [GIFUrl] NVARCHAR(500) NULL,
    [ReplyToMessageId] INT NULL,          -- For reply functionality (future)
    [SystemEventType] NVARCHAR(50) NULL,  -- 'TaskCompleted', 'TaskFailed', 'RewardAdded'
    [SystemEventData] NVARCHAR(MAX) NULL, -- JSON data for system events
    [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
    [IsDeleted] BIT NOT NULL DEFAULT 0,
    FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
    FOREIGN KEY ([ReplyToMessageId]) REFERENCES [dbo].[FamilyMessages]([Id])
)
```

**Note**: The `UserId` column is nullable to support system messages that don't have an associated user.

### FamilyMessageReactions Table

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

## API Endpoints (WebMethods)

### GetChatMessages
**Purpose**: Load initial chat messages when page loads  
**Method**: `POST`  
**Parameters**: None (uses session data)  
**Returns**: JSON array of message objects

### GetNewChatMessages
**Purpose**: Poll for new messages (called every 3 seconds)  
**Method**: `POST`  
**Parameters**: `lastMessageId` (int) - ID of the last loaded message  
**Returns**: JSON array of new message objects

### SendChatMessage
**Purpose**: Send a text message  
**Method**: `POST`  
**Parameters**: `message` (string) - Message text  
**Returns**: JSON object with `success` (bool) and `messageId` (int)

### SendChatGIF
**Purpose**: Send a GIF message  
**Method**: `POST`  
**Parameters**: `gifUrl` (string) - URL of the GIF from Giphy  
**Returns**: JSON object with `success` (bool) and `messageId` (int)

### ToggleReaction
**Purpose**: Add or remove an emoji reaction  
**Method**: `POST`  
**Parameters**: 
- `messageId` (int) - ID of the message
- `reactionType` (string) - Type of reaction ('Like', 'Love', 'Haha', 'Sad', 'Angry')  
**Returns**: JSON object with `success` (bool) and `action` (string: 'added' or 'removed')

### GetMessageReactions
**Purpose**: Get all reactions for a specific message  
**Method**: `POST`  
**Parameters**: `messageId` (int) - ID of the message  
**Returns**: JSON array of reaction objects

## Configuration

### Giphy API Key

The chat system requires a Giphy API key for GIF search functionality. Configure it in `Web.config`:

```xml
<appSettings>
    <add key="GIPHY_API_KEY" value="YOUR_API_KEY_HERE" />
</appSettings>
```

**Getting a Giphy API Key**:
1. Visit [Giphy Developers](https://developers.giphy.com/)
2. Create an account and create a new app
3. Copy your API key
4. Add it to `Web.config`

**Note**: If the API key is missing or invalid, users will see a friendly error message when trying to search for GIFs.

### Image Upload Settings

Image upload settings are configured in `ChatImageUploadHandler.cs`:

```csharp
private const int MAX_FILE_SIZE = 50 * 1024 * 1024;      // 50MB
private const int MAX_IMAGE_WIDTH = 1920;
private const int MAX_IMAGE_HEIGHT = 1080;
private const long JPEG_QUALITY = 85L;                    // 85% quality
```

Images larger than 50MB are automatically compressed to fit within these limits.

## Usage

### For Parents

1. **Accessing Chat**: Navigate to the Family page - the chat is displayed in the center area
2. **Sending Messages**: Type in the message input box and click "Send" or press Enter
3. **Uploading Images**: Click the image upload button, select an image, and it will be uploaded and sent automatically
4. **Sending GIFs**: Click the GIF button, search for a GIF, and click to send
5. **Reacting to Messages**: Click the "React" button on any message, select an emoji from the picker
6. **Viewing System Messages**: System messages appear automatically when:
   - A child completes a task
   - A child fails a task
   - You create a new reward item

### For Children

Children have the same chat functionality as parents:
- Can send text messages, images, and GIFs
- Can react to messages
- Can view all messages and system notifications
- Cannot access family management features (separate from chat)

## Technical Details

### Real-time Updates

The chat uses a **polling mechanism** that checks for new messages every 3 seconds:

```javascript
setInterval(checkForNewMessages, 3000);
```

The polling:
- Fetches new messages since the last loaded message ID
- Appends new messages to the top of the chat (newest first)
- Refreshes reactions on all visible messages
- Applies animation effects to new messages

### Message Ordering

Messages are ordered by `CreatedDate DESC` in the database, and displayed with newest messages at the top. When new messages arrive, they are prepended to the chat container using `prepend()`.

### Emoji Encoding

Emojis are handled using Unicode escape sequences to ensure proper display across different browsers and file encodings:

```javascript
// Example: Party popper emoji (🎉)
emoji = String.fromCharCode(0xD83C, 0xDF89); // Surrogate pair for UTF-16
```

### Image Path Resolution

All image paths are returned as absolute URLs (starting with `/`) to ensure proper loading:
- Profile pictures: `/Images/ProfilePictures/{filename}`
- Chat images: `/Images/FamilyChat/{filename}`

### System Messages

System messages are created with `UserId = NULL` in the database. The frontend handles this by:
- Checking if `UserId` is `null` or `undefined`
- Displaying system messages with distinct styling
- Showing appropriate emojis based on `SystemEventType`:
  - 🎉 for `RewardAdded`
  - ✅ for `TaskCompleted`
  - ❌ for `TaskFailed`

### CSS Animations

New messages have entrance animations defined in CSS:

```css
@keyframes slideDown {
    from { opacity: 0; transform: translateY(-20px); }
    to { opacity: 1; transform: translateY(0); }
}

@keyframes bounceIn {
    0%, 20%, 50%, 80%, 100% { transform: translateY(0); }
    40% { transform: translateY(-10px); }
    60% { transform: translateY(-5px); }
}

@keyframes pulse {
    0%, 100% { transform: scale(1); }
    50% { transform: scale(1.1); }
}
```

## Troubleshooting

### Images Not Loading (404 Errors)

**Problem**: Uploaded images or profile pictures show 404 errors  
**Solution**: 
- Ensure image paths are absolute (starting with `/`)
- Check that the `Images/FamilyChat/` directory exists and has write permissions
- Verify `ChatImageUploadHandler.cs` returns absolute paths

### GIF Search Not Working (403 Errors)

**Problem**: GIF search returns 403 Forbidden errors  
**Solution**:
- Verify `GIPHY_API_KEY` is set in `Web.config`
- Check that the API key is valid and not expired
- Ensure the API key has the correct permissions

### Emojis Displaying as Squares

**Problem**: Emojis appear as squares or gibberish  
**Solution**:
- Ensure UTF-8 encoding is set: `Response.Charset = "utf-8"` and `Response.ContentEncoding = System.Text.Encoding.UTF8`
- Use Unicode escape sequences (`String.fromCharCode()`) instead of literal emoji characters
- For emojis requiring surrogate pairs (like 🎉), use two code points: `String.fromCharCode(0xD83C, 0xDF89)`

### Reactions Not Updating in Real-time

**Problem**: Reactions don't appear on other users' screens without refresh  
**Solution**:
- Ensure `refreshAllReactions()` is called in `checkForNewMessages()`
- Verify polling is active (check browser console for errors)
- Check that `GetMessageReactions` WebMethod is working correctly

### System Messages Not Appearing

**Problem**: Task completion/failure or new reward messages don't show in chat  
**Solution**:
- Verify `UserId` column in `FamilyMessages` table is nullable
- Check that `PostSystemMessage` in `ChatHelper.cs` uses `NULL` for `UserId`
- Ensure `RewardHelper.CreateReward` and task review methods call `ChatHelper.PostSystemMessage`
- Check database migration was applied correctly

### "Object cannot be cast from DBNull" Error

**Problem**: Error when loading messages: "Object cannot be cast from DBNull to other types"  
**Solution**:
- Ensure `GetChatMessages` and `GetNewChatMessages` handle `DBNull.Value` for `UserId`:
  ```csharp
  UserId = row["UserId"] != DBNull.Value ? (int?)Convert.ToInt32(row["UserId"]) : null
  ```

### Chat Bubbles Too Long

**Problem**: Short messages have excessively long chat bubbles  
**Solution**:
- Ensure CSS includes `width: fit-content;` on `.chat-message`
- Verify `.chat-message-bubble` has `display: inline-block; max-width: 100%;`

## Security Considerations

1. **File Upload Security**:
   - Only image files are accepted (validated by MIME type and file extension)
   - File size limited to 50MB
   - Filenames sanitized to prevent path traversal attacks
   - Images stored in a dedicated directory outside web root (if possible)

2. **Message Security**:
   - User input sanitized to prevent XSS attacks
   - SQL injection prevention using parameterized queries
   - Only family members can view/send messages (verified via `FamilyHelper.IsFamilyMember`)

3. **Authentication**:
   - All WebMethods check for authenticated session
   - User must be part of the family to send messages
   - Image upload handler verifies session authentication

## Future Enhancements

Potential improvements for future versions:

- [ ] **Reply Functionality**: Reply to specific messages with quoted text
- [ ] **SignalR Integration**: Replace polling with SignalR for true real-time updates
- [ ] **Message Pagination**: Load older messages on scroll up
- [ ] **Image Lightbox**: Click to view full-size images
- [ ] **Message Search**: Search through chat history
- [ ] **File Attachments**: Support for PDFs, documents, etc.
- [ ] **Read Receipts**: Show when messages are read
- [ ] **Typing Indicators**: Show when someone is typing
- [ ] **Message Editing/Deletion**: Edit or delete sent messages
- [ ] **Mention Notifications**: @mention users in messages

## Contributing

When modifying the chat system:

1. **Database Changes**: Always update `DatabaseInitializer.cs` with migration scripts
2. **Path Handling**: Always use absolute paths (starting with `/`) for images
3. **Encoding**: Always use UTF-8 encoding and Unicode escape sequences for emojis
4. **Testing**: Test with both parent and child accounts
5. **Error Handling**: Add comprehensive error logging using `System.Diagnostics.Debug`
6. **Backward Compatibility**: Ensure changes don't break existing messages or reactions

## License

This chat system is part of the MokiPoints application. All rights reserved.

---

**Last Updated**: December 2024  
**Version**: 1.0  
**Maintainer**: MokiPoints Development Team

