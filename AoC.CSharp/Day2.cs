using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AoC.CSharp
{
    class Day2
    {
        public static int Solution1()
        {
            var input = File.ReadAllText("data/day2.txt")
                .Split(',')
                .Select(x => Convert.ToInt32(x))
                .ToList();

            input[1] = 12;
            input[2] = 2;

            for (var i = 0; i < input.Count; i += 4)
            {
                if (input[i] == 1)
                {
                    input[input[i + 3]] = input[input[i + 1]] + input[input[i + 2]];
                }
                else if (input[i] == 2)
                {
                    input[input[i + 3]] = input[input[i + 1]] * input[input[i + 2]];
                }
                else
                {
                    break;
                }
            }

            return input[0];
        }

        public static int Solution2()
        {
            var input = File.ReadAllText("data/day2.txt")
                .Split(',')
                .Select(x => Convert.ToInt32(x))
                .ToList();

            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    if (CalculateResult(new List<int>(input), i, j) == 19690720)
                    {
                        return 100 * i + j;
                    }
                }
            }

            return 0;
        }

        public static int CalculateResult(List<int> input, int noun, int verb)
        {
            input[1] = noun;
            input[2] = verb;

            for (var i = 0; i < input.Count; i += 4)
            {
                if (input[i] == 1)
                {
                    input[input[i + 3]] = input[input[i + 1]] + input[input[i + 2]];
                }
                else if (input[i] == 2)
                {
                    input[input[i + 3]] = input[input[i + 1]] * input[input[i + 2]];
                }
                else
                {
                    break;
                }
            }

            return input[0];
        }
    }
}
