using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoryData
{
    public ConditionConfig ConditionConfig;
    [TextArea(3,5)]
    public string Text;
}
