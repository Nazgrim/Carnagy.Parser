using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Utility
{
    public class DownloadImage
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

            foreach (var imageForSave in images)
            {
                var filePath = Path.Combine(folder, $"{imageForSave.Id}.jpg");
                if (File.Exists(filePath))
                    continue;
                DownloadRemoteImageFile(imageForSave.Url, filePath);
            }
        }

        private static bool DownloadRemoteImageFile(string uri, string fileName)
        {
            var request = (HttpWebRequest)WebRequest.Create("http:" + uri);
            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception)
            {
                return false;
            }

            // Check that the remote file was found. The ContentType
            // check is performed since a request for a non-existent
            // image file might be redirected to a 404-page, which would
            // yield the StatusCode "OK", even though the image was not
            // found.
            if ((response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Moved &&
                 response.StatusCode != HttpStatusCode.Redirect) ||
                !response.ContentType.StartsWith("image", StringComparison.OrdinalIgnoreCase)) return false;
            // if the remote file was found, download it
            using (var inputStream = response.GetResponseStream())
            using (var outputStream = File.OpenWrite(fileName))
            {
                var buffer = new byte[4096];
                int bytesRead;
                do
                {
                    bytesRead = inputStream.Read(buffer, 0, buffer.Length);
                    outputStream.Write(buffer, 0, bytesRead);
                } while (bytesRead != 0);
            }
            return true;
        }
    }
}
