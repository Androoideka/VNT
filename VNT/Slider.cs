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
    public partial class Slider : Form
    {
        string fileName;
        bool playOrEdit;
        public Slider(string fName, bool openOrSave)
        {
            InitializeComponent();
            fileName = fName;
            playOrEdit = openOrSave;
        }
        private struct variables
        {
            public string[] name;
            public int[] value;
            public int numVars;
            public int countVariables(StreamReader sr)
            {
                sr.ReadLine();
                int i = 0;
                string x = sr.ReadLine();
                while (x.Length < 5 || x.Substring(0, 5) != "Slide")
                {
                    i++;
                    x = sr.ReadLine();
                }
                sr.BaseStream.Position = 0;
                sr.DiscardBufferedData();
                return i;
            }
            public void findVariables(StreamReader sr)
            {
                int count = countVariables(sr);
                name = new string[count];
                value = new int[count];
                sr.ReadLine();
                int i = 0;
                for(i = 0; i < count; i++)
                {
                    name[i] = sr.ReadLine();
                    value[i] = 0;
                }
                numVars = i;
                sr.BaseStream.Position = 0;
                sr.DiscardBufferedData();
            }
            public void findVariables(ListBox listeBarks)
            {
                name = new string[listeBarks.Items.Count];
                value = new int[listeBarks.Items.Count];
                for (int i = 0; i < listeBarks.Items.Count; i++)
                {
                    name[i] = listeBarks.Items[i].ToString();
                    value[i] = 0;
                }
            }
        }
        PictureBox[] pictureBoxs;
        Slide[] slides;
        private int slideNum(StreamReader sr)
        {
            sr.BaseStream.Position = 0;
            sr.DiscardBufferedData();
            string x;
            int n = 0;
            while (!sr.EndOfStream)
            {
                x = sr.ReadLine();
                if (x.Length > 5 && x.Substring(0, 5) == "Slide")
                    n++;
            }
            sr.BaseStream.Position = 0;
            sr.DiscardBufferedData();
            return n;
        }
        private void fillSlides(StreamReader sr, Slide[] slides)
        {
            for(int i = 0; i < slides.Length; i++)
            {
                string x = sr.ReadLine();
                slides[i].index = Convert.ToSingle(x.Substring(6, x.Length - 6));
                slides[i].pathBG = sr.ReadLine();
                slides[i].pbNum = Convert.ToInt32(sr.ReadLine());
                slides[i].info = new string[slides[i].pbNum, 5];
                for (int j = 0; j < slides[i].pbNum; j++)
                {
                    for (int k = 0; k < 5; k++)
                     
                        slides[i].info[j, k] = sr.ReadLine();
                }
            }
        }
        private void createSlides(StreamReader sr, out Slide[] slides)
        {
            int numSlides = slideNum(sr);
            slides = new Slide[numSlides];
            for (int i = 0; i < numSlides; i++)
                slides[i] = new Slide();
            sr.BaseStream.Position = 0;
            sr.DiscardBufferedData();
        }
        private void Slider_Load(object sender, EventArgs e)
        {
            StreamReader sr;
            vars = new variables();
            int numSlide;
            if (playOrEdit)
            {
                try
                {
                    sr = new StreamReader(fileName);
                    vars.findVariables(sr);
                    createSlides(sr, out slides);
                    string x = sr.ReadLine();
                    picsMaxNum = Convert.ToInt32(x);
                    pictureBoxs = new PictureBox[picsMaxNum];
                    x = sr.ReadLine();
                    for (int i = 0; i < vars.numVars - 1; i++)
                        x = sr.ReadLine();
                    fillSlides(sr, slides);
                    for (int i = 0; i < picsMaxNum; i++)
                    {
                        pictureBoxs[i] = new PictureBox();
                        pictureBoxs[i].Tag = i;
                        pictureBoxs[i].MouseClick += new MouseEventHandler(Clicky);
                        pictureBoxs[i].Paint += new PaintEventHandler(Painter);
                    }
                    sr.Close();
                    numericUpDown1.Value = 1;
                }
                catch { }
            }
            else
            {
                StartForm start = new StartForm();
                Slide[] tempSlides;
                try
                {
                    sr = new StreamReader(fileName);
                    createSlides(sr, out tempSlides);
                    int varNum = vars.countVariables(sr);
                    start.numOfSlides.Minimum = tempSlides.Length;
                    start.numOfPictures.Minimum = Convert.ToInt32(sr.ReadLine());
                    string x;
                    for (int i = 0; i < varNum; i++)
                    {
                        x = sr.ReadLine();
                        start.ListBox1.Items.Add(x);
                    }
                    fillSlides(sr, tempSlides);
                    sr.Close();
                }
                catch
                {
                    tempSlides = new Slide[0];
                }
                start.ShowDialog();
                vars.findVariables(start.ListBox1);
                numSlide = (int)start.numOfSlides.Value;
                picsMaxNum = (int)start.numOfPictures.Value;
                pictureBoxs = new PictureBox[picsMaxNum];
                for (int i = 0; i < picsMaxNum; i++)
                {
                    pictureBoxs[i] = new PictureBox();
                    pictureBoxs[i].SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBoxs[i].Tag = i;
                    pictureBoxs[i].Image = Image.FromFile(@"C:\Users\agasic14\Desktop\Projekt-2015-06-11\Projekt\Untitled.png");
                    pictureBoxs[i].MouseClick += new MouseEventHandler(Clicky);
                    pictureBoxs[i].Paint += new PaintEventHandler(Painter);
                    pictureBoxs[i].MouseEnter += new EventHandler(Hover);
                    pictureBoxs[i].MouseLeave += new EventHandler(HoverOut);
                }
                slides = new Slide[numSlide];
                for (int i = 0; i < numSlide; i++)
                {
                    slides[i] = new Slide();
                    slides[i].info = new string[picsMaxNum, 5];
                    if (i < tempSlides.Length)
                    {
                        slides[i].index = tempSlides[i].index;
                        slides[i].pathBG = tempSlides[i].pathBG;
                        slides[i].pbNum = tempSlides[i].pbNum;
                        for(int j = 0; j < tempSlides[i].pbNum; j++)
                        {
                            slides[i].info[j, 0] = tempSlides[i].info[j, 0];
                            slides[i].info[j, 1] = tempSlides[i].info[j, 1];
                            slides[i].info[j, 2] = tempSlides[i].info[j, 2];
                            slides[i].info[j, 3] = tempSlides[i].info[j, 3];
                            slides[i].info[j, 4] = tempSlides[i].info[j, 4];
                        }
                    }
                }
                numericUpDown1.Value = 1;
            }
        }
        private void Painter(object sender, PaintEventArgs e)
        {
            for (int i = 0; i < slides.Length; i++)
            {
                if(slides[i].index == Convert.ToSingle(numericUpDown1.Value))
                    e.Graphics.DrawString(slides[i].info[Convert.ToInt32((sender as PictureBox).Tag), 3], new Font("Arial", 10), Brushes.Black, 0, (sender as PictureBox).Height / 2 - (sender as PictureBox).Height / 20);
            }
        }
        private void Slider_MouseClick(object sender, MouseEventArgs e)
        {
            uradjeno = false;
            bool notEnoughSlides = false;
            if (!playOrEdit)
            {
                int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                if (slideRefer == -1)
                {
                    for (int i = (slides.Length - 1); i >= 0; i--)
                    {
                        if (slides[i].index == 0)
                            slideRefer = i;
                    }
                    if (slideRefer == -1)
                        notEnoughSlides = true;
                }
                if (slides[slideRefer].pbNum < picsMaxNum && !notEnoughSlides)
                {
                    pictureBoxs[slides[slideRefer].pbNum].Top = e.Y - pictureBoxs[slides[slideRefer].pbNum].Size.Height / 2;
                    pictureBoxs[slides[slideRefer].pbNum].Left = e.X - pictureBoxs[slides[slideRefer].pbNum].Size.Width / 2;
                    pictureBoxs[slides[slideRefer].pbNum].Width = ClientRectangle.Width / 6;
                    pictureBoxs[slides[slideRefer].pbNum].Height = ClientRectangle.Height / 6;
                    slides[slideRefer].index = Convert.ToSingle(numericUpDown1.Value);
                    slides[slideRefer].info[slides[slideRefer].pbNum, 0] = pictureBoxs[slides[slideRefer].pbNum].Top + "," + pictureBoxs[slides[slideRefer].pbNum].Left;
                    slides[slideRefer].info[slides[slideRefer].pbNum, 1] = pictureBoxs[slides[slideRefer].pbNum].Width + "," + pictureBoxs[slides[slideRefer].pbNum].Height;
                    if(slides[slideRefer].info[slides[slideRefer].pbNum, 2] == null)
                        slides[slideRefer].info[slides[slideRefer].pbNum, 2] = Application.StartupPath + @"\Untitled.png";
                    if(slides[slideRefer].info[slides[slideRefer].pbNum, 3] == null)
                        slides[slideRefer].info[slides[slideRefer].pbNum, 3] = "Text";
                    if(slides[slideRefer].info[slides[slideRefer].pbNum, 4] == null)
                        slides[slideRefer].info[slides[slideRefer].pbNum, 4] = "1/" + slides[slideRefer].index;
                    if (slides[slideRefer].pathBG == null)
                        slides[slideRefer].pathBG = Application.StartupPath + @"\Untitled.png";
                    pictureBoxs[slides[slideRefer].pbNum].Visible = true;
                    pictureBoxs[slides[slideRefer].pbNum].Enabled = true;
                    Controls.Add(pictureBoxs[slides[slideRefer].pbNum]);
                    pictureBoxs[slides[slideRefer].pbNum].Refresh();
                    slides[slideRefer].pbNum++;
                    uradjeno = true;
                }
                else
                {
                    MessageBox.Show("Too Many Slides/Pictures!");
                }
            }
        }
        variables vars;
        private int findSlide(Slide[] slides, float number)
        {
            for (int i = 0; i < slides.Length; i++)
            {
                if (slides[i].index == number)
                    return i;
            }
            return -1;
        }
        private void Clicky(object sender, MouseEventArgs e)
        {
            if (playOrEdit)
            {
                try
                {
                    int index = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                    string role = slides[index].info[(int)(sender as PictureBox).Tag, 4];
                    if (Convert.ToInt32(role.Substring(0, 1)) == 1)
                        numericUpDown1.Value = Convert.ToDecimal(role.Substring(2, role.Length - 2));
                    else if (Convert.ToInt32(role.Substring(0, 1)) == 2)
                    {
                        for (int i = 0; i < vars.numVars; i++)
                        {
                            if (vars.name[i] == role.Substring(2, role.IndexOf(">") - 2))
                            {
                                if (vars.value[i] > Convert.ToInt32(role.Substring(role.IndexOf(">") + 1, role.IndexOf("/", 2) - role.IndexOf(">") - 1)))
                                    numericUpDown1.Value = Convert.ToDecimal(role.Substring(role.IndexOf("/", 2) + 1, role.IndexOf(";") - role.IndexOf("/", 2) - 1));
                                else
                                    numericUpDown1.Value = Convert.ToDecimal(role.Substring(role.IndexOf(";") + 1, role.Length - role.IndexOf(";") - 1));
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < vars.numVars; i++)
                        {
                            if (vars.name[i] == role.Substring(2, role.IndexOf("+") - 2))
                                vars.value[i] += Convert.ToInt32(role.Substring(role.IndexOf("+") + 1, role.IndexOf("/", 2) - role.IndexOf("+") - 1));
                        }
                        numericUpDown1.Value = Convert.ToDecimal(role.Substring(role.IndexOf("/", 2) + 1, role.Length - role.IndexOf("/", 2) - 1));
                    }
                }
                catch { MessageBox.Show("Try again."); }
            }
            else
            {
                Tweaker twok = new Tweaker();
                twok.ShowDialog();
                int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                if (slideRefer == -1)
                {
                    for (int i = (slides.Length - 1); i >= 0; i--)
                    {
                        if (slides[i].index == 0)
                            slideRefer = i;
                    }
                }
                if (twok.fileName != null)
                {
                    slides[slideRefer].info[(int)(sender as PictureBox).Tag, 2] = twok.fileName;
                    (sender as PictureBox).Image = Image.FromFile(slides[slideRefer].info[(int)(sender as PictureBox).Tag, 2]);
                    if (twok.clickSetting != 1)
                        slides[slideRefer].info[(int)(sender as PictureBox).Tag, 4] = twok.clickSetting + "/" + twok.variableManip + "/" + twok.slideJmp;
                    else
                        slides[slideRefer].info[(int)(sender as PictureBox).Tag, 4] = twok.clickSetting + "/" + twok.slideJmp;
                }
            }
        }
        bool uradjeno = false;
        private void Hover(object sender, EventArgs e)
        {
            if (!playOrEdit)
            {
                textBox1.Enabled = true;
                textBox1.Visible = true;
                textBox1.Top = (sender as PictureBox).Top - 10;
                textBox1.Left = (sender as PictureBox).Top;
                textBox1.Focus();
            }
        }
        private void HoverOut(object sender, EventArgs e)
        {
            if (!playOrEdit && uradjeno == true)
            {
                textBox1.Enabled = false;
                textBox1.Visible = false;
                int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                slides[slideRefer].info[Convert.ToInt32((sender as PictureBox).Tag), 3] = textBox1.Text;
                (sender as PictureBox).Refresh();
            }
        }
        int picsMaxNum;
        private void Slider_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!playOrEdit)
            {
                bool notEnoughSlides = false;
                int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                if (slideRefer == -1)
                {
                    for (int i = (slides.Length - 1); i >= 0; i--)
                    {
                        if (slides[i].index == 0)
                            slideRefer = i;
                    }
                    if (slideRefer == -1)
                        notEnoughSlides = true;
                }
                if (!notEnoughSlides)
                {
                    if (e.KeyChar == (char)98)
                    {
                        if (openFileDialog1.ShowDialog() == DialogResult.OK)
                        {
                            slides[slideRefer].pathBG = openFileDialog1.FileName;
                            this.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
                        }
                    }
                    else if (e.KeyChar == (char)Keys.Delete)
                    {
                        slides[slideRefer].pbNum--;

                    }
                }
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (!playOrEdit)
            {
                StreamWriter sw = new StreamWriter(fileName);
                sw.Write(picsMaxNum);
                int i = 0;
                for (i = 0; i < vars.name.Length; i++)
                    sw.Write("\r\n" + vars.name[i]);
                i = 0;
                while(i < slides.Length && slides[i].index != 0)
                {
                    sw.Write("\r\n" + "Slide " + slides[i].index);
                    sw.Write("\r\n" + slides[i].pathBG);
                    sw.Write("\r\n" + slides[i].pbNum);
                    for (int j = 0; j < slides[i].pbNum; j++)
                    {
                        sw.Write("\r\n" + slides[i].info[j, 0]);
                        sw.Write("\r\n" + slides[i].info[j, 1]);
                        sw.Write("\r\n" + slides[i].info[j, 2]);
                        sw.Write("\r\n" + slides[i].info[j, 3]);
                        sw.Write("\r\n" + slides[i].info[j, 4]);
                    }
                    i++;
                }
                sw.Close();
            }
            Application.Exit();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int k = 0;
            int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
            for (int i = 0; i < slides.Length; i++)
            {
                slides[i - k] = slides[i];
                if (i == slideRefer)
                    k++;
            }
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int index = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                for (int i = 0; i < picsMaxNum; i++)
                {
                    if (i < slides[index].pbNum)
                    {
                        string location = slides[index].info[i, 0],
                            size = slides[index].info[i, 1],
                            path = slides[index].info[i, 2];
                        this.BackgroundImage = Image.FromFile(slides[index].pathBG);
                        pictureBoxs[i].Top = Convert.ToInt32(location.Substring(0, location.IndexOf(",")));
                        pictureBoxs[i].Left = Convert.ToInt32(location.Substring(location.IndexOf(",") + 1, location.Length - location.IndexOf(",") - 1));
                        pictureBoxs[i].Width = Convert.ToInt32(size.Substring(0, size.IndexOf(",")));
                        pictureBoxs[i].Height = Convert.ToInt32(size.Substring(size.IndexOf(",") + 1, size.Length - size.IndexOf(",") - 1));
                        pictureBoxs[i].Image = Image.FromFile(@path);
                        pictureBoxs[i].SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBoxs[i].Visible = true;
                        pictureBoxs[i].Enabled = true;
                        Controls.Add(pictureBoxs[i]);
                        pictureBoxs[i].Refresh();
                    }
                    else
                    {
                        pictureBoxs[i].Visible = false;
                        pictureBoxs[i].Enabled = false;
                    }
                }
            }
            catch
            {
                for (int i = 0; i < picsMaxNum; i++)
                {
                    this.BackgroundImage = null;
                    pictureBoxs[i].Visible = false;
                    pictureBoxs[i].Enabled = false;
                }
            }
        }
    }
}
