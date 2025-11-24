using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace mokipointsCS
{
    /// <summary>
    /// Helper class for OTP code generation and validation
    /// </summary>
    public class OTPHelper
    {
        private const int OTP_LENGTH = 6;
        private const int OTP_EXPIRY_MINUTES = 10;

        /// <summary>
        /// Generates a 6-digit OTP code
        /// </summary>
        public static string GenerateOTP()
        {
            Random random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        /// <summary>
        /// Creates and stores an OTP code in the database
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="purpose">Purpose: "Registration" or "ForgotPassword"</param>
        /// <returns>OTP code if successful, null if failed</returns>
        public static string CreateOTP(string email, string purpose)
        {
            try
            {
                // Invalidate any existing unused OTPs for this email and purpose
                InvalidateExistingOTPs(email, purpose);

                // Generate new OTP
                string otpCode = GenerateOTP();
                DateTime expiryDate = DateTime.Now.AddMinutes(OTP_EXPIRY_MINUTES);

                // Store in database
                string query = @"
                    INSERT INTO [dbo].[OTPCodes] (Email, Code, Purpose, ExpiryDate, IsUsed)
                    VALUES (@Email, @Code, @Purpose, @ExpiryDate, 0)";

                int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@Email", email),
                    new SqlParameter("@Code", otpCode),
                    new SqlParameter("@Purpose", purpose),
                    new SqlParameter("@ExpiryDate", expiryDate));

                if (rowsAffected > 0)
                {
                    LogOTPCreation(email, purpose, true, null);
                    return otpCode;
                }
                else
                {
                    LogOTPCreation(email, purpose, false, "Failed to insert OTP into database");
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogOTPCreation(email, purpose, false, ex.Message);
                System.Diagnostics.Debug.WriteLine("OTP creation error: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Validates an OTP code
        /// </summary>
        /// <param name="email">User's email address</param>
        /// <param name="code">OTP code to validate</param>
        /// <param name="purpose">Purpose: "Registration" or "ForgotPassword"</param>
        /// <returns>True if valid, false otherwise</returns>
        public static bool ValidateOTP(string email, string code, string purpose)
        {
            try
            {
                string query = @"
                    SELECT Id, ExpiryDate, IsUsed
                    FROM [dbo].[OTPCodes]
                    WHERE Email = @Email 
                      AND Code = @Code 
                      AND Purpose = @Purpose
                      AND IsUsed = 0
                    ORDER BY CreatedDate DESC";

                using (DataTable dt = DatabaseHelper.ExecuteQuery(query,
                    new SqlParameter("@Email", email),
                    new SqlParameter("@Code", code),
                    new SqlParameter("@Purpose", purpose)))
                {
                    if (dt.Rows.Count == 0)
                    {
                        LogOTPValidation(email, code, purpose, false, "OTP not found or already used");
                        return false;
                    }

                    DataRow row = dt.Rows[0];
                    DateTime expiryDate = Convert.ToDateTime(row["ExpiryDate"]);
                    bool isUsed = Convert.ToBoolean(row["IsUsed"]);

                    // Check if expired
                    if (DateTime.Now > expiryDate)
                    {
                        LogOTPValidation(email, code, purpose, false, "OTP expired");
                        return false;
                    }

                    // Check if already used
                    if (isUsed)
                    {
                        LogOTPValidation(email, code, purpose, false, "OTP already used");
                        return false;
                    }

                    // Mark as used
                    int otpId = Convert.ToInt32(row["Id"]);
                    MarkOTPAsUsed(otpId);

                    LogOTPValidation(email, code, purpose, true, null);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogOTPValidation(email, code, purpose, false, ex.Message);
                System.Diagnostics.Debug.WriteLine("OTP validation error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Invalidates existing unused OTPs for an email and purpose
        /// </summary>
        private static void InvalidateExistingOTPs(string email, string purpose)
        {
            try
            {
                string query = @"
                    UPDATE [dbo].[OTPCodes]
                    SET IsUsed = 1
                    WHERE Email = @Email 
                      AND Purpose = @Purpose
                      AND IsUsed = 0
                      AND ExpiryDate > GETDATE()";

                DatabaseHelper.ExecuteNonQuery(query,
                    new SqlParameter("@Email", email),
                    new SqlParameter("@Purpose", purpose));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Invalidate OTPs error: " + ex.Message);
            }
        }

        /// <summary>
        /// Marks an OTP as used
        /// </summary>
        private static void MarkOTPAsUsed(int otpId)
        {
            try
            {
                string query = @"
                    UPDATE [dbo].[OTPCodes]
                    SET IsUsed = 1
                    WHERE Id = @Id";

                DatabaseHelper.ExecuteNonQuery(query, new SqlParameter("@Id", otpId));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Mark OTP as used error: " + ex.Message);
            }
        }

        /// <summary>
        /// Logs OTP creation attempts
        /// </summary>
        private static void LogOTPCreation(string email, string purpose, bool success, string errorMessage)
        {
            try
            {
                string logMessage = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] OTP Creation - Email: {1}, Purpose: {2}, Success: {3}", 
                    DateTime.Now, email, purpose, success);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    logMessage += ", Error: " + errorMessage;
                }
                System.Diagnostics.Debug.WriteLine(logMessage);
            }
            catch
            {
                // Ignore logging errors
            }
        }

        /// <summary>
        /// Logs OTP validation attempts
        /// </summary>
        private static void LogOTPValidation(string email, string code, string purpose, bool success, string errorMessage)
        {
            try
            {
                string logMessage = string.Format("[{0:yyyy-MM-dd HH:mm:ss}] OTP Validation - Email: {1}, Purpose: {2}, Success: {3}", 
                    DateTime.Now, email, purpose, success);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    logMessage += ", Error: " + errorMessage;
                }
                System.Diagnostics.Debug.WriteLine(logMessage);
            }
            catch
            {
                // Ignore logging errors
            }
        }
    }
}

