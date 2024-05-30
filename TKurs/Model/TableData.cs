using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKurs.Model
{
    public class TableData
    {
        public TableData(double t1, double t2, double cost)
        {
            T1 = t1;
            T2 = t2;
            Cost = cost;
        }
        public double T1 { get; set; }
        public double T2 { get; set; }

        public double Cost { get; set; }
    }
}
