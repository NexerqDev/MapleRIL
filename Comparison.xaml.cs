using MapleLib.WzLib;
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
        MainWindow.SearchedItem Item;

        public Comparison(MainWindow mw, MainWindow.SearchedItem si)
        {
            InitializeComponent();

            _mw = mw;
            Item = si;

            lookupLabel.Content = "Lookup: ID " + si.Id.ToString();
            sourceRegionLabel.Content = _mw.SourceRegion;
            targetRegionLabel.Content = _mw.TargetRegion;
            sourceNameLabel.Content = si.Name;

            // now the true lookup starts
            string id = "0" + si.Id.ToString(); // pad the leading 0
            if (_mw.ItemProperties.Contains(si.StringWzCategory) || si.StringWzCategory == "Ins")
            {
                // TODO: make source and target loading modular, so we dont end up copy pasting like here
                // should just be one function that takes a WzFile or something and looks the id up.
                // especially because we have these Ins & Pet cases that just keep needing to be copied...

                // Item.wz (other items)
                safeDescAndParse(sourceDescBlock, si.WzProperty["desc"]);

                // lets start with the easy - source
                // things are stored as Consume/0200.img/02000000
                string itemWzCategory = si.StringWzCategory;
                if (itemWzCategory == "Ins")
                    itemWzCategory = "Install"; // setup = install in item.wz
                WzDirectory sourceItemDir = _mw.SourceItemWz.WzDirectory[itemWzCategory] as WzDirectory;

                WzImageProperty sourceItemInfoProp;
                if (itemWzCategory != "Pet")
                {
                    WzImage sourceItemImage = sourceItemDir.GetImageByName(id.Substring(0, 4) + ".img");
                    sourceItemInfoProp = sourceItemImage[id]["info"];
                }
                else // pls no more edge cases O_o -- need more modular handling for these
                {
                    // pets dont use the padded id and are just image, not by first 4 0002 thing zzzz
                    WzImage sourceItemImage = sourceItemDir.GetImageByName(si.Id.ToString() + ".img");
                    sourceItemInfoProp = sourceItemImage["info"];
                }

                // TODO: do all the other spec infos
                try // idk what happens if we cant find it
                {
                    sourceImage.Source = wpfImage(sourceItemInfoProp["icon"].GetBitmap());
                }
                catch { }


                // target
                WzImage targetStringImage = _mw.TargetStringWz.WzDirectory.GetImageByName(si.StringWzCategory + ".img");
                WzImageProperty targetStringProp = targetStringImage[si.Id.ToString()];
                if (targetStringProp == null)
                {
                    targetNotExist();
                    return;
                }
                targetNameLabel.Content = targetStringProp["name"].GetString();
                safeDescAndParse(targetDescBlock, targetStringProp["desc"]);

                WzDirectory targetItemDir = _mw.TargetItemWz.WzDirectory[itemWzCategory] as WzDirectory;

                WzImageProperty targetInfoItemProp;
                if (itemWzCategory != "Pet")
                {
                    WzImage targetItemImage = targetItemDir.GetImageByName(id.Substring(0, 4) + ".img");
                    targetInfoItemProp = targetItemImage[id]["info"];
                }
                else
                {
                    WzImage targetItemImage = targetItemDir.GetImageByName(si.Id.ToString() + ".img");
                    targetInfoItemProp = targetItemImage["info"];
                }

                // TODO: do all the other spec infos
                try // idk what happens if we cant find it
                {
                    targetImage.Source = wpfImage(targetInfoItemProp["icon"].GetBitmap());
                }
                catch { }
            }
            else
            {
                // Character.wz (equip)
                // main difference is instead of general 4 digit img then id
                // it is each item has its own image.

                // source
                // stored as Accessory/01010000.img
                WzDirectory sourceDir = _mw.SourceCharacterWz.WzDirectory[si.StringWzCategory] as WzDirectory;
                WzImage sourceWzImage = sourceDir.GetImageByName(id + ".img");
                sourceDescBlock.Text = buildEquipDescription(sourceWzImage["info"]); // build desc here as we need it from char.wz

                // TODO: do all the other reqDEX etc info
                try // idk what happens if we cant find it
                {
                    sourceImage.Source = wpfImage(sourceWzImage["info"]["icon"].GetBitmap());
                }
                catch { }


                // target
                WzImage targetStringImage = _mw.TargetStringWz.WzDirectory.GetImageByName("Eqp.img");
                WzImageProperty targetStringProp = targetStringImage["Eqp"][si.StringWzCategory][si.Id.ToString()];
                if (targetStringProp == null)
                {
                    targetNotExist();
                    return;
                }
                targetNameLabel.Content = targetStringProp["name"].GetString();
                //targetDescBlock.Text = safeDesc(targetStringProp["desc"]);

                WzDirectory targetItemDir = _mw.TargetCharacterWz.WzDirectory[si.StringWzCategory] as WzDirectory;
                WzImage targetItemImage = sourceDir.GetImageByName(id + ".img");
                WzImageProperty infoProp = targetItemImage["info"];
                targetDescBlock.Text = buildEquipDescription(infoProp);

                // TODO: do all the other spec infos
                try // idk what happens if we cant find it
                {
                    targetImage.Source = wpfImage(infoProp["icon"].GetBitmap());
                }
                catch { }
            }
        }

        private string buildEquipDescription(WzImageProperty infoProp)
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
            desc += ifIntExistsOutputFormat(infoProp, "incSTR", "STR: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incDEX", "DEX: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incINT", "INT: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incLUK", "LUK: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incPAD", "WEAPON ATTACK: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incMAD", "MAGIC ATTACK: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "incACC", "ACCURACY: +{0}\n");
            desc += ifIntExistsOutputFormat(infoProp, "imdR", "IED: +{0}%\n");
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
