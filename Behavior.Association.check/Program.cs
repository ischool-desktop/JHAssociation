using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FISCA;
using DataRationality;

namespace JHSchool.Association.check
{
    public class Program
    {
        [MainMethod]
        public static void Main()
        {
            //社團成績重覆檢查2
            DataRationalityManager.Checks.Add(new AssociationRATRecordRAT_2());
        }
    }
}
