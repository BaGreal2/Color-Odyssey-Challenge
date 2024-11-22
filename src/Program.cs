using System.Reflection;
using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace COC
{
  class Program
  {
    static string currentScreen = "menu";
    static bool exitRequested = false;
    static bool resetRequested = false;

    const int MaxLength = 4;
    const int MaxAttempts = 6;

    static void Main(string[] args)
    {
      RunGame();
    }

    static Color GetColorFromChar(char c)
    {
      Color color;

      switch (c)
      {
        case 'R':
          color = Constants.Red;
          break;
        case 'G':
          color = Constants.Green;
          break;
        case 'B':
          color = Constants.Blue;
          break;
        case 'O':
          color = Constants.Orange;
          break;
        case 'C':
          color = Constants.Brown;
          break;
        case 'A':
          color = Constants.Gray;
          break;
        default:
          color = Color.Black;
          break;
      }

      return color;
    }

    static void DrawGameScreen(Font customFont, ref int currentAttempt, ref string[] userCodes, ref string[] feedback, CodeManager codeManager)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.DrawTextCentered("Enter colors from your keyboard!", new Vector2(width / 2, 80), 50, 2, Constants.TitleColor, customFont);

      // Boxes
      int spacing = 10;
      Vector2 boxSize = new Vector2(60, 60);
      Vector2 boxPosition = new Vector2(width / 2 - (boxSize.X * MaxLength + spacing * (MaxLength - 1)) / 2, 150);

      for (int i = 0; i < MaxAttempts; i++)
      {
        for (int j = 0; j < MaxLength; j++)
        {
          Vector2 oldSize = boxSize;
          Vector2 oldPosition = boxPosition;
          int border = 6;

          if (feedback[i].Length > j)
          {
            switch (feedback[i][j])
            {
              case 'X':
                UI.DrawRoundedRectangle(new Rectangle(boxPosition.X + (boxSize.X + spacing) * j, boxPosition.Y + (boxSize.Y + spacing) * i, boxSize.X, boxSize.Y), 0.2f, 20, Constants.Green);
                boxSize = new Vector2(boxSize.X - border, boxSize.Y - border);
                boxPosition = new Vector2(boxPosition.X + border / 2, boxPosition.Y + border / 2);
                break;
              case 'Y':
                UI.DrawRoundedRectangle(new Rectangle(boxPosition.X + (boxSize.X + spacing) * j, boxPosition.Y + (boxSize.Y + spacing) * i, boxSize.X, boxSize.Y), 0.2f, 20, Constants.Yellow);
                boxSize = new Vector2(boxSize.X - border, boxSize.Y - border);
                boxPosition = new Vector2(boxPosition.X + border / 2, boxPosition.Y + border / 2);
                break;
            }
          }

          Color bgColor = Constants.BoxColor;

          if (i == currentAttempt && j == userCodes[currentAttempt].Length)
          {
            bgColor.R -= 40;
            bgColor.G -= 40;
            bgColor.B -= 40;
          }
          UI.DrawRoundedRectangle(new Rectangle(boxPosition.X + (oldSize.X + spacing) * j, boxPosition.Y + (oldSize.Y + spacing) * i, boxSize.X, boxSize.Y), 0.2f, 20, bgColor);

          boxSize = oldSize;
          boxPosition = oldPosition;

          char charToDraw = userCodes[i].Length > j ? userCodes[i][j] : ' ';
          Color textColor = GetColorFromChar(charToDraw);
          UI.DrawTextCentered(charToDraw.ToString(), new Vector2(boxPosition.X + (boxSize.X + spacing) * j + boxSize.X / 2, boxPosition.Y + (boxSize.Y + spacing) * i + boxSize.Y / 2), 35, 1, textColor, customFont);
        }
      }

      // Color legend
      int legendSpacing = 24;
      Vector2 colorLegendPosition = new Vector2(width / 2 - (boxSize.X * MaxLength + spacing * (MaxLength - 1)) / 2 - 150, 150);
      UI.DrawTextLeft("R: Red", new Vector2(colorLegendPosition.X, colorLegendPosition.Y), 25, 1, Constants.Red, customFont);
      UI.DrawTextLeft("G: Green", new Vector2(colorLegendPosition.X, colorLegendPosition.Y + legendSpacing), 25, 1, Constants.Green, customFont);
      UI.DrawTextLeft("B: Blue", new Vector2(colorLegendPosition.X, colorLegendPosition.Y + legendSpacing * 2), 25, 1, Constants.Blue, customFont);
      UI.DrawTextLeft("O: Orange", new Vector2(colorLegendPosition.X, colorLegendPosition.Y + legendSpacing * 3), 25, 1, Constants.Orange, customFont);
      UI.DrawTextLeft("C: Brown", new Vector2(colorLegendPosition.X, colorLegendPosition.Y + legendSpacing * 4), 25, 1, Constants.Brown, customFont);
      UI.DrawTextLeft("A: Gray", new Vector2(colorLegendPosition.X, colorLegendPosition.Y + legendSpacing * 5), 25, 1, Constants.Gray, customFont);

      // Attempts
      UI.DrawTextLeft($"Attempts left: {MaxAttempts - currentAttempt}", new Vector2(width / 2 + (boxSize.X * MaxLength + spacing * (MaxLength - 1)) / 2 + 50, 150), 25, 1, Constants.MenuTextColor, customFont);

      // Input handling
      int keyPressed = GetKeyPressed();

      if (keyPressed != 0)
      {
        char keyChar = (char)keyPressed;
        bool isValidCode = Array.Exists(new char[] { 'R', 'G', 'B', 'O', 'C', 'A' }, c => c == Char.ToUpper(keyChar));
        if (isValidCode)
        {
          userCodes[currentAttempt] += Char.ToUpper(keyChar);
          if (userCodes[currentAttempt].Length == MaxLength)
          {
            currentAttempt++;

            if (codeManager.GetFeedback(userCodes[currentAttempt - 1]) == "XXXX")
            {
              currentScreen = "victory";
            }
            else if (currentAttempt == MaxAttempts)
            {
              currentScreen = "defeat";
            }

            for (int i = 0; i < MaxAttempts; i++)
            {
              feedback[i] = codeManager.GetFeedback(userCodes[i]);
            }
          }
        }
        else if (keyPressed == (int)KeyboardKey.Backspace)
        {
          if (userCodes[currentAttempt].Length > 0)
          {
            userCodes[currentAttempt] = userCodes[currentAttempt].Substring(0, userCodes[currentAttempt].Length - 1);
          }
        }
      }
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

      UI.DrawTextLeft("1. The program draws the codes of four colors chosen from among six:", textPosition, 24, letterSpacing, Constants.MenuTextColor, customFont);
      UI.DrawTextLeft("   R: Red", textPosition + spacing, 24, letterSpacing, Constants.Red, customFont);
      UI.DrawTextLeft("   G: Green", textPosition + spacing * 2, 24, letterSpacing, Constants.Green, customFont);
      UI.DrawTextLeft("   B: Blue", textPosition + spacing * 3, 24, letterSpacing, Constants.Blue, customFont);
      UI.DrawTextLeft("   O: Orange", textPosition + spacing * 4, 24, letterSpacing, Constants.Orange, customFont);
      UI.DrawTextLeft("   C: Brown", textPosition + spacing * 5, 24, letterSpacing, Constants.Brown, customFont);
      UI.DrawTextLeft("   A: Gray", textPosition + spacing * 6, 24, letterSpacing, Constants.Gray, customFont);
      UI.DrawTextLeft("2. The player enters his code. The program informs the player:", textPosition + spacing * 7, 24, letterSpacing, Constants.MenuTextColor, customFont);
      UI.DrawTextLeft("   Yellow border: color matches but position does not match", textPosition + spacing * 8, 24, letterSpacing, Constants.Yellow, customFont);
      UI.DrawTextLeft("   Green border: both color and position match.", textPosition + spacing * 9, 24, letterSpacing, Constants.Green, customFont);
      UI.DrawTextLeft("3. If you get 4 green borders - you win.", textPosition + spacing * 10, 24, letterSpacing, Constants.MenuTextColor, customFont);
      UI.DrawTextLeft("4. If you made a 6 attempt to guess the color code - you lose.", textPosition + spacing * 11, 24, letterSpacing, Constants.MenuTextColor, customFont);

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
      UI.Button("View Stats", customFont, new Rectangle(menuButtonPosition.X, menuButtonPosition.Y + (menuButtonSize.Y + spacing) * 2, menuButtonSize.X, menuButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => currentScreen = "stats");
      UI.Button("Exit", customFont, new Rectangle(menuButtonPosition.X, menuButtonPosition.Y + (menuButtonSize.Y + spacing) * 3, menuButtonSize.X, menuButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => exitRequested = true);
    }

    public static void SaveStat(bool isVictory, int attempts = 0)
    {
      string path = Constants.StatsPath;
      string stat = (isVictory ? "V" : "D") + attempts.ToString();

      if (File.Exists(path))
      {
        File.AppendAllText(path, stat + Environment.NewLine);
      }
      else
      {
        File.WriteAllText(path, stat + Environment.NewLine);
      }
    }

    public static string ReadStats()
    {
      string path = Constants.StatsPath;

      if (File.Exists(path))
      {
        string[] stats = File.ReadAllLines(path);
        int victories = 0;
        int defeats = 0;
        int totalAttempts = 0;

        foreach (string stat in stats)
        {
          if (stat[0] == 'V')
          {
            victories++;
            totalAttempts += int.Parse(stat.Substring(1));
          }
          else
          {
            defeats++;
          }
        }
        return stats.Length == 0 ? "" : $"Victories: {victories}\n\nDefeats: {defeats}\n\nAverage attempts to win: {totalAttempts / (float)victories}";
      }
      return "";
    }

    static void DrawVictoryScreen(Font customFont, CodeManager codeManager, int attempts)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.DrawTextCentered("You won!", new Vector2(width / 2, 100), 50, 2, Constants.TitleColor, customFont);
      UI.DrawTextCentered($"The code was: {codeManager.Code}", new Vector2(width / 2, 150), 30, 1, Constants.MenuTextColor, customFont);

      int spacing = 20;
      Vector2 feedbackButtonSize = new Vector2(200, 50);
      Vector2 feedbackButtonPosition = new Vector2(width / 2 - (feedbackButtonSize.X) / 2, height - feedbackButtonSize.Y * 2 - 50 - spacing);

      UI.Button("Back to Menu", customFont, new Rectangle(feedbackButtonPosition.X, feedbackButtonPosition.Y, feedbackButtonSize.X, feedbackButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { currentScreen = "menu"; resetRequested = true; SaveStat(true, attempts); });
      UI.Button("Play Again", customFont, new Rectangle(feedbackButtonPosition.X, feedbackButtonPosition.Y + feedbackButtonSize.Y + spacing, feedbackButtonSize.X, feedbackButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { currentScreen = "game"; resetRequested = true; SaveStat(true, attempts); });
    }

    static void DrawDefeatScreen(Font customFont, CodeManager codeManager, int attempts)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.DrawTextCentered("You lost!", new Vector2(width / 2, 100), 50, 2, Constants.TitleColor, customFont);
      UI.DrawTextCentered($"The code was: {codeManager.Code}", new Vector2(width / 2, 150), 30, 1, Constants.MenuTextColor, customFont);

      int spacing = 20;
      Vector2 feedbackButtonSize = new Vector2(200, 50);
      Vector2 feedbackButtonPosition = new Vector2(width / 2 - (feedbackButtonSize.X) / 2, height - feedbackButtonSize.Y * 2 - 50 - spacing);

      UI.Button("Back to Menu", customFont, new Rectangle(feedbackButtonPosition.X, feedbackButtonPosition.Y, feedbackButtonSize.X, feedbackButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { currentScreen = "menu"; resetRequested = true; SaveStat(false); });
      UI.Button("Play Again", customFont, new Rectangle(feedbackButtonPosition.X, feedbackButtonPosition.Y + feedbackButtonSize.Y + spacing, feedbackButtonSize.X, feedbackButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { currentScreen = "game"; resetRequested = true; SaveStat(false); });
    }

    static void DrawStatsScreen(Font customFont)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.DrawTextCentered("STATISTICS", new Vector2(width / 2, 50), 50, 2, Constants.TitleColor, customFont);

      string stats = ReadStats();
      UI.DrawTextLeft(stats, new Vector2(50, 100), 24, 1, Constants.MenuTextColor, customFont);

      Vector2 backButtonSize = new Vector2(200, 50);
      Vector2 backButtonPosition = new Vector2(width / 2 - (backButtonSize.X) / 2, height - backButtonSize.Y - 50);

      UI.Button("Back", customFont, new Rectangle(backButtonPosition.X, backButtonPosition.Y, backButtonSize.X, backButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => currentScreen = "menu");
    }

    static void RunGame()
    {
      InitWindow(Constants.WindowWidth, Constants.WindowHeight, "COC");
      SetTargetFPS(60);

      string tempFontPath = Path.Combine(Path.GetTempPath(), "Poppins-Regular.ttf");
      if (!File.Exists(tempFontPath))
      {
        using (Stream fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FinalProject.resources.fonts.Poppins-Regular.ttf"))
        using (FileStream fileStream = new FileStream(tempFontPath, FileMode.Create, FileAccess.Write))
        {
          fontStream.CopyTo(fileStream);
        }
      }

      Font customFont = LoadFontEx(tempFontPath, 90, null, 0);
      SetTextureFilter(customFont.Texture, TextureFilter.Bilinear);

      string[] userCodes = new string[MaxAttempts];
      string[] feedback = new string[MaxAttempts];
      int currentAttempt = 0;

      for (int i = 0; i < MaxAttempts; i++)
      {
        feedback[i] = "";
        userCodes[i] = "";
      }

      CodeManager codeManager = new CodeManager(MaxLength);
      codeManager.GenerateCode();

      while (!WindowShouldClose() && !exitRequested)
      {
        if (resetRequested)
        {
          currentAttempt = 0;
          for (int i = 0; i < MaxAttempts; i++)
          {
            feedback[i] = "";
            userCodes[i] = "";
          }
          codeManager.GenerateCode();
          resetRequested = false;
        }

        BeginDrawing();

        switch (currentScreen)
        {
          case "menu":
            DrawMenuScreen(customFont);
            break;
          case "game":
            DrawGameScreen(customFont, ref currentAttempt, ref userCodes, ref feedback, codeManager);
            break;
          case "rules":
            DrawRulesScreen(customFont);
            break;
          case "victory":
            DrawVictoryScreen(customFont, codeManager, currentAttempt);
            break;
          case "defeat":
            DrawDefeatScreen(customFont, codeManager, currentAttempt);
            break;
          case "stats":
            DrawStatsScreen(customFont);
            break;
        }

        EndDrawing();
      }

      UnloadFont(customFont);
      CloseWindow();
    }
  }
}
