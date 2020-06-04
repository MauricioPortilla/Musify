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
    /// Lógica de interacción para ConsultAlbumPage.xaml
    /// </summary>
    public partial class ConsultAlbumPage : Page {
        private readonly ObservableCollection<SongTable> albumSongs = new ObservableCollection<SongTable>();
        public ObservableCollection<SongTable> AlbumSongs {
            get => albumSongs;
        }

        public ConsultAlbumPage(Album album) {
            InitializeComponent();
            DataContext = this;
            Session.MainWindow.TitleBar.Text = album.Name;
            typeTextBlock.Text = album.Type;
            launchYearTextBlock.Text = album.LaunchYear.ToString();
            discographyTextBlock.Text = album.Discography;
            artistTextBlock.Text = album.GetArtistsNames();
            albumImage.Source = album.FetchImage();
            LoadSongs(album);
        }

        private void LoadSongs(Album album) {
            album.FetchSongs(() => {
                foreach (Song albumSong in album.Songs) {
                    AlbumSongs.Add(new SongTable {
                        Song = albumSong,
                        Title = albumSong.Title,
                        ArtistsNames = albumSong.GetArtistsNames(),
                        Album = album,
                        Duration = albumSong.Duration
                    });
                }
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar las canciones.");
            });
        }

        private void SongsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            UIFunctions.SongTable_OnDoubleClick(sender, e);
        }

        private void SongsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (songsDataGrid.SelectedIndex >= 0) {
                optionsMenu.Visibility = Visibility.Visible;
            } else {
                optionsMenu.Visibility = Visibility.Hidden;
            }
        }

        private void AddToQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.Add(((SongTable)songsDataGrid.SelectedItem).Song.SongId);
            songsDataGrid.SelectedIndex = -1;
        }

        private void AddToPlaylistMenuItem_Click(object sender, RoutedEventArgs e) {
            new AddSongToPlaylistWindow(((SongTable)songsDataGrid.SelectedItem).Song).Show();
            songsDataGrid.SelectedIndex = -1;
        }

        private void GenerateRadioStationMenuItem_Click(object sender, RoutedEventArgs e) {
            songsDataGrid.SelectedIndex = -1;
        }
    }
}
