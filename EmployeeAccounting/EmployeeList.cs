﻿using System;
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
    public partial class EmployeeList : Form
    {
        private Employee employeeForm;
        private EmployeeCreate employeeCreateForm;
        DataTable dataTable = new();

        public EmployeeList()
        {
            InitializeComponent();
            FillDataGridView();
        }

        private void EmployeeList_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (employeeForm != null && !employeeForm.IsDisposed)
            {
                employeeForm.Close();
            }
            if (employeeCreateForm != null && !employeeCreateForm.IsDisposed)
            {
                employeeCreateForm.Close();
            }
            if (Application.OpenForms.Count == 0)
            {
                Application.Exit();
            }
        }

        private void EmployeeList_Load(object sender, EventArgs e)
        {

        }

        private void FillDataGridView()
        {
            try
            {
                // Создаем подключение к базе данных
                using (SqlConnection connection = new(DatabaseHelper.connectionString))
                {
                    connection.Open();

                    // SQL-запрос
                    string query =
                        "SELECT e.id AS N'id', e.full_name AS N'ФИО', jt.name AS N'Должность', phone_number AS N'Номер телефона' " +
                        "FROM Employee e " +
                        "INNER JOIN JobTitle jt " +
                        "ON e.job_title_id = jt.id ";

                    // Создаем адаптер данных
                    using (SqlDataAdapter dataAdapter = new(query, connection))
                    {

                        // Заполняем DataTable данными из базы данных
                        dataAdapter.Fill(dataTable);

                        // Устанавливаем DataTable как источник данных для DataGridView
                        dataGridView1.DataSource = dataTable;
                    }

                    connection.Close();
                }

                DataGridViewButtonColumn buttonColumn = new()
                {
                    HeaderText = "Действие",
                    Text = "Подробнее",
                    UseColumnTextForButtonValue = true
                };
                dataGridView1.Columns.Add(buttonColumn);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn &&
                e.RowIndex >= 0)
            {
                int employeeId = Convert.ToInt32(dataGridView1.Rows[e.RowIndex].Cells["id"].Value);
                if (employeeForm == null || employeeForm.IsDisposed)
                {
                    employeeForm = new Employee(employeeId);
                    employeeForm.Show();
                    employeeForm.InfoUpdateEvent += InfoUpdate;
                    employeeForm.EmployeeDeleteEvent += EmployeeDelete;
                }
                else
                {
                    employeeForm.BringToFront();
                }
            }
        }

        private void InfoUpdate(object sender, int employeeId, string full_name, string phone_number, string job_title)
        {
            DataRow[] foundRows = dataTable.Select($"id = {employeeId}");

            if (foundRows.Length > 0)
            {
                // Assuming the columns exist in the DataTable
                foundRows[0]["ФИО"] = full_name;
                foundRows[0]["Номер телефона"] = phone_number;
                foundRows[0]["Должность"] = job_title;

                // Optionally, you can call AcceptChanges to commit the changes to the DataRow
                foundRows[0].AcceptChanges();
            }
            else
            {
                MessageBox.Show("Ошибка. Не удалось обновить информацию о сотруднике.");
            }
        }

        private void EmployeeDelete(object sender, int employeeId)
        {
            try
            {
                DataRow[] foundRows = dataTable.Select($"id = {employeeId}");

                if (foundRows.Length > 0)
                {
                    foundRows[0].Delete();
                }
                else
                {
                    MessageBox.Show("Ошибка. Сотрудника с переданным ID не найдено");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка в при удалении сотрудника из таблицы:\n" + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            HomeForm homeForm = new();
            this.Hide();
            if (employeeForm != null && !employeeForm.IsDisposed)
            {
                employeeForm.Close();
            }
            if (employeeCreateForm != null && !employeeCreateForm.IsDisposed)
            {
                employeeCreateForm.Close();
            }
            homeForm.Show();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (employeeCreateForm == null || employeeCreateForm.IsDisposed)
            {
                employeeCreateForm = new EmployeeCreate();
                employeeCreateForm.Show();
                employeeCreateForm.EmployeeCreateEvent += CreateEmployee;
            }
            else
            {
                employeeCreateForm.BringToFront();
            }
        }

        private void CreateEmployee(object sender, int id, string full_name, string job_title, string phone_number)
        {
            try
            {
                DataRow newRow = dataTable.NewRow();

                newRow["id"] = id;
                newRow["ФИО"] = full_name;
                newRow["Номер телефона"] = phone_number;
                newRow["Должность"] = job_title;

                dataTable.Rows.Add(newRow);

                //int rowIndex = dataTable.Rows.IndexOf(newRow);

                //// Check if the rowIndex is within the valid range
                //if (rowIndex >= 0 && rowIndex < dataGridView1.Rows.Count)
                //{
                //    dataGridView1.Rows[rowIndex].Selected = true;

                //    // Optionally, scroll to the selected row
                //    dataGridView1.FirstDisplayedScrollingRowIndex = rowIndex;
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка: " + ex.Message);
            }
        }
    }
}
