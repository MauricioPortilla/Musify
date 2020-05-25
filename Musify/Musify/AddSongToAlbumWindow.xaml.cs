using Forms = System.Windows.Forms;
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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Musify.Models;
using Musify.Pages;

namespace Musify {
    /// <summary>
    /// Lógica de interacción para AddSongToAlbumWindow.xaml
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

        public AddSongToAlbumWindow(CreateAlbumPage createAlbumPage) {
            InitializeComponent();
            this.createAlbumPage = createAlbumPage;
            DataContext = this;
            artistsList.Add(Session.Account.Artist);
            Genre.FetchAll((genres) => {
                genresList.Clear();
                foreach (Genre genre in genres) {
                    genresList.Add(genre);
                }
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar los géneros.");
            });
        }

        private void SelectSongButton_Click(object sender, RoutedEventArgs e) {
            Forms.OpenFileDialog fileExplorer = new Forms.OpenFileDialog();
            fileExplorer.Filter = "MP3 Files|*.mp3|WAV Files|*.wav";
            fileExplorer.Multiselect = false;
            if (fileExplorer.ShowDialog() == Forms.DialogResult.OK) {
                selectedSong = fileExplorer.FileNames[0];
                songNameTextBlock.Text = selectedSong.Split('\\').Last();
                songNameTextBox.Text = songNameTextBlock.Text.Split('.').First();
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

        private void DeleteArtistButton_Click(object sender, RoutedEventArgs e) {
            artistsList.RemoveAt(artistsListBox.SelectedIndex);
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e) {
            if (!selectedSong.Equals("") &&!songNameTextBox.Text.Equals("") && !genreComboBox.Text.Equals("") && artistsList.Count > 0) {
                createAlbumPage.SongsList.Add(new Song {
                    GenreId = (genreComboBox.SelectedItem as Genre).GenreId,
                    Title = songNameTextBox.Text,
                    SongLocation = selectedSong,
                    Artists = artistsList.ToList()
                });
                Close();
            } else {
                MessageBox.Show("Faltan campos por completar.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            Close();
        }
    }
}
