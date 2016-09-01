using System;
using System.Drawing;
using System.Windows.Forms;

namespace Particles
{
    /// <summary>
    /// Siła sprężystosci Fs = ks * (L-r)
    /// Siła tłumienia Ft = kt * (Vv1-Vv2)
    /// Siła wypadkowa F1 = -(ks*(L-r)+kt*((Vv1-Vv2)*Lv)/L)*Lv/L, F2 = -F1
    /// </summary>
    class ParticleConnection : SimObj
    {
        public Segment Segment = null;
        public Particle Particle1, Particle2;

        public double RestLength;
        public double SpringConstant;
        public double DampingConstant;

        public double Length
        {
            get { return (Particle1.Position - Particle2.Position).Length; }
        }

        public ParticleConnection(Particle p1, Particle p2)
            : base()
        {
            Particle1 = p1;
            Particle2 = p2;
            RestLength = Length;
            SpringConstant = 10;
            DampingConstant = 1;

            //MakeSolid();
            RenderOn = false;
        }

        private const int D = 3;
        public void MakeSolid()
        {
            if (Segment != null)
                return;
            Vector u = Particle1.Position - Particle2.Position;
            u.Normalize();
            Segment = new Segment(Particle1.Position - u * (Particle1.Radius + D), Particle2.Position + u * (Particle2.Radius + D));
            World.Lines.Add(Segment);
        }

        public virtual void Apply()
        {
            double ks = SpringConstant; double kt = DampingConstant;
            Vector Lv = Particle1.Position - Particle2.Position;
            double L = Lv.Length; double r = RestLength;
            Vector v1 = Particle1.Velocity; Vector v2 = Particle2.Velocity;
            Vector F1 = -(ks * (L - r) + kt * ((v1 - v2) * Lv) / L) * Lv / L;
            Particle1.Force += F1;
            Particle2.Force -= F1;

            if (Segment != null)
            {
                Vector u = Particle1.Position - Particle2.Position;
                u.Normalize();
                Segment.Point1 = Particle1.Position - u * (Particle1.Radius + D);
                Segment.Point2 = Particle2.Position + u * (Particle2.Radius + D);
                Segment.Particle1.Position = Particle1.Position - u * (Particle1.Radius + D);
                Segment.Particle2.Position = Particle2.Position + u * (Particle2.Radius + D);
            }
        }

        public override void Render(Graphics g)
        {
            if (RenderOn)
            {
                g.DrawLine(Pen, 
                    (int)Particle1.Position.X, (int)Particle1.Position.Y, 
                    (int)Particle2.Position.X, (int)Particle2.Position.Y);
            }
            if (RenderStatOn)
            {
                float x = (float)((Particle1.Position.X + Particle2.Position.X) / 2);
                float y = (float)((Particle1.Position.Y + Particle2.Position.Y) / 2);
                string text = string.Format("L={0:F1} R={1:F1} C={2:F1} D={3:F1}", 
                    Length, RestLength, SpringConstant, DampingConstant);
                g.DrawString(text, Font, Brushes.Crimson, x, y);
            }
        }
    }
}
