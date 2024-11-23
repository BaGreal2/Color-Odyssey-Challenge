namespace COC
{
  public class CodeManager
  {
    char[] _colors = { 'R', 'G', 'B', 'O', 'C', 'A' };
    int _maxLength = 4;
    public string Code = "";

    public CodeManager(int length)
    {
      _maxLength = length;
    }

    public void GenerateCode()
    {
      Random random = new Random();
      string result = "";

      for (int i = 0; i < _maxLength; i++)
      {
        int index = random.Next(_colors.Length);
        result += _colors[index];
      }

      Code = result;
    }

    public string GetFeedback(string userCode)
    {
      if (userCode.Length != Code.Length)
      {
        return "";
      }

      string feedback = "";
      for (int i = 0; i < Code.Length; i++)
      {
        if (Code[i] == userCode[i])
        {
          feedback += "X";
        }
        else if (Code.Contains(userCode[i]))
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
  }
}
