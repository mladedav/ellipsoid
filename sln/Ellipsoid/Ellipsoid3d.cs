using System;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;

namespace Ellipsoid
{
    public struct Ellipsoid3d
    {
        private const double _epsillon = 1e-8;

        public Ellipsoid3d(Point3d center, Matrix<double> matrix)
        {
            Center = center;
            Matrix = matrix;
        }

        public Point3d Center { get; }
        public Matrix<double> Matrix { get; }

        public static Matrix<double> SquareRoot(Matrix<double> matrix)
        {
            var evd = matrix.Evd(MathNet.Numerics.LinearAlgebra.Symmetricity.Symmetric);
            var result = Matrix<double>.Build.Dense(3, 3);
            for (var i = 0; i < 3; i++)
            {
                result += 1 / Math.Sqrt(evd.EigenValues[i].Real) *
                    evd.EigenVectors.Column(i).ToColumnMatrix() *
                    evd.EigenVectors.Column(i).ToRowMatrix();
            }

            result = result.Inverse();

            return result;
        }

        public bool Contains(Point3d point)
        {
            var centeredPoint = point - Center;
            var pointVector = Matrix<double>.Build.Dense(3, 1, new[] { centeredPoint.X, centeredPoint.Y, centeredPoint.Z });
            var inverse = Matrix.Inverse();
            var res = (pointVector.Transpose() * inverse * pointVector)[0, 0];
            return res <= 1 + _epsillon;
        }

        public Vector<double>[] Axes()
        {
            var result = new Vector<double>[3];

            var sqrtMatrix = Ellipsoid3d.SquareRoot(Matrix);
            var evd = sqrtMatrix.Evd(Symmetricity.Symmetric);

            var ev0 = evd.EigenVectors.Column(0);
            var q0 = evd.EigenValues[0].Real / ev0.L2Norm();
            result[0] = q0 * ev0;

            var ev1 = evd.EigenVectors.Column(1);
            var q1 = evd.EigenValues[1].Real / ev1.L2Norm();
            result[1] = q1 * ev1;

            var ev2 = evd.EigenVectors.Column(2);
            var q2 = evd.EigenValues[2].Real / ev2.L2Norm();
            result[2] = q2 * ev2;

            return result;
        }
    }
}
