using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VNT
{
    class Slide
    {
        //Change this to properties
        public float index { get; set; }
        public string pathBG { get; set; }
        public List<string[]> info { get; set; } //0 - picture box location, 1 - picturebox size, 2 - picturebox path, 3 - picturebox role
        public Slide(string[] feed, int startFrom)
        {
            index = Convert.ToSingle(feed[startFrom].Substring(6, feed[startFrom].Length - 6));
            pathBG = feed[startFrom + 1];
            info = new List<string[]>();
            for (int i = 0; feed.Length > startFrom + 2 + i * 4 && (feed[startFrom + 2 + i * 4].Length < 5 || feed[startFrom + 2 + i * 4].Substring(0, 5) != "Slide"); i++)
            {
                info.Add(new string[4]);
                for (int j = 0; j < 4; j++)
                    info[i][j] = feed[startFrom + 2 + i * 4 + j];
            }
        }
        public Slide()
        {
            info = new List<string[]>();
        }
    }
}
