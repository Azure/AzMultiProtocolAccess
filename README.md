---
page_type: sample
name: AzMultiProtocolAccess
topic: sample
description: |
  AzMultiProtocolAccess is a sample application designed to showcase how to use Multi-protocol access on Azure Data Lake Storage Gen2.
languages:
  - csharp
products:
  - azure
  - azure-blob-storage
urlFragment: azmultiprotocolaccess
---

# Azure Storage Multi-protocol access
AzMultiProtocolAccess is a sample application designed to showcase how to use Multi-protocol access on Azure Data Lake Storage Gen2.
The following actions will be performed in sequence:
* Generating Fake Data
* Uploading files using Azure DFS API
* Listing files using Azure Blob API using a PageSize
* Downloading files using Azure Blob API 
* Uploading files using Azure Blob API and keeping folder structure
* Listing files using Azure DFS API
* Downloading files and creating folder structure locally using Azure DFS API

## Contents
| File/folder       | Description                                |
|-------------------|--------------------------------------------|
| `src`             | Sample source code.                        |
| `.gitignore`      | Define what to ignore at commit time.      |
| `CONTRIBUTING.md` | Guidelines for contributing to the sample. |
| `README.md`       | This README file.                          |
| `LICENSE`         | The license for the sample.                |

## Prerequisites
To use this sample, you need to have an Azure subscription. If you do not have an Azure subscription, you can create a free account before you begin. For more information, see [Create an Azure free account](https://azure.microsoft.com/free/?WT.mc_id=A261C142F).

All access to Azure Storage takes place through a storage account. For this sample, create a storage account using the [Azure portal](https://portal.azure.com), Azure PowerShell, or Azure CLI. For help creating a storage account, see [Create a storage account](https://learn.microsoft.com/en-us/azure/storage/common/storage-account-create?tabs=azure-portal) and *enable hierarchical namespace* for [Multi-protocol access](https://learn.microsoft.com/en-us/azure/storage/blobs/data-lake-storage-multi-protocol-access).

For authentication, Azure Active Directory is used. In this sample read and write operations are performed on a storage account. To grant access to the storage account, you need to grant your Identity at least "Storage Blob Data Contributor" privileges. For more information, see [Authorize access to Azure blobs and queues using Azure Active Directory](https://docs.microsoft.com/en-us/azure/storage/common/storage-auth-aad?tabs=dotnet).

### Using Azure CLI
```console
# Create Ressource Group 
az group create -n rgAzMultiProtocolAccess -l westeurope

# Create Storage Account with Hierarchical Namespace enabled
az storage account create --name YourAccountName \
--resource-group rgAzMultiProtocolAccess --location westeurope \
--sku Standard_LRS --enable-hierarchical-namespace

# Add Role Assignment and grant your Identity "Storage Blob Data Contributor" privileges
az role assignment create --assignee "user@tenant.onmicrosoft.com" \
--role "Storage Blob Data Contributor" \
--scope "/subscriptions/00000000-0000-0000-0000-000000000000/resourceGroups/rgAzMultiProtocolAccess/providers/Microsoft.Storage/storageAccounts/YourAccountName"
```


## Setup
### Option one
Before running the sample, you must change "YourAccountName" in the *src/Program.cs* file (line 11) to the name of your storage account created in the step before.

```csharp
private static readonly string storageAccountName = "YourAccountName";
```        

### Option two
Assign the storage account name as a command line argument when running the sample.

```console  
dotnet run YourAccountName
```


## Running the sample
To build and run the sample, change to the *src* directory and execute the following command:

```console
dotnet run
```

![](AzMultiProtocolAccess.gif)

## Key concepts
For more information about Multi-protocol access, see the following articles:

* [Documentation](https://learn.microsoft.com/en-us/azure/storage/blobs/data-lake-storage-multi-protocol-access)
* [Training](https://learn.microsoft.com/en-us/training/modules/access-data-azure-blob-storage-multiple-protocols/)

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft
trademarks or logos is subject to and must follow
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
