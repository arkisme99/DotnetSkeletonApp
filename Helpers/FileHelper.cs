using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetSkeletonApp.Helpers
{
    public static class FileHelper
    {
        public static async Task<string?> UploadFile(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) return null;

            // Tentukan path folder
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", folderName);

            // Buat folder jika belum ada
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // Buat nama file unik (mencegah overwrite)
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName; // Simpan nama ini ke Database
        }

        public static void DeleteFile(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", folderName, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}