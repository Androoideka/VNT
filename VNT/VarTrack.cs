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
    public partial class VarTrack : Form
    {
        Variable[] vars;
        public string config { get; set; }
        bool playMode;
        public VarTrack(string[] variables, int[] variableValues, string setting, bool playOrEdit)
        {
            InitializeComponent();
            vars = new Variable[variables.Length];
            for(int i = 0; i < variables.Length; i++)
            {
                vars[i] = new Variable(variables[i]);
                vars[i].value = variableValues[i];
            }
            playMode = playOrEdit;
            config = setting;
        }
        private void processCommand(Variable[] variables, string setting, bool playMode)
        {
            int i = 2;
            int j = 0;
            while(setting.IndexOf(";", i) != -1)
            {
                j = findAddVariable(j, setting.Substring(i, setting.IndexOf(";", i) - i), variables);
                if (playMode)
                    listBox1.Items.Add(variables[j].name + " = " + variables[j].value);
                else
                    checkedListBox1.SetItemChecked(j, true);
                i = setting.IndexOf(";", i) + 1;
            }
        }
        private int findAddVariable(int i, string name, Variable[] variables)
        {
            for(; i < variables.Length; i++)
            {
                if (variables[i].name == name)
                    return i;
            }
            return -1;
        }
        private void VarTrack_Load(object sender, EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            if (playMode)
            {
                listBox1.Enabled = true;
                listBox1.Visible = true;
            }
            else
            {
                checkedListBox1.Enabled = true;
                checkedListBox1.Visible = true;
                checkedListBox1.CheckOnClick = true;
                for (int i = 0; i < vars.Length; i++)
                    checkedListBox1.Items.Add(vars[i].name);
            }
            processCommand(vars, config, playMode);
        }
        private void VarTrack_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!playMode)
            {
                config = "4/";
                for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                    config += checkedListBox1.CheckedItems[i].ToString() + ";";
            }
        }
    }
}
