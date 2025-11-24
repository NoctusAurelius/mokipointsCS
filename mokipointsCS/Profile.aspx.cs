using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.UI;

namespace mokipointsCS
{
    public partial class Profile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Profile.aspx Page_Load called at " + DateTime.Now.ToString());
            try
            {
                // Check authentication
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("Profile: User not authenticated - redirecting to Login");
                    Response.Redirect("Login.aspx", false);
                    return;
                }

                if (!IsPostBack)
                {
                    System.Diagnostics.Debug.WriteLine("Profile: Loading profile page");
                    LoadUserProfile();
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                throw; // Re-throw to allow redirect to complete
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Profile Page_Load error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowMessage("An error occurred loading your profile.", "error");
            }
        }

        private void LoadUserProfile()
        {
            try
            {
                int userId = Convert.ToInt32(Session["UserId"]);
                var userInfo = AuthenticationHelper.GetUserById(userId);

                if (userInfo != null)
                {
                    // Display user information
                    string firstName = userInfo["FirstName"].ToString();
                    string lastName = userInfo["LastName"].ToString();
                    string middleName = (userInfo["MiddleName"] != null && userInfo["MiddleName"] != DBNull.Value) ? userInfo["MiddleName"].ToString() : "";
                    string email = userInfo["Email"].ToString();
                    string role = userInfo["Role"].ToString();
                    DateTime? birthday = (userInfo["Birthday"] != null && userInfo["Birthday"] != DBNull.Value) ? (DateTime?)userInfo["Birthday"] : null;
                    DateTime createdDate = (userInfo["CreatedDate"] != null && userInfo["CreatedDate"] != DBNull.Value) ? Convert.ToDateTime(userInfo["CreatedDate"]) : DateTime.Now;
                    
                    // Check if ProfilePicture column exists (for databases that haven't been migrated yet)
                    string profilePicture = null;
                    if (userInfo.Table.Columns.Contains("ProfilePicture"))
                    {
                        profilePicture = (userInfo["ProfilePicture"] != null && userInfo["ProfilePicture"] != DBNull.Value) ? userInfo["ProfilePicture"].ToString() : null;
                    }

                    // Set literals
                    litFullName.Text = firstName + " " + lastName;
                    litEmail.Text = email;
                    litRole.Text = role;
                    litFirstName.Text = firstName;
                    litLastName.Text = lastName;
                    litMiddleName.Text = string.IsNullOrEmpty(middleName) ? "N/A" : middleName;
                    litBirthday.Text = birthday.HasValue ? birthday.Value.ToString("MMMM dd, yyyy") : "Not set";
                    litCreatedDate.Text = createdDate.ToString("MMMM dd, yyyy");

                    // Load profile picture
                    if (!string.IsNullOrEmpty(profilePicture))
                    {
                        // Check new location first (Images/ProfilePictures/)
                        string picturePath = Server.MapPath("~/Images/ProfilePictures/" + profilePicture);
                        if (!File.Exists(picturePath))
                        {
                            // Fallback to old location (App_Data/ProfilePictures/) for migration
                            string oldPath = Server.MapPath("~/App_Data/ProfilePictures/" + profilePicture);
                            if (File.Exists(oldPath))
                            {
                                // Migrate file to new location
                                string newDir = Server.MapPath("~/Images/ProfilePictures");
                                if (!Directory.Exists(newDir))
                                {
                                    Directory.CreateDirectory(newDir);
                                }
                                File.Copy(oldPath, picturePath, true);
                                System.Diagnostics.Debug.WriteLine("Profile: Migrated profile picture from App_Data to Images folder");
                            }
                        }
                        
                        if (File.Exists(picturePath))
                        {
                            imgProfilePicture.ImageUrl = "~/Images/ProfilePictures/" + profilePicture;
                            imgProfilePicture.Visible = true;
                            profilePlaceholder.Visible = false;
                        }
                        else
                        {
                            imgProfilePicture.Visible = false;
                            profilePlaceholder.Visible = true;
                        }
                    }
                    else
                    {
                        imgProfilePicture.Visible = false;
                        profilePlaceholder.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadUserProfile error: " + ex.Message);
                ShowMessage("Error loading profile information.", "error");
            }
        }

        protected void btnUploadHidden_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Profile: btnUploadHidden_Click called at " + DateTime.Now.ToString());
            try
            {
                System.Diagnostics.Debug.WriteLine("Profile: Checking if file exists");
                if (fileUpload.HasFile)
                {
                    System.Diagnostics.Debug.WriteLine("Profile: File found - " + fileUpload.FileName);
                    System.Diagnostics.Debug.WriteLine("Profile: File size - " + fileUpload.PostedFile.ContentLength + " bytes");
                    
                    // Validate file
                    string fileName = fileUpload.FileName;
                    string fileExtension = Path.GetExtension(fileName).ToLower();
                    System.Diagnostics.Debug.WriteLine("Profile: File extension - " + fileExtension);
                    
                    // Check file extension
                    if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png" && fileExtension != ".gif")
                    {
                        ShowMessage("Please upload a valid image file (JPG, PNG, or GIF).", "error");
                        return;
                    }

                    // Check file size (25MB = 25 * 1024 * 1024 bytes)
                    if (fileUpload.PostedFile.ContentLength > 25 * 1024 * 1024)
                    {
                        ShowMessage("File size must be less than 25MB.", "error");
                        return;
                    }

                    // Get user ID
                    int userId = Convert.ToInt32(Session["UserId"]);

                    // Create ProfilePictures directory if it doesn't exist (in Images folder for HTTP access)
                    string profilePicturesDir = Server.MapPath("~/Images/ProfilePictures");
                    if (!Directory.Exists(profilePicturesDir))
                    {
                        Directory.CreateDirectory(profilePicturesDir);
                        System.Diagnostics.Debug.WriteLine("Profile: Created ProfilePictures directory at " + profilePicturesDir);
                    }

                    // Generate unique filename
                    string uniqueFileName = userId + "_" + DateTime.Now.Ticks + fileExtension;
                    string filePath = Path.Combine(profilePicturesDir, uniqueFileName);

                    // Process and save image
                    using (System.Drawing.Image originalImage = System.Drawing.Image.FromStream(fileUpload.PostedFile.InputStream))
                    {
                        // Resize and crop to 500x500 square
                        using (System.Drawing.Image resizedImage = ResizeAndCropToSquare(originalImage, 500))
                        {
                            // Save as JPEG
                            string jpegPath = filePath.Replace(fileExtension, ".jpg");
                            resizedImage.Save(jpegPath, ImageFormat.Jpeg);
                            uniqueFileName = Path.GetFileName(jpegPath);
                        }
                    }

                    // Update database
                    string query = @"
                        UPDATE [dbo].[Users]
                        SET ProfilePicture = @ProfilePicture
                        WHERE Id = @UserId";

                    int rowsAffected = DatabaseHelper.ExecuteNonQuery(query,
                        new System.Data.SqlClient.SqlParameter("@ProfilePicture", uniqueFileName),
                        new System.Data.SqlClient.SqlParameter("@UserId", userId));

                    if (rowsAffected > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("Profile: Database updated successfully");
                        ShowMessage("Profile picture updated successfully!", "success");
                        // Reload profile to show new picture
                        LoadUserProfile();
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Profile: Database update failed - no rows affected");
                        ShowMessage("Failed to update profile picture.", "error");
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Upload profile picture error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                ShowMessage("An error occurred while uploading the profile picture: " + ex.Message, "error");
            }
            finally
            {
                System.Diagnostics.Debug.WriteLine("Profile: btnUploadHidden_Click completed");
            }
        }

        private System.Drawing.Image ResizeAndCropToSquare(System.Drawing.Image originalImage, int size)
        {
            // Calculate dimensions for square crop
            int originalWidth = originalImage.Width;
            int originalHeight = originalImage.Height;
            int cropSize = Math.Min(originalWidth, originalHeight);
            int x = (originalWidth - cropSize) / 2;
            int y = (originalHeight - cropSize) / 2;

            // Create bitmap for cropped image
            Bitmap croppedBitmap = new Bitmap(cropSize, cropSize);
            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                // Draw cropped image
                g.DrawImage(originalImage, new Rectangle(0, 0, cropSize, cropSize),
                    new Rectangle(x, y, cropSize, cropSize), GraphicsUnit.Pixel);
            }

            // Resize to target size
            Bitmap resizedBitmap = new Bitmap(size, size);
            using (Graphics g = Graphics.FromImage(resizedBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                g.DrawImage(croppedBitmap, new Rectangle(0, 0, size, size));
            }

            // Dispose cropped bitmap
            croppedBitmap.Dispose();

            return resizedBitmap;
        }

        protected void btnChangePassword_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Profile: Change Password button clicked");
            Response.Redirect("VerifyCurrentPassword.aspx", false);
        }

        private void ShowMessage(string message, string type)
        {
            lblMessage.Text = message;
            lblMessage.CssClass = "message " + (type == "success" ? "message-success" : "message-error");
            lblMessage.Visible = true;
        }
    }
}

