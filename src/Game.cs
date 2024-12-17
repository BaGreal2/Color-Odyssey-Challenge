using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace COC
{
  public class Game
  {
    Action<string> screenSetter;
    Action<bool> exitSetter;
    Action<bool> resetSetter;
    Action gameInit;

    int maxLength;
    int maxAttempts;

    int currentAttempt = 0;
    string[] userCodes;
    string[] feedback;

    Font customFont;

    public Game(Font _customFont, Action<bool> _resetSetter, Action<bool> _exitSetter, Action<string> _screenSetter, Action _gameInit, int _maxAttempts = 6, int _maxLength = 4)
    {
      screenSetter = _screenSetter;
      resetSetter = _resetSetter;
      exitSetter = _exitSetter;
      maxLength = _maxLength;
      maxAttempts = _maxAttempts;
      customFont = _customFont;
      gameInit = _gameInit;
      userCodes = new string[maxAttempts];
      feedback = new string[maxAttempts];
      for (int i = 0; i < maxAttempts; i++)
      {
        feedback[i] = "";
        userCodes[i] = "";
      }
    }

    public void Reset()
    {
      currentAttempt = 0;
      for (int i = 0; i < maxAttempts; i++)
      {
        feedback[i] = "";
        userCodes[i] = "";
      }
      resetSetter(false);
    }

    Color GetColorFromChar(char c)
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

    void SaveStat(bool isVictory, string mode, int attempts = 0)
    {
      string appDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "COC");
      string path;
      if (mode == "random")
      {
        path = Path.Combine(appDirectory, Constants.StatsPath);
      }
      else
      {
        path = Path.Combine(appDirectory, Constants.DailyStatsPath);
      }
      string stat = (isVictory ? "V" : "D") + attempts.ToString();

      try
      {
        if (!Directory.Exists(appDirectory))
        {
          Directory.CreateDirectory(appDirectory);
        }

        if (File.Exists(path))
        {
          File.AppendAllText(path, stat + Environment.NewLine);
        }
        else
        {
          File.WriteAllText(path, stat + Environment.NewLine);
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error saving stats: {ex.Message}");
      }
    }

    public static string ReadStats(string mode)
    {
      string appDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "COC");
      string path;
      if (mode == "random")
      {
        path = Path.Combine(appDirectory, Constants.StatsPath);
      }
      else
      {
        path = Path.Combine(appDirectory, Constants.DailyStatsPath);
      }

      try
      {
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

          if (stats.Length == 0)
          {
            return "";
          }

          return $"Victories: {victories}\n\nDefeats: {defeats}\n\nAverage attempts to win: {totalAttempts / (float)victories}";
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error reading stats: {ex.Message}");
      }

      return "";
    }

    public void MenuScreen()
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.TextCentered("COLOR ODYSSEY CHALLENGE", new Vector2(width / 2, 100), 50, 2, Constants.TitleColor, customFont);

      int spacing = 10;
      Vector2 menuButtonSize = new Vector2(200, 50);
      Vector2 menuButtonPosition = new Vector2(width / 2 - (menuButtonSize.X) / 2, (height / 2) - menuButtonSize.Y * 2 - spacing * 2);

      UI.Button("Start Game", customFont, new Rectangle(menuButtonPosition.X, menuButtonPosition.Y, menuButtonSize.X, menuButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => screenSetter("mode_select"));
      UI.Button("View Rules", customFont, new Rectangle(menuButtonPosition.X, menuButtonPosition.Y + menuButtonSize.Y + spacing, menuButtonSize.X, menuButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => screenSetter("rules"));
      UI.Button("View Stats", customFont, new Rectangle(menuButtonPosition.X, menuButtonPosition.Y + (menuButtonSize.Y + spacing) * 2, menuButtonSize.X, menuButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => screenSetter("stats"));
      UI.Button("Exit", customFont, new Rectangle(menuButtonPosition.X, menuButtonPosition.Y + (menuButtonSize.Y + spacing) * 3, menuButtonSize.X, menuButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => exitSetter(true));
    }

    void GetColorInput(char ch, ref string[] userCodes, ref CodeManager codeManager)
    {
      bool isValidCode = Array.Exists(new char[] { 'R', 'G', 'B', 'O', 'C', 'A' }, c => c == Char.ToUpper(ch));
      if (isValidCode)
      {
        if (userCodes[currentAttempt].Length == maxLength)
        {
          return;
        }
        userCodes[currentAttempt] += Char.ToUpper(ch);
      }
    }

    void HandleSubmit(ref string[] userCodes, ref CodeManager codeManager)
    {
      if (userCodes[currentAttempt].Length == maxLength)
      {
        currentAttempt++;

        if (codeManager.GetFeedback(userCodes[currentAttempt - 1]) == "XXXX")
        {
          screenSetter("victory");
        }
        else if (currentAttempt == maxAttempts)
        {
          screenSetter("defeat");
        }

        for (int i = 0; i < maxAttempts; i++)
        {
          feedback[i] = codeManager.GetFeedback(userCodes[i]);
        }
      }
    }

    bool HasLetterBeenUsed(char ch, ref string[] userCodes, string code)
    {
      for (int i = 0; i < currentAttempt; i++)
      {
        if (userCodes[i].Contains(ch))
        {
          return !code.Contains(ch);
        }
      }
      return false;
    }

    public void GameScreen(CodeManager codeManager)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.TextCentered("Enter colors from your keyboard!", new Vector2(width / 2, 80), 50, 2, Constants.TitleColor, customFont);
      UI.Button("Back", customFont, new Rectangle(50, 80, 100, 40), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { screenSetter("menu"); resetSetter(true); });

      // Boxes
      int spacing = 10;
      Vector2 boxSize = new Vector2(60, 60);
      float contentPositionY = height / 2 - (boxSize.Y * maxAttempts + spacing * (maxAttempts - 1)) / 2 + 50;
      Vector2 boxPosition = new Vector2(width / 2 - (boxSize.X * maxLength + spacing * (maxLength - 1)) / 2, contentPositionY);

      for (int i = 0; i < maxAttempts; i++)
      {
        for (int j = 0; j < maxLength; j++)
        {
          Vector2 oldSize = boxSize;
          Vector2 oldPosition = boxPosition;
          int border = 10;
          int innerBorder = 2;

          if (feedback[i].Length > j)
          {
            switch (feedback[i][j])
            {
              case 'X':
                UI.RoundedRectangle(new Rectangle(boxPosition.X + (boxSize.X + spacing) * j, boxPosition.Y + (boxSize.Y + spacing) * i, boxSize.X, boxSize.Y), 0.2f, 20, Constants.Green);
                boxSize = new Vector2(boxSize.X - border, boxSize.Y - border);
                boxPosition = new Vector2(boxPosition.X + border / 2, boxPosition.Y + border / 2);
                UI.RoundedRectangle(new Rectangle(boxPosition.X + (oldSize.X + spacing) * j - innerBorder / 2, boxPosition.Y + (oldSize.Y + spacing) * i - innerBorder / 2, boxSize.X + innerBorder, boxSize.Y + innerBorder), 0.2f, 20, Color.Black);
                break;
              case 'Y':
                UI.RoundedRectangle(new Rectangle(boxPosition.X + (boxSize.X + spacing) * j, boxPosition.Y + (boxSize.Y + spacing) * i, boxSize.X, boxSize.Y), 0.2f, 20, Constants.Yellow);
                boxSize = new Vector2(boxSize.X - border, boxSize.Y - border);
                boxPosition = new Vector2(boxPosition.X + border / 2, boxPosition.Y + border / 2);
                UI.RoundedRectangle(new Rectangle(boxPosition.X + (oldSize.X + spacing) * j - innerBorder / 2, boxPosition.Y + (oldSize.Y + spacing) * i - innerBorder / 2, boxSize.X + innerBorder, boxSize.Y + innerBorder), 0.2f, 20, Color.Black);
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

          char charToDraw = userCodes[i].Length > j ? userCodes[i][j] : ' ';
          Color elementColor = GetColorFromChar(charToDraw);
          UI.RoundedRectangle(new Rectangle(boxPosition.X + (oldSize.X + spacing) * j, boxPosition.Y + (oldSize.Y + spacing) * i, boxSize.X, boxSize.Y), 0.2f, 20, charToDraw == ' ' ? bgColor : elementColor);

          boxSize = oldSize;
          boxPosition = oldPosition;
        }
      }

      // Color buttons
      int colorButtonSpacing = 16;
      int colorButtonSize = 60;
      Vector2 colorButtonPosition = new Vector2(width / 2 - (boxSize.X * maxLength + spacing * (maxLength - 1)) / 2 - colorButtonSize - spacing - 24, contentPositionY);
      UI.Button("", customFont, new Rectangle(colorButtonPosition.X, colorButtonPosition.Y, colorButtonSize, colorButtonSize), Constants.Red, Constants.MenuTextColor, () => { GetColorInput('R', ref userCodes, ref codeManager); }, HasLetterBeenUsed('R', ref userCodes, codeManager.Code));
      UI.Button("", customFont, new Rectangle(colorButtonPosition.X, colorButtonPosition.Y + colorButtonSize + colorButtonSpacing, colorButtonSize, colorButtonSize), Constants.Green, Constants.MenuTextColor, () => { GetColorInput('G', ref userCodes, ref codeManager); }, HasLetterBeenUsed('G', ref userCodes, codeManager.Code));
      UI.Button("", customFont, new Rectangle(colorButtonPosition.X, colorButtonPosition.Y + (colorButtonSize + colorButtonSpacing) * 2, colorButtonSize, colorButtonSize), Constants.Blue, Constants.MenuTextColor, () => { GetColorInput('B', ref userCodes, ref codeManager); }, HasLetterBeenUsed('B', ref userCodes, codeManager.Code));
      UI.Button("", customFont, new Rectangle(colorButtonPosition.X - colorButtonSize - colorButtonSpacing, colorButtonPosition.Y, colorButtonSize, colorButtonSize), Constants.Orange, Constants.MenuTextColor, () => { GetColorInput('O', ref userCodes, ref codeManager); }, HasLetterBeenUsed('O', ref userCodes, codeManager.Code));
      UI.Button("", customFont, new Rectangle(colorButtonPosition.X - colorButtonSize - colorButtonSpacing, colorButtonPosition.Y + colorButtonSize + colorButtonSpacing, colorButtonSize, colorButtonSize), Constants.Brown, Constants.MenuTextColor, () => { GetColorInput('C', ref userCodes, ref codeManager); }, HasLetterBeenUsed('C', ref userCodes, codeManager.Code));
      UI.Button("", customFont, new Rectangle(colorButtonPosition.X - colorButtonSize - colorButtonSpacing, colorButtonPosition.Y + (colorButtonSize + colorButtonSpacing) * 2, colorButtonSize, colorButtonSize), Constants.Gray, Constants.MenuTextColor, () => { GetColorInput('A', ref userCodes, ref codeManager); }, HasLetterBeenUsed('A', ref userCodes, codeManager.Code));
      UI.Button("Sub", customFont, new Rectangle(colorButtonPosition.X - colorButtonSize - colorButtonSpacing, colorButtonPosition.Y + (colorButtonSize + colorButtonSpacing) * 3, colorButtonSize, colorButtonSize), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { HandleSubmit(ref userCodes, ref codeManager); });
      UI.Button("<-", customFont, new Rectangle(colorButtonPosition.X, colorButtonPosition.Y + (colorButtonSize + colorButtonSpacing) * 3, colorButtonSize, colorButtonSize), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => userCodes[currentAttempt] = userCodes[currentAttempt].Substring(0, userCodes[currentAttempt].Length - 1), userCodes[currentAttempt].Length == 0);

      UI.TextLeft($"Attempts left: {maxAttempts - currentAttempt}", new Vector2(width / 2 + (boxSize.X * maxLength + spacing * (maxLength - 1)) / 2 + 50, contentPositionY), 25, 1, Constants.MenuTextColor, customFont);
    }

    static void SaveDailyDate()
    {
      string appDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "COC");
      string path = Path.Combine(appDirectory, Constants.DailyPath);
      string currentDate = DateTime.Now.ToString("yyyyMMdd");

      try
      {
        if (!Directory.Exists(appDirectory))
        {
          Directory.CreateDirectory(appDirectory);
        }

        File.WriteAllText(path, currentDate);
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error saving daily date: {ex.Message}");
      }

    }

    static bool HasPlayedToday()
    {
      string appDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "COC");
      string path = Path.Combine(appDirectory, Constants.DailyPath);
      string currentDate = DateTime.Now.ToString("yyyyMMdd");

      if (File.Exists(path))
      {
        string lastPlayed = File.ReadAllText(path);
        return lastPlayed == currentDate;
      }
      return false;
    }

    public void ModeSelectScreen(Action<string> modeSetter)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.TextCentered("CHOOSE GAME MODE", new Vector2(width / 2, 100), 50, 2, Constants.TitleColor, customFont);
      UI.Button("Back", customFont, new Rectangle(50, 80, 100, 40), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => screenSetter("menu"));

      int spacing = 10;
      Vector2 selectButtonSize = new Vector2(200, 50);
      Vector2 selectButtonPosition = new Vector2(width / 2 - (selectButtonSize.X) / 2, (height / 2) - selectButtonSize.Y * 2);

      UI.Button("Play Random", customFont, new Rectangle(selectButtonPosition.X, selectButtonPosition.Y, selectButtonSize.X, selectButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { screenSetter("game"); modeSetter("random"); gameInit(); });
      UI.Button("Play Daily", customFont, new Rectangle(selectButtonPosition.X, selectButtonPosition.Y + selectButtonSize.Y + spacing, selectButtonSize.X, selectButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { screenSetter("game"); modeSetter("daily"); gameInit(); SaveDailyDate(); }, HasPlayedToday());
    }

    public void RulesScreen()
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.TextCentered("RULES", new Vector2(width / 2, 50), 50, 2, Constants.TitleColor, customFont);

      UI.List(
          new ListText[] {
            new ListText("1. The program draws the codes of four colors chosen from among six:", Constants.MenuTextColor),
            new ListText("   R: Red", Constants.Red),
            new ListText("   G: Green", Constants.Green),
            new ListText("   B: Blue", Constants.Blue),
            new ListText("   O: Orange", Constants.Orange),
            new ListText("   C: Brown", Constants.Brown),
            new ListText("   A: Gray", Constants.Gray),
            new ListText("2. The player enters his code. The program informs the player:", Constants.MenuTextColor),
            new ListText("   Yellow border: color matches but position does not match.", Constants.Yellow),
            new ListText("   Green border: both color and position match.", Constants.Green),
            new ListText("3. If you get 4 green borders - you win.", Constants.MenuTextColor),
            new ListText("4. If you made a 6 attempt to guess the color code - you lose.", Constants.MenuTextColor)
          },
          new Vector2(50, 100),
          new Vector2(0, 24),
          24,
          1,
          customFont
      );

      Vector2 backButtonSize = new Vector2(200, 50);
      Vector2 backButtonPosition = new Vector2(width / 2 - (backButtonSize.X) / 2, height - backButtonSize.Y - 50);

      UI.Button("Back", customFont, new Rectangle(backButtonPosition.X, backButtonPosition.Y, backButtonSize.X, backButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => screenSetter("menu"));
    }

    public void VictoryScreen(CodeManager codeManager, string mode)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.TextCentered("You won!", new Vector2(width / 2, 100), 50, 2, Constants.TitleColor, customFont);
      UI.TextCentered($"The code was: {codeManager.Code}", new Vector2(width / 2, 150), 30, 1, Constants.MenuTextColor, customFont);

      int spacing = 20;
      Vector2 feedbackButtonSize = new Vector2(200, 50);
      Vector2 feedbackButtonPosition = new Vector2(width / 2 - (feedbackButtonSize.X) / 2, height - feedbackButtonSize.Y * 2 - 50 - spacing);

      UI.Button("Back to Menu", customFont, new Rectangle(feedbackButtonPosition.X, feedbackButtonPosition.Y, feedbackButtonSize.X, feedbackButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { screenSetter("menu"); resetSetter(true); SaveStat(true, mode, currentAttempt); });
      UI.Button("Play Again", customFont, new Rectangle(feedbackButtonPosition.X, feedbackButtonPosition.Y + feedbackButtonSize.Y + spacing, feedbackButtonSize.X, feedbackButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { screenSetter("game"); resetSetter(true); SaveStat(true, mode, currentAttempt); });
    }

    public void DefeatScreen(CodeManager codeManager, string mode)
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.TextCentered("You lost!", new Vector2(width / 2, 100), 50, 2, Constants.TitleColor, customFont);
      UI.TextCentered($"The code was: {codeManager.Code}", new Vector2(width / 2, 150), 30, 1, Constants.MenuTextColor, customFont);

      int spacing = 20;
      Vector2 feedbackButtonSize = new Vector2(200, 50);
      Vector2 feedbackButtonPosition = new Vector2(width / 2 - (feedbackButtonSize.X) / 2, height - feedbackButtonSize.Y * 2 - 50 - spacing);

      UI.Button("Back to Menu", customFont, new Rectangle(feedbackButtonPosition.X, feedbackButtonPosition.Y, feedbackButtonSize.X, feedbackButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { screenSetter("menu"); resetSetter(true); SaveStat(false, mode); });
      UI.Button("Play Again", customFont, new Rectangle(feedbackButtonPosition.X, feedbackButtonPosition.Y + feedbackButtonSize.Y + spacing, feedbackButtonSize.X, feedbackButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => { screenSetter("game"); resetSetter(true); SaveStat(false, mode); });
    }

    public void StatsScreen()
    {
      ClearBackground(Constants.BackgroundColor);

      int width = GetScreenWidth();
      int height = GetScreenHeight();

      UI.TextCentered("STATISTICS", new Vector2(width / 2, 50), 50, 2, Constants.TitleColor, customFont);
      UI.TextCentered("Random mode stats", new Vector2(width / 2 - width / 4, 100), 30, 2, Constants.TitleColor, customFont);
      UI.TextCentered("Daily mode stats", new Vector2(width / 2 + width / 4, 100), 30, 2, Constants.TitleColor, customFont);

      string randomStats = ReadStats("random");
      string dailyStats = ReadStats("daily");
      UI.TextLeft(randomStats, new Vector2(50, 125), 24, 1, Constants.MenuTextColor, customFont);
      UI.TextLeft(dailyStats, new Vector2((width / 2) + 50, 125), 24, 1, Constants.MenuTextColor, customFont);

      Vector2 backButtonSize = new Vector2(200, 50);
      Vector2 backButtonPosition = new Vector2(width / 2 - (backButtonSize.X) / 2, height - backButtonSize.Y - 50);

      UI.Button("Back", customFont, new Rectangle(backButtonPosition.X, backButtonPosition.Y, backButtonSize.X, backButtonSize.Y), Constants.MenuButtonColor, Constants.MenuButtonTextColor, () => screenSetter("menu"));
    }
  }
}
