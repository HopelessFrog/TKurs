using SciChart.Charting.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TKurs.Model.Calculators
{
    public class BoxMethod : IMethod
    {
        public double Epsilon { get; set; } = 0.1;
        public ExtremumType Extremum { get; set; } = ExtremumType.Minimum;
        public ITask? Task { get; set; }
        private int n { get; set; } = 2;

        public int N { get; set; }
        public BoxMethod()
        {
            if (n > 5)
            {
                N = n + 2;
            }
            else
            {
                N = n * 2;
            }
        }
        public (double, double, double, double) Solve()
        {
            if (Task is null)
                throw new Exception();
            Random rand = new Random();
            double[,] points = new double[n, N];
            points = GetPoints(points, rand);
            (bool areAllBad, List<int> invalidPos, List<int> validPos) = CheckPoints(points);
            if (areAllBad)
            {
                int b = 0;
                while (areAllBad)
                {
                    if (b >= 100)
                    {
                        throw new Exception();
                    }
                    points = GetPoints(points, rand);
                    (areAllBad, invalidPos, validPos) = CheckPoints(points);
                    b++;
                }
            }
            foreach (int inval in invalidPos)
            {
                while (points[1, inval] - points[0, inval] < Task.TDiff)
                {
                    double valSumForFirst = 0;
                    double valSumForSecond = 0;
                    foreach (int val in validPos)
                    {
                        valSumForFirst += points[0, val];
                        valSumForSecond += points[1, val];
                    }
                    points[0, inval] = 0.5 * (points[0, inval] + (valSumForFirst / validPos.Count));
                    points[1, inval] = 0.5 * (points[1, inval] + (valSumForSecond / validPos.Count));
                }
            }

            double[] funcValues = new double[N];
            for (int x = 0; x < N; x++)
            {
                funcValues[x] = Task.Equation(points[0, x], points[1, x]);
            }
            (int bestPointInd, int worstPointInd) = GetExtrIndices(funcValues);
            double[] centers = GetCenters(points, worstPointInd);
            while (!CheckBIndicator(points, centers, bestPointInd, worstPointInd))
            {
                (points, funcValues) = ChangeWorstPoint(points, funcValues, centers, worstPointInd, bestPointInd);
                (bestPointInd, worstPointInd) = GetExtrIndices(funcValues);
                centers = GetCenters(points, worstPointInd);
            }
            double T1 = centers[0];
            double T2 = centers[1];
            double compQuan = Task.Equation(T1, T2);
            double res = compQuan * Task.S;
            return (T1, T2, compQuan, res);
        }
        private (double[,], double[]) ChangeWorstPoint(double[,] points, double[] funcValues, double[] centers, int worstPointInd, int bestPointInd)
        {
            double[] mids = new double[n];
            for (int i = 0; i < n; i++)
            {

                mids[i] = 2.3 * centers[i] - 1.3 * points[i, worstPointInd];
                double from, to;
                if (i == 0)
                {
                    from = Task!.T1From;
                    to = Task.T1To;
                }
                else
                {
                    from = Task!.T2From;
                    to = Task.T2To;
                }
                if (mids[i] < from)
                {
                    mids[i] = from + Epsilon;
                }
                else if (mids[i] > to)
                {
                    mids[i] = to - Epsilon;
                }
            }
            while (mids[1] - mids[0] < Task!.TDiff)
            {
                mids[0] = 0.5 * (mids[0] + centers[0]);
                mids[1] = 0.5 * (mids[1] + centers[1]);
            }
            double newFuncValue = Task.Equation(mids[0], mids[1]);
            if (Extremum == ExtremumType.Minimum)
            {
                while (newFuncValue > funcValues[worstPointInd])
                {
                    for (int i = 0; i < n; i++)
                    {
                        mids[i] = 0.5 * (mids[i] + points[i, bestPointInd]);
                    }
                    newFuncValue = Task.Equation(mids[0], mids[1]);
                }
            }
            else
            {
                while (newFuncValue < funcValues[worstPointInd])
                {
                    for (int i = 0; i < n; i++)
                    {
                        mids[i] = 0.5 * (mids[i] + points[i, bestPointInd]);
                    }
                    newFuncValue = Task.Equation(mids[0], mids[1]);
                }
            }
            funcValues[worstPointInd] = newFuncValue;
            points[0, worstPointInd] = mids[0];
            points[1, worstPointInd] = mids[1];
            return (points, funcValues);
        }
        private bool CheckBIndicator(double[,] points, double[] centers, int bestPointInd, int worstPointInd)
        {
            bool shouldStop = false;
            double mid = 0;
            for (int x = 0; x < n; x++)
            {
                mid += Math.Abs(centers[x] - points[x, worstPointInd]) + Math.Abs(centers[x] - points[x, bestPointInd]);
            }
            double b = mid / (n * 2);
            if (b < Epsilon)
            {
                shouldStop = true;
            }
            return shouldStop;
        }
        private (int bestPointInd, int worstPointInd) GetExtrIndices(double[] funcValues)
        {
            int bestPointInd, worstPointInd;
            if (Extremum == ExtremumType.Maximum)
            {
                bestPointInd = funcValues.IndexOf(funcValues.Max());
                worstPointInd = funcValues.IndexOf(funcValues.Min());
            }
            else
            {
                bestPointInd = funcValues.IndexOf(funcValues.Min());
                worstPointInd = funcValues.IndexOf(funcValues.Max());
            }
            return (bestPointInd, worstPointInd);
        }
        private double[] GetCenters(double[,] points, int worstPointInd)
        {
            double[] centers = new double[n];
            for (int x = 0; x < n; x++)
            {
                double sum = 0;
                for (int l = 0; l < N; l++)
                {
                    sum += points[x, l];
                }
                double center = (sum - points[x, worstPointInd]) / (N - 1);
                centers[x] = center;
            }
            return centers;
        }
        private double[,] GetPoints(double[,] points, Random rand)
        {
            for (int i = 0; i < N; i++)
            {
                points[0, i] = Task!.T1From + rand.NextDouble() * (Task.T1To - Task.T1From);
                points[1, i] = Task.T2From + rand.NextDouble() * (Task.T2To - Task.T2From);
            }
            return points;
        }
        private (bool areAllBad, List<int> invalidPos, List<int> validPos) CheckPoints(double[,] points)
        {
            bool areAllBad = false;
            List<int> invalidPos = new();
            List<int> validPos = new();
            for (int i = 0; i < N; i++)
            {
                double diff = points[1, i] - points[0, i];
                if (diff < Task!.TDiff)
                {
                    invalidPos.Add(i);
                    continue;
                }
                validPos.Add(i);
            }
            if (invalidPos.Count == N)
            {
                areAllBad = true;
            }
            return (areAllBad, invalidPos, validPos);
        }
    }
}
