using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Pulumi.Azure.Constants;
using Pulumi.Azure.Extensions.Storage;
using Xunit;

namespace Pulumi.Azure.Extensions.Tests.Storage
{
    public class BlobCollectionTests
    {
        private const string BlobCollectionName = "test";
        private static string FilesFolder => Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "netcoreapp3.1", "files");

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

        private class BlobCollectionStackZipFile : Stack
        {
            public BlobCollectionStackZipFile()
            {
                var args = new BlobCollectionArgs
                {
                    Source = Path.Combine(FilesFolder, "files.zip"),
                    StorageAccountName = "sa",
                    StorageContainerName = "sc",
                    Type = BlobTypes.Block
                };

                _ = new BlobCollection(BlobCollectionName, args);
            }
        }

        [Fact]
        public async Task Folder_NoFiles()
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

        //[Fact]
        //public async Task ZipFile()
        //{
        //    // Arrange and Act
        //    var resources = await Testing.RunAsync<BlobCollectionStackZipFile>();

        //    // Assert
        //    resources.Length.Should().Be(2);
        //    var blobCollection = resources.OfType<BlobCollection>().FirstOrDefault();

        //    Assert.NotNull(blobCollection);
        //    blobCollection.GetResourceName().Should().Be("test");
        //}
    }
}