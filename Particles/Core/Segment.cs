using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Particles
{
    class Segment : Line
    {
        public Particle Particle1, Particle2;

        public Segment(Vector p1, Vector p2)
            : base(p1, p2)
        {
            Particle1 = new Particle();
            Particle2 = new Particle();
            Particle1.Position = p1;
            Particle2.Position = p2;
            Particle1.Radius = 0.1;
            Particle2.Radius = 0.1;
            Particle1.Mass = double.PositiveInfinity;
            Particle2.Mass = double.PositiveInfinity;
            Particle1.SimulateOn = false;
            Particle2.SimulateOn = false;

            World.AddParticle(Particle1);
            World.AddParticle(Particle2);
        }

        public override void Render(Graphics g)
        {
            if (RenderOn)
            {
                Vector p1 = Point1;
                Vector p2 = Point2;
                g.DrawLine(this.Pen, (int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y);
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
