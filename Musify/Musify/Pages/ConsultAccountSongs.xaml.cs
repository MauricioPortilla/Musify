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
using static Musify.Models.AccountSong;

namespace Musify.Pages {
    /// <summary>
    /// Lógica de interacción para ConsultAccountSongs.xaml
    /// </summary>
    public partial class ConsultAccountSongs : Page {

        private readonly ObservableCollection<AccountSongTable> accountSongList = new ObservableCollection<AccountSongTable>();
        public ObservableCollection<AccountSongTable> AccountSongList {
            get => accountSongList;
        }

        public ConsultAccountSongs() {
            InitializeComponent();
            DataContext = this;
            Session.Account.FetchAccountSongs(() => {
                foreach (AccountSong accountSong in Session.Account.AccountSongs) {
                    AccountSongList.Add(new AccountSongTable {
                        AccountSong = accountSong,
                        Title = accountSong.Title,
                        Duration = "00:00"
                    });
                }
            }, () => {
                MessageBox.Show("Ocurrió un error al cargar las canciones.");
            });
        }

        private void AccountSongsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            UIFunctions.AccountSongTable_OnDoubleClick(sender, e);
        }

        private void AddSongButton_Click(object sender, RoutedEventArgs e) {

        }

        private void DeleteSongButton_Click(object sender, RoutedEventArgs e) {

        }
    }
}
