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

namespace Samara_Academy.Views.DialogBoxes
{
    /// <summary>
    /// Interaction logic for InfoDialog.xaml
    /// </summary>
    public partial class InfoDialog : Window
    {
        public InfoDialog(string message)
        {
            InitializeComponent();
            MessageText.Text = message;
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
    }
}
