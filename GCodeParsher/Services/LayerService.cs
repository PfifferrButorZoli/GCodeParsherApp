using netDxf;
using netDxf.Tables;

namespace oGCodeParshernsoleApp1.Services;

public static class LayerService
{
    public static Layer GetLayer(int index)
    {
        var layer = new Layer("Machining_" + index)
        {
            Color = ColorService.GetColor(index),
            IsVisible = true,
            IsFrozen = false,
            IsLocked = false,
            Plot = true,
        };
        return layer;
    }
}