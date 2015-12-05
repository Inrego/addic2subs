using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NEbml.Core;
using SubsDownloader.Matroska;

namespace SubsDownloader
{
    public class MediaFile
    {
        public string FilePath { get; private set; }

        public MediaFile(string filePath)
        {
            FilePath = filePath;
        }

        public List<string> EmbeddedSubtitleLanguages
        {
            get
            {
                try
                {
                    if (Path.GetExtension(FilePath.ToUpper()) != ".MKV")
                        return new List<string>();
                    var medp = new MatroskaElementDescriptorProvider();
                    List<string> lstLanguages = new List<string>();

                    using (var fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                    using (EbmlReader ebmlReader = new EbmlReader(fs))
                    {
                        var segmentFound = ebmlReader.LocateElement(MatroskaElementDescriptorProvider.Segment);
                        if (segmentFound)
                        {
                            ebmlReader.EnterContainer();
                            while (ebmlReader.ReadNext())
                            {
                                var descriptor = medp.GetElementDescriptor(ebmlReader.ElementId);
                                if (descriptor == null) continue;

                                if (descriptor.Name == "Tracks")
                                {
                                    ebmlReader.EnterContainer();
                                    while (ebmlReader.ReadNext())
                                    {
                                        var trackDescriptor = medp.GetElementDescriptor(ebmlReader.ElementId);
                                        if (trackDescriptor == null) continue;

                                        if (trackDescriptor.Name == "TrackEntry")
                                        {
                                            ebmlReader.EnterContainer();

                                            long trackType = 0;
                                            string trackLanguage = null;

                                            while (ebmlReader.ReadNext())
                                            {
                                                var trackEntryDescriptor = medp.GetElementDescriptor(ebmlReader.ElementId);
                                                if (trackEntryDescriptor == null) continue;

                                                if (trackEntryDescriptor.Name == "TrackType")
                                                {
                                                    trackType = ebmlReader.ReadInt();
                                                }
                                                else if (trackEntryDescriptor.Name == "Language")
                                                {
                                                    trackLanguage = ebmlReader.ReadUtf();
                                                }
                                            }

                                            if (trackType == 0x11) //subtitle
                                            {
                                                lstLanguages.Add(trackLanguage ?? "en");
                                            }

                                            ebmlReader.LeaveContainer();
                                        }
                                    }
                                    ebmlReader.LeaveContainer();
                                    break;
                                }
                            }
                        }
                    }

                    return lstLanguages;
                }
                catch (Exception)
                {
                    return new List<string>();
                }
            }
        }

        public List<string> ExternalSubtitleLanguages
        {
            get
            {
                var folder = Path.GetDirectoryName(FilePath);
                var fileName = Path.GetFileNameWithoutExtension(FilePath);

                // Source: http://en.wikipedia.org/wiki/Subtitle_(captioning)#For_software_video_players
                var subtitleExtensions = new[]
                {
                    ".AQT", ".JSS", ".SUB", ".TTXT", ".PJS", ".PSB", ".RT", ".SMI", ".SSF", ".GSUB", ".SSA", ".ASS", ".USF",
                    ".IDX", ".SRT"
                };

                var subtitleFiles = (
                    from file in Directory.GetFiles(folder)
                    let baseName = Path.GetFileNameWithoutExtension(file)
                    let ext = Path.GetExtension(file)
                    where
                        baseName.StartsWith(fileName) &&
                        subtitleExtensions.Contains(ext, StringComparer.InvariantCultureIgnoreCase)
                    select baseName);


                var subtitleLanguages = (
                    from file in subtitleFiles
                    let langExt = file.Replace(fileName, "")
                    let lang = string.IsNullOrEmpty(langExt) ? Config.Instance.UnknownLanguage : langExt
                    select lang.Replace(".", "")
                    ).Distinct().ToList();

                return subtitleLanguages;
            }
        }

        public List<string> GetAllSubtitleLanguages
        {
            get
            {
                return EmbeddedSubtitleLanguages.Union(ExternalSubtitleLanguages).Distinct().ToList();
            }
        }
    }
}
