using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VisualGraphEditor;

[CustomNodeView((typeof(DialogNode)))]
public sealed class DialogNodeView : VisualGraphNodeView
{
    // For this example the ability to hide/show the properties for the node are
    // hidden. Comment this out or set it to true to see the properties
    [HideInInspector] public override bool ShowNodeProperties => true;

    public override void DrawNode()
    {
        base.DrawNode();

        //VisualElement node_data = new VisualElement();
        //node_data.style.backgroundColor = Color.blue;
        //mainContainer.Add(node_data);

        //Label example = new Label("Custom Example");
        //node_data.Add(example);

        //mainContainer.Add(node_data);
    }
}

