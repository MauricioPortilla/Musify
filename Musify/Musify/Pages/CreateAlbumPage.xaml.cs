using Musify.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Lógica de interacción para CreateAlbumPage.xaml
    /// </summary>
    public partial class CreateAlbumPage : Page {
        private readonly ObservableCollection<Artist> artistsList = new ObservableCollection<Artist>();
        public ObservableCollection<Artist> ArtistsList {
            get => artistsList;
        }

        public CreateAlbumPage() {
            InitializeComponent();
            DataContext = this;
        }

        private void ArtistSearchTextBox_KeyUp(object sender, KeyEventArgs e) {
            if (string.IsNullOrWhiteSpace(artistSearchTextBox.Text)) {
                artistsFoundListBox.Items.Clear();
                return;
            } else if (artistSearchTextBox.Text.Length < 3) {
                return;
            }
            Artist.FetchByArtisticNameCoincidences(artistSearchTextBox.Text, (artists) => {
                artistsFoundListBox.Items.Clear();
                foreach (var artist in artists) {
                    artistsFoundListBox.Items.Add(artist);
                }
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar los artistas.");
            });
        }

        private void artistsFoundListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (artistsFoundListBox.SelectedIndex == -1) {
                return;
            }
            foreach (Artist artist in artistsList) {
                if (artist.ArtisticName.Equals((artistsFoundListBox.SelectedItem as Artist).ArtisticName)) {
                    artistsFoundListBox.Items.Clear();
                    artistSearchTextBox.Text = "";
                    MessageBox.Show("Este artista ya se encuentra agregado.");
                    return;
                }
            }
            artistsList.Add(artistsFoundListBox.SelectedItem as Artist);
            artistsFoundListBox.Items.Clear();
            artistSearchTextBox.Text = "";
        }

        private void DeleteArtistButton_Click(object sender, RoutedEventArgs e) {
        }

        private void AddSongButton_Click(object sender, RoutedEventArgs e) {
        }

        private void DeleteSongButton_Click(object sender, RoutedEventArgs e) {
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e) {
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
        }
    }
}
