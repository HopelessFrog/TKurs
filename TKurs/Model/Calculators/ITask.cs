using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKurs.Model.Calculators
{
    public interface ITask
    {
        public double T1From { get; set; }
        public double T1To { get; set; }
        public double T2From { get; set; }
        public double T2To { get; set; }
        public double TDiff { get; set; }
        public double S { get; set; }
        public double Equation(double T1, double T2);
    }
}
