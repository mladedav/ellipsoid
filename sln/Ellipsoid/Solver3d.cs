using System;
using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace Ellipsoid
{
    public class Solver3d
    {
        private List<Equation3d> _problem = new List<Equation3d>();
        private const double _epsillon = 1e-8;

        public Solver3d()
        {
        }

        public void AddEquation(Equation3d equation)
        {
            _problem.Add(equation);
        }

        public void AddEquation(double x, double y, double z, double a)
        {
            AddEquation(new Equation3d(x, y, z, a));
        }

        // Optimize me and make me return something more resembling shape -- vertices, edges and faces maybe?
        public IList<Point3d> GetShape()
        {
            return Solver3d.GetShape(_problem);
        }

        public static IList<Point3d> GetShape(IList<Equation3d> equations)
        {
            var result = new SortedSet<Point3d>(new PointComparer());
            for (var i = equations.Count - 1; i >= 0 ; --i)
            {
                for (var j = i - 1; j >= 0; --j)
                {
                    for (var k = j - 1; k >= 0; --k)
                    {
                        // var point = Solve(equations[i], equations[j], equations[k]);
                        var point = SolveMathNet(equations[i], equations[j], equations[k]);
                        if (point is null)
                        {
                            continue;
                        }

                        var valid = true;
                        for (var l = 0; l < equations.Count; l++)
                        {
                            if (l == i || l == j || l == k)
                            {
                                continue;
                            }

                            if (!Satisfies(point.Value, equations[l]))
                            {
                                valid = false;
                                break;
                            }
                        }

                        if (valid)
                        {
                            result.Add(point.Value);
                        }
                    }
                }
            }

            return result.ToList();
        }

        // Convex hull
        // A crazy n^4 awesomeness right here!
        public static IList<Triangle> GetTriangles(IList<Point3d> points)
        {
            bool isFace(Point3d a, Point3d b, Point3d c)
            {
                var vx = a.X - b.X;
                var vy = a.Y - b.Y;
                var vz = a.Z - b.Z;
                var ux = a.X - c.X;
                var uy = a.Y - c.Y;
                var uz = a.Z - c.Z;

                var x = uz * vy - uy * vz;
                var y = ux * vz - uz * vx;
                var z = uy * vx - ux * vy;

                var alpha = x * a.X + y * a.Y + z * a.Z;

                var equation = new Equation3d(x, y, z, alpha);

                var directionSet = false;
                var positiveDirection = true;
                foreach (var point in points)
                {
                    var leftSide = x * point.X + y * point.Y + z * point.Z;

                    if (leftSide < alpha - _epsillon)
                    {
                        if (!directionSet)
                        {
                            directionSet = true;
                            // redundant
                            positiveDirection = true;
                        }
                        if (!positiveDirection)
                        {
                            return false;
                        }
                    }
                    else if (leftSide > alpha + _epsillon)
                    {
                        if (!directionSet)
                        {
                            directionSet = true;
                            positiveDirection = false;
                        }
                        if (positiveDirection)
                        {
                            return false;
                        }
                    }
                }
                return true;
            };

            var result = new List<Triangle>();

            for (var i = 0; i < points.Count; i++)
            {
                for (var j = i + 1; j < points.Count; j++)
                {
                    for (var k = j + 1; k < points.Count; k++)
                    {
                        if (isFace(points[i], points[j], points[k]))
                        {
                            result.Add(new Triangle(points[i], points[j], points[k]));
                        }
                    }
                }
            }
            return result;
        }

        public static bool Satisfies(Point3d point, Equation3d equation)
        {
            return equation.X * point.X + equation.Y * point.Y + equation.Z * point.Z <= equation.A + _epsillon;
        }

        private static Point3d? SolveMathNet(Equation3d p1, Equation3d p2, Equation3d p3)
        {
            var m = MathNet.Numerics.LinearAlgebra.Matrix<double>.Build.Dense(3, 3);
            m[0, 0] = p1.X;
            m[0, 1] = p1.Y;
            m[0, 2] = p1.Z;
            m[1, 0] = p2.X;
            m[1, 1] = p2.Y;
            m[1, 2] = p2.Z;
            m[2, 0] = p3.X;
            m[2, 1] = p3.Y;
            m[2, 2] = p3.Z;

            var b = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(3);
            b[0] = p1.A;
            b[1] = p2.A;
            b[2] = p3.A;

            // var evd = m.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Symmetric);
            // Console.WriteLine(evd.EigenValues);
            // Console.WriteLine(evd.EigenVectors);

            var res = m.Solve(b);
            return new Point3d(res[0], res[1], res[2]);
        }

        public static void Main()
        {

            Equation3d[] _baseBorder = new[]
            {
                new Equation3d(1, 0, 0, 1_000_000),
                new Equation3d(-1, 0, 0, 1_000_000),
                new Equation3d(0, 1, 0, 1_000_000),
                new Equation3d(0, -1, 0, 1_000_000),
                new Equation3d(0, 0, 1, 1_000_000),
                new Equation3d(0, 0, -1, 1_000_000),
                new Equation3d(1, 1, 1, 100),
                new Equation3d(-1, 1, 1, 100),
                new Equation3d(-1, -1, 1, 100),
                new Equation3d(1, -1, 1, 100),
                new Equation3d(1, -1, -1, 100),
                new Equation3d(1, 1, -1, 100),
                new Equation3d(-1, 1, -1, 100),
                new Equation3d(-1, -1, -1, 100),
                new Equation3d(0, 0, 0, 0),
            };

            var points = GetShape(_baseBorder);
            foreach (var point in points)
            {
                Console.WriteLine($"X: {point.X} Y: {point.Y} Z: {point.Z} ");
            }

            var triangles = GetTriangles(points);
            foreach (var triangle in triangles)
            {
                Console.WriteLine($"A: X: {triangle.A.X} Y: {triangle.A.Y} Z: {triangle.A.Z}");
                Console.WriteLine($"B: X: {triangle.B.X} Y: {triangle.B.Y} Z: {triangle.B.Z}");
                Console.WriteLine($"C: X: {triangle.C.X} Y: {triangle.C.Y} Z: {triangle.C.Z}");
                Console.WriteLine();
            }
        }

        public Ellipsoid3d? Iterate(Ellipsoid3d original)
        {
            var unsatisfied = UnsatisfiedEquation(original.Center);
            if (unsatisfied is null)
            {
                return null;
            }
            Console.WriteLine($"x: {unsatisfied.Value.X}, y: {unsatisfied.Value.Y}, z: {unsatisfied.Value.Y}, a: {unsatisfied.Value.A}");
            const int n = 3;
            var z = Vector<double>.Build.Dense(3);
            z[0] = original.Center.X;
            z[1] = original.Center.Y;
            z[2] = original.Center.Z;

            var a = Matrix<double>.Build.Dense(3, 1);
            a[0, 0] = unsatisfied.Value.X;
            a[1, 0] = unsatisfied.Value.Y;
            a[2, 0] = unsatisfied.Value.Z;

            // Calculate new center
            var Ea = original.Matrix * a;
            var numerator = Ea;
            var at = a.Transpose();
            var atEa = (at * Ea)[0, 0];
            var denominator = Math.Sqrt(atEa);

            var change = new Point3d(numerator[0, 0], numerator[1, 0], numerator[2, 0]);
            change /= -(1 + n) * denominator;

            var newCenter = original.Center + change;

            // Calculate new matrix
            var aat = a * at;
            var EaatE = original.Matrix * aat * original.Matrix;
            var newMatrix = (double)(n * n) / (n * n - 1) * (original.Matrix - (double)2 / (n + 1) * EaatE / atEa);

            return new Ellipsoid3d(newCenter, newMatrix);
        }

        public Equation3d? UnsatisfiedEquation(Point3d point)
        {
            foreach (var equation in _problem)
            {
                if (!Satisfies(point, equation))
                {
                    return equation;
                }
            }
            return null;
        }
    }
}
