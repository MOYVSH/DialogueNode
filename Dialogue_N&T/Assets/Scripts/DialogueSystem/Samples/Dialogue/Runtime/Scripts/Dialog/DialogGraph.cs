using System.Collections;
using System.Linq;
using UnityEngine;
using VisualGraphRuntime;

// Ability to create the asset
[CreateAssetMenu]
// Default Node type for this graph
[DefaultNodeType(typeof(BaseNode))]
public class DialogGraph : VisualGraph
{
    [HideInInspector]
    public DialogNode currentState;

    public override void Init()
    {
        currentState = (DialogNode)StartingNode.Outputs.First().Connections[0].Node;
    }
}