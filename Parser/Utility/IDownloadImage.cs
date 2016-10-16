using System.Collections.Generic;

namespace Utility
{
    public interface IDownloadImage
    {
        void Download(string dirName, IEnumerable<ImageDownloadCommand> commands);
    }
}