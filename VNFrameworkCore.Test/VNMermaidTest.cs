using VNFramework.Core;

namespace VNFrameworkUtilsTests;

public class VNMermaidTest
{
    [Theory]
    [InlineData("Mermaid节点[章节名称]", "Mermaid节点", "章节名称")]
    [InlineData("   Mermaid节点   [  章节名称  ]", "Mermaid节点", "章节名称")]
    public void ExtractDefineNode_ValidInput_Success(string input, string nodeName, string chapterName)
    {
        var result = Mermaid.ExtractDefineNode(input);

        Assert.Equal(nodeName, result.nodeName);
        Assert.Equal(chapterName, result.chapterName);
    }

    [Theory]
    [InlineData("Mermaid节点[]")]
    [InlineData("[章节名称]")]
    [InlineData("[]")]
    [InlineData("")]
    public void ExtractDefineText_InvalidInput_ThrowsArgumentException(string input)
    {
        Assert.Throws<ArgumentException>(() => Mermaid.ExtractDefineNode(input));
    }

    [Theory]
    [InlineData("Node A -->|Option| Node B", "Node A", "Node B", "Option")]
    [InlineData(" Node A --> | Option | Node B ", "Node A", "Node B", "Option")]
    [InlineData("Node A --> Node B", "Node A", "Node B", "")]
    public void ExtractLinkNode_ValidInput_Success(string linkLine, string fromNode, string toNode, string optionText)
    {
        var unit = Mermaid.ExtractLinkNode(linkLine);

        Assert.Equal(fromNode, unit.fromNode);
        Assert.Equal(toNode, unit.toNode);
        Assert.Equal(optionText, unit.optionText);
    }

    [Theory]
    [InlineData("Invalid Link Node Format")]
    [InlineData("Node A -- Node B")]
    [InlineData("Node A --> ")]
    [InlineData("Node A -->|Option|")]
    [InlineData(" --> Node B")]
    [InlineData(" -->|Option| Node B")]
    public void ExtractLinkNode_InvalidInput_ThrowsArgumentException(string input)
    {
        Assert.Throws<ArgumentException>(() => Mermaid.ExtractLinkNode(input));
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
    public void ExtraMermaidText_WithValidMermaidText_ParsesSuccessfully()
    {
        // Arrange
        string mermaidText = """
            # 这里是定义
            Node1[Chapter1]
            Node2[Chapter2]
            Node3[Chapter3]

            # 这里是链接
            Node1 -->|OptionText1to2| Node2
            Node1 -->|OptionText1to3| Node3
            Node3 -->|OptionText3to2| Node2
            Node3 --> Node4

            # 这里是第二部分定义
            Node4[Chapter4]
            Node5[Chapter5]
            Node6[Chapter6]
            """;

        var (defineLines, linkLines) = Mermaid.ExtractMermaidText(mermaidText.Split(Environment.NewLine));

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
            "Node1 -->|OptionText1to2| Node2",
            "Node1 -->|OptionText1to3| Node3",
            "Node3 -->|OptionText3to2| Node2",
            "Node3 --> Node4"
        };

        Assert.Equal(expectedDefineLines, defineLines);
        Assert.Equal(expectedLinkLines, linkLines);
    }

    [Fact]
    public void ParseVNMermaid_WithValidMermaidText_ParsesSuccessfully()
    {
        // Arrange
        var mermaid = new Mermaid();
        string mermaidText =
            """
            Node1[Chapter1]
            Node2[Chapter2]
            Node3[Chapter3]

            Node1 -->|OptionText1to2| Node2
            Node1 -->|OptionText1to3| Node3
            Node3 -->|OptionText3to2| Node2
            """;

        // Act
        mermaid.ParseVNMermaid(mermaidText.Split(Environment.NewLine));
        var mermaidMap = mermaid.GetMermaidMap();

        // Assert
        Assert.Contains("Node1 -> Node2", mermaidMap);
        Assert.Contains("Node1 -> Node3 -> Node2", mermaidMap);
    }
}