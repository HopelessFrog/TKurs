using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKurs.Model.Calculators
{
    class FirstTask : ITask
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
        public double CostPerCubicMeter { get; set; }

        public double T1From { get; set; }
        public double T1To { get; set; }
        public double T2From { get; set; }
        public double T2To { get; set; }
        public double TDiff { get; set; }
        public double S { get; set; }

        public FirstTask()
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
            S = 24;
        }

        public double Equation(double T1, double T2)
        {
            double term1 = α * G * Math.Pow(T1 * T1 + β * T2 - μ * Δp1, N);
            double term2 = γ * Math.Pow(β1 * T1 + T2 * T2 - μ1 * Δp2, N);
            return term1 + term2;
        }

        public bool Validate()
        {
            if (α < 0 || α > 1)
                return false;
            if (β < 0 || β > 1)
                return false;
            if (μ < 0 || μ > 1)
                return false;
            if (γ < 0 || γ > 1)
                return false;
            if (β1 < 0 || β1 > 1)
                return false;
            if (μ1 < 0 || μ1 > 1)
                return false;
            if (T1To < -273 || T1From < -273 || T1From > T1To)
                return false;
            if (T2To < -273 || T2From < -273 || T2From > T2To)
                return false;
            if (Δp1 < 0 || G < 0 || Δp2 < 0)
                return false;
            return true;
        }
    }
}
