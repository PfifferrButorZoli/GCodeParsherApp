using System.Dynamic;
using System.Text.RegularExpressions;
using Flee.PublicTypes;
using netDxf;
using netDxf.Entities;
using oGCodeParshernsoleApp1.Models;

namespace oGCodeParshernsoleApp1.Services;

public class XilogPargsherService : IXilogPargsherService
{
    private readonly IFile _file;


    public XilogPargsherService(IFile file)
    {
        _file = file;

    }




    

    public List<XilogMachining> GetMachinings(string filePath)
    {
        var machinings = new List<XilogMachining>();
        var lines = _file.ReadLines(filePath).ToList();

        
        var headerRow = ParsheXilogRow(lines[0]);
        

        

        var machiningStartLines = lines 
            .Select((m, i) => new {Text = m, Index = i})
            .Where(line => line.Text.Contains("XG0") || line.Text.Contains("G03D"))
            .Select(m => m.Index)
            .ToList();

        var counter = 0;
        for (var i = 0; i < machiningStartLines.Count; i++)
        {
            var context = new ExpressionContext();
            context.Variables.Add("DX", headerRow.DX);
            context.Variables.Add("DY", headerRow.DY);
            context.Variables.Add("DZ", headerRow.DZ);

            var end = i == machiningStartLines.Count - 1;
            var startLine = machiningStartLines[i];
            var endLine = end ? lines.Count : machiningStartLines[i + 1];


            for (int j = 0; j < endLine; j++)
            {
                if (lines[j].StartsWith("PAR ") || lines[j].StartsWith("L "))
                {
                    var keplet = lines[j];
                    keplet = keplet.Replace("PAR ", "");
                    keplet = keplet.Replace("L ", "");
                    keplet = keplet.Contains(";") ? keplet.Substring(0, keplet.IndexOf(";")) : keplet;
                    keplet = keplet.Contains("\"") ? keplet.Substring(0, keplet.IndexOf("\"")) : keplet;
                    
                    
                    var par = keplet.Split('=');
                    var raw = par[1].Replace(" ", "");
                    if(string.IsNullOrEmpty(raw)) continue;
                    raw = raw.Replace(".", ",");
                    var ertek = context.CompileGeneric<double>(raw);
                    
                    context.Variables.Add(par[0].Replace(" ", ""), ertek);
                }
            }
            
            var list = lines.Skip(startLine).Take(endLine-startLine).ToList();
            machinings.Add(new XilogMachining(counter++, list, context));

        }
        return machinings;
    }



    public List<EntityObject> GetDxfObjects(XilogMachining machining)
    {
        var objects = new List<EntityObject>();
        for (var i = 0; i < machining.XilogLines.Count - 1; i++)
        {
            var xilogRow = ParsheXilogRow(machining.XilogLines[i], machining.Context);
            var nextXilogRow = ParsheXilogRow(machining.XilogLines[i + 1], machining.Context);
            var startPoint = new Vector3(xilogRow.X, xilogRow.Y, xilogRow.Z);
            var endPoint = new Vector3(nextXilogRow.X, nextXilogRow.Y, nextXilogRow.Z);
        
        
            switch (nextXilogRow.Command)
            {
                case "XL2P":
                    objects.Add(
                        new Line()
                        {
                            Color = AciColor.ByLayer,
                            Layer = LayerService.GetLayer(machining.Sequence),
                            StartPoint = startPoint,
                            EndPoint = endPoint
                        });
                    break;
                
                case "G13D":
                                        
                    startPoint.Z = xilogRow.H;
                    endPoint.Z = nextXilogRow.H;
             

      
                    var rotatedVectorPoit = EulerRotation(startPoint, xilogRow.R, 0, xilogRow.Q);
                        objects.Add(new Line()
                    {
                        Color = AciColor.ByLayer,
                        Layer = LayerService.GetLayer(machining.Sequence),
                        StartPoint = startPoint,
                        EndPoint = rotatedVectorPoit
                    });
                    

                    objects.Add(
                        new Line()
                        {
                            Color = AciColor.ByLayer,
                            Layer = LayerService.GetLayer(machining.Sequence),
                            StartPoint = startPoint,
                            EndPoint = endPoint
                        });
                    break;
                
                
                case "XAR2":
                    var center = GcodeMathHelper.ArcCenter(startPoint, endPoint, nextXilogRow.Radius, nextXilogRow.G, out string s);

                    double startAngle;
                    double endAngle;
                    
                    if (nextXilogRow.G == 2)
                    {
                        endAngle = GcodeMathHelper.CalculateAngle(new Vector2(center.X, center.Y),new Vector2(startPoint.X, startPoint.Y));
                        startAngle = GcodeMathHelper.CalculateAngle(new Vector2(center.X, center.Y),new Vector2(endPoint.X, endPoint.Y));
                    }
                    else
                    {
                        startAngle = GcodeMathHelper.CalculateAngle(new Vector2(center.X, center.Y),new Vector2(startPoint.X, startPoint.Y));
                        endAngle = GcodeMathHelper.CalculateAngle(new Vector2(center.X, center.Y),new Vector2(endPoint.X, endPoint.Y));
                    }
                    

                    objects.Add(new Arc
                    {
                        Color = AciColor.ByLayer,
                        Layer = LayerService.GetLayer(machining.Sequence),
                        Center = new Vector3(center.X, center.Y, nextXilogRow.Z),
                        Radius = nextXilogRow.Radius,
                        StartAngle = (180 / Math.PI) * startAngle,
                        EndAngle = (180 / Math.PI) * endAngle,
                    });
                    
                    break;           
            }
        }
        return objects;
    }

    //euler rotate point around center with z y x sequence 
    private Vector3 EulerRotation(Vector3 centerPoint, double a, double b, double c)
    {
        var rotationx = System.Numerics.Matrix4x4.CreateRotationX(ConvertToRadians((float) a));
        var rotationz = System.Numerics.Matrix4x4.CreateRotationZ(ConvertToRadians((float) c));
        
        var myVector = new System.Numerics.Vector3(0, 0, 10);
        myVector =  System.Numerics.Vector3.Transform(myVector, rotationx);
        myVector =  System.Numerics.Vector3.Transform(myVector, rotationz);
        return new Vector3(centerPoint.X + myVector.X, centerPoint.Y + myVector.Y, centerPoint.Z + myVector.Z);
    }

    
    private float ConvertToRadians(float angle)
    {
        return (float)(Math.PI / 180) * angle;
    }
    

    public XilogRow ParsheXilogRow(string rawLine, ExpressionContext context = null)
    {
        
        var xilogRow = new XilogRow();
        if (string.IsNullOrEmpty(rawLine)) return xilogRow;
        
        if (rawLine.StartsWith(";"))
        {
            xilogRow.IsComment = true;
            xilogRow.Comment = rawLine[1..];
            return xilogRow;
        }
        
        xilogRow.Command = rawLine.Split(" ")[0];
        
        var fields = new[]
        {
            new {Address = "X", Field = nameof(xilogRow.X)},
            new {Address = "Y", Field = nameof(xilogRow.Y)},
            new {Address = "Z", Field = nameof(xilogRow.Z)},
            new {Address = "r", Field = nameof(xilogRow.Radius)},
            new {Address = "G", Field = nameof(xilogRow.G)},
            new {Address = "I", Field = nameof(xilogRow.I)},
            new {Address = "J", Field = nameof(xilogRow.J)},
            new {Address = "H", Field = nameof(xilogRow.H)},
            new {Address = "Q", Field = nameof(xilogRow.Q)},
            new {Address = "R", Field = nameof(xilogRow.R)},
            new {Address = "DX", Field = nameof(xilogRow.DX)},
            new {Address = "DY", Field = nameof(xilogRow.DY)},
            new {Address = "DZ", Field = nameof(xilogRow.DZ)},
        };

        foreach (var field in fields)
        {

            var regex = new Regex(@$" {field.Address}=(\S*)");
            var match = regex.Match(rawLine);            
            if(!match.Success) continue;
            var value = match.Groups[1].Value;
            if (string.IsNullOrEmpty(value)) continue;
            
            value = value.Replace(".",",").Replace("#",string.Empty);

            object oValue = value;
            
            var property = xilogRow.GetType().GetProperty(field.Field);
            if(property == null) continue;
            if (context != null && property.PropertyType == typeof(double))
            {
                var comp = context.CompileGeneric<double>(value);
                oValue = comp.Evaluate();
            }
            property.SetValue(xilogRow, Convert.ChangeType(oValue, property.PropertyType));
        }

        return xilogRow;
    }
    
    
    
}