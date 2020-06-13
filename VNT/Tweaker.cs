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
    public partial class Tweaker : Form
    {
        public Attributes setting { get; set; }
        public string var { get; set; }
        private string[] vars, slides;
        private string fileName { get; set; }
        public Tweaker(string[] slideList, string[] variables, Attributes attribute, string filePath)
        {
            InitializeComponent();
            slides = slideList;
            vars = variables;
            setting = attribute;
            fileName = filePath;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Focus();
            openFileDialog1.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!System.IO.File.Exists(Path.Combine(System.IO.Path.GetDirectoryName(fileName), openFileDialog1.SafeFileName)))
                    System.IO.File.Copy(openFileDialog1.FileName, Path.Combine(System.IO.Path.GetDirectoryName(fileName), openFileDialog1.SafeFileName));
            }
            setting.path = Path.Combine(System.IO.Path.GetDirectoryName(fileName), openFileDialog1.SafeFileName);
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked == true)
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
                    comboBox3.Enabled = false;
                }
                else if (radioButton3.Checked)
                {
                    comboBox1.Enabled = true;
                    numericUpDown1.Enabled = true;
                    comboBox3.Enabled = true;
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
        }
        private void Tweaker_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (radioButton1.Checked)
                setting = new Attributes(setting.position.X + "," + setting.position.Y, setting.size.X + "," + setting.size.Y, setting.path, "1/" + comboBox2.Items[comboBox2.SelectedIndex].ToString());
            else
            {
                var = @comboBox1.Text;
                if (radioButton2.Checked)
                    setting = new Attributes(setting.position.X + "," + setting.position.Y, setting.size.X + "," + setting.size.Y, setting.path, "2/" + @comboBox1.Text + "*" + (int)numericUpDown1.Value + "/" + comboBox2.Items[comboBox2.SelectedIndex].ToString());
                else if (radioButton3.Checked)
                    setting = new Attributes(setting.position.X + "," + setting.position.Y, setting.size.X + "," + setting.size.Y, setting.path, "3/" + @comboBox1.Text + "*" + (int)numericUpDown1.Value + "/" + comboBox2.Items[comboBox2.SelectedIndex].ToString() + ";" + comboBox3.Items[comboBox3.SelectedIndex].ToString());
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
            if (setting.type == 4)
            {
                radioButton1.Checked = false;
                radioButton3.Checked = false;
                radioButton2.Checked = false;
                radioButton4.Checked = true;
            }
            else
            {
                comboBox2.SelectedIndex = findIndex(comboBox2, setting.slides[0].ToString());
                if (setting.type == 1)
                {
                    radioButton1.Checked = true;
                    radioButton3.Checked = false;
                    radioButton2.Checked = false;
                    radioButton4.Checked = false;
                }
                else
                {
                    comboBox1.SelectedIndex = findIndex(comboBox1, setting.variables[0]);
                    numericUpDown1.Value = Convert.ToDecimal(setting.value);
                    if (setting.type == 3)
                    {
                        comboBox3.SelectedIndex = findIndex(comboBox3, setting.slides[1].ToString());
                        radioButton1.Checked = false;
                        radioButton3.Checked = true;
                        radioButton2.Checked = false;
                        radioButton4.Checked = false;
                    }
                    else
                    {
                        radioButton1.Checked = false;
                        radioButton3.Checked = false;
                        radioButton2.Checked = true;
                        radioButton4.Checked = false;
                    }
                }
            }
        }
    }
}
