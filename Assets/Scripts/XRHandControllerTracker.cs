using UnityEngine;
using UnityEngine.InputSystem;

public class XRHandControllerTracker : MonoBehaviour
{
    [Header("Tracking Input Actions")]
    [Tooltip("Input Action for the controller's position (expects a Vector3).")]
    public InputActionProperty positionAction;

    [Tooltip("Input Action for the controller's rotation (expects a Quaternion).")]
    public InputActionProperty rotationAction;

    private void OnEnable()
    {
        if (positionAction != null && positionAction.action != null)
            positionAction.action.Enable();
        if (rotationAction != null && rotationAction.action != null)
            rotationAction.action.Enable();
    }

    private void OnDisable()
    {
        if (positionAction != null && positionAction.action != null)
            positionAction.action.Disable();
        if (rotationAction != null && rotationAction.action != null)
            rotationAction.action.Disable();
    }

    private void Update()
    {
        if (positionAction != null && rotationAction != null)
        {
            Vector3 pos = positionAction.action.ReadValue<Vector3>();
            Quaternion rot = rotationAction.action.ReadValue<Quaternion>();
            transform.SetPositionAndRotation(pos, rot);
        }
    }
}
