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
using static Musify.Models.Song;

namespace Musify.Pages {
    /// <summary>
    /// Lógica de interacción para ConsultArtistPage.xaml
    /// </summary>
    public partial class ConsultArtistPage : Page {
        public ConsultArtistPage(Artist artist) {
            InitializeComponent();
            LoadArtist(artist);
            Session.MainWindow.TitleBar.Text = artist.ArtisticName;
        }

        private void LoadArtist(Artist artist) {
            artist.FetchAlbums(() => {
                if (artist.Albums == null) {
                    MessageBox.Show("Ocurrió un error al cargar el artista.");
                    return;
                }
                foreach (Album album in artist.Albums) {
                    album.FetchArtists(() => {
                        album.FetchSongs(() => {
                            CreateAlbumUI(album);
                        }, () => {
                            MessageBox.Show("Ocurrió un error al cargar el artista.");
                        });
                    }, () => {
                        MessageBox.Show("Ocurrió un error al cargar el artista.");
                    });
                }
            }, () => { });
        }

        private void CreateAlbumUI(Album album) {
            ObservableCollection<SongTable> albumSongsList = new ObservableCollection<SongTable>();
            foreach (Song albumSong in album.Songs) {
                albumSongsList.Add(new SongTable {
                    Album = album,
                    Song = albumSong,
                    Title = albumSong.Title,
                    Duration = albumSong.Duration,
                    ArtistsNames = albumSong.GetArtistsNames()
                });
            }
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
            };
            Dictionary<string, string> columns = new Dictionary<string, string>() {
                { "Title", "Title" }, { "Duration", "Duración" }
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
