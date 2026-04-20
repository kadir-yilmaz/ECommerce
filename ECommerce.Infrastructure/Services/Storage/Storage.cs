using Microsoft.AspNetCore.Hosting;

namespace ECommerce.Infrastructure.Services.Storage
{
    public abstract class StorageBase
    {
        protected delegate bool HasFile(string pathOrContainerName, string fileName);
        protected async Task<string> FileRenameAsync(string pathOrContainerName, string fileName, HasFile hasFileMethod, bool hasContainer = false)
        {
            string extension = Path.GetExtension(fileName);
            string oldFileName = Path.GetFileNameWithoutExtension(fileName);
            string regulatedFileName = NameOperation.CharacterRegulatory(oldFileName);

            // First attempt with the regulated name
            string newFileName = $"{regulatedFileName}{extension}";

            if (hasFileMethod(pathOrContainerName, newFileName))
            {
                // If it exists, add a suffix (e.g., -1, -2 or timestamp)
                int suffix = 1;
                while (hasFileMethod(pathOrContainerName, $"{regulatedFileName}-{suffix}{extension}"))
                {
                    suffix++;
                }
                return $"{regulatedFileName}-{suffix}{extension}";
            }
            
            return newFileName;
        }
    }

    public static class NameOperation
    {
        public static string CharacterRegulatory(string name)
        {
            return name.ToLower()
                .Replace("\"", "")
                .Replace("!", "")
                .Replace("'", "")
                .Replace("^", "")
                .Replace("+", "")
                .Replace("%", "")
                .Replace("&", "")
                .Replace("/", "")
                .Replace("(", "")
                .Replace(")", "")
                .Replace("=", "")
                .Replace("?", "")
                .Replace("_", "-")
                .Replace(" ", "-")
                .Replace("@", "")
                .Replace("€", "")
                .Replace("¨", "")
                .Replace("~", "")
                .Replace(",", "")
                .Replace(";", "")
                .Replace(":", "")
                .Replace(".", "-")
                .Replace("Ö", "o")
                .Replace("ö", "o")
                .Replace("Ü", "u")
                .Replace("ü", "u")
                .Replace("ı", "i")
                .Replace("İ", "i")
                .Replace("ğ", "g")
                .Replace("Ğ", "g")
                .Replace("æ", "")
                .Replace("ß", "")
                .Replace("â", "a")
                .Replace("î", "i")
                .Replace("ş", "s")
                .Replace("Ş", "s")
                .Replace("Ç", "c")
                .Replace("ç", "c")
                .Replace("<", "")
                .Replace(">", "")
                .Replace("|", "");
        }
    }
}
