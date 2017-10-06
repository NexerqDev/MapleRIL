using System;
using System.Collections.Generic;
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

namespace MapleRIL
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        public Setup()
        {
            InitializeComponent();

            loadRegions();
        }

        public string[] Regions = new string[] { "GMS", "KMS", "MSEA" };

        private void loadRegions()
        {
            foreach (string r in Regions)
            {
                sourceRegionBox.Items.Add(r);
                targetRegionBox.Items.Add(r);
            }

            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.sourceFolder))
            {
                sourceRegionBox.Text = Properties.Settings.Default.sourceRegion;
                targetRegionBox.Text = Properties.Settings.Default.targetRegion;
                sourceLabel.Content = "OK";
                sourceLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
                targetLabel.Content = "OK";
                targetLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
            }
            else
            {
                sourceRegionBox.Text = "GMS";
                targetRegionBox.Text = "KMS";
            }
        }

        private void sourceButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.sourceFolder = getFolder();
            sourceLabel.Content = "OK";
            sourceLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
        }

        private void targetButton_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.targetFolder = getFolder();
            targetLabel.Content = "OK";
            targetLabel.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 0));
        }

        private string getFolder()
        {
            string folder = "";
            using (var fbd = new System.Windows.Forms.FolderBrowserDialog())
            {
                while (String.IsNullOrWhiteSpace(folder))
                {
                    System.Windows.Forms.DialogResult res = fbd.ShowDialog();
                    if (res == System.Windows.Forms.DialogResult.OK && !String.IsNullOrWhiteSpace(fbd.SelectedPath))
                    {
                        if (!File.Exists(Path.Combine(fbd.SelectedPath, "Item.wz")))
                            MessageBox.Show("This directory does not seem like a MapleStory directory. Please choose a valid path. (if you are using Nexon Launcher, make sure you select the 'appdata' path, not just the 'maplestory' path.)");
                        else
                            folder = fbd.SelectedPath;
                    }
                    else
                    {
                        MessageBox.Show("Please select a folder.");
                    }
                }
            }
            return folder;
        }

        private void doneButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Properties.Settings.Default.sourceFolder)
             || String.IsNullOrWhiteSpace(Properties.Settings.Default.targetFolder))
            {
                MessageBox.Show("You need to select a source and target folder.");
                return;
            }

            if (sourceRegionBox.Text == targetRegionBox.Text)
            {
                MessageBox.Show("The source region and target region is the same... are you sure you know what this tool is for...");
                return;
            }

            Properties.Settings.Default.sourceRegion = sourceRegionBox.Text;
            Properties.Settings.Default.targetRegion = targetRegionBox.Text;

            Properties.Settings.Default.Save();
            MessageBox.Show("Setup complete. If at any time you need to return to this setup menu, simply click the MapleRIL \"logo\" on the main search window. Thanks for using and welcome to MapleRIL!");

            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(Properties.Settings.Default.sourceFolder))
                return; // this is only intended for first time setups

            e.Cancel = true;

            MessageBoxResult mbr = MessageBox.Show("If you close setup now, you will not be able to use MapleRIL. Still close?", "Warning", MessageBoxButton.YesNo);
            if (mbr == MessageBoxResult.Yes)
                Environment.Exit(0);
        }
    }
}
