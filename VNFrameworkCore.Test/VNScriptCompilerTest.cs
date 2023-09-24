using System.Reflection;
using VNFramework.Core;
using Xunit;
using Xunit.Abstractions; 

namespace VNFrameworkUtilsTests;

public class VNScriptCompilerTest
{
    private readonly ITestOutputHelper _output;
    public VNScriptCompilerTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void GetPerformanceStateByIndexTest()
    {
        var lines = File.ReadAllLines("../../../TestFiles/VNScriptCompilerTest.txt");

        var performanceState = VNScriptCompiler.GetPerformanceStateByIndex(lines, 20);
        _output.WriteLine(performanceState.ToString());

        Assert.True(performanceState != null);
    }
}