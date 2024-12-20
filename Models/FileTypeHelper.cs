using System;
using System.IO;

namespace CloudFileManager.Models
{
    public static class FileTypeHelper
    {
        public static bool CanPreview(string extension)
        {
            var ext = extension.ToLower();
            return ext == ".pdf" || ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".txt";
        }

        public static string GetContentType(string extension)
        {
            var ext = extension.ToLower();
            return ext switch
            {
                ".pdf" => "application/pdf",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}
