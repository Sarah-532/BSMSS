using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex_01_MasterDetails
{
    public partial class frmAddSup : Form
    {
        public frmAddSup()
        {
            InitializeComponent();
        }
        private void LoadCombo()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM roles", con))
                {
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    cmbSub.DataSource = ds.Tables[0];
                    cmbSub.DisplayMember = "role";
                    cmbSub.ValueMember = "roleId";
                }
            }
        }

        private void btnPicture_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(openFileDialog1.FileName);
                this.pictureBox1.Image = img;
                txtPicturePath.Text = openFileDialog1.FileName;
            }
        }

        private void btnInsert_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                Image img = Image.FromFile(txtPicturePath.Text);
                MemoryStream ms = new MemoryStream();
                img.Save(ms, ImageFormat.Bmp);

                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = "INSERT INTO supervisor(Id,Name,Contact,Email,picture,roleId) VALUES(@i,@n,@c,@e,@p,@s)";
                cmd.Parameters.AddWithValue("@i", txtId.Text);
                cmd.Parameters.AddWithValue("@n", txtName.Text);
                cmd.Parameters.AddWithValue("@c", txtContact.Text);
                cmd.Parameters.AddWithValue("@e", txtEmail.Text);
                cmd.Parameters.Add(new SqlParameter("@p", SqlDbType.VarBinary) { Value = ms.ToArray() });
                cmd.Parameters.AddWithValue("@s", cmbSub.SelectedValue);

                if (cmd.ExecuteNonQuery() > 0)
                {
                    MessageBox.Show("Data Inserted Successfully!!!");
                    ClearAll();
                }
                con.Close();
            }
        }
        private void ClearAll()
        {
            txtId.Clear();
            txtName.Clear();
            txtContact.Clear();
            txtEmail.Clear();
            txtPicturePath.Clear();
            cmbSub.SelectedIndex = -1;
            pictureBox1.Image = null;
        }

        private void frmAddSup_Load(object sender, EventArgs e)
        {
            LoadCombo();
        }

        private void cmbSub_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
