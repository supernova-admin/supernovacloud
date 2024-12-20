using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudFileManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System;
using CloudFileManager.Models;

namespace CloudFileManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly FileService _fileService;

        public FilesController(FileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile[] files, [FromQuery] int? folderId)
        {
            var uploadedFiles = new List<FileItem>();
            foreach (var file in files)
            {
                using var stream = file.OpenReadStream();
                var uploaded = await _fileService.UploadFileAsync(file.FileName, stream, folderId);
                uploadedFiles.Add(uploaded);
            }

            return Ok(uploadedFiles);
        }

        [HttpGet("list")]
        public async Task<IActionResult> List([FromQuery] int? folderId, [FromQuery] string? search, [FromQuery] string? ext)
        {
            var files = await _fileService.GetFilesAsync(folderId, search, ext);
            return Ok(files);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _fileService.DeleteFileAsync(id);
            return Ok(new { message = "File deleted successfully." });
        }

        [HttpPut("{id}/rename")]
        public async Task<IActionResult> Rename(int id, [FromBody] string newName)
        {
            await _fileService.UpdateFileNameAsync(id, newName);
            return Ok(new { message = "File renamed successfully." });
        }
    }
}
