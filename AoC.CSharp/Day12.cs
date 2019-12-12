using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AoC.CSharp
{
    public class Day12
    {
        public static int Solution1()
        {
            var listOfMoons = new List<Moon>()
            {
                new Moon(new Vectord3D(-9, 10, -1)),
                new Moon(new Vectord3D(-14, -8, 14)),
                new Moon(new Vectord3D(1, 5, 6)),
                new Moon(new Vectord3D(-19, 7, 8))
            };

            var timeSteps = 1000;
            foreach (var step in Enumerable.Range(0, timeSteps))
            {
                //listOfMoons.ForEach(x => Console.WriteLine(x));
                //Console.WriteLine();
                //APply gravity
                for (int i = 0; i < listOfMoons.Count; i++)
                {
                    for (int j = i + 1; j < listOfMoons.Count; j++)
                    {
                        listOfMoons[i].ApplyGravity(listOfMoons[j]);
                    }
                }

                listOfMoons.ForEach(x => x.ApplyVelocity());  
            }

            var result = listOfMoons.Sum(x => x.TotalEnergy);

            return result;
        }

        public static int Solution2()
        {
            var listOfMoons = new List<Moon>()
            {
                new Moon(new Vectord3D(-9, 10, -1)),
                new Moon(new Vectord3D(-14, -8, 14)),
                new Moon(new Vectord3D(1, 5, 6)),
                new Moon(new Vectord3D(-19, 7, 8))
            };

            var timeSteps = 1000;
            foreach (var step in Enumerable.Range(0, timeSteps))
            {
                //listOfMoons.ForEach(x => Console.WriteLine(x));
                //Console.WriteLine();
                //APply gravity
                for (int i = 0; i < listOfMoons.Count; i++)
                {
                    for (int j = i + 1; j < listOfMoons.Count; j++)
                    {
                        listOfMoons[i].ApplyGravity(listOfMoons[j]);
                    }
                }

                listOfMoons.ForEach(x => x.ApplyVelocity());
            }

            var result = listOfMoons.Sum(x => x.TotalEnergy);

            return result;
        }

        public class Moon
        {
            public Vectord3D Velocity { get; set; }
            public Vectord3D Position { get; set; }
            public int PotentialEnergy => Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z);
            public int KineticEnergy => Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);
            public int TotalEnergy => PotentialEnergy * KineticEnergy;

            public Moon(Vectord3D position)
            {
                Velocity = new Vectord3D(0, 0, 0);
                Position = position;
            }

            public void ApplyVelocity()
            {
                Position = new Vectord3D(Position.X + Velocity.X, Position.Y + Velocity.Y, Position.Z + Velocity.Z);
            }

            public void ApplyGravity(Moon other)
            {
                if (Position.X < other.Position.X)
                {
                    Velocity.X += 1;
                    other.Velocity.X -= 1;
                }
                else if (Position.X > other.Position.X)
                {
                    Velocity.X -= 1;
                    other.Velocity.X += 1;
                }

                if (Position.Y < other.Position.Y)
                {
                    Velocity.Y += 1;
                    other.Velocity.Y -= 1;
                }
                else if (Position.Y > other.Position.Y)
                {
                    Velocity.Y -= 1;
                    other.Velocity.Y += 1;
                }

                if (Position.Z < other.Position.Z)
                {
                    Velocity.Z += 1;
                    other.Velocity.Z -= 1;
                }
                else if (Position.Z > other.Position.Z)
                {
                    Velocity.Z -= 1;
                    other.Velocity.Z += 1;
                }
            }

            public override string ToString()
            {
                return $"pos=<x={Position.X}, y={Position.Y}, z={Position.Z}>, vel=<x={Velocity.X}, y={Velocity.Y}, z={Velocity.Z}> ENERGY: {TotalEnergy}";
            }

            public override bool Equals(object obj)
            {
                var moon = obj as Moon;
                return moon != null && Velocity.Equals(moon.Velocity) && Position.Equals(moon.Position);
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Velocity, Position);
            }
        }

        public class Vectord3D
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Z { get; set; }

            public Vectord3D(int x, int y, int z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public override bool Equals(object obj)
            {
                var d = obj as Vectord3D;
                return d != null &&
                       X == d.X &&
                       Y == d.Y &&
                       Z == d.Z;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(X, Y, Z);
            }
        }

        
    }
}
