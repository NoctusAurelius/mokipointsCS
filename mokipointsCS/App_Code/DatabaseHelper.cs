using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using System.Web;

namespace mokipointsCS
{
    /// <summary>
    /// Database helper class for Mokipoints application
    /// Provides methods for database operations
    /// </summary>
    public class DatabaseHelper
    {
        private static string ConnectionString
        {
            get
            {
                string connString = ConfigurationManager.ConnectionStrings["MokipointsDB"].ConnectionString;
                
                // Expand |DataDirectory| token to actual App_Data path
                if (System.Web.HttpContext.Current != null)
                {
                    string dataDir = System.Web.HttpContext.Current.Server.MapPath("~/App_Data");
                    connString = connString.Replace("|DataDirectory|", dataDir);
                }
                else
                {
                    // Fallback for non-web contexts
                    string appDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data");
                    connString = connString.Replace("|DataDirectory|", appDataPath);
                }
                
                return connString;
            }
        }

        /// <summary>
        /// Gets a database connection
        /// </summary>
        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        /// <summary>
        /// Tests the database connection
        /// </summary>
        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                System.Diagnostics.Debug.WriteLine("Database connection error: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Executes a SQL command and returns a DataTable
        /// </summary>
        public static DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        conn.Open();
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Query execution error: " + ex.Message);
                throw;
            }
            return dt;
        }

        /// <summary>
        /// Executes a non-query SQL command (INSERT, UPDATE, DELETE)
        /// </summary>
        public static int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            int rowsAffected = 0;
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        conn.Open();
                        rowsAffected = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Non-query execution error: " + ex.Message);
                throw;
            }
            return rowsAffected;
        }

        /// <summary>
        /// Executes a scalar query and returns a single value
        /// </summary>
        public static object ExecuteScalar(string query, params SqlParameter[] parameters)
        {
            object result = null;
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (parameters != null)
                        {
                            cmd.Parameters.AddRange(parameters);
                        }
                        conn.Open();
                        result = cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Scalar execution error: " + ex.Message);
                throw;
            }
            return result;
        }
    }
}

