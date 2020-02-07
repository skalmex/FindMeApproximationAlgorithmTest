using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest.Helpers
{
    public class Conversions
    {
        public static string ConvertNumberListToString(List<int> numberList)
        {
            StringBuilder sb = new StringBuilder();

            if (numberList != null)
            {
                for (int i=0; i< numberList.Count; i++)
                {
                    if (i > 0) sb.Append(",");

                    sb.Append(numberList[i].ToString());
                }
            }

            return sb.ToString();
        }
    }
}
