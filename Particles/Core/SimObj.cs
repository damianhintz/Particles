using System;
using System.Text;
using System.Windows;
using System.Drawing;

namespace Particles
{
    abstract class SimObj
    {
        public bool RenderOn;
        public bool RenderStatOn;
        public bool SimulateOn;
        public Pen Pen;
        public Font Font;

        public double Restitution;

        protected SimObj()
        {
            RenderOn = true;
            RenderStatOn = false;
            SimulateOn = true;
            Pen = new Pen(Brushes.Black);
            Font = new Font(FontFamily.GenericMonospace, 8);
            Restitution = 1.0f;
        }

        public abstract void Render(Graphics g);

        public virtual void Mark()
        {
            Pen = new Pen(Brushes.Red);
            RenderStatOn = true;
        }

        public virtual void Unmark()
        {
            Pen = new Pen(Brushes.Black);
            RenderStatOn = false;
        }
    }
}
