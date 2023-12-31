namespace EmployeeAccounting
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            HomeForm mainForm = new HomeForm();
            mainForm.Show();
            // Run the application loop without exiting when the main form is closed
            Application.Run();
        }
    }
}