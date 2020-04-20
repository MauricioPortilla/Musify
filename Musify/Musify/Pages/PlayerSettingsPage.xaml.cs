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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Musify.Pages {
    /// <summary>
    /// Lógica de interacción para PlayerSettingsPage.xaml
    /// </summary>
    public partial class PlayerSettingsPage : Page {
        public PlayerSettingsPage() {
            InitializeComponent();
            LoadPlayerSettings();
        }

        public void LoadPlayerSettings() {
            string songStreamingQuality = Session.SongStreamingQuality;
            switch (songStreamingQuality) {
                case "lowQuality":
                    lowCheckBox.IsChecked = true;
                    break;
                case "mediumQuality":
                    mediumCheckBox.IsChecked = true;
                    break;
                case "highQuality":
                    highCheckBox.IsChecked = true;
                    break;
                default:
                    automaticCheckBox.IsChecked = true;
                    break;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) {
            string songStreamingQuality = ((CheckBox)sender).Name;
            switch (songStreamingQuality) {
                case "lowCheckBox":
                    Session.SongStreamingQuality = "lowQuality";
                    mediumCheckBox.IsChecked = false;
                    highCheckBox.IsChecked = false;
                    automaticCheckBox.IsChecked = false;
                    break;
                case "mediumCheckBox":
                    Session.SongStreamingQuality = "mediumQuality";
                    lowCheckBox.IsChecked = false;
                    highCheckBox.IsChecked = false;
                    automaticCheckBox.IsChecked = false;
                    break;
                case "highCheckBox":
                    Session.SongStreamingQuality = "highQuality";
                    lowCheckBox.IsChecked = false;
                    mediumCheckBox.IsChecked = false;
                    automaticCheckBox.IsChecked = false;
                    break;
                default:
                    lowCheckBox.IsChecked = false;
                    mediumCheckBox.IsChecked = false;
                    highCheckBox.IsChecked = false;
                    break;
            }

        }
    }
}
