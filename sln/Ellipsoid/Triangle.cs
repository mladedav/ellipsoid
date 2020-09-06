namespace Ellipsoid
{
    public struct Triangle
    {
        public Triangle(Point3d a, Point3d b, Point3d c)
        {
            A = a;
            B = b;
            C = c;
        }

        public Point3d A { get; }
        public Point3d B { get; }
        public Point3d C { get; }
    }
}
