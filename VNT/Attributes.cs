using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace VNT
{
    public class Attributes
    {
        public Point position { get; set; }
        public Point size { get; set; }
        public string path { get; set; }
        public int type { get; set; }
        public int value { get; set; }
        public decimal[] slides { get; set; }
        public string[] variables { get; set; }
        public Attributes(string location, string locationEnd, string file, string setting)
        {
            position = new Point(Convert.ToInt32(location.Substring(0, location.IndexOf(","))), Convert.ToInt32(location.Substring(location.IndexOf(",") + 1, location.Length - location.IndexOf(",") - 1)));
            size = new Point(Convert.ToInt32(locationEnd.Substring(0, locationEnd.IndexOf(","))), Convert.ToInt32(locationEnd.Substring(locationEnd.IndexOf(",") + 1, locationEnd.Length - locationEnd.IndexOf(",") - 1)));
            path = file;
            type = Convert.ToInt32(setting.Substring(0, 1));
            if (type == 1)
                slides = new decimal[1] { Convert.ToDecimal(setting.Substring(2)) };
            else if (type == 4)
            {
                int i = 2, j = 0;
                while (setting.IndexOf(";", i) != -1)
                {
                    i = setting.IndexOf(";", i) + 1;
                    j++;
                }
                variables = new string[j];
                i = 2;
                j = 0;
                while (setting.IndexOf(";", i) != -1)
                {
                    variables[j] = setting.Substring(i, setting.IndexOf(";", i) - i);
                    i = setting.IndexOf(";", i) + 1;
                    j++;
                }
            }
            else
            {
                variables = new string[1] { setting.Substring(2, setting.IndexOf("*") - 2) };
                value = Convert.ToInt32(setting.Substring(setting.IndexOf("*") + 1, setting.IndexOf("/", 2) - setting.IndexOf("*") - 1));
                if (type == 2)
                    slides = new decimal[1] { Convert.ToDecimal(setting.Substring(setting.IndexOf("/", 2) + 1)) };
                else
                    slides = new decimal[2] { Convert.ToDecimal(setting.Substring(setting.IndexOf("/", 2) + 1, setting.IndexOf(";") - setting.IndexOf("/", 2) - 1)), Convert.ToDecimal(setting.Substring(setting.IndexOf(";") + 1)) };
            }
        }
        public string outputSetting(Attributes attributes)
        {
            if (type == 1)
                return "1/" + attributes.slides[0].ToString();
            else if (type == 2)
                return "2/" + attributes.variables[0] + "*" + attributes.value + "/" + attributes.slides[0].ToString();
            else if (type == 3)
                return "3/" + attributes.variables[0] + "*" + attributes.value + "/" + attributes.slides[0].ToString() + ";" + attributes.slides[1].ToString();
            else
            {
                string holder = "4/";
                for (int i = 0; i < variables.Length; i++)
                    holder += attributes.variables[i] + ";";
                return holder;
            }
        }
    }
}
