using MaterialDesignThemes.Wpf;
using Musify.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Musify.Models.AccountSong;
using static Musify.Models.Song;

namespace Musify.Pages {
    /// <summary>
    /// Interaction logic for PlayHistoryPage.xaml
    /// </summary>
    public partial class PlayHistoryPage : Page {
        private DialogOpenedEventArgs dialogOpenEventArgs;
        private readonly ObservableCollection<object> songsPlayHistory = new ObservableCollection<object>();
        public ObservableCollection<object> SongsPlayHistory {
            get => songsPlayHistory;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public PlayHistoryPage() {
            InitializeComponent();
            DataContext = this;
            LoadPlayHistory();
        }

        /// <summary>
        /// Loads song IDs that are in play history.
        /// </summary>
        public void LoadPlayHistory() {
            songsPlayHistory.Clear();
            List<int> songsIdPlayHistory = Session.SongsIdPlayHistory;
            LoadSong(songsIdPlayHistory.Count - 1, songsIdPlayHistory);
        }

        /// <summary>
        /// Load a song from the play history.
        /// </summary>
        /// <param name="i">Index in the song IDs list of the play history</param>
        /// <param name="songsIdPlayHistory">Song IDs list of the play history</param>
        public void LoadSong(int i, List<int> songsIdPlayHistory) {
            if (i >= 0) {
                if (songsIdPlayHistory.ElementAt(i) > 0) {
                    Song.FetchById(songsIdPlayHistory.ElementAt(i), (song) => {
                        SongsPlayHistory.Add(new SongTable {
                            Song = song,
                            Title = song.Title,
                            ArtistsNames = song.GetArtistsNames(),
                            Album = song.Album,
                            Genre = song.Genre,
                            Duration = song.Duration
                        });
                        LoadSong(i - 1, songsIdPlayHistory);
                    }, (errorResponse) => {
                        MessageBox.Show(errorResponse.Message);
                    }, () => {
                        MessageBox.Show("Ocurrió un error al cargar las canciones.");
                    });
                } else {
                    AccountSong.FetchById(songsIdPlayHistory.ElementAt(i) * -1, (accountSong) => {
                        SongsPlayHistory.Add(new AccountSongTable {
                            AccountSong = accountSong,
                            Title = accountSong.Title,
                            Duration = accountSong.Duration
                        });
                        LoadSong(i - 1, songsIdPlayHistory);
                    }, (errorResponse) => {
                        MessageBox.Show(errorResponse.Message);
                    }, () => {
                        MessageBox.Show("Ocurrió un error al cargar las canciones.");
                    });
                }
            }
        }

        /// <summary>
        /// Attempts to play the double clicked song.
        /// </summary>
        /// <param name="sender">DataGrid</param>
        /// <param name="e">Event</param>
        private void PlayHistoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (playHistoryDataGrid.SelectedItem is SongTable) {
                UIFunctions.SongTable_OnDoubleClick(sender, e);
            } else {
                UIFunctions.AccountSongTable_OnDoubleClick(sender, e);
            }
            Session.historyIndex = Session.SongsIdPlayHistory.Count - 1;
            Session.SongsIdSongList.Clear();
            LoadPlayHistory();
        }

        /// <summary>
        /// Shows up a menu with the options according the selected song.
        /// </summary>
        /// <param name="sender">DataGrid</param>
        /// <param name="e">Event</param>
        private void PlayHistoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (playHistoryDataGrid.SelectedIndex >= 0) {
                optionsMenu.Visibility = Visibility.Visible;
                if (playHistoryDataGrid.SelectedItem is SongTable) {
                    addToPlaylistMenuItem.Visibility = Visibility.Visible;
                    generateRadioStationMenuItem.Visibility = Visibility.Visible;
                } else {
                    addToPlaylistMenuItem.Visibility = Visibility.Collapsed;
                    generateRadioStationMenuItem.Visibility = Visibility.Collapsed;
                }
            } else {
                optionsMenu.Visibility = Visibility.Hidden;
                addToPlaylistMenuItem.Visibility = Visibility.Collapsed;
                generateRadioStationMenuItem.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Opens up a dialog to add to play queue.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
        private void AddToQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            DialogHost.Show(mainStackPanel, "PlayHistoryPage_WindowDialogHost", (openSender, openEventArgs) => {
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
            if (playHistoryDataGrid.SelectedItem is SongTable) {
                Session.SongsIdPlayQueue.Insert(0, ((SongTable)playHistoryDataGrid.SelectedItem).Song.SongId);
            } else {
                Session.SongsIdPlayQueue.Insert(0, ((AccountSongTable)playHistoryDataGrid.SelectedItem).AccountSong.AccountSongId * -1);
            }
            playHistoryDataGrid.SelectedIndex = -1;
            dialogOpenEventArgs.Session.Close(true);
            dialogAddToQueueGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Adds the selected song to the end of the queue.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AddToTheEndButton_Click(object sender, RoutedEventArgs e) {
            if (playHistoryDataGrid.SelectedItem is SongTable) {
                Session.SongsIdPlayQueue.Add(((SongTable)playHistoryDataGrid.SelectedItem).Song.SongId);
            } else {
                Session.SongsIdPlayQueue.Add(((AccountSongTable)playHistoryDataGrid.SelectedItem).AccountSong.AccountSongId * -1);
            }
            playHistoryDataGrid.SelectedIndex = -1;
            dialogOpenEventArgs.Session.Close(true);
            dialogAddToQueueGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Shows up a new window to add the selected song to a playlist.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
        private void AddToPlaylistMenuItem_Click(object sender, RoutedEventArgs e) {
            new AddSongToPlaylistWindow(((SongTable)playHistoryDataGrid.SelectedItem).Song).Show();
            playHistoryDataGrid.SelectedIndex = -1;
        }

        /// <summary>
        /// Generates a radio station with the selected song genre.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
        private void GenerateRadioStationMenuItem_Click(object sender, RoutedEventArgs e) {
            if (Session.GenresIdRadioStations.Find(x => x == ((SongTable)playHistoryDataGrid.SelectedItem).Song.Genre.GenreId) == 0) {
                Session.GenresIdRadioStations.Add(((SongTable)playHistoryDataGrid.SelectedItem).Song.Genre.GenreId);
            } else {
                MessageBox.Show("Ya existe la estación de radio de este género.");
            }
            playHistoryDataGrid.SelectedIndex = -1;
        }
    }
}
