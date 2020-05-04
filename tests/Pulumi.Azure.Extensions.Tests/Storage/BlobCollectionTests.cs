using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Pulumi.Azure.Constants;
using Pulumi.Azure.Extensions.Storage;
using Pulumi.Azure.Storage;
using Xunit;

namespace Pulumi.Azure.Extensions.Tests.Storage
{
    public class BlobCollectionTests
    {
        private const string BlobCollectionName = "test";
        private static string FilesFolder => Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "netcoreapp3.1", "files");

        private class BlobStack : Stack
        {
            public BlobStack()
            {
                var blobArgs = new BlobArgs
                {
                    AccessTier = BlobAccessTiers.Hot,
                    Name = "test",
                    Source = new FileAsset(Path.Combine(FilesFolder, "TextFile1.txt")),
                    StorageAccountName = "sa",
                    StorageContainerName = "sa",
                    Type = BlobTypes.Block
                };

                var blobOptions = new CustomResourceOptions
                {
                    Parent = this
                };

                _ = new Blob("test", blobArgs, blobOptions);
            }
        }

        private class BlobCollectionStackFolderNoFiles : Stack
        {
            public BlobCollectionStackFolderNoFiles()
            {
                var args = new BlobCollectionArgs
                {
                    Source = Path.Combine(FilesFolder, "y"),
                    StorageAccountName = "sa",
                    StorageContainerName = "sc",
                    Type = BlobTypes.Block
                };

                _ = new BlobCollection(BlobCollectionName, args);
            }
        }

        private class BlobCollectionStackFolderWithFiles : Stack
        {
            public BlobCollectionStackFolderWithFiles()
            {
                var args = new BlobCollectionArgs
                {
                    Source = FilesFolder,
                    StorageAccountName = "sa",
                    StorageContainerName = "sc",
                    Type = BlobTypes.Block
                };

                _ = new BlobCollection(BlobCollectionName, args);
            }
        }

        private class BlobCollectionStackEmptyFile : Stack
        {
            public BlobCollectionStackEmptyFile()
            {
                var args = new BlobCollectionArgs
                {
                    Source = Path.Combine(FilesFolder, "y", "0.txt"),
                    StorageAccountName = "sa",
                    StorageContainerName = "sc",
                    Type = BlobTypes.Block
                };

                _ = new BlobCollection(BlobCollectionName, args);
            }
        }

        private class BlobCollectionStackFile : Stack
        {
            public BlobCollectionStackFile()
            {
                var args = new BlobCollectionArgs
                {
                    Source = Path.Combine(FilesFolder, "x", "TextFile3.txt"),
                    StorageAccountName = "sa",
                    StorageContainerName = "sc",
                    Type = BlobTypes.Block
                };

                _ = new BlobCollection(BlobCollectionName, args);
            }
        }

        //private class BlobCollectionStackZipFile : Stack
        //{
        //    public BlobCollectionStackZipFile()
        //    {
        //        var args = new BlobCollectionArgs
        //        {
        //            Source = Path.Combine(FilesFolder, "files.zip"),
        //            StorageAccountName = "sa",
        //            StorageContainerName = "sc",
        //            Type = BlobTypes.Block
        //        };

        //        _ = new BlobCollection(BlobCollectionName, args);
        //    }
        //}

        [Fact]
        public async Task Blob()
        {
            // Arrange and Act
            var resources = await Testing.RunAsync<BlobStack>();

            // Assert
            resources.Length.Should().Be(2);
            var blob = resources.OfType<Blob>().FirstOrDefault();

            Assert.NotNull(blob);
            blob.GetResourceName().Should().Be("test");
        }

        [Fact]
        public async Task Folder_WithEmptyFile()
        {
            // Arrange and Act
            var resources = await Testing.RunAsync<BlobCollectionStackFolderNoFiles>();

            // Assert
            resources.Length.Should().Be(2);
            var blobCollection = resources.OfType<BlobCollection>().FirstOrDefault();

            Assert.NotNull(blobCollection);
            blobCollection.GetResourceName().Should().Be("test");
        }

        [Fact]
        public async Task Folder_WithFiles()
        {
            // Arrange and Act
            var resources = await Testing.RunAsync<BlobCollectionStackFolderWithFiles>();

            // Assert
            resources.Length.Should().Be(6);
            var blobCollection = resources.OfType<BlobCollection>().FirstOrDefault();

            Assert.NotNull(blobCollection);
            blobCollection.GetResourceName().Should().Be("test");

            var blobs = resources.OfType<Blob>().ToList();
            Assert.NotNull(blobs);

            blobs.Count.Should().Be(4);
        }

        [Fact]
        public async Task EmptyFile()
        {
            // Arrange and Act
            var resources = await Testing.RunAsync<BlobCollectionStackEmptyFile>();

            // Assert
            resources.Length.Should().Be(2);
            var blobCollection = resources.OfType<BlobCollection>().FirstOrDefault();

            Assert.NotNull(blobCollection);
            blobCollection.GetResourceName().Should().Be("test");
        }

        [Fact]
        public async Task File()
        {
            // Arrange and Act
            var resources = await Testing.RunAsync<BlobCollectionStackFile>();

            // Assert
            resources.Length.Should().Be(3);
            var blobCollection = resources.OfType<BlobCollection>().FirstOrDefault();

            Assert.NotNull(blobCollection);
            blobCollection.GetResourceName().Should().Be("test");

            var blobs = resources.OfType<Blob>().ToList();
            Assert.NotNull(blobs);

            blobs.Count.Should().Be(1);
        }

        //[Fact]
        //public async Task ZipFile()
        //{
        //    // Arrange and Act
        //    var resources = await Testing.RunAsync<BlobCollectionStackZipFile>();

        //    // Assert
        //    resources.Length.Should().Be(3);
        //    var blobCollection = resources.OfType<BlobCollection>().FirstOrDefault();

        //    Assert.NotNull(blobCollection);
        //    blobCollection.GetResourceName().Should().Be("test");

        //    var blobs = resources.OfType<Blob>().ToList();
        //    Assert.NotNull(blobs);

        //    blobs.Count.Should().Be(1);
        //}
    }
}