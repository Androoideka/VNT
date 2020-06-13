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
        private void findVariables(string[] feed, out List<Variable> variables)
        {
            int count = 0;
            while (feed[count].Length < 5 || feed[count].Substring(0, 5) != "Slide")
                count++;
            variables = new List<Variable>(count);
            for (int i = 0; i < count; i++)
                variables.Add(new Variable(feed[i]));
        }
        List<Variable> vars;
        List<PictureBox> pictureBoxs;
        List<Slide> slides;
        private int slideNum(string[] feed)
        {
            int n = 0;
            for (int i = 0; i < feed.Length; i++)
            {
                if (feed[i].Length > 5 && feed[i].Substring(0, 5) == "Slide")
                    n++;
            }
            return n;
        }
        private string[] storeFromFile(StreamReader sr)
        {
            int i = 0;
            while (!sr.EndOfStream)
            {
                sr.ReadLine();
                i++;
            }
            sr.BaseStream.Position = 0;
            sr.DiscardBufferedData();
            string[] data = new string[i];
            i = 0;
            while (!sr.EndOfStream)
            {
                data[i] = sr.ReadLine();
                i++;
            }
            sr.Close();
            return data;
        }
        private int maxPictureBox(List<Slide> slides)
        {
            int max = 0;
            for(int i = 0; i < slides.Count; i++)
            {
                if (slides[i].pbNum > max)
                    max = slides[i].pbNum;
            }
            return max;
        }
        private bool varDoesntExist(List<Variable> variables, string name)
        {
            for (int i = 0; i < variables.Count; i++)
            {
                if (variables[i].name == name)
                    return false;
            }
            return true;
        }
        private void Slider_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.Magenta;
            this.TransparencyKey = Color.Magenta;
            this.BackgroundImage = Image.FromFile(Application.StartupPath + @"\Default.png");
            if (playOrEdit)
            {
                string[] gameData = storeFromFile(new StreamReader(fileName));
                pictureBoxs = new List<PictureBox>();
                findVariables(gameData, out vars);
                slides = new List<Slide>(slideNum(gameData));
                //try
                //{
                    int startInt = vars.Count;
                    for (int i = 0; i < slideNum(gameData); i++)
                    {
                        slides.Add(new Slide(gameData, startInt));
                        startInt += 3 + slides[i].pbNum * 5;
                    }
                /*}
                catch
                { MessageBox.Show("Incorrect format. Contact developer at andrej@gasic.rs"); }*/
                for (int i = 0; i < maxPictureBox(slides); i++)
                {
                    pictureBoxs.Add(new PictureBox());
                    pictureBoxs[i].Tag = i;
                    pictureBoxs[i].MouseClick += new MouseEventHandler(Clicky);
                    pictureBoxs[i].Paint += new PaintEventHandler(Painter);
                }
                try
                {
                    numericUpDown1.Value = Convert.ToDecimal(slides[0].index);
                }
                catch { }
            }
            else
            {
                List<Slide> tempSlides;
                try
                {
                    string[] gameData = storeFromFile(new StreamReader(fileName));
                    findVariables(gameData, out vars);
                    tempSlides = new List<Slide>(slideNum(gameData));
                    try
                    {
                        int startInt = vars.Count;
                        for (int i = 0; i < slideNum(gameData); i++)
                        {
                            slides.Add(new Slide(gameData, startInt));
                            startInt += 3 + slides[i].pbNum * 5;
                        }
                    }
                    catch
                    { MessageBox.Show("Incorrect format. Contact developer at andrej@gasic.rs"); }
                }
                catch
                {
                    tempSlides = new List<Slide>();
                    vars = new List<Variable>();
                }
                pictureBoxs = new List<PictureBox>();
                slides = new List<Slide>(tempSlides.Count);
                for (int i = 0; i < tempSlides.Count; i++)
                    slides[i] = tempSlides[i];
                for (int i = 0; i < maxPictureBox(tempSlides); i++)
                {
                    pictureBoxs[i] = new PictureBox();
                    pictureBoxs[i].SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBoxs[i].Tag = i;
                    Bitmap pic = new Bitmap(Image.FromFile(Application.StartupPath + @"\DefaultPic.png"));
                    pic.MakeTransparent(Color.White);
                    Controls.Add(pictureBoxs[i]);
                    pictureBoxs[i].Image = pic;
                    pictureBoxs[i].MouseClick += new MouseEventHandler(Clicky);
                    pictureBoxs[i].Paint += new PaintEventHandler(Painter);
                    pictureBoxs[i].MouseEnter += new EventHandler(Hover);
                    pictureBoxs[i].MouseLeave += new EventHandler(HoverOut);
                }
                button1.Enabled = true;
                button1.Visible = true;
                numericUpDown1.Value = 1;
            }
        }
        private void Painter(object sender, PaintEventArgs e)
        {
            int i = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
            if (i != -1)
                e.Graphics.DrawString(slides[i].info[Convert.ToInt32((sender as PictureBox).Tag)][3], new Font("Arial", 10), Brushes.Black, 0, (sender as PictureBox).Height / 2 - (sender as PictureBox).Height / 20);
            else
                MessageBox.Show("Something's up with the slide generation. Contact developer at andrej@gasic.rs");
        }
        private int findSlide(List<Slide> slides, float number)
        {
            for (int i = 0; i < slides.Count; i++)
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
                    string role = slides[index].info[(int)(sender as PictureBox).Tag][4];
                    if (Convert.ToInt32(role.Substring(0, 1)) == 1)
                        numericUpDown1.Value = Convert.ToDecimal(role.Substring(2, role.Length - 2));
                    else if (Convert.ToInt32(role.Substring(0, 1)) == 3)
                    {
                        for (int i = 0; i < vars.Count; i++)
                        {
                            if (vars[i].name == role.Substring(2, role.IndexOf(">") - 2))
                            {
                                if (vars[i].value > Convert.ToInt32(role.Substring(role.IndexOf(">") + 1, role.IndexOf("/", 2) - role.IndexOf(">") - 1)))
                                    numericUpDown1.Value = Convert.ToDecimal(role.Substring(role.IndexOf("/", 2) + 1, role.IndexOf(";") - role.IndexOf("/", 2) - 1));
                                else
                                    numericUpDown1.Value = Convert.ToDecimal(role.Substring(role.IndexOf(";") + 1, role.Length - role.IndexOf(";") - 1));
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < vars.Count; i++)
                        {
                            if (vars[i].name == role.Substring(2, role.IndexOf("+") - 2))
                                vars[i].value += Convert.ToInt32(role.Substring(role.IndexOf("+") + 1, role.IndexOf("/", 2) - role.IndexOf("+") - 1));
                        }
                        numericUpDown1.Value = Convert.ToDecimal(role.Substring(role.IndexOf("/", 2) + 1, role.Length - role.IndexOf("/", 2) - 1));
                    }
                }
                catch { MessageBox.Show("The maker of this was probably using an older version. Contact them."); }
            }
            else
            {
                int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                List<string> slideList = new List<string>();
                List<string> variables = new List<string>();
                for(int i = 0; i < slides.Count; i++)
                    slideList.Add(slides[i].index.ToString());
                for (int i = 0; i < vars.Count; i++)
                    variables.Add(vars[i].name);
                string path;
                if (slides[slideRefer].info[(int)(sender as PictureBox).Tag][2] != "DefaultPic.png")
                { path = fileName.Substring(0, fileName.LastIndexOf(@"\") + 1) + slides[slideRefer].info[(int)(sender as PictureBox).Tag][2]; }
                else
                { path = Application.StartupPath + @"\" + slides[slideRefer].info[(int)(sender as PictureBox).Tag][2]; }
                Tweaker twok = new Tweaker(slideList.ToArray(), variables.ToArray(), path, slides[slideRefer].info[(int)(sender as PictureBox).Tag][4]);
                twok.ShowDialog();
                if(varDoesntExist(vars, twok.var))
                    vars.Add(new Variable(twok.var));
                System.IO.File.Copy(twok.image, fileName.Substring(0, fileName.LastIndexOf(@"\")) + twok.image.Substring(twok.image.LastIndexOf(@"\"), twok.image.Length - twok.image.LastIndexOf(@"\")), true);
                Bitmap bmp = new Bitmap(Image.FromFile(twok.image));
                bmp.MakeTransparent(Color.White);
                (sender as PictureBox).Image = bmp;
                slides[slideRefer].info[(int)(sender as PictureBox).Tag][2] = twok.image.Substring(twok.image.LastIndexOf(@"\") + 1, twok.image.Length - 1 - twok.image.LastIndexOf(@"\"));
                slides[slideRefer].info[(int)(sender as PictureBox).Tag][4] = twok.setting;
                for(int i = 0; i < slides.Count; i++)
                {
                    if (vars[i].IsUnused(slides))
                        vars.RemoveAt(i);
                }
            }
        }
        private void Hover(object sender, EventArgs e)
        {
            pictar = Convert.ToInt32((sender as PictureBox).Tag);
            textBox1.Enabled = true;
            textBox1.Visible = true;
            textBox1.Text = slides[findSlide(slides, Convert.ToSingle(numericUpDown1.Value))].info[Convert.ToInt32((sender as PictureBox).Tag)][3];
            textBox1.Top = (sender as PictureBox).Top - 10;
            textBox1.Left = (sender as PictureBox).Top;
            textBox1.Focus();
        }
        private void HoverOut(object sender, EventArgs e)
        {
            pictar = -1;
            textBox1.Enabled = false;
            textBox1.Visible = false;
            int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
            slides[slideRefer].info[Convert.ToInt32((sender as PictureBox).Tag)][3] = textBox1.Text;
            (sender as PictureBox).Refresh();
        }
        int pictar;
        private void Slider_KeyPress(object sender, KeyPressEventArgs e)
        {
            int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
            if (slideRefer == -1)
            {
                slides.Add(new Slide());
                slideRefer = slides.Count - 1;
            }
            try
            {
                if (e.KeyChar == (char)Keys.Delete)
                {
                    slides[slideRefer].info.RemoveAt(pictar);
                    slides[slideRefer].pbNum--;
                    numericUpDown1.Value = Convert.ToDecimal(slides[slideRefer].index);
                }
            }
            catch { }
            if (!playOrEdit)
            {
                if (e.KeyChar == (char)98)
                {
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        slides[slideRefer].pathBG = openFileDialog1.FileName;
                        this.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
                    }
                }
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (!playOrEdit)
            {
                StreamWriter sw = new StreamWriter(fileName);
                int i = 0;
                for (i = 0; i < vars.Count; i++)
                    sw.WriteLine(vars[i].name);
                i = 0;
                while(i < slides.Count)
                {
                    sw.WriteLine("Slide " + slides[i].index);
                    sw.WriteLine(slides[i].pathBG);
                    sw.WriteLine(slides[i].pbNum);
                    for (int j = 0; j < slides[i].pbNum; j++)
                    {
                        sw.WriteLine(slides[i].info[j][0]);
                        sw.WriteLine(slides[i].info[j][1]);
                        sw.WriteLine(slides[i].info[j][2]);
                        sw.WriteLine(slides[i].info[j][3]);
                        if (i == slides.Count - 1 && j == slides[i].pbNum - 1)
                            sw.Write(slides[i].info[j][4]);
                        else
                            sw.WriteLine(slides[i].info[j][4]);
                    }
                    i++;
                }
                sw.Close();
            }
            Application.Exit();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                slides.RemoveAt(findSlide(slides, Convert.ToSingle(numericUpDown1.Value)));
            }
            catch { }
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int index = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                for (int i = 0; i < pictureBoxs.Count; i++)
                {
                    if (i < slides[index].pbNum)
                    {
                        string location = slides[index].info[i][0],
                            size = slides[index].info[i][1];
                        Bitmap b = new Bitmap(Image.FromFile(slides[index].info[i][2]));
                        b.MakeTransparent(Color.White);
                        this.BackColor = Color.Magenta;
                        this.TransparencyKey = Color.Magenta;
                        if (slides[index].pathBG != @"Default.png")
                            this.BackgroundImage = Image.FromFile(fileName.Substring(0, fileName.LastIndexOf(@"\") + 1) + slides[index].pathBG);
                        else
                            this.BackgroundImage = Image.FromFile(Application.StartupPath + @"\Default.png");
                        pictureBoxs[i].Top = Convert.ToInt32(location.Substring(0, location.IndexOf(",")));
                        pictureBoxs[i].Left = Convert.ToInt32(location.Substring(location.IndexOf(",") + 1, location.Length - location.IndexOf(",") - 1));
                        pictureBoxs[i].Width = Convert.ToInt32(size.Substring(0, size.IndexOf(",")));
                        pictureBoxs[i].Height = Convert.ToInt32(size.Substring(size.IndexOf(",") + 1, size.Length - size.IndexOf(",") - 1));
                        pictureBoxs[i].Image = b;
                        pictureBoxs[i].SizeMode = PictureBoxSizeMode.StretchImage;
                        pictureBoxs[i].Visible = true;
                        pictureBoxs[i].Enabled = true;
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
                this.BackgroundImage = Image.FromFile(Application.StartupPath + @"\Default.png");
                this.BackColor = Color.Magenta;
                this.TransparencyKey = Color.Magenta;
                for (int i = 0; i < pictureBoxs.Count; i++)
                {
                    pictureBoxs[i].Visible = false;
                    pictureBoxs[i].Enabled = false;
                }
            }
        }

        private void Slider_MouseDown(object sender, MouseEventArgs e)
        {
            if (!playOrEdit)
            {
                int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                if (slideRefer == -1)
                {
                    slides.Add(new Slide());
                    slideRefer = slides.Count - 1;
                }
                if (slides[slideRefer].pbNum >= pictureBoxs.Count)
                {
                    while (slides[slideRefer].pbNum >= pictureBoxs.Count)
                    {
                        pictureBoxs.Add(new PictureBox());
                        pictureBoxs[pictureBoxs.Count - 1].Tag = pictureBoxs.Count - 1;
                        Controls.Add(pictureBoxs[pictureBoxs.Count - 1]);
                    }
                }
                Bitmap b = new Bitmap(Image.FromFile(Application.StartupPath + @"\DefaultPic.png"));
                b.MakeTransparent(Color.White);
                pictureBoxs[slides[slideRefer].pbNum].Left = e.X;
                pictureBoxs[slides[slideRefer].pbNum].Top = e.Y;
                pictureBoxs[slides[slideRefer].pbNum].Image = b;
                slides[slideRefer].index = Convert.ToSingle(numericUpDown1.Value);
                slides[slideRefer].info.Add(new string[5]);
                slides[slideRefer].pathBG = "Default.png";
                slides[slideRefer].info[slides[slideRefer].pbNum][2] = "DefaultPic.png";
                slides[slideRefer].info[slides[slideRefer].pbNum][3] = "";
                slides[slideRefer].info[slides[slideRefer].pbNum][4] = "1/" + slides[slideRefer].index;
            }
        }

        private void Slider_MouseUp(object sender, MouseEventArgs e)
        {
            if (!playOrEdit)
            {
                int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                if (e.X < pictureBoxs[slides[slideRefer].pbNum].Left)
                {
                    pictureBoxs[slides[slideRefer].pbNum].Width = pictureBoxs[slides[slideRefer].pbNum].Left - e.X;
                    pictureBoxs[slides[slideRefer].pbNum].Left = e.X;
                }
                else
                    pictureBoxs[slides[slideRefer].pbNum].Width = e.X - pictureBoxs[slides[slideRefer].pbNum].Left;
                if (e.Y < pictureBoxs[slides[slideRefer].pbNum].Top)
                {
                    pictureBoxs[slides[slideRefer].pbNum].Height = pictureBoxs[slides[slideRefer].pbNum].Top - e.Y;
                    pictureBoxs[slides[slideRefer].pbNum].Top = e.Y;
                }
                else
                    pictureBoxs[slides[slideRefer].pbNum].Height = e.Y - pictureBoxs[slides[slideRefer].pbNum].Top;
                slides[slideRefer].info[slides[slideRefer].pbNum][0] = pictureBoxs[slides[slideRefer].pbNum].Top + "," + pictureBoxs[slides[slideRefer].pbNum].Left;
                slides[slideRefer].info[slides[slideRefer].pbNum][1] = pictureBoxs[slides[slideRefer].pbNum].Width + "," + pictureBoxs[slides[slideRefer].pbNum].Height;
                pictureBoxs[slides[slideRefer].pbNum].MouseClick += new MouseEventHandler(Clicky);
                pictureBoxs[slides[slideRefer].pbNum].Paint += new PaintEventHandler(Painter);
                pictureBoxs[slides[slideRefer].pbNum].MouseEnter += new EventHandler(Hover);
                pictureBoxs[slides[slideRefer].pbNum].MouseLeave += new EventHandler(HoverOut);
                pictureBoxs[slides[slideRefer].pbNum].Refresh();
                Controls.Add(pictureBoxs[slides[slideRefer].pbNum]);
                pictureBoxs[slides[slideRefer].pbNum].Visible = true;
                pictureBoxs[slides[slideRefer].pbNum].Enabled = true;
                slides[slideRefer].pbNum++;
            }
        }
    }
}
