using SQLite;
using System;
using System.Collections.Generic;
using System.IO;

namespace C971_Performance_Assessment
{
    [Table("Terms")]
    public class Term
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    [Table("Courses")]
    public class Course
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int TermId { get; set; }
        public string Title { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public bool StartNotification { get; set; }
        public DateTime EndDate { get; set; }
        public bool EndNotification { get; set; }
        public int InstructorId { get; set; }
        public string Notes { get; set; }
    }

    [Table("Assessments")]
    public class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public DateTime StartDate { get; set; }
        public bool StartNotification { get; set; }
        public DateTime EndDate { get; set; }
        public bool EndNotification { get; set; }
    }

    [Table("Instructors")]
    public class Instructor
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }

    public class Database
    {
        public static List<Term> termList;
        public static List<Course> courseList;
        public static List<Assessment> assessmentList;
        public static List<Instructor> instructorList;
        public static SQLiteAsyncConnection dbConn;

        public static void SetDatabase()
        {
            //File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SchoolPlanner.db"));
            dbConn = new SQLiteAsyncConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SchoolPlanner.db"));
        }
    }
}