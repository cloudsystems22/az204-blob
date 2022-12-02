//DefaultEndpointsProtocol=https;AccountName=mystorageaz204rg;AccountKey=iURHrX3wfhLjAEiUxjKZlp9SC05AP2lP5s83ymuIHjCv+4HybLS7ujiHZQzfxv0bpYf0ygYfe940+ASt+e21Rg==;EndpointSuffix=core.windows.ne
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace AzureBlobContainers
{
    class Program
    {

        public static IConfiguration builder = new ConfigurationBuilder()
                 .AddJsonFile($"appsettings.json", true, true)
                 .Build();

        public static string storageConnectionString = builder["StorageConnectionString"];
        public static BlobServiceClient blobServiceClient = new BlobServiceClient(storageConnectionString);
        public static string localPath = "./data/";

        public static DirectoryInfo di = new DirectoryInfo(localPath);

        public static async Task Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("::                                             ::");
            Console.WriteLine("::       Aplicativo para Blob Containers       ::");
            Console.WriteLine("::                                             ::");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            //await CreateContainer("");
            await Exec();

        }

        private static Task<String> Menu(){
           
            return Task.Run(() => Result());

            string Result(){
                Console.WriteLine("");
                Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
                Console.WriteLine("::             Menu comandos Azure             ::");
                Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
                Console.WriteLine("1 - Criar novo container;");
                Console.WriteLine("2 - Upload de novo Arquivo;");
                Console.WriteLine("3 - Download de Arquivo;");
                Console.WriteLine("4 - Listar containers;");
                Console.WriteLine("5 - Listar arquivos;");
                Console.WriteLine("6 - Apagar container;");
                Console.WriteLine("0 - Sair;");
                Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
                Console.WriteLine("Entre com valor numérico (Ex:1):...");
                return Console.ReadLine();
            }

        }

        private static Task Exec(){

            return Task.Run(async() => {
                bool menuExec = false;
                do {
                    var switch_on = Convert.ToInt32(await Menu());
                    switch (switch_on)
                    {
                     case 1:
                         await CreateContainer();
                         menuExec = true;
                         break;

                     case 2:
                         await LoadFile();
                         menuExec = true;
                         break;

                     case 3:
                         await DownloadFile();
                         menuExec = true;
                         break;

                     case 4:
                         await ListContainers();
                         menuExec = true;
                         break;

                     case 5:
                         await ListBlobs();
                         menuExec = true;
                         break;
                    
                    case 6:
                         await DeleteContainer();
                         menuExec = true;
                         break;

                     default:
                         Console.Write($"{Environment.NewLine}Agora pressione qualquer tecla para sair...");
                         Console.ReadKey(true);
                         Console.Clear();
                         menuExec = false;
                         break;
                    }

                } while (menuExec);

            });

        }

        private static async Task CreateContainer()
        {
            // Create the container and return a container client object
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("::             Menu comandos Azure             ::");
            Console.WriteLine("::             Criar novo container            ::");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("Entre com um nome");
            string containerName = Console.ReadLine();

            try
            {
                 BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
                 Console.WriteLine($"Um container nomeado '{containerName}' foi criado verifique o portal.");
                 Console.WriteLine("Press 'Enter' para retornar ao menu.");
                 Console.ReadLine();
                 Console.Clear();
            } catch(Exception ex)
            {
                 Console.WriteLine($"Erro {ex.Message}");
                 Console.WriteLine("Press 'Enter' para retornar ao menu.");
                 Console.ReadLine();
                 Console.Clear();
            }

           
        }

        private static async Task LoadFile()
        {

            // Create a local file in the ./data/ directory for uploading and downloading
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("::           Selecione um container            ::");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("Entre com um nome");
            string containerName = Console.ReadLine();
            try
            {
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                Console.WriteLine("");
                Console.WriteLine("Upload de novo Arquivo::...");
                Console.WriteLine("Entre com um nome");
                string fileName = Console.ReadLine();

                string file = $"{fileName}.txt";
                string localFilePath = Path.Combine(localPath, file);

                // Write text to the file
                await File.WriteAllTextAsync(localFilePath, "Hello, World!");

                // Get a reference to the blob
                BlobClient blobClient = containerClient.GetBlobClient(file);

                Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

                // Open the file and upload its data
                using FileStream uploadFileStream = File.OpenRead(localFilePath);
                await blobClient.UploadAsync(uploadFileStream, true);

                Console.WriteLine("Upload de arquivo completo. Verifique listando");
                Console.WriteLine("Press 'Enter' para retornar ao menu.");
                Console.ReadLine();
                Console.Clear();

            } catch(Exception ex)
            {
                Console.WriteLine($"Erro {ex.Message}");
                Console.WriteLine("Press 'Enter' para retornar ao menu.");
                Console.ReadLine();
                Console.Clear();

            }
            
        }

        async static Task ListContainers()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("::              Listar containers              ::");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("");
           // Call the listing operation and enumerate the result segment.
            var resultSegment = 
                blobServiceClient.GetBlobContainersAsync(BlobContainerTraits.Metadata, "", default)
                .AsPages(default, 99);
    
            await foreach (Azure.Page<BlobContainerItem> containerPage in resultSegment)
            {
                foreach (BlobContainerItem containerItem in containerPage.Values)
                {
                    Console.WriteLine("Container name: {0}", containerItem.Name);
                }
    
                Console.WriteLine("------------------------------------------------------");
            }
            Console.WriteLine("Press 'Enter' para retornar ao menu.");
            Console.ReadLine();
            Console.Clear();
        }

        async static Task ListBlobs()
        {
            // List blobs in the container
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("::               Listar Arquivos               ::");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("");

            Console.WriteLine("Selecione o container::...");
            Console.WriteLine("Entre com um nome");
            string containerName = Console.ReadLine();

            try
            {
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                Console.WriteLine("---------------------------------");
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    Console.WriteLine($"file: {blobItem.Name}");
                    Console.WriteLine("---------------------------------");
                }

                Console.WriteLine("Press 'Enter' para retornar ao menu.");
                Console.ReadLine();
                Console.Clear();

            } catch(Exception ex)
            {
                Console.WriteLine($"Erro {ex.Message}");
                Console.WriteLine("Press 'Enter' para retornar ao menu.");
                Console.ReadLine();
                Console.Clear();

            }

            
        }

        async static Task DownloadFile(){
            // Download the blob to a local file
            // Append the string "DOWNLOADED" before the .txt extension 
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("::             Downloade de Arquivo            ::");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("");

            Console.WriteLine("Selecione o container::...");
            Console.WriteLine("Entre com um nome");
            string containerName = Console.ReadLine();

            try
            {
                 BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                 Console.WriteLine("Entre com o nome do arquivo");
                 string fileName = Console.ReadLine();
     
                 
                 string file = $"{fileName}.txt";
                 
                 string localFilePath = Path.Combine(localPath, file);
                 string downloadFilePath = localFilePath.Replace(".txt", "DOWNLOADED.txt");
     
                 Console.WriteLine("\nDownloading blob para\n\t{0}\n", downloadFilePath);
     
                 BlobClient blobClient = containerClient.GetBlobClient(file);
     
                 // Download the blob's contents and save it to a file
                 BlobDownloadInfo download = await blobClient.DownloadAsync();
     
                 using (FileStream downloadFileStream = File.OpenWrite(downloadFilePath))
                 {
                     await download.Content.CopyToAsync(downloadFileStream);
                 }
                 Console.WriteLine("\nLocalize o arquivo local no diretório de dados criado anteriormente para verificar se foi baixado.");
                 Console.WriteLine("Press 'Enter' para retornar ao menu.");
                 Console.ReadLine();
                 Console.Clear();

            } catch(Exception ex)
            {
                Console.WriteLine($"Erro {ex.Message}");
                Console.WriteLine("Press 'Enter' para retornar ao menu.");
                Console.ReadLine();
                Console.Clear();

            }

        }

        async static Task DeleteContainer()
        {
            // Delete the container and clean up local files created
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("::               Apagar container              ::");
            Console.WriteLine(":::::::::::::::::::::::::::::::::::::::::::::::::");
            Console.WriteLine("");

            Console.WriteLine("Selecione o container::...");
            Console.WriteLine("Entre com um nome");
            string containerName = Console.ReadLine();
            try{
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                await containerClient.DeleteAsync();
                Console.WriteLine("Excluindo a fonte local e os arquivos baixados...");
                foreach(FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                Console.WriteLine("Terminada a limpeza.");
                Console.WriteLine("Press 'Enter' para retornar ao menu.");
                Console.ReadLine();
                Console.Clear();

            } catch(Exception ex)
            {
                Console.WriteLine($"Erro {ex.Message}");
                Console.WriteLine("Press 'Enter' para retornar ao menu.");
                Console.ReadLine();
                Console.Clear();
            }


            
        }
       
    }
}