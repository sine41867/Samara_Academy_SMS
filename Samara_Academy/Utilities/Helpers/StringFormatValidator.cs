using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samara_Academy.Utilities.Helpers
{
    public static class StringFormatValidator
    {
        public static bool IsValidClassID(string classId)
        {
            if (string.IsNullOrEmpty(classId))
            {
                return false;
            }

            if (classId.Length != 9)
            {
                return false;
            }

            if (!classId.StartsWith("CLS"))
            {
                return false;
            }

            try
            {
                Convert.ToInt32(classId.Substring(3, 6));
            }
            catch
            {
                return false;
            }

            return true;


        }
        public static bool IsValidTeacherID(string teacherId)
        {
            if (string.IsNullOrEmpty(teacherId))
            {
                return false;
            }

            if (teacherId.Length != 9)
            {
                return false;
            }

            if (!teacherId.StartsWith("TCR"))
            {
                return false;
            }

            try
            {
                Convert.ToInt32(teacherId.Substring(3, 6));
            }
            catch
            {
                return false;
            }

            return true;


        }

        public static bool IsValidStudentID(string studentId)
        {
            if (string.IsNullOrEmpty(studentId))
            {
                return false;
            }

            if (studentId.Length != 9)
            {
                return false;
            }

            if (!studentId.StartsWith("STD"))
            {
                return false;
            }

            try
            {
                Convert.ToInt32(studentId.Substring(3, 6));
            }
            catch
            {
                return false;
            }

            return true;


        }
    }
}
