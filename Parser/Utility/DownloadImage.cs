using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Utility
{
    public class DownloadImage: IDownloadImage
    {
        private string BaseDir { get; set; }
        public DownloadImage(string baseDir)
        {
            BaseDir = baseDir;
        }

        public void Download(string dir, IEnumerable<ImageForSave> images)
        {
            var folder = Path.Combine(BaseDir, dir);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            var threadCount = 5;

            var taskList = new List<Task>();

            foreach (var imageForSave in images)
            {
                var filePath = Path.Combine(folder, $"{imageForSave.Id}.jpg");
                if (File.Exists(filePath))
                    return;

                taskList.Add(DownloadRemoteImageFile(imageForSave.Url, filePath));
                if (taskList.Count != threadCount) continue;
                Task.WaitAll(taskList.ToArray());
                taskList.Clear();
            }
            Task.WaitAll(taskList.ToArray());
        }

        private static async Task DownloadRemoteImageFile(string uri, string fileName)
        {
            var request = (HttpWebRequest)WebRequest.Create("http:" + uri);
            WebResponse webResponse = null;
            try
            {
                webResponse = await request.GetResponseAsync();
            }
            catch (Exception)
            {
                return;
            }
            var response = (HttpWebResponse)webResponse;

            if ((response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Moved &&
                response.StatusCode != HttpStatusCode.Redirect) ||
               !response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)) return;
            // if the remote file was found, download it
            using (var inputStream = response.GetResponseStream())
            using (var outputStream = File.OpenWrite(fileName))
            {
                var buffer = new byte[4096];
                int bytesRead;
                do
                {
                    bytesRead = await inputStream.ReadAsync(buffer, 0, buffer.Length);
                    await outputStream.WriteAsync(buffer, 0, bytesRead);
                } while (bytesRead != 0);
            }
        }
    }
}
