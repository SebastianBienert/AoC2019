using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using AoC.CSharp.data;

namespace AoC.CSharp
{
    public static class Day7
    {
        public static List<int> INPUT;

        public static int Solution1()
        {
            INPUT = File.ReadAllText("data/day7.txt")
                .Split(',')
                .Select(x => Convert.ToInt32(x))
                .ToList();

            var allSequences = new List<int> {0, 1, 2, 3, 4}.Permute();
            var maximumSignal = allSequences.Max(sequence => GetThrusterSignal(sequence.ToList()));

            return maximumSignal;
        }

        public static int Solution2()
        {
            INPUT = File.ReadAllText("data/day7.txt")
                .Split(',')
                .Select(x => Convert.ToInt32(x))
                .ToList();

            var allSequences = new List<int> { 5, 6, 7, 8, 9 }.Permute();
            var maximumSignal = allSequences.Max(sequence => GetThrusterSignalFeedbackLoop(sequence.ToList()));

            return maximumSignal;
        }

        private static int GetThrusterSignalFeedbackLoop(List<int> inputSequenceList)
        {
            var signals = new Stack<int>(new[] {0});
            int i = 0;
            var output = (final: false, value: 0);
            var amplifiers = Enumerable.Range(0, 5).Select(x => new Amplifier(x, INPUT.Select(y => y).ToList(), inputSequenceList[x])).ToList();
            while (!(output.final && i % 5 == 0))
            {
                var amplifier = amplifiers[i % 5];
                output = amplifier.GetOutput(new List<int> { signals.Pop() });
                signals.Push(output.value);
                i++;
            } 

            return signals.Pop();
        }

        private static int GetThrusterSignal(List<int> inputSequenceList)
        {
            var secondParameterStack = new Stack<int>(new[] { 0 });
            var amplifiers = Enumerable.Range(0, 5).Select(x => new Amplifier(x, INPUT.Select(y => y).ToList(), inputSequenceList[x])).ToList();
            for (int i = 0; i < amplifiers.Count; i++)
            {
                var amplifier = amplifiers[i % 5];
                var output = amplifier.GetOutput(new List<int> { secondParameterStack.Pop() });
                secondParameterStack.Push(output.value);
            }

            return secondParameterStack.Pop();
        }

    }



    public enum Mode
    {
        Position,
        Immediate
    }


    public class Amplifier
    {
        public int Pointer { get; set; }
        private readonly int _id;
        public List<int> _memory;
        public Queue<int> _inputs;
        private int _outputValue;

        public Amplifier(int id, List<int> memory, params int[] initialInputs)
        {
            Pointer = 0;
            _memory = memory;
            _inputs = new Queue<int>(initialInputs);
            _id = id;
            _outputValue = 0;
        }

        public (bool final, int value) GetOutput(List<int> inputs)
        {
            inputs.ForEach(x => _inputs.Enqueue(x));
            var output = (final: false, value: 0);
            do
            {
                var codeAndModes = GetOpCodeAndModes(_memory[Pointer]);
                var instructionLength = GetInstructionLength(codeAndModes.opcode);

                if (codeAndModes.opcode == 1)
                {
                    _memory.Add((_memory[Pointer + 1], codeAndModes.firstParameter),
                        (_memory[Pointer + 2], codeAndModes.secondParameter),
                        (_memory[Pointer + 3], codeAndModes.thirdParameter));
                }
                else if (codeAndModes.opcode == 2)
                {
                    _memory.Multiply((_memory[Pointer + 1], codeAndModes.firstParameter),
                        (_memory[Pointer + 2], codeAndModes.secondParameter),
                        (_memory[Pointer + 3], codeAndModes.thirdParameter));
                }
                else if (codeAndModes.opcode == 3)
                {
                    _memory[_memory[Pointer + 1]] = _inputs.Dequeue();
                }
                else if (codeAndModes.opcode == 4)
                {
                    _outputValue = codeAndModes.firstParameter == Mode.Position ? _memory[_memory[Pointer + 1]] : _memory[Pointer + 1];
                    Pointer += instructionLength;
                    output = (false, _outputValue);
                    break;
                }
                else if (codeAndModes.opcode == 5)
                {
                    var first = codeAndModes.firstParameter == Mode.Position ? _memory[_memory[Pointer + 1]] : _memory[Pointer + 1];
                    var second = codeAndModes.secondParameter == Mode.Position ? _memory[_memory[Pointer + 2]] : _memory[Pointer + 2];
                    if (first != 0)
                    {
                        Pointer = second;
                        continue;
                    }
                }
                else if (codeAndModes.opcode == 6)
                {
                    var first = codeAndModes.firstParameter == Mode.Position ? _memory[_memory[Pointer + 1]] : _memory[Pointer + 1];
                    var second = codeAndModes.secondParameter == Mode.Position ? _memory[_memory[Pointer + 2]] : _memory[Pointer + 2];
                    if (first == 0)
                    {
                        Pointer = second;
                        continue;
                    }
                }
                else if (codeAndModes.opcode == 7)
                {
                    var first = codeAndModes.firstParameter == Mode.Position ? _memory[_memory[Pointer + 1]] : _memory[Pointer + 1];
                    var second = codeAndModes.secondParameter == Mode.Position ? _memory[_memory[Pointer + 2]] : _memory[Pointer + 2];
                    var third = _memory[Pointer + 3];
                    if (first < second)
                    {
                        _memory[third] = 1;
                    }
                    else
                    {
                        _memory[third] = 0;
                    }
                }
                else if (codeAndModes.opcode == 8)
                {
                    var first = codeAndModes.firstParameter == Mode.Position ? _memory[_memory[Pointer + 1]] : _memory[Pointer + 1];
                    var second = codeAndModes.secondParameter == Mode.Position ? _memory[_memory[Pointer + 2]] : _memory[Pointer + 2];
                    var third = _memory[Pointer + 3];
                    if (first == second)
                    {
                        _memory[third] = 1;
                    }
                    else
                    {
                        _memory[third] = 0;
                    }
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

            if (opCode == 3 || opCode == 4)
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

            return Mode.Position;
        }
    }


    public static class Extensions
    {
        public static List<int> Multiply(this List<int> input,
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

        public static List<int> Add(this List<int> input,
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

        public static IEnumerable<IEnumerable<T>> Permute<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
            {
                yield break;
            }

            var list = sequence.ToList();

            if (!list.Any())
            {
                yield return Enumerable.Empty<T>();
            }
            else
            {
                var startingElementIndex = 0;

                foreach (var startingElement in list)
                {
                    var index = startingElementIndex;
                    var remainingItems = list.Where((e, i) => i != index);

                    foreach (var permutationOfRemainder in remainingItems.Permute())
                    {
                        yield return startingElement.Concat(permutationOfRemainder);
                    }

                    startingElementIndex++;
                }
            }
        }

        private static IEnumerable<T> Concat<T>(this T firstElement, IEnumerable<T> secondSequence)
        {
            yield return firstElement;
            if (secondSequence == null)
            {
                yield break;
            }

            foreach (var item in secondSequence)
            {
                yield return item;
            }
        }
    }
}
