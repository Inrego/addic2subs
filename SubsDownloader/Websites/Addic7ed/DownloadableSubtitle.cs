using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubsDownloader.Websites.Addic7ed
{
    public class DownloadableSubtitle
    {
        public string Url { get; private set; }
        public Language Language { get; private set; }
        public Episode Episode { get; private set; }

        public DownloadableSubtitle(string url, Language language, Episode episode)
        {
            Url = url;
            Language = language;
            Episode = episode;
        }
    }
}
