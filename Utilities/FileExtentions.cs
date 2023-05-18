using Microsoft.AspNetCore.Hosting;
using WebFrontToBack.Areas.Admin.ViewModels;

namespace WebFrontToBack.Utilities
{
    public static class FileExtentions
    {
        public static bool CheckContentType(this IFormFile file,string ContentType)
        {
            return file.ContentType.Contains(ContentType);
        }

        public static bool CheckFileSize(this IFormFile file, Double Size)
        {
            return file.Length / 1024 < Size;
        }

        public async static Task<string> SaveAsync(this IFormFile file,string root)
        {
            string fileName = Guid.NewGuid().ToString() + file.FileName;
           
            string resaultRoot = Path.Combine(root, "assets", "img", fileName);
            using (FileStream fileStream = new FileStream(resaultRoot, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return fileName;
        }
    }
}
