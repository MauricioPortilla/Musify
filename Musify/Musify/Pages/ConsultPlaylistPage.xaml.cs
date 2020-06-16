using Musify.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            UIFunctions.SongTable_OnDoubleClick(sender, e);
        }

        /// <summary>
        /// Attempts to download all the playlist songs.
        /// </summary>
        /// <param name="sender">ToggleButton</param>
        /// <param name="e">Event</param>
        private void DownloadToggleButton_Checked(object sender, RoutedEventArgs e) {

        }

        /// <summary>
        /// Attempts to delete all the downloaded playlist songs.
        /// </summary>
        /// <param name="sender">ToggleButton</param>
        /// <param name="e">Event</param>
        private void DownloadToggleButton_Unchecked(object sender, RoutedEventArgs e) {

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
