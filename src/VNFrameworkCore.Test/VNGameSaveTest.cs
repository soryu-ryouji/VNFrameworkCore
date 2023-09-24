using Xunit.Abstractions;

namespace VNFrameworkCore.Test;

public class VNGameSaveTest
{
    private readonly ITestOutputHelper _output;
    public VNGameSaveTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void GetPerformanceStateByIndexTest()
    {
        var lines = File.ReadAllText("../../../TestFiles/VNGameSaveTest.txt");
        _output.WriteLine(lines);

        var gameSaves = VNGameSave.ParseGameSaveText(lines);

        foreach (var save in gameSaves)
        {
            _output.WriteLine(save.ToString());
        }

        // Assert.True(false);
    }
}