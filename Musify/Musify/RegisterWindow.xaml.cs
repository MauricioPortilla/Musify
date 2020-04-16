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
using System.Windows.Shapes;

namespace Musify {
    /// <summary>
    /// Lógica de interacción para RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window {
        public RegisterWindow() {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e) {
            Account account = new Account(
                emailTextBox.Text, 
                passwordPasswordBox.Password, 
                nameTextBox.Text, 
                lastNameTextBox.Text
            );
            account.Register(imAnArtistCheckBox.IsChecked.GetValueOrDefault(), () => {
                MessageBox.Show("Cuenta registrada.");
                Close();
            }, () => {
                MessageBox.Show("Error al registrar la cuenta.");
            }, artisticNameTextBox.Text);
        }

        private void ImAnArtistCheckBox_Checked(object sender, RoutedEventArgs e) {
            artisticNameTextBox.IsEnabled = true;
        }

        private void ImAnArtistCheckBox_Unchecked(object sender, RoutedEventArgs e) {
            artisticNameTextBox.IsEnabled = false;
        }
    }
}
