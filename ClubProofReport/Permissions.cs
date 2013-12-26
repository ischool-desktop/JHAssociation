using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubProofReport
{
    class Permissions
    {
        public static string 社團參與證明單 { get { return "ClubProofReport74F8FC9C-1822-4141-BFB2-27507DDD8176"; } }
        public static bool 社團參與證明單權限
        {
            get
            {
                return FISCA.Permission.UserAcl.Current[社團參與證明單].Executable;
            }
        }
    }
}
