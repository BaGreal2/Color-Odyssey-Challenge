namespace COC
{
  class CodeManager
  {
    char[] _colors = { 'R', 'G', 'B', 'O', 'C', 'A' };
    public string Code = "";
    const int MaxLength = 4;
    const int MaxAttempts = 6;

    public void GenerateCode(int length)
    {
      Random random = new Random();
      string result = "";

      for (int i = 0; i < length; i++)
      {
        int index = random.Next(_colors.Length);
        result += _colors[index];
      }

      Code = result;
    }

    // string GetUserCode()
    // {
    //   string userCode = "";
    //   while (userCode.Length != MaxLength)
    //   {
    //     var key = Console.ReadKey(intercept: true);
    //     if (key.Key == ConsoleKey.Enter)
    //       break;
    //
    //     if (key.Key == ConsoleKey.Backspace)
    //     {
    //       Console.ResetColor();
    //       userCode = userCode.Substring(0, userCode.Length - 1);
    //       if (Console.CursorLeft > 0)
    //       {
    //         Console.CursorLeft--;
    //         Console.CursorLeft--;
    //         Console.Write("  ");
    //         Console.CursorLeft--;
    //         Console.CursorLeft--;
    //       }
    //     }
    //     else
    //     {
    //       bool isValidCode = Array.Exists(_colors, c => c == Char.ToUpper(key.KeyChar));
    //       if (!isValidCode)
    //       {
    //         continue;
    //       }
    //       char colorCode = Char.ToUpper(key.KeyChar);
    //       userCode += colorCode;
    //       DrawBox(userCode);
    //     }
    //     Console.ResetColor();
    //   }
    //   Console.WriteLine();
    //
    //   return userCode;
    // }

    public string GetFeedback(string userCode)
    {
      string feedback = "";
      for (int i = 0; i < Code.Length; i++)
      {
        if (Code[i] == userCode[i])
        {
          feedback += "X";
        }
        else if (Code.Contains(userCode[i]))
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

    // public void Start()
    // {
    //   Console.Clear();
    //   string code = GenerateCode(MaxLength);
    //   Console.WriteLine("The game has started! Try to guess the code of four colors.");
    //   Console.WriteLine("- The color codes are: R, G, B, O, C, A.");
    //   Console.WriteLine("- You have 6 attempts to guess the code.");
    //   Console.WriteLine("- The color codes can be repeated.");
    //
    //   Console.Write("[DEBUG] The code is: ");
    //   foreach (char c in code)
    //   {
    //     ChangeConsoleColor(c);
    //     Console.Write(c);
    //     Console.Write(" ");
    //   }
    //   Console.ResetColor();
    //   Console.WriteLine();
    //
    //   for (int i = 0; i < MaxAttempts; i++)
    //   {
    //     string userCode = GetUserCode();
    //     string feedback = GetFeedback(code, userCode);
    //     Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + 2);
    //
    //     if (feedback == "XXXX")
    //     {
    //       Console.WriteLine("Congratulations! You have guessed the code correctly.");
    //       return;
    //     }
    //
    //     DrawBox(feedback);
    //     Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop + 2);
    //     Console.WriteLine();
    //     Console.WriteLine();
    //   }
    // }
  }
}
