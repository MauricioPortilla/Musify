using Musify.Models;
using System.Windows;

namespace Musify {
    /// <summary>
    /// Lógica de interacción para AddSongToPlaylistPage.xaml
    /// </summary>
    public partial class AddSongToPlaylistWindow : Window {

        private Song songToAdd;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="songToAdd">Song to add</param>
        public AddSongToPlaylistWindow(Song songToAdd) {
            InitializeComponent();
            this.songToAdd = songToAdd;
            LoadPlaylists();
        }

        /// <summary>
        /// Loads all account playlists.
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
        /// Adds the song to the selected playlist.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AddButton_Click(object sender, RoutedEventArgs e) {
            if (playlistsListBox.SelectedItem == null) {
                MessageBox.Show("Debes seleccionar una lista de reproducción.");
                return;
            }
            Playlist playlistSelected = playlistsListBox.SelectedItem as Playlist;
            playlistSelected.ContainsSong(songToAdd, () => {
                MessageBox.Show("Esta canción ya existe en esta lista de reproducción.");
            }, (errorResponse) => {
                playlistSelected.AddSong(songToAdd, () => {
                    Close();
                }, (errorResponse2) => {
                    MessageBox.Show(errorResponse2.Message);
                }, () => {
                    MessageBox.Show("Ocurrió un error al guardar la canción en la lista de reproducción.");
                });
            }, () => {
                MessageBox.Show("Ocurrió un error al guardar la canción en la lista de reproducción.");
            });
        }

        /// <summary>
        /// Closes the window.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
