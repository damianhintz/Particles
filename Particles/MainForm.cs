using System;
using System.Drawing;
using System.Windows.Forms;

namespace Particles
{
    public partial class simForm : Form
    {
        private Vector dv = new Vector();

        private Particle particle = null;
        private Line line = null;
        private ParticleConnection connection = null;

        private bool renderParticleSystem = true;

        public simForm()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (renderParticleSystem)
            {
                World.Render(e.Graphics);
                Font font = new Font(FontFamily.GenericMonospace, 9);

                string modeText = "";
                switch (mode)
                {
                    case Keys.G:
                        modeText = "Gravity";
                        break;
                    case Keys.T:
                        modeText = "Interval";
                        break;
                    case Keys.W:
                        modeText = "Wind/Width";
                        break;
                    case Keys.S:
                        modeText = "Step";
                        break;
                    case Keys.I:
                        modeText = "AirDrag";
                        break;
                    case Keys.V:
                        modeText = "Velocity";
                        break;
                    case Keys.E:
                        modeText = "Restitution";
                        break;
                    case Keys.M:
                        modeText = "Mass";
                        break;
                    case Keys.R:
                        modeText = "Radius";
                        break;
                    case Keys.H:
                        modeText = "Height";
                        break;
                    case Keys.D:
                        modeText = "Large radius";
                        break;
                    default:
                        modeText = "none";
                        break;
                }

                UpdateObject();
                //Render info
                string text = string.Format("Mode: {6}\nStart/[P]ause: {0}\n[S]tep: {1:F3}\nIn[t]erval: {2}\n[G]ravity: {3:F2}\n[W]ind: ({4:F2};{5:F2})\nA[i]r drag: {7:F2}\n[Fn]Object: {8}",
                    worldTimer.Enabled ? "On" : "Off", World.Dt, worldTimer.Interval,
                    World.Gravity, World.Wind.X, World.Wind.Y,
                    modeText, World.AirDrag, obj);
                e.Graphics.DrawString(text, font, Brushes.Crimson, 1, 1);
            }
        }

        private void worldTimer_Tick(object sender, EventArgs e)
        {
            World.DoStep();
            Invalidate();
        }

        Vector a = null;
        private void simForm_MouseDown(object sender, MouseEventArgs e)
        {
            if (particle != null) particle.Unmark();
            if (line != null) line.Unmark();
            if (connection != null) connection.Unmark();

            a = new Vector(e.X, e.Y);

            particle = null;
            foreach (Particle p in World.Particles)
            {
                if ((p.Position - a).Length < (p.Radius / 2) + 1)
                {
                    particle = p;
                    particle.Mark();
                    break;
                }
            }

            if (true)
            {
                line = null;
                foreach (Line l in World.Lines)
                {
                    if (l.Distance(a) < 5)
                    {
                        line = l;
                        line.Mark();
                        break;
                    }
                }
            }

            if (true)
            {
                connection = null;
                foreach (ParticleConnection c in World.Connections)
                {
                    Line temp = new Line(c.Particle1.Position, c.Particle2.Position);
                    if (temp.Distance(a) < 5)
                    {
                        connection = c;
                        connection.Mark();
                        break;
                    }
                }
            }
        }

        private void simForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (a == null)
                return;

            Vector b = new Vector(e.X, e.Y);
            Line line = null;
            if (this.mode == Keys.L)
                line = new Line(a, b);
            else
            {
                if ((a - b).Length > 40)
                    line = new Segment(a, b);
            }

            dv = b - a;
            if (particle != null)
            {
                Particle p2 = null;
                foreach (Particle p in World.Particles)
                    if ((p.Position - b).Length < p.Radius * 4)
                    {
                        p2 = p;
                        p2.Pen = new Pen(Brushes.Green, 1.0f);
                        break;
                    }
                if (p2 != null)
                {
                    if ((particle.Position - p2.Position).Length > 10)
                    {
                        ParticleConnection c = new ParticleConnection(particle, p2);
                        c.RestLength = dv.Length;
                        if (p2 != particle)
                            World.Connections.Add(c);
                    }
                }
                else
                    particle.Position += dv;

            }
            else
            {
                if (line != null && line.Length > 40)
                {

                    World.Lines.Add(line);
                }
            }
            Invalidate();
        }

        private void UpdateObject()
        {
            switch (name)
            {
                case "particle":
                    obj = string.Format("particle(m={0:F1},r={1:F1})", m, r);
                    break;
                case "circle":
                    obj = string.Format("circle(r={0:F1},d={1:F1})", r, d);
                    break;
                case "wall":
                    obj = string.Format("wall(h={0:F2},w={1:F2},r={2:F2})", h, w, r);
                    break;
                case "rope":
                    obj = string.Format("rope(l={0:F0},e={1:F2},m={2:F1},r={3:F1})", l, e, m, r);
                    break;
                case "square":
                    obj = string.Format("square(h={0:F0},w={1:F0})", h, w);
                    break;
                case "chain":
                    obj = string.Format("chain(l={0:F0},r={1:F0},m={2:F0})", l, r, m);
                    break;
                default:
                    break;
            }
        }

        private void simForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            switch (name)
            {
                case "particle":
                    Particle p = new Particle();
                    p.Position = new Vector(e.X, e.Y);
                    p.Mass = m;
                    p.Radius = r;
                    p.Mark();
                    World.AddParticle(p);
                    particle = p;
                    break;
                case "circle":
                    World.BuildCircle(e.X, e.Y, (int)d, (int)r);
                    break;
                case "wall":
                    World.BuildWall(e.X, e.Y, (int)h, (int)w, (int)r, false);
                    break;
                case "rope":
                    World.BuildRope(e.X, e.Y, (int)l, this.e, m, r);
                    break;
                case "square":
                    World.BuildSquare(e.X, e.Y, (int)h, (int)w);
                    break;
                case "chain":
                    World.BuildChain(e.X, e.Y, (int)l, (int)r, (int)m, true, true);
                    break;
                default:
                    break;
            }
            Invalidate();
        }

        private Keys mode = Keys.F1;
        private string obj = "null";
        private void simForm_KeyDown(object sender, KeyEventArgs e)
        {

            switch (e.KeyCode)
            {
                case Keys.Left:
                    switch (mode)
                    {
                        case Keys.M:
                            if (particle != null)
                            {
                                particle.Mass = 1;
                                particle.SimulateOn = true;
                            }
                            break;
                        case Keys.V:
                            if (particle != null) particle.Velocity += new Vector(-10, 0);
                            break;
                        case Keys.U:
                            if (particle != null) particle.Velocity = Vector.Empty;
                            break;
                        case Keys.W:
                            World.Wind += new Vector(-0.1, 0);
                            break;
                    }

                    break;
                case Keys.Right:
                    switch (mode)
                    {
                        case Keys.M:
                            if (particle != null)
                            {
                                particle.SimulateOn = false;
                                particle.Mass = double.PositiveInfinity;
                            }
                            break;
                        case Keys.V:
                            if (particle != null) particle.Velocity += new Vector(10, 0);
                            break;
                        case Keys.W:
                            World.Wind += new Vector(0.1, 0);
                            break;
                    }
                    break;
                case Keys.Up:
                    {
                        switch (mode)
                        {
                            case Keys.S:
                                World.Dt += 0.001;
                                break;
                            case Keys.I:
                                World.AirDrag += 0.01;
                                break;
                            case Keys.W:
                                World.Wind += new Vector(0, -0.1);
                                break;
                            case Keys.V:
                                if (particle != null) particle.Velocity += new Vector(0, -10);
                                break;
                            case Keys.M:
                                if (particle != null) particle.Mass += 1;
                                break;
                            case Keys.T:
                                worldTimer.Interval += 1;
                                break;
                            case Keys.G:
                                World.Gravity += 0.5;
                                break;
                            case Keys.R:
                                if (particle != null) particle.Radius += 2;
                                break;
                            case Keys.D:
                                if (connection != null) connection.DampingConstant += 0.1;
                                break;
                            case Keys.A:
                                if (line != null) line.Point2.Y += 1;
                                break;
                            case Keys.E:
                                if (line != null) line.Restitution += 0.1;
                                if (particle != null) particle.Restitution += 0.1;
                                break;
                            case Keys.C:
                                if (connection != null) connection.SpringConstant += 0.1;
                                break;
                        }
                        break;
                    }
                case Keys.Down:
                    {
                        switch (mode)
                        {
                            case Keys.W:
                                World.Wind += new Vector(0, 0.1);
                                break;
                            case Keys.S:
                                World.Dt -= 0.001;
                                break;
                            case Keys.I:
                                World.AirDrag -= 0.01;
                                break;
                            case Keys.V:
                                if (particle != null) particle.Velocity += new Vector(0, 10);
                                break;
                            case Keys.M:
                                if (particle != null) particle.Mass -= 1;
                                break;
                            case Keys.T:
                                if (worldTimer.Interval - 1 > 0)
                                    worldTimer.Interval -= 1;
                                break;
                            case Keys.G:
                                World.Gravity -= 0.5;
                                break;
                            case Keys.R:
                                if (particle != null) particle.Radius -= 2;
                                break;
                            case Keys.D:
                                if (connection != null) connection.DampingConstant -= 0.1;
                                break;
                            case Keys.A:
                                if (line != null) line.Point2.Y -= 1;
                                break;
                            case Keys.E:
                                if (line != null) line.Restitution -= 0.1;
                                if (particle != null) particle.Restitution -= 0.1;
                                break;
                            case Keys.C:
                                if (connection != null) connection.SpringConstant -= 0.1;
                                break;
                        }
                        break;
                    }
                case Keys.P:
                    worldTimer.Enabled = !worldTimer.Enabled;
                    mode = Keys.P;
                    break;
                case Keys.V:
                    mode = e.KeyCode;
                    break;
                case Keys.U:
                    mode = e.KeyCode;
                    break;
                case Keys.M:
                    mode = e.KeyCode;
                    break;
                case Keys.T:
                    mode = e.KeyCode;
                    break;
                case Keys.G:
                    mode = e.KeyCode;
                    break;
                case Keys.R:
                    mode = e.KeyCode;
                    break;
                case Keys.D:
                    mode = e.KeyCode;
                    break;
                case Keys.W:
                    mode = e.KeyCode;
                    break;
                case Keys.E:
                    mode = e.KeyCode;
                    break;
                case Keys.A:
                    mode = e.KeyCode;
                    break;
                case Keys.C:
                    mode = e.KeyCode;
                    break;
                case Keys.S:
                    mode = e.KeyCode;
                    break;
                case Keys.I:
                    mode = e.KeyCode;
                    break;
                case Keys.L:
                    if (mode != Keys.L)
                        mode = Keys.L;
                    else
                        mode = Keys.L;
                    //mode = e.KeyCode;
                    break;
                case Keys.Delete:
                    if (particle != null) World.Particles.Remove(particle);
                    if (line != null) World.Lines.Remove(line);
                    if (connection != null) World.Connections.Remove(connection);
                    break;
                case Keys.Q:
                    if (connection != null) connection.RenderOn = !connection.RenderOn;
                    break;
                case Keys.H:
                    mode = Keys.H;
                    break;
                case Keys.F1:
                    mode = e.KeyCode;
                    obj = string.Format("particle(m={0:F1},r={1:F1})", m, r);
                    name = "particle";
                    break;
                case Keys.F2:
                    mode = e.KeyCode;
                    obj = string.Format("circle(r={0:F1},d={1:F1})", r, d);
                    name = "circle";
                    break;
                case Keys.F3:
                    mode = e.KeyCode;
                    obj = string.Format("wall(h={0:F2},w={1:F2},r={2:F2})", h, w, r);
                    name = "wall";
                    break;
                case Keys.F4:
                    mode = e.KeyCode;
                    obj = string.Format("rope(l={0:F0},e={1:F2},m={2:F1},r={3:F1})", l, this.e, m, r);
                    name = "rope";
                    break;
                case Keys.F5:
                    mode = e.KeyCode;
                    obj = string.Format("square(h={0:F0},w={1:F0})", h, w);
                    name = "square";
                    break;
                case Keys.F6:
                    mode = e.KeyCode;
                    obj = string.Format("chain(l={0:F0},r={1:F0},m={2:F0})", l, r, m);
                    name = "chain";
                    break;
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    if (!isnum)
                        n = "";
                    n += e.KeyCode.ToString()[1];
                    Text = n + " -> then enter";
                    isnum = true;
                    break;
                case Keys.OemPeriod:
                case Keys.Oemcomma:
                    if (n.Contains(",") == false)
                    {
                        if (n.Length == 0)
                            n = "0";
                        n += ",";
                        Text = n + " -> then enter";
                    }
                    break;
                case Keys.Back:
                    if (n.Length > 0)
                        n = n.Remove(n.Length - 1);
                    Text = n + " -> then enter";
                    break;
                case Keys.Enter:
                    isnum = false;
                    switch (mode)
                    {
                        case Keys.V:

                            break;
                        case Keys.U:

                            break;
                        case Keys.M:
                            m = double.Parse(n);
                            break;
                        case Keys.T:
                            worldTimer.Interval = (int)double.Parse(n);
                            break;
                        case Keys.G:
                            World.Gravity = double.Parse(n);
                            break;
                        case Keys.R:
                            r = double.Parse(n);
                            break;
                        case Keys.W:
                            w = double.Parse(n);
                            break;
                        case Keys.E:
                            this.e = double.Parse(n);
                            break;
                        case Keys.D:
                            d = double.Parse(n);
                            break;
                        case Keys.S:
                            World.Dt = double.Parse(n);
                            break;
                        case Keys.H:
                            h = double.Parse(n);
                            break;
                        case Keys.I:
                            World.AirDrag = double.Parse(n);
                            break;
                        case Keys.L:
                            l = double.Parse(n);
                            break;
                    }

                    //KeyEventArgs ee = new KeyEventArgs(Keys.V);
                    //this.RaiseKeyEvent(this, ee);

                    break;
                default:
                    break;
            }
            Invalidate();
        }
        private string name = "particle";
        private bool isnum = false;
        //private string k = "";
        private string n = "";

        private double m = 1.0;
        private double r = 5;
        private double l = 100;
        private double h = 100;
        private double w = 100;
        private double d = 50;
        private double e = 1.0;
        //private double v = 1.0;
    }
}
