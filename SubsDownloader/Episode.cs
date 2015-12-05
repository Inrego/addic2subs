using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubsDownloader
{
    public class Episode
    {
        public Episode(MediaFile file, Language[] languages, string originalFilename)
        {
            var possibleNames = new[]
            {
                Path.GetFileNameWithoutExtension(originalFilename),
                Path.GetFileNameWithoutExtension(file.FilePath),
                Path.GetFileName(Path.GetDirectoryName(originalFilename)),
                Path.GetFileName(Path.GetDirectoryName(file.FilePath))
            };

            foreach (var name in possibleNames)
            {
                try
                {
                    ParseFileName(name);
                }
                catch (Exception)
                {
                    continue;
                }
                break;
            }
            
            Languages = languages;
            File = file;
            OriginalFilename = originalFilename;
        }

        public string OriginalFilename { get; set; }
        public string ShowName { get; set; }
        public short Season { get; set; }
        public short EpisodeNo { get; set; }
        public string[] Groups { get; set; }
        public Language[] Languages { get; set; }
        public MediaFile File { get; set; }
        public Source Source { get; set; }

        private void ParseFileName(string fileName)
        {
            if (fileName.IndexOf("dvdrip", StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                Source = Source.DVDRip;
            }
            else if (fileName.IndexOf("bluray", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                     fileName.IndexOf("blueray", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                     fileName.IndexOf("blu-ray", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                     fileName.IndexOf("blue-ray", StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                Source = Source.BluRay;
            }
            else if (fileName.IndexOf("web-dl", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                     fileName.IndexOf("webdl", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                     fileName.IndexOf("web.dl", StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                Source = Source.WebDL;
            }
            else if (fileName.IndexOf("hdtv", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                     fileName.IndexOf("hd-tv", StringComparison.CurrentCultureIgnoreCase) >= 0 ||
                     fileName.IndexOf("hd.tv", StringComparison.CurrentCultureIgnoreCase) >= 0)
            {
                Source = Source.HDTV;
            }
            else
            {
                Source = Source.Unknown;
            }

            string regex = @"^(?<showName>[a-z0-9\.\-]+).S(?<season>[0-9]+)E(?<episode>[0-9]+).+\.[^\-]*\-(?<groups>[\-a-z0-9]+)";
            var regexStandard = new Regex(regex, RegexOptions.IgnoreCase);
            Match episode = regexStandard.Match(fileName);

            if (!episode.Success)
                throw new ParsingException("The regex did not match.", this);

            try
            {
                ShowName = episode.Groups["showName"].Value.Replace('.', ' ');
            }
            catch (Exception e)
            {
                throw new ParsingException("There was an error parsing showName: \n" + e, this);
            }
            try
            {
                Season = Convert.ToInt16(episode.Groups["season"].Value);
            }
            catch (Exception e)
            {
                throw new ParsingException("There was an error parsing season: \n" + e, this);
            }
            try
            {
                EpisodeNo = Convert.ToInt16(episode.Groups["episode"].Value);
            }
            catch (Exception e)
            {
                throw new ParsingException("There was an error parsing episode: \n" + e, this);
            }
            try
            {
                Groups = episode.Groups["groups"].Value.Split('-');
            }
            catch (Exception e)
            {
                throw new ParsingException("There was an error parsing groups: \n" + e, this);
            }
        }

        public class ParsingException : Exception
        {
            public Episode Episode { get; private set; }
            public ParsingException(string reason, Episode episode) : base(reason)
            {
                Episode = episode;
            }
        }
    }
    public enum Source
    {
        HDTV,
        WebDL,
        DVDRip,
        BluRay,
        Unknown
    }
}
