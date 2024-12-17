using System.Numerics;
using Raylib_cs;
using static Raylib_cs.Raylib;

namespace COC
{
  struct ListText
  {
    public ListText(string text, Color color)
    {
      Text = text;
      Color = color;
    }
    public string Text;
    public Color Color;
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

    static Color GetDisabledColor(Color bgColor)
    {
      return GetAdjustedColor(bgColor, -50);
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

    public static void RoundedRectangle(Rectangle rect, float roundness, int segments, Color color)
    {
      float radius = Math.Min(rect.Width, rect.Height) * roundness / 2;

      DrawRectangle((int)(rect.X + radius), (int)rect.Y, (int)(rect.Width - 2 * radius), (int)rect.Height, color); // Horizontal
      DrawRectangle((int)rect.X, (int)(rect.Y + radius), (int)rect.Width, (int)(rect.Height - 2 * radius), color); // Vertical

      DrawCircleSector(new Vector2(rect.X + radius, rect.Y + radius), radius, 180, 270, segments, color); // Top-left
      DrawCircleSector(new Vector2(rect.X + rect.Width - radius, rect.Y + radius), radius, 270, 360, segments, color); // Top-right
      DrawCircleSector(new Vector2(rect.X + radius, rect.Y + rect.Height - radius), radius, 90, 180, segments, color); // Bottom-left
      DrawCircleSector(new Vector2(rect.X + rect.Width - radius, rect.Y + rect.Height - radius), radius, 0, 90, segments, color); // Bottom-right
    }

    public static void TextCentered(string text, Vector2 position, float fontSize, float spacing, Color color, Font font)
    {
      Vector2 textSize = MeasureTextEx(font, text, fontSize, spacing);
      Vector2 centeredPosition = position - textSize / 2;

      DrawTextEx(font, text, centeredPosition, fontSize, spacing, color);
    }

    public static void TextLeft(string text, Vector2 position, float fontSize, float spacing, Color color, Font font)
    {
      DrawTextEx(font, text, position, fontSize, spacing, color);
    }

    public static void Button(string text, Font font, Rectangle bounds, Color bgColor, Color textColor, Action callback, bool isDisabled = false)
    {
      Vector2 mousePos = GetMousePosition();
      bool isHovered = CheckCollisionPointRec(mousePos, bounds) && !isDisabled;
      bool isActive = isHovered && IsMouseButtonDown(MouseButton.Left) && !isDisabled;
      Color hoverColor = GetHoverColor(bgColor);
      Color activeColor = GetActiveColor(bgColor);
      Color disabledColor = GetDisabledColor(bgColor);

      Color currentColor = bgColor;
      if (isHovered)
      {
        currentColor = hoverColor;
      }
      if (isActive)
      {
        currentColor = activeColor;
      }
      if (isDisabled)
      {
        currentColor = disabledColor;
      }

      if (isHovered || isActive)
      {
        SetMouseCursor(MouseCursor.PointingHand);
      }

      bool isClicked = isHovered && IsMouseButtonReleased(MouseButton.Left) && !isDisabled;
      if (isClicked)
      {
        callback();
      }

      RoundedRectangle(bounds, 0.2f, 10, currentColor);
      TextCentered(text, new Vector2(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2), 30, 2, textColor, font);
    }

    public static void List(ListText[] items, Vector2 pos, Vector2 spacing, float fontSize, float letterSpacing, Font font)
    {
      for (int i = 0; i < items.Length; i++)
      {
        TextLeft(items[i].Text, pos + spacing * i, fontSize, letterSpacing, items[i].Color, font);
      }
    }
  }
}
