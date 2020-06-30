using Musify.Models;
using System.Windows;
using System.Windows.Controls;

namespace Musify.Pages {
    /// <summary>
    /// Interaction logic for RadioStationsPage.xaml
    /// </summary>
    public partial class RadioStationsPage : Page {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RadioStationsPage() {
            InitializeComponent();
            LoadGenres();
        }

        /// <summary>
        /// Loads the genres from which a radio station has been created.
        /// </summary>
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

        /// <summary>
        /// Shows up a consult radio station page with the selected genre.
        /// </summary>
        /// <param name="sender">ListBox</param>
        /// <param name="e">Event</param>
        private void GenresListBox_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ConsultRadioStationPage consultRadioStationPage = new ConsultRadioStationPage(genresListBox.SelectedItem as Genre);
            Session.MainFrame.Navigate(consultRadioStationPage);
        }
    }
}
