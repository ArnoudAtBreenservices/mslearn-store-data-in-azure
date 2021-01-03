using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FileUploader.Models
{
    public class BlobStorage : IStorage
    {
        private readonly AzureStorageConfig storageConfig;

        public BlobStorage(IOptions<AzureStorageConfig> storageConfig)
        {
            this.storageConfig = storageConfig.Value;
        }

        public Task Initialize()
        {
            // Create container if not exists.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConfig.ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(storageConfig.FileContainerName);

            return container.CreateIfNotExistsAsync();
        }

        public Task Save(Stream fileStream, string name)
        //save a single blob from a stream
        {
            //prepare container
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConfig.ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(storageConfig.FileContainerName);
            
            // Get blob reference from name.
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(name);
            //actual save
            return blockBlob.UploadFromStreamAsync(fileStream);
        }

        public async Task<IEnumerable<string>> GetNames()
        //Get all blob names.
        {
            List<string> names = new List<string>();

            //prepare container (again?)
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConfig.ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(storageConfig.FileContainerName);

            //prepare api call
            BlobContinuationToken continuationToken = null;
            BlobResultSegment resultSegment = null;

            //Call api, read names and append
            do
            {
                resultSegment = await container.ListBlobsSegmentedAsync(continuationToken);
                names.AddRange(resultSegment.Results.OfType<ICloudBlob>().Select(b => b.Name));
                continuationToken = resultSegment.ContinuationToken;
            } while (continuationToken != null);

            return names;
        }

        public Task<Stream> Load(string name)
        //Load a single blob as stream
        {
            //prepare... again. lol bad coding in an MSLearn project? inconceivable!
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConfig.ConnectionString);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference(storageConfig.FileContainerName);
            //actual load
            return container.GetBlobReference(name).OpenReadAsync();
        }
    }
}