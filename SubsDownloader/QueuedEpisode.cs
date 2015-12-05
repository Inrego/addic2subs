using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubsDownloader
{
    public class QueuedEpisode
    {
        public string OriginalFilename { get; set; }
        public string FilePath { get; set; }
        public DateTime AddedAt { get; set; }
        public int NumberOfTries { get; set; }
    }
}
