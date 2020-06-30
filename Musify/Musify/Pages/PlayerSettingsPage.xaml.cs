using System.Windows;
using System.Windows.Controls;

namespace Musify.Pages {
    /// <summary>
    /// Interaction logic for PlayerSettingsPage.xaml
    /// </summary>
    public partial class PlayerSettingsPage : Page {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public PlayerSettingsPage() {
            InitializeComponent();
            LoadPlayerSettings();
        }

        /// <summary>
        /// Loads player settings (song streaming quality).
        /// </summary>
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

        /// <summary>
        /// Change song streaming quality.
        /// </summary>
        /// <param name="sender">CheckBox</param>
        /// <param name="e">Event</param>
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
