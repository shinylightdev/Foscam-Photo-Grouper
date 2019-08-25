using System;
using System.IO;
using System.Linq;

namespace FoscamImageGroup
{
  class FoscamImage
  {
    public string ImagePath { get; set; }
    public DateTime LastModified { get; set; }
  }

  class Program
  {
    /// <summary>
    /// Let's add a 0 before the digit in case single digit month/day for nice format. 
    /// </summary>
    /// <param name="digit"></param>
    /// <returns></returns>
    static private string AddLeadingZero(string digit)
    {
      return digit.Length == 1 ? "0" + digit : digit;
    }

    static void Main(string[] args)
    {
      string imagePath = @"D:\Documents\Inbox\VideoMonitor";

      var imageList = Directory.GetFiles(imagePath)
          .Select<string, FoscamImage>(path => new FoscamImage { ImagePath = path, LastModified = File.GetLastWriteTime(path) })
          .OrderBy(x => x.LastModified);


      // Make a list of potential newly created directories (to-be created).
      var newDirectories = imageList
          .Select(image => image.LastModified.Year.ToString() + "-" + AddLeadingZero(image.LastModified.Month.ToString()) + "-" + AddLeadingZero(image.LastModified.Day.ToString()))
          .Distinct();

      // Let's create the directories if they don't exist.
      foreach (var directoryPath in newDirectories)
      {
        // Does directory exist? If not, create it.
        if (!Directory.Exists(imagePath + @"\" + directoryPath))
        {
          // Create new directory.
          try
          {
            Directory.CreateDirectory(imagePath + @"\" + directoryPath);
          }
          catch (Exception e)
          {
            Console.WriteLine(e.Message);
          }
        }
      }

      Int64 filesMoved = 0;

      // Now let's loop through each file to see where it has to be moved to.
      foreach (FoscamImage image in imageList)
      {
        string directoryName = image.LastModified.Year.ToString() + "-" + AddLeadingZero(image.LastModified.Month.ToString()) + "-" + AddLeadingZero(image.LastModified.Day.ToString());
        string fromLocation  = image.ImagePath;
        string fileName      = image.ImagePath.Split('\\').Last();
        string toLocation    = imagePath + @"\" + directoryName + @"\" + fileName;

        try
        {
          File.Move(fromLocation, toLocation);
          filesMoved++;
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
        }
      }

      Console.WriteLine(filesMoved + " files were moved.");

      Console.ReadLine();

    }
  }
}
