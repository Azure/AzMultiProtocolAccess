using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.DataLake;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace AzureStorageMPAdemo
{
    class AzureBlob : AzureStorage
    {
        private BlobServiceClient? _blobServiceClient;

        public AzureBlob() : base(API_ENDPOINT.BLOB) { }


        /// <summary>
        /// List Blobs flat on a given page size
        /// </summary>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task ListBlobsFlatListing(int pageSize)
        {

            BlobContainerClient containerClient = this.InitalizeClient().GetBlobContainerClient(containerName);

            var resultSegment = containerClient.GetBlobsAsync()
                                               .AsPages(default, pageSize);

            // Enumerate the blobs returned for each page.
            await foreach (Page<BlobItem> blobPage in resultSegment)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {
                    Console.WriteLine(blobItem.Name);
                }

                Console.WriteLine();
            }

        }

        /// <summary>
        /// Downloads a container to a local path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task DownloadDirectoryAsync(string path)
        {
            DownloadDirectoryPath = path;

            BlobContainerClient containerClient = this.InitalizeClient().GetBlobContainerClient(containerName);
            var blobPages = containerClient.GetBlobsAsync().AsPages();

            await foreach (Page<BlobItem> blobPage in blobPages)
            {
                foreach (BlobItem blobItem in blobPage.Values)
                {

                    BlobClient blobClient = containerClient.GetBlobClient(blobItem.Name);
                    
                    //Console.WriteLine("Download from Blob storage as blob:\n\t {0}\n", blobClient.Uri);
                    Console.Write(".");

                    using var file = File.Create(Path.Combine(DownloadDirectoryPath, blobItem.Name));
                    await blobClient.DownloadToAsync(file);

                }

            }

            Console.WriteLine();

        }

        /// <summary>
        /// Uploads all directories and files from the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public async Task<AzureBlob> UploadDirectoryAsync(string path)
        {
            UploadDirectoryPath = path;

            BlobContainerClient containerClient = await this.InitalizeClient().CreateBlobContainerAsync(containerName);

            //Uploading 
            foreach (String file in Directory.GetFiles(UploadDirectoryPath))
            {

                BlobClient blobClient = containerClient.GetBlobClient(file.Replace(Directory.GetCurrentDirectory(), ""));

                //Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);
                Console.Write(".");
                await blobClient.UploadAsync(file, true);

            }

            Console.WriteLine();

            return this;
        }


        private BlobServiceClient InitalizeClient()
        {
             
            return _blobServiceClient = _blobServiceClient ?? new BlobServiceClient(storageAccountUri, AzureCredentials);
 
        }

    }
}
