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
        public float index = 0;
        public int pbNum = 0;
        public string pathBG = @"Default.png";
        public List<string[]> info; //0 - picture box location, 1 - picturebox size, 2 - picturebox path, 3 - text, 4 - picturebox role
        public Slide(string[] feed, int startFrom)
        {
            index = Convert.ToSingle(feed[startFrom].Substring(6, feed[startFrom].Length - 6));
            pathBG = feed[startFrom + 1];
            pbNum = Convert.ToInt32(feed[startFrom + 2]);
            info = new List<string[]>(pbNum);
            for (int i = 0; i < pbNum; i++)
            {
                info[i] = new string[5];
                for (int j = 0; j < 5; j++)
                    info[i][j] = feed[startFrom + 3 + i * 5 + j];
            }
        }
        public Slide()
        {
            info = new List<string[]>();
        }
    }
}
