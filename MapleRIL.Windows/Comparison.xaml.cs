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
            safeDescAndParse(sourceDescBlock, sourceItem.Description);
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
            safeDescAndParse(targetDescBlock, targetItem.Description);
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

        private Regex orangeDescRegex = new Regex("#c(.*?)#", RegexOptions.Singleline); // wait so singleline is the one that matches multiline strings ok then fuck thats confusing
        private Regex orangeDescToEndRegex = new Regex("(?!#c(.*?)#)#c(.*?)$", RegexOptions.Singleline);
        private void safeDescAndParse(TextBlock t, string d)
        {
            t.Inlines.Clear();
            t.Text = "";

            if (d == null)
            {
                t.Text = "(no description)";
                return;
            }

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
