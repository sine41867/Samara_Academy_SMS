using Samara_Academy.VMs.CommonVMs;
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

namespace Samara_Academy.Views.CommonViews
{
    /// <summary>
    /// Interaction logic for EnrollStudentsView.xaml
    /// </summary>
    public partial class EnrollStudentsView : Window
    {
        public EnrollStudentsView(string studentID = null)
        {
            InitializeComponent();
            DataContext = new EnrollAStudentVM(studentID);
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
