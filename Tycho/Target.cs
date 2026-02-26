using System;
using System.Threading;
using System.Threading.Tasks;
using static Tycho.Testting.IAlphaModule;

namespace Tycho.Testting
{
    public partial interface IAlphaModule : IAsyncDisposable
    {
        Task ExecuteAsync(object requestData, CancellationToken cancellationToken = default);
    }

    public partial interface IAlphaModule : IAsyncDisposable
    {
        public interface Parent
        {
            // Execute
        }
    }

    public partial interface IAlphaModule : IAsyncDisposable
    {
        public interface Publisher
        {
            // Publish
        }
    }

    public class XD
    {
        public IAlphaModule AlphaModule { get; set; }

        public Parent Parent { get; set; }

        public Publisher Publisher { get; set; }
    }
}