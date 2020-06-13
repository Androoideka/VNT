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
            openFileDialog1.Filter = "VNT (*.vnt)|*.vnt|All files|*.*";
            openFileDialog1.DefaultExt = "vnt";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
                novelFormLoader(openFileDialog1.FileName, true);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "VNT (*.vnt)|*.vnt|All files|*.*";
            saveFileDialog1.DefaultExt = "vnt";
            DialogResult result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
                novelFormLoader(saveFileDialog1.FileName, false);
        }
        private void novelFormLoader(string file, bool openOrSave)
        {
            Slider form = new Slider(file, openOrSave);
            if (openOrSave)
                form.Text = "Slide Player";
            else
                form.Text = "Slide Creator";
            form.Show();
            this.Hide();
        }
    }
}
