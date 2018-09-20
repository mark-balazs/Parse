using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HelpDeskReporter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        OpenFileDialog opf1 = new OpenFileDialog();
        OpenFileDialog opf2 = new OpenFileDialog();
        FolderBrowserDialog fbd = new FolderBrowserDialog();

        private void button1_Click(object sender, EventArgs e)
        {
            opf1.Multiselect = true;
            opf1.Filter = "csv|*.csv";
            if(opf1.ShowDialog()==DialogResult.OK)
            {
                foreach(string name in opf1.FileNames)
                {
                    textBox1.Text += name + ",";
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            opf2.Filter = "xlsx|*.xlsx";
            if (opf2.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = opf2.FileName;
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                textBox3.Text = fbd.SelectedPath;
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if(textBox1.Text==""||textBox2.Text=="" || textBox3.Text=="")
            {
                MessageBox.Show("There are some empty fields!");
                return;
            }
            MainLibrary.HDReporter reporter = new MainLibrary.HDReporter(opf1.FileNames, opf2.FileName);
            reporter.SaveFolder = fbd.SelectedPath;
            reporter.MakeReport();
            MessageBox.Show("Files processed succesfully.");
        }
    }
}
