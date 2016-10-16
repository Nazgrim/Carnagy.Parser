using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Utility
{
    public class ImageDownloader : IDownloadImage
    {
        private readonly string _baseDir;
        private readonly int _threadCount;

        public ImageDownloader(string baseDir, int threadCount)
        {
            _baseDir = baseDir;
            _threadCount = threadCount;
        }

        public void Download(string dirName, IEnumerable<ImageDownloadCommand> commands)
        {
            var dirPath = Path.Combine(_baseDir, dirName);

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            commands
                .Select(cmd => new
                {
                    Path = Path.Combine(dirPath, $"{cmd.Id}.jpg"),
                    cmd.Url
                })
                .Where(cmd => !File.Exists(cmd.Path))
                .SplitBy(_threadCount)
                .ForEach(grp =>
                {
                    var tasks = grp
                        .Select(img => DownloadAndSaveImage(img.Url, img.Path))
                        .ToArray();

                    Task.WaitAll(tasks);
                });
        }

        private static async Task DownloadAndSaveImage(string url, string path)
        {
            // TODO: Removed using for response stream. Please confirm there is no memory leak.
            await SaveImage(path, await DownloadImage(url));
        }

        private static async Task<Stream> DownloadImage(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create("http:" + url);
            var webResponse = await request.GetResponseAsync();
            var response = (HttpWebResponse)webResponse;

            var validHttpStatusCodes = new [] { HttpStatusCode.OK, HttpStatusCode.Moved, HttpStatusCode.Redirect };
            if (validHttpStatusCodes.Contains(response.StatusCode) == false)
            {
                throw new InvalidOperationException($"The server responded with {response.StatusCode} status code.");
            }

            if (!response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"The server returned the content with \"{response.ContentType}\" content type.");
            }

            return response.GetResponseStream();
        }

        private static async Task SaveImage(string path, Stream memory)
        {
            using (var file = File.OpenWrite(path))
            {
                var buffer = new byte[4096];
                int bytesRead;
                do
                {
                    bytesRead = await memory.ReadAsync(buffer, 0, buffer.Length);
                    await file.WriteAsync(buffer, 0, bytesRead);
                } while (bytesRead != 0);
            }
        }
    }
}
