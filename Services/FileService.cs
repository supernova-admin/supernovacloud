using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CloudFileManager.Data;
using CloudFileManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace CloudFileManager.Services
{
    public class FileService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        
        public FileService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
            EnsureDatabaseCreated();
        }

        private void EnsureDatabaseCreated()
        {
            using (var connection = _context.Database.GetDbConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Files (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            Name TEXT NOT NULL,
                            FolderId INTEGER,
                            Extension TEXT,
                            Size INTEGER,
                            CreatedAt DATETIME
                        )";
                    command.ExecuteNonQuery();
                }
            }
        }

        public async Task<FileItem> UploadFileAsync(string originalFileName, Stream fileStream, int? folderId = null)
        {
            var uploadsPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);

            var extension = Path.GetExtension(originalFileName);
            var newFileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(uploadsPath, newFileName);

            using (var file = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(file);
            }

            var fileInfo = new FileInfo(filePath);

            var fileItem = new FileItem
            {
                FileName = originalFileName,
                PhysicalPath = newFileName,
                Size = fileInfo.Length,
                Extension = extension,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                FolderId = folderId
            };

            _context.Files.Add(fileItem);
            await _context.SaveChangesAsync();

            return fileItem;
        }

        public async Task<List<FileItem>> GetFilesAsync(int? folderId = null, string? search = null, string? extensionFilter = null)
        {
            var query = _context.Files.AsQueryable();

            if (folderId.HasValue)
                query = query.Where(f => f.FolderId == folderId.Value);

            if (!string.IsNullOrEmpty(search))
                query = query.Where(f => f.FileName.Contains(search));

            if (!string.IsNullOrEmpty(extensionFilter))
                query = query.Where(f => f.Extension.ToLower() == extensionFilter.ToLower());

            return await query.OrderByDescending(f => f.UpdatedAt).ToListAsync();
        }

        public async Task<FileItem?> GetFileByIdAsync(int id)
        {
            return await _context.Files.FindAsync(id);
        }

        public async Task DeleteFileAsync(int id)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null) return;

            var filePath = Path.Combine(_env.WebRootPath, "uploads", file.PhysicalPath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            
            _context.Files.Remove(file);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateFileNameAsync(int id, string newName)
        {
            var file = await _context.Files.FindAsync(id);
            if (file == null) return;
            file.FileName = newName;
            file.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        public async Task<List<FolderItem>> GetFoldersAsync(int? parentFolderId = null)
        {
            return await _context.Folders
                .Where(f => f.ParentFolderId == parentFolderId)
                .OrderBy(f => f.FolderName)
                .ToListAsync();
        }

        public async Task<FolderItem?> GetFolderByIdAsync(int id)
        {
            return await _context.Folders.Include(f => f.SubFolders).Include(f => f.Files)
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<FolderItem> CreateFolderAsync(string folderName, int? parentFolderId = null)
        {
            var folder = new FolderItem
            {
                FolderName = folderName,
                ParentFolderId = parentFolderId
            };
            _context.Folders.Add(folder);
            await _context.SaveChangesAsync();
            return folder;
        }
    }
}
