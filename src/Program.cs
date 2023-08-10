using Azure.Identity;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;


namespace AzureStorageMPAdemo
{
    internal class Program
    {

        private static readonly string storageAccountName = "YourAccountName";
        private static InteractiveBrowserCredential? azureCredentials;
        private static int pageSize = 10;
        private static int files = 50;
        private static int personsPerFile = 250;
        private static string guid = Guid.NewGuid().ToString();
        private static string containerNameDfs
        {
            get
            {
                return "fakedata-dfs-" + guid;
            }
        }
        private static string containerNameBlob
        {
            get
            {
                return "fakedata-blob-" + guid;
            }
        }

        static async Task Main(string[] args)
        {

            try
            {
                Console.WriteLine("Hello Azure Storage Friends!");

                var fakeDataDirectory = Path.Combine(Environment.CurrentDirectory, "FakeData", "FakePersons");

                Console.WriteLine($"Generating Fake Data: {files} files, each file {personsPerFile} persons");
                generateFakePersionsJSON(fakeDataDirectory, files, personsPerFile);

                azureCredentials = new InteractiveBrowserCredential();

                AzureDataLake dfs = (AzureDataLake)new AzureDataLake()
                    .SetAzureCredentials(azureCredentials)
                    .SetContainerName(containerNameDfs)
                    .SetStorageAccountName(storageAccountName);


                Console.WriteLine($"Uploading files using Azure DFS API: Containername: {containerNameDfs}");
                await dfs.UploadDirectoryAsync(fakeDataDirectory);

                AzureBlob blob = (AzureBlob)new AzureBlob()
                    .SetAzureCredentials(azureCredentials)
                    .SetContainerName(containerNameDfs)
                    .SetStorageAccountName(storageAccountName);


                Console.WriteLine($"Listing files using Azure Blob API using {pageSize} as PageSize: Containername: {containerNameDfs}");
                await blob.ListBlobsFlatListing(pageSize);


                var fakeDataDirectoryDownload = Path.Combine(Environment.CurrentDirectory, "FakeData-Download", "Blob", containerNameDfs);
                Console.WriteLine($"Downloading files using Azure Blob API to: {fakeDataDirectoryDownload}");
                await blob.DownloadDirectoryAsync(fakeDataDirectoryDownload);


                blob.SetContainerName(containerNameBlob);
                Console.WriteLine($"Uploading files using Azure Blob API and keeping folder structure: Containername: {containerNameBlob}");
                await blob.UploadDirectoryAsync(fakeDataDirectory);


                Console.WriteLine($"Listing files using Azure DFS API: Containername: {containerNameBlob}");
                dfs.SetContainerName(containerNameBlob);
                await dfs.ListFilesInDirectoryRecursivelyAsync();


                fakeDataDirectoryDownload = Path.Combine(Environment.CurrentDirectory, "FakeData-Download", "DFS", containerNameBlob);
                Console.WriteLine($"Downloading files and creating folder structure locally using Azure DFS API to: {fakeDataDirectoryDownload}");
                await dfs.DownloadDirectoryRecursivelyAsync(fakeDataDirectoryDownload);


                Console.WriteLine("Finished");
                Console.ReadLine();

            }
            catch (Exception ex)
            { 
                Console.WriteLine("An error occourred: " + ex.Message);
            } 


        }       

        private static void generateFakePersionsJSON(string fakeDataDirectory, int files, int personPerFile)
        {

            if (!Directory.Exists(fakeDataDirectory))
            {
                
                Console.WriteLine($"Fake Data will be created at {fakeDataDirectory}");

                Directory.CreateDirectory(fakeDataDirectory);

                for (int i = 0; i < files; i++)
                {

                    var testData = Person.FakeData.Generate(personPerFile).ToList();

                    var jsonData = JsonConvert.SerializeObject(testData, Formatting.Indented);

                    var filePath = Path.Combine(fakeDataDirectory, $"FakePerson-{DateTime.Now.ToFileTime()}.json");

                    Console.Write(".");

                    File.WriteAllText(filePath, jsonData);

                }

                Console.WriteLine();
            } 
            else
            {
                Console.WriteLine($"Fake data already exists at {fakeDataDirectory}");
            }

        }

    }
}