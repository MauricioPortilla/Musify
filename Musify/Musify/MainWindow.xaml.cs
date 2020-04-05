using Musify.Pages;
using System;
using System.Collections.Generic;
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
            mainFrame.Source = new Uri("Pages/PlaylistsPage.xaml", UriKind.RelativeOrAbsolute);
            playerFrame.Source = new Uri("Pages/PlayerPage.xaml", UriKind.RelativeOrAbsolute);
            Session.PlayerPage = playerFrame.Content as PlayerPage;
        }
        
        public void MenuButton_Click(object sender, RoutedEventArgs e) {
            MenuItem button = (MenuItem)sender;
            string opcion = button.Header.ToString();
            switch (opcion) {
                case "Menú principal":
                    mainFrame.Source = new Uri("Pages/PlaylistsPage.xaml", UriKind.RelativeOrAbsolute);
                    subscribeButton.Visibility = Visibility.Visible;
                    break;
                case "Cola de reproducción":
                    mainFrame.Source = new Uri("Pages/PlayQueuePage.xaml", UriKind.RelativeOrAbsolute);
                    subscribeButton.Visibility = Visibility.Hidden;
                    break;
            }
            TitleBar.Text = opcion;
        }
    }
}
