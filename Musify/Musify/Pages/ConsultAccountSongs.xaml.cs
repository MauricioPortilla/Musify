﻿using Musify.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Forms = System.Windows.Forms;
using System.Windows.Input;
using static Musify.Models.AccountSong;
using MaterialDesignThemes.Wpf;

namespace Musify.Pages {
    /// <summary>
    /// Lógica de interacción para ConsultAccountSongs.xaml
    /// </summary>
    public partial class ConsultAccountSongs : Page {

        private DialogOpenedEventArgs dialogOpenEventArgs;
        private readonly ObservableCollection<AccountSongTable> accountSongList = new ObservableCollection<AccountSongTable>();
        public ObservableCollection<AccountSongTable> AccountSongList {
            get => accountSongList;
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public ConsultAccountSongs() {
            InitializeComponent();
            DataContext = this;
            Session.Account.FetchAccountSongs(() => {
                SetAccountSongs();
            }, (errorResponse) => {
                MessageBox.Show(errorResponse.Message);
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar las canciones.");
            });
        }

        /// <summary>
        /// Refreshes the account songs table.
        /// </summary>
        private void SetAccountSongs() {
            accountSongList.Clear();
            foreach (AccountSong accountSong in Session.Account.AccountSongs) {
                AccountSongList.Add(new AccountSongTable {
                    AccountSong = accountSong,
                    Title = accountSong.Title,
                    Duration = accountSong.Duration
                });
            }
        }

        /// <summary>
        /// Attempts to play the double clicked account song.
        /// </summary>
        /// <param name="sender">DataGrid</param>
        /// <param name="e">Event</param>
        private void AccountSongsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            UIFunctions.AccountSongTable_OnDoubleClick(sender, e);
            Session.historyIndex = Session.SongsIdPlayHistory.Count - 1;
            Session.SongsIdSongList.Clear();
            for (int i = accountSongsDataGrid.SelectedIndex + 1; i < accountSongList.Count; i++) {
                Session.SongsIdSongList.Add(accountSongList.ElementAt(i).AccountSong.AccountSongId * -1);
            }
        }

        /// <summary>
        /// Shows up a dialog to add to play queue.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AddToQueueButton_Click(object sender, RoutedEventArgs e) {
            if (accountSongsDataGrid.SelectedItem == null) {
                MessageBox.Show("Debes seleccionar una canción de la lista.");
                return;
            }
            DialogHost.Show(mainStackPanel, "ConsultAccountSongs_WindowDialogHost", (openSender, openEventArgs) => {
                dialogOpenEventArgs = openEventArgs;
                dialogAddToQueueGrid.Visibility = Visibility.Visible;
            }, null);
        }

        /// <summary>
        /// Adds the selected account song to the beginning of the queue.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AddToBelowButton_Click(object sender, RoutedEventArgs e) {
            List<int> songsIdPlayQueue = new List<int> { ((AccountSongTable)accountSongsDataGrid.SelectedItem).AccountSong.AccountSongId * -1 };
            songsIdPlayQueue.AddRange(Session.SongsIdPlayQueue);
            Session.SongsIdPlayQueue = songsIdPlayQueue;
            accountSongsDataGrid.SelectedIndex = -1;
            dialogOpenEventArgs.Session.Close(true);
            dialogAddToQueueGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Adds the selected account song to the end of the queue.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AddToTheEndButton_Click(object sender, RoutedEventArgs e) {
            Session.SongsIdPlayQueue.Add(((AccountSongTable)accountSongsDataGrid.SelectedItem).AccountSong.AccountSongId * -1);
            accountSongsDataGrid.SelectedIndex = -1;
            dialogOpenEventArgs.Session.Close(true);
            dialogAddToQueueGrid.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Adds songs to the list.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void AddSongButton_Click(object sender, RoutedEventArgs e) {
            Forms.OpenFileDialog fileExplorer = new Forms.OpenFileDialog();
            fileExplorer.Filter = "MP3 Files|*.mp3|WAV Files|*.wav";
            fileExplorer.Multiselect = true;
            if (fileExplorer.ShowDialog() == Forms.DialogResult.OK) {
                var selectedFiles = fileExplorer.FileNames;
                if (Session.Account.AccountSongs.Count + selectedFiles.Length > Core.MAX_ACCOUNT_SONGS_PER_ACCOUNT) {
                    MessageBox.Show("Has superado el límite de 250 canciones.");
                    return;
                }
                try {
                    Session.Account.AddAccountSongs(selectedFiles, () => {
                        SetAccountSongs();
                        MessageBox.Show("Canciones cargadas con éxito.");
                    }, (errorResponse) => {
                        MessageBox.Show(errorResponse.Message);
                    }, () => {
                        MessageBox.Show("Ocurrió un error al cargar las canciones.");
                    });
                } catch (Exception) {
                    MessageBox.Show("Ocurrió un error al cargar las canciones.");
                }
            }
        }

        /// <summary>
        /// Deletes the selected account song.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void DeleteSongButton_Click(object sender, RoutedEventArgs e) {
            if (accountSongsDataGrid.SelectedItem == null) {
                MessageBox.Show("Debes seleccionar una canción de la lista.");
                return;
            }
            AccountSong accountSongSelected = ((AccountSongTable) accountSongsDataGrid.SelectedItem).AccountSong;
            Session.Account.DeleteAccountSong(accountSongSelected, () => {
                accountSongList.Remove((AccountSongTable) accountSongsDataGrid.SelectedItem);
                MessageBox.Show("Canción eliminada.");
            }, (errorResponse) => {
                MessageBox.Show(errorResponse.Message);
            }, () => {
                MessageBox.Show("Ocurrió un error al eliminar la canción.");
            });
        }
    }
}
