using UnityEngine;
using UnityEngine.InputSystem;

public enum ViewMode
{
    Smooth,
    Snap,
    FOV
}

public class ViewController : MonoBehaviour
{
    [Header("General View Mode Settings")]
    [Tooltip("Current view mode: Smooth, Snap, or FOV.")]
    public ViewMode currentViewMode = ViewMode.Smooth;

    [Header("Common Input Settings")]
    [Tooltip("Input Action for view rotation (expects a Vector2, e.g., right thumbstick).")]
    public InputActionProperty rotationAction;

    [Header("Smooth View Settings")]
    [Tooltip("Rotation speed in degrees per second for smooth turning.")]
    public float smoothRotationSpeed = 90f;

    [Header("Snap View Settings")]
    [Tooltip("Angle (in degrees) to snap when turning.")]
    public float snapAngle = 45f;
    [Tooltip("Thumbstick x-axis threshold to trigger a snap turn (0..1).")]
    public float snapThreshold = 0.8f;
    private bool snapTurnReady = true;

    [Header("FOV View Settings")]
    [Tooltip("Transform of the headset (Main Camera) that represents the player's eyes.")]
    public Transform headTransform;
    public float fovThreshold = 30f;
    public float fovRecenterSpeed = 5f;

    #region Input Enabling
    private void OnEnable()
    {
        if (rotationAction != null && rotationAction.action != null)
            rotationAction.action.Enable();
    }

    private void OnDisable()
    {
        if (rotationAction != null && rotationAction.action != null)
            rotationAction.action.Disable();
    }
    #endregion

    private void Update()
    {
        switch (currentViewMode)
        {
            case ViewMode.Smooth:
                HandleSmoothView();
                break;
            case ViewMode.Snap:
                HandleSnapView();
                break;
            case ViewMode.FOV:
                HandleFOVView();
                break;
        }
    }
    private void HandleSmoothView()
    {
        Vector2 rotationInput = rotationAction.action.ReadValue<Vector2>();
        float horizontalRotation = rotationInput.x * smoothRotationSpeed * Time.deltaTime;
        transform.Rotate(0f, horizontalRotation, 0f);
    }
    private void HandleSnapView()
    {
        Vector2 rotationInput = rotationAction.action.ReadValue<Vector2>();

        if (Mathf.Abs(rotationInput.x) >= snapThreshold && snapTurnReady)
        {
            float direction = Mathf.Sign(rotationInput.x);
            transform.Rotate(0f, direction * snapAngle, 0f);
            snapTurnReady = false;
        }
        else if (Mathf.Abs(rotationInput.x) < snapThreshold)
        {
            snapTurnReady = true;
        }
    }
    private void HandleFOVView()
    {
        if (headTransform == null)
        {
            if (Camera.main != null)
                headTransform = Camera.main.transform;
            else
            {
                Debug.LogError("No headTransform assigned and no Main Camera found.");
                return;
            }
        }

        Vector3 rigForward = transform.forward;
        Vector3 headForward = headTransform.forward;
        rigForward.y = 0f;
        headForward.y = 0f;
        rigForward.Normalize();
        headForward.Normalize();

        float angleDifference = Vector3.SignedAngle(rigForward, headForward, Vector3.up);

        if (Mathf.Abs(angleDifference) > fovThreshold)
        {
            float recenterAmount = Mathf.Lerp(0f, angleDifference, Time.deltaTime * fovRecenterSpeed);
            transform.Rotate(0f, recenterAmount, 0f);
        }
    }

    #region Public Accessors
    /// <summary>
    /// Set the current view mode.
    /// </summary>
    public void SetViewMode(ViewMode mode)
    {
        currentViewMode = mode;
        Debug.Log("View mode changed to: " + currentViewMode);
    }

    /// <summary>
    /// Get the current view mode.
    /// </summary>
    public ViewMode GetViewMode()
    {
        return currentViewMode;
    }
    #endregion
}
