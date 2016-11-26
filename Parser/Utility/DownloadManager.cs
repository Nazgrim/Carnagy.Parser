using System.Net;

namespace Utility
{
    public class DownloadManager : IDownloadManager
    {
        public string DownloadString(string url)
        {
            using (var myWebClient = new WebClient())
            {
                return myWebClient.DownloadString(url);
            }
        }
    }
}
