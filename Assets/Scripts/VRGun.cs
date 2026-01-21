using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunGrabInteractable : XRGrabInteractable
{
    [Header("Gun Settings")]
    public Transform gripAttachPoint;
    public Transform frontGripAttachPoint;
    public Rigidbody gunRigidbody;

    [Header("Interaction Settings")]
    public Collider rearGripCollider;
    public Collider frontGripCollider;

    [Header("Recoil Settings")]
    public float recoilForce = 0.05f;
    public float recoilRecoverySpeed = 5f;

    private XRBaseInteractor primaryInteractor = null;
    private XRBaseInteractor secondaryInteractor = null;

    private bool isTwoHanded => primaryInteractor != null && secondaryInteractor != null;

    protected override void Awake()
    {
        base.Awake();
        gunRigidbody = GetComponent<Rigidbody>();

        if (gripAttachPoint == null || frontGripAttachPoint == null)
        {
            Debug.LogError("Attach points are not assigned! Please assign them in the inspector.");
        }

        if (rearGripCollider == null || frontGripCollider == null)
        {
            Debug.LogError("Grip colliders are not assigned! Please assign them in the inspector.");
        }

        movementType = MovementType.Instantaneous;
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;
        if (interactor == null) return;

        if (primaryInteractor == null)
        {
            primaryInteractor = interactor;
            attachTransform = gripAttachPoint;
        }
        else if (secondaryInteractor == null && IsTouchingCollider(interactor, frontGripCollider))
        {
            secondaryInteractor = interactor;
        }

        base.OnSelectEntered(args);
    }



    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        XRBaseInteractor interactor = args.interactorObject as XRBaseInteractor;
        if (interactor == null) return;

        if (interactor == primaryInteractor)
        {
            primaryInteractor = null;
        }
        else if (interactor == secondaryInteractor)
        {
            secondaryInteractor = null;
        }

        base.OnSelectExited(args);
    }

    private void FixedUpdate()
    {
        if (isTwoHanded)
        {
            ApplyTwoHandedStabilization();
        }

        if (primaryInteractor != null)
        {
            transform.position = primaryInteractor.transform.position;
            transform.rotation = primaryInteractor.transform.rotation;
        }
        else if (secondaryInteractor != null)
        {
            transform.position = secondaryInteractor.transform.position;
            transform.rotation = secondaryInteractor.transform.rotation;
        }
    }


    private void ApplyTwoHandedStabilization()
    {
        if (primaryInteractor == null || secondaryInteractor == null) return;

        Vector3 handDirection = (secondaryInteractor.transform.position - primaryInteractor.transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(handDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    private bool IsTouchingCollider(XRBaseInteractor interactor, Collider gripCollider)
    {
        return gripCollider.bounds.Contains(interactor.transform.position);
    }

    public void ApplyRecoil()
    {
        if (gunRigidbody != null)
        {
            gunRigidbody.AddForce(-transform.forward * recoilForce, ForceMode.Impulse);
        }
    }
}