using System.Collections.Generic;
using System.Threading.Tasks;
using CloudFileManager.Models;
using CloudFileManager.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace CloudFileManager.Pages
{
    public class IndexModel : PageModel
    {
        private readonly FileService _fileService;
        public List<FileItem> Files { get; set; } = new();
        public List<FolderItem> Folders { get; set; } = new();
        public string SearchQuery { get; set; } = string.Empty;
        public string ExtensionFilter { get; set; } = string.Empty;
        public int? CurrentFolderId { get; set; }
        
        // Yeni özellikler
        [BindProperty]
        public string NewFolderName { get; set; } = string.Empty;

        [BindProperty]
        public IFormFile[] FilesToUpload { get; set; } = System.Array.Empty<IFormFile>();

        public string SortOption { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public List<string> AvailableExtensions { get; set; } = new();

        public IndexModel(FileService fileService)
        {
            _fileService = fileService;
        }

        public async Task OnGetAsync(int? folderId, string search, string ext, string sort)
        {
            CurrentFolderId = folderId;
            SearchQuery = search ?? string.Empty;
            ExtensionFilter = ext ?? string.Empty;
            SortOption = sort ?? string.Empty;

            Files = await _fileService.GetFilesAsync(folderId, SearchQuery, ExtensionFilter);
            Folders = await _fileService.GetFoldersAsync(folderId);

            // Get available extensions
            AvailableExtensions = Files.Select(f => f.Extension).Distinct().ToList();

            // Sıralama işlemi
            if (!string.IsNullOrEmpty(SortOption))
            {
                if (SortOption == "name")
                {
                    Files = Files.OrderBy(f => f.FileName).ToList();
                }
                else if (SortOption == "date")
                {
                    Files = Files.OrderByDescending(f => f.UpdatedAt).ToList();
                }
            }
        }

        public string GetFriendlyName(string extension)
        {
            return extension.ToLower() switch
            {
                ".png" => ".png Fotoğraf",
                ".jpg" => ".jpg Fotoğraf",
                ".jpeg" => ".jpeg Fotoğraf",
                ".pdf" => ".pdf Belge",
                ".txt" => ".txt Metin",
                ".doc" => ".doc Word Belgesi",
                ".docx" => ".docx Word Belgesi",
                ".xls" => ".xls Excel Belgesi",
                ".xlsx" => ".xlsx Excel Belgesi",
                ".ppt" => ".ppt PowerPoint Sunumu",
                ".pptx" => ".pptx PowerPoint Sunumu",
                ".mp3" => ".mp3 Ses Dosyası",
                ".wav" => ".wav Ses Dosyası",
                ".mp4" => ".mp4 Video Dosyası",
                ".avi" => ".avi Video Dosyası",
                ".mkv" => ".mkv Video Dosyası",
                ".zip" => ".zip Sıkıştırılmış Dosya",
                _ => extension
            };
        }

        public string GetIconForFileType(string extension)
        {
            return extension.ToLower() switch
            {
                ".png" => "image-icon.png",
                ".jpg" => "image-icon.png",
                ".jpeg" => "image-icon.png",
                ".pdf" => "pdf-icon.png",
                ".txt" => "text-icon.png",
                ".doc" => "word-icon.png",
                ".docx" => "word-icon.png",
                ".xls" => "excel-icon.png",
                ".xlsx" => "excel-icon.png",
                ".ppt" => "ppt-icon.png",
                ".pptx" => "ppt-icon.png",
                ".mp3" => "audio-icon.png",
                ".wav" => "audio-icon.png",
                ".mp4" => "video-icon.png",
                ".avi" => "video-icon.png",
                ".mkv" => "video-icon.png",
                ".zip" => "zip-icon.png",
                _ => "file-icon.png"
            };
        }

        public async Task<IActionResult> OnPostCreateFolderAsync(int? folderId)
        {
            if (!string.IsNullOrEmpty(NewFolderName))
            {
                await _fileService.CreateFolderAsync(NewFolderName, folderId);
                Message = "Klasör başarıyla oluşturuldu.";
            }
            else
            {
                Message = "Klasör adı boş olamaz.";
            }

            return RedirectToPage(new { folderId = folderId, search = SearchQuery, ext = ExtensionFilter, sort=SortOption });
        }

        public async Task<IActionResult> OnPostUploadAsync(int? folderId)
        {
            if (FilesToUpload != null && FilesToUpload.Length > 0)
            {
                foreach(var file in FilesToUpload)
                {
                    using var stream = file.OpenReadStream();
                    await _fileService.UploadFileAsync(file.FileName, stream, folderId);
                }
                Message = "Dosyalar başarıyla yüklendi.";
            }
            else
            {
                Message = "Yüklenecek dosya seçilmedi.";
            }

            return RedirectToPage(new { folderId = folderId, search = SearchQuery, ext = ExtensionFilter, sort=SortOption });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id, int? folderId)
        {
            await _fileService.DeleteFileAsync(id);
            Message = "Dosya başarıyla silindi.";
            return RedirectToPage(new { folderId = folderId, search = SearchQuery, ext = ExtensionFilter, sort = SortOption });
        }
    }
}
