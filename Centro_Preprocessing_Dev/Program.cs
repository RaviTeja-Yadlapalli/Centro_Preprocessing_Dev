using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Centro_Preprocessing_Dev
{
    class Program
    {
        static void Main(string[] args)
        {
            Fixing99999_Records fixing99999_Records = new Fixing99999_Records();
            fixing99999_Records.fixing99999Records();
            FixNullTimes fixNullTimes = new FixNullTimes();
            fixNullTimes.fixArriveDepartTimes();
            FixTimePointMinus1 fixTimePointMinus1 = new FixTimePointMinus1();
            fixTimePointMinus1.fixTimepointMinus1();
        }
    }
}
