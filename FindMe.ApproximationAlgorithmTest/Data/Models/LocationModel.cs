using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest.Data.Models
{
    public class LocationModel
    {
        public virtual int TagId { get; set; }

        public virtual decimal Position_X { get; set; }

        public virtual decimal Position_Y { get; set; }

        public virtual decimal Position_Z { get; set; }

        public virtual DateTime Time { get; set; }

        public virtual bool NoMovement { get; set; }
    }
}
