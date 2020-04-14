using Musify.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static Musify.Models.AccountSong;
using static Musify.Models.Song;

namespace Musify {
    class UIFunctions {
        public static void SongTable_OnDoubleClick(object sender, MouseButtonEventArgs e) {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element != null && element is FrameworkElement) {
                if (((FrameworkElement) element).Parent is DataGridCell) {
                    var grid = sender as DataGrid;
                    if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1) {
                        var songRow = (SongTable) grid.SelectedItem;
                        Song song = songRow.Song;
                        Session.PlayerPage.PlaySong(song);
                    }
                }
            }
        }

        public static void AccountSongTable_OnDoubleClick(object sender, MouseButtonEventArgs e) {
            IInputElement element = e.MouseDevice.DirectlyOver;
            if (element != null && element is FrameworkElement) {
                if (((FrameworkElement) element).Parent is DataGridCell) {
                    var grid = sender as DataGrid;
                    if (grid != null && grid.SelectedItems != null && grid.SelectedItems.Count == 1) {
                        var accountSongRow = (AccountSongTable) grid.SelectedItem;
                        AccountSong accountSong = accountSongRow.AccountSong;
                        Session.PlayerPage.PlayAccountSong(accountSong);
                    }
                }
            }
        }
    }
}
