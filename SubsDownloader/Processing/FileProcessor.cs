using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace SubsDownloader.Processing
{
    public class FileProcessor
    {
        public bool ValidFile { get; private set; }
        public string FilePath { get; private set; }

        public readonly Websites.Addic7ed.Episode Episode;
        private readonly MediaFile _mediaFile;

        public FileProcessor(string filePath, string originalFilename = null)
        {
            var ext = Path.GetExtension(filePath);
            ValidFile = Config.Instance.MediaFileExtensions.Contains(ext, StringComparer.CurrentCultureIgnoreCase) && File.Exists(filePath);
            FilePath = filePath;
            _mediaFile = new MediaFile(filePath);

            originalFilename = originalFilename ?? Path.GetFileName(filePath);
            Episode = new Websites.Addic7ed.Episode(_mediaFile, MissingSubtitleLanguages.ToArray(), originalFilename);
        }

        public void DownloadSubtitles(bool addToQueue = false)
        {
            if (!ValidFile)
                return;
            var plugin = new Websites.Addic7ed.Plugin(new[] { Episode });
            var downloadableSubtitles = plugin.GetDownloadLinks();
            bool success;
            try
            {
                foreach (var downloadableSubtitle in downloadableSubtitles)
                {
                    var downloader = new SubtitleDownloader(downloadableSubtitle);
                    downloader.DownloadSubtitle();
                }
                success = true;
            }
            catch (Exception)
            {
                success = false;
            }
            if (addToQueue && (!success || MissingSubtitleLanguages.Any()))
            {
                var queuedEpisode = new QueuedEpisode
                {
                    AddedAt = DateTime.Now,
                    FilePath = FilePath,
                    NumberOfTries = 1,
                    OriginalFilename = Episode.OriginalFilename
                };
                QueueProcessor.AddToQueue(queuedEpisode);
            }
        }

        public IEnumerable<Language> MissingSubtitleLanguages
        {
            get
            {
                var enabledLanguages = Config.Instance.LanguageCollection.Where(l => l.LookForSubtitles);
                var currentLanguages = _mediaFile.GetAllSubtitleLanguages;
                var neededLanguages =
                    enabledLanguages.Where(
                        enabledLanguage =>
                            !currentLanguages.Any(
                                currentLanguage =>
                                    enabledLanguage.Values.Contains(currentLanguage, StringComparer.CurrentCultureIgnoreCase)));

                return neededLanguages;
            }
        }

        public List<string> GetSubtitles()
        {
            return _mediaFile.GetAllSubtitleLanguages;
        }
    }
}
