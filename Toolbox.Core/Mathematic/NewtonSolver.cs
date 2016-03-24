using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Mathematic
{
    public class NewtonSolver
    {
        public Func<double, double> Function { get; set; }
        public Func<double, double> Gradient { get; set; }
        public int MaxInterations { get; set; }
        public double Precision { get; set; }
        public double Target { get; set; }
        public double Step { get; set; }
        public double Solution { get; set; }
        public bool HasConverged { get; set; }
        public int Iterations { get; set; }

        public NewtonSolver(Func<double, double> function)
        {
            Function = function;
            MaxInterations = 100;

            Precision = 1e-6;
            Target = 0;
            Step = 1e-6;
        }

        public NewtonSolver WithPrecision(double precision)
        {
            Precision = precision;
            return this;
        }

        public NewtonSolver WithTarget(double target)
        {
            Target = target;
            return this;
        }

        public NewtonSolver WithFiniteDifferenceStep(double step)
        {
            Step = step;
            return this;
        }

        public NewtonSolver WithGradient(Func<double, double> grad)
        {
            Gradient = grad;
            return this;
        }

        public NewtonSolver Solve(double startPoint = 0)
        {
            HasConverged = false;
            Iterations = 0;

            if (Gradient == null)
                SolveWithFiniteDifferencesApproximation(startPoint);
            else
                SolveWithGradient(startPoint);

            return this;
        }

        private void SolveWithGradient(double startPoint)
        {
            double v, d;
            double s = startPoint;

            v = Function.Invoke(s) - Target;
            do
            {
                d = Gradient.Invoke(s);
                if (d == 0)
                {
                    Solution = Double.NaN;
                    return;
                }
                s -= v / d;
                v = Function.Invoke(s) - Target;
            } while (Math.Abs(v) > Precision && Iterations++ < MaxInterations);

            Iterations++;
            Solution = s;
            if (Iterations < MaxInterations)
                HasConverged = true;
            else
                Solution = Double.NaN;
        }

        private void SolveWithFiniteDifferencesApproximation(double startPoint)
        {
            double v, ve;
            double s = startPoint;

            v = Function.Invoke(s) - Target;
            do
            {
                ve = Function.Invoke(s + Step) - Target;
                if (v == ve)
                {
                    Solution = Double.NaN;
                    return;
                }
                s += v * Step / (v - ve);
                v = Function.Invoke(s) - Target;
            } while (Math.Abs(v) > Precision && Iterations++ < MaxInterations);

            Iterations++;
            Solution = s;
            if (Iterations < MaxInterations && !Double.IsNaN(v))
                HasConverged = true;
            else
                Solution = Double.NaN;
        }
    }
}
