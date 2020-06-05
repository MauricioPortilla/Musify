using Musify.Models;
using System;
using System.Collections.Generic;
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

namespace Musify.Pages {
    /// <summary>
    /// Lógica de interacción para RadioStationsPage.xaml
    /// </summary>
    public partial class RadioStationsPage : Page {
        public RadioStationsPage() {
            InitializeComponent();
            LoadGenres();
        }

        private void LoadGenres() {
            genresListBox.Items.Clear();
            foreach (int genreId in Session.GenresIdRadioStations) {
                Genre.FetchById(genreId, (genre) => {
                    genresListBox.Items.Add(genre);
                }, () => {
                    MessageBox.Show("Ocurrió un error al cargar las estaciones de radio.");
                });
            }
        }

        private void GenresListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ConsultRadioStationPage consultRadioStationPage = new ConsultRadioStationPage(genresListBox.SelectedItem as Genre);
            Session.MainFrame.Navigate(consultRadioStationPage);
        }
    }
}
