using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKurs.Model.Calculators
{
    public class FiltrationProcess
    {
        public double Δp1 { get; set; }
        public double Δp2 { get; set; }
        public int N { get; set; }
        public double G { get; set; }
        public double α { get; set; }
        public double β { get; set; }
        public double μ { get; set; }
        public double γ { get; set; }
        public double β1 { get; set; }
        public double μ1 { get; set; }
        public double TDiff { get; set; }
        public double CostPerCubicMeter { get; set; }

        public FiltrationProcess()
        {
            α = 1;
            β = 1;
            μ = 1;
            γ = 1;
            β1 = 1;
            μ1 = 1;
            Δp1 = 11;
            Δp2 = 7;
            N = 2;
            G = 1;
            CostPerCubicMeter = 200;
        }


        public double CalculateVolume(double T1, double T2)
        {
            double term1 = α * G * Math.Pow(T1 * T1 + β * T2 - μ * Δp1, N);
            double term2 = γ * Math.Pow(β1 * T1 + T2 * T2 - μ1 * Δp2, N);
            return term1 + term2;
        }

        public double CalculateCost(double volume)
        {
            return volume * CostPerCubicMeter * 24;
        }
    }
}