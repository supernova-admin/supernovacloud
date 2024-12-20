using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CloudFileManager.Models
{
    public class FolderItem
    {
        [Key]
        public int Id { get; set; }
        public string FolderName { get; set; } = string.Empty;

        public int? ParentFolderId { get; set; }
        public FolderItem? ParentFolder { get; set; }

        public List<FolderItem> SubFolders { get; set; } = new();
        public List<FileItem> Files { get; set; } = new();
    }
}
