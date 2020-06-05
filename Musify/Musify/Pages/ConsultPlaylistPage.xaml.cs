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

        public ConsultPlaylistPage(Playlist playlist) {
            InitializeComponent();
            DataContext = this;
            Session.MainWindow.TitleBar.Text = "Lista de reproducción";
            this.playlist = playlist;
            playlistNameTextBlock.Text = playlist.Name;
            LoadPlaylistSongs();
        }

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

        private void SongsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            UIFunctions.SongTable_OnDoubleClick(sender, e);
        }

        private void DownloadToggleButton_Checked(object sender, RoutedEventArgs e) {

        }

        private void DownloadToggleButton_Unchecked(object sender, RoutedEventArgs e) {

        }

        private void DeletePlaylistButton_Click(object sender, RoutedEventArgs e) {

        }
    }
}
