using Musify.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Musify.Models.AccountSong;
using static Musify.Models.Song;

namespace Musify {
    static class UIFunctions {
        /// <summary>
        /// Plays the double clicked song.
        /// </summary>
        /// <param name="sender">DataGridCell</param>
        /// <param name="e">Event</param>
        public static void SongTable_OnDoubleClick(object sender, MouseButtonEventArgs e) {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element is FrameworkElement && ((FrameworkElement) element).Parent is DataGridCell) {
                var grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1) {
                    var songRow = (SongTable) grid.SelectedItem;
                    Song song = songRow.Song;
                    Session.PlayerPage.PlaySong(song, false);
                }
            }
        }

        /// <summary>
        /// Plays the double clicked account song.
        /// </summary>
        /// <param name="sender">DataGridCell</param>
        /// <param name="e">Event</param>
        public static void AccountSongTable_OnDoubleClick(object sender, MouseButtonEventArgs e) {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element is FrameworkElement && ((FrameworkElement) element).Parent is DataGridCell) {
                var grid = sender as DataGrid;
                if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1) {
                    var accountSongRow = (AccountSongTable) grid.SelectedItem;
                    AccountSong accountSong = accountSongRow.AccountSong;
                    Session.PlayerPage.PlayAccountSong(accountSong, false);
                }
            }
        }
    }
}
