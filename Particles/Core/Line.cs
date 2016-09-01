using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Particles
{
    class Line : SimObj
    {
        public Vector Point1, Point2;
    
        public Line(Vector p1, Vector p2)
        {
            Point1 = p1;
            Point2 = p2;
            SimulateOn = false;
        }

        public double Length
        {
            get { return (Point1 - Point2).Length; }
        }

        public double Distance(Vector v)
        {
            Vector u = Point2 - Point1; u.Normalize();
            Vector L = v - Point1;
            return Math.Abs(L.OrthogonalDistance(u));
        }

        public Vector Middle()
        {
            return new Vector((Point1.X + Point2.X) / 2, (Point1.Y + Point2.Y) / 2);
        }


        public override void Render(Graphics g)
        {
            if (RenderOn)
            {
                Vector u = Point1 - Point2;
                u.Normalize();
                Vector p1 = Point1 - 1000 * u;
                Vector p2 = Point2 + 1000 * u;
                g.DrawLine(Pen, (int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y);
            }
            if (RenderStatOn)
            {
                Vector mid = Middle();
                string text = string.Format("E={0:F1}", Restitution);
                g.DrawString(text, Font, Brushes.Crimson, (float)mid.X, (float)mid.Y);
            }
        }
    }
}
