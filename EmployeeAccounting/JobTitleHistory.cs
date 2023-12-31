using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace EmployeeAccounting
{
    public partial class JobTitleHistory : Form
    {
        public JobTitleHistory()
        {
            InitializeComponent();
            FillDataGridView();
        }

        private void FillDataGridView()
        {
            try
            {
                using (SqlConnection connection = new(DatabaseHelper.connectionString))
                {
                    connection.Open();

                    string query =
                        "SELECT " +
                        "   e.full_name AS N'ФИО', " +
                        "   e.phone_number AS N'Номер телефона', " +
                        "   jt.name AS N'Должность', " +
                        "   jh.start_date AS N'Дата начала', " +
                        "   jh.end_date AS N'Дата конца' " +
                        "FROM JobHistory jh " +
                        "INNER JOIN JobTitle jt ON jh.job_title_id = jt.id " +
                        "INNER JOIN Employee e ON jh.employee_id = e.id";

                    using (SqlDataAdapter dataAdapter = new(query, connection))
                    {
                        DataTable dataTable = new();

                        dataAdapter.Fill(dataTable);

                        dataGridView1.DataSource = dataTable;
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void JobTitleHistory_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms.Count == 0)
            {
                Application.Exit();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HomeForm homeForm = new();
            this.Hide();
            homeForm.Show();
            this.Close();
        }
    }
}
