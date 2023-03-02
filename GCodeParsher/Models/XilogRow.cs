namespace oGCodeParshernsoleApp1.Models;

public class XilogRow
{
    public string? Command { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }
    public double I { get; set; }
    public double J { get; set; }
    public double H { get; set; }
    public double DX { get; set; }
    public double DY { get; set; }
    public double DZ { get; set; }
    
    public double Q { get; set; }
    
    public double R { get; set; }
    public double Radius { get; set; }
    public int G { get; set; }
    public bool IsComment { get; set; }
    public string? Comment { get; set; }
}