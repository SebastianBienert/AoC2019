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
            var precision = 0.0001;
            return Math.Abs(x.X - y.X) < precision && Math.Abs(x.Y - y.Y) < precision;
        }

        public int GetHashCode(Vector obj)
        {
            //WTF? why obj.X.HashCode() + obj.y.Hashcode() does not work
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

            var asteroids = input.SelectMany(x => x).Where(x => x.Value == "#").ToList();

            var asteroidsSight = asteroids.Select(x => new {Asteroid = x, SightCount = GetNumberOfAsteroidsInSight(input, x)}).ToList();

            var answer = asteroidsSight.Max(x => x.SightCount);


            return answer;
        }



        public static int GetNumberOfAsteroidsInSight(List<List<Coord>> map, Coord initAsteroid)
        {
            var otherAsteroids = map.SelectMany(x => x).Where(x => x.Value == "#" && !(x.X == initAsteroid.X && x.Y == initAsteroid.Y)).ToList();
            var otherSteroidsLines = otherAsteroids.Select(x =>
            {
                var lineCoefficient = (double) (initAsteroid.Y - x.Y) / (initAsteroid.X - x.X);
                var vector = new Vector(initAsteroid.X - x.X, initAsteroid.Y - x.Y);
                var vectorNormalized = vector.GetNormalized();
                //var dir = (initAsteroid.X - x.X) < 0 ? -1 : 1;
                //vectorNormalizedlineCoefficient *= dir;
                var manhattanDistance = Math.Abs(x.Y - initAsteroid.Y) + Math.Abs(x.X - initAsteroid.X);
                return new {LineCoefficient = lineCoefficient, Distance = manhattanDistance, VectorNormalized = vectorNormalized };
            })
            .ToList();

            var groups = otherSteroidsLines
                .GroupBy(x => x.VectorNormalized, new VectorEqualityComparer())
                .ToList();

            var answer = groups.Count;

            return answer;

        }

        public static int Solution2()
        {
            return 0;
        }
    }
}
