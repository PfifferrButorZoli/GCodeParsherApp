using Flee.PublicTypes;

namespace oGCodeParshernsoleApp1.Models;

public class XilogMachining
{
    public ExpressionContext Context { get; set; }
    
    public XilogMachining(int sequence, List<string> xilogLines, ExpressionContext context)
    {
        Sequence = sequence;
        XilogLines = xilogLines;
        Context = context;
    }

    public int Sequence { get;  }
    public List<string> XilogLines { get;  }
}