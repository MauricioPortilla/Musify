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
    /// Lógica de interacción para PlayHistoryPage.xaml
    /// </summary>
    public partial class PlayHistoryPage : Page {
        private readonly ObservableCollection<SongTable> songsPlayHistory = new ObservableCollection<SongTable>();
        public ObservableCollection<SongTable> SongsPlayHistory {
            get => songsPlayHistory;
        }

        public PlayHistoryPage() {
            InitializeComponent();
            DataContext = this;
            LoadSongPlayHistory();
        }

        public void LoadSongPlayHistory() {
            songsPlayHistory.Clear();
            List<int> songsIdPlayHistory = Session.SongsIdPlayHistory;
            int limit = 0;
            if (songsIdPlayHistory.Count > 50) {
                limit = songsIdPlayHistory.Count - 50;
            }
            for (int i = songsIdPlayHistory.Count - 1; i >= limit; i--) {
                Song.FetchById(songsIdPlayHistory.ElementAt(i), (song) => {
                    songsPlayHistory.Add(new SongTable {
                        Song = song,
                        Title = song.Title,
                        ArtistsNames = song.Album.GetArtistsNames(),
                        Album = song.Album,
                        Duration = song.Duration
                    });
                }, () => {
                    MessageBox.Show("Ocurrió un error al cargar las canciones.");
                });
            }
        }

        private void PlayHistoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            UIFunctions.SongTable_OnDoubleClick(sender, e);
        }

        private void PlayHistoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (playHistoryDataGrid.SelectedIndex >= 0) {
                optionsMenu.Visibility = Visibility.Visible;
            } else {
                optionsMenu.Visibility = Visibility.Hidden;
            }
        }

        private void AddToQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.Add(((SongTable)playHistoryDataGrid.SelectedItem).Song.SongId);
            playHistoryDataGrid.SelectedIndex = -1;
        }

        private void AddToPlaylistMenuItem_Click(object sender, RoutedEventArgs e) {

        }

        private void GenerateRadioStationMenuItem_Click(object sender, RoutedEventArgs e) {

        }
    }
}
