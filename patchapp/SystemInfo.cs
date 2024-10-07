namespace patchApp
{
  public class SystemInfo
  {
    public static SystemProp GetSystemDataFromFile(string filePath)
    {
      var systemObject = new SystemProp();

      string[] lines = File.ReadAllLines(filePath);

      foreach (string line in lines)
      {
        if (line.StartsWith("systemname:"))
        {
          systemObject.SystemName = line.Split(':')[1].Trim();
        }
        else if (line.StartsWith("ip:"))
        {
          systemObject.IPAddress = line.Split(':')[1].Trim();
        }
        else if (line.StartsWith("os_version:"))
        {
          systemObject.OSVersion = line.Split(':')[1].Trim();
        }
      }
      return systemObject;
    }
  }

}