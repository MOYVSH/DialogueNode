using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualGraphRuntime;

// Override the default node name (otherwise the name of the class is used)
[NodeName(_name: "OptionNode")]
// Override the default settings for how many ports the node will handle
[NodePortAggregateAttribute(NodePortAggregateAttribute.PortAggregate.Single, NodePortAggregateAttribute.PortAggregate.Single)]
// Override the default settings for the Port Capacity
[PortCapacity(PortCapacityAttribute.Capacity.Single, PortCapacityAttribute.Capacity.Single)]
// Custom style sheet for your node
[CustomNodeStyle("ExampleNodeStyle")]

public class OptionNode : BaseNode
{
    [TextArea(1, 5)]
    public string Text;

    public override void Init()
    {
        base.Init();
        NodeEnum = DialogNodeEnum.OptionNode;
    }
}
