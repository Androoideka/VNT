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
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }
        public ListBox ListBox1
        {
            get
            {
                return listBox1;
            }
        }
        private void StartForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                listBox1.Items.Add(textBox1.Text);
                textBox1.Text = "";
            }
        }
        public NumericUpDown numOfSlides
        {
            get
            {
                return numericUpDown1;
            }
            set
            {
                numericUpDown1 = value;
            }
        }
        public NumericUpDown numOfPictures
        {
            get
            {
                return numericUpDown2;
            }
            set
            {
                numericUpDown2 = value;
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
