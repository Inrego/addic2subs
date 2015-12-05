using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace SubsDownloader.Websites.Addic7ed
{
    public class Plugin
    {
        private Episode[] _episodes;
        public Plugin(IEnumerable<Episode> episodes)
        {
            _episodes = episodes.ToArray();
            short season = -1;
            int showId = -1;
            foreach (var episode in _episodes)
            {
                if (season == -1)
                {
                    season = episode.Season;
                }
                if (showId == -1)
                {
                    showId = episode.Show.Id;
                }
                if (season != episode.Season)
                    throw new ArgumentException(string.Format("No more than 1 season at a time is allowed. Collection contains season {0} and {1}", season, episode.Season), "episodes");
                if (showId != episode.Show.Id)
                {
                    var shows = _episodes.GroupBy(e => e.Show.Id).Select(g => g.FirstOrDefault().ShowName);
                    throw new ArgumentException(string.Format("It is only possible to search for one season in one show at a time. Collection contains the following shows: " + string.Join(", ", shows)));
                }
            }
        }
        
        public List<DownloadableSubtitle> GetDownloadLinks()
        {
            var url = string.Format(@"http://www.addic7ed.com/ajax_loadShow.php?show={0}&season={1}", _episodes.FirstOrDefault().Show.Id, _episodes.FirstOrDefault().Season);
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var seasonTable = doc.GetElementbyId("season");
            for (var i = 0; i < 5 && seasonTable == null; i++)
            {
                Thread.Sleep(2000);
                doc = web.Load(url);
                seasonTable = doc.GetElementbyId("season");
            }
            if (seasonTable == null)
                return new List<DownloadableSubtitle>();
            var tableBody = seasonTable.Descendants("tbody");

            var downloadableSubtitles = new List<DownloadableSubtitle>();
            foreach (var episode in _episodes)
            {
                foreach (var language in episode.Languages)
                {
                    short currentEpisode;
                    var episodeRows =
        tableBody.FirstOrDefault().Elements("tr").Where(
            row => row.HasChildNodes && short.TryParse(row.ChildNodes[1].InnerText, out currentEpisode) && currentEpisode == episode.EpisodeNo);

                    var correctGroupRows = (
                        from row in episodeRows
                        let rowGroups = HtmlEntity.DeEntitize(row.ChildNodes[4].InnerText).Trim().Split(new []{' ', '-', '.'})
                        let directMatch = episode.Groups.Intersect(rowGroups, StringComparer.CurrentCultureIgnoreCase).Any()
                        let groupMatch = directMatch ? directMatch : SubsDownloader.Config.Instance.ReleaseGroupRelations.Any(
                            grpRel => grpRel.Groups.Intersect(episode.Groups, StringComparer.CurrentCultureIgnoreCase).Any()
                                && grpRel.Groups.Any(@group => rowGroups.Contains(@group, StringComparer.CurrentCultureIgnoreCase))
                        )
                        where groupMatch
                        select row);

                    var validSubs = correctGroupRows.Where(row => language.Values.Contains(row.ChildNodes[3].InnerText, StringComparer.CurrentCultureIgnoreCase));

                    HtmlNode chosenRow;
                    if (SubsDownloader.Config.Instance.Addic7edConfig.PreferHI)
                    {
                        chosenRow = validSubs.FirstOrDefault(row => !string.IsNullOrWhiteSpace(row.ChildNodes[6].InnerText)) ??
                                  validSubs.FirstOrDefault();
                    }
                    else
                    {
                        chosenRow = validSubs.FirstOrDefault(row => string.IsNullOrWhiteSpace(row.ChildNodes[6].InnerText)) ??
                                    validSubs.FirstOrDefault();
                    }
                    var downloadUrl = chosenRow == null ? null : @"http://www.addic7ed.com" + chosenRow.Elements("td").ElementAt(9).FirstChild.GetAttributeValue("href", null);
                    if (downloadUrl != null)
                    {
                        downloadableSubtitles.Add(new DownloadableSubtitle(downloadUrl, language, episode));
                    }
                }
            }
            return downloadableSubtitles;
        }
        public static ShowsCollection LoadAddic7edShowIds()
        {
            var url = @"http://www.addic7ed.com/";
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var showsDropdownOptions = doc.GetElementbyId("qsShow").Elements("option");

            var shows =
                showsDropdownOptions.Where(option => option.GetAttributeValue("value", 0) != 0)
                    .Select(
                        show =>
                            new Show
                            {
                                DisplayName = show.NextSibling.InnerText,
                                Id = show.GetAttributeValue("value", 0),
                                KeyName = Regex.Replace(show.NextSibling.InnerText.ToUpper().Replace('.', ' ').Trim(), @"[^A-Za-z0-9\s\-]", "")
                            });
            var showsCollection = new ShowsCollection();
            showsCollection.AddRange(shows);

            return showsCollection;
        }
    }
}
