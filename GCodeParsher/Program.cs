// See https://aka.ms/new-console-template for more information

using netDxf;
using netDxf.Entities;
using oGCodeParshernsoleApp1.Models;
using oGCodeParshernsoleApp1.Services;

Console.WriteLine("Hello, World!");


var parser = new XilogPargsherService(new FileProxy());
var filename = @"c:\Projects\test2.xxl";
var machining = parser.GetMachinings(filename);


var doc = new DxfDocument();
doc.AddEntity(new Line(Vector2.Zero, new Vector2(0,10)));
doc.AddEntity(new Line(Vector2.Zero, new Vector2(10,0)));

foreach (var xilogMachining in machining)
{
    var objects = parser.GetDxfObjects(xilogMachining);
    foreach (var dxfObject in objects)
    {
        doc.AddEntity(dxfObject);
    }
}
doc.Save(Path.ChangeExtension(filename, ".dxf"));