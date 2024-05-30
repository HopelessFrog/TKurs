using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKurs.Model.Calculators
{
    public class BoxMethodOptimizer : ICalculator
    {
        private  FiltrationProcess _process;
       

      

     
        public (double T1, double T2, double OptimalCost, List<(double T1, double T2, double Cost)>) DataOptimize(
            FiltrationProcess process, double T1Min, double T1Max,
            double T2Min, double T2Max, double TRe, double step, bool findMinimum)
        {
            _process = process;
            double optimalCost = findMinimum ? double.MaxValue : double.MinValue;
            double bestT1 = 0;
            double bestT2 = 0;
            var data = new List<(double T1, double T2, double  cost)>();

            for (double T1 = T1Min; T1 <= T1Max; T1 += step)
            {
                for (double T2 = T2Min; T2 <= T2Max; T2 += step)
                {
                    if ((0.5 * T1 + T2) <= TRe)
                    {
                        double volume = _process.CalculateVolume(T1, T2);
                        double cost = _process.CalculateCost(volume);
                        data.Add((T1, T2, cost));

                        if (findMinimum)
                        {
                            if (cost < optimalCost)
                            {
                                optimalCost = cost;
                                bestT1 = T1;
                                bestT2 = T2;
                            }
                        }
                        else
                        {
                            if (cost > optimalCost)
                            {
                                optimalCost = cost;
                                bestT1 = T1;
                                bestT2 = T2;
                            }
                        }
                    }
                }
            }

            return (bestT1, bestT2, optimalCost, data);
        }
    }
}
