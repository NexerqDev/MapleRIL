using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapleRIL.Common
{
    public class RILFileManager : Dictionary<string, WzFile>
    {
        public string Region;

        public RILFileManager(string region, string path, string[] files)
            : base()
        {
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
    }
}
