using Musify.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
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
using System.Windows.Shapes;

namespace Musify {
    /// <summary>
    /// Lógica de interacción para MainMenuWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Session.MainWindow = this;
            Session.MainFrame = mainFrame;
            mainFrame.Source = new Uri("Pages/PlaylistsPage.xaml", UriKind.RelativeOrAbsolute);
            Session.PlayerPage = new PlayerPage();
            playerFrame.Navigate(Session.PlayerPage);
            LoadConfiguration();
            if (Session.Account.Subscription != null) {
                subscribeButton.IsEnabled = false;
                subscribeButton.Content = "Suscrito";
            }
        }

        public void LoadConfiguration() {
            if (Session.Account.Artist.ArtisticName != null) {
                CreateAlbumMenuItem.Visibility = Visibility.Visible;
            }
            App.CreateDirectories();
            LoadSongsIdPlayQueue();
            LoadSongsIdPlayHistory();
            LoadConfigurationPlayer();
            LoadGenresIdRadioStations();
        }

        public void LoadSongsIdPlayQueue() {
            string file = App.DATA_SONGS_DIRECTORY + "/playQueue" + Session.Account.AccountId;
            if (File.Exists(file)) {
                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                for (int i = 0; i < fileStream.Length / 4; i++) {
                    Session.SongsIdPlayQueue.Add(binaryReader.ReadInt32());
                }
                binaryReader.Close();
                fileStream.Close();
            }
        }

        public void LoadSongsIdPlayHistory() {
            string file = App.DATA_SONGS_DIRECTORY + "/playHistory" + Session.Account.AccountId;
            if (File.Exists(file)) {
                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                for (int i = 0; i < fileStream.Length / 4; i++) {
                    Session.SongsIdPlayHistory.Add(binaryReader.ReadInt32());
                }
                binaryReader.Close();
                fileStream.Close();
            }
        }

        public void LoadConfigurationPlayer() {
            string file = App.DATA_PLAYER_DIRECTORY + "/player" + Session.Account.AccountId;
            if (File.Exists(file)) {
                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                if (fileStream.Length / 4 == 1) {
                    int configurationPlayer = binaryReader.ReadInt32();
                    switch (configurationPlayer) {
                        case 0:
                            Session.SongStreamingQuality = "automaticquality";
                            break;
                        case 1:
                            Session.SongStreamingQuality = "lowquality";
                            break;
                        case 2:
                            Session.SongStreamingQuality = "mediumquality";
                            break;
                        case 3:
                            Session.SongStreamingQuality = "highquality";
                            break;
                    }
                }
                binaryReader.Close();
                fileStream.Close();
            }
        }

        public void LoadGenresIdRadioStations() {
            string file = App.DATA_STATIONS_DIRECTORY + "/stations" + Session.Account.AccountId;
            if (File.Exists(file)) {
                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                for (int i = 0; i < fileStream.Length / 4; i++) {
                    Session.GenresIdRadioStations.Add(binaryReader.ReadInt32());
                }
                binaryReader.Close();
                fileStream.Close();
            }
        }

        public void MenuButton_Click(object sender, RoutedEventArgs e) {
            MenuItem button = (MenuItem) sender;
            string opcion = button.Name;
            subscribeButton.Visibility = Visibility.Hidden;
            switch (opcion) {
                case "MainMenuMenuItem":
                    mainFrame.Source = new Uri("Pages/PlaylistsPage.xaml", UriKind.RelativeOrAbsolute);
                    subscribeButton.Visibility = Visibility.Visible;
                    break;
                case "SearchMenuItem":
                    mainFrame.Source = new Uri("Pages/SearchPage.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "PlayQueueMenuItem":
                    mainFrame.Source = new Uri("Pages/PlayQueuePage.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "PlayHistoryMenuItem":
                    mainFrame.Source = new Uri("Pages/PlayHistoryPage.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "PersonalLibraryMenuItem":
                    mainFrame.Source = new Uri("Pages/ConsultAccountSongs.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "RadioStationsMenuItem":
                    mainFrame.Source = new Uri("Pages/RadioStationsPage.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "PlayerSettingsMenuItem":
                    mainFrame.Source = new Uri("Pages/PlayerSettingsPage.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "CreateAlbumMenuItem":
                    mainFrame.Source = new Uri("Pages/CreateAlbumPage.xaml", UriKind.RelativeOrAbsolute);
                    break;
            }
            TitleBar.Text = button.Header.ToString();
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            App.CreateDirectories();
            string file = App.DATA_SONGS_DIRECTORY + "/playHistory" + Session.Account.AccountId;
            List<int> songsId = Session.SongsIdPlayHistory;
            if (Session.PlayerPage.LatestSongPlayed != null) {
                songsId.Add(Session.PlayerPage.LatestSongPlayed.SongId);
            } else {
                if (Session.PlayerPage.LatestAccountSongPlayed != null) {
                    songsId.Add(Session.PlayerPage.LatestAccountSongPlayed.AccountSongId * -1);
                }
            }
            int start = 0;
            if (songsId.Count > Core.MAX_SONGS_IN_HISTORY) {
                start = songsId.Count - Core.MAX_SONGS_IN_HISTORY;
            }
            FileStream fileStream = new FileStream(file, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            for (int i = start; i < songsId.Count; i++) {
                binaryWriter.Write(songsId.ElementAt(i));
            }
            binaryWriter.Close();
            fileStream.Close();
            file = App.DATA_SONGS_DIRECTORY + "/playQueue" + Session.Account.AccountId;
            songsId = Session.SongsIdPlayQueue;
            fileStream = new FileStream(file, FileMode.Create);
            binaryWriter = new BinaryWriter(fileStream);
            foreach (int songId in songsId) {
                binaryWriter.Write(songId);
            }
            binaryWriter.Close();
            fileStream.Close();
            file = App.DATA_PLAYER_DIRECTORY + "/player" + Session.Account.AccountId;
            string configurationPlayer = Session.SongStreamingQuality;
            fileStream = new FileStream(file, FileMode.Create);
            binaryWriter = new BinaryWriter(fileStream);
            switch (configurationPlayer) {
                case "automaticquality":
                    binaryWriter.Write(0);
                    break;
                case "lowquality":
                    binaryWriter.Write(1);
                    break;
                case "mediumquality":
                    binaryWriter.Write(2);
                    break;
                case "highquality":
                    binaryWriter.Write(3);
                    break;
            }
            binaryWriter.Close();
            fileStream.Close(); 
            file = App.DATA_STATIONS_DIRECTORY + "/stations" + Session.Account.AccountId;
            List<int> genresId = Session.GenresIdRadioStations;
            fileStream = new FileStream(file, FileMode.Create);
            binaryWriter = new BinaryWriter(fileStream);
            foreach (int genreId in genresId) {
                binaryWriter.Write(genreId);
            }
            binaryWriter.Close();
            fileStream.Close();
        }

        /// <summary>
        /// Attempts to subscribe.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void SubscribeButton_Click(object sender, RoutedEventArgs e) {
            var messageDialog = MessageBox.Show("¿Desea suscribirse?", "Confirmar", MessageBoxButton.YesNo);
            if (messageDialog == MessageBoxResult.Yes) {
                Session.Account.Subscribe((subscription) => {
                    MessageBox.Show("Cuenta suscrita.");
                }, () => {
                    MessageBox.Show("Ocurrió un error al intentar suscribirse.");
                }, () => {
                    MessageBox.Show("Ocurrió un error al intentar suscribirse.");
                });
            }
        }
    }
}
