using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AoC.CSharpDay10
{
    public struct Coord
    {
        public int X { get; }
        public int Y { get; }
        public string Value { get; }
        public Coord(int x, int y, string value) : this()
        {
            X = x;
            Y = y;
            Value = value;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Coord))
            {
                return false;
            }

            var coord = (Coord)obj;
            return X == coord.X &&
                   Y == coord.Y &&
                   Value == coord.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Value);
        }
    }

    public struct Vector
    {
        public double X { get; }
        public double Y { get; }

        public Vector(double x, double y) : this()
        {
            X = x;
            Y = y;
        }

        public Vector GetNormalized()
        {
            var vectorDistance = Math.Sqrt(Math.Pow(Y, 2) + Math.Pow(X, 2));
            return new Vector(X / vectorDistance, Y / vectorDistance);
        }
    }

    public class VectorEqualityComparer : IEqualityComparer<Vector>
    {
        public bool Equals(Vector x, Vector y)
        {
            var precision = 0.00000000001;
            return Math.Abs(x.X - y.X) < precision && Math.Abs(x.Y - y.Y) < precision;
        }

        public int GetHashCode(Vector obj)
        {
            //THIS IS TRICKY
            return 1;
        }
    }
    public class Day10
    {
        public static int Solution1()
        {
            var input = File.ReadLines("data/day10.txt")
                .Select((line, y) => line.ToCharArray().Select((sign, x) => new Coord(x, y, sign.ToString())).ToList())
                .ToList();

            var answer = input.SelectMany(x => x).Where(x => x.Value == "#")
                              .Select(x => new { Asteroid = x, SightCount = GetNumberOfAsteroidsInSight(input, x) })
                              .Max(x => x.SightCount);

            return answer;
        }

        public static int Solution2()
        {
            var input = File.ReadLines("data/day10.txt")
                .Select((line, y) => line.ToCharArray().Select((sign, x) => new Coord(x, y, sign.ToString())).ToList())
                .ToList();

            var asteroidsSight = input
                .SelectMany(x => x).Where(x => x.Value == "#")
                .Select(x => new { Asteroid = x, SightCount = GetNumberOfAsteroidsInSight(input, x) })
                .ToList();

            var bestSightCount = asteroidsSight.Max(x => x.SightCount);

            var bestAsteroid = asteroidsSight.Where(x => x.SightCount == bestSightCount)
                                             .Select(x => x.Asteroid)
                                             .FirstOrDefault();

            var result = GetVaporizedAsteroids(input, bestAsteroid);
            return result;
        }

        public static int GetVaporizedAsteroids(List<List<Coord>> map, Coord bestAsteroid)
        {
            var queues = map
                .SelectMany(x => x)
                .Where(x => x.Value == "#" && !(x.X == bestAsteroid.X && x.Y == bestAsteroid.Y))
                .Select(x => new
                {
                    Distance = Math.Abs(x.Y - bestAsteroid.Y) + Math.Abs(x.X - bestAsteroid.X),
                    Angle = GetAngleNormalizedVectors(new Vector(0, -1), new Vector(x.X - bestAsteroid.X, x.Y - bestAsteroid.Y).GetNormalized()),
                    Coords = x
                })
                .GroupBy(x => x.Angle)
                .OrderBy(x => x.First().Angle)
                .Select(g => new Queue<Coord>(g.OrderBy(x => x.Distance).Select(x => x.Coords).ToList()))
                .ToList();

            int counter = 1;
            int index = 0;
            while(queues.Any(q => q.Count > 0))
            {
                var queue = queues[index % queues.Count];
                if (queue.Count > 0)
                {
                    var coord = queue.Dequeue();
                    Console.WriteLine($"#{counter} X: {coord.X}, Y: {coord.Y}");
                    counter++;
                    if (counter > 200)
                        return coord.X * 100 + coord.Y;
                }

                index++;
            }

            return -1;
        }

        public static double GetAngleNormalizedVectors(Vector x, Vector y)
        {
            var dotProduct = x.X * y.X + x.Y * y.Y;
            var det = x.X * y.Y - y.X * x.Y;
            var result = Math.Atan2(det, dotProduct) * (180.0 / Math.PI);
            return result < 0 ? result + 360 : result;
        }


        public static int GetNumberOfAsteroidsInSight(List<List<Coord>> map, Coord initAsteroid)
        {
            var groups = map.SelectMany(x => x)
                    .Where(x => x.Value == "#" && !(x.X == initAsteroid.X && x.Y == initAsteroid.Y))
                    .Select(x => new Vector(initAsteroid.X - x.X, initAsteroid.Y - x.Y)
                        .GetNormalized())
                    .GroupBy(x => x, new VectorEqualityComparer())
                    .ToList();

            return groups.Count;
        }


    }
}
