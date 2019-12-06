using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC.CSharp
{
    public class Node
    {
        public string Name { get; set; }
        public string Parent { get; set; }
        public Node(string name, string parent)
        {
            Name = name;
            Parent = parent;
        }
    }
    public static class Day6
    {

        public static List<Node> PLANETS = new List<Node>();
       
        public static int Solution1()
        {
            PLANETS = File.ReadAllText("data/day6.txt")
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .Select(line =>
                {
                    var split = line.Split(')');
                    return new Node(split[1], split[0]);
                })
                .ToList();

            var result = PLANETS.Sum(GetNodePoints);

            return result;
        }

        public static int Solution2()
        {
            PLANETS = File.ReadAllText("data/day6.txt")
                .Split(new[] { Environment.NewLine }, StringSplitOptions.None)
                .Select(line =>
                {
                    var split = line.Split(')');
                    return new Node(split[1], split[0]);
                })
                .ToList();

            var youParentPlanet = PLANETS.First(x => x.Name == PLANETS.First(p => p.Name == "YOU").Parent);
            var sanParentPlanet = PLANETS.First(x => x.Name == PLANETS.First(p => p.Name == "SAN").Parent);

            var result = FindShortestPath(youParentPlanet, sanParentPlanet);

            return result;
        }


        private static int FindShortestPath(Node x, Node y)
        {
            var xAncestors = GetAncestors(x);
            var yAncestors = GetAncestors(y);
            var commonAncestors = xAncestors.Intersect(yAncestors);
            var minimumAncestors = commonAncestors.Max(a => GetNodePoints(PLANETS.First(p => p.Name == a)));
            var xDistance = GetNodePoints(x);
            var yDistance = GetNodePoints(y);

            var result = xDistance + yDistance - (2 * minimumAncestors);
            return result;
        }

        private static List<string> GetAncestors(Node node)
        {
            var currentNode = node;
            var list = new List<string>();
            while (currentNode.Parent != "COM")
            {
                list.Add(currentNode.Parent);
                currentNode = PLANETS.First(x => x.Name == currentNode.Parent);
            }

            return list;
        }

        private static int GetNodePoints(Node node)
        {
            var currentNode = node;
            var sum = 1;
            while (currentNode.Parent != "COM")
            {
                sum++;
                currentNode = PLANETS.First(x => x.Name == currentNode.Parent);
            }

            return sum;
        }


        
    }
}
