using MaterialDesignThemes.Wpf;
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

        private DialogOpenedEventArgs dialogOpenEventArgs;
        private List<DataGrid> albumSongsDataGrids = new List<DataGrid>();
        private Song selectedSong;

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
            DataGrid albumSongsDataGrid = new DataGrid {
                Margin = new Thickness(34, 0, 34, 0),
                ItemsSource = albumSongsList,
                CanUserSortColumns = true,
                CanUserAddRows = false,
                CanUserReorderColumns = false,
                AutoGenerateColumns = false,
                IsReadOnly = true,
                SelectionMode = DataGridSelectionMode.Single
            };
            albumSongsDataGrid.MouseDoubleClick += (sender, e) => {
                UIFunctions.SongTable_OnDoubleClick(sender, e);
                Session.HistoryIndex = Session.SongsIdPlayHistory.Count - 1;
                Session.SongsIdSongList.Clear();
                for (int i = albumSongsDataGrid.SelectedIndex + 1; i < albumSongsList.Count; i++) {
                    Session.SongsIdSongList.Add(albumSongsList.ElementAt(i).Song.SongId);
                }
            };
            albumSongsDataGrid.SelectionChanged += (sender, e) => {
                foreach (var dataGrid in albumSongsDataGrids) {
                    if (dataGrid == sender) {
                        continue;
                    }
                    dataGrid.SelectedIndex = -1;
                }
                if (e.AddedItems.Count > 0) {
                    selectedSong = ((SongTable) e.AddedItems[0]).Song;
                }
            };
            Dictionary<string, string> columns = new Dictionary<string, string>() {
                { "Title", "Title" }, { "ArtistsNames", "Artista" }, { "Genre", "Género" }, { "Duration", "Duración" }
            };
            foreach (var column in columns) {
                albumSongsDataGrid.Columns.Add(new System.Windows.Controls.DataGridTextColumn {
                    Binding = new Binding(column.Key),
                    Header = column.Value
                });
            }
            albumSongsDataGrids.Add(albumSongsDataGrid);
            mainStackPanel.Children.Add(albumSongsDataGrid);
            albumsStackPanel.Children.Add(mainStackPanel);
        }

        /// <summary>
        /// Opens up a dialog to add to play queue.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
        private void AddToQueueButton_Click(object sender, RoutedEventArgs e) {
            DialogHost.Show(mainStackPanel, "ConsultAlbumPage_WindowDialogHost", (openSender, openEventArgs) => {
                dialogOpenEventArgs = openEventArgs;
                dialogAddToQueueGrid.Visibility = Visibility.Visible;
            }, null);
        }

        /// <summary>
        /// Adds the selected song to the beginning of the queue.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AddToBelowButton_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.Insert(0, selectedSong.SongId);
            //songsDataGrid.SelectedIndex = -1;
            dialogOpenEventArgs.Session.Close(true);
            dialogAddToQueueGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Adds the selected song to the end of the queue.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AddToTheEndButton_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.Add(selectedSong.SongId);
            //songsDataGrid.SelectedIndex = -1;
            dialogOpenEventArgs.Session.Close(true);
            dialogAddToQueueGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Shows up a new window to add the selected song to a playlist.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
        private void AddToPlaylistButton_Click(object sender, RoutedEventArgs e) {
            new AddSongToPlaylistWindow(selectedSong).Show();
            //songsDataGrid.SelectedIndex = -1;
        }

        /// <summary>
        /// Generates a radio station with the selected song genre.
        /// </summary>
        /// <param name="sender">MenuItem</param>
        /// <param name="e">Event</param>
        private void GenerateRadioStationButton_Click(object sender, RoutedEventArgs e) {
            if (Session.GenresIdRadioStations.Find(x => x == selectedSong.Genre.GenreId) == 0) {
                Session.GenresIdRadioStations.Add(selectedSong.Genre.GenreId);
            } else {
                MessageBox.Show("Ya existe la estación de radio de este género.");
            }
            //songsDataGrid.SelectedIndex = -1;
        }
    }
}
