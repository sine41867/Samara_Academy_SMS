using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samara_Academy.Utilities.Helpers
{
    public class Resources
    {
        public Dictionary<string, int> MonthlyCount = new Dictionary<string, int>
        {
            { "01", 0 }, { "02", 0 }, { "03", 0 }, { "04", 0 }, { "05", 0 }, { "06", 0 },
            { "07", 0 }, { "08", 0 }, { "09", 0 }, { "10", 0 }, { "11", 0 }, { "12", 0 }
        };

        public static Dictionary<string, string> ClassStudentsHeadingKeyPairs = new Dictionary<string, string>
        {
            { "Student ID", "tbl_student.student_id" },
            { "Name", "name" },
            { "Mobile", "mobile" },
            { "WhatsApp", "whatsapp" },
            { "Enrolled Date", "registered_date" },
            { "Enrolled By", "tbl_enrollment.user_id" }
        };
        public static Dictionary<string, string> ClassHeadingKeyPairs = new Dictionary<string, string>
        {
            { "Class ID", "class_id" },
            { "Name", "name" },
            { "Fee", "fee" },
            { "Time", "time" },
            { "Day", "day" },
            { "Registered Date", "registered_date" }
        };
        public static Dictionary<string, string> TeacherHeadingKeyPairs = new Dictionary<string, string>
        {
            { "Teacher ID", "teacher_id" },
            { "Name", "name" },
            { "Mobile", "mobile" },
            { "WhatsApp", "whatsapp" },
            { "Registered Date", "registered_date" }
        };
        public static Dictionary<string, string> OwnLogHeadingKeyPairs = new Dictionary<string, string>
        {
            { "Log ID", "log_id" },
            { "Type", "type" },
            { "Time", "time" },
            { "Date", "date" },
            { "Description", "description" }
        };
        public static Dictionary<string, string> LogHeadingKeyPairs = new Dictionary<string, string>
        {
            { "Log ID", "log_id" },
            { "Type", "type" },
            { "Time", "time" },
            { "Date", "date" },
            { "User ID", "user_id" },
            { "Description", "description" }
        };
        public static Dictionary<string, string> StudentHeadingKeyPairs = new Dictionary<string, string>
        {
            { "Student ID", "student_id" },
            { "Name", "name" },
            { "Mobile", "mobile" },
            { "WhatsApp", "whatsapp" },
            { "Registered Date", "registered_date" }
        };
        public static Dictionary<string, string> UserHeadingKeyPairs = new Dictionary<string, string>
        {
            { "User ID", "user_id" },
            { "Name", "name" },
            { "Mobile", "mobile" },
            { "Inserted By", "inserted_by" },
            { "User Role", "user_role" },
            { "Registered Date", "registered_date" }
        };



        public static List<string> ClassStudentsHeadings = ClassStudentsHeadingKeyPairs.Keys.ToList();
        public static List<string> ClassHeadings = ClassHeadingKeyPairs.Keys.ToList();
        public static List<string> TeacherHeadings = TeacherHeadingKeyPairs.Keys.ToList();
        public static List<string> LogHeadings = LogHeadingKeyPairs.Keys.ToList();
        public static List<string> OwnLogHeadings = OwnLogHeadingKeyPairs.Keys.ToList();
        public static List<string> StudentHeadings = StudentHeadingKeyPairs.Keys.ToList();
        public static List<string> UserHeadings = UserHeadingKeyPairs.Keys.ToList();
        public Resources()
        {

        }

        
    }
}
