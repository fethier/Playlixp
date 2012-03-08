using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;


using Playlixp.Entity;
using Playlixp.Entity.Playlist;
using System.Threading;
using Playlixp.Exporter;
using System.ComponentModel;

namespace Playlixp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<BasePlaylist> _playLists;

        public MainWindow()
        {
            InitializeComponent();
            _playLists = new List<BasePlaylist>();
        }

        private void btnChoosePlaylist_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new System.Windows.Forms.OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.Filter = ".wpl, .m3u, .txt|*.wpl;*.m3u;*.txt";
            DialogResult fileResult = fileDialog.ShowDialog();

            if (fileResult == System.Windows.Forms.DialogResult.OK){
                string[] filePaths = fileDialog.FileNames;
                string[] filesNames = fileDialog.SafeFileNames;

                for (int i = 0; i < filePaths.Length; i++)
			    {
                    BasePlaylist currectPlaylist = FactoryPlaylist.createPlaylist(filePaths[i]);
                    _playLists.Add(currectPlaylist);
                    lstPlaylists.Items.Add(currectPlaylist);
                }

                lstPlaylists.SelectedIndex = 0;
            };

            
        }

        private void lstPlaylists_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            lstSongs.ItemsSource = _playLists[lstPlaylists.SelectedIndex].Songs;
            pbExport.Value = 0;
        }

        private void lstSongs_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Song currentSong = _playLists[lstPlaylists.SelectedIndex].Songs[lstSongs.SelectedIndex];
            currentSong.Status = currentSong.Status == SongStatus.Disabled ? SongStatus.Enabled : SongStatus.Disabled;
            lstSongs.Items.Refresh();
        }

        private void mnuEnable_Click(object sender, RoutedEventArgs e)
        {
            foreach (Song song in lstSongs.SelectedItems)
            {
                song.Status = SongStatus.Enabled;
            }

            lstSongs.Items.Refresh();
        }

        private void mnuDisable_Click(object sender, RoutedEventArgs e)
        {
            foreach (Song song in lstSongs.SelectedItems)
            {
                song.Status = SongStatus.Disabled;
            }

            lstSongs.Items.Refresh();
        }

        private void lstSongs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
                mnuDisable.IsEnabled = lstSongs.SelectedIndex != -1;
                mnuEnable.IsEnabled = lstSongs.SelectedIndex != -1;
        }

        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            BasePlaylist currentPlaylist = _playLists[lstPlaylists.SelectedIndex];
            SaveFileDialog saveDiag = new SaveFileDialog();

            saveDiag.FileName = currentPlaylist.PlaylistName.Split('.')[0]; // Default file name
            saveDiag.DefaultExt = ".zip"; // Default file extension

            // Show save file dialog box
            DialogResult fileResult = saveDiag.ShowDialog();

            // Process save file dialog box results
            if (fileResult != System.Windows.Forms.DialogResult.OK)
                return;

            ZipExporter zipExporter = new ZipExporter(currentPlaylist, 0);
            zipExporter.AddProgressChangedEventHandler(new ProgressChangedEventHandler(Export_ProgressChanged));
            zipExporter.AddWorkerCompletedEventHandler(new RunWorkerCompletedEventHandler(Export_Completed));
            zipExporter.Export(saveDiag.FileName);
        }

        private void Export_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pbExport.Value = e.ProgressPercentage;
            lblExportFile.Content = ((Song)e.UserState).FileName;
        }

        private void Export_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            //Reset controls
            pbExport.Value = 0;
            lblExportFile.Content = string.Empty;

            System.Windows.MessageBox.Show("Export completed!");
        }

    }
}
