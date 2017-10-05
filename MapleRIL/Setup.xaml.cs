using System;
using System.Collections.Generic;
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
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        public Setup()
        {
            InitializeComponent();

            loadRegions();
        }

        private void loadRegions()
        {
            //Enum.GetNames(typeof(reWZ.WZVariant)).ToList().ForEach(r =>
            //{
            //    sourceRegionBox.Items.Add(r);
            //    targetRegionBox.Items.Add(r);
            //});

            sourceRegionBox.Text = "GMS";
            targetRegionBox.Text = "KMS";
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
                        folder = fbd.SelectedPath;
                    else
                        MessageBox.Show("Please select a folder.");
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
            Close();
        }
    }
}
