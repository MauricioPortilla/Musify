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
        private readonly ObservableCollection<SongTable> songPlayHistory = new ObservableCollection<SongTable>();
        public ObservableCollection<SongTable> SongPlayHistory {
            get => songPlayHistory;
        }

        public PlayHistoryPage() {
            InitializeComponent();
            DataContext = this;
            LoadSongPlayHistory();
        }

        public void LoadSongPlayHistory() {
            songPlayHistory.Clear();
            List<int> idSongsPlayHistory = Session.SongsIdPlayHistory;
            for (int i = idSongsPlayHistory.Count - 1; i <= 0; i--) {
                Song.FetchById(idSongsPlayHistory.ElementAt(i), (song) => {
                    songPlayHistory.Add(new SongTable {
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
    }
}
