using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private AudioSource hitSound;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            PlayHitSound();
        }
    }

    void PlayHitSound()
    {
        if (hitSound != null)
        {
            hitSound.Play();
        }
    }
}
