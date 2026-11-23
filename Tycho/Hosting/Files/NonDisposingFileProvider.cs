using System;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace Tycho.Hosting.Files
{
    internal sealed class NonDisposingFileProvider : IFileProvider
    {
        private readonly IFileProvider _fileProvider;

        public NonDisposingFileProvider(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider ?? throw new ArgumentNullException(nameof(fileProvider));
        }

        public IFileInfo GetFileInfo(string subpath) => _fileProvider.GetFileInfo(subpath);

        public IDirectoryContents GetDirectoryContents(string subpath) => _fileProvider.GetDirectoryContents(subpath);

        public IChangeToken Watch(string filter) => _fileProvider.Watch(filter);
    }
}
