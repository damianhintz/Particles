using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Particles
{
    static class World
    {
        public static double Friction = 0.5f;   //Współczynnik siły tarcia Ff = f*N
        public static double Gravity = 9.81f;   //Siła grawitacji
        public static double AirDrag = 0.1f;    //Współczynnik oporu powietrza Fa = -a*V lub Fa = -a*V^2
        public static double Dt = 0.01;         //Krok czasowy

        public static Vector Wind = Vector.Empty;   //Siła wiatru

        public static List<Line> Lines = new List<Line>();
        public static List<Particle> Particles = new List<Particle>();
        public static List<ParticleConnection> Connections = new List<ParticleConnection>();

        public static void AddParticle(Particle particle)
        {
            Particles.Add(particle);
        }

        public static void SetForce(Particle p)
        {
            Vector force = Vector.Empty;
            Vector gravity = new Vector(0, Gravity);
            force += p.Mass * gravity;
            Vector drag = -AirDrag * p.Velocity;
            force += drag;
            force += Wind;
            p.Force += force;
        }

        /// <summary>
        /// Oblicz siły działające na cząstki
        /// </summary>
        public static void CalculateForces()
        {
            foreach (Particle p in Particles)
                SetForce(p);
            foreach (ParticleConnection c in Connections)
                c.Apply();
        }

        /// <summary>
        /// Rozwiąż kolizje
        /// </summary>
        public static void ResolveCollisions()
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                for (int j = 0; j < Particles.Count; j++)
                {
                    if (Lines[i] is Segment)
                        Collision.TryCollision(Particles[j], (Segment)Lines[i]);
                    else
                        Collision.TryCollision(Particles[j], Lines[i]);                        
                }
            }
            for (int i = 0; i < Particles.Count - 1; i++)
            {
                for (int j = i + 1; j < Particles.Count; j++)
                {
                    Collision.TryCollision(Particles[i], Particles[j]);
                }
            }
        }

        /// <summary>
        /// Całkowanie metodą Eulera
        /// </summary>
        /// <param name="dt"></param>
        public static void Euler(Particle p, double dt)
        {
            Vector f = p.Force;
            Vector a = f / p.Mass;
            p.Velocity += a * dt;
            p.Position += p.Velocity * dt;
        }

        public static void ResetForces()
        {
            foreach (Particle particle in Particles)
            {
                particle.Force = Vector.Empty;
            }
        }

        public static void DoStep()
        {
            ResetForces();

            //Najpierw poszukaj kolizji, które mogłyby wprowadzić nowe siły.
            ResolveCollisions();

            //Dodaj znane siły
            CalculateForces();
            
            //Oblicz nowe położenia i prędkości cząstek
            foreach (Particle particle in Particles)
            {
                if (particle.SimulateOn)
                    Euler(particle, Dt);
            }
        }

        public static void Render(Graphics g)
        {
            foreach (Line p in Lines)
                p.Render(g);
            foreach (Particle p in Particles)
                p.Render(g);
            foreach (ParticleConnection c in Connections)
                c.Render(g);
        }

        public static void BuildCircle(int x, int y, int r, int R)
        {
            Particle mid = new Particle();
            mid.Position = new Vector(x, y);
            mid.Radius = R;
            World.AddParticle(mid);

            Particle prev = null;
            Particle first = null;
            Vector rv = new Vector(r, 0);

            int k = (int)((2 * 3.14 * r) / (mid.Radius * 3));
            k += 1;
            for (int i = 0; i < k; i++)
            {
                Particle p = new Particle();
                p.Radius = R;
                p.Mass = 1;
                if (first == null) first = p;
                p.Position = new Vector(x + rv.X, y + rv.Y);
                Vector n = rv.Orthogonal();
                n.Normalize();
                n *= (p.Radius * 3);
                rv += n;
                rv.Normalize();
                rv *= r;
                World.AddParticle(p);
                ParticleConnection cm = new ParticleConnection(mid, p);
                //cm.RestLength = r - r / 8;
                cm.SpringConstant = 200;
                //cm.DampingConstant = 100;
                World.Connections.Add(cm);

                if (prev != null)
                {
                    ParticleConnection c = new ParticleConnection(prev, p);
                    c.RestLength /= 1.1;
                    c.SpringConstant = 300;
                    c.DampingConstant = 40;
                    World.Connections.Add(c);
                }
                prev = p;
            }
            ParticleConnection cc = new ParticleConnection(prev, first);
            cc.SpringConstant = 300;
            cc.DampingConstant = 40;
            cc.RestLength /= 1.1;
            //cc.RestLength = (prev.Position - first.Position).Length;
            World.Connections.Add(cc);
        }

        public static void BuildChain(int x, int y, int l, int r, int m, bool xory, bool pin)
        {
            Particle prev = null;
            Particle last = null;
            int xx = x;
            int yy = y;
            for (int i = 0; true; i++)
            {
                Particle p = new Particle();
                World.AddParticle(p);
                p.Radius = r;
                p.Position = new Vector(x, y);
                p.Mass = m;
                if (prev != null)
                {
                    ParticleConnection c = new ParticleConnection(prev, p);
                    c.SpringConstant = 2000;
                    c.RestLength -= p.Radius * 2;
                    World.Connections.Add(c);
                }

                if (i == 0)
                {
                    p.SimulateOn = false;
                    p.Mass = double.PositiveInfinity;
                }

                last = p;
                prev = p;
                if (xory)
                {
                    x += 4 * r;
                    if (x > xx + l)
                        break;
                }
                else
                {
                    y += 4 * r;
                    if (y > yy + l)
                        break;
                }
                
            }
            if (pin && last != null)
            {
                last.SimulateOn = false;
                last.Mass = double.PositiveInfinity;
            }
        }

        public static void BuildSquare(int x, int y, int a, int b)
        {
            Particle p1 = new Particle();
            p1.Position = new Vector(x, y);
            Particle p2 = new Particle();
            p2.Position = new Vector(x + a, y);
            Particle p3 = new Particle();
            p3.Position = new Vector(x + a, y + b);
            Particle p4 = new Particle();
            p4.Position = new Vector(x, y + b);
            p1.Radius = p2.Radius = p3.Radius = p4.Radius = 2;
            World.AddParticle(p1);
            World.AddParticle(p2);
            World.AddParticle(p3);
            World.AddParticle(p4);
            ParticleConnection c12 = new ParticleConnection(p1, p2);
            ParticleConnection c23 = new ParticleConnection(p2, p3);
            ParticleConnection c34 = new ParticleConnection(p3, p4);
            ParticleConnection c41 = new ParticleConnection(p4, p1);
            ParticleConnection c24 = new ParticleConnection(p2, p4);
            ParticleConnection c13 = new ParticleConnection(p1, p3);

            c12.RenderOn = true;
            c23.RenderOn = true;
            c34.RenderOn = true;
            c41.RenderOn = true;
            World.Connections.Add(c12);
            World.Connections.Add(c23);
            World.Connections.Add(c34);
            World.Connections.Add(c41);
            World.Connections.Add(c24);
            World.Connections.Add(c13);
        }

        public static void BuildWall(int x, int y, int h, int w, int r, bool box)
        {
            int n = w / (2 * r);
            int m = h / (2 * r);
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Particle p = new Particle();
                    p.Radius = r;
                    p.Position = new Vector(x + j * r * 2, y + i * r * 2);
                    p.Mass = 1;
                    World.AddParticle(p);
                }
            }
            if (box)
            {
                Line line1 = new Line(new Vector(x, y), new Vector(x, y + h));
                Line line2 = new Line(new Vector(x, y + h), new Vector(x + w, y + h));
                Line line3 = new Line(new Vector(x + w, y + h), new Vector(x + w, y));
                World.Lines.Add(line1);
                World.Lines.Add(line2);
                World.Lines.Add(line3);
            }
        }

        public static void BuildRope(int x, int y, int len, double e, double m, double r)
        {
            Particle prev = null;
            //Particle last = null;
            int X = x;
            for (int i = 0; x < X + len; i++)
            {
                Particle p = new Particle();
                World.AddParticle(p);
                p.Radius = r;
                p.Position = new Vector(x, y);
                p.Mass = m;
                p.Restitution = e;
                if (prev != null)
                {
                    ParticleConnection c = new ParticleConnection(prev, p);
                    c.SpringConstant = 300;
                    c.RestLength = p.Radius * 1.3;
                    c.DampingConstant = 50;
                    World.Connections.Add(c);
                }
                prev = p;
                x += (int)(2 * r);
            }
        }

    }
}
