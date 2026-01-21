using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;

public class Gun : MonoBehaviour
{
    public enum FireMode { SemiAuto, Burst2, Burst3, FullAuto }

    [Header("XR Interaction")]
    public XRSocketInteractor magazineSocket;
    public XRGrabInteractable grabInteractable;

    [Header("Gun Mechanism")]
    public Transform chamberBolt;
    public float boltMinZ = -0.00492f;
    public float boltMaxZ = 0.0f;
    public float boltSpeed = 10f;
    public bool isChambered = false;

    [Header("Firing Components")]
    public Transform barrelEnd;
    public Transform chamberEffect;
    public GameObject bulletPrefab;
    public GameObject shellPrefab;
    public GameObject muzzleFlash;
    public GameObject chamberSmoke;
    public AudioSource gunshotAudio;
    public AudioSource magInAudio;
    public AudioSource magOutAudio;
    public AudioSource clickAudio;
    public AudioSource slideAudio;
    public AudioSource shellHitAudio;
    public float bulletSpeed = 100f;

    [Header("Fire Mode Settings")]
    public FireMode fireMode = FireMode.SemiAuto;
    public float roundsPerMinute = 600f;
    private bool canFire = true;
    private bool triggerHeld = false;
    private int burstCount = 0;

    private Magazine currentMagazine;
    private bool firstMagInserted = false;

    private void Start()
    {
        chamberBolt.localPosition = new Vector3(chamberBolt.localPosition.x, chamberBolt.localPosition.y, boltMinZ);
        magazineSocket.selectEntered.AddListener(OnMagazineInserted);
        magazineSocket.selectExited.AddListener(OnMagazineRemoved);

        if (grabInteractable != null)
        {
            grabInteractable.activated.AddListener(OnGunTriggerPressed);
            grabInteractable.deactivated.AddListener(OnGunTriggerReleased);
        }

        if (muzzleFlash != null) muzzleFlash.SetActive(false);
        if (chamberSmoke != null) chamberSmoke.SetActive(false);
    }

    private void OnDestroy()
    {
        magazineSocket.selectEntered.RemoveListener(OnMagazineInserted);
        magazineSocket.selectExited.RemoveListener(OnMagazineRemoved);

        if (grabInteractable != null)
        {
            grabInteractable.activated.RemoveListener(OnGunTriggerPressed);
            grabInteractable.deactivated.RemoveListener(OnGunTriggerReleased);
        }
    }

    private void OnGunTriggerPressed(ActivateEventArgs args)
    {
        triggerHeld = true;

        if (fireMode == FireMode.FullAuto)
        {
            StartCoroutine(AutoFire());
        }
        else
        {
            FireBurst();
        }
    }

    private void OnGunTriggerReleased(DeactivateEventArgs args)
    {
        triggerHeld = false;
        burstCount = 0;
    }

    private void OnMagazineInserted(SelectEnterEventArgs args)
    {
        if (args.interactableObject is Magazine mag)
        {
            currentMagazine = mag;
            isChambered = false;
            if (magInAudio != null) magInAudio.Play();

            if (!firstMagInserted)
            {
                firstMagInserted = true;
                MoveBoltForward();
            }
            else if (mag.HasAmmo())
            {
                MoveBoltForward();
            }
        }
    }

    private void OnMagazineRemoved(SelectExitEventArgs args)
    {
        currentMagazine = null;
        isChambered = false;
        StartCoroutine(MoveBolt(boltMinZ));
        if (magOutAudio != null) magOutAudio.Play();
    }

    private void FireBurst()
    {
        if (fireMode == FireMode.Burst2 && burstCount >= 2) return;
        if (fireMode == FireMode.Burst3 && burstCount >= 3) return;

        Fire();
        burstCount++;
    }

    private IEnumerator AutoFire()
    {
        while (triggerHeld && fireMode == FireMode.FullAuto)
        {
            Fire();
            yield return new WaitForSeconds(60f / roundsPerMinute);
        }
    }

    public void Fire()
    {
        if (!canFire) return;

        if (!isChambered)
        {
            if (clickAudio != null) clickAudio.Play();
            return;
        }

        if (barrelEnd == null)
        {
            return;
        }

        IXRSelectInteractable selectedInteractable = magazineSocket.GetOldestInteractableSelected();
        if (!(selectedInteractable is Magazine mag) || !mag.HasAmmo())
        {
            if (clickAudio != null) clickAudio.Play();
            StartCoroutine(MoveBolt(boltMinZ));
            return;
        }

        mag.UseRound();
        StartCoroutine(CycleBolt());

        if (gunshotAudio != null)
            gunshotAudio.Play();

        if (bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, barrelEnd.position, barrelEnd.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                bulletRb.velocity = Vector3.zero;
                bulletRb.angularVelocity = Vector3.zero;
                bulletRb.Sleep();

                Vector3 bulletDirection = barrelEnd.forward;
                bulletRb.isKinematic = false;
                bulletRb.useGravity = true;
                bulletRb.AddForce(bulletDirection * bulletSpeed, ForceMode.Impulse);
            }
        }

        StartCoroutine(FlashEffect());
        EjectShell();
    }

    private IEnumerator FlashEffect()
    {
        if (muzzleFlash != null) muzzleFlash.SetActive(true);
        if (chamberSmoke != null) chamberSmoke.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        if (muzzleFlash != null) muzzleFlash.SetActive(false);
        if (chamberSmoke != null) chamberSmoke.SetActive(false);
    }

    private void EjectShell()
    {
        if (shellPrefab == null || chamberEffect == null) return;

        GameObject shell = Instantiate(shellPrefab, chamberEffect.position, chamberEffect.rotation);
        Rigidbody shellRb = shell.GetComponent<Rigidbody>();

        if (shellRb != null)
        {
            shellRb.velocity = Vector3.zero;
            shellRb.angularVelocity = Vector3.zero;
            shellRb.Sleep();

            Vector3 ejectionForce = chamberEffect.right * -2f;
            shellRb.AddForce(ejectionForce, ForceMode.Impulse);
            shellRb.AddTorque(Random.insideUnitSphere * 1f, ForceMode.Impulse);
        }
    }

    private IEnumerator CycleBolt()
    {
        yield return MoveBolt(boltMinZ);
        yield return new WaitForSeconds(0.05f);
        yield return MoveBolt(boltMaxZ);
        isChambered = true;
    }

    private void MoveBoltForward()
    {
        StartCoroutine(MoveBolt(boltMaxZ));
        isChambered = true;
        if (slideAudio != null) slideAudio.Play();
    }

    private IEnumerator MoveBolt(float targetZ)
    {
        while (!Mathf.Approximately(chamberBolt.localPosition.z, targetZ))
        {
            chamberBolt.localPosition = new Vector3(
                chamberBolt.localPosition.x,
                chamberBolt.localPosition.y,
                Mathf.MoveTowards(chamberBolt.localPosition.z, targetZ, Time.deltaTime * boltSpeed)
            );
            yield return null;
        }
    }
}