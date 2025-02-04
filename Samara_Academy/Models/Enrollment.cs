using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samara_Academy.Models
{
    public class Enrollment
    {
        public string StudentID { get; set; }
        public string ClassID { get; set; }
        public string EnrolledDate { get; set; }
        public string InsertedBy { get; set; }
        public string ClassName { get; set; }
        public Enrollment()
        {

        }
    }
}