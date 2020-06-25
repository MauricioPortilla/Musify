using Musify.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Musify.Models.Song;

namespace Musify.Pages {
    /// <summary>
    /// Lógica de interacción para ConsultPlaylistPage.xaml
    /// </summary>
    public partial class ConsultPlaylistPage : Page {
        
        private Playlist playlist;
        private ObservableCollection<SongTable> songsObservableCollection = new ObservableCollection<SongTable>();
        public ObservableCollection<SongTable> SongsObservableCollection {
            get => songsObservableCollection;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="playlist">Playlist to consult</param>
        public ConsultPlaylistPage(Playlist playlist) {
            InitializeComponent();
            DataContext = this;
            Session.MainWindow.TitleBar.Text = "Lista de reproducción";
            this.playlist = playlist;
            playlistNameTextBlock.Text = playlist.Name;
            LoadPlaylistSongs();
            CheckIfIsDownloaded();
        }

        /// <summary>
        /// Checks if this playlist is already downloaded. If so,
        /// changes download toggle button to true
        /// </summary>
        private void CheckIfIsDownloaded() {
            var downloadedPlaylists = Properties.Settings.Default.DownloadedPlaylists;
            if (downloadedPlaylists.Contains(playlist.PlaylistId.ToString())) {
                downloadToggleButton.IsChecked = true;
            }
        }

        /// <summary>
        /// Loads all the playlist songs.
        /// </summary>
        private void LoadPlaylistSongs() {
            playlist.FetchSongs(() => {
                songsObservableCollection.Clear();
                foreach (Song playlistSong in playlist.Songs) {
                    songsObservableCollection.Add(new SongTable {
                        Song = playlistSong,
                        Title = playlistSong.Title,
                        Album = playlistSong.Album,
                        Genre = playlistSong.Genre,
                        ArtistsNames = playlistSong.GetArtistsNames(),
                        Duration = playlistSong.Duration
                    });
                }
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar la lista de reproducción.");
            });
        }

        /// <summary>
        /// Attempts to play the double clicked song.
        /// </summary>
        /// <param name="sender">DataGrid</param>
        /// <param name="e">Event</param>
        private void SongsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Session.historyIndex = Session.SongsIdPlayHistory.Count - 1;
            UIFunctions.SongTable_OnDoubleClick(sender, e);
            Session.SongsIdSongList.Clear();
            for (int i = songsDataGrid.SelectedIndex + 1; i < songsObservableCollection.Count; i++) {
                Session.SongsIdSongList.Add(songsObservableCollection.ElementAt(i).Song.SongId);
            }
        }

        /// <summary>
        /// Attempts to download all the playlist songs.
        /// </summary>
        /// <param name="sender">ToggleButton</param>
        /// <param name="e">Event</param>
        private void DownloadToggleButton_Checked(object sender, RoutedEventArgs e) {
            if (Session.Account.Subscription == null) {
                MessageBox.Show("No tienes una suscripción activa.");
                downloadToggleButton.IsChecked = false;
                return;
            }
            foreach (var song in playlist.Songs) {
                try {
                    if (!File.Exists(App.DATA_DOWNLOADS_DIRECTORY + "/" + song.SongId + ".bin")) {
                        using (BinaryWriter songFileWriter = new BinaryWriter(new FileStream(App.DATA_DOWNLOADS_DIRECTORY + "/" + song.SongId + ".bin", FileMode.Create))) {
                            WebRequest webRequest = WebRequest.Create(Core.SERVER_API_URL + "/stream/song/" + song.SongId + "/" + Session.SongStreamingQuality);
                            webRequest.Headers["Authorization"] = Session.AccessToken ?? "";
                            using (Stream stream = webRequest.GetResponse().GetResponseStream()) {
                                byte[] buffer = new byte[1024 * 1024];
                                int read;
                                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0) {
                                    songFileWriter.Write(buffer, 0, read);
                                }
                            }
                            songFileWriter.Close();
                            Properties.Settings.Default.DownloadedPlaylists.Add(playlist.PlaylistId.ToString());
                            Properties.Settings.Default.Save();
                        }
                    }
                } catch (Exception) {
                    MessageBox.Show("Ocurrió un error al intentar descargar " + song.Title);
                }
            }
        }

        /// <summary>
        /// Attempts to delete all the downloaded playlist songs.
        /// </summary>
        /// <param name="sender">ToggleButton</param>
        /// <param name="e">Event</param>
        private void DownloadToggleButton_Unchecked(object sender, RoutedEventArgs e) {
            foreach (var song in playlist.Songs) {
                if (File.Exists(App.DATA_DOWNLOADS_DIRECTORY + "/" + song.SongId + ".bin")) {
                    File.Delete(App.DATA_DOWNLOADS_DIRECTORY + "/" + song.SongId + ".bin");
                }
            }
            Properties.Settings.Default.DownloadedPlaylists.Remove(playlist.PlaylistId.ToString());
            Properties.Settings.Default.Save();
        }

        /// <summary>
        /// Deletes the playlist.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void DeletePlaylistButton_Click(object sender, RoutedEventArgs e) {
            playlist.Delete(() => {
                Session.MainFrame.Source = new Uri("Pages/PlaylistsPage.xaml", UriKind.RelativeOrAbsolute);
            }, () => {
                MessageBox.Show("Ocurrió un error al eliminar esta lista de reproducción.");
            });
        }

        /// <summary>
        /// Deletes the selected song from playlist.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void DeleteSongMenuItem_Click(object sender, RoutedEventArgs e) {
            if (songsDataGrid.SelectedItem == null) {
                MessageBox.Show("Debes seleccionar una canción.");
                return;
            }
            playlist.DeleteSong(((SongTable) songsDataGrid.SelectedItem).Song, () => {
                songsObservableCollection.Remove((SongTable) songsDataGrid.SelectedItem);
            }, () => {
                MessageBox.Show("Ocurrió un error al eliminar esta canción.");
            });
        }
    }
}
