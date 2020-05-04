using System;
using System.Collections.Immutable;
using System.Threading.Tasks;
using Moq;
using Pulumi.Testing;

namespace Pulumi.Azure.Extensions.Tests
{
    public static class Testing
    {
        public static Task<ImmutableArray<Resource>> RunAsync<T>() where T : Stack, new()
        {
            var mocks = new Mock<IMocks>();

            mocks.Setup(m => m.NewResourceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<ImmutableDictionary<string, object>>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync((string type, string name, ImmutableDictionary<string, object> inputs, string? provider, string? id) => (id ?? Guid.NewGuid().ToString(), Convert(inputs)));

            mocks.Setup(m => m.CallAsync(It.IsAny<string>(), It.IsAny<ImmutableDictionary<string, object>>(), It.IsAny<string>()))
                .ReturnsAsync((string token, ImmutableDictionary<string, object> args, string? provider) => args);

            return Deployment.TestAsync<T>(mocks.Object, new TestOptions { IsPreview = false });
        }

        private static ImmutableDictionary<string, object> Convert(ImmutableDictionary<string, object> dictionary)
        {
            var builder = ImmutableDictionary.CreateBuilder<string, object>();
            foreach (var entry in dictionary)
            {
                // https://github.com/pulumi/pulumi/issues/4558
                switch (entry.Value)
                {
                    case int _:
                    case double _:
                    case bool _:
                    case string s:
                    case ImmutableArray<object> _:
                    case ImmutableDictionary<string, object> _:
                        builder.Add(entry.Key, entry.Value);
                        break;
                }
            }

            return builder.ToImmutable();
        }
    }
}