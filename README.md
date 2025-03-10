# .NET Application - Azure Storage File Management

## Overview
This .NET application interacts with Azure Storage to manage file operations, including uploading, deleting, and listing files in an Azure Blob Storage container.

## Prerequisites
- .NET 6 or later installed
- An Azure Storage Account
- A Storage Container created in Azure Blob Storage

## Configuration
Update `appsettings.json` with your Azure Storage details:
```json
{
  "AzureStorage": {
    "AccountName": "your-storage-account",
    "AccountKey": "your-storage-key",
    "ContainerName": "files"
  }
}
```

## Installation
1. Clone the repository:
   ```sh
   git clone https://github.com/your-repo/your-project.git](https://github.com/ThiagoFragaK/AzureApi
   ```
2. Navigate to the project directory:
   ```sh
   cd your-project
   ```
3. Restore dependencies:
   ```sh
   dotnet restore
   ```

## File Operations
### Upload File
#### Endpoint
`POST /api/files/upload`
#### Request
- Header: `Content-Type: multipart/form-data`
- Body: File to be uploaded
#### Example
```sh
curl -X POST "https://your-api-url/api/files/upload" -F "file=@path-to-your-file"
```

### List Files
#### Endpoint
`GET /api/files`
#### Response
```json
[
  {
    "Uri": "https://yourstorageaccount.blob.core.windows.net/files/file1.jpg",
    "Name": "file1.jpg",
    "ContentType": "image/jpeg"
  },
  {
    "Uri": "https://yourstorageaccount.blob.core.windows.net/files/file2.png",
    "Name": "file2.png",
    "ContentType": "image/png"
  }
]
```
#### Example
```sh
curl -X GET "https://your-api-url/api/files"
```

### Delete File
#### Endpoint
`DELETE /api/files/{filename}`
#### Example
```sh
curl -X DELETE "https://your-api-url/api/files/file1.jpg"
```

## Code Implementation
### File Service
```csharp
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

public class BlobDTO
{
    public string Uri { get; set; }
    public string Name { get; set; }
    public string ContentType { get; set; }
}

public class FilesService
{
    private readonly string _storageAccount = "your-storage-account";
    private readonly string _key = "your-storage-key";
    private readonly BlobContainerClient _filesContainer;

    public FilesService()
    {
        var credentials = new StorageSharedKeyCredential(_storageAccount, _key);
        var blobUri = $"https://{_storageAccount}.blob.core.windows.net";
        var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credentials);
        _filesContainer = blobServiceClient.GetBlobContainerClient("files");
    }

    public async Task UploadFileAsync(Stream fileStream, string fileName)
    {
        var blobClient = _filesContainer.GetBlobClient(fileName);
        await blobClient.UploadAsync(fileStream, new BlobHttpHeaders { ContentType = "application/octet-stream" });
    }

    public async Task<IEnumerable<BlobDTO>> ListFilesAsync()
    {
        var files = new List<BlobDTO>();
        await foreach (BlobItem file in _filesContainer.GetBlobsAsync())
        {
            var blobClient = _filesContainer.GetBlobClient(file.Name);
            files.Add(new BlobDTO
            {
                Uri = blobClient.Uri.ToString(),
                Name = file.Name,
                ContentType = file.Properties.ContentType,
            });
        }
        return files;
    }

    public async Task DeleteFileAsync(string fileName)
    {
        var blobClient = _filesContainer.GetBlobClient(fileName);
        await blobClient.DeleteIfExistsAsync();
    }
}
```

## Deployment
To deploy this API to Azure:
1. Publish the project:
   ```sh
   dotnet publish -c Release -o ./publish
   ```
2. Deploy to Azure App Service using Azure CLI:
   ```sh
   az webapp up --name your-app-name --resource-group your-resource-group --runtime "DOTNET|6.0"
   ```

## Conclusion
This documentation provides a comprehensive guide to setting up and using the .NET API for managing files in Azure Storage. Let me know if you need further enhancements!
