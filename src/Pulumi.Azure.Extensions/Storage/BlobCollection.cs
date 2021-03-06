﻿using System;
using System.Collections.Generic;
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
        /// An absolute path to a folder on the local file system.
        /// </summary>
        public string Source { get; set; }

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

    public sealed class BlobCollection : ComponentResource
    {
        private const string SearchPattern = "*.*";

        /// <summary>
        /// Upload all files and folders from a sourceFolder to a Blob Storage Account in Azure.
        /// </summary>
        /// <param name="name">The unique name of the resource</param>
        /// <param name="args">The arguments used to populate this resource's properties</param>
        /// <param name="options">A bag of options that control this resource's behavior</param>
        public BlobCollection(string name, BlobCollectionArgs args, ComponentResourceOptions? options = null) :
            base("azure-extensions:storage:BlobCollection", name, options)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            if (string.IsNullOrEmpty(args.Source))
            {
                throw new ArgumentNullException(nameof(args.Source));
            }

            foreach (var (fileInfo, blobName) in GetAllFiles(args.Source))
            {
                var blobArgs = new BlobArgs
                {
                    AccessTier = args.AccessTier,
                    ContentType = MimeTypeMap.GetMimeType(fileInfo.Extension),
                    Name = blobName,
                    Parallelism = args.Parallelism,
                    StorageAccountName = args.StorageAccountName,
                    StorageContainerName = args.StorageContainerName,
                    Type = args.Type
                };

                // https://github.com/pulumi/pulumi-azure/issues/544
                if (fileInfo.Length > 0)
                {
                    blobArgs.Source = new FileAsset(fileInfo.FullName);
                }

                var blobOptions = new CustomResourceOptions
                {
                    Parent = this
                };

                _ = new Blob(blobName, blobArgs, blobOptions);
            }
        }

        private static IEnumerable<(FileInfo fileInfo, string blobName)> GetAllFiles(string source)
        {
            var fileInfo = new FileInfo(source);
            if (fileInfo.Exists)
            {
                return new (FileInfo fileInfo, string blobName)[] { (fileInfo, fileInfo.Name) };
            }

            if (Directory.Exists(source))
            {
                int sourceFolderLength = source.Length + 1;

                return Directory.EnumerateFiles(source, SearchPattern, SearchOption.AllDirectories)
                    .Select(path =>
                    (
                        new FileInfo(path),
                        path.Remove(0, sourceFolderLength).Replace(Path.PathSeparator, '/') // Make the blobName Azure Storage compatible
                    ));
            }

            throw new NotSupportedException("The source provided must be an existing file or folder.");
        }
    }
}