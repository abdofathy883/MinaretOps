using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace Infrastructure.Services.MediaUploads
{
    public class MediaUploadService
    {
        private readonly IHttpContextAccessor contextAccessor;
        public MediaUploadService(IHttpContextAccessor httpContext)
        {
            contextAccessor = httpContext;
        }
        public record MediaUploadResult(string Url, string PhysicalPath);

        public async Task<MediaUploadResult> UploadImageWithPath(IFormFile image, string entityTitle)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var sanitizedEntityName = string.Join("_", entityTitle.Split(Path.GetInvalidFileNameChars()));
            var fileNameWithoutExt = $"{sanitizedEntityName}_The_Minaret_Agency";
            var webpFileName = fileNameWithoutExt + ".webp";
            var webpFilePath = Path.Combine(uploadsFolder, webpFileName);

            using var webPImage = await Image.LoadAsync(image.OpenReadStream());
            await webPImage.SaveAsync(webpFilePath, new WebpEncoder { Quality = 75 });

            var request = contextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var url = $"{baseUrl}/uploads/{webpFileName}";
            return new MediaUploadResult(url, webpFilePath);
        }

        public async Task<MediaUploadResult> UploadVideoWithPath(IFormFile video, string projectTitle)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var sanitizedProjectTitle = string.Join("_", projectTitle.Split(Path.GetInvalidFileNameChars()));
            var fileExtention = Path.GetExtension(video.FileName).ToLower();
            var videoName = $"{sanitizedProjectTitle}_Abdo Fathy{fileExtention}";
            var filePath = Path.Combine(uploadsFolder, videoName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await video.CopyToAsync(stream);

            var request = contextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var url = $"{baseUrl}/uploads/{videoName}";
            return new MediaUploadResult(url, filePath);
        }

        public Task DeleteIfExistsAsync(string physicalPath)
        {
            if (!string.IsNullOrWhiteSpace(physicalPath) && File.Exists(physicalPath))
                File.Delete(physicalPath);
            return Task.CompletedTask;
        }

        public async Task<string> UploadPDFFile(IFormFile pdfFile, string instructorName)
        {
            var fileExtention = Path.GetExtension(pdfFile.FileName).ToLower();
            if (fileExtention != ".pdf")
            {
                throw new InvalidOperationException("Only PDF files are allowed");
            }
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "pdfs");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var sanitizedInstructorName = string.Join("_", instructorName.Split(Path.GetInvalidFileNameChars()));

            var pdfName = $"{sanitizedInstructorName}_{Guid.NewGuid()}{fileExtention}";
            var filePath = Path.Combine(uploadsFolder, pdfName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await pdfFile.CopyToAsync(stream);
            }

            var request = contextAccessor.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return $"{baseUrl}/uploads/{pdfName}";

        }
    }
}
