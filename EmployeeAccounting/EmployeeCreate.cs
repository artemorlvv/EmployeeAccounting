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

namespace EmployeeAccounting
{
    public partial class EmployeeCreate : Form
    {
        public EmployeeCreate()
        {
            InitializeComponent();
            FillComboBox();
        }

        public delegate void EmployeeCreateEventHandler(object sender, int id, string full_name, string job_title, string phone_number);
        public event EmployeeCreateEventHandler EmployeeCreateEvent;

        private void RaiseEmployeeDeleteEvent(int id, string full_name, string job_title, string phone_number)
        {
            EmployeeCreateEvent?.Invoke(this, id, full_name, job_title, phone_number);
        }

        private void FillComboBox()
        {
            List<string> jobTitles = new List<string>();

            try
            {
                using (SqlConnection connection = new SqlConnection(DatabaseHelper.connectionString))
                {
                    connection.Open();

                    string query = "SELECT name FROM JobTitle";

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(query, connection))
                    {
                        DataTable dataTable = new DataTable();
                        dataAdapter.Fill(dataTable);

                        // Loop through the rows and add each job title to the list
                        foreach (DataRow row in dataTable.Rows)
                        {
                            string jobTitle = row["name"].ToString();
                            jobTitles.Add(jobTitle);
                        }
                    }

                    connection.Close();

                    comboBox1.DataSource = jobTitles;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string full_name = textBox1.Text.Trim();
            string job_title = comboBox1.Text;
            string phone_number = maskedTextBox1.Text;

            if (full_name == "")
            {
                MessageBox.Show("Поле ФИО не может быть пустым");
                return;
            }
            if (!maskedTextBox1.MaskCompleted)
            {
                MessageBox.Show("Заполните номер телефона полностью");
                return;
            }

            try
            {
                using (SqlConnection connection = new(DatabaseHelper.connectionString))
                {
                    connection.Open();

                    string query =
                        "INSERT INTO Employee(job_title_id, full_name, phone_number) " +
                        "VALUES (" +
                        "   (SELECT id FROM JobTitle WHERE name = @job_title), " +
                        "   @full_name, " +
                        "   @phone_number" +
                        ");" +
                        "SELECT CAST(scope_identity() AS int)";

                    SqlCommand sqlCommand = new SqlCommand(query, connection);
                    sqlCommand.Parameters.AddWithValue("@full_name", full_name);
                    sqlCommand.Parameters.AddWithValue("@job_title", job_title);
                    sqlCommand.Parameters.AddWithValue("@phone_number", phone_number);

                    int generatedId = (int)sqlCommand.ExecuteScalar();

                    connection.Close();

                    RaiseEmployeeDeleteEvent(generatedId, full_name, job_title, phone_number);

                    this.Close();

                    MessageBox.Show($"Запись успешно добавлена с ID: {generatedId}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
