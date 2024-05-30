using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKurs.Model.Calculators
{
    public interface ICalculator
    {
        (double T1, double T2, double OptimalCost, List<(double T1, double T2, double Cost)>) DataOptimize(FiltrationProcess process, double T1Min, double T1Max, double T2Min, double T2Max, double step, double TRe, bool findMinimum);
    }
}
