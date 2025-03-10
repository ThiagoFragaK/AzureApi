using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureApi.DTOs;
using System.Net.Mime;
using System.Reflection.Metadata;
using static System.Reflection.Metadata.BlobBuilder;

namespace AzureApi.Services
{
    public class FilesService
    {
        private readonly string _storageAccount = "devstoreaccount1";
        private readonly string _key = "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==";
        private readonly BlobContainerClient _filesContainer;

        public FilesService()
        {
            var credentials = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credentials);
            _filesContainer = blobServiceClient.GetBlobContainerClient("files");
        }

        public async Task<List<BlobDTO>> ListAsync()
        {
            List<BlobDTO> files = new List<BlobDTO>();
            await foreach(var file in _filesContainer.GetBlobsAsync())
            {
                string uri = _filesContainer.Uri.ToString();
                var name = file.Name;
                var fullUri = $"{uri}/{name}";

                files.Add(new BlobDTO
                {
                    Uri = fullUri,
                    Name = name,
                    ContentType = file.Properties.ContentType,
                });
            }

            return files;
        }

        public async Task<BlobResponseDTO> UploadAsync(IFormFile blob)
        {
            BlobClient client = _filesContainer.GetBlobClient(blob.FileName);
            await using (Stream? data = blob.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            return new BlobResponseDTO {
                Status = $"File {blob.FileName} Uploaded suceessfully.",
                Error = false,
                Blob = new BlobDTO
                {
                    Uri = client.Uri.AbsoluteUri,
                    Name = client.Name,
                }
            };
        }

        public async Task<BlobDTO?> DownloadAsync(string blobFilename)
        {
            BlobClient client = _filesContainer.GetBlobClient(blobFilename);
            if(await client.ExistsAsync())
            {
                Stream blobContent = await client.OpenReadAsync();
                var content = await client.DownloadContentAsync();

                return new BlobDTO
                {
                    Content = blobContent,
                    Name = blobFilename,
                    ContentType = content.Value.Details.ContentType,
                };
            }
            return null;
        }

        public async Task<BlobResponseDTO> DeleteAsync(string blobFilename)
        {
            BlobClient client = _filesContainer.GetBlobClient(blobFilename);
            await client.DeleteAsync();

            return new BlobResponseDTO
            {
                Error = false,
                Status = $"File {blobFilename} Uploaded suceessfully.",
            };
        }
    }
}
