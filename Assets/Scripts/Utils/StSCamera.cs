using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using Utils;

/// <summary>
/// A basic class that just ensures we only have one camera loaded at a time. 
/// </summary>
public class StsCamera : Singleton<StsCamera>
{
    [Serializable]
    public enum CameraStateName
    {
        Default,
        SelectedStagePosition,
    }
    
    [Serializable]
    public struct CameraState
    {
        public CameraStateName Name;
        [Tooltip("This can be left null, stage positions will automatically fill this.")]
        public Transform focusTarget; 
        public float fieldOfView;
        public float blendDuration;
    }

    [SerializeField] private List<CameraState> cameraStates;
    [SerializeField] private CameraState currentCameraState;
    [SerializeField] private CinemachineVirtualCamera vCam;
    private CinemachineTransposer vCamTransposer;
    
    private void OnEnable()
    {
        vCamTransposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
        StagePosition.OnStagePositionClicked += OnStagePositionClicked;
        Backdrop.OnBackgroundClicked += OnBackgroundClicked;
    }

    private void OnDisable()
    {
        StagePosition.OnStagePositionClicked -= OnStagePositionClicked;
        Backdrop.OnBackgroundClicked -= OnBackgroundClicked;
    }

    private void OnStagePositionClicked(StagePosition stagePosition)
    {
        ChangeCameraState(CameraStateName.SelectedStagePosition, stagePosition.GetViewTarget());
    }

    private void OnBackgroundClicked()
    {
        if (currentCameraState.Name == CameraStateName.SelectedStagePosition)
        {
            ChangeCameraState(CameraStateName.Default);
        }
    }

    private void ChangeCameraState(CameraStateName state, Transform transientFocus = null)
    {
        currentCameraState = cameraStates.Find(findState => findState.Name == state);

        if (transientFocus)
        {
            currentCameraState.focusTarget = transientFocus;
        }
        
        ChangeFocus(currentCameraState.focusTarget);
    }

    private void ChangeFocus(Transform newFocus)
    {
        vCam.Follow = newFocus;
    }
}
