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
    /// Lógica de interacción para ConsultRadioStationPage.xaml
    /// </summary>
    public partial class ConsultRadioStationPage : Page {
        private Genre genre;
        private ObservableCollection<SongTable> songsRadioStation = new ObservableCollection<SongTable>();
        public ObservableCollection<SongTable> SongsRadioStation {
            get => songsRadioStation;
        }

        public ConsultRadioStationPage(Genre genre) {
            InitializeComponent();
            DataContext = this;
            Session.MainWindow.TitleBar.Text = "Estación de radio";
            this.genre = genre;
            radioStationNameTextBlock.Text = genre.Name;
            LoadRadioStationSongs();
        }

        private void LoadRadioStationSongs() {
            genre.FetchSongs((songs) => {
                songsRadioStation.Clear();
                foreach (Song radioStationSong in songs) {
                    songsRadioStation.Add(new SongTable {
                        Song = radioStationSong,
                        Title = radioStationSong.Title,
                        Album = radioStationSong.Album,
                        Genre = radioStationSong.Genre,
                        ArtistsNames = radioStationSong.GetArtistsNames(),
                        Duration = radioStationSong.Duration
                    });
                }
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar la estación de radio.");
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

        private void DeleteRadioStationButton_Click(object sender, RoutedEventArgs e) {
            Session.GenresIdRadioStations.Remove(genre.GenreId);
            Session.MainWindow.TitleBar.Text = "Estaciones de radio";
            RadioStationsPage radioStationPage = new RadioStationsPage();
            Session.MainFrame.Navigate(radioStationPage);
        }

        private void AddToQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.Add(((SongTable)songsDataGrid.SelectedItem).Song.SongId);
            songsDataGrid.SelectedIndex = -1;
        }

        private void AddToPlaylistMenuItem_Click(object sender, RoutedEventArgs e) {
            new AddSongToPlaylistWindow(((SongTable)songsDataGrid.SelectedItem).Song).Show();
            songsDataGrid.SelectedIndex = -1;
        }
    }
}
