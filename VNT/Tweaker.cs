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
        string image, setting, var;
        string[] vars, slides, selected;
        public Tweaker(string[] slideList, string[] variables, string[] info)
        {
            InitializeComponent();
            slides = slideList;
            vars = variables;
            selected = info;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                image = openFileDialog1.FileName;

        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                label1.Text = "";
                comboBox1.Enabled = false;
                numericUpDown1.Enabled = false;
                comboBox3.Enabled = false;
            }
            if (radioButton2.Checked)
            {
                label1.Text = "Compare to:";
                comboBox1.Enabled = true;
                numericUpDown1.Enabled = true;
                comboBox3.Enabled = true;
            }
            if (radioButton3.Checked)
            {
                label1.Text = "Add:";
                comboBox1.Enabled = true;
                numericUpDown1.Enabled = true;
                comboBox3.Enabled = false;
            }
        }
        public string pictar
        {
            get
            {
                return image;
            }
        }
        public string clickedy
        {
            get
            {
                return setting;
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            var = comboBox1.Text;
            comboBox1.Items[comboBox1.Items.Count - 1] = var;
            comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
        }
        public string variable
        {
            get
            {
                return var;
            }
        }
        private void Tweaker_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                if (radioButton1.Checked)
                    setting = @"1\" + @comboBox2.Items[comboBox2.SelectedIndex];
                if (radioButton2.Checked)
                    setting = @"3\" + @comboBox1.Items[comboBox1.SelectedIndex] + @">" + (int)numericUpDown1.Value + @"\" + @comboBox2.Items[comboBox2.SelectedIndex];
                if (radioButton3.Checked)
                    setting = @"2\" + @comboBox1.Items[comboBox1.SelectedIndex] + @"+" + (int)numericUpDown1.Value + @"\" + @comboBox2.Items[comboBox2.SelectedIndex] + @";" + @comboBox3.Items[comboBox3.SelectedIndex];
                this.Close();
            }
        }

        private void Tweaker_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("");
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            int max = slides.Length;
            for(int i = 0; i < vars.Length; i++)
            {
                comboBox1.Items.Add(vars[i]);
            }
            for(int i = 0; i < slides.Length; i++)
            {
                comboBox2.Items.Add(slides[i]);
                comboBox3.Items.Add(slides[i]);
            }
            if (Convert.ToInt32(selected[4].Substring(0, 1)) == 1)
            {
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (Convert.ToSingle(selected[4].Substring(2, selected[4].Length - 2)) == Convert.ToSingle(comboBox2.Items[i]))
                        comboBox2.SelectedIndex = i;
                }
                radioButton1.Checked = true;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
            }
            if (Convert.ToInt32(selected[4].Substring(0, 1)) == 2)
            {
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if (selected[4].Substring(2, selected[4].IndexOf(">") - 2) == comboBox1.Items[i].ToString())
                        comboBox1.SelectedIndex = i;
                }
                numericUpDown1.Value = Convert.ToDecimal(selected[4].Substring(selected[4].IndexOf(">") + 1, selected[4].IndexOf("/", 2) - selected[4].IndexOf(">") - 1));
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (Convert.ToSingle(selected[4].Substring(selected[4].IndexOf("/", 2) + 1, selected[4].IndexOf(";") - selected[4].IndexOf("/", 2) - 1)) == Convert.ToSingle(comboBox2.Items[i]))
                        comboBox2.SelectedIndex = i;
                }
                for (int i = 0; i < comboBox3.Items.Count; i++)
                {
                    if (Convert.ToSingle(selected[4].Substring(selected[4].IndexOf(";") + 1, selected[4].Length - selected[4].IndexOf(";") - 1)) == Convert.ToSingle(comboBox3.Items[i]))
                        comboBox3.SelectedIndex = i;
                }
                radioButton1.Checked = false;
                radioButton2.Checked = true;
                radioButton3.Checked = false;
            }
            if (Convert.ToInt32(selected[4].Substring(0, 1)) == 3)
            {
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if (selected[4].Substring(2, selected[4].IndexOf("+") - 2) == comboBox1.Items[i].ToString())
                        comboBox1.SelectedIndex = i;
                }
                numericUpDown1.Value = Convert.ToDecimal(selected[4].Substring(selected[4].IndexOf("+") + 1, selected[4].IndexOf("/", 2) - selected[4].IndexOf("+") - 1));
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (Convert.ToSingle(selected[4].Substring(selected[4].IndexOf("/", 2) + 1, selected[4].Length - selected[4].IndexOf("/", 2) - 1)) == Convert.ToSingle(comboBox2.Items[i]))
                        comboBox2.SelectedIndex = i;
                }
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
            }
            image = selected[2];
        }
    }
}
