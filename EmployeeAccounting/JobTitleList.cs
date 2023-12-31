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
    public partial class JobTitleList : Form
    {
        public JobTitleList()
        {
            InitializeComponent();
            FillDataGridView();
        }

        private object originalValue;
        private readonly string name = "Название";
        private readonly string description = "Описание";
        private readonly string salary = "Оклад";

        private void FillDataGridView()
        {
            try
            {
                // Создаем подключение к базе данных
                using (SqlConnection connection = new(DatabaseHelper.connectionString))
                {
                    connection.Open();

                    // SQL-запрос
                    string query = $"SELECT id, name AS N'{name}', description AS N'{description}', salary AS N'{salary}' FROM JobTitle";

                    // Создаем адаптер данных
                    using (SqlDataAdapter dataAdapter = new(query, connection))
                    {
                        // Создаем DataTable для хранения данных
                        DataTable dataTable = new();

                        // Заполняем DataTable данными из базы данных
                        dataAdapter.Fill(dataTable);

                        // Устанавливаем DataTable как источник данных для DataGridView
                        dataGridView1.DataSource = dataTable;
                        dataGridView1.Columns["id"].Visible = false;
                    }

                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void JobTitleList_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Application.OpenForms.Count == 0)
            {
                Application.Exit();
            }
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                originalValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                object currentValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                string currentColumnName = dataGridView1.Columns[e.ColumnIndex].Name;
                int job_title_id = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value);
                string query = null;

                if (!object.Equals(currentValue, originalValue))
                {
                    if (currentValue != null)
                    {
                        if (currentColumnName == salary)
                        {
                            if (int.TryParse(currentValue.ToString(), out int salaryValue) && salaryValue > 0)
                            {
                                query = "UPDATE JobTitle SET salary = @Value WHERE id = @JobTitleId";
                            }
                            else
                            {
                                MessageBox.Show("Запрос отменен.\nЗначение оклада должно быть больше 0.");
                                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = originalValue;
                            }
                        }
                        else if (currentColumnName == name)
                        {
                            if (currentValue.ToString().Trim() != "")
                            {
                                query = "UPDATE JobTitle SET name = @Value WHERE id = @JobTitleId";
                            }
                            else
                            {
                                MessageBox.Show("Запрос отменен.\nНазвание не может быть пустым");
                                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = originalValue;
                            }
                        }
                        else if (currentColumnName == description)
                        {
                            if (currentValue.ToString().Trim() != "")
                            {
                                query = "UPDATE JobTitle SET description = @Value WHERE id = @JobTitleId";
                            }
                            else
                            {
                                MessageBox.Show("Запрос отменен.\nОписание не может быть пустым");
                                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = originalValue;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Значение не может быть пустым");
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = originalValue;
                    }
                }

                if (query != null)
                {
                    try
                    {
                        using (SqlConnection connection = new(DatabaseHelper.connectionString))
                        {
                            connection.Open();

                            using (SqlCommand sqlCommand = new(query, connection))
                            {
                                sqlCommand.Parameters.AddWithValue("@Value", currentValue?.ToString());
                                sqlCommand.Parameters.AddWithValue("@JobTitleId", job_title_id);
                                sqlCommand.ExecuteNonQuery();
                            }

                            connection.Close();

                            MessageBox.Show("Изменения сохранены");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: " + ex.Message);
                        dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = originalValue;
                    }
                }
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

            if (e.Exception is FormatException)
            {
                MessageBox.Show("Неверный формат. В окладе может быть только число");
            }
            else
            {
                MessageBox.Show($"Неопознанная ошибка: {e.Exception.Message}");
            }

            e.ThrowException = false;
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
