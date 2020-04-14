using Musify.Models;
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
    /// Lógica de interacción para AddSongToPlaylistPage.xaml
    /// </summary>
    public partial class AddSongToPlaylistWindow : Window {

        private Song songToAdd;

        public AddSongToPlaylistWindow(Song songToAdd) {
            InitializeComponent();
            this.songToAdd = songToAdd;
            LoadPlaylists();
        }

        private void LoadPlaylists() {
            playlistsListBox.Items.Clear();
            Playlist.Fetch(Session.Account.AccountId, (playlists) => {
                foreach (var playlist in playlists) {
                    playlistsListBox.Items.Add(playlist);
                }
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar las listas de reproducción.");
            });
        }

        private void AddButton_Click(object sender, RoutedEventArgs e) {
            if (playlistsListBox.SelectedItem == null) {
                MessageBox.Show("Debes seleccionar una lista de reproducción.");
                return;
            }
            Playlist playlistSelected = playlistsListBox.SelectedItem as Playlist;
            playlistSelected.ContainsSong(songToAdd, () => {
                MessageBox.Show("Esta canción ya existe en esta lista de reproducción.");
            }, () => {
                playlistSelected.AddSong(songToAdd, () => {
                    Close();
                }, () => {
                    MessageBox.Show("Ocurrió un error al guardar la canción en la lista de reproducción.");
                });
            });
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
