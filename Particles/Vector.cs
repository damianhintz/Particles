using System;
using System.Drawing;

namespace Particles
{
    class Vector
    {
        public static readonly Vector Empty = new Vector();

        public double Length
        {
            get { return (double)Math.Sqrt(X * X + Y * Y); }
        }

        public double X, Y;

        public Vector()
        {
            X = 0.0f;
            Y = 0.0f;
        }

        public Vector(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public Vector(Vector vector)
        {
            this.X = vector.X;
            this.Y = vector.Y;
        }

        public void Normalize()
        {
            double length = Length;
            if (length > 0.0f)
            {
                X /= length;
                Y /= length;
            }
            else
            {
                X = 0.0f;
                Y = 0.0f;
            }
        }

        public static double Direction(Vector pi, Vector pj, Vector pk)
        {
            return (pk - pi) ^ (pj - pi);
        }

        public Vector Orthogonal()
        {
            return new Vector(-Y, X);
        }

        public double OrthogonalDistance(Vector v)
        {
            Vector n = new Vector(v);
            n.Normalize();
            return X * n.Y - Y * n.X;
        }

        public static Vector Normalize(Vector vector)
        {
            double length = vector.Length;
            if (length > 0.0f)
                return new Vector(vector.X / length, vector.Y / length);
            else
                return Vector.Empty;
        }

        public static Vector operator -(Vector lhs)
        {
            return new Vector(-lhs.X, -lhs.Y);
        }

        public static Vector operator +(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        public static Vector operator -(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        public static Vector operator *(Vector lhs, double rhs)
        {
            return new Vector(lhs.X * rhs, lhs.Y * rhs);
        }

        public static Vector operator *(double rhs, Vector lhs)
        {
            return new Vector(lhs.X * rhs, lhs.Y * rhs);
        }

        public static Vector operator /(Vector lhs, double rhs)
        {
            return new Vector(lhs.X / rhs, lhs.Y / rhs);
        }

        public static double Dot(Vector lhs, Vector rhs)
        {
            return lhs.X * rhs.X + lhs.Y * rhs.Y;
        }

        public static double operator *(Vector lhs, Vector rhs)
        {
            return lhs.X * rhs.X + lhs.Y * rhs.Y;
        }

        public static double operator ^(Vector lhs, Vector rhs)
        {
            return lhs.X * rhs.Y - rhs.X * lhs.Y;
        }

        public static double DistanceBetween(Vector lhs, Vector rhs)
        {
            return (lhs - rhs).Length;
        }

        public static explicit operator Vector(Point point)
        {
            return new Vector(point.X, point.Y);
        }

        public static explicit operator Point(Vector vector)
        {
            return new Point((int)vector.X, (int)vector.Y);
        }
    }
}
