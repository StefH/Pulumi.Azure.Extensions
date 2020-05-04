using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pulumi.Azure.Extensions.Tests
{
    internal static class TestingExtensions
    {
        public static T GetValue<T>(this Output<T> output)
        {
            var field = output.GetType().GetField("DataTask");

            return default;
        }
    }
}