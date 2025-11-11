using System;
using System.Collections.Generic;
using System.Reflection;

namespace BattleshipGame
{
    class Ship
    {
        public List<(int row, int col)> Positions { get; } = new List<(int row, int col)>();
        public bool IsSunk { get; set; } = false;
    }

    class Game
    {
        private char[,] board;
        private List<int> shipSizes;
        private List<Ship> ships;
        private Random rand;
        private int cursorX;
        private int cursorY;
        int misses;
        int shipsSunk;
        int totalHitsNeeded;
        int hits;
        bool gameRunning;
        bool gameWon;

        public Game()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            // Initialize the hidden board (10x10 grid)
            board = new char[10, 10]; // Defaults to '\0' for empty cells

            // List of ship sizes: Carrier(5), Battleship(4), Cruiser(3), Submarine(3), Destroyer(2)
            shipSizes = new List<int> { 5, 4, 3, 3, 2 };

            // List to track ships
            ships = new List<Ship>();

            // Random number generator
            rand = new Random();

            // Cursor position
            cursorX = 0;
            cursorY = 0;

            // Game variables
            misses = 0;
            shipsSunk = 0;
            totalHitsNeeded = 17; // Sum of all ship sizes
            hits = 0;
            gameRunning = true;
            gameWon = false;

            GenerateBoard();
        }
        private void GenerateBoard()
        {
            foreach (int size in shipSizes)
            {
                bool placed = false;
                while (!placed)
                {
                    int row = rand.Next(10);
                    int col = rand.Next(10);
                    bool horizontal = rand.Next(2) == 0;

                    if (CanPlaceShip(row, col, size, horizontal))
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
        }
        private bool CanPlaceShip(int row, int col, int size, bool horizontal)
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

        private void DisplayBoard()
        {
            Console.Clear();
            Console.WriteLine("  1 2 3 4 5 6 7 8 9 10");
            for (int i = 0; i < 10; i++)
            {
                char rowLabel = (char)('A' + i);
                Console.Write(rowLabel);
                for (int j = 0; j < 10; j++)
                {
                    if (cursorX == j && cursorY == i)
                    {
                        Console.Write(">");
                    }
                    else
                    {
                        Console.Write(" ");
                    }
                    char cell = board[i, j];
                    if (cell == 'S' || cell == '\0')
                    {
                        Console.Write(".");
                    }
                    else if (cell == 'M')
                    {
                        Console.Write("M");
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
                            Console.Write("H");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.Write("H");
                        }
                    }
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine($"Current misses: {misses}/25");
            Console.WriteLine($"Ships sunk: {shipsSunk}/5");

            Console.WriteLine("Submit your guess by hitting Enter, hit R to restart, or hit Esc to exit.");
        }

        private void HandleCursor()
        {
            while (misses < 25 && hits < totalHitsNeeded)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);

                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            cursorY--;
                            DisplayBoard();
                            break;
                        case ConsoleKey.DownArrow:
                            cursorY++;
                            DisplayBoard();
                            break;
                        case ConsoleKey.LeftArrow:
                            cursorX--;
                            DisplayBoard();
                            break;
                        case ConsoleKey.RightArrow:
                            cursorX++;
                            DisplayBoard();
                            break;
                        case ConsoleKey.Enter:
                            handleGuess();
                            DisplayBoard();
                            break;
                        case ConsoleKey.Escape:
                            gameRunning = false;
                            gameWon = false;
                            return;
                        case ConsoleKey.R:
                            InitializeGame();
                            DisplayBoard();
                            break;
                    }
                }
            }
        }

        private void handleGuess()
        {
            if (board[cursorY, cursorX] == 'H' || board[cursorY, cursorX] == 'M')
            {
                Console.Clear();
                Console.WriteLine("You already guessed that spot.");
                Console.WriteLine("Press any button to continue.");
                Console.ReadKey(true);
                return;
            }

            // Process guess
            if (board[cursorY, cursorX] == 'S')
            {
                board[cursorY, cursorX] = 'H';
                hits++;
                if (hits == totalHitsNeeded)
                {
                    gameRunning = false;
                    gameWon = true;
                }

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
                    Console.Clear();
                    Console.WriteLine("You sank a ship!");
                    Console.WriteLine("Press any button to continue.");
                    Console.ReadKey(true);
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Hit!");
                    Console.WriteLine("Press any button to continue.");
                    Console.ReadKey(true);
                }
            }
            else
            {
                board[cursorY, cursorX] = 'M';
                misses++;
                Console.Clear();
                Console.WriteLine("Miss!");
                Console.WriteLine("Press any button to continue.");
                Console.ReadKey(true);
            }
        }

        public void runGame()
        {
            ShowWelcomeScreen();
            DisplayBoard();
            HandleCursor();
            Console.Clear();
            if (gameWon)
            {
                Console.WriteLine("You Won!");
            }
            else
            {
                Console.WriteLine("You Lost :(");
            }
            Console.WriteLine("Press esc to stop. Press any other key to play again.");
            ConsoleKeyInfo key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.Escape)
            {
                return;
            }
            else
            {
                InitializeGame();
                runGame();
            }

        }

        private void ShowWelcomeScreen()
        {
            // Display rules before the game starts
            Console.Clear();
            Console.WriteLine("Welcome to Battleship!");
            Console.WriteLine("Rules: There are 5 ships scattered throughout the 10x10 board.");
            Console.WriteLine("You have 25 misses allowed.");
            Console.WriteLine("Guess coordinates (e.g., A5 or J10) until you sink all ships or hit 25 misses.");
            Console.WriteLine("Use the arrow keys to select coordinates, hit enter to submit your  guess");
            Console.WriteLine("Press any key to start.");
            Console.ReadKey(true);
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var game = new Game();
            game.runGame();
        }   
    }
}