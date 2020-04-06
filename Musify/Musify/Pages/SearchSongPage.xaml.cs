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
    /// Lógica de interacción para SearchSongPage.xaml
    /// </summary>
    public partial class SearchSongPage : Page {

        private readonly ObservableCollection<SongTable> songList = new ObservableCollection<SongTable>();
        public ObservableCollection<SongTable> SongList {
            get => songList;
        }
        public SearchSongPage() {
            InitializeComponent();
            DataContext = this;
        }

        private void SongNameTextBox_KeyUp(object sender, KeyEventArgs e) {
            if (string.IsNullOrWhiteSpace(songNameTextBox.Text)) {
                songList.Clear();
                return;
            } else if (songNameTextBox.Text.Length < 3) {
                return;
            }
            Song.FetchByTitleCoincidences(songNameTextBox.Text, (songs) => {
                songList.Clear();
                foreach (Song song in songs) {
                    songList.Add(new SongTable {
                        Title = song.Title,
                        ArtistsNames = song.Album.GetArtistsNames(),
                        Album = song.Album,
                        Duration = song.Duration
                    });
                }
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar las canciones.");
            });
        }
    }
}
