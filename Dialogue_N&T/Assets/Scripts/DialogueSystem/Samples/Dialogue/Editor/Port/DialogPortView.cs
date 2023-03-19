using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VisualGraphEditor;
using VisualGraphRuntime;

[CustomPortView(typeof(DialogPort))]
public class DialogPortView : VisualGraphPortView
{
    public override void CreateView(VisualGraphPort port)
    {
        //TestPort testPort = (TestPort)port;

        //EnumField field = new EnumField(TestPort.State1.None);
        //field.SetValueWithoutNotify(testPort.state);
        //field.RegisterValueChangedCallback<System.Enum>(evt =>
        //{
        //    testPort.state = (TestPort.State1)evt.newValue;
        //});
        //Add(field);
    }
}