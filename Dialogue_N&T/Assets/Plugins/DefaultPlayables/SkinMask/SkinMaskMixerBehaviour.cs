using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class SkinMaskMixerBehaviour : PlayableBehaviour
{
    public float _hight;
    public float _defaultHight;
    private bool isFirst;
    private Renderer trackBinding;
    // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        trackBinding = playerData as Renderer;

        if (!trackBinding)
            return;

        float totalWeight = 0f;
        float greatestWeight = 0f;
        int currentInputs = 0;
        float height = 0;
        if (!isFirst)
        {
            _defaultHight = trackBinding.material.GetFloat("_Hight");
            isFirst = true;
        }

        int inputCount = playable.GetInputCount ();

        for (int i = 0; i < inputCount; i++)
        {
            float inputWeight = playable.GetInputWeight(i);
            ScriptPlayable<SkinMaskBehaviour> inputPlayable = (ScriptPlayable<SkinMaskBehaviour>)playable.GetInput(i);
            SkinMaskBehaviour input = inputPlayable.GetBehaviour ();
            totalWeight += inputWeight;
            // Use the above variables to process each frame of this playable.
            height = input._hight;
            if (!Mathf.Approximately(inputWeight, 0f))
                currentInputs++;
        }

        trackBinding.material.SetFloat("_Hight",height);

        if (currentInputs != 1 && 1f - totalWeight > greatestWeight)
        {
            trackBinding.material.SetFloat("_Hight",_defaultHight);
        }
    }

    public override void OnPlayableDestroy(Playable playable)
    {
        isFirst = false;

        if (trackBinding == null)
            return;
        trackBinding.material.SetFloat("_Hight",_defaultHight);
        
    }

}
