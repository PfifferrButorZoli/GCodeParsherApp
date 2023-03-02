using oGCodeParshernsoleApp1.Models;

namespace oGCodeParshernsoleApp1.Services;

public interface IXilogPargsherService
{
    List<XilogMachining> GetMachinings(string filePath);
}