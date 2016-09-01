using System;
using System.Text;
using System.Drawing;

namespace Particles
{
    class Particle : SimObj
    {
        public double Mass;                 //Masa
        public Vector Velocity;             //Wektor prędkości
        public Vector Position;             //Położenie w zewnętrznym układzie
        public Vector Force;                //Siła
        public double Radius;               //Promień

        public Particle() : base()
        {
            Mass = 10.0f;
            Force = Vector.Empty;
            Position = Vector.Empty;
            Velocity = Vector.Empty;
            Radius = 10.0f;
        }

        public Particle(double mass, Vector position, Vector velocity, double radius)
            : base()
        {
            Mass = mass;
            Position = position;
            Velocity = velocity;
            Force = Vector.Empty;
            Radius = radius;
        }

        public void Pin()
        {
            SimulateOn = false;
            Mass = double.PositiveInfinity;
        }

        public void Unpin()
        {
            SimulateOn = true;
            Mass = 1;
        }

        public override void Render(Graphics g)
        {
            if (RenderOn)
            {
                g.DrawEllipse(Pen, (float)(Position.X - Radius), (float)(Position.Y - Radius), (float)(2 * Radius), (float)(2 * Radius));
                Vector n = new Vector(Velocity.X, Velocity.Y); n.Normalize(); n *= Radius;
                //g.DrawLine(Pen, (int)Position.X, (int)Position.Y, (int)n.X + (int)Position.X, (int)n.Y + (int)Position.Y);
            }
            if (RenderStatOn)
            {
                string text = string.Format("M={0:F1} V={1:F1} E={2:F1}", Mass, Velocity.Length, Restitution);
                g.DrawString(text, Font, Brushes.Crimson, (float)Position.X, (float)Position.Y);
            }
        }

    }
}
