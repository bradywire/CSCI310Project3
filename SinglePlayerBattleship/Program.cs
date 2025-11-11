using System;
using System.Collections.Generic;

namespace BattleshipGame
{
    class Ship
    {
        public List<(int row, int col)> Positions { get; } = new List<(int row, int col)>();
        public bool IsSunk { get; set; } = false;
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the hidden board (10x10 grid)
            char[,] board = new char[10, 10]; // Defaults to '\0' for empty cells

            // List of ship sizes: Carrier(5), Battleship(4), Cruiser(3), Submarine(3), Destroyer(2)
            List<int> shipSizes = new List<int> { 5, 4, 3, 3, 2 };

            // List to track ships
            List<Ship> ships = new List<Ship>();

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
                        Ship ship = new Ship();
                        if (horizontal)
                        {
                            for (int i = 0; i < size; i++)
                            {
                                board[row, col + i] = 'S';
                                ship.Positions.Add((row, col + i));
                            }
                        }
                        else
                        {
                            for (int i = 0; i < size; i++)
                            {
                                board[row + i, col] = 'S';
                                ship.Positions.Add((row + i, col));
                            }
                        }
                        ships.Add(ship);
                        placed = true;
                    }
                }
            }

            // Game variables
            int misses = 0;
            int shipsSunk = 0;
            int totalHitsNeeded = 17; // Sum of all ship sizes
            int hits = 0;

            // Display rules before the game starts
            Console.WriteLine("Welcome to Battleship!");
            Console.WriteLine("Rules: There are 5 ships scattered throughout the 10x10 board.");
            Console.WriteLine("You have 10 misses allowed.");
            Console.WriteLine("Guess coordinates (e.g., A5 or J10) until you sink all ships or hit 15 misses.");

            // Game loop
            while (misses < 15 && hits < totalHitsNeeded)
            {
                DisplayBoard(board, ships);
                Console.WriteLine($"Current misses: {misses}/10");
                Console.WriteLine($"Ships sunk: {shipsSunk}/5");

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

                    // Check for sunk ships
                    bool anySunk = false;
                    foreach (var ship in ships)
                    {
                        if (!ship.IsSunk && ship.Positions.All(p => board[p.row, p.col] == 'H'))
                        {
                            ship.IsSunk = true;
                            shipsSunk++;
                            anySunk = true;
                        }
                    }
                    if (anySunk)
                    {
                        Console.WriteLine("You sank a ship!");
                    }
                }
                else
                {
                    board[rowIndex, colIndex] = 'M';
                    misses++;
                    Console.WriteLine("Miss!");
                }
            }

            // End of game
            DisplayBoard(board, ships);
            if (hits == totalHitsNeeded)
            {
                Console.WriteLine("Congratulations! You sank all the ships and won the game!");
            }
            else
            {
                Console.WriteLine("Game over! You reached 15 misses.");
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

        static void DisplayBoard(char[,] board, List<Ship> ships)
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
                    else if (cell == 'M')
                    {
                        Console.Write("M ");
                    }
                    else if (cell == 'H')
                    {
                        bool isSunk = false;
                        foreach (var ship in ships)
                        {
                            if (ship.Positions.Contains((i, j)) && ship.IsSunk)
                            {
                                isSunk = true;
                                break;
                            }
                        }
                        if (isSunk)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("H ");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.Write("H ");
                        }
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }
}