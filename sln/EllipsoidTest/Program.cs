using System;
using Ellipsoid;

namespace EllipsoidTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var matrix = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.Dense(3, 3, new double[]
            {
                250_000, 0, 0,
                0, 22_500, 0,
                0, 0, 22_500
            });
            var center = new Point3d(150, 0, 0);
            var el = new Ellipsoid3d(center, matrix);
            var point = new Point3d(600, 0, 0);
            var point_ = new Point3d(-300, 0, 0);
            var axes = el.Axes();
            foreach (var a in axes)
            {
                Console.WriteLine(a);
            }
            Console.WriteLine(el.Contains(point));
            Console.WriteLine(el.Contains(point_));

            Equation3d[] _baseBorder = new[]
            {
                new Equation3d(1, 1, 1, 100),
                new Equation3d(-1, 1, 1, 100),
                new Equation3d(-1, -1, 1, 100),
                new Equation3d(1, -1, 1, 100),
                new Equation3d(1, -1, -1, 100),
                new Equation3d(1, 1, -1, 100),
                new Equation3d(-1, 1, -1, 100),
                new Equation3d(-1, -1, -1, 100),
            };

            var solver = new Solver3d();
            foreach (var equation in _baseBorder)
            {
                solver.AddEquation(equation);
            }
            el = solver.Iterate(el).Value;
            axes = el.Axes();
            foreach (var a in axes)
            {
                Console.WriteLine(a);
            }

            Console.WriteLine(el.Contains(point));
            Console.WriteLine(el.Contains(point_));
        }
    }
}
