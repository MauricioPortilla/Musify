using Musify.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static Musify.Models.Song;

namespace Musify.Pages {
    /// <summary>
    /// Interaction logic for ConsultArtistPage.xaml
    /// </summary>
    public partial class ConsultArtistPage : Page {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="artist">Artist to consult</param>
        public ConsultArtistPage(Artist artist) {
            InitializeComponent();
            LoadArtist(artist);
            Session.MainWindow.titleBarTextBlock.Text = artist.ArtisticName;
        }

        /// <summary>
        /// Loads the artist albums and songs.
        /// </summary>
        /// <param name="artist">Artist to load</param>
        private void LoadArtist(Artist artist) {
            artist.FetchAlbums(() => {
                if (artist.Albums == null) {
                    MessageBox.Show("Ocurrió un error al cargar el artista.");
                    return;
                }
                foreach (Album album in artist.Albums) {
                    album.FetchArtists(() => {
                        CreateAlbumUI(album);
                    }, (errorResponse) => {
                        MessageBox.Show(errorResponse.Message);
                    }, () => {
                        MessageBox.Show("Ocurrió un error al cargar el artista.");
                    });
                }
            }, (errorResponse) => {
                MessageBox.Show(errorResponse.Message);
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar el artista.");
            });
        }

        /// <summary>
        /// Creates the UI for an album.
        /// </summary>
        /// <param name="album">Album</param>
        private void CreateAlbumUI(Album album) {
            ObservableCollection<SongTable> albumSongsList = new ObservableCollection<SongTable>();
            album.FetchSongs(() => {
                albumSongsList.Clear();
                foreach (Song albumSong in album.Songs) {
                    albumSongsList.Add(new SongTable {
                        Album = album,
                        Song = albumSong,
                        Title = albumSong.Title,
                        Genre = albumSong.Genre,
                        Duration = albumSong.Duration,
                        ArtistsNames = albumSong.GetArtistsNames()
                    });
                }
            }, (errorResponse) => {
                MessageBox.Show(errorResponse.Message);
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar el artista.");
            });
            StackPanel mainStackPanel = new StackPanel();
            mainStackPanel.Orientation = Orientation.Vertical;
            StackPanel albumHeaderStackPanel = new StackPanel();
            albumHeaderStackPanel.Orientation = Orientation.Horizontal;
            Image albumImage = new Image();
            albumImage.Source = album.FetchImage();
            albumImage.Margin = new Thickness(35, 30, 0, 0);
            albumImage.Width = 110;
            albumImage.Height = 110;
            albumHeaderStackPanel.Children.Add(albumImage);
            StackPanel albumDataStackPanel = new StackPanel();
            albumDataStackPanel.Margin = new Thickness(12, 35, 0, 0);
            albumDataStackPanel.Orientation = Orientation.Vertical;
            TextBlock albumTypeTextBlock = new TextBlock();
            albumTypeTextBlock.Text = album.Type;
            albumTypeTextBlock.FontSize = 10;
            albumTypeTextBlock.FontWeight = FontWeights.Bold;
            albumDataStackPanel.Children.Add(albumTypeTextBlock);
            TextBlock albumNameTextBlock = new TextBlock();
            albumNameTextBlock.Text = album.Name;
            albumNameTextBlock.FontSize = 20;
            albumNameTextBlock.FontWeight = FontWeights.Bold;
            albumDataStackPanel.Children.Add(albumNameTextBlock);
            TextBlock albumLaunchYearTextBlock = new TextBlock();
            albumLaunchYearTextBlock.Text = album.LaunchYear.ToString();
            albumLaunchYearTextBlock.FontSize = 12;
            albumDataStackPanel.Children.Add(albumLaunchYearTextBlock);
            TextBlock albumDiscographyTextBlock = new TextBlock();
            albumDiscographyTextBlock.Text = album.Discography;
            albumDiscographyTextBlock.FontSize = 11;
            albumDataStackPanel.Children.Add(albumDiscographyTextBlock);
            albumHeaderStackPanel.Children.Add(albumDataStackPanel);
            mainStackPanel.Children.Add(albumHeaderStackPanel);
            DataGrid albumSongsDataGrid = new DataGrid();
            albumSongsDataGrid.Margin = new Thickness(34, 0, 34, 0);
            albumSongsDataGrid.ItemsSource = albumSongsList;
            albumSongsDataGrid.CanUserSortColumns = true;
            albumSongsDataGrid.CanUserAddRows = false;
            albumSongsDataGrid.CanUserReorderColumns = false;
            albumSongsDataGrid.AutoGenerateColumns = false;
            albumSongsDataGrid.IsReadOnly = true;
            albumSongsDataGrid.MouseDoubleClick += (sender, e) => {
                UIFunctions.SongTable_OnDoubleClick(sender, e);
                Session.HistoryIndex = Session.SongsIdPlayHistory.Count - 1;
                Session.SongsIdSongList.Clear();
                for (int i = albumSongsDataGrid.SelectedIndex + 1; i < albumSongsList.Count; i++) {
                    Session.SongsIdSongList.Add(albumSongsList.ElementAt(i).Song.SongId);
                }
            };
            Dictionary<string, string> columns = new Dictionary<string, string>() {
                { "Title", "Title" }, { "ArtistsNames", "Artista" }, { "Genre", "Género" }, { "Duration", "Duración" }
            };
            foreach (var column in columns) {
                albumSongsDataGrid.Columns.Add(new DataGridTextColumn {
                    Binding = new Binding(column.Key),
                    Header = column.Value
                });
            }
            mainStackPanel.Children.Add(albumSongsDataGrid);
            albumsStackPanel.Children.Add(mainStackPanel);
        }
    }
}
