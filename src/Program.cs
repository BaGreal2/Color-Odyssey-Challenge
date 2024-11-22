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
          color = Color.Red;
          break;
        case 'G':
          color = Color.Green;
          break;
        case 'B':
          color = Color.Blue;
          break;
        case 'O':
          color = Color.Orange;
          break;
        case 'C':
          color = Color.Brown;
          break;
        case 'A':
          color = Color.Gray;
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
      Vector2 boxSize = new Vector2(50, 50);
      Vector2 boxPosition = new Vector2(width / 2 - (boxSize.X * MaxLength + spacing * (MaxLength - 1)) / 2, 150);

      for (int i = 0; i < MaxAttempts; i++)
      {
        for (int j = 0; j < MaxLength; j++)
        {
          Color bgColor = Constants.BoxColor;
          if (feedback[i].Length > j)
          {
            switch (feedback[i][j])
            {
              case 'X':
                bgColor = Color.Green;
                break;
              case 'Y':
                bgColor = Color.Yellow;
                break;
            }
          }
          UI.DrawRoundedRectangle(new Rectangle(boxPosition.X + (boxSize.X + spacing) * j, boxPosition.Y + (boxSize.Y + spacing) * i, boxSize.X, boxSize.Y), 0.2f, 20, bgColor);

          char charToDraw = userCodes[i].Length > j ? userCodes[i][j] : ' ';
          Color textColor = GetColorFromChar(charToDraw);
          UI.DrawTextCentered(charToDraw.ToString(), new Vector2(boxPosition.X + (boxSize.X + spacing) * j + boxSize.X / 2, boxPosition.Y + (boxSize.Y + spacing) * i + boxSize.Y / 2), 35, 1, textColor, customFont);
        }
      }

      // Color legend
      int legendSpacing = 24;
      Vector2 colorLegendPosition = new Vector2(width / 2 - (boxSize.X * MaxLength + spacing * (MaxLength - 1)) / 2 - 130, 150);
      UI.DrawTextLeft("R: Red", new Vector2(colorLegendPosition.X, colorLegendPosition.Y), 20, 1, Color.Red, customFont);
      UI.DrawTextLeft("G: Green", new Vector2(colorLegendPosition.X, colorLegendPosition.Y + legendSpacing), 20, 1, Color.Green, customFont);
      UI.DrawTextLeft("B: Blue", new Vector2(colorLegendPosition.X, colorLegendPosition.Y + legendSpacing * 2), 20, 1, Color.Blue, customFont);
      UI.DrawTextLeft("O: Orange", new Vector2(colorLegendPosition.X, colorLegendPosition.Y + legendSpacing * 3), 20, 1, Color.Orange, customFont);
      UI.DrawTextLeft("C: Brown", new Vector2(colorLegendPosition.X, colorLegendPosition.Y + legendSpacing * 4), 20, 1, Color.Brown, customFont);
      UI.DrawTextLeft("A: Gray", new Vector2(colorLegendPosition.X, colorLegendPosition.Y + legendSpacing * 5), 20, 1, Color.Gray, customFont);

      // Attempts
      UI.DrawTextLeft($"Attempts left: {MaxAttempts - currentAttempt}", new Vector2(width / 2 + (boxSize.X * MaxLength + spacing * (MaxLength - 1)) / 2 + 50, 150), 20, 1, Constants.MenuTextColor, customFont);

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

    static void DrawVictoryScreen(Font customFont, CodeManager codeManager)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.DrawTextCentered("You won!", new Vector2(width / 2, 100), 50, 2, Constants.TitleColor, customFont);
      UI.DrawTextCentered($"The code was: {codeManager.Code}", new Vector2(width / 2, 150), 30, 1, Constants.MenuTextColor, customFont);
      UI.Button("Back to Menu", customFont, new Rectangle(width / 2 - 100, 200, 200, 50), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { currentScreen = "menu"; resetRequested = true; });
      UI.Button("Play Again", customFont, new Rectangle(width / 2 - 100, 260, 200, 50), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { currentScreen = "game"; resetRequested = true; });
    }

    static void DrawDefeatScreen(Font customFont, CodeManager codeManager)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.DrawTextCentered("You lost!", new Vector2(width / 2, 100), 50, 2, Constants.TitleColor, customFont);
      UI.DrawTextCentered($"The code was: {codeManager.Code}", new Vector2(width / 2, 150), 30, 1, Constants.MenuTextColor, customFont);
      UI.Button("Back to Menu", customFont, new Rectangle(width / 2 - 100, 200, 200, 50), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { currentScreen = "menu"; resetRequested = true; });
      UI.Button("Play Again", customFont, new Rectangle(width / 2 - 100, 260, 200, 50), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { currentScreen = "game"; resetRequested = true; });
    }

    static void RunGame()
    {
      SetConfigFlags(ConfigFlags.HighDpiWindow);
      InitWindow(Constants.WindowWidth, Constants.WindowHeight, "COC");
      SetTargetFPS(60);

      Font customFont = LoadFont("resources/fonts/Poppins/Poppins-Regular.ttf");

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
            DrawVictoryScreen(customFont, codeManager);
            break;
          case "defeat":
            DrawDefeatScreen(customFont, codeManager);
            break;
        }

        EndDrawing();
      }

      UnloadFont(customFont);
      CloseWindow();
    }
  }
}
