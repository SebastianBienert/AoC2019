using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CHash
{
    public static class Day1
    {
        public static int Solution1()
        {
            var answer = File.ReadAllLines("data/day1.txt")
                    .Select(x => Convert.ToInt32(x))
                    .Sum(x => (x / 3) - 2);

            return answer;
        }

        public static int Solution2()
        {
            var answer = File.ReadAllLines("data/day1.txt")
                .Select(x => Convert.ToInt32(x))
                .Sum(GetFuel);

            return answer;
        }


        private static int GetFuel(int x)
        {
            var fuel = (x / 3) - 2;
            if (fuel <= 0)
                return 0;

            return fuel + GetFuel(fuel);
        }

    }
}
