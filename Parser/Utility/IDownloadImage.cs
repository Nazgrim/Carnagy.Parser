using System.Collections.Generic;

namespace Utility
{
    public interface IDownloadImage
    {
        void Download(string dir, IEnumerable<ImageForSave> images);
    }
}