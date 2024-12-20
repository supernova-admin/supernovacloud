using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CloudFileManager.Services;
using CloudFileManager.Models;

namespace CloudFileManager.Pages
{
    public class EditModel : PageModel
    {
        private readonly FileService _fileService;
        [BindProperty]
        public FileItem FileItem { get; set; }

        public EditModel(FileService fileService)
        {
            _fileService = fileService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var file = await _fileService.GetFileByIdAsync(id);
            if (file == null)
                return NotFound();
            FileItem = file;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _fileService.UpdateFileNameAsync(FileItem.Id, FileItem.FileName);
            return RedirectToPage("Index", new { folderId = FileItem.FolderId });
        }
    }
}
