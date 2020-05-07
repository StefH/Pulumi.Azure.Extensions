using System.Threading.Tasks;

namespace Pulumi.Azure.Extensions.Tests
{
    internal static class TestingExtensions
    {
        public static Task<T> GetValueAsync<T>(this Output<T> output)
        {
            var tcs = new TaskCompletionSource<T>();
            output.Apply(v =>
            {
                tcs.SetResult(v);
                return v;
            });

            return tcs.Task;
        }

        public static T GetValue<T>(this Output<T> output)
        {
            return output.GetValueAsync().GetAwaiter().GetResult();
        }
    }
}