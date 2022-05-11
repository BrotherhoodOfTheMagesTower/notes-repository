using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.WindowsAzure.Storage;

namespace NotesRepository.Services
{
    public class AzureStorageHelper
    {
        IConfiguration configuration;
        string baseUrl = "";
        CloudStorageAccount cloudStorageAccount;

        public AzureStorageHelper(IConfiguration _configuration)
        {
            configuration = _configuration;
            baseUrl = configuration["StorageBaseUrl"];
            cloudStorageAccount = CloudStorageAccount.Parse(configuration["StorageConnectionString"]);
        }

        public async Task<List<string>> GetFileUrls(string containerName)
        {
            var files = new List<string>();
            var container = OpenContianer(containerName);
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
            var container = OpenContianer(containerName);
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

                BlobClient blob = container.GetBlobClient(destFileName);

                if (overWrite == true)
                {
                    blob.DeleteIfExists();
                }

                var cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                var cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);
                var cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(file.Name);
                cloudBlockBlob.Properties.ContentType = file.ContentType;
                await cloudBlockBlob.UploadFromStreamAsync(file.OpenReadStream());

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
            var container = OpenContianer(containerName);
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

        BlobContainerClient OpenContianer(string containerName)
        {
            try
            {
                string setting = configuration["StorageConnectionString"];

                // Create a BlobServiceClient object which will be used to create a container client
                BlobServiceClient blobServiceClient = new BlobServiceClient(setting);

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
            var container = OpenContianer(containerName);
            await container.DeleteBlobAsync(imageName);
        }
    }
}
