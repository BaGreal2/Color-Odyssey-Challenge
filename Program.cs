namespace COC
{
  class Program
  {
    static void Main(string[] args)
    {
      Console.WriteLine("Welcome to the Color Odyssey Challenge (COC)!");
      Console.WriteLine("=====================");
      Console.WriteLine("Press any key to start...");
      Console.ReadKey();

      RunGame();
    }

    static void RunGame()
    {
      bool isRunning = true;

      while (isRunning)
      {
        Console.Clear();
        Console.WriteLine("Game Menu:");
        Console.WriteLine("1. Play");
        Console.WriteLine("2. View Rules");
        Console.WriteLine("3. Exit");
        Console.Write("Choose an option (1-3): ");

        string? input = Console.ReadLine();

        switch (input)
        {
          case "1":
            StartGame();
            break;
          case "2":
            ShowRules();
            break;
          case "3":
            isRunning = false;
            Console.WriteLine("Thanks for playing!");
            break;
          default:
            Console.WriteLine("Invalid option. Please try again.");
            break;
        }

        if (isRunning)
        {
          Console.WriteLine("Press any key to return to the menu...");
          Console.ReadKey();
        }
      }
    }

    static void StartGame()
    {
      Console.Clear();
      Console.WriteLine("Starting the game...");
      Console.WriteLine("This is where the game happens!");
    }

    static void ShowRules()
    {
      Console.Clear();
      Console.WriteLine("Game Rules:");
      Console.WriteLine("1. Rule one.");
      Console.WriteLine("2. Rule two.");
      Console.WriteLine("3. Have fun!");
    }
  }
}
