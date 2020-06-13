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
        public string var { get; set; }
        Slide[] slides;
        private string fileName { get; set; }
        public Tweaker(Slide[] slideList, string filePath)
        {
            InitializeComponent();
            slides = slideList;
            fileName = filePath;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (!File.Exists(Path.Combine(System.IO.Path.GetDirectoryName(fileName), openFileDialog1.SafeFileName)))
                    File.Copy(openFileDialog1.FileName, Path.Combine(Path.GetDirectoryName(fileName), openFileDialog1.SafeFileName));
            }
        }
        private void Tweaker_FormClosing(object sender, FormClosingEventArgs e)
        {
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

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            treeView1.
        }

        private void Tweaker_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
        }
    }
}
