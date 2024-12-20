using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloudFileManager.Services;
using CloudFileManager.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CloudFileManager.Pages
{
    public class PreviewModel : PageModel
    {
        private readonly FileService _fileService;
        public FileItem? FileItem { get; set; }
        public bool CanPreviewFile => FileItem != null && FileTypeHelper.CanPreview(FileItem.Extension);

        // Örnek ek özellikler
        public bool IsFavorite { get; set; } = false;
        public List<string> Tags { get; set; } = new List<string> { "Proje", "Önemli" };
        public List<FileItem> PreviousVersions { get; set; } = new List<FileItem>();
        // Yorumlar, paylaşım bilgileri, vb. gelecekte eklenebilir.

        public PreviewModel(FileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            FileItem = await _fileService.GetFileByIdAsync(id);
            if (FileItem == null)
                return NotFound();

            // Örnek: Önceki versiyonları buradan çekebilirsiniz
            // PreviousVersions = await _fileService.GetPreviousVersionsAsync(FileItem.Id);

            // Favori bilgisi, etiketler veya paylaşım durumunu DB’den çekebilirsiniz.
            // Bu örnekte statik değerler kullandık.

            return Page();
        }

        // Gelecekte yorum ekleme, paylaşılan link oluşturma, favori ekleme gibi POST işlemler için handler metodlar ekleyebilirsiniz.
        // Örneğin:
        // public async Task<IActionResult> OnPostAddCommentAsync(int id, string comment) { ... }
        // public async Task<IActionResult> OnPostToggleFavoriteAsync(int id) { ... }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            await _fileService.DeleteFileAsync(id);
            return RedirectToPage("/Index");
        }
    }
}
