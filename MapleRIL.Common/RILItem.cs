using MapleLib.WzLib;
using MapleRIL.Common.RILItemType;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MapleRIL.Common
{
    public class RILItem
    {
        public RILFileManager OriginatingFileManager;

        public WzImageProperty StringWzProperty; // this is the overall property, its name is the items id

        private bool _iwpDone = false;
        private WzImageProperty _iwp;
        public WzImageProperty InfoWzProperty
        {
            get
            {
                if (!_iwpDone)
                {
                    try
                    {
                        _iwp = ItemType.GetInfoPropertyById(OriginatingFileManager, Id);
                    }
                    catch { _iwp = null; }
                    _iwpDone = true;
                }

                return _iwp;
            }
        }

        public RILBaseItemType ItemType { get; private set; }

        public string Id => StringWzProperty.Name;
        public string Name => StringWzProperty["name"].GetString();
        public string Category => ItemType.FriendlyName;
        public string RawDescription => ItemType.GetDescription(StringWzProperty, InfoWzProperty);

        public string Description
        {
            get
            {
                return RawDescription
                    .Replace("\\r", "")
                    .Replace("\\n", "\n");
            }
        }

        private Regex orangeDescRegex = new Regex("#c(.*?)#", RegexOptions.Singleline); // wait so singleline is the one that matches multiline strings ok then fuck thats confusing
        private Regex orangeDescToEndRegex = new Regex("(?!#c(.*?)#)#c(.*?)[^#]$", RegexOptions.Singleline);
        /// <summary>
        /// Includes the orange text as an array. [White text, Orange text, White ...]
        /// </summary>
        public string[] ParsedDescription
        {
            get
            {
                string d = Description;

                // sometimes you can have #c......... to the end so have to color the whole thing
                if (orangeDescToEndRegex.IsMatch(d))
                    d += "#"; // just add it back save the headache

                MatchCollection matches = orangeDescRegex.Matches(d);
                if (matches.Count < 1)
                    return new string[] { d };

                var l = new List<string>();
                // now is the intense shit
                // eg string "do this and #cDouble-click#" - index 12 - so go up to 12 (will include the space as substring is length)
                l.Add(d.Substring(0, matches[0].Index));
                l.Add(d.Substring(matches[0].Index + 2, matches[0].Length - 3)); // skip the #c (+2) and remove #c and #'s length (-3)
                if (matches.Count > 1) // more than 1 match we need to do some tricks - basically last regex's index+length to the next index is regular then orange
                {
                    for (int i = 1; i < matches.Count; i++)
                    {
                        int continueIndex = matches[i - 1].Index + matches[i - 1].Length; // the index after the previous match
                        l.Add(d.Substring(continueIndex, matches[i].Index - continueIndex)); // go up to before the next orange
                        l.Add(d.Substring(matches[i].Index + 2, matches[i].Length - 3)); // same as above, process the orange
                    }
                }
                if (matches[matches.Count - 1].Index + matches[matches.Count - 1].Length < d.Length)
                {
                    // theres still more text to go as normal after last match as the index + regex match length added is less than the entire length
                    int continueIndex = matches[matches.Count - 1].Index + matches[matches.Count - 1].Length;
                    l.Add(d.Substring(continueIndex)); // just from teh index to the end
                }

                return l.ToArray();
            }
        }

        public System.Drawing.Bitmap Icon
        {
            get
            {
                var bmp = ItemType.GetIcon(InfoWzProperty);
                if (bmp.Height == 1 && bmp.Width == 1) // empty pixel, this means no actual icon
                    return null;

                return bmp;
            }
        }

        // take the rfm as well to load info stuff for this item
        public RILItem(RILFileManager rfm, WzImageProperty wzStrProp, RILBaseItemType type)
        {
            OriginatingFileManager = rfm;
            StringWzProperty = wzStrProp;
            ItemType = type;
        }

        public RILItem TryLoadInOtherFileManager(RILFileManager rfm)
        {
            WzImageProperty remoteProp = ItemType.GetStringPropertyById(rfm, Id);
            if (remoteProp == null)
                return null;
            return new RILItem(rfm, remoteProp, ItemType);
        }
    }
}
