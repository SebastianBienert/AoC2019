using NUnit.Framework;

namespace AoC.Tests
{
    public class Day1Tests
    {
        [Test]
        public void SameResults()
        {
            var fSharpPart1 = FsharpDay1.part1;
            var cSharpPart1 = CHash.Day1.Solution1();
            var fSharpPart2 = FsharpDay1.part2;
            var cSharpPart2 = CHash.Day1.Solution2();

            Assert.IsTrue(fSharpPart1 == cSharpPart1);
            Assert.IsTrue(fSharpPart2 == cSharpPart2);
        }
    }
}