using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VNT
{
    class Variable
    {
        //Change this to properties
        public string name { get; set; }
        public int value { get; set; }
        public Variable(string feed)
        {
            name = feed;
            value = 0;
        }
        public bool IsUnused(List<Slide> slideList)
        {
            for(int i = 0; i < slideList.Count; i++)
            {
                for (int j = 0; j < slideList[i].info.Count; j++)
                {
                    if (Convert.ToInt32(slideList[i].info[j][4].Substring(0, 1)) == 2)
                    {
                        if (slideList[i].info[j][4].Substring(2, slideList[i].info[j][4].IndexOf("+") - 2) == name)
                            return false;
                    }
                    else if (Convert.ToInt32(slideList[i].info[j][4].Substring(0, 1)) == 3)
                    {
                        if (slideList[i].info[j][4].Substring(2, slideList[i].info[j][4].IndexOf(">") - 2) == name)
                            return false;
                    }
                }
            }
            return true;
        }
    }
}
