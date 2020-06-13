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
        public Attributes config { get; set; }
        bool playMode;
        public VarTrack(string[] variables, int[] variableValues, Attributes setting, bool playOrEdit)
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
            int j = 0;
            for(int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                if(config.variables != null && j < config.variables.Length)
                {
                    if (checkedListBox1.Items[i].ToString() == config.variables[j])
                    {
                        checkedListBox1.SetItemChecked(i, true);
                        j++;
                    }
                    else
                        checkedListBox1.SetItemChecked(i, false);
                }
            }
        }
        private void VarTrack_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!playMode)
            {
                config.type = 4;
                config.variables = new string[checkedListBox1.CheckedItems.Count];
                for (int i = 0; i < checkedListBox1.CheckedItems.Count; i++)
                    config.variables[i] = checkedListBox1.CheckedItems[i].ToString();
            }
        }
    }
}
