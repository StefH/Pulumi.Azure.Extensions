# Pulumi.Azure.Extensions
[![NuGet](https://buildstats.info/nuget/Pulumi.Azure.Extensions)](https://www.nuget.org/packages/Pulumi.Azure.Extensions)

Additional extensions for Microsoft Azure resources with [Pulumi Azure](https://github.com/pulumi/pulumi-azure).

The following extensions are defined:

### Storage

#### BlobCollection
_Type:_ `azure-extensions:storage:BlobCollection`

When you want to publish all files from a Blazor WASM website to an Azure Storage Static Website, use the code below:

``` c#
string sourceFolder = "C:\Users\xxx\Documents\GitHub\BlazorApp\publish\wwwroot";
var blobCollection = new BlobCollection("static-website-files", new BlobCollectionArgs
{
    // Required
    Source = sourcefolder,
    Type = BlobTypes.Block,
    StorageAccountName = storageAccount.Name,
    StorageContainerName = "$web",
    AccessTier = BlobAccessTiers.Hot
});
```

Notes:
- empty files are skipped
- there is no need to specify the ContentType for each file, this is automatically resolved using [MimeTypeMap](https://github.com/samuelneff/MimeTypeMap).