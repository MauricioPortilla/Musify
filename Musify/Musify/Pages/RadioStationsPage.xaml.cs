using Musify.Models;
using System.Windows;
using System.Windows.Controls;

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
                }, (errorResponse) => {
                    MessageBox.Show(errorResponse.Message);
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
