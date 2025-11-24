using System;
using System.Data;
using System.Data.SqlClient;

namespace mokipointsCS
{
    /// <summary>
    /// Helper class for user authentication
    /// </summary>
    public class AuthenticationHelper
    {
        /// <summary>
        /// Authenticates a user by email and password
        /// </summary>
        /// <param name="email">User's email</param>
        /// <param name="password">User's password (plain text)</param>
        /// <returns>User ID if authenticated, -1 if not</returns>
        public static int AuthenticateUser(string email, string password)
        {
            try
            {
                string query = @"
                    SELECT Id, Password, IsActive 
                    FROM [dbo].[Users] 
                    WHERE Email = @Email";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query, 
                    new SqlParameter("@Email", email)))
                {
                    if (dt.Rows.Count == 0)
                    {
                        // User not found
                        return -1;
                    }

                    DataRow row = dt.Rows[0];
                    string storedPasswordHash = row["Password"].ToString();
                    bool isActive = Convert.ToBoolean(row["IsActive"]);

                    // Check if user is active
                    if (!isActive)
                    {
                        return -1;
                    }

                    // Verify password
                    if (PasswordHelper.VerifyPassword(password, storedPasswordHash))
                    {
                        return Convert.ToInt32(row["Id"]);
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Authentication error: " + ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Gets user information by ID
        /// </summary>
        public static DataRow GetUserById(int userId)
        {
            try
            {
                // Try to get user with ProfilePicture column first
                string query = @"
                    SELECT Id, FirstName, LastName, MiddleName, Email, Birthday, Role, CreatedDate, IsActive, ProfilePicture 
                    FROM [dbo].[Users] 
                    WHERE Id = @UserId AND IsActive = 1";

                try
                {
                    using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                        new SqlParameter("@UserId", userId)))
                    {
                        if (dt.Rows.Count > 0)
                        {
                            return dt.Rows[0];
                        }
                    }
                }
                catch (System.Data.SqlClient.SqlException sqlEx)
                {
                    // If ProfilePicture column doesn't exist, try without it
                    if (sqlEx.Message.Contains("ProfilePicture") || sqlEx.Message.Contains("Invalid column name"))
                    {
                        System.Diagnostics.Debug.WriteLine("ProfilePicture column not found, using fallback query");
                        query = @"
                            SELECT Id, FirstName, LastName, MiddleName, Email, Birthday, Role, CreatedDate, IsActive 
                            FROM [dbo].[Users] 
                            WHERE Id = @UserId AND IsActive = 1";

                        using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                            new SqlParameter("@UserId", userId)))
                        {
                            if (dt.Rows.Count > 0)
                            {
                                return dt.Rows[0];
                            }
                        }
                    }
                    else
                    {
                        throw; // Re-throw if it's a different SQL error
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("GetUserById error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
            }
            return null;
        }

        /// <summary>
        /// Creates a new user account
        /// </summary>
        public static int CreateUser(string firstName, string lastName, string middleName, string email, string password, string role, DateTime? birthday)
        {
            try
            {
                // Check if email already exists
                string checkQuery = "SELECT COUNT(*) FROM [dbo].[Users] WHERE Email = @Email";
                object count = DatabaseHelper.ExecuteScalar(checkQuery, new SqlParameter("@Email", email));
                
                if (Convert.ToInt32(count) > 0)
                {
                    // Email already exists
                    return -1;
                }

                // Hash the password
                string passwordHash = PasswordHelper.HashPassword(password);

                // Insert new user
                string insertQuery = @"
                    INSERT INTO [dbo].[Users] (FirstName, LastName, MiddleName, Email, Password, Birthday, Role, IsActive)
                    VALUES (@FirstName, @LastName, @MiddleName, @Email, @Password, @Birthday, @Role, 1);
                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                object userId = DatabaseHelper.ExecuteScalar(insertQuery,
                    new SqlParameter("@FirstName", firstName),
                    new SqlParameter("@LastName", lastName),
                    new SqlParameter("@MiddleName", string.IsNullOrEmpty(middleName) ? (object)DBNull.Value : middleName),
                    new SqlParameter("@Email", email),
                    new SqlParameter("@Password", passwordHash),
                    new SqlParameter("@Birthday", birthday.HasValue ? (object)birthday.Value : DBNull.Value),
                    new SqlParameter("@Role", role));

                return Convert.ToInt32(userId);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CreateUser error: " + ex.Message);
                return -1;
            }
        }
    }
}

