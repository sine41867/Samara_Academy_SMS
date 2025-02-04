using Samara_Academy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samara_Academy.Utilities.Helpers
{
    public static class SharedResources
    {
        public static string UserID { get; set; } = "";
        public static User LoggedUser { get; set; } = new User();

        public static string[] Months { get; } = { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

       

    }
}
