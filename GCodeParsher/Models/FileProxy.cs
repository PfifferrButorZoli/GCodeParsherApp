using System.Text;

namespace oGCodeParshernsoleApp1.Models;



public class FileProxy : IFile
{
    public FileProxy()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
    
    public void Copy(string sourceFileName, string destFileName)
    {
        System.IO.File.Copy(sourceFileName, destFileName);
    }

    public void Delete(string file)
    {
        System.IO.File.Delete(file);
    }

    public bool Exists(string? path)
    {
        return System.IO.File.Exists(path);
    }

    public IEnumerable<string> ReadLines(string path)
    {
        return System.IO.File.ReadLines(path, Encoding.GetEncoding("iso-8859-2"));
    }

    public void WriteAllLines(string path, string[] contents)
    {
        System.IO.File.WriteAllLines(path, contents, Encoding.GetEncoding("iso-8859-2"));
    }

    public void WriteAllLines(string path, IEnumerable<string> contents)
    {
        System.IO.File.WriteAllLines(path, contents, Encoding.GetEncoding("iso-8859-2"));
    }

    public void WriteAllText(string path, string? contents)
    {
        System.IO.File.WriteAllText(path, contents, Encoding.GetEncoding("iso-8859-2"));
    }
    
}