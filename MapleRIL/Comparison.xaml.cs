using MapleLib.WzLib;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
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
            bool isItemWz = _mw.ItemProperties.Contains(si.Category);
            if (isItemWz)
            {
                // Item.wz (other items)
                sourceDescBlock.Text = safeDesc(si.WzProperty["desc"]);

                // lets start with the easy - source
                // things are stored as Consume/0200.img/02000000
                WzDirectory sourceDir = _mw.SourceItemWz.WzDirectory[si.Category] as WzDirectory;
                WzImage sourceWzImage = sourceDir.GetImageByName(id.Substring(0, 4) + ".img");
                WzImageProperty sourceProp = sourceWzImage[id];

                // TODO: do all the other spec infos
                try // idk what happens if we cant find it
                {
                    sourceImage.Source = wpfImage(sourceProp["info"]["icon"].GetBitmap());
                }
                catch { }


                // target
                WzImage targetStringImage = _mw.TargetStringWz.WzDirectory.GetImageByName(si.Category + ".img");
                WzImageProperty targetStringProp = targetStringImage[si.Id.ToString()];
                if (targetStringProp == null)
                {
                    targetNotExist();
                    return;
                }
                targetNameLabel.Content = targetStringProp["name"].GetString();
                targetDescBlock.Text = safeDesc(targetStringProp["desc"]);

                WzDirectory targetItemDir = _mw.SourceItemWz.WzDirectory[si.Category] as WzDirectory;
                WzImage targetItemImage = sourceDir.GetImageByName(id.Substring(0, 4) + ".img");
                WzImageProperty targetItemProp = sourceWzImage[id];

                // TODO: do all the other spec infos
                try // idk what happens if we cant find it
                {
                    targetImage.Source = wpfImage(targetItemProp["info"]["icon"].GetBitmap());
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
                WzDirectory sourceDir = _mw.SourceCharacterWz.WzDirectory[si.Category] as WzDirectory;
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
                WzImageProperty targetStringProp = targetStringImage["Eqp"][si.Category][si.Id.ToString()];
                if (targetStringProp == null)
                {
                    targetNotExist();
                    return;
                }
                targetNameLabel.Content = targetStringProp["name"].GetString();
                //targetDescBlock.Text = safeDesc(targetStringProp["desc"]);

                WzDirectory targetItemDir = _mw.TargetCharacterWz.WzDirectory[si.Category] as WzDirectory;
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

        private string safeDesc(WzObject w)
        {
            if (w == null)
                return "(no description)";

            string d = w.GetString();
            return d.Replace("\\n", "\n");
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
