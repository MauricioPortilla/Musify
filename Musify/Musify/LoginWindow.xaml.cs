using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

namespace Musify {
    /// <summary>
    /// Lógica de interacción para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window {
        public LoginWindow() {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            Account.Login(emailTextBox.Text, passwordPasswordBox.Password, (account) => {
                Session.Account = account;
                MessageBox.Show("OK");
            }, () => {
                MessageBox.Show("Error al iniciar sesión.");
            });
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            new RegisterWindow().Show();
        }
    }
}
