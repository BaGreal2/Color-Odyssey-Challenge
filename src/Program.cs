using System.Reflection;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace COC
{
  class Program
  {
    static string currentScreen = "menu";
    static string mode = "random";
    static bool exitRequested = false;
    static bool resetRequested = false;

    static Font LoadLocalFont(string fontName, int fontSize)
    {
      string tempFontPath = Path.Combine(Path.GetTempPath(), fontName);
      if (!File.Exists(tempFontPath))
      {
        using (Stream? fontStream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"FinalProject.resources.fonts.{fontName}"))
        {
          if (fontStream == null)
          {
            throw new InvalidOperationException($"Resource not found: {fontName}");
          }

          using (FileStream fileStream = new FileStream(tempFontPath, FileMode.Create, FileAccess.Write))
          {
            fontStream.CopyTo(fileStream);
          }
        }
      }

      Font font = LoadFontEx(tempFontPath, fontSize, null, 0);
      SetTextureFilter(font.Texture, TextureFilter.Bilinear);
      return font;
    }

    static void Main(string[] args)
    {
      SetConfigFlags(ConfigFlags.ResizableWindow);
      InitWindow(Constants.WindowWidth, Constants.WindowHeight, Constants.GameTitle);
      SetTargetFPS(60);

      Font customFont = LoadLocalFont("Poppins-Regular.ttf", 90);
      CodeManager codeManager = new CodeManager(Constants.MaxLength);
      Game game = new Game(customFont, (bool value) => resetRequested = value, (bool value) => exitRequested = value, (string screen) => currentScreen = screen, () => codeManager.GenerateCode(mode), Constants.MaxAttempts, Constants.MaxLength);

      while (!WindowShouldClose() && !exitRequested)
      {
        SetMouseCursor(MouseCursor.Default);

        if (resetRequested)
        {
          game.Reset();
          codeManager.GenerateCode(mode);
        }

        BeginDrawing();

        switch (currentScreen)
        {
          case "menu":
            game.MenuScreen();
            break;
          case "game":
            game.GameScreen(codeManager);
            break;
          case "mode_select":
            game.ModeSelectScreen((string value) => mode = value);
            break;
          case "rules":
            game.RulesScreen();
            break;
          case "victory":
            game.VictoryScreen(codeManager, mode);
            break;
          case "defeat":
            game.DefeatScreen(codeManager, mode);
            break;
          case "stats":
            game.StatsScreen();
            break;
        }

        EndDrawing();
      }

      UnloadFont(customFont);
      CloseWindow();
    }
  }
}
