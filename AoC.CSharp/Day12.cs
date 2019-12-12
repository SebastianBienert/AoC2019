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

        public static long Solution2()
        {
            var listOfMoons = new List<Moon>()
            {
                new Moon(new Vectord3D(-9, 10, -1)),
                new Moon(new Vectord3D(-14, -8, 14)),
                new Moon(new Vectord3D(1, 5, 6)),
                new Moon(new Vectord3D(-19, 7, 8))
            };

            var xSteps = 0l;
            do
            {
                for (int i = 0; i < listOfMoons.Count; i++)
                {
                    for (int j = i + 1; j < listOfMoons.Count; j++)
                    {
                        listOfMoons[i].ApplyGravityX(listOfMoons[j]);
                    }
                }

                xSteps++;
                listOfMoons.ForEach(x => x.ApplyVelocityX());
            } while (!listOfMoons.All(m => m.IsOriginalX));

            var ySteps = 0l;
            do
            {
                for (int i = 0; i < listOfMoons.Count; i++)
                {
                    for (int j = i + 1; j < listOfMoons.Count; j++)
                    {
                        listOfMoons[i].ApplyGravityY(listOfMoons[j]);
                    }
                }

                ySteps++;
                listOfMoons.ForEach(x => x.ApplyVelocityY());
            } while (!listOfMoons.All(m => m.IsOriginalY));

            var zSteps = 0l;
            do
            {
                for (int i = 0; i < listOfMoons.Count; i++)
                {
                    for (int j = i + 1; j < listOfMoons.Count; j++)
                    {
                        listOfMoons[i].ApplyGravityZ(listOfMoons[j]);
                    }
                }

                zSteps++;
                listOfMoons.ForEach(x => x.ApplyVelocityZ());
            } while (!listOfMoons.All(m => m.IsOriginalZ));

            var result = Lcm(xSteps, Lcm(ySteps, zSteps));

            return result;
        }

        static long Gcf(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static long Lcm(long a, long b)
        {
            return (a / Gcf(a, b)) * b;
        }

        public class Moon
        {
            private readonly Vectord3D _initialPosition;
            public Vectord3D Velocity { get; set; }
            public Vectord3D Position { get; set; }
            public int PotentialEnergy => Math.Abs(Position.X) + Math.Abs(Position.Y) + Math.Abs(Position.Z);
            public int KineticEnergy => Math.Abs(Velocity.X) + Math.Abs(Velocity.Y) + Math.Abs(Velocity.Z);
            public int TotalEnergy => PotentialEnergy * KineticEnergy;
            public bool IsOriginalX => Position.X == _initialPosition.X && Velocity.X == 0;
            public bool IsOriginalY => Position.Y == _initialPosition.Y && Velocity.Y == 0;
            public bool IsOriginalZ => Position.Z == _initialPosition.Z && Velocity.Z == 0;

            public Moon(Vectord3D position)
            {
                Velocity = new Vectord3D(0, 0, 0);
                _initialPosition = new Vectord3D(position.X, position.Y, position.Z);
                Position = position;   
            }

            public void ApplyVelocity()
            {
                ApplyVelocityX();
                ApplyVelocityY();
                ApplyVelocityZ();
            }

            public void ApplyVelocityX()
            {
                Position.X += Velocity.X;
            }
            public void ApplyVelocityY()
            {
                Position.Y += Velocity.Y;
            }
            public void ApplyVelocityZ()
            {
                Position.Z += Velocity.Z;
            }

            public void ApplyGravityX(Moon other)
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
            }

            public void ApplyGravityY(Moon other)
            {
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
            }

            public void ApplyGravityZ(Moon other)
            {
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

            public void ApplyGravity(Moon other)
            {
                ApplyGravityX(other);
                ApplyGravityY(other);
                ApplyGravityZ(other);
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
