using System.Diagnostics;

public class Program
{
    public static void Main(string[] args)
    {
        int size;
        int winningCond;

        Console.ForegroundColor = ConsoleColor.White;

        Console.Write("Enter board size: ");
        while (!int.TryParse(Console.ReadLine(), out size) || size <= 1)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("Invalid size, retry: ");
            Console.ForegroundColor = ConsoleColor.White;
        }

        Console.Write("Enter winning condition: ");
        while (!int.TryParse(Console.ReadLine(), out winningCond) || winningCond <= 1 || winningCond > size)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Invalid condition, please retry");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($"Board size \"{size}\", condition: ");
        }

        Board board = new(size, winningCond);
        board.GenerateBoard();
        board.Display();

        while (!board.Finished())
        {
            Console.Write($"Player \"{board.Whose}\": ");

            if (int.TryParse(Console.ReadLine(), out int num))
            {
                board.UpdateBoard(num);
                board.Display();

                continue;
            }
            else
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input");
                Console.ForegroundColor = ConsoleColor.White;

                board.Display();
            }
        }

        if (board.Drawn) Console.WriteLine("Draw");
        else Console.WriteLine($"Player {board.Player} wins!");

        Console.Write("Play again? (Y/N): ");
        var confirmation = Console.ReadLine().ToUpper();
        while (confirmation != "Y" && confirmation != "N")
        {
            Console.WriteLine("Invalid input, retry (Y/N)");
            confirmation = Console.ReadLine().ToUpper();
        }
        if (confirmation == "Y")
        {
            Console.Clear();
            Main(args);
        }
        else
        {
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}

public class Board(int Size, int WinningCond)
{
    public Dictionary<int, string> Slot;
    public string Whose => Turn % 2 != 0 ? "X" : "O";
    public string Player;
    public bool Drawn;

    private int Turn = 1;
    private string[] Item;
    private List<int[]> solutions => Solutions();

    public string[] GenerateBoard()
    {
        Item = new string[Size];
        Slot = new Dictionary<int, string>();

        int idx = 0;
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                Slot.Add(++idx, $"{idx}");
                Item[y] += "|_|";
            }
        }

        return Item;
    }
    public string[]? UpdateBoard(int slot)
    {
        Console.Clear();
        if (slot > Size * Size || slot <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nNon-existent, retry");
            Console.ForegroundColor = ConsoleColor.White;

            return null;
        }
        if (Slot[slot] == "X" || Slot[slot] == "O")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\nOccupied, retry");
            Console.ForegroundColor = ConsoleColor.White;

            return null;
        }

        Item = new string[Size];

        Slot[slot] = Turn % 2 != 0 ? "X" : "O";

        int idx = 0;
        for (int y = 0; y < Size; y++)
        {
            for (int x = 0; x < Size; x++)
            {
                Slot[++idx] = $"{Slot[idx]}";
                switch (Slot[idx])
                {
                    case "X":
                    case "O":
                        Item[y] += $"|{Slot[idx]}|";
                        break;
                    default:
                        Item[y] += "|_|";
                        break;
                }
            }
        }

        Player = Turn % 2 != 0 ? "X" : "O";
        Turn++;
        return Item;
    }
    public void Display()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\nWinning condition: {WinningCond}");
        Console.ForegroundColor = ConsoleColor.White;

        string horizontalGuide = " ";
        for (int i = 0; i < (Size * (Size - 1)).ToString().Length; i++)
        {
            horizontalGuide += " ";
        }
        for (int i = 1; i <= Size; i++)
        {
            if (i >= 10) horizontalGuide += $" {i}";
            else horizontalGuide += $" {i} ";
        }
        Console.WriteLine(horizontalGuide);

        for (int i = 0; i < Size; i++)
        {
            var space = " ";
            for (int j = 0; j < (Size * (Size - 1)).ToString().Length - (i * Size).ToString().Length; j++)
            {
                space += " ";
            }
            Console.Write($"{i * Size}{space}");
            Console.WriteLine(Item[i]);
        }
    }

    public bool Finished()
    {
        // Stopwatch stopwatch = new();
        // stopwatch.Start();

        bool won = false;

        for (int y = 0; y + WinningCond <= Size; y++)
        {
            for (int x = 0; x + WinningCond <= Size; x++)
            {
                won = Check(x, y, won);
            }
        }

        if (Turn > Math.Pow(Size, 2) && !won)
        {
            Drawn = true;
            return true;
        }

        // stopwatch.Stop();
        // Console.WriteLine($"Elapsed time: {stopwatch}");

        return won;
    }
    private bool Check(int x, int y, bool won)
    {
        foreach (var solution in solutions)
        {
            if (won) break;
            for (int i = 0; i < solution.Length - 1; i++)
            {
                if (Slot[solution[i] + x + y * Size] != Slot[solution[i + 1] + x + y * Size])
                {
                    won = false;
                    break;
                }
                won = true;
            }
        }
        return won;
    }

    public List<int[]> Solutions()
    {
        List<int[]> solutions = new List<int[]>();
        int[] horizontal;
        int[] vertical;

        for (int i = 0; i < WinningCond; i++)
        {
            horizontal = new int[WinningCond];
            vertical = new int[WinningCond];
            for (int j = 0; j < WinningCond; j++)
            {
                horizontal[j] = j + 1 + Size * i;
                vertical[j] = i + 1 + Size * j;
            }
            solutions.Add(horizontal);
            solutions.Add(vertical);
        }

        int[] rDiag = new int[WinningCond];
        int[] lDiag = new int[WinningCond];
        for (int i = 0; i < WinningCond; i++)
        {
            rDiag[i] = i + 1 + Size * i;
            lDiag[i] = WinningCond - i + Size * i;
        }
        solutions.Add(rDiag);
        solutions.Add(lDiag);

        return solutions;
    }
}