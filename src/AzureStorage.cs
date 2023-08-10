using Azure.Core;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Azure;
using System.Text;
using System.Threading.Tasks;
using Azure.Identity;

namespace AzureStorageMPAdemo
{
    class AzureStorage
    {

        protected enum API_ENDPOINT
        {
            BLOB,
            DFS
        }

        private API_ENDPOINT _apiEndpoint;
        private string? _uploadDirectoryPath;
        private string? _downloadDirectoryPath;
        private TokenCredential? _azureCredentials;

        protected string storageAccountName = String.Empty;
        protected Uri? storageAccountUri;
        protected string containerName = String.Empty;

        protected string? UploadDirectoryPath
        {
            get => _uploadDirectoryPath;
            set
            {
                if (!Directory.Exists(value))
                {
                    throw new DirectoryNotFoundException();
                }
                _uploadDirectoryPath = value;
            }
        }
        protected string? DownloadDirectoryPath
        {
            get => _downloadDirectoryPath;
            set
            {
               
                if (!Directory.Exists(value))
                {
#pragma warning disable CS8604 // Possible null reference argument.
                    Directory.CreateDirectory(value);
#pragma warning restore CS8604 // Possible null reference argument.
                }

                _downloadDirectoryPath = value;
            }
        }

        protected TokenCredential? AzureCredentials
        {
            get
            {
                if (_azureCredentials == null)
                {
                    _azureCredentials = new InteractiveBrowserCredential();
                }

                return _azureCredentials;
            }
            set => _azureCredentials = value;
        }
 
        protected AzureStorage(API_ENDPOINT api) {
            _apiEndpoint = api switch
            {
                API_ENDPOINT.BLOB => API_ENDPOINT.BLOB,
                API_ENDPOINT.DFS => API_ENDPOINT.DFS,
                _ => throw new NotImplementedException(),
            };

        }

        public AzureStorage SetStorageAccountName(string name)
        {
            storageAccountName = name;
            storageAccountUri = _apiEndpoint switch
            {
                API_ENDPOINT.BLOB => new Uri("https://" + storageAccountName + ".blob.core.windows.net"),
                API_ENDPOINT.DFS => new Uri("https://" + storageAccountName + ".dfs.core.windows.net"),
                _ => throw new NotImplementedException(),
            };
            
            return this;
        }

        public AzureStorage SetContainerName(string name)
        {
            containerName = name;
            return this;
        }

        public AzureStorage SetAzureCredentials(TokenCredential credential)
        {
            _azureCredentials = credential;
            return this;
        }


    }
}
