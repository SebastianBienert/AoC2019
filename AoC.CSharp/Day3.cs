using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC.CSharp
{
    public class Coords
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Coords(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Coords Right => new Coords(X + 1, Y);

        private sealed class XYEqualityComparer : IEqualityComparer<Coords>
        {
            public bool Equals(Coords x, Coords y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return x.X == y.X && x.Y == y.Y;
            }

            public int GetHashCode(Coords obj)
            {
                unchecked
                {
                    return (obj.X * 397) ^ obj.Y;
                }
            }
        }

        public static IEqualityComparer<Coords> XYComparer { get; } = new XYEqualityComparer();

        public Coords Left => new Coords(X - 1, Y);
        public Coords Up => new Coords(X, Y + 1);
        public Coords Down => new Coords(X, Y - 1);

        public Coords GetNextCord(char dir)
        {
            switch (dir)
            {
                case 'R':
                    return Right;
                case 'L':
                    return Left;
                case 'U':
                    return Up;
                case 'D':
                    return Down;
                default:
                    return Right;
            }
        }
    }
    public class Day3
    {
        public static int Solution1()
        {
            var input = File.ReadAllText("data/day3.txt")
                .Split(new[] {Environment.NewLine}, StringSplitOptions.None)
                .Select(path => path.Split(',').ToList())
                .ToList();

            var firstPath = GetCoordinatesFromPath(input[0]);
            var secondPath = GetCoordinatesFromPath(input[1]);

            var intersection = firstPath.Intersect(secondPath, Coords.XYComparer);
            var min = intersection.Min(p => Math.Abs(p.X) + Math.Abs(p.Y));

            return min;
        }

        public static int Solution2()
        {
            var input = File.ReadAllText("data/day3.txt")
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .Select(path => path.Split(',').ToList())
                .ToList();

            var firstPath = GetCoordinatesFromPath(input[0]);
            var secondPath = GetCoordinatesFromPath(input[1]);

            var intersection = firstPath.Intersect(secondPath, Coords.XYComparer);
            var intersectionMap = intersection.ToDictionary(x => x, v =>
            {
                var first = firstPath.FindIndex(p => p.X == v.X && p.Y == v.Y);
                var second = secondPath.FindIndex(p => p.X == v.X && p.Y == v.Y);
                return first + second + 2;
            });
            var min = intersectionMap.Min(x => x.Value);

            return min;
        }

        public static List<Coords> GetCoordinatesFromPath(List<string> path)
        {
            var coords = new List<Coords>();
            var currentCoord = new Coords(0, 0);
            foreach (var step in path)
            {
                var dir = step[0];
                var distance = Convert.ToInt32(step.Substring(1));
                coords.AddRange(GenerateCoords(dir, distance, currentCoord));
                currentCoord = coords.Last();
            }

            return coords;
        }

        private static List<Coords> GenerateCoords(char dir, int dist, Coords start)
        {
            List<Coords> coords = new List<Coords>();
            Coords currentCoord = start;

            for (int i = 0; i < dist; i++)
            {
                currentCoord = currentCoord.GetNextCord(dir);
                coords.Add(currentCoord);
            }

            return coords;
        }    
    }
}
