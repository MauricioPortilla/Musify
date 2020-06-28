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
using static Musify.Models.AccountSong;
using static Musify.Models.Song;

namespace Musify.Pages {
    /// <summary>
    /// Lógica de interacción para PlayQueuePage.xaml
    /// </summary>
    public partial class PlayQueuePage : Page {
        private DialogOpenedEventArgs dialogOpenEventArgs;
        private readonly ObservableCollection<object> songsPlayQueue = new ObservableCollection<object>();
        public ObservableCollection<object> SongsPlayQueue {
            get => songsPlayQueue;
        }

        public PlayQueuePage() {
            InitializeComponent();
            DataContext = this;
            LoadPlayQueue();
        }

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
                    }, () => {
                        MessageBox.Show("Ocurrió un error al cargar las canciones.");
                    });
                }
            }
        }

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

        private void DeletePlayQueueButton_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue = new List<int>();
            LoadPlayQueue();
        }

        private void AddToQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            DialogHost.Show(mainStackPanel, "PlayQueuePage_WindowDialogHost", (openSender, openEventArgs) => {
                dialogOpenEventArgs = openEventArgs;
                dialogAddToQueueGrid.Visibility = Visibility.Visible;
            }, null);
        }

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

        private void AddToPlaylistMenuItem_Click(object sender, RoutedEventArgs e) {
            new AddSongToPlaylistWindow(((SongTable)playQueueDataGrid.SelectedItem).Song).Show();
            playQueueDataGrid.SelectedIndex = -1;
        }

        private void GenerateRadioStationMenuItem_Click(object sender, RoutedEventArgs e) {
            if (Session.GenresIdRadioStations.Find(x => x == ((SongTable)playQueueDataGrid.SelectedItem).Song.Genre.GenreId) == 0) {
                Session.GenresIdRadioStations.Add(((SongTable)playQueueDataGrid.SelectedItem).Song.Genre.GenreId);
            } else {
                MessageBox.Show("Ya existe la estación de radio de este género.");
            }
            playQueueDataGrid.SelectedIndex = -1;
        }

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
