using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samara_Academy.Models
{
    public class Class
    {
        public string ClassID { get; set; }
        public string Name { get; set; }
        public float Fee { get; set; }
        public float Margin { get; set; }
        public string Time { get; set; }
        public string Day { get; set; }
        public string RegisteredDate { get; set; }
        public string TeacherID { get; set; }
        public string InsertedBy { get; set; }
        public string LastModifiedDate { get; set; }
        public string TeacherName { get; set; }


        public Class()
        {

        }
    }
}
