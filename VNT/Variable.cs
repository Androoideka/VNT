using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VNT
{
    class Variable
    {
        //Change this to properties
        public string name;
        public int value;
        public Variable(string feed)
        {
            name = feed;
            value = 0;
        }
    }
}
