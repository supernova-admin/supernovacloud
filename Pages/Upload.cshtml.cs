using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloudFileManager.Services;
using Microsoft.AspNetCore.Http;

namespace CloudFileManager.Pages
{
    public class UploadModel : PageModel
    {
        private readonly FileService _fileService;

        [BindProperty]
        public IFormFile[] Files { get; set; }

        [BindProperty]
        public int? FolderId { get; set; }

        public UploadModel(FileService fileService)
        {
            _fileService = fileService;
        }

        public void OnGet(int? folderId)
        {
            FolderId = folderId;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Files != null && Files.Length > 0)
            {
                foreach (var f in Files)
                {
                    using var stream = f.OpenReadStream();
                    await _fileService.UploadFileAsync(f.FileName, stream, FolderId);
                }
            }
            return RedirectToPage("Index", new { folderId = FolderId });
        }
    }
}
