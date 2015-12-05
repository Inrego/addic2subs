using System;
using System.IO;
using System.Linq;
using SubsDownloader;
using SubsDownloader.Processing;

namespace SonarrScript
{
    class Program
    {
        static void Main(string[] args)
        {
            var debug = args.Any(s => s.ToLower() == "debug");
            debug = true;
            try
            {
                var sonarrEventType = Environment.GetEnvironmentVariable("sonarr_eventtype");
                if ((args.Length == 0 || string.IsNullOrEmpty(args[0])) && string.IsNullOrEmpty(sonarrEventType))
                {
                    if (debug)
                        Logger.WriteLine("Handling queue");
                    QueueProcessor.HandleQueue();
                }
                else
                {
                    var filePath = string.IsNullOrEmpty(sonarrEventType)
                        ? args[0]
                        : Environment.GetEnvironmentVariable("sonarr_episodefile_path");
                    var originalName = string.IsNullOrEmpty(sonarrEventType)
                        ? (args.Length == 6 ? args[1] : null)
                        : $"{Environment.GetEnvironmentVariable("sonarr_episodefile_scenename")}{Path.GetExtension(filePath)}";
                    if (debug)
                    {
                        Logger.WriteLine("FilePath: {0}", filePath);
                        Logger.WriteLine("OriginalName: {0}", originalName);
                    }
                    FileAttributes fileAttr;
                    try
                    {
                        fileAttr = File.GetAttributes(filePath);
                    }
                    catch (Exception)
                    {
                        Logger.WriteLine("The provided path was not found.");
                        return;
                    }
                    if (fileAttr.HasFlag(FileAttributes.Directory))
                    {
                        var folderProcessor = new FolderProcessor(filePath, true);
                        if (folderProcessor.ValidFolder)
                            folderProcessor.DownloadSubtitles(true);
                    }
                    else
                    {
                        var fileProcessor = new FileProcessor(filePath, originalName);
                        if (fileProcessor.ValidFile)
                            fileProcessor.DownloadSubtitles(true);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.WriteLine("An error occurred: {0}", e);
            }
        }
    }
}
