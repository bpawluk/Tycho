using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Tycho.Utils.SourceGenerator.Utils
{
    public static class EmbeddedResource
    {
        public static string GetContent(string relativePath)
        {
            var assembly = Assembly.GetExecutingAssembly();

            string baseName = assembly
                .GetName()
                .Name;

            string resourceName = relativePath
                .TrimStart('.')
                .Replace(Path.DirectorySeparatorChar, '.')
                .Replace(Path.AltDirectorySeparatorChar, '.');

            string manifestResourceName = assembly
                .GetManifestResourceNames()
                .FirstOrDefault(x => x
                    .EndsWith(
                        resourceName,
                        StringComparison.InvariantCulture));

            if (string.IsNullOrEmpty(manifestResourceName))
            {
                throw new InvalidOperationException(
                    $"Did not find required resource ending in '{resourceName}' in assembly '{baseName}'.");
            }

            using Stream stream = assembly.GetManifestResourceStream(manifestResourceName) ?? throw new InvalidOperationException(
                    $"Did not find required resource '{manifestResourceName}' in assembly '{baseName}'.");
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
