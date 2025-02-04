using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samara_Academy.Models
{
    public class Student
    {
        public string StudentID { get; set; }
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string WhatsApp { get; set; }
        public string RegisteredDate { get; set; }
        public string LastModifiedDate { get; set; }

        public string InsertedBy { get; set; }

        public Student()
        {

        }

    }
}
