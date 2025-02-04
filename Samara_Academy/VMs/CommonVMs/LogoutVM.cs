using Samara_Academy.Models;
using Samara_Academy.Utilities.Helpers;
using Samara_Academy.VMs.ClassVMs;
using Samara_Academy.VMs.LogVMs;
using Samara_Academy.VMs.ProfileVMs;
using Samara_Academy.VMs.StudentVMs;
using Samara_Academy.VMs.TeacherVMs;
using Samara_Academy.VMs.UserVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Samara_Academy.VMs.CommonVMs
{
    public class LogoutVM:VMBase
    {
       
        public LogoutVM()
        {


            Exit();
            

        }

        private async void Exit()
        {

            SharedResources.UserID = "";
            SharedResources.LoggedUser = new User();
            LoginWindow window = new LoginWindow();

            window.Show();

            Application.Current.Windows[0]?.Close();
        }
    }
}
