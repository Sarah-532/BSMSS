using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex_01_MasterDetails
{
    public partial class frmAdd : Form
    {
        string currentFile = string.Empty;
        List<CourseDetails> details = new List<CourseDetails>();
        public frmAdd()
        {
            InitializeComponent();
        }

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

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    using (SqlCommand cmd=new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = trx;
                        //for image
                        string ext = Path.GetExtension(currentFile);
                        string f = Path.GetFileNameWithoutExtension(DateTime.Now.Ticks.ToString()) + ext;

                        string savePath = @"..\..\Picture\" + f;
                        MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                        byte[] bytes=ms.ToArray();
                        FileStream fs = new FileStream(savePath, FileMode.Create);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();

                        //for data
                        cmd.CommandText = "INSERT INTO members(memberName,dateOfBirth,running,picture) VALUES(@sn,@dob,@isd,@pic); SELECT SCOPE_IDENTITY()";
                        cmd.Parameters.AddWithValue("@sn",txtStudentName.Text);
                        cmd.Parameters.AddWithValue("@dob",dptDob.Value);
                        cmd.Parameters.AddWithValue("@isd", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@pic",f);

                        try
                        {
                            var sid = cmd.ExecuteScalar();
                            foreach (var s in details)
                            {
                                cmd.CommandText = @"INSERT INTO roles(role,assignDate,deadline,remuneration,memberId) VALUES(@cn,@sd,@ed,@cf,@i)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@cn", s.Role);
                                cmd.Parameters.AddWithValue("@sd", s.AssignDate);
                                cmd.Parameters.AddWithValue("@ed", s.Deadline);
                                cmd.Parameters.AddWithValue("@cf", s.Remuneration);
                                cmd.Parameters.AddWithValue("@i", sid);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();
                            MessageBox.Show("Data Saved successfully!!");
                            details.Clear();
                        }
                        catch
                        {
                            trx.Rollback();
                        }
                    }
                }
                con.Close();
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void frmAdd_Load(object sender, EventArgs e)
        {

        }
    }

}
