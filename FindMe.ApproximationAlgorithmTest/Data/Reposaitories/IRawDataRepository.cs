using FindMe.ApproximationAlgorithmTest.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest.Data.Reposaitories
{
    public interface IRawDataRepository
    {
        IEnumerable<RawDataModel> GetData();
    }
}
