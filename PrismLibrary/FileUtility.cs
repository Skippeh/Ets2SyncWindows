using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PrismLibrary
{
    public static class FileUtility
    {
        public static async Task<FileStream> WaitForFile(string fullPath, FileMode mode, FileAccess access, FileShare share, int maxWaitTimeMs = 0, CancellationToken cancellationToken = default)
        {
            DateTime startTime = DateTime.UtcNow;

            while (true)
            {
                if (cancellationToken.IsCancellationRequested || DateTime.UtcNow - startTime > TimeSpan.FromMilliseconds(maxWaitTimeMs))
                    break;
                
                FileStream fileStream = null;
                
                try
                {
                    fileStream = new FileStream(fullPath, mode, access, share);
                    return fileStream;
                }
                catch (IOException)
                {
                    if (fileStream != null)
                        await fileStream.DisposeAsync();
                    
                    Thread.Sleep(50);
                }
            }

            return null;
        }

        public static IEnumerable<string> ReadAllLines(FileStream fileStream)
        {
            var streamReader = new StreamReader(fileStream); // Don't wrap in using statement. The caller is responsible for disposing the stream.
            string line;
            while ((line = streamReader.ReadLine()) != null)
            {
                yield return line;
            }
        }
    }
}