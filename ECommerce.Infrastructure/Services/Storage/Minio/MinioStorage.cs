using Amazon.S3;
using Amazon.S3.Model;
using ECommerce.Application.Abstractions.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Services.Storage.Minio
{
    public class MinioStorage : StorageBase, IStorage
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public MinioStorage(IConfiguration configuration)
        {
            var endpoint = configuration["Minio:Endpoint"];
            var accessKey = configuration["Minio:AccessKey"];
            var secretKey = configuration["Minio:SecretKey"];
            _bucketName = configuration["Minio:BucketName"] ?? "ecommerce-images";

            var config = new AmazonS3Config
            {
                ServiceURL = endpoint,
                ForcePathStyle = true
            };

            _s3Client = new AmazonS3Client(accessKey, secretKey, config);
        }

        public async Task DeleteAsync(string pathOrContainerName, string fileName)
        {
            string folder = pathOrContainerName.Replace("\\", "/").Trim('/');
            string key = string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";

            try
            {
                await _s3Client.DeleteObjectAsync(new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                });
            }
            catch (Exception)
            {
                // Optionally log exception
            }
        }

        public List<string> GetFiles(string pathOrContainerName)
        {
            string folder = pathOrContainerName.Replace("\\", "/").Trim('/');
            if (!string.IsNullOrEmpty(folder) && !folder.EndsWith("/"))
            {
                folder += "/";
            }

            try
            {
                var request = new ListObjectsV2Request
                {
                    BucketName = _bucketName,
                    Prefix = folder
                };

                var response = _s3Client.ListObjectsV2Async(request).GetAwaiter().GetResult();

                return response.S3Objects
                    .Select(o => Path.GetFileName(o.Key))
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public bool HasFile(string pathOrContainerName, string fileName)
        {
            string folder = pathOrContainerName.Replace("\\", "/").Trim('/');
            string key = string.IsNullOrEmpty(folder) ? fileName : $"{folder}/{fileName}";

            try
            {
                _s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = key
                }).GetAwaiter().GetResult();
                return true;
            }
            catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<(string fileName, string pathOrContainerName)>> UploadAsync(string pathOrContainerName, IFormFileCollection files, string? productName = null)
        {
            // Ensure bucket exists
            bool bucketExists = await Amazon.S3.Util.AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _bucketName);
            if (!bucketExists)
            {
                await _s3Client.PutBucketAsync(new PutBucketRequest { BucketName = _bucketName });
            }

            var datas = new List<(string fileName, string pathOrContainerName)>();
            foreach (IFormFile file in files)
            {
                string baseName = file.FileName;
                if (productName != null)
                {
                    string randomId = Guid.NewGuid().ToString("N").Substring(0, 6);
                    string ext = Path.GetExtension(file.FileName);
                    baseName = $"{productName}-{randomId}{ext}";
                }

                // Check if file name exists and rename appropriately using base StorageBase.FileRenameAsync helper
                string fileNewName = await FileRenameAsync(pathOrContainerName, baseName, HasFile);

                string folder = pathOrContainerName.Replace("\\", "/").Trim('/');
                string key = string.IsNullOrEmpty(folder) ? fileNewName : $"{folder}/{fileNewName}";

                using (var stream = file.OpenReadStream())
                {
                    var putRequest = new PutObjectRequest
                    {
                        BucketName = _bucketName,
                        Key = key,
                        InputStream = stream,
                        ContentType = file.ContentType
                    };

                    putRequest.CannedACL = S3CannedACL.PublicRead;

                    await _s3Client.PutObjectAsync(putRequest);
                }

                // Path format saved in the DB should be compatible with the url routing
                datas.Add((fileNewName, key));
            }

            return datas;
        }
    }
}
