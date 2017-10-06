using MapleLib.WzLib;
using MapleRIL.Structure;
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

namespace MapleRIL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public bool LookupReversed = false;

        public string SourceWzPath;
        public string TargetWzPath;

        public string SourceRegion;
        public string TargetRegion;

        public Dictionary<string, WzFile> SourceWzs;
        public Dictionary<string, WzFile> TargetWzs;

        public string[] RequiredWzs = new string[] { "String.wz", "Item.wz", "Character.wz" };

        public List<WzItemType> ItemTypes;

        public ObservableCollection<SearchedItem> SearchResults = new ObservableCollection<SearchedItem>();

        public MainWindow()
        {
            InitializeComponent();

            checkForSetup();

            SourceRegion = Properties.Settings.Default.sourceRegion;
            TargetRegion = Properties.Settings.Default.targetRegion;

            loadPaths();
            loadWzs();

            loadFilters();
            dataGrid.ItemsSource = SearchResults;
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

        private void loadPaths()
        {
            SourceWzPath = Properties.Settings.Default.sourceFolder;
            TargetWzPath = Properties.Settings.Default.targetFolder;
        }

        private void loadWzs()
        {
            SourceWzs = new Dictionary<string, WzFile>();
            TargetWzs = new Dictionary<string, WzFile>();

            foreach (string w in RequiredWzs)
            {
                // gms, kms, msea all use classic encryption now
                WzFile source = new WzFile(Path.Combine(SourceWzPath, w), WzMapleVersion.CLASSIC);
                source.ParseWzFile();
                SourceWzs.Add(w, source);

                WzFile target = new WzFile(Path.Combine(TargetWzPath, w), WzMapleVersion.CLASSIC);
                target.ParseWzFile();
                TargetWzs.Add(w, target);
            }
        }

        private void loadFilters()
        {
            ItemTypes = new List<WzItemType> // default Item.wz types
            {
                new ItemWzItemType("Consume"),
                new ItemWzItemType("Etc"),
                new PetItemWzItemType(),
                new ItemWzItemType("Cash"),
                new SetupItemWzItemType()
            };

            IEnumerable<string> equipProps = SourceWzs["String.wz"].WzDirectory.GetImageByName("Eqp.img")["Eqp"].WzProperties.Select(w => w.Name);
            foreach (string e in equipProps)
                ItemTypes.Add(new EquipWzItemType(e));

            filterBox.Items.Clear();
            filterBox.Items.Add("All");
            foreach (WzItemType e in ItemTypes)
                filterBox.Items.Add(e);

            filterBox.Text = "All";
        }

        private WzFile loadWzFile(string path, WzMapleVersion version)
        {
            try
            {
                WzFile w = new WzFile(path, version);
                w.ParseWzFile();
                return w;
            }
            catch (IOException e)
            {
                MessageBox.Show($"An error occured whilst loading WZ file: {path} -- this can happen as the file is in use somewhere else, such as MapleStory is still using the file. MapleRIL must now close, please try again later.\n\nAdditional information: {e.Message}", "MapleRIL - Error loading required files", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
                return null;
            }
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(searchBox.Text))
                return;

            SearchResults.Clear();

            if (filterBox.Text != "All")
            {
                searchInType((WzItemType)filterBox.SelectedItem);
            }
            else
            {
                foreach (WzItemType p in ItemTypes)
                    searchInType(p);
            }
        }

        private void searchInType(WzItemType type)
        {
            List<WzImageProperty> searchProperties = type.GetAllStringIdProperties(SourceWzs); // these are the properties we will be looping for the item names

            // look up a property in the image, eg 2000000 where inside the property "name"'s string is what the user is looking for.
            // loose search so use regexes
            Regex r = new Regex("(^| )" + searchBox.Text + "($| |')", RegexOptions.IgnoreCase);
            IEnumerable<WzImageProperty> props = searchProperties.Where(w => {
                var nameProp = w.WzProperties.Where(p => p.Name == "name");
                if (nameProp.Count() < 1)
                    return false;

                return r.IsMatch(nameProp.First().GetString());
            });

            foreach (SearchedItem i in props.Select(p => new SearchedItem(p, type)))
                SearchResults.Add(i);
        }

        // // https://stackoverflow.com/questions/3120616/wpf-datagrid-selected-row-clicked-event sol #2
        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGridRow row = ItemsControl.ContainerFromElement((DataGrid)sender, e.OriginalSource as DependencyObject) as DataGridRow;
            if (row == null)
                return;

            SearchedItem si = row.Item as SearchedItem;
            if (si == null)
                return;

            (new Comparison(this, si)).ShowDialog();
        }

        private void swapDirectionBox_Toggled(object sender, RoutedEventArgs e)
        {
            reverseLookup();
        }

        private void reverseLookup()
        {
            // switcharoo
            swap(ref SourceRegion, ref TargetRegion);
            swap(ref SourceWzPath, ref TargetWzPath);
            swap(ref SourceWzs, ref TargetWzs);

            // reload
            searchBox.Text = "";
            SearchResults.Clear();
            loadFilters();

            regionDirectionLabel.Content = SourceRegion + " -> " + TargetRegion;
            searchBox.Focus();

            LookupReversed = !LookupReversed;
        }

        private void swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
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
    }
}
