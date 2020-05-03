using System;
using System.IO;

namespace Pulumi.Azure.Extensions.Utils
{
    /// <summary>
    /// Represents a temporary storage on file system.
    /// </summary>
    public sealed class TempStorage : IDisposable
    {
        public TempStorage() : this(FileUtils.GetTemporaryDirectory())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TempStorage"/> class.
        /// </summary>
        /// <param name="path">The path to use as temp storage.</param>
        public TempStorage(string path)
        {
            Path = path;
            Clear();
            Create();
        }

        public string Path { get; }

        private void Create()
        {
            try
            {
                if (!Directory.Exists(Path))
                {
                    Directory.CreateDirectory(Path);
                }
            }
            catch (IOException)
            {
            }
        }

        public void Clear()
        {
            try
            {
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path, true);
                }
            }
            catch (IOException)
            {
            }
        }

        /// <summary>
        /// An indicator whether this object is beeing actively disposed or not.
        /// </summary>
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Throws an exception if something is tried to be done with an already disposed object.
        /// </summary>
        /// <remarks>
        /// All public methods of the class must first call this.
        /// </remarks>
        public void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>
        /// Releases managed resources upon dispose.
        /// </summary>
        /// <remarks>
        /// All managed resources must be released in this
        /// method, so after disposing this object no other
        /// object is being referenced by it anymore.
        /// </remarks>
        private void ReleaseManagedResources()
        {
            Clear();
        }

        /// <summary>
        /// Releases unmanaged resources upon dispose.
        /// </summary>
        /// <remarks>
        /// All unmanaged resources must be released in this
        /// method, so after disposing this object no other
        /// object is beeing referenced by it anymore.
        /// </remarks>
        private void ReleaseUnmanagedResources()
        {
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                /* Release unmanaged resources */
                ReleaseUnmanagedResources();

                if (disposing)
                {
                    /* Release managed resources */
                    ReleaseManagedResources();
                }

                /* Set indicator that this object is disposed */
                _disposed = true;
            }
        }
    }
}