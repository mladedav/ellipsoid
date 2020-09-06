namespace Ellipsoid
{
    public struct Equation3d
    {
        public Equation3d(double x, double y, double z, double a)
        {
            X = x;
            Y = y;
            Z = z;
            A = a;
        }

        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public double A { get; }
    }
}
