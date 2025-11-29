# MokiPoints - Family Chore & Reward Management System

**Version**: 5.0.4  
**Last Updated**: December 2024  
**Status**: ✅ Active Development

---

## 🆕 Latest Updates (Patch 5.0.4)

### ✅ Validation & Security Improvements

**Patch 5.0.4** includes comprehensive validation, security enhancements, and user experience improvements:

#### Security Enhancements
- **Duplicate Account Prevention**: System now prevents creating accounts with the same first name, last name, and birthday combination, even with different email addresses
- **Account Identity Verification**: Enhanced registration validation to prevent duplicate identities

#### Validation Improvements
- **Task Creation Limits**: Maximum reward points per task set to 1,000 points
- **Reward Creation Limits**: Maximum point cost per reward set to 10,000 points (matching child's maximum point cap)
- **Age Validation**: Child accounts must be between 8 and 19 years old during registration
- **Task Deadline Limits**: Task deadlines cannot exceed 30 days in the future
- **Task Deadline Restrictions**: Deadlines cannot be set on the current date (must be tomorrow or later)

#### Task Timer System
- **Auto-Fail Timer**: Tasks now have a timer that starts when a child accepts the task
- **Timer Range**: 10 minutes (minimum) to 24 hours (maximum)
- **Auto-Fail on Expiration**: Tasks automatically fail if timer expires before submission
- **Real-Time Countdown**: Timer countdown displayed on ongoing tasks with auto-refresh on expiration

#### User Experience Improvements
- **Collapsible Information Panels**: Information/instruction panels in Task Creation, Reward Creation, and Parent Dashboard are now hidden behind hover buttons to save space
- **Welcome Messages**: Automatic welcome messages posted to family chat when new members join
- **Multiple Child Assignment**: Parents can now assign the same task to multiple children simultaneously
- **Button Loading States**: Create Task button now disables after click to prevent duplicate submissions
- **Individual Child Metrics**: Parent Dashboard now shows individual child task completion/failure metrics instead of aggregate statistics

#### Bug Fixes
- Fixed encoding issues (gibberish characters) in information panels
- Fixed auto-refresh when timer expires in child tasks
- Fixed family rejoin bug (UNIQUE constraint violation)
- Fixed compilation errors with null-conditional operators

**For detailed documentation, see**: [PATCH_5.0.4_VALIDATION_AND_SECURITY.md](mokipointsCS/Documentation/PATCH_5.0.4_VALIDATION_AND_SECURITY.md)

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [User Roles & Permissions](#user-roles--permissions)
- [Installation & Setup](#installation--setup)
- [Configuration](#configuration)
- [Database Schema](#database-schema)
- [Key Components](#key-components)
- [API Reference](#api-reference)
- [Security](#security)
- [Troubleshooting](#troubleshooting)
- [Documentation](#documentation)
- [Contributing](#contributing)

---

## Overview

**MokiPoints** is a comprehensive family management platform designed to help parents motivate children through a structured task and reward system. The platform enables families to create tasks, assign them to children, track completion, award points, and allow children to redeem points for rewards.

### Core Concept

1. **Parents** create tasks and assign them to children
2. **Children** complete tasks and earn points
3. **Parents** review completed tasks and award points with ratings
4. **Children** use points to purchase rewards from the family shop
5. **Family Chat** enables real-time communication between family members
6. **System Messages** automatically notify about task completions, failures, and new rewards

### Technology Stack

- **Framework**: ASP.NET Web Forms (.NET Framework 4.7.2)
- **Database**: SQL Server LocalDB
- **Frontend**: HTML5, CSS3, JavaScript (vanilla)
- **Image Processing**: System.Drawing
- **External APIs**: Giphy API (for GIF search)
- **Email**: SMTP (for OTP and password reset)

---

## Features

### 🔐 Authentication & User Management

- **User Registration**: Email-based registration with OTP verification
- **Login System**: Secure password-based authentication
- **Password Management**: Forgot password, reset password, change password
- **Session Management**: Secure session handling with role-based access
- **Profile Management**: User profiles with profile pictures, personal information
- **Settings**: User preferences and account settings

### 👨‍👩‍👧‍👦 Family Management

- **Family Creation**: Parents can create families
- **Family Code System**: Unique family codes for children to join
- **Member Management**: View all family members, profile pictures, statistics
- **Child Monitoring**: 
  - View child statistics (points, completed/failed tasks)
  - Ban/unban children from receiving tasks
  - Remove children from family
- **Family Chat**: Real-time messaging system (see [Family Chat Documentation](README_FAMILY_CHAT.md))

### 📝 Task Management System

#### For Parents:
- **Task Creation**: Create tasks with:
  - Title and description
  - Category classification
  - Points reward
  - Multiple objectives/checklist items
- **Task Assignment**: Assign tasks to children with optional deadlines
- **Task Templates**: Create reusable task templates
- **Task Review**: 
  - Review completed tasks
  - Rate tasks (1-5 stars)
  - Award points based on rating
  - Fail tasks (deduct 50% of points)
- **Task History**: View all reviewed tasks with ratings and points

#### For Children:
- **Task Dashboard**: View assigned tasks with status
- **Task Acceptance**: Accept or decline assigned tasks
- **Task Completion**: 
  - Check off objectives
  - Submit tasks for review
- **Task History**: View completed tasks, ratings, and points earned

#### Task Status Flow:
```
Assigned → Ongoing → Completed → Reviewed
    ↓
Declined (no points)
```

### 🎁 Rewards System

#### For Parents:
- **Reward Management**: 
  - Create rewards (name, description, point cost, category, image)
  - Edit existing rewards
  - Delete/archive rewards
  - View all family rewards
- **Order Management**:
  - View pending orders from children
  - Confirm orders (deduct points, generate refund code)
  - Decline orders (no points deducted)
  - Mark orders as fulfilled after giving items to children
  - Track all family orders

#### For Children:
- **Reward Shop**: Browse available rewards, add to cart
- **Shopping Cart**: Add multiple rewards, calculate total
- **Order Placement**: Checkout and create orders
- **Order Tracking**: 
  - View order status (Pending, Waiting to Fulfill, Fulfilled, etc.)
  - Confirm fulfillment after receiving items
  - Claim refund with transaction code if not fulfilled
- **Order History**: View purchase history

#### Reward Order Status Flow:
```
Pending → Waiting to Fulfill → Fulfilled → (Child confirms) → History
   ↓
Declined (no points deducted)

OR

Waiting to Fulfill → Fulfilled → (Child claims not fulfilled) → NotFulfilled (refunded)
```

### 💰 Points System

- **Treasury-Based System**: All points flow through family treasury
- **Points Cap**: Children maximum 10,000 points (excess goes to treasury)
- **Point Transactions**: 
  - Earn points from task completion (based on rating)
  - Lose points from task failure (50% deduction)
  - Spend points on rewards
  - Refund points for unfulfilled orders
- **Transaction History**: Complete history of all point transactions
- **Points Balance**: Real-time balance tracking

### 💬 Family Chat System

A comprehensive real-time messaging system with:

- **Text Messaging**: Send and receive text messages
- **Image Upload**: Upload and share images (max 50MB, auto-compression)
- **GIF Support**: Search and send GIFs using Giphy API
- **Emoji Reactions**: React to messages with 5 emojis (👍, ❤️, 😂, 😢, 😠)
- **System Messages**: Automatic notifications for:
  - Task completions (with rating and points)
  - Task failures (with points lost)
  - New reward items added
- **Real-time Updates**: Polling mechanism (3-second intervals)
- **Message Ordering**: Latest messages displayed first
- **Animation Effects**: Entrance animations for new messages

**See [Family Chat Documentation](README_FAMILY_CHAT.md) for complete details.**

### 📊 Dashboard & Analytics

#### Parent Dashboard:
- Family overview
- Recent activity
- Task statistics
- Reward statistics
- Quick actions

#### Child Dashboard:
- Personal statistics
- Recent tasks
- Points balance
- Available rewards preview
- Quick actions

### 🔔 Notifications

- System notifications for:
  - New task assignments
  - Task review results
  - Order confirmations/declines
  - Order fulfillment
  - Family events

---

## Architecture

### Project Structure

```
mokipointsCS/
├── App_Code/                      # Backend helper classes
│   ├── AuthenticationHelper.cs   # User authentication
│   ├── ChatHelper.cs              # Chat operations
│   ├── ChatImageUploadHandler.cs  # Image upload handler
│   ├── DatabaseHelper.cs          # Database operations
│   ├── DatabaseInitializer.cs     # Database setup & migrations
│   ├── DashboardHelper.cs         # Dashboard data
│   ├── EmailHelper.cs             # Email sending
│   ├── FamilyHelper.cs            # Family operations
│   ├── OTPHelper.cs               # OTP generation/verification
│   ├── PasswordHelper.cs          # Password hashing
│   ├── PointHelper.cs             # Points operations
│   ├── RewardHelper.cs            # Reward operations
│   ├── TaskHelper.cs              # Task operations
│   └── TreasuryHelper.cs          # Treasury operations
│
├── App_Data/                      # Database files
│   └── Mokipoints.mdf             # SQL Server LocalDB database
│
├── Documentation/                 # System documentation
│   ├── RewardsSystem/            # Rewards system docs
│   └── TaskSystem/               # Task system docs
│
├── Images/                        # Image storage
│   ├── FamilyChat/               # Chat images
│   ├── Landing/                   # Landing page images
│   └── ProfilePictures/           # User profile pictures
│
├── *.aspx                         # Web Forms pages
├── *.aspx.cs                      # Code-behind files
├── Web.config                     # Configuration
└── README.md                      # This file
```

### Design Patterns

- **Helper Classes**: Business logic separated into helper classes
- **WebMethods**: AJAX endpoints for client-server communication
- **Session Management**: Server-side session for authentication
- **Database Abstraction**: Centralized database operations via `DatabaseHelper`
- **Error Handling**: Comprehensive logging using `System.Diagnostics.Debug`

---

## User Roles & Permissions

### Parent Role

**Full Access:**
- ✅ Create and manage families
- ✅ Create, edit, delete tasks
- ✅ Assign tasks to children
- ✅ Review and rate completed tasks
- ✅ Fail tasks (deduct points)
- ✅ Create, edit, delete rewards
- ✅ Manage orders (confirm, decline, fulfill)
- ✅ Monitor children (ban/unban, statistics)
- ✅ View all family data
- ✅ Access family chat
- ✅ Change family code

**Restrictions:**
- ❌ Cannot complete tasks
- ❌ Cannot purchase rewards
- ❌ Cannot be assigned tasks

### Child Role

**Access:**
- ✅ Join families via family code
- ✅ View assigned tasks
- ✅ Accept/decline tasks
- ✅ Complete tasks
- ✅ View task history
- ✅ View points balance and history
- ✅ Browse reward shop
- ✅ Purchase rewards
- ✅ Track orders
- ✅ Access family chat
- ✅ View family members

**Restrictions:**
- ❌ Cannot create tasks
- ❌ Cannot assign tasks
- ❌ Cannot review tasks
- ❌ Cannot create rewards
- ❌ Cannot manage orders
- ❌ Cannot ban/unban users
- ❌ Cannot change family code
- ❌ Cannot access parent-only pages

### Banned Children

**Additional Restrictions:**
- ❌ Cannot receive new task assignments
- ✅ Can still complete existing tasks
- ✅ Can still access chat and rewards

---

## Installation & Setup

### Prerequisites

- **Visual Studio 2017+** (or compatible IDE)
- **.NET Framework 4.7.2** or higher
- **SQL Server LocalDB** (included with Visual Studio)
- **IIS Express** (included with Visual Studio) or **IIS**
- **SMTP Server** (for email functionality - optional)

### Setup Steps

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd mokipointsCS
   ```

2. **Open in Visual Studio**
   - Open `mokipointsCS.sln` in Visual Studio
   - Restore NuGet packages if prompted

3. **Database Setup**
   - The database is automatically initialized on first run via `DatabaseInitializer.cs`
   - Database file: `App_Data/Mokipoints.mdf`
   - If manual setup needed, run `App_Data/CreateDatabase.sql` in SQL Server Management Studio

4. **Configuration**
   - Update `Web.config` with your settings (see [Configuration](#configuration))
   - Set Giphy API key for GIF functionality
   - Configure SMTP settings for email (optional)

5. **Build & Run**
   - Build the solution (F6)
   - Run the application (F5)
   - Default URL: `http://localhost:port/Default.aspx`

6. **First User**
   - Register a new account (will be created as PARENT role)
   - Create a family
   - Add children or have them join via family code

---

## Configuration

### Web.config Settings

#### Connection String
```xml
<connectionStrings>
  <add name="MokipointsConnectionString" 
       connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\Mokipoints.mdf;Integrated Security=True;Connect Timeout=30" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

#### App Settings
```xml
<appSettings>
  <!-- Giphy API Key for GIF search -->
  <add key="GIPHY_API_KEY" value="YOUR_API_KEY_HERE" />
  
  <!-- Email Settings (optional) -->
  <add key="SMTP_Server" value="smtp.gmail.com" />
  <add key="SMTP_Port" value="587" />
  <add key="SMTP_Username" value="your-email@gmail.com" />
  <add key="SMTP_Password" value="your-password" />
  <add key="SMTP_FromEmail" value="your-email@gmail.com" />
</appSettings>
```

### Getting a Giphy API Key

1. Visit [Giphy Developers](https://developers.giphy.com/)
2. Create an account
3. Create a new app
4. Copy your API key
5. Add it to `Web.config`

**Note**: If the API key is missing, GIF search will show a friendly error message.

---

## Database Schema

### Core Tables

#### Users
- `Id` (PK, int)
- `FirstName`, `LastName`, `MiddleName` (nvarchar)
- `Email` (nvarchar, unique)
- `Password` (nvarchar, hashed)
- `Birthday` (datetime, nullable)
- `Role` (nvarchar) - 'PARENT' or 'CHILD'
- `IsActive` (bit)
- `IsBanned` (bit) - For child monitoring
- `CreatedDate` (datetime)

#### Families
- `Id` (PK, int)
- `FamilyCode` (nvarchar, unique)
- `FamilyName` (nvarchar)
- `CreatedBy` (FK to Users)
- `CreatedDate` (datetime)

#### FamilyMembers
- `Id` (PK, int)
- `FamilyId` (FK to Families)
- `UserId` (FK to Users)
- `JoinedDate` (datetime)
- `IsActive` (bit)

### Task System Tables

#### Tasks
- `Id` (PK, int)
- `Title`, `Description` (nvarchar)
- `Category` (nvarchar)
- `PointsReward` (int)
- `CreatedBy` (FK to Users)
- `FamilyId` (FK to Families)
- `CreatedDate` (datetime)
- `IsDeleted` (bit)

#### TaskAssignments
- `Id` (PK, int)
- `TaskId` (FK to Tasks)
- `UserId` (FK to Users)
- `Status` (nvarchar) - 'Assigned', 'Ongoing', 'Completed', 'Reviewed', 'Declined'
- `Deadline` (datetime, nullable)
- `AssignedDate` (datetime)
- `CompletedDate` (datetime, nullable)
- `ReviewedDate` (datetime, nullable)
- `Rating` (int, nullable) - 1-5 stars
- `PointsAwarded` (int, nullable)

#### TaskObjectives
- `Id` (PK, int)
- `TaskId` (FK to Tasks)
- `ObjectiveText` (nvarchar)
- `OrderIndex` (int)

### Rewards System Tables

#### Rewards
- `Id` (PK, int)
- `FamilyId` (FK to Families)
- `Name`, `Description` (nvarchar)
- `PointCost` (int)
- `Category` (nvarchar)
- `ImagePath` (nvarchar, nullable)
- `CreatedDate` (datetime)
- `IsDeleted` (bit)

#### RewardOrders
- `Id` (PK, int)
- `FamilyId` (FK to Families)
- `UserId` (FK to Users)
- `Status` (nvarchar) - 'Pending', 'WaitingToFulfill', 'Fulfilled', 'Declined', 'NotFulfilled'
- `TotalPoints` (int)
- `RefundCode` (nvarchar, nullable)
- `CreatedDate` (datetime)
- `ConfirmedDate` (datetime, nullable)
- `FulfilledDate` (datetime, nullable)

#### RewardOrderItems
- `Id` (PK, int)
- `OrderId` (FK to RewardOrders)
- `RewardId` (FK to Rewards)
- `Quantity` (int)
- `PointCost` (int)

#### FamilyTreasury
- `Id` (PK, int)
- `FamilyId` (FK to Families, unique)
- `Balance` (int) - Default 0

#### TreasuryTransactions
- `Id` (PK, int)
- `FamilyId` (FK to Families)
- `Amount` (int) - Can be positive or negative
- `TransactionType` (nvarchar)
- `Description` (nvarchar)
- `TransactionDate` (datetime)

#### RewardPurchaseHistory
- `Id` (PK, int)
- `OrderId` (FK to RewardOrders)
- `UserId` (FK to Users)
- `ConfirmedDate` (datetime)

### Points System Tables

#### PointTransactions
- `Id` (PK, int)
- `UserId` (FK to Users)
- `Points` (int) - Can be positive or negative
- `TransactionType` (nvarchar) - 'TaskCompleted', 'TaskFailed', 'RewardPurchase', 'Refund', etc.
- `Description` (nvarchar)
- `TransactionDate` (datetime)
- `TreasuryTransactionId` (FK to TreasuryTransactions, nullable)

### Chat System Tables

#### FamilyMessages
- `Id` (PK, int)
- `FamilyId` (FK to Families)
- `UserId` (FK to Users, nullable) - NULL for system messages
- `MessageType` (nvarchar) - 'Text', 'Image', 'GIF', 'System'
- `MessageText` (nvarchar(max), nullable)
- `ImagePath` (nvarchar, nullable)
- `GIFUrl` (nvarchar, nullable)
- `ReplyToMessageId` (FK to FamilyMessages, nullable)
- `SystemEventType` (nvarchar, nullable) - 'TaskCompleted', 'TaskFailed', 'RewardAdded'
- `SystemEventData` (nvarchar(max), nullable) - JSON data
- `CreatedDate` (datetime)
- `IsDeleted` (bit)

#### FamilyMessageReactions
- `Id` (PK, int)
- `MessageId` (FK to FamilyMessages)
- `UserId` (FK to Users)
- `ReactionType` (nvarchar) - 'Like', 'Love', 'Haha', 'Sad', 'Angry'
- `CreatedDate` (datetime)
- UNIQUE constraint on (`MessageId`, `UserId`, `ReactionType`)

---

## Key Components

### Helper Classes

#### AuthenticationHelper.cs
- User registration and login
- Session management
- Password verification
- User data retrieval

#### TaskHelper.cs
- Task CRUD operations
- Task assignment
- Task completion handling
- Task review and rating
- Task failure handling

#### RewardHelper.cs
- Reward CRUD operations
- Order creation and management
- Order confirmation/decline
- Order fulfillment
- System message posting for new rewards

#### PointHelper.cs
- Point transactions
- Balance calculations
- Points cap enforcement (10,000 max for children)
- Treasury integration

#### TreasuryHelper.cs
- Treasury balance management
- Treasury transactions
- Excess points handling

#### ChatHelper.cs
- Message sending and retrieval
- Reaction management
- System message posting
- Image path handling

#### FamilyHelper.cs
- Family creation and management
- Family member operations
- Family code generation
- Child monitoring (ban/unban)

#### DatabaseHelper.cs
- Centralized database operations
- Parameterized queries (SQL injection prevention)
- Connection management
- Error handling

#### DatabaseInitializer.cs
- Database creation
- Table initialization
- Schema migrations
- Automatic setup on first run

### Key Pages

#### Authentication Pages
- `Default.aspx` - Landing page
- `Login.aspx` - User login
- `Register.aspx` - User registration
- `OTP.aspx` - OTP verification
- `ForgotPassword.aspx` - Password reset request
- `ChangePassword.aspx` - Password change

#### Parent Pages
- `ParentDashboard.aspx` - Parent dashboard
- `Tasks.aspx` - Task management
- `AssignTask.aspx` - Task assignment
- `TaskReview.aspx` - Task review and rating
- `Rewards.aspx` - Reward management
- `RewardOrders.aspx` - Order management
- `Family.aspx` - Family management and chat
- `Profile.aspx` - User profile
- `Settings.aspx` - User settings

#### Child Pages
- `ChildDashboard.aspx` - Child dashboard
- `ChildTasks.aspx` - Assigned tasks
- `TaskHistory.aspx` - Completed tasks history
- `RewardShop.aspx` - Browse and purchase rewards
- `Cart.aspx` - Shopping cart
- `MyOrders.aspx` - Order tracking
- `OrderHistory.aspx` - Purchase history
- `PointsHistory.aspx` - Points transaction history
- `Family.aspx` - Family chat
- `Profile.aspx` - User profile
- `Settings.aspx` - User settings

#### Shared Pages
- `Dashboard.aspx` - Role-based dashboard redirect
- `Notifications.aspx` - System notifications
- `PrivacyPolicy.aspx` - Privacy policy
- `TermsAndConditions.aspx` - Terms and conditions

---

## API Reference

### WebMethods (AJAX Endpoints)

All WebMethods are defined in `.aspx.cs` files and called via JavaScript `XMLHttpRequest`.

#### Chat WebMethods (`Family.aspx.cs`)

- `GetChatMessages()` - Load initial messages
- `GetNewChatMessages(int lastMessageId)` - Poll for new messages
- `SendChatMessage(string message)` - Send text message
- `SendChatGIF(string gifUrl)` - Send GIF
- `ToggleReaction(int messageId, string reactionType)` - Add/remove reaction
- `GetMessageReactions(int messageId)` - Get reactions for message

#### Task WebMethods (`Tasks.aspx.cs`, `TaskReview.aspx.cs`)

- `GetTasks()` - Get all tasks for family
- `CreateTask(...)` - Create new task
- `UpdateTask(...)` - Update existing task
- `DeleteTask(int taskId)` - Delete task
- `GetTaskAssignments(int taskId)` - Get assignments for task
- `ReviewTask(int assignmentId, int rating, int pointsAwarded)` - Review completed task
- `FailTask(int assignmentId)` - Fail task and deduct points

#### Reward WebMethods (`Rewards.aspx.cs`, `RewardOrders.aspx.cs`)

- `GetRewards()` - Get all rewards for family
- `CreateReward(...)` - Create new reward
- `UpdateReward(...)` - Update existing reward
- `DeleteReward(int rewardId)` - Delete reward
- `GetOrders()` - Get all orders for family
- `ConfirmOrder(int orderId)` - Confirm order
- `DeclineOrder(int orderId)` - Decline order
- `FulfillOrder(int orderId)` - Mark order as fulfilled

---

## Security

### Authentication & Authorization

- **Password Hashing**: Passwords are hashed using secure hashing algorithms
- **Session Management**: Server-side session validation on all protected pages
- **Role-Based Access**: Pages check user role before allowing access
- **SQL Injection Prevention**: All queries use parameterized statements
- **XSS Prevention**: User input is sanitized before display

### File Upload Security

- **File Type Validation**: Only image files accepted (MIME type and extension check)
- **File Size Limits**: Maximum 50MB per file
- **Filename Sanitization**: Filenames sanitized to prevent path traversal
- **Storage Location**: Files stored in dedicated directories

### Data Protection

- **Family Isolation**: Users can only access their own family's data
- **Permission Checks**: All operations verify user permissions
- **Transaction Integrity**: Database transactions ensure data consistency
- **Error Handling**: Errors logged without exposing sensitive information

### Best Practices

- Never expose database connection strings in client-side code
- Always validate user input on server-side
- Use HTTPS in production
- Regularly update dependencies
- Monitor error logs for suspicious activity

---

## Troubleshooting

### Common Issues

#### Database Connection Errors

**Problem**: "Cannot open database" or connection timeout  
**Solutions**:
- Verify SQL Server LocalDB is installed and running
- Check connection string in `Web.config`
- Ensure `App_Data` folder has write permissions
- Verify database file exists: `App_Data/Mokipoints.mdf`

#### Images Not Loading

**Problem**: Profile pictures or chat images show 404 errors  
**Solutions**:
- Verify image paths are absolute (starting with `/`)
- Check that image directories exist and have correct permissions
- Ensure `Images/ProfilePictures/` and `Images/FamilyChat/` directories exist

#### GIF Search Not Working

**Problem**: GIF search returns 403 or error messages  
**Solutions**:
- Verify `GIPHY_API_KEY` is set in `Web.config`
- Check API key is valid and not expired
- Ensure API key has correct permissions

#### Session Expiration

**Problem**: Users logged out unexpectedly  
**Solutions**:
- Check session timeout settings in `Web.config`
- Verify session state configuration
- Check for session clearing code

#### Points Not Updating

**Problem**: Points balance not reflecting transactions  
**Solutions**:
- Check `PointTransactions` table for transaction records
- Verify `FamilyTreasury` balance
- Check for points cap enforcement (10,000 max for children)
- Review transaction logs in `PointsHistory.aspx`

### Debugging

Enable detailed error logging:

1. Check `System.Diagnostics.Debug` output in Visual Studio Output window
2. Review browser console for JavaScript errors
3. Check server logs for exceptions
4. Use SQL Server Management Studio to inspect database directly

### Getting Help

- Check existing documentation in `Documentation/` folder
- Review `PROGRESS.md` for known issues and fixes
- Check system-specific READMEs:
  - [Family Chat System](README_FAMILY_CHAT.md)
  - [Rewards System Documentation](Documentation/RewardsSystem/README.md)

---

## Documentation

### System Documentation

- **README.md** (this file) - System overview
- **README_FAMILY_CHAT.md** - Family chat system documentation
- **PROGRESS.md** - Development progress and bug fixes

### Feature Documentation

Located in `Documentation/` folder:

- **RewardsSystem/** - Complete rewards system documentation
  - `README.md` - Overview
  - `REWARDS_SYSTEM_SCHEMATIC.md` - Architecture and flows
  - `IMPLEMENTATION.md` - Implementation details
  - `TESTING.md` - Testing guide

- **TaskSystem/** - Task system documentation
  - Task assignment flows
  - Task review processes
  - Status management

### Code Documentation

- Inline XML comments in helper classes
- Code-behind files include method documentation
- Database schema documented in `DatabaseInitializer.cs`

---

## Contributing

### Development Guidelines

1. **Code Style**:
   - Follow existing code patterns
   - Use meaningful variable and method names
   - Add XML comments to public methods
   - Keep methods focused and single-purpose

2. **Database Changes**:
   - Always update `DatabaseInitializer.cs` with migrations
   - Test migrations on existing databases
   - Document schema changes

3. **Security**:
   - Always validate user input
   - Use parameterized queries
   - Check user permissions before operations
   - Never expose sensitive data in client-side code

4. **Testing**:
   - Test with both parent and child accounts
   - Test edge cases and error scenarios
   - Verify database integrity after changes
   - Check for SQL injection vulnerabilities

5. **Documentation**:
   - Update relevant README files
   - Document new features
   - Add troubleshooting notes for common issues

### Pull Request Process

1. Create a feature branch
2. Make changes following guidelines
3. Test thoroughly
4. Update documentation
5. Submit pull request with description

### Reporting Issues

When reporting bugs, include:
- Steps to reproduce
- Expected behavior
- Actual behavior
- Error messages/logs
- Browser and version
- .NET Framework version

---

## License

This project is proprietary software. All rights reserved.

---

## Version History

### Version 1.0 (December 2024)
- ✅ Complete authentication system
- ✅ Family management
- ✅ Task management system
- ✅ Rewards system
- ✅ Points system with treasury
- ✅ Family chat system
- ✅ Real-time messaging
- ✅ Image upload and GIF support
- ✅ System notifications

---

## Support & Contact

For issues, questions, or contributions, please refer to the project repository or contact the development team.

---

**Last Updated**: December 2024  
**Maintained By**: MokiPoints Development Team

