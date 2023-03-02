using System.Collections.Generic;
using Flee.PublicTypes;
using NSubstitute;
using NUnit.Framework;
using oGCodeParshernsoleApp1.Models;
using oGCodeParshernsoleApp1.Services;

namespace TestProjecTEstt1;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void GetMachiningTest()
    {
        var file = Substitute.For<IFile>();
        file.ReadLines(Arg.Any<string>())
            .Returns(new List<string>()
            {
                "hur",
                "ka",
                "XG0",
                "teso",
                "XG1",
                "XG0",
                "XG1",
                "XG0",
                "XG1",
                "XG11",
                "XG0",
            });

        var sut = new XilogPargsherService(file);
        
        var machining = sut.GetMachinings("hurka.txt");
        
        
        
        
        Assert.Pass();
    }

    [Test]
    public void XilogRowParsher()
    {
        var file = Substitute.For<IFile>();
        var sut = new XilogPargsherService(file);
        var result = sut.ParsheXilogRow("XG0 X=1003.59 Y=99.375 Z=-18 E=0 V=2000 D=20 T=1074 C=2 P=0");
        
    }
    
    
    [Test]
    public void XilogRowParsher2()
    {
        var file = "631,53+A";
        var ctx = new ExpressionContext();
        ctx.Variables.Add("A", 110);
        var result = ctx.CompileGeneric<double>(file);
        var q = result.Evaluate();
    }
    

}