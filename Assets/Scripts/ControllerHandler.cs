using UnityEngine;

public class ControllerHandler : MonoBehaviour
{
    public GameObject leftControllerPrefab;
    public GameObject rightControllerPrefab;
    public Transform leftHandAnchor;
    public Transform rightHandAnchor;

    private GameObject leftControllerInstance;
    private GameObject rightControllerInstance;

    private void Start()
    {
        if (leftControllerPrefab != null && leftHandAnchor != null)
        {
            leftControllerInstance = Instantiate(leftControllerPrefab, leftHandAnchor.position, leftHandAnchor.rotation, leftHandAnchor);
            leftControllerInstance.transform.localPosition = Vector3.zero;
            leftControllerInstance.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("Left controller prefab or left hand anchor is not assigned.");
        }

        if (rightControllerPrefab != null && rightHandAnchor != null)
        {
            rightControllerInstance = Instantiate(rightControllerPrefab, rightHandAnchor.position, rightHandAnchor.rotation, rightHandAnchor);
            rightControllerInstance.transform.localPosition = Vector3.zero;
            rightControllerInstance.transform.localRotation = Quaternion.identity;
        }
        else
        {
            Debug.LogWarning("Right controller prefab or right hand anchor is not assigned.");
        }
    }
}
