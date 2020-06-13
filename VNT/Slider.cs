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
        private Bitmap setImage(string file)
        {
            Bitmap b = new Bitmap(Image.FromFile(file));
            b.MakeTransparent(Color.Gray);
            return b;
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
                if (slides[i].pbInfo.Count > max)
                    max = slides[i].pbInfo.Count;
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
                    start += 5 + slides[i].pbInfo.Count * 4;
                }
            }
            catch
            {
                MessageBox.Show("Incorrect format. Contact developer at andrej@gasic.rs");
            }
        }
        private void Slider_Load(object sender, EventArgs e)
        {
            if (!System.IO.File.Exists(Path.Combine(System.IO.Path.GetDirectoryName(fileName), "Default.png")))
                System.IO.File.Copy(Path.Combine(Application.StartupPath, "Default.png"), Path.Combine(Path.GetDirectoryName(fileName), "Default.png"));
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
                pictureBoxs[i].Image = setImage(Path.Combine(Application.StartupPath, "Default.png"));
            }
            if(!playOrEdit)
            {
                this.KeyPress += new KeyPressEventHandler(Slider_KeyPress);
                this.MouseDown += new MouseEventHandler(Slider_MouseDown);
                this.MouseMove += new MouseEventHandler(Slider_MouseMove);
                this.MouseUp += new MouseEventHandler(Slider_MouseUp);
            }
            numericUpDown1_ValueChanged(null, EventArgs.Empty);
        }
        private Attributes setupClick(Attributes currentSettings)
        {
            int slideRefer = findSlide(slides, Convert.ToDecimal(numericUpDown1.Value));
            List<string> slideList = new List<string>();
            List<string> variables = new List<string>();
            for (int i = 0; i < Math.Max(slides.Count, vars.Count); i++)
            {
                if (i < slides.Count)
                    slideList.Add(slides[i].index.ToString());
                if (i < vars.Count)
                    variables.Add(vars[i].name);
            }
            Tweaker twok = new Tweaker(slideList.ToArray(), variables.ToArray(), currentSettings, fileName);
            twok.ShowDialog();
            currentSettings = twok.setting;
            if (!String.IsNullOrWhiteSpace(twok.var) && varExists(vars, twok.var) == -1)
                vars.Add(new Variable(twok.var));
            return currentSettings;
        }
        private void lookAtVariables()
        {
            for (int i = 0; i < vars.Count; i++)
            {
                if (vars[i].IsUnused(slides))
                {
                    cleanUp(vars[i].name, slides);
                    vars.RemoveAt(i);
                }
            }
        }
        private int findSlide(List<Slide> slides, decimal number)
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
                int index = findSlide(slides, Convert.ToDecimal(numericUpDown1.Value));
                Attributes role = slides[index].pbInfo[(int)(sender as PictureBox).Tag];
                if (Convert.ToInt32(role.type) == 1)
                    numericUpDown1.Value = Convert.ToDecimal(role.slides[0]);
                else if (Convert.ToInt32(role.type) == 3)
                {
                    if (vars[varExists(vars, role.variables[0])].value > role.value)
                        numericUpDown1.Value = role.slides[0];
                    else
                        numericUpDown1.Value = role.slides[1];
                }
                else if (Convert.ToInt32(role.type) == 2)
                {
                    vars[varExists(vars, role.variables[0])].value += role.value;
                    numericUpDown1.Value = role.slides[0];
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
                int slideRefer = findSlide(slides, Convert.ToDecimal(numericUpDown1.Value));
                if (e.Button == MouseButtons.Left)
                {
                    slides[slideRefer].pbInfo[(int)(sender as PictureBox).Tag] = setupClick(slides[slideRefer].pbInfo[(int)(sender as PictureBox).Tag]);
                    (sender as PictureBox).Image = setImage(Path.Combine(Path.GetDirectoryName(fileName), slides[slideRefer].pbInfo[(int)(sender as PictureBox).Tag].path));
                }
                else
                {
                    slides[slideRefer].pbInfo.RemoveAt((int)(sender as PictureBox).Tag);
                    if (slides[slideRefer].pbInfo.Count == 0)
                        delete();
                }
                lookAtVariables();
            }
            numericUpDown1_ValueChanged(null, EventArgs.Empty);
        }
        private void cleanUp(string remove, List<Slide> slides)
        {
            for(int i = 0; i < slides.Count; i++)
            {
                for (int j = 0; j < slides[i].pbInfo.Count; j++)
                {
                    if (slides[i].pbInfo[j].type == 4)
                        slides[i].pbInfo[j].variables = findNDelete(remove, slides[i].pbInfo[j].variables);
                }
            }
        }
        private string[] findNDelete(string remove, string[] find)
        {
            string[] replacement = new string[find.Length - 1];
            int k = 0;
            for(int i = 0; i < find.Length; i++)
            {
                if (find[i] == remove)
                    replacement[i - k] = find[i];
                else
                    k++;
            }
            return replacement;
        }
        private void Slider_KeyPress(object sender, KeyPressEventArgs e)
        {
            int slideRefer = findSlide(slides, Convert.ToDecimal(numericUpDown1.Value));
            if (slideRefer == -1)
            {
                string[] info = new string[5] { "Slide " + numericUpDown1.Value, this.Location.X + "," + this.Location.Y, this.Size.Width + "," + this.Size.Height, null, "1/" + numericUpDown1.Value };
                slides.Add(new Slide(info, 0));
                slideRefer = slides.Count - 1;
            }
            if (e.KeyChar == 'b' || e.KeyChar == 'B')
            {
                slides[slideRefer].bgInfo = setupClick(slides[slideRefer].bgInfo);
                lookAtVariables();
                this.BackgroundImage = Image.FromFile(Path.Combine(Path.GetDirectoryName(fileName), slides[slideRefer].bgInfo.path));
            }
            else if (e.KeyChar == 's' || e.KeyChar == 'S')
            {
                save();
            }
            else if (e.KeyChar == (char)46)
                delete();
        }
        private void save()
        {
            StreamWriter sw = new StreamWriter(fileName);
            int i = 0;
            for (i = 0; i < vars.Count; i++)
                sw.WriteLine(vars[i].name);
            i = 0;
            while(i < slides.Count)
            {
                sw.WriteLine("Slide " + slides[i].index);
                sw.WriteLine(slides[i].bgInfo.position.X + "," + slides[i].bgInfo.position.Y);
                sw.WriteLine(slides[i].bgInfo.size.X + "," + slides[i].bgInfo.size.Y);
                sw.WriteLine(slides[i].bgInfo.path);
                if (i == slides.Count - 1 && slides[i].pbInfo.Count == 0)
                    sw.Write(slides[i].bgInfo.outputSetting(slides[i].bgInfo));
                else
                    sw.WriteLine(slides[i].bgInfo.outputSetting(slides[i].bgInfo));
                for (int j = 0; j < slides[i].pbInfo.Count; j++)
                {
                    sw.WriteLine(slides[i].pbInfo[j].position.X + "," + slides[i].pbInfo[j].position.Y);
                    sw.WriteLine(slides[i].pbInfo[j].size.X + "," + slides[i].pbInfo[j].size.Y);
                    sw.WriteLine(slides[i].pbInfo[j].path);
                    if (i == slides.Count - 1 && j == slides[i].pbInfo.Count - 1)
                        sw.Write(slides[i].pbInfo[j].outputSetting(slides[i].pbInfo[j]));
                    else
                        sw.WriteLine(slides[i].pbInfo[j].outputSetting(slides[i].pbInfo[j]));
                }
                i++;
            }
            sw.Close();
        }
        private void delete()
        {
            try
            {
                slides.RemoveAt(findSlide(slides, numericUpDown1.Value));
                numericUpDown1_ValueChanged(null, EventArgs.Empty);
            }
            catch { }
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                int slideRefer = findSlide(slides, numericUpDown1.Value);
                if (!String.IsNullOrWhiteSpace(slides[slideRefer].bgInfo.path))
                    this.BackgroundImage = Image.FromFile(Path.Combine(Path.GetDirectoryName(fileName), slides[slideRefer].bgInfo.path));
                else
                    this.BackgroundImage = null;
                for (int i = 0; i < pictureBoxs.Count; i++)
                {
                    if (i < slides[slideRefer].pbInfo.Count)
                    {
                        pictureBoxs[i].Location = slides[slideRefer].pbInfo[i].position;
                        pictureBoxs[i].Size = (Size)slides[slideRefer].pbInfo[i].size;
                        pictureBoxs[i].Image = setImage(Path.Combine(Path.GetDirectoryName(fileName), slides[slideRefer].pbInfo[i].path));
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
            int slideRefer = findSlide(slides, numericUpDown1.Value);
            if (slideRefer == -1)
            {
                string[] info = new string[5] { "Slide " + numericUpDown1.Value, this.Location.X + "," + this.Location.Y, this.Size.Width + "," + this.Size.Height, null, "1/" + numericUpDown1.Value };
                slides.Add(new Slide(info, 0));
                slideRefer = slides.Count - 1;
            }
            while (slides[slideRefer].pbInfo.Count >= pictureBoxs.Count)
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
            }
            slides[slideRefer].index = numericUpDown1.Value;
            anchor1 = e.X;
            anchor2 = e.Y;
            slides[slideRefer].pbInfo.Add(new Attributes(anchor1 + "," + anchor2, 0 + "," + 0, "Default.png", "1/" + slides[slideRefer].index));
            pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Image = setImage(Path.Combine(Application.StartupPath, "Default.png"));
            pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Size = (Size)new Point(0, 0);
            pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Location = new Point(anchor1, anchor2);
            pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Visible = true;
            pomeranje = true;
        }
        private void Slider_MouseUp(object sender, MouseEventArgs e)
        {
            pomeranje = false;
            int slideRefer = findSlide(slides, numericUpDown1.Value);
            try
            {
                slides[slideRefer].pbInfo[slides[slideRefer].pbInfo.Count - 1].position = pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Location;
                slides[slideRefer].pbInfo[slides[slideRefer].pbInfo.Count - 1].size = (Point)pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Size;
                pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Enabled = true;
            }
            catch { }
            this.Focus();
        }
        private void Slider_FormClosing(object sender, FormClosingEventArgs e)
        {
            save();
            Application.Exit();
        }
        private void Slider_MouseMove(object sender, MouseEventArgs e)
        {
            if(pomeranje)
            {
                int slideRefer = findSlide(slides, numericUpDown1.Value);
                pictureBoxs[slides[slideRefer].pbInfo.Count - 1].SuspendLayout();
                if (e.X <= pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Left)
                {
                    pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Width = anchor1 - e.X;
                    pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Left = e.X;
                }
                else
                {
                    pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Width = e.X - anchor1;
                    pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Left = anchor1;
                }
                if (e.Y <= pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Top)
                {
                    pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Height = anchor2 - e.Y;
                    pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Top = e.Y;
                }
                else
                {
                    pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Height = e.Y - anchor2;
                    pictureBoxs[slides[slideRefer].pbInfo.Count - 1].Top = anchor2;
                }
                pictureBoxs[slides[slideRefer].pbInfo.Count - 1].ResumeLayout();
            }
        }
    }
}
