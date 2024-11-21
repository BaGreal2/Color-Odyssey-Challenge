using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace COC
{
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
}
