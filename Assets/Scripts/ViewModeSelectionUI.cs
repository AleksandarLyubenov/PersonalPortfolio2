using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ViewModeSelectionUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button smoothTurnButton;
    public Button snapTurnButton;
    public Button fovEdgeTurnButton;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI prosText;
    public TextMeshProUGUI consText;
    public TextMeshProUGUI howToUseText;

    [Header("Player Controller")]
    public ViewController playerViewController;

    private void Start()
    {
        smoothTurnButton.onClick.AddListener(() => SetViewMode(ViewMode.Smooth));
        snapTurnButton.onClick.AddListener(() => SetViewMode(ViewMode.Snap));
        fovEdgeTurnButton.onClick.AddListener(() => SetViewMode(ViewMode.FOV));

        UpdateUI(playerViewController.currentViewMode);
    }

    private void SetViewMode(ViewMode mode)
    {
        if (playerViewController == null)
        {
            Debug.LogError("Player ViewController is not assigned!");
            return;
        }

        playerViewController.SetViewMode(mode);
        UpdateUI(mode);
    }

    private void UpdateUI(ViewMode selectedMode)
    {
        smoothTurnButton.interactable = true;
        snapTurnButton.interactable = true;
        fovEdgeTurnButton.interactable = true;

        switch (selectedMode)
        {
            case ViewMode.Smooth:
                smoothTurnButton.interactable = false;
                descriptionText.text = "Smooth turning provides continuous camera rotation using the joystick.";
                prosText.text = "Advantages:\n• Provides continuous rotation.\n• More immersive experience.\n• Suitable for experienced VR users.";
                consText.text = "Disadvantages:\n• Can cause motion sickness for some users.\n• Might feel unnatural without acceleration smoothing.";
                howToUseText.text = "How to Use:\n• Use the right thumbstick to turn the camera.";
                break;

            case ViewMode.Snap:
                snapTurnButton.interactable = false;
                descriptionText.text = "Snap turning instantly rotates the player by a fixed angle when the joystick is pushed.";
                prosText.text = "Advantages:\n• Reduces motion sickness.\n• Provides instant, predictable turns.\n• Easier for new VR users.";
                consText.text = "Disadvantages:\n• Can feel unnatural compared to real-world turning.\n• Can be jarring for some players.";
                howToUseText.text = "How to Use:\n• Use the right thumbstick to turn the camera.";
                break;

            case ViewMode.FOV:
                fovEdgeTurnButton.interactable = false;
                descriptionText.text = "FOV Edge turning gradually rotates the player based on head orientation when they turn far enough.";
                prosText.text = "Advantages:\n• Reduces the chance of motion sickness.\n• Aligns player rotation with real-world head movement.\n• Feels more natural for some users.";
                consText.text = "Disadvantages:\n• Requires adaptation.\n• Can be frustrating if overly sensitive.\n• May cause unintended recentering.";
                howToUseText.text = "How to Use:\n• Turn your head far enough to the side and it will engage.";
                break;
        }
    }
}
