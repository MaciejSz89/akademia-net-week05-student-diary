using System;
using System.IO;
using System.Windows.Forms;

namespace StudentDiary
{
    public static class Program
    {
        public static string FilePathStudents { get; set; } = Path.Combine($@"{Environment.CurrentDirectory}", "students.txt");
        public static string FilePathGroups { get; set; } = Path.Combine($@"{Environment.CurrentDirectory}", "groups.txt");
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}
