using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubsDownloader.Processing
{
    public class FolderProcessor
    {
        public string FolderPath { get; private set; }
        public bool ValidFolder { get; private set; }
        public bool CheckSubFolders { get; private set; }

        public FolderProcessor(string folderPath, bool subFolders)
        {
            FolderPath = folderPath;
            ValidFolder = Directory.Exists(folderPath);
            CheckSubFolders = subFolders;
        }

        public void DownloadSubtitles(bool addToQueue = false)
        {
            var mediaFiles = new List<string>();
            var searchOption = CheckSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            foreach (var mediaExt in Config.Instance.MediaFileExtensions)
            {
                var files = Directory.GetFiles(FolderPath, "*" + mediaExt, searchOption);
                mediaFiles.AddRange(files.Where(file => !Config.Instance.IgnoredFiles.Contains(file)));
            }

            foreach (var mediaFile in mediaFiles)
            {
                var fileProcessor = new FileProcessor(mediaFile);
                if (fileProcessor.ValidFile)
                    fileProcessor.DownloadSubtitles(addToQueue);
            }
        }
    }
}
