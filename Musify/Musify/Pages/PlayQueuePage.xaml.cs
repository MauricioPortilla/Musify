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
    /// Lógica de interacción para PlayQueuePage.xaml
    /// </summary>
    public partial class PlayQueuePage : Page {
        private readonly ObservableCollection<SongTable> songsPlayQueue = new ObservableCollection<SongTable>();
        public ObservableCollection<SongTable> SongsPlayQueue {
            get => songsPlayQueue;
        }

        public PlayQueuePage() {
            InitializeComponent();
            DataContext = this;
            LoadPlayQueue();
        }

        public void LoadPlayQueue() {
            songsPlayQueue.Clear();
            List<int> songsIdPlayQueue = Session.SongsIdPlayQueue;
            if (songsIdPlayQueue.Count > 0) {
                deletePlayQueueButton.Visibility = Visibility.Visible;
            } else {
                deletePlayQueueButton.Visibility = Visibility.Hidden;
            }
            LoadSong(0, songsIdPlayQueue.Count, songsIdPlayQueue);
        }

        public void LoadSong(int i, int limit, List<int> songsIdPlayQueue) {
            if (i < limit) {
                Song.FetchById(songsIdPlayQueue.ElementAt(i), (song) => {
                    SongsPlayQueue.Add(new SongTable {
                        Song = song,
                        Title = song.Title,
                        ArtistsNames = song.GetArtistsNames(),
                        Album = song.Album,
                        Duration = song.Duration
                    });
                    LoadSong(i + 1, limit, songsIdPlayQueue);
                }, () => {
                    MessageBox.Show("Ocurrió un error al cargar las canciones.");
                });
            }
        }

        private void PlayQueueDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            UIFunctions.SongTable_OnDoubleClick(sender, e);
            for (int i = 0; i <= playQueueDataGrid.SelectedIndex; i++) {
                Session.SongsIdPlayQueue.RemoveAt(0);
            }
            LoadPlayQueue();
        }

        private void PlayQueueDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (playQueueDataGrid.SelectedIndex >= 0) {
                optionsMenu.Visibility = Visibility.Visible;
            } else {
                optionsMenu.Visibility = Visibility.Hidden;
            }
        }

        private void DeletePlayQueueButton_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue = new List<int>();
            LoadPlayQueue();
        }

        private void AddToQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.Add(((SongTable)playQueueDataGrid.SelectedItem).Song.SongId);
            LoadPlayQueue();
        }

        private void AddToPlaylistMenuItem_Click(object sender, RoutedEventArgs e) {
            new AddSongToPlaylistWindow(((SongTable)playQueueDataGrid.SelectedItem).Song).Show();
            playQueueDataGrid.SelectedIndex = -1;
        }

        private void GenerateRadioStationMenuItem_Click(object sender, RoutedEventArgs e) {
            playQueueDataGrid.SelectedIndex = -1;
        }

        private void RemoveFromQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.RemoveAt(playQueueDataGrid.SelectedIndex);
            LoadPlayQueue();
        }
    }
}
