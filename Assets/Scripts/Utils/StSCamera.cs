using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
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
    [Header("Camera States")][Space(20)]
    [SerializeField] private List<CameraState> cameraStates;
    [SerializeField] private CameraState currentCameraState;
    [SerializeField] private CinemachineVirtualCamera vCam;
    private CinemachineFramingTransposer vCamTransposer;
    
    [Header("Camera Input")]
    [SerializeField] private bool IsMovingCamera = false;
    private PlayerInput input;
    private InputAction onPointerPress;
    private InputAction onPointerDelta;
     private Vector2 panDelta;
    [SerializeField] private Vector2 panMin;
    [SerializeField] private Vector2 panMax;
    [SerializeField] private float panSensitvity;
    [Header("The amount of time it takes for panning to reset to the target when changing focus.")]
    [SerializeField] private float panResetDuration = 1;
    
    private void OnEnable()
    {
        input = FindObjectOfType<PlayerInput>();
        
        // Bind to the pointer down event for when to pan
        onPointerPress = input.actions["PointerPress"];
        onPointerPress.performed += OnPointerDown;
        onPointerPress.canceled += OnPointerUp;
        
        // Bind to the delta event for how for panning control
        onPointerDelta = input.actions["PointerDelta"];
        onPointerDelta.performed += OnPointerDelta;
        onPointerDelta.canceled += OnPointerDelta;

        vCamTransposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        StagePosition.OnStagePositionClicked += OnStagePositionClicked;
        Backdrop.OnBackgroundClicked += OnBackgroundClicked;
    }

    private void OnDisable()
    {
        if (onPointerPress != null)
        {
            onPointerPress.performed -= OnPointerDown;
            onPointerPress.canceled -= OnPointerUp;
        }

        if (onPointerDelta != null)
        {
            onPointerDelta.performed -= OnPointerDelta;
            onPointerDelta.canceled -= OnPointerDelta;
        }
        
        StagePosition.OnStagePositionClicked -= OnStagePositionClicked;
        Backdrop.OnBackgroundClicked -= OnBackgroundClicked;
    }

    private void Update()
    {
        if (IsMovingCamera)
        {
            if (panDelta != Vector2.zero)
            {
                Vector3 currentPan = vCamTransposer.m_TrackedObjectOffset;
                
                // Calculate how much to move this frame.
                Vector3 panMovement = panSensitvity * Time.deltaTime * panDelta;
                
                // Invert
                panMovement *= -1;
                
                // Increment
                currentPan += panMovement;

                // Clamp to panMin, panMax
                currentPan = Vector3.Min(currentPan, panMax);
                currentPan = Vector3.Max(currentPan, panMin);

                // Set value.
                vCamTransposer.m_TrackedObjectOffset = currentPan;
            }
        }
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

    private void OnPointerDown(InputAction.CallbackContext context)
    {
        IsMovingCamera = true;
    }

    private void OnPointerUp(InputAction.CallbackContext context)
    {
        IsMovingCamera = false;
    }

    private void OnPointerDelta(InputAction.CallbackContext context)
    {
        panDelta = context.ReadValue<Vector2>().normalized;
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

    private IEnumerator ResetFollow()
    {
        Vector3 startFollow = vCamTransposer.m_TrackedObjectOffset;
        Vector3 endFollow = Vector3.zero;
        float t = 0;
        while (t < panResetDuration)
        {
            vCamTransposer.m_TrackedObjectOffset = Vector3.Lerp(startFollow, endFollow, t / panResetDuration);
            t += Time.deltaTime;
            yield return null;
        }

        vCamTransposer.m_TrackedObjectOffset = endFollow;
        
        yield return null;
    }

    private void ChangeFocus(Transform newFocus)
    {
        vCam.Follow = newFocus;
        vCam.LookAt = newFocus;
        StartCoroutine(ResetFollow());
    }
}
