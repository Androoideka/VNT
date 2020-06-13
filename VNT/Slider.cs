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
        int anchor1, anchor2;
        string fileName;
        bool playOrEdit, pomeranje = false;
        List<Variable> vars = new List<Variable>();
        List<PictureBox> pictureBoxs = new List<PictureBox>();
        List<Slide> slides = new List<Slide>();
        public Slider(string fName, bool openOrSave)
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            fileName = fName;
            playOrEdit = openOrSave;
        }
        private void setImage(PictureBox pictureBex, string file)
        {
            Bitmap b = new Bitmap(Image.FromFile(file));
            b.MakeTransparent(Color.Gray);
            pictureBex.Image = b;
        }
        private void findVariables(string[] feed, List<Variable> variables)
        {
            int i = 0;
            while (feed[i].Length < 5 || feed[i].Substring(0, 5) != "Slide")
            {
                variables.Add(new Variable(feed[i]));
                i++;
            }
        }
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
        private string[] storeFromFile(string file)
        {
            StreamReader sr = new StreamReader(file);
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
                if (slides[i].info.Count > max)
                    max = slides[i].info.Count;
            }
            return max;
        }
        private int varExists(List<Variable> variables, string name)
        {
            for (int i = 0; i < variables.Count; i++)
            {
                if (variables[i].name == name)
                    return i;
            }
            return -1;
        }
        private void fillSlides(List<Slide> slides, int start, string[] feed)
        {
            try
            {
                for (int i = 0; i < slideNum(feed); i++)
                {
                    slides.Add(new Slide(feed, start));
                    start += 2 + slides[i].info.Count * 4;
                }
            }
            catch
            {
                MessageBox.Show("Incorrect format. Contact developer at andrej@gasic.rs");
            }
        }
        private void Slider_Load(object sender, EventArgs e)
        {
            try
            {
                string[] gameData = storeFromFile(fileName);
                findVariables(gameData, vars);
                fillSlides(slides, vars.Count, gameData);
            }
            catch { }
            for (int i = 0; i < maxPictureBox(slides); i++)
            {
                pictureBoxs.Add(new PictureBox());
                Controls.Add(pictureBoxs[i]);
                pictureBoxs[i].Tag = i;
                pictureBoxs[i].SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBoxs[i].MouseClick += new MouseEventHandler(Clicky);
                pictureBoxs[i].BackColor = Color.Transparent;
                pictureBoxs[i].Parent = this;
                pictureBoxs[i].Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top);
                setImage(pictureBoxs[i], Application.StartupPath + @"\Default.png");
            }
            if(!playOrEdit)
            {
                this.KeyPress += new KeyPressEventHandler(Slider_KeyPress);
                this.MouseDown += new MouseEventHandler(Slider_MouseDown);
                this.MouseMove += new MouseEventHandler(Slider_MouseMove);
                this.MouseUp += new MouseEventHandler(Slider_MouseUp);
                button1.Enabled = true;
                button1.Visible = true;
                button2.Enabled = true;
                button2.Visible = true;
            }
            numericUpDown1_ValueChanged(null, EventArgs.Empty);
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
                int index = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                string role = slides[index].info[(int)(sender as PictureBox).Tag][3];
                if (Convert.ToInt32(role.Substring(0, 1)) == 1)
                    numericUpDown1.Value = Convert.ToDecimal(role.Substring(2, role.Length - 2));
                else if (Convert.ToInt32(role.Substring(0, 1)) == 3)
                {
                    if (vars[varExists(vars, role.Substring(2, role.IndexOf(">") - 2))].value > Convert.ToInt32(role.Substring(role.IndexOf(">") + 1, role.IndexOf("/", 2) - role.IndexOf(">") - 1)))
                        numericUpDown1.Value = Convert.ToDecimal(role.Substring(role.IndexOf("/", 2) + 1, role.IndexOf(";") - role.IndexOf("/", 2) - 1));
                    else
                        numericUpDown1.Value = Convert.ToDecimal(role.Substring(role.IndexOf(";") + 1, role.Length - role.IndexOf(";") - 1));
                }
                else if (Convert.ToInt32(role.Substring(0, 1)) == 2)
                {
                    vars[varExists(vars, role.Substring(2, role.IndexOf("+") - 2))].value += Convert.ToInt32(role.Substring(role.IndexOf("+") + 1, role.IndexOf("/", 2) - role.IndexOf("+") - 1));
                    numericUpDown1.Value = Convert.ToDecimal(role.Substring(role.IndexOf("/", 2) + 1, role.Length - role.IndexOf("/", 2) - 1));
                }
                else
                {
                    string[] names = new string[vars.Count];
                    int[] values = new int[vars.Count];
                    for(int i = 0; i < vars.Count; i++)
                    {
                        names[i] = vars[i].name;
                        values[i] = vars[i].value;
                    }
                    VarTrack varValues = new VarTrack(names, values, role, true);
                    varValues.ShowDialog();
                }
            }
            else
            {
                int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                if (e.Button == MouseButtons.Left)
                {
                    List<string> slideList = new List<string>();
                    List<string> variables = new List<string>();
                    for (int i = 0; i < Math.Max(slides.Count, vars.Count); i++)
                    {
                        if(i < slides.Count)
                            slideList.Add(slides[i].index.ToString());
                        if(i < vars.Count)
                            variables.Add(vars[i].name);
                    }
                    string path;
                    if (slides[slideRefer].info[(int)(sender as PictureBox).Tag][2] != "Default.png")
                    { path = fileName.Substring(0, fileName.LastIndexOf(@"\") + 1) + slides[slideRefer].info[(int)(sender as PictureBox).Tag][2]; }
                    else
                    { path = Application.StartupPath + @"\" + slides[slideRefer].info[(int)(sender as PictureBox).Tag][2]; }
                    Tweaker twok = new Tweaker(slideList.ToArray(), variables.ToArray(), path, slides[slideRefer].info[(int)(sender as PictureBox).Tag][3]);
                    twok.ShowDialog();
                    slides[slideRefer].info[(int)(sender as PictureBox).Tag][2] = twok.image.Substring(twok.image.LastIndexOf(@"\") + 1, twok.image.Length - 1 - twok.image.LastIndexOf(@"\"));
                    slides[slideRefer].info[(int)(sender as PictureBox).Tag][3] = twok.setting;
                    if (varExists(vars, twok.var) == -1)
                        vars.Add(new Variable(twok.var));
                    try
                    { System.IO.File.Copy(twok.image, fileName.Substring(0, fileName.LastIndexOf(@"\")) + twok.image.Substring(twok.image.LastIndexOf(@"\"), twok.image.Length - twok.image.LastIndexOf(@"\")), true); }
                    catch { }
                    setImage((sender as PictureBox), twok.image);
                    for (int i = 0; i < vars.Count; i++)
                    {
                        if (vars[i].IsUnused(slides))
                        {
                            cleanUp(vars[i].name, slides);
                            vars.RemoveAt(i);
                        }
                    }
                }
                else
                {
                    slides[slideRefer].info.RemoveAt((int)(sender as PictureBox).Tag);
                    for (int i = 0; i < vars.Count; i++)
                    {
                        if (vars[i].IsUnused(slides))
                        {
                            cleanUp(vars[i].name, slides);
                            vars.RemoveAt(i);
                        }
                    }
                    if (slides[slideRefer].info.Count == 0)
                        button1_Click(null, EventArgs.Empty);
                }
            }
            numericUpDown1_ValueChanged(null, EventArgs.Empty);
        }
        private void cleanUp(string remove, List<Slide> slides)
        {
            for(int i = 0; i < slides.Count; i++)
            {
                for (int j = 0; j < slides[i].info.Count; j++)
                {
                    if (Convert.ToInt32(slides[i].info[j][3].Substring(0,1)) == 4)
                        slides[i].info[j][3] = findNDelete(remove, slides[i].info[j][3]);
                }
            }
        }
        private string findNDelete(string remove, string find)
        {
            int i = 2;
            while (find.IndexOf(";", i) != -1)
            {
                if (find.Substring(i, find.IndexOf(";", i) - i) == remove)
                {
                    find = find.Remove(i, find.IndexOf(";", i) - i + 1);
                    return find;
                }
                i = find.IndexOf(";", i) + 1;
            }
            return find;
        }
        private void Slider_KeyPress(object sender, KeyPressEventArgs e)
        {
            int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
            if (slideRefer == -1)
            {
                slides.Add(new Slide());
                slideRefer = slides.Count - 1;
            }
            if (!playOrEdit)
            {
                if (e.KeyChar == (char)98)
                {
                    openFileDialog1.Filter = "Image Files(*.BMP; *.JPG; *.GIF; *.PNG)| *.BMP; *.JPG; *.GIF; *.PNG";
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        slides[slideRefer].pathBG = openFileDialog1.FileName.Substring(openFileDialog1.FileName.LastIndexOf(@"\") + 1, openFileDialog1.FileName.Length - openFileDialog1.FileName.LastIndexOf(@"\") - 1);
                        this.BackgroundImage = Image.FromFile(openFileDialog1.FileName);
                        try
                        { System.IO.File.Copy(openFileDialog1.FileName, fileName.Substring(0, fileName.LastIndexOf(@"\")) + slides[slideRefer].pathBG, true); }
                        catch { }
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
                    for (int j = 0; j < slides[i].info.Count; j++)
                    {
                        sw.WriteLine(slides[i].info[j][0]);
                        sw.WriteLine(slides[i].info[j][1]);
                        sw.WriteLine(slides[i].info[j][2]);
                        if (i == slides.Count - 1 && j == slides[i].info.Count - 1)
                            sw.Write(slides[i].info[j][3]);
                        else
                            sw.WriteLine(slides[i].info[j][3]);
                    }
                    i++;
                }
                sw.Close();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                slides.RemoveAt(findSlide(slides, Convert.ToSingle(numericUpDown1.Value)));
                numericUpDown1_ValueChanged(null, EventArgs.Empty);
            }
            catch { }
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int index = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                if (!String.IsNullOrWhiteSpace(slides[index].pathBG))
                    this.BackgroundImage = Image.FromFile(fileName.Substring(0, fileName.LastIndexOf(@"\") + 1) + slides[index].pathBG);
                else
                    this.BackgroundImage = null;
                for (int i = 0; i < pictureBoxs.Count; i++)
                {
                    if (i < slides[index].info.Count)
                    {
                        string location = slides[index].info[i][0],
                            size = slides[index].info[i][1];
                        pictureBoxs[i].Top = Convert.ToInt32(location.Substring(0, location.IndexOf(",")));
                        pictureBoxs[i].Left = Convert.ToInt32(location.Substring(location.IndexOf(",") + 1, location.Length - location.IndexOf(",") - 1));
                        pictureBoxs[i].Width = Convert.ToInt32(size.Substring(0, size.IndexOf(",")));
                        pictureBoxs[i].Height = Convert.ToInt32(size.Substring(size.IndexOf(",") + 1, size.Length - size.IndexOf(",") - 1));
                        setImage(pictureBoxs[i], fileName.Substring(0, fileName.LastIndexOf(@"\") + 1) + slides[index].info[i][2]);
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
                this.BackgroundImage = null;
                for (int i = 0; i < pictureBoxs.Count; i++)
                {
                    pictureBoxs[i].Visible = false;
                    pictureBoxs[i].Enabled = false;
                }
            }
        }
        private void Slider_MouseDown(object sender, MouseEventArgs e)
        {
            int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
            if (slideRefer == -1)
            {
                slides.Add(new Slide());
                slideRefer = slides.Count - 1;
                slides[slideRefer].pathBG = null;
            }
            while (slides[slideRefer].info.Count >= pictureBoxs.Count)
            {
                pictureBoxs.Add(new PictureBox());
                pictureBoxs[pictureBoxs.Count - 1].Tag = pictureBoxs.Count - 1;
                pictureBoxs[pictureBoxs.Count - 1].SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBoxs[pictureBoxs.Count - 1].BackColor = Color.Transparent;
                pictureBoxs[pictureBoxs.Count - 1].Parent = this;
                pictureBoxs[pictureBoxs.Count - 1].Anchor = (AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top);
                pictureBoxs[pictureBoxs.Count - 1].MouseClick += Clicky;
                pictureBoxs[pictureBoxs.Count - 1].Enabled = false;
                Controls.Add(pictureBoxs[pictureBoxs.Count - 1]);
                setImage(pictureBoxs[pictureBoxs.Count - 1], Application.StartupPath + @"\Default.png");
            }
            slides[slideRefer].index = Convert.ToSingle(numericUpDown1.Value);
            slides[slideRefer].info.Add(new string[4]);
            slides[slideRefer].info[slides[slideRefer].info.Count - 1][2] = "Default.png";
            slides[slideRefer].info[slides[slideRefer].info.Count - 1][3] = "1/" + slides[slideRefer].index;
            anchor1 = e.X;
            anchor2 = e.Y;
            pictureBoxs[slides[slideRefer].info.Count - 1].Width = 0;
            pictureBoxs[slides[slideRefer].info.Count - 1].Height = 0;
            pictureBoxs[slides[slideRefer].info.Count - 1].Left = anchor1;
            pictureBoxs[slides[slideRefer].info.Count - 1].Top = anchor2;
            pictureBoxs[slides[slideRefer].info.Count - 1].Visible = true;
            pomeranje = true;
        }
        private void Slider_MouseUp(object sender, MouseEventArgs e)
        {
            pomeranje = false;
            int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
            try
            {
                slides[slideRefer].info[slides[slideRefer].info.Count - 1][0] = pictureBoxs[slides[slideRefer].info.Count - 1].Top + "," + pictureBoxs[slides[slideRefer].info.Count - 1].Left;
                slides[slideRefer].info[slides[slideRefer].info.Count - 1][1] = pictureBoxs[slides[slideRefer].info.Count - 1].Width + "," + pictureBoxs[slides[slideRefer].info.Count - 1].Height;
                pictureBoxs[slides[slideRefer].info.Count - 1].Enabled = true;
            }
            catch { }
        }
        private void Slider_FormClosing(object sender, FormClosingEventArgs e)
        {
            button2_Click(null, EventArgs.Empty);
            Application.Restart();
        }
        private void Slider_MouseMove(object sender, MouseEventArgs e)
        {
            if(pomeranje)
            {
                int slideRefer = findSlide(slides, Convert.ToSingle(numericUpDown1.Value));
                pictureBoxs[slides[slideRefer].info.Count - 1].SuspendLayout();
                if (e.X <= pictureBoxs[slides[slideRefer].info.Count - 1].Left)
                {
                    pictureBoxs[slides[slideRefer].info.Count - 1].Width = anchor1 - e.X;
                    pictureBoxs[slides[slideRefer].info.Count - 1].Left = e.X;
                }
                else
                {
                    pictureBoxs[slides[slideRefer].info.Count - 1].Width = e.X - anchor1;
                    pictureBoxs[slides[slideRefer].info.Count - 1].Left = anchor1;
                }
                if (e.Y <= pictureBoxs[slides[slideRefer].info.Count - 1].Top)
                {
                    pictureBoxs[slides[slideRefer].info.Count - 1].Height = anchor2 - e.Y;
                    pictureBoxs[slides[slideRefer].info.Count - 1].Top = e.Y;
                }
                else
                {
                    pictureBoxs[slides[slideRefer].info.Count - 1].Height = e.Y - anchor2;
                    pictureBoxs[slides[slideRefer].info.Count - 1].Top = anchor2;
                }
                pictureBoxs[slides[slideRefer].info.Count - 1].ResumeLayout();
            }
        }
    }
}
