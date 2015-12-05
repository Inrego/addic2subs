using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SubsDownloader.Websites.Addic7ed
{
    [XmlType("Addic7edConfig")]
    public class Config
    {
        private ShowsCollection _addic7edShows = new ShowsCollection();
        public ShowsCollection Addic7edShows { get { return _addic7edShows; } set { _addic7edShows = value; } }

        private ShowsCollection _customAddic7edShows = new ShowsCollection();
        public ShowsCollection CustomAddic7edShows { get { return _customAddic7edShows; } set{_customAddic7edShows = value;} }

        private bool _preferHI = false;
        public bool PreferHI { get { return _preferHI; } set { _preferHI = value; } }

        private string _username;
        public string Username { get { return _username; } set { _username = value; } }

        private string _password;
        public string Password { get { return _password; } set { _password = value; } }
    }
}
