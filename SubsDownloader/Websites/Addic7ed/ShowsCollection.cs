using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubsDownloader.Websites.Addic7ed
{
    public class ShowsCollection : List<Show>
    {
        public Show this[string keyName]
        {
            get
            {
                return this.FirstOrDefault(show => show.KeyName == keyName);
            }
        }

        public Show FindById(int id)
        {
            return this.FirstOrDefault(show => show.Id == id);
        }
    }
}
