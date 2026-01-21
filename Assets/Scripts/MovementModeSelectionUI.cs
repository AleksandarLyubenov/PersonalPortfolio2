using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VRMovementUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button teleportButton;
    public Button smoothButton;
    public Button armSwingButton;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI prosText;
    public TextMeshProUGUI consText;
    public TextMeshProUGUI howToUseText;

    [Header("Player Controller")]
    public SimpleVRPlayerController playerController;

    private void Start()
    {
        teleportButton.onClick.AddListener(() => SetMovementMode(SimpleVRPlayerController.MovementMode.Teleportation));
        smoothButton.onClick.AddListener(() => SetMovementMode(SimpleVRPlayerController.MovementMode.SmoothLocomotion));
        armSwingButton.onClick.AddListener(() => SetMovementMode(SimpleVRPlayerController.MovementMode.ArmSwing));

        UpdateUI(playerController.currentMovementMode);
    }

    private void SetMovementMode(SimpleVRPlayerController.MovementMode mode)
    {
        playerController.SetMovementMode(mode);
        UpdateUI(mode);
    }

    private void UpdateUI(SimpleVRPlayerController.MovementMode mode)
    {
        teleportButton.interactable = true;
        smoothButton.interactable = true;
        armSwingButton.interactable = true;

        switch (mode)
        {
            case SimpleVRPlayerController.MovementMode.Teleportation:
                teleportButton.interactable = false;
                descriptionText.text = "Point and teleport instantly to a location.";
                prosText.text = "Advantages:\n• Reduces motion sickness.\n• Simple and intuitive.";
                consText.text = "Disadvantages:\n• Less immersive.\n• Can feel restrictive.";
                howToUseText.text = "How to Use:\n• Point with the controller and press the teleport button.";
                break;

            case SimpleVRPlayerController.MovementMode.SmoothLocomotion:
                smoothButton.interactable = false;
                descriptionText.text = "Move continuously with the joystick.";
                prosText.text = "Advantages:\n• Immersive.\n• Natural movement.";
                consText.text = "Disadvantages:\n• Can cause motion sickness.";
                howToUseText.text = "How to Use:\n• Use the joystick to move.";
                break;

            case SimpleVRPlayerController.MovementMode.ArmSwing:
                armSwingButton.interactable = false;
                descriptionText.text = "Move by swinging your arms.";
                prosText.text = "Advantages:\n• More physical.\n• Reduces motion sickness.";
                consText.text = "Disadvantages:\n• Can be tiring.";
                howToUseText.text = "How to Use:\n• Swing your arms to move forward.";
                break;
        }
    }
}
