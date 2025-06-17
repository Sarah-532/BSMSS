using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex_01_MasterDetails
{
    public partial class EditForm : Form
    {
        string currentFile = string.Empty;
        List<CourseDetails> details = new List<CourseDetails>();
        string oldFile = "";
        public EditForm()
        {
            InitializeComponent();
        }
        public frmEdit TheForm { get; set; }
        public int IdToEdit {  get; set; }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(currentFile);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            details.Add(new CourseDetails
            {
                Role = txtCourseName.Text,
                AssignDate = dptStartDate.Value,
                Deadline = dptEndDate.Value,
                Remuneration = numericUpDown1.Value
            });
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;
        }

        private void EditForm_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadInForm();
        }
        private void LoadInForm()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM members WHERE memberId=@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        txtStudentName.Text = dr.GetString(1);
                        dptDob.Value = dr.GetDateTime(2).Date;
                        checkBox1.Checked = dr.GetBoolean(3);
                        pictureBox1.Image = Image.FromFile(@"..\..\Picture\" + dr.GetString(4));
                        oldFile=dr.GetString(4);

                    }
                    dr.Close();
                    cmd.CommandText = "SELECT * FROM roles WHERE memberId=@i";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    SqlDataReader dr2 = cmd.ExecuteReader();
                    while (dr2.Read())
                    {
                        details.Add(new CourseDetails
                        {
                            Role = dr2.GetString(1),
                            AssignDate = dr2.GetDateTime(2).Date,
                            Deadline = dr2.GetDateTime(3).Date,
                            Remuneration = dr2.GetDecimal(4)
                        });
                    }
                    SetDataSource();
                    con.Close();
                }
            }
        }
        private void SetDataSource()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = details;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4) 
            {
                details.RemoveAt(e.RowIndex);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = details;
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = trx;

                        string f = oldFile;
                        if(currentFile!= "")
                        {
                            string ext = Path.GetExtension(currentFile);
                            f = Path.GetFileNameWithoutExtension(DateTime.Now.Ticks.ToString()) + ext;

                            string savePath = @"..\..\Picture\" + f;
                            MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                            byte[] bytes = ms.ToArray();
                            FileStream fs = new FileStream(savePath, FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }

                        cmd.CommandText = "UPDATE members SET memberName=@sn, dateOfBirth=@dob, running=@isd,picture=@pic WHERE memberId=@id";
                        cmd.Parameters.AddWithValue("@id", IdToEdit);
                        cmd.Parameters.AddWithValue("@sn", txtStudentName.Text);
                        cmd.Parameters.AddWithValue("@dob", dptDob.Value);
                        cmd.Parameters.AddWithValue("@isd", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@pic", f);

                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = "DELETE FROM roles WHERE memberId=@id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            foreach (var s in details)
                            {
                                cmd.CommandText = @"INSERT INTO roles(role,assignDate,deadline,remuneration,memberId) VALUES(@cn,@sd,@ed,@cf,@i)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@cn", s.Role);
                                cmd.Parameters.AddWithValue("@sd", s.AssignDate);
                                cmd.Parameters.AddWithValue("@ed", s.Deadline);
                                cmd.Parameters.AddWithValue("@cf", s.Remuneration);
                                cmd.Parameters.AddWithValue("@i", IdToEdit);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();
                            TheForm.LoadDataBindingSource();
                            MessageBox.Show("Data Updated successfully!!");
                            details.Clear();
                        }
                        catch (Exception)
                        {
                            trx.Rollback();
                        }
                    }
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    string sql = @"DELETE FROM roles WHERE memberId=@id";
                    using (SqlCommand cmd = new SqlCommand(sql,con,trx))
                    {
                        cmd.Parameters.AddWithValue("@id",IdToEdit);
                        try
                        {
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            cmd.CommandText = "DELETE FROM members WHERE memberId=@id";
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            trx.Commit();
                            TheForm.LoadDataBindingSource();
                            MessageBox.Show("Data Deleted successfully!!");
                            details.Clear();
                            this.Close();
                        }
                        catch (Exception)
                        {
                            trx.Rollback();
                            MessageBox.Show("Failed to delete data!!");
                        }
                        con.Close();
                    }
                }
                
            }
        }
    }
}
