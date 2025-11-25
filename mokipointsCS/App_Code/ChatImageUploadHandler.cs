using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.SessionState;

namespace mokipointsCS
{
    /// <summary>
    /// Handler for uploading chat images
    /// Supports image compression for files over 50MB
    /// </summary>
    public class ChatImageUploadHandler : IHttpHandler, IRequiresSessionState
    {
        private const int MAX_FILE_SIZE = 50 * 1024 * 1024; // 50MB
        private const int MAX_IMAGE_WIDTH = 1920;
        private const int MAX_IMAGE_HEIGHT = 1080;
        private const long JPEG_QUALITY = 85L; // 85% quality

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.Headers.Add("Access-Control-Allow-Origin", "*");

            try
            {
                // Check authentication
                if (context.Session["UserId"] == null)
                {
                    WriteErrorResponse(context, "User not authenticated");
                    return;
                }

                int userId = Convert.ToInt32(context.Session["UserId"]);

                // Get family ID
                int? familyId = FamilyHelper.GetUserFamilyId(userId);
                if (!familyId.HasValue)
                {
                    WriteErrorResponse(context, "User is not part of a family");
                    return;
                }

                // Check if file was uploaded
                if (context.Request.Files.Count == 0)
                {
                    WriteErrorResponse(context, "No file uploaded");
                    return;
                }

                HttpPostedFile file = context.Request.Files[0];

                // Validate file
                if (file.ContentLength == 0)
                {
                    WriteErrorResponse(context, "File is empty");
                    return;
                }

                // Check file extension
                string fileName = file.FileName;
                string fileExtension = Path.GetExtension(fileName).ToLower();
                if (fileExtension != ".jpg" && fileExtension != ".jpeg" && fileExtension != ".png" && fileExtension != ".gif")
                {
                    WriteErrorResponse(context, "Invalid file type. Only JPG, PNG, and GIF images are allowed.");
                    return;
                }

                // Check file size
                if (file.ContentLength > MAX_FILE_SIZE)
                {
                    WriteErrorResponse(context, string.Format("File size exceeds maximum allowed size of {0}MB", MAX_FILE_SIZE / (1024 * 1024)));
                    return;
                }

                // Create FamilyChat directory if it doesn't exist
                string familyChatDir = context.Server.MapPath("~/Images/FamilyChat");
                if (!Directory.Exists(familyChatDir))
                {
                    Directory.CreateDirectory(familyChatDir);
                    System.Diagnostics.Debug.WriteLine("ChatImageUpload: Created FamilyChat directory at " + familyChatDir);
                }

                // Generate unique filename
                string uniqueFileName = string.Format("chat_{0}_{1}_{2}.jpg", familyId.Value, userId, DateTime.Now.Ticks);
                string filePath = Path.Combine(familyChatDir, uniqueFileName);

                // Process and save image
                bool needsCompression = file.ContentLength > (10 * 1024 * 1024); // Compress if > 10MB

                using (System.Drawing.Image originalImage = System.Drawing.Image.FromStream(file.InputStream))
                {
                    System.Drawing.Image processedImage = originalImage;

                    try
                    {
                        // Compress/resize if needed
                        if (needsCompression || originalImage.Width > MAX_IMAGE_WIDTH || originalImage.Height > MAX_IMAGE_HEIGHT)
                        {
                            processedImage = ResizeImage(originalImage, MAX_IMAGE_WIDTH, MAX_IMAGE_HEIGHT);
                        }

                        // Save as JPEG with quality setting
                        ImageCodecInfo jpegCodec = GetEncoder(ImageFormat.Jpeg);
                        EncoderParameters encoderParams = new EncoderParameters(1);
                        encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, JPEG_QUALITY);

                        processedImage.Save(filePath, jpegCodec, encoderParams);

                        encoderParams.Dispose();
                        if (processedImage != originalImage)
                        {
                            processedImage.Dispose();
                        }
                    }
                    finally
                    {
                        if (processedImage != originalImage && processedImage != null)
                        {
                            processedImage.Dispose();
                        }
                    }
                }

                // Return success response with image path (absolute from site root)
                string imageUrl = "/Images/FamilyChat/" + uniqueFileName;
                WriteSuccessResponse(context, imageUrl, uniqueFileName);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ChatImageUpload error: " + ex.Message);
                System.Diagnostics.Debug.WriteLine("Stack Trace: " + ex.StackTrace);
                WriteErrorResponse(context, "An error occurred while uploading the image: " + ex.Message);
            }
        }

        private System.Drawing.Image ResizeImage(System.Drawing.Image originalImage, int maxWidth, int maxHeight)
        {
            int newWidth = originalImage.Width;
            int newHeight = originalImage.Height;

            // Calculate new dimensions maintaining aspect ratio
            if (newWidth > maxWidth || newHeight > maxHeight)
            {
                double ratioX = (double)maxWidth / newWidth;
                double ratioY = (double)maxHeight / newHeight;
                double ratio = Math.Min(ratioX, ratioY);

                newWidth = (int)(newWidth * ratio);
                newHeight = (int)(newHeight * ratio);
            }

            // Create new bitmap with new dimensions
            Bitmap newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphics = Graphics.FromImage(newImage))
            {
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.DrawImage(originalImage, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private void WriteSuccessResponse(HttpContext context, string imageUrl, string fileName)
        {
            var response = new
            {
                success = true,
                imageUrl = imageUrl,
                fileName = fileName
            };

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(response);
            context.Response.Write(json);
        }

        private void WriteErrorResponse(HttpContext context, string errorMessage)
        {
            var response = new
            {
                success = false,
                error = errorMessage
            };

            JavaScriptSerializer serializer = new JavaScriptSerializer();
            string json = serializer.Serialize(response);
            context.Response.Write(json);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}

