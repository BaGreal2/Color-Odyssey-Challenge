using System.Security.Cryptography;
using System.Text;

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

    static int GenerateStableSeed(string input)
    {
      using (SHA256 sha256 = SHA256.Create())
      {
        byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

        return BitConverter.ToInt32(hashBytes, 0);
      }
    }

    public void GenerateCode(string mode)
    {
      Random random;
      if (mode == "random")
      {
        random = new Random();
      }
      else
      {
        string currentDate = DateTime.Now.ToString("yyyyMMdd");
        int seed = GenerateStableSeed(currentDate);
        random = new Random(seed);
      }

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
