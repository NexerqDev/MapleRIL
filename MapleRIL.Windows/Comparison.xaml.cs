using MapleLib.WzLib;
using MapleRIL.Common;
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

namespace MapleRIL.Windows
{
    /// <summary>
    /// Interaction logic for Comparison.xaml
    /// </summary>
    public partial class Comparison : Window
    {
        MainWindow _mw;

        public Comparison(MainWindow mw, RILItem sourceItem)
        {
            InitializeComponent();

            _mw = mw;

            lookupLabel.Content = "Lookup: ID " + sourceItem.Id;
            Title = "MapleRIL - Lookup: ID " + sourceItem.Id;

            // source
            sourceRegionLabel.Content = _mw.SourceRegion;
            sourceNameLabel.Content = sourceItem.Name;
            safeDescAndParse(sourceDescBlock, sourceItem.ParsedDescription);
            try
            {
                sourceImage.Source = Util.DrawingBmpToWpfBmp(sourceItem.Icon);
            }
            catch { }

            // target
            targetRegionLabel.Content = _mw.TargetRegion;
            RILItem targetItem = sourceItem.TryLoadInOtherFileManager(_mw.TargetRfm);
            if (targetItem == null)
            {
                targetNotExist();
                return;
            }

            targetNameLabel.Content = targetItem.Name;
            safeDescAndParse(targetDescBlock, targetItem.ParsedDescription);
            try
            {
                targetImage.Source = Util.DrawingBmpToWpfBmp(targetItem.Icon);
            }
            catch { }
        }

        private void targetNotExist()
        {
            targetNameLabel.Content = "N/A";
            targetDescBlock.Text = "What d'you know? - This item seems to not exist in " + Properties.Settings.Default.targetRegion + "!";
        }

        private void safeDescAndParse(TextBlock t, string[] pd)
        {
            t.Inlines.Clear();
            t.Text = "";

            if (pd == null)
            {
                t.Text = "(no description)";
                return;
            }

            bool orange = false;
            foreach (string d in pd)
            {
                Run r = new Run(d);
                if (orange)
                    r.Foreground = Brushes.DarkOrange;

                t.Inlines.Add(r);
                orange = !orange;
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
