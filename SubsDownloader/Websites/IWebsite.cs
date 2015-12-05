using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubsDownloader.Websites
{
    interface IWebsite
    {
        string GetDownloadLink(Episode episode, bool hi);
    }
}
