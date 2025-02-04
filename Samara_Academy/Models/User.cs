using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samara_Academy.Models
{
    public class User
    {
        public string UserID { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string UserRole { get; set; }
        public bool IsDisable { get; set; }
        public string DateOfRegistration { get; set; }
        public string InsertedBy { get; set; }
        public User()
        {
            
        }
    }
}
