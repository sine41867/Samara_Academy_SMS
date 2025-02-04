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

namespace Samara_Academy
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {

            if (txtPassword.Visibility == Visibility.Visible)
            {
                txtVisiblePass.Text = txtPassword.Password;
                
                if (DataContext is VMs.CommonVMs.LoginVM viewModel)
                {
                    viewModel.Password = ((PasswordBox)sender).Password;
                }
                
            }

        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void TogglePasswordVisibility(object sender, RoutedEventArgs e)
        {
            if (txtPassword.Visibility == Visibility.Visible)
            {
                // Show password in TextBox
                txtVisiblePass.Text = txtPassword.Password;
                txtPassword.Visibility = Visibility.Collapsed;
                txtVisiblePass.Visibility = Visibility.Visible;
                showIcon.Visibility = Visibility.Collapsed;
                hideIcon.Visibility = Visibility.Visible;
            }
            else
            {
                // Show password in PasswordBox
                txtPassword.Password = txtVisiblePass.Text;
                txtPassword.Visibility = Visibility.Visible;
                txtVisiblePass.Visibility = Visibility.Collapsed;
                hideIcon.Visibility = Visibility.Collapsed;
                showIcon.Visibility = Visibility.Visible;

            }
        }


        private void txtVisiblePass_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtVisiblePass.Visibility == Visibility.Visible)
            {
                txtPassword.Password = txtVisiblePass.Text;

            }
        }
    }
}
