using System;
using System.Collections.Generic;

namespace BattleshipGame
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the hidden board (10x10 grid)
            char[,] board = new char[10, 10]; // Defaults to '\0' for empty cells

            // List of ship sizes: Carrier(5), Battleship(4), Cruiser(3), Submarine(3), Destroyer(2)
            List<int> shipSizes = new List<int> { 5, 4, 3, 3, 2 };

            // Random number generator
            Random rand = new Random();

            // Place each ship randomly
            foreach (int size in shipSizes)
            {
                bool placed = false;
                while (!placed)
                {
                    int row = rand.Next(10);
                    int col = rand.Next(10);
                    bool horizontal = rand.Next(2) == 0;

                    if (CanPlaceShip(board, row, col, size, horizontal))
                    {
                        PlaceShip(board, row, col, size, horizontal);
                        placed = true;
                    }
                }
            }

            // Game variables
            int misses = 0;
            int totalHitsNeeded = 17; // Sum of all ship sizes
            int hits = 0;

            // Game loop
            while (misses < 10 && hits < totalHitsNeeded)
            {
                DisplayBoard(board);

                Console.WriteLine("Enter your guess (e.g., A5 or J10):");
                string input = Console.ReadLine().ToUpper().Trim();

                // Parse input
                if (input.Length < 2 || input.Length > 3 || !char.IsLetter(input[0]) || input[0] < 'A' || input[0] > 'J')
                {
                    Console.WriteLine("Invalid input. Please enter a valid coordinate like A5.");
                    continue;
                }

                int rowIndex = input[0] - 'A';
                string colStr = input.Substring(1);
                if (!int.TryParse(colStr, out int colIndex) || colIndex < 1 || colIndex > 10)
                {
                    Console.WriteLine("Invalid input. Column must be between 1 and 10.");
                    continue;
                }
                colIndex--; // Adjust to 0-based index

                // Check if already guessed
                if (board[rowIndex, colIndex] == 'H' || board[rowIndex, colIndex] == 'M')
                {
                    Console.WriteLine("You already guessed that spot.");
                    continue;
                }

                // Process guess
                if (board[rowIndex, colIndex] == 'S')
                {
                    board[rowIndex, colIndex] = 'H';
                    hits++;
                    Console.WriteLine("Hit!");
                }
                else
                {
                    board[rowIndex, colIndex] = 'M';
                    misses++;
                    Console.WriteLine("Miss!");
                }
            }

            // End of game
            DisplayBoard(board);
            if (hits == totalHitsNeeded)
            {
                Console.WriteLine("Congratulations! You sank all the ships and won the game!");
            }
            else
            {
                Console.WriteLine("Game over! You reached 10 misses.");
            }
        }

        static bool CanPlaceShip(char[,] board, int row, int col, int size, bool horizontal)
        {
            if (horizontal)
            {
                if (col + size > 10) return false;
                for (int i = 0; i < size; i++)
                {
                    if (board[row, col + i] != '\0') return false;
                }
            }
            else
            {
                if (row + size > 10) return false;
                for (int i = 0; i < size; i++)
                {
                    if (board[row + i, col] != '\0') return false;
                }
            }
            return true;
        }

        static void PlaceShip(char[,] board, int row, int col, int size, bool horizontal)
        {
            if (horizontal)
            {
                for (int i = 0; i < size; i++)
                {
                    board[row, col + i] = 'S';
                }
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    board[row + i, col] = 'S';
                }
            }
        }

        static void DisplayBoard(char[,] board)
        {
            Console.WriteLine("  1 2 3 4 5 6 7 8 9 10");
            for (int i = 0; i < 10; i++)
            {
                char rowLabel = (char)('A' + i);
                Console.Write(rowLabel + " ");
                for (int j = 0; j < 10; j++)
                {
                    char cell = board[i, j];
                    if (cell == 'S' || cell == '\0')
                    {
                        Console.Write(". ");
                    }
                    else if (cell == 'H')
                    {
                        Console.Write("H ");
                    }
                    else if (cell == 'M')
                    {
                        Console.Write("M ");
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}