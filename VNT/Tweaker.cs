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
            this.Focus();
            openFileDialog1.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                image = openFileDialog1.FileName;
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                comboBox1.Enabled = false;
                numericUpDown1.Enabled = false;
                comboBox3.Enabled = false;
            }
            else if (radioButton2.Checked)
            {
                comboBox1.Enabled = true;
                numericUpDown1.Enabled = true;
                comboBox3.Enabled = true;
                if(comboBox3.SelectedIndex == -1)
                    comboBox3.SelectedIndex = comboBox2.SelectedIndex;
            }
            else if (radioButton3.Checked)
            {
                comboBox1.Enabled = true;
                numericUpDown1.Enabled = true;
                comboBox3.Enabled = false;
            }
            else
            {
                comboBox1.Enabled = false;
                numericUpDown1.Enabled = false;
                comboBox2.Enabled = false;
                comboBox3.Enabled = false;
                VarTrack varSetter = new VarTrack(vars, new int[vars.Length], setting, false);
                varSetter.ShowDialog();
                setting = varSetter.config;
            }
        }
        private void Tweaker_FormClosing(object sender, FormClosingEventArgs e)
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
                else if (radioButton3.Checked)
                    setting = "2/" + @comboBox1.Items[comboBox1.SelectedIndex] + @"+" + (int)numericUpDown1.Value + @"/" + comboBox2.Items[comboBox2.SelectedIndex];
            }
        }
        private int findIndex(ComboBox cb, string compare)
        {
            for(int i = 0; i < cb.Items.Count; i++)
            {
                if (compare == cb.Items[i].ToString())
                    return i;
            }
            return 0;
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
                /*for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (Convert.ToSingle(setting.Substring(2, setting.Length - 2)) == Convert.ToSingle(comboBox2.Items[i]))
                        comboBox2.SelectedIndex = i;
                }*/
                comboBox2.SelectedIndex = findIndex(comboBox2, setting.Substring(2, setting.Length - 2));
                radioButton1.Checked = true;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
            }
            else if (Convert.ToInt32(setting.Substring(0, 1)) == 3)
            {
                /*for (int i = 0; i < comboBox1.Items.Count; i++)
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
                }*/
                comboBox1.SelectedIndex = findIndex(comboBox1, setting.Substring(2, setting.IndexOf(">") - 2));
                comboBox2.SelectedIndex = findIndex(comboBox2, setting.Substring(setting.IndexOf("/", 2) + 1, setting.IndexOf(";") - setting.IndexOf("/", 2) - 1));
                comboBox3.SelectedIndex = findIndex(comboBox3, setting.Substring(setting.IndexOf(";") + 1, setting.Length - setting.IndexOf(";") - 1));
                numericUpDown1.Value = Convert.ToDecimal(setting.Substring(setting.IndexOf(">") + 1, setting.IndexOf("/", 2) - setting.IndexOf(">") - 1));
                radioButton1.Checked = false;
                radioButton2.Checked = true;
                radioButton3.Checked = false;
                radioButton4.Checked = false;
            }
            else if (Convert.ToInt32(setting.Substring(0, 1)) == 2)
            {
                /*for (int i = 0; i < comboBox1.Items.Count; i++)
                {
                    if (setting.Substring(2, setting.IndexOf("+") - 2) == comboBox1.Items[i].ToString())
                        comboBox1.SelectedIndex = i;
                }
                for (int i = 0; i < comboBox2.Items.Count; i++)
                {
                    if (Convert.ToSingle(setting.Substring(setting.IndexOf("/", 2) + 1, setting.Length - setting.IndexOf("/", 2) - 1)) == Convert.ToSingle(comboBox2.Items[i]))
                        comboBox2.SelectedIndex = i;
                }*/
                comboBox1.SelectedIndex = findIndex(comboBox1, setting.Substring(2, setting.IndexOf("+") - 2));
                comboBox2.SelectedIndex = findIndex(comboBox2, setting.Substring(setting.IndexOf("/", 2) + 1, setting.Length - setting.IndexOf("/", 2) - 1));
                numericUpDown1.Value = Convert.ToDecimal(setting.Substring(setting.IndexOf("+") + 1, setting.IndexOf("/", 2) - setting.IndexOf("+") - 1));
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = true;
                radioButton4.Checked = false;
            }
            else if (Convert.ToInt32(setting.Substring(0, 1)) == 4)
            {
                radioButton1.Checked = false;
                radioButton2.Checked = false;
                radioButton3.Checked = false;
                radioButton4.Checked = true;
            }
        }
    }
}
