using MaterialDesignThemes.Wpf;
using Musify.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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

namespace Musify {
    /// <summary>
    /// Lógica de interacción para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window {

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public LoginWindow() {
            InitializeComponent();
        }

        /// <summary>
        /// Verifies if fields are filled.
        /// </summary>
        /// <returns>true if fields are filled; false if not</returns>
        private bool ValidateFields() {
            return !string.IsNullOrWhiteSpace(emailTextBox.Text) &&
                !string.IsNullOrWhiteSpace(passwordPasswordBox.Password);
        }

        /// <summary>
        /// Verifies if fields data are valid.
        /// </summary>
        /// <returns>true if are valid; false if not</returns>
        private bool ValidateFieldsData() {
            return Regex.IsMatch(emailTextBox.Text, Core.REGEX_EMAIL);
        }

        /// <summary>
        /// Attempts to log in with given data.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            if (!ValidateFields()) {
                MessageBox.Show("Faltan campos por completar.");
                return;
            } else if (!ValidateFieldsData()) {
                MessageBox.Show("Debes introducir datos válidos.");
                return;
            }
            try {
                DialogHost.Show(mainStackPanel, "LoginWindow_WindowDialogHost", (openSender, openEventArgs) => {
                    Account.Login(emailTextBox.Text, passwordPasswordBox.Password, (account) => {
                        Session.Account = account;
                        Session.Account.FetchSubscription((subscription) => {
                            Session.Account.Subscription = subscription;
                        }, null, null);
                        Session.Account.FetchArtist(() => {
                            openEventArgs.Session.Close(true);
                            new MainWindow().Show();
                            Close();
                        }, null);
                    }, () => {
                        openEventArgs.Session.Close(true);
                        MessageBox.Show("Error al iniciar sesión.");
                    });
                }, null);
            } catch (Exception) {
                MessageBox.Show("Error al iniciar sesión.");
            }
        }

        /// <summary>
        /// Shows up a new register window.
        /// </summary>
        /// <param name="sender">Button</param>
        /// <param name="e">Event</param>
        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            new RegisterWindow().Show();
        }
    }
}
