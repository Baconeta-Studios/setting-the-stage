using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageLighting : MonoBehaviour
{
    [SerializeField] private List<Light> stageLights;

    private void OnEnable()
    {
        
        StageSelection.OnStageSelectionStarted += ClearStageLights;
        StageSelection.OnStageSelectionEnded += ActivateStageLights;
    }

    private void OnDisable()
    {
        StageSelection.OnStageSelectionStarted -= ClearStageLights;
        StageSelection.OnStageSelectionEnded -= ActivateStageLights;
    }

    private void ActivateStageLights()
    {
        foreach (Light stageLight in stageLights)
        {
            SetLightStatus(stageLight, true);
        }
    }
    private void ClearStageLights()
    {
        foreach (Light stageLight in stageLights)
        {
            SetLightStatus(stageLight, false);
        }
    }

    private void SetLightStatus(Light stageLight, bool isEnabled)
    {
        stageLight.enabled = isEnabled;
    }

}
