using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AoC.CSharp
{
    public class Day4
    {
        public static int MIN = 256310;
        public static int MAX = 732736;

        public static int Solution1()
        {
            var sum = Enumerable
                .Range(MIN, MAX - MIN)
                .Select(i => i.ToString())
                .Aggregate(0, (acc, val) => DigitsDecrease(val) && TwoAdjacentDigits(val) ? acc + 1 : acc);

            return sum;
        }

        public static int Solution2()
        {
            var sum = Enumerable
                .Range(MIN, MAX - MIN)
                .Select(i => i.ToString())
                .Aggregate(0, (acc, val) => DigitsDecrease(val) && ExactlyTwoAdjacentDigits(val) ? acc + 1 : acc);

            return sum;
        }

        private static bool DigitsDecrease(string input)
        {
            for (int i = 0; i < input.Length - 1; i++)
            {
                if (input[i + 1] < input[i])
                    return false;
            }

            return true;
        }

        private static bool TwoAdjacentDigits(string input)
        {
            for (int i = 0; i < input.Length - 1; i++)
            {
                if (input[i] == input[i + 1])
                    return true;
            }

            return false;
        }

        private static bool ExactlyTwoAdjacentDigits(string input)
        {
            var result = input.ToCharArray()
                .Select((x, i) => new {Index = i, Value = x})
                .GroupBy(x => x.Value)
                .Where(g =>
                {
                    var indexes = g.Select(c => c.Index).ToList();
                    return g.Count() == 2 && Math.Abs(indexes[0] - indexes[1]) == 1;
                })
                .Any();

            return result;
        }
    }
}
