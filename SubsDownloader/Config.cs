using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SubsDownloader
{
    [XmlType("GeneralConfig")]
    public class Config
    {
        public static Config Instance { get; set; }

        private Websites.Addic7ed.Config _addic7edConfig = new Websites.Addic7ed.Config();
        
        public Websites.Addic7ed.Config Addic7edConfig { get { return _addic7edConfig; } set{_addic7edConfig = value;} }

        private string _unknownLanguage = "en";
        public string UnknownLanguage {get { return _unknownLanguage; } set { _unknownLanguage = value; }}

        private string _runProgramAfterQueue = "";
        public string RunProgramAfterQueue {get { return _runProgramAfterQueue; } set { _runProgramAfterQueue = value; }}

        private string[] _foldersToScan = new string[0];
        public string[] FoldersToScan { get { return _foldersToScan; } set { _foldersToScan = value; } }

        private string[] _ignoredFiles = new string[0];
        public string[] IgnoredFiles { get { return _ignoredFiles; } set { _ignoredFiles = value; } }

        private string[] _mediaFileExtensions;
        public string[] MediaFileExtensions { get { return _mediaFileExtensions; } set { _mediaFileExtensions = value; } }

        private bool _appendLanguageCode = true;
        public bool AppendLanguageCode { get { return _appendLanguageCode; } set { _appendLanguageCode = value; } }

        private List<QueuedEpisode> _queuedEpisodes = new List<QueuedEpisode>();
        public List<QueuedEpisode> QueuedEpisodes {get { return _queuedEpisodes; } set { _queuedEpisodes = value; }}

        private int _maxTries = 10;
        public int MaxTries {get { return _maxTries; } set { _maxTries = value; }}

        public List<Language> LanguageCollection { get { return _languageCollection; } set {_languageCollection = value;} }
        private List<Language> _languageCollection;

        private List<RlsGrpRelation> _releaseGroupRelations;
        public List<RlsGrpRelation> ReleaseGroupRelations {get { return _releaseGroupRelations; } set {_releaseGroupRelations = value;}} 

        private static string FileLocation 
        {
            get
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(Websites.Addic7ed.Config)).Location), "config.xml");
            }
        }
        static Config()
        {
            try
            {
                Instance = ObjectXMLSerializer<Config>.Load(FileLocation);
            }
            catch (Exception)
            {
                Instance = new Config();
                Instance.MediaFileExtensions = new[] {".avi", ".mp4", ".mkv"};
                Instance.LanguageCollection = new List<Language>
                {
                    new Language
                    {
                        DisplayName = "English",
                        PrimaryValue = "en",
                        Values = new[] {"en", "eng", "en-US", "en-GB", "English", "English (US)", "English (UK)"},
                        LookForSubtitles = true
                    },
                    new Language
                    {
                        DisplayName = "Danish",
                        PrimaryValue = "da",
                        Values = new[] {"dk", "da", "dan", "dk-dk", "Danish"}
                    },
                    new Language
                    {
                        DisplayName = "French",
                        PrimaryValue = "fr",
                        Values = new[] {"fr", "fre", "fr-fr", "French"}
                    },
                    new Language
                    {
                        DisplayName = "German",
                        PrimaryValue = "de",
                        Values = new[] {"de", "ger", "de-de", "German"}
                    },
                    new Language
                    {
                        DisplayName = "Dutch",
                        PrimaryValue = "nl",
                        Values = new[] {"nl", "nl-nl", "Dutch"}
                    },
                    new Language
                    {
                        DisplayName = "Italian",
                        PrimaryValue = "it",
                        Values = new[] {"it", "it-it", "Italian"}
                    },
                    new Language
                    {
                        DisplayName = "Spanish",
                        PrimaryValue = "es",
                        Values = new[] {"es", "es-es", "Spanish", "Spanish (Spain)"}
                    },
                    new Language
                    {
                        DisplayName = "Swedish",
                        PrimaryValue = "se",
                        Values = new[] {"se", "se-se", "swe", "Swedish"}
                    },
                    new Language
                    {
                        DisplayName = "Norwegian",
                        PrimaryValue = "no",
                        Values = new[] {"no", "no-no", "nor", "Norwegian"}
                    }
                };
                Instance.ReleaseGroupRelations = new List<RlsGrpRelation>
                {
                    new RlsGrpRelation
                    {
                        Groups = new[]
                        {
                            "LOL",
                            "SYS",
                            "DIMENSION"
                        }
                    },
                    new RlsGrpRelation
                    {
                        Groups = new[]
                        {
                            "XII",
                            "IMMERSE",
                            "ASAP"
                        }
                    },
                    new RlsGrpRelation
                    {
                        Groups = new[]
                        {
                            "FQM",
                            "ORENJI"
                        }
                    },
                    new RlsGrpRelation
                    {
                        Groups = new[]
                        {
                            "REMARKABLE",
                            "EXCELLENCE"
                        }
                    }
                };
                Save();
            }
        }

        public static void Save()
        {
            ObjectXMLSerializer<Config>.Save(Instance, FileLocation);
        }
    }
}
