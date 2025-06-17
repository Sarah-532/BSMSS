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

namespace Ex_01_MasterDetails
{
    public partial class frmCRUD_withSP : Form
    {
        string conString = "Data Source=SULTANA;Initial Catalog=ms;Integrated Security=True";
        SqlConnection sqlCon;
        SqlCommand cmd;
        string id = "";
        public frmCRUD_withSP()
        {
            InitializeComponent();
            sqlCon= new SqlConnection(conString);
            sqlCon.Open();
        }

       

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Enter attendee name!!");
                txtName.Select();
            }
            else if (string.IsNullOrWhiteSpace(txtCity.Text))
            {
                MessageBox.Show("Enter city!!");
                txtCity.Select();
            }
            else if (string.IsNullOrWhiteSpace(txtDepartment.Text))
            {
                MessageBox.Show("Enter institution!!");
                txtDepartment.Select();
            }
            else if (comboBox1.SelectedIndex<=-1)
            {
                MessageBox.Show("Select gender!!");
                comboBox1.Select();
            }
            else
            {
                try
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                    }
                    DataTable dtData = new DataTable();
                    cmd = new SqlCommand("SPattendee", sqlCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@actionType", "SaveData");
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@name", txtName.Text);
                    cmd.Parameters.AddWithValue("@city", txtCity.Text);
                    cmd.Parameters.AddWithValue("@institution", txtDepartment.Text);
                    cmd.Parameters.AddWithValue("@gender", comboBox1.Text);

                    int numRes = cmd.ExecuteNonQuery();
                    if (numRes > 0) 
                    {
                        MessageBox.Show("Data saved successfully!!!");
                        LoadGrid();
                        ClearAll();
                    }


                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        private DataTable ShowAllEmployeeData()
        {
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
            DataTable dtData = new DataTable();
            cmd = new SqlCommand("SPattendee", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@actionType", "ShowAllData");
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dtData);
            return dtData;
        }

        private void ClearAll()
        {
            btnSave.Text = "Save";
            txtName.Clear();
            txtCity.Clear();
            txtDepartment.Clear();
            comboBox1.SelectedIndex = -1;
            id = "";
            LoadGrid();
        }

        private void frmCRUD_withSP_Load(object sender, EventArgs e)
        {
            LoadGrid();
        }

        private void LoadGrid()
        {
            dataGridView1.DataSource = ShowAllEmployeeData();
        }

        private DataTable ShowEmpRecordById(string empId)
        {
            if (sqlCon.State == ConnectionState.Closed)
            {
                sqlCon.Open();
            }
            DataTable dtData = new DataTable();
            cmd = new SqlCommand("SPattendee", sqlCon);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@actionType", "ShowAllDataById");
            cmd.Parameters.AddWithValue("@Id", empId);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            sda.Fill(dtData);
            return dtData;
        }
        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                btnSave.Text = "Update";
                id = dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                DataTable dtData = ShowEmpRecordById(id);
                if (dtData.Rows.Count > 0)
                {
                    id = dtData.Rows[0][0].ToString();
                    txtName.Text = dtData.Rows[0][1].ToString();
                    txtCity.Text = dtData.Rows[0][2].ToString();
                    txtDepartment.Text = dtData.Rows[0][3].ToString();
                    comboBox1.Text = dtData.Rows[0][4].ToString();
                }
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    if (sqlCon.State == ConnectionState.Closed)
                    {
                        sqlCon.Open();
                    }
                    DataTable dtData = new DataTable();
                    cmd = new SqlCommand("SPattendee", sqlCon);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@actionType", "DeleteData");
                    cmd.Parameters.AddWithValue("@Id", id);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    int numRes = cmd.ExecuteNonQuery();
                    if (numRes > 0)
                    {
                        MessageBox.Show("Data deleted successfully!!!");
                        LoadGrid();
                        ClearAll();
                    }
                    else
                    {
                        MessageBox.Show("Please try again!!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: - " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a record!!");
            }
        }
    }
}
