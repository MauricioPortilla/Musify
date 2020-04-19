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
        }

        public void LoadConfiguration() {
            string directory = AppDomain.CurrentDomain.BaseDirectory + "\\data\\songs";
            if (Directory.Exists(directory)) {
                LoadSongsIdPlayQueue();
                LoadSongsIdPlayHistory();
            } else {
                Directory.CreateDirectory(directory);
                File.Create(AppDomain.CurrentDomain.BaseDirectory + "\\data\\songs\\playQueue" + Session.Account.AccountId);
                File.Create(AppDomain.CurrentDomain.BaseDirectory + "\\data\\songs\\playHistory" + Session.Account.AccountId);
            }
            directory = AppDomain.CurrentDomain.BaseDirectory + "\\data\\player";
            if (Directory.Exists(directory)) {
                LoadConfigurationPlayer();
            } else {
                Directory.CreateDirectory(directory);
                File.Create(AppDomain.CurrentDomain.BaseDirectory + "\\data\\player\\player" + Session.Account.AccountId);
            }
        }

        public void LoadSongsIdPlayQueue() {
            string file = AppDomain.CurrentDomain.BaseDirectory + "\\data\\songs\\playQueue" + Session.Account.AccountId;
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
            string file = AppDomain.CurrentDomain.BaseDirectory + "\\data\\songs\\playHistory" + Session.Account.AccountId;
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
            string file = AppDomain.CurrentDomain.BaseDirectory + "\\data\\player\\player" + Session.Account.AccountId;
            if (File.Exists(file)) {
                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                BinaryReader binaryReader = new BinaryReader(fileStream);
                if (fileStream.Length / 4 == 1) {
                    int configurationPlayer = binaryReader.ReadInt32();
                    switch (configurationPlayer) {
                        case 0:
                            Session.SongStreamingQuality = "automaticQuality";
                            break;
                        case 1:
                            Session.SongStreamingQuality = "lowQuality";
                            break;
                        case 2:
                            Session.SongStreamingQuality = "mediumQuality";
                            break;
                        case 3:
                            Session.SongStreamingQuality = "highQuality";
                            break;
                    }
                }
                binaryReader.Close();
                fileStream.Close();
            }
        }

        public void MenuButton_Click(object sender, RoutedEventArgs e) {
            MenuItem button = (MenuItem) sender;
            string opcion = button.Header.ToString();
            subscribeButton.Visibility = Visibility.Hidden;
            switch (opcion) {
                case "Menú principal":
                    mainFrame.Source = new Uri("Pages/PlaylistsPage.xaml", UriKind.RelativeOrAbsolute);
                    subscribeButton.Visibility = Visibility.Visible;
                    break;
                case "Buscar canción":
                    mainFrame.Source = new Uri("Pages/SearchSongPage.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Cola de reproducción":
                    mainFrame.Source = new Uri("Pages/PlayQueuePage.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Historial de reproducción":
                    mainFrame.Source = new Uri("Pages/PlayHistoryPage.xaml", UriKind.RelativeOrAbsolute);
                    break;
                case "Biblioteca personal":
                    mainFrame.Source = new Uri("Pages/ConsultAccountSongs.xaml", UriKind.RelativeOrAbsolute);
                    break;
            }
            TitleBar.Text = opcion;
        }

        private void Window_Closing(object sender, CancelEventArgs e) {
            string directory = AppDomain.CurrentDomain.BaseDirectory + "\\data\\songs";
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            directory = AppDomain.CurrentDomain.BaseDirectory + "\\data\\player";
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            string file = AppDomain.CurrentDomain.BaseDirectory + "\\data\\songs\\playHistory" + Session.Account.AccountId;
            List<int> songsId = Session.SongsIdPlayHistory;
            int limit = 0;
            if (songsId.Count > 50) {
                limit = songsId.Count - 50;
            }
            FileStream fileStream = new FileStream(file, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            for (int i = songsId.Count - 1; i >= limit; i--) {
                binaryWriter.Write(songsId.ElementAt(i));
            }
            binaryWriter.Close();
            fileStream.Close();
            file = AppDomain.CurrentDomain.BaseDirectory + "\\data\\songs\\playQueue" + Session.Account.AccountId;
            songsId = Session.SongsIdPlayQueue;
            fileStream = new FileStream(file, FileMode.Create);
            binaryWriter = new BinaryWriter(fileStream);
            foreach (int songId in songsId) {
                binaryWriter.Write(songId);
            }
            binaryWriter.Close();
            fileStream.Close();
            file = AppDomain.CurrentDomain.BaseDirectory + "\\data\\player\\player" + Session.Account.AccountId;
            string configurationPlayer = Session.SongStreamingQuality;
            fileStream = new FileStream(file, FileMode.Create);
            binaryWriter = new BinaryWriter(fileStream);
            switch (configurationPlayer) {
                case "automaticQuality":
                    binaryWriter.Write(0);
                    break;
                case "lowQuality":
                    binaryWriter.Write(1);
                    break;
                case "mediumQuality":
                    binaryWriter.Write(2);
                    break;
                case "highQuality":
                    binaryWriter.Write(3);
                    break;
            }
            binaryWriter.Close();
            fileStream.Close();
        }
    }
}
