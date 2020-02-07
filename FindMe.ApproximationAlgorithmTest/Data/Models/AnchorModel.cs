using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest.Data.Models
{
    public class AnchorModel
    {
        public AnchorModel()
        {
        }

        public virtual int AnchorId { get; set; }

        public virtual decimal? X { get; set; }

        public virtual decimal? Y { get; set; }

        public virtual int Height { get; set; }


        public virtual double Scale { get; set; }
    }
}
