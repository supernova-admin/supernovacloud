using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudFileManager.Services;
using CloudFileManager.Models;
using Microsoft.AspNetCore.Mvc;

namespace CloudFileManager.Pages
{
    public class FoldersModel : PageModel
    {
        private readonly FileService _fileService;
        public List<FolderItem> Folders { get; set; } = new();
        public int? CurrentFolderId { get; set; }
        [BindProperty]
        public string NewFolderName { get; set; }

        public FoldersModel(FileService fileService)
        {
            _fileService = fileService;
        }

        public async Task OnGetAsync(int? folderId)
        {
            CurrentFolderId = folderId;
            Folders = await _fileService.GetFoldersAsync(folderId);
        }

        public async Task<IActionResult> OnPostAsync(int? folderId)
        {
            if (!string.IsNullOrEmpty(NewFolderName))
            {
                await _fileService.CreateFolderAsync(NewFolderName, folderId);
            }
            return RedirectToPage("Folders", new { folderId });
        }
    }
}
