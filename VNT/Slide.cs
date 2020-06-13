using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;

namespace VNT
{
    internal class Slide
    {
        public decimal index { get; set; }
        public Attributes bgInfo { get; set; }
        public List<Attributes> pbInfo { get; set; }
        public Slide(string[] feed, int startFrom)
        {
            index = Convert.ToDecimal(feed[startFrom].Substring(6, feed[startFrom].Length - 6));
            bgInfo = new Attributes(feed[startFrom + 1], feed[startFrom + 2], feed[startFrom + 3], feed[startFrom + 4]);
            pbInfo = new List<Attributes>();
            for (int i = 1; feed.Length > startFrom + i * 4 + 1 && (feed[startFrom + i * 4 + 1].Length < 5 || feed[startFrom + i * 4 + 1].Substring(0, 5) != "Slide"); i++)
                pbInfo.Add(new Attributes(feed[startFrom + i * 4 + 1], feed[startFrom + i * 4 + 2], feed[startFrom + i * 4 + 3], feed[startFrom + i * 4 + 4]));
        }
    }
}
