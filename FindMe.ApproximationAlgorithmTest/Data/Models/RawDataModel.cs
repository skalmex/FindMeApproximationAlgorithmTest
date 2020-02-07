using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FindMe.ApproximationAlgorithmTest.Data.Models
{
    public class RawDataModel
    {
        public virtual int AnchorId { get; set; }

        public virtual int TagId { get; set; }

        public virtual decimal Distance { get; set; }

        public virtual DateTime Time { get; set; }

        public virtual bool NoMovement { get; set; }

        public virtual decimal NextDistance { get; set; }

        public virtual DateTime NextTime { get; set; }

        public virtual decimal PreviousDistance { get; set; }

        public virtual DateTime PreviousTime { get; set; }
    }
}
