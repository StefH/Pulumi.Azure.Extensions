[![NuGet](https://buildstats.info/nuget/Pulumi.Azure.Extensions)](https://www.nuget.org/packages/Pulumi.Azure.Extensions)

# Pulumi.Azure.Constants
Additional extensions for Microsoft Azure resources with [Pulumi Azure](https://github.com/pulumi/pulumi-azure).

The following extensions are defined:

### Storage

#### BlobCollection

When you want to publish all files from a Blazor WASM website to an Azure Storage Static Website, use the code below:

``` c#
string sourceFolder = "C:\Users\xxx\Documents\GitHub\BlazorApp\publish\wwwroot";
var blobCollection = new BlobCollection(sourceFolder, new BlobCollectionArgs
{
    // Required
    Type = BlobTypes.Block,
    StorageAccountName = storageAccount.Name,
    StorageContainerName = "$web",
    AccessTier = BlobAccessTiers.Hot
});
```

There is no need to specify the ContentType for each file, this is automatically resolved using [MimeTypeMap](https://github.com/samuelneff/MimeTypeMap).