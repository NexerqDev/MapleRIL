using MapleLib.WzLib;
using MapleRIL.Common;
using MapleRIL.Common.RILItemType;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;

namespace MapleRIL.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool LookupReversed = false;

        public RILFileManager SourceRfm;
        public RILFileManager TargetRfm;

        public RILSearcher Searcher;

        public string SourceRegion => SourceRfm.Region;
        public string TargetRegion => TargetRfm.Region;

        public string[] RequiredWzs = new string[] { "String.wz", "Item.wz", "Character.wz" };

        public ObservableCollection<RILItem> SearchResults = new ObservableCollection<RILItem>();

        public MainWindow()
        {
            InitializeComponent();

            checkForSetup();

            try
            {
                loadRfms();
            }
            catch (IOException e)
            {
                MessageBox.Show($"An error occured whilst loading required WZ files -- this can happen as a file is in use somewhere else, such as MapleStory is still using the file. MapleRIL must now close, please try again later.\n\nAdditional information: {e.Message}", "MapleRIL - Error loading required files", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
                return;
            }

            loadSearcher();
            loadFilters();
            dataGrid.ItemsSource = SearchResults;

            aboutLabel.Content = $"{Util.FriendlyAppVersion} ~ Click for about info";
        }

        private void checkForSetup()
        {
            // Checks if old folders are still OK or if we need a new setup
            if (String.IsNullOrWhiteSpace(Properties.Settings.Default.sourceFolder))
                (new Setup()).ShowDialog();
            if (!File.Exists(Path.Combine(Properties.Settings.Default.sourceFolder, "String.wz"))
             || !File.Exists(Path.Combine(Properties.Settings.Default.targetFolder, "String.wz")))
            {
                MessageBox.Show("The MapleStory folder(s) seem to not exist anymore. Relaunching setup for you to reconfigure.", "MapleRIL - Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Properties.Settings.Default.sourceFolder = "";
                Properties.Settings.Default.Save();
                (new Setup()).ShowDialog();
            }
        }

        private void loadRfms()
        {
            SourceRfm = new RILFileManager(Properties.Settings.Default.sourceRegion, Properties.Settings.Default.sourceFolder, RequiredWzs);
            TargetRfm = new RILFileManager(Properties.Settings.Default.targetRegion, Properties.Settings.Default.targetFolder, RequiredWzs);
        }

        private void loadSearcher()
        {
            Searcher = new RILSearcher(SourceRfm);
        }

        private void loadFilters()
        {
            filterBox.Items.Clear();
            filterBox.Items.Add("All");
            foreach (RILBaseItemType it in Searcher.ItemTypes)
                filterBox.Items.Add(it);

            filterBox.Text = "All";
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(searchBox.Text))
                return;

            SearchResults.Clear();

            RILItem[] r;
            if (filterBox.Text == "All")
                r = Searcher.Search(searchBox.Text);
            else
                r = Searcher.SearchInType((RILBaseItemType)filterBox.SelectedItem, searchBox.Text);

            foreach (RILItem i in r)
                SearchResults.Add(i);
        }

        // // https://stackoverflow.com/questions/3120616/wpf-datagrid-selected-row-clicked-event sol #2
        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;
            if (row == null)
                return;

            RILItem i = row.Item as RILItem;
            if (i == null)
                return;

            (new Comparison(this, i)).ShowDialog();
        }

        private void swapDirectionBox_Toggled(object sender, RoutedEventArgs e)
        {
            reverseLookup();
        }

        private void reverseLookup()
        {
            // switcharoo
            Util.Swap(ref SourceRfm, ref TargetRfm);
            Searcher.FileManager = SourceRfm;

            // reload
            //searchBox.Text = "";
            SearchResults.Clear();
            loadFilters();

            regionDirectionLabel.Content = SourceRegion + " -> " + TargetRegion;
            searchBox.Focus();

            LookupReversed = !LookupReversed;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            searchBox.Focus();
        }

        private void filterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (filterBox.Text == "All")
                warningLabel.Content = "Note: when using All, searches may be slower. Filters are strongly recommended.";
            else
                warningLabel.Content = "";
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (LookupReversed)
                reverseLookup();

            (new Setup()).ShowDialog();
        }

        private void aboutLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            (new About(this)).ShowDialog();
        }
    }
}
