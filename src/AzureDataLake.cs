using Azure.Identity;
using Azure.Storage.Files.DataLake.Models;
using Azure.Storage.Files.DataLake;
using Azure;
using Azure.Storage.Blobs;
using System.IO.Enumeration;
using System.IO;
using Azure.Storage.Files.DataLake.Specialized;

namespace AzureStorageMPAdemo
{
    class AzureDataLake : AzureStorage
    {
        private DataLakeServiceClient? _serviceClient;
        private DataLakeFileSystemClient? _fileSystemClient;

        public AzureDataLake() : base(API_ENDPOINT.DFS) { }

        public async Task ListFilesInDirectoryRecursivelyAsync()
        {
            _serviceClient = new DataLakeServiceClient(storageAccountUri, AzureCredentials);

            IAsyncEnumerator<PathItem> enumerator = _serviceClient.GetFileSystemClient(containerName).GetPathsAsync(recursive: true).GetAsyncEnumerator();

            await enumerator.MoveNextAsync();

            PathItem item = enumerator.Current;

            while (item != null)
            {
                if (item.IsDirectory == false)
                {
                    Console.WriteLine(item.Name);
                }


                if (!await enumerator.MoveNextAsync())
                {
                    break;
                }

                item = enumerator.Current;
            }

            Console.WriteLine();

        }



        /// <summary>
        /// Uploads all files in a given path to the container
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task UploadDirectoryAsync(string path)
        {

            UploadDirectoryPath = path;

            _serviceClient = new DataLakeServiceClient(storageAccountUri, AzureCredentials);
            _fileSystemClient = await _serviceClient.CreateFileSystemAsync(containerName);

            DataLakeDirectoryClient directoryClient = _fileSystemClient.GetDirectoryClient("");

            foreach (string file in Directory.GetFiles(UploadDirectoryPath))
            {

                DataLakeFileClient fileClient = await directoryClient.CreateFileAsync(Path.GetFileName(file));

                FileStream fileStream = File.OpenRead(file);

                long fileSize = fileStream.Length;

                await fileClient.AppendAsync(fileStream, offset: 0);

                await fileClient.FlushAsync(position: fileSize);

                //Console.WriteLine("Uploading to Blob storage using DFS:\n\t {0}\n", fileClient.Uri);
                Console.Write(".");

            }

            Console.WriteLine();

        }


        /// <summary>
        /// Download all directories and files recursively to the given path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>        
        public async Task DownloadDirectoryRecursivelyAsync(string path)
        {

            DownloadDirectoryPath = path;

            _serviceClient = new DataLakeServiceClient(storageAccountUri, AzureCredentials);
            _fileSystemClient = _serviceClient.GetFileSystemClient(containerName);

            IAsyncEnumerator<PathItem> enumerator = _fileSystemClient.GetPathsAsync(recursive: true).GetAsyncEnumerator();

            await enumerator.MoveNextAsync();

            PathItem item = enumerator.Current;

            while (item != null)
            {

                var localitem = Path.Combine(DownloadDirectoryPath, item.Name);

                if (item.IsDirectory == true)
                {
                    Directory.CreateDirectory(localitem);

                }
                else
                {

                    DataLakeFileClient fileClient = _fileSystemClient.GetFileClient(item.Name);

                    Response<FileDownloadInfo> downloadResponse = await fileClient.ReadAsync();

                    BinaryReader reader = new BinaryReader(downloadResponse.Value.Content);

                    FileStream fileStream = File.OpenWrite(localitem);

                    int bufferSize = 4096;

                    byte[] buffer = new byte[bufferSize];

                    int count;

                    while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        fileStream.Write(buffer, 0, count);
                    }

                    await fileStream.FlushAsync();

                    fileStream.Close();

                }

                Console.Write(".");

                if (!await enumerator.MoveNextAsync())
                {
                    break;
                }

                item = enumerator.Current;

            }

            Console.WriteLine();

        }

    }
 
}
