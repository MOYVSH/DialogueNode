using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class SkinMaskBehaviour : PlayableBehaviour
{
    public float _hight;

    public override void OnPlayableCreate (Playable playable)
    {
        
    }
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
    }
}
