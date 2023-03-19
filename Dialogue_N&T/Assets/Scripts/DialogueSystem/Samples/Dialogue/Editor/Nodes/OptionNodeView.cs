using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VisualGraphEditor;

[CustomNodeView((typeof(OptionNode)))]
public class OptionNodeView : VisualGraphNodeView
{
    [HideInInspector] public override bool ShowNodeProperties => true;

    public override void DrawNode()
    {
        base.DrawNode();
        titleContainer.style.backgroundColor = Color.green;
    }
}
