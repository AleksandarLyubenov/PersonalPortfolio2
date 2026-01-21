using UnityEngine;
using System.Collections;

public class ChamberMechanism : MonoBehaviour
{
    public Transform bolt;
    public Transform gunReference;
    public float minZ = -0.05f;
    public float maxZ = 0.0f;
    public float moveSpeed = 10f;
    public float returnSpeed = 15f;
    public bool isRacked = false;

    private bool isBeingHeld = false;
    private Vector3 initialLocalOffset;

    private void Start()
    {
        if (gunReference != null)
        {
            initialLocalOffset = gunReference.InverseTransformPoint(transform.position);
        }
    }

    private void FixedUpdate()
    {
        if (gunReference != null)
        {
            transform.position = gunReference.TransformPoint(initialLocalOffset);
            transform.rotation = gunReference.rotation;
        }
    }

    public void RackBolt()
    {
        if (!isBeingHeld)
            StartCoroutine(MoveBoltBack());
    }

    private IEnumerator MoveBoltBack()
    {
        isBeingHeld = true;
        while (bolt.localPosition.z > minZ)
        {
            bolt.localPosition = new Vector3(bolt.localPosition.x, bolt.localPosition.y,
                Mathf.MoveTowards(bolt.localPosition.z, minZ, Time.deltaTime * moveSpeed));
            yield return null;
        }
        isBeingHeld = false;
        isRacked = true;
    }

    public void ReleaseBolt()
    {
        StartCoroutine(MoveBoltForward());
    }

    private IEnumerator MoveBoltForward()
    {
        while (bolt.localPosition.z < maxZ)
        {
            bolt.localPosition = new Vector3(bolt.localPosition.x, bolt.localPosition.y,
                Mathf.MoveTowards(bolt.localPosition.z, maxZ, Time.deltaTime * returnSpeed));
            yield return null;
        }
        isRacked = false;
    }

    public void AutoRack()
    {
        if (!isBeingHeld)
            StartCoroutine(MoveBoltBack());
    }
}