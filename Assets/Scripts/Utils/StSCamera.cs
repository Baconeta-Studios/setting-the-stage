using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
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
        public Vector3 focusOffset;
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
    private InputAction onPointerPosition;
    private InputAction onPointerDelta;
     private Vector2 panDelta;
    [SerializeField] private Vector3 panMin = new Vector3(-2f, -0.5f, 0f);
    [SerializeField] private Vector3 panMax = new Vector3(-2f, 0.5f, 0f);
    [SerializeField] private float panSensitvity;

    private ChapterUI chapterUi;
    private Act _act;


    private void OnEnable()
    {
        vCamTransposer = vCam.GetCinemachineComponent<CinemachineFramingTransposer>();
        _act = FindObjectOfType<Act>();
        if (_act)
        {
            _act.onChapterOpen += ChapterInitialize;
            _act.onChapterClosed += ChapterClearInitialize;
        }
    }

    private void ChapterInitialize()
    {
        // Subscribe to pointer events immediately, even if the game is not in a chapter.
        // They may get used later on, no harm in having the values whenever.
        input = FindObjectOfType<PlayerInput>();

        if (input)
        {
            // Bind to the pointer down event for when to pan
            onPointerPress = input.actions["PointerPress"];
            onPointerPress.performed += OnPointerDown;
            onPointerPress.canceled += OnPointerUp;
            
            // Bind to the delta event for how for panning control
            onPointerDelta = input.actions["PointerDelta"];
            onPointerDelta.performed += OnPointerDelta;
            onPointerDelta.canceled += OnPointerDelta;
        }

        // Bind to STATIC events, doesn't matter if there is an object or not.
        StageSelection.OnStageSelectionEnded += OnStageSelectionEnded;
    }

    private void ChapterClearInitialize()
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
        
        StageSelection.OnStageSelectionEnded += OnStageSelectionEnded;
    }

    private void OnDisable()
    {
        ChapterClearInitialize();
        
        if (_act)
        {
            _act.onChapterOpen -= ChapterInitialize;
            _act.onChapterClosed -= ChapterClearInitialize;
        }
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

                currentPan.z = vCamTransposer.m_TrackedObjectOffset.z;

                // Set value.
                vCamTransposer.m_TrackedObjectOffset = currentPan;
            }
        }
    }

    public void OnStagePositionClicked(StagePosition stagePosition)
    {
        // Zoom onto the stage selection
        ChangeCameraState(CameraStateName.SelectedStagePosition, stagePosition.GetViewTarget());
    }

    private void OnStageSelectionEnded()
    {
        // If we're zoomed in, then zoom out.
        if (currentCameraState.Name == CameraStateName.SelectedStagePosition)
        {
            ChangeCameraState(CameraStateName.Default);
        }
    }

    private void OnPointerDown(InputAction.CallbackContext context)
    {
        if (chapterUi is null)
        {
            chapterUi = FindObjectOfType<ChapterUI>();
        }

        if (chapterUi is null)
        {
            StSDebug.LogWarning("No chapter Ui found in camera system when checking for if the pointer is over the Ui.");
            return;
        }
        
        StartCoroutine(EPointerDown());
    }

    private IEnumerator EPointerDown()
    {
        // Wait until the end of the frame to ensure that the Pointer Position has been able to update.
        // Solves an issue where the pointer down would happen before the pointer position and there would be invalid coordinates.
        yield return new WaitForEndOfFrame();
        IsMovingCamera = !chapterUi.IsPointerOverUi();
        yield return null;
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
        
        vCam.Follow = currentCameraState.focusTarget;
        vCamTransposer.m_TrackedObjectOffset = currentCameraState.focusOffset;
    }
}
