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
            string file = AppDomain.CurrentDomain.BaseDirectory + "\\data\\songs\\playHistory" + Session.Account.Name;
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            List<int> songsIdPlayHistory = Session.PlayerPage.PlayHistory;
            FileStream fileStream = new FileStream(file, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter(fileStream);
            foreach (int songID in songsIdPlayHistory) {
                binaryWriter.Write(songID);
            }
            binaryWriter.Close();
            fileStream.Close();
        }
    }
}
