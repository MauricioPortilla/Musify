using Musify.Pages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Musify {
    /// <summary>
    /// Lógica de interacción para MainMenuWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private Timer calculateDownloadSpeedTimer;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
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
            SetCalculateDownloadSpeedTimer();
        }

        /// <summary>
        /// Creates a timer that executes a method every 1 minute to calculate
        /// download speed.
        /// </summary>
        private void SetCalculateDownloadSpeedTimer() {
            calculateDownloadSpeedTimer = new Timer((timerEvent) => {
                if (Session.SongStreamingQuality == "automaticquality") {
                    Session.SetSongStreamingQualityAutomatically();
                }
            }, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        }

        /// <summary>
        /// Loads user configuration.
        /// </summary>
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

        /// <summary>
        /// Loads all songs in play queue.
        /// </summary>
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

        /// <summary>
        /// Loads all songs in play history.
        /// </summary>
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
            Session.historyIndex = Session.SongsIdPlayHistory.Count - 1;
        }

        /// <summary>
        /// Loads player configuration.
        /// </summary>
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

        /// <summary>
        /// Loads all radio stations stored.
        /// </summary>
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

        /// <summary>
        /// Shows up the menu.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
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

        /// <summary>
        /// Stores user data before closing the window.
        /// </summary>
        /// <param name="sender">Window</param>
        /// <param name="e">Event</param>
        private void Window_Closing(object sender, CancelEventArgs e) {
            App.CreateDirectories();
            string file = App.DATA_SONGS_DIRECTORY + "/playHistory" + Session.Account.AccountId;
            List<int> songsId = Session.SongsIdPlayHistory;
            if (Session.PlayerPage.LatestSongPlayed != null) {
                if (Session.SongsIdPlayHistory.Count == Core.MAX_SONGS_IN_HISTORY) {
                    Session.SongsIdPlayHistory.RemoveAt(0);
                }
                songsId.Add(Session.PlayerPage.LatestSongPlayed.SongId);
            } else {
                if (Session.PlayerPage.LatestAccountSongPlayed != null) {
                    if (Session.SongsIdPlayHistory.Count == Core.MAX_SONGS_IN_HISTORY) {
                        Session.SongsIdPlayHistory.RemoveAt(0);
                    }
                    songsId.Add(Session.PlayerPage.LatestAccountSongPlayed.AccountSongId * -1);
                }
            }
            FileStream fileStream = new FileStream(file, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            foreach (int songId in songsId) {
                binaryWriter.Write(songId);
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
                }, (errorResponse) => {
                    MessageBox.Show(errorResponse.Message);
                }, () => {
                    MessageBox.Show("Ocurrió un error al intentar suscribirse.");
                });
            }
        }
    }
}
