using MapleLib.WzLib;
using MapleRIL.Structure;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MapleRIL
{
    /// <summary>
    /// Interaction logic for Comparison.xaml
    /// </summary>
    public partial class Comparison : Window
    {
        MainWindow _mw;
        SearchedItem Item;

        public Comparison(MainWindow mw, SearchedItem si)
        {
            InitializeComponent();

            _mw = mw;
            Item = si;

            lookupLabel.Content = "Lookup: ID " + si.Id.ToString();

            // source lookup
            sourceRegionLabel.Content = _mw.SourceRegion;
            sourceNameLabel.Content = si.Name;
            safeDescAndParse(sourceDescBlock, si.SourceWzProperty["desc"]);
            WzImageProperty sourceInfo = si.ItemType.GetInfoPropertyById(_mw.SourceWzs, si.Id);
            try
            {
                sourceImage.Source = wpfImage(sourceInfo["icon"].GetBitmap());
            }
            catch { }

            // target lookup
            targetRegionLabel.Content = _mw.TargetRegion;
            WzImageProperty targetStringProp = si.ItemType.GetStringPropertyById(_mw.TargetWzs, si.Id);
            if (targetStringProp == null)
            {
                targetNotExist();
                return;
            }

            targetNameLabel.Content = targetStringProp["name"].GetString();
            safeDescAndParse(targetDescBlock, targetStringProp["desc"]);
            WzImageProperty targetInfo = si.ItemType.GetInfoPropertyById(_mw.TargetWzs, si.Id);
            try
            {
                targetImage.Source = wpfImage(sourceInfo["icon"].GetBitmap());
            }
            catch { }
        }

        private string buildEquipDescription(WzImageProperty infoProp, string category)
        {
            string desc = "WARNING: Equip stats are still WIP - a lot of stats may be missing!\n\n";
            desc += "REQ LEV: " + (infoProp["reqLevel"] == null ? "0" : infoProp["reqLevel"].GetInt().ToString());
            desc += "\n";
            desc += "REQ STR: " + (infoProp["reqSTR"] == null ? "0" : infoProp["reqSTR"].GetInt().ToString());
            desc += "   ";
            desc += "REQ LUK: " + (infoProp["reqLUK"] == null ? "0" : infoProp["reqLUK"].GetInt().ToString());
            desc += "\n";
            desc += "REQ DEX: " + (infoProp["reqDEX"] == null ? "0" : infoProp["reqDEX"].GetInt().ToString());
            desc += "   ";
            desc += "REQ INT: " + (infoProp["reqINT"] == null ? "0" : infoProp["reqINT"].GetInt().ToString());
            desc += "\n";
            desc += (category == "Weapon") ? (getFriendlyWeaponAttackSpeed(infoProp) + "\n") : "";
            desc += ifIntExistsOutputFormat(infoProp, "incSTR", "STR: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incDEX", "DEX: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incINT", "INT: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incLUK", "LUK: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incPAD", "WEAPON ATTACK: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incMAD", "MAGIC ATTACK: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incACC", "ACCURACY: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "bdR", "BOSS DAMAGE: +{0}%\n");
            desc += ifIntExistsOutputFormat(infoProp, "imdR", "IED: +{0}%\n");
            desc += ifIntExistsOutputFormat(infoProp, "charmEXP", "CHARM EXP ON FIRST EQUIP: +{0}%\n");
            desc = desc.Trim();

            return desc;
        }

        // eg pass in (info prop, "incLUK", "LUK: +{0}\n") -> formatted as LUK: +20\n if the value was 20
        // if no prop then returns empty string
        private string ifIntExistsOutputFormat(WzImageProperty infoProp, string propToCheck, string formatString)
        {
            WzImageProperty prop = infoProp[propToCheck];
            if (prop == null)
                return "";

            return String.Format(formatString, prop.GetInt().ToString());
        }

        private string getFriendlyWeaponAttackSpeed(WzImageProperty infoProp)
        {
            string friendlyAtkSpd;
            if (infoProp["attackSpeed"] != null)
            {
                switch (infoProp["attackSpeed"].GetInt())
                {
                    case 2:
                        friendlyAtkSpd = "Faster (2)"; break;
                    case 3:
                        friendlyAtkSpd = "Faster (3)"; break;
                    case 4:
                        friendlyAtkSpd = "Fast (4)"; break;
                    case 5:
                        friendlyAtkSpd = "Fast (5)"; break;
                    case 6:
                        friendlyAtkSpd = "Normal (6)"; break;
                    default:
                        friendlyAtkSpd = infoProp["attackSpeed"].GetInt().ToString(); break;
                }
            }
            else
            {
                return "";
            }
            return friendlyAtkSpd;
        }

        private void targetNotExist()
        {
            targetNameLabel.Content = "N/A";
            targetDescBlock.Text = "What d'you know? - This item seems to not exist in " + Properties.Settings.Default.targetRegion + "!";
            return;
        }

        private BitmapFrame wpfImage(System.Drawing.Bitmap bmp)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            return BitmapFrame.Create(ms);
        }

        private Regex orangeDescRegex = new Regex("#c(.*?)#", RegexOptions.Singleline); // wait so singleline is the one that matches multiline strings ok then fuck thats confusing
        private Regex orangeDescToEndRegex = new Regex("(?!#c(.*?)#)#c(.*?)$", RegexOptions.Singleline);
        private void safeDescAndParse(TextBlock t, WzObject w)
        {
            t.Inlines.Clear();
            t.Text = "";

            if (w == null)
            {
                t.Text = "(no description)";
                return;
            }

            string d = w.GetString();
            d = d.Replace("\\r", "")
                 .Replace("\\n", "\n");

            // sometimes you can have #c......... to the end so have to color the whole thing
            if (orangeDescToEndRegex.IsMatch(d))
                d += "#"; // just add it back save the headache

            MatchCollection matches = orangeDescRegex.Matches(d);
            if (matches.Count < 1)
            {
                t.Text = d;
            }
            else
            {
                // eg string "do this and #cDouble-click#" - index 12 - so go up to 12 (will include the space as substring is length)
                t.Inlines.Add(new Run(d.Substring(0, matches[0].Index)));
                t.Inlines.Add(new Run(d.Substring(matches[0].Index + 2, matches[0].Length - 3)) { Foreground = Brushes.DarkOrange }); // skip the #c (+2) and remove #c and #'s length (-3)
                if (matches.Count > 1) // more than 1 match we need to do some tricks - basically last regex's index+length to the next index is regular then orange
                {
                    for (int i = 1; i < matches.Count; i++)
                    {
                        int continueIndex = matches[i - 1].Index + matches[i - 1].Length; // the index after the previous match
                        t.Inlines.Add(new Run(d.Substring(continueIndex, matches[i].Index - continueIndex))); // go up to before the next orange
                        t.Inlines.Add(new Run(d.Substring(matches[i].Index + 2, matches[i].Length - 3)) { Foreground = Brushes.DarkOrange }); // same as above, process the orange
                    }
                }
                if (matches[matches.Count - 1].Index + matches[matches.Count - 1].Length < d.Length)
                {
                    // theres still more text to go as normal after last match as the index + regex match length added is less than the entire length
                    int continueIndex = matches[matches.Count - 1].Index + matches[matches.Count - 1].Length;
                    t.Inlines.Add(new Run(d.Substring(continueIndex))); // just from teh index to the end
                }
            }
        }

        private void copySourceButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(sourceNameLabel.Content.ToString());
        }

        private void copyTargetButton_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(targetNameLabel.Content.ToString());
        }
    }
}
