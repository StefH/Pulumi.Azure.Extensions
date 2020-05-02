using System;
using System.IO;
using System.Linq;
using Pulumi.Azure.Extensions.Utils;
using Pulumi.Azure.Storage;

namespace Pulumi.Azure.Extensions.Storage
{
    public sealed class BlobCollectionArgs : ResourceArgs
    {
        /// <summary>
        /// The access tier of the storage blob. Possible values are `Archive`, `Cool` and `Hot`.
        /// </summary>
        [Input("accessTier", false, false)]
        public Input<string> AccessTier { get; set; }

        /// <summary>
        /// The number of workers per CPU core to run for concurrent uploads. Defaults to `8`.
        /// </summary>
        [Input("parallelism", false, false)]
        public Input<int> Parallelism { get; set; }

        /// <summary>
        /// Specifies the storage account in which to create the storage container.
        /// Changing this forces a new resource to be created.
        /// </summary>
        [Input("storageAccountName", true, false)]
        public Input<string> StorageAccountName { get; set; }

        /// <summary>
        /// The name of the storage container in which this blob should be created.
        /// </summary>
        [Input("storageContainerName", true, false)]
        public Input<string> StorageContainerName { get; set; }

        /// <summary>
        /// The type of the storage blobs to be created. Possible values are `Append`, `Block` or `Page`. Changing this forces a new resource to be created.
        /// </summary>
        [Input("type", true, false)]
        public Input<string> Type { get; set; }
    }

    public sealed class BlobCollection
    {
        private const string SearchPattern = "*.*";

        /// <summary>
        /// Upload all files and folders from a sourceFolder to a Blob Storage Account in Azure.
        /// </summary>
        /// <param name="sourceFolder">An absolute path to a folder on the local file system.</param>
        /// <param name="args">The arguments used to populate the <see cref="Blob"/> resources.</param>
        public BlobCollection(string sourceFolder, BlobCollectionArgs args)
        {
            if (string.IsNullOrEmpty(sourceFolder))
            {
                throw new ArgumentNullException(nameof(sourceFolder));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            int sourceFolderLength = sourceFolder.Length + 1;

            var files = Directory.EnumerateFiles(sourceFolder, SearchPattern, SearchOption.AllDirectories)
                .Select(path => new
                {
                    info = new FileInfo(path),
                    name = path.Remove(0, sourceFolderLength).Replace(Path.PathSeparator, '/'), // Make the name Azure Storage compatible
                })
                .Where(file => file.info.Length > 0) // https://github.com/pulumi/pulumi-azure/issues/544
            ;

            foreach (var file in files)
            {
                _ = new Blob(file.name, new BlobArgs
                {
                    AccessTier = args.AccessTier,
                    Name = file.name,
                    StorageAccountName = args.StorageAccountName,
                    StorageContainerName = args.StorageContainerName,
                    Type = args.Type,
                    Source = new FileAsset(file.info.FullName),
                    ContentType = MimeTypeMap.GetMimeType(file.info.Extension)
                });
            }
        }
    }
}