/*
 * Script is based on script used in http://www.dos486.com/sudoku/, but with some changes and additions.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Sudoku
{
    public class SudokuPuzzle : ICloneable
    {
        private int[][] _cells;
        private readonly int _length;

        private int BoxSize => (int)Math.Sqrt(_length);

        private SudokuPuzzle(int n)
        {
            _cells = Enumerable.Repeat(Enumerable.Range(1, n).ToArray(), n * n).ToArray();
            _length = n;
        }

        private SudokuPuzzle(int[] input) : this((int)Math.Sqrt(input.Length))
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] <= 0 || input[i] > _length) continue;
                var puzzle = PlaceValue(i, input[i]);
                if (puzzle == null)
                    throw new ArgumentException("This puzzle is unsolvable!");
                _cells = puzzle._cells;
            }
        }

        public object Clone()
        {
            var clone = new SudokuPuzzle(_length)
            {
                _cells = new int[_cells.Length][]
            };

            for (int i = 0; i < _cells.Length; i++)
            {
                clone._cells[i] = new int[_cells[i].Length];
                Buffer.BlockCopy(_cells[i], 0, clone._cells[i], 0, Buffer.ByteLength(_cells[i]));
            }

            return clone;
        }

        private bool IsPeer(int c1, int c2)
        {
            return (c1 / _length == c2 / _length //Cells in same row
                    || c1 % _length == c2 % _length //Cells in same column
                    || (c1 / _length / BoxSize == c2 / _length / BoxSize &&
                        c1 % _length / BoxSize == c2 % _length / BoxSize)) //Cells in same box
                   && c1 != c2; //Cell is not peer to itself
        }

        //Dictionary to memoize lists of peers
        //The key is a tuple of <int, int>, where the first value is the length of the puzzle (typically 9) and the second value is the cell index
        private static readonly Dictionary<Tuple<int, int>, int[]> SavedPeers = new();

        private int[] Peers(int cell)
        {
            var key = new Tuple<int, int>(_length, cell);

            if (SavedPeers.ContainsKey(key)) return SavedPeers[key];

            var items = new List<int>();
            for (int j = 0; j < _length * _length; j++)
            {
                if (IsPeer(cell, j)) items.Add(j);
            }

            SavedPeers.Add(key, items.ToArray());

            return SavedPeers[key];
        }

        private SudokuPuzzle ApplyConstraints(int cellIndex, int value)
        {
            var puzzle = (SudokuPuzzle)Clone();

            //Standard Sudoku constraint logic: Set this cell to one and only one candidate, and remove this value from the candidate list of all its peers
            puzzle._cells[cellIndex] = new[] { value };

            foreach (int peerIndex in puzzle.Peers(cellIndex))
            {
                int[] newPeers = puzzle._cells[peerIndex].Except(new[] { value }).ToArray();
                if (newPeers.Length == 0)
                    return null;

                puzzle._cells[peerIndex] = newPeers;
            }

            return puzzle;
        }

        private static List<int> FindSingularizedCells(SudokuPuzzle puzzle1, SudokuPuzzle puzzle2, int cellIndex)
        {
            Debug.Assert(puzzle1._length == puzzle2._length);
            var result = new List<int>();
            foreach (int i in puzzle1.Peers(cellIndex))
            {
                if (puzzle1._cells[i].Length > 1 && puzzle2._cells[i].Length == 1)
                    result.Add(i);
            }

            return result;
        }

        private SudokuPuzzle PlaceValue(int cellIndex, int value)
        {
            if (!_cells[cellIndex].Contains(value))
                return null;

            var puzzle = ApplyConstraints(cellIndex, value);
            if (puzzle == null)
                return null;

            foreach (int i in FindSingularizedCells(this, puzzle, cellIndex))
            {
                if ((puzzle = puzzle.PlaceValue(i, puzzle._cells[i].Single())) == null)
                    return null;
            }

            return puzzle;
        }

        private int FindWorkingCell()
        {
            int minCandidates = _cells.Where(candidates => candidates.Length >= 2).Min(candidates => candidates.Length);
            return Array.FindIndex(_cells, c => c.Length == minCandidates);
        }

        private static List<SudokuPuzzle> MultiSolve(SudokuPuzzle input, int maximumSolutions = -1)
        {
            var solutions = new List<SudokuPuzzle>();
            input.Solve(p =>
            {
                solutions.Add(p);
                return solutions.Count < maximumSolutions || maximumSolutions == -1;
            });
            return solutions;
        }

        private SudokuPuzzle Solve(Func<SudokuPuzzle, bool> solutionFunc = null)
        {
            if (_cells.All(cell => cell.Length == 1))
                return (solutionFunc != null && solutionFunc(this)) ? null : this;

            int activeCell = FindWorkingCell();
            foreach (int guess in _cells[activeCell])
            {
                SudokuPuzzle puzzle;
                if ((puzzle = PlaceValue(activeCell, guess)) == null) continue;
                if ((puzzle = puzzle.Solve(solutionFunc)) != null)
                    return puzzle;
            }

            return null;
        }

        public static int[] GetSolution(int[] clues)
        {
            int[] solution = new int[81];
            SudokuPuzzle puzzle = new SudokuPuzzle(clues).Solve();
            if (puzzle == null) throw new ArgumentException("Can't create solution from an unsolvable puzzle!");
            for (int i = 0; i < puzzle._cells.Length; i++)
            {
                int[] cell = puzzle._cells[i];
                if (cell.Length == 0) throw new ArgumentException("Can't create solution from an unsolvable puzzle!");
                solution[i] = cell[0];
            }

            return solution;
        }

        public static SudokuPuzzle RandomGrid(int size)
        {
            var stopwatch = new Stopwatch();
            var puzzle = new SudokuPuzzle(size);
            var rand = new Random();
            stopwatch.Start();
            while (true)
            {
                if (stopwatch.ElapsedMilliseconds > 500)
                {
                    puzzle = new SudokuPuzzle(size);
                    stopwatch.Reset();
                    rand = new Random();
                }

                int[] unsolvedCellIndexes = puzzle._cells
                    .Select((candidates, index) =>
                        new
                        {
                            cands = candidates, index
                        }) //Project to a new sequence of candidates and index (an anonymous type behaving like a tuple)
                    .Where(t => t.cands.Length >= 2) //Filter to cells with at least 2 candidates
                    .Select(u => u.index) //Project the tuple to only the index
                    .ToArray();

                int cellIndex = unsolvedCellIndexes[rand.Next(unsolvedCellIndexes.Length)];
                int candidateValue = puzzle._cells[cellIndex][rand.Next(puzzle._cells[cellIndex].Length)];

                var workingPuzzle = puzzle.PlaceValue(cellIndex, candidateValue);
                if (workingPuzzle != null)
                {
                    var solutions = MultiSolve(workingPuzzle, 2);
                    switch (solutions.Count)
                    {
                        case 0: continue;
                        case 1: return solutions.Single();
                        default:
                            puzzle = workingPuzzle;
                            break;
                    }
                }
            }
        }

        public static int[] CreateClues(SudokuPuzzle input, int maxClues = 0, int maxMilliseconds = -1)
        {
            //Get a unique solution for the input puzzle, in case it wasn't already
            var puzzle = input.Solve();
            if (puzzle == null) throw new ArgumentException("Can't create clues from an unsolvable puzzle!");

            //This is the list of clues we work on.  It can be reconstituted into a new puzzle object by way of a constructor
            int[] clues = puzzle._cells.Select(c => c[0]).ToArray();
            var rand = new Random();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (maxMilliseconds > 0 && stopwatch.ElapsedMilliseconds > maxMilliseconds)
                {
                    stopwatch.Stop();
                    stopwatch.Reset();
                    return clues;
                }

                //Pick a random cell to blank
                int clueCell = rand.Next(clues.Length);
                if (clues[clueCell] == 0) continue;
                int[] workingClues = clues.ToArray();
                workingClues[clueCell] = 0;
                if (MultiSolve(new SudokuPuzzle(workingClues), 2).Count > 1)
                {
                    if (maxClues == 0) return clues;
                    continue;
                }

                clues = workingClues;
                if (clues.Count(c => c != 0) <= maxClues)
                {
                    stopwatch.Stop();
                    stopwatch.Reset();
                    return clues;
                }
            }
        }

        public override string ToString()
        {
            var result = new StringBuilder();

            foreach (int[] t in _cells)
            {
                if (t.Length == 1) result.Append(t[0]);
                else result.Append('.');
            }

            return result.ToString();
        }
    }
}