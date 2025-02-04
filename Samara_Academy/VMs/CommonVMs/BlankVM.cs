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
using System.Windows.Input;

namespace Samara_Academy.VMs.CommonVMs
{
    public class BlankVM : VMBase
    {
        private readonly NavigationVM _navigationVM;
        private string _id;

        private string _targetVM;
        public ICommand ReloadCommand { get; }
        public BlankVM(NavigationVM navigationVM, string targetVM, string id = null)
        {
            _navigationVM = navigationVM;
            _targetVM = targetVM;
            _id = id;
            ReloadCommand = new RelayCommand(Reload);


        }

        private void Reload(object parameter)
        {

            switch (_targetVM)
            {
                case "DashboardVM":
                    _navigationVM.CurrentView = new DashboardVM(_navigationVM);
                    break;

                case "StudentVM":
                    _navigationVM.CurrentView = new StudentVM(_navigationVM);
                    break;

                case "ClassVM":
                    _navigationVM.CurrentView = new ClassVM(_navigationVM);
                    break;

                case "TeacherVM":
                    _navigationVM.CurrentView = new TeacherVM(_navigationVM);
                    break;
                case "LogVM":
                    _navigationVM.CurrentView = new LogVM(_navigationVM);
                    break;
                case "ClassStudentVM":
                    _navigationVM.CurrentView = new ClassStudentsVM(_navigationVM, _id);
                    break;

                case "UserVM":
                    _navigationVM.CurrentView = new UserVM(_navigationVM);
                    break;

                case "ProfileVM":
                    _navigationVM.CurrentView = new ProfileVM(_navigationVM);
                    break;




            }

        }
    }
}
