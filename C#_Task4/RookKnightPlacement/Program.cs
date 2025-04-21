using System;
using System.Collections.Generic;

class KentavrProgram
{
    const int BoardSize = 8;
    static string[,] board = new string[BoardSize, BoardSize];
    static int[,] knightMoves = { { 2, 1 }, { 2, -1 }, { -2, 1 }, { -2, -1 }, { 1, 2 }, { 1, -2 }, { -1, 2 }, { -1, -2 } };
    static List<(int, int)> kentavrPositions = new List<(int, int)>();

    static void Main()
    {
        for (int r = 0; r < BoardSize; r++)
            for (int c = 0; c < BoardSize; c++)
                board[r, c] = "0";

        var (row, col) = GetPositionFromInput();
        PlaceKentavr(row, col);
        UpdateBoardState();
        PrintBoard();

        bool placedAny;
        do
        {
            placedAny = PlaceNextKentavr();
            if (placedAny)
            {
                UpdateBoardState();
                PrintBoard();
            }
        } while (placedAny);

        bool foundMore = false;
        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (IsSafePosition(r, c))
                {
                    PlaceKentavr(r, c);
                    foundMore = true;

                    char colLetter = (char)('A' + c);
                    int rowNumber = 8 - r;
                    Console.WriteLine($"Found missed Kentavr position at {colLetter}{rowNumber}");
                    UpdateBoardState();
                    PrintBoard();
                }
            }
        }

        if (!foundMore)
        {
            Console.WriteLine("No missed positions found.");
        }

        Console.WriteLine($"Done. Total Kentavrs placed: {kentavrPositions.Count}");
        Console.WriteLine("Kentavr positions:");
        foreach (var (r, c) in kentavrPositions)
        {
            char colLetter = (char)('A' + c);
            int rowNumber = 8 - r;
            Console.WriteLine($"  {colLetter}{rowNumber}");
        }
        Console.ReadKey();
    }

    static void PlaceKentavr(int row, int col)
    {
        board[row, col] = "K";
        kentavrPositions.Add((row, col));

        char colLetter = (char)('A' + col);
        int rowNumber = 8 - row;
        Console.WriteLine($"Kentavr placed at {colLetter}{rowNumber}");
        Console.WriteLine();
    }

    static bool IsSafePosition(int row, int col)
    {
        if (board[row, col] == "K" || board[row, col] == "1")
            return false;

        foreach (var (kr, kc) in kentavrPositions)
        {
            if (kr == row || kc == col)
                return false;

            for (int i = 0; i < knightMoves.GetLength(0); i++)
            {
                int attackRow = kr + knightMoves[i, 0];
                int attackCol = kc + knightMoves[i, 1];
                if (attackRow == row && attackCol == col)
                    return false;
            }
        }

        return true;
    }

    static void MarkAttacked()
    {
        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (board[r, c] != "K")
                    board[r, c] = "0";
            }
        }

        foreach (var (kr, kc) in kentavrPositions)
        {
            for (int i = 0; i < BoardSize; i++)
            {
                if (board[kr, i] != "K") board[kr, i] = "1";
                if (board[i, kc] != "K") board[i, kc] = "1";
            }

            for (int i = 0; i < knightMoves.GetLength(0); i++)
            {
                int newRow = kr + knightMoves[i, 0];
                int newCol = kc + knightMoves[i, 1];
                if (IsInBounds(newRow, newCol) && board[newRow, newCol] != "K")
                {
                    board[newRow, newCol] = "1";
                }
            }
        }
    }

    static void UpdateBoardState()
    {
        MarkAttacked();

        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (board[r, c] != "K" && board[r, c] != "1")
                {
                    int safeCount = SimulateAndCountSafePositions(r, c);
                    board[r, c] = safeCount.ToString();
                }
            }
        }
    }

    static void PrintBoard()
    {
        for (int r = 0; r < BoardSize; r++)
        {
            Console.Write(8 - r + "| ");
            for (int c = 0; c < BoardSize; c++)
            {
                if (board[r, c] == "K")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("K".PadLeft(3));
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(board[r, c].PadLeft(3));
                }
            }
            Console.WriteLine();
        }

        Console.Write("   ");
        for (char c = 'A'; c <= 'H'; c++)
        {
            Console.Write(c.ToString().PadLeft(3));
        }
        Console.WriteLine();
        Console.WriteLine();
    }

    static (int, int) GetPositionFromInput()
    {
        Console.Write("Enter letter (A-H): ");
        char colChar = char.ToUpper(Console.ReadLine()[0]);
        int col = colChar - 'A';

        Console.Write("Enter number (1-8): ");
        int num = int.Parse(Console.ReadLine());
        int row = 8 - num;

        Console.WriteLine($"User placed Kentavr at {colChar}{num}");
        Console.WriteLine();

        return (row, col);
    }

    static bool IsInBounds(int r, int c) => r >= 0 && r < BoardSize && c >= 0 && c < BoardSize;

    static int SimulateAndCountSafePositions(int row, int col)
    {
        string[,] tempBoard = new string[BoardSize, BoardSize];
        for (int r = 0; r < BoardSize; r++)
            for (int c = 0; c < BoardSize; c++)
                tempBoard[r, c] = board[r, c];

        tempBoard[row, col] = "K";

        for (int i = 0; i < BoardSize; i++)
        {
            if (tempBoard[row, i] != "K") tempBoard[row, i] = "1";
            if (tempBoard[i, col] != "K") tempBoard[i, col] = "1";
        }

        for (int i = 0; i < knightMoves.GetLength(0); i++)
        {
            int newRow = row + knightMoves[i, 0];
            int newCol = col + knightMoves[i, 1];
            if (IsInBounds(newRow, newCol) && tempBoard[newRow, newCol] != "K")
            {
                tempBoard[newRow, newCol] = "1";
            }
        }

        int safeCount = 0;
        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (tempBoard[r, c] != "1" && tempBoard[r, c] != "K")
                    safeCount++;
            }
        }

        return safeCount;
    }

    static bool PlaceNextKentavr()
    {
        int maxValue = -1;
        int maxRow = -1, maxCol = -1;

        for (int r = 0; r < BoardSize; r++)
        {
            for (int c = 0; c < BoardSize; c++)
            {
                if (board[r, c] != "1" && board[r, c] != "K")
                {
                    int cellValue = int.Parse(board[r, c]);
                    if (cellValue > maxValue)
                    {
                        maxValue = cellValue;
                        maxRow = r;
                        maxCol = c;
                    }
                }
            }
        }

        if (maxValue == -1) return false;

        PlaceKentavr(maxRow, maxCol);

        char colLetter = (char)('A' + maxCol);
        int rowNumber = 8 - maxRow;
        Console.WriteLine($"Kentavr placed at {colLetter}{rowNumber} because it had the biggest value: {maxValue}");
        Console.WriteLine();

        return true;
    }
}