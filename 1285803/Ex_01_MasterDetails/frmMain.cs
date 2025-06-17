using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ex_01_MasterDetails
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAdd ad = new frmAdd();
            ad.Show();
        }

        private void editDeleteShowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEdit fe = new frmEdit();
            fe.Show();
        }

        private void studentInfoReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MembersReport mr = new MembersReport();
            mr.Show();
        }

        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            frmAddSup fas =new frmAddSup();
            fas.Show();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmEditSup se = new frmEditSup();
            se.Show();
        }

        private void tutorInformationReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //frmTutorInformationReport tr = new frmTutorInformationReport();
            //tr.Show();
        }

        private void addEditDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmCRUD_withSP sp = new frmCRUD_withSP();
            sp.Show();
        }

        private void studentToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }
    }
}
