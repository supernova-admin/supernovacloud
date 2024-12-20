using System;
using System.ComponentModel.DataAnnotations;

namespace CloudFileManager.Models
{
    public class FileItem
    {
        [Key]
        public int Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string PhysicalPath { get; set; } = string.Empty;
        public long Size { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string Extension { get; set; } = string.Empty;
        
        // Klasör ilişkisi
        public int? FolderId { get; set; }
        public FolderItem? Folder { get; set; }
    }
}
