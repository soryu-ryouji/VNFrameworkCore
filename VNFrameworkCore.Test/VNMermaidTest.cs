using VNFramework.Core;

namespace VNFrameworkUtilsTests;

public class VNMermaidTest
{
    [Theory]
    [InlineData("Mermaid节点[章节名称]", "Mermaid节点", "章节名称")]
    [InlineData("   Mermaid节点   [  章节名称  ]", "Mermaid节点", "章节名称")]
    public void ExtractDefineText_ValidInput_Success(string input, string nodeName, string chapterName)
    {
        var result = Mermaid.ExtractDefineText(input);

        Assert.Equal(nodeName, result.nodeName);
        Assert.Equal(chapterName, result.chapterName);
    }

    [Theory]
    [InlineData("Mermaid节点[]")]
    [InlineData("[章节名称]")]
    [InlineData("")]
    public void ExtractDefineText_InvalidInput_ThrowsArgumentException(string input)
    {
        Assert.Throws<ArgumentException>(() => Mermaid.ExtractDefineText(input));
    }

    [Theory]
    [InlineData("节点1 --> to节点1(选项文本1)", "节点1", "to节点1", "选项文本1")]
    [InlineData("节点2 --> to节点2(选项文本2)", "节点2", "to节点2", "选项文本2")]
    [InlineData("节点3 --> to节点3(选项文本3)", "节点3", "to节点3", "选项文本3")]
    [InlineData("from节点 --> to节点(选项文本)", "from节点", "to节点", "选项文本")]
    [InlineData("SingleLineNode --> SingleLineToNode(SingleLineOptionText)", "SingleLineNode", "SingleLineToNode", "SingleLineOptionText")]
    [InlineData("  LeadingSpaces  -->  TrailingSpaces  (  SpacesInOptionText  )  ", "LeadingSpaces", "TrailingSpaces", "SpacesInOptionText")]
    public void ExtractLinkText_ValidInput_Success(string input, string fromNode, string toNode, string optionText)
    {
        var result = Mermaid.ExtractLinkText(input);

        Assert.Equal(fromNode, result.fromNode);
        Assert.Equal(toNode, result.toNode);
        Assert.Equal(optionText, result.optionText);
    }

    [Theory]
    [InlineData("节点1 --> (选项文本)")]
    [InlineData("--> 节点2(选项文本)")]
    [InlineData("InvalidFormat")]
    public void ExtractLinkText_InvalidInput_ThrowsArgumentException(string input)
    {
        Assert.Throws<ArgumentException>(() => Mermaid.ExtractLinkText(input));
    }

    [Theory]
    [InlineData("Node1", 0)]
    [InlineData("Node2", 1)]
    [InlineData("Node3", 2)]
    public void GetGhostNodeIndex_ExistingNode_ReturnsCorrectIndex(string nodeName, int expectedIndex)
    {
        var mermaid = new Mermaid();
        mermaid.AddGhostNode("Node1", "chapter1");
        mermaid.AddGhostNode("Node2", "chapter2");
        mermaid.AddGhostNode("Node3", "chapter3");
        mermaid.AddGhostNode("Mode4", "chapter4");

        int result = mermaid.GetGhostNodeIndex(nodeName);
        Assert.Equal(expectedIndex, result);
    }

    [Fact]
    public void GetGhostNodeIndex_NonExistingNode_ReturnsMinusOne()
    {
        var mermaid = new Mermaid();
        mermaid.AddGhostNode("Node1", "chapter1");
        mermaid.AddGhostNode("Node2", "chapter2");
        mermaid.AddGhostNode("Node3", "chapter3");
        mermaid.AddGhostNode("Mode4", "chapter4");

        int result = mermaid.GetGhostNodeIndex("NonExistingNode");
        Assert.Equal(-1, result);
    }

    [Fact]
    public void GetMermaidNode_MultipleNodes_ReturnsCorrectNode()
    {
        // Arrange
        var node1 = new MermaidNode("Node1", "Chapter1");
        var node2 = new MermaidNode("Node2", "Chapter2");
        var node3 = new MermaidNode("Node3", "Chapter3");
        var node4 = new MermaidNode("Node4", "Chapter4");
        node1.AddLinkedNode(node2, "OptionText1to2");
        node1.AddLinkedNode(node3, "OptionText1to3");
        node3.AddLinkedNode(node4, "OptionText3to4");
        node2.AddLinkedNode(node3, "OptionText2to3");

        var parentNodeList = new List<MermaidNode> { node1, node2, node3, node4 };

        // Act
        // Test List
        var resultNode1 = MermaidNode.GetMermaidNode(parentNodeList, "Node1");
        var resultNode2 = MermaidNode.GetMermaidNode(parentNodeList, "Node2");
        var resultNode3 = MermaidNode.GetMermaidNode(parentNodeList, "Node3");
        var resultNode4 = MermaidNode.GetMermaidNode(parentNodeList, "Node4");
        // Test Mermaid Node
        var resultNode5 = node1.GetMermaidNode("Node1");
        var resultNode6 = node1.GetMermaidNode("Node2");
        var resultNode7 = node1.GetMermaidNode("Node3");
        var resultNode8 = node1.GetMermaidNode("Node4");

        // Assert
        Assert.Equal(node1, resultNode1);
        Assert.Equal(node2, resultNode2);
        Assert.Equal(node3, resultNode3);
        Assert.Equal(node4, resultNode4);

        Assert.Equal(node1, resultNode5);
        Assert.Equal(node2, resultNode6);
        Assert.Equal(node3, resultNode7);
        Assert.Equal(node4, resultNode8);
    }


    [Fact]
    public void GetNodePaths_MultipleNodes_ReturnsCorrectPaths()
    {
        // Arrange
        var node1 = new MermaidNode("Node1", "Chapter1");
        var node2 = new MermaidNode("Node2", "Chapter2");
        var node3 = new MermaidNode("Node3", "Chapter3");
        var node4 = new MermaidNode("Node4", "Chapter4");
        node1.AddLinkedNode(node2, "OptionText1to2");
        node1.AddLinkedNode(node3, "OptionText1to3");
        node3.AddLinkedNode(node4, "OptionText3to4");
        node2.AddLinkedNode(node3, "OptionText2to3");

        // Act
        string[] paths = node1.GetNodePaths();

        // Assert
        Assert.Equal(2, paths.Length);
        Assert.Contains("Node1 -> Node2 -> Node3 -> Node4", paths);
        Assert.Contains("Node1 -> Node3 -> Node4", paths);
    }

    [Fact]
    public void GetMermaidMap_MultipleNodes_ReturnsCorrectMap()
    {
        var mermaid = new Mermaid();

        // Add Ghost Node
        mermaid.AddGhostNode("Node1", "Chapter1");
        mermaid.AddGhostNode("Node2", "Chapter2");
        mermaid.AddGhostNode("Node3", "Chapter3");
        mermaid.AddGhostNode("Node4", "Chapter4");

        // Link Mermaid Node
        mermaid.LinkMermaidNode("Node1", "Node2", "OptionText1to2");
        mermaid.LinkMermaidNode("Node3", "Node4", "OptionText3to4");
        mermaid.LinkMermaidNode("Node2", "Node3", "OptionText2to3");
        mermaid.LinkMermaidNode("Node1", "Node3", "OptionText1to3");
        mermaid.LinkMermaidNode("Node1", "Node4", "OptionText1to4");

        // Get Mermaid Paths
        var paths = mermaid.GetMermaidMap();

        // Assert
        Assert.Contains("Node1 -> Node2 -> Node3 -> Node4", paths);
        Assert.Contains("Node1 -> Node3 -> Node4", paths);
        Assert.Contains("Node1 -> Node4", paths);
    }

    [Fact]
    public void ExtraMermaidTagText_WithValidMermaidText_ParsesSuccessfully()
    {
        // Arrange
        string mermaidText = @"
# 这里是定义
[Define]
Node1[Chapter1]
Node2[Chapter2]
Node3[Chapter3]

# 这里是链接
[Link]
Node1 --> Node2 (OptionText1to2)
Node1 --> Node3 (OptionText1to3)
Node3 --> Node2 (OptionText3to2)

# 这里是第二部分定义
[Define]
Node4[Chapter4]
Node5[Chapter5]
Node6[Chapter6]

";
        // Act
        var defineLines = Mermaid.ExtractMermaidTagText(mermaidText, MermaidTag.Define);
        var linkLines = Mermaid.ExtractMermaidTagText(mermaidText, MermaidTag.Link);

        // Assert
        var expectedDefineLines = new List<string>
        {
            "Node1[Chapter1]",
            "Node2[Chapter2]",
            "Node3[Chapter3]",

            "Node4[Chapter4]",
            "Node5[Chapter5]",
            "Node6[Chapter6]"
        };

        var expectedLinkLines = new List<string>
        {
            "Node1 --> Node2 (OptionText1to2)",
            "Node1 --> Node3 (OptionText1to3)",
            "Node3 --> Node2 (OptionText3to2)"
        };

        Assert.Equal(expectedDefineLines, defineLines);
        Assert.Equal(expectedLinkLines, linkLines);
    }

    [Fact]
    public void ParseVNMermaid_WithValidMermaidText_ParsesSuccessfully()
    {
        // Arrange
        var mermaid = new Mermaid();
        string mermaidText = @"
[Define]
Node1[Chapter1]
Node2[Chapter2]
Node3[Chapter3]

[Link]
Node1 --> Node2 (OptionText1to2)
Node1 --> Node3 (OptionText1to3)
Node3 --> Node2 (OptionText3to2)";

        // Act
        mermaid.ParseVNMermaid(mermaidText);
        var mermaidMap = mermaid.GetMermaidMap();

        // Assert
        Assert.Contains("Node1 -> Node2", mermaidMap);
        Assert.Contains("Node1 -> Node3 -> Node2", mermaidMap);
    }
}