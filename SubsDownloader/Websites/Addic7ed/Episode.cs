using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubsDownloader.Websites.Addic7ed
{
    public class Episode : SubsDownloader.Episode
    {
        public Episode(MediaFile file, Language[] languages, string originalFilename) : base(file, languages, originalFilename)
        {
            Show = SubsDownloader.Config.Instance.Addic7edConfig.Addic7edShows.FromKey(ShowName.ToUpper());
            if (Show == null)
            {
                Show = SubsDownloader.Config.Instance.Addic7edConfig.CustomAddic7edShows.FromKey(ShowName.ToUpper());
            }
            if (Show == null)
            {
                SubsDownloader.Config.Instance.Addic7edConfig.Addic7edShows = Plugin.LoadAddic7edShowIds();
                SubsDownloader.Config.Save();

                Show = SubsDownloader.Config.Instance.Addic7edConfig.Addic7edShows.FromKey(ShowName.ToUpper());
            }
        }

        public Show Show { get; set; }
    }
}
