using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace AoC.CSharpDay9
{
    class Day9
    {
        public static BigInteger Solution1()
        {
            var input = File.ReadAllText("data/day9.txt")
                .Split(',')
                .Select(BigInteger.Parse)
                .ToList();

            var memory = new Amplifier(1, input, BigInteger.One);

            var final = false;
            do
            {
                var output = memory.GetOutput();
                final = output.final;
                Console.WriteLine(output.value);
            } while (!final);


            return BigInteger.MinusOne;
        }

        public static BigInteger Solution2()
        {
            var input = File.ReadAllText("data/day9.txt")
                .Split(',')
                .Select(BigInteger.Parse)
                .ToList();

            var memory = new Amplifier(1, input, 2);

            var final = false;
            do
            {
                var output = memory.GetOutput();
                final = output.final;
                Console.WriteLine(output.value);
            } while (!final);


            return BigInteger.MinusOne;
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

        public (bool final, BigInteger value) GetOutput()
        {
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
                    _memory[(int) dstIndex] = first + second;
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
                    //var third = _memory[Pointer + 3];
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
                    //var third = _memory[Pointer + 3];
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
