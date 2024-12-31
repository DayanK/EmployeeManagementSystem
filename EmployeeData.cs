using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    class EmployeeData
    {
        public int ID { get; set; }
        public string EmployeeID { get; set; }
        public string Name { get; set; }
        public string Gender { get; set; }
        public string Contact { get; set; }
        public string Position { get; set; }
        public string Image { get; set; }
        public int Salary { get; set; }
        public string Status { get; set; }



        SqlConnection connect = new SqlConnection(@"Data Source=MAXIM_KEMAJOU;Initial Catalog=Employee;Integrated Security=True;TrustServerCertificate=True");

        public List<EmployeeData> EmployeeListData()
        {
            List<EmployeeData> listData = new List<EmployeeData>();

            if (connect.State != ConnectionState.Open)
            {
                try
                {
                    connect.Open();
                    string selectData = "SELECT * FROM employees WHERE delete_date IS NULL";

                    using (SqlCommand cmd = new SqlCommand(selectData, connect))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            EmployeeData employeeData = new EmployeeData();

                            employeeData.ID = (int)reader["id"];
                            employeeData.EmployeeID = reader["employee_id"].ToString();
                            employeeData.Name = reader["full_name"].ToString();
                            employeeData.Gender = reader["gender"].ToString();
                            employeeData.Contact = reader["contact_number"].ToString();
                            employeeData.Position = reader["position"].ToString();
                            employeeData.Image = reader["image"].ToString();
                            //employeeData.Salary = (int)reader["salary"];
                            employeeData.Status = reader["status"].ToString();

                            // Check for DBNull before converting to Int32
                            //employeeData.Salary = reader["salary"] != DBNull.Value ? Convert.ToInt32(reader["salary"]) : 0;

                            listData.Add(employeeData);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error : " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    if (connect.State == ConnectionState.Open)
                    {
                        connect.Close();
                    }
                }

            }
            return listData;
        }
    }
}