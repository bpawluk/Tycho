namespace Tycho.Persistence.EFCore.TestUtils.Databases;

internal static class ContainerStartupLock
{
    private const int ErrorCodeMask = 0xffff;
    private const int ResourceTemporarilyUnavailable = 11;
    private const int SharingViolation = 32;
    private const int LockViolation = 33;

    internal static async Task<FileStream> AcquireAsync(string containerName, CancellationToken cancellationToken)
    {
        string path = Path.Combine(Path.GetTempPath(), containerName + ".lock");

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                return new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException exception) when (IsLockContention(exception))
            {
                await Task.Delay(TimeSpan.FromMilliseconds(100), cancellationToken);
            }
        }
    }

    private static bool IsLockContention(IOException exception) => (exception.HResult & ErrorCodeMask) is ResourceTemporarilyUnavailable or SharingViolation or LockViolation;
}
