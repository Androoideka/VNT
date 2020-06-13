using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace VNT
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            novelFormLoader(openFileDialog1);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            novelFormLoader(saveFileDialog1);
        }
        private void novelFormLoader(FileDialog fileChoice)
        {
            fileChoice.Filter = "VNT (*.vnt)|*.vnt|All files|*.*";
            fileChoice.FilterIndex = 1;
            fileChoice.FileName = "";
            DialogResult result = fileChoice.ShowDialog();
            if (result == DialogResult.OK)
            {
                Slider form;
                if (fileChoice.GetType() == typeof(OpenFileDialog))
                {
                    form = new Slider(fileChoice.FileName, true);
                    form.Text = "Slide Player";
                }
                else
                {
                    form = new Slider(fileChoice.FileName, false);
                    form.Text = "Slide Creator";
                }
                form.Show();
                this.Hide();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            MessageBox.Show("You need to put all the required pictures in the same folder otherwise it will not work!");
        }
    }
}
