using FindMe.ApproximationAlgorithmTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest.Data.Reposaitories
{
    public class RawDataRepository : IRawDataRepository
    {
        public IEnumerable<RawDataModel> GetData()
        {
            List<RawDataModel> dataList = new List<RawDataModel>();


            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FindMe.ApproximationAlgorithmTest.Resources.test_data.csv"))
            {
                using (var reader = new StreamReader(stream))
                {
                    while (!reader.EndOfStream)
                    {
                        string csvLine = reader.ReadLine();

                        string[] csvLineParts = csvLine.Split(';');

                        if (csvLineParts != null && csvLineParts.Length == 5)
                        {
                            RawDataModel rawDataModel = new RawDataModel();
                            rawDataModel.AnchorId = Convert.ToInt32(csvLineParts[0]);
                            rawDataModel.TagId = Convert.ToInt32(csvLineParts[1]);
                            rawDataModel.Distance = (decimal)Convert.ToDouble(csvLineParts[2], new NumberFormatInfo() { NumberDecimalSeparator = "." });
                            rawDataModel.Time = Convert.ToDateTime(csvLineParts[3]);
                            rawDataModel.NoMovement = Convert.ToBoolean(Convert.ToInt32(csvLineParts[4]));
                            dataList.Add(rawDataModel);
                        }
                    }
                }
            }
            return dataList;
        }
    }
}
