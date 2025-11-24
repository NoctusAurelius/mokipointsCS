using System;
using System.Data.SqlClient;
using System.IO;
using System.Web;

namespace mokipointsCS
{
    /// <summary>
    /// Handles database initialization and setup
    /// </summary>
    public class DatabaseInitializer
    {
        /// <summary>
        /// Initializes the database by creating tables if they don't exist
        /// </summary>
        public static void InitializeDatabase()
        {
            try
            {
                // Get database path - handle both HttpContext available and not available cases
                string dbPath;
                string dbDir;
                
                if (HttpContext.Current != null)
                {
                    // During normal requests, use HttpContext
                    dbPath = HttpContext.Current.Server.MapPath("~/App_Data/Mokipoints.mdf");
                }
                else
                {
                    // During Application_Start, HttpContext may be null, use AppDomain path
                    string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
                    // Remove trailing "bin\" if present (for web apps)
                    if (appBasePath.EndsWith("bin\\") || appBasePath.EndsWith("bin/"))
                    {
                        appBasePath = Path.GetDirectoryName(appBasePath);
                    }
                    dbPath = Path.Combine(appBasePath, "App_Data", "Mokipoints.mdf");
                }
                
                dbDir = Path.GetDirectoryName(dbPath);
                string logPath = dbPath.Replace(".mdf", "_log.ldf");

                System.Diagnostics.Debug.WriteLine("Database initialization started. Database path: " + dbPath);

                // Ensure App_Data directory exists
                if (!Directory.Exists(dbDir))
                {
                    Directory.CreateDirectory(dbDir);
                    System.Diagnostics.Debug.WriteLine("Created App_Data directory: " + dbDir);
                }

                // If database file doesn't exist, create it first
                if (!File.Exists(dbPath))
                {
                    System.Diagnostics.Debug.WriteLine("Database file not found. Creating database...");
                    CreateDatabaseFile(dbPath);
                    
                    // Wait a moment for database to be fully created before creating tables
                    System.Threading.Thread.Sleep(500);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Database file exists: " + dbPath);
                }

                // Create tables if they don't exist
                System.Diagnostics.Debug.WriteLine("Creating tables if they don't exist...");
                CreateTablesIfNotExist();
                System.Diagnostics.Debug.WriteLine("Database initialization completed successfully.");
            }
            catch (Exception ex)
            {
                string errorMsg = "Database initialization error: " + ex.Message;
                System.Diagnostics.Debug.WriteLine(errorMsg);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                    System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.InnerException.StackTrace);
                }
                System.Diagnostics.Debug.WriteLine("Full stack trace: " + ex.StackTrace);
                // Don't throw - let the app continue, user can manually run SQL script
            }
        }

        private static void CreateDatabaseFile(string dbPath)
        {
            try
            {
                // Create database using master connection
                string masterConnectionString = "Data Source=(LocalDB)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30";
                string dbName = "Mokipoints";
                string logPath = dbPath.Replace(".mdf", "_log.ldf");

                System.Diagnostics.Debug.WriteLine("Attempting to connect to LocalDB master...");
                
                using (SqlConnection conn = new SqlConnection(masterConnectionString))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("Connected to LocalDB master successfully.");
                    
                    // Check if database already exists in LocalDB
                    string checkDb = string.Format("SELECT COUNT(*) FROM sys.databases WHERE name = '{0}'", dbName);
                    object result = new SqlCommand(checkDb, conn).ExecuteScalar();
                    int dbExists = Convert.ToInt32(result);
                    
                    System.Diagnostics.Debug.WriteLine("Database exists check result: " + dbExists);
                    
                    if (dbExists > 0)
                    {
                        // Database exists in LocalDB - check if physical file exists
                        if (!File.Exists(dbPath))
                        {
                            // Database is registered in LocalDB but file is missing - DROP it
                            System.Diagnostics.Debug.WriteLine("Database exists in LocalDB but physical file is missing. Dropping database...");
                            
                            // Set database to single user mode to close connections
                            string setSingleUser = string.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", dbName);
                            try
                            {
                                using (SqlCommand cmd = new SqlCommand(setSingleUser, conn))
                                {
                                    cmd.ExecuteNonQuery();
                                    System.Diagnostics.Debug.WriteLine("Database set to single user mode.");
                                }
                            }
                            catch (SqlException ex)
                            {
                                System.Diagnostics.Debug.WriteLine("Warning: Could not set single user mode (may already be offline): " + ex.Message);
                            }
                            
                            // Drop the database
                            string dropDb = string.Format("DROP DATABASE [{0}]", dbName);
                            using (SqlCommand cmd = new SqlCommand(dropDb, conn))
                            {
                                cmd.ExecuteNonQuery();
                                System.Diagnostics.Debug.WriteLine("Database dropped successfully from LocalDB.");
                            }
                            
                            // Wait a moment for cleanup
                            System.Threading.Thread.Sleep(500);
                            dbExists = 0; // Reset to indicate we need to create it
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Database already exists in LocalDB and file exists: " + dbName);
                            return; // Database and file both exist, nothing to do
                        }
                    }
                    
                    if (dbExists == 0)
                    {
                        // Database doesn't exist or was just dropped - create it
                        // Ensure the directory exists
                        string dbDir = Path.GetDirectoryName(dbPath);
                        if (!Directory.Exists(dbDir))
                        {
                            Directory.CreateDirectory(dbDir);
                            System.Diagnostics.Debug.WriteLine("Created directory: " + dbDir);
                        }
                        
                        // Create database
                        string createDb = string.Format(
                            "CREATE DATABASE [{0}] ON (NAME = '{0}', FILENAME = '{1}') LOG ON (NAME = '{0}_Log', FILENAME = '{2}')",
                            dbName, dbPath, logPath);
                        
                        System.Diagnostics.Debug.WriteLine("Creating database: " + dbName);
                        System.Diagnostics.Debug.WriteLine("Database file: " + dbPath);
                        System.Diagnostics.Debug.WriteLine("Log file: " + logPath);
                        
                        using (SqlCommand cmd = new SqlCommand(createDb, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        
                        System.Diagnostics.Debug.WriteLine("Database created successfully: " + dbPath);
                        
                        // Wait a moment for database to be fully ready
                        System.Threading.Thread.Sleep(500);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine("SQL Error in CreateDatabaseFile: " + sqlEx.Message);
                System.Diagnostics.Debug.WriteLine("SQL Error Number: " + sqlEx.Number);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + sqlEx.StackTrace);
                throw; // Re-throw to be caught by outer handler
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error in CreateDatabaseFile: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                throw; // Re-throw to be caught by outer handler
            }
        }

        private static void CreateTablesIfNotExist()
        {
            try
            {
                // Get connection string and expand DataDirectory
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["MokipointsDB"].ConnectionString;
                
                if (string.IsNullOrEmpty(connectionString))
                {
                    System.Diagnostics.Debug.WriteLine("ERROR: Connection string 'MokipointsDB' not found in Web.config!");
                    return;
                }
                
                // Expand |DataDirectory| token
                string appDataPath;
                if (HttpContext.Current != null)
                {
                    appDataPath = HttpContext.Current.Server.MapPath("~/App_Data");
                }
                else
                {
                    // During Application_Start, HttpContext may be null
                    string appBasePath = AppDomain.CurrentDomain.BaseDirectory;
                    // Remove trailing "bin\" if present (for web apps)
                    if (appBasePath.EndsWith("bin\\") || appBasePath.EndsWith("bin/"))
                    {
                        appBasePath = Path.GetDirectoryName(appBasePath);
                    }
                    appDataPath = Path.Combine(appBasePath, "App_Data");
                }
                
                connectionString = connectionString.Replace("|DataDirectory|", appDataPath);
                System.Diagnostics.Debug.WriteLine("Connection string resolved. App_Data path: " + appDataPath);

                System.Diagnostics.Debug.WriteLine("Attempting to connect to database to create tables...");
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        System.Diagnostics.Debug.WriteLine("Successfully connected to database. Starting table creation...");
                    }
                    catch (SqlException sqlEx)
                    {
                        System.Diagnostics.Debug.WriteLine("ERROR: Failed to connect to database!");
                        System.Diagnostics.Debug.WriteLine("SQL Error: " + sqlEx.Message);
                        System.Diagnostics.Debug.WriteLine("SQL Error Number: " + sqlEx.Number);
                        System.Diagnostics.Debug.WriteLine("Connection string: " + connectionString.Replace("Password=.*;", "Password=***;"));
                        throw; // Re-throw to be caught by outer handler
                    }

                // Create Users table
                string createUsersTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[Users] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [FirstName] NVARCHAR(50) NOT NULL,
                            [LastName] NVARCHAR(50) NOT NULL,
                            [MiddleName] NVARCHAR(50) NULL,
                            [Email] NVARCHAR(255) NOT NULL UNIQUE,
                            [Password] NVARCHAR(255) NOT NULL,
                            [Birthday] DATETIME NULL,
                            [Role] NVARCHAR(20) NOT NULL,
                            [ProfilePicture] NVARCHAR(255) NULL,
                            [IsBanned] BIT NOT NULL DEFAULT 0,
                            [Points] INT NOT NULL DEFAULT 0,
                            [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            [IsActive] BIT NOT NULL DEFAULT 1
                        )
                        CREATE UNIQUE INDEX IX_Users_Email ON [dbo].[Users]([Email])
                    END
                    ELSE
                    BEGIN
                        -- Add new columns if they don't exist (for existing databases)
                        -- Only run migration if FirstName column doesn't exist
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'FirstName')
                        BEGIN
                            ALTER TABLE [dbo].[Users] ADD [FirstName] NVARCHAR(50) NULL;
                            ALTER TABLE [dbo].[Users] ADD [LastName] NVARCHAR(50) NULL;
                            ALTER TABLE [dbo].[Users] ADD [MiddleName] NVARCHAR(50) NULL;
                            ALTER TABLE [dbo].[Users] ADD [Birthday] DATETIME NULL;
                            ALTER TABLE [dbo].[Users] ADD [Role] NVARCHAR(20) NULL;
                            
                            -- Set default values for new columns
                            UPDATE [dbo].[Users] SET [FirstName] = 'User', [LastName] = '', [Role] = 'PARENT' WHERE [FirstName] IS NULL;
                            
                            -- Only migrate from Name column if it exists (use dynamic SQL to avoid compilation error)
                            DECLARE @sql NVARCHAR(MAX);
                            IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Name')
                            BEGIN
                                SET @sql = N'UPDATE [dbo].[Users] SET [FirstName] = [Name], [LastName] = '''' WHERE [FirstName] = ''User''';
                                EXEC sp_executesql @sql;
                            END
                            
                            ALTER TABLE [dbo].[Users] ALTER COLUMN [FirstName] NVARCHAR(50) NOT NULL;
                            ALTER TABLE [dbo].[Users] ALTER COLUMN [LastName] NVARCHAR(50) NOT NULL;
                            ALTER TABLE [dbo].[Users] ALTER COLUMN [Role] NVARCHAR(20) NOT NULL;
                        END
                        
                        -- Add ProfilePicture column if it doesn't exist
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'ProfilePicture')
                        BEGIN
                            ALTER TABLE [dbo].[Users] ADD [ProfilePicture] NVARCHAR(255) NULL;
                        END
                        
                        -- Add IsBanned column if it doesn't exist
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'IsBanned')
                        BEGIN
                            ALTER TABLE [dbo].[Users] ADD [IsBanned] BIT NOT NULL DEFAULT 0;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Password')
                        BEGIN
                            ALTER TABLE [dbo].[Users] ADD [Password] NVARCHAR(255) NULL;
                            UPDATE [dbo].[Users] SET [Password] = 'changeme' WHERE [Password] IS NULL;
                            ALTER TABLE [dbo].[Users] ALTER COLUMN [Password] NVARCHAR(255) NOT NULL;
                        END
                        -- Add Points column if it doesn't exist
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Points')
                        BEGIN
                            ALTER TABLE [dbo].[Users] ADD [Points] INT NOT NULL DEFAULT 0;
                        END
                        -- Make Email unique if not already
                        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID(N'[dbo].[Users]'))
                        BEGIN
                            CREATE UNIQUE INDEX IX_Users_Email ON [dbo].[Users]([Email]);
                        END
                    END";

                // Create Chores table
                string createChoresTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Chores]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[Chores] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [Name] NVARCHAR(200) NOT NULL,
                            [Description] NVARCHAR(MAX) NULL,
                            [Points] INT NOT NULL DEFAULT 0,
                            [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            [IsActive] BIT NOT NULL DEFAULT 1
                        )
                    END";

                // Create ChoreAssignments table
                string createChoreAssignmentsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChoreAssignments]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[ChoreAssignments] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [UserId] INT NOT NULL,
                            [ChoreId] INT NOT NULL,
                            [AssignedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            [CompletedDate] DATETIME NULL,
                            [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
                            FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
                            FOREIGN KEY ([ChoreId]) REFERENCES [dbo].[Chores]([Id])
                        )
                    END";

                // Create PointTransactions table
                string createPointTransactionsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PointTransactions]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[PointTransactions] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [UserId] INT NOT NULL,
                            [Points] INT NOT NULL,
                            [TransactionType] NVARCHAR(50) NOT NULL,
                            [Description] NVARCHAR(500) NULL,
                            [TransactionDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            [ChoreAssignmentId] INT NULL,
                            [TaskAssignmentId] INT NULL,
                            FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
                            FOREIGN KEY ([ChoreAssignmentId]) REFERENCES [dbo].[ChoreAssignments]([Id]),
                            FOREIGN KEY ([TaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id])
                        )
                    END
                    ELSE
                    BEGIN
                        -- Add TaskAssignmentId column if it doesn't exist (only if TaskAssignments table exists)
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PointTransactions]') AND name = 'TaskAssignmentId')
                        BEGIN
                            -- Check if TaskAssignments table exists before adding foreign key
                            IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskAssignments]') AND type in (N'U'))
                            BEGIN
                                ALTER TABLE [dbo].[PointTransactions] ADD [TaskAssignmentId] INT NULL;
                                ALTER TABLE [dbo].[PointTransactions] ADD FOREIGN KEY ([TaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id]);
                            END
                        END
                    END";

                // Create OTPCodes table
                string createOTPCodesTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[OTPCodes]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[OTPCodes] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [Email] NVARCHAR(255) NOT NULL,
                            [Code] NVARCHAR(10) NOT NULL,
                            [Purpose] NVARCHAR(50) NOT NULL,
                            [ExpiryDate] DATETIME NOT NULL,
                            [IsUsed] BIT NOT NULL DEFAULT 0,
                            [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE()
                        )
                        CREATE INDEX IX_OTPCodes_Email ON [dbo].[OTPCodes]([Email])
                        CREATE INDEX IX_OTPCodes_Code ON [dbo].[OTPCodes]([Code])
                    END";

                // Create Families table
                string createFamiliesTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Families]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[Families] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [Name] NVARCHAR(200) NOT NULL,
                            [PinCode] NVARCHAR(6) NOT NULL,
                            [FamilyCode] NVARCHAR(6) NOT NULL UNIQUE,
                            [OwnerId] INT NOT NULL,
                            [TreasuryPoints] INT NOT NULL DEFAULT 1000000,
                            [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            FOREIGN KEY ([OwnerId]) REFERENCES [dbo].[Users]([Id])
                        )
                        CREATE UNIQUE INDEX IX_Families_FamilyCode ON [dbo].[Families]([FamilyCode])
                        CREATE INDEX IX_Families_OwnerId ON [dbo].[Families]([OwnerId])
                    END";

                // Create FamilyMembers table
                string createFamilyMembersTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FamilyMembers]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[FamilyMembers] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [FamilyId] INT NOT NULL,
                            [UserId] INT NOT NULL,
                            [Role] NVARCHAR(20) NOT NULL,
                            [JoinedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            [IsActive] BIT NOT NULL DEFAULT 1,
                            FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id]),
                            FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
                            UNIQUE ([FamilyId], [UserId])
                        )
                        CREATE INDEX IX_FamilyMembers_FamilyId ON [dbo].[FamilyMembers]([FamilyId])
                        CREATE INDEX IX_FamilyMembers_UserId ON [dbo].[FamilyMembers]([UserId])
                    END";

                // Create Tasks table (Enhanced with new fields)
                string createTasksTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[Tasks] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [Title] NVARCHAR(200) NOT NULL,
                            [Description] NVARCHAR(MAX) NULL,
                            [Category] NVARCHAR(100) NOT NULL,
                            [PointsReward] INT NOT NULL DEFAULT 0,
                            [CreatedBy] INT NOT NULL,
                            [FamilyId] INT NOT NULL,
                            [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            [IsActive] BIT NOT NULL DEFAULT 1,
                            [Priority] NVARCHAR(20) NULL DEFAULT 'Medium',
                            [Difficulty] NVARCHAR(20) NULL,
                            [EstimatedMinutes] INT NULL,
                            [Instructions] NVARCHAR(MAX) NULL,
                            [IsTemplate] BIT NOT NULL DEFAULT 0,
                            [TemplateName] NVARCHAR(200) NULL,
                            [RecurrencePattern] NVARCHAR(50) NULL,
                            FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([Id]),
                            FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id])
                        )
                        CREATE INDEX IX_Tasks_CreatedBy ON [dbo].[Tasks]([CreatedBy])
                        CREATE INDEX IX_Tasks_FamilyId ON [dbo].[Tasks]([FamilyId])
                    END
                    ELSE
                    BEGIN
                        -- Add new columns if they don't exist
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND name = 'Priority')
                        BEGIN
                            ALTER TABLE [dbo].[Tasks] ADD [Priority] NVARCHAR(20) NULL DEFAULT 'Medium';
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND name = 'Difficulty')
                        BEGIN
                            ALTER TABLE [dbo].[Tasks] ADD [Difficulty] NVARCHAR(20) NULL;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND name = 'EstimatedMinutes')
                        BEGIN
                            ALTER TABLE [dbo].[Tasks] ADD [EstimatedMinutes] INT NULL;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND name = 'Instructions')
                        BEGIN
                            ALTER TABLE [dbo].[Tasks] ADD [Instructions] NVARCHAR(MAX) NULL;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND name = 'IsTemplate')
                        BEGIN
                            ALTER TABLE [dbo].[Tasks] ADD [IsTemplate] BIT NOT NULL DEFAULT 0;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND name = 'TemplateName')
                        BEGIN
                            ALTER TABLE [dbo].[Tasks] ADD [TemplateName] NVARCHAR(200) NULL;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Tasks]') AND name = 'RecurrencePattern')
                        BEGIN
                            ALTER TABLE [dbo].[Tasks] ADD [RecurrencePattern] NVARCHAR(50) NULL;
                        END
                    END";

                // Create TaskObjectives table
                string createTaskObjectivesTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskObjectives]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[TaskObjectives] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [TaskId] INT NOT NULL,
                            [ObjectiveText] NVARCHAR(500) NOT NULL,
                            [OrderIndex] INT NOT NULL DEFAULT 0,
                            FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks]([Id]) ON DELETE CASCADE
                        )
                        CREATE INDEX IX_TaskObjectives_TaskId ON [dbo].[TaskObjectives]([TaskId])
                    END";

                // Create TaskAssignments table (Enhanced with soft-delete)
                string createTaskAssignmentsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskAssignments]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[TaskAssignments] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [TaskId] INT NOT NULL,
                            [UserId] INT NOT NULL,
                            [AssignedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            [Deadline] DATETIME NULL,
                            [Status] NVARCHAR(50) NOT NULL DEFAULT 'Assigned',
                            [AcceptedDate] DATETIME NULL,
                            [CompletedDate] DATETIME NULL,
                            [IsDeleted] BIT NOT NULL DEFAULT 0,
                            [DeletedDate] DATETIME NULL,
                            [DeletedBy] INT NULL,
                            FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks]([Id]),
                            FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
                            FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[Users]([Id])
                        )
                        CREATE INDEX IX_TaskAssignments_TaskId ON [dbo].[TaskAssignments]([TaskId])
                        CREATE INDEX IX_TaskAssignments_UserId ON [dbo].[TaskAssignments]([UserId])
                        CREATE INDEX IX_TaskAssignments_Status ON [dbo].[TaskAssignments]([Status])
                        CREATE INDEX IX_TaskAssignments_IsDeleted ON [dbo].[TaskAssignments]([IsDeleted])
                    END
                    ELSE
                    BEGIN
                        -- Add soft-delete columns if they don't exist
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TaskAssignments]') AND name = 'IsDeleted')
                        BEGIN
                            ALTER TABLE [dbo].[TaskAssignments] ADD [IsDeleted] BIT NOT NULL DEFAULT 0;
                            CREATE INDEX IX_TaskAssignments_IsDeleted ON [dbo].[TaskAssignments]([IsDeleted]);
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TaskAssignments]') AND name = 'DeletedDate')
                        BEGIN
                            ALTER TABLE [dbo].[TaskAssignments] ADD [DeletedDate] DATETIME NULL;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TaskAssignments]') AND name = 'DeletedBy')
                        BEGIN
                            ALTER TABLE [dbo].[TaskAssignments] ADD [DeletedBy] INT NULL;
                            ALTER TABLE [dbo].[TaskAssignments] ADD FOREIGN KEY ([DeletedBy]) REFERENCES [dbo].[Users]([Id]);
                        END
                    END";

                // Create TaskReviews table (Enhanced with IsAutoFailed)
                string createTaskReviewsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskReviews]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[TaskReviews] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [TaskAssignmentId] INT NOT NULL,
                            [Rating] INT NULL,
                            [PointsAwarded] INT NOT NULL,
                            [IsFailed] BIT NOT NULL DEFAULT 0,
                            [IsAutoFailed] BIT NOT NULL DEFAULT 0,
                            [ReviewDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            [ReviewedBy] INT NOT NULL,
                            FOREIGN KEY ([TaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id]),
                            FOREIGN KEY ([ReviewedBy]) REFERENCES [dbo].[Users]([Id])
                        )
                        CREATE INDEX IX_TaskReviews_TaskAssignmentId ON [dbo].[TaskReviews]([TaskAssignmentId])
                    END
                    ELSE
                    BEGIN
                        -- Add IsAutoFailed column if it doesn't exist
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[TaskReviews]') AND name = 'IsAutoFailed')
                        BEGIN
                            ALTER TABLE [dbo].[TaskReviews] ADD [IsAutoFailed] BIT NOT NULL DEFAULT 0;
                        END
                    END";

                // Execute table creation
                using (SqlCommand cmd = new SqlCommand(createUsersTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(createChoresTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(createChoreAssignmentsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(createOTPCodesTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(createFamiliesTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(createFamilyMembersTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(createTasksTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(createTaskObjectivesTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(createTaskAssignmentsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create PointTransactions AFTER TaskAssignments (since it references it)
                using (SqlCommand cmd = new SqlCommand(createPointTransactionsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                using (SqlCommand cmd = new SqlCommand(createTaskReviewsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create new tables for enhanced features
                
                // TaskObjectiveCompletions table (Critical Flaw Fix #1)
                string createTaskObjectiveCompletionsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskObjectiveCompletions]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[TaskObjectiveCompletions] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [TaskAssignmentId] INT NOT NULL,
                            [TaskObjectiveId] INT NOT NULL,
                            [IsCompleted] BIT NOT NULL DEFAULT 0,
                            [CompletedDate] DATETIME NULL,
                            FOREIGN KEY ([TaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id]),
                            FOREIGN KEY ([TaskObjectiveId]) REFERENCES [dbo].[TaskObjectives]([Id])
                        )
                        CREATE INDEX IX_TaskObjectiveCompletions_AssignmentId ON [dbo].[TaskObjectiveCompletions]([TaskAssignmentId])
                        CREATE INDEX IX_TaskObjectiveCompletions_ObjectiveId ON [dbo].[TaskObjectiveCompletions]([TaskObjectiveId])
                    END";
                using (SqlCommand cmd = new SqlCommand(createTaskObjectiveCompletionsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Notifications table (Critical Flaw Fix #4)
                string createNotificationsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Notifications]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[Notifications] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [UserId] INT NOT NULL,
                            [Title] NVARCHAR(200) NOT NULL,
                            [Message] NVARCHAR(MAX) NOT NULL,
                            [Type] NVARCHAR(50) NOT NULL,
                            [RelatedTaskId] INT NULL,
                            [RelatedAssignmentId] INT NULL,
                            [IsRead] BIT NOT NULL DEFAULT 0,
                            [ReadDate] DATETIME NULL,
                            [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
                            FOREIGN KEY ([RelatedTaskId]) REFERENCES [dbo].[Tasks]([Id]),
                            FOREIGN KEY ([RelatedAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id])
                        )
                        CREATE INDEX IX_Notifications_UserId ON [dbo].[Notifications]([UserId])
                        CREATE INDEX IX_Notifications_IsRead ON [dbo].[Notifications]([IsRead])
                    END";
                using (SqlCommand cmd = new SqlCommand(createNotificationsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // TaskTags table
                string createTaskTagsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskTags]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[TaskTags] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [Name] NVARCHAR(50) NOT NULL,
                            [Color] NVARCHAR(7) NULL,
                            [FamilyId] INT NULL,
                            FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id])
                        )
                        CREATE INDEX IX_TaskTags_FamilyId ON [dbo].[TaskTags]([FamilyId])
                    END";
                using (SqlCommand cmd = new SqlCommand(createTaskTagsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // TaskTagAssignments table
                string createTaskTagAssignmentsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskTagAssignments]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[TaskTagAssignments] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [TaskId] INT NOT NULL,
                            [TagId] INT NOT NULL,
                            FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks]([Id]) ON DELETE CASCADE,
                            FOREIGN KEY ([TagId]) REFERENCES [dbo].[TaskTags]([Id]) ON DELETE CASCADE,
                            UNIQUE ([TaskId], [TagId])
                        )
                        CREATE INDEX IX_TaskTagAssignments_TaskId ON [dbo].[TaskTagAssignments]([TaskId])
                        CREATE INDEX IX_TaskTagAssignments_TagId ON [dbo].[TaskTagAssignments]([TagId])
                    END";
                using (SqlCommand cmd = new SqlCommand(createTaskTagAssignmentsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // TaskTemplates table
                string createTaskTemplatesTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskTemplates]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[TaskTemplates] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [Name] NVARCHAR(200) NOT NULL,
                            [Description] NVARCHAR(MAX) NULL,
                            [Category] NVARCHAR(100) NOT NULL,
                            [PointsReward] INT NOT NULL DEFAULT 0,
                            [FamilyId] INT NULL,
                            [CreatedBy] INT NOT NULL,
                            [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            [IsActive] BIT NOT NULL DEFAULT 1,
                            FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id]),
                            FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([Id])
                        )
                        CREATE INDEX IX_TaskTemplates_FamilyId ON [dbo].[TaskTemplates]([FamilyId])
                    END";
                using (SqlCommand cmd = new SqlCommand(createTaskTemplatesTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // TaskTemplateObjectives table
                string createTaskTemplateObjectivesTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskTemplateObjectives]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[TaskTemplateObjectives] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [TemplateId] INT NOT NULL,
                            [ObjectiveText] NVARCHAR(500) NOT NULL,
                            [OrderIndex] INT NOT NULL DEFAULT 0,
                            FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[TaskTemplates]([Id]) ON DELETE CASCADE
                        )
                        CREATE INDEX IX_TaskTemplateObjectives_TemplateId ON [dbo].[TaskTemplateObjectives]([TemplateId])
                    END";
                using (SqlCommand cmd = new SqlCommand(createTaskTemplateObjectivesTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // TaskComments table
                string createTaskCommentsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskComments]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[TaskComments] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [TaskAssignmentId] INT NOT NULL,
                            [UserId] INT NOT NULL,
                            [CommentText] NVARCHAR(MAX) NOT NULL,
                            [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            [IsEdited] BIT NOT NULL DEFAULT 0,
                            [EditedDate] DATETIME NULL,
                            FOREIGN KEY ([TaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id]),
                            FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
                        )
                        CREATE INDEX IX_TaskComments_AssignmentId ON [dbo].[TaskComments]([TaskAssignmentId])
                    END";
                using (SqlCommand cmd = new SqlCommand(createTaskCommentsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // TaskAttachments table
                string createTaskAttachmentsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskAttachments]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[TaskAttachments] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [TaskAssignmentId] INT NOT NULL,
                            [FileName] NVARCHAR(255) NOT NULL,
                            [FilePath] NVARCHAR(500) NOT NULL,
                            [FileSize] INT NOT NULL,
                            [MimeType] NVARCHAR(100) NULL,
                            [UploadedBy] INT NOT NULL,
                            [UploadedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            FOREIGN KEY ([TaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id]),
                            FOREIGN KEY ([UploadedBy]) REFERENCES [dbo].[Users]([Id])
                        )
                        CREATE INDEX IX_TaskAttachments_AssignmentId ON [dbo].[TaskAttachments]([TaskAssignmentId])
                    END";
                using (SqlCommand cmd = new SqlCommand(createTaskAttachmentsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // TaskAuditLog table
                string createTaskAuditLogTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskAuditLog]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[TaskAuditLog] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [TaskId] INT NOT NULL,
                            [Action] NVARCHAR(50) NOT NULL,
                            [UserId] INT NOT NULL,
                            [OldValues] NVARCHAR(MAX) NULL,
                            [NewValues] NVARCHAR(MAX) NULL,
                            [ActionDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            FOREIGN KEY ([TaskId]) REFERENCES [dbo].[Tasks]([Id]),
                            FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id])
                        )
                        CREATE INDEX IX_TaskAuditLog_TaskId ON [dbo].[TaskAuditLog]([TaskId])
                        CREATE INDEX IX_TaskAuditLog_UserId ON [dbo].[TaskAuditLog]([UserId])
                    END";
                using (SqlCommand cmd = new SqlCommand(createTaskAuditLogTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // ============================================
                // REWARDS SYSTEM TABLES
                // ============================================

                // Rewards table
                string createRewardsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rewards]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[Rewards] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [FamilyId] INT NOT NULL,
                            [Name] NVARCHAR(200) NOT NULL,
                            [Description] NVARCHAR(MAX) NULL,
                            [PointCost] INT NOT NULL CHECK ([PointCost] > 0),
                            [Category] NVARCHAR(50) NULL,
                            [ImageUrl] NVARCHAR(500) NULL,
                            [IsActive] BIT NOT NULL DEFAULT 1,
                            [IsDeleted] BIT NOT NULL DEFAULT 0,
                            [CreatedBy] INT NOT NULL,
                            [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            [UpdatedDate] DATETIME NULL,
                            [UpdatedBy] INT NULL,
                            FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id]),
                            FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([Id]),
                            FOREIGN KEY ([UpdatedBy]) REFERENCES [dbo].[Users]([Id])
                        )
                        CREATE INDEX IX_Rewards_FamilyId_IsActive ON [dbo].[Rewards]([FamilyId], [IsActive]) WHERE [IsDeleted] = 0
                        CREATE INDEX IX_Rewards_Category ON [dbo].[Rewards]([Category]) WHERE [IsDeleted] = 0
                    END";
                using (SqlCommand cmd = new SqlCommand(createRewardsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // RewardOrders table
                string createRewardOrdersTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RewardOrders]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[RewardOrders] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [OrderNumber] NVARCHAR(50) NOT NULL UNIQUE,
                            [ChildId] INT NOT NULL,
                            [FamilyId] INT NOT NULL,
                            [TotalPoints] INT NOT NULL CHECK ([TotalPoints] > 0),
                            [Status] NVARCHAR(50) NOT NULL,
                            [RefundCode] NVARCHAR(50) NULL,
                            [OrderDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            [ConfirmedDate] DATETIME NULL,
                            [ConfirmedBy] INT NULL,
                            [DeclinedDate] DATETIME NULL,
                            [DeclinedBy] INT NULL,
                            [DeclinedReason] NVARCHAR(500) NULL,
                            [FulfilledDate] DATETIME NULL,
                            [FulfilledBy] INT NULL,
                            [ChildConfirmedDate] DATETIME NULL,
                            [RefundedDate] DATETIME NULL,
                            [RefundedBy] INT NULL,
                            [Notes] NVARCHAR(MAX) NULL,
                            FOREIGN KEY ([ChildId]) REFERENCES [dbo].[Users]([Id]),
                            FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id]),
                            FOREIGN KEY ([ConfirmedBy]) REFERENCES [dbo].[Users]([Id]),
                            FOREIGN KEY ([DeclinedBy]) REFERENCES [dbo].[Users]([Id]),
                            FOREIGN KEY ([FulfilledBy]) REFERENCES [dbo].[Users]([Id]),
                            FOREIGN KEY ([RefundedBy]) REFERENCES [dbo].[Users]([Id])
                        )
                        CREATE INDEX IX_RewardOrders_ChildId ON [dbo].[RewardOrders]([ChildId])
                        CREATE INDEX IX_RewardOrders_FamilyId_Status ON [dbo].[RewardOrders]([FamilyId], [Status])
                        CREATE INDEX IX_RewardOrders_OrderNumber ON [dbo].[RewardOrders]([OrderNumber])
                        -- Create filtered unique index for RefundCode (allows multiple NULLs)
                        CREATE UNIQUE NONCLUSTERED INDEX IX_RewardOrders_RefundCode 
                        ON [dbo].[RewardOrders]([RefundCode]) 
                        WHERE [RefundCode] IS NOT NULL
                    END";
                using (SqlCommand cmd = new SqlCommand(createRewardOrdersTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Migration: Fix RefundCode UNIQUE constraint for existing databases
                // Drop existing UNIQUE constraint if it exists (from old schema) and create filtered unique index
                string fixRefundCodeConstraint = @"
                    -- Find and drop existing UNIQUE constraint on RefundCode column
                    DECLARE @ConstraintName NVARCHAR(200)
                    SELECT TOP 1 @ConstraintName = kc.name
                    FROM sys.key_constraints kc
                    INNER JOIN sys.index_columns ic ON kc.parent_object_id = ic.object_id AND kc.unique_index_id = ic.index_id
                    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
                    WHERE kc.parent_object_id = OBJECT_ID(N'[dbo].[RewardOrders]')
                      AND kc.type = 'UQ'
                      AND c.name = 'RefundCode'
                    
                    IF @ConstraintName IS NOT NULL
                    BEGIN
                        DECLARE @DropSQL NVARCHAR(MAX) = 'ALTER TABLE [dbo].[RewardOrders] DROP CONSTRAINT [' + @ConstraintName + ']'
                        EXEC sp_executesql @DropSQL
                    END
                    
                    -- Drop existing non-unique index if it exists
                    IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RewardOrders_RefundCode' AND object_id = OBJECT_ID(N'[dbo].[RewardOrders]') AND is_unique = 0)
                    BEGIN
                        DROP INDEX IX_RewardOrders_RefundCode ON [dbo].[RewardOrders]
                    END
                    
                    -- Create filtered unique index (allows multiple NULLs, enforces uniqueness for non-NULL values)
                    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_RewardOrders_RefundCode' AND object_id = OBJECT_ID(N'[dbo].[RewardOrders]'))
                    BEGIN
                        CREATE UNIQUE NONCLUSTERED INDEX IX_RewardOrders_RefundCode 
                        ON [dbo].[RewardOrders]([RefundCode]) 
                        WHERE [RefundCode] IS NOT NULL
                    END";
                using (SqlCommand cmd = new SqlCommand(fixRefundCodeConstraint, conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                        System.Diagnostics.Debug.WriteLine("RefundCode constraint migration completed");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("RefundCode constraint migration error (may be expected for new databases): " + ex.Message);
                    }
                }

                // RewardOrderItems table
                string createRewardOrderItemsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RewardOrderItems]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[RewardOrderItems] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [OrderId] INT NOT NULL,
                            [RewardId] INT NOT NULL,
                            [Quantity] INT NOT NULL DEFAULT 1 CHECK ([Quantity] > 0),
                            [PointCost] INT NOT NULL CHECK ([PointCost] > 0),
                            [Subtotal] INT NOT NULL CHECK ([Subtotal] > 0),
                            FOREIGN KEY ([OrderId]) REFERENCES [dbo].[RewardOrders]([Id]) ON DELETE CASCADE,
                            FOREIGN KEY ([RewardId]) REFERENCES [dbo].[Rewards]([Id])
                        )
                        CREATE INDEX IX_RewardOrderItems_OrderId ON [dbo].[RewardOrderItems]([OrderId])
                        CREATE INDEX IX_RewardOrderItems_RewardId ON [dbo].[RewardOrderItems]([RewardId])
                    END";
                using (SqlCommand cmd = new SqlCommand(createRewardOrderItemsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // FamilyTreasury table
                string createFamilyTreasuryTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FamilyTreasury]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[FamilyTreasury] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [FamilyId] INT NOT NULL UNIQUE,
                            [Balance] INT NOT NULL DEFAULT 0 CHECK ([Balance] >= 0),
                            [LastUpdated] DATETIME NOT NULL DEFAULT GETDATE(),
                            FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id])
                        )
                        CREATE INDEX IX_FamilyTreasury_FamilyId ON [dbo].[FamilyTreasury]([FamilyId])
                    END";
                using (SqlCommand cmd = new SqlCommand(createFamilyTreasuryTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // TreasuryTransactions table (must be created after RewardOrders and TaskAssignments exist)
                string createTreasuryTransactionsTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TreasuryTransactions]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[TreasuryTransactions] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [FamilyId] INT NOT NULL,
                            [TransactionType] NVARCHAR(50) NOT NULL,
                            [Amount] INT NOT NULL,
                            [BalanceAfter] INT NOT NULL CHECK ([BalanceAfter] >= 0),
                            [Description] NVARCHAR(500) NULL,
                            [RelatedOrderId] INT NULL,
                            [RelatedTaskAssignmentId] INT NULL,
                            [RelatedPointTransactionId] INT NULL,
                            [CreatedBy] INT NULL,
                            [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
                            FOREIGN KEY ([FamilyId]) REFERENCES [dbo].[Families]([Id])
                        )
                        CREATE INDEX IX_TreasuryTransactions_FamilyId ON [dbo].[TreasuryTransactions]([FamilyId])
                        CREATE INDEX IX_TreasuryTransactions_CreatedDate ON [dbo].[TreasuryTransactions]([CreatedDate])
                    END
                    ELSE
                    BEGIN
                        -- Add foreign keys if tables exist and keys don't exist
                        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RewardOrders]') AND type in (N'U'))
                        AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TreasuryTransactions_RewardOrders')
                        BEGIN
                            ALTER TABLE [dbo].[TreasuryTransactions] ADD FOREIGN KEY ([RelatedOrderId]) REFERENCES [dbo].[RewardOrders]([Id]);
                        END
                        IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_TreasuryTransactions_RelatedOrderId')
                        BEGIN
                            CREATE INDEX IX_TreasuryTransactions_RelatedOrderId ON [dbo].[TreasuryTransactions]([RelatedOrderId]) WHERE [RelatedOrderId] IS NOT NULL;
                        END
                        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TaskAssignments]') AND type in (N'U'))
                        AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TreasuryTransactions_TaskAssignments')
                        BEGIN
                            ALTER TABLE [dbo].[TreasuryTransactions] ADD FOREIGN KEY ([RelatedTaskAssignmentId]) REFERENCES [dbo].[TaskAssignments]([Id]);
                        END
                        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PointTransactions]') AND type in (N'U'))
                        AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TreasuryTransactions_PointTransactions')
                        BEGIN
                            ALTER TABLE [dbo].[TreasuryTransactions] ADD FOREIGN KEY ([RelatedPointTransactionId]) REFERENCES [dbo].[PointTransactions]([Id]);
                        END
                        IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_TreasuryTransactions_Users')
                        BEGIN
                            ALTER TABLE [dbo].[TreasuryTransactions] ADD FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([Id]);
                        END
                    END";
                using (SqlCommand cmd = new SqlCommand(createTreasuryTransactionsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // RewardPurchaseHistory table
                string createRewardPurchaseHistoryTable = @"
                    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RewardPurchaseHistory]') AND type in (N'U'))
                    BEGIN
                        CREATE TABLE [dbo].[RewardPurchaseHistory] (
                            [Id] INT IDENTITY(1,1) PRIMARY KEY,
                            [OrderId] INT NOT NULL,
                            [ChildId] INT NOT NULL,
                            [RewardId] INT NOT NULL,
                            [RewardName] NVARCHAR(200) NOT NULL,
                            [PointCost] INT NOT NULL,
                            [Quantity] INT NOT NULL,
                            [PurchaseDate] DATETIME NOT NULL,
                            [FulfillmentStatus] NVARCHAR(50) NOT NULL,
                            [FulfilledDate] DATETIME NULL,
                            FOREIGN KEY ([OrderId]) REFERENCES [dbo].[RewardOrders]([Id]),
                            FOREIGN KEY ([ChildId]) REFERENCES [dbo].[Users]([Id]),
                            FOREIGN KEY ([RewardId]) REFERENCES [dbo].[Rewards]([Id])
                        )
                        CREATE INDEX IX_RewardPurchaseHistory_ChildId ON [dbo].[RewardPurchaseHistory]([ChildId])
                        CREATE INDEX IX_RewardPurchaseHistory_OrderId ON [dbo].[RewardPurchaseHistory]([OrderId])
                        CREATE INDEX IX_RewardPurchaseHistory_FulfillmentStatus ON [dbo].[RewardPurchaseHistory]([FulfillmentStatus])
                    END";
                using (SqlCommand cmd = new SqlCommand(createRewardPurchaseHistoryTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Update PointTransactions table with treasury-related columns
                string updatePointTransactionsTable = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PointTransactions]') AND type in (N'U'))
                    BEGIN
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PointTransactions]') AND name = 'TreasuryTransactionId')
                        BEGIN
                            ALTER TABLE [dbo].[PointTransactions] ADD [TreasuryTransactionId] INT NULL;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PointTransactions]') AND name = 'IsFromTreasury')
                        BEGIN
                            ALTER TABLE [dbo].[PointTransactions] ADD [IsFromTreasury] BIT NOT NULL DEFAULT 0;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PointTransactions]') AND name = 'IsToTreasury')
                        BEGIN
                            ALTER TABLE [dbo].[PointTransactions] ADD [IsToTreasury] BIT NOT NULL DEFAULT 0;
                        END
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[PointTransactions]') AND name = 'ExcessAmount')
                        BEGIN
                            ALTER TABLE [dbo].[PointTransactions] ADD [ExcessAmount] INT NULL;
                        END
                        -- Add foreign key to TreasuryTransactions if it exists
                        IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TreasuryTransactions]') AND type in (N'U'))
                        AND NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_PointTransactions_TreasuryTransactions')
                        BEGIN
                            ALTER TABLE [dbo].[PointTransactions] ADD FOREIGN KEY ([TreasuryTransactionId]) REFERENCES [dbo].[TreasuryTransactions]([Id]);
                        END
                    END";
                using (SqlCommand cmd = new SqlCommand(updatePointTransactionsTable, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Add points cap constraint to Users table
                string updateUsersTablePointsCap = @"
                    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
                    BEGIN
                        -- Ensure Points column exists first
                        IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND name = 'Points')
                        BEGIN
                            ALTER TABLE [dbo].[Users] ADD [Points] INT NOT NULL DEFAULT 0;
                        END
                        -- Add constraint if it doesn't exist
                        IF NOT EXISTS (SELECT * FROM sys.check_constraints WHERE name = 'CK_Users_Points_Range')
                        BEGIN
                            ALTER TABLE [dbo].[Users] ADD CONSTRAINT [CK_Users_Points_Range] CHECK ([Points] >= 0 AND [Points] <= 10000);
                        END
                    END";
                using (SqlCommand cmd = new SqlCommand(updateUsersTablePointsCap, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Create indexes
                CreateIndexesIfNotExist(conn);
                System.Diagnostics.Debug.WriteLine("All tables created successfully.");
                }
            }
            catch (SqlException sqlEx)
            {
                // Log SQL exceptions with details
                System.Diagnostics.Debug.WriteLine("SQL Error in CreateTablesIfNotExist: " + sqlEx.Message);
                System.Diagnostics.Debug.WriteLine("SQL Error Number: " + sqlEx.Number);
                System.Diagnostics.Debug.WriteLine("SQL Server: " + sqlEx.Server);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + sqlEx.StackTrace);
                
                // If database already exists or is in use, that's okay
                if (sqlEx.Number == 1801 || sqlEx.Number == 1832 || sqlEx.Number == 5120)
                {
                    System.Diagnostics.Debug.WriteLine("Database already exists or is in use - continuing...");
                }
                else
                {
                    // Re-throw other SQL exceptions so they're logged at the outer level
                    throw;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error creating tables: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Error type: " + ex.GetType().Name);
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                // Re-throw to be caught by outer handler
                throw;
            }
        }

        private static void CreateIndexesIfNotExist(SqlConnection conn)
        {
            string[] indexQueries = {
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChoreAssignments_UserId') CREATE INDEX IX_ChoreAssignments_UserId ON [dbo].[ChoreAssignments]([UserId])",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ChoreAssignments_ChoreId') CREATE INDEX IX_ChoreAssignments_ChoreId ON [dbo].[ChoreAssignments]([ChoreId])",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PointTransactions_UserId') CREATE INDEX IX_PointTransactions_UserId ON [dbo].[PointTransactions]([UserId])",
                "IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_PointTransactions_TransactionDate') CREATE INDEX IX_PointTransactions_TransactionDate ON [dbo].[PointTransactions]([TransactionDate])"
            };

            foreach (string query in indexQueries)
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    try
                    {
                        cmd.ExecuteNonQuery();
                    }
                    catch
                    {
                        // Index might already exist, continue
                    }
                }
            }
        }
    }
}

