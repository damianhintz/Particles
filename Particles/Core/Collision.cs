using System;
using System.Collections.Generic;
using System.Text;

namespace Particles
{
    class Collision
    {
        public static bool TryCollision(Particle p1, Particle p2)
        {
            if((p1.Position - p2.Position).Length > (p1.Radius + p2.Radius))
                return false;

            Vector n = (p1.Position - p2.Position);
            double d = (p1.Radius + p2.Radius) - n.Length;
            n.Normalize();
            
            Vector vr = p1.Velocity - p2.Velocity;

            //double m1 = p1.Mass, m2 = p2.Mass;
            //if (!p1.SimulateOn)                p1.Mass = double.PositiveInfinity;
            //if (!p2.SimulateOn)                p2.Mass = double.PositiveInfinity;

            double e = Math.Min(p1.Restitution, p2.Restitution);
            double j = (-vr * n) * (e + 1) / (1 / p1.Mass + 1 / p2.Mass);

            p1.Velocity += (j * n) / p1.Mass;
            p2.Velocity -= (j * n) / p2.Mass;

            if (d > 0)
            {
                if (p1.SimulateOn)
                    p1.Position += (d / 2) * n;
                if (p2.SimulateOn)
                    p2.Position -= (d / 2) * n;
            }

            //if (!p1.SimulateOn)                p1.Mass = m1;
            //if (!p2.SimulateOn)                p2.Mass = m2;
            //Console.Beep(200, 30);
            return true;
        }

        public static bool TryCollision(Particle p, Line line)
        {
            Vector L = p.Position - line.Point1;
            Vector u = line.Point2 - line.Point1; u.Normalize();

            double d = Math.Abs(L.OrthogonalDistance(u));
            if(d > p.Radius)
                return false;

            Vector v = p.Velocity;
            Vector x = (v * u) * u;
            
            Vector n = u.Orthogonal(); n.Normalize();
            
            if (Vector.Direction(line.Point1, line.Point2, p.Position) < 0)
                n = -n;

            Vector y = (v * n) * n;

            double e = Math.Min(p.Restitution, line.Restitution);
            p.Velocity -= (2 * y) * e;

            Vector G = p.Mass * new Vector(0, -World.Gravity);
            //p.Force += G;

            //Vector F = (G * u) * u;
            Vector N = (G * n) * n;
            //p.Force += N;

            //? czy jest ok ?
            if ((n ^ p.Velocity) > 0)
                u = -u;

            Vector T = 10000.9 * (N * u) * u;
            //p.Force += T;

            p.Position += (p.Radius - d) * -n;
            
            return true;
        }

        public static bool TryCollision(Particle p, Segment s)
        {
            Vector L = p.Position - s.Point1;
            Vector u = s.Point2 - s.Point1; u.Normalize();

            double d = Math.Abs(L.OrthogonalDistance(u));
            if (d > p.Radius)
                return false;

            Vector v = p.Velocity;
            Vector x = (v * u) * u;

            Vector n = u.Orthogonal(); n.Normalize();

            if (Vector.Direction(s.Point1, s.Point2, p.Position) < 0)
                n = -n;

            Vector s1 = s.Point1 + n;
            Vector s2 = s.Point2 + n;
            Vector p1 = p.Position - s.Point1;
            Vector p2 = p.Position - s.Point2;

            Vector mid = s.Middle();
            if ((mid - p.Position).Length > s.Length / 2)  //nie ma zderzenia z odcinkiem
                return false;

            //if ((s.Point1 - p.Position).Length > p.Radius && (s.Point2 - p.Position).Length > p.Radius)
              //  return false;

            Vector y = (v * n) * n;

            double e = Math.Min(p.Restitution, s.Restitution);
            p.Velocity -= (2 * y) * e;

            Vector G = p.Mass * new Vector(0, -World.Gravity);
            //p.Force += G;

            //Vector F = (G * u) * u;
            Vector N = (G * n) * n;
            //p.Force += N;

            if ((n ^ p.Velocity) > 0)
                u = -u;

            Vector T = 10000.9 * (N * u) * u;
            //p.Force += T;

            p.Position += (p.Radius - d) * -n;
            
            return true;
        }
    }
}
