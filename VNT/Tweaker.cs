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
        public string image { get; set; }
        public string setting { get; set; }
        public string var { get; set; }
        private string[] vars, slides;
        public Tweaker(string[] slideList, string[] variables, string img, string info)
        {
            InitializeComponent();
            slides = slideList;
            vars = variables;
            setting = info;
            image = img;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG";
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
                if(comboBox3.SelectedIndex == -1)
                    comboBox3.SelectedIndex = comboBox2.SelectedIndex;
            }
            if (radioButton3.Checked)
            {
                label1.Text = "Add:";
                comboBox1.Enabled = true;
                numericUpDown1.Enabled = true;
                comboBox3.Enabled = false;
            }
        }
        private void Tweaker_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                if (radioButton1.Checked)
                    setting = "1/" + comboBox2.Items[comboBox2.SelectedIndex];
                else
                {
                    for (int i = 0; i < comboBox1.Items.Count; i++)
                    {
                        if (comboBox1.Items[i].ToString() == @comboBox1.Text)
                            comboBox1.SelectedIndex = i;
                    }
                    if (comboBox1.SelectedIndex == -1)
                    {
                        comboBox1.Items.Add(@comboBox1.Text);
                        comboBox1.SelectedIndex = comboBox1.Items.Count - 1;
                    }
                    var = comboBox1.Items[comboBox1.SelectedIndex].ToString();
                    if (radioButton2.Checked)
                        setting = "3/" + @comboBox1.Items[comboBox1.SelectedIndex] + @">" + (int)numericUpDown1.Value + @"/" + comboBox2.Items[comboBox2.SelectedIndex] + @";" + comboBox3.Items[comboBox3.SelectedIndex];
                    if (radioButton3.Checked)
                        setting = "2/" + @comboBox1.Items[comboBox1.SelectedIndex] + @"+" + (int)numericUpDown1.Value + @"/" + comboBox2.Items[comboBox2.SelectedIndex];
                }
                this.Close();
            }
        }
        private void Tweaker_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            for (int i = 0; i < Math.Max(slides.Length, vars.Length); i++)
            {
                if (i < vars.Length)
                    comboBox1.Items.Add(@vars[i]);
                if (i < slides.Length)
                {
                    comboBox2.Items.Add(slides[i]);
                    comboBox3.Items.Add(slides[i]);
                }
            }
            if (Convert.ToInt32(setting.Substring(0, 1)) == 1)
            {
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (Convert.ToSingle(setting.Substring(2, setting.Length - 2)) == Convert.ToSingle(comboBox2.Items[i]))
                        comboBox2.SelectedIndex = i;
                }
                radioButton1.Checked = true;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
            }
            if (Convert.ToInt32(setting.Substring(0, 1)) == 3)
            {
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if (setting.Substring(2, setting.IndexOf(">") - 2) == comboBox1.Items[i].ToString())
                        comboBox1.SelectedIndex = i;
                }
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (Convert.ToSingle(setting.Substring(setting.IndexOf("/", 2) + 1, setting.IndexOf(";") - setting.IndexOf("/", 2) - 1)) == Convert.ToSingle(comboBox2.Items[i]))
                        comboBox2.SelectedIndex = i;
                }
                for (int i = 0; i < comboBox3.Items.Count; i++)
                {
                    if (Convert.ToSingle(setting.Substring(setting.IndexOf(";") + 1, setting.Length - setting.IndexOf(";") - 1)) == Convert.ToSingle(comboBox3.Items[i]))
                        comboBox3.SelectedIndex = i;
                }
                numericUpDown1.Value = Convert.ToDecimal(setting.Substring(setting.IndexOf(">") + 1, setting.IndexOf("/", 2) - setting.IndexOf(">") - 1));
                radioButton1.Checked = false;
                radioButton2.Checked = true;
                radioButton3.Checked = false;
            }
            if (Convert.ToInt32(setting.Substring(0, 1)) == 2)
            {
                for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if (setting.Substring(2, setting.IndexOf("+") - 2) == comboBox1.Items[i].ToString())
                        comboBox1.SelectedIndex = i;
                }
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (Convert.ToSingle(setting.Substring(setting.IndexOf("/", 2) + 1, setting.Length - setting.IndexOf("/", 2) - 1)) == Convert.ToSingle(comboBox2.Items[i]))
                        comboBox2.SelectedIndex = i;
                }
                numericUpDown1.Value = Convert.ToDecimal(setting.Substring(setting.IndexOf("+") + 1, setting.IndexOf("/", 2) - setting.IndexOf("+") - 1));
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
            }
        }
    }
}
