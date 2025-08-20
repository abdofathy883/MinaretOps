using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class MediaUploadsService
    {
        private readonly IHttpContextAccessor httpContext;
        public MediaUploadsService(IHttpContextAccessor http)
        {
            httpContext = http;
        }

        public async Task<string> UploadImage(IFormFile image, string imageName)
        {
            var originalExtension = Path.GetExtension(image.FileName).ToLower();
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var sanitizedImageName = string.Join("_", imageName.Split(Path.GetInvalidFileNameChars()));
            var fileNameWithoutExt = $"{sanitizedImageName}_Tahfez-Quran";
            var webpFileName = fileNameWithoutExt + ".webp";
            var webpFilePath = Path.Combine(uploadsFolder, webpFileName);
            using var webPImage = await Image.LoadAsync(image.OpenReadStream());
            await webPImage.SaveAsync(webpFilePath, new WebpEncoder { Quality = 75 });

            var request = httpContext.HttpContext.Request;
            var baseUrl = $"{request.Scheme}://{request.Host}";

            return $"{baseUrl}/uploads/{webpFileName}";
        }
    }
}
