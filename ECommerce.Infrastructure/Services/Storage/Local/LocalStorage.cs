using ECommerce.Application.Abstractions.Storage.Local;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ECommerce.Infrastructure.Services.Storage.Local
{
    public class LocalStorage : StorageBase, ILocalStorage
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public LocalStorage(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task DeleteAsync(string path, string fileName)
        {
            string webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
            string filePath = Path.Combine(webRootPath, path, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public List<string> GetFiles(string path)
        {
            string webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
            DirectoryInfo directory = new(Path.Combine(webRootPath, path));
            return directory.GetFiles().Select(f => f.Name).ToList();
        }

        public bool HasFile(string path, string fileName)
        {
            string webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
            return File.Exists(Path.Combine(webRootPath, path, fileName));
        }
        async Task<bool> CopyFileAsync(string path, IFormFile file)
        {
            try
            {
                await using FileStream fileStream = new(path, FileMode.Create, FileAccess.Write, FileShare.None, 1024 * 1024, useAsync: false);

                await file.CopyToAsync(fileStream);
                await fileStream.FlushAsync();
                return true;
            }
            catch (Exception ex)
            {
                //todo log!
                throw ex;
            }
        }
        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string path, IFormFileCollection files, string? productName = null)
        {
            string webRootPath = _webHostEnvironment.WebRootPath ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
            string uploadPath = Path.Combine(webRootPath, path);
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            List<(string fileName, string path)> datas = new();
            foreach (IFormFile file in files)
            {
                string baseName = file.FileName;
                if (productName != null)
                {
                    string randomId = Guid.NewGuid().ToString("N").Substring(0, 6);
                    string ext = Path.GetExtension(file.FileName);
                    baseName = $"{productName}-{randomId}{ext}";
                }
                string fileNewName = await FileRenameAsync(path, baseName, HasFile);

                await CopyFileAsync($"{uploadPath}\\{fileNewName}", file);
                datas.Add((fileNewName, $"{path}\\{fileNewName}"));
            }

            return datas;
        }
    }
}
