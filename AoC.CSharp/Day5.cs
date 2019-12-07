using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace AoC.CSharp.data
{
    enum Mode
    {
        Position,
        Immediate
    }

    static class Day5
    {
        public static int INPUT = 5;
        public static int POINTER = 0;

        public static int Solution1()
        {
            var input = File.ReadAllText("data/day5.txt")
                .Split(',')
                .Select(x => Convert.ToInt32(x))
                .ToList();

            return CalculateResult1(input);
        }

        public static int Solution2()
        {
            var input = File.ReadAllText("data/day5.txt")
                .Split(',')
                .Select(x => Convert.ToInt32(x))
                .ToList();

            return CalculateResult2(input);
        }

        public static int CalculateResult2(List<int> input)
        {
            do
            {
                var codeAndModes = GetOpCodeAndModes(input[POINTER]);
                var instructionLength = GetInstructionLength(codeAndModes.opcode);

                if (codeAndModes.opcode == 1)
                {
                    input.Add((input[POINTER + 1], codeAndModes.firstParameter),
                        (input[POINTER + 2], codeAndModes.secondParameter),
                        (input[POINTER + 3], codeAndModes.thirdParameter));
                }
                else if (codeAndModes.opcode == 2)
                {
                    input.Multiply((input[POINTER + 1], codeAndModes.firstParameter),
                        (input[POINTER + 2], codeAndModes.secondParameter),
                        (input[POINTER + 3], codeAndModes.thirdParameter));
                }
                else if (codeAndModes.opcode == 3)
                {
                    input[input[POINTER + 1]] = INPUT;
                }
                else if (codeAndModes.opcode == 4)
                {
                    var output = codeAndModes.firstParameter == Mode.Position ? input[input[POINTER+ 1]] : input[POINTER+ 1];
                    Console.WriteLine($"OUTPUT: {output}");
                }
                else if (codeAndModes.opcode == 5)
                {
                    var first = codeAndModes.firstParameter == Mode.Position ? input[input[POINTER + 1]] : input[POINTER + 1];
                    var second = codeAndModes.secondParameter == Mode.Position ? input[input[POINTER + 2]] : input[POINTER + 2];
                    if (first != 0)
                    {
                        POINTER = second;
                        continue;
                    }
                }
                else if (codeAndModes.opcode == 6)
                {
                    var first = codeAndModes.firstParameter == Mode.Position ? input[input[POINTER + 1]] : input[POINTER + 1];
                    var second = codeAndModes.secondParameter == Mode.Position ? input[input[POINTER + 2]] : input[POINTER + 2];
                    if (first == 0)
                    {
                        POINTER = second;
                        continue;
                    }
                }
                else if (codeAndModes.opcode == 7)
                {
                    var first = codeAndModes.firstParameter == Mode.Position ? input[input[POINTER + 1]] : input[POINTER + 1];
                    var second = codeAndModes.secondParameter == Mode.Position ? input[input[POINTER + 2]] : input[POINTER + 2];
                    var third = input[POINTER + 3];
                    if (first < second)
                    {
                        input[third] = 1;
                    }
                    else
                    {
                        input[third] = 0;
                    }
                }
                else if (codeAndModes.opcode == 8)
                {
                    var first = codeAndModes.firstParameter == Mode.Position ? input[input[POINTER + 1]] : input[POINTER + 1];
                    var second = codeAndModes.secondParameter == Mode.Position ? input[input[POINTER + 2]] : input[POINTER + 2];
                    var third = input[POINTER + 3];
                    if (first == second)
                    {
                        input[third] = 1;
                    }
                    else
                    {
                        input[third] = 0;
                    }
                }
                else if (codeAndModes.opcode == 99)
                {
                    Console.WriteLine($"HALT:");
                    break;
                }

                POINTER += instructionLength;
            } while (true);

            return input[0];
        }

        public static int CalculateResult1(List<int> input)
        {
            for (var i = 0; i < input.Count;)
            {
                var codeAndModes = GetOpCodeAndModes(input[i]);
                var instructionLength = GetInstructionLength(codeAndModes.opcode);

                if (codeAndModes.opcode == 1)
                {
                    input.Add((input[i + 1], codeAndModes.firstParameter),
                        (input[i + 2], codeAndModes.secondParameter),
                        (input[i + 3], codeAndModes.thirdParameter));
                }
                else if (codeAndModes.opcode == 2)
                {
                    input.Multiply((input[i + 1], codeAndModes.firstParameter),
                        (input[i + 2], codeAndModes.secondParameter),
                        (input[i + 3], codeAndModes.thirdParameter));
                }
                else if (codeAndModes.opcode == 3)
                {
                    input[input[i + 1]] = INPUT;
                }
                else if (codeAndModes.opcode == 4)
                {
                    var output = codeAndModes.firstParameter == Mode.Position ? input[input[i + 1]] : input[i + 1];
                    Console.WriteLine($"OUTPUT: {output}");
                }
                else if(codeAndModes.opcode == 99)
                {
                    Console.WriteLine($"HALT:");
                    break;
                }

                i += instructionLength;
            }

            return input[0];
        }

        public static (int opcode, Mode firstParameter, Mode secondParameter, Mode thirdParameter) GetOpCodeAndModes(int number)
        {
            var s = number.ToString().PadLeft(5, '0');
            var opCode = Convert.ToInt32(s.Substring(s.Length - 2, 2));
            var modes = s.Substring(0, 3);
            var firstMode = GetMode(Convert.ToInt32(modes[2].ToString()));
            var secondMode = GetMode(Convert.ToInt32(modes[1].ToString()));
            var thirdMode = GetMode(Convert.ToInt32(modes[0].ToString()));

            return (opCode, firstMode, secondMode, thirdMode);
        }

        private static List<int> Multiply(this List<int> input,
            (int parameter, Mode mode) first,
            (int parameter, Mode mode) second,
            (int parameter, Mode mode) third)
        {
            var firstValue = first.mode == Mode.Position ? input[first.parameter] : first.parameter;
            var secondvalue = second.mode == Mode.Position ? input[second.parameter] : second.parameter;
            //ALwyas position mode?
            input[third.parameter] = firstValue * secondvalue;
            return input;
        }

        private static List<int> Add(this List<int> input,
            (int parameter, Mode mode) first,
            (int parameter, Mode mode) second,
            (int parameter, Mode mode) third)
        {
            var firstValue = first.mode == Mode.Position ? input[first.parameter] : first.parameter;
            var secondvalue = second.mode == Mode.Position ? input[second.parameter] : second.parameter;
            //ALwyas position mode?
            input[third.parameter] = firstValue + secondvalue;
            return input;
        }

        private static int GetInstructionLength(int opCode)
        {
            if (opCode == 1 || opCode == 2 || opCode == 7 || opCode == 8)
            {
                return 4;
            }
            else if (opCode == 3 || opCode == 4)
            {
                return 2;
            }
            else if (opCode == 5 || opCode == 6)
            {
                return 3;
            }

            return 1;
        }


        private static Mode GetMode(int x)
        {
            if (x == 1)
            {
                return Mode.Immediate;
            }

            return Mode.Position;
        }

    }
}
