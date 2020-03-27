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
                lastNameTextBox.Text, 
                secondLastNameTextBox.Text
            );
            account.Register((jsonResponse) => {
                MessageBox.Show("Cuenta registrada.");
                Close();
            }, (jsonResponse) => {
                MessageBox.Show("Error al registrar la cuenta.");
            });
        }
    }
}
