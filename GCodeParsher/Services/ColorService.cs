using System.Drawing;
using netDxf;

namespace oGCodeParshernsoleApp1.Services;

public static class ColorService
{
    public static AciColor GetColor(int color)
    {
        
        var colorList = new List<AciColor>()
        {
            new AciColor(Color.Red),
            new AciColor(Color.Green),
            new AciColor(Color.Blue),
            new AciColor(Color.Yellow),
            new AciColor(Color.Purple),
            new AciColor(Color.Orange),
            new AciColor(Color.Brown),
            new AciColor(Color.Gray),
            new AciColor(Color.Pink),
            new AciColor(Color.LightBlue),
            new AciColor(Color.LightGreen),
            new AciColor(Color.LightGray),
            new AciColor(Color.LightPink),
        };
        var colorIndex = color % colorList.Count;
        return colorList[colorIndex];
    }
}