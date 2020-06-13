using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VNT
{
    class Slide
    {
        public float index = 0;
        public int pbNum;
        public string pathBG;
        public string[,] info; //0 - picture box location (label also), 1 - picturebox size, 2 - picturebox path, 3 - label text, 4 - picturebox role
        public Slide()
        {
        }
    }
}
