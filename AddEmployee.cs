using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace EmployeeManagementSystem
{
    public partial class AddEmployee : UserControl
    {
        SqlConnection connect = new SqlConnection(@"Data Source=MAXIM_KEMAJOU;Initial Catalog=Employee;Integrated Security=True;TrustServerCertificate=True");
        public AddEmployee()
        {
            InitializeComponent();

            // display employee data view    
            displayEmployeeData();
        }

        public void displayEmployeeData()
        {
            EmployeeData employeeData = new EmployeeData();
            List<EmployeeData> listData = employeeData.EmployeeListData();

            dataGridViewEmployeeData.DataSource = listData;
        }
        private void addEmployee_addBtn_Click(object sender, EventArgs e)
        {
            if (addEmployee_id.Text == "" || addEmployee_id.Text == "" || addEmployee_fullName.Text == "" ||
                addEmployee_gender.Text == "" || addEmployee_status.Text == "" || addEmployee_position.Text == "" ||
                addEmployee_phoneNum.Text == "" || addEmployee_picture.Image == null)
            {
                MessageBox.Show("Please fill all blank fields",
                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (connect.State == ConnectionState.Closed)
                {
                    try
                    {
                        connect.Open();
                        string checkEmployeeId = "SELECT COUNT(*) FROM employees WHERE employee_id = @employee_id";

                        using (SqlCommand cmdEmployeeId = new SqlCommand(checkEmployeeId, connect))
                        {
                            cmdEmployeeId.Parameters.AddWithValue("@employee_id", addEmployee_id.Text.Trim());
                            int count = (int)cmdEmployeeId.ExecuteScalar();

                            if (count >= 1)
                            {
                                MessageBox.Show(addEmployee_id.Text.Trim() + " is already taken", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                DateTime today = DateTime.Now;

                                string insertData = "INSERT INTO employees (employee_id, full_name, gender, contact_number, position, image, insert_date, status) VALUES (@employeeID, @fullName, @gender, @contactNumber, @position, @image, @insertDate, @status)";

                                string path = Path.Combine(@"D:\Programing\C#\Master C#\Practic\Projects\EmployeeManagementSystem\Directory\" + addEmployee_id.Text.Trim() + " .jpg");

                                string directoryPath = Path.GetDirectoryName(path);

                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }

                                File.Copy(addEmployee_picture.ImageLocation, path, true);

                                using (SqlCommand cmdInsert = new SqlCommand(insertData, connect))
                                {
                                    cmdInsert.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());
                                    cmdInsert.Parameters.AddWithValue("@fullName", addEmployee_fullName.Text.Trim());
                                    cmdInsert.Parameters.AddWithValue("@gender", addEmployee_gender.Text.Trim());
                                    cmdInsert.Parameters.AddWithValue("@contactNumber", addEmployee_phoneNum.Text.Trim());
                                    cmdInsert.Parameters.AddWithValue("@position", addEmployee_position.Text.Trim());
                                    //cmdInsert.Parameters.AddWithValue("@salary", 0);
                                    cmdInsert.Parameters.AddWithValue("@image", path);
                                    cmdInsert.Parameters.AddWithValue("@insertDate", today);
                                    cmdInsert.Parameters.AddWithValue("@status", addEmployee_status.Text.Trim());

                                    cmdInsert.ExecuteNonQuery();

                                    // Clear fields after adding employee
                                    ClearFields();

                                    displayEmployeeData();

                                    MessageBox.Show("Employee added successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error : " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }
                }
            }
        }

        private void addEmployee_importBtn_Click(object sender, EventArgs e)
        {

            try
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.Filter = "Image files(*.jpg)|*.jpg| PNG files(*.png)|*.png| All Files(*.*)|*.*";

                string imagePath = "";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    imagePath = dialog.FileName;
                    addEmployee_picture.ImageLocation = imagePath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred :" + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        //Méthode pour effacer les champs du formulaire
        private void ClearFields()
        {
            addEmployee_id.Clear();
            addEmployee_fullName.Clear();
            addEmployee_gender.SelectedIndex = -1;
            addEmployee_status.SelectedIndex = -1;
            addEmployee_position.SelectedIndex = -1;
            addEmployee_phoneNum.Clear();
            addEmployee_picture.Image = null;
        }

        private void dataGridViewEmployeeData_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1)
            {
                DataGridViewRow row = dataGridViewEmployeeData.Rows[e.RowIndex];
                addEmployee_id.Text = row.Cells[1].Value.ToString();
                addEmployee_fullName.Text = row.Cells[2].Value.ToString();
                addEmployee_gender.Text = row.Cells[3].Value.ToString();
                addEmployee_phoneNum.Text = row.Cells[4].Value.ToString();
                addEmployee_position.Text = row.Cells[5].Value.ToString();


                string imagePath = row.Cells[6].Value.ToString();

                if (imagePath != null)
                {
                    addEmployee_picture.ImageLocation = System.Drawing.Image.FromFile(imagePath).ToString();
                }
                else
                {
                    addEmployee_picture.Image = null;
                }

                addEmployee_status.Text = row.Cells[8].Value.ToString();
            }
        }

        private void addEmployee_updateBtn_Click(object sender, EventArgs e)
        {
            if (addEmployee_id.Text == "" || addEmployee_id.Text == "" || addEmployee_fullName.Text == "" ||
             addEmployee_gender.Text == "" || addEmployee_status.Text == "" || addEmployee_position.Text == "" ||
             addEmployee_phoneNum.Text == "" || addEmployee_picture.Image == null)
            {
                MessageBox.Show("Please fill all blank fields",
                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult checkConfirmation = MessageBox.Show("Are you sure you want to update this employee with Employee ID: " + addEmployee_id.Text.Trim() + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (checkConfirmation == DialogResult.Yes)
                {

                    try
                    {
                        connect.Open();

                        DateTime today = DateTime.Now;

                        string updateData = "UPDATE employees SET full_name = @fullName, gender = @gender, contact_number = @contactNum, position = @position, update_date = @updateDate, status = @status WHERE employee_id = @employeeID";
                        using (SqlCommand cmdUpdate = new SqlCommand(updateData, connect))
                        {
                            cmdUpdate.Parameters.AddWithValue("@fullName", addEmployee_fullName.Text.Trim());
                            cmdUpdate.Parameters.AddWithValue("@gender", addEmployee_gender.Text.Trim());
                            cmdUpdate.Parameters.AddWithValue("@contactNum", addEmployee_phoneNum.Text.Trim());
                            cmdUpdate.Parameters.AddWithValue("@position", addEmployee_position.Text.Trim());
                            cmdUpdate.Parameters.AddWithValue("@updateDate", today);
                            cmdUpdate.Parameters.AddWithValue("@status", addEmployee_status.Text.Trim());
                            cmdUpdate.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());

                            cmdUpdate.ExecuteNonQuery();

                            // Clear fields after adding employee
                            ClearFields();

                            displayEmployeeData();

                            MessageBox.Show("Employee updated successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error : " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }

                }
                else
                {
                    MessageBox.Show("Update canceled!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }

        private void addEmployee_clearBtn_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void addEmployee_deleteBtn_Click(object sender, EventArgs e)
        {
            if (addEmployee_id.Text == "" || addEmployee_id.Text == "" || addEmployee_fullName.Text == "" ||
            addEmployee_gender.Text == "" || addEmployee_status.Text == "" || addEmployee_position.Text == "" ||
            addEmployee_phoneNum.Text == "" || addEmployee_picture.Image == null)
            {
                MessageBox.Show("Please fill all blank fields",
                    "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                DialogResult checkConfirmation = MessageBox.Show("Are you sure you want to delete this employee with Employee ID: " + addEmployee_id.Text.Trim() + "?", "Confirmation Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (checkConfirmation == DialogResult.Yes)
                {

                    try
                    {
                        connect.Open();

                        DateTime today = DateTime.Now;

                        string deleteData = "DELETE FROM employees WHERE employee_id = @employeeID";

                        using (SqlCommand cmdDelete = new SqlCommand(deleteData, connect))

                        {

                            cmdDelete.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());
                            //cmdUpdate.Parameters.AddWithValue("@deleteDate", today);
                            //cmdUpdate.Parameters.AddWithValue("@employeeID", addEmployee_id.Text.Trim());


                            cmdDelete.ExecuteNonQuery();

                            // Clear fields after adding employee
                            ClearFields();

                            displayEmployeeData();

                            MessageBox.Show("Employee deleted successfully!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error : " + ex, "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connect.Close();
                    }

                }
                else
                {
                    MessageBox.Show("Update canceled!", "Information Message", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }
    }
}
