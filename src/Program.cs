using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace COC
{
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
