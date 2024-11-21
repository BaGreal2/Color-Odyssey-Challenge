using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace COC
{
  class Constants
  {
    public static readonly int WindowWidth = 800;
    public static readonly int WindowHeight = 600;

    // #3E4E50
    // #FACFAD
    // #F8BD7F
    // #F5AC72
    // #F2AA7E
    public static readonly Color BackgroundColor = new Color(0x3E, 0x4E, 0x50, 255);
    public static readonly Color MenuButtonColor = new Color(0xF5, 0xAC, 0x72, 255);
    public static readonly Color MenuButtonTextColor = new Color(0x8F, 70, 50, 255);
    public static readonly Color MenuTextColor = new Color(0xFA, 0xFA, 0xFA, 255);
    public static readonly Color TitleColor = new Color(0xFA, 0xCF, 0xAD, 255);
  }
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

  class UI
  {
    static Color GetHoverColor(Color bgColor)
    {
      return GetAdjustedColor(bgColor, 20);
    }

    static Color GetActiveColor(Color bgColor)
    {
      return GetAdjustedColor(bgColor, 30);
    }

    static Color GetAdjustedColor(Color color, int adjustment)
    {
      int brightness = (color.R + color.G + color.B) / 3;

      bool closerToWhite = brightness > 127;

      int adjustValue = closerToWhite ? -adjustment : adjustment;

      int r = Math.Clamp(color.R + adjustValue, 0, 255);
      int g = Math.Clamp(color.G + adjustValue, 0, 255);
      int b = Math.Clamp(color.B + adjustValue, 0, 255);

      return new Color(r, g, b, color.A);
    }

    public static void DrawRoundedRectangle(Rectangle rect, float roundness, int segments, Color color)
    {
      float radius = Math.Min(rect.Width, rect.Height) * roundness / 2;

      DrawRectangle((int)(rect.X + radius), (int)rect.Y, (int)(rect.Width - 2 * radius), (int)rect.Height, color); // Horizontal
      DrawRectangle((int)rect.X, (int)(rect.Y + radius), (int)rect.Width, (int)(rect.Height - 2 * radius), color); // Vertical

      DrawCircleSector(new Vector2(rect.X + radius, rect.Y + radius), radius, 180, 270, segments, color); // Top-left
      DrawCircleSector(new Vector2(rect.X + rect.Width - radius, rect.Y + radius), radius, 270, 360, segments, color); // Top-right
      DrawCircleSector(new Vector2(rect.X + radius, rect.Y + rect.Height - radius), radius, 90, 180, segments, color); // Bottom-left
      DrawCircleSector(new Vector2(rect.X + rect.Width - radius, rect.Y + rect.Height - radius), radius, 0, 90, segments, color); // Bottom-right
    }

    public static void DrawTextCentered(string text, Vector2 position, float fontSize, float spacing, Color color, Font font)
    {
      Vector2 textSize = MeasureTextEx(font, text, fontSize, spacing);
      Vector2 centeredPosition = position - textSize / 2;

      DrawTextEx(font, text, centeredPosition, fontSize, spacing, color);
    }

    public static void DrawTextLeft(string text, Vector2 position, float fontSize, float spacing, Color color, Font font)
    {
      DrawTextEx(font, text, position, fontSize, spacing, color);
    }

    public static void Button(string text, Font font, Rectangle bounds, Color bgColor, Color textColor, Action callback)
    {
      Vector2 mousePos = GetMousePosition();
      bool isHovered = CheckCollisionPointRec(mousePos, bounds);
      bool isActive = isHovered && IsMouseButtonDown(MouseButton.Left);
      Color hoverColor = GetHoverColor(bgColor);
      Color activeColor = GetActiveColor(bgColor);

      Color currentColor = bgColor;
      if (isHovered)
      {
        currentColor = hoverColor;
      }
      if (isActive)
      {
        currentColor = activeColor;
      }

      bool isClicked = isHovered && IsMouseButtonPressed(MouseButton.Left);
      if (isClicked)
      {
        callback();
      }

      DrawRoundedRectangle(bounds, 0.2f, 10, currentColor);
      DrawTextCentered(text, new Vector2(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2), 30, 2, textColor, font);
    }
  }

  class Program
  {
    static string currentScreen = "menu";
    static bool exitRequested = false;

    static void Main(string[] args)
    {
      RunGame();
    }

    static void DrawGameScreen(Font customFont)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();
    }

    static void DrawRulesScreen(Font customFont)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.DrawTextCentered("RULES", new Vector2(width / 2, 50), 50, 2, Constants.TitleColor, customFont);


      int leftMargin = 50;
      float letterSpacing = 1;
      Vector2 spacing = new Vector2(0, 24);
      Vector2 textPosition = new Vector2(leftMargin, 100);

      UI.DrawTextLeft("1. The program draws the codes of four colors chosen from among six:", textPosition, 20, letterSpacing, Constants.MenuTextColor, customFont);
      UI.DrawTextLeft("   R: Red", textPosition + spacing, 20, letterSpacing, Color.Red, customFont);
      UI.DrawTextLeft("   G: Green", textPosition + spacing * 2, 20, letterSpacing, Color.Green, customFont);
      UI.DrawTextLeft("   B: Blue", textPosition + spacing * 3, 20, letterSpacing, Color.Blue, customFont);
      UI.DrawTextLeft("   O: Orange", textPosition + spacing * 4, 20, letterSpacing, Color.Orange, customFont);
      UI.DrawTextLeft("   C: Brown", textPosition + spacing * 5, 20, letterSpacing, Color.Brown, customFont);
      UI.DrawTextLeft("   A: Gray", textPosition + spacing * 6, 20, letterSpacing, Color.Gray, customFont);
      UI.DrawTextLeft("2. The player enters his code. The program informs the player:", textPosition + spacing * 7, 20, letterSpacing, Constants.MenuTextColor, customFont);
      UI.DrawTextLeft("   Y: color matches but position does not match", textPosition + spacing * 8, 20, letterSpacing, Color.Yellow, customFont);
      UI.DrawTextLeft("   X: both color and position match.", textPosition + spacing * 9, 20, letterSpacing, Color.Green, customFont);
      UI.DrawTextLeft("3. If you get 4 X symbols - you win.", textPosition + spacing * 10, 20, letterSpacing, Constants.MenuTextColor, customFont);
      UI.DrawTextLeft("4. If you made a 6 attempt to guess the color code - you lose.", textPosition + spacing * 11, 20, letterSpacing, Constants.MenuTextColor, customFont);

      Vector2 backButtonSize = new Vector2(200, 50);
      Vector2 backButtonPosition = new Vector2(width / 2 - (backButtonSize.X) / 2, height - backButtonSize.Y - 50);

      UI.Button("Back", customFont, new Rectangle(backButtonPosition.X, backButtonPosition.Y, backButtonSize.X, backButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => currentScreen = "menu");
    }

    static void DrawMenuScreen(Font customFont)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.DrawTextCentered("COLOR ODYSSEY CHALLENGE", new Vector2(width / 2, 100), 50, 2, Constants.TitleColor, customFont);

      Vector2 menuButtonSize = new Vector2(200, 50);
      Vector2 menuButtonPosition = new Vector2(width / 2 - (menuButtonSize.X) / 2, 200);
      int spacing = 10;

      UI.Button("Start Game", customFont, new Rectangle(menuButtonPosition.X, menuButtonPosition.Y, menuButtonSize.X, menuButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => currentScreen = "game");
      UI.Button("View Rules", customFont, new Rectangle(menuButtonPosition.X, menuButtonPosition.Y + menuButtonSize.Y + spacing, menuButtonSize.X, menuButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => currentScreen = "rules");
      UI.Button("Exit", customFont, new Rectangle(menuButtonPosition.X, menuButtonPosition.Y + (menuButtonSize.Y + spacing) * 2, menuButtonSize.X, menuButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => exitRequested = true);
    }

    static void RunGame()
    {
      SetConfigFlags(ConfigFlags.HighDpiWindow);
      InitWindow(Constants.WindowWidth, Constants.WindowHeight, "COC");
      SetTargetFPS(60);

      Font customFont = LoadFont("resources/fonts/Poppins/Poppins-Regular.ttf");

      while (!WindowShouldClose() && !exitRequested)
      {
        BeginDrawing();

        switch (currentScreen)
        {
          case "menu":
            DrawMenuScreen(customFont);
            break;
          case "game":
            DrawGameScreen(customFont);
            break;
          case "rules":
            DrawRulesScreen(customFont);
            break;
        }

        EndDrawing();
      }

      UnloadFont(customFont);
      CloseWindow();
    }
  }
}
