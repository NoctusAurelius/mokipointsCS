using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
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
            System.Drawing.Image originalImage = null;
            System.Drawing.Image resizedImage = null;
            string tempFilePath = null;
            
            try
            {
                // Check authentication
                if (Session["UserId"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("Profile: User not authenticated");
                    ShowMessage("You must be logged in to upload a profile picture.", "error");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("Profile: Checking if file exists");
                if (!fileUpload.HasFile)
                {
                    System.Diagnostics.Debug.WriteLine("Profile: No file uploaded");
                    ShowMessage("Please select an image file to upload.", "error");
                    return;
                }

                System.Diagnostics.Debug.WriteLine("Profile: File found - " + fileUpload.FileName);
                System.Diagnostics.Debug.WriteLine("Profile: File size - " + fileUpload.PostedFile.ContentLength + " bytes");
                
                // Validate file
                string fileName = fileUpload.FileName;
                string fileExtension = Path.GetExtension(fileName).ToLower();
                System.Diagnostics.Debug.WriteLine("Profile: File extension - " + fileExtension);
                
                // Check file extension
                if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png" && fileExtension != ".gif" && fileExtension != ".bmp")
                {
                    ShowMessage("Please upload a valid image file (JPG, PNG, GIF, or BMP).", "error");
                    return;
                }

                // Check file size (10MB limit for better compatibility)
                const int maxFileSize = 10 * 1024 * 1024; // 10MB
                if (fileUpload.PostedFile.ContentLength > maxFileSize)
                {
                    ShowMessage("File size must be less than 10MB. Please compress or resize your image.", "error");
                    return;
                }

                // Check minimum file size (must have some content)
                if (fileUpload.PostedFile.ContentLength < 100)
                {
                    ShowMessage("The uploaded file appears to be empty or corrupted.", "error");
                    return;
                }

                // Get user ID
                int userId = Convert.ToInt32(Session["UserId"]);
                System.Diagnostics.Debug.WriteLine("Profile: UserId = " + userId);

                // Create ProfilePictures directory if it doesn't exist (in Images folder for HTTP access)
                string profilePicturesDir = null;
                try
                {
                    profilePicturesDir = Server.MapPath("~/Images/ProfilePictures");
                    System.Diagnostics.Debug.WriteLine("Profile: ProfilePictures directory path: " + profilePicturesDir);
                    
                    if (string.IsNullOrEmpty(profilePicturesDir))
                    {
                        throw new Exception("Unable to resolve ProfilePictures directory path.");
                    }

                    // Ensure Images directory exists first
                    string imagesDir = Server.MapPath("~/Images");
                    if (!Directory.Exists(imagesDir))
                    {
                        try
                        {
                            Directory.CreateDirectory(imagesDir);
                            System.Diagnostics.Debug.WriteLine("Profile: Created Images directory at " + imagesDir);
                        }
                        catch (Exception dirEx)
                        {
                            System.Diagnostics.Debug.WriteLine("Profile: Failed to create Images directory: " + dirEx.Message);
                            ShowMessage("Unable to create Images directory. Please check folder permissions.", "error");
                            return;
                        }
                    }

                    if (!Directory.Exists(profilePicturesDir))
                    {
                        try
                        {
                            Directory.CreateDirectory(profilePicturesDir);
                            System.Diagnostics.Debug.WriteLine("Profile: Created ProfilePictures directory at " + profilePicturesDir);
                        }
                        catch (Exception dirEx)
                        {
                            System.Diagnostics.Debug.WriteLine("Profile: Failed to create ProfilePictures directory: " + dirEx.Message);
                            ShowMessage("Unable to create ProfilePictures directory. Please check folder permissions.", "error");
                            return;
                        }
                    }

                    // Check write permissions
                    try
                    {
                        string testFile = Path.Combine(profilePicturesDir, "test_write.tmp");
                        File.WriteAllText(testFile, "test");
                        File.Delete(testFile);
                        System.Diagnostics.Debug.WriteLine("Profile: Write permissions verified");
                    }
                    catch (Exception permEx)
                    {
                        System.Diagnostics.Debug.WriteLine("Profile: Write permission check failed: " + permEx.Message);
                        ShowMessage("Unable to write to ProfilePictures directory. Please check folder permissions.", "error");
                        return;
                    }
                }
                catch (Exception pathEx)
                {
                    System.Diagnostics.Debug.WriteLine("Profile: Path resolution error: " + pathEx.Message);
                    ShowMessage("Unable to access the upload directory. Please contact support.", "error");
                    return;
                }

                // Generate unique filename
                string uniqueFileName = userId + "_" + DateTime.Now.Ticks + ".jpg";
                string filePath = Path.Combine(profilePicturesDir, uniqueFileName);

                // Validate and process image directly from stream (no temp file)
                try
                {
                    // Validate image format by trying to load it directly from the uploaded stream
                    try
                    {
                        // Reset stream position to beginning
                        fileUpload.PostedFile.InputStream.Position = 0;
                        
                        // Load image directly from stream with validation
                        originalImage = System.Drawing.Image.FromStream(fileUpload.PostedFile.InputStream, false, false);
                        System.Diagnostics.Debug.WriteLine("Profile: Image loaded successfully - Size: " + originalImage.Width + "x" + originalImage.Height);
                    }
                    catch (ArgumentException imgEx)
                    {
                        System.Diagnostics.Debug.WriteLine("Profile: Invalid image format: " + imgEx.Message);
                        ShowMessage("The uploaded file is not a valid image or is corrupted. Please try a different image.", "error");
                        return;
                    }
                    catch (OutOfMemoryException memEx)
                    {
                        System.Diagnostics.Debug.WriteLine("Profile: Image too large for processing: " + memEx.Message);
                        ShowMessage("The image is too large to process. Please resize it and try again.", "error");
                        return;
                    }

                    // Process and resize image
                    try
                    {
                        resizedImage = ResizeAndCropToSquare(originalImage, 500);
                        System.Diagnostics.Debug.WriteLine("Profile: Image resized successfully");

                        // Dispose original image before saving to free resources
                        if (originalImage != null)
                        {
                            originalImage.Dispose();
                            originalImage = null;
                        }

                        // Save as JPEG with quality settings
                        // Use a new bitmap to ensure it's not locked
                        using (Bitmap finalBitmap = new Bitmap(resizedImage))
                        {
                            ImageCodecInfo jpegCodec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == ImageFormat.Jpeg.Guid);
                            
                            if (jpegCodec != null)
                            {
                                using (EncoderParameters encoderParams = new EncoderParameters(1))
                                {
                                    encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, 85L); // 85% quality
                                    
                                    // Ensure directory exists and file doesn't exist
                                    if (File.Exists(filePath))
                                    {
                                        try
                                        {
                                            File.Delete(filePath);
                                            System.Diagnostics.Debug.WriteLine("Profile: Deleted existing file");
                                        }
                                        catch (Exception delEx)
                                        {
                                            System.Diagnostics.Debug.WriteLine("Profile: Warning - Could not delete existing file: " + delEx.Message);
                                        }
                                    }
                                    
                                    // Save the image
                                    finalBitmap.Save(filePath, jpegCodec, encoderParams);
                                    System.Diagnostics.Debug.WriteLine("Profile: Image saved successfully: " + filePath);
                                }
                            }
                            else
                            {
                                // Fallback if codec not found
                                if (File.Exists(filePath))
                                {
                                    try { File.Delete(filePath); } catch { }
                                }
                                finalBitmap.Save(filePath, ImageFormat.Jpeg);
                                System.Diagnostics.Debug.WriteLine("Profile: Image saved successfully (fallback): " + filePath);
                            }
                        }
                    }
                    catch (System.Runtime.InteropServices.ExternalException gdiEx)
                    {
                        System.Diagnostics.Debug.WriteLine("Profile: GDI+ error saving image: " + gdiEx.Message);
                        // Clean up any partial file
                        if (File.Exists(filePath))
                        {
                            try 
                            { 
                                File.Delete(filePath);
                                System.Diagnostics.Debug.WriteLine("Profile: Cleaned up partial file");
                            } 
                            catch { }
                        }
                        ShowMessage("Failed to save the image. Please try again or use a different image file.", "error");
                        return;
                    }
                    catch (Exception saveEx)
                    {
                        System.Diagnostics.Debug.WriteLine("Profile: Error saving image: " + saveEx.Message);
                        // Clean up any partial file
                        if (File.Exists(filePath))
                        {
                            try { File.Delete(filePath); } catch { }
                        }
                        ShowMessage("Failed to process the image. Please try a different image file.", "error");
                        return;
                    }
                }
                catch (Exception processEx)
                {
                    System.Diagnostics.Debug.WriteLine("Profile: Error processing image: " + processEx.Message);
                    ShowMessage("An error occurred while processing the image: " + processEx.Message, "error");
                    return;
                }

                // Update database
                try
                {
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
                        // Clean up uploaded file if database update failed
                        if (File.Exists(filePath))
                        {
                            try { File.Delete(filePath); } catch { }
                        }
                        ShowMessage("Failed to update profile picture in database. Please try again.", "error");
                    }
                }
                catch (Exception dbEx)
                {
                    System.Diagnostics.Debug.WriteLine("Profile: Database error: " + dbEx.Message);
                    // Clean up uploaded file if database update failed
                    if (File.Exists(filePath))
                    {
                        try { File.Delete(filePath); } catch { }
                    }
                    ShowMessage("Failed to save profile picture. Please try again.", "error");
                }
            }
            catch (System.Threading.ThreadAbortException)
            {
                // Ignore thread abort exceptions (from redirects)
                throw;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Upload profile picture error: " + ex.Message);
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
                System.Diagnostics.Debug.WriteLine("Stack trace: " + ex.StackTrace);
                
                // Clean up any temporary files
                if (!string.IsNullOrEmpty(tempFilePath) && File.Exists(tempFilePath))
                {
                    try { File.Delete(tempFilePath); } catch { }
                }
                
                // Show user-friendly error message
                string errorMessage = "An error occurred while uploading the profile picture.";
                if (ex.Message.Contains("timeout") || ex.Message.Contains("time"))
                {
                    errorMessage = "The upload timed out. Please try a smaller image file.";
                }
                else if (ex.Message.Contains("permission") || ex.Message.Contains("access"))
                {
                    errorMessage = "Permission denied. Please check folder permissions.";
                }
                else if (ex.Message.Contains("path") || ex.Message.Contains("directory"))
                {
                    errorMessage = "Unable to access the upload directory. Please contact support.";
                }
                
                ShowMessage(errorMessage, "error");
            }
            finally
            {
                // Dispose image resources
                if (originalImage != null)
                {
                    try { originalImage.Dispose(); } catch { }
                }
                if (resizedImage != null)
                {
                    try { resizedImage.Dispose(); } catch { }
                }
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

            // Create bitmap for cropped image with proper pixel format
            Bitmap croppedBitmap = new Bitmap(cropSize, cropSize, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                // Clear with white background first
                g.Clear(Color.White);

                // Draw cropped image
                g.DrawImage(originalImage, new Rectangle(0, 0, cropSize, cropSize),
                    new Rectangle(x, y, cropSize, cropSize), GraphicsUnit.Pixel);
            }

            // Resize to target size with proper pixel format
            Bitmap resizedBitmap = new Bitmap(size, size, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(resizedBitmap))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

                // Clear with white background first
                g.Clear(Color.White);

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

