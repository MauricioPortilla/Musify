using Forms = System.Windows.Forms;
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
        private string selectedImage = "";
        private readonly ObservableCollection<Artist> artistsList = new ObservableCollection<Artist>();
        public ObservableCollection<Artist> ArtistsList {
            get => artistsList;
        }
        private readonly ObservableCollection<Song> songsList = new ObservableCollection<Song>();
        public ObservableCollection<Song> SongsList {
            get => songsList;
        }
        private readonly ObservableCollection<int> yearsList = new ObservableCollection<int>();
        public ObservableCollection<int> YearsList {
            get => yearsList;
        }

        public CreateAlbumPage() {
            InitializeComponent();
            DataContext = this;
            artistsList.Add(Session.Account.Artist);
            for (int i = DateTime.Now.Year; i >= 1900; i--) {
                yearsList.Add(i);
            }
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

        private void ArtistsFoundListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
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

        private void SelectImageButton_Click(object sender, RoutedEventArgs e) {
            Forms.OpenFileDialog fileExplorer = new Forms.OpenFileDialog();
            fileExplorer.Filter = "Image Files|*.jpg;*.png";
            fileExplorer.Multiselect = false;
            if (fileExplorer.ShowDialog() == Forms.DialogResult.OK) {
                selectedImage = fileExplorer.FileNames[0];
                imageNameTextBlock.Text = selectedImage.Split('\\').Last();
            }
        }

        private void DeleteArtistButton_Click(object sender, RoutedEventArgs e) {
            artistsList.RemoveAt(artistsListBox.SelectedIndex);
        }

        private void AddSongButton_Click(object sender, RoutedEventArgs e) {
            new AddSongToAlbumWindow(this).Show();
        }

        private void DeleteSongButton_Click(object sender, RoutedEventArgs e) {
            songsList.RemoveAt(songsListBox.SelectedIndex);
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e) {
            if (!albumNameTextBox.Text.Equals("") && !discographyTextBox.Text.Equals("") && !launchYearComboBox.Text.Equals("") && !selectedImage.Equals("") 
                && artistsList.Count > 0 && songsList.Count > 0) {
                // TODO Create album.
            } else {
                MessageBox.Show("Faltan campos por completar.");
            }
        }
    }
}
