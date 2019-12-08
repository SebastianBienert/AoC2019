using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC.CSharp
{
    public static class Day8
    {
        public static int WIDTH = 25;
        public static int HEIGHT = 6;

        public static int Solution1()
        {
            var data = File.ReadAllText("data/day8.txt")
                .ToCharArray()
                .Select((x, i) => new {Index = i, Value = Convert.ToInt32(x.ToString())})
                .GroupBy(x => x.Index / (WIDTH * HEIGHT))
                .Select((x, i) => new {Index = i, Value = x.Select(y => y.Value)})
                .ToList();

            var min = (Group: data[0], Value: Int32.MaxValue);
       
            foreach (var group in data)
            {
                var zeroCount = group.Value.Count(x => x == 0);
                if (zeroCount < min.Value)
                {
                    min = (Group: group, Value: zeroCount);
                }
            }

            var oneCount = min.Group.Value.Count(x => x == 1);
            var twoCount = min.Group.Value.Count(x => x == 2);


            return oneCount * twoCount;
        }

        public static int Solution2()
        {
            var data = File.ReadAllText("data/day8.txt")
                .ToCharArray()
                .Select((x, i) => new { Index = i, Value = Convert.ToInt32(x.ToString()) })
                .GroupBy(x => x.Index / (WIDTH * HEIGHT))
                .Select((x, i) => new { Index = i, Value = x.Select(y => y.Value).ToList() })
                .ToList();

            var finalPicture = Enumerable.Repeat(0, WIDTH * HEIGHT)
                .Select((x, i) => data.Select(y => y.Value[i])
                                      .First(y => y != 2))
                .ToList();

            PrintLayer(finalPicture, WIDTH, HEIGHT);

            return 0;
        }

        private static void PrintLayer(List<int> layer, int width, int height)
        {
            for (int i = 0; i < width * height; i++)
            {
                Console.ForegroundColor = layer[i] == 0 ? ConsoleColor.Black : ConsoleColor.White;
                if (i % width == 0)
                    Console.WriteLine();
                Console.Write(layer[i]);
            }
        }
    }
}
