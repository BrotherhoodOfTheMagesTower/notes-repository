using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.WindowsAzure.Storage;

namespace NotesRepository.Services.Azure
{
    public class AzureStorageHelper
    {
        IConfiguration configuration;
        string baseUrl = "";
        private readonly string cloudStorageConnectionString;
        CloudStorageAccount cloudStorageAccount;

        public AzureStorageHelper(IConfiguration _configuration)
        {
            configuration = _configuration;
            baseUrl = configuration["StorageBaseUrl"];
            cloudStorageAccount = CloudStorageAccount.Parse(configuration["StorageConnectionString"]);
        }
        
        public AzureStorageHelper(string storageBaseUrl, string cloudStorageConnectionString)
        {
            baseUrl = storageBaseUrl;
            cloudStorageAccount = CloudStorageAccount.Parse(cloudStorageConnectionString);
            this.cloudStorageConnectionString = cloudStorageConnectionString;
        }

        public async Task<List<string>> GetFileUrls(string containerName)
        {
            var files = new List<string>();
            var container = OpenContainer(containerName);
            if (container == null) return files;

            try
            {
                // get the list
                await foreach (BlobItem item in container.GetBlobsAsync())
                {
                    var Url = container.Uri.ToString() + "/" + item.Name.ToString();
                    files.Add(Url);
                }
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }

            return files;
        }

        public async Task<string> UploadFileToAzureAsync(IBrowserFile file, string containerName, string destFileName, bool overWrite = false)
        {
            var container = OpenContainer(containerName);
            if (container == null) return "";
            try
            {
                BlobUploadOptions options = new BlobUploadOptions
                {
                    TransferOptions = new StorageTransferOptions
                    {
                        // Set the maximum length of a transfer to 50MB.
                        // If the file is bigger than 50MB it will be sent in 50MB chunks.
                        MaximumTransferSize = 50 * 1024 * 1024
                    }
                };

                destFileName = destFileName.Replace(' ', '_');
                BlobClient blob = container.GetBlobClient(destFileName);

                if (overWrite == true)
                {
                    blob.DeleteIfExists();
                }

                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(destFileName);
                cloudBlockBlob.Properties.ContentType = file.ContentType;
                long maxAllowedSizeInBytes = 4194304;
                await cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream(maxAllowedSizeInBytes));

                // return the url to the blob
                return $"{baseUrl}{containerName}\\{ destFileName}";
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return "";
            }

        }
        
        public async Task<string> UploadFileFromPathToAzureAsync(string filePath, string containerName, string destFileName, string storageConnectionString, bool overWrite = false)
        {
            var container = OpenContainerWithConnectionString(containerName, storageConnectionString);
            if (container == null) return "";
            try
            {
                BlobUploadOptions options = new BlobUploadOptions
                {
                    TransferOptions = new StorageTransferOptions
                    {
                        // Set the maximum length of a transfer to 50MB.
                        // If the file is bigger than 50MB it will be sent in 50MB chunks.
                        MaximumTransferSize = 50 * 1024 * 1024
                    }
                };

                destFileName = destFileName.Replace(' ', '_');
                BlobClient blob = container.GetBlobClient(destFileName);

                if (overWrite == true)
                {
                    blob.DeleteIfExists();
                }

                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(destFileName);
                //cloudBlockBlob.Properties.ContentType = file.ContentType;
                long maxAllowedSizeInBytes = 4194304;
                await cloudBlockBlob.UploadFromStreamAsync(new FileStream(filePath, FileMode.Open));

                // return the url to the blob
                return $"{baseUrl}{containerName}\\{ destFileName}";
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return "";
            }

        }

        public async Task<string> DownloadFile(string containerName, string sourceFilename, string destFileName)
        {
            var container = OpenContainer(containerName);
            if (container == null) return "";

            try
            {
                BlobClient blob = container.GetBlobClient(sourceFilename);

                BlobDownloadInfo download = await blob.DownloadAsync();

                using (FileStream downloadFileStream = File.OpenWrite(destFileName))
                {
                    await download.Content.CopyToAsync(downloadFileStream);
                    downloadFileStream.Close();
                }
                return "OK";
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return "";
            }
        }

        BlobContainerClient OpenContainer(string containerName)
        {
            try
            {
                string? connectionString = configuration?["StorageConnectionString"];

                // Create a BlobServiceClient object which will be used to create a container client
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString ?? cloudStorageConnectionString);

                // Create the container and return a container client object
                return blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }
        
        BlobContainerClient OpenContainerWithConnectionString(string containerName, string storageConnectionString)
        {
            try
            {
                // Create a BlobServiceClient object which will be used to create a container client
                BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);

                // Create the container and return a container client object
                return blobServiceClient.GetBlobContainerClient(containerName);
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                return null;
            }
        }

        public async Task DeleteImageFromAzure(string imageName, string containerName)
        {
            var container = OpenContainer(containerName);
            await container.DeleteBlobAsync(imageName);
        }

        public void DeleteImageFromAzureNotAsync(string imageName, string containerName)
        {
            var container = OpenContainer(containerName);
            container.DeleteBlobAsync(imageName);
        }
    }
}
