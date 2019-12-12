using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace AoC.CSharpDay11
{
    public enum Direction
    {
        UP,
        RIGHT,
        DOWN,
        LEFT
    }

    public class Day11
    {
        public static int PLANE_SIZE = 10000;
        public static List<Direction> Directions = new List<Direction> { Direction.UP, Direction.RIGHT, Direction.DOWN, Direction.LEFT };

        public static int Solution2()
        {
            var input = File.ReadAllText("data/day11.txt")
                .Split(',')
                .Select(BigInteger.Parse)
                .ToList();

            var plane = Enumerable.Range(0, PLANE_SIZE)
                .Select(l => Enumerable.Repeat(".", PLANE_SIZE).ToList())
                .ToList();
            //START_POS

            var currentInput = BigInteger.One;
            var currentPos = (X: PLANE_SIZE / 2, Y: PLANE_SIZE / 2);
            plane[currentPos.Y][currentPos.X] = "#";
            var currentDirection = Direction.UP;
            var amplifier = new Amplifier(0, input);

            var points = new List<(int X, int Y)>();
            
            do
            {
                points.Add(currentPos);
                var outputColor = amplifier.GetOutput(currentInput);
                if (outputColor.final)
                    break;
                var outputDirection = amplifier.GetOutput();

                currentDirection = outputDirection.value == BigInteger.One
                    ? Directions[Mod(Directions.FindIndex(x => x == currentDirection) + 1, 4)]
                    : Directions[Mod(Directions.FindIndex(x => x == currentDirection) - 1, 4)];

                plane[currentPos.Y][currentPos.X] = outputColor.value == 0 ? "." : "#";
                currentPos = GetNextPosition(currentDirection, currentPos);
                currentInput = plane[currentPos.Y][currentPos.X] == "." ? BigInteger.Zero : BigInteger.One;
            } while (true);

            PrintToBnp(plane);
            var result = points.GroupBy(x => new { x.X, x.Y }).Count();
            return result;
        }

        public static int Solution1()
        {
            var input = File.ReadAllText("data/day11.txt")
                .Split(',')
                .Select(BigInteger.Parse)
                .ToList();

            var plane = Enumerable.Range(0, PLANE_SIZE)
                .Select(l => Enumerable.Repeat(".", PLANE_SIZE).ToList())
                .ToList();


            var currentInput = BigInteger.Zero;
            var currentPos = (X: PLANE_SIZE / 2, Y: PLANE_SIZE / 2);
            var currentDirection = Direction.UP;
            var amplifier = new Amplifier(0, input);

            var points = new List<(int X, int Y)>();
            do
            {
                points.Add(currentPos);
                var outputColor = amplifier.GetOutput(currentInput);
                if (outputColor.final)
                    break;
                var outputDirection = amplifier.GetOutput();
                if (outputDirection.final)
                    break;

                currentDirection = outputDirection.value == BigInteger.One
                    ? Directions[Mod(Directions.FindIndex(x => x == currentDirection) + 1, 4)]
                    : Directions[Mod(Directions.FindIndex(x => x == currentDirection) - 1, 4)];

                plane[currentPos.Y][currentPos.X] = outputColor.value == 0 ? "." : "#";
                currentPos = GetNextPosition(currentDirection, currentPos);
                currentInput = plane[currentPos.Y][currentPos.X] == "." ? BigInteger.Zero : BigInteger.One;
            } while (true);

            var result = points.GroupBy(x => new { x.X, x.Y }).Count();
            return result;
        }

        private static (int X, int Y) GetNextPosition(Direction currentDirection, (int X, int Y) currentPos)
        {
            switch (currentDirection)
            {
                case Direction.UP:
                {
                    return (X: currentPos.X, Y: currentPos.Y - 1);
                }
                case Direction.DOWN:
                {
                    return (X: currentPos.X, Y: currentPos.Y + 1);
                    }
                case Direction.RIGHT:
                {
                    return (X: currentPos.X + 1, Y: currentPos.Y);
                    }
                case Direction.LEFT:
                {
                    return (X: currentPos.X - 1, Y: currentPos.Y);
                }
                default:
                    return currentPos;
            }
        }

        private static int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        private static void PrintPlane(List<List<string>> plane)
        {
            for (int i = 0; i < PLANE_SIZE; i++)
            {
                for (int j = 0; j < PLANE_SIZE; j++)
                    Console.Write(plane[i][j]);

                Console.WriteLine();
            }
        }

        private static void PrintToBnp(List<List<string>> plane)
        {
            using (Bitmap b = new Bitmap(PLANE_SIZE, PLANE_SIZE))
            using (Graphics g = Graphics.FromImage(b))
            {
                for (int i = 0; i < PLANE_SIZE; i++)
                {
                    for (int j = 0; j < PLANE_SIZE; j++)
                    {
                        var brush = plane[i][j] == "." ? Brushes.Black : Brushes.White;
                        g.FillRectangle(brush, new Rectangle(j, i, 1, 1));
                    }
                }
                b.Save(@"D:\solve.png", ImageFormat.Png);
            }
        }
    }


    public enum Mode
    {
        Position,
        Immediate,
        Relative
    }


    public class Amplifier
    {
        public int Pointer { get; set; }
        public BigInteger RelativeBase { get; private set; }
        private readonly int _id;
        public List<BigInteger> _memory;
        public Queue<BigInteger> _inputs;
        private BigInteger _outputValue;

        public Amplifier(int id, List<BigInteger> memory, params BigInteger[] initialInputs)
        {
            Pointer = 0;
            var memorySize = 100000000;
            _memory = new List<BigInteger>(memorySize);
            _memory.AddRange(Enumerable.Repeat(BigInteger.Zero, memorySize));
            for (int i = 0; i < memory.Count; i++)
            {
                _memory[i] = memory[i];
            }

            _inputs = new Queue<BigInteger>(initialInputs);
            _id = id;
            _outputValue = 0;
            RelativeBase = 0;
        }

        public (bool final, BigInteger value) GetOutput(BigInteger? input = null)
        {
            if (input != null)
                _inputs.Enqueue(input.Value);

            var output = (final: false, value: BigInteger.Zero);
            do
            {
                var codeAndModes = GetOpCodeAndModes((int)_memory[Pointer]);
                var instructionLength = GetInstructionLength(codeAndModes.opcode);

                if (codeAndModes.opcode == 1)
                {
                    var first = GetArgumentValue(_memory[Pointer + 1], codeAndModes.firstParameter);
                    var second = GetArgumentValue(_memory[Pointer + 2], codeAndModes.secondParameter);

                    var dstIndex = codeAndModes.thirdParameter == Mode.Position ? _memory[Pointer + 3] : _memory[Pointer + 3] + RelativeBase;
                    _memory[(int)dstIndex] = first + second;
                }
                else if (codeAndModes.opcode == 2)
                {
                    var first = GetArgumentValue(_memory[Pointer + 1], codeAndModes.firstParameter);
                    var second = GetArgumentValue(_memory[Pointer + 2], codeAndModes.secondParameter);
                    var dstIndex = codeAndModes.thirdParameter == Mode.Position ? _memory[Pointer + 3] : _memory[Pointer + 3] + RelativeBase;
                    _memory[(int)dstIndex] = first * second;
                }
                else if (codeAndModes.opcode == 3)
                {
                    if (codeAndModes.firstParameter == Mode.Relative)
                    {
                        _memory[(int)_memory[Pointer + 1] + (int)RelativeBase] = _inputs.Dequeue();
                    }
                    else
                    {
                        _memory[(int)_memory[Pointer + 1]] = _inputs.Dequeue();
                    }
                }
                else if (codeAndModes.opcode == 4)
                {
                    _outputValue = GetArgumentValue(_memory[Pointer + 1], codeAndModes.firstParameter);
                    Pointer += instructionLength;
                    output = (false, _outputValue);
                    break;
                }
                else if (codeAndModes.opcode == 5)
                {
                    var first = GetArgumentValue(_memory[Pointer + 1], codeAndModes.firstParameter);
                    var second = GetArgumentValue(_memory[Pointer + 2], codeAndModes.secondParameter);
                    if (first != 0)
                    {
                        Pointer = (int)second;
                        continue;
                    }
                }
                else if (codeAndModes.opcode == 6)
                {
                    var first = GetArgumentValue(_memory[Pointer + 1], codeAndModes.firstParameter);
                    var second = GetArgumentValue(_memory[Pointer + 2], codeAndModes.secondParameter);
                    if (first == 0)
                    {
                        Pointer = (int)second;
                        continue;
                    }
                }
                else if (codeAndModes.opcode == 7)
                {
                    var first = GetArgumentValue(_memory[Pointer + 1], codeAndModes.firstParameter);
                    var second = GetArgumentValue(_memory[Pointer + 2], codeAndModes.secondParameter);

                    var dstIndex = codeAndModes.thirdParameter == Mode.Position ? _memory[Pointer + 3] : _memory[Pointer + 3] + RelativeBase;
                    _memory[(int)dstIndex] = first * second;
                    if (first < second)
                    {
                        _memory[(int)dstIndex] = 1;
                    }
                    else
                    {
                        _memory[(int)dstIndex] = 0;
                    }
                }
                else if (codeAndModes.opcode == 8)
                {
                    var first = GetArgumentValue(_memory[Pointer + 1], codeAndModes.firstParameter);
                    var second = GetArgumentValue(_memory[Pointer + 2], codeAndModes.secondParameter);

                    var dstIndex = codeAndModes.thirdParameter == Mode.Position ? _memory[Pointer + 3] : _memory[Pointer + 3] + RelativeBase;
                    _memory[(int)dstIndex] = first * second;
                    if (first == second)
                    {
                        _memory[(int)dstIndex] = 1;
                    }
                    else
                    {
                        _memory[(int)dstIndex] = 0;
                    }
                }
                else if (codeAndModes.opcode == 9)
                {
                    var value = GetArgumentValue(_memory[Pointer + 1], codeAndModes.firstParameter);
                    RelativeBase += value;
                }
                else if (codeAndModes.opcode == 99)
                {
                    Pointer += instructionLength;
                    output = (true, _outputValue);
                    break;
                }

                Pointer += instructionLength;
            } while (true);

            return output;
        }

        private BigInteger GetArgumentValue(BigInteger argument, Mode mode)
        {
            //TODO PATTERN MATTCHING
            switch (mode)
            {
                case Mode.Immediate:
                    {
                        return argument;
                    }
                case Mode.Position:
                    {
                        return _memory[(int)argument];
                    }
                case Mode.Relative:
                    {
                        return _memory[(int)argument + (int)RelativeBase];
                    }
                default:
                    {
                        return 0;
                    }
            }
        }

        private (int opcode, Mode firstParameter, Mode secondParameter, Mode thirdParameter) GetOpCodeAndModes(int number)
        {
            var s = number.ToString().PadLeft(5, '0');
            var opCode = Convert.ToInt32(s.Substring(s.Length - 2, 2));
            var modes = s.Substring(0, 3);
            var firstMode = GetMode(Convert.ToInt32(modes[2].ToString()));
            var secondMode = GetMode(Convert.ToInt32(modes[1].ToString()));
            var thirdMode = GetMode(Convert.ToInt32(modes[0].ToString()));

            return (opCode, firstMode, secondMode, thirdMode);
        }

        private int GetInstructionLength(int opCode)
        {
            if (opCode == 1 || opCode == 2 || opCode == 7 || opCode == 8)
            {
                return 4;
            }

            if (opCode == 3 || opCode == 4 || opCode == 9)
            {
                return 2;
            }

            if (opCode == 5 || opCode == 6)
            {
                return 3;
            }

            return 1;
        }

        private Mode GetMode(int x)
        {
            if (x == 1)
            {
                return Mode.Immediate;
            }

            if (x == 2)
            {
                return Mode.Relative;
            }

            return Mode.Position;
        }
    }

}
