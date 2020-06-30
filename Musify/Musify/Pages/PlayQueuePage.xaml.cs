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
    /// Interaction logic for PlayQueuePage.xaml
    /// </summary>
    public partial class PlayQueuePage : Page {
        private DialogOpenedEventArgs dialogOpenEventArgs;
        private readonly ObservableCollection<object> songsPlayQueue = new ObservableCollection<object>();
        public ObservableCollection<object> SongsPlayQueue {
            get => songsPlayQueue;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public PlayQueuePage() {
            InitializeComponent();
            DataContext = this;
            LoadPlayQueue();
        }

        /// <summary>
        /// Loads song IDs that are in play queue.
        /// </summary>
        public void LoadPlayQueue() {
            songsPlayQueue.Clear();
            List<int> songsIdPlayQueue = new List<int>();
            foreach (int songId in Session.SongsIdPlayQueue) {
                songsIdPlayQueue.Add(songId);
            }
            foreach (int songId in Session.SongsIdSongList) {
                songsIdPlayQueue.Add(songId);
            }
            if (Session.SongsIdPlayQueue.Count > 0) {
                deletePlayQueueButton.Visibility = Visibility.Visible;
            } else {
                deletePlayQueueButton.Visibility = Visibility.Hidden;
            }
            LoadSong(0, songsIdPlayQueue.Count, songsIdPlayQueue);
        }

        /// <summary>
        /// Load a song from the play queue.
        /// </summary>
        /// <param name="i">Index in the song IDs list of the play queue</param>
        /// <param name="limit">Total song IDs in the play queue</param>
        /// <param name="songsIdPlayQueue">Song IDs list of the play queue</param>
        public void LoadSong(int i, int limit, List<int> songsIdPlayQueue) {
            if (i < limit) {
                if (songsIdPlayQueue.ElementAt(i) > 0) {
                    Song.FetchById(songsIdPlayQueue.ElementAt(i), (song) => {
                        SongsPlayQueue.Add(new SongTable {
                            Song = song,
                            Title = song.Title,
                            ArtistsNames = song.GetArtistsNames(),
                            Album = song.Album,
                            Genre = song.Genre,
                            Duration = song.Duration
                        });
                        LoadSong(i + 1, limit, songsIdPlayQueue);
                    }, (errorResponse) => {
                        MessageBox.Show(errorResponse.Message);
                    }, () => {
                        MessageBox.Show("Ocurrió un error al cargar las canciones.");
                    });
                } else {
                    AccountSong.FetchById(songsIdPlayQueue.ElementAt(i) * -1, (accountSong) => {
                        SongsPlayQueue.Add(new AccountSongTable {
                            AccountSong = accountSong,
                            Title = accountSong.Title,
                            Duration = accountSong.Duration
                        });
                        LoadSong(i + 1, limit, songsIdPlayQueue);
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
        private void PlayQueueDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (playQueueDataGrid.SelectedItem is SongTable) {
                UIFunctions.SongTable_OnDoubleClick(sender, e);
            } else {
                UIFunctions.AccountSongTable_OnDoubleClick(sender, e);
            }
            Session.historyIndex = Session.SongsIdPlayHistory.Count - 1;
            for (int i = 0; i <= playQueueDataGrid.SelectedIndex; i++) {
                if (Session.SongsIdPlayQueue.Count > 0) {
                    Session.SongsIdPlayQueue.RemoveAt(0);
                } else {
                    Session.SongsIdSongList.RemoveAt(0);
                }
            }
            LoadPlayQueue();
        }

        /// <summary>
        /// Shows up a menu with the options according the selected song.
        /// </summary>
        /// <param name="sender">DataGrid</param>
        /// <param name="e">Event</param>
        private void PlayQueueDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (playQueueDataGrid.SelectedIndex >= 0) {
                optionsMenu.Visibility = Visibility.Visible;
                if (playQueueDataGrid.SelectedItem is SongTable) {
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
        /// Deletes the play queue.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void DeletePlayQueueButton_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue = new List<int>();
            LoadPlayQueue();
        }

        /// <summary>
        /// Opens up a dialog to add to play queue.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
        private void AddToQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            DialogHost.Show(mainStackPanel, "PlayQueuePage_WindowDialogHost", (openSender, openEventArgs) => {
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
            List<int> songsIdPlayQueue = new List<int>();
            if (playQueueDataGrid.SelectedItem is SongTable) {
                songsIdPlayQueue.Add(((SongTable)playQueueDataGrid.SelectedItem).Song.SongId);
            } else {
                songsIdPlayQueue.Add(((AccountSongTable)playQueueDataGrid.SelectedItem).AccountSong.AccountSongId * -1);
            }
            songsIdPlayQueue.AddRange(Session.SongsIdPlayQueue);
            Session.SongsIdPlayQueue = songsIdPlayQueue;
            LoadPlayQueue();
            dialogOpenEventArgs.Session.Close(true);
            dialogAddToQueueGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Adds the selected song to the end of the queue.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AddToTheEndButton_Click(object sender, RoutedEventArgs e) {
            if (playQueueDataGrid.SelectedItem is SongTable) {
                Session.SongsIdPlayQueue.Add(((SongTable)playQueueDataGrid.SelectedItem).Song.SongId);
            } else {
                Session.SongsIdPlayQueue.Add(((AccountSongTable)playQueueDataGrid.SelectedItem).AccountSong.AccountSongId * -1);
            }
            LoadPlayQueue();
            dialogOpenEventArgs.Session.Close(true);
            dialogAddToQueueGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Shows up a new window to add the selected song to a playlist.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
        private void AddToPlaylistMenuItem_Click(object sender, RoutedEventArgs e) {
            new AddSongToPlaylistWindow(((SongTable)playQueueDataGrid.SelectedItem).Song).Show();
            playQueueDataGrid.SelectedIndex = -1;
        }

        /// <summary>
        /// Generates a radio station with the selected song genre.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
        private void GenerateRadioStationMenuItem_Click(object sender, RoutedEventArgs e) {
            if (Session.GenresIdRadioStations.Find(x => x == ((SongTable)playQueueDataGrid.SelectedItem).Song.Genre.GenreId) == 0) {
                Session.GenresIdRadioStations.Add(((SongTable)playQueueDataGrid.SelectedItem).Song.Genre.GenreId);
            } else {
                MessageBox.Show("Ya existe la estación de radio de este género.");
            }
            playQueueDataGrid.SelectedIndex = -1;
        }

        /// <summary>
        /// Deletes the selected song from play queue.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
        private void RemoveFromQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            if (playQueueDataGrid.SelectedIndex < Session.SongsIdPlayQueue.Count) {
                Session.SongsIdPlayQueue.RemoveAt(playQueueDataGrid.SelectedIndex);
            } else {
                Session.SongsIdSongList.RemoveAt(playQueueDataGrid.SelectedIndex - Session.SongsIdPlayQueue.Count);
            }
            LoadPlayQueue();
        }
    }
}
