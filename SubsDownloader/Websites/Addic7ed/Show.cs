using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubsDownloader.Websites.Addic7ed
{
    public class Show
    {
        public int Id { get; set; }
        public string KeyName { get; set; }
        public string DisplayName { get; set; }
    }

    public static class Extension
    {
        public static Show FromKey(this List<Show> showList, string key)
        {
            return showList.FirstOrDefault(show => show.KeyName == key);
        }

        public static Show FromId(this List<Show> showList, int id)
        {
            return showList.FirstOrDefault(show => show.Id == id);
        }
    }
}
