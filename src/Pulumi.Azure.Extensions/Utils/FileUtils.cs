using System;
using System.IO;

namespace Pulumi.Azure.Extensions.Utils
{
    internal static class FileUtils
    {
        public static string GetTemporaryDirectory()
        {
            return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        }
    }
}