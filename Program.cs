namespace COC
{
  class CodeGuesser
  {
    char[] _colors = { 'R', 'G', 'B', 'O', 'C', 'A' };
    const int MaxLength = 4;
    const int MaxAttempts = 6;

    void ChangeConsoleColor(char colorCode)
    {
      switch (colorCode)
      {
        case 'R':
          Console.ForegroundColor = ConsoleColor.Red;
          break;
        case 'G':
          Console.ForegroundColor = ConsoleColor.Green;
          break;
        case 'B':
          Console.ForegroundColor = ConsoleColor.Blue;
          break;
        case 'O':
          Console.ForegroundColor = ConsoleColor.DarkYellow;
          break;
        case 'C':
          Console.ForegroundColor = ConsoleColor.DarkRed;
          break;
        case 'A':
          Console.ForegroundColor = ConsoleColor.Gray;
          break;
        case 'Y':
          Console.ForegroundColor = ConsoleColor.Yellow;
          break;
        case 'X':
          Console.ForegroundColor = ConsoleColor.Green;
          break;
        default:
          Console.ResetColor();
          break;
      }
    }

    string GenerateCode(int length)
    {
      Random random = new Random();
      string result = "";

      for (int i = 0; i < length; i++)
      {
        int index = random.Next(_colors.Length);
        result += _colors[index];
      }

      return result;
    }

    // TODO: Any max width of code
    void DrawBox(string code)
    {
      int currentCursorLeft = Console.CursorLeft;
      int currentCursorTop = Console.CursorTop;

      Console.SetCursorPosition(0, currentCursorTop);
      Console.Write(new string(' ', Console.WindowWidth)); // Clear the current line

      Console.SetCursorPosition(0, currentCursorTop);  // Move back to the start of the current line
      Console.WriteLine("┌───┬───┬───┬───┐");

      int i = 0;
      while (i < code.Length)
      {
        Console.Write("│ ");
        ChangeConsoleColor(code[i]);
        Console.Write(code[i]);
        Console.ResetColor();
        Console.Write(" ");
        i++;
      }

      if (i < MaxLength)
      {
        for (int j = i; j < MaxLength; j++)
        {
          Console.Write("│   ");
        }
      }

      Console.WriteLine("│");
      Console.WriteLine("└───┴───┴───┴───┘");

      Console.SetCursorPosition(currentCursorLeft, currentCursorTop);
    }

    string GetUserCode()
    {
      string userCode = "";
      while (userCode.Length != MaxLength)
      {
        var key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Enter)
          break;

        if (key.Key == ConsoleKey.Backspace)
        {
          Console.ResetColor();
          userCode = userCode.Substring(0, userCode.Length - 1);
          if (Console.CursorLeft > 0)
          {
            Console.CursorLeft--;
            Console.CursorLeft--;
            Console.Write("  ");
            Console.CursorLeft--;
            Console.CursorLeft--;
          }
        }
        else
        {
          bool isValidCode = Array.Exists(_colors, c => c == Char.ToUpper(key.KeyChar));
          if (!isValidCode)
          {
            continue;
          }
          char colorCode = Char.ToUpper(key.KeyChar);
          userCode += colorCode;
          DrawBox(userCode);
        }
        Console.ResetColor();
      }
      Console.WriteLine();

      return userCode;
    }

    string GetFeedback(string code, string userCode)
    {
      string feedback = "";
      for (int i = 0; i < code.Length; i++)
      {
        if (code[i] == userCode[i])
        {
          feedback += "X";
        }
        else if (code.Contains(userCode[i]))
        {
          feedback += "Y";
        }
        else
        {
          feedback += " ";
        }
      }
      return feedback;
    }

    public void Start()
    {
      Console.Clear();
      string code = GenerateCode(MaxLength);
      Console.WriteLine("The game has started! Try to guess the code of four colors.");
      Console.WriteLine("- The color codes are: R, G, B, O, C, A.");
      Console.WriteLine("- You have 6 attempts to guess the code.");
      Console.WriteLine("- The color codes can be repeated.");

      Console.Write("[DEBUG] The code is: ");
      foreach (char c in code)
      {
        ChangeConsoleColor(c);
        Console.Write(c);
        Console.Write(" ");
      }
      Console.ResetColor();
      Console.WriteLine();

      for (int i = 0; i < MaxAttempts; i++)
      {
        string userCode = GetUserCode();
        string feedback = GetFeedback(code, userCode);
        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + 2);

        if (feedback == "XXXX")
        {
          Console.WriteLine("Congratulations! You have guessed the code correctly.");
          return;
        }

        DrawBox(feedback);
        Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + 2);
        Console.WriteLine();
        Console.WriteLine();
      }
    }
  }

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
      CodeGuesser cg = new CodeGuesser();

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
            cg.Start();
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

    static void ShowRules()
    {
      Console.Clear();
      Console.WriteLine("Game Rules:");
      Console.WriteLine("1. The program draws the codes of four colors chosen from among six (the color codes can be repeated); the color codes are:");
      Console.ForegroundColor = ConsoleColor.Red;
      Console.WriteLine("R: Red");
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("G: Green");
      Console.ForegroundColor = ConsoleColor.Blue;
      Console.WriteLine("B: Blue");
      Console.ForegroundColor = ConsoleColor.DarkYellow;
      Console.WriteLine("O: Orange");
      Console.ForegroundColor = ConsoleColor.DarkRed;
      Console.WriteLine("C: Brown");
      Console.ForegroundColor = ConsoleColor.Gray;
      Console.WriteLine("A: Gray");
      Console.ResetColor();
      Console.WriteLine("2. The player enters his code for the four colors. The program informs the player that the two color codes match as follows:\nY: color matches but position does not match\nX: both color and position match.");
      Console.WriteLine("3. If the player gets four X symbols, the game ends with the player winning.");
      Console.WriteLine("4. If the player has made a sixth attempt to guess the color code of the program, the game ends with the player losing.");
    }
  }
}
