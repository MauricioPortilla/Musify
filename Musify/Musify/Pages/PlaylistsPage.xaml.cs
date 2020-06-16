using MaterialDesignThemes.Wpf;
using Musify.Models;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Musify {
    /// <summary>
    /// Lógica de interacción para PlaylistsPage.xaml
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
            DialogHost.Show(mainStackPanel, "PlaylistPage_WindowDialogHost", (openSender, openEventArgs) => {
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
            }, () => {
                MessageBox.Show("Ocurrió un error al crear la lista de reproducción.");
            });
        }

        /// <summary>
        /// Shows up a consult playlist page with the selected playlist.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void PlaylistsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ConsultPlaylistPage consultPlaylistPage = new ConsultPlaylistPage(playlistsListBox.SelectedItem as Playlist);
            Session.MainFrame.Navigate(consultPlaylistPage);
        }
    }
}
