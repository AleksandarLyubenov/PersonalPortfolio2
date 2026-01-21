using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Magazine : XRGrabInteractable
{
    public int maxAmmo = 30;
    public int currentAmmo;
    public Transform attachPoint;
    public GameObject round1;
    public GameObject round2;

    protected override void Awake()
    {
        base.Awake();
        currentAmmo = maxAmmo;

        if (attachPoint != null)
        {
            attachTransform = attachPoint;
        }

        UpdateRoundsVisibility();
    }

    public bool HasAmmo()
    {
        return currentAmmo > 0;
    }

    public int UseRound()
    {
        if (currentAmmo > 0)
        {
            currentAmmo--;
            UpdateRoundsVisibility();
            return 1;
        }
        return 0;
    }

    private void UpdateRoundsVisibility()
    {
        if (round1 != null)
        {
            round1.SetActive(currentAmmo > 1);
        }
        if (round2 != null)
        {
            round2.SetActive(currentAmmo > 0);
        }
    }
}