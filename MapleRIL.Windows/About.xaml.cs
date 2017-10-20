using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace MapleRIL.Windows
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class About : Window
    {
        public About(MainWindow mw)
        {
            InitializeComponent();

            string i = String.Format(@"MapleRIL: MapleStory Region Item Lookup
{0}
(c) 2017 Nicholas Tay / ""Nexerq"" <nexerq@gmail.com>.
Licensed under the zlib/libpng license.
Full license text can be found in the root of the repository.", mw.FriendlyAppVersion);
            infoBlock.Text = i;

            System.Reflection.Assembly mlib = typeof(MapleLib.WzLib.WzFile).Assembly;
            FileVersionInfo mlibFvi = FileVersionInfo.GetVersionInfo(mlib.Location);
            string o = String.Format(@"MapleRIL also utilizes libraries from other open source projects, including:

MapleLib v{0} (uses WZ component(s))
{1}", $"{mlibFvi.ProductMajorPart}.{mlibFvi.ProductMinorPart}.{mlibFvi.ProductBuildPart}", mlibFvi.LegalCopyright);
            otherBlock.Text = o;

            foreach (var file in mw.SourceWzs.Values)
                displayListBox.Items.Add($"{mw.SourceRegion}: {file.FilePath}");
            foreach (var file in mw.TargetWzs.Values)
                displayListBox.Items.Add($"{mw.TargetRegion}: {file.FilePath}");

        }
    }
}
