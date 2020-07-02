using Forms = System.Windows.Forms;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Musify.Models;
using Musify.Pages;

namespace Musify {
    /// <summary>
    /// Interaction logic for AddSongToAlbumWindow.xaml
    /// </summary>
    public partial class AddSongToAlbumWindow : Window {
        private CreateAlbumPage createAlbumPage;
        private string selectedSong = "";
        private readonly ObservableCollection<Artist> artistsList = new ObservableCollection<Artist>();
        public ObservableCollection<Artist> ArtistsList {
            get => artistsList;
        }
        private readonly ObservableCollection<Genre> genresList = new ObservableCollection<Genre>();
        public ObservableCollection<Genre> GenresList {
            get => genresList;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="createAlbumPage">Page where the album is being created</param>
        public AddSongToAlbumWindow(CreateAlbumPage createAlbumPage) {
            InitializeComponent();
            this.createAlbumPage = createAlbumPage;
            DataContext = this;
            artistsList.Add(Session.Account.Artist);
            LoadGenres();
        }

        /// <summary>
        /// Loads all genres.
        /// </summary>
        private void LoadGenres() {
            Genre.FetchAll((genres) => {
                genresList.Clear();
                foreach (Genre genre in genres) {
                    genresList.Add(genre);
                }
            }, (errorResponse) => {
                MessageBox.Show(errorResponse.Message);
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar los géneros.");
            });
        }

        /// <summary>
        /// Adds a song to album.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void SelectSongButton_Click(object sender, RoutedEventArgs e) {
            Forms.OpenFileDialog fileExplorer = new Forms.OpenFileDialog();
            fileExplorer.Filter = "MP3 Files|*.mp3|WAV Files|*.wav";
            fileExplorer.Multiselect = false;
            if (fileExplorer.ShowDialog() == Forms.DialogResult.OK) {
                selectedSong = fileExplorer.FileNames[0];
                songNameTextBlock.Text = selectedSong.Split('\\').Last();
                if (songNameTextBlock.Text.Split('.').First().Length > 255) {
                    songNameTextBox.Text = songNameTextBlock.Text.Split('.').First().Substring(0,255);
                } else {
                    songNameTextBox.Text = songNameTextBlock.Text.Split('.').First();
                }
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
        /// Deletes the selected artist.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void DeleteArtistButton_Click(object sender, RoutedEventArgs e) {
            artistsList.RemoveAt(artistsListBox.SelectedIndex);
        }

        /// <summary>
        /// Add the song to the album if all the fields are complete.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AcceptButton_Click(object sender, RoutedEventArgs e) {
            if (string.IsNullOrWhiteSpace(selectedSong) || string.IsNullOrWhiteSpace(songNameTextBox.Text) || string.IsNullOrWhiteSpace(genreComboBox.Text) || artistsList.Count <= 0) {
                MessageBox.Show("Faltan campos por completar.");
                return;
            }
            createAlbumPage.SongsList.Add(new Song {
                GenreId = (genreComboBox.SelectedItem as Genre).GenreId,
                Title = songNameTextBox.Text,
                SongLocation = selectedSong,
                Artists = artistsList.ToList()
            });
            Close();
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
