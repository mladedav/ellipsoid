using System;
using System.Collections.Generic;

namespace Ellipsoid
{
    public struct Point3d
    {
        public Point3d(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static Point3d operator+(Point3d first, Point3d second)
        {
            return new Point3d(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
        }

        public static Point3d operator-(Point3d first, Point3d second)
        {
            return new Point3d(first.X - second.X, first.Y - second.Y, first.Z - second.Z);
        }

        public static Point3d operator/(Point3d first, double scale)
        {
            return new Point3d(first.X / scale, first.Y / scale, first.Z / scale);
        }

        public override string ToString()
        {
            return $"X: {X} Y: {Y} Z: {Z}";
        }
    }

    internal class PointComparer : IComparer<Point3d>
    {
        private const double _epsillon = 1e-8;
        public int Compare(Point3d x, Point3d y) => StaticCompare(x, y);
        public static int StaticCompare(Point3d x, Point3d y)
        {
            if (Math.Abs(x.X - y.X) > _epsillon)
            {
                if (x.X < y.X)
                {
                    return 1;
                }
                return -1;
            }
            if (Math.Abs(x.Y - y.Y) > _epsillon)
            {
                if (x.Y < y.Y)
                {
                    return 1;
                }
                return -1;
            }
            if (Math.Abs(x.Z - y.Z) > _epsillon)
            {
                if (x.Z < y.Z)
                {
                    return 1;
                }
                return -1;
            }
            return 0;
        }
    }
}
