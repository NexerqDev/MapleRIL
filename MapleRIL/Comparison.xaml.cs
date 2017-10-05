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
            sourceDescBlock.Text = safeDesc(si.WzProperty["desc"]);

            // now the true lookup starts
            string id = "0" + si.Id.ToString(); // pad the leading 0
            bool isItemWz = _mw.ItemProperties.Contains(_mw.filterBox.Text);
            if (isItemWz)
            {
                // Item.wz (other items)

                // lets start with the easy - source
                // things are stored as Consume/0200.img/02000000
                WzDirectory sourceDir = _mw.SourceItemWz.WzDirectory[_mw.filterBox.Text] as WzDirectory;
                WzImage sourceWzImage = sourceDir.GetImageByName(id.Substring(0, 4) + ".img");
                WzImageProperty sourceProp = sourceWzImage[id];

                // TODO: do all the other spec infos
                try // idk what happens if we cant find it
                {
                    sourceImage.Source = wpfImage(sourceProp["info"]["icon"].GetBitmap());
                }
                catch { }


                // target
                WzImage targetStringImage = _mw.TargetStringWz.WzDirectory.GetImageByName(_mw.filterBox.Text + ".img");
                WzImageProperty targetStringProp = targetStringImage[si.Id.ToString()];
                if (targetStringProp == null)
                {
                    targetNotExist();
                    return;
                }
                targetNameLabel.Content = targetStringProp["name"].GetString();
                targetDescBlock.Text = safeDesc(targetStringProp["desc"]);

                WzDirectory targetItemDir = _mw.SourceItemWz.WzDirectory[_mw.filterBox.Text] as WzDirectory;
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
                WzDirectory sourceDir = _mw.SourceCharacterWz.WzDirectory[_mw.filterBox.Text] as WzDirectory;
                WzImage sourceWzImage = sourceDir.GetImageByName(id + ".img");

                // TODO: do all the other reqDEX etc info
                try // idk what happens if we cant find it
                {
                    sourceImage.Source = wpfImage(sourceWzImage["info"]["icon"].GetBitmap());
                }
                catch { }


                // target
                WzImage targetStringImage = _mw.TargetStringWz.WzDirectory.GetImageByName("Eqp.img");
                WzImageProperty targetStringProp = targetStringImage["Eqp"][_mw.filterBox.Text][si.Id.ToString()];
                if (targetStringProp == null)
                {
                    targetNotExist();
                    return;
                }
                targetNameLabel.Content = targetStringProp["name"].GetString();
                targetDescBlock.Text = safeDesc(targetStringProp["desc"]);

                WzDirectory targetItemDir = _mw.TargetCharacterWz.WzDirectory[_mw.filterBox.Text] as WzDirectory;
                WzImage targetItemImage = sourceDir.GetImageByName(id + ".img");

                // TODO: do all the other spec infos
                try // idk what happens if we cant find it
                {
                    targetImage.Source = wpfImage(targetItemImage["info"]["icon"].GetBitmap());
                }
                catch { }
            }
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
