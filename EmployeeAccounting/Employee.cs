using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace EmployeeAccounting
{
    public partial class Employee : Form
    {
        private readonly int employeeId;
        readonly DataTable dataTable = new();
        private string full_name;
        private string phone_number;
        private string job_title;

        public Employee(int employee_id)
        {
            InitializeComponent();
            employeeId = employee_id;
            FillInfo();
            FillDataGridView();
        }

        public delegate void InfoUpdateEventHandler(object sender, int employeeId, string full_name, string phone_number, string job_title);
        public event InfoUpdateEventHandler InfoUpdateEvent;

        private void RaiseInfoUpdateEvent()
        {
            InfoUpdateEvent?.Invoke(this, employeeId, full_name, phone_number, job_title);
        }

        public delegate void EmployeeDeleteEventHandler(object sender, int employeeId);
        public event EmployeeDeleteEventHandler EmployeeDeleteEvent;

        private void RaiseEmployeeDeleteEvent()
        {
            EmployeeDeleteEvent?.Invoke(this, employeeId);
        }

        private void FillInfo()
        {
            try
            {
                using (SqlConnection connection = new(DatabaseHelper.connectionString))
                {
                    connection.Open();

                    string query =
                        "SELECT e.full_name, e.phone_number, jt.name AS N'job_title_name' " +
                        "FROM Employee e " +
                        "INNER JOIN JobTitle jt ON e.job_title_id = jt.id " +
                        "WHERE e.id = @id";

                    using (SqlDataAdapter dataAdapter = new(query, connection))
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@id", employeeId);

                        dataAdapter.Fill(dataTable);

                        full_name = dataTable.Rows[0]["full_name"].ToString();
                        phone_number = dataTable.Rows[0]["phone_number"].ToString();
                        job_title = dataTable.Rows[0]["job_title_name"].ToString();

                        textBox1.Text = full_name;
                        maskedTextBox1.Text = phone_number;
                    }

                    List<string> jobTitleList = GetJobTitleList();

                    comboBox1.DataSource = jobTitleList;
                    comboBox1.SelectedItem = job_title;

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
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
                        "   jt.name AS N'Должность', " +
                        "   jh.start_date AS N'Дата начала', " +
                        "   jh.end_date AS N'Дата конца' " +
                        "FROM JobHistory jh " +
                        "INNER JOIN JobTitle jt ON jh.job_title_id = jt.id " +
                        "INNER JOIN Employee e ON jh.employee_id = e.id " +
                        "WHERE jh.employee_id = @id";

                    using (SqlDataAdapter dataAdapter = new(query, connection))
                    {
                        dataAdapter.SelectCommand.Parameters.AddWithValue("@id", employeeId);

                        DataTable dataTableJobHistory = new();

                        dataAdapter.Fill(dataTableJobHistory);

                        dataGridView1.DataSource = dataTableJobHistory;
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private List<string> GetJobTitleList()
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
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            return jobTitles;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
            {
                MessageBox.Show("Ошибка!\nПоле 'ФИО' не может быть пустым");
                return;
            }
            if (!maskedTextBox1.MaskCompleted)
            {
                MessageBox.Show("Ошибка!\nЗаполните номер телефона полностью");
                return;
            }

            List<string> updatedValues = [];

            if (textBox1.Text.Trim() != full_name)
            {
                updatedValues.Add("full_name = @full_name");
            }

            if (maskedTextBox1.Text != phone_number)
            {
                updatedValues.Add("phone_number = @phone_number");
            }

            if (comboBox1.SelectedItem.ToString() != job_title)
            {
                updatedValues.Add("job_title_id = (SELECT id FROM JobTitle WHERE name = @job_title)");
            }

            if (updatedValues.Count == 0)
            {
                MessageBox.Show("Нет изменений для сохранения.");
                return;
            }

            string setClause = string.Join(", ", updatedValues);

            string query = $"UPDATE Employee SET {setClause} WHERE id = @employee_id";

            try
            {
                using (SqlConnection connection = new(DatabaseHelper.connectionString))
                {
                    connection.Open();

                    using (SqlCommand sqlCommand = new(query, connection))
                    {
                        sqlCommand.Parameters.AddWithValue("@full_name", textBox1.Text.Trim());
                        sqlCommand.Parameters.AddWithValue("@phone_number", maskedTextBox1.Text);
                        sqlCommand.Parameters.AddWithValue("@job_title", comboBox1.SelectedItem.ToString());
                        sqlCommand.Parameters.AddWithValue("@employee_id", employeeId);
                        sqlCommand.ExecuteNonQuery();
                    }

                    connection.Close();

                    full_name = textBox1.Text.Trim();
                    phone_number = maskedTextBox1.Text;
                    job_title = comboBox1.SelectedItem.ToString();

                    FillDataGridView();
                    RaiseInfoUpdateEvent();

                    MessageBox.Show("Изменения сохранены");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить сотрудника?\nВместе с этим удалятся все записи в истории должностей.\nЭто действие невозможно отменить!", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (result == DialogResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new(DatabaseHelper.connectionString))
                    {
                        connection.Open();

                        string query = "DELETE FROM Employee WHERE id = @employee_id";

                        using (SqlCommand sqlCommand = new(query, connection))
                        {
                            sqlCommand.Parameters.AddWithValue("@employee_id", employeeId);
                            sqlCommand.ExecuteNonQuery();
                        }

                        connection.Close();

                        RaiseEmployeeDeleteEvent();

                        this.Close();

                        MessageBox.Show("Сотрудник удален!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении:\n" + ex.Message);
                }
            }
        }
    }
}
