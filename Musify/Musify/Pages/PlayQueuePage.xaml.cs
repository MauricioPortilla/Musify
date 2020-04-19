﻿using MaterialDesignThemes.Wpf;
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
    /// Lógica de interacción para PlayQueuePage.xaml
    /// </summary>
    public partial class PlayQueuePage : Page {
        private readonly ObservableCollection<SongTable> songsPlayQueue = new ObservableCollection<SongTable>();
        public ObservableCollection<SongTable> SongsPlayQueue {
            get => songsPlayQueue;
        }

        public PlayQueuePage() {
            InitializeComponent();
            DataContext = this;
            LoadPlayQueue();
        }

        private void LoadPlayQueue() {
            songsPlayQueue.Clear();
            List<int> songsIdPlayQueue = Session.SongsIdPlayQueue;
            if (songsIdPlayQueue.Count > 0) {
                deletePlayQueueButton.Visibility = Visibility.Visible;
            }
            for (int i = 0; i < songsIdPlayQueue.Count; i++) {
                Song.FetchById(songsIdPlayQueue.ElementAt(i), (song) => {
                    songsPlayQueue.Add(new SongTable {
                        Song = song,
                        Title = song.Title,
                        ArtistsNames = song.Album.GetArtistsNames(),
                        Album = song.Album,
                        Duration = song.Duration
                    });
                }, () => {
                    MessageBox.Show("Ocurrió un error al cargar las canciones.");
                });
            }
        }

        private void PlayQueueDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            UIFunctions.SongTable_OnDoubleClick(sender, e);
        }

        private void PlayQueueDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (playQueueDataGrid.SelectedIndex >= 0) {
                optionsMenu.Visibility = Visibility.Visible;
            } else {
                optionsMenu.Visibility = Visibility.Hidden;
            }
        }

        private void DeletePlayQueueButton_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue = new List<int>();
            LoadPlayQueue();
        }

        private void AddToQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.Add(((SongTable)playQueueDataGrid.SelectedItem).Song.SongId);
            LoadPlayQueue();
        }

        private void AddToPlaylistMenuItem_Click(object sender, RoutedEventArgs e) {

        }

        private void GenerateRadioStationMenuItem_Click(object sender, RoutedEventArgs e) {

        }

        private void RemoveFromQueueMenuItem_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.RemoveAt(playQueueDataGrid.SelectedIndex);
            LoadPlayQueue();
        }
    }
}
