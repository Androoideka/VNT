using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VNT
{
    public partial class Tweaker : Form
    {
        string path;
        int selected = 1;
        public Tweaker()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                path = openFileDialog1.FileName;
        }
        public int clickSetting
        {
            get
            {
                return selected;
            }
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                textBox1.Enabled = false;
                textBox2.Text = "Format: 1.6";
                selected = 1;
            }
            if (radioButton3.Checked)
            {
                textBox1.Enabled = true;
                textBox1.Text = "Format: health+num";
                textBox2.Text = "Format: 1.6";
                selected = 3;
            }
            if (radioButton2.Checked)
            {
                textBox1.Enabled = true;
                textBox1.Text = "Format: health>50";
                textBox2.Text = "Format: 1;1.6";
                selected = 2;
            }
        }
        public string fileName
        {
            get
            {
                return path;
            }
        }
        public string variableManip
        {
            get
            {
                return textBox1.Text;
            }
        }
        public string slideJmp
        {
            get
            {
                return textBox2.Text;
            }
        }
        private void Tweaker_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                this.Close();
            }
        }
    }
}
