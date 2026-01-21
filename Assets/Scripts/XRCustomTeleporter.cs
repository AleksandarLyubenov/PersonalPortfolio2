using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class XRCustomTeleport : MonoBehaviour
{
    [Header("Input Settings")]
    public InputActionProperty teleportAction;
    public InputActionProperty confirmAction;

    [Header("Teleportation Settings")]
    public Transform xrRig;
    public Transform leftController;
    public LayerMask teleportLayer;
    public float maxTeleportDistance = 10f;
    public float teleportFadeDuration = 0.2f;

    [Header("Visuals")]
    public LineRenderer teleportLine;
    public GameObject teleportMarkerPrefab;
    private GameObject teleportMarkerInstance;

    private bool isTeleporting = false;
    private Vector3 targetPosition;

    private void Start()
    {
        teleportAction.action.Enable();
        confirmAction.action.Enable();

        // Instantiate teleport marker
        teleportMarkerInstance = Instantiate(teleportMarkerPrefab);
        teleportMarkerInstance.SetActive(false);
    }

    private void Update()
    {
        if (teleportAction.action.IsPressed())
        {
            StartTeleport();
        }

        if (isTeleporting && confirmAction.action.WasReleasedThisFrame())
        {
            ConfirmTeleport();
        }
    }

    public void StartTeleport()
    {
        isTeleporting = true;
        teleportLine.enabled = true;
        teleportMarkerInstance.SetActive(true);
        UpdateTeleportRay();
    }
    private void UpdateTeleportRay()
    {
        RaycastHit hit;
        if (Physics.Raycast(leftController.position, leftController.forward, out hit, maxTeleportDistance, teleportLayer))
        {
            targetPosition = hit.point;
            teleportMarkerInstance.transform.position = targetPosition;
            teleportMarkerInstance.SetActive(true);
        }
        else
        {
            targetPosition = Vector3.zero;
            teleportMarkerInstance.SetActive(false);
        }

        DrawTeleportLine(targetPosition);
    }

    private void ConfirmTeleport()
    {
        if (targetPosition == Vector3.zero)
        {
            isTeleporting = false;
            return;
        }

        StartCoroutine(TeleportPlayer());
    }

    private IEnumerator TeleportPlayer()
    {
        yield return FadeScreen(true);

        float heightOffset = xrRig.transform.position.y;
        xrRig.position = new Vector3(targetPosition.x, targetPosition.y + heightOffset, targetPosition.z);

        yield return FadeScreen(false);

        isTeleporting = false;
        teleportLine.enabled = false;
        teleportMarkerInstance.SetActive(false);
    }
    private void DrawTeleportLine(Vector3 endPoint)
    {
        teleportLine.SetPosition(0, leftController.position);
        teleportLine.SetPosition(1, endPoint);
    }

    private IEnumerator FadeScreen(bool fadeOut)
    {
        float duration = teleportFadeDuration;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }
}
