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
using static Musify.Models.Album;
using static Musify.Models.Artist;
using static Musify.Models.Song;

namespace Musify.Pages {
    /// <summary>
    /// Lógica de interacción para SearchPage.xaml
    /// </summary>
    public partial class SearchPage : Page {

        private readonly ObservableCollection<SongTable> songList = new ObservableCollection<SongTable>();
        public ObservableCollection<SongTable> SongList {
            get => songList;
        }
        private readonly ObservableCollection<AlbumTable> albumList = new ObservableCollection<AlbumTable>();
        public ObservableCollection<AlbumTable> AlbumList {
            get => albumList;
        }
        private readonly ObservableCollection<ArtistTable> artistList = new ObservableCollection<ArtistTable>();
        public ObservableCollection<ArtistTable> ArtistList {
            get => artistList;
        }

        public SearchPage() {
            InitializeComponent();
            DataContext = this;
        }

        private void SearchTextBox_KeyUp(object sender, KeyEventArgs e) {
            if (string.IsNullOrWhiteSpace(searchTextBox.Text)) {
                songList.Clear();
                albumList.Clear();
                artistList.Clear();
                return;
            } else if (searchTextBox.Text.Length < 3) {
                return;
            }
            if (searchTabControl.SelectedIndex == 0) {
                Song.FetchByTitleCoincidences(searchTextBox.Text, (songs) => {
                    songList.Clear();
                    foreach (Song song in songs) {
                        songList.Add(new SongTable {
                            Song = song,
                            Title = song.Title,
                            ArtistsNames = song.Album.GetArtistsNames(),
                            Album = song.Album,
                            Duration = song.Duration
                        });
                    }
                }, () => {
                    MessageBox.Show("Ocurrió un error al cargar las canciones.");
                });
            } else if (searchTabControl.SelectedIndex == 1) {
                Album.FetchByNameCoincidences(searchTextBox.Text, (albums) => {
                    albumList.Clear();
                    foreach (Album album in albums) {
                        albumList.Add(new AlbumTable {
                            Album = album,
                            Type = album.Type,
                            Name = album.Name,
                            Artist = album.GetArtistsNames(),
                            LaunchYear = album.LaunchYear
                        });
                    }
                }, () => {
                    MessageBox.Show("Ocurrió un error al cargar los álbumes.");
                });
            } else if (searchTabControl.SelectedIndex == 2) {
                Artist.FetchByArtisticNameCoincidences(searchTextBox.Text, (artists) => {
                    artistList.Clear();
                    foreach (Artist artist in artists) {
                        artistList.Add(new ArtistTable {
                            Artist = artist,
                            ArtisticName = artist.ArtisticName
                        });
                    }
                }, () => {
                    MessageBox.Show("Ocurrió un error al cargar los artistas.");
                });
            }
        }

        private void SongsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            UIFunctions.SongTable_OnDoubleClick(sender, e);
        }

        private void AddToQueueButton_Click(object sender, RoutedEventArgs e) {

        }

        private void AddToPlaylistButton_Click(object sender, RoutedEventArgs e) {
            if (songsDataGrid.SelectedItem != null) {
                new AddSongToPlaylistWindow(((SongTable) songsDataGrid.SelectedItem).Song).Show();
            }
        }

        private void GenerateRadioStation_Click(object sender, RoutedEventArgs e) {

        }

        private void AlbumsListView_SelectionChanged(object sender, RoutedEventArgs e) {
            if (albumsListView.SelectedIndex == -1) {
                return;
            }
            var selectedAlbum = (AlbumTable) albumsListView.SelectedItem;
            // TODO: Go to ConsultAlbumPage
            albumsListView.SelectedIndex = -1;
        }

        private void ArtistsListView_SelectionChanged(object sender, RoutedEventArgs e) {
            if (artistsListView.SelectedIndex == -1) {
                return;
            }
            var selectedArtist = (ArtistTable) artistsListView.SelectedItem;
            Session.MainWindow.mainFrame.Navigate(new ConsultArtistPage(selectedArtist.Artist));
            artistsListView.SelectedIndex = -1;
        }
    }
}
