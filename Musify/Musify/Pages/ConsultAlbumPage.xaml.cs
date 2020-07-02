using MaterialDesignThemes.Wpf;
using Musify.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Musify.Models.Song;

namespace Musify.Pages {
    /// <summary>
    /// Interaction logic for ConsultAlbumPage.xaml
    /// </summary>
    public partial class ConsultAlbumPage : Page {
        private DialogOpenedEventArgs dialogOpenEventArgs;
        private readonly ObservableCollection<SongTable> albumSongs = new ObservableCollection<SongTable>();
        public ObservableCollection<SongTable> AlbumSongs {
            get => albumSongs;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="album">Album to consult</param>
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

        /// <summary>
        /// Loads all the album songs.
        /// </summary>
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
            }, (errorResponse) => {
                MessageBox.Show(errorResponse.Message);
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar las canciones.");
            });
        }

        /// <summary>
        /// Attempts to play the double clicked song.
        /// </summary>
        /// <param name="sender">DataGrid</param>
        /// <param name="e">Event</param>
        private void SongsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            UIFunctions.SongTable_OnDoubleClick(sender, e);
            Session.HistoryIndex = Session.SongsIdPlayHistory.Count - 1;
            Session.SongsIdSongList.Clear();
            for (int i = songsDataGrid.SelectedIndex + 1; i < albumSongs.Count; i++) {
                Session.SongsIdSongList.Add(albumSongs.ElementAt(i).Song.SongId);
            }
        }

        /// <summary>
        /// Shows up a menu with the options for selected song.
        /// </summary>
        /// <param name="sender">DataGrid</param>
        /// <param name="e">Event</param>
        private void SongsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (songsDataGrid.SelectedIndex >= 0) {
                optionsMenu.Visibility = Visibility.Visible;
            } else {
                optionsMenu.Visibility = Visibility.Hidden;
            }
        }

        /// <summary>
        /// Opens up a dialog to add to play queue.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
        private void AddToQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            DialogHost.Show(mainStackPanel, "ConsultAlbumPage_WindowDialogHost", (openSender, openEventArgs) => {
                dialogOpenEventArgs = openEventArgs;
                dialogAddToQueueGrid.Visibility = Visibility.Visible;
            }, null);
        }

        /// <summary>
        /// Adds the selected song to the beginning of the queue.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AddToBelowButton_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.Insert(0, ((SongTable)songsDataGrid.SelectedItem).Song.SongId);
            songsDataGrid.SelectedIndex = -1;
            dialogOpenEventArgs.Session.Close(true);
            dialogAddToQueueGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Adds the selected song to the end of the queue.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AddToTheEndButton_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.Add(((SongTable)songsDataGrid.SelectedItem).Song.SongId);
            songsDataGrid.SelectedIndex = -1;
            dialogOpenEventArgs.Session.Close(true);
            dialogAddToQueueGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Shows up a new window to add the selected song to a playlist.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
        private void AddToPlaylistMenuItem_Click(object sender, RoutedEventArgs e) {
            new AddSongToPlaylistWindow(((SongTable)songsDataGrid.SelectedItem).Song).Show();
            songsDataGrid.SelectedIndex = -1;
        }

        /// <summary>
        /// Generates a radio station with the selected song genre.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
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
