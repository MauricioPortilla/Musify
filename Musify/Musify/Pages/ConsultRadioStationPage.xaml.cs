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
    /// Lógica de interacción para ConsultRadioStationPage.xaml
    /// </summary>
    public partial class ConsultRadioStationPage : Page {
        private DialogOpenedEventArgs dialogOpenEventArgs;
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
            }, (errorResponse) => {
                MessageBox.Show(errorResponse.Message);
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar la estación de radio.");
            });
        }

        private void SongsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            UIFunctions.SongTable_OnDoubleClick(sender, e);
            Session.historyIndex = Session.SongsIdPlayHistory.Count - 1;
            Session.SongsIdSongList.Clear();
            for (int i = songsDataGrid.SelectedIndex + 1; i < songsRadioStation.Count; i++) {
                Session.SongsIdSongList.Add(songsRadioStation.ElementAt(i).Song.SongId);
            }
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
            DialogHost.Show(mainStackPanel, "ConsultRadioStationPage_WindowDialogHost", (openSender, openEventArgs) => {
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
    }
}
