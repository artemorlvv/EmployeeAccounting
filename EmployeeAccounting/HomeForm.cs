namespace EmployeeAccounting
{
    public partial class HomeForm : Form
    {
        public HomeForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            EmployeeList employeeList = new();
            this.Hide();
            employeeList.Show();
            this.Close();
        }

        private void HomeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms.Count == 0)
            {
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            JobTitleList jobTitleList = new();
            this.Hide();
            jobTitleList.Show();
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            JobTitleHistory jobTitleHistory = new();
            this.Hide();
            jobTitleHistory.Show();
            this.Close();
        }
    }
}
