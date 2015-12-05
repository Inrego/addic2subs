using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SubsDownloader.Processing
{
    public static class QueueProcessor
    {
        public static void HandleQueue()
        {
            var indices = new List<int>();
            for (var i = 0; i < Config.Instance.QueuedEpisodes.Count; i++)
            {
                var queuedEpisode = Config.Instance.QueuedEpisodes[i];
                try
                {
                    var fileProcessor = new FileProcessor(queuedEpisode.FilePath, queuedEpisode.OriginalFilename);
                    if (!fileProcessor.ValidFile)
                    {
                        indices.Add(i);
                        continue;
                    }
                    if (!fileProcessor.MissingSubtitleLanguages.Any())
                    {
                        indices.Add(i);
                        continue;
                    }
                    fileProcessor.DownloadSubtitles();
                    if (!fileProcessor.MissingSubtitleLanguages.Any())
                    {
                        indices.Add(i);
                        continue;
                    }
                    queuedEpisode.NumberOfTries++;
                    if (queuedEpisode.NumberOfTries >= Config.Instance.MaxTries)
                    {
                        indices.Add(i);
                    }
                }
                catch (Exception e)
                {
                    throw new Exception("Error (see inner exception) caused by file: " + queuedEpisode.FilePath, e);
                }
            }
            foreach (var index in indices.OrderByDescending(index => index))
            {
                Config.Instance.QueuedEpisodes.RemoveAt(index);
            }
            Config.Save();
            if (!string.IsNullOrEmpty(Config.Instance.RunProgramAfterQueue))
            {
                string path;
                if (Path.IsPathRooted(Config.Instance.RunProgramAfterQueue))
                {
                    path = Config.Instance.RunProgramAfterQueue;
                }
                else
                {
                    path =
                        Path.Combine(
                            Path.GetDirectoryName(Assembly.GetAssembly(typeof (Websites.Addic7ed.Config)).Location),
                            Config.Instance.RunProgramAfterQueue);
                }
                Process.Start(path);
            }
        }

        public static bool IsInQueue(QueuedEpisode queuedEpisode)
        {
            return
                Config.Instance.QueuedEpisodes.Any(
                    e => e.FilePath.Equals(queuedEpisode.FilePath, StringComparison.CurrentCultureIgnoreCase));
        }

        public static void AddToQueue(QueuedEpisode queuedEpisode)
        {
            var alreadyQueued =
                Config.Instance.QueuedEpisodes.FirstOrDefault(
                    e => e.FilePath.Equals(queuedEpisode.FilePath, StringComparison.CurrentCultureIgnoreCase));
            if (alreadyQueued == null)
            {
                Config.Instance.QueuedEpisodes.Add(queuedEpisode);
            }
            else
            {
                alreadyQueued.NumberOfTries = 1;
                alreadyQueued.AddedAt = DateTime.Now;
            }
            Config.Save();
        }
    }
}
