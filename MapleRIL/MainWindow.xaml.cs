using MapleLib.WzLib;
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
        public string SourceStringWzPath;
        public string SourceItemWzPath;
        public string SourceCharacterWzPath;
        public string TargetStringWzPath;
        public string TargetItemWzPath;
        public string TargetCharacterWzPath;

        public WzFile SourceStringWz;
        public WzFile SourceItemWz;
        public WzFile SourceCharacterWz;
        public WzFile TargetStringWz;
        public WzFile TargetItemWz;
        public WzFile TargetCharacterWz;

        public string SourceRegion = Properties.Settings.Default.sourceRegion;
        public string TargetRegion = Properties.Settings.Default.targetRegion;

        public string[] ItemProperties = new string[] { "Consume", "Etc", "Pet", "Cash" };
        public string[] EquipProperties;

        public ObservableCollection<SearchedItem> SearchResults = new ObservableCollection<SearchedItem>();

        public MainWindow()
        {
            InitializeComponent();

            if (String.IsNullOrWhiteSpace(Properties.Settings.Default.sourceFolder))
                (new Setup()).ShowDialog();

            SourceStringWzPath = Path.Combine(Properties.Settings.Default.sourceFolder, "String.wz");
            SourceItemWzPath = Path.Combine(Properties.Settings.Default.sourceFolder, "Item.wz");
            SourceCharacterWzPath = Path.Combine(Properties.Settings.Default.sourceFolder, "Character.wz");
            TargetStringWzPath = Path.Combine(Properties.Settings.Default.targetFolder, "String.wz");
            TargetItemWzPath = Path.Combine(Properties.Settings.Default.targetFolder, "Item.wz");
            TargetCharacterWzPath = Path.Combine(Properties.Settings.Default.targetFolder, "Character.wz");

            SourceStringWz = loadWzFile(SourceStringWzPath, WzMapleVersion.CLASSIC); // new GMS AND KMS now uses CLASSIC
            SourceItemWz = loadWzFile(SourceItemWzPath, WzMapleVersion.CLASSIC);
            SourceCharacterWz = loadWzFile(SourceCharacterWzPath, WzMapleVersion.CLASSIC);

            TargetStringWz = loadWzFile(TargetStringWzPath, WzMapleVersion.CLASSIC);
            TargetItemWz = loadWzFile(TargetItemWzPath, WzMapleVersion.CLASSIC);
            TargetCharacterWz = loadWzFile(TargetCharacterWzPath, WzMapleVersion.CLASSIC);

            EquipProperties = SourceStringWz.WzDirectory.GetImageByName("Eqp.img")["Eqp"].WzProperties.Select(w => w.Name).ToArray();

            foreach (string i in ItemProperties)
                filterBox.Items.Add(i);
            foreach (string e in EquipProperties)
                filterBox.Items.Add(e);
            filterBox.Items.Add("All");
            filterBox.Text = "All";

            dataGrid.ItemsSource = SearchResults;
        }

        private WzFile loadWzFile(string path, WzMapleVersion version)
        {
            WzFile w = new WzFile(path, version);
            w.ParseWzFile();
            return w;
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(searchBox.Text))
                return;

            SearchResults.Clear();

            if (filterBox.Text != "All")
            {
                searchInCategory(filterBox.Text);
            }
            else
            {
                foreach (string p in ItemProperties.Concat(EquipProperties))
                    searchInCategory(p);
            }
        }

        private void searchInCategory(string category)
        {
            List<WzImageProperty> searchProperties; // these are the properties we will be looping for the item names
            if (ItemProperties.Contains(category))
            {
                // Dealing with a Item.wz
                // This means the string is in eg Consume.img/ID
                WzImage workingImage = SourceStringWz.WzDirectory.GetImageByName(category + ".img");
                searchProperties = workingImage.WzProperties;
            }
            else
            {
                // Dealing with a Character.wz / it is an equip
                // This means the string is in eg Eqp.img/Eqp/Acessory/ID
                WzImage workingImage = SourceStringWz.WzDirectory.GetImageByName("Eqp.img");
                searchProperties = workingImage["Eqp"][category].WzProperties;
            }

            // look up a property in the image, eg 2000000 where inside the property "name"'s string is what the user is looking for.
            // loose search so use regexes
            Regex r = new Regex("(^| )" + searchBox.Text + "($| )", RegexOptions.IgnoreCase);
            IEnumerable<WzImageProperty> props = searchProperties.Where(w => {
                var nameProp = w.WzProperties.Where(p => p.Name == "name");
                if (nameProp.Count() < 1)
                    return false;

                return r.IsMatch(nameProp.First().GetString());
            });

            foreach (SearchedItem i in props.Select(p => new SearchedItem(p, category)))
                SearchResults.Add(i);
        }

        public class SearchedItem
        {
            public WzImageProperty WzProperty; // this is the overall property, its name is the items id

            public string Id => WzProperty.Name;
            public string Name => WzProperty["name"].GetString();
            public string Category { get; private set; }

            public SearchedItem(WzImageProperty wzProp, string category)
            {
                WzProperty = wzProp;
                Category = category;
            }
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
            string _oldSourceRegion = SourceRegion;
            WzFile _oldSourceString = SourceStringWz;
            WzFile _oldSourceCharacter = SourceCharacterWz;
            WzFile _oldSourceItem = SourceItemWz;

            // the ol' switcharoo
            SourceRegion = TargetRegion;
            SourceStringWz = TargetStringWz;
            SourceCharacterWz = TargetCharacterWz;
            SourceItemWz = TargetItemWz;

            TargetRegion = _oldSourceRegion;
            TargetStringWz = _oldSourceString;
            TargetCharacterWz = _oldSourceCharacter;
            TargetItemWz = _oldSourceItem;

            regionDirectionLabel.Content = SourceRegion + " -> " + TargetRegion;
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
    }
}
