
### Criando Account Storage
1. az login
2. az group create --location <myLocation> --name az204-blob-rg
3. az storage account create --resource-group az204-blob-rg --name <myStorageAcct> --location <myLocation> --sku Standard_LRS

### DependĂȘncias
dotnet add package Azure.Storage.Blobs

dotnet add package Microsoft.Extensions.Configuration </br>
dotnet add package Microsoft.Extensions.Configuration.Json