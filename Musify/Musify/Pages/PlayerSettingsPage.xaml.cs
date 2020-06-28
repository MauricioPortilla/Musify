using System.Windows;
using System.Windows.Controls;

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
                case "lowquality":
                    lowCheckBox.IsChecked = true;
                    break;
                case "mediumquality":
                    mediumCheckBox.IsChecked = true;
                    break;
                case "highquality":
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
                    Session.SongStreamingQuality = "lowquality";
                    mediumCheckBox.IsChecked = false;
                    highCheckBox.IsChecked = false;
                    automaticCheckBox.IsChecked = false;
                    break;
                case "mediumCheckBox":
                    Session.SongStreamingQuality = "mediumquality";
                    lowCheckBox.IsChecked = false;
                    highCheckBox.IsChecked = false;
                    automaticCheckBox.IsChecked = false;
                    break;
                case "highCheckBox":
                    Session.SongStreamingQuality = "highquality";
                    lowCheckBox.IsChecked = false;
                    mediumCheckBox.IsChecked = false;
                    automaticCheckBox.IsChecked = false;
                    break;
                default:
                    Session.SongStreamingQuality = "automaticquality";
                    lowCheckBox.IsChecked = false;
                    mediumCheckBox.IsChecked = false;
                    highCheckBox.IsChecked = false;
                    break;
            }

        }
    }
}
