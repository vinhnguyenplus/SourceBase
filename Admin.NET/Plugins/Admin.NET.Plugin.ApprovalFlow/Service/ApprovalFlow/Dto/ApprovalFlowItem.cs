// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using System.Text.Json.Serialization;

namespace Admin.NET.Plugin.ApprovalFlow.Service;

public class ApprovalFlowItem
{
    [JsonPropertyName("nodes")]
    public List<ApprovalFlowNodeItem> Nodes { get; set; }

    [JsonPropertyName("edges")]
    public List<ApprovalFlowEdgeItem> Edges { get; set; }
}

public class ApprovalFlowNodeItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }

    [JsonPropertyName("properties")]
    public FlowProperties Properties { get; set; }

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FlowTextItem Text { get; set; }
}

public class ApprovalFlowEdgeItem
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("sourceNodeId")]
    public string SourceNodeId { get; set; }

    [JsonPropertyName("targetNodeId")]
    public string TargetNodeId { get; set; }

    [JsonPropertyName("startPoint")]
    public FlowEdgePointItem StartPoint { get; set; }

    [JsonPropertyName("endPoint")]
    public FlowEdgePointItem EndPoint { get; set; }

    [JsonPropertyName("properties")]
    public FlowProperties Properties { get; set; }

    [JsonPropertyName("text")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public FlowTextItem Text { get; set; }

    [JsonPropertyName("pointsList")]
    public List<FlowEdgePointItem> PointsList { get; set; }
}

public class FlowProperties
{
}

public class FlowTextItem
{
    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }

    [JsonPropertyName("value")]
    public string Value { get; set; }
}

public class FlowEdgePointItem
{
    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }
}