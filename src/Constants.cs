using Raylib_cs;

namespace COC
{
  class Constants
  {
    public const int WindowWidth = 800;
    public const int WindowHeight = 600;
    public const string GameTitle = "COC";

    public const string StatsPath = "stats.txt";
    public const string DailyStatsPath = "daily-stats.txt";
    public const string DailyPath = "daily.txt";

    public const int MaxAttempts = 6;
    public const int MaxLength = 4;

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
    public static readonly Color BoxColor = new Color(0xF8, 0xBD, 0x7F, 255);

    public static readonly Color Red = new Color(0xD9, 0x5A, 0x40, 255);
    public static readonly Color Green = new Color(0x76, 0x9F, 0x72, 255);
    public static readonly Color Blue = new Color(0x69, 0xA5, 0xB6, 255);
    public static readonly Color Orange = new Color(0xF4, 0x87, 0x4C, 255);
    public static readonly Color Brown = new Color(0x8C, 0x6A, 0x48, 255);
    public static readonly Color Gray = new Color(0x94, 0xA1, 0xA4, 255);
    public static readonly Color Yellow = new Color(0xF6, 0xD9, 0x6F, 255);
  }
}
