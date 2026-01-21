using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(CharacterController))]
public class SimpleVRPlayerController : MonoBehaviour
{
    public enum MovementMode
    {
        Teleportation,
        SmoothLocomotion,
        ArmSwing
    }

    [Header("General Settings")]
    public MovementMode currentMovementMode = MovementMode.SmoothLocomotion;

    [Header("Smooth Locomotion Settings")]
    public float moveSpeed = 2.0f;
    public float gravity = -9.81f;

    [Header("Input Settings")]
    public InputActionProperty moveAction;

    [Header("Headset Settings")]
    public Transform headTransform;

    [Header("Teleportation Settings")]
    [Tooltip("Custom Teleportation System.")]
    public XRCustomTeleport customTeleportSystem;

    [Header("Arm Swing Movement Settings")]
    public InputActionProperty leftHandMovement;
    public InputActionProperty rightHandMovement;
    public float armSwingSpeed = 3.0f;

    private CharacterController characterController;
    private float verticalVelocity = 0f;

    private Vector3 lastLeftHandPos;
    private Vector3 lastRightHandPos;
    private float armSwingThreshold = 0.02f;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();

        if (headTransform == null && Camera.main != null)
            headTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        leftHandMovement.action.Enable();
        rightHandMovement.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        leftHandMovement.action.Disable();
        rightHandMovement.action.Disable();
    }

    private void Update()
    {
        switch (currentMovementMode)
        {
            case MovementMode.SmoothLocomotion:
                HandleSmoothLocomotion();
                break;
            case MovementMode.Teleportation:
                HandleTeleportation();
                break;
            case MovementMode.ArmSwing:
                HandleArmSwingMovement();
                break;
        }
    }

    private void HandleSmoothLocomotion()
    {
        Vector2 input = moveAction.action.ReadValue<Vector2>();

        Quaternion headRotation = Quaternion.Euler(0, headTransform.eulerAngles.y, 0);
        Vector3 moveDirection = headRotation * new Vector3(input.x, 0, input.y);

        if (characterController.isGrounded)
        {
            verticalVelocity = 0f;
        }
        verticalVelocity += gravity * Time.deltaTime;
        moveDirection.y = verticalVelocity;

        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
    }
    private void HandleTeleportation()
    {
        if (customTeleportSystem == null)
        {
            Debug.LogError("Custom Teleportation System is not assigned!");
            return;
        }

        characterController.enabled = false;
        customTeleportSystem.StartTeleport();
    }

    private void HandleArmSwingMovement()
    {
        Vector3 leftHandPos = leftHandMovement.action.ReadValue<Vector3>();
        Vector3 rightHandPos = rightHandMovement.action.ReadValue<Vector3>();

        Vector3 leftVelocity = (leftHandPos - lastLeftHandPos) / Time.deltaTime;
        Vector3 rightVelocity = (rightHandPos - lastRightHandPos) / Time.deltaTime;

        lastLeftHandPos = leftHandPos;
        lastRightHandPos = rightHandPos;

        if (leftVelocity.magnitude < armSwingThreshold && rightVelocity.magnitude < armSwingThreshold)
            return;

        Vector3 movementDirection = headTransform.forward;
        movementDirection.y = 0;
        movementDirection.Normalize();

        float speedFactor = (leftVelocity.magnitude + rightVelocity.magnitude) * 0.5f;
        Vector3 movement = movementDirection * speedFactor * armSwingSpeed * Time.deltaTime;

        characterController.Move(movement);
    }



    public void SetMovementMode(MovementMode mode)
    {
        currentMovementMode = mode;

        characterController.enabled = (mode != MovementMode.Teleportation);

        if (mode == MovementMode.Teleportation && customTeleportSystem != null)
        {
            customTeleportSystem.enabled = true;
        }
        else if (customTeleportSystem != null)
        {
            customTeleportSystem.enabled = false;
        }

        Debug.Log("Movement mode changed to: " + currentMovementMode);
    }
}
