using Musify.Models;
using System.Text.RegularExpressions;
using System.Windows;

namespace Musify {
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public RegisterWindow() {
            InitializeComponent();
        }

        /// <summary>
        /// Verifies if fields are filled.
        /// </summary>
        /// <returns>true if fields are filled; false if not</returns>
        private bool ValidateFields() {
            return !string.IsNullOrWhiteSpace(emailTextBox.Text) &&
                !string.IsNullOrWhiteSpace(passwordPasswordBox.Password) &&
                !string.IsNullOrWhiteSpace(nameTextBox.Text) &&
                !string.IsNullOrWhiteSpace(lastNameTextBox.Text) &&
                imAnArtistCheckBox.IsChecked.GetValueOrDefault() ?
                    !string.IsNullOrWhiteSpace(artisticNameTextBox.Text) : true;
        }

        /// <summary>
        /// Verifies if fields data are valid.
        /// </summary>
        /// <returns>true if fields data are valid; false if not</returns>
        private bool ValidateFieldsData() {
            return Regex.IsMatch(emailTextBox.Text, Core.REGEX_EMAIL) &&
                Regex.IsMatch(nameTextBox.Text, Core.REGEX_ONLY_LETTERS) &&
                Regex.IsMatch(lastNameTextBox.Text, Core.REGEX_ONLY_LETTERS);
        }

        /// <summary>
        /// Attempts to register a new account.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            if (!ValidateFields()) {
                MessageBox.Show("Faltan campos por completar.");
                return;
            } else if (!ValidateFieldsData()) {
                MessageBox.Show("Debes introducir datos válidos.");
                return;
            }
            Account account = new Account(
                emailTextBox.Text, 
                passwordPasswordBox.Password, 
                nameTextBox.Text, 
                lastNameTextBox.Text
            );
            account.Register(imAnArtistCheckBox.IsChecked.GetValueOrDefault(), () => {
                MessageBox.Show("Cuenta registrada.");
                Close();
            }, (errorResponse) => {
                MessageBox.Show(errorResponse.Message);
            }, () => {
                MessageBox.Show("Ocurrió un error al momento de registrar la cuenta.");
            }, artisticNameTextBox.Text);
        }

        /// <summary>
        /// Enables artistic name textbox.
        /// </summary>
        /// <param name="sender">CheckBox</param>
        /// <param name="e">Event</param>
        private void ImAnArtistCheckBox_Checked(object sender, RoutedEventArgs e) {
            artisticNameTextBox.IsEnabled = true;
        }

        /// <summary>
        /// Disables artistic name textbox.
        /// </summary>
        /// <param name="sender">CheckBox</param>
        /// <param name="e">Event</param>
        private void ImAnArtistCheckBox_Unchecked(object sender, RoutedEventArgs e) {
            artisticNameTextBox.IsEnabled = false;
        }
    }
}
