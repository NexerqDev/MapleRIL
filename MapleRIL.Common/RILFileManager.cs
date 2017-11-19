using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Common
{
    public class RILFileManager : Dictionary<string, WzFile>
    {
        public static string[] RequiredWzs = new string[] { "String.wz", "Item.wz", "Character.wz" };

        public string RootPath;
        public string Region;

        public RILFileManager(string region, string path, string[] files = null)
            : base()
        {
            if (files == null)
                files = RequiredWzs;

            RootPath = path;
            Region = region;

            foreach (string w in files)
            {
                // gms, kms, msea all use classic encryption now
                WzFile source = new WzFile(Path.Combine(path, w), WzMapleVersion.CLASSIC);
                source.ParseWzFile();
                this.Add(w, source);
            }
        }

        public string[] LoadedFiles => Values.Select(v => v.FilePath).ToArray();

        private string _version = null;
        public string GameVersion
        {
            get
            {
                if (_version != null)
                    return _version;

                if (File.Exists(Path.Combine(this.RootPath, "MapleStory.exe")))
                {
                    // get ver # from exe
                    // minor.buid gives number
                    FileVersionInfo v = FileVersionInfo.GetVersionInfo(Path.Combine(this.RootPath, "MapleStory.exe"));
                    return $"v{v.ProductMinorPart}.{v.ProductBuildPart}";
                }

                if (File.Exists(Path.Combine(this.RootPath, "ril_ver.txt")))
                {
                    // get from our txt if we just had wzs
                    return File.ReadAllText(Path.Combine(this.RootPath, "ril_ver.txt"));
                }

                return "unknown";
            }
        }
    }
}
