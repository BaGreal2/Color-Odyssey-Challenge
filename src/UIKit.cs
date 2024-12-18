using System.Numerics;
using Raylib_cs;

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

  class UIKit
  {

    public static void List(ListText[] items, Vector2 pos, Vector2 spacing, float fontSize, float letterSpacing, Font font)
    {
      for (int i = 0; i < items.Length; i++)
      {
        UI.TextLeft(items[i].Text, pos + spacing * i, fontSize, letterSpacing, items[i].Color, font);
      }
    }
  }
}
