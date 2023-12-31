using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeAccounting
{
    internal class DatabaseHelper
    {
        public static readonly string connectionString = 
            "Data Source=(localdb)\\MSSQLLocalDB;AttachDbFilename=" +
            "C:\\.college\\database\\EmployeeAccounting\\EmployeeAccounting\\EmployeeAccounting\\Employee.mdf";
    }
}
