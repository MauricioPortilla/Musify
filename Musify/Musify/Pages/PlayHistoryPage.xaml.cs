﻿using Musify.Models;
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
using static Musify.Models.AccountSong;
using static Musify.Models.Song;

namespace Musify.Pages {
    /// <summary>
    /// Lógica de interacción para PlayHistoryPage.xaml
    /// </summary>
    public partial class PlayHistoryPage : Page {
        private readonly ObservableCollection<object> songsPlayHistory = new ObservableCollection<object>();
        public ObservableCollection<object> SongsPlayHistory {
            get => songsPlayHistory;
        }

        public PlayHistoryPage() {
            InitializeComponent();
            DataContext = this;
            LoadPlayHistory();
        }

        public void LoadPlayHistory() {
            songsPlayHistory.Clear();
            List<int> songsIdPlayHistory = Session.SongsIdPlayHistory;
            int limit = 0;
            if (songsIdPlayHistory.Count > Core.MAX_SONGS_IN_HISTORY) {
                limit = songsIdPlayHistory.Count - Core.MAX_SONGS_IN_HISTORY;
            }
            LoadSong(songsIdPlayHistory.Count - 1, limit, songsIdPlayHistory);
        }

        public void LoadSong(int i, int limit, List<int> songsIdPlayHistory) {
            if (i >= limit) {
                if (songsIdPlayHistory.ElementAt(i) > 0) {
                    Song.FetchById(songsIdPlayHistory.ElementAt(i), (song) => {
                        SongsPlayHistory.Add(new SongTable {
                            Song = song,
                            Title = song.Title,
                            ArtistsNames = song.GetArtistsNames(),
                            Album = song.Album,
                            Genre = song.Genre,
                            Duration = song.Duration
                        });
                        LoadSong(i - 1, limit, songsIdPlayHistory);
                    }, () => {
                        MessageBox.Show("Ocurrió un error al cargar las canciones.");
                    });
                } else {
                    AccountSong.FetchById(songsIdPlayHistory.ElementAt(i) * -1, (accountSong) => {
                        SongsPlayHistory.Add(new AccountSongTable {
                            AccountSong = accountSong,
                            Title = accountSong.Title,
                            Duration = accountSong.Duration
                        });
                        LoadSong(i - 1, limit, songsIdPlayHistory);
                    }, () => {
                        MessageBox.Show("Ocurrió un error al cargar las canciones.");
                    });
                }
            }
        }

        private void PlayHistoryDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            if (playHistoryDataGrid.SelectedItem is SongTable) {
                UIFunctions.SongTable_OnDoubleClick(sender, e);
            } else {
                UIFunctions.AccountSongTable_OnDoubleClick(sender, e);
            }
            Session.SongsIdSongList.Clear();
            LoadPlayHistory();
        }

        private void PlayHistoryDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (playHistoryDataGrid.SelectedIndex >= 0) {
                optionsMenu.Visibility = Visibility.Visible;
                if (playHistoryDataGrid.SelectedItem is SongTable) {
                    addToPlaylistMenuItem.Visibility = Visibility.Visible;
                    generateRadioStationMenuItem.Visibility = Visibility.Visible;
                } else {
                    addToPlaylistMenuItem.Visibility = Visibility.Collapsed;
                    generateRadioStationMenuItem.Visibility = Visibility.Collapsed;
                }
            } else {
                optionsMenu.Visibility = Visibility.Hidden;
                addToPlaylistMenuItem.Visibility = Visibility.Collapsed;
                generateRadioStationMenuItem.Visibility = Visibility.Collapsed;
            }
        }

        private void AddToQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.Add(((SongTable)playHistoryDataGrid.SelectedItem).Song.SongId);
            playHistoryDataGrid.SelectedIndex = -1;
        }

        private void AddToPlaylistMenuItem_Click(object sender, RoutedEventArgs e) {
            new AddSongToPlaylistWindow(((SongTable)playHistoryDataGrid.SelectedItem).Song).Show();
            playHistoryDataGrid.SelectedIndex = -1;
        }

        private void GenerateRadioStationMenuItem_Click(object sender, RoutedEventArgs e) {
            if (Session.GenresIdRadioStations.Find(x => x == ((SongTable)playHistoryDataGrid.SelectedItem).Song.Genre.GenreId) == 0) {
                Session.GenresIdRadioStations.Add(((SongTable)playHistoryDataGrid.SelectedItem).Song.Genre.GenreId);
            } else {
                MessageBox.Show("Ya existe la estación de radio de este género.");
            }
            playHistoryDataGrid.SelectedIndex = -1;
        }
    }
}
