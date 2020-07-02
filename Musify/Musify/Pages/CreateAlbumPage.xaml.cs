using Forms = System.Windows.Forms;
using Musify.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Musify.Pages {
    /// <summary>
    /// Interaction logic for CreateAlbumPage.xaml
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

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public CreateAlbumPage() {
            InitializeComponent();
            DataContext = this;
            artistsList.Add(Session.Account.Artist);
            for (int i = DateTime.Now.Year; i >= 1900; i--) {
                yearsList.Add(i);
            }
        }

        /// <summary>
        /// Searches a artist that starts with given string.
        /// </summary>
        /// <param name="sender">TextBox</param>
        /// <param name="e">Event</param>
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
            }, (errorResponse) => {
                MessageBox.Show(errorResponse.Message);
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar los artistas.");
            });
        }

        /// <summary>
        /// Add an artist to the song, if it has not been added.
        /// </summary>
        /// <param name="sender">ListBox</param>
        /// <param name="e">Event</param>
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

        /// <summary>
        /// Adds a image to album.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void SelectImageButton_Click(object sender, RoutedEventArgs e) {
            Forms.OpenFileDialog fileExplorer = new Forms.OpenFileDialog();
            fileExplorer.Filter = "Image Files|*.png";
            fileExplorer.Multiselect = false;
            if (fileExplorer.ShowDialog() == Forms.DialogResult.OK) {
                selectedImage = fileExplorer.FileNames[0];
                imageNameTextBlock.Text = selectedImage.Split('\\').Last();
            }
        }

        /// <summary>
        /// Deletes the selected artist.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void DeleteArtistButton_Click(object sender, RoutedEventArgs e) {
            if (artistsListBox.SelectedIndex == -1) {
                return;
            }
            artistsList.RemoveAt(artistsListBox.SelectedIndex);
        }

        /// <summary>
        /// Shows up a add song to album paget.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AddSongButton_Click(object sender, RoutedEventArgs e) {
            new AddSongToAlbumWindow(this).Show();
        }

        /// <summary>
        /// Deletes the selected song.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void DeleteSongButton_Click(object sender, RoutedEventArgs e) {
            if (songsListBox.SelectedIndex == -1) {
                return;
            }
            songsList.RemoveAt(songsListBox.SelectedIndex);
        }

        /// <summary>
        /// Creates a new album if all the fields are complete..
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AcceptButton_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(albumNameTextBox.Text) || string.IsNullOrWhiteSpace(discographyTextBox.Text) || string.IsNullOrWhiteSpace(launchYearComboBox.Text)
                    || string.IsNullOrWhiteSpace(selectedImage) || artistsList.Count <= 0 || songsList.Count <= 0) {
                MessageBox.Show("Faltan campos por completar.");
                return;
            }
            string type = "Álbum";
            if (songsList.Count == 1) {
                type = "Sencillo";
            }
            Album album = new Album {
                Type = type,
                Name = albumNameTextBox.Text,
                LaunchYear = int.Parse(launchYearComboBox.Text),
                Discography = discographyTextBox.Text,
                ImageLocation = selectedImage,
                Artists = artistsList.ToList(),
                Songs = songsList.ToList()
            };
            album.Save(() => {
                albumNameTextBox.Text = "";
                discographyTextBox.Text = "";
                launchYearComboBox.Text = "";
                imageNameTextBlock.Text = "";
                artistSearchTextBox.Text = "";
                artistsList.Clear();
                artistsList.Add(Session.Account.Artist);
                songsList.Clear();
                MessageBox.Show("Se creó el álbum con éxito. Las canciones se están procesando.");
            }, (errorResponse) => {
                MessageBox.Show(errorResponse.Message);
            }, () => {
                MessageBox.Show("Ocurrió un error al crear el álbum.");
            });
        }
    }
}
