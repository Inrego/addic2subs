using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using SubsDownloader.Websites.Addic7ed;

namespace SubsDownloader.Processing
{
    public class SubtitleDownloader
    {
        private readonly DownloadableSubtitle _downloadableSubtitle;
        private static CookieContainer _cookies;

        static SubtitleDownloader()
        {
            if (!string.IsNullOrEmpty(Config.Instance.Addic7edConfig.Username) &&
                !string.IsNullOrEmpty(Config.Instance.Addic7edConfig.Password))
            {
                using (var wc = new MyWebClient())
                {
                    var url = @"http://www.addic7ed.com/dologin.php";
                    wc.UploadValues(new Uri(url),
                        new NameValueCollection
                        {
                            {"username", Config.Instance.Addic7edConfig.Username},
                            {"password", Config.Instance.Addic7edConfig.Password},
                            {"remember", "false"}
                        });
                    _cookies = wc.Cookie;
                }
            }
        }

        public SubtitleDownloader(DownloadableSubtitle downloadableSubtitle)
        {
            _downloadableSubtitle = downloadableSubtitle;
        }

        public void DownloadSubtitle()
        {
            var downloadFolder = Path.GetDirectoryName(_downloadableSubtitle.Episode.File.FilePath);
            var fileName = Path.GetFileNameWithoutExtension(_downloadableSubtitle.Episode.File.FilePath);
            var lanAppend = Config.Instance.AppendLanguageCode ? "." + _downloadableSubtitle.Language.PrimaryValue : "";
            var filePath = Path.Combine(downloadFolder, string.Format("{0}{1}.srt", fileName, lanAppend));

            using (var wc = new MyWebClient(_cookies))
            {
                wc.Headers.Add("Referer", _downloadableSubtitle.Url);
                wc.DownloadFile(_downloadableSubtitle.Url, filePath);
            }
        }
    }
}
