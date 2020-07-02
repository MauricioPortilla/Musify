using MaterialDesignThemes.Wpf;
using Musify.Models;
using Musify.Pages;
using System.Windows;
using System.Windows.Controls;

namespace Musify {
    /// <summary>
    /// Interaction logic for PlaylistsPage.xaml
    /// </summary>
    public partial class PlaylistsPage : Page {
        private DialogOpenedEventArgs dialogOpenEventArgs;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public PlaylistsPage() {
            InitializeComponent();
            LoadPlaylists();
        }

        /// <summary>
        /// Loads all the account playlists.
        /// </summary>
        private void LoadPlaylists() {
            playlistsListBox.Items.Clear();
            Playlist.FetchByAccountId(Session.Account.AccountId, (playlists) => {
                foreach (var playlist in playlists) {
                    playlistsListBox.Items.Add(playlist);
                }
            }, (errorResponse) => {
                MessageBox.Show(errorResponse.Message);
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar las listas de reproducción.");
            });
        }

        /// <summary>
        /// Opens up a dialog to create a new playlist.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void NewPlaylistButton_Click(object sender, RoutedEventArgs e) {
            DialogHost.Show(mainStackPanel, "PlaylistsPage_WindowDialogHost", (openSender, openEventArgs) => {
                dialogOpenEventArgs = openEventArgs;
                dialogCreatePlaylistGrid.Visibility = Visibility.Visible;
            }, null);
        }

        /// <summary>
        /// Creates a new playlist.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void CreatePlaylistButton_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(dialogPlaylistNameTextBox.Text)) {
                MessageBox.Show("Debes ingresar un nombre.");
                return;
            }
            Playlist newPlaylist = new Playlist {
                Name = dialogPlaylistNameTextBox.Text,
                AccountId = Session.Account.AccountId
            };
            newPlaylist.Save((_) => {
                dialogOpenEventArgs.Session.Close(true);
                dialogCreatePlaylistGrid.Visibility = Visibility.Collapsed;
                LoadPlaylists();
            }, (errorResponse) => {
                MessageBox.Show(errorResponse.Message);
            }, () => {
                MessageBox.Show("Ocurrió un error al crear la lista de reproducción.");
            });
        }

        /// <summary>
        /// Shows up a consult playlist page with the selected playlist.
        /// </summary>
        /// <param name="sender">ListBox</param>
        /// <param name="e">Event</param>
        private void PlaylistsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ConsultPlaylistPage consultPlaylistPage = new ConsultPlaylistPage(playlistsListBox.SelectedItem as Playlist);
            Session.MainWindow.mainFrame.Navigate(consultPlaylistPage);
        }
    }
}
