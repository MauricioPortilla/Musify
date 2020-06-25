using MaterialDesignThemes.Wpf;
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
        private DialogOpenedEventArgs dialogOpenEventArgs;
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
                albumSongs.Clear();
                foreach (Song albumSong in album.Songs) {
                    AlbumSongs.Add(new SongTable {
                        Song = albumSong,
                        Title = albumSong.Title,
                        ArtistsNames = albumSong.GetArtistsNames(),
                        Album = album,
                        Genre = albumSong.Genre,
                        Duration = albumSong.Duration
                    });
                }
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar las canciones.");
            });
        }

        private void SongsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Session.historyIndex = Session.SongsIdPlayHistory.Count - 1;
            UIFunctions.SongTable_OnDoubleClick(sender, e);
            Session.SongsIdSongList.Clear();
            for (int i = songsDataGrid.SelectedIndex + 1; i < albumSongs.Count; i++) {
                Session.SongsIdSongList.Add(albumSongs.ElementAt(i).Song.SongId);
            }
        }

        private void SongsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (songsDataGrid.SelectedIndex >= 0) {
                optionsMenu.Visibility = Visibility.Visible;
            } else {
                optionsMenu.Visibility = Visibility.Hidden;
            }
        }

        private void AddToQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            DialogHost.Show(mainStackPanel, "ConsultAlbumPage_WindowDialogHost", (openSender, openEventArgs) => {
                dialogOpenEventArgs = openEventArgs;
                dialogAddToQueueGrid.Visibility = Visibility.Visible;
            }, null);
        }

        private void AddToBelowButton_Click(object sender, RoutedEventArgs e) {
            List<int> songsIdPlayQueue = new List<int> { ((SongTable)songsDataGrid.SelectedItem).Song.SongId };
            songsIdPlayQueue.AddRange(Session.SongsIdPlayQueue);
            Session.SongsIdPlayQueue = songsIdPlayQueue;
            songsDataGrid.SelectedIndex = -1;
            dialogOpenEventArgs.Session.Close(true);
            dialogAddToQueueGrid.Visibility = Visibility.Collapsed;
        }

        private void AddToTheEndButton_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.Add(((SongTable)songsDataGrid.SelectedItem).Song.SongId);
            songsDataGrid.SelectedIndex = -1;
            dialogOpenEventArgs.Session.Close(true);
            dialogAddToQueueGrid.Visibility = Visibility.Collapsed;
        }

        private void AddToPlaylistMenuItem_Click(object sender, RoutedEventArgs e) {
            new AddSongToPlaylistWindow(((SongTable)songsDataGrid.SelectedItem).Song).Show();
            songsDataGrid.SelectedIndex = -1;
        }

        private void GenerateRadioStationMenuItem_Click(object sender, RoutedEventArgs e) {
            if (Session.GenresIdRadioStations.Find(x => x == ((SongTable)songsDataGrid.SelectedItem).Song.Genre.GenreId) == 0) {
                Session.GenresIdRadioStations.Add(((SongTable)songsDataGrid.SelectedItem).Song.Genre.GenreId);
            } else {
                MessageBox.Show("Ya existe la estación de radio de este género.");
            }
            songsDataGrid.SelectedIndex = -1;
        }
    }
}
