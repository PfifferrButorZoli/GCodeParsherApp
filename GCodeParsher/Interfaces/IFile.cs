namespace oGCodeParshernsoleApp1.Models;

public interface IFile
{
    void Copy(string sourceFileName, string destFileName);
    void Delete(string file);
    bool Exists(string? path);
    IEnumerable<string> ReadLines(string path);
    void WriteAllLines(string path, string[] contents);
    void WriteAllLines(string path, IEnumerable<string> contents);
    void WriteAllText(string path, string? contents);
}