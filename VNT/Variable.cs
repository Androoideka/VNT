using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VNT
{
    class Variable
    {
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
                for (int j = 0; j < slideList[i].pbInfo.Count; j++)
                {
                    if (slideList[i].pbInfo[j].type == 2)
                    {
                        if (slideList[i].pbInfo[j].variables[0] == name)
                            return false;
                    }
                    else if (slideList[i].pbInfo[j].type == 3)
                    {
                        if (slideList[i].pbInfo[j].variables[0] == name)
                            return false;
                    }
                }
            }
            return true;
        }
    }
}
